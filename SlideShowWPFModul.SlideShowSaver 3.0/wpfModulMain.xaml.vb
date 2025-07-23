Imports System.IO
Imports System.Windows.Forms
Imports System.Windows.Threading
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLoader
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ScreenHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.WPFHandling
Imports System.Runtime.InteropServices
Imports System.Windows.Interop
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowLogging
Imports System.Drawing

Partial Public Class wpfModulMain

#Region "Variablen"
    'Variablendeklaration

    'Settings
    Private aktuelleSettings As ModulMain.SettingsModul_SSS

    'Bildanzeige & -auswahl
    Private aktuellesBild As BitmapImage
    Private aktuellesImage As Image
    Private neuesBild As BitmapImage
    Private neuesImage As Image
    Private bildPfad As String
    Private initialePfade As List(Of String)
    Private aktuellesVerzeichnis As New List(Of String)
    Private aktuellesVerzeichnisCounter As Integer

    'Für frmPauseModul
    Public Shared listeDerZuletztAngezeigtenBilder As New List(Of String)

    'Transitionen
    Public aktiveTransition As ISlideShowTransition = Nothing
    Private neueTransition As String = Nothing
    Public transitionIstAktiv As Boolean = False
    Private listOfAvailableTransitions As List(Of SlideShowTransitionInfo)
    Private listOfEnabledTransitions As List(Of String)
    Private warteAufDelay As Boolean
    Private stoppuhr As New Stopwatch
    Private sizeWinForm As System.Drawing.Size

    'Shader
    Private aktiverShader As ISlideShowShader = Nothing
    Private neuerShader As String = Nothing
    Private listOfAvailableShaders As List(Of SlideShowShaderInfo)
    Private listOfEnabledShaders As List(Of String)

    'Timer
    Public WithEvents tmrModul As New DispatcherTimer()
    Private WithEvents tmrDelay As New DispatcherTimer()

    'Sonstiges
    Private rnd As New Random()
    Private Const SW_SHOWMAXIMIZED As Integer = 3
    Private Const SW_RESTORE As Integer = 9

#End Region

    'DLL Imports
    '<DllImport("user32.dll")>
    'Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    'End Function

    '<DllImport("user32.dll")>
    'Private Shared Function ShowWindow(hWnd As IntPtr, nCmdShow As Integer) As Boolean
    'End Function

    Public Sub New()
        'Erzeugt und initialisiert das Fenster und seine Komponenten

        'Eventhandler
        AddHandler tmrModul.Tick, AddressOf TmrModul_Tick
        AddHandler tmrDelay.Tick, AddressOf tmrDelay_Tick
        AddHandler ModulMain.YouHaveMail_SSS, AddressOf CheckYourMail

        'Initialisierung der Komponenten
        InitializeComponent()

        ' Fenster in den Vordergrund und maximiert
        Me.WindowState = WindowState.Maximized

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        'Weitere Initialisierungen der Form

        'Versuch, das Fenster in den Vordergrund zu bringen - Ebene 1
        Me.Topmost = False
        Me.ShowActivated = True
        Me.Show()
        Me.Activate()

        'Versuch, das Fenster in den Vordergrund zu bringen - Ebene 2
        ' Nach einem kleinen Delay erneut aktivieren
        Dim bringToFrontTimer As New DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(250)}
        AddHandler bringToFrontTimer.Tick,
                                            Sub()
                                                bringToFrontTimer.Stop()
                                                Me.Topmost = False
                                                Me.Focus()
                                                Me.Activate()
                                            End Sub
        bringToFrontTimer.Start()


        CheckYourMail()
        LadeErstesBild()
        tmrModul.Start()

    End Sub

    Private Sub TmrModul_Tick(sender As Object, e As EventArgs)
        'Nac Beendigung der Anzeige des Bildes gemäß Anzeigedauer startet der Timer die nächste Transition
        'oder, falls keine ausgewählt ist, initiert den Bildwechsel.


        If transitionIstAktiv Then Exit Sub
        If warteAufDelay Then Exit Sub

        'Falls der Benutzer in der Zwischenzeit an den Optionen 'rumgepfuscht hat
        LegitimeTransitionsListeErstellen()

        If listOfEnabledTransitions.Count > 0 AndAlso aktuellesBild IsNot Nothing Then

#Region "Transition wechseln"
            'Transition aussuchen
            Select Case aktuelleSettings.TransitionsReihenfolge
                Case "In Reihenfolge"
                    WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition", aktiveTransition.TransitionName)
                    neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, aktiveTransition.TransitionName)
                Case "Zufällig"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                Case "Zufällig bei Start"
                    ' Falls der Benutzer in der Zwischenzeit die Settings geändert hat
                    If aktiveTransition Is Nothing Then
                        neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                    End If
                Case "In Reihenfolge bei Start"
                    ' Falls der Benutzer in der Zwischenzeit die Settings geändert hat
                    If aktiveTransition Is Nothing Then
                        neueTransition = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition")
                        If neueTransition = Nothing Then neueTransition = ""
                        neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, neueTransition)
                    End If
                Case Else
                    ' Falls der Benutzer in der Zwischenzeit die Settings geändert hat (entspricht zufällig bei Start)
                    If aktiveTransition Is Nothing Then
                        neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                    End If
            End Select

            'Neue Transition laden und Handler behandeln
            If aktiveTransition IsNot Nothing Then
                'Alte Eventhandler löschen
                RemoveHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
                RemoveHandler aktiveTransition.FrameIstFertig, AddressOf Transition_FrameIstFertig
            End If

            aktiveTransition = TransitionByNameLoader.LadeTransitionNachName(neueTransition)

            If aktiveTransition IsNot Nothing Then
                'Handler hinzufügen
                AddHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
                AddHandler aktiveTransition.FrameIstFertig, AddressOf Transition_FrameIstFertig
            End If
#End Region

            'Transition starten, Status & Stoppuhr setzen

            transitionIstAktiv = True
            stoppuhr = Stopwatch.StartNew()
            sizeWinForm = New Size(Me.RenderSize.Width, Me.RenderSize.Height)
            aktiveTransition.RunTransition(aktuellesBild, PictureBoxSizeMode.Zoom, neuesBild, PictureBoxSizeMode.Zoom, sizeWinForm)

            'Timer beenden 
            tmrModul.Stop()

        Else
            'Dann muss der Timer halt selber ran...

            Bildwechsel()
            transitionIstAktiv = False
            warteAufDelay = False

        End If

    End Sub

    Private Function LadeBild(pfad As String) As BitmapImage
        'Lädt ein Bild aus einem Filestream und gibt es als Bitmap zurück

        Dim bmp As New BitmapImage()

        Using fs As New FileStream(pfad, FileMode.Open, FileAccess.Read)
            bmp.BeginInit()
            bmp.CacheOption = BitmapCacheOption.OnLoad
            bmp.StreamSource = fs
            bmp.EndInit()
        End Using

        Return bmp

    End Function

    Private Sub CheckYourMail()
        'Liest die aktuelleSettings ein

        aktuelleSettings = GetSettings(Of ModulMain.SettingsModul_SSS)(ModulMain.nameModul)

    End Sub

    Private Sub LadeErstesBild()
        'Wählt die ersten Bilder zur Anzeige aus.

        Dim helper As New WindowInteropHelper(Me)
        Dim hwnd As IntPtr = helper.Handle

#Region "Transition aussuchen"
        'Transition aussuchen
        If listOfEnabledTransitions.Count > 0 Then
            Select Case aktuelleSettings.TransitionsReihenfolge
                Case "In Reihenfolge"
                    neueTransition = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition")
                    If neueTransition = Nothing Then neueTransition = ""
                    neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, neueTransition)
                Case "Zufällig"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                Case "Zufällig bei Start"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                Case "In Reihenfolge bei Start"
                    neueTransition = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition")
                    If neueTransition = Nothing Then neueTransition = ""
                    neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, neueTransition)
                Case Else
                    'Entspricht "Zufällig bei Start"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
            End Select

            If aktiveTransition IsNot Nothing Then
                'Sinnlos bei Initialisierung, aber gehört sich ja so. Und vielleicht gibt es ja doch noch 
                'Leichen im Speicher...
                RemoveHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
            End If

            aktiveTransition = TransitionByNameLoader.LadeTransitionNachName(neueTransition)

            If aktiveTransition IsNot Nothing Then
                'Handler hinzufügen
                AddHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
            End If

        End If
#End Region

#Region "Shader aussuchen"
        'Aktiven Shader laden 
        If listOfEnabledShaders.Count > 0 Then
            Select Case aktuelleSettings.ShaderReihenfolge
                Case "In Reihenfolge"
                    neuerShader = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader")
                    If neuerShader = Nothing Then neuerShader = ""
                    neuerShader = GetNextAlphabeticItemName(listOfEnabledShaders, neuerShader)
                Case "Zufällig"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
                Case "Zufällig bei Start"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
                Case "In Reihenfolge bei Start"
                    neuerShader = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader")
                    If neuerShader = Nothing Then neuerShader = ""
                    neuerShader = GetNextAlphabeticItemName(listOfEnabledTransitions, neuerShader)
                Case Else
                    'Entspricht "Zufällig bei Start"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
            End Select

            aktiverShader = ShaderByNameLoader.LadeShaderNachName(neuerShader)

        End If
#End Region


#Region "Erste Bilder laden"
        'erste Bilder laden (ggf. Shader anwenden)
        If aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then

            Do
                aktuellesVerzeichnis = GetPicturesByDirectory()
            Loop Until aktuellesVerzeichnis.Count >= 2 'Das Verzeichnis muss mindestens 2 Einträge haben.
            aktuellesVerzeichnisCounter = 2 'Die ersten beiden Einträge werden gleich "verbraucht"

            'aktuellesBild setzen
            bildPfad = aktuellesVerzeichnis(0)
            aktuellesImage = GetPictureByName(bildPfad)

            Try
                If aktiverShader IsNot Nothing Then
                    aktuellesImage = aktiverShader.RunShader(aktuellesImage, bildPfad, GetNativeScreenResolution())
                End If
            Catch ex As Exception
                LogHandling.LogError("Modul SSS 3.0 - wpfModulMain.LadeErstesBild(): Problem beim Starten des Shaders: " & ex.ToString)
            End Try

            aktuellesBild = ConvertImageToBitmapImage(aktuellesImage)

            listeDerZuletztAngezeigtenBilder.Clear()
            listeDerZuletztAngezeigtenBilder.Add(bildPfad)

            'neuesBild setzen
            bildPfad = aktuellesVerzeichnis(1)
            neuesImage = GetPictureByName(bildPfad)

            Try
                If aktiverShader IsNot Nothing Then
                    neuesImage = aktiverShader.RunShader(neuesImage, bildPfad, GetNativeScreenResolution())
                End If
            Catch ex As Exception
                LogHandling.LogError("Modul SSS 3.0 - wpfModulMain.LadeErstesBild(): Problem den Shader zu starten: " & ex.Message)
            End Try

            neuesBild = ConvertImageToBitmapImage(neuesImage)

            'listeDerZuletztAngezeigtenBilder wird in der Methode Bildwechsel gefüllt.

        Else

            '2 Zufallsbilder raussuchen
            initialePfade = GetPictures(2)

            'aktuellesBild setzen
            bildPfad = initialePfade(0)
            aktuellesImage = GetPictureByName(bildPfad)

            If aktiverShader IsNot Nothing Then
                aktuellesImage = aktiverShader.RunShader(aktuellesImage, bildPfad, Screen.PrimaryScreen.Bounds.Size)
            End If

            aktuellesBild = ConvertImageToBitmapImage(aktuellesImage)

            listeDerZuletztAngezeigtenBilder.Clear()
            listeDerZuletztAngezeigtenBilder.Add(bildPfad)

            'neuesBild setzen
            bildPfad = initialePfade(1)
            neuesImage = GetPictureByName(bildPfad)

            If aktiverShader IsNot Nothing Then
                neuesImage = aktiverShader.RunShader(neuesImage, bildPfad, GetNativeScreenResolution())
            End If

            neuesBild = ConvertImageToBitmapImage(neuesImage)

            'listeDerZuletztAngezeigtenBilder für neuesBild wird in der Methode Bildwechsel gefüllt.

        End If
#End Region

        'Erstes Bild anzeigen
        imgAnzeige.Source = aktuellesBild

        'Versuch, das Fenster in den Vordergrund zu bringen - Ebene 3
        'Direkt in die Windows.InteropServices eingreifen - unglaublich!!!
        'ShowWindow(hwnd, SW_SHOWMAXIMIZED)
        'SetForegroundWindow(hwnd)

        'Bildinfo initialisieren - mit Werten von aktuellesBild

        If aktuelleSettings.BildInfoAnzeigen AndAlso bildPfad IsNot Nothing Then
            ModulMain.sssInfo.RefreshLabels(bildPfad)
            ModulMain.sssInfo.Refresh()
            ModulMain.sssInfo.BringToFront()
        End If

    End Sub

    Private Sub Window_KeyDown(sender As Object, e As System.Windows.Input.KeyEventArgs)
        'Leitet Tastatureingaben über den Global Key Event an das MCP weiter

        If e.Key = Key.Escape Then
            e.Handled = True
        End If

        SlideShowTools.KeyAndMouseHandling.ForwardKeyDownWPF(sender, e)

    End Sub

    Private Sub Window_MouseDown(sender As Object, e As System.Windows.Input.MouseButtonEventArgs)
        'Leitet Maustasten über den Global MouseDown Evente an das MCP weiter

        SlideShowTools.KeyAndMouseHandling.ForwardMouseDownWPF(Me, e)

    End Sub

    Private Sub Bildwechsel()
        'Eigentliche Anzeige des Bildes, Verwaltungsaufgaben und Auswahl des nächsten Bildes

        aktuellesImage = neuesImage
        aktuellesBild = neuesBild
        imgAnzeige.Source = aktuellesBild

        'BildInfo des Bildes aktualisieren
        If aktuelleSettings.BildInfoAnzeigen Then
            If bildPfad IsNot Nothing Then
                ModulMain.sssInfo.RefreshLabels(bildPfad)
            End If
            ModulMain.sssInfo.Refresh()
            ModulMain.sssInfo.BringToFront()
        End If

        'Liste der letzten 10 Bilder befüllen und ggf. das erste Element wieder aus der Liste löschen.
        If bildPfad IsNot Nothing Then
            listeDerZuletztAngezeigtenBilder.Add(bildPfad)
            If listeDerZuletztAngezeigtenBilder.Count > 10 Then
                listeDerZuletztAngezeigtenBilder.RemoveAt(0)
            End If
        End If

        'Neues Bild laden. 
        If aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then
            aktuellesVerzeichnisCounter += 1
            If aktuellesVerzeichnisCounter >= aktuellesVerzeichnis.Count Then
                aktuellesVerzeichnisCounter = 0
                Do
                    aktuellesVerzeichnis = GetPicturesByDirectory()
                Loop Until aktuellesVerzeichnis.Count > 0
            End If
            bildPfad = aktuellesVerzeichnis(aktuellesVerzeichnisCounter)
        Else
            bildPfad = GetPictures(1).Item(0)
        End If

        neuesImage = GetPictureByName(bildPfad)

        'Falls der Benutzer in der Zwischenzeit an den Optionen 'rumgepfuscht hat
        LegitimeShaderListeErstellen()

        If listOfEnabledShaders.Count > 0 Then
            'Aktiven Shader laden & auf neuesBild anwenden
            Select Case aktuelleSettings.ShaderReihenfolge
                Case "In Reihenfolge"
                    WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader", aktiverShader.ShaderName)
                    neuerShader = GetNextAlphabeticItemName(listOfEnabledShaders, aktiverShader.ShaderName)
                Case "Zufällig"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
                Case "Zufällig bei Start"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
                Case "In Reihenfolge bei Start"
                    neuerShader = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader")
                    If neuerShader = Nothing Then neuerShader = ""
                    neuerShader = GetNextAlphabeticItemName(listOfEnabledTransitions, neuerShader)
                Case Else
                    'Entspricht "Zufällig bei Start"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
            End Select

            If Not String.IsNullOrEmpty(neuerShader) Then
                aktiverShader = ShaderByNameLoader.LadeShaderNachName(neuerShader)
                neuesImage = aktiverShader.RunShader(neuesImage, bildPfad, GetNativeScreenResolution())
            End If

        End If

        neuesBild = ConvertImageToBitmapImage(neuesImage)

    End Sub

    Private Sub wpfModulMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        'Aufräumen
        If transitionIstAktiv Then
            aktiveTransition.StopTransition()
        End If

        RemoveHandler ModulMain.YouHaveMail_SSS, AddressOf CheckYourMail
        RemoveHandler tmrModul.Tick, AddressOf TmrModul_Tick

        tmrModul = Nothing
        tmrDelay = Nothing

    End Sub

    Public Sub LegitimeShaderListeErstellen()
        'Aktualisiert die Liste der Shader, die das Modul SlideShowSaver 3.0 aktuell anzeigen darf

        Dim enabledShadersRegVal As String
        Dim tempList As New List(Of String)
        Dim defaults As New Dictionary(Of String, String)

        ' Listen der Module und aktivierten Module neu Laden, gegeneinander abgleichen.
        defaults = ModulMain.GetModulDefaultSettings()

        listOfAvailableShaders = ShaderListLoader.LadeShaderInfoListe()
        enabledShadersRegVal = ReadFromRegOrDefaults(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Shader", defaults)

        tempList =
                enabledShadersRegVal.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries).
                Select(Function(s) s.Trim()).
                Where(Function(name) listOfAvailableShaders.Any(
                    Function(shader) shader.ShaderName.Equals(name, StringComparison.OrdinalIgnoreCase))).
                Distinct(StringComparer.OrdinalIgnoreCase).
                OrderBy(Function(s) s).
                ToList()

        listOfEnabledShaders = tempList

    End Sub

    Public Sub LegitimeTransitionsListeErstellen()
        'Aktualisiert die Liste der Transitionen, die das Modul SlideShowSaver 3.0 aktuell anzeigen darf

        Dim enabledTransitionsRegVal As String
        Dim tempList As New List(Of String)
        Dim defaults As New Dictionary(Of String, String)

        ' Listen der Module und aktivierten Module neu Laden, gegeneinander abgleichen.
        defaults = ModulMain.GetModulDefaultSettings()

        listOfAvailableTransitions = TransitionListLoader.LadeTransitionInfoListe()
        enabledTransitionsRegVal = ReadFromRegOrDefaults(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Transitionseffekte", defaults)

        tempList =
                enabledTransitionsRegVal.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries).
                Select(Function(s) s.Trim()).
                Where(Function(name) listOfAvailableTransitions.Any(
                    Function(transition) transition.TransitionName.Equals(name, StringComparison.OrdinalIgnoreCase))).
                Distinct(StringComparer.OrdinalIgnoreCase).
                OrderBy(Function(s) s).
                ToList()

        listOfEnabledTransitions = tempList

    End Sub

    Private Sub Transition_TransitionIsRunning(state As Boolean)
        'Wechselt das aktuelle Bild nach erfolgreichem Abschluss der Transition und startet tmrModul 

        'Sofort raus, falls die Transition noch läuft
        If state Then Exit Sub

        'Stoppuhr anhalten
        stoppuhr.Stop()
        tmrModul.Stop()

        'Für Transitionen, die so schnell fertig werden, dass es zu einer Racing-Condition mit tmrModul kommt.
        If stoppuhr.ElapsedMilliseconds < 1000 Then
            warteAufDelay = True
            tmrDelay.Interval = TimeSpan.FromSeconds(1)
            tmrDelay.Start()
        Else
            'Hier beginnt die Bildanzeige
            Bildwechsel()

            'Status setzen und tmrModul starten
            transitionIstAktiv = False
            warteAufDelay = False
            tmrModul.Interval = TimeSpan.FromSeconds(aktuelleSettings.Anzeigedauer)
            tmrModul.Start()
        End If

    End Sub

    Private Sub tmrDelay_Tick(sender As Object, e As EventArgs) Handles tmrDelay.Tick
        'Falls eine Transition so schnell ist, dass frmModulMain das nicht rechtzeitig mitbekommt.
        tmrDelay.Stop()
        warteAufDelay = False

        'Hier beginnt die Bildanzeige
        Bildwechsel()

        'Status setzen und tmrModul starten
        transitionIstAktiv = False
        tmrModul.Interval = TimeSpan.FromSeconds(aktuelleSettings.Anzeigedauer)
        tmrModul.Start()

    End Sub

    Private Sub Transition_FrameIstFertig(rtb As RenderTargetBitmap)
        imgAnzeige.Source = Nothing
        GC.Collect()
        GC.WaitForPendingFinalizers()
        imgAnzeige.Source = rtb
    End Sub

End Class
