Imports SlideShowBildauswahl
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLoader
Imports SlideShowLogging
Imports SlideShowTools
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ScreenHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling

Public Class frmSaverMain
#Region "Variablendeklaration"
    'Variablendeklarationen 

    'Variablen für die Start-Transition
    Private WithEvents einmaligeTransition As ISlideShowTransition
    Private einmaligeTransitionName As String = Nothing
    Private transitionBeendet As New Threading.ManualResetEventSlim(False)
    Private startScreen As Image
    Private startLogo As Image
    Private picMain As PictureBox
    Private letzteTransition As String

    'Main Settings
    Private aktuelleSettings As SettingsMain

#End Region

    Private Sub frmSaverMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Bereitet das MCP zum Start vor

        'Key & Mauseingaben werden von den Modulen weitergeleitet und NICHT über die Form-Events abgehandelt.
        AddHandler GlobalKeyDown, AddressOf HandleGlobalKeyDown
        AddHandler GlobalMouseDown, AddressOf HandleGlobalMouseDown

        'Aussehen und Verhalten
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.TopMost = False
        Me.ShowInTaskbar = False
        Me.WindowState = FormWindowState.Maximized

        If CursorHandling.IsCursorVisible() AndAlso Not optionsDialogIsActive Then
            Cursor.Hide()
        End If

        'Settings einlesen
        IniAndReinitialize()
        LegitimeListeErstellen()

        'Bildauswahl Bescheid geben, dass es losgeht
        BildauswahlMain.CheckYourSettings()

        'Transition vorbereiten
        listOfEnabledTransitions = aktuelleSettings.ModulTransitionListe

        If listOfEnabledTransitions.Count > 0 Then
            listOfAvailableTransitions = TransitionListLoader.LadeTransitionInfoListe()
            letzteTransition = ReadFromRegistry(SLIDESHOWMAIN_PATH & "LetztgespieleTransition")

            Select Case aktuelleSettings.ModulTransitionReihenfolge
                Case "Zufällig bei Start"
                    Do
                        einmaligeTransitionName = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                    Loop Until listOfAvailableTransitions.Any(Function(t) t.TransitionName = einmaligeTransitionName)
                Case "In Reihenfolge bei Start"
                    einmaligeTransitionName = GetNextAlphabeticItemName(listOfEnabledTransitions, letzteTransition)
            End Select

            picMain = New PictureBox
            picMain.Dock = DockStyle.Fill
            picMain.SizeMode = PictureBoxSizeMode.Zoom
            picMain.BackColor = Color.Black

            If Not StartBildWurdeVerwendet Then
                startScreen = StartBild
            Else
                startScreen = GetCurrentScreen()
            End If

            startLogo = My.Resources.Splashscreen

        End If


    End Sub

    Private Sub frmSaverMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'Das MCP anzeigen

        Dim erstesModul As String
        Dim picMainGFX As Graphics = Nothing
        Dim screenSize As New Size

        'MCP sagt "Hallo"
        LogHandling.LogInfo("MCP wurde gestartet.")

        'Auswahl des ersten Moduls (oder des FallbackSavers)
        If listOfEnabledModules.Count > 0 Then
            If aktuelleSettings.ModulReihenfolge = "In Reihenfolge" Then
                erstesModul = ReadFromRegistry(SLIDESHOWMAIN_PATH & "LetztgespieltesModul")
                erstesModul = ListHandling.GetNextAlphabeticItemName(listOfEnabledModules, erstesModul)
            Else
                erstesModul = ListHandling.GetRandomItemFromList(Of String)(listOfEnabledModules)
            End If

            AktuellesModulAuswählen(erstesModul)
        Else
            fallbackIsActive = True
            fallbackPaused = False
        End If

        'Label anzeigen
        screenSize = GetNativeScreenResolution()

        lblMCP.Left = (screenSize.Width - lblMCP.Width) \ 2
        lblMCP.Visible = True

        lblNameFramework.Left = (screenSize.Width - lblNameFramework.Width) \ 2
        lblNameFramework.Visible = True

        lblInitialisiere.Text = "Initialisiere Schoner-Modul: " & erstesModul
        lblInitialisiere.Top = (screenSize.Height - lblInitialisiere.Height) \ 2
        lblInitialisiere.Left = (screenSize.Width - lblInitialisiere.Width) \ 2
        lblInitialisiere.Visible = True

        'TODO: Falls eine Transition ausgewählt ist, vor dem Start des ersten Moduls Start-Transition zeigen.
        'If einmaligeTransitionName IsNot Nothing Then
        '
        'lblMCP.Visible = False
        'lblNameFramewok.Visible = False
        '    picMainGFX = Graphics.FromHwnd(picMain.Handle)
        '    ZeigeEinmaligeeinmaligeTransitionName(StartBild, startLogo, picMainGFX, einmaligeTransitionName)
        '    If transitionReihenfolge = "In Reihenfolge bei Start" Then
        '        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", einmaligeTransitionName)
        '    End If
        '
        'End If

        'Das gewählte Modul anzeigen.
        If fallbackIsActive Then
            fallbackInstanz.Show()
            LogHandling.LogInfo("Initiales Modul gestartet: Fallbacksaver")
        Else
            activeModule.StartModul(Screen.PrimaryScreen)
            LogHandling.LogInfo("Initiales Modul gestartet: " & activeModule.ModulName.ToString)
        End If

    End Sub

    Private Sub tmrMain_Tick(sender As Object, e As EventArgs) Handles tmrMain.Tick
        'Wechsel der Module

        WechselLogik()

    End Sub

    Private Sub HandleGlobalKeyDown(sender As Object, e As KeyEventArgs)
        'Behandlung von Tastatureingaben

        If isInputLocked Then Exit Sub

        'Tastaturereignisse
        If e.KeyCode = Keys.Escape Then

            openOptionsDialog()

        ElseIf e.KeyCode = Keys.P Then

            PauseModus()

        Else ' Alle anderen Tasten - Den Saver beenden

            '  closeSlideShowSaver()

        End If
    End Sub

    Private Sub HandleGlobalMouseDown(sender As Object, e As MouseEventArgs)
        'Behandlung von Mouse-Buttons

        If isInputLocked Then Exit Sub

        If e.Button = MouseButtons.Left Then

            closeSlideShowSaver()

        ElseIf e.Button = MouseButtons.Right Then

            openOptionsDialog()

        ElseIf e.Button = MouseButtons.Middle Then

            PauseModus()

        End If

    End Sub

    Private Sub IniAndReinitialize()
        '(Re-)Initialisieren gemäß Settings

        'Basisdaten auslesen und in SettingsInbox ablegen
        ReadMainSettingsFromRegistryOrDefaults()
        StoreSettings("Main", aktuelleSettings)

        'Den Modulen die Hintergrundfarbe bereitstellen
        HintergrundFarbeSaver = aktuelleSettings.Hintergrundfarbe

        If aktuelleSettings.ModulReihenfolge <> "Zufällig bei Start" Then
            'Main Loop gemäß ModulDauer in Minuten setzten & Starten
            tmrMain.Interval = aktuelleSettings.ModulDauer * 60 * 1000
            tmrMain.Start()
        Else
            tmrMain.Stop()
        End If

    End Sub

    Private Function aktivesModulLebensberechtigungPrüfen() As Boolean
        ' Prüft ob das aktuelle Modul noch in der Liste der aktiven Module ist

        If activeModule IsNot Nothing Then

            If listOfEnabledModules.Contains(activeModule.ModulName.ToString) Then
                Return True
            End If

        End If

        Return False 'Default-Wert bei gescheiterten Tests

    End Function

    Private Sub LegitimeListeErstellen()
        'Aktualisiert die Liste der Module, die das MCP aktuell anzeigen darf

        Dim enabledModulesRegVal As String
        Dim tempList As New List(Of String)

        ' Listen der Module und aktivierten Module neu Laden, gegeneinander abgleichen.
        listOfAvailableModules = ModulListLoader.LadeModulInfoListe()
        enabledModulesRegVal = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulAktivListe", defaults)

        tempList =
            enabledModulesRegVal.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(s) s.Trim()).
            Where(Function(name) listOfAvailableModules.Any(
                Function(modul) modul.ModulName.Equals(name, StringComparison.OrdinalIgnoreCase))).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(s) s).
            ToList()

        listOfEnabledModules = tempList

    End Sub

    Private Sub AktuellesModulAuswählen(neuesModul As String)
        ' Wählt das nächste abzuspielende Modul

        Try
            ' Neues Modul aus der Liste der aktivierten Module laden, starten und als aktives Modul setzen. Bei 
            ' Fehlschlag den FallbackSaver aktivieren.
            fallbackIsActive = False
            fallbackPaused = False
            activeModule = ModulByNameLoader.LadeModulNachName(neuesModul)
        Catch ex As Exception
            fallbackIsActive = True
            fallbackPaused = False
            LogHandling.LogError("Problem beim Laden eines neuen Moduls (" & neuesModul & ") aus SaverMain heraus: " & ex.ToString)
        End Try

    End Sub

    Private Sub WechselLogik()
        'Wechselt zwischen den Modulen

        Dim neuesModul As String

        If fallbackIsActive Then
            fallbackInstanz.Show()
            LogHandling.LogInfo("Neues Modul gestartet: Fallbacksaver")
        Else

            If aktuelleSettings.ModulReihenfolge = "In Reihenfolge" Then
                neuesModul = ListHandling.GetNextAlphabeticItemName(listOfEnabledModules, activeModule.ModulName.ToString)
            Else
                'entspricht automatisch "Zufällig" und "Zufällig bei Start", da letzteres den trmMain
                'ja eh schon (in IniAndReinitialize()) abschaltet .
                neuesModul = ListHandling.GetRandomItemFromList(Of String)(listOfEnabledModules)
            End If

            'Wechsel bei gleichem Modul nicht notwendig
            If neuesModul = activeModule.ModulName Then Exit Sub

            'Label aktualisieren
            lblInitialisiere.Text = "Initialisiere Schoner-Modul: " & neuesModul

            'Aktuelles Modul aufräumen
            If activeModule IsNot Nothing Then
                activeModule.StopModul()
                activeModule = Nothing
            End If

            AktuellesModulAuswählen(neuesModul)
            activeModule.StartModul(Screen.PrimaryScreen)
            LogHandling.LogInfo("Neues Modul gestartet: " & activeModule.ModulName.ToString)

        End If

    End Sub

    Private Sub openOptionsDialog()
        'Öffnet den Options-Dialog

        'Aktuelle Settings in die SettingsInbox stellen
        ReadMainSettingsFromRegistryOrDefaults()
        StoreSettings("Main", aktuelleSettings)

        Try
            If optionsDialog Is Nothing Then
                optionsDialog = New frmOptionsMain()
            End If
        Catch ex As Exception
            LogHandling.LogError("Der Optionsdialog konnte nicht geladen werden: " & ex.ToString)
        End Try

        'OptionsDialog aufrufen inklusive Prüfung, ob der Dialog über den OK-Button geschlossen wurde
        Application.DoEvents()
        Threading.Thread.Sleep(100)

        If optionsDialog.ShowDialog() = DialogResult.OK Then

            If optionsDialog IsNot Nothing Then
                optionsDialog.Dispose()
                optionsDialog = Nothing
            End If

            'Alles Re-Initialisieren
            IniAndReinitialize()
            LegitimeListeErstellen()

            If listOfEnabledModules.Count = 0 Then
                fallbackIsActive = True
                fallbackPaused = False
            ElseIf fallbackIsActive Then
                fallbackIsActive = False
                fallbackPaused = False
                fallbackInstanz.Close()
            End If

            'Und prüfen ob das aktuelle Modul weiterlaufen darf oder nicht
            If aktivesModulLebensberechtigungPrüfen() = False Then
                WechselLogik()
            End If

            'Der Bildauswahl und dem Modul befehlen, frische Settings zu laden
            BildauswahlMain.CheckYourSettings()
            If activeModule IsNot Nothing Then
                activeModule.CheckYourSettings()
            End If

            'Dem Fallbacksaver befehlen, seine Hintergrundfarbe anzupassen
            If fallbackIsActive Then
                fallbackInstanz.CheckYourMail()
            End If

        End If

    End Sub

    Private Sub PauseModus()
        'Startet den Pause-Modus des Moduls, so implementiert

        If fallbackIsActive Then
            'PauseModus für Fallbacksaver
            If fallbackPaused = False Then
                fallbackInstanz.tmrFallback.Stop()
                fallbackPaused = True
            Else
                fallbackInstanz.tmrFallback.Start()
                fallbackPaused = False
            End If
        Else
            'PauseModus für Modul aufrufen
            activeModule.PauseModusModul()
        End If

    End Sub

    Private Sub closeSlideShowSaver()
        '...und Schluss.
        CursorHandling.CursorPowerShow()

        ' Instanzen aufrämen
        If activeModule IsNot Nothing Then
            WriteToRegistry(SLIDESHOWMAIN_PATH & "LetztgespieltesModul", activeModule.ModulName.ToString)
            activeModule.StopModul()
            activeModule = Nothing
        End If

        If optionsDialog IsNot Nothing Then
            optionsDialog.Close()
            optionsDialog.Dispose()
            optionsDialog = Nothing
        End If

        If fallbackInstanz IsNot Nothing Then
            fallbackInstanz.Close()
            fallbackInstanz.Dispose()
            fallbackInstanz = Nothing
        End If

        LogHandling.LogInfo("Der Bildschirmschoner wurde beendet.")

        Application.Exit()

    End Sub

    Private Sub ZeigeEinmaligeeinmaligeTransitionName(startBild As Image, zielBild As Image, g As Graphics, transition As String)

        '' Transition initialisieren
        'einmaligeTransition = TransitionByNameLoader.LadeTransitionNachName(transition)
        'AddHandler einmaligeTransition.TransitionIsRunning, AddressOf TransitionEinmalBeendet

        '' Starten
        'einmaligeTransition.RunTransition(startBild, PictureBoxSizeMode.Zoom, zielBild,
        ' PictureBoxSizeMode.CenterImage, g,
        'picMain.ClientSize, 3000)

        '' Auf Beendigung warten (max. 5 Sekunden als Sicherheit)
        'transitionBeendet.Wait(5000)

        '' Aufräumen
        'RemoveHandler einmaligeTransition.TransitionIsRunning, AddressOf TransitionEinmalBeendet
        'einmaligeTransition = Nothing
        'transitionBeendet.Reset()
        'lblMCP.Visible = True
        'lblNameFramework.Visible = True

    End Sub

    Private Sub TransitionEinmalBeendet(state As Boolean)
        If state = False Then
            transitionBeendet.Set()
        End If
    End Sub

    Private Sub ReadMainSettingsFromRegistryOrDefaults()
        'Liest die aktuellenSettings aus der Registry oder setzt Default-Werte

        Dim tmpRegistryValues As String
        Dim defaults As Dictionary(Of String, String) = GetMainDefaultSettings()

        'ModulDauer
        aktuelleSettings.ModulDauer = CInt(ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulDauer", defaults))

        'ModulReihenfolge
        aktuelleSettings.ModulReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulReihenfolge", defaults)

        'Aktivierte Module
        tmpRegistryValues = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulAktivListe", defaults)
        aktuelleSettings.ModulAktivListe = SplitSemicolonList(tmpRegistryValues)

        'MultiMonitor
        aktuelleSettings.MultiMonitor = CBool(ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "MultiMonitor", defaults))

        'Modul-Transitions
        tmpRegistryValues = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulTransitionListe", defaults)
        aktuelleSettings.ModulTransitionListe = SplitSemicolonList(tmpRegistryValues)

        'Modul-TransitionsReihenfolge
        aktuelleSettings.ModulTransitionReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", defaults)

        'Hintergrundfarbve
        aktuelleSettings.Hintergrundfarbe = StringToColor(ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "Hintergrundfarbe", defaults))

    End Sub

End Class
