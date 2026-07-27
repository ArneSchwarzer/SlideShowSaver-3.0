Imports System.IO
Imports System.Windows.Forms
Imports System.Windows.Input
Imports System.Windows.Interop
Imports System.Windows.Threading
Imports SlideShowLogging
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling
Imports SlideShowTools.XmlHandling
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowWPFModul.Mandelbrot.ModulMain

Public Class wpfModulMain

#Region "Variablendeklaration"

    Private Shared aktuelleSettings As ModulMain.ModulSettings_Mandelbrot

    'Timer und Zeitmanagement
    Public WithEvents tmrPraesentation As New DispatcherTimer()

    'Initialisierung
    Private hintergrundWM As Windows.Media.Color
    Private zielePfad As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SlideShowSaver 3.0\Module\Mandelbrot\MandelbrotZiele.xml"
            )
    Private gradientenPfad As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SlideShowSaver 3.0\Module\Mandelbrot\MandelbrotGradienten.xml"
            )

    'Gradienten
    Private gradienten As List(Of MandelbrotGradient)
    Private aktuellerGradient As MandelbrotGradient
    Private aktuellerGradientBrush As ImageBrush
    Private gradientOffset As Double = 0.0
    Private gradientGeschwindigkeit As Double = 0.05

    'Start- und Zielpunkte
    Private startpunkt As MandelbrotZiel
    Private aktuellesZiel As MandelbrotZiel
    Private aktuellePosition As MandelbrotZiel

    'Skalierung und Iteration
    Private maxIterationen As Integer
    Private Const startIterationen As Integer = 100
    Private aktuelleSkala As Double

    'Shader
    Private mandelbrotClassicEffect As MandelbrotEffect
    Private renderingAktiv As Boolean = False

    'Kamera
    Private Enum ZoomPhaseTyp
        FreezeIn
        Translation
        Cruise
        EaseOut
        FreezeOut
    End Enum

    Private Structure ZoomPhase
        Public Property Typ As ZoomPhaseTyp
        Public Property Dauer As TimeSpan
    End Structure

    Private aktuelleKamerafahrt As List(Of ZoomPhase)
    Private aktuellePhaseIndex As Integer
    Private phasenStartZeit As DateTime
    Private kamerafahrtStartZeit As DateTime

    Private easeOutStartSkala As Double

    'Rotation 
    Private Const minRotationsWinkel As Double = 33.0
    Private Const maxRotationsWinkel As Double = 180.0

    Private startRotation As Double
    Private zielRotation As Double
    Private aktuelleRotation As Double

    'Sonstiges
    Private rnd As New Random()

    'Finetuning/Diagnose
    Private Const aktuellesTarget As Integer = 55

    Public Structure MandelbrotZiel
        Public Property Name As String
        Public Property CenterX As Double
        Public Property CenterY As Double
        Public Property TargetScale As Double
    End Structure

#End Region

    Public Sub New()

        AddHandler ModulMain.YouHaveMail_Mandelbrot,
               AddressOf CheckYourMail

        InitializeComponent()

        Me.AddHandler(
        System.Windows.UIElement.PreviewMouseDownEvent,
        New System.Windows.Input.MouseButtonEventHandler(
            AddressOf Window_PreviewMouseDown),
        True)

        Me.AddHandler(
        System.Windows.UIElement.PreviewKeyDownEvent,
        New System.Windows.Input.KeyEventHandler(
            AddressOf Window_PreviewKeyDown),
        True)

        Me.WindowState = WindowState.Maximized

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        Dim helper As New WindowInteropHelper(Me)
        Dim hwnd As IntPtr = helper.Handle
        Dim currentScreen As Screen = Screen.FromHandle(hwnd)
        Dim maxWidth As Double = currentScreen.Bounds.Width * 0.9

        Me.Topmost = False
        Me.ShowActivated = True
        Me.Activate()

        hintergrundWM = SDColorToWMColor(HintergrundFarbeSaver)
        Me.Background = New SolidColorBrush(hintergrundWM)
        grdHauptbereich.Background = New SolidColorBrush(hintergrundWM)

        txbPraesentationsschirm.MaxWidth = maxWidth
        txbPraesentationsschirm.TextAlignment = TextAlignment.Center
        txbPraesentationsschirm.TextWrapping = TextWrapping.Wrap
        txbPraesentationsschirm.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))

        rctMandelbrot.Fill = New SolidColorBrush(hintergrundWM)

        CheckYourMail()

        gradienten = MandelbrotGradientRepository.LadeGradienten(gradientenPfad)

        InitialisiereStartpunkt()
        InitialisiereShader()

        PraesentationsschirmAnzeigen()

    End Sub

    Private Sub InitialisiereStartpunkt()

        startpunkt.Name = "Apfelmännchen"
        startpunkt.CenterX = -0.5
        startpunkt.CenterY = 0.0
        startpunkt.TargetScale = 3.0

    End Sub

    Private Sub InitialisiereShader()

        mandelbrotClassicEffect = New MandelbrotEffect()
        rctMandelbrot.Effect = mandelbrotClassicEffect

    End Sub

    Private Sub InitialisiereZielOverlay()

        Dim zielMaxIterationen As Integer
        Dim textfarbe As Windows.Media.Color

        If Not aktuelleSettings.KoordinatenAnzeigen Then
            ucZielOverlay.Verberge()
            Exit Sub
        End If

        zielMaxIterationen =
        BerechneMaxIterationen(aktuellesZiel.TargetScale)

        textfarbe =
        InvertWMColor(hintergrundWM)

        ucZielOverlay.InitialisiereZiel(aktuellesZiel.Name, aktuellesZiel.CenterX, aktuellesZiel.CenterY,
                                        aktuellesZiel.TargetScale, zielMaxIterationen, aktuelleSettings.Rotation,
                                        zielRotation, hintergrundWM, textfarbe)

        ucZielOverlay.ZeigeFreezeIn()

    End Sub

    Private Sub InitialisiereKamerafahrt()

        Dim freezeInDauer As TimeSpan
        Dim translationDauer As TimeSpan
        Dim cruiseDauer As TimeSpan
        Dim easeOutDauer As TimeSpan
        Dim freezeOutDauer As TimeSpan
        Dim gesamtZoomSekunden As Double
        Dim easeOutSekunden As Double
        Dim cruiseSekunden As Double

        freezeInDauer = TimeSpan.FromSeconds(2)
        translationDauer = TimeSpan.FromSeconds(4)
        freezeOutDauer = TimeSpan.FromSeconds(10)

        gesamtZoomSekunden = BerechneGesamtZoomdauerSekunden()

        'Die EaseOut-Phase darf höchstens fünf Sekunden dauern. Bei sehr kurzen
        'Zooms wird sie so verkürzt, dass kein negativer Cruise-Anteil entsteht.
        easeOutSekunden = Math.Min(5.0, gesamtZoomSekunden * 2.0)

        'Die aktuelle EaseOut-Formel entspricht am Ende der halben Dauer bei
        'voller Zoomgeschwindigkeit.
        cruiseSekunden = Math.Max(0.0, gesamtZoomSekunden - easeOutSekunden / 2.0)

        cruiseDauer = TimeSpan.FromSeconds(cruiseSekunden)
        easeOutDauer = TimeSpan.FromSeconds(easeOutSekunden)

        aktuelleKamerafahrt = New List(Of ZoomPhase) From {
        New ZoomPhase With {
            .Typ = ZoomPhaseTyp.FreezeIn,
            .Dauer = freezeInDauer
        },
        New ZoomPhase With {
            .Typ = ZoomPhaseTyp.Translation,
            .Dauer = translationDauer
        },
        New ZoomPhase With {
            .Typ = ZoomPhaseTyp.Cruise,
            .Dauer = cruiseDauer
        },
        New ZoomPhase With {
            .Typ = ZoomPhaseTyp.EaseOut,
            .Dauer = easeOutDauer
        },
        New ZoomPhase With {
            .Typ = ZoomPhaseTyp.FreezeOut,
            .Dauer = freezeOutDauer
        }
    }

    End Sub

    Private Sub PraesentationsschirmAnzeigen()

        Dim praesentationsText As String
        Dim praesentationsZeit As Integer

        praesentationsText = "Mandelbrot"
        praesentationsZeit = 5

        rctMandelbrot.Visibility = Visibility.Collapsed

        txbPraesentationsschirm.Text = praesentationsText
        txbPraesentationsschirm.Visibility = Visibility.Visible

        tmrPraesentation.Interval = TimeSpan.FromSeconds(praesentationsZeit)
        tmrPraesentation.Start()

        StopRendering()

    End Sub

    Private Sub tmrPresentation_Tick() Handles tmrPraesentation.Tick

        txbPraesentationsschirm.Visibility = Visibility.Collapsed
        tmrPraesentation.Stop()

        rctMandelbrot.Visibility = Visibility.Visible

        If StarteNeueKamerafahrt() Then
            StartRendering()
        Else
            LogHandling.LogWarn("Modul Mandelbrot: Rendering wurde nicht gestartet.")
        End If

    End Sub

    Private Function StarteNeueKamerafahrt() As Boolean

        If Not WaehleNeuesZiel() Then
            Return False
        End If

        InitialisiereKamerafahrt()
        WaehleNeuenGradienten()

        aktuellePosition = startpunkt
        aktuelleSkala = startpunkt.TargetScale
        maxIterationen = startIterationen

        rctMandelbrot.Effect = mandelbrotClassicEffect
        gradientOffset = 0.0

        startRotation = 0.0
        aktuelleRotation = startRotation

        If aktuelleSettings.Rotation Then
            zielRotation = WaehleZielRotation()
        Else
            zielRotation = 0.0
        End If

        aktuellePhaseIndex = 0
        phasenStartZeit = DateTime.Now
        kamerafahrtStartZeit = phasenStartZeit

        InitialisiereZielOverlay()

        Return True

    End Function

    Private Function WaehleZielRotation() As Double

        Dim richtung As Double
        Dim winkelGrad As Double

        If rnd.Next(2) = 0 Then
            richtung = -1.0
        Else
            richtung = 1.0
        End If

        winkelGrad = rnd.NextDouble() * (maxRotationsWinkel - minRotationsWinkel) + minRotationsWinkel

        Return richtung * winkelGrad * Math.PI / 180.0

    End Function

    Private Function WaehleNeuesZiel() As Boolean

        Dim count As Integer
        Dim node As XElement

        count = XMLDatensaetzeCount(zielePfad, "Target")

        If count <= 0 Then
            LogHandling.LogWarn("Modul Mandelbrot: Es konnten keine Mandelbrot-Ziele geladen werden.")
            Return False
        End If

        node = XMLDatensatzPerIndex(zielePfad, "Target", rnd.Next(count))

        'Für Finetuning der Mandelbrotziele
        'node = XMLDatensatzPerIndex(zielePfad, "Target", aktuellesTarget)

        If node Is Nothing Then
            LogHandling.LogWarn("Modul Mandelbrot: Das ausgewählte Mandelbrot-Ziel konnte nicht geladen werden.")
            Return False
        End If

        With node
            aktuellesZiel.Name = CStr(.Attribute("Name"))
            aktuellesZiel.CenterX = CDbl(.Element("CenterX"))
            aktuellesZiel.CenterY = CDbl(.Element("CenterY"))
            aktuellesZiel.TargetScale = CDbl(.Element("TargetScale"))
        End With

        If aktuellesZiel.TargetScale <= 0.0 OrElse aktuellesZiel.TargetScale >= startpunkt.TargetScale Then

            LogHandling.LogWarn("Modul Mandelbrot: Ziel '" & aktuellesZiel.Name & "' besitzt eine ungültige TargetScale: " &
                                aktuellesZiel.TargetScale.ToString("G17"))


            Return False

        End If

        If rnd.Next(2) = 0 Then
            aktuellesZiel.CenterY *= -1.0
        End If

        If aktuellesZiel.CenterY < 0 Then
            aktuellesZiel.Name &= " (Nord)"
        ElseIf aktuellesZiel.CenterY > 0 Then
            aktuellesZiel.Name &= " (Süd)"
        End If

        LogHandling.LogInfo("Modul Mandelbrot: Aktuelles Ziel: " & aktuellesZiel.Name)

        Return True

    End Function

    Private Sub WaehleNeuenGradienten()

        Dim erlaubteGradienten As List(Of MandelbrotGradient)

        If gradienten Is Nothing OrElse gradienten.Count = 0 Then
            Exit Sub
        End If

        If aktuelleSettings.Gradienten Is Nothing OrElse
       aktuelleSettings.Gradienten.Count = 0 Then

            Exit Sub
        End If

        erlaubteGradienten = gradienten.Where(
        Function(gradient) aktuelleSettings.Gradienten.Contains(gradient.Name)).ToList()

        If erlaubteGradienten.Count = 0 Then
            erlaubteGradienten = gradienten
        End If

        aktuellerGradient = erlaubteGradienten(rnd.Next(erlaubteGradienten.Count))
        aktuellerGradientBrush =
        MandelbrotGradientRepository.ErzeugeGradientBrush(aktuellerGradient, 1024)

        If mandelbrotClassicEffect IsNot Nothing Then
            mandelbrotClassicEffect.GradientTexture = aktuellerGradientBrush
        End If

        LogHandling.LogInfo(
        "Modul Mandelbrot: Aktueller Gradient: " & aktuellerGradient.Name)

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

        AktualisiereKlassischeShaderParameter()

    End Sub


#End Region

#Region "Update"

    Private Sub AktualisiereKamera()

        Dim aktuellePhase As ZoomPhase
        Dim elapsed As TimeSpan
        Dim phasenProgress As Double

        If aktuelleKamerafahrt Is Nothing OrElse
           aktuelleKamerafahrt.Count = 0 Then
            Exit Sub
        End If

        If aktuellePhaseIndex < 0 OrElse
           aktuellePhaseIndex >= aktuelleKamerafahrt.Count Then
            Exit Sub
        End If

        aktuellePhase = aktuelleKamerafahrt(aktuellePhaseIndex)
        elapsed = DateTime.Now - phasenStartZeit

        If aktuellePhase.Dauer.TotalMilliseconds <= 0 Then
            WechsleZurNaechstenPhase()
            Exit Sub
        End If

        phasenProgress = Math.Min(1.0, elapsed.TotalMilliseconds / aktuellePhase.Dauer.TotalMilliseconds)

        Select Case aktuellePhase.Typ

            Case ZoomPhaseTyp.FreezeIn
                AktualisiereFreezeIn()

            Case ZoomPhaseTyp.Translation
                AktualisiereTranslation(phasenProgress)

            Case ZoomPhaseTyp.Cruise
                AktualisiereCruise(elapsed.TotalSeconds, phasenProgress)

            Case ZoomPhaseTyp.EaseOut
                AktualisiereEaseOut(elapsed.TotalSeconds, aktuellePhase.Dauer.TotalSeconds)

            Case ZoomPhaseTyp.FreezeOut
                AktualisiereFreezeOut(phasenProgress)

        End Select

        If phasenProgress >= 1.0 Then
            WechsleZurNaechstenPhase()
        End If

    End Sub

    Private Sub AktualisiereFreezeIn()

        aktuellePosition.CenterX = startpunkt.CenterX
        aktuellePosition.CenterY = startpunkt.CenterY
        aktuellePosition.TargetScale = startpunkt.TargetScale

        aktuelleSkala = startpunkt.TargetScale
        maxIterationen = startIterationen
        aktuelleRotation = startRotation

        If ZielOverlayIstAktiv() Then
            ucZielOverlay.ZeigeFreezeIn()
        End If

    End Sub

    Private Sub AktualisiereTranslation(phasenProgress As Double)

        Dim translationT As Double

        translationT = EaseInOut(phasenProgress)

        aktuellePosition.CenterX =
        Lerp(
            startpunkt.CenterX,
            aktuellesZiel.CenterX,
            translationT)

        aktuellePosition.CenterY =
        Lerp(
            startpunkt.CenterY,
            aktuellesZiel.CenterY,
            translationT)

        aktuelleSkala = startpunkt.TargetScale
        maxIterationen = startIterationen

        aktuellePosition.TargetScale = aktuelleSkala

        'Während der Translation noch keine Rotation.
        aktuelleRotation = startRotation

        If ZielOverlayIstAktiv() Then
            ucZielOverlay.AktualisiereTranslation(phasenProgress)
        End If

    End Sub

    Private Sub AktualisiereCruise(elapsedSeconds As Double, phasenProgress As Double)

        Dim rotationT As Double

        aktuellePosition.CenterX = aktuellesZiel.CenterX
        aktuellePosition.CenterY = aktuellesZiel.CenterY

        aktuelleSkala = Math.Max(aktuellesZiel.TargetScale, BerechneAktuelleSkala(startpunkt.TargetScale, elapsedSeconds))

        maxIterationen = BerechneMaxIterationen(aktuelleSkala)

        aktuellePosition.TargetScale = aktuelleSkala

        rotationT = EaseInOut(phasenProgress)

        aktuelleRotation = Lerp(startRotation, zielRotation, rotationT)

        If ZielOverlayIstAktiv() Then

            ucZielOverlay.ZeigeInformationszeile()

            ucZielOverlay.AktualisiereWerte(aktuellePosition.CenterX, aktuellePosition.CenterY, aktuelleSkala, maxIterationen)

        End If

    End Sub

    Private Sub AktualisiereEaseOut(
    elapsedSeconds As Double,
    dauerSeconds As Double)

        Dim effektiveZoomSekunden As Double

        aktuellePosition.CenterX =
        aktuellesZiel.CenterX

        aktuellePosition.CenterY =
        aktuellesZiel.CenterY

        'Lineares Abbremsen der Zoomgeschwindigkeit.
        effektiveZoomSekunden =
        elapsedSeconds -
        ((elapsedSeconds * elapsedSeconds) /
         (2.0 * dauerSeconds))

        aktuelleSkala =
        Math.Max(
            aktuellesZiel.TargetScale,
            BerechneAktuelleSkala(
                easeOutStartSkala,
                effektiveZoomSekunden))

        maxIterationen =
        BerechneMaxIterationen(aktuelleSkala)

        aktuellePosition.TargetScale =
        aktuelleSkala

        'Der Zielwinkel wurde am Ende von Cruise vollständig erreicht.
        'Während EaseOut wird ausschließlich der Zoom abgebremst.
        aktuelleRotation =
        zielRotation

        If ZielOverlayIstAktiv() Then

            ucZielOverlay.ZeigeInformationszeile()

            ucZielOverlay.AktualisiereWerte(
            aktuellePosition.CenterX,
            aktuellePosition.CenterY,
            aktuelleSkala,
            maxIterationen)

        End If

    End Sub

    Private Sub AktualisiereFreezeOut(phasenProgress As Double)

        aktuellePosition.CenterX = aktuellesZiel.CenterX
        aktuellePosition.CenterY = aktuellesZiel.CenterY

        aktuelleSkala = aktuellesZiel.TargetScale
        maxIterationen = BerechneMaxIterationen(aktuelleSkala)

        aktuellePosition.TargetScale = aktuelleSkala

        If ZielOverlayIstAktiv() Then

            ucZielOverlay.AktualisiereWerte(
            aktuellePosition.CenterX,
            aktuellePosition.CenterY,
            aktuelleSkala,
            maxIterationen)

            ucZielOverlay.AktualisiereFreezeOut(
            phasenProgress)

        End If

    End Sub

    Private Sub WechsleZurNaechstenPhase()

        Dim neuePhase As ZoomPhaseTyp

        aktuellePhaseIndex += 1

        If aktuelleKamerafahrt Is Nothing OrElse
           aktuellePhaseIndex >= aktuelleKamerafahrt.Count Then

            If Not StarteNeueKamerafahrt() Then
                StopRendering()
            End If

            Exit Sub

        End If

        neuePhase = aktuelleKamerafahrt(aktuellePhaseIndex).Typ

        Select Case neuePhase

            Case ZoomPhaseTyp.EaseOut

                easeOutStartSkala = aktuelleSkala
                aktuelleRotation = zielRotation

        End Select

        phasenStartZeit = DateTime.Now

    End Sub

    Private Sub AktualisiereGradient()

        Dim elapsedSeconds As Double

        elapsedSeconds = (DateTime.Now - kamerafahrtStartZeit).TotalSeconds

        If aktuelleSettings.GradientAnimieren Then
            gradientOffset = (elapsedSeconds * gradientGeschwindigkeit) Mod 1.0
        Else
            gradientOffset = 0.0
        End If

    End Sub

#End Region

#Region "Render"

    Private Sub AktualisiereKlassischeShaderParameter()

        If mandelbrotClassicEffect Is Nothing Then Exit Sub

        Dim centerXHigh As Single, centerXLow As Single
        Dim centerYHigh As Single, centerYLow As Single
        Dim scaleHigh As Single, scaleLow As Single

        SplitDouble(aktuellePosition.CenterX, centerXHigh, centerXLow)
        SplitDouble(aktuellePosition.CenterY, centerYHigh, centerYLow)
        SplitDouble(aktuellePosition.TargetScale, scaleHigh, scaleLow)

        With mandelbrotClassicEffect
            .CenterXHigh = centerXHigh
            .CenterXLow = centerXLow
            .CenterYHigh = centerYHigh
            .CenterYLow = centerYLow
            .ScaleHigh = scaleHigh
            .ScaleLow = scaleLow

            .MaxIterations = CSng(maxIterationen)

            .ViewportWidth = CSng(Math.Max(1, rctMandelbrot.ActualWidth))
            .ViewportHeight = CSng(Math.Max(1, rctMandelbrot.ActualHeight))

            .Rotation = CSng(aktuelleRotation)

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

    Private Function BerechneZoomFaktorProSekunde() As Double

        Dim slider As Double
        Dim minFaktor As Double
        Dim maxFaktor As Double
        Dim t As Double

        slider = Math.Max(1.0, Math.Min(100.0, CDbl(aktuelleSettings.Zoomgeschwindigkeit)))

        minFaktor = 0.985
        maxFaktor = 0.7

        t = (slider - 1.0) / 99.0

        Return Lerp(minFaktor, maxFaktor, t)

    End Function

    Private Function BerechneGesamtZoomdauerSekunden() As Double

        Dim zoomFaktorProSekunde As Double
        Dim scaleVerhaeltnis As Double

        If aktuellesZiel.TargetScale <= 0.0 Then
            Return 0.0
        End If

        If aktuellesZiel.TargetScale >= startpunkt.TargetScale Then
            Return 0.0
        End If

        zoomFaktorProSekunde = BerechneZoomFaktorProSekunde()
        scaleVerhaeltnis = aktuellesZiel.TargetScale / startpunkt.TargetScale

        Return Math.Log(scaleVerhaeltnis) / Math.Log(zoomFaktorProSekunde)

    End Function

    Private Function BerechneAktuelleSkala(startScale As Double, elapsedSeconds As Double) As Double

        Dim zoomFaktorProSekunde As Double

        zoomFaktorProSekunde = BerechneZoomFaktorProSekunde()

        Return startScale * Math.Pow(zoomFaktorProSekunde, elapsedSeconds)

    End Function

    Private Function BerechneMaxIterationen(scale As Double) As Integer

        Dim startScale As Double
        Dim zoomTiefe As Double
        Dim basis As Integer
        Dim faktor As Double
        Dim iterations As Integer

        startScale = startpunkt.TargetScale
        basis = startIterationen
        faktor = 35.0

        If scale <= 0.0 Then
            Return startIterationen
        End If

        zoomTiefe = Math.Log(startScale / scale, 2.0)
        iterations = CInt(Math.Round(basis + zoomTiefe * faktor))

        Return Math.Max(startIterationen, Math.Min(iterations, 2000))

    End Function

    Private Sub SplitDouble(value As Double, ByRef high As Single, ByRef low As Single)

        high = CSng(value)
        low = CSng(value - CDbl(high))

    End Sub

    Private Function ZielOverlayIstAktiv() As Boolean

        If ucZielOverlay Is Nothing Then
            Return False
        End If

        Return aktuelleSettings.KoordinatenAnzeigen

    End Function

#End Region

#Region "Framework / Events"

    Private Sub Window_PreviewKeyDown(
    sender As Object,
    e As System.Windows.Input.KeyEventArgs)

        If e.Key = System.Windows.Input.Key.Escape Then
            e.Handled = True
        End If

        SlideShowTools.KeyAndMouseHandling.ForwardKeyDownWPF(
        sender,
        e)

    End Sub

    Private Sub Window_PreviewMouseDown(
    sender As Object,
    e As System.Windows.Input.MouseButtonEventArgs)

        SlideShowTools.KeyAndMouseHandling.ForwardMouseDownWPF(
        Me,
        e)

    End Sub

    Private Sub CheckYourMail()

        aktuelleSettings =
        GetSettings(Of ModulSettings_Mandelbrot)(nameModul)

        If ucZielOverlay Is Nothing Then
            Exit Sub
        End If

        If aktuelleSettings.KoordinatenAnzeigen Then

            If aktuellesZiel.Name IsNot Nothing AndAlso
           aktuellesZiel.Name <> String.Empty Then

                InitialisiereZielOverlay()

            End If

        Else

            ucZielOverlay.Verberge()

        End If

    End Sub

    Private Sub wpfModulMain_Closed(sender As Object,
                                e As EventArgs) Handles Me.Closed

        RemoveHandler ModulMain.YouHaveMail_Mandelbrot,
        AddressOf CheckYourMail

        StopRendering()

        If tmrPraesentation IsNot Nothing Then
            tmrPraesentation.Stop()
            tmrPraesentation = Nothing
        End If

        If rctMandelbrot IsNot Nothing Then
            rctMandelbrot.Effect = Nothing
        End If

        If mandelbrotClassicEffect IsNot Nothing Then
            mandelbrotClassicEffect.GradientTexture = Nothing
        End If

        mandelbrotClassicEffect = Nothing

        aktuellerGradientBrush = Nothing

        rctMandelbrot.Effect = Nothing
        rctMandelbrot.Fill = Nothing

        Me.Content = Nothing

    End Sub

#End Region

End Class