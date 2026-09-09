Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations

Public Class ShaderMain
    Implements ISlideShowShader

#Region "Variablendeklaration"

    Public Const nameShader As String = "Originalbild"

    Private wurdeBereinigt As Boolean

#End Region

#Region "Eigenschaften"

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

#End Region

#Region "Shader-Ausführung"

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "",
                              Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Liefert eine unveränderte Kopie des Quellbildes.

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ShaderMain))

        End If

        If baseImage Is Nothing Then

            Throw New ArgumentNullException(NameOf(baseImage))

        End If

        Return DirectCast(baseImage.Clone(), Image)

    End Function

#End Region

#Region "Optionen / Optionsdialog"

    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ShaderMain))

        End If

        Return New ucOptionsShader()

    End Function

#End Region

#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class