Imports System.Drawing
Imports System.Drawing.Imaging

Public Class TextureHandling
    Public Shared Function GenerateNoiseTexture(width As Integer, height As Integer, Optional dichte As Byte = 128) As Bitmap
        Dim bmp As New Bitmap(width, height, PixelFormat.Format32bppArgb)
        Dim rnd As New Random()

        For y As Integer = 0 To height - 1
            For x As Integer = 0 To width - 1
                Dim gray As Integer = rnd.Next(0, 256)
                Dim alpha As Integer

                If gray < dichte Then
                    alpha = 255 - gray ' Sichtbar – dunkle Pixel stärker sichtbar
                Else
                    alpha = 0 ' Unsichtbar
                End If

                bmp.SetPixel(x, y, Color.FromArgb(alpha, gray, gray, gray))
            Next
        Next

        Return bmp
    End Function

    Public Shared Function GenerateScanlineTexture(width As Integer, height As Integer, Optional dichte As Integer = 5) As Bitmap
        Dim bmp As New Bitmap(width, height, PixelFormat.Format32bppArgb)
        Dim rnd As New Random
        Dim istScanline As Boolean

        For y As Integer = 0 To height - 1
            istScanline = If(rnd.Next(dichte) = 0, True, False)
            For x As Integer = 0 To width - 1
                If istScanline Then
                    bmp.SetPixel(x, y, Color.FromArgb(64, 0, 255, 0)) ' leicht sichtbares Scanline-Grün
                Else
                    bmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0)) ' vollständig transparent
                End If
            Next
        Next

        Return bmp
    End Function

    Public Shared Function ConvertGrayscaleToAlphaMask(inputBitmap As Bitmap) As Bitmap
        Dim resultBitmap As New Bitmap(inputBitmap.Width, inputBitmap.Height, PixelFormat.Format32bppArgb)

        ' Grauwert → Alpha, RGB → 0
        Dim cm As New Imaging.ColorMatrix(New Single()() {
            New Single() {0, 0, 0, 0, 0},                      ' R = 0
            New Single() {0, 0, 0, 0, 0},                      ' G = 0
            New Single() {0, 0, 0, 0, 0},                      ' B = 0
            New Single() {0.3F, 0.59F, 0.11F, 0, 0},           ' A = Helligkeit des Originalbilds
            New Single() {0, 0, 0, 0, 1}
        })

        Dim ia As New Imaging.ImageAttributes()
        ia.SetColorMatrix(cm)

        Using g As Graphics = Graphics.FromImage(resultBitmap)
            g.DrawImage(
                inputBitmap,
                New Rectangle(0, 0, inputBitmap.Width, inputBitmap.Height),
                0, 0, inputBitmap.Width, inputBitmap.Height,
                GraphicsUnit.Pixel,
                ia
            )
        End Using

        Return resultBitmap
    End Function

End Class
