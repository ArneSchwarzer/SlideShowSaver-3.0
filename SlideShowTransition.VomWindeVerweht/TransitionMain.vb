Imports System.Drawing
Imports System.Diagnostics
Imports System.Windows.Threading
Imports System.Windows.Forms
Imports System.Windows.Media.Imaging
Imports System.Windows.Media
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class TransitionMain
    Implements ISlideShowTransition

#Region "Variablendeklaration"
    'Settings und Konstanten
    Public Const SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Vom Winde verweht\"
    Public Const nameTransition As String = "Vom Winde verweht"
    Private aktuelleSettings As TransitionSettings_VomWindeVerweht

    Private aktuelleDauerAbrisskanteMS As Double
    Private aktuelleSchwerkraftAktiv As Boolean

    'Direct3D Rendering
    Private direct3DRenderer As D3DRenderer
    Private rasterGenerator As PartikelRasterGenerator

    Private oldBitmapGerahmt As RenderTargetBitmap
    Private newBitmapGerahmt As RenderTargetBitmap

    Private sandkornPartikel() As PartikelDaten
    Private aktuellePartikelGroesse As Integer

    'Zeitmanagement
    Private Const FPS As Integer = 60

    Private frameTimer As DispatcherTimer
    Private ReadOnly laufzeit As New Stopwatch()

    Private letzteFrameZeitMS As Double

    'FlowField
    Private flowFieldGenerator As FlowFieldGenerator
    Private flowField As FlowFieldDaten

    Private aktuelleWindStaerke As Integer
    Private flowFieldSeed As Integer
    Private letzteWindSteigerungsStufe As Integer

    'Sonstiges
    Private ReadOnly zufall As New Random()

    'V2/V3 Experiment: Dünenbasierte Ablösung
    'Private duenenGenerator As DuenenGenerator
    'Private duenenFeld As DuenenFeldDaten

    'Randbasierte Ablösung
    Private randAbloeseGenerator As RandAbloeseGenerator
    Private randAbloeseFeld As RandAbloeseFeldDaten

    'Lifecycle
    Private transitionLaeuft As Boolean
    Private wurdeBereinigt As Boolean
    Private frameZaehler As Integer

    Structure TransitionSettings_VomWindeVerweht

        Public partikelGroesse As Integer
        Public partikelGroesseZufall As Boolean

        Public windStaerke As Integer
        Public windStaerkeZufall As Boolean

        Public dauerAbrisskante As Integer

        Public schwerkraftModus As String

    End Structure

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName

        Get
            Return nameTransition
        End Get

    End Property

    Public ReadOnly Property TransitionKurzBeschreibung As String Implements ISlideShowTransition.TransitionKurzBeschreibung

        Get
            Return "Computational Fluid Dynamics mit Scarlett O'Hara"
        End Get

    End Property

    Public ReadOnly Property TransitionVersion As Version Implements ISlideShowTransition.TransitionVersion

        Get
            Return New Version(1, 0, 0, 0)
        End Get

    End Property

#End Region

#Region "Events"

    Public Event TransitionIsRunning As ISlideShowTransition.TransitionIsRunningEventHandler Implements ISlideShowTransition.TransitionIsRunning

    Public Event TransitionFrameIstFertig As ISlideShowTransition.TransitionFrameIstFertigEventHandler Implements ISlideShowTransition.TransitionFrameIstFertig

#End Region

#Region "Transition"

    Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As BitmapImage,
                             picBoxModeNew As PictureBoxSizeMode, clientSize As Size, Optional durationMs _
                             As Integer = 0) Implements ISlideShowTransition.RunTransition

        'Startet die Transition

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(TransitionMain))
        End If

        BeendeUndBereinigeTransition()
        ReadTransitionSettingsFromRegistryOrDefaults()

        aktuelleWindStaerke = ErmittleWindStaerke()
        aktuelleSchwerkraftAktiv = ErmittleSchwerkraftAktiv()
        aktuelleDauerAbrisskanteMS = ErmittleDauerAbrisskanteMS()
        aktuellePartikelGroesse = ErmittlePartikelGroesse()

        rasterGenerator = New PartikelRasterGenerator()

        sandkornPartikel = rasterGenerator.ErzeugePartikelRaster(clientSize.Width, clientSize.Height,
                                                             aktuellePartikelGroesse,
                                                             zufall.Next(1, Integer.MaxValue))

        ' -------------------------------------------------------
        ' Abrissgeometrie
        ' -------------------------------------------------------
        '
        ' Der Generator legt den gemeinsamen Ursprung fest.

        randAbloeseGenerator = New RandAbloeseGenerator()

        randAbloeseFeld = randAbloeseGenerator.ErzeugeRandAbloeseFeld(clientSize.Width, clientSize.Height,
                                                                      aktuelleSchwerkraftAktiv,
                                                                      zufall.Next(1, Integer.MaxValue))

        ' -------------------------------------------------------
        ' FlowField
        ' -------------------------------------------------------
        '
        ' Der Hauptwind übernimmt exakt die Richtung, die aus
        ' dem Ursprung der Abrisskante hervorgegangen ist.

        flowFieldGenerator = New FlowFieldGenerator()

        flowFieldSeed = zufall.Next(1, Integer.MaxValue)

        flowField = flowFieldGenerator.ErzeugeFlowField(clientSize.Width, clientSize.Height, aktuelleWindStaerke,
                                                        randAbloeseFeld.windRichtung, flowFieldSeed)

        'Bitmaps
        oldBitmapGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
        newBitmapGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

        'Renderer
        direct3DRenderer = New D3DRenderer()

        Try

            direct3DRenderer.Initialisiere(clientSize.Width, clientSize.Height, sandkornPartikel, oldBitmapGerahmt,
                                           newBitmapGerahmt, flowField, randAbloeseFeld)
        Catch

            BeendeUndBereinigeTransition()

            Throw

        End Try

        transitionLaeuft = True

        RaiseEvent TransitionIsRunning(True)

        RaiseEvent TransitionFrameIstFertig(direct3DRenderer.FrameImage)

        letzteFrameZeitMS = 0.0
        frameZaehler = 0
        letzteWindSteigerungsStufe = 0

        laufzeit.Restart()

        StarteRenderTimer()

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Beendet eine gegebenenfalls noch laufende Transition.

        BeendeUndBereinigeTransition()

    End Sub

    Private Sub TransitionIstFertig()

        If Not transitionLaeuft Then
            Exit Sub
        End If


        ' Den dynamischen D3D11Image-Frame ausdrücklich durch
        ' das endgültige, normale WPF-Zielbild ersetzen.
        '
        ' Danach darf der D3DRenderer gefahrlos freigegeben werden.
        '
        If newBitmapGerahmt IsNot Nothing Then

            RaiseEvent TransitionFrameIstFertig(
            newBitmapGerahmt)

        End If

        '
        ' Jetzt normal beenden:
        ' Timer stoppen, D3D freigeben und dem Framework
        ' TransitionIsRunning(False) melden.
        '
        BeendeUndBereinigeTransition()

    End Sub

#End Region

#Region "Optionsdialog"

    Public Function GetTransitionOptionsDialog() As UserControl _
    Implements ISlideShowTransition.GetTransitionOptionsDialog
        'Liest die aktuellen Einstellungen ein, stellt sie dem
        'Optionsdialog über die SettingsInbox zur Verfügung und
        'liefert anschließend das UserControl zurück.

        ReadTransitionSettingsFromRegistryOrDefaults()

        StoreSettings(nameTransition, aktuelleSettings)

        Return New ucOptionsTransition()

    End Function
#End Region

#Region "Settings"

    Private Sub ReadTransitionSettingsFromRegistryOrDefaults()
        'Liest sämtliche Transitionseinstellungen aus der Registry.
        'Nicht vorhandene Werte werden durch die Defaultwerte ersetzt.

        Dim defaults As Dictionary(Of String, String)

        defaults = GetTransitionDefaultSettings()

        aktuelleSettings.partikelGroesse = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH &
                                                                      "PartikelGroesse", defaults))

        aktuelleSettings.partikelGroesseZufall = CBool(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH &
                                                                             "PartikelGroesseZufall", defaults))

        aktuelleSettings.windStaerke = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH &
                                                                  "WindStaerke", defaults))

        aktuelleSettings.windStaerkeZufall = CBool(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH &
                                                                         "WindStaerkeZufall", defaults))
        aktuelleSettings.dauerAbrisskante = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH &
                                                                       "DauerAbrisskante", defaults))
        aktuelleSettings.schwerkraftModus = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH &
                                                                       "SchwerkraftModus", defaults)

    End Sub

    Friend Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Standardwerte der Transition.

        Dim defaults As Dictionary(Of String, String)

        defaults = New Dictionary(Of String, String)()

        defaults.Add("PartikelGroesse", "4")
        defaults.Add("PartikelGroesseZufall", "False")
        defaults.Add("WindStaerke", "3")
        defaults.Add("WindStaerkeZufall", "False")
        defaults.Add("DauerAbrisskante", "7")
        defaults.Add("SchwerkraftModus", "Zufällig")

        Return defaults

    End Function

#End Region

#Region "Timer"

    Private Sub StarteRenderTimer()

        frameTimer = New DispatcherTimer(DispatcherPriority.Render)

        frameTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / FPS)

        AddHandler frameTimer.Tick, AddressOf FrameTimer_Tick

        frameTimer.Start()

    End Sub

    Private Sub FrameTimer_Tick(sender As Object, e As EventArgs)

        Dim aktuelleFrameZeitMS As Double
        Dim deltaTime As Double
        Dim abloeseProgress As Double

        Dim pruefeTransitionsende As Boolean
        Dim anzahlLebendePartikel As Integer

        Dim gravitation As Single

        If Not transitionLaeuft Then
            Exit Sub
        End If

        If direct3DRenderer Is Nothing Then
            Exit Sub
        End If

        ' Backpressure:
        '
        ' Solange der zuletzt angeforderte Frame noch nicht
        ' tatsächlich durch RenderSurface() gelaufen ist,
        ' wird kein weiterer Simulationszustand erzeugt.

        If Not direct3DRenderer.KannNaechstenFrameRendern Then
            Exit Sub
        End If

        aktuelleFrameZeitMS = laufzeit.Elapsed.TotalMilliseconds

        PruefeUndErhoeheWindStaerke(aktuelleFrameZeitMS)

        abloeseProgress = aktuelleFrameZeitMS / aktuelleDauerAbrisskanteMS
        abloeseProgress = Math.Max(0.0, Math.Min(1.0, abloeseProgress))

        deltaTime = (aktuelleFrameZeitMS - letzteFrameZeitMS) / 1000.0

        letzteFrameZeitMS = aktuelleFrameZeitMS

        If deltaTime <= 0.0 Then
            Exit Sub
        End If

        'Schwerkraftcheck (lässt Kaffetasse fallen: Funktioniert noch ;-) 

        If aktuelleSchwerkraftAktiv Then
            gravitation = 18.0F
        Else
            gravitation = 0.0F
        End If

        'Verhindert nach Breakpoints oder längeren Hängern
        'extrem große Simulationssprünge.
        deltaTime = Math.Min(deltaTime, 0.1)

        frameZaehler += 1

        ' Der GPU-Readback kann einen Pipeline-Stall verursachen.
        ' Für das Transitionsende reicht eine Prüfung alle zwölf Frames.

        pruefeTransitionsende = abloeseProgress >= 1.0 AndAlso frameZaehler Mod 12 = 0

        anzahlLebendePartikel = direct3DRenderer.RenderFrame(CSng(deltaTime), CSng(abloeseProgress), gravitation,
                                                             pruefeTransitionsende)

        If frameZaehler Mod 60 = 0 Then

            Debug.WriteLine("APC Render-Partikel: " & direct3DRenderer.GibAPCAnzahlZurueck().ToString())

        End If


        If anzahlLebendePartikel = 0 Then

            TransitionIstFertig()

        End If

    End Sub

#End Region

#Region "Ermittle Parameter"

    Private Function ErmittleWindStaerke() As Integer

        If aktuelleSettings.windStaerkeZufall Then

            Return zufall.Next(1, 13)

        End If

        Return Math.Max(0, Math.Min(12, aktuelleSettings.windStaerke))

    End Function

    Private Sub PruefeUndErhoeheWindStaerke(aktuelleFrameZeitMS As Double)

        Dim aktuelleWindSteigerungsStufe As Integer

        If aktuelleDauerAbrisskanteMS <= 0.0 Then
            Exit Sub
        End If

        aktuelleWindSteigerungsStufe = CInt(Math.Floor(aktuelleFrameZeitMS / aktuelleDauerAbrisskanteMS))

        If aktuelleWindSteigerungsStufe <= letzteWindSteigerungsStufe Then

            Exit Sub

        End If

        letzteWindSteigerungsStufe = aktuelleWindSteigerungsStufe

        If aktuelleWindStaerke >= 12 Then
            Exit Sub
        End If

        aktuelleWindStaerke += 1

        AktualisiereFlowField()

    End Sub

    Private Sub AktualisiereFlowField()

        If flowFieldGenerator Is Nothing Then
            Exit Sub
        End If

        If randAbloeseFeld Is Nothing Then
            Exit Sub
        End If

        If direct3DRenderer Is Nothing Then
            Exit Sub
        End If

        flowField = flowFieldGenerator.ErzeugeFlowField(CInt(oldBitmapGerahmt.PixelWidth),
                                                        CInt(oldBitmapGerahmt.PixelHeight), aktuelleWindStaerke,
                                                        randAbloeseFeld.windRichtung, flowFieldSeed)

        direct3DRenderer.AktualisiereFlowField(flowField)

    End Sub

    Private Function ErmittleWindRichtung() As Single

        If zufall.Next(0, 2) = 0 Then

            Return -1.0F

        End If

        Return 1.0F

    End Function

    Private Function ErmittlePartikelGroesse() As Integer

        Dim exponent As Integer

        If aktuelleSettings.partikelGroesseZufall Then

            ' Die Trackbar bildet unsere LOD-Stufen 1, 2, 4, 8,
            ' 16, 32, 64 und 128 px ab.
            '
            ' Zufällig wird deshalb nicht irgendein Wert zwischen
            ' 1 und 128 gewählt, sondern exakt eine dieser Stufen.

            exponent = zufall.Next(0, 8)

            Return CInt(Math.Pow(2, exponent))

        End If

        Return Math.Max(1, Math.Min(128, aktuelleSettings.partikelGroesse))

    End Function

    Private Function ErmittleSchwerkraftAktiv() As Boolean

        Select Case aktuelleSettings.schwerkraftModus

            Case "An"

                Return True

            Case "Aus"

                Return False

            Case Else

                Return zufall.Next(0, 2) = 0

        End Select

    End Function

    Private Function ErmittleDauerAbrisskanteMS() As Double

        Dim dauerSekunden As Integer

        dauerSekunden = Math.Max(5, Math.Min(30, aktuelleSettings.dauerAbrisskante))

        Return CDbl(dauerSekunden) * 1000.0

    End Function

#End Region

#Region "Bereinigung & Dispose"

    Private Sub BeendeUndBereinigeTransition()

        Dim warTransitionAktiv As Boolean

        warTransitionAktiv = transitionLaeuft

        transitionLaeuft = False

        If frameTimer IsNot Nothing Then

            frameTimer.Stop()

            RemoveHandler frameTimer.Tick, AddressOf FrameTimer_Tick

            frameTimer = Nothing

        End If

        laufzeit.Stop()
        laufzeit.Reset()

        letzteFrameZeitMS = 0.0
        frameZaehler = 0

        If direct3DRenderer IsNot Nothing Then

            direct3DRenderer.Dispose()
            direct3DRenderer = Nothing

        End If

        oldBitmapGerahmt = Nothing
        newBitmapGerahmt = Nothing

        sandkornPartikel = Nothing
        rasterGenerator = Nothing

        flowField = Nothing
        flowFieldGenerator = Nothing

        randAbloeseFeld = Nothing
        randAbloeseGenerator = Nothing

        aktuelleWindStaerke = 0
        flowFieldSeed = 0
        letzteWindSteigerungsStufe = 0

        If warTransitionAktiv Then

            RaiseEvent TransitionIsRunning(False)

        End If

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        'Beendet die Transition endgültig.

        If wurdeBereinigt Then
            Exit Sub
        End If

        BeendeUndBereinigeTransition()

        wurdeBereinigt = True

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class