Imports System.Windows.Forms
Imports SlideShowWPFModul.Mandelbrot.ModulMain
Imports System.Windows.Interop
Imports System.Windows.Threading
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling
Imports SlideShowTools.XmlHandling

Public Class wpfModulMain

#Region "Variablendeklaration"
    'Variablendeklaration & Datenstrukturen

    Private Shared aktuelleSettings As ModulMain.ModulSettings_Mandelbrot

    'Timer
    Public WithEvents tmrModul As New DispatcherTimer()
    Public WithEvents tmrPresentation As New DispatcherTimer()

    'Initialisierungsphase
    Private hintergrundWM As Windows.Media.Color

    'Start- und Zielpunkte
    Private startpunkt As MandelbrotZiel
    Private listeDerZiele As List(Of MandelbrotZiel)
    Private aktuellesZiel As MandelbrotZiel

    'Sonstiges
    Private rnd As New Random()

    Public Structure MandelbrotZiel
        Public Property Name As String
        Public Property CenterX As Double
        Public Property CenterY As Double
        Public Property TargetScale As Double
        Public Property MaxIterations As Integer
    End Structure
#End Region

    Public Sub New()
        'Erzeugt und initialisiert das Fenster und seine Komponenten

        'Eventhandler
        AddHandler ModulMain.YouHaveMail_Mandelbrot, AddressOf CheckYourMail

        'Initialisierung der Komponenten
        InitializeComponent()

        ' Fenster in den Vordergrund und maximiert
        Me.WindowState = WindowState.Maximized

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Weitere Initialisierungen der Form

        'Versuch, das Fenster in den Vordergrund zu bringen 
        Me.Topmost = False
        Me.ShowActivated = True
        Me.Show()
        Me.Activate()

        'Hintergrundfarbe setzen
        hintergrundWM = SDColorToWMColor(HintergrundFarbeSaver)
        Me.Background = New SolidColorBrush(hintergrundWM)

        'Initialisierungslabel einstellen

        'Handle des aktuellen Fensters holen
        Dim helper As New WindowInteropHelper(Me)
        Dim hwnd As IntPtr = helper.Handle

        'Bildschirm ermitteln, auf dem sich das Fenster befindet
        Dim currentScreen As Screen = Screen.FromHandle(hwnd)

        'Maximalbreite = 90 % der aktuellen Bildschirmbreite
        Dim maxWidth As Double = currentScreen.Bounds.Width * 0.9

        'Präsentationslabel einstellen und anzeigen
        txbPräsentationsschirm.MaxWidth = maxWidth
        txbPräsentationsschirm.TextAlignment = TextAlignment.Center
        txbPräsentationsschirm.TextWrapping = TextWrapping.Wrap
        txbPräsentationsschirm.Foreground = New SolidColorBrush(InvertWMColor(hintergrundWM))
        PräsentationsschirmAnzeigen()

        'Aktuelle Settings abholen
        CheckYourMail()

        'Startpunkt setzen
        startpunkt.Name = "Apfelmännchen"
        startpunkt.CenterX = -0.5
        startpunkt.CenterY = 0.0
        startpunkt.TargetScale = 3.0

        'aktuellenZielpunkt auswählen
        With XMLDatensatzPerIndex("MandelbrotZiele.XML.", "Target", rnd.Next(XMLDatensaetzeCount("MandelbrotZiele.XML", "Target")))
            aktuellesZiel.Name = CStr(.Attribute("Name"))
            aktuellesZiel.CenterX = CDbl(.Element("CenterX"))
            aktuellesZiel.CenterY = CDbl(.Element("CenterY"))
            aktuellesZiel.TargetScale = CDbl(.Element("TargetScale"))
            aktuellesZiel.MaxIterations = CInt(.Element("MaxIterations"))
        End With

        'Timer Modul starten
        tmrModul.Interval = TimeSpan.FromSeconds(60)
        tmrModul.Start()

    End Sub

    Private Async Sub TmrModul_Tick(sender As Object, e As EventArgs) Handles tmrModul.Tick
        'Neue Animation, neues Ziel



    End Sub

    Private Sub PräsentationsschirmAnzeigen()
        'Zeigt den Präsentationsschirm mit dem Text "Mandelbrot" für 15 Sekunden an.

        Dim präsentationsText As String
        Dim präsentationsZeit As Integer

        präsentationsText = "Mandelbrot"

        txbPräsentationsschirm.Text = präsentationsText
        txbPräsentationsschirm.Visibility = Visibility.Visible

        'Präsentationstext 15 Sekunden
        präsentationsZeit = 15
        tmrPresentation.Interval = TimeSpan.FromSeconds(präsentationsZeit)
        tmrPresentation.Start()

        'Sicherheitshalber tmrModul anhalten
        tmrModul.Stop()

    End Sub

    Private Sub tmrPresentation_Tick() Handles tmrPresentation.Tick
        'Beendet die Anzeige des Präsentationsschirms

        txbPräsentationsschirm.Visibility = Visibility.Collapsed
        tmrPresentation.Stop()

        'Und Timer der Hauptschleife reaktivieren
        tmrModul.Start()

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

        'HIER DIE LOGIK FÜR DEN WIEDEREINSTIEG NACH PAUSE

        ' Timer sauber neu starten
        tmrModul.Stop()
        tmrModul.Interval = TimeSpan.FromSeconds(15)
        tmrModul.Start()
    End Sub

    'Ende
    Private Sub wpfModulMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        'Aufräumen

        'Handler entfernen
        RemoveHandler ModulMain.YouHaveMail_Mandelbrot, AddressOf CheckYourMail

        'Timer freigeben
        tmrModul = Nothing
        ' Inhalte leeren
        Me.Content = Nothing

        ' Garbage Collection
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()

    End Sub

    Private Sub CheckYourMail()
        'Holt die aktuelleSettings aus der SettingsHandling.SettingsInbox ab.

        aktuelleSettings = GetSettings(Of ModulSettings_Mandelbrot)(nameModul)

    End Sub

End Class