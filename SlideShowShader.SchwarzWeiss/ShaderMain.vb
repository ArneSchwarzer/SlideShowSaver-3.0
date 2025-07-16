Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Imaging
Imports SlideShowInterfaces.InterfaceDeclarations

Public Class ShaderMain
    Implements ISlideShowShader

    'Eigenschaften
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return "Schwarz-Weiß"
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Wandelt das Bild in Graustufen um."
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    'Shader Ausführung
    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        Dim bmp As New Bitmap(baseImage.Width, baseImage.Height)

        ' Farbmatrix zur Umwandlung in Greyscale (nach europäischem Stil 😉)
        Dim colorMatrix As New Imaging.ColorMatrix(New Single()() {
        New Single() {0.299F, 0.299F, 0.299F, 0, 0},
        New Single() {0.587F, 0.587F, 0.587F, 0, 0},
        New Single() {0.114F, 0.114F, 0.114F, 0, 0},
        New Single() {0, 0, 0, 1, 0},
        New Single() {0, 0, 0, 0, 1}
    })

        Dim attributes As New Imaging.ImageAttributes()
        attributes.SetColorMatrix(colorMatrix)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.DrawImage(baseImage,
                    New Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, baseImage.Width, baseImage.Height,
                    GraphicsUnit.Pixel,
                    attributes)
        End Using

        Return bmp
    End Function

    'Optionen & OptionsDialog
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        Return New ucOptionsShader()
    End Function

End Class
