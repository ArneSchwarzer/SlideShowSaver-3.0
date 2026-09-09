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

Public NotInheritable Class WpfShaderRunner
    Implements IDisposable

    'Variablendeklaration
    Private renderTarget As RenderTargetBitmap
    Private wurdeBereinigt As Boolean

    Public Function ApplyLutEffect(srcBitmap As Bitmap, lut As LutInfo, strength As Single,
                                   Optional targetN As Integer = 33) As Bitmap

        'Wendet eine LUT mittels WPF/HLSL auf das übergebene Bitmap an
        'und gibt ein neues GDI-Bitmap zurück.

        Dim width As Integer
        Dim height As Integer

        Dim bmpSource As BitmapSource
        Dim atlas As LutAtlas

        Dim lutBrush As ImageBrush
        Dim fx As LUTEffect
        Dim img As Controls.Image

        Dim lutParams As Point4D
        Dim renderSize As System.Windows.Size
        Dim result As Bitmap

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(WpfShaderRunner))

        End If

        If srcBitmap Is Nothing Then

            Throw New ArgumentNullException(NameOf(srcBitmap))

        End If

        If lut.Type = LutType.Unknown Then

            Return DirectCast(srcBitmap.Clone(), Bitmap)

        End If

        width = srcBitmap.Width
        height = srcBitmap.Height

        If width <= 0 OrElse height <= 0 Then

            Throw New ArgumentOutOfRangeException(NameOf(srcBitmap),
                                                  "Das Quellbild besitzt keine gültigen Abmessungen.")

        End If

        strength = Math.Max(0.0F, Math.Min(1.0F, strength))

        bmpSource = ConvertBitmapToImageSource(srcBitmap)

        If bmpSource Is Nothing Then

            Throw New InvalidOperationException("Das Quellbild konnte nicht in ein WPF-Bitmap konvertiert werden.")

        End If

        atlas = BuildAtlas(lut, targetN)

        If atlas.Atlas Is Nothing Then

            Throw New InvalidOperationException("Der LUT-Atlas konnte nicht erzeugt werden.")

        End If

        lutBrush = New ImageBrush(atlas.Atlas)
        lutBrush.Stretch = Stretch.Fill
        lutBrush.TileMode = TileMode.None
        lutBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox

        If lutBrush.CanFreeze Then

            lutBrush.Freeze()

        End If

        lutParams =
            New Point4D(
                atlas.N,
                1.0 /
                Math.Max(
                    1.0,
                    atlas.N - 1.0),
                strength,
                1.0 /
                (atlas.N * atlas.N))

        fx = New LUTEffect()
        fx.LutTex = lutBrush
        fx.LutParams = lutParams

        img = New Controls.Image()
        img.Source = bmpSource
        img.Width = width
        img.Height = height
        img.Stretch = Stretch.Fill
        img.Effect = fx

        renderSize = New System.Windows.Size(width, height)

        img.Measure(renderSize)
        img.Arrange(New Rect(0, 0, width, height))
        img.UpdateLayout()

        InitialisiereRenderTarget(width, height)

        renderTarget.Render(img)

        result = ConvertRenderTargetBitmapToBitmap(renderTarget)

        img.Effect = Nothing
        img.Source = Nothing

        fx.LutTex = Nothing

        Return result

    End Function

    Private Sub InitialisiereRenderTarget(width As Integer, height As Integer)
        'Erzeugt den RenderTargetBitmap bei Bedarf neu.
        'Ein vorhandener Puffer mit identischen Abmessungen wird wiederverwendet.

        If renderTarget IsNot Nothing AndAlso
           renderTarget.PixelWidth = width AndAlso
           renderTarget.PixelHeight = height Then

            Return

        End If

        renderTarget = New RenderTargetBitmap(width, height, 96.0, 96.0, PixelFormats.Pbgra32)

    End Sub

#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
        'Gibt sämtliche vom Runner gehaltenen Referenzen frei.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        renderTarget = Nothing

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class