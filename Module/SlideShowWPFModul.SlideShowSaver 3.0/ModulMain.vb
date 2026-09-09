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
    Private aktuelleSettings As SettingsModul_SSS
    Public Const SLIDESHOWMODUL_SSS_FULLPATH As String = SLIDESHOWMODULBASE_PATH & "SlideShowSaver 3.0\"
    Public Const nameModul As String = "SlideShowSaver 3.0"

    'Fenster
    Friend Property sssScreen As wpfModulMain
    Friend Property sssPause As frmPauseModusOverlay
    Friend Property sssInfo As frmPictureInfo

    'Statusvariablen
    Private modulWurdeGestoppt As Boolean
    Private wurdeDisposed As Boolean
    Private pauseIsActive As Boolean

    Friend ReadOnly Property WirdBeendet As Boolean

        Get
            Return modulWurdeGestoppt OrElse
               wurdeDisposed
        End Get

    End Property

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
#End Region

    'Start/Stop/Pause
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False,
                          Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        'Initialisiert und startet eine neue Darstellung dieser Modulinstanz.

        ThrowIfDisposed()

        If sssScreen IsNot Nothing Then

            Throw New InvalidOperationException("Das Modul """ & ModulName & """ wurde bereits gestartet.")

        End If

        modulWurdeGestoppt = False
        pauseIsActive = False

        Try

            sssScreen = New wpfModulMain(Me)

            AddHandler sssScreen.DarstellungIstBereit, AddressOf SssScreen_DarstellungIstBereit
            AddHandler sssScreen.Closed, AddressOf SssScreen_Closed

            CheckYourSettings()

            sssScreen.WindowState = System.Windows.WindowState.Maximized
            sssScreen.Topmost = True

            sssScreen.Show()
            sssScreen.Focus()

            RaiseEvent ModulStateChanged("Running")

        Catch ex As Exception

            LogHandling.LogError("Modul """ & ModulName & """ konnte nicht gestartet werden: " & ex.ToString())

            BereinigeModulfenster()

            RaiseEvent ModulStateChanged("Error")

            Throw

        End Try

    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Beendet sämtliche aktive Arbeit der Modulinstanz.

        If modulWurdeGestoppt Then
            Exit Sub
        End If

        modulWurdeGestoppt = True

        BereinigePausefenster()
        BereinigeInfofenster()
        BereinigeModulfenster()

        pauseIsActive = False

        RaiseEvent ModulStateChanged("Stopped")

    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        'Startet den Pausemodus dieser Modulinstanz.

        Dim dialogErgebnis As DialogResult

        ThrowIfDisposed()

        If sssScreen Is Nothing OrElse pauseIsActive Then

            Exit Sub

        End If

        RaiseEvent ModulStateChanged("Pause")

        sssScreen.PausiereDarstellung()

        BereinigePausefenster()

        sssPause = New frmPauseModusOverlay(Me)

        pauseIsActive = True
        dialogErgebnis = DialogResult.None

        Try

            dialogErgebnis = sssPause.ShowDialog()

        Finally

            BereinigePausefenster()

        End Try

        If dialogErgebnis = DialogResult.OK AndAlso sssScreen IsNot Nothing Then

            CheckYourSettings()

            RaiseEvent ModulStateChanged("Running")

        End If

    End Sub

    'Optionen & OptionsDialog
    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog

        ThrowIfDisposed()

        ReadModulSettingsFromRegistryOrDefaults()

        StoreSettings(ModulName, aktuelleSettings)

        Return New ucOptionsModul()

    End Function

    'Info-Kommunikation
    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        '(Re-)Initialisiert die Moduloptionen nach einer Settingsänderung.

        ThrowIfDisposed()

        ReadModulSettingsFromRegistryOrDefaults()
        StoreSettings(nameModul, aktuelleSettings)

        If sssScreen IsNot Nothing Then

            sssScreen.AktualisiereSettings(aktuelleSettings)

        End If

        'Basierend auf den aktuellen Settings frmPictureInfo ein- oder ausblenden.
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

    Private Sub SssScreen_Closed(sender As Object, e As EventArgs)
        'Löst sämtliche Modulreferenzen auf ein selbstständig
        'geschlossenes WPF-Darstellungsfenster.

        Dim geschlossenesFenster As wpfModulMain

        geschlossenesFenster = TryCast(sender, wpfModulMain)

        If geschlossenesFenster Is Nothing Then
            Exit Sub
        End If

        RemoveHandler geschlossenesFenster.DarstellungIstBereit, AddressOf SssScreen_DarstellungIstBereit
        RemoveHandler geschlossenesFenster.Closed, AddressOf SssScreen_Closed

        Try

            geschlossenesFenster.BereinigeRessourcen()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim nachträglichen Bereinigen des Modulfensters: " & ex.ToString())

        End Try

        If ReferenceEquals(sssScreen, geschlossenesFenster) Then

            sssScreen = Nothing

        End If

    End Sub

    'Bereinigen und Dispose
    Public Sub Dispose() Implements IDisposable.Dispose
        'Gibt die Modulinstanz endgültig frei.

        If wurdeDisposed Then
            Exit Sub
        End If

        Try

            StopModul()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Stoppen des Moduls während Dispose: " & ex.ToString())

        Finally

            aktuelleSettings.Transitionseffekte = Nothing
            aktuelleSettings.Shader = Nothing

            wurdeDisposed = True

            GC.SuppressFinalize(Me)

        End Try

    End Sub

    Private Sub ThrowIfDisposed()
        'Verhindert die Wiederverwendung einer endgültig
        'freigegebenen Modulinstanz.

        If wurdeDisposed Then

            Throw New ObjectDisposedException(
                Me.GetType().FullName)

        End If

    End Sub

    Private Sub BereinigeInfofenster()
        'Schließt und disposed das Bildinformationsfenster.

        If sssInfo Is Nothing Then
            Exit Sub
        End If

        Try

            sssInfo.Close()

        Catch ex As Exception

            LogHandling.LogWarn("Das Bildinformationsfenster konnte nicht geschlossen werden: " & ex.Message)

        End Try

        Try

            sssInfo.Dispose()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Freigeben des Bildinformationsfensters: " & ex.ToString())

        Finally

            sssInfo = Nothing

        End Try

    End Sub

    Private Sub BereinigePausefenster()
        'Schließt und disposed das Pausefenster.

        If sssPause Is Nothing Then
            Exit Sub
        End If

        Try

            sssPause.Close()

        Catch ex As Exception

            LogHandling.LogWarn("Das Pausefenster konnte nicht geschlossen werden: " & ex.Message)

        End Try

        Try

            sssPause.Dispose()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Freigeben des Pausefensters: " & ex.ToString())

        Finally

            sssPause = Nothing
            pauseIsActive = False

        End Try

    End Sub

    Private Sub BereinigeModulfenster()
        'Entfernt Frameworkhandler, bereinigt den Fensterinhalt
        'und schließt das WPF-Modulfenster.

        Dim zuBereinigendesFenster As wpfModulMain

        zuBereinigendesFenster = sssScreen

        If zuBereinigendesFenster Is Nothing Then
            Exit Sub
        End If

        'Die Modulreferenz zuerst lösen. Der Closed-Handler darf danach
        'nicht nochmals dieselbe Instanz als aktives Fenster behandeln.
        sssScreen = Nothing

        Try

            RemoveHandler zuBereinigendesFenster.DarstellungIstBereit, AddressOf SssScreen_DarstellungIstBereit
            RemoveHandler zuBereinigendesFenster.Closed, AddressOf SssScreen_Closed

        Catch ex As Exception

            LogHandling.LogWarn("Fensterhandler des Moduls """ & ModulName &
                                """ konnten nicht vollständig entfernt werden: " & ex.Message)

        End Try

        Try

            zuBereinigendesFenster.BereinigeRessourcen()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Bereinigen des Modulfensters """ & ModulName & """: " & ex.ToString())

        End Try

        Try

            zuBereinigendesFenster.Close()

        Catch ex As Exception

            LogHandling.LogWarn("Das Modulfenster """ & ModulName & """ konnte nicht geschlossen werden: " &
                                ex.Message)

        End Try

    End Sub

End Class
