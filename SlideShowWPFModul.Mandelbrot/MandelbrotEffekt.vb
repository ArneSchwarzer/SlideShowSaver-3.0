Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects

Public Class MandelbrotEffect
    Inherits ShaderEffect

    Private Shared ReadOnly _pixelShader As New PixelShader()

    Shared Sub New()
        _pixelShader.UriSource = New Uri(
            "pack://application:,,,/SlideShowWPFModul.Mandelbrot;component/Shader/Mandelbrot.ps",
            UriKind.Absolute)
    End Sub

    Public Sub New()
        PixelShader = _pixelShader

        UpdateShaderValue(CenterXProperty)
        UpdateShaderValue(CenterYProperty)
        UpdateShaderValue(ScaleProperty)
        UpdateShaderValue(MaxIterationsProperty)
        UpdateShaderValue(ViewportWidthProperty)
        UpdateShaderValue(ViewportHeightProperty)
        UpdateShaderValue(GradientOffsetProperty)
        UpdateShaderValue(GradientIndexProperty)
    End Sub

    Public Shared ReadOnly CenterXProperty As DependencyProperty =
        DependencyProperty.Register(
            "CenterX",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(-0.5F, PixelShaderConstantCallback(0)))

    Public Property CenterX As Single
        Get
            Return CSng(GetValue(CenterXProperty))
        End Get
        Set(value As Single)
            SetValue(CenterXProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CenterYProperty As DependencyProperty =
        DependencyProperty.Register(
            "CenterY",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(1)))

    Public Property CenterY As Single
        Get
            Return CSng(GetValue(CenterYProperty))
        End Get
        Set(value As Single)
            SetValue(CenterYProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ScaleProperty As DependencyProperty =
        DependencyProperty.Register(
            "Scale",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(3.0F, PixelShaderConstantCallback(2)))

    Public Property Scale As Single
        Get
            Return CSng(GetValue(ScaleProperty))
        End Get
        Set(value As Single)
            SetValue(ScaleProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MaxIterationsProperty As DependencyProperty =
        DependencyProperty.Register(
            "MaxIterations",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(200.0F, PixelShaderConstantCallback(3)))

    Public Property MaxIterations As Single
        Get
            Return CSng(GetValue(MaxIterationsProperty))
        End Get
        Set(value As Single)
            SetValue(MaxIterationsProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ViewportWidthProperty As DependencyProperty =
        DependencyProperty.Register(
            "ViewportWidth",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(1920.0F, PixelShaderConstantCallback(4)))

    Public Property ViewportWidth As Single
        Get
            Return CSng(GetValue(ViewportWidthProperty))
        End Get
        Set(value As Single)
            SetValue(ViewportWidthProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ViewportHeightProperty As DependencyProperty =
        DependencyProperty.Register(
            "ViewportHeight",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(1080.0F, PixelShaderConstantCallback(5)))

    Public Property ViewportHeight As Single
        Get
            Return CSng(GetValue(ViewportHeightProperty))
        End Get
        Set(value As Single)
            SetValue(ViewportHeightProperty, value)
        End Set
    End Property

    Public Shared ReadOnly GradientOffsetProperty As DependencyProperty =
        DependencyProperty.Register(
            "GradientOffset",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(6)))

    Public Property GradientOffset As Single
        Get
            Return CSng(GetValue(GradientOffsetProperty))
        End Get
        Set(value As Single)
            SetValue(GradientOffsetProperty, value)
        End Set
    End Property

    Public Shared ReadOnly GradientIndexProperty As DependencyProperty =
        DependencyProperty.Register(
            "GradientIndex",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(7)))

    Public Property GradientIndex As Single
        Get
            Return CSng(GetValue(GradientIndexProperty))
        End Get
        Set(value As Single)
            SetValue(GradientIndexProperty, value)
        End Set
    End Property

End Class