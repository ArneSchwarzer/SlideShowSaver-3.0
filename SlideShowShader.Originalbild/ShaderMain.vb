Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowInterfaces.InterfaceDeclarations

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

    'Shader Ausführung
    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader

        Return baseImage

    End Function

    'Optionen & OptionsDialog
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        Return New ucOptionsShader()
    End Function

End Class
