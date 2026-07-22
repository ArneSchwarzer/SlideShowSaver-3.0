Imports System
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects

Public Class MultipassTestPass2Effect
    Inherits ShaderEffect

#Region "Shader"

    Private Shared ReadOnly pixelShaderIntern As PixelShader

    Shared Sub New()

        pixelShaderIntern =
            New PixelShader With {
                .UriSource =
                    New Uri(
                        "pack://application:,,,/" &
                        "SlideShowWPFModul.Mandelbrot;" &
                        "component/Shader/" &
                        "MultipassTestPass2.ps",
                        UriKind.Absolute)
            }

    End Sub

#End Region

#Region "Sampler"

    Public Shared ReadOnly StateTextureProperty As DependencyProperty =
        RegisterPixelShaderSamplerProperty(
            "StateTexture",
            GetType(MultipassTestPass2Effect),
            0,
            SamplingMode.NearestNeighbor)

    Public Property StateTexture As Brush
        Get
            Return CType(
                GetValue(
                    StateTextureProperty),
                Brush)
        End Get
        Set(value As Brush)
            SetValue(
                StateTextureProperty,
                value)
        End Set
    End Property

#End Region

    Public Sub New()

        PixelShader =
            pixelShaderIntern

        UpdateShaderValue(
            StateTextureProperty)

    End Sub

End Class
