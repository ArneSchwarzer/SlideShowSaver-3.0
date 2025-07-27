Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations

Public Class ShaderMain
    Implements ISlideShowShader

    'Variablendeklaration
    Public Const nameShader As String = "Originalbild"

    'Eigenschaften
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return nameShader
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

        If clientSize = Nothing Then clientSize = baseImage.Size

        'Ergebnis liefern
        Return baseImage

    End Function

    'Optionen & OptionsDialog
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        Return New ucOptionsShader()
    End Function

End Class
