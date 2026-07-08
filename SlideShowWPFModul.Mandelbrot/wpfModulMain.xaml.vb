Imports System.Windows.Forms
Imports System.Windows.Interop
Imports System.Windows.Media
Imports System.Windows.Threading
Imports System.IO
Imports SlideShowLogging
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling
Imports SlideShowTools.XmlHandling
Imports SlideShowWPFModul.Mandelbrot.ModulMain

Public Class wpfModulMain

#Region "Variablendeklaration"

    Private Shared aktuelleSettings As ModulMain.ModulSettings_Mandelbrot

    'Timer und Zeitmanagement
    Public WithEvents tmrModul As New DispatcherTimer()
    Public WithEvents tmrPresentation As New DispatcherTimer()
    Private zoomStartZeit As DateTime
    Private zoomDauer As TimeSpan

    'Initialisierung
    Private hintergrundWM As Windows.Media.Color
    Private renderSize As Windows.Size
    Private zielePfad As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SlideShowSaver 3.0\Module\Mandelbrot\MandelbrotZiele.xml"
            )
    Private gradientenPfad As String = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "SlideShowSaver 3.0\Module\Mandelbrot\MandelbrotGradienten.xml"
)

    Private gradienten As List(Of MandelbrotGradient)
    Private aktuellerGradient As MandelbrotGradient
    Private aktuellerGradientBrush As ImageBrush


    'Start- und Zielpunkte & Skalierung
    Private startpunkt As MandelbrotZiel
    Private aktuellesZiel As MandelbrotZiel
    Private aktuellePosition As MandelbrotZiel
    Private maxIterationen As Integer
    Private aktuelleSkala As Double

    'Shader
    Private mandelbrotEffect As MandelbrotEffect
    Private renderingAktiv As Boolean = False

    'Gradient / Palette
    Private gradientOffset As Double = 0.0
    Private gradientGeschwindigkeit As Double = 0.05
    Private gradientIndex As Integer

    'Sonstiges
    Private rnd As New Random()

    Public Structure MandelbrotZiel
        Public Property Name As String
        Public Property CenterX As Double
        Public Property CenterY As Double
        Public Property TargetScale As Double
        Public Property MaxIterations As Integer
    End Structure

#End Region

    Public Sub New()

        AddHandler ModulMain.YouHaveMail_Mandelbrot, AddressOf CheckYourMail

        InitializeComponent()

        Me.WindowState = WindowState.Maximized

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        Me.Topmost = False
        Me.ShowActivated = True
        Me.Activate()

        hintergrundWM = SDColorToWMColor(HintergrundFarbeSaver)
        Me.Background = New SolidColorBrush(hintergrundWM)

        Dim helper As New WindowInteropHelper(Me)
        Dim hwnd As IntPtr = helper.Handle
        Dim currentScreen As Screen = Screen.FromHandle(hwnd)

        Dim maxWidth As Double = currentScreen.Bounds.Width * 0.9
        renderSize.Width = currentScreen.Bounds.Width
        renderSize.Height = currentScreen.Bounds.Height

        txbPräsentationsschirm.MaxWidth = maxWidth
        txbPräsentationsschirm.TextAlignment = TextAlignment.Center
        txbPräsentationsschirm.TextWrapping = TextWrapping.Wrap
        txbPräsentationsschirm.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))

        rctMandelbrot.Fill = New SolidColorBrush(hintergrundWM)

        CheckYourMail()

        gradienten = MandelbrotGradientRepository.LadeGradienten(gradientenPfad)
        InitialisiereStartpunkt()
        InitialisiereShader()
        WaehleNeuesZiel()
        WaehleNeuenGradienten()

        PräsentationsschirmAnzeigen()

    End Sub

    Private Sub InitialisiereStartpunkt()

        startpunkt.Name = "Apfelmännchen"
        startpunkt.CenterX = -0.5
        startpunkt.CenterY = 0.0
        startpunkt.TargetScale = 3.0
        startpunkt.MaxIterations = 100

    End Sub

    Private Sub InitialisiereShader()

        mandelbrotEffect = New MandelbrotEffect()

        rctMandelbrot.Effect = mandelbrotEffect

    End Sub

    Private Sub PräsentationsschirmAnzeigen()

        Dim präsentationsText As String = "Mandelbrot"
        Dim präsentationsZeit As Integer = 5

        rctMandelbrot.Visibility = Visibility.Collapsed

        txbPräsentationsschirm.Text = präsentationsText
        txbPräsentationsschirm.Visibility = Visibility.Visible

        tmrPresentation.Interval = TimeSpan.FromSeconds(präsentationsZeit)
        tmrPresentation.Start()

        tmrModul.Stop()
        StopRendering()

    End Sub

    Private Sub tmrPresentation_Tick() Handles tmrPresentation.Tick

        txbPräsentationsschirm.Visibility = Visibility.Collapsed
        tmrPresentation.Stop()

        zoomDauer = TimeSpan.FromMinutes(Math.Max(1, aktuelleSettings.Zoomdauer))
        zoomStartZeit = DateTime.Now

        rctMandelbrot.Visibility = Visibility.Visible

        tmrModul.Interval = zoomDauer
        tmrModul.Start()

        StartRendering()

    End Sub

    Private Sub TmrModul_Tick(sender As Object, e As EventArgs) Handles tmrModul.Tick

        WaehleNeuesZiel()
        WaehleNeuenGradienten()

        zoomStartZeit = DateTime.Now
        tmrModul.Interval = zoomDauer
        tmrModul.Start()

    End Sub

    Private Sub WaehleNeuesZiel()

        Dim count As Integer = XMLDatensaetzeCount(zielePfad, "Target")
        If count <= 0 Then
            LogHandling.LogInfo("Count Mandelbrot-Ziele fehlgeschlagen.")
            Exit Sub
        End If

        Dim node = XMLDatensatzPerIndex(zielePfad, "Target", rnd.Next(count))
        If node Is Nothing Then Exit Sub

        With node
            aktuellesZiel.Name = CStr(.Attribute("Name"))
            aktuellesZiel.CenterX = CDbl(.Element("CenterX"))
            aktuellesZiel.CenterY = CDbl(.Element("CenterY"))
        End With

        If rnd.Next(2) = 0 Then
            aktuellesZiel.CenterY *= -1.0
            aktuellesZiel.Name &= " gespiegelt"
        End If

        LogHandling.LogInfo("Modul Mandelbrot: Aktuelles Ziel: " & aktuellesZiel.Name)

    End Sub

    Private Sub WaehleNeuenGradienten()

        If gradienten Is Nothing OrElse gradienten.Count = 0 Then Exit Sub
        If aktuelleSettings.Gradienten Is Nothing OrElse aktuelleSettings.Gradienten.Count = 0 Then Exit Sub

        Dim erlaubteGradienten = gradienten.
        Where(Function(g) aktuelleSettings.Gradienten.Contains(g.Name)).
        ToList()

        If erlaubteGradienten.Count = 0 Then
            erlaubteGradienten = gradienten
        End If

        aktuellerGradient = erlaubteGradienten(rnd.Next(erlaubteGradienten.Count))
        aktuellerGradientBrush = MandelbrotGradientRepository.ErzeugeGradientBrush(aktuellerGradient, 1024)

        If mandelbrotEffect IsNot Nothing Then
            mandelbrotEffect.GradientTexture = aktuellerGradientBrush
        End If

        LogHandling.LogInfo("Modul Mandelbrot: Aktueller Gradient: " & aktuellerGradient.Name)

    End Sub

#Region "Rendering-Ablauf"

    Private Sub StartRendering()

        If renderingAktiv Then Exit Sub

        renderingAktiv = True
        AddHandler CompositionTarget.Rendering, AddressOf OnRenderingFrame

    End Sub

    Private Sub StopRendering()

        If Not renderingAktiv Then Exit Sub

        RemoveHandler CompositionTarget.Rendering, AddressOf OnRenderingFrame
        renderingAktiv = False

    End Sub

    Private Sub OnRenderingFrame(sender As Object, e As EventArgs)

        UpdateFrame()
        RenderFrame()

    End Sub

    Private Sub UpdateFrame()

        AktualisiereKamera()
        AktualisiereGradient()

    End Sub

    Private Sub RenderFrame()

        AktualisiereShaderParameter()

    End Sub

#End Region

#Region "Update"

    Private Sub AktualisiereKamera()

        If zoomDauer.TotalMilliseconds <= 0 Then Exit Sub

        Dim elapsedSeconds As Double = (DateTime.Now - zoomStartZeit).TotalSeconds
        Dim progress As Double = Math.Min(1.0, elapsedSeconds / zoomDauer.TotalSeconds)

        ' Kamera-Zentrum erreicht das Ziel früh im Zoom.
        Dim centerProgress As Double = Math.Min(1.0, progress / 0.15)
        Dim centerT As Double = EaseOutCubic(centerProgress)

        aktuellePosition.CenterX = Lerp(startpunkt.CenterX, aktuellesZiel.CenterX, centerT)
        aktuellePosition.CenterY = Lerp(startpunkt.CenterY, aktuellesZiel.CenterY, centerT)

        aktuelleSkala = BerechneAktuelleSkala(elapsedSeconds)
        maxIterationen = BerechneMaxIterationen(aktuelleSkala)

        aktuellePosition.TargetScale = aktuelleSkala
        aktuellePosition.MaxIterations = maxIterationen

    End Sub

    Private Sub AktualisiereGradient()

        ' V0.1: einfacher animierter Offset.
        ' Später kann hier die komplette Gradient-/Palette-Engine hängen.

        Dim elapsedSeconds As Double = (DateTime.Now - zoomStartZeit).TotalSeconds

        If aktuelleSettings.GradientAnimieren Then
            gradientOffset = (elapsedSeconds * gradientGeschwindigkeit) Mod 1.0
        Else
            gradientOffset = 0
        End If

    End Sub

#End Region

#Region "Render"

    Private Sub AktualisiereShaderParameter()

        If mandelbrotEffect Is Nothing Then Exit Sub

        With mandelbrotEffect
            .CenterX = CSng(aktuellePosition.CenterX)
            .CenterY = CSng(aktuellePosition.CenterY)
            .Scale = CSng(aktuellePosition.TargetScale)
            .MaxIterations = aktuellePosition.MaxIterations

            .ViewportWidth = CSng(Math.Max(1, rctMandelbrot.ActualWidth))
            .ViewportHeight = CSng(Math.Max(1, rctMandelbrot.ActualHeight))

            .GradientOffset = CSng(gradientOffset)
        End With

    End Sub

#End Region

#Region "Hilfsfunktionen"

    Private Function Lerp(a As Double, b As Double, t As Double) As Double
        Return a + (b - a) * t
    End Function

    Private Function EaseInOut(t As Double) As Double
        Return t * t * (3.0 - 2.0 * t)
    End Function
    Private Function EaseOutCubic(t As Double) As Double
        t = Math.Max(0.0, Math.Min(1.0, t))
        Return 1.0 - Math.Pow(1.0 - t, 3.0)
    End Function

    Private Function BerechneAktuelleSkala(elapsedSeconds As Double) As Double

        Dim startScale As Double = startpunkt.TargetScale

        ' Trackbar 1..100:
        ' 1   = sehr langsamer Zoom
        ' 50  = normal
        ' 100 = sehr schneller Zoom
        Dim slider As Double = Math.Max(1.0, Math.Min(100.0, CDbl(aktuelleSettings.Zoomgeschwindigkeit)))

        ' Übersetzung in Zoomfaktor pro Sekunde.
        ' Werte kleiner 1.0 verkleinern die Scale.
        Dim minFaktor As Double = 0.985   ' langsam
        Dim maxFaktor As Double = 0.7     ' schnell

        Dim t As Double = (slider - 1.0) / 99.0
        Dim zoomFaktorProSekunde As Double = Lerp(minFaktor, maxFaktor, t)

        Return startScale * Math.Pow(zoomFaktorProSekunde, elapsedSeconds)

    End Function

    Private Function BerechneMaxIterationen(scale As Double) As Integer

        Dim startScale As Double = startpunkt.TargetScale

        If scale <= 0 Then Return 100

        Dim zoomTiefe As Double = Math.Log(startScale / scale, 2.0)

        ' V0.2-Formel:
        ' Basis + Iterationen pro Verdopplung der Zoomtiefe.
        Dim basis As Integer = 100
        Dim faktor As Double = 35.0

        Dim iterations As Integer = CInt(Math.Round(basis + zoomTiefe * faktor))

        Return Math.Max(100, Math.Min(iterations, 2000))

    End Function

#End Region

#Region "Framework / Events"

    Private Sub Window_KeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs)

        If e.Key = Key.Escape Then
            e.Handled = True
        End If

        SlideShowTools.KeyAndMouseHandling.ForwardKeyDownWPF(sender, e)

    End Sub

    Private Sub Window_MouseDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs)

        SlideShowTools.KeyAndMouseHandling.ForwardMouseDownWPF(Me, e)

    End Sub

    Private Sub CheckYourMail()

        aktuelleSettings = GetSettings(Of ModulSettings_Mandelbrot)(nameModul)

    End Sub

    Private Sub wpfModulMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed

        RemoveHandler ModulMain.YouHaveMail_Mandelbrot, AddressOf CheckYourMail

        StopRendering()

        If tmrModul IsNot Nothing Then
            tmrModul.Stop()
            tmrModul = Nothing
        End If

        If tmrPresentation IsNot Nothing Then
            tmrPresentation.Stop()
            tmrPresentation = Nothing
        End If

        If rctMandelbrot IsNot Nothing Then
            rctMandelbrot.Effect = Nothing
        End If

        mandelbrotEffect = Nothing

        Me.Content = Nothing

    End Sub

#End Region

End Class