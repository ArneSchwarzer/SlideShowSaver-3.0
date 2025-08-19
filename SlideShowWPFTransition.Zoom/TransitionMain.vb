Imports System.Drawing
Imports System.Windows.Forms
Imports System.Windows.Threading
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging.LogHandling
Imports SlideShowTools
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling

Public Class TransitionMain
    Implements ISlideShowTransition

    'Variablendeklaration

    'Settings und Konstanten
    Public Const SLIDESHOWTRANSITION_ZOOM_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Zoom\"
    Public Const nameTransition As String = "Zoom"
    Private aktuelleSettings As SlideShowTransitionSettings_Zoom

    'Timer und Zeitmanagement
    Private WithEvents tmrDuration As New Timer
    Private startTime As DateTime
    Private dauerInMS As Integer
    Private Const FPS As Integer = 120

    'Transitions-Bilder
    Private oldBmpSource As BitmapSource
    Private newBmpSource As BitmapSource
    Private oldBmpGerahmt As RenderTargetBitmap
    Private newBmpGerahmt As RenderTargetBitmap

    'Animation
    Private aktuellerAnkerpunktOld As String
    Private aktuellerAnkerpunktNew As String

    Private clntSize As Windows.Size

    'Rendering
    Private drawAction As Action(Of DrawingContext, Windows.Size)
    Private frameTimer As DispatcherTimer
    Private renderSize As Windows.Size
    Private rtbCache As RenderTargetBitmap
    Private stopRequested As Boolean = False

    Public Structure SlideShowTransitionSettings_Zoom
        Public ankerpunkte As List(Of String)
        Public geschwindigkeit As Integer
        Public gleicherAnkerpunkt As Boolean
    End Structure

    'Eigenschaften
    Public ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName
        Get
            Return nameTransition
        End Get
    End Property

    Public ReadOnly Property TransitionKurzBeschreibung As String Implements ISlideShowTransition.TransitionKurzBeschreibung
        Get
            Return "Schrumpft das alte Bild und lässt das neue wachsen."
        End Get
    End Property

    Public ReadOnly Property TransitionVersion As Version Implements ISlideShowTransition.TransitionVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    'Events
    Public Event TransitionIsRunning As ISlideShowTransition.TransitionIsRunningEventHandler Implements ISlideShowTransition.TransitionIsRunning
    Public Event TransitionFrameIstFertig As ISlideShowTransition.TransitionFrameIstFertigEventHandler Implements ISlideShowTransition.TransitionFrameIstFertig

    'Start, Stop & OptionsDialog
    Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As BitmapImage, picBoxModeNew As PictureBoxSizeMode, clientSize As Size, Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition
        'Bereitet die Animation vor und startet den Render-Loop
        Dim ankerArray() As String = {"N", "NO", "O", "SO", "S", "SW", "W", "NW", "Z"}
        Dim rnd As New Random

        'Interne Initialisierungen & Formalitäten
        ReadTransitionSettingsFromRegistryOrDefaults()
        StoreSettings(nameTransition, aktuelleSettings)

        RaiseEvent TransitionIsRunning(True)

        'Konvertieren und in lokalen Variablen speichern
        clntSize = New Windows.Size(clientSize.Width, clientSize.Height)

        ' Bilder vorbereiten
        oldBmpGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
        newBmpGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)
        oldBmpSource = CType(oldBmpGerahmt, ImageSource)
        newBmpSource = CType(newBmpGerahmt, ImageSource)

        'Zufällige Ankerpunkte wählen
        If aktuelleSettings.ankerpunkte.Count > 0 Then
            aktuellerAnkerpunktOld = aktuelleSettings.ankerpunkte(rnd.Next(aktuelleSettings.ankerpunkte.Count))
        Else
            aktuellerAnkerpunktOld = ankerArray(rnd.Next(9))
        End If

        If aktuelleSettings.gleicherAnkerpunkt Then
            aktuellerAnkerpunktNew = aktuellerAnkerpunktOld
        Else
            If aktuelleSettings.ankerpunkte.Count > 0 Then
                aktuellerAnkerpunktNew = aktuelleSettings.ankerpunkte(rnd.Next(aktuelleSettings.ankerpunkte.Count))
            Else
                aktuellerAnkerpunktNew = ankerArray(rnd.Next(9))
            End If
        End If

        'Falls vom Modul gewünscht, Notbremse setzen.
        If durationMs > 0 Then
            If tmrDuration Is Nothing Then tmrDuration = New Timer()
            tmrDuration.Interval = durationMs
            AddHandler tmrDuration.Tick, Sub()
                                             StopTransition()
                                         End Sub
            tmrDuration.Start()
        End If

        ' Animation starten 
        dauerInMS = 1000 * aktuelleSettings.geschwindigkeit
        startTime = DateTime.Now

        StartRenderLoop(AddressOf DrawTransitionFrame, clntSize)

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Aufräumen und Transition beenden.

        If tmrDuration IsNot Nothing Then
            tmrDuration.Stop()
            tmrDuration.Dispose()
            tmrDuration = Nothing
        End If

        stopRequested = True

        RaiseEvent TransitionIsRunning(False)

    End Sub

    Public Function GetTransitionOptionsDialog() As UserControl Implements ISlideShowTransition.GetTransitionOptionsDialog
        'Liefert den Options-Dialog der Transition

        'Liest die aktuelleSettings ein und speichert sie in SettingsInbox
        ReadTransitionSettingsFromRegistryOrDefaults()
        StoreSettings(nameTransition, aktuelleSettings)

        'Und liefert dann die ucOptionsTransition
        Return New ucOptionsTransition()

    End Function

    'Private Funktionen

    ' Settings und Defaultwerte
    Sub ReadTransitionSettingsFromRegistryOrDefaults()
        'Setzt aktuelleTransitonSettings mit den Werten aus der Registry oder mit Defaultwerten.

        Dim defaults As Dictionary(Of String, String) = GetTransitionDefaultSettings()

        aktuelleSettings.geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Geschwindigkeit", defaults))
        aktuelleSettings.ankerpunkte = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Ankerpunkte", defaults))
        If ReadFromRegOrDefaults(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "GleicherAnkerpunkt", defaults) = "True" Then
            aktuelleSettings.gleicherAnkerpunkt = True
        Else
            aktuelleSettings.gleicherAnkerpunkt = False
        End If

    End Sub

    Public Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        Dim defaults As New Dictionary(Of String, String)

        defaults.Add("Geschwindigkeit", "10")
        defaults.Add("Ankerpunkte", "Z")
        defaults.Add("GleicherAnkerpunkt", "True")

        Return defaults

    End Function

    'Abbruch- und Endverwaltung
    Public Sub EndBildZeichnen()
        'Gibt das Endbild aus

        RaiseEvent TransitionFrameIstFertig(newBmpGerahmt)

    End Sub

    Sub tmrDuration_Tick() Handles tmrDuration.Tick
        'Bricht die Transition nach Ende von DurationMS ab.

        'Zum Schluss noch einmal die aufrufende targetGraphics aktualisieren
        EndBildZeichnen()

        StopTransition()

    End Sub

    'Animation und rendern
    Private Sub DrawTransitionFrame(dc As DrawingContext, size As System.Windows.Size)
        Dim elapsed As Double = (DateTime.Now - startTime).TotalMilliseconds
        Dim progress As Double = Math.Min(1.0, elapsed / dauerInMS)

        Dim oldSize As Windows.Size
        Dim newSize As Windows.Size

        Dim oldPoint As Windows.Point
        Dim newPoint As Windows.Point

        Dim oldRect As Rect
        Dim newRect As Rect

        'Rectangles berechnen (linear interpoliert)

        'Größen...
        If progress <= 0.5 Then
            Dim faktor As Double = 1.0 - (progress / 0.5)
            Dim w As Double = Math.Max(1.0, clntSize.Width * faktor)
            Dim h As Double = Math.Max(1.0, clntSize.Height * faktor)
            oldSize = New Windows.Size(w, h)
            newSize = New Windows.Size(1, 1) ' wird nicht verwendet
        Else
            Dim faktor As Double = (progress - 0.5) / 0.5
            Dim w As Double = Math.Max(1.0, clntSize.Width * faktor)
            Dim h As Double = Math.Max(1.0, clntSize.Height * faktor)
            oldSize = New Windows.Size(1, 1) ' wird nicht verwendet
            newSize = New Windows.Size(w, h)
        End If

        '...und Eckpunkte
        Select Case aktuellerAnkerpunktOld
            Case "NW"
                oldPoint = New Windows.Point(0, 0)
            Case "N"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width) \ 2, 0)
            Case "NO"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width), 0)
            Case "O"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width), (clntSize.Height - oldSize.Height) \ 2)
            Case "SO"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width), (clntSize.Height - oldSize.Height))
            Case "S"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width) \ 2, (clntSize.Height - oldSize.Height))
            Case "SW"
                oldPoint = New Windows.Point(0, (clntSize.Height - oldSize.Height))
            Case "W"
                oldPoint = New Windows.Point(0, (clntSize.Height - oldSize.Height) \ 2)
            Case "Z"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width) \ 2, (clntSize.Height - oldSize.Height) \ 2)
        End Select

        Select Case aktuellerAnkerpunktNew
            Case "NW"
                newPoint = New Windows.Point(0, 0)
            Case "N"
                newPoint = New Windows.Point((clntSize.Width - newSize.Width) \ 2, 0)
            Case "NO"
                newPoint = New Windows.Point((clntSize.Width - newSize.Width), 0)
            Case "O"
                newPoint = New Windows.Point((clntSize.Width - newSize.Width), (clntSize.Height - newSize.Height) \ 2)
            Case "SO"
                newPoint = New Windows.Point((clntSize.Width - newSize.Width), (clntSize.Height - newSize.Height))
            Case "S"
                newPoint = New Windows.Point((clntSize.Width - newSize.Width) \ 2, (clntSize.Height - newSize.Height))
            Case "SW"
                newPoint = New Windows.Point(0, (clntSize.Height - newSize.Height))
            Case "W"
                newPoint = New Windows.Point(0, (clntSize.Height - newSize.Height) \ 2)
            Case "Z"
                newPoint = New Windows.Point((clntSize.Width - newSize.Width) \ 2, (clntSize.Height - newSize.Height) \ 2)
        End Select

        'Rects erzeugen
        oldRect = New Rect(oldPoint, oldSize)
        newRect = New Rect(newPoint, newSize)

        'Bilder zeichnen
        If progress <= 0.5 Then
            dc.DrawImage(oldBmpSource, oldRect)
        Else
            dc.DrawImage(newBmpSource, newRect)
        End If

        'Fertig?
        If progress >= 1.0 Then
            StopTransition()
        End If

    End Sub

    Public Sub StartRenderLoop(drawActionInput As Action(Of DrawingContext, Windows.Size),
                           zielGroesse As Windows.Size)
        'RenderLoop starten und einmaliges renderTargetBitmap anlegen

        drawAction = drawActionInput
        renderSize = zielGroesse

        'Alten Timer stoppen
        StopRenderLoop()

        'Falls Größe geändert → neues RTB erzeugen
        If rtbCache Is Nothing OrElse
       rtbCache.PixelWidth <> CInt(renderSize.Width) OrElse
       rtbCache.PixelHeight <> CInt(renderSize.Height) Then

            rtbCache = New RenderTargetBitmap(CInt(renderSize.Width),
                                          CInt(renderSize.Height),
                                          96, 96, PixelFormats.Pbgra32)
        End If

        'Neuen Timer starten
        frameTimer = New DispatcherTimer()
        AddHandler frameTimer.Tick, AddressOf OnFrameTick
        frameTimer.Interval = TimeSpan.FromMilliseconds(1000 \ FPS)
        frameTimer.Start()
    End Sub

    Private Sub OnFrameTick(sender As Object, e As EventArgs)
        'Zeichnet einen einzelnen Frame
        Dim hintergrundBrush As New Media.SolidColorBrush(SDColorToWMColor(HintergrundFarbeSaver))

        If rtbCache Is Nothing OrElse drawAction Is Nothing Then Exit Sub

        Dim drawingVisual As New DrawingVisual()
        Using dc As DrawingContext = drawingVisual.RenderOpen()
            ' Hintergrund leeren
            dc.DrawRectangle(hintergrundBrush, Nothing, New Rect(0, 0, renderSize.Width, renderSize.Height))
            'Animation aufrufen
            drawAction.Invoke(dc, renderSize)
        End Using

        'RTB mit neuem Inhalt füllen
        ClearRTB(rtbCache)
        rtbCache.Render(drawingVisual)

        RaiseEvent TransitionFrameIstFertig(rtbCache)

        'Aufräumen nach dem letzten Frame
        If stopRequested Then
            StopRenderLoop()
            rtbCache = Nothing
        End If

    End Sub

    Public Sub StopRenderLoop()
        'RenderLoop beenden und aufräumen

        If frameTimer IsNot Nothing Then
            frameTimer.Stop()
            RemoveHandler frameTimer.Tick, AddressOf OnFrameTick
            frameTimer = Nothing
        End If

    End Sub

    Public Sub ClearRTB(rtb As RenderTargetBitmap)
        'rtbCache leeren

        If rtb Is Nothing Then Exit Sub
        Dim dv As New DrawingVisual()
        Using dc As DrawingContext = dv.RenderOpen()
            dc.DrawRectangle(Media.Brushes.Transparent, Nothing,
                             New Rect(0, 0, rtb.PixelWidth, rtb.PixelHeight))
        End Using
        rtb.Render(dv)

    End Sub

End Class
