Imports System.Drawing
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

    'Lifecycle
    Private transitionLaeuft As Boolean
    Private wurdeBereinigt As Boolean

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

        Dim sizeWPF As Windows.Size
        Dim zielFrame As RenderTargetBitmap

        Dim rasterGenerator As PartikelRasterGenerator
        Dim testPartikel() As PartikelDaten


        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(TransitionMain))

        End If

        BeendeUndBereinigeTransition()
        ReadTransitionSettingsFromRegistryOrDefaults()

        rasterGenerator = New PartikelRasterGenerator()

        testPartikel = rasterGenerator.ErzeugePartikelRaster(clientSize.Width, clientSize.Height,
                                                             aktuelleSettings.partikelGroesse, 12345)

        oldBitmapGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
        newBitmapGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

        direct3DRenderer = New D3DRenderer()

        direct3DRenderer.Initialisiere(clientSize.Width, clientSize.Height, testPartikel, oldBitmapGerahmt)

        transitionLaeuft = True

        RaiseEvent TransitionIsRunning(True)

        RaiseEvent TransitionFrameIstFertig(direct3DRenderer.FrameImage)

        direct3DRenderer.RenderFrame()

        'sizeWPF = New Windows.Size(clientSize.Width, clientSize.Height)

        'zielFrame = Nothing

        'Try

        '    transitionLaeuft = True

        '    RaiseEvent TransitionIsRunning(True)

        '    zielFrame = ConvertBitmapImageToRenderTargetBitmap(newImage, sizeWPF)

        '    'Dem aufrufenden Modul kurz Zeit zum Umschalten geben.
        '    System.Threading.Thread.Sleep(500)

        '    If Not transitionLaeuft Then
        '        Exit Sub
        '    End If

        '    RaiseEvent TransitionFrameIstFertig(zielFrame)

        'Finally

        '    BeendeUndBereinigeTransition()

        'End Try

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Beendet eine gegebenenfalls noch laufende Cut-Transition.

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

        defaults.Add("PartikelGroesse", "20")
        defaults.Add("PartikelGroesseZufall", "False")
        defaults.Add("WindStaerke", "3")
        defaults.Add("WindStaerkeZufall", "False")

        Return defaults

    End Function

#End Region

#Region "Bereinigung & Dispose"

    Private Sub BeendeUndBereinigeTransition()
        'Setzt den Laufzustand kontrolliert zurück.

        If Not transitionLaeuft Then
            Exit Sub
        End If

        transitionLaeuft = False

        If direct3DRenderer IsNot Nothing Then

            direct3DRenderer.Dispose()
            direct3DRenderer = Nothing

        End If

        oldBitmapGerahmt = Nothing
        newBitmapGerahmt = Nothing

        testPartikel = Nothing
        rasterGenerator = Nothing

        RaiseEvent TransitionIsRunning(False)

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