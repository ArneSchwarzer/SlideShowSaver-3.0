Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging.LogHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.WPFHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.GraphicsSizeModeHandling
Imports System.Windows.Threading

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
    Private aktuellerAnkerpunkt As String
    Private clntSize As Windows.Size

    'Rendering
    Private Shared drawAction As Action(Of DrawingContext, Windows.Size)
    Private Shared frameTimer As DispatcherTimer
    Private Shared renderSize As Windows.Size

    Public Structure SlideShowTransitionSettings_Zoom
        Public ankerpunkte As List(Of String)
        Public geschwindigkeit As Integer
    End Structure

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

    Public Event TransitionIsRunning As ISlideShowTransition.TransitionIsRunningEventHandler Implements ISlideShowTransition.TransitionIsRunning
    Public Event TransitionFrameIstFertig As ISlideShowTransition.TransitionFrameIstFertigEventHandler Implements ISlideShowTransition.TransitionFrameIstFertig

    Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As BitmapImage, picBoxModeNew As PictureBoxSizeMode, clientSize As Size, Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition
        'Bereitet die Animation vor und startet den Render-Loop
        Dim ankerArray() As String = {"N", "NO", "O", "SO", "S", "SW", "W", "NW", "Z"}

        'Interne Initialisierungen & Formalitäten
        ReadTransitionSettingsFromRegistryOrDefaults()
        StoreSettings(TransitionName, aktuelleSettings)

        RaiseEvent TransitionIsRunning(True)

        'Konvertieren und in lokalen Variablen speichern
        clntSize = New Windows.Size(clientSize.Width, clientSize.Height)

        ' Bilder vorbereiten
        oldBmpGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
        newBmpGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)
        oldBmpSource = CType(oldBmpGerahmt, ImageSource)
        newBmpSource = CType(newBmpGerahmt, ImageSource)

        'Zufälligen Ankerpunkt wählen
        If aktuelleSettings.ankerpunkte.Count > 0 Then
            aktuellerAnkerpunkt = aktuelleSettings.ankerpunkte(New Random().Next(aktuelleSettings.ankerpunkte.Count))
        Else
            aktuellerAnkerpunkt = ankerArray(New Random().Next(9))
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
        aktuelleSettings.ankerpunkte = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Richtungen", defaults))

    End Sub

    Public Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        Dim defaults As New Dictionary(Of String, String)

        defaults.Add("Geschwindigkeit", "10")
        defaults.Add("Ankerpunkte", "Z")

        Return defaults

    End Function

    'Abbruch- und Endverwaltung
    Public Sub EndBildZeichnen()
        'Gibt das Endbild aus

        RaiseEvent TransitionFrameIstFertig(newBmpGerahmt)

    End Sub

    Sub tmrDuration_Tick() Handles tmrDuration.Tick
        'Bricht die Transition nach Ende von DurationMS ab.
        LogDebug("Transition SuW - TransitionMain.tmrDuration_Tick() wurde aufgerufen.")

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
        Select Case aktuellerAnkerpunkt
            Case "NW"
                oldPoint = New Windows.Point(0, 0)
                newPoint = New Windows.Point(0, 0)
            Case "N"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width) \ 2, 0)
                newPoint = New Windows.Point((clntSize.Width - newSize.Width) \ 2, 0)
            Case "NO"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width), 0)
                newPoint = New Windows.Point((clntSize.Width - newSize.Width), 0)
            Case "O"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width), (clntSize.Height - oldSize.Height) \ 2)
                newPoint = New Windows.Point((clntSize.Width - newSize.Width), (clntSize.Height - newSize.Height) \ 2)
            Case "SO"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width), (clntSize.Height - oldSize.Height))
                newPoint = New Windows.Point((clntSize.Width - newSize.Width), (clntSize.Height - newSize.Height))
            Case "S"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width) \ 2, (clntSize.Height - oldSize.Height))
                newPoint = New Windows.Point((clntSize.Width - newSize.Width) \ 2, (clntSize.Height - newSize.Height))
            Case "SW"
                oldPoint = New Windows.Point(0, (clntSize.Height - oldSize.Height))
                newPoint = New Windows.Point(0, (clntSize.Height - newSize.Height))
            Case "W"
                oldPoint = New Windows.Point(0, (clntSize.Height - oldSize.Height) \ 2)
                newPoint = New Windows.Point(0, (clntSize.Height - newSize.Height) \ 2)
            Case "Z"
                oldPoint = New Windows.Point((clntSize.Width - oldSize.Width) \ 2, (clntSize.Height - oldSize.Height) \ 2)
                newPoint = New Windows.Point((clntSize.Width - newSize.Width) \ 2, (clntSize.Height - newSize.Height) \ 2)
        End Select

        'Rects erzeugen
        oldRect = New Rect(oldPoint, oldSize)
        newRect = New Rect(newPoint, newSize)

        'Hintergrund füllen
        dc.DrawRectangle(Media.Brushes.Black, Nothing, New Rect(0, 0, clntSize.Width, clntSize.Height))

        'Bilder zeichnen
        If progress <= 0.5 Then
            dc.DrawImage(oldBmpSource, oldRect)
        Else
            dc.DrawImage(newBmpSource, newRect)
        End If

        'Fertig?
        If progress >= 1.0 Then
            StopRenderLoop()
            StopTransition()
        End If

    End Sub

    Public Sub StartRenderLoop(drawActionInput As Action(Of DrawingContext, Windows.Size),
                                      zielGroesse As Windows.Size)

        drawAction = drawActionInput
        renderSize = zielGroesse

        StopRenderLoop()

        frameTimer = New DispatcherTimer()
        AddHandler frameTimer.Tick, AddressOf OnFrameTick
        frameTimer.Interval = TimeSpan.FromMilliseconds(1000 \ fps)
        frameTimer.Start()

    End Sub

    Public Sub StopRenderLoop()
        If frameTimer IsNot Nothing Then
            frameTimer.Stop()
            RemoveHandler frameTimer.Tick, AddressOf OnFrameTick
            frameTimer = Nothing
        End If
    End Sub

    Private Sub OnFrameTick(sender As Object, e As EventArgs)
        If drawAction Is Nothing Then Exit Sub

        ' Neuen Frame zeichnen
        Dim drawingVisual As New DrawingVisual()
        Using dc As DrawingContext = drawingVisual.RenderOpen()
            dc.DrawRectangle(Media.Brushes.Black, Nothing, New Rect(0, 0, renderSize.Width, renderSize.Height))
            drawAction.Invoke(dc, renderSize)
        End Using

        ' Rendern in Bitmap
        Dim rtb As New RenderTargetBitmap(CInt(renderSize.Width),
                                          CInt(renderSize.Height),
                                          96, 96, PixelFormats.Pbgra32)
        rtb.Render(drawingVisual)

        RaiseEvent TransitionFrameIstFertig(rtb)

    End Sub
End Class
