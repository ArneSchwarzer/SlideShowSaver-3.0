Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SharedDataHandling
Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Windows.Forms

'KOPIERVORLAGEN
'
'Für die Render-Engine für Transitionen, da die zugehörigen Events aus den jeweiligen DLLs selber
'gefeuert werden sollten. Ein "Doppeltes" Eventhandling (1x aus der Transition, 1x aus einer
'RenderHandling-Klasse) ist hier zu umständlich.
'
'Es werden sowohl Kopiervorlagen für WPF- als auch für WinForms-RenderLoops angeboten

Public Class RenderLoopKopiervorlage


    '*************************************
    '*                                   *
    '* Kopiervorlage für WPF-Renderloops *
    '*                                   *
    '*************************************

    'Varablendeklaration

    'Zeitmanagement
    Private WithEvents tmrDuration As New Timer
    Private startTime As DateTime
    Private dauerInMS As Integer
    Private Const FPS As Integer = 120

    'Rendering
    Private drawAction As Action(Of DrawingContext, Windows.Size)
    Private frameTimer As DispatcherTimer
    Private renderSize As Windows.Size
    Private rtbCache As RenderTargetBitmap
    Private stopRequested As Boolean = False

    'Events
    Public Shared Event TransitionFrameIstFertig(bitmap As RenderTargetBitmap)
    Public Shared Event TransitionIsRunning(state As Boolean)

    'Ablaufsteuerung
    Sub StopTransition() 'Implements ISlideShowTransition.StopTransition
        'Aufräumen und Transition beenden.

        If tmrDuration IsNot Nothing Then
            tmrDuration.Stop()
            tmrDuration.Dispose()
            tmrDuration = Nothing
        End If

        stopRequested = True

        RaiseEvent TransitionIsRunning(False)

    End Sub

    'Animation & Rendering
    Private Sub DrawTransitionFrame(dc As DrawingContext, size As System.Windows.Size)

        'Dies ist nur die Grundstruktur für ein DrawTransitionFrame(). Die eigentliche Animations-Logik
        'der Transition wird an dieser Stelle umgesetzt. Entsprechend ist diese Methode von Transition
        'zu Transition individuell zu gestalten.


        Dim elapsed As Double = (DateTime.Now - startTime).TotalMilliseconds
        Dim progress As Double = Math.Min(1.0, elapsed / dauerInMS)

        Dim oldBmpSource As BitmapSource
        Dim newBmpSource As BitmapSource

        Dim oldRect As Rect
        Dim newRect As Rect

        'Bilder zeichnen
        dc.DrawImage(oldBmpSource, oldRect)
        dc.DrawImage(newBmpSource, newRect)

        'Fertig?
        If progress >= 1.0 Then
            StopTransition()
        End If

    End Sub

    Public Sub StartRenderLoop(drawActionInput As Action(Of DrawingContext, Windows.Size),
                           zielGroesse As Windows.Size)
        'RenderLoop starten und einmaliges renderTargetBitmap anlegen

        StopRenderLoop()

        drawAction = drawActionInput
        renderSize = zielGroesse
        stopRequested = False

        ' Falls Größe geändert → neues RTB erzeugen
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
            'Hintergrund leeren
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

        rtbCache = Nothing

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



    '******************************************
    '*                                        *
    '* Kopiervorlage für WinForms-Renderloops *
    '*                                        *
    '******************************************

End Class
