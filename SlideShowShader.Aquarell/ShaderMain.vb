Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ShaderMain
    Implements ISlideShowShader

#Region "Variablendeklaration und Strukturen"

    'Allgemeines
    Public Const SLIDESHOWSHADER_AQUARELL_FULLPATH As String =
        SLIDESHOWSHADER_PATH &
        "Aquarell\"

    Public Const nameShader As String = "Aquarell"

    Private wurdeBereinigt As Boolean

    Public Enum ShaderModus

        Bilateral = 0
        Kuwahara = 1
        AnisotropicKuwahara = 2

    End Enum

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName

        Get

            Return nameShader

        End Get

    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung

        Get

            Return "Panta rhei!"

        End Get

    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion

        Get

            Return New Version(0, 1, 0, 0)

        End Get

    End Property

#End Region

#Region "Shader-Ausführung"

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "",
                              Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader

        Dim renderer As D3DRenderer
        Dim ergebnis As Bitmap

        renderer = Nothing
        ergebnis = Nothing

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(ShaderMain))
        End If

        If baseImage Is Nothing Then
            Throw New ArgumentNullException(NameOf(baseImage))
        End If

        Try

            renderer = New D3DRenderer()

            renderer.Initialisiere(baseImage)

            ergebnis = renderer.RenderTestbild()

            Return ergebnis

        Finally

            If renderer IsNot Nothing Then

                renderer.Dispose()
                renderer = Nothing

            End If

        End Try

    End Function

#End Region

#Region "Dialog und Optionen"

    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Liest die aktuellen Settings ein und stellt sie dem
        'Options-Control über die SettingsInbox bereit.

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(ShaderMain))
        End If

        Return New ucOptionsShader()

    End Function

#End Region

#Region "Settings und Defaultwerte"

    Friend Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Werte des Shaders.

        Dim defaults As New Dictionary(Of String, String)

        defaults("Glättungsalgorithmus") = "Kuwahara"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Liest die aktuellen Shadereinstellungen aus der Registry
        'oder verwendet die definierten Defaultwerte.

        Dim defaults As Dictionary(Of String, String)

        defaults = GetShaderDefaultSettings()

    End Sub

#End Region


#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
        'Markiert die Shaderinstanz als endgültig freigegeben.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class