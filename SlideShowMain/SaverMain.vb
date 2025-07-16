Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports SlideShowLogging
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLoader
Imports SlideShowTools.RegistryHandling
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowTools.SharedDataHandling
Imports SlideShowTools

Module SaverMain

#Region "Variablendeklaration"
    'Variablendeklaration

    'Für den Fallback-Saver
    Public fallbackInstanz As frmFallbackSaver
    Public fallbackIsActive As Boolean = False
    Public fallbackPaused As Boolean = False

    'Für Schoner-Module
    Public listOfAvailableModules As List(Of SlideShowModulInfo)
    Public listOfEnabledModules As List(Of String)
    Public activeModule As ISlideShowModul = Nothing

    'Für Transitions (für den späteren Gebrauch, sobald implementiert)
    Public listOfAvailableTransitions As List(Of SlideShowTransitionInfo)
    Public listOfEnabledTransitions As List(Of String)
    Public activeTransition As ISlideShowTransition = Nothing

    'Für Settings & Options-Dialog
    Public optionsDialog As frmOptionsMain = Nothing
    Public optionsDialogIsActive As Boolean
    Public isInputLocked As Boolean
    Public defaults As New Dictionary(Of String, String)

    'Sonstiges
    Public masterControlProgram As frmSaverMain = Nothing
    Public rnd As New Random()
    Public screenDim As Rectangle = Screen.PrimaryScreen.Bounds
    Public initialerScreenShot As Image = GetCurrentScreen()

    'Structure SettingsMain
    Public Structure SettingsMain
        Public ModulDauer As Integer
        Public ModulReihenfolge As String
        Public ModulAktivListe As List(Of String)
        Public MultiMonitor As Boolean
        Public ModulTransitionListe As List(Of String)
        Public ModulTransitionReihenfolge As String
    End Structure

#End Region

    ' -- Funktionen und Prozeduren --
    Public Sub Main()
        Dim args As String() = Environment.GetCommandLineArgs()

        'Exception Handling & Debugging
        LogHandling.SetMinimumLogLevel(LogLevel.DebugLevel)
        AddHandler Application.ThreadException, AddressOf ThreadExceptionHandler
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf UnhandledExceptionHandler

        ' Hübsch machen
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        'Hallo sagen
        LogHandling.LogInfo("SlideShowSaver 3.0 Framework wurde gestartet")

        'Modulen den initialen Screenshot zur Verfügung stellen
        StartBild = initialerScreenshot
        StartBildWurdeVerwendet = False

        'Fallbackserver instanzieren und MCP initialisieren
        fallbackInstanz = New frmFallbackSaver()
        Try
            If masterControlProgram Is Nothing Then
                masterControlProgram = New frmSaverMain()
            End If
        Catch ex As Exception
            LogHandling.LogError("Das MCP konnte nicht geladen werden: " & ex.ToString)
        End Try


        ' ----------------------------------------------------------------
        ' Schoner gemäß mitgelieferter Argument im passenden Modus starten
        ' ----------------------------------------------------------------

        If args.Length <= 1 Then
            ' Kein Argument → Standardmäßig als Bildschirmschoner starten
            Application.Run(masterControlProgram)
            Return
        End If

        Select Case args(1).ToLowerInvariant()
            Case "/c"
                ' Konfiguration anzeigen
                Application.Run(optionsDialog)

            Case "/s"
                ' Bildschirmschoner im Vollbild starten
                Application.Run(masterControlProgram)


                ' Solange noch kein Preview-Handling implementiert ist...

                'Case "/p"
                '    ' Vorschau im kleinen Fenster (z. B. im Systemeinstellungsfenster)
                '    If args.Length >= 3 Then
                '        Try
                '            Dim previewHandle As IntPtr = CType(Integer.Parse(args(2)), IntPtr)
                '            Dim previewForm As New frmSaverMain()
                '            previewForm.StartModul(Screen.PrimaryScreen, True, previewHandle)
                '            Application.Run(previewForm)
                '        Catch ex As Exception
                '            ' Ignoriere Fehler – z. B. ungültiger Handle
                '            LogHandling.LogError("Fehler beim Starten des Preview-Modus:" & ex.ToString)
                '        End Try
                '    End If

            Case Else
                ' Unbekannter Parameter – Standardmäßig starten
                Application.Run(masterControlProgram)

        End Select
    End Sub

    'Defaultwerte für allgemeine Einstellungen
    Public Function GetMainDefaultSettings() As Dictionary(Of String, String)

        defaults("ModulDauer") = "2" 'In Minuten
        defaults("ModulReihenfolge") = "Zufällig"
        defaults("ModulAktivListe") = "" 'Liste aktivierter Module (durch Semikola getrennt)
        defaults("MultiMonitor") = "False" 'Multi-Monitor Support aktivieren
        defaults("ModulTransitionListe") = "" 'Liste der Transitions zwischen den Modulen
        defaults("ModulTransitionReihenfolge") = "Zufällig bei Start" 'Wie wechseln die Transitions zwischen Modulen

        Return defaults
    End Function


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


End Module
