Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Windows.Media.Imaging
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.WPFHandling

Public Class ShaderMain
    Implements ISlideShowShader

    'Eigenschaften
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return "Originalbild"
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Zeigt das Bild unverändert an."
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    'Events
    Event ShaderFrameIstFertig(bitmap As RenderTargetBitmap) Implements ISlideShowShader.ShaderFrameIstFertig

    'Shader Ausführung
    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader

        Dim rtb As RenderTargetBitmap
        Dim bm As BitmapImage

        If clientSize = Nothing Then clientSize = baseImage.Size

        'Ergebnis sowohl als Event als auch als Bitmap liefern
        bm = ConvertImageToBitmapImage(baseImage)
        rtb = ErzeugeGerahmtesBild(bm, PictureBoxSizeMode.Zoom, clientSize)
        RaiseEvent ShaderFrameIstFertig(rtb)

        Return baseImage

    End Function

    Public Sub StopShader() Implements ISlideShowShader.StopShader
        ' Statischer Shader, keine Aktion notwendig
    End Sub

    'Optionen & OptionsDialog
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        Return New ucOptionsShader()
    End Function

End Class
