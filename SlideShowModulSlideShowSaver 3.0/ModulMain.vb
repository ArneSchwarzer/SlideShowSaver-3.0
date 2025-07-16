Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowBildauswahl.BildauswahlMain

Public Class ModulMain
    Implements ISlideShowModul

#Region "Variablendeklaration"
    'Variablen, Konstanten, ENUMs etc. deklarieren.

    'Allgemein
    Public Shared aktuelleSettings As ModulSettings_SSS
    Public Const SLIDESHOWMODUL_SSS_FULLPATH As String = SLIDESHOWMODULBASE_PATH & "SlideShowSaver 3.0\"
    Public Const nameModul As String = "SlideShowSaver 3.0"
    Public Shared pauseIsActive As Boolean = False

    'Instanzen
    Public Shared Property sssScreen As frmModulMain
    Public Shared sssPause As frmPauseModusOverlay
    Public Shared Property sssInfo As frmPictureInfo
    Public Shared Property activeModuleInstanz As ISlideShowModul

    'Struktur für die Moduloptionen
    Public Structure ModulSettings_SSS
        Public Bildauswahl As String
        Public Anzeigedauer As Integer
        Public Transitionseffekte As List(Of String)
        Public TransitionsReihenfolge As String
        Public Shader As List(Of String)
        Public ShaderReihenfolge As String
        Public BildInfoAnzeigen As Boolean
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
            Return "Die klassische Diashow im neuen Gewand."
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
            Return True
        End Get
    End Property

    Public ReadOnly Property ModulNutztShader As Boolean Implements ISlideShowModul.ModulNutztShader
        Get
            Return True
        End Get
    End Property

    'Events
#Region "Events"
    Public Event ModulStateChanged(newState As String) Implements ISlideShowModul.ModulStateChanged
    Public Shared Event YouHaveMail_SSS()
#End Region

    'Start/Stop/Pause
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        'Initialisiert und startet das eigentliche Modul

        RaiseEvent ModulStateChanged("Running")

        ' Instanzen generieren
        activeModuleInstanz = Me

        If sssScreen Is Nothing Then
            sssScreen = New frmModulMain()
        End If


        'Settings einlesen und erste Inititalisierungen durchführen
        CheckYourSettings()

        'Modul anzeigen
        sssScreen.WindowState = FormWindowState.Minimized
        sssScreen.Show()

    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Räumt auf und meldet, das der Schoner ordungsgemäß beendet wurde

        If sssInfo IsNot Nothing Then
            sssInfo.Close()
            sssInfo.Dispose()
            sssInfo = Nothing
        End If

        If sssPause IsNot Nothing Then
            sssPause.Close()
            sssPause.Dispose()
            sssPause = Nothing
        End If

        If sssScreen IsNot Nothing Then
            sssScreen.Close()
            sssScreen.Dispose()
            sssScreen = Nothing
        End If

        RaiseEvent ModulStateChanged("Stopped")

    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        ' Startet den Pause-Modus des Moduls

        RaiseEvent ModulStateChanged("Pause")

        If sssScreen.transitionIstAktiv Then
            sssScreen.aktiveTransition.StopTransition()
        End If

        sssScreen.tmrModul.Stop()

        If sssPause Is Nothing Then
            sssPause = New frmPauseModusOverlay()
        End If

        pauseIsActive = True

        If sssPause.ShowDialog() = DialogResult.OK Then
            sssPause.Dispose()
            sssPause = Nothing
            CheckYourSettings()
            RaiseEvent ModulStateChanged("Running")
        End If

    End Sub

    'Optionen & OptionsDialog

    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog
        'Liefert der frmOptionsMain das leere UC zum Einbau in die Modul-Tabpage.

        'Settings abholen und zwischenspeichern
        ReadModulSettingsFromRegistryOrDefaults()
        StoreSettings(ModulName, aktuelleSettings)

        'Jetzt die UC ausgeben
        Return New ucOptionsModul()

    End Function

    'Info-Kommunikation
    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        '(Re-)Initalisierung der Optionen aus der Registry nachdem das MCP eine Änderung gemeldet hat.

        'Liest die aktuellen Settings und legt sie in der SettingsInbox ab
        ReadModulSettingsFromRegistryOrDefaults()
        StoreSettings(nameModul, aktuelleSettings)

        'Der Form-Instanz Bescheid geben
        RaiseEvent YouHaveMail_SSS()

        'Basierend auf den aktuellen Settings die Instanz der frmPictureInfo ein- oder ausblenden  
        If aktuelleSettings.BildInfoAnzeigen Then
            If sssInfo Is Nothing Then
                sssInfo = New frmPictureInfo()
            End If
            sssInfo.Show()
        ElseIf sssInfo IsNot Nothing Then
            sssInfo.Close()
            sssInfo.Dispose()
            sssInfo = Nothing
        End If

        'Timer gemäß der neuen Anzeigedauer setzen. Stop/Start, um die
        'Änderungen sofort wirken zu lassen (falls z.B. die Anzeigedauer von 2m auf 20s zurückgesetzt wurde,
        'möchte der Benutzer keine 2 Minuten warten, bis die Änderung greift).

        sssScreen.tmrModul.Stop()
        If aktuelleSettings.Anzeigedauer > 0 Then
            sssScreen.tmrModul.Interval = aktuelleSettings.Anzeigedauer * 1000
        Else
            sssScreen.tmrModul.Interval = 20 * 1000 'Defaultwert, falls beim Lesen aus der Registry etwas falsch gelaufen ist
        End If
        sssScreen.tmrModul.Start()

    End Sub

    'Private Methoden
    Public Shared Function GetModulDefaultSettings() As Dictionary(Of String, String)
        Dim defaultModulSettings As New Dictionary(Of String, String)
        'Liefert die Default-Werte des Moduls

        defaultModulSettings("Bildauswahl") = "Zufallsbild"
        defaultModulSettings("Anzeigedauer") = "20"
        defaultModulSettings("Transitionseffekte") = ""
        defaultModulSettings("TransitionsReihenfolge") = "Zufällig bei Start"
        defaultModulSettings("Shader") = ""
        defaultModulSettings("ShaderReihenfolge") = "Zufällig bei Start"
        defaultModulSettings("BildInfoAnzeigen") = "False"

        Return defaultModulSettings
    End Function

    Public Sub ReadModulSettingsFromRegistryOrDefaults()
        'Liest die Settings des Moduls aus der Registry. Falls diese nicht gesetzt sind, werden Default-Werte ausgegeben.

        Dim defaults As New Dictionary(Of String, String)
        Dim tempRegVal As String

        defaults = GetModulDefaultSettings()

        'Modus Bildauswahl
        aktuelleSettings.Bildauswahl = ReadFromRegOrDefaults(SLIDESHOWMODUL_SSS_FULLPATH & "Bildauswahl", defaults)

        'Anzeigedauer
        aktuelleSettings.Anzeigedauer = CInt(ReadFromRegOrDefaults(SLIDESHOWMODUL_SSS_FULLPATH & "Anzeigedauer", defaults))

        'Liste Transitionen
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWMODUL_SSS_FULLPATH & "Transitionseffekte", defaults)
        aktuelleSettings.Transitionseffekte = SplitSemicolonList(tempRegVal)

        'Modus Transitionen
        aktuelleSettings.TransitionsReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMODUL_SSS_FULLPATH & "TransitionsReihenfolge", defaults)

        'Liste Shader
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWMODUL_SSS_FULLPATH & "Shader", defaults)
        aktuelleSettings.Shader = SplitSemicolonList(tempRegVal)

        'Modus Shader
        aktuelleSettings.ShaderReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMODUL_SSS_FULLPATH & "ShaderReihenfolge", defaults)

        'Modus BildInfo Anzeigen
        aktuelleSettings.BildInfoAnzeigen = CBool(ReadFromRegOrDefaults(SLIDESHOWMODUL_SSS_FULLPATH & "BildInfoAnzeigen", defaults))

    End Sub

End Class
