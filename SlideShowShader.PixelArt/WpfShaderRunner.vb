Imports System.Drawing
Imports System.IO
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Controls

Public NotInheritable Class WpfShaderRunner

    ' *** Variablen am Anfang ***
    Private Sub New()
    End Sub

    Public Shared Function ApplyPixelArtEffect(srcBitmap As Bitmap,
                                               cellSize As Single,
                                               levelsPerChannel As Integer) As Bitmap
        Dim bmpSource As BitmapSource = Nothing
        Dim vis As New DrawingVisual()
        Dim rtb As RenderTargetBitmap = Nothing
        Dim finalBitmap As Bitmap = Nothing
        Dim width As Integer = srcBitmap.Width
        Dim height As Integer = srcBitmap.Height
        Dim fx As PixelArtEffect = Nothing
        Dim p0 As Media.Color

        ' WinForms Bitmap -> WPF BitmapSource
        Using ms As New MemoryStream()
            srcBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
            ms.Position = 0
            bmpSource = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad)
        End Using

        ' Params packen: (cellSize, levels, _, _)
        p0 = Media.Color.FromScRgb(1.0F, cellSize, CSng(levelsPerChannel), 0.0F)

        fx = New PixelArtEffect() With {
            .Input = New ImageBrush(bmpSource) With {.Stretch = Stretch.Fill},
            .Params0 = p0
        }

        ' Effekt auf ein Visual anwenden
        Using dc = vis.RenderOpen()
            Dim rect As New Rect(0, 0, width, height)
            Dim vb As New VisualBrush() With {.Visual = New Border With {.Effect = fx, .Width = width, .Height = height}}
            dc.DrawRectangle(vb, Nothing, rect)
        End Using

        rtb = New RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(vis)

        finalBitmap = BitmapFromSource(rtb)
        Return finalBitmap
    End Function

    Private Shared Function BitmapFromSource(source As BitmapSource) As Bitmap
        Dim bmp As Bitmap = Nothing
        Dim encoder As New PngBitmapEncoder()
        encoder.Frames.Add(BitmapFrame.Create(source))
        Using ms As New MemoryStream()
            encoder.Save(ms)
            Using temp As New Bitmap(ms)
                bmp = New Bitmap(temp)
            End Using
        End Using
        Return bmp
    End Function
End Class
