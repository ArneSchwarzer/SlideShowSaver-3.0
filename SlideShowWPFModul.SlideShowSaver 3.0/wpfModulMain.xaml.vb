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
Imports SlideShowTools.BildHandling
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.FileHandling
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
    Private WithEvents tmrInitialisierung As New DispatcherTimer()

    'Initialisierungsphase
    Private initialesBildWurdeGeladen As Boolean
    Private hintergrundWM As Windows.Media.Color
    Private sbBilder As Storyboard
    Private sbVerz As Storyboard

    'Events
    Public Event DarstellungIstBereit()

    Private darstellungsbereitschaftWurdeGemeldet As Boolean = False

    'Sonstiges
    Private rnd As New Random()

#End Region

#Region "Konstruktor, Fensterstart und Settings"

    'Initialisierungen
    Public Sub New()
        'Erzeugt und initialisiert das Fenster und seine Komponenten.

        InitializeComponent()

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

        BereiteInitialisierungsanzeigeVor()
        StarteInitialisierungspruefung()

        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, New Action(AddressOf MeldeDarstellungsbereitschaft))

    End Sub

    Public Sub AktualisiereSettings(neueSettings As ModulMain.SettingsModul_SSS)
        'Übernimmt aktualisierte Moduleinstellungen direkt von ModulMain.

        aktuelleSettings = neueSettings

        hintergrundWM = SDColorToWMColor(HintergrundFarbeSaver)

        Me.Background = New SolidColorBrush(hintergrundWM)

        txbPräsentationsschirm.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))

        tmrModul.Interval = TimeSpan.FromSeconds(aktuelleSettings.Anzeigedauer)

    End Sub

    Private Sub MeldeDarstellungsbereitschaft()
        'Meldet den vollständig aufgebauten Initialisierungsbildschirm einmalig an ModulMain.

        If darstellungsbereitschaftWurdeGemeldet Then
            Exit Sub
        End If

        darstellungsbereitschaftWurdeGemeldet = True

        Me.UpdateLayout()

        RaiseEvent DarstellungIstBereit()

    End Sub

#End Region

#Region "Initialisierung und erste Bildauswahl"

    Private Sub BereiteInitialisierungsanzeigeVor()
        'Zeigt nur den Status des tatsächlich benötigten Auswahlbestands an.

        initialesBildWurdeGeladen = False

        pnlStatus.Visibility = Visibility.Visible

        txbPräsentationsschirm.Visibility = Visibility.Visible

        Select Case aktuelleSettings.Bildauswahl

            Case "Zufallsverzeichnis"

                lblInitialisiereBilder.Visibility = Visibility.Collapsed
                hourglassBilder.Visibility = Visibility.Collapsed

                StopHourglassAnimation(rtBilder)

                lblInitialisiereVerzeichnisse.Visibility = Visibility.Visible
                hourglassVerz.Visibility = Visibility.Visible

            Case Else

                lblInitialisiereVerzeichnisse.Visibility = Visibility.Collapsed
                hourglassVerz.Visibility = Visibility.Collapsed

                StopHourglassAnimation(rtVerz)

                lblInitialisiereBilder.Visibility = Visibility.Visible
                hourglassBilder.Visibility = Visibility.Visible

        End Select

    End Sub

    Private Sub StarteInitialisierungspruefung()
        'Prüft in kurzen Abständen, ob ein erstes Bild verfügbar ist.

        tmrInitialisierung.Stop()

        tmrInitialisierung.Interval = TimeSpan.FromMilliseconds(200)

        'Unmittelbar prüfen, bevor der erste Timer-Tick abgewartet wird.
        If VersucheErstesBildZuLaden() Then
            Exit Sub
        End If

        tmrInitialisierung.Start()

    End Sub

    Private Sub TmrInitialisierung_Tick(sender As Object, e As EventArgs) Handles tmrInitialisierung.Tick
        'Prüft, ob der benötigte Bildbestand inzwischen verfügbar ist.

        If initialesBildWurdeGeladen Then

            tmrInitialisierung.Stop()

            Exit Sub

        End If

        VersucheErstesBildZuLaden()

    End Sub

    Private Function VersucheErstesBildZuLaden() As Boolean
        'Versucht, das erste für den gewählten Modus verfügbare Bild zu laden.

        Dim ausgewaehlterPfad As String
        Dim ausgewaehltesBild As Image
        Dim verzeichnisBilder As List(Of String)
        Dim bildIndex As Integer

        ausgewaehlterPfad = Nothing
        ausgewaehltesBild = Nothing
        verzeichnisBilder = Nothing
        bildIndex = -1

        If initialesBildWurdeGeladen Then
            Return True
        End If

        Select Case aktuelleSettings.Bildauswahl

            Case "Zufallsverzeichnis"

                If Not TryGetRandomDirectoryPictures(verzeichnisBilder) Then

                    Return False

                End If

                If Not TryLadeBildAusVerzeichnis(verzeichnisBilder, 0, ausgewaehlterPfad, ausgewaehltesBild, bildIndex) Then

                    Return False

                End If

                aktuellesVerzeichnis = verzeichnisBilder
                aktuellesVerzeichnisCounter = bildIndex

                präsentationAnzeigen = aktuelleSettings.Präsentationsschirm

            Case Else

                If Not TryLadeZufaelligesEinzelbild(ausgewaehlterPfad, ausgewaehltesBild) Then

                    Return False

                End If

                aktuellesVerzeichnis = New List(Of String)
                aktuellesVerzeichnisCounter = 0
                präsentationAnzeigen = False

        End Select

        bildPfad = ausgewaehlterPfad
        aktuellesImage = ausgewaehltesBild

        LadeNeuenShader(True)

        If aktiverShader IsNot Nothing Then

            aktuellesImage = aktiverShader.RunShader(aktuellesImage, bildPfad, GetNativeScreenResolution())

        End If

        aktuellesBild = ConvertImageToBitmapImage(aktuellesImage)

        initialesBildWurdeGeladen = True

        tmrInitialisierung.Stop()

        StopHourglassAnimation(rtBilder)
        StopHourglassAnimation(rtVerz)

        pnlStatus.Visibility = Visibility.Collapsed
        txbPräsentationsschirm.Visibility = Visibility.Collapsed

        If präsentationAnzeigen Then

            PräsentationsschirmAnzeigen()

        Else

            BildAnzeigen()

        End If

        Return True

    End Function

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

    Private Function TryLadeZufaelligesEinzelbild(ByRef ausgewaehlterPfad As String,
                                                  ByRef ausgewaehltesBild As Image) As Boolean
        'Versucht mehrere unterschiedliche vorbereitete Bildpfade zu laden.

        Dim versuchtePfade As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Dim kandidatPfad As String
        Dim kandidatBild As Image
        Dim maximaleVersuche As Integer
        Dim versuch As Integer

        ausgewaehlterPfad = Nothing
        ausgewaehltesBild = Nothing

        maximaleVersuche =
        Math.Min(
            Math.Max(
                GetPreparedPictureCount(),
                1),
            25)

        For versuch = 1 To maximaleVersuche

            kandidatPfad = Nothing
            kandidatBild = Nothing

            If Not TryGetRandomPicture(kandidatPfad) Then

                Return False

            End If

            If Not versuchtePfade.Add(kandidatPfad) Then

                Continue For

            End If

            kandidatBild = LadeBild(kandidatPfad)

            If kandidatBild IsNot Nothing Then

                ausgewaehlterPfad = kandidatPfad
                ausgewaehltesBild = kandidatBild

                Return True

            End If

        Next

        Return False

    End Function

    Private Function TryLadeBildAusVerzeichnis(
    verzeichnisBilder As List(Of String),
    startIndex As Integer,
    ByRef ausgewaehlterPfad As String,
    ByRef ausgewaehltesBild As Image,
    ByRef ausgewaehlterIndex As Integer
) As Boolean
        'Sucht ab dem angegebenen Index zyklisch nach einem ladbaren Bild.

        Dim index As Integer
        Dim pruefIndex As Integer
        Dim kandidatPfad As String
        Dim kandidatBild As Image

        ausgewaehlterPfad = Nothing
        ausgewaehltesBild = Nothing
        ausgewaehlterIndex = -1

        If verzeichnisBilder Is Nothing OrElse verzeichnisBilder.Count = 0 Then

            Return False

        End If

        If startIndex < 0 Then
            startIndex = 0
        End If

        For index = 0 To verzeichnisBilder.Count - 1

            pruefIndex = (startIndex + index) Mod verzeichnisBilder.Count
            kandidatPfad = verzeichnisBilder(pruefIndex)
            kandidatBild = LadeBild(kandidatPfad)

            If kandidatBild IsNot Nothing Then

                ausgewaehlterPfad = kandidatPfad
                ausgewaehltesBild = kandidatBild
                ausgewaehlterIndex = pruefIndex

                Return True

            End If

        Next

        Return False

    End Function

#End Region

#Region "Bildanzeige und Hauptschleife"

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
        'Bereitet das nächste Bild vor. Ist momentan kein neues Bild verfügbar,
        'wird das aktuelle Bild beziehungsweise Verzeichnis wiederholt.

        Dim naechsterPfad As String
        Dim naechstesImage As Image
        Dim neueVerzeichnisBilder As List(Of String)
        Dim naechsterIndex As Integer
        Dim bildWurdeGeladen As Boolean
        Dim verzeichnisWurdeGewechselt As Boolean

        naechsterPfad = Nothing
        naechstesImage = Nothing
        neueVerzeichnisBilder = Nothing
        naechsterIndex = -1
        bildWurdeGeladen = False
        verzeichnisWurdeGewechselt = False

        Select Case aktuelleSettings.Bildauswahl

            Case "Zufallsverzeichnis"

                If aktuellesVerzeichnis Is Nothing OrElse aktuellesVerzeichnis.Count = 0 Then

                    If TryGetRandomDirectoryPictures(neueVerzeichnisBilder) Then

                        aktuellesVerzeichnis = neueVerzeichnisBilder

                        aktuellesVerzeichnisCounter = 0
                        verzeichnisWurdeGewechselt = True

                    End If

                Else

                    naechsterIndex = aktuellesVerzeichnisCounter + 1

                    If naechsterIndex >= aktuellesVerzeichnis.Count Then

                        If TryGetRandomDirectoryPictures(neueVerzeichnisBilder) Then

                            aktuellesVerzeichnis = neueVerzeichnisBilder

                        End If

                        naechsterIndex = 0
                        verzeichnisWurdeGewechselt = True

                    End If

                End If

                If aktuellesVerzeichnis IsNot Nothing AndAlso aktuellesVerzeichnis.Count > 0 Then

                    bildWurdeGeladen = TryLadeBildAusVerzeichnis(aktuellesVerzeichnis, naechsterIndex,
                                                                 naechsterPfad, naechstesImage, naechsterIndex)

                End If

                If bildWurdeGeladen Then

                    aktuellesVerzeichnisCounter = naechsterIndex

                    präsentationAnzeigen = verzeichnisWurdeGewechselt AndAlso
                    aktuelleSettings.Präsentationsschirm

                End If

            Case Else

                bildWurdeGeladen = TryLadeZufaelligesEinzelbild(naechsterPfad, naechstesImage)

                präsentationAnzeigen = False

        End Select

        If Not bildWurdeGeladen Then
            'Es ist momentan kein neues ladbares Bild verfügbar.
            'Das aktuelle Bild wird als nächstes Bild wiederverwendet.

            neuesImage = aktuellesImage
            neuesBild = aktuellesBild

            Return

        End If

        bildPfad = naechsterPfad
        neuesImage = naechstesImage

        LadeNeuenShader(False)

        If aktiverShader IsNot Nothing Then

            neuesImage = aktiverShader.RunShader(neuesImage, bildPfad, GetNativeScreenResolution())

        End If

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


#End Region

#Region "Transitionen"

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


#End Region

#Region "Shader"

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


#End Region

#Region "Tastatur- und Mausereignisse"

    'Settings & Verwaltung
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


#End Region

#Region "Pausemodus"

    'Pause-Modus Veraltung
    Public Sub FortsetzenNachPause()
        'Setzt die Darstellung nach dem Pausemodus fort.

        txbPräsentationsschirm.Visibility = Visibility.Collapsed
        imgAnzeige.Visibility = Visibility.Visible

        transitionIstAktiv = False

        If Not initialesBildWurdeGeladen Then

            StarteInitialisierungspruefung()

            Exit Sub

        End If

        If aktuellesBild IsNot Nothing Then
            imgAnzeige.Source = aktuellesBild
        End If

        If neuesBild Is Nothing Then
            NächstesBildLaden()
        End If

        tmrModul.Stop()
        tmrModul.Interval = TimeSpan.FromSeconds(aktuelleSettings.Anzeigedauer)
        tmrModul.Start()

    End Sub

#End Region

#Region "Aufräumen und Fensterende"

    'Ende
    Private Sub wpfModulMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        'Aufräumen
        If transitionIstAktiv Then
            aktiveTransition.StopTransition()
        End If

        'Shader & Transitionen freigeben
        aktiverShader = Nothing
        aktiveTransition = Nothing

        'Timer stoppen und freigeben
        tmrInitialisierung.Stop()
        tmrModul.Stop()
        tmrPresentation.Stop()

        tmrInitialisierung = Nothing
        tmrModul = Nothing
        tmrPresentation = Nothing

        ' Bilder freigeben
        imgAnzeige.Source = Nothing

        ' Inhalte leeren
        Me.Content = Nothing

        ' Garbage Collection
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()

    End Sub

#End Region

End Class