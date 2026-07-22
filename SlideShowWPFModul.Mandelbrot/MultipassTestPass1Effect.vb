Imports System
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects

Public Class MultipassTestPass1Effect
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
                        "MultipassTestPass1.ps",
                        UriKind.Absolute)
            }

    End Sub

#End Region

#Region "Sampler"

    Public Shared ReadOnly InputProperty As DependencyProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            GetType(MultipassTestPass1Effect),
            0,
            SamplingMode.NearestNeighbor)

    Public Property Input As Brush
        Get
            Return CType(
                GetValue(
                    InputProperty),
                Brush)
        End Get
        Set(value As Brush)
            SetValue(
                InputProperty,
                value)
        End Set
    End Property

#End Region

    Public Sub New()

        PixelShader =
            pixelShaderIntern

        UpdateShaderValue(
            InputProperty)

    End Sub

End Class