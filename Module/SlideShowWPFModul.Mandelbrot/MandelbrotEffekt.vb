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

        UpdateShaderValue(CenterXHighProperty)
        UpdateShaderValue(CenterXLowProperty)
        UpdateShaderValue(CenterYHighProperty)
        UpdateShaderValue(CenterYLowProperty)
        UpdateShaderValue(ScaleHighProperty)
        UpdateShaderValue(ScaleLowProperty)
        UpdateShaderValue(MaxIterationsProperty)
        UpdateShaderValue(ViewportWidthProperty)
        UpdateShaderValue(ViewportHeightProperty)
        UpdateShaderValue(GradientOffsetProperty)
        UpdateShaderValue(RotationProperty)
        UpdateShaderValue(GradientTextureProperty)
    End Sub

    Public Shared ReadOnly CenterXHighProperty As DependencyProperty =
    DependencyProperty.Register(
        "CenterXHigh",
        GetType(Single),
        GetType(MandelbrotEffect),
        New UIPropertyMetadata(-0.5F, PixelShaderConstantCallback(0)))

    Public Property CenterXHigh As Single
        Get
            Return CSng(GetValue(CenterXHighProperty))
        End Get
        Set(value As Single)
            SetValue(CenterXHighProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CenterXLowProperty As DependencyProperty =
    DependencyProperty.Register(
        "CenterXLow",
        GetType(Single),
        GetType(MandelbrotEffect),
        New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(1)))

    Public Property CenterXLow As Single
        Get
            Return CSng(GetValue(CenterXLowProperty))
        End Get
        Set(value As Single)
            SetValue(CenterXLowProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CenterYHighProperty As DependencyProperty =
    DependencyProperty.Register(
        "CenterYHigh",
        GetType(Single),
        GetType(MandelbrotEffect),
        New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(2)))

    Public Property CenterYHigh As Single
        Get
            Return CSng(GetValue(CenterYHighProperty))
        End Get
        Set(value As Single)
            SetValue(CenterYHighProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CenterYLowProperty As DependencyProperty =
    DependencyProperty.Register(
        "CenterYLow",
        GetType(Single),
        GetType(MandelbrotEffect),
        New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(3)))

    Public Property CenterYLow As Single
        Get
            Return CSng(GetValue(CenterYLowProperty))
        End Get
        Set(value As Single)
            SetValue(CenterYLowProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ScaleHighProperty As DependencyProperty =
    DependencyProperty.Register(
        "ScaleHigh",
        GetType(Single),
        GetType(MandelbrotEffect),
        New UIPropertyMetadata(3.0F, PixelShaderConstantCallback(4)))

    Public Property ScaleHigh As Single
        Get
            Return CSng(GetValue(ScaleHighProperty))
        End Get
        Set(value As Single)
            SetValue(ScaleHighProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ScaleLowProperty As DependencyProperty =
    DependencyProperty.Register(
        "ScaleLow",
        GetType(Single),
        GetType(MandelbrotEffect),
        New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(5)))

    Public Property ScaleLow As Single
        Get
            Return CSng(GetValue(ScaleLowProperty))
        End Get
        Set(value As Single)
            SetValue(ScaleLowProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MaxIterationsProperty As DependencyProperty =
        DependencyProperty.Register(
            "MaxIterations",
            GetType(Single),
            GetType(MandelbrotEffect),
            New UIPropertyMetadata(200.0F, PixelShaderConstantCallback(6)))

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
            New UIPropertyMetadata(1920.0F, PixelShaderConstantCallback(7)))

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
            New UIPropertyMetadata(1080.0F, PixelShaderConstantCallback(8)))

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
            New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(9)))

    Public Property GradientOffset As Single
        Get
            Return CSng(GetValue(GradientOffsetProperty))
        End Get
        Set(value As Single)
            SetValue(GradientOffsetProperty, value)
        End Set
    End Property

    Public Shared ReadOnly RotationProperty As DependencyProperty =
    DependencyProperty.Register(
        "Rotation",
        GetType(Single),
        GetType(MandelbrotEffect),
        New UIPropertyMetadata(0.0F, PixelShaderConstantCallback(10)))

    Public Property Rotation As Single
        Get
            Return CSng(GetValue(RotationProperty))
        End Get
        Set(value As Single)
            SetValue(RotationProperty, value)
        End Set
    End Property

    Public Shared ReadOnly GradientTextureProperty As DependencyProperty =
    ShaderEffect.RegisterPixelShaderSamplerProperty(
        "GradientTexture",
        GetType(MandelbrotEffect),
        0)

    Public Property GradientTexture As Brush
        Get
            Return CType(GetValue(GradientTextureProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(GradientTextureProperty, value)
        End Set
    End Property
End Class