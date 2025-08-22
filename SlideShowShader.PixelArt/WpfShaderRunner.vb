Imports System.Drawing
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Controls
Imports SlideShowTools.ImageConversionHandling  ' <— wichtig

Public NotInheritable Class WpfShaderRunner
    Private Sub New()
    End Sub

    Public Shared Function ApplyPixelArtEffect(srcBitmap As Bitmap,
                                               cellSize As Single,
                                               levelsPerChannel As Integer) As Bitmap
        If srcBitmap Is Nothing Then Return Nothing

        Dim width As Integer = srcBitmap.Width
        Dim height As Integer = srcBitmap.Height

        ' 1) GDI -> WPF ohne PNG: nutzt HBITMAP + DeleteObject (kein Leak)
        Dim bmpSource As BitmapSource = ConvertBitmapToImageSource(srcBitmap) ' liefert bereits Frozen. :contentReference[oaicite:0]{index=0}

        ' 2) Effekt vorbereiten (TexelSize = 1/px, Params = (cellPx, levels))
        Dim fx As New PixelArtEffect() With {
            .TexelSize = New System.Windows.Point(1.0 / width, 1.0 / height),
            .Params01 = New System.Windows.Point(Math.Max(1.0, cellSize), CDbl(Math.Max(2, levelsPerChannel)))
        }

        ' 3) Ein echtes Element rendern
        Dim img As New System.Windows.Controls.Image() With {
            .Source = bmpSource,
            .Width = width,
            .Height = height,
            .Stretch = Stretch.Fill,
            .Effect = fx
        }
        img.Measure(New System.Windows.Size(width, height))
        img.Arrange(New Rect(0, 0, width, height))
        img.UpdateLayout()

        ' 4) Reuse eines RTB (optional, aber gut gegen LOH-Fragmentierung)
        Static rtb As RenderTargetBitmap = Nothing
        If rtb Is Nothing OrElse rtb.PixelWidth <> width OrElse rtb.PixelHeight <> height Then
            rtb = New RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32)
        Else
            ' "Löschen": transparent drüberzeichnen
            Dim dv As New DrawingVisual()
            Using dc = dv.RenderOpen()
                dc.DrawRectangle(System.Windows.Media.Brushes.Transparent, Nothing, New Rect(0, 0, width, height))
            End Using
            rtb.Render(dv)
        End If

        rtb.Render(img)

        ' 5) WPF -> GDI ohne PNG: direkt per CopyPixels
        Dim result As Bitmap = ConvertRenderTargetBitmapToBitmap(rtb)

        ' 6) Referenzen lösen, damit GC räumen kann
        img.Source = Nothing
        img.Effect = Nothing

        Return result
    End Function
End Class
