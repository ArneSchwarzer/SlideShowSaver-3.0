Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.IO

Public Class WPFHandling

    Public Shared Event FrameIstFertig(bitmap As RenderTargetBitmap)

    Private Shared drawAction As Action(Of DrawingContext, Windows.Size)
    Private Shared frameTimer As DispatcherTimer
    Private Shared renderSize As Windows.Size
    Private Shared fps As Integer = 30

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

        RaiseEvent FrameIstFertig(rtb)
    End Sub

    Private Shared Function ConvertRenderTargetBitmapToBitmap(rtb As RenderTargetBitmap) As Bitmap
        Dim width As Integer = rtb.PixelWidth
        Dim height As Integer = rtb.PixelHeight
        Dim stride As Integer = width * 4

        Dim pixelData(stride * height - 1) As Byte
        rtb.CopyPixels(pixelData, stride, 0)

        Dim bmp As New Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
        Dim bmpData As BitmapData = bmp.LockBits(New Rectangle(0, 0, width, height),
                                                 ImageLockMode.WriteOnly,
                                                 System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
        Marshal.Copy(pixelData, 0, bmpData.Scan0, pixelData.Length)
        bmp.UnlockBits(bmpData)

        Return bmp
    End Function

    Public Shared Function ConvertBitmapImageToRenderTargetBitmap(bmpImage As BitmapImage, size As Windows.Size) As RenderTargetBitmap
        Dim imageControl As New Windows.Controls.Image()
        imageControl.Source = bmpImage
        imageControl.Width = size.Width
        imageControl.Height = size.Height
        imageControl.Stretch = Stretch.Uniform ' Alternativ: Stretch.Fill, je nach gewünschtem Verhalten

        imageControl.Measure(New Windows.Size(size.Width, size.Height))
        imageControl.Arrange(New Rect(0, 0, size.Width, size.Height))

        Dim rtb As New RenderTargetBitmap(size.Width, size.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(imageControl)

        Return rtb
    End Function

    Public Shared Function ConvertImageToBitmapImage(img As Image) As BitmapImage
        Using ms As New MemoryStream()
            img.Save(ms, ImageFormat.Png)
            ms.Seek(0, SeekOrigin.Begin)

            Dim bmpImage As New BitmapImage()
            bmpImage.BeginInit()
            bmpImage.CacheOption = BitmapCacheOption.OnLoad
            bmpImage.StreamSource = ms
            bmpImage.EndInit()
            bmpImage.Freeze() ' wichtig für Cross-Thread-Access

            Return bmpImage
        End Using
    End Function
End Class
