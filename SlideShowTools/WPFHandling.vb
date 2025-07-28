Imports System.Windows.Media
Imports System.Windows.Interop
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.IO
Imports System.Threading

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

    'Konverter
    'Image to...
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

    Public Shared Function ConvertImageToImageSource(img As Image) As ImageSource
        If img Is Nothing Then Return Nothing

        Using ms As New MemoryStream()
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            ms.Seek(0, SeekOrigin.Begin)

            Dim bitmapImage As New BitmapImage()
            bitmapImage.BeginInit()
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad
            bitmapImage.StreamSource = ms
            bitmapImage.EndInit()
            bitmapImage.Freeze() ' Optional, aber empfohlen für Performance

            Return bitmapImage
        End Using
    End Function

    Public Shared Function ConvertImageToRenderTargetBitmap(bmp As Bitmap) As RenderTargetBitmap
        If bmp Is Nothing Then Return Nothing

        ' Bitmap in MemoryStream speichern
        Using ms As New MemoryStream()
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            ms.Seek(0, SeekOrigin.Begin)

            ' BitmapImage laden
            Dim decoder As New PngBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad)
            Dim source As BitmapSource = decoder.Frames(0)

            ' In DrawingVisual zeichnen
            Dim drawingVisual As New DrawingVisual()
            Using dc As DrawingContext = drawingVisual.RenderOpen()
                dc.DrawImage(source, New Rect(0, 0, source.PixelWidth, source.PixelHeight))
            End Using

            ' RenderTargetBitmap erzeugen
            Dim rtb As New RenderTargetBitmap(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, PixelFormats.Pbgra32)
            rtb.Render(drawingVisual)

            Return rtb
        End Using
    End Function


    'BitmapImage to...
    Public Shared Function ConvertBitmapImageToImage(bmpImage As BitmapImage) As System.Drawing.Image
        If bmpImage Is Nothing Then Return Nothing

        ' Sicherstellen, dass das Bild vollständig geladen ist
        If bmpImage.IsDownloading Then
            Dim done As New ManualResetEvent(False)
            AddHandler bmpImage.DownloadCompleted, Sub() done.Set()
            done.WaitOne()
        End If

        Using ms As New MemoryStream()
            Dim encoder As New PngBitmapEncoder()
            encoder.Frames.Add(BitmapFrame.Create(bmpImage))
            encoder.Save(ms)
            ms.Seek(0, SeekOrigin.Begin)
            Return Image.FromStream(ms)
        End Using
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

    'RenderTarget to...
    Public Shared Function ConvertRenderTargetBitmapToBitmap(rtb As RenderTargetBitmap) As Bitmap
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

    Public Shared Function ConvertRenderTargetBitmapToBitmapImage(rtb As RenderTargetBitmap) As BitmapImage
        If rtb Is Nothing Then Return Nothing

        Dim encoder As New PngBitmapEncoder()
        encoder.Frames.Add(BitmapFrame.Create(rtb))

        Using ms As New MemoryStream()
            encoder.Save(ms)
            ms.Seek(0, SeekOrigin.Begin)

            Dim bmpImage As New BitmapImage()
            bmpImage.BeginInit()
            bmpImage.CacheOption = BitmapCacheOption.OnLoad
            bmpImage.StreamSource = ms
            bmpImage.EndInit()
            bmpImage.Freeze()

            Return bmpImage
        End Using
    End Function


    <DllImport("gdi32.dll")>
    Private Shared Function DeleteObject(hObject As IntPtr) As Boolean
    End Function

End Class
