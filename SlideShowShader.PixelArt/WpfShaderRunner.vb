Imports System.Drawing
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports SlideShowTools.ImageConversionHandling

Public NotInheritable Class WpfShaderRunner
    Implements IDisposable

#Region "Variablendeklaration"

    Private renderTarget As RenderTargetBitmap

    Private wurdeBereinigt As Boolean

#End Region

#Region "Shader-Ausführung"

    Public Function ApplyPixelArtEffect(srcBitmap As Bitmap, cellSize As Single, levelsPerChannel As Integer) _
        As Bitmap
        'Wendet den HLSL-PixelArt-Effekt auf das übergebene
        'Bitmap an und liefert ein neues GDI-Bitmap zurück.

        Dim width As Integer
        Dim height As Integer

        Dim bmpSource As BitmapSource

        Dim fx As PixelArtEffect
        Dim img As Controls.Image

        Dim texelSize As System.Windows.Point
        Dim shaderParams As System.Windows.Point

        Dim renderSize As System.Windows.Size

        Dim result As Bitmap

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(WpfShaderRunner))

        End If

        If srcBitmap Is Nothing Then

            Throw New ArgumentNullException(NameOf(srcBitmap))

        End If

        width = srcBitmap.Width
        height = srcBitmap.Height

        If width <= 0 OrElse height <= 0 Then

            Throw New ArgumentOutOfRangeException(NameOf(srcBitmap),
                                                  "Das Quellbild besitzt keine gültigen Abmessungen.")

        End If

        cellSize = Math.Max(1.0F, cellSize)
        levelsPerChannel = Math.Max(2, levelsPerChannel)
        bmpSource = ConvertBitmapToImageSource(srcBitmap)

        If bmpSource Is Nothing Then

            Throw New InvalidOperationException("Das Quellbild konnte nicht in ein WPF-Bitmap konvertiert werden.")

        End If

        texelSize = New System.Windows.Point(1.0 / width, 1.0 / height)
        shaderParams = New System.Windows.Point(cellSize, levelsPerChannel)

        fx = New PixelArtEffect()
        fx.TexelSize = texelSize
        fx.Params01 = shaderParams

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

        Try

            renderTarget.Render(img)

            result = ConvertRenderTargetBitmapToBitmap(renderTarget)

        Finally

            img.Effect = Nothing
            img.Source = Nothing

        End Try

        Return result

    End Function

#End Region

#Region "Renderverwaltung"

    Private Sub InitialisiereRenderTarget(width As Integer, height As Integer)
        'Erzeugt den RenderTargetBitmap nur dann neu,
        'wenn noch keiner existiert oder sich die Größe geändert hat.

        If renderTarget IsNot Nothing AndAlso
           renderTarget.PixelWidth = width AndAlso
           renderTarget.PixelHeight = height Then

            Return

        End If

        renderTarget = New RenderTargetBitmap(width, height, 96.0, 96.0, PixelFormats.Pbgra32)

    End Sub

#End Region

#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
        'Löst sämtliche gehaltenen WPF-Renderressourcen.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        renderTarget = Nothing

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class