Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowTools
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowMain.SaverMain
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowLogging
Imports SlideShowLoader
Imports SlideShowBildauswahl
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.SharedDataHandling


Public Class frmSaverMain
    'Variablendeklarationen 

    'Variablen für die Start-Transition
    Private WithEvents einmaligeTransition As ISlideShowTransition
    Private transitionBeendet As New Threading.ManualResetEventSlim(False)
    Private startScreen As Image
    Private startLogo As Image
    Private picMain As PictureBox
    Private letzteTransition As String
    Private startTransition As String = Nothing

    'Default Main Settings
    Public defaults As Dictionary(Of String, String) = GetMainDefaultSettings()

    Private Sub frmSaverMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Bereitet das MCP zum Start vor

        'Key & Mauseingaben werden von den Modulen weitergeleitet und NICHT über die Form-Events abgehandelt.
        AddHandler GlobalKeyDown, AddressOf HandleGlobalKeyDown
        AddHandler GlobalMouseDown, AddressOf HandleGlobalMouseDown

        'Aussehen und Verhalten
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.TopMost = False
        Me.ShowInTaskbar = False
        If CursorHandling.IsCursorVisible() AndAlso Not optionsDialogIsActive Then
            Cursor.Hide()
        End If

        'Settings einlesen
        IniAndReinitialize()
        LegitimeListeErstellen()
        BildauswahlMain.CheckYourSettings() 'Bildauswahl Bescheid geben, dass es losgeht

        'Transition vorbereiten
        listOfEnabledTransitions = SplitSemicolonList(ReadFromRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionListe"))

        If listOfEnabledTransitions.Count > 0 Then
            listOfAvailableTransitions = TransitionListLoader.LadeTransitionInfoListe()
            letzteTransition = ReadFromRegistry(SLIDESHOWMAIN_PATH & "LetztgespieleTransition")

            Select Case ReadFromRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge")
                Case "Zufällig bei Start"
                    Do
                        startTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                    Loop Until listOfAvailableTransitions.Any(Function(t) t.TransitionName = startTransition)
                Case "In Reihenfolge bei Start"
                    startTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, letzteTransition)
            End Select

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

        'MCP sagt "Hallo"
        LogHandling.LogInfo("MCP wurde gestartet.")

        'Auswahl des ersten Moduls (oder des FallbackSavers)
        If listOfEnabledModules.Count > 0 Then
            If modulReihenfolge = "In Reihenfolge" Then
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

        'Falls eine Transition ausgewählt ist, vor dem Start des ersten Moduls Start-Transition zeigen.
        If startTransition IsNot Nothing Then
            picMainGFX = Graphics.FromHwnd(picMain.Handle)
            ZeigeEinmaligeStartTransition(StartBild, startLogo, picMainGFX, startTransition)
            If transitionReihenfolge = "In Reihenfolge bei Start" Then
                WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", startTransition)
            End If
        End If

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

        Else ' Alle anderen Tasten - Den Saver beenden

            closeSlideShowSaver()

        End If
    End Sub

    Private Sub HandleGlobalMouseDown(sender As Object, e As MouseEventArgs)
        'Behandlung von Mouse-Buttons

        If isInputLocked Then Exit Sub

        If e.Button = MouseButtons.Left Then

            closeSlideShowSaver()

        ElseIf e.Button = MouseButtons.Right Then

            openOptionsDialog()

        End If

    End Sub

    Private Sub IniAndReinitialize()
        '(Re-)Initialisieren gemäß Settings

        'Basisdaten auslesen
        modulDauer = CInt(ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulDauer", defaults))
        modulReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulReihenfolge", defaults)

        If modulReihenfolge <> "Zufällig bei Start" AndAlso listOfEnabledModules.Count > 1 Then
            'Main Loop gemäß ModulDauer in Minuten setzten & Starten
            tmrMain.Interval = modulDauer * 60 * 1000
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

            If modulReihenfolge = "In Reihenfolge" Then
                neuesModul = ListHandling.GetNextAlphabeticItemName(listOfEnabledModules, activeModule.ModulName.ToString)
            Else
                'entspricht automatisch "Zufällig" und "Zufällig bei Start", da letzteres den trmMain
                'ja eh schon (in IniAndReinitialize()) abschaltet .
                neuesModul = ListHandling.GetRandomItemFromList(Of String)(listOfEnabledModules)
            End If

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

    Private Sub ZeigeEinmaligeStartTransition(startBild As Image, zielBild As Image, g As Graphics, transition As String)

        ' Transition initialisieren
        einmaligeTransition = TransitionByNameLoader.LadeTransitionNachName(transition)
        AddHandler einmaligeTransition.TransitionIsRunning, AddressOf TransitionEinmalBeendet

        ' Starten
        einmaligeTransition.RunTransition(startBild, PictureBoxSizeMode.Zoom, zielBild,
         PictureBoxSizeMode.CenterImage, g,
        picMain.ClientSize, 3000)

        ' Auf Beendigung warten (max. 5 Sekunden als Sicherheit)
        transitionBeendet.Wait(5000)

        ' Aufräumen
        RemoveHandler einmaligeTransition.TransitionIsRunning, AddressOf TransitionEinmalBeendet
        einmaligeTransition = Nothing
        transitionBeendet.Reset()

    End Sub

    Private Sub TransitionEinmalBeendet(state As Boolean)
        If state = False Then
            transitionBeendet.Set()
        End If
    End Sub

End Class
