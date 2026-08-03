Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ModulMain
    Implements ISlideShowModul

#Region "Variablendeklaration"
    'Variablen, Konstanten, ENUMs etc. deklarieren.

    'Allgemein
    Public Shared aktuelleSettings As SettingsModul_SSS
    Public Const SLIDESHOWMODUL_SSS_FULLPATH As String = SLIDESHOWMODULBASE_PATH & "SlideShowSaver 3.0\"
    Public Const nameModul As String = "SlideShowSaver 3.0"
    Public Shared pauseIsActive As Boolean = False

    'Instanzen
    Public Shared Property sssScreen As wpfModulMain
    Public Shared sssPause As frmPauseModusOverlay
    Public Shared Property sssInfo As frmPictureInfo
    Public Shared Property activeModuleInstanz As ISlideShowModul

    'Struktur für die Moduloptionen
    Public Structure SettingsModul_SSS
        Public Bildauswahl As String
        Public Präsentationsschirm As Boolean
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
    Public Event ModulIstDarstellungsbereit() Implements ISlideShowModul.ModulIstDarstellungsbereit
    Public Shared Event YouHaveMail_SSS()
#End Region

    'Start/Stop/Pause
    Public Sub StartModul(
    targetScreen As Screen,
    Optional isPreview As Boolean = False,
    Optional targetHandle As IntPtr = Nothing) _
    Implements ISlideShowModul.StartModul
        'Initialisiert und startet das eigentliche Modul

        'Instanz des aktuell laufenden Moduls bereitstellen
        activeModuleInstanz = Me

        'Eine möglicherweise noch vorhandene Fensterreferenz
        'darf nicht wiederverwendet werden.
        If sssScreen IsNot Nothing Then

            Try

                RemoveHandler sssScreen.DarstellungIstBereit, AddressOf SssScreen_DarstellungIstBereit
                RemoveHandler sssScreen.Closed, AddressOf SssScreen_Closed

                sssScreen.Close()

            Catch ex As Exception

                'Eine bereits geschlossene WPF-Window-Instanz
                'ist ohnehin nicht mehr verwendbar.

            Finally

                sssScreen = Nothing

            End Try

        End If

        Try

            'Bei jedem Start eine neue WPF-Window-Instanz erzeugen
            sssScreen = New wpfModulMain()

            AddHandler sssScreen.DarstellungIstBereit, AddressOf SssScreen_DarstellungIstBereit
            AddHandler sssScreen.Closed, AddressOf SssScreen_Closed

            'Settings einlesen und Initialisierungen durchführen
            CheckYourSettings()

            'Modul anzeigen
            sssScreen.WindowState = System.Windows.WindowState.Maximized

            sssScreen.Topmost = True

            sssScreen.Show()
            sssScreen.Focus()

            RaiseEvent ModulStateChanged("Running")

        Catch ex As Exception

            LogHandling.LogError("Modul """ & ModulName & """ konnte nicht gestartet werden: " & ex.ToString())

            If sssScreen IsNot Nothing Then

                Try

                    RemoveHandler sssScreen.Closed,
                    AddressOf SssScreen_Closed

                    sssScreen.Close()

                Catch
                    'Keine weitere Behandlung notwendig
                Finally
                    sssScreen = Nothing
                End Try

            End If

            activeModuleInstanz = Nothing

            RaiseEvent ModulStateChanged("Error")

            Throw

        End Try

    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Räumt auf und meldet dass der Schoner ordnungsgemäß beendet wurde

        If sssInfo IsNot Nothing Then

            Try
                sssInfo.Close()
                sssInfo.Dispose()
            Finally
                sssInfo = Nothing
            End Try

        End If

        If sssPause IsNot Nothing Then

            Try
                sssPause.Close()
                sssPause.Dispose()
            Finally
                sssPause = Nothing
            End Try

        End If

        If sssScreen IsNot Nothing Then

            Try

                RemoveHandler sssScreen.DarstellungIstBereit, AddressOf SssScreen_DarstellungIstBereit
                RemoveHandler sssScreen.Closed, AddressOf SssScreen_Closed

                sssScreen.Close()

            Catch ex As Exception

                LogHandling.LogError("Fehler beim Schließen des Modulfensters """ & ModulName & """: " & ex.ToString())

            Finally

                sssScreen = Nothing

            End Try

        End If

        activeModuleInstanz = Nothing
        pauseIsActive = False

        RaiseEvent ModulStateChanged("Stopped")

    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        'Startet den Pause-Modus des Moduls

        If sssScreen Is Nothing Then
            Exit Sub
        End If

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

            pauseIsActive = False

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

        Dim sekunden As Integer

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

    End Sub

    'Private Methoden
    Private Sub SssScreen_DarstellungIstBereit()
        'Leitet die WPF-Darstellungsbereitschaft an das Framework weiter.

        RaiseEvent ModulIstDarstellungsbereit()

    End Sub

    Public Shared Function GetModulDefaultSettings() As Dictionary(Of String, String)
        Dim defaultModulSettings As New Dictionary(Of String, String)
        'Liefert die Default-Werte des Moduls

        defaultModulSettings("Bildauswahl") = "Zufallsbild"
        defaultModulSettings("Präsentationsschirm") = "True"
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

        'Präsentationsschirm
        aktuelleSettings.Präsentationsschirm = CBool(ReadFromRegOrDefaults(SLIDESHOWMODUL_SSS_FULLPATH & "Präsentationsschirm", defaults))

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

    Private Sub SssScreen_Closed(
    sender As Object,
    e As EventArgs)
        'Entfernt die Referenz auch dann,
        'wenn sich das WPF-Fenster selbst geschlossen hat.

        Dim geschlossenesFenster As wpfModulMain

        geschlossenesFenster =
            TryCast(sender, wpfModulMain)

        If geschlossenesFenster IsNot Nothing Then

            RemoveHandler geschlossenesFenster.Closed,
                AddressOf SssScreen_Closed

        End If

        If ReferenceEquals(
            sssScreen,
            geschlossenesFenster) Then

            sssScreen = Nothing

        End If

    End Sub

End Class
