Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ConversionHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.ListHandling
Imports SlideShowLogging

Public Class ModulMain
    Implements ISlideShowModul

#Region "Variablendeklaration"
    'Variablendeklaration

    Private matrixScreen As frmModulMain

    Private Shared aktuelleSettings As ModulSettings_Matrix
    Public Shared nameModul As String = "Matrix"
    Public Shared SLIDESHOWMODUL_MATRIX_FULLPATH As String = SLIDESHOWMODULBASE_PATH & "Matrix\"
#End Region

#Region "Strukturen,Enums etc."
    'Strukturen, Enums etc.
    Public Structure ModulSettings_Matrix
        Public HighlightTexte As List(Of String)
        Public SzenendauerSekunden As Integer
    End Structure

#End Region

    'Eigenschaften
    Public ReadOnly Property ModulName As String Implements ISlideShowModul.ModulName
        Get
            Return nameModul
        End Get
    End Property

    Public ReadOnly Property ModulBeschreibung As String Implements ISlideShowModul.ModulBeschreibung
        Get
            Return "The Matrix has you..."
        End Get
    End Property

    Public ReadOnly Property ModulVersion As Version Implements ISlideShowModul.ModulVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    Public ReadOnly Property ModulNutztSlideShowBildauswahl As Boolean Implements ISlideShowModul.ModulNutztSlideShowBildauswahl
        Get
            Return True
        End Get
    End Property

    Public ReadOnly Property ModulNutztTransitions As Boolean Implements ISlideShowModul.ModulNutztTransitions
        Get
            Return False
        End Get
    End Property

    Public ReadOnly Property ModulNutztShader As Boolean Implements ISlideShowModul.ModulNutztShader
        Get
            Return False
        End Get
    End Property

    'Events

#Region "Events"
    Public Event ModulStateChanged(newState As String) Implements ISlideShowModul.ModulStateChanged
    Public Event ModulIstDarstellungsbereit() Implements ISlideShowModul.ModulIstDarstellungsbereit
    Public Shared Event YouHaveMail_Matrix()

#End Region

    'Start/Stop/Pause

    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        'Started das Modul als Instanz

        'AktuelleSettings auffrischen
        CheckYourSettings()

        Try

            matrixScreen = New frmModulMain()

            AddHandler matrixScreen.DarstellungIstBereit, AddressOf MatrixScreen_DarstellungIstBereit

        Catch ex As Exception

            LogHandling.LogError("Modul Matrix - ModulMain.StartModul(): Fehler beim Laden des Moduls: " & ex.ToString())

            Throw

        End Try

        matrixScreen.WindowState = FormWindowState.Normal

        matrixScreen.Show()

        RaiseEvent ModulStateChanged("Running")

    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Räumt auf und beendet das Modul

        If matrixScreen IsNot Nothing Then
            RemoveHandler matrixScreen.DarstellungIstBereit, AddressOf MatrixScreen_DarstellungIstBereit
            matrixScreen.Close()
            matrixScreen.Dispose()
            matrixScreen = Nothing
        End If

        RaiseEvent ModulStateChanged("Stopped")

    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        ' Wird für dieses Modul nicht benötigt
    End Sub

    'Eventweiterleitung
    Private Sub MatrixScreen_DarstellungIstBereit()
        'Leitet die Darstellungsbereitschaft an das Framework weiter.

        RaiseEvent ModulIstDarstellungsbereit()

    End Sub

    'Optionen & OptionsDialog
    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog
        'Liest die aktuellen Settings ein, speichert sie in SettingsInbox und liefert dann das ucOptionsModul

        'Aktuelle Settings einlesen und abspeichern
        ReadModulSettingsFromRegistryOrDefaults()
        StoreSettings(nameModul, aktuelleSettings)

        'Und jetzt die ucOptionsModul ausgeben
        Return New ucOptionsModul()

    End Function

    'Info-Kommunikation

    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        ' Initialisierung und Re-Initalisierung der Optionen aus der Registry

        'Aktuelle Settings abholen und auch gleich in SettingsInbox ablegen
        ReadModulSettingsFromRegistryOrDefaults()
        StoreSettings(nameModul, aktuelleSettings)

        'Und auch gleich matrixScreen Bescheid geben
        RaiseEvent YouHaveMail_Matrix()

    End Sub

    'Private Methoden

    Private Function GetModulDefaults() As Dictionary(Of String, String)
        'Liefert die Default-Einstellungen des Moduls

        Dim defaults As New Dictionary(Of String, String)

        defaults("Highlighttexte") = ""
        defaults("SzenendauerSekunden") = "20"

        Return defaults

    End Function

    Private Sub ReadModulSettingsFromRegistryOrDefaults()
        'Setzt aktuelle Settings entweder per Registry oder per Defaultwerten

        Dim temRegVal As String
        Dim defaults As Dictionary(Of String, String)

        defaults = GetModulDefaults()

        temRegVal = ReadFromRegOrDefaults(SLIDESHOWMODUL_MATRIX_FULLPATH & "Highlighttexte", defaults)
        aktuelleSettings.HighlightTexte = SplitSemicolonList(temRegVal)

        aktuelleSettings.SzenendauerSekunden = CInt(ReadFromRegOrDefaults(SLIDESHOWMODUL_MATRIX_FULLPATH & "SzenendauerSekunden", defaults))

    End Sub

End Class
