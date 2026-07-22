Imports System
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects

Public Class MandelbrotPerturbationEffect
    Inherits ShaderEffect

#Region "Shader"

    Private Shared ReadOnly pixelShaderIntern As PixelShader

    Shared Sub New()

        pixelShaderIntern = New PixelShader With {
            .UriSource = New Uri(
                "pack://application:,,,/SlideShowWPFModul.Mandelbrot;component/Shader/MandelbrotPerturbation.ps",
                UriKind.Absolute)
        }

    End Sub

#End Region

#Region "Sampler"

    's0: Farbgradient
    Public Shared ReadOnly GradientTextureProperty As DependencyProperty =
        RegisterPixelShaderSamplerProperty(
            "GradientTexture",
            GetType(MandelbrotPerturbationEffect),
            0,
            SamplingMode.Bilinear)

    Public Property GradientTexture As Brush
        Get
            Return CType(GetValue(GradientTextureProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(GradientTextureProperty, value)
        End Set
    End Property

    '--------------------------------------------------------------
    's1: Referenzorbit – Real High
    '--------------------------------------------------------------

    Public Shared ReadOnly ReferenceOrbitRealHighTextureProperty As DependencyProperty =
    RegisterPixelShaderSamplerProperty(
        "ReferenceOrbitRealHighTexture",
        GetType(MandelbrotPerturbationEffect),
        1,
        SamplingMode.NearestNeighbor)

    Public Property ReferenceOrbitRealHighTexture As Brush
        Get
            Return CType(
            GetValue(
                ReferenceOrbitRealHighTextureProperty),
            Brush)
        End Get
        Set(value As Brush)
            SetValue(
            ReferenceOrbitRealHighTextureProperty,
            value)
        End Set
    End Property


    '--------------------------------------------------------------
    's2: Referenzorbit – Imaginär High
    '--------------------------------------------------------------

    Public Shared ReadOnly ReferenceOrbitImaginaryHighTextureProperty As DependencyProperty =
    RegisterPixelShaderSamplerProperty(
        "ReferenceOrbitImaginaryHighTexture",
        GetType(MandelbrotPerturbationEffect),
        2,
        SamplingMode.NearestNeighbor)

    Public Property ReferenceOrbitImaginaryHighTexture As Brush
        Get
            Return CType(
            GetValue(
                ReferenceOrbitImaginaryHighTextureProperty),
            Brush)
        End Get
        Set(value As Brush)
            SetValue(
            ReferenceOrbitImaginaryHighTextureProperty,
            value)
        End Set
    End Property


    '--------------------------------------------------------------
    's3: Referenzorbit – Real Low
    '--------------------------------------------------------------

    Public Shared ReadOnly ReferenceOrbitRealLowTextureProperty As DependencyProperty =
    RegisterPixelShaderSamplerProperty(
        "ReferenceOrbitRealLowTexture",
        GetType(MandelbrotPerturbationEffect),
        3,
        SamplingMode.NearestNeighbor)

    Public Property ReferenceOrbitRealLowTexture As Brush
        Get
            Return CType(
            GetValue(
                ReferenceOrbitRealLowTextureProperty),
            Brush)
        End Get
        Set(value As Brush)
            SetValue(
            ReferenceOrbitRealLowTextureProperty,
            value)
        End Set
    End Property


    '--------------------------------------------------------------
    's4: Referenzorbit – Imaginär Low
    '--------------------------------------------------------------

    Public Shared ReadOnly ReferenceOrbitImaginaryLowTextureProperty As DependencyProperty =
    RegisterPixelShaderSamplerProperty(
        "ReferenceOrbitImaginaryLowTexture",
        GetType(MandelbrotPerturbationEffect),
        4,
        SamplingMode.NearestNeighbor)

    Public Property ReferenceOrbitImaginaryLowTexture As Brush
        Get
            Return CType(
            GetValue(
                ReferenceOrbitImaginaryLowTextureProperty),
            Brush)
        End Get
        Set(value As Brush)
            SetValue(
            ReferenceOrbitImaginaryLowTextureProperty,
            value)
        End Set
    End Property

#End Region

#Region "Shaderkonstanten"

    'c0
    Public Shared ReadOnly ScaleHighProperty As DependencyProperty =
        DependencyProperty.Register(
            "ScaleHigh",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                3.0F,
                PixelShaderConstantCallback(0)))

    Public Property ScaleHigh As Single
        Get
            Return CSng(GetValue(ScaleHighProperty))
        End Get
        Set(value As Single)
            SetValue(ScaleHighProperty, value)
        End Set
    End Property

    'c1
    Public Shared ReadOnly ScaleLowProperty As DependencyProperty =
        DependencyProperty.Register(
            "ScaleLow",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                0.0F,
                PixelShaderConstantCallback(1)))

    Public Property ScaleLow As Single
        Get
            Return CSng(GetValue(ScaleLowProperty))
        End Get
        Set(value As Single)
            SetValue(ScaleLowProperty, value)
        End Set
    End Property

    'c2
    Public Shared ReadOnly MaxIterationsProperty As DependencyProperty =
        DependencyProperty.Register(
            "MaxIterations",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                100.0F,
                PixelShaderConstantCallback(2)))

    Public Property MaxIterations As Single
        Get
            Return CSng(GetValue(MaxIterationsProperty))
        End Get
        Set(value As Single)
            SetValue(MaxIterationsProperty, value)
        End Set
    End Property

    'c3
    Public Shared ReadOnly OrbitLengthProperty As DependencyProperty =
        DependencyProperty.Register(
            "OrbitLength",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                1.0F,
                PixelShaderConstantCallback(3)))

    Public Property OrbitLength As Single
        Get
            Return CSng(GetValue(OrbitLengthProperty))
        End Get
        Set(value As Single)
            SetValue(OrbitLengthProperty, value)
        End Set
    End Property

    'c4
    Public Shared ReadOnly OrbitTextureWidthProperty As DependencyProperty =
        DependencyProperty.Register(
            "OrbitTextureWidth",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                2.0F,
                PixelShaderConstantCallback(4)))

    Public Property OrbitTextureWidth As Single
        Get
            Return CSng(GetValue(OrbitTextureWidthProperty))
        End Get
        Set(value As Single)
            SetValue(OrbitTextureWidthProperty, value)
        End Set
    End Property

    'c9
    Public Shared ReadOnly OrbitTextureHeightProperty As DependencyProperty =
    DependencyProperty.Register(
        "OrbitTextureHeight",
        GetType(Single),
        GetType(MandelbrotPerturbationEffect),
        New UIPropertyMetadata(
            1.0F,
            PixelShaderConstantCallback(9)))

    Public Property OrbitTextureHeight As Single
        Get
            Return CSng(
            GetValue(
                OrbitTextureHeightProperty))
        End Get
        Set(value As Single)
            SetValue(
            OrbitTextureHeightProperty,
            value)
        End Set
    End Property

    'c5
    Public Shared ReadOnly ViewportWidthProperty As DependencyProperty =
        DependencyProperty.Register(
            "ViewportWidth",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                1920.0F,
                PixelShaderConstantCallback(5)))

    Public Property ViewportWidth As Single
        Get
            Return CSng(GetValue(ViewportWidthProperty))
        End Get
        Set(value As Single)
            SetValue(ViewportWidthProperty, value)
        End Set
    End Property

    'c6
    Public Shared ReadOnly ViewportHeightProperty As DependencyProperty =
        DependencyProperty.Register(
            "ViewportHeight",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                1080.0F,
                PixelShaderConstantCallback(6)))

    Public Property ViewportHeight As Single
        Get
            Return CSng(GetValue(ViewportHeightProperty))
        End Get
        Set(value As Single)
            SetValue(ViewportHeightProperty, value)
        End Set
    End Property

    'c7
    Public Shared ReadOnly GradientOffsetProperty As DependencyProperty =
        DependencyProperty.Register(
            "GradientOffset",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                0.0F,
                PixelShaderConstantCallback(7)))

    Public Property GradientOffset As Single
        Get
            Return CSng(GetValue(GradientOffsetProperty))
        End Get
        Set(value As Single)
            SetValue(GradientOffsetProperty, value)
        End Set
    End Property

    'c8 – bereits für die spätere Rotation reserviert
    Public Shared ReadOnly RotationProperty As DependencyProperty =
        DependencyProperty.Register(
            "Rotation",
            GetType(Single),
            GetType(MandelbrotPerturbationEffect),
            New UIPropertyMetadata(
                0.0F,
                PixelShaderConstantCallback(8)))

    Public Property Rotation As Single
        Get
            Return CSng(GetValue(RotationProperty))
        End Get
        Set(value As Single)
            SetValue(RotationProperty, value)
        End Set
    End Property

#End Region

#Region "Konstruktor"

    Public Sub New()

        'Bei PS 3.0 sollte der PixelShader gesetzt sein,
        'bevor die Samplerwerte registriert/aktualisiert werden.
        PixelShader = pixelShaderIntern

        UpdateShaderValue(GradientTextureProperty)
        UpdateShaderValue(ReferenceOrbitRealHighTextureProperty)
        UpdateShaderValue(ReferenceOrbitImaginaryHighTextureProperty)
        UpdateShaderValue(ReferenceOrbitRealLowTextureProperty)
        UpdateShaderValue(ReferenceOrbitImaginaryLowTextureProperty)

        UpdateShaderValue(ScaleHighProperty)
        UpdateShaderValue(ScaleLowProperty)
        UpdateShaderValue(MaxIterationsProperty)
        UpdateShaderValue(OrbitLengthProperty)
        UpdateShaderValue(OrbitTextureWidthProperty)
        UpdateShaderValue(OrbitTextureHeightProperty)
        UpdateShaderValue(ViewportWidthProperty)
        UpdateShaderValue(ViewportHeightProperty)
        UpdateShaderValue(GradientOffsetProperty)
        UpdateShaderValue(RotationProperty)

    End Sub

#End Region

End Class