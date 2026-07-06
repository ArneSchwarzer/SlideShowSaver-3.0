Imports System.Windows.Forms
Imports System.Windows.Interop
Imports System.Windows.Media
Imports System.Windows.Threading
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

    'Start- und Zielpunkte
    Private startpunkt As MandelbrotZiel
    Private aktuellesZiel As MandelbrotZiel
    Private aktuellePosition As MandelbrotZiel

    'Shader
    Private mandelbrotEffect As MandelbrotEffect
    Private renderingAktiv As Boolean = False

    'Gradient / Palette
    Private gradientOffset As Double = 0.0
    Private gradientGeschwindigkeit As Double = 0.15

    'Sonstiges
    Private rnd As New Random()

    Public Structure MandelbrotZiel
        Public Property Name As String
        Public Property CenterX As Single
        Public Property CenterY As Single
        Public Property TargetScale As Single
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

        InitialisiereStartpunkt()
        InitialisiereShader()
        WaehleNeuesZiel()

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
        Dim präsentationsZeit As Integer = 15

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

        zoomDauer = TimeSpan.FromSeconds(60)
        zoomStartZeit = DateTime.Now

        rctMandelbrot.Visibility = Visibility.Visible

        tmrModul.Interval = zoomDauer
        tmrModul.Start()

        StartRendering()

    End Sub

    Private Sub TmrModul_Tick(sender As Object, e As EventArgs) Handles tmrModul.Tick

        WaehleNeuesZiel()

        zoomStartZeit = DateTime.Now
        tmrModul.Interval = zoomDauer
        tmrModul.Start()

    End Sub

    Private Sub WaehleNeuesZiel()

        Dim count As Integer = XMLDatensaetzeCount("MandelbrotZiele.XML", "Target")
        If count <= 0 Then
            LogHandling.LogInfo("Count Mandelbrot-Ziele fehlgeschlagen.")
            Exit Sub
        End If

        Dim node = XMLDatensatzPerIndex("MandelbrotZiele.XML", "Target", rnd.Next(count))
        If node Is Nothing Then Exit Sub

        With node
            aktuellesZiel.Name = CStr(.Attribute("Name"))
            aktuellesZiel.CenterX = CDbl(.Element("CenterX"))
            aktuellesZiel.CenterY = CDbl(.Element("CenterY"))
            aktuellesZiel.TargetScale = CDbl(.Element("TargetScale"))
            aktuellesZiel.MaxIterations = CInt(.Element("MaxIterations"))
        End With

        LogHandling.LogInfo("Modul Mandelbrot: Aktuelles Ziel: " & aktuellesZiel.Name)

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

        Dim elapsed As Double = (DateTime.Now - zoomStartZeit).TotalMilliseconds
        Dim progress As Double = Math.Min(1.0, elapsed / zoomDauer.TotalMilliseconds)

        ' Kamera-Zentrum erreicht das Ziel bereits nach 15 % der Zoomdauer.
        Dim centerProgress As Double = Math.Min(1.0, progress / 0.15)
        Dim centerT As Double = EaseOutCubic(centerProgress)

        ' Skalierung läuft über die volle Dauer.
        Dim scaleT As Double = EaseInOut(progress)

        aktuellePosition.CenterX = Lerp(startpunkt.CenterX, aktuellesZiel.CenterX, centerT)
        aktuellePosition.CenterY = Lerp(startpunkt.CenterY, aktuellesZiel.CenterY, centerT)

        aktuellePosition.TargetScale =
            startpunkt.TargetScale * Math.Pow(aktuellesZiel.TargetScale / startpunkt.TargetScale, scaleT)

        aktuellePosition.MaxIterations =
            CInt(Math.Round(Lerp(startpunkt.MaxIterations, aktuellesZiel.MaxIterations, scaleT)))

    End Sub

    Private Function EaseOutCubic(t As Double) As Double
        t = Math.Max(0.0, Math.Min(1.0, t))
        Return 1.0 - Math.Pow(1.0 - t, 3.0)
    End Function

    Private Sub AktualisiereGradient()

        ' V0.1: einfacher animierter Offset.
        ' Später kann hier die komplette Gradient-/Palette-Engine hängen.

        Dim elapsedSeconds As Double = (DateTime.Now - zoomStartZeit).TotalSeconds
        gradientOffset = (elapsedSeconds * gradientGeschwindigkeit) Mod 1.0

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
            .GradientIndex = 0.0F
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