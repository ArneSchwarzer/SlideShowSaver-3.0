Imports System.Windows.Forms
Imports System.Windows.Interop
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
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
    Public WithEvents tmrPraesentation As New DispatcherTimer()

    'Initialisierung
    Private hintergrundWM As Windows.Media.Color
    Private Shadows renderSize As Windows.Size
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
    Private aktuelleSkala As Double

    'Referenzorbit
    Private referenzOrbit As MandelbrotReferenzOrbit
    Private Const PerturbationSwitchScale As Double = 0.1
    Private referenzOrbitRealHighBrush As ImageBrush
    Private referenzOrbitImaginaryHighBrush As ImageBrush
    Private referenzOrbitRealLowBrush As ImageBrush
    Private referenzOrbitImaginaryLowBrush As ImageBrush

    'Diagnose
    Private Const BrushStreifenTestAktiv As Boolean = False

    'Multipass-Proof-of-Concept
    Private Const MultipassProofOfConceptAktiv As Boolean = True
    Private multipassPass1Bitmap As RenderTargetBitmap
    Private multipassPass2Bitmap As RenderTargetBitmap
    Private multipassPass1Brush As ImageBrush

    'Shader
    Private mandelbrotClassicEffect As MandelbrotEffect
    Private mandelbrotPerturbationEffect As MandelbrotPerturbationEffect
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

    'Render-Wechsel-Logik
    Private Enum MandelbrotRendererTyp
        Klassisch
        Perturbation
    End Enum

    Private aktuellerRenderer As MandelbrotRendererTyp

    'Rotation – zunächst nur architektonisch vorbereitet
    Private startRotation As Double
    Private zielRotation As Double
    Private aktuelleRotation As Double
    Private easeOutStartRotation As Double

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

        txbPraesentationsschirm.MaxWidth = maxWidth
        txbPraesentationsschirm.TextAlignment = TextAlignment.Center
        txbPraesentationsschirm.TextWrapping = TextWrapping.Wrap
        txbPraesentationsschirm.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))

        rctMandelbrot.Fill = New SolidColorBrush(hintergrundWM)

        CheckYourMail()

        gradienten = MandelbrotGradientRepository.LadeGradienten(gradientenPfad)

        InitialisiereStartpunkt()

        If MultipassProofOfConceptAktiv Then

            txbPraesentationsschirm.Visibility =
        Visibility.Collapsed

            rctMandelbrot.Visibility =
        Visibility.Visible

            StopRendering()

            Dispatcher.BeginInvoke(
        DispatcherPriority.Render,
        New Action(
            AddressOf StarteMultipassProofOfConcept))

            Return

        End If

        InitialisiereShader()

        PraesentationsschirmAnzeigen()

    End Sub

    Private Sub InitialisiereStartpunkt()

        startpunkt.Name = "Apfelmännchen"
        startpunkt.CenterX = -0.5
        startpunkt.CenterY = 0.0
        startpunkt.TargetScale = 3.0
        startpunkt.MaxIterations = 100

    End Sub

    Private Sub InitialisiereShader()

        mandelbrotClassicEffect =
        New MandelbrotEffect()

        mandelbrotPerturbationEffect =
        New MandelbrotPerturbationEffect()

        rctMandelbrot.Effect =
        mandelbrotClassicEffect

        aktuellerRenderer =
        MandelbrotRendererTyp.Klassisch

    End Sub

    Private Sub InitialisiereKamerafahrt()

        Dim freezeInDauer As TimeSpan
        Dim translationDauer As TimeSpan
        Dim cruiseDauer As TimeSpan
        Dim easeOutDauer As TimeSpan
        Dim freezeOutDauer As TimeSpan

        freezeInDauer = TimeSpan.FromSeconds(2)
        translationDauer = TimeSpan.FromSeconds(4)
        cruiseDauer = TimeSpan.FromMinutes(Math.Max(1, aktuelleSettings.Zoomdauer))
        easeOutDauer = TimeSpan.FromSeconds(5)
        freezeOutDauer = TimeSpan.FromSeconds(10)

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
            LogHandling.LogWarn(
            "Modul Mandelbrot: Rendering wurde nicht gestartet.")
        End If

    End Sub

    Private Function StarteNeueKamerafahrt() As Boolean

        InitialisiereKamerafahrt()

        If Not WaehleGueltigesDeepZoomZiel() Then

            LogHandling.LogWarn(
            "Modul Mandelbrot: Kamerafahrt konnte " &
            "nicht gestartet werden.")

            Return False

        End If

        WaehleNeuenGradienten()

        aktuellePosition = startpunkt
        aktuelleSkala = startpunkt.TargetScale
        maxIterationen = startpunkt.MaxIterations

        aktuellerRenderer = MandelbrotRendererTyp.Klassisch
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

        winkelGrad = rnd.NextDouble() * 20.0 + 5.0

        Return richtung * winkelGrad * Math.PI / 180.0

    End Function

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

    Private Function WaehleGueltigesDeepZoomZiel() As Boolean

        Dim versuch As Integer
        Dim maxVersuche As Integer

        maxVersuche = 30

        For versuch = 1 To maxVersuche

            WaehleNeuesZiel()

            If ErzeugeReferenzOrbitFuerAktuellesZiel() Then

                LogHandling.LogInfo(
                "Modul Mandelbrot: Deepzoom-Ziel nach " &
                versuch.ToString() &
                " Versuch(en) gefunden.")

                Return True

            End If

        Next

        referenzOrbit = Nothing

        LogHandling.LogWarn(
        "Modul Mandelbrot: Nach " &
        maxVersuche.ToString() &
        " Versuchen wurde kein geeignetes " &
        "Deepzoom-Ziel gefunden.")

        Return False

    End Function

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

        If mandelbrotClassicEffect IsNot Nothing Then
            mandelbrotClassicEffect.GradientTexture = aktuellerGradientBrush
        End If

        If mandelbrotPerturbationEffect IsNot Nothing Then
            mandelbrotPerturbationEffect.GradientTexture = aktuellerGradientBrush
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

        Select Case aktuellerRenderer

            Case MandelbrotRendererTyp.Klassisch
                AktualisiereKlassischeShaderParameter()

            Case MandelbrotRendererTyp.Perturbation
                AktualisierePerturbationShaderParameter()

        End Select

    End Sub

    Private Sub AktivierePerturbationRenderer()

        If mandelbrotPerturbationEffect Is Nothing Then
            Exit Sub
        End If

        If referenzOrbit Is Nothing Then
            Exit Sub
        End If

        If referenzOrbitRealHighBrush Is Nothing OrElse
            referenzOrbitImaginaryHighBrush Is Nothing OrElse
            referenzOrbitRealLowBrush Is Nothing OrElse
            referenzOrbitImaginaryLowBrush Is Nothing Then

            Exit Sub

        End If

        With mandelbrotPerturbationEffect

            .GradientTexture = aktuellerGradientBrush

            .ReferenceOrbitRealHighTexture = referenzOrbitRealHighBrush
            .ReferenceOrbitImaginaryHighTexture = referenzOrbitImaginaryHighBrush
            .ReferenceOrbitRealLowTexture = referenzOrbitRealLowBrush
            .ReferenceOrbitImaginaryLowTexture = referenzOrbitImaginaryLowBrush

            .OrbitLength = CSng(referenzOrbit.Count)
            .OrbitTextureWidth = CSng(referenzOrbit.TexturBreite)
            .OrbitTextureHeight = CSng(referenzOrbit.TexturHoehe)

        End With

        rctMandelbrot.Effect = mandelbrotPerturbationEffect
        aktuellerRenderer = MandelbrotRendererTyp.Perturbation

        LogHandling.LogInfo("Modul Mandelbrot: Umschaltung auf Perturbation bei Scale " & aktuelleSkala.ToString("E6", Globalization.CultureInfo.InvariantCulture))

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

        phasenProgress = Math.Min(
        1.0,
        elapsed.TotalMilliseconds /
        aktuellePhase.Dauer.TotalMilliseconds)

        Select Case aktuellePhase.Typ

            Case ZoomPhaseTyp.FreezeIn
                AktualisiereFreezeIn()

            Case ZoomPhaseTyp.Translation
                AktualisiereTranslation(phasenProgress)

            Case ZoomPhaseTyp.Cruise
                AktualisiereCruise(
                elapsed.TotalSeconds,
                phasenProgress)

            Case ZoomPhaseTyp.EaseOut
                AktualisiereEaseOut(
                elapsed.TotalSeconds,
                aktuellePhase.Dauer.TotalSeconds,
                phasenProgress)

            Case ZoomPhaseTyp.FreezeOut
                AktualisiereFreezeOut()

        End Select

        If phasenProgress >= 1.0 Then
            WechsleZurNaechstenPhase()
        End If

    End Sub

    Private Sub AktualisiereFreezeIn()

        aktuellePosition.CenterX = startpunkt.CenterX
        aktuellePosition.CenterY = startpunkt.CenterY
        aktuellePosition.TargetScale = startpunkt.TargetScale
        aktuellePosition.MaxIterations = startpunkt.MaxIterations

        aktuelleSkala = startpunkt.TargetScale
        maxIterationen = startpunkt.MaxIterations
        aktuelleRotation = startRotation

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
        maxIterationen = startpunkt.MaxIterations

        aktuellePosition.TargetScale = aktuelleSkala
        aktuellePosition.MaxIterations = maxIterationen

        'Während der Translation noch keine Rotation.
        aktuelleRotation = startRotation

    End Sub

    Private Sub AktualisiereCruise(elapsedSeconds As Double,
                               phasenProgress As Double)

        Dim rotationT As Double

        aktuellePosition.CenterX = aktuellesZiel.CenterX
        aktuellePosition.CenterY = aktuellesZiel.CenterY

        aktuelleSkala =
        BerechneAktuelleSkala(
            startpunkt.TargetScale,
            elapsedSeconds)

        maxIterationen =
        BerechneMaxIterationen(aktuelleSkala)

        aktuellePosition.TargetScale = aktuelleSkala
        aktuellePosition.MaxIterations = maxIterationen

        'Rotation bereits berechnen, aber noch nicht an den Shader übergeben.
        rotationT = EaseInOut(phasenProgress)

        aktuelleRotation =
        Lerp(
            startRotation,
            zielRotation,
            rotationT)

        AktualisiereRendererAuswahl()

    End Sub

    Private Sub AktualisiereEaseOut(elapsedSeconds As Double,
                                dauerSeconds As Double,
                                phasenProgress As Double)

        Dim effektiveZoomSekunden As Double
        Dim rotationRest As Double
        Dim rotationT As Double

        aktuellePosition.CenterX = aktuellesZiel.CenterX
        aktuellePosition.CenterY = aktuellesZiel.CenterY

        'Lineares Abbremsen der Zoomgeschwindigkeit.
        effektiveZoomSekunden =
        elapsedSeconds -
        ((elapsedSeconds * elapsedSeconds) /
         (2.0 * dauerSeconds))

        aktuelleSkala =
        BerechneAktuelleSkala(
            easeOutStartSkala,
            effektiveZoomSekunden)

        maxIterationen =
        BerechneMaxIterationen(aktuelleSkala)

        aktuellePosition.TargetScale = aktuelleSkala
        aktuellePosition.MaxIterations = maxIterationen

        'Kleine Fortsetzung der Rotation während des Abbremsens.
        rotationRest =
        (zielRotation - startRotation) * 0.05

        rotationT = EaseOutCubic(phasenProgress)

        aktuelleRotation = Lerp(
            easeOutStartRotation,
            easeOutStartRotation + rotationRest,
            rotationT)

        AktualisiereRendererAuswahl()

    End Sub

    Private Sub AktualisiereFreezeOut()

        aktuellePosition.CenterX = aktuellesZiel.CenterX
        aktuellePosition.CenterY = aktuellesZiel.CenterY

        aktuellePosition.TargetScale = aktuelleSkala
        aktuellePosition.MaxIterations = maxIterationen

    End Sub

    Private Sub AktualisiereRendererAuswahl()

        If aktuellerRenderer = MandelbrotRendererTyp.Perturbation Then

            Exit Sub

        End If

        If referenzOrbit Is Nothing Then Exit Sub

        If referenzOrbitRealHighBrush Is Nothing OrElse
            referenzOrbitImaginaryHighBrush Is Nothing OrElse
            referenzOrbitRealLowBrush Is Nothing OrElse
            referenzOrbitImaginaryLowBrush Is Nothing Then

            Exit Sub

        End If

        If aktuelleSkala <= PerturbationSwitchScale Then
            AktivierePerturbationRenderer()
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

            Case ZoomPhaseTyp.Cruise

                If referenzOrbit Is Nothing Then

                    LogHandling.LogWarn("Modul Mandelbrot: Beim Eintritt in Cruise ist kein gültiger Referenzorbit vorhanden.")

                    If Not StarteNeueKamerafahrt() Then
                        StopRendering()
                    End If

                    Return

                End If

            Case ZoomPhaseTyp.EaseOut

                easeOutStartSkala = aktuelleSkala
                easeOutStartRotation = aktuelleRotation

        End Select

        phasenStartZeit = DateTime.Now

    End Sub

    Private Function ErzeugeReferenzOrbitFuerAktuellesZiel() As Boolean

        Dim benoetigteIterationen As Integer

        referenzOrbit = Nothing
        referenzOrbitRealHighBrush = Nothing
        referenzOrbitImaginaryHighBrush = Nothing
        referenzOrbitRealLowBrush = Nothing
        referenzOrbitImaginaryLowBrush = Nothing

        benoetigteIterationen = BerechneBenoetigteReferenzIterationen()

        referenzOrbit = New MandelbrotReferenzOrbit(
            aktuellesZiel.CenterX,
            aktuellesZiel.CenterY,
            benoetigteIterationen)

        If referenzOrbit.Count < benoetigteIterationen Then

            LogHandling.LogWarn("Modul Mandelbrot: Ziel """ & aktuellesZiel.Name & """ ist als Referenzpunkt ungeeignet. " &
            "Orbit endete nach " & referenzOrbit.Count.ToString() & " von benötigten " & benoetigteIterationen.ToString() & " Iterationen.")

            referenzOrbit = Nothing
            referenzOrbitRealHighBrush = Nothing
            referenzOrbitImaginaryHighBrush = Nothing
            referenzOrbitRealLowBrush = Nothing
            referenzOrbitImaginaryLowBrush = Nothing

            If mandelbrotPerturbationEffect IsNot Nothing Then

                mandelbrotPerturbationEffect.ReferenceOrbitRealHighTexture = Nothing
                mandelbrotPerturbationEffect.ReferenceOrbitImaginaryHighTexture = Nothing
                mandelbrotPerturbationEffect.ReferenceOrbitRealLowTexture = Nothing
                mandelbrotPerturbationEffect.ReferenceOrbitImaginaryLowTexture = Nothing

            End If

            Return False

        End If

        LogHandling.LogInfo("Modul Mandelbrot: Referenzorbit für """ & aktuellesZiel.Name & """ mit " &
                            referenzOrbit.Count.ToString() & " Werten erfolgreich berechnet.")

        referenzOrbit.InitialisiereTexturlayout(Math.Max(1, CInt(Math.Floor(rctMandelbrot.ActualWidth))))

        referenzOrbitRealHighBrush = referenzOrbit.ErzeugeRealHighOrbitBrush()
        referenzOrbitImaginaryHighBrush = referenzOrbit.ErzeugeImaginaryHighOrbitBrush()
        referenzOrbitRealLowBrush = referenzOrbit.ErzeugeRealLowOrbitBrush()
        referenzOrbitImaginaryLowBrush = referenzOrbit.ErzeugeImaginaryLowOrbitBrush()

        With mandelbrotPerturbationEffect

            .ReferenceOrbitRealHighTexture = referenzOrbitRealHighBrush
            .ReferenceOrbitImaginaryHighTexture = referenzOrbitImaginaryHighBrush
            .ReferenceOrbitRealLowTexture = referenzOrbitRealLowBrush
            .ReferenceOrbitImaginaryLowTexture = referenzOrbitImaginaryLowBrush

            .OrbitLength = CSng(referenzOrbit.Count)
            .OrbitTextureWidth = CSng(referenzOrbit.TexturBreite)
            .OrbitTextureHeight = CSng(referenzOrbit.TexturHoehe)

        End With

        LogHandling.LogInfo("Modul Mandelbrot: Orbittextur " & referenzOrbit.TexturBreite.ToString() &
                            " × " & referenzOrbit.TexturHoehe.ToString() & " DIPs.")

        Return True

    End Function

    Private Sub AktualisiereGradient()

        Dim elapsedSeconds As Double

        elapsedSeconds =
        (DateTime.Now - kamerafahrtStartZeit).TotalSeconds

        If aktuelleSettings.GradientAnimieren Then
            gradientOffset =
            (elapsedSeconds * gradientGeschwindigkeit) Mod 1.0
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

            .MaxIterations = CSng(aktuellePosition.MaxIterations)

            .ViewportWidth = CSng(Math.Max(1, rctMandelbrot.ActualWidth))
            .ViewportHeight = CSng(Math.Max(1, rctMandelbrot.ActualHeight))

            .GradientOffset = CSng(gradientOffset)
        End With

    End Sub

    Private Sub AktualisierePerturbationShaderParameter()

        Dim scaleHigh As Single
        Dim scaleLow As Single
        Dim verwendeteIterationen As Integer

        If mandelbrotPerturbationEffect Is Nothing Then
            Exit Sub
        End If

        If referenzOrbit Is Nothing Then
            Exit Sub
        End If

        SplitDouble(aktuellePosition.TargetScale, scaleHigh, scaleLow)

        verwendeteIterationen = Math.Min(aktuellePosition.MaxIterations, referenzOrbit.Count)

        With mandelbrotPerturbationEffect

            .ScaleHigh = scaleHigh
            .ScaleLow = scaleLow
            .MaxIterations = CSng(verwendeteIterationen)
            .OrbitLength = CSng(referenzOrbit.Count)
            .OrbitTextureWidth = CSng(referenzOrbit.TexturBreite)
            .OrbitTextureHeight = CSng(referenzOrbit.TexturHoehe)
            .ViewportWidth = CSng(Math.Max(1.0, rctMandelbrot.ActualWidth))
            .ViewportHeight = CSng(Math.Max(1.0, rctMandelbrot.ActualHeight))
            .GradientOffset = CSng(gradientOffset)
            .Rotation = CSng(aktuelleRotation)

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

    Private Function BerechneAktuelleSkala(startScale As Double,
                                       elapsedSeconds As Double) As Double

        Dim slider As Double
        Dim minFaktor As Double
        Dim maxFaktor As Double
        Dim t As Double
        Dim zoomFaktorProSekunde As Double

        slider = Math.Max(
        1.0,
        Math.Min(100.0, CDbl(aktuelleSettings.Zoomgeschwindigkeit)))

        minFaktor = 0.985
        maxFaktor = 0.7

        t = (slider - 1.0) / 99.0

        zoomFaktorProSekunde =
        Lerp(minFaktor, maxFaktor, t)

        Return startScale *
           Math.Pow(zoomFaktorProSekunde, elapsedSeconds)

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

    Private Function BerechneBenoetigteReferenzIterationen() As Integer

        Dim cruiseSekunden As Double
        Dim easeOutSekunden As Double
        Dim effektiveGesamtZoomSekunden As Double
        Dim voraussichtlicheEndSkala As Double
        Dim benoetigteIterationen As Integer

        If aktuelleKamerafahrt Is Nothing OrElse
       aktuelleKamerafahrt.Count = 0 Then

            Return startpunkt.MaxIterations

        End If

        cruiseSekunden =
        aktuelleKamerafahrt.
        First(Function(p) p.Typ = ZoomPhaseTyp.Cruise).
        Dauer.TotalSeconds

        easeOutSekunden =
        aktuelleKamerafahrt.
        First(Function(p) p.Typ = ZoomPhaseTyp.EaseOut).
        Dauer.TotalSeconds

        'Während EaseOut sinkt die Geschwindigkeit linear auf 0.
        'Daher entspricht die Phase ungefähr der halben Laufzeit
        'bei voller Zoomgeschwindigkeit.
        effektiveGesamtZoomSekunden =
        cruiseSekunden +
        easeOutSekunden * 0.5

        voraussichtlicheEndSkala =
        BerechneAktuelleSkala(
            startpunkt.TargetScale,
            effektiveGesamtZoomSekunden)

        benoetigteIterationen =
        BerechneMaxIterationen(
            voraussichtlicheEndSkala)

        Return benoetigteIterationen

    End Function

    Private Sub SplitDouble(value As Double, ByRef high As Single, ByRef low As Single)

        high = CSng(value)
        low = CSng(value - CDbl(high))

    End Sub

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

        If mandelbrotPerturbationEffect IsNot Nothing Then
            mandelbrotPerturbationEffect.GradientTexture = Nothing
            mandelbrotPerturbationEffect.ReferenceOrbitRealHighTexture = Nothing
            mandelbrotPerturbationEffect.ReferenceOrbitImaginaryHighTexture = Nothing
            mandelbrotPerturbationEffect.ReferenceOrbitRealLowTexture = Nothing
            mandelbrotPerturbationEffect.ReferenceOrbitImaginaryLowTexture =
    Nothing
        End If

        If mandelbrotClassicEffect IsNot Nothing Then
            mandelbrotClassicEffect.GradientTexture = Nothing
        End If

        mandelbrotClassicEffect = Nothing
        mandelbrotPerturbationEffect = Nothing

        referenzOrbit = Nothing
        referenzOrbitRealHighBrush = Nothing
        referenzOrbitImaginaryHighBrush = Nothing
        referenzOrbitRealLowBrush = Nothing
        referenzOrbitImaginaryLowBrush = Nothing
        aktuellerGradientBrush = Nothing

        rctMandelbrot.Effect = Nothing
        rctMandelbrot.Fill = Nothing

        multipassPass1Brush = Nothing
        multipassPass1Bitmap = Nothing
        multipassPass2Bitmap = Nothing

        Me.Content = Nothing

    End Sub

#End Region

#Region "Brush-Diagnose"

    Private Sub StarteBrushStreifenTest()

        Dim dpiInfo As DpiScale
        Dim effektBreitePixel As Integer
        Dim effektHoehePixel As Integer
        Dim orbitWert As MandelbrotComplex
        Dim realHigh As Single
        Dim realLow As Single
        Dim imaginaryHigh As Single
        Dim imaginaryLow As Single
        Dim i As Integer

        rctMandelbrot.UpdateLayout()

        dpiInfo =
        VisualTreeHelper.GetDpi(
            rctMandelbrot)

        effektBreitePixel =
        Math.Max(
            1,
            CInt(
                Math.Round(
                    rctMandelbrot.ActualWidth *
                    dpiInfo.DpiScaleX)))

        effektHoehePixel =
        Math.Max(
            1,
            CInt(
                Math.Round(
                    rctMandelbrot.ActualHeight *
                    dpiInfo.DpiScaleY)))

        mandelbrotClassicEffect =
        New MandelbrotEffect()

        mandelbrotPerturbationEffect =
        New MandelbrotPerturbationEffect()

        'Acht echte Referenzorbitwerte für den Diagnosetest.
        referenzOrbit =
        New MandelbrotReferenzOrbit(
            startpunkt.CenterX,
            startpunkt.CenterY,
            8)

        If referenzOrbit.Count < 8 Then

            LogHandling.LogWarn(
            "Brush-Orbit-Komponententest: " &
            "Der Referenzorbit enthält nur " &
            referenzOrbit.Count.ToString() &
            " statt 8 Werten.")

            Return

        End If

        referenzOrbitRealHighBrush =
        referenzOrbit.
        ErzeugeRealHighOrbitBrush()

        referenzOrbitImaginaryHighBrush =
        referenzOrbit.
        ErzeugeImaginaryHighOrbitBrush()

        referenzOrbitRealLowBrush =
        referenzOrbit.
        ErzeugeRealLowOrbitBrush()

        referenzOrbitImaginaryLowBrush =
        referenzOrbit.
        ErzeugeImaginaryLowOrbitBrush()

        With mandelbrotPerturbationEffect

            .ReferenceOrbitRealHighTexture =
            referenzOrbitRealHighBrush

            .ReferenceOrbitImaginaryHighTexture =
            referenzOrbitImaginaryHighBrush

            .ReferenceOrbitRealLowTexture =
            referenzOrbitRealLowBrush

            .ReferenceOrbitImaginaryLowTexture =
            referenzOrbitImaginaryLowBrush

            .OrbitLength =
            CSng(
                referenzOrbit.Count)

            .OrbitTextureWidth =
            CSng(
                referenzOrbit.TexturBreite)

            .ViewportWidth =
            CSng(
                rctMandelbrot.ActualWidth)

            .ViewportHeight =
            CSng(
                rctMandelbrot.ActualHeight)

        End With

        For i = 0 To Math.Min(
        7,
        referenzOrbit.Count - 1)

            orbitWert =
            referenzOrbit.Item(i)

            SplitDouble(
            orbitWert.Real,
            realHigh,
            realLow)

            SplitDouble(
            orbitWert.Imaginary,
            imaginaryHigh,
            imaginaryLow)

            LogHandling.LogInfo(
            "Brush-Orbit-4K-Test: Index=" &
            i.ToString() &
            ", RealDouble=" &
            orbitWert.Real.ToString(
                "R",
                Globalization.CultureInfo.InvariantCulture) &
            ", RealHigh=" &
            realHigh.ToString(
                "R",
                Globalization.CultureInfo.InvariantCulture) &
            ", RealLow=" &
            realLow.ToString(
                "R",
                Globalization.CultureInfo.InvariantCulture) &
            ", ImaginaryDouble=" &
            orbitWert.Imaginary.ToString(
                "R",
                Globalization.CultureInfo.InvariantCulture) &
            ", ImaginaryHigh=" &
            imaginaryHigh.ToString(
                "R",
                Globalization.CultureInfo.InvariantCulture) &
            ", ImaginaryLow=" &
            imaginaryLow.ToString(
                "R",
                Globalization.CultureInfo.InvariantCulture))

        Next

        aktuellerRenderer =
        MandelbrotRendererTyp.Perturbation

        rctMandelbrot.Effect =
        mandelbrotPerturbationEffect

        LogHandling.LogInfo(
        "Modul Mandelbrot: Vier-Komponenten-Orbittest aktiviert. " &
        "OrbitCount=" &
        referenzOrbit.Count.ToString() &
        ", Texturbreite=" &
        referenzOrbit.TexturBreite.ToString() &
        ", ActualWidth=" &
        rctMandelbrot.ActualWidth.ToString(
            "F2",
            Globalization.CultureInfo.InvariantCulture) &
        ", ActualHeight=" &
        rctMandelbrot.ActualHeight.ToString(
            "F2",
            Globalization.CultureInfo.InvariantCulture) &
        ", Pixelbreite=" &
        effektBreitePixel.ToString() &
        ", Pixelhöhe=" &
        effektHoehePixel.ToString())

    End Sub

#End Region

#Region "Multipass-Proof-of-Concept"

    Private Sub StarteMultipassProofOfConcept()

        Dim testEffect As MultipassTestPass1Effect

        testEffect =
        New MultipassTestPass1Effect()

        rctMandelbrot.Fill =
        Brushes.White

        rctMandelbrot.Effect =
        testEffect

        LogHandling.LogInfo(
        "Multipass-Test: Pass-1-Shader direkt auf rctMandelbrot gelegt.")

    End Sub


    Private Function RendereMultipassPass1(
    breite As Integer,
    hoehe As Integer) As RenderTargetBitmap

        Dim passFlaeche As Rectangle
        Dim passEffect As MultipassTestPass1Effect
        Dim renderTarget As RenderTargetBitmap

        passEffect = New MultipassTestPass1Effect()

        passEffect.Input = Effect.ImplicitInput

        passFlaeche =
        New Rectangle()

        With passFlaeche

            .Width =
            CDbl(
                breite)

            .Height =
            CDbl(
                hoehe)

            .Fill =
            Brushes.White

            .Effect =
            passEffect

            .SnapsToDevicePixels =
            True

        End With

        passFlaeche.Measure(
        New Windows.Size(
            CDbl(breite),
            CDbl(hoehe)))

        passFlaeche.Arrange(
        New Rect(
            0.0,
            0.0,
            CDbl(breite),
            CDbl(hoehe)))

        passFlaeche.UpdateLayout()

        renderTarget =
        New RenderTargetBitmap(
            breite,
            hoehe,
            96.0,
            96.0,
            PixelFormats.Pbgra32)

        renderTarget.Render(
        passFlaeche)

        renderTarget.Freeze()

        passEffect.Input = Nothing

        passFlaeche.Effect = Nothing

        Return renderTarget

    End Function


    Private Function RendereMultipassPass2(
    breite As Integer,
    hoehe As Integer,
    stateBrush As ImageBrush) As RenderTargetBitmap

        Dim passFlaeche As Rectangle
        Dim passEffect As MultipassTestPass2Effect
        Dim renderTarget As RenderTargetBitmap

        passEffect =
        New MultipassTestPass2Effect()

        passEffect.StateTexture =
        stateBrush

        passFlaeche =
        New Rectangle()

        With passFlaeche

            .Width =
            CDbl(
                breite)

            .Height =
            CDbl(
                hoehe)

            .Fill =
            Brushes.White

            .Effect =
            passEffect

            .SnapsToDevicePixels =
            True

        End With

        passFlaeche.Measure(
        New Windows.Size(
            CDbl(breite),
            CDbl(hoehe)))

        passFlaeche.Arrange(
        New Rect(
            0.0,
            0.0,
            CDbl(breite),
            CDbl(hoehe)))

        passFlaeche.UpdateLayout()

        renderTarget =
        New RenderTargetBitmap(
            breite,
            hoehe,
            96.0,
            96.0,
            PixelFormats.Pbgra32)

        renderTarget.Render(
        passFlaeche)

        renderTarget.Freeze()

        passEffect.StateTexture =
        Nothing

        passFlaeche.Effect =
        Nothing

        Return renderTarget

    End Function

    Private Function ErzeugeMultipassBrush(
    bitmap As BitmapSource) As ImageBrush

        Dim brush As ImageBrush

        If bitmap Is Nothing Then

            Throw New ArgumentNullException(
            NameOf(bitmap))

        End If

        brush =
        New ImageBrush(
            bitmap)

        With brush

            .Stretch =
            Stretch.Fill

            .TileMode =
            TileMode.None

            .AlignmentX =
            AlignmentX.Left

            .AlignmentY =
            AlignmentY.Top

        End With

        RenderOptions.SetBitmapScalingMode(
        brush,
        BitmapScalingMode.NearestNeighbor)

        brush.Freeze()

        Return brush

    End Function

    Private Sub ZeigeMultipassErgebnis(
    bitmap As BitmapSource)

        Dim ergebnisBrush As ImageBrush

        ergebnisBrush =
        New ImageBrush(
            bitmap)

        With ergebnisBrush

            .Stretch =
            Stretch.Fill

            .TileMode =
            TileMode.None

            .AlignmentX =
            AlignmentX.Left

            .AlignmentY =
            AlignmentY.Top

        End With

        RenderOptions.SetBitmapScalingMode(
        ergebnisBrush,
        BitmapScalingMode.NearestNeighbor)

        rctMandelbrot.Effect =
        Nothing

        rctMandelbrot.Fill =
        ergebnisBrush

    End Sub

    Private Sub PruefeMultipassErgebnis(
    bitmap As BitmapSource)

        Dim breite As Integer
        Dim hoehe As Integer

        If bitmap Is Nothing Then

            LogHandling.LogWarn(
            "Multipass-Test: Ergebnisbitmap ist Nothing.")

            Exit Sub

        End If

        breite =
        bitmap.PixelWidth

        hoehe =
        bitmap.PixelHeight

        LoggeMultipassTestpixel(
        bitmap,
        breite \ 4,
        hoehe \ 4,
        "oben links",
        0.225)

        LoggeMultipassTestpixel(
        bitmap,
        breite * 3 \ 4,
        hoehe \ 4,
        "oben rechts",
        0.425)

        LoggeMultipassTestpixel(
        bitmap,
        breite \ 4,
        hoehe * 3 \ 4,
        "unten links",
        0.675)

        LoggeMultipassTestpixel(
        bitmap,
        breite * 3 \ 4,
        hoehe * 3 \ 4,
        "unten rechts",
        0.875)

    End Sub

    Private Sub LoggeMultipassTestpixel(
    bitmap As BitmapSource,
    x As Integer,
    y As Integer,
    bezeichnung As String,
    erwarteterWert As Double)

        Dim pixelDaten(3) As Byte
        Dim bereich As Int32Rect
        Dim gemessenerWert As Double
        Dim abweichung As Double

        bereich =
        New Int32Rect(
            x,
            y,
            1,
            1)

        bitmap.CopyPixels(
        bereich,
        pixelDaten,
        4,
        0)

        'Pbgra32/Bgra32:
        'Byte 0 = Blau
        'Byte 1 = Grün
        'Byte 2 = Rot
        'Byte 3 = Alpha
        '
        'Pass 2 gibt Grau aus, daher sind RGB identisch.
        gemessenerWert =
        CDbl(
            pixelDaten(2)) /
        255.0

        abweichung =
        Math.Abs(
            gemessenerWert -
            erwarteterWert)

        LogHandling.LogInfo(
        "Multipass-Test: " &
        bezeichnung &
        ", erwartet=" &
        erwarteterWert.ToString(
            "F6",
            Globalization.CultureInfo.InvariantCulture) &
        ", gemessen=" &
        gemessenerWert.ToString(
            "F6",
            Globalization.CultureInfo.InvariantCulture) &
        ", Abweichung=" &
        abweichung.ToString(
            "E6",
            Globalization.CultureInfo.InvariantCulture) &
        ", BGRA=" &
        pixelDaten(0).ToString() &
        "/" &
        pixelDaten(1).ToString() &
        "/" &
        pixelDaten(2).ToString() &
        "/" &
        pixelDaten(3).ToString())

    End Sub

    Private Sub PruefeMultipassPass1Ergebnis(
    bitmap As BitmapSource)

        Dim breite As Integer
        Dim hoehe As Integer

        If bitmap Is Nothing Then

            LogHandling.LogWarn(
                "Multipass-Test Pass 1: Bitmap ist Nothing.")

            Exit Sub

        End If

        breite =
            bitmap.PixelWidth

        hoehe =
            bitmap.PixelHeight

        LoggeMultipassPass1Pixel(
            bitmap,
            breite \ 4,
            hoehe \ 4,
            "oben links")

        LoggeMultipassPass1Pixel(
            bitmap,
            breite * 3 \ 4,
            hoehe \ 4,
            "oben rechts")

        LoggeMultipassPass1Pixel(
            bitmap,
            breite \ 4,
            hoehe * 3 \ 4,
            "unten links")

        LoggeMultipassPass1Pixel(
            bitmap,
            breite * 3 \ 4,
            hoehe * 3 \ 4,
            "unten rechts")

    End Sub

    Private Sub LoggeMultipassPass1Pixel(
    bitmap As BitmapSource,
    x As Integer,
    y As Integer,
    bezeichnung As String)

        Dim pixelDaten(3) As Byte
        Dim bereich As Int32Rect

        bereich =
            New Int32Rect(
                x,
                y,
                1,
                1)

        bitmap.CopyPixels(
            bereich,
            pixelDaten,
            4,
            0)

        LogHandling.LogInfo(
            "Multipass-Test Pass 1: " &
            bezeichnung &
            ", BGRA=" &
            pixelDaten(0).ToString() &
            "/" &
            pixelDaten(1).ToString() &
            "/" &
            pixelDaten(2).ToString() &
            "/" &
            pixelDaten(3).ToString())

    End Sub

#End Region

End Class