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

    ' c0: (1/width, 1/height)
    Public Shared ReadOnly TexelSizeProperty As DependencyProperty =
        DependencyProperty.Register(
            NameOf(TexelSize),
            GetType(System.Windows.Point),
            GetType(PixelArtEffect),
            New UIPropertyMetadata(New System.Windows.Point(1.0, 1.0), ShaderEffect.PixelShaderConstantCallback(0))
        )

    ' c1: (cellPx, levels)
    Public Shared ReadOnly Params01Property As DependencyProperty =
        DependencyProperty.Register(
            NameOf(Params01),
            GetType(System.Windows.Point),
            GetType(PixelArtEffect),
            New UIPropertyMetadata(New System.Windows.Point(8.0, 6.0), ShaderEffect.PixelShaderConstantCallback(1))
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

    ' TexelSize -> c0.xy
    Public Property TexelSize As System.Windows.Point
        Get
            Return CType(GetValue(TexelSizeProperty), System.Windows.Point)
        End Get
        Set(ByVal value As System.Windows.Point)
            SetValue(TexelSizeProperty, value)
        End Set
    End Property

    ' (cellPx, levels) -> c1.xy
    Public Property Params01 As System.Windows.Point
        Get
            Return CType(GetValue(Params01Property), System.Windows.Point)
        End Get
        Set(ByVal value As System.Windows.Point)
            SetValue(Params01Property, value)
        End Set
    End Property

    ' Shader laden
    Shared Sub New()
        sharedPs.UriSource = New Uri("/SlideShowShader.PixelArt;component/Shaders/PixelArtShader.ps", UriKind.Relative)
    End Sub

    Public Sub New()
        MyBase.New()
        MyBase.PixelShader = sharedPs
        ' Wichtig: alle DPs beim Effekt registrieren
        Me.UpdateShaderValue(InputProperty)
        Me.UpdateShaderValue(TexelSizeProperty)
        Me.UpdateShaderValue(Params01Property)
    End Sub
End Class
