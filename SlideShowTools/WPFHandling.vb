Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading

Public Class WPFHandling

    Public Shared Event TransitionFrameIstFertig(bitmap As RenderTargetBitmap)

    Private Shared drawAction As Action(Of DrawingContext, Windows.Size)
    Private Shared frameTimer As DispatcherTimer
    Public Shared renderSize As Windows.Size
    Private Shared fps As Integer = 30

    'KOPIERVORLAGEN für Transitionen und animierte Shader (Events müssen aus den jeweiligen DLLs selber gefeuert werden!)
    Public Shared Sub StartRenderLoop(drawActionInput As Action(Of DrawingContext, Windows.Size),
                                      zielGroesse As Windows.Size,
                                      Optional framesPerSecond As Integer = 30)

        drawAction = drawActionInput
        renderSize = zielGroesse
        fps = framesPerSecond

        StopRenderLoop()

        frameTimer = New DispatcherTimer()
        AddHandler frameTimer.Tick, AddressOf OnFrameTick
        frameTimer.Interval = TimeSpan.FromMilliseconds(1000 \ fps)
        frameTimer.Start()
    End Sub

    Public Shared Sub StopRenderLoop()
        If frameTimer IsNot Nothing Then
            frameTimer.Stop()
            RemoveHandler frameTimer.Tick, AddressOf OnFrameTick
            frameTimer = Nothing
        End If
    End Sub

    Private Shared Sub OnFrameTick(sender As Object, e As EventArgs)
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

        ' Umwandeln in System.Drawing.Bitmap
        'Dim bitmap As Bitmap = ConvertRenderTargetBitmapToBitmap(rtb)

        RaiseEvent TransitionFrameIstFertig(rtb)
    End Sub

    <DllImport("gdi32.dll")>
    Private Shared Function DeleteObject(hObject As IntPtr) As Boolean
    End Function

End Class
