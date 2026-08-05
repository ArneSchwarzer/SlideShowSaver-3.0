Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows
Imports System.Windows.Interop
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Public Class ImageConversionHandling

    'Helferfunktionen

    <DllImport("gdi32.dll")>
    Private Shared Function DeleteObject(hObject As IntPtr) As Boolean
        'Native API für GDI-Bitmapfreigabe
    End Function

    Private Shared Function CreateBitmapSourceFromGdiBitmap(bmp As Bitmap) As BitmapSource
        'Erzeugt eine BitmapSource aus einem GDI-Bitmap und gibt das HBITMAP
        'in jedem Fall wieder frei.

        Dim hBmp As IntPtr
        Dim source As BitmapSource

        hBmp = IntPtr.Zero
        source = Nothing

        If bmp Is Nothing Then
            Return Nothing
        End If

        Try

            hBmp = bmp.GetHbitmap()

            source = Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty,
                                                           BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height))

            source.Freeze()

            Return source

        Finally

            If hBmp <> IntPtr.Zero Then
                DeleteObject(hBmp)
            End If

        End Try

    End Function

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

    Public Shared Function ConvertBitmapToWriteableBitmap(bmp As Bitmap) As WriteableBitmap
        If bmp Is Nothing Then Return Nothing
        Dim wb As New WriteableBitmap(bmp.Width, bmp.Height, 96, 96, PixelFormats.Pbgra32, Nothing)

        Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
        Dim data = bmp.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
        Try
            wb.Lock()
            Dim bufferSize = data.Stride * data.Height
            Dim bytes(bufferSize - 1) As Byte
            Marshal.Copy(data.Scan0, bytes, 0, bufferSize)
            Marshal.Copy(bytes, 0, wb.BackBuffer, bufferSize)
            wb.AddDirtyRect(New Int32Rect(0, 0, bmp.Width, bmp.Height))
            wb.Unlock()
        Finally
            bmp.UnlockBits(data)
        End Try
        Return wb
    End Function

    ' WriteableBitmap -> Bitmap (32bppArgb)
    Public Shared Function ConvertWriteableBitmapToBitmap(wb As WriteableBitmap) As Bitmap
        If wb Is Nothing Then Return Nothing
        Dim bmp As New Bitmap(wb.PixelWidth, wb.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        bmp.SetResolution(96, 96)

        Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
        Dim data = bmp.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        Try
            wb.Lock()
            Dim bufferSize = wb.BackBufferStride * wb.PixelHeight
            Dim bytes(bufferSize - 1) As Byte
            Marshal.Copy(wb.BackBuffer, bytes, 0, bufferSize)
            Marshal.Copy(bytes, 0, data.Scan0, bufferSize)
            wb.Unlock()
        Finally
            bmp.UnlockBits(data)
        End Try
        Return bmp
    End Function

    Public Shared Function ConvertBitmapToImageSource(bmp As Bitmap) As ImageSource

        If bmp Is Nothing Then Return Nothing

        Return CreateBitmapSourceFromGdiBitmap(bmp)

    End Function

    Public Shared Function ConvertByteArrayToImageSource(imageData As Byte()) As ImageSource

        If imageData Is Nothing OrElse imageData.Length = 0 Then Return Nothing

        Using ms As New MemoryStream(imageData)
            Using bmp As New Bitmap(ms)
                Return CreateBitmapSourceFromGdiBitmap(bmp)
            End Using
        End Using

    End Function

    Public Shared Function ConvertImageToRenderTargetBitmap(bmp As Bitmap) As RenderTargetBitmap

        If bmp Is Nothing Then Return Nothing

        Dim src As BitmapSource = CreateBitmapSourceFromGdiBitmap(bmp)
        Dim rtb As New RenderTargetBitmap(bmp.Width, bmp.Height, 96, 96, PixelFormats.Pbgra32)
        Dim dv As New DrawingVisual()

        Using dc = dv.RenderOpen()
            dc.DrawImage(src, New Rect(0, 0, bmp.Width, bmp.Height))
        End Using

        rtb.Render(dv)
        rtb.Freeze()

        Return rtb

    End Function

    Public Shared Sub RenderImageInRenderTargetBitmap(bmp As Bitmap, renderTarget As RenderTargetBitmap,
                                                      drawingVisual As DrawingVisual)
        'Rendert ein GDI-Bitmap in ein bereits vorhandenes WPF-RenderTargetBitmap.

        Dim source As BitmapSource
        Dim zielRechteck As Rect

        source = Nothing

        If bmp Is Nothing OrElse renderTarget Is Nothing OrElse drawingVisual Is Nothing Then

            Exit Sub

        End If

        source = CreateBitmapSourceFromGdiBitmap(bmp)
        zielRechteck = New Rect(0.0, 0.0, renderTarget.PixelWidth, renderTarget.PixelHeight)

        Using drawingContext As DrawingContext = drawingVisual.RenderOpen()

            'Der Frame ist vollständig deckend. Dadurch überschreibt er den
            'Inhalt des vorherigen Frames vollständig.
            drawingContext.DrawImage(source, zielRechteck)

        End Using

        renderTarget.Render(drawingVisual)

    End Sub

    'BitmapImage to...
    Public Shared Function ConvertBitmapImageToImage(bmpImage As BitmapImage) As System.Drawing.Image

        Dim encoder As PngBitmapEncoder
        Dim temporaeresBild As System.Drawing.Image
        Dim unabhaengigesBild As System.Drawing.Bitmap

        encoder = Nothing
        temporaeresBild = Nothing
        unabhaengigesBild = Nothing

        If bmpImage Is Nothing Then
            Return Nothing
        End If

        Using ms As New MemoryStream()

            encoder = New PngBitmapEncoder()
            encoder.Frames.Add(BitmapFrame.Create(bmpImage))
            encoder.Save(ms)

            ms.Position = 0

            Try

                temporaeresBild = System.Drawing.Image.FromStream(ms, True, True)

                unabhaengigesBild = New System.Drawing.Bitmap(temporaeresBild)

            Finally

                If temporaeresBild IsNot Nothing Then

                    temporaeresBild.Dispose()
                    temporaeresBild = Nothing

                End If

            End Try

        End Using

        Return unabhaengigesBild

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

End Class
