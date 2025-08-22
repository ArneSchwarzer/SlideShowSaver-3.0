Imports System.Drawing
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowTools
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.LUTHandling
Imports Size = System.Windows.Size

Public NotInheritable Class WpfShaderRunner
    Private Sub New()
    End Sub

    ' Wendet die ausgewählte LUT (cube oder Hald) auf ein GDI+ Bitmap an und gibt ein neues Bitmap zurück.
    ' strength: 0..1   targetN: z.B. 33 (resample falls nötig)
    Public Shared Function ApplyLutEffect(srcBitmap As Bitmap,
                                          lut As LutInfo,
                                          strength As Single,
                                          Optional targetN As Integer = 33) As Bitmap
        If srcBitmap Is Nothing OrElse lut.Type = LutType.Unknown Then Return Nothing

        Dim width As Integer = srcBitmap.Width
        Dim height As Integer = srcBitmap.Height

        ' GDI+ -> WPF ohne PNG-Umweg
        Dim bmpSource As BitmapSource = ConvertBitmapToImageSource(srcBitmap)

        ' LUT-Atlas bauen (gecacht in LUTHandling)
        Dim atlas As LutAtlas = BuildAtlas(lut, targetN)

        ' ImageBrush für LUT-Atlas (Sampler s1)
        Dim lutBrush As New ImageBrush(atlas.Atlas) With {
            .Stretch = Stretch.Fill,
            .TileMode = TileMode.None,
            .ViewportUnits = BrushMappingMode.RelativeToBoundingBox
        }
        lutBrush.Freeze()

        ' Effekt instanzieren und Parameter setzen
        Dim fx As New LUTEffect() With {
            .LutTex = lutBrush,
            .LutParams = New System.Windows.Media.Media3D.Point4D(
                            atlas.N,
                            1.0 / Math.Max(1.0, atlas.N - 1.0),   ' y: 1/(N-1) (optional)
                            Math.Max(0.0, Math.Min(1.0, strength)), ' z: Stärke 0..1
                            1.0 / (atlas.N * atlas.N)            ' w: invWidth = 1/(N*N)
                        )
        }

        ' Das zu rendernde Element
        Dim img As New Controls.Image() With {
            .Source = bmpSource,
            .Width = width,
            .Height = height,
            .Stretch = Stretch.Fill,
            .Effect = fx
        }
        img.Measure(New Size(width, height))
        img.Arrange(New Rect(0, 0, width, height))
        img.UpdateLayout()

        ' RenderTargetBitmap wiederverwenden, um LOH-Fragmentierung zu vermeiden
        Static rtb As RenderTargetBitmap = Nothing
        If rtb Is Nothing OrElse rtb.PixelWidth <> width OrElse rtb.PixelHeight <> height Then
            rtb = New RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32)
        Else
            ' optional: „löschen“
            Dim dv As New DrawingVisual()
            Using dc = dv.RenderOpen()
                dc.DrawRectangle(Media.Brushes.Transparent, Nothing, New Rect(0, 0, width, height))
            End Using
            rtb.Render(dv)
        End If

        rtb.Render(img)

        ' WPF -> GDI+ ohne PNG-Encoder
        Dim result As Bitmap = ConvertRenderTargetBitmapToBitmap(rtb)

        ' Referenzen lösen
        img.Source = Nothing
        img.Effect = Nothing

        Return result
    End Function
End Class
