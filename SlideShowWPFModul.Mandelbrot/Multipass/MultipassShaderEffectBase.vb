Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects

Public MustInherit Class MultipassShaderEffectBase
    Inherits ShaderEffect

#Region "Klassenvariablen"

    Private Shared ReadOnly shaderCache As Dictionary(Of String, PixelShader)
    Private Shared ReadOnly shaderCacheLock As Object

#End Region

#Region "Konstruktoren"

    Shared Sub New()

        shaderCache =
            New Dictionary(Of String, PixelShader)(
                StringComparer.OrdinalIgnoreCase)

        shaderCacheLock =
            New Object()

    End Sub

    Protected Sub New(
        shaderDateiname As String,
        ParamArray shaderProperties() As DependencyProperty)

        Dim pixelShaderIntern As PixelShader
        Dim shaderProperty As DependencyProperty

        If String.IsNullOrWhiteSpace(
            shaderDateiname) Then

            Throw New ArgumentException(
                "Der Shaderdateiname darf nicht leer sein.",
                NameOf(shaderDateiname))

        End If

        pixelShaderIntern =
            LadePixelShader(
                shaderDateiname)

        PixelShader =
            pixelShaderIntern

        If shaderProperties IsNot Nothing Then

            For Each shaderProperty In shaderProperties

                If shaderProperty IsNot Nothing Then

                    UpdateShaderValue(
                        shaderProperty)

                End If

            Next

        End If

    End Sub

#End Region

#Region "Shader laden"

    Private Shared Function LadePixelShader(
        shaderDateiname As String) As PixelShader

        Dim pixelShaderIntern As PixelShader
        Dim shaderUri As Uri

        SyncLock shaderCacheLock

            If shaderCache.TryGetValue(
                shaderDateiname,
                pixelShaderIntern) Then

                Return pixelShaderIntern

            End If

            shaderUri =
                New Uri(
                    "pack://application:,,,/" &
                    "SlideShowWPFModul.Mandelbrot;" &
                    "component/Shader/" &
                    shaderDateiname,
                    UriKind.Absolute)

            pixelShaderIntern =
                New PixelShader With {
                    .UriSource = shaderUri
                }

            shaderCache.Add(
                shaderDateiname,
                pixelShaderIntern)

        End SyncLock

        Return pixelShaderIntern

    End Function

#End Region

End Class