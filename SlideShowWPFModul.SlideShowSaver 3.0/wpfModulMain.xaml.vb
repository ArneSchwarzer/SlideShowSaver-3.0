Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports System.Windows.Interop
Imports System.Windows.Media.Animation
Imports System.Windows.Threading
Imports SlideShowBildauswahl
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLoader
Imports SlideShowLogging
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ScreenHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling

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
    Private aktuellesVerzeichnis As New List(Of String)
    Private aktuellesVerzeichnisCounter As Integer

    'Für frmPauseModul
    Public Shared listeDerZuletztAngezeigtenBilder As New List(Of String)

    'Präsentationsmodus
    Private präsentationAnzeigen As Boolean = False

    'Transitionen
    Public aktiveTransition As ISlideShowTransition = Nothing
    Private neueTransition As String = Nothing
    Public transitionIstAktiv As Boolean = False
    Private listOfAvailableTransitions As List(Of SlideShowTransitionInfo)
    Private listOfEnabledTransitions As List(Of String)
    Private sizeWinForm As System.Drawing.Size

    'Shader
    Private aktiverShader As ISlideShowShader = Nothing
    Private neuerShader As String = Nothing
    Private listOfAvailableShaders As List(Of SlideShowShaderInfo)
    Private listOfEnabledShaders As List(Of String)

    'Timer
    Public WithEvents tmrModul As New DispatcherTimer()
    Public WithEvents tmrPresentation As New DispatcherTimer()

    'Initialisierungsphase
    Private hasFirstVerzeichnisse As Boolean = False
    Private hasFirstBilder As Boolean = False
    Private hintergrundWM As Windows.Media.Color
    Private sbBilder As Storyboard
    Private sbVerz As Storyboard

    'Sonstiges
    Private rnd As New Random()

#End Region

    'Initialisierungen
    Public Sub New()
        'Erzeugt und initialisiert das Fenster und seine Komponenten

        'Eventhandler
        AddHandler ModulMain.YouHaveMail_SSS, AddressOf CheckYourMail
        AddHandler BildauswahlMain.ErsteBilderGefunden, AddressOf BildauswahlMain_ErsteBilderGefunden
        AddHandler BildauswahlMain.ErsteVerzeichnisseGefunden, AddressOf BildauswahlMain_ErsteVerzeichnisseGefunden

        'Initialisierung der Komponenten
        InitializeComponent()

        ' Fenster in den Vordergrund und maximiert
        Me.WindowState = WindowState.Maximized

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Weitere Initialisierungen der Form

        'Versuch, das Fenster in den Vordergrund zu bringen - Ebene 1
        Me.Topmost = False
        Me.ShowActivated = True
        Me.Show()
        Me.Activate()

        'Hintergrundfarbe setzen
        hintergrundWM = SDColorToWMColor(HintergrundFarbeSaver)
        Me.Background = New SolidColorBrush(hintergrundWM)

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

        'Initialisierungslabel einstellen

        'Handle des aktuellen Fensters holen
        Dim helper As New WindowInteropHelper(Me)
        Dim hwnd As IntPtr = helper.Handle

        'Bildschirm ermitteln, auf dem sich das Fenster befindet
        Dim currentScreen As Screen = Screen.FromHandle(hwnd)

        'Maximalbreite = 90 % der aktuellen Bildschirmbreite
        Dim maxWidth As Double = currentScreen.Bounds.Width * 0.9

        'Label einstellen und anzeigen
        imgAnzeige.Visibility = Visibility.Visible

        pnlStatus.Visibility = Visibility.Visible

        txbPräsentationsschirm.MaxWidth = maxWidth
        txbPräsentationsschirm.TextAlignment = TextAlignment.Center
        txbPräsentationsschirm.TextWrapping = TextWrapping.Wrap
        txbPräsentationsschirm.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))
        txbPräsentationsschirm.Visibility = Visibility.Visible

        lblInitialisiereBilder.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))
        lblInitialisiereBilder.Visibility = Visibility.Visible

        lblInitialisiereVerzeichnisse.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))
        lblInitialisiereVerzeichnisse.Visibility = Visibility.Visible

        'Sanduhren starten
        StartHourglassAnimation(hourglassBilder, rtBilder)
        StartHourglassAnimation(hourglassVerz, rtVerz)

        CheckYourMail()

    End Sub

    Private Sub BildauswahlMain_ErsteVerzeichnisseGefunden()

        Dispatcher.Invoke(Sub()
                              lblInitialisiereVerzeichnisse.Content &= "OK"
                              StopHourglassAnimation(rtVerz)
                              hourglassVerz.Visibility = Visibility.Collapsed
                              hasFirstVerzeichnisse = True
                              VersucheErstesBildZuLaden()
                          End Sub)

    End Sub

    Private Sub BildauswahlMain_ErsteBilderGefunden()

        Dispatcher.Invoke(Sub()
                              lblInitialisiereBilder.Content &= "OK"
                              StopHourglassAnimation(rtBilder)
                              hourglassBilder.Visibility = Visibility.Collapsed
                              hasFirstBilder = True
                              VersucheErstesBildZuLaden()
                          End Sub)

    End Sub

    Private Sub VersucheErstesBildZuLaden()
        Dispatcher.Invoke(Sub()
                              'Erst laden, wenn beide Events eingetroffen sind
                              If hasFirstBilder AndAlso hasFirstVerzeichnisse Then

                                  'Init-Overlay ausblenden
                                  pnlStatus.Visibility = Visibility.Collapsed
                                  txbPräsentationsschirm.Visibility = Visibility.Collapsed

                                  'Erstes Bild laden
                                  LadeErstesBild()
                              End If
                          End Sub)
    End Sub

    Private Sub LadeErstesBild()
        'Wählt die ersten Bilder zur Anzeige aus. 

#Region "Erstes Bild laden"
        Do
            If aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then
                Do
                    aktuellesVerzeichnis = GetPicturesByDirectory()
                Loop Until aktuellesVerzeichnis.Count >= 1
                '.Count zählt von 1 bis 2, aktuellesVerzeichnisCounter von 0 bis 1...
                aktuellesVerzeichnisCounter = 0
                bildPfad = aktuellesVerzeichnis(0)

                'Präsentationsmodus aktivieren, wenn eingestellt
                If aktuelleSettings.Präsentationsschirm Then
                    präsentationAnzeigen = True
                Else
                    präsentationAnzeigen = False
                End If

            Else
                bildPfad = GetPictures(1).Item(0)
                präsentationAnzeigen = False
            End If

            aktuellesImage = GetPictureByName(bildPfad)

        Loop Until aktuellesImage IsNot Nothing

        'Shader anwenden 
        LadeNeuenShader(True)

        If aktiverShader IsNot Nothing Then
            aktuellesImage = aktiverShader.RunShader(aktuellesImage, bildPfad, GetNativeScreenResolution())
        End If

        'Für Anzeige in imgAnzeige und in Transitionen konvertieren
        aktuellesBild = ConvertImageToBitmapImage(aktuellesImage)

#End Region

        LogHandling.LogDebug("Modul SSS 3.0 - wpfModulMain.LadeErstesBild(): aktuellesBild ausgewählt: " & bildPfad)

        If präsentationAnzeigen Then
            PräsentationsschirmAnzeigen()
        Else
            BildAnzeigen()
        End If

    End Sub

    Private Sub StartHourglassAnimation(elem As UIElement, ByRef rt As RotateTransform)
        If rt Is Nothing Then
            rt = New RotateTransform(0)
            Dim fe = TryCast(elem, FrameworkElement)
            If fe IsNot Nothing Then fe.RenderTransformOrigin = New Windows.Point(0.5, 0.5)
            fe.RenderTransform = rt
        End If

        Dim anim As New DoubleAnimation() With {
        .From = 0, .To = 360,
        .Duration = TimeSpan.FromSeconds(1),
        .RepeatBehavior = RepeatBehavior.Forever
    }

        rt.BeginAnimation(RotateTransform.AngleProperty, anim)
    End Sub

    Private Sub StopHourglassAnimation(rt As RotateTransform)
        If rt IsNot Nothing Then
            rt.BeginAnimation(RotateTransform.AngleProperty, Nothing)
        End If
    End Sub


    'Hauptschleife
    Private Async Sub TmrModul_Tick(sender As Object, e As EventArgs) Handles tmrModul.Tick
        'Nac Beendigung der Anzeige des Bildes gemäß Anzeigedauer startet der Timer die nächste Transition
        'oder, falls keine ausgewählt ist, initiiert den Bildwechsel.

        If transitionIstAktiv Then Exit Sub

        'Timer beenden
        tmrModul.Stop()

        ' Kleine Verzögerung, damit Stop() garantiert fertig ist
        Await Task.Delay(750)

        'Falls der Päsentationsschirm angezeigt werden soll, diesen starten
        If präsentationAnzeigen Then

            'Status setzen
            transitionIstAktiv = False

            'Präsentationsschirm anzeigen
            BildWechseln()
            PräsentationsschirmAnzeigen()

        Else
            'Sonst die Transition laufen lassen

            txbPräsentationsschirm.Visibility = Visibility.Collapsed
            imgAnzeige.Visibility = Visibility.Visible
            LadeNeueTransition(False)

            If listOfEnabledTransitions.Count > 0 AndAlso aktuellesBild IsNot Nothing Then

                'Transition vorbereiten und Status setzen
                sizeWinForm = New Size(Me.RenderSize.Width, Me.RenderSize.Height)
                transitionIstAktiv = True

                'Transition starten
                aktiveTransition.RunTransition(aktuellesBild, PictureBoxSizeMode.Zoom, neuesBild, PictureBoxSizeMode.Zoom, sizeWinForm)

            Else
                'Wenn weder der Präsentationsschirm gezeigt werden soll, noch eine legitime Transition gefunden
                'wurde, dann muss der Timer halt selber ran...

                'Status setzen
                transitionIstAktiv = False

                'Bildanzeige starten
                BildWechseln()
                BildAnzeigen()

            End If
        End If

    End Sub

    Private Sub Transition_TransitionIsRunning(state As Boolean)
        'Wechselt das aktuelle Bild nach erfolgreichem Abschluss der Transition und startet tmrModul 

        'Sofort raus, falls die Transition noch läuft
        If state Then Exit Sub

        'tmrModul sicherheitshalber noch einmal stoppen
        tmrModul.Stop()

        'Status setzen
        transitionIstAktiv = False

        'Hier beginnt die Bildanzeige
        BildWechseln()
        BildAnzeigen()

    End Sub

    Private Sub Transition_TransitionFrameIstFertig(rtb As RenderTargetBitmap)
        'Bildanzeige während Transitionen

        imgAnzeige.Source = Nothing

        'Notwendig, um "Leere Frames" zu verhindern
        GC.Collect()
        GC.WaitForPendingFinalizers()

        imgAnzeige.Source = rtb

    End Sub

    Private Sub BildAnzeigen()
        'Eigentliche Anzeige des aktuellen Bildes sowie dazugehörige Verwaltungsaufgaben

        imgAnzeige.Source = aktuellesBild
        imgAnzeige.Visibility = Visibility.Visible

        'BildInfo des Bildes aktualisieren
        If aktuelleSettings.BildInfoAnzeigen Then
            If ModulMain.sssInfo IsNot Nothing AndAlso bildPfad IsNot Nothing Then
                ModulMain.sssInfo.RefreshLabels(bildPfad)
                ModulMain.sssInfo.Refresh()
                ModulMain.sssInfo.BringToFront()
            End If
        End If

        'Liste der letzten 10 Bilder befüllen und ggf. das erste Element wieder aus der Liste löschen.
        If bildPfad IsNot Nothing Then
            listeDerZuletztAngezeigtenBilder.Add(bildPfad)
            If listeDerZuletztAngezeigtenBilder.Count > 10 Then
                listeDerZuletztAngezeigtenBilder.RemoveAt(0)
            End If
        End If

        'Jetzt das nächste Bild laden
        NächstesBildLaden()

        'tmrModul starten
        tmrModul.Interval = TimeSpan.FromSeconds(aktuelleSettings.Anzeigedauer)
        tmrModul.Start()

    End Sub

    Private Sub NächstesBildLaden()
        'Sucht ein neues Bild aus und bestimmt, ob eine Transition oder der Präsentationsschirm als
        'nächstes angezeigt werden soll

        'Neues Bild laden. 
        Do
            If aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then

                aktuellesVerzeichnisCounter += 1

                'Wenn wir das letzte Bild des aktuellen Verzeichnisses angezeigt haben, neues Verzeichnis laden
                If aktuellesVerzeichnisCounter >= aktuellesVerzeichnis.Count Then

                    aktuellesVerzeichnisCounter = 0

                    Do
                        aktuellesVerzeichnis = GetPicturesByDirectory()
                    Loop Until aktuellesVerzeichnis.Count >= 1

                    If aktuelleSettings.Präsentationsschirm Then
                        präsentationAnzeigen = True
                    Else
                        präsentationAnzeigen = False
                    End If

                End If

                bildPfad = aktuellesVerzeichnis(aktuellesVerzeichnisCounter)

            Else
                präsentationAnzeigen = False
                bildPfad = GetPictures(1).Item(0)
            End If

            neuesImage = GetPictureByName(bildPfad)

        Loop Until neuesImage IsNot Nothing

        LadeNeuenShader(False)

        If aktiverShader IsNot Nothing Then
            neuesImage = aktiverShader.RunShader(neuesImage, bildPfad, GetNativeScreenResolution())
        End If

        'Für Transitionen und imgAnzeige konvertieren
        neuesBild = ConvertImageToBitmapImage(neuesImage)

    End Sub

    Private Sub BildWechseln()
        'Hilfsfunktion zum tatsächlichen Wechsel der Bilder (wird je nach Präsentationsschirm/Transition/direkter
        'Wechsel zu unterschiedlichen Zeitpunkten aufgerufen)

        aktuellesImage = neuesImage
        aktuellesBild = neuesBild

    End Sub

    Private Sub PräsentationsschirmAnzeigen()
        'Zeigt den Präsentationsschirm an, der die Bilder in voller Größe anzeigt
        'und den Timer für die Präsentation startet

        Dim präsentationsText As String
        Dim präsentationsZeit As Integer

        präsentationsText = Path.GetFileName(Path.GetDirectoryName(aktuellesVerzeichnis(0)))

        txbPräsentationsschirm.Text = präsentationsText
        txbPräsentationsschirm.Visibility = Visibility.Visible
        imgAnzeige.Visibility = Visibility.Hidden

        präsentationAnzeigen = False

        'Präsentationstext so lange wie ein Bild anzeigen, aber höchstens 15 Sekunden
        präsentationsZeit = Math.Min(aktuelleSettings.Anzeigedauer, 15)
        tmrPresentation.Interval = TimeSpan.FromSeconds(präsentationsZeit)
        tmrPresentation.Start()

        'Sicherheitshalber tmrModul anhalten
        tmrModul.Stop()

    End Sub

    Private Sub tmrPresentation_Tick() Handles tmrPresentation.Tick
        'Beendet die Anzeige des Präsentationsschirms

        txbPräsentationsschirm.Visibility = Visibility.Collapsed
        tmrPresentation.Stop()
        BildAnzeigen()

    End Sub

    'Hilfs- und Verwaltungsfunktionen
    'Transitionen
    Private Sub LadeNeueTransition(istInitialisierung As Boolean)

        LegitimeTransitionsListeErstellen()

        'Transition aussuchen
        If listOfEnabledTransitions.Count > 0 Then
            Select Case aktuelleSettings.TransitionsReihenfolge
                Case "In Reihenfolge"

                    If istInitialisierung Then
                        neueTransition = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition")
                        If neueTransition = Nothing Then neueTransition = ""
                        neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, neueTransition)
                    Else
                        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition", aktiveTransition.TransitionName)
                        neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, aktiveTransition.TransitionName)
                    End If

                Case "Zufällig"

                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))

                Case "Zufällig bei Start"
                    'TODO - Echte Logik einbauen, so dass NUR bei Änderungen der Einstellungen eine "Not-Transition" geladen wird.

                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))

                Case "In Reihenfolge bei Start"
                    'TODO - Echte Logik einbauen, so dass NUR bei Änderungen der Einstellungen eine "Not-Transition" geladen wird.

                    If istInitialisierung Then
                        neueTransition = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition")
                        If neueTransition = Nothing Then neueTransition = ""
                        neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, neueTransition)
                    Else
                        neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, aktiveTransition.TransitionName)
                    End If

                Case Else

                    'Entspricht "Zufällig bei Start"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))

            End Select

            If aktiveTransition IsNot Nothing Then
                'Alte Handler entfernen
                RemoveHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
                RemoveHandler aktiveTransition.TransitionFrameIstFertig, AddressOf Transition_TransitionFrameIstFertig
            End If

            aktiveTransition = TransitionByNameLoader.LadeTransitionNachName(neueTransition)

            If aktiveTransition IsNot Nothing Then
                'Neue Handler hinzufügen
                AddHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
                AddHandler aktiveTransition.TransitionFrameIstFertig, AddressOf Transition_TransitionFrameIstFertig
            End If

        End If

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

    'Shader
    Private Sub LadeNeuenShader(istInitialisierung As Boolean)

        LegitimeShaderListeErstellen()

        'Aktiven Shader laden 
        If listOfEnabledShaders.Count > 0 Then
            Select Case aktuelleSettings.ShaderReihenfolge
                Case "In Reihenfolge"

                    If istInitialisierung Then
                        neuerShader = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader")
                        If neuerShader = Nothing Then neuerShader = ""
                        neuerShader = GetNextAlphabeticItemName(listOfEnabledShaders, neuerShader)
                    Else
                        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader", aktiverShader.ShaderName)
                        neuerShader = GetNextAlphabeticItemName(listOfEnabledShaders, aktiverShader.ShaderName)
                    End If

                Case "Zufällig"

                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))

                Case "Zufällig bei Start"
                    'TODO - Echte Logik einbauen, so dass NUR bei Änderungen der Einstellungen eine "Not-Shader" geladen wird.

                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))

                Case "In Reihenfolge bei Start"
                    'TODO - Echte Logik einbauen, so dass NUR bei Änderungen der Einstellungen eine "Not-Shader" geladen wird.

                    If istInitialisierung Then
                        neuerShader = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader")
                        If neuerShader = Nothing Then neuerShader = ""
                        neuerShader = GetNextAlphabeticItemName(listOfEnabledShaders, neuerShader)
                    Else
                        neuerShader = GetNextAlphabeticItemName(listOfEnabledShaders, aktiverShader.ShaderName)
                    End If

                Case Else
                    'Entspricht "Zufällig bei Start"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
            End Select

            aktiverShader = ShaderByNameLoader.LadeShaderNachName(neuerShader)

        End If

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

    'Settings & Verwaltung
    Private Sub CheckYourMail()
        'Liest die aktuelleSettings ein

        aktuelleSettings = GetSettings(Of ModulMain.SettingsModul_SSS)(ModulMain.nameModul)

        'Hintergrund- und TextBox-Farbe setzen
        hintergrundWM = SDColorToWMColor(HintergrundFarbeSaver)
        Me.Background = New SolidColorBrush(hintergrundWM)
        txbPräsentationsschirm.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))

        'Timer anpassen
        tmrModul.Interval = TimeSpan.FromSeconds(aktuelleSettings.Anzeigedauer)

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

    'Pause-Modus Veraltung
    Public Sub FortsetzenNachPause()
        ' Sicherstellen, dass wir im normalen Anzeigezustand sind
        txbPräsentationsschirm.Visibility = Visibility.Collapsed
        imgAnzeige.Visibility = Visibility.Visible
        transitionIstAktiv = False

        ' Das zuletzt aktive Bild wieder anzeigen
        If aktuellesBild IsNot Nothing Then
            imgAnzeige.Source = aktuellesBild
        End If

        ' Falls noch kein nächstes Bild vorbereitet ist, jetzt laden
        If neuesBild Is Nothing Then
            NächstesBildLaden()
        End If

        ' Timer sauber neu starten
        tmrModul.Stop()
        tmrModul.Interval = TimeSpan.FromSeconds(aktuelleSettings.Anzeigedauer)
        tmrModul.Start()
    End Sub

    'Ende
    Private Sub wpfModulMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        'Aufräumen
        If transitionIstAktiv Then
            aktiveTransition.StopTransition()
        End If

        'Handler entfernen
        RemoveHandler ModulMain.YouHaveMail_SSS, AddressOf CheckYourMail
        RemoveHandler BildauswahlMain.ErsteBilderGefunden, AddressOf BildauswahlMain_ErsteBilderGefunden
        RemoveHandler BildauswahlMain.ErsteVerzeichnisseGefunden, AddressOf BildauswahlMain_ErsteVerzeichnisseGefunden

        'Shader & Transitionen freigeben
        aktiverShader = Nothing
        aktiveTransition = Nothing

        'Timer freigeben
        tmrModul = Nothing

        ' Bilder freigeben
        imgAnzeige.Source = Nothing

        ' Inhalte leeren
        Me.Content = Nothing

        ' Garbage Collection
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()

    End Sub

End Class
