Imports System.Runtime.InteropServices
Imports SlideShowBildauswahl
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLoader
Imports SlideShowLogging
Imports SlideShowTools
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling
Imports SW = System.Windows
Imports SWC = System.Windows.Controls
Imports SWD = System.Windows.Threading
Imports SWI = System.Windows.Input
Imports SWM = System.Windows.Media
Imports SWMI = System.Windows.Media.Imaging

Module SaverMain

#Region "Variablendeklaration"

#Region "Strukturen und Enumerationen"
    Friend Structure SettingsMain
        Public ModulDauer As Integer
        Public ModulReihenfolge As String
        Public ModulAktivListe As List(Of String)
        Public MultiMonitor As Boolean
        Public ModulTransitionListe As List(Of String)
        Public ModulTransitionReihenfolge As String
        Public Hintergrundfarbe As Color
    End Structure

    Private Enum ModulLadeErgebnis
        ModulGeladen
        KeinWechselNotwendig
        FallbackAktiviert
    End Enum

    Private Enum FrameworkStartmodus
        Bildschirmschoner
        Konfiguration
        Vorschau
    End Enum

    Private Enum MCPWechselPhase
        Keine
        HostVorbereitung
        Transition
        Titelcard
        Modulstart
    End Enum

    Private Enum AktionNachModulwechsel
        Keine
        Optionsdialog
        Pause
    End Enum
#End Region

#Region "Framework-Settings"
    Private aktuelleSettings As SettingsMain
#End Region

#Region "Fallback-Saver"
    Private fallbackInstanz As frmFallbackSaver = Nothing
    Private fallbackIsActive As Boolean = False
    Private fallbackPaused As Boolean = False
#End Region

#Region "Module"
    Private listOfAvailableModules As List(Of SlideShowModulInfo)
    Private listOfEnabledModules As List(Of String)
    Friend activeModule As ISlideShowModul = Nothing
    Private nextModule As ISlideShowModul = Nothing
#End Region

#Region "Transitionen"
    Private listOfAvailableTransitions As List(Of SlideShowTransitionInfo)
    Private listOfEnabledTransitions As List(Of String)
    Private activeTransition As ISlideShowTransition = Nothing

    Private modulwechselTransitionName As String = Nothing
    Private letzteTransition As String = Nothing
#End Region

#Region "Optionsdialog und Eingabesteuerung"
    Private optionsDialog As frmOptionsMain = Nothing
    Friend optionsDialogIsActive As Boolean = False
    Friend isInputLocked As Boolean = False
    Private eingabesteuerungIstRegistriert As Boolean = False
#End Region

#Region "MCP-Status und Timer"

    Private WithEvents tmrMCP As New Timer()
    Private mcpWurdeGestartet As Boolean = False
    Private mcpStartIstRegistriert As Boolean = False
    Private modulwechselIstAktiv As Boolean = False
    Private shutdownWurdeGestartet As Boolean = False

    Private startBild As SWMI.BitmapImage = Nothing
    Private zielBild As SWMI.BitmapImage = Nothing
#End Region

#Region "TransitionHost"
    Private transitionHost As SW.Window = Nothing
    Private transitionHostGrid As SWC.Grid = Nothing
    Private transitionHostImage As SWC.Image = Nothing
    Private transitionHostWriteableBitmap As SWMI.WriteableBitmap = Nothing
    Private transitionFramePuffer() As Byte = Nothing
    Private transitionFrameStride As Integer = 0

    Private transitionIstAktiv As Boolean = False
    Private transitionIstVorbereitet As Boolean = False
    Private transitionHostWartetAufErstdarstellung As Boolean = False

    Private WithEvents tmrTitelcard As New Timer()

    Private aktuelleWechselPhase As MCPWechselPhase = MCPWechselPhase.Keine
    Private ausstehendeAktion As AktionNachModulwechsel = AktionNachModulwechsel.Keine

    Private vorbereiteterModulName As String = Nothing

    Private Const TITELCARD_ANZEIGEDAUER_MS As Integer = 3000
    Private Const TITELCARD_SCHRIFTGROESSE_PT As Double = 72.0
#End Region

#Region "DWM und native Fenstersteuerung"
    Private ReadOnly HWND_TOPMOST As New IntPtr(-1)

    Private Const SWP_NOSIZE As UInteger = &H1UI
    Private Const SWP_NOMOVE As UInteger = &H2UI
    Private Const SWP_NOACTIVATE As UInteger = &H10UI
    Private Const SWP_SHOWWINDOW As UInteger = &H40UI

    <DllImport("user32.dll", SetLastError:=True)>
    Private Function SetWindowPos(hWnd As IntPtr, hWndInsertAfter As IntPtr, x As Integer, y As Integer, cx As Integer,
                                    cy As Integer, uFlags As UInteger) As Boolean
    End Function

    <DllImport("dwmapi.dll", CharSet:=CharSet.Auto, SetLastError:=False)>
    Private Function DwmFlush() As Integer
    End Function

#End Region

    ' Sonstiges
    Private ReadOnly rnd As New Random()

#End Region

    'Funktionen und Prozeduren
#Region "Start und Initialisierung"

#Region "Programmeinstieg und Startmodus"

    Public Sub Main()
        Dim args As String()

        args = Environment.GetCommandLineArgs()

        'Exception Handling & Debugging
        LogHandling.SetMinimumLogLevel(LogLevel.DebugLevel)

        AddHandler Application.ThreadException, AddressOf ThreadExceptionHandler
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf UnhandledExceptionHandler

        'Hübsch machen
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        'Hallo sagen
        LogHandling.LogInfo("SlideShowSaver 3.0 Framework wurde gestartet")

        Try

            Select Case ErmittleStartmodus(args)

                Case FrameworkStartmodus.Konfiguration

                    optionsDialog = New frmOptionsMain()
                    Application.Run(optionsDialog)

                Case FrameworkStartmodus.Bildschirmschoner

                    AktualisiereStartBild()

                    fallbackInstanz = New frmFallbackSaver()

                    RegistriereEingabesteuerung()
                    RegistriereMCPStart()

                    Application.Run()

                Case FrameworkStartmodus.Vorschau

                    LogHandling.LogWarn("Der Vorschau-Modus ist noch nicht implementiert.")

            End Select

        Catch ex As Exception

            LogHandling.LogError("Unbehandelter Fehler in SaverMain.Main(): " & ex.ToString())

            Throw

        Finally

            EntferneMCPStart()
            EntferneEingabesteuerung()

            RemoveHandler Application.ThreadException,
            AddressOf ThreadExceptionHandler

            RemoveHandler AppDomain.CurrentDomain.UnhandledException,
            AddressOf UnhandledExceptionHandler

        End Try

    End Sub

    Private Function ErmittleStartmodus(args As String()) As FrameworkStartmodus
        'Ermittelt anhand der übergebenen Kommandozeilenparameter den gewünschten Startmodus.

        If args Is Nothing OrElse args.Length <= 1 Then
            Return FrameworkStartmodus.Bildschirmschoner
        End If

        Select Case args(1).ToLowerInvariant()

            Case "/c"
                Return FrameworkStartmodus.Konfiguration

            Case "/p"
                Return FrameworkStartmodus.Vorschau

            Case "/s"
                Return FrameworkStartmodus.Bildschirmschoner

            Case Else

                LogHandling.LogWarn("Unbekannter Startparameter """ & args(1) & """ - Starte Bildschirmschoner.")

                Return FrameworkStartmodus.Bildschirmschoner

        End Select

    End Function

#End Region

#Region "MCP-Start"

    Private Sub RegistriereMCPStart()
        'Registriert den einmaligen MCP-Start nach Beginn der Nachrichtenschleife.

        If mcpStartIstRegistriert Then
            Exit Sub
        End If

        AddHandler Application.Idle, AddressOf StarteMCPBeiLeerlauf

        mcpStartIstRegistriert = True

    End Sub

    Private Sub EntferneMCPStart()
        'Entfernt den noch ausstehenden MCP-Start.

        If Not mcpStartIstRegistriert Then
            Exit Sub
        End If

        RemoveHandler Application.Idle, AddressOf StarteMCPBeiLeerlauf

        mcpStartIstRegistriert = False

    End Sub

    Private Sub StarteMCPBeiLeerlauf(sender As Object, e As EventArgs)
        'Startet das MCP einmalig, sobald die Nachrichtenschleife aktiv ist.

        EntferneMCPStart()
        StarteMCP()

    End Sub

    Private Sub StarteMCP()
        'Initialisiert das MCP und startet das erste funktionsfähige Modul.

        Dim ladeErgebnis As ModulLadeErgebnis

        If mcpWurdeGestartet OrElse shutdownWurdeGestartet Then
            Exit Sub
        End If

        mcpWurdeGestartet = True

        LogHandling.LogInfo("MCP wurde gestartet.")

        If CursorHandling.IsCursorVisible() AndAlso Not optionsDialogIsActive Then

            Cursor.Hide()

        End If

        IniAndReinitialize()
        LegitimeListeErstellen()

        BildauswahlMain.CheckYourSettings()

        InitialisiereMCPTransitionen()

        ladeErgebnis = LadeInitialesModul()

        Select Case ladeErgebnis

            Case ModulLadeErgebnis.FallbackAktiviert

                fallbackInstanz.Show()

                LogHandling.LogInfo("Initiales Modul gestartet: FallbackSaver")

            Case ModulLadeErgebnis.ModulGeladen

                If nextModule Is Nothing Then

                    LogHandling.LogError("Das MCP meldete ein geladenes Initialmodul, aber nextModule ist Nothing.")

                    AktiviereFallbackSaver()
                    fallbackInstanz.Show()

                    Exit Select

                End If

                MCPModulwechsel(True)

        End Select

    End Sub

#End Region

#Region "Framework-Initialisierung"

    Private Sub IniAndReinitialize()
        '(Re-)Initialisieren der zentralen Framework-Daten gemäß Settings

        'Basisdaten auslesen
        ReadMainSettingsFromRegistryOrDefaults()

        'Aktuelle Settings für andere Framework-Komponenten bereitstellen
        StoreSettings("Main", aktuelleSettings)

        'Den Modulen die Hintergrundfarbe bereitstellen
        HintergrundFarbeSaver = aktuelleSettings.Hintergrundfarbe

    End Sub

#End Region

#Region "Transitionsinitialisierung"

    Private Sub InitialisiereMCPTransitionen()
        'Lädt die verfügbaren Transitionen und bestimmt die nächste Modultransition.

        Dim gueltigeTransitionen As List(Of String)

        listOfEnabledTransitions = aktuelleSettings.ModulTransitionListe

        listOfAvailableTransitions = TransitionListLoader.LadeTransitionInfoListe()

        modulwechselTransitionName = Nothing
        letzteTransition = Nothing

        If listOfEnabledTransitions Is Nothing OrElse listOfEnabledTransitions.Count = 0 Then

            Exit Sub

        End If

        If listOfAvailableTransitions Is Nothing OrElse listOfAvailableTransitions.Count = 0 Then

            Exit Sub

        End If

        gueltigeTransitionen = listOfEnabledTransitions.
                                    Where(
                                        Function(transitionName)
                                            Return listOfAvailableTransitions.Any(
                                                Function(transitionInfo)
                                                    Return transitionInfo.TransitionName.Equals(
                                                        transitionName,
                                                        StringComparison.OrdinalIgnoreCase)
                                                End Function)
                                        End Function).
                                    ToList()


        If gueltigeTransitionen.Count = 0 Then

            LogHandling.LogWarn("Keine der aktivierten Modultransitionen ist verfügbar.")

            Exit Sub

        End If

        letzteTransition = ReadFromRegistry(SLIDESHOWMAIN_PATH & "LetztgespieleTransition")

        Select Case aktuelleSettings.ModulTransitionReihenfolge

            Case "Zufällig bei Start"

                modulwechselTransitionName = gueltigeTransitionen(rnd.Next(gueltigeTransitionen.Count))

            Case "In Reihenfolge bei Start"

                modulwechselTransitionName = GetNextAlphabeticItemName(gueltigeTransitionen, letzteTransition)

            Case "Zufällig", "In Reihenfolge"

                'Die Transition wird bei jedem Modulwechsel neu bestimmt.
                modulwechselTransitionName = Nothing

            Case Else

                LogHandling.LogWarn(
        "Unbekannte Reihenfolge für Modultransitionen """ &
        aktuelleSettings.ModulTransitionReihenfolge &
        """. Es wird bei jedem Wechsel zufällig ausgewählt.")

                modulwechselTransitionName = Nothing

        End Select

    End Sub

#End Region

#End Region

#Region "MCP"

#Region "Modulauswahl und Laden"

    Private Sub LegitimeListeErstellen()
        'Aktualisiert die Liste der Module, die das Framework aktuell anzeigen darf

        Dim enabledModulesRegVal As String
        Dim tempList As New List(Of String)
        Dim mainDefaults As Dictionary(Of String, String)

        mainDefaults = GetMainDefaultSettings()

        'Verfügbare Module neu laden
        listOfAvailableModules = ModulListLoader.LadeModulInfoListe()

        'Konfigurierte aktive Module auslesen
        enabledModulesRegVal = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulAktivListe", mainDefaults)

        'Registryliste gegen tatsächlich verfügbare Module prüfen
        tempList = enabledModulesRegVal.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries).
            Select(
                Function(s)
                    Return s.Trim()
                End Function).
            Where(
                Function(name)
                    Return listOfAvailableModules.Any(
                        Function(modul)
                            Return modul.ModulName.Equals(
                                name,
                                StringComparison.OrdinalIgnoreCase)
                        End Function)
                End Function).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(
                Function(name)
                    Return name
                End Function).
            ToList()

        listOfEnabledModules = tempList

    End Sub

    Private Function IstAktuellesModulWeiterhinAktiviert() As Boolean
        'Prüft, ob das aktuell laufende Modul weiterhin in der Liste der aktivierten Module enthalten ist.

        If activeModule Is Nothing Then
            Return False
        End If

        If listOfEnabledModules Is Nothing Then
            Return False
        End If

        Return listOfEnabledModules.Any(
            Function(modulName)
                Return modulName.Equals(
                    activeModule.ModulName,
                    StringComparison.OrdinalIgnoreCase)
            End Function)

    End Function

    Private Function ErmittleModulKandidaten(vorherigesModul As String, aktuellesModulAusschliessen As Boolean) As List(Of String)
        'Erstellt eine vollständige Liste möglicher Module in der Reihenfolge, in der Ladeversuche erfolgen sollen.

        Dim kandidaten As New List(Of String)
        Dim sortierteModule As List(Of String)
        Dim tempModulName As String
        Dim startIndex As Integer
        Dim index As Integer
        Dim i As Integer

        If listOfEnabledModules Is Nothing OrElse listOfEnabledModules.Count = 0 Then

            Return kandidaten

        End If

        sortierteModule = listOfEnabledModules.
                Where(
                    Function(modulName)
                        Return Not String.IsNullOrWhiteSpace(modulName)
                    End Function).
                Distinct(StringComparer.OrdinalIgnoreCase).
                OrderBy(
                    Function(modulName)
                        Return modulName
                    End Function).
                ToList()

        If sortierteModule.Count = 0 Then
            Return kandidaten
        End If

        Select Case aktuelleSettings.ModulReihenfolge

            Case "In Reihenfolge", "In Reihenfolge bei Start"

                startIndex = -1

                If Not String.IsNullOrWhiteSpace(vorherigesModul) Then

                    startIndex =
                    sortierteModule.FindIndex(
                        Function(modulName)
                            Return modulName.Equals(
                                vorherigesModul,
                                StringComparison.OrdinalIgnoreCase)
                        End Function)

                End If

                If startIndex < 0 Then

                    kandidaten.AddRange(sortierteModule)

                Else

                    For i = 1 To sortierteModule.Count

                        index = (startIndex + i) Mod sortierteModule.Count

                        kandidaten.Add(sortierteModule(index))

                    Next

                End If

            Case "Zufällig", "Zufällig bei Start"

                kandidaten.AddRange(sortierteModule)

                'Fisher-Yates-Mischung
                For i = kandidaten.Count - 1 To 1 Step -1

                    index = rnd.Next(i + 1)

                    tempModulName = kandidaten(i)
                    kandidaten(i) = kandidaten(index)
                    kandidaten(index) = tempModulName

                Next

            Case Else

                LogHandling.LogWarn("Unbekannte Modulreihenfolge """ & aktuelleSettings.ModulReihenfolge &
                                    """. Es wird eine zufällige Reihenfolge verwendet.")

                kandidaten.AddRange(sortierteModule)

                For i = kandidaten.Count - 1 To 1 Step -1

                    index = rnd.Next(i + 1)

                    tempModulName = kandidaten(i)
                    kandidaten(i) = kandidaten(index)
                    kandidaten(index) = tempModulName

                Next

        End Select

        If aktuellesModulAusschliessen AndAlso Not String.IsNullOrWhiteSpace(vorherigesModul) Then

            kandidaten.RemoveAll(
            Function(modulName)
                Return modulName.Equals(
                    vorherigesModul,
                    StringComparison.OrdinalIgnoreCase)
            End Function)

        End If

        Return kandidaten

    End Function

    Private Function VersucheModulZuLaden(modulName As String) As ISlideShowModul
        'Versucht, eine Modulinstanz zu laden.Bei einem Fehler wird Nothing zurückgegeben.

        Dim geladenesModul As ISlideShowModul

        geladenesModul = Nothing

        If String.IsNullOrWhiteSpace(modulName) Then
            Return Nothing
        End If

        Try

            geladenesModul = ModulByNameLoader.LadeModulNachName(modulName)

            If geladenesModul Is Nothing Then

                LogHandling.LogError("Das Modul """ & modulName & """ konnte nicht geladen werden. Der Loader lieferte Nothing zurück.")

            End If

        Catch ex As Exception

            geladenesModul = Nothing

            LogHandling.LogError("Problem beim Laden des Moduls """ & modulName & """: " & ex.ToString())

        End Try

        Return geladenesModul

    End Function

    Private Function LadeInitialesModul() As ModulLadeErgebnis
        'Versucht, das erste funktionsfähige Modul zu laden. Der FallbackSaver wird erst verwendet,
        'wenn sämtliche legitimen Module gescheitert sind.

        Dim kandidaten As List(Of String)
        Dim letztesModul As String
        Dim modulName As String
        Dim geladenesModul As ISlideShowModul

        activeModule = Nothing
        nextModule = Nothing

        fallbackIsActive = False
        fallbackPaused = False

        letztesModul = ReadFromRegistry(SLIDESHOWMAIN_PATH & "LetztgespieltesModul")

        kandidaten = ErmittleModulKandidaten(letztesModul, False)

        For Each modulName In kandidaten

            geladenesModul = VersucheModulZuLaden(modulName)

            If geladenesModul IsNot Nothing Then

                nextModule = geladenesModul

                fallbackIsActive = False
                fallbackPaused = False

                Return ModulLadeErgebnis.ModulGeladen

            End If

        Next

        AktiviereFallbackSaver()

        Return ModulLadeErgebnis.FallbackAktiviert

    End Function

    Private Function BereiteNaechstesModulVor() As ModulLadeErgebnis
        'Versucht, ein nächstes funktionsfähiges Modul zu laden, ohne das aktuell laufende Modul bereits zu beenden.

        Dim kandidaten As List(Of String)
        Dim aktuellerModulName As String
        Dim modulName As String
        Dim geladenesModul As ISlideShowModul

        aktuellerModulName = Nothing
        nextModule = Nothing

        If activeModule IsNot Nothing Then
            aktuellerModulName = activeModule.ModulName
        End If

        kandidaten = ErmittleModulKandidaten(aktuellerModulName, True)

        If kandidaten.Count = 0 Then

            If IstAktuellesModulWeiterhinAktiviert() Then
                Return ModulLadeErgebnis.KeinWechselNotwendig
            End If

            AktiviereFallbackSaver()

            Return ModulLadeErgebnis.FallbackAktiviert

        End If

        For Each modulName In kandidaten

            geladenesModul = VersucheModulZuLaden(modulName)

            If geladenesModul IsNot Nothing Then

                nextModule = geladenesModul

                Return ModulLadeErgebnis.ModulGeladen

            End If

        Next

        'Läuft noch ein gültiges Modul, darf dieses weiterlaufen.
        If IstAktuellesModulWeiterhinAktiviert() Then

            LogHandling.LogWarn("Kein alternatives Modul konnte geladen werden. Das aktuelle Modul läuft weiter.")

            Return ModulLadeErgebnis.KeinWechselNotwendig

        End If

        'Das bisherige Modul ist nicht mehr zulässig und kein Ersatzmodul konnte geladen werden.
        AktiviereFallbackSaver()

        Return ModulLadeErgebnis.FallbackAktiviert

    End Function

    Private Function UebernehmeVorbereitetesModul() As Boolean
        'Übernimmt das erfolgreich vorbereitete Modul als neues aktives Modul.

        If nextModule Is Nothing Then
            Return False
        End If

        activeModule = nextModule
        nextModule = Nothing

        fallbackIsActive = False
        fallbackPaused = False

        Return True

    End Function

#End Region

#Region "Fallback-Verwaltung"

    Private Sub AktiviereFallbackSaver()
        'Aktiviert den FallbackSaver, wenn kein reguläres Modul verwendet werden kann.

        activeModule = Nothing
        nextModule = Nothing

        fallbackIsActive = True
        fallbackPaused = False

        LogHandling.LogWarn("Keines der aktivierten Module konnte geladen werden. Der FallbackSaver wird verwendet.")

    End Sub

    Private Sub WechselZumFallbackSaver()
        'Wechselt kontrolliert zum FallbackSaver.

        AktualisiereStartBild()

        If activeModule IsNot Nothing Then

            activeModule.StopModul()
            activeModule = Nothing

        End If

        If fallbackInstanz IsNot Nothing Then
            fallbackInstanz.Show()
        End If

        LogHandling.LogInfo(
        "Neues Modul gestartet: FallbackSaver")

        ModulwechselAbschliessen()

    End Sub

#End Region

#Region "Start- und Zielbilder"

    Private Sub AktualisiereStartBild()
        'Erstellt das Ausgangsbild für die nächste Transition.

        Dim screenshot As Image
        Dim neuesStartBild As SWMI.BitmapImage

        screenshot = Nothing
        neuesStartBild = Nothing

        'Ein altes Startbild darf bei einem fehlgeschlagenen Screenshot
        'nicht unbemerkt erneut verwendet werden.
        startBild = Nothing

        Try

            screenshot = GetCurrentScreen()

            If screenshot Is Nothing Then

                LogHandling.LogWarn(
                "Für die nächste Transition konnte kein " &
                "Startbild erstellt werden.")

                Exit Sub

            End If

            neuesStartBild =
            ConvertImageToBitmapImage(screenshot)

            startBild = neuesStartBild

        Catch ex As Exception

            startBild = Nothing

            LogHandling.LogError(
            "Fehler beim Erstellen des Startbildes: " &
            ex.ToString())

        Finally

            If screenshot IsNot Nothing Then

                screenshot.Dispose()
                screenshot = Nothing

            End If

        End Try

    End Sub

    Private Sub ZielBildErstellen()
        'Erstellt die Titelcard des vorbereiteten Moduls vollständig in WPF.

        Dim bildschirmGrenzen As Rectangle
        Dim titelcardGrid As SWC.Grid
        Dim titelText As SWC.TextBlock
        Dim hintergrundFarbe As SWM.Color
        Dim textFarbe As SWM.Color
        Dim renderTarget As SWMI.RenderTargetBitmap
        Dim schriftgroesseDip As Double
        Dim modulName As String

        titelcardGrid = Nothing
        titelText = Nothing
        renderTarget = Nothing
        modulName = Nothing

        If nextModule Is Nothing Then

            LogHandling.LogWarn(
        "Die Titelcard konnte nicht erstellt werden, " &
        "weil nextModule Nothing ist.")

            zielBild = Nothing

            Exit Sub

        End If

        Try

            bildschirmGrenzen = Screen.PrimaryScreen.Bounds
            modulName = nextModule.ModulName

            hintergrundFarbe =
        SWM.Color.FromArgb(
        255,
        aktuelleSettings.Hintergrundfarbe.R,
        aktuelleSettings.Hintergrundfarbe.G,
        aktuelleSettings.Hintergrundfarbe.B)

            textFarbe =
        SWM.Color.FromArgb(
        255,
        CByte(255 - aktuelleSettings.Hintergrundfarbe.R),
        CByte(255 - aktuelleSettings.Hintergrundfarbe.G),
        CByte(255 - aktuelleSettings.Hintergrundfarbe.B))

            'WPF verwendet 96 geräteunabhängige Einheiten pro Zoll.
            'Ein typografischer Punkt entspricht 1/72 Zoll.
            schriftgroesseDip =
        TITELCARD_SCHRIFTGROESSE_PT * 96.0 / 72.0

            titelcardGrid = New SWC.Grid()
            titelcardGrid.Width = bildschirmGrenzen.Width
            titelcardGrid.Height = bildschirmGrenzen.Height
            titelcardGrid.Background =
        New SWM.SolidColorBrush(hintergrundFarbe)

            titelText = New SWC.TextBlock()
            titelText.Text = modulName
            titelText.FontFamily =
        New SWM.FontFamily("Segoe UI")
            titelText.FontSize = schriftgroesseDip
            titelText.FontWeight = SW.FontWeights.Bold
            titelText.Foreground =
        New SWM.SolidColorBrush(textFarbe)
            titelText.HorizontalAlignment =
        SW.HorizontalAlignment.Center
            titelText.VerticalAlignment =
        SW.VerticalAlignment.Center
            titelText.TextAlignment =
        SW.TextAlignment.Center
            titelText.TextWrapping =
        SW.TextWrapping.Wrap
            titelText.Margin =
        New SW.Thickness(80.0)

            titelcardGrid.Children.Add(titelText)

            titelcardGrid.Measure(
        New SW.Size(
        bildschirmGrenzen.Width,
        bildschirmGrenzen.Height))

            titelcardGrid.Arrange(
        New SW.Rect(
        0.0,
        0.0,
        bildschirmGrenzen.Width,
        bildschirmGrenzen.Height))

            titelcardGrid.UpdateLayout()

            renderTarget =
        New SWMI.RenderTargetBitmap(
        bildschirmGrenzen.Width,
        bildschirmGrenzen.Height,
        96.0,
        96.0,
        SWM.PixelFormats.Pbgra32)

            renderTarget.Render(titelcardGrid)
            renderTarget.Freeze()

            zielBild =
        ConvertRenderTargetBitmapToBitmapImage(renderTarget)

        Catch ex As Exception

            zielBild = Nothing

            LogHandling.LogError(
        "Fehler beim Erstellen der WPF-Titelcard für Modul """ &
        modulName & """: " &
        ex.ToString())

        End Try

    End Sub

#End Region

#Region "Transitionsauswahl und Ausführung"

    Private Function TransitionseffektAuswaehlen() As Boolean
        'Wählt für den aktuellen Modulwechsel eine verfügbare Transition aus und lädt sie.

        Dim gueltigeTransitionen As List(Of String)
        Dim ausgewaehlterName As String

        activeTransition = Nothing
        ausgewaehlterName = Nothing

        If listOfEnabledTransitions Is Nothing OrElse listOfEnabledTransitions.Count = 0 Then

            Return False

        End If

        If listOfAvailableTransitions Is Nothing OrElse listOfAvailableTransitions.Count = 0 Then

            Return False

        End If

        gueltigeTransitionen = listOfEnabledTransitions.
                                    Where(
                                        Function(transitionName)
                                            Return listOfAvailableTransitions.Any(
                                                Function(transitionInfo)
                                                    Return transitionInfo.TransitionName.Equals(
                                                    transitionName,
                                                    StringComparison.OrdinalIgnoreCase)
                                                End Function)
                                        End Function).
                                    ToList()

        If gueltigeTransitionen.Count = 0 Then
            Return False
        End If

        Select Case aktuelleSettings.ModulTransitionReihenfolge

            Case "Zufällig"

                ausgewaehlterName = gueltigeTransitionen(rnd.Next(gueltigeTransitionen.Count))

            Case "In Reihenfolge"

                ausgewaehlterName = GetNextAlphabeticItemName(gueltigeTransitionen, letzteTransition)

            Case "Zufällig bei Start", "In Reihenfolge bei Start"

                ausgewaehlterName = modulwechselTransitionName

            Case Else

                LogHandling.LogWarn("Unbekannte Reihenfolge für Modultransitionen """ &
                                    aktuelleSettings.ModulTransitionReihenfolge &
                                    """. Es wird zufällig ausgewählt.")

                ausgewaehlterName = gueltigeTransitionen(rnd.Next(gueltigeTransitionen.Count))

        End Select

        If String.IsNullOrWhiteSpace(ausgewaehlterName) OrElse
           Not gueltigeTransitionen.Any(
               Function(transitionName)
                   Return transitionName.Equals(
                   ausgewaehlterName,
                   StringComparison.OrdinalIgnoreCase)
               End Function) Then

            ausgewaehlterName = gueltigeTransitionen(0)

        End If

        Try

            activeTransition = TransitionByNameLoader.LadeTransitionNachName(ausgewaehlterName)

            If activeTransition Is Nothing Then

                LogHandling.LogWarn("Die Modultransition """ & ausgewaehlterName & """ konnte nicht geladen werden.")

                Return False

            End If

            If aktuelleSettings.ModulTransitionReihenfolge = "Zufällig bei Start" OrElse
                aktuelleSettings.ModulTransitionReihenfolge = "In Reihenfolge bei Start" Then

                modulwechselTransitionName = ausgewaehlterName

            End If

            Return True

        Catch ex As Exception

            activeTransition = Nothing

            LogHandling.LogError("Fehler beim Laden der Modultransition """ & ausgewaehlterName & """: " & ex.ToString())

            Return False

        End Try

    End Function

    Private Function TransitionseffektStarten() As Boolean
        'Startet die ausgewählte Transition und kehrt unmittelbar zurück.
        'Der weitere Ablauf erfolgt über TransitionIsRunning.

        Dim clientSize As System.Drawing.Size

        If activeTransition Is Nothing OrElse startBild Is Nothing OrElse zielBild Is Nothing Then

            Return False

        End If

        clientSize = New System.Drawing.Size(startBild.PixelWidth, startBild.PixelHeight)

        Try

            AddHandler activeTransition.TransitionFrameIstFertig, AddressOf TransitionFrameIstFertig
            AddHandler activeTransition.TransitionIsRunning, AddressOf TransitionIsRunning

            aktuelleWechselPhase = MCPWechselPhase.Transition

            transitionIstAktiv = True

            activeTransition.RunTransition(startBild, PictureBoxSizeMode.Zoom, zielBild, PictureBoxSizeMode.CenterImage,
                                            clientSize)

            Return True

        Catch ex As Exception

            transitionIstAktiv = False

            RemoveHandler activeTransition.TransitionFrameIstFertig, AddressOf TransitionFrameIstFertig
            RemoveHandler activeTransition.TransitionIsRunning, AddressOf TransitionIsRunning

            LogHandling.LogError("Fehler beim Starten der Modultransition: " & ex.ToString())

            Return False

        End Try

    End Function

    Private Sub TransitionIsRunning(state As Boolean)
        'Übernimmt den Laufstatus und setzt die Wechselpipeline
        'nach dem Ende der Transition asynchron fort.

        transitionIstAktiv = state

        If state Then
            Exit Sub
        End If

        TransitionAbgeschlossenVormerken()

    End Sub

    Private Sub TransitionAbgeschlossenVormerken()
        'Stellt die Fortsetzung hinter das aktuelle Renderereignis
        'in die Dispatcher-Warteschlange.

        If transitionHost Is Nothing Then

            TransitionAbgeschlossen()
            Exit Sub

        End If

        transitionHost.Dispatcher.BeginInvoke(
        SWD.DispatcherPriority.Background,
        New Action(
            AddressOf TransitionAbgeschlossen))

    End Sub

    Private Sub TransitionAbgeschlossen()
        'Beendet die Transitionphase und startet die Titelcardphase.

        If aktuelleWechselPhase <> MCPWechselPhase.Transition Then
            Exit Sub
        End If

        transitionIstAktiv = False

        If activeTransition IsNot Nothing Then

            RemoveHandler activeTransition.TransitionFrameIstFertig,
            AddressOf TransitionFrameIstFertig

            RemoveHandler activeTransition.TransitionIsRunning,
            AddressOf TransitionIsRunning

            letzteTransition =
            activeTransition.TransitionName

            WriteToRegistry(
            SLIDESHOWMAIN_PATH & "LetztgespieleTransition",
            letzteTransition)

        End If

        TitelcardPhaseStarten()

    End Sub

    Private Sub TransitionVorzeitigBeenden()
        'Beendet eine laufende Transition kontrolliert.
        'Die Wechselpipeline wird anschließend regulär fortgesetzt.

        If aktuelleWechselPhase <> MCPWechselPhase.Transition Then
            Exit Sub
        End If

        If activeTransition Is Nothing OrElse
       Not transitionIstAktiv Then

            Exit Sub

        End If

        Try

            activeTransition.StopTransition()

        Catch ex As Exception

            LogHandling.LogError(
            "Fehler beim vorzeitigen Beenden der Transition: " &
            ex.ToString())

            transitionIstAktiv = False
            TransitionAbgeschlossenVormerken()

        End Try

    End Sub

    Private Sub TransitionBereinigen()
        'Entfernt Eventhandler und gibt die Transitioninstanz frei.

        If activeTransition Is Nothing Then
            Exit Sub
        End If

        Try

            RemoveHandler activeTransition.TransitionFrameIstFertig, AddressOf TransitionFrameIstFertig
            RemoveHandler activeTransition.TransitionIsRunning, AddressOf TransitionIsRunning

            If TypeOf activeTransition Is IDisposable Then

                DirectCast(activeTransition, IDisposable).Dispose()

            End If

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Freigeben der Modultransition: " & ex.ToString())

        Finally

            activeTransition = Nothing
            transitionIstAktiv = False

        End Try

    End Sub

#End Region

#Region "TransitionHost-Erzeugung und Darstellung"

    Private Sub TransitionHostVorbereiten()
        'Erstellt einen rahmenlosen WPF-Vollbildhost für Transitionen
        'und Titelcards.

        Dim hintergrundFarbe As SWM.Color

        hintergrundFarbe =
                SWM.Color.FromArgb(
                255,
                aktuelleSettings.Hintergrundfarbe.R,
                aktuelleSettings.Hintergrundfarbe.G,
                aktuelleSettings.Hintergrundfarbe.B)

        If transitionHost Is Nothing Then

            transitionHost = New SW.Window()

            transitionHost.WindowStyle = SW.WindowStyle.None
            transitionHost.ResizeMode = SW.ResizeMode.NoResize
            transitionHost.WindowStartupLocation = SW.WindowStartupLocation.Manual

            transitionHost.ShowInTaskbar = False
            transitionHost.ShowActivated = False
            transitionHost.Topmost = True

            transitionHost.WindowState = SW.WindowState.Normal

            TransitionHostAufPrimaerbildschirmPositionieren()

            transitionHost.Background = New SWM.SolidColorBrush(hintergrundFarbe)

            transitionHostGrid = New SWC.Grid()
            transitionHostGrid.Background = New SWM.SolidColorBrush(hintergrundFarbe)

            transitionHostImage = New SWC.Image()
            transitionHostImage.Stretch = SWM.Stretch.Fill
            transitionHostImage.HorizontalAlignment = SW.HorizontalAlignment.Stretch
            transitionHostImage.VerticalAlignment = SW.VerticalAlignment.Stretch
            transitionHostImage.SnapsToDevicePixels = True

            SWM.RenderOptions.SetBitmapScalingMode(transitionHostImage, SWM.BitmapScalingMode.HighQuality)

            transitionHostGrid.Children.Add(transitionHostImage)
            transitionHost.Content = transitionHostGrid

            AddHandler transitionHost.ContentRendered, AddressOf TransitionHostContentRendered
            AddHandler transitionHost.Closed, AddressOf TransitionHostClosed

            AddHandler transitionHost.PreviewKeyDown, AddressOf TransitionHostPreviewKeyDown
            AddHandler transitionHost.PreviewMouseDown, AddressOf TransitionHostPreviewMouseDown

        End If

        transitionHost.Background = New SWM.SolidColorBrush(hintergrundFarbe)
        transitionHostGrid.Background = New SWM.SolidColorBrush(hintergrundFarbe)

        'Das Fenster wird hier nur vorbereitet.
        'Angezeigt wird es erst, nachdem eine gültige Bildquelle gesetzt wurde.

        transitionHost.Topmost = True

    End Sub

    Private Sub TransitionHostAnzeigen()
        'Zeigt den bereits vollständig vorbereiteten Host,
        'ohne einen Aktivierungs- oder Maximierungswechsel auszulösen.

        If transitionHost Is Nothing OrElse transitionHostImage Is Nothing OrElse transitionHostImage.Source Is Nothing Then

            Exit Sub

        End If

        Try

            If Not transitionHost.IsVisible Then

                'Die endgültige Geometrie wird unmittelbar vor Show() noch einmal gesetzt.
                TransitionHostAufPrimaerbildschirmPositionieren()

                transitionHost.Topmost = True
                transitionHost.Show()

                TransitionHostNachVorneSetzen()
                TransitionHostDarstellungErzwingen()

            End If

            transitionHost.Topmost = True
            TransitionHostDarstellungErzwingen()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Anzeigen des TransitionHosts: " & ex.ToString())

        End Try

    End Sub

    Private Sub TransitionHostAufPrimaerbildschirmPositionieren()
        'Positioniert den Host exakt auf dem primären Bildschirm.

        Dim bounds As Rectangle

        bounds = Screen.PrimaryScreen.Bounds

        transitionHost.WindowState = SW.WindowState.Normal
        transitionHost.Left = bounds.Left
        transitionHost.Top = bounds.Top
        transitionHost.Width = bounds.Width
        transitionHost.Height = bounds.Height

    End Sub

    Private Sub ZeigeBildImTransitionHost(bild As SWM.ImageSource)
        'Setzt das Bild gleichzeitig als Image.Source und als Hintergrund.
        'Dadurch zeigt bereits der erste native Fensterframe das Bild.

        Dim bildBrush As SWM.ImageBrush

        If bild Is Nothing Then
            Exit Sub
        End If

        bildBrush = Nothing

        TransitionHostVorbereiten()

        Try
            bildBrush = New SWM.ImageBrush(bild)
            bildBrush.Stretch = SWM.Stretch.Fill
            bildBrush.AlignmentX = SWM.AlignmentX.Center
            bildBrush.AlignmentY = SWM.AlignmentY.Center

            If bildBrush.CanFreeze Then
                bildBrush.Freeze()
            End If

            transitionHost.Background = bildBrush
            transitionHostGrid.Background = bildBrush

            transitionHostImage.Source = Nothing
            transitionHostWriteableBitmap = Nothing

            transitionHostImage.Source = bild
            transitionHostImage.InvalidateVisual()

            TransitionHostAnzeigen()

        Catch ex As Exception

            LogHandling.LogError(
            "Fehler beim Anzeigen eines statischen Transitionbildes: " &
            ex.ToString())

        End Try

    End Sub

    Private Sub TransitionFrameIstFertig(bitmap As SWMI.RenderTargetBitmap)
        'Kopiert den fertigen Transitionframe in einen dauerhaft
        'wiederverwendeten WPF-Framebuffer.

        Dim quellRechteck As SW.Int32Rect
        Dim benoetigteBytes As Integer
        Dim neuerStride As Integer

        If bitmap Is Nothing Then
            Exit Sub
        End If

        Try

            If transitionHost Is Nothing OrElse
           transitionHostImage Is Nothing Then

                TransitionHostVorbereiten()

            End If

            If transitionHostWriteableBitmap Is Nothing OrElse
           transitionHostWriteableBitmap.PixelWidth <> bitmap.PixelWidth OrElse
           transitionHostWriteableBitmap.PixelHeight <> bitmap.PixelHeight OrElse
           transitionHostWriteableBitmap.Format <> bitmap.Format Then

                transitionHostWriteableBitmap =
            New SWMI.WriteableBitmap(
            bitmap.PixelWidth,
            bitmap.PixelHeight,
            bitmap.DpiX,
            bitmap.DpiY,
            bitmap.Format,
            bitmap.Palette)

                transitionHostImage.Source =
            transitionHostWriteableBitmap

            End If

            neuerStride =
        CInt(
        Math.Ceiling(
        bitmap.PixelWidth *
        bitmap.Format.BitsPerPixel / 8.0))

            benoetigteBytes =
        neuerStride * bitmap.PixelHeight

            If transitionFramePuffer Is Nothing OrElse
           transitionFramePuffer.Length <> benoetigteBytes Then

                ReDim transitionFramePuffer(benoetigteBytes - 1)

            End If

            transitionFrameStride = neuerStride

            quellRechteck =
        New SW.Int32Rect(
        0,
        0,
        bitmap.PixelWidth,
        bitmap.PixelHeight)

            bitmap.CopyPixels(
        quellRechteck,
        transitionFramePuffer,
        transitionFrameStride,
        0)

            transitionHostWriteableBitmap.WritePixels(
        quellRechteck,
        transitionFramePuffer,
        transitionFrameStride,
        0)

        Catch ex As Exception

            LogHandling.LogError(
        "Fehler beim Anzeigen eines WPF-Transitionframes: " &
        ex.ToString())

        End Try

    End Sub

    Private Sub TransitionHostDarstellungErzwingen()
        'Veranlasst WPF, alle ausstehenden Layout- und Renderoperationen
        'des TransitionHosts vollständig abzuarbeiten.

        If transitionHost Is Nothing Then
            Exit Sub
        End If

        Try

            transitionHost.UpdateLayout()

            transitionHost.Dispatcher.Invoke(
        SWD.DispatcherPriority.Render,
        New Action(
        Sub()
        End Sub))

        Catch ex As Exception

            LogHandling.LogError(
        "Fehler beim Erzwingen der TransitionHost-Darstellung: " &
        ex.ToString())

        End Try

    End Sub

    Private Sub TransitionHostClosed(sender As Object, e As EventArgs)
        'Entfernt Referenzen auf einen geschlossenen WPF-Host.

        transitionHost = Nothing
        transitionHostGrid = Nothing
        transitionHostImage = Nothing

    End Sub

    Private Sub TransitionHostSchliessen()
        'Entfernt sämtliche Referenzen und schließt den WPF-TransitionHost.

        Try

            If activeTransition IsNot Nothing Then

                RemoveHandler activeTransition.TransitionFrameIstFertig, AddressOf TransitionFrameIstFertig
                RemoveHandler activeTransition.TransitionIsRunning, AddressOf TransitionIsRunning

            End If

            transitionIstAktiv = False

            If transitionHostImage IsNot Nothing Then

                transitionHostImage.Source = Nothing
                transitionHostImage.DataContext = Nothing

            End If

            If transitionHostGrid IsNot Nothing Then

                transitionHostGrid.Children.Clear()
                transitionHostGrid.DataContext = Nothing

            End If

            If transitionHost IsNot Nothing Then

                RemoveHandler transitionHost.ContentRendered, AddressOf TransitionHostContentRendered
                RemoveHandler transitionHost.Closed, AddressOf TransitionHostClosed

                RemoveHandler transitionHost.PreviewKeyDown, AddressOf TransitionHostPreviewKeyDown
                RemoveHandler transitionHost.PreviewMouseDown, AddressOf TransitionHostPreviewMouseDown

                transitionHost.Content = Nothing

                If transitionHost.IsVisible Then
                    transitionHost.Close()
                End If

            End If

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Schließen des WPF-TransitionHosts: " & ex.ToString())

        Finally

            transitionHostImage = Nothing
            transitionHostGrid = Nothing
            transitionHost = Nothing
            transitionHostWriteableBitmap = Nothing
            transitionFramePuffer = Nothing
            transitionFrameStride = 0

            startBild = Nothing
            zielBild = Nothing

        End Try

    End Sub

#End Region

#Region "DWM und Z-Order"

    Private Function TransitionHostDWMAnzeigeAbwarten() As Boolean
        'Wartet, bis der Desktop Window Manager die aktuelle Darstellung
        'des TransitionHosts tatsächlich verarbeitet hat.

        Dim ergebnis As Integer

        If transitionHost Is Nothing OrElse
       Not transitionHost.IsVisible Then

            Return False

        End If

        Try

            TransitionHostNachVorneSetzen()
            TransitionHostDarstellungErzwingen()

            ergebnis = DwmFlush()

            If ergebnis <> 0 Then

                LogHandling.LogWarn(
            "DwmFlush konnte die Darstellung des TransitionHosts " &
            "nicht bestätigen. HRESULT: " &
            ergebnis.ToString())

                Return False

            End If

            Return True

        Catch ex As Exception

            LogHandling.LogError(
        "Fehler beim Abwarten der DWM-Darstellung des " &
        "TransitionHosts: " &
        ex.ToString())

            Return False

        End Try

    End Function

    Private Function TransitionHostNachVorneSetzen() As Boolean
        'Setzt den TransitionHost nativ an die Spitze der TopMost-Gruppe,
        'ohne den Fokus zu übernehmen.

        Dim interopHelper As SW.Interop.WindowInteropHelper
        Dim fensterHandle As IntPtr
        Dim ergebnis As Boolean

        If transitionHost Is Nothing Then
            Return False
        End If

        Try

            interopHelper =
        New SW.Interop.WindowInteropHelper(transitionHost)

            fensterHandle = interopHelper.Handle

            If fensterHandle = IntPtr.Zero Then
                Return False
            End If

            ergebnis =
        SetWindowPos(
        fensterHandle,
        HWND_TOPMOST,
        0,
        0,
        0,
        0,
        SWP_NOMOVE Or
        SWP_NOSIZE Or
        SWP_NOACTIVATE Or
        SWP_SHOWWINDOW)

            Return ergebnis

        Catch ex As Exception

            LogHandling.LogError(
        "Fehler beim nativen Positionieren des TransitionHosts: " &
        ex.ToString())

            Return False

        End Try

    End Function

    Private Sub TransitionHostContentRendered(sender As Object, e As EventArgs)
        'Setzt die Modulwechselpipeline erst fort, nachdem der Host
        'von WPF gerendert und vom DWM verarbeitet wurde.

        If Not transitionHostWartetAufErstdarstellung Then
            Exit Sub
        End If

        transitionHostWartetAufErstdarstellung = False

        If transitionHost Is Nothing Then

            ModulwechselFehlgeschlagen()
            Exit Sub

        End If

        transitionHost.Dispatcher.BeginInvoke(
    SWD.DispatcherPriority.ContextIdle,
    New Action(
        AddressOf TransitionHostNachErstdarstellungFortsetzen))

    End Sub

    Private Sub TransitionHostNachErstdarstellungFortsetzen()
        'Erzeugt eine echte Präsentationsbarriere zwischen dem sichtbaren
        'TransitionHost und dem Beenden des bisherigen Moduls.

        If shutdownWurdeGestartet Then
            Exit Sub
        End If

        If aktuelleWechselPhase <>
       MCPWechselPhase.HostVorbereitung Then

            Exit Sub

        End If

        If Not TransitionHostDWMAnzeigeAbwarten() Then

            LogHandling.LogWarn(
        "Die DWM-Darstellung des TransitionHosts konnte nicht " &
        "eindeutig bestätigt werden. Der Modulwechsel wird dennoch " &
        "fortgesetzt.")

        End If

        ModulwechselNachHostDarstellungFortsetzen()

    End Sub

#End Region

#Region "Titelcard und Modulübergabe"

    Private Sub TitelcardPhaseStarten()
        'Zeigt die Titelcard und startet den nicht blockierenden Timer.

        aktuelleWechselPhase =
        MCPWechselPhase.Titelcard

        If zielBild IsNot Nothing Then
            ZeigeBildImTransitionHost(zielBild)
        End If

        tmrTitelcard.Stop()
        tmrTitelcard.Interval =
        TITELCARD_ANZEIGEDAUER_MS

        tmrTitelcard.Start()

    End Sub

    Private Sub tmrTitelcard_Tick(
    sender As Object,
    e As EventArgs) Handles tmrTitelcard.Tick
        'Beendet die Titelcardphase und startet das neue Modul.

        tmrTitelcard.Stop()

        NeuesModulStarten()

    End Sub

    Private Sub NeuesModulStarten()
        'Übernimmt und startet das vorbereitete Modul. Der TransitionHost bleibt bis zur bestätigten
        'Darstellungsbereitschaft sichtbar.

        aktuelleWechselPhase = MCPWechselPhase.Modulstart

        If shutdownWurdeGestartet Then
            Exit Sub
        End If

        If Not UebernehmeVorbereitetesModul() Then

            LogHandling.LogError("Das vorbereitete Modul konnte nicht übernommen werden.")

            ModulwechselFehlgeschlagen()

            Exit Sub

        End If

        Try

            AddHandler activeModule.ModulIstDarstellungsbereit, AddressOf ActiveModule_ModulIstDarstellungsbereit

            activeModule.StartModul(Screen.PrimaryScreen)

            If transitionHost IsNot Nothing Then

                transitionHost.Topmost = True
                TransitionHostNachVorneSetzen()

            End If

        Catch ex As Exception

            If activeModule IsNot Nothing Then

                RemoveHandler activeModule.ModulIstDarstellungsbereit, AddressOf ActiveModule_ModulIstDarstellungsbereit

            End If

            LogHandling.LogError("Fehler beim Starten des vorbereiteten Moduls: " & ex.ToString())

            ModulwechselFehlgeschlagen()

        End Try

    End Sub

    Private Sub ActiveModule_ModulIstDarstellungsbereit()
        'Übergibt die Darstellung erst nach bestätigtem
        'sichtbaren Modulzustand vom Host an das Modul.

        If aktuelleWechselPhase <> MCPWechselPhase.Modulstart Then

            Exit Sub

        End If

        If activeModule IsNot Nothing Then

            RemoveHandler activeModule.ModulIstDarstellungsbereit, AddressOf ActiveModule_ModulIstDarstellungsbereit

        End If

        If transitionHost IsNot Nothing Then

            transitionHost.Topmost = True
            TransitionHostNachVorneSetzen()
            TransitionHostDarstellungErzwingen()
            DwmFlush()

        End If

        TransitionHostSchliessen()

        LogHandling.LogInfo("Neues Modul gestartet: " & vorbereiteterModulName)

        ModulwechselAbschliessen()

    End Sub

#End Region

#Region "MCP-Modulwechsel"

    Private Sub tmrMCP_Tick(sender As Object, e As EventArgs) Handles tmrMCP.Tick
        'Fordert einen automatischen Modulwechsel an.

        MCPModulwechsel()

    End Sub

    Private Sub AktualisiereMCPTimer()
        'Aktualisiert den Timer für automatische Modulwechsel.

        If tmrMCP Is Nothing Then
            Exit Sub
        End If

        tmrMCP.Stop()

        Select Case aktuelleSettings.ModulReihenfolge

            Case "Zufällig", "In Reihenfolge"

                tmrMCP.Interval = aktuelleSettings.ModulDauer * 60 * 1000
                tmrMCP.Start()

            Case "Zufällig bei Start", "In Reihenfolge bei Start"
                'Kein automatischer Wechsel während der Laufzeit.

            Case Else

                LogHandling.LogWarn("Unbekannte Modulreihenfolge """ & aktuelleSettings.ModulReihenfolge &
                                    """. Der automatische Modulwechsel wurde deaktiviert.")

        End Select

    End Sub

    Private Sub MCPModulwechsel(Optional istInitialerWechsel As Boolean = False)
        'Bereitet einen Modulwechsel vor und startet anschließend
        'die ereignisgesteuerte Wechselpipeline.

        Dim ladeErgebnis As ModulLadeErgebnis

        If shutdownWurdeGestartet OrElse modulwechselIstAktiv Then

            Exit Sub

        End If

        modulwechselIstAktiv = True

        aktuelleWechselPhase = MCPWechselPhase.Keine
        vorbereiteterModulName = Nothing
        ausstehendeAktion = AktionNachModulwechsel.Keine

        tmrMCP.Stop()

        Try

            If istInitialerWechsel Then

                If nextModule Is Nothing Then

                    LogHandling.LogError("Für den initialen Modulwechsel ist kein vorbereitetes Modul vorhanden.")

                    ModulwechselFehlgeschlagen()

                    Exit Sub

                End If

                ladeErgebnis = ModulLadeErgebnis.ModulGeladen

            Else

                ladeErgebnis = BereiteNaechstesModulVor()

            End If

            Select Case ladeErgebnis

                Case ModulLadeErgebnis.KeinWechselNotwendig

                    ModulwechselAbschliessen()

                    Exit Sub

                Case ModulLadeErgebnis.FallbackAktiviert

                    WechselZumFallbackSaver()

                    Exit Sub

                Case ModulLadeErgebnis.ModulGeladen

                    If nextModule Is Nothing Then

                        LogHandling.LogError("Das MCP meldete ein geladenes Folgemodul, aber nextModule ist Nothing.")

                        ModulwechselAbschliessen()

                        Exit Sub

                    End If

            End Select

            vorbereiteterModulName = nextModule.ModulName

            'Beim initialen Wechsel liegt der Desktop-Screenshot
            'bereits aus Main() in startBild.
            'Bei späteren Wechseln wird das aktuell laufende Modul aufgenommen.
            If Not istInitialerWechsel Then
                AktualisiereStartBild()
            End If

            ZielBildErstellen()

            transitionIstVorbereitet = TransitionseffektAuswaehlen()

            If zielBild Is Nothing Then

                LogHandling.LogError("Der Modulwechsel wurde abgebrochen, weil keine Titelcard erstellt werden konnte.")

                ModulwechselAbschliessen()

                Exit Sub

            End If

            aktuelleWechselPhase = MCPWechselPhase.HostVorbereitung

            transitionHostWartetAufErstdarstellung = True

            If startBild IsNot Nothing Then

                ZeigeBildImTransitionHost(startBild)

            Else

                LogHandling.LogWarn("Für den Modulwechsel steht kein Startbild bereit. Die Titelcard wird direkt angezeigt.")

                transitionHostWartetAufErstdarstellung = False
                TitelcardPhaseStarten()

            End If

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Vorbereiten des MCP-Modulwechsels: " & ex.ToString())

            ModulwechselFehlgeschlagen()

        End Try

    End Sub

    Private Sub ModulwechselNachHostDarstellungFortsetzen()
        'Beendet das bisherige Modul erst, nachdem der TransitionHost
        'mit dem Startbild tatsächlich dargestellt wurde.

        Dim transitionWurdeGestartet As Boolean

        If shutdownWurdeGestartet Then
            Exit Sub
        End If

        If aktuelleWechselPhase <> MCPWechselPhase.HostVorbereitung Then
            Exit Sub
        End If

        Try

            'Ab diesem Zeitpunkt existiert eine tatsächlich gerenderte
            'Vollbildabdeckung. Das alte Modul darf nun beendet werden.
            If activeModule IsNot Nothing Then

                activeModule.StopModul()
                activeModule = Nothing

            End If

            If fallbackIsActive AndAlso fallbackInstanz IsNot Nothing Then

                fallbackInstanz.Hide()

            End If

            transitionWurdeGestartet = False

            If transitionIstVorbereitet Then

                transitionWurdeGestartet = TransitionseffektStarten()

            End If

            If Not transitionWurdeGestartet Then

                LogHandling.LogWarn("Die Modultransition konnte nicht gestartet werden. Die Titelcard wird direkt angezeigt.")

                TitelcardPhaseStarten()

            End If

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Fortsetzen des Modulwechsels nach der Hostdarstellung: " & ex.ToString())

            ModulwechselFehlgeschlagen()

        End Try

    End Sub

    Private Sub ModulwechselAbschliessen()
        'Gibt die Wechselressourcen frei und aktiviert wieder
        'die normale MCP-Steuerung.

        TransitionBereinigen()

        aktuelleWechselPhase = MCPWechselPhase.Keine

        vorbereiteterModulName = Nothing
        modulwechselIstAktiv = False

        transitionHostWartetAufErstdarstellung = False
        transitionIstVorbereitet = False

        AktualisiereMCPTimer()
        AusstehendeAktionAusfuehren()

    End Sub

    Private Sub ModulwechselFehlgeschlagen()
        'Versucht nach einem fehlgeschlagenen Wechsel,
        'einen kontrollierten sichtbaren Zustand herzustellen.

        tmrTitelcard.Stop()

        transitionHostWartetAufErstdarstellung = False
        transitionIstVorbereitet = False

        TransitionHostSchliessen()
        TransitionBereinigen()

        If activeModule Is Nothing AndAlso
       nextModule IsNot Nothing Then

            If UebernehmeVorbereitetesModul() Then

                Try

                    activeModule.StartModul(
                    Screen.PrimaryScreen)

                Catch ex As Exception

                    LogHandling.LogError(
                    "Auch das vorbereitete Ersatzmodul konnte nicht " &
                    "gestartet werden: " &
                    ex.ToString())

                    activeModule = Nothing

                End Try

            End If

        End If

        If activeModule Is Nothing Then

            AktiviereFallbackSaver()

            If fallbackInstanz IsNot Nothing Then
                fallbackInstanz.Show()
            End If

        End If

        ModulwechselAbschliessen()

    End Sub

#End Region

#Region "Eingaben während des Modulwechsels"

    Private Sub TransitionHostPreviewKeyDown(sender As Object, e As SWI.KeyEventArgs)
        'Verarbeitet Eingaben, solange der WPF-TransitionHost aktiv ist.

        If shutdownWurdeGestartet Then
            Exit Sub
        End If

        Select Case e.Key

            Case SWI.Key.Space

                e.Handled = True
                CloseSlideShowSaver()

            Case SWI.Key.Escape

                e.Handled = True
                ausstehendeAktion =
                AktionNachModulwechsel.Optionsdialog

                TransitionVorzeitigBeenden()

            Case SWI.Key.P

                e.Handled = True
                ausstehendeAktion =
                AktionNachModulwechsel.Pause

                TransitionVorzeitigBeenden()

        End Select

    End Sub

    Private Sub TransitionHostPreviewMouseDown(
    sender As Object,
    e As SWI.MouseButtonEventArgs)
        'Verarbeitet Mauseingaben während des Modulwechsels.

        If shutdownWurdeGestartet Then
            Exit Sub
        End If

        Select Case e.ChangedButton

            Case SWI.MouseButton.Left

                e.Handled = True
                CloseSlideShowSaver()

            Case SWI.MouseButton.Right

                e.Handled = True
                ausstehendeAktion =
                AktionNachModulwechsel.Optionsdialog

                TransitionVorzeitigBeenden()

            Case SWI.MouseButton.Middle

                e.Handled = True
                ausstehendeAktion =
                AktionNachModulwechsel.Pause

                TransitionVorzeitigBeenden()

        End Select

    End Sub

    Private Sub AusstehendeAktionAusfuehren()
        'Führt eine während des Modulwechsels angeforderte Aktion aus.

        Dim auszufuehrendeAktion As AktionNachModulwechsel

        auszufuehrendeAktion =
        ausstehendeAktion

        ausstehendeAktion =
        AktionNachModulwechsel.Keine

        Select Case auszufuehrendeAktion

            Case AktionNachModulwechsel.Optionsdialog

                OpenOptionsDialog()

            Case AktionNachModulwechsel.Pause

                PauseModus()

        End Select

    End Sub

#End Region

#End Region

#Region "Optionsdialog"

    Private Sub OpenOptionsDialog()
        'Öffnet den zentralen Optionsdialog und verarbeitet
        'anschließend die geänderten Frameworkeinstellungen.

        Dim dialogErgebnis As DialogResult
        Dim modulwechselIstNotwendig As Boolean

        If optionsDialogIsActive OrElse shutdownWurdeGestartet Then
            Exit Sub
        End If

        dialogErgebnis = DialogResult.Cancel
        modulwechselIstNotwendig = False

        'Aktuelle Settings in die SettingsInbox stellen
        ReadMainSettingsFromRegistryOrDefaults()
        StoreSettings("Main", aktuelleSettings)

        Try

            If optionsDialog Is Nothing OrElse
               optionsDialog.IsDisposed Then

                optionsDialog = New frmOptionsMain()

            End If

            dialogErgebnis = optionsDialog.ShowDialog()

        Catch ex As Exception

            LogHandling.LogError("Der Optionsdialog konnte nicht angezeigt werden: " & ex.ToString())

        Finally

            If optionsDialog IsNot Nothing Then

                Try
                    optionsDialog.Dispose()
                Finally
                    optionsDialog = Nothing
                End Try

            End If

        End Try

        If dialogErgebnis <> DialogResult.OK Then
            Exit Sub
        End If

        'Zentrale Frameworkdaten neu einlesen
        IniAndReinitialize()
        LegitimeListeErstellen()

        'Den Timer aktualisieren.
        AktualisiereMCPTimer()

        'Prüfen, ob nach den Einstellungsänderungen ein Modulwechsel notwendig ist.
        If Not IstAktuellesModulWeiterhinAktiviert() Then

            modulwechselIstNotwendig = True

        ElseIf fallbackIsActive AndAlso
               listOfEnabledModules IsNot Nothing AndAlso
               listOfEnabledModules.Count > 0 Then

            modulwechselIstNotwendig = True

        End If

        If modulwechselIstNotwendig Then
            MCPModulwechsel()
        End If

        'Bildauswahl und laufendes Modul neu konfigurieren
        BildauswahlMain.CheckYourSettings()

        If activeModule IsNot Nothing Then
            activeModule.CheckYourSettings()
        End If

        'Fallbackfarbe aktualisieren
        If fallbackIsActive AndAlso
           fallbackInstanz IsNot Nothing Then

            fallbackInstanz.CheckYourMail()

        End If

    End Sub

#End Region

#Region "Pause-Modus"

    Private Sub PauseModus()
        'Schaltet den Pausezustand des aktiven Schoners um.

        If shutdownWurdeGestartet Then
            Exit Sub
        End If

        If fallbackIsActive Then

            If fallbackInstanz Is Nothing Then
                Exit Sub
            End If

            If Not fallbackPaused Then

                fallbackInstanz.tmrFallback.Stop()
                fallbackPaused = True

            Else

                fallbackInstanz.tmrFallback.Start()
                fallbackPaused = False

            End If

            Exit Sub

        End If

        If activeModule IsNot Nothing Then
            activeModule.PauseModusModul()
        End If

    End Sub

#End Region

#Region "Eingabesteuerung"

    Private Sub RegistriereEingabesteuerung()
        'Registriert die zentralen globalen Tastatur-
        'und Mausereignisse genau einmal.

        If eingabesteuerungIstRegistriert Then
            Exit Sub
        End If

        AddHandler GlobalKeyDown,
            AddressOf HandleGlobalKeyDown

        AddHandler GlobalMouseDown,
            AddressOf HandleGlobalMouseDown

        eingabesteuerungIstRegistriert = True

    End Sub

    Private Sub EntferneEingabesteuerung()
        'Entfernt die zentralen globalen Tastatur-
        'und Mausereignisse.

        If Not eingabesteuerungIstRegistriert Then
            Exit Sub
        End If

        RemoveHandler GlobalKeyDown, AddressOf HandleGlobalKeyDown
        RemoveHandler GlobalMouseDown, AddressOf HandleGlobalMouseDown

        eingabesteuerungIstRegistriert = False

    End Sub

    Private Sub HandleGlobalKeyDown(sender As Object, e As KeyEventArgs)
        'Verarbeitet global weitergeleitete Tastatureingaben.

        If isInputLocked OrElse shutdownWurdeGestartet Then
            Exit Sub
        End If

        Select Case e.KeyCode

            Case Keys.Escape

                OpenOptionsDialog()

            Case Keys.Space

                CloseSlideShowSaver()

            Case Keys.P

                PauseModus()

        End Select

    End Sub

    Private Sub HandleGlobalMouseDown(sender As Object, e As MouseEventArgs)
        'Verarbeitet global weitergeleitete Maustasten.

        If isInputLocked OrElse shutdownWurdeGestartet Then
            Exit Sub
        End If

        Select Case e.Button

            Case MouseButtons.Left

                CloseSlideShowSaver()

            Case MouseButtons.Right

                OpenOptionsDialog()

            Case MouseButtons.Middle

                PauseModus()

        End Select

    End Sub

#End Region

#Region "Settings"
    Private Sub ReadMainSettingsFromRegistryOrDefaults()
        'Liest die aktuellen Settings aus der Registry oder verwendet die definierten Standardwerte

        Dim tmpRegistryValues As String
        Dim mainDefaults As Dictionary(Of String, String)

        mainDefaults = GetMainDefaultSettings()

        'ModulDauer
        aktuelleSettings.ModulDauer = CInt(ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulDauer", mainDefaults))

        'ModulReihenfolge
        aktuelleSettings.ModulReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulReihenfolge", mainDefaults)

        'Aktivierte Module
        tmpRegistryValues = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulAktivListe", mainDefaults)

        aktuelleSettings.ModulAktivListe = SplitSemicolonList(tmpRegistryValues)

        'MultiMonitor
        aktuelleSettings.MultiMonitor = CBool(ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "MultiMonitor", mainDefaults))

        'Modul-Transitionen
        tmpRegistryValues = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulTransitionListe", mainDefaults)

        aktuelleSettings.ModulTransitionListe = SplitSemicolonList(tmpRegistryValues)

        'Reihenfolge der Modul-Transitionen
        aktuelleSettings.ModulTransitionReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", mainDefaults)

        'Hintergrundfarbe
        aktuelleSettings.Hintergrundfarbe = StringToColor(ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "Hintergrundfarbe", mainDefaults))

    End Sub

    'Defaultwerte für allgemeine Einstellungen
    Friend Function GetMainDefaultSettings() As Dictionary(Of String, String)
        'Erstellt die Defaultwerte für die allgemeinen Frameworkeinstellungen

        Dim mainDefaults As New Dictionary(Of String, String)

        mainDefaults("ModulDauer") = "2"
        mainDefaults("ModulReihenfolge") = "Zufällig"
        mainDefaults("ModulAktivListe") = ""
        mainDefaults("MultiMonitor") = "False"
        mainDefaults("ModulTransitionListe") = ""
        mainDefaults("ModulTransitionReihenfolge") = "Zufällig bei Start"
        mainDefaults("Hintergrundfarbe") = "0,0,0,255"

        Return mainDefaults

    End Function
#End Region

#Region "Framework-Shutdown"

    Private Sub CloseSlideShowSaver()
        'Beendet den Bildschirmschoner kontrolliert und gibt alle zentralen Instanzen frei.

        If shutdownWurdeGestartet Then
            Exit Sub
        End If

        shutdownWurdeGestartet = True
        isInputLocked = True

        EntferneMCPStart()

        If tmrMCP IsNot Nothing Then

            tmrMCP.Stop()
            tmrMCP.Dispose()

        End If

        If tmrTitelcard IsNot Nothing Then

            tmrTitelcard.Stop()
            tmrTitelcard.Dispose()

        End If

        transitionHostWartetAufErstdarstellung = False
        transitionIstVorbereitet = False

        mcpWurdeGestartet = False
        modulwechselIstAktiv = False

        aktuelleWechselPhase = MCPWechselPhase.Keine
        ausstehendeAktion = AktionNachModulwechsel.Keine

        TransitionBereinigen()

        TransitionHostSchliessen()

        EntferneEingabesteuerung()

        CursorHandling.CursorPowerShow()

        If activeModule IsNot Nothing Then

            Try

                WriteToRegistry(SLIDESHOWMAIN_PATH & "LetztgespieltesModul", activeModule.ModulName)

                activeModule.StopModul()

            Catch ex As Exception

                LogHandling.LogError("Fehler beim Beenden des aktiven Moduls """ & activeModule.ModulName & """: " & ex.ToString())

            Finally

                activeModule = Nothing

            End Try

        End If

        If nextModule IsNot Nothing Then

            Try
                nextModule.StopModul()
            Catch ex As Exception
                LogHandling.LogError("Fehler beim Aufräumen des vorbereiteten Moduls: " & ex.ToString())
            Finally
                nextModule = Nothing
            End Try

        End If

        If optionsDialog IsNot Nothing Then

            Try

                optionsDialog.Close()
                optionsDialog.Dispose()

            Catch ex As Exception

                LogHandling.LogError("Fehler beim Schließen des Optionsdialogs: " & ex.ToString())

            Finally

                optionsDialog = Nothing

            End Try

        End If

        If fallbackInstanz IsNot Nothing Then

            Try

                fallbackInstanz.Close()
                fallbackInstanz.Dispose()

            Catch ex As Exception

                LogHandling.LogError("Fehler beim Schließen des FallbackSavers: " & ex.ToString())

            Finally

                fallbackInstanz = Nothing

            End Try

        End If

        fallbackIsActive = False
        fallbackPaused = False

        If startBild IsNot Nothing Then

            startBild = Nothing

        End If

        If zielBild IsNot Nothing Then

            zielBild = Nothing

        End If

        LogHandling.LogInfo("Der Bildschirmschoner wurde beendet.")

        Application.Exit()

    End Sub

#End Region

#Region "Exception Handling"
    Private Sub ThreadExceptionHandler(sender As Object, e As Threading.ThreadExceptionEventArgs)
        LogHandling.LogError("ThreadException: " & e.Exception.ToString())
        MessageBox.Show("Fehler im UI-Thread: " & e.Exception.Message)
    End Sub

    Private Sub UnhandledExceptionHandler(sender As Object, e As UnhandledExceptionEventArgs)
        Dim ex = TryCast(e.ExceptionObject, Exception)
        If ex IsNot Nothing Then
            LogHandling.LogError("UnhandledException: " & ex.ToString())
            MessageBox.Show("Nicht abgefangene Ausnahme: " & ex.Message)
        End If
    End Sub

#End Region

End Module
