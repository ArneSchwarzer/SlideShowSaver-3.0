Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects

Public Class RevealShaderEffect
    Inherits ShaderEffect

#Region "Variablendeklaration"

    Private Shared ReadOnly revealPixelShader As PixelShader

    Public Shared ReadOnly InputProperty As DependencyProperty
    Public Shared ReadOnly NewImageProperty As DependencyProperty
    Public Shared ReadOnly MaskImageProperty As DependencyProperty
    Public Shared ReadOnly ProgressProperty As DependencyProperty

#End Region

#Region "Initialisierung"

    Shared Sub New()

        revealPixelShader = New PixelShader()

        revealPixelShader.UriSource =
            New Uri(
                "pack://application:,,,/SlideShowTransition.BrennendesPapier;component/Shader/RevealShader.ps",
                UriKind.Absolute)

        InputProperty = RegisterPixelShaderSamplerProperty("Input", GetType(RevealShaderEffect), 0)

        NewImageProperty = RegisterPixelShaderSamplerProperty("NewImage", GetType(RevealShaderEffect), 1)

        MaskImageProperty = RegisterPixelShaderSamplerProperty("MaskImage", GetType(RevealShaderEffect), 2)

        ProgressProperty = DependencyProperty.Register("Progress", GetType(Double), GetType(RevealShaderEffect),
                New UIPropertyMetadata(
                    0.0,
                    PixelShaderConstantCallback(0)))

    End Sub

    Public Sub New()

        PixelShader = revealPixelShader

        UpdateShaderValue(InputProperty)
        UpdateShaderValue(NewImageProperty)
        UpdateShaderValue(MaskImageProperty)
        UpdateShaderValue(ProgressProperty)

    End Sub

#End Region

#Region "Eigenschaften"

    Public Property Input As Brush

        Get
            Return DirectCast(
                GetValue(InputProperty),
                Brush)
        End Get

        Set(value As Brush)
            SetValue(InputProperty, value)
        End Set

    End Property

    Public Property NewImage As Brush

        Get
            Return DirectCast(
                GetValue(NewImageProperty),
                Brush)
        End Get

        Set(value As Brush)
            SetValue(NewImageProperty, value)
        End Set

    End Property

    Public Property MaskImage As Brush

        Get
            Return DirectCast(
                GetValue(MaskImageProperty),
                Brush)
        End Get

        Set(value As Brush)
            SetValue(MaskImageProperty, value)
        End Set

    End Property

    Public Property Progress As Double

        Get
            Return CDbl(
                GetValue(ProgressProperty))
        End Get

        Set(value As Double)
            SetValue(
                ProgressProperty,
                value)
        End Set

    End Property

#End Region

End Class