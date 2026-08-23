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

    'Direct3D Rendering
    Private direct3DRenderer As D3DRenderer
    Private rasterGenerator As PartikelRasterGenerator

    Private oldBitmapGerahmt As RenderTargetBitmap
    Private newBitmapGerahmt As RenderTargetBitmap

    Private testPartikel() As PartikelDaten

    'Zeitmanagement
    Private Const FPS As Integer = 60
    Private Const ABLOESE_DAUER_MS As Double = 5500.0

    Private frameTimer As DispatcherTimer
    Private ReadOnly laufzeit As New Stopwatch()

    Private letzteFrameZeitMS As Double

    'FlowField
    Private flowFieldGenerator As FlowFieldGenerator
    Private flowField As FlowFieldDaten

    Private aktuelleWindStaerke As Integer
    Private aktuelleWindRichtung As Single

    Private ReadOnly zufall As New Random()

    'Dünenfeld / Ablöse-HeatMap
    Private duenenGenerator As DuenenGenerator
    Private duenenFeld As DuenenFeldDaten

    'Lifecycle
    Private transitionLaeuft As Boolean
    Private wurdeBereinigt As Boolean
    Private frameZaehler As Integer

    Structure TransitionSettings_VomWindeVerweht
        Public partikelGroesse As Integer
        Public partikelGroesseZufall As Boolean
        Public windStaerke As Integer
        Public windStaerkeZufall As Boolean
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

        'Erzeugt direkt den finalen Ziel-Frame und meldet ihn
        'als fertiges Transitionsergebnis.

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(TransitionMain))
        End If

        BeendeUndBereinigeTransition()
        ReadTransitionSettingsFromRegistryOrDefaults()

        aktuelleWindStaerke = ErmittleWindStaerke()
        aktuelleWindRichtung = ErmittleWindRichtung()

        rasterGenerator = New PartikelRasterGenerator()

        testPartikel = rasterGenerator.ErzeugePartikelRaster(clientSize.Width, clientSize.Height,
                                                             aktuelleSettings.partikelGroesse, 12345)
        'FlowField Generation

        flowFieldGenerator = New FlowFieldGenerator()

        flowField = flowFieldGenerator.ErzeugeFlowField(clientSize.Width, clientSize.Height, aktuelleWindStaerke,
                                                        aktuelleWindRichtung, 12345)


        ' Das Dünenfeld ist ausdrücklich unabhängig
        ' vom aktuellen FlowField.
        duenenGenerator = New DuenenGenerator()

        duenenFeld = duenenGenerator.ErzeugeDuenenFeld(clientSize.Width, clientSize.Height, 54321)

        'Bitmaps
        oldBitmapGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
        newBitmapGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

        'Renderer
        direct3DRenderer = New D3DRenderer()

        Try

            direct3DRenderer.Initialisiere(clientSize.Width, clientSize.Height, testPartikel, oldBitmapGerahmt,
                                           newBitmapGerahmt, flowField, duenenFeld)
        Catch

            BeendeUndBereinigeTransition()

            Throw

        End Try

        transitionLaeuft = True

        RaiseEvent TransitionIsRunning(True)

        RaiseEvent TransitionFrameIstFertig(direct3DRenderer.FrameImage)

        letzteFrameZeitMS = 0.0
        frameZaehler = 0

        laufzeit.Restart()

        StarteRenderTimer()

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Beendet eine gegebenenfalls noch laufende Cut-Transition.

        BeendeUndBereinigeTransition()

    End Sub

    Private Sub TransitionIstFertig()

        If Not transitionLaeuft Then
            Exit Sub
        End If

        '
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

    End Sub

    Friend Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Standardwerte der Transition.

        Dim defaults As Dictionary(Of String, String)

        defaults = New Dictionary(Of String, String)()

        defaults.Add("PartikelGroesse", "4")
        defaults.Add("PartikelGroesseZufall", "False")
        defaults.Add("WindStaerke", "3")
        defaults.Add("WindStaerkeZufall", "False")

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

        If Not transitionLaeuft Then
            Exit Sub
        End If

        If direct3DRenderer Is Nothing Then
            Exit Sub
        End If

        aktuelleFrameZeitMS = laufzeit.Elapsed.TotalMilliseconds

        abloeseProgress = aktuelleFrameZeitMS / ABLOESE_DAUER_MS
        abloeseProgress = Math.Max(0.0, Math.Min(1.0, abloeseProgress))

        deltaTime = (aktuelleFrameZeitMS - letzteFrameZeitMS) / 1000.0

        letzteFrameZeitMS = aktuelleFrameZeitMS

        If deltaTime <= 0.0 Then
            Exit Sub
        End If

        'Verhindert nach Breakpoints oder längeren Hängern
        'extrem große Simulationssprünge.
        deltaTime = Math.Min(deltaTime, 0.1)

        frameZaehler += 1

        '
        ' Der GPU-Readback kann einen Pipeline-Stall verursachen.
        ' Für das Transitionsende reicht eine Prüfung alle vier Frames.
        '
        pruefeTransitionsende = frameZaehler Mod 4 = 0

        anzahlLebendePartikel = direct3DRenderer.RenderFrame(CSng(deltaTime), CSng(abloeseProgress),
                                                             pruefeTransitionsende)

        If anzahlLebendePartikel = 0 Then

            TransitionIstFertig()

        End If

    End Sub
#End Region

#Region "Wind & FlowField"

    Private Function ErmittleWindStaerke() As Integer

        If aktuelleSettings.windStaerkeZufall Then

            Return zufall.Next(1, 13)

        End If

        Return Math.Max(0, Math.Min(12, aktuelleSettings.windStaerke))

    End Function

    Private Function ErmittleWindRichtung() As Single

        If zufall.Next(0, 2) = 0 Then

            Return -1.0F

        End If

        Return 1.0F

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

        testPartikel = Nothing
        rasterGenerator = Nothing

        flowField = Nothing
        flowFieldGenerator = Nothing

        duenenFeld = Nothing
        duenenGenerator = Nothing

        aktuelleWindStaerke = 0
        aktuelleWindRichtung = 0.0F

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