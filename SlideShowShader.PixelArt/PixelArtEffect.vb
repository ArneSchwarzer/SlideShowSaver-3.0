Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects

Public NotInheritable Class PixelArtEffect
    Inherits ShaderEffect

    ' *** Variablen am Anfang ***
    Private Shared ReadOnly sharedPs As New PixelShader()

    ' s0: Input
    Public Shared ReadOnly InputProperty As DependencyProperty =
        ShaderEffect.RegisterPixelShaderSamplerProperty(NameOf(Input), GetType(PixelArtEffect), 0)

    ' c1: (cellSize, levelsPerChannel, _, _)
    Public Shared ReadOnly Params0Property As DependencyProperty =
        DependencyProperty.Register(
            NameOf(Params0),
            GetType(Color),
            GetType(PixelArtEffect),
            New UIPropertyMetadata(Colors.Transparent, ShaderEffect.PixelShaderConstantCallback(1))
        )

    Shared Sub New()
        ' Pack-URI ggf. anpassen (Assembly-/Ordnername)
        sharedPs.UriSource = New Uri("/SlideShowShader.PixelArt;component/Shaders/PixelArtShader.ps", UriKind.Relative)
    End Sub

    Public Sub New()
        MyBase.New()
        MyBase.PixelShader = sharedPs
        Me.UpdateShaderValue(InputProperty)
        Me.UpdateShaderValue(Params0Property)
    End Sub

    Public Property Input As Brush
        Get
            Return CType(GetValue(InputProperty), Brush)
        End Get
        Set(ByVal value As Brush)
            SetValue(InputProperty, value)
        End Set
    End Property

    Public Property Params0 As Color
        Get
            Return CType(GetValue(Params0Property), Color)
        End Get
        Set(ByVal value As Color)
            SetValue(Params0Property, value)
        End Set
    End Property

End Class
