Imports System.Windows.Media
Imports System.Windows.Interop
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.IO

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


    Public Shared Function ConvertImageToRenderTargetBitmap(img As Image, targetSize As System.Drawing.Size) As RenderTargetBitmap
        If img Is Nothing Then Return Nothing

        ' Konvertiere System.Drawing.Image → BitmapSource
        Using bmp As Bitmap = New Bitmap(img)
            Dim hBitmap As IntPtr = bmp.GetHbitmap()
            Try
                Dim bitmapSource As BitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions())

                bitmapSource.Freeze() ' wichtig für Thread-Sicherheit

                ' Erzeuge RTB und zeichne das BitmapSource hinein
                Dim rtb As New RenderTargetBitmap(targetSize.Width, targetSize.Height, 96, 96, PixelFormats.Pbgra32)
                Dim dv As New DrawingVisual()
                Using dc As DrawingContext = dv.RenderOpen()
                    dc.DrawImage(bitmapSource, New Rect(0, 0, targetSize.Width, targetSize.Height))
                End Using
                rtb.Render(dv)

                Return rtb

            Finally
                ' Clean up unmanaged HBITMAP
                DeleteObject(hBitmap)
            End Try
        End Using
    End Function

    'BitmapImage to...
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
