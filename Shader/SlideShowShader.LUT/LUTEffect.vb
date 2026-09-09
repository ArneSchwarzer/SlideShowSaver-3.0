Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Media3D

' ShaderEffect für den LUT-Shader (sampler0 = Bild, sampler1 = LUT-Atlas)
' HLSL-Seite erwartet:
'   sampler2D srcTex : register(s0);
'   sampler2D lutTex : register(s1);
'   float4    lutParams : register(c1); // (N, 1/(N-1), strength, _)
Public NotInheritable Class LUTEffect
    Inherits ShaderEffect

    ' *** Variablen am Anfang ***
    Private Shared ReadOnly _ps As New PixelShader()

    ' s0 – Eingabe (ImplicitInput, muss nicht gesetzt werden)
    Public Shared ReadOnly InputProperty As DependencyProperty =
        ShaderEffect.RegisterPixelShaderSamplerProperty(NameOf(Input), GetType(LUTEffect), 0)

    ' s1 – LUT-Atlas (ImageBrush auf BitmapSource N*N x N)
    Public Shared ReadOnly LutTexProperty As DependencyProperty =
        ShaderEffect.RegisterPixelShaderSamplerProperty(NameOf(LutTex), GetType(LUTEffect), 1)

    ' c1 – (N, 1/(N-1), strength, _)
    Public Shared ReadOnly LutParamsProperty As DependencyProperty =
        DependencyProperty.Register(
            NameOf(LutParams),
            GetType(Point4D),
            GetType(LUTEffect),
            New UIPropertyMetadata(New Point4D(33.0, 1.0 / 32.0, 1.0, 0.0),
                                   ShaderEffect.PixelShaderConstantCallback(1))
        )

    ' Properties
    Public Property Input As Brush
        Get
            Return CType(GetValue(InputProperty), Brush)
        End Get
        Set(ByVal value As Brush)
            SetValue(InputProperty, value)
        End Set
    End Property

    Public Property LutTex As Brush
        Get
            Return CType(GetValue(LutTexProperty), Brush)
        End Get
        Set(ByVal value As Brush)
            SetValue(LutTexProperty, value)
        End Set
    End Property

    Public Property LutParams As Point4D
        Get
            Return CType(GetValue(LutParamsProperty), Point4D)
        End Get
        Set(ByVal value As Point4D)
            SetValue(LutParamsProperty, value)
        End Set
    End Property

    ' Shader laden (Pack-URI auf dein Projekt "SlideShowShader.LUT")
    Shared Sub New()
        _ps.UriSource = New Uri("/SlideShowShader.LUT;component/Shaders/LUTShader.ps", UriKind.Relative)
    End Sub

    Public Sub New()
        MyBase.New()
        PixelShader = _ps
        ' WPF wissen lassen, welche DPs an Shader gebunden sind
        UpdateShaderValue(InputProperty)
        UpdateShaderValue(LutTexProperty)
        UpdateShaderValue(LutParamsProperty)
    End Sub
End Class
