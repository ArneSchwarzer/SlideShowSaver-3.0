Imports System.IO
Imports System.Windows.Input
Imports System.Windows.Threading
Imports SlideShowLogging
Imports SlideShowTools
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SharedDataHandling
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowWPFModul.Mandelbrot.ModulMain

Public Class wpfModulMain

#Region "Variablendeklaration"

    Private aktuelleSettings As ModulMain.ModulSettings_Mandelbrot

    'Initialisierung
    Private hintergrundWM As Windows.Media.Color
    Private ReadOnly zielePfad As String = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "SlideShowSaver 3.0\Module\Mandelbrot\MandelbrotZiele.xml"
                    )

    'Gradienten
    Private gradienten As List(Of SlideShowGradient)
    Private aktuellerGradient As SlideShowGradient
    Private aktuellerGradientBrush As ImageBrush
    Private gradientOffset As Double = 0.0
    Private gradientGeschwindigkeit As Double = 0.05

    'Start- und Zielpunkte
    Private startpunkt As MandelbrotZiel
    Private aktuellesZiel As MandelbrotZiel

    Private zielRepository As MandelbrotZielRepository
    Private zielAuswahl As MandelbrotZielAuswahl

    'Skalierung und Iteration
    Private Const startIterationen As Integer = 100

    'Shader
    Private mandelbrotClassicEffect As MandelbrotEffect
    Private renderingAktiv As Boolean = False

    'Kamera
    Private kameraController As MandelbrotKameraController
    Private kameraZustand As MandelbrotKameraZustand

    'Rotation 
    Private Const minRotationsWinkel As Double = 33.0
    Private Const maxRotationsWinkel As Double = 180.0

    'Lebenszyklus
    Private wirdGeschlossen As Boolean
    Private wurdeBereinigt As Boolean

    'Routed-Event-Handler
    Private previewMouseDownHandler As MouseButtonEventHandler
    Private previewKeyDownHandler As System.Windows.Input.KeyEventHandler

    'Dispatcheroperationen
    Private darstellungsbereitRenderOperation As DispatcherOperation
    Private darstellungsbereitIdleOperation As DispatcherOperation
    Private darstellungsbereitschaftWurdeEingeplant As Boolean

    'Sonstiges
    Private rnd As New Random()
    Private darstellungsbereitschaftWurdeGemeldet As Boolean = False

#End Region

    'Events
    Public Event DarstellungIstBereit()

    Public Sub New(settings As ModulMain.ModulSettings_Mandelbrot)

        InitializeComponent()
        AktualisiereSettingsIntern(settings)

        previewMouseDownHandler = New MouseButtonEventHandler(AddressOf Window_PreviewMouseDown)
        previewKeyDownHandler = New System.Windows.Input.KeyEventHandler(AddressOf Window_PreviewKeyDown)

        Me.AddHandler(UIElement.PreviewMouseDownEvent, previewMouseDownHandler, True)
        Me.AddHandler(UIElement.PreviewKeyDownEvent, previewKeyDownHandler, True)

        Me.WindowState = WindowState.Maximized

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        Me.Topmost = False
        Me.ShowActivated = True
        Me.Activate()

        hintergrundWM = SDColorToWMColor(HintergrundFarbeSaver)
        Me.Background = New SolidColorBrush(hintergrundWM)
        grdHauptbereich.Background = New SolidColorBrush(hintergrundWM)

        rctMandelbrot.Fill = New SolidColorBrush(hintergrundWM)

        gradienten = GradientenHandling.LadeGradienten()

        InitialisiereStartpunkt()
        InitialisiereShader()
        InitialisiereKameraController()

        If Not InitialisiereZielverwaltung() Then

            LogHandling.LogWarn("Modul Mandelbrot: Die Zielverwaltung konnte nicht initialisiert werden.")

            Exit Sub

        End If

        If StarteNeueKamerafahrt() Then

            StartRendering()
            MeldeDarstellungsbereitschaft()

        Else

            LogHandling.LogWarn("Modul Mandelbrot: Rendering wurde nicht gestartet.")

        End If

    End Sub

    Private Sub InitialisiereStartpunkt()
        'Initialisiert die vollständige Ansicht des Mandelbrot-Sets.

        startpunkt =
        New MandelbrotZiel() With {
            .Name = "Apfelmännchen",
            .CenterX = -0.5,
            .CenterY = 0.0,
            .TargetScale = 3.0
        }

    End Sub

    Private Function InitialisiereZielverwaltung() As Boolean
        'Initialisiert Repository und Zufallsauswahl
        'und lädt sämtliche Mandelbrot-Ziele einmalig.

        zielRepository = New MandelbrotZielRepository(zielePfad)
        zielAuswahl = New MandelbrotZielAuswahl(rnd)

        Return zielRepository.LadeZiele()

    End Function

    Private Sub InitialisiereKameraController()
        'Initialisiert den zustandsbehafteten Kamera-Controller.

        kameraController =
        New MandelbrotKameraController(
            rnd,
            minRotationsWinkel,
            maxRotationsWinkel,
            startIterationen)

        kameraZustand = kameraController.AktuellerZustand

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

        If aktuellesZiel Is Nothing OrElse kameraZustand Is Nothing Then

            Exit Sub

        End If

        zielMaxIterationen = MandelbrotMathematik.BerechneMaxIterationen(
            startpunkt.TargetScale,
            aktuellesZiel.TargetScale,
            startIterationen)

        textfarbe = InvertWMColor(hintergrundWM)

        ucZielOverlay.InitialisiereZiel(
            aktuellesZiel.Name,
            aktuellesZiel.CenterX,
            aktuellesZiel.CenterY,
            aktuellesZiel.TargetScale,
            zielMaxIterationen,
            aktuelleSettings.Rotation,
            kameraZustand.ZielRotation,
            hintergrundWM,
            textfarbe)

        ucZielOverlay.ZeigeFreezeIn()

    End Sub

    Private Function StarteNeueKamerafahrt() As Boolean
        'Wählt Ziel und Gradient und startet eine neue Kamerafahrt.

        If Not WaehleNeuesZiel() Then
            Return False
        End If

        If kameraController Is Nothing Then

            LogHandling.LogWarn("Modul Mandelbrot - wpfModulMain.StarteNeueKamerafahrt(): " &
                                "Der Kamera-Controller wurde noch nicht initialisiert.")

            Return False

        End If

        WaehleNeuenGradienten()

        If Not kameraController.StarteKamerafahrt(
        startpunkt,
        aktuellesZiel,
        aktuelleSettings.Zoomgeschwindigkeit,
        aktuelleSettings.Rotation) Then

            LogHandling.LogWarn("Modul Mandelbrot - wpfModulMain.StarteNeueKamerafahrt(): " &
                                "Die Kamerafahrt konnte nicht gestartet werden.")

            Return False

        End If

        kameraZustand = kameraController.AktuellerZustand
        rctMandelbrot.Effect = mandelbrotClassicEffect

        gradientOffset = 0.0

        InitialisiereZielOverlay()

        Return True

    End Function

    Private Function WaehleNeuesZiel() As Boolean
        'Wählt über die Zielverwaltung ein neues Mandelbrot-Ziel.

        If zielRepository Is Nothing OrElse zielAuswahl Is Nothing OrElse startpunkt Is Nothing Then

            LogHandling.LogWarn("Modul Mandelbrot - wpfModulMain.WaehleNeuesZiel(): " &
                                "Die Zielverwaltung wurde noch nicht vollständig initialisiert.")

            Return False

        End If

        aktuellesZiel = zielAuswahl.WaehleZiel(zielRepository.Ziele, startpunkt.TargetScale)

        If aktuellesZiel Is Nothing Then
            Return False
        End If

        LogHandling.LogInfo("Modul Mandelbrot: Aktuelles Ziel: " & aktuellesZiel.Name)

        Return True

    End Function

    Private Sub WaehleNeuenGradienten()
        'Wählt einen für Mandelbrot freigegebenen Gradient aus
        'der zentralen Gradientenbibliothek.

        Dim erlaubteGradienten As List(Of SlideShowGradient)

        If gradienten Is Nothing OrElse gradienten.Count = 0 Then

            LogHandling.LogWarn("Modul Mandelbrot: Die zentrale Gradientenbibliothek enthält keine Gradienten.")

            Exit Sub

        End If

        If aktuelleSettings.Gradienten Is Nothing OrElse aktuelleSettings.Gradienten.Count = 0 Then

            erlaubteGradienten = gradienten.ToList()

        Else

            erlaubteGradienten =
            gradienten.Where(
                Function(gradient)

                    Return aktuelleSettings.Gradienten.Contains(
                        gradient.Name,
                        StringComparer.OrdinalIgnoreCase)

                End Function).
            ToList()

        End If

        If erlaubteGradienten.Count = 0 Then

            erlaubteGradienten = gradienten.ToList()

        End If

        aktuellerGradient =
        erlaubteGradienten(
            rnd.Next(
                erlaubteGradienten.Count))

        aktuellerGradientBrush = GradientenHandling.ErzeugeGradientBrush(aktuellerGradient, 1024)

        If mandelbrotClassicEffect IsNot Nothing Then

            mandelbrotClassicEffect.GradientTexture = aktuellerGradientBrush

        End If

        LogHandling.LogInfo("Modul Mandelbrot: Aktueller Gradient: " & aktuellerGradient.Name)

    End Sub

#Region "Rendering-Ablauf"

    Private Sub StartRendering()

        If wirdGeschlossen OrElse wurdeBereinigt Then

            Exit Sub

        End If

        If renderingAktiv Then
            Exit Sub
        End If

        AddHandler CompositionTarget.Rendering, AddressOf OnRenderingFrame
        renderingAktiv = True

    End Sub

    Private Sub StopRendering()
        'Die Abmeldung erfolgt immer und ist damit idempotent.

        RemoveHandler CompositionTarget.Rendering, AddressOf OnRenderingFrame

        renderingAktiv = False

    End Sub

    Private Sub OnRenderingFrame(sender As Object, e As EventArgs)

        If wirdGeschlossen OrElse wurdeBereinigt Then

            Exit Sub

        End If

        UpdateFrame()
        RenderFrame()

    End Sub

    Private Sub UpdateFrame()

        AktualisiereKameraZustand()
        AktualisiereGradient()

    End Sub

    Private Sub RenderFrame()

        AktualisiereKlassischeShaderParameter()

    End Sub

#End Region

#Region "Update"

    Private Sub AktualisiereKameraZustand()
        'Aktualisiert den Kamera-Controller und überträgt
        'den aktuellen Zustand auf das Ziel-Overlay.

        If kameraController Is Nothing Then
            Exit Sub
        End If

        kameraZustand = kameraController.Aktualisiere()

        If kameraZustand Is Nothing Then
            Exit Sub
        End If

        AktualisiereZielOverlayAusKameraZustand()

        If kameraZustand.KamerafahrtBeendet Then

            If Not StarteNeueKamerafahrt() Then

                StopRendering()

            End If

        End If

    End Sub

    Private Sub AktualisiereZielOverlayAusKameraZustand()
        'Aktualisiert das Ziel-Overlay anhand der aktuellen Kameraphase.

        If Not ZielOverlayIstAktiv() OrElse kameraZustand Is Nothing Then

            Exit Sub

        End If

        Select Case kameraZustand.Phase

            Case MandelbrotKameraZustand.KameraPhaseTyp.FreezeIn

                ucZielOverlay.ZeigeFreezeIn()

            Case MandelbrotKameraZustand.KameraPhaseTyp.Translation

                ucZielOverlay.AktualisiereTranslation(kameraZustand.PhasenFortschritt)

            Case MandelbrotKameraZustand.KameraPhaseTyp.Cruise

                ucZielOverlay.ZeigeInformationszeile()

                AktualisiereZielOverlayWerte()

            Case MandelbrotKameraZustand.KameraPhaseTyp.EaseOut

                ucZielOverlay.ZeigeInformationszeile()

                AktualisiereZielOverlayWerte()

            Case MandelbrotKameraZustand.KameraPhaseTyp.FreezeOut

                AktualisiereZielOverlayWerte()

                ucZielOverlay.AktualisiereFreezeOut(kameraZustand.PhasenFortschritt)

        End Select

    End Sub

    Private Sub AktualisiereZielOverlayWerte()
        'Überträgt die aktuellen Kamerawerte auf das Overlay.

        If kameraZustand Is Nothing OrElse ucZielOverlay Is Nothing Then

            Exit Sub

        End If

        ucZielOverlay.AktualisiereWerte(
            kameraZustand.CenterX,
            kameraZustand.CenterY,
            kameraZustand.Skala,
            kameraZustand.MaxIterationen)

    End Sub

    Private Sub AktualisiereGradient()
        'Aktualisiert den animierten Offset des aktuellen Gradienten.

        If kameraZustand Is Nothing Then

            gradientOffset = 0.0

            Exit Sub

        End If

        If aktuelleSettings.GradientAnimieren Then

            gradientOffset = (kameraZustand.KamerafahrtSekunden * gradientGeschwindigkeit) Mod 1.0

        Else

            gradientOffset = 0.0

        End If

    End Sub

#End Region

#Region "Render"

    Private Sub AktualisiereKlassischeShaderParameter()
        'Überträgt den aktuellen Kamerazustand auf den klassischen Shader.

        Dim centerXHigh As Single
        Dim centerXLow As Single
        Dim centerYHigh As Single
        Dim centerYLow As Single
        Dim scaleHigh As Single
        Dim scaleLow As Single

        If mandelbrotClassicEffect Is Nothing OrElse kameraZustand Is Nothing Then

            Exit Sub

        End If

        MandelbrotMathematik.SplitDouble(kameraZustand.CenterX, centerXHigh, centerXLow)
        MandelbrotMathematik.SplitDouble(kameraZustand.CenterY, centerYHigh, centerYLow)
        MandelbrotMathematik.SplitDouble(kameraZustand.Skala, scaleHigh, scaleLow)

        With mandelbrotClassicEffect

            .CenterXHigh = centerXHigh
            .CenterXLow = centerXLow

            .CenterYHigh = centerYHigh
            .CenterYLow = centerYLow

            .ScaleHigh = scaleHigh
            .ScaleLow = scaleLow

            .MaxIterations = CSng(kameraZustand.MaxIterationen)

            .ViewportWidth = CSng(Math.Max(1, rctMandelbrot.ActualWidth))
            .ViewportHeight = CSng(Math.Max(1, rctMandelbrot.ActualHeight))

            .Rotation = CSng(kameraZustand.Rotation)

            .GradientOffset = CSng(gradientOffset)

        End With

    End Sub

#End Region

#Region "Hilfsfunktionen"

    Private Function ZielOverlayIstAktiv() As Boolean

        If ucZielOverlay Is Nothing Then
            Return False
        End If

        Return aktuelleSettings.KoordinatenAnzeigen

    End Function

#End Region

#Region "Framework / Events"

    Private Sub MeldeDarstellungsbereitschaft()
        'Meldet dem Framework einmalig die fertige Darstellung.

        If wirdGeschlossen OrElse wurdeBereinigt Then

            Exit Sub

        End If

        If darstellungsbereitschaftWurdeEingeplant OrElse darstellungsbereitschaftWurdeGemeldet Then

            Exit Sub

        End If

        darstellungsbereitschaftWurdeEingeplant = True

        darstellungsbereitRenderOperation = Dispatcher.BeginInvoke(DispatcherPriority.Render,
                                                                   New Action(AddressOf DarstellungsbereitschaftRendern))

    End Sub

    Private Sub DarstellungsbereitschaftRendern()

        darstellungsbereitRenderOperation = Nothing

        If wirdGeschlossen OrElse wurdeBereinigt Then

            Exit Sub

        End If

        Me.UpdateLayout()

        darstellungsbereitIdleOperation = Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                                                                 New Action(AddressOf DarstellungsbereitschaftMelden))

    End Sub

    Private Sub DarstellungsbereitschaftMelden()

        darstellungsbereitIdleOperation = Nothing

        If wirdGeschlossen OrElse wurdeBereinigt Then

            Exit Sub

        End If

        If darstellungsbereitschaftWurdeGemeldet Then
            Exit Sub
        End If

        darstellungsbereitschaftWurdeGemeldet = True

        RaiseEvent DarstellungIstBereit()

    End Sub

    Private Sub BrecheDispatcherOperationAb(ByRef operation As DispatcherOperation)
        'Bricht eine noch ausstehende Dispatcheroperation ab.

        If operation Is Nothing Then
            Exit Sub
        End If

        Try

            If operation.Status = DispatcherOperationStatus.Pending Then

                operation.Abort()

            End If

        Catch ex As InvalidOperationException

            'Die Operation wurde zwischen Prüfung und Abbruch beendet.

        Finally

            operation = Nothing

        End Try

    End Sub

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

    Public Sub AktualisiereSettings(settings As ModulMain.ModulSettings_Mandelbrot)
        'Übernimmt neue Moduleinstellungen.

        If wirdGeschlossen OrElse wurdeBereinigt Then

            Exit Sub

        End If

        AktualisiereSettingsIntern(settings)

        If ucZielOverlay Is Nothing Then
            Exit Sub
        End If

        If aktuelleSettings.KoordinatenAnzeigen Then

            If aktuellesZiel IsNot Nothing AndAlso Not String.IsNullOrEmpty(aktuellesZiel.Name) Then

                InitialisiereZielOverlay()

            End If

        Else

            ucZielOverlay.Verberge()

        End If

    End Sub

    Private Sub AktualisiereSettingsIntern(settings As ModulMain.ModulSettings_Mandelbrot)
        'Übernimmt die Settings und kopiert veränderbare Listen.

        aktuelleSettings = settings

        If settings.Gradienten IsNot Nothing Then

            aktuelleSettings.Gradienten = New List(Of String)(settings.Gradienten)

        Else

            aktuelleSettings.Gradienten = New List(Of String)()

        End If

    End Sub

    Private Sub BereinigeFensterRessourcen()
        'Löst alle externen Bindungen und anschließend den Objektgraphen.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wirdGeschlossen = True

        Try

            BrecheDispatcherOperationAb(darstellungsbereitRenderOperation)
            BrecheDispatcherOperationAb(darstellungsbereitIdleOperation)

            StopRendering()

            If previewMouseDownHandler IsNot Nothing Then

                Me.RemoveHandler(UIElement.PreviewMouseDownEvent, previewMouseDownHandler)

                previewMouseDownHandler = Nothing

            End If

            If previewKeyDownHandler IsNot Nothing Then

                Me.RemoveHandler(UIElement.PreviewKeyDownEvent, previewKeyDownHandler)

                previewKeyDownHandler = Nothing

            End If

            If rctMandelbrot IsNot Nothing Then

                rctMandelbrot.Effect = Nothing
                rctMandelbrot.Fill = Nothing

            End If

            If mandelbrotClassicEffect IsNot Nothing Then

                mandelbrotClassicEffect.GradientTexture = Nothing

            End If

            mandelbrotClassicEffect = Nothing
            aktuellerGradientBrush = Nothing
            aktuellerGradient = Nothing
            gradienten = Nothing

            If kameraController IsNot Nothing Then

                kameraController.Beende()
                kameraController = Nothing

            End If

            kameraZustand = Nothing

            zielAuswahl = Nothing
            zielRepository = Nothing

            startpunkt = Nothing
            aktuellesZiel = Nothing

            Me.Content = Nothing

        Finally

            wurdeBereinigt = True

        End Try

    End Sub

    Private Sub wpfModulMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed

        BereinigeFensterRessourcen()

    End Sub

#End Region

End Class