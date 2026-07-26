Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Effects

Public MustInherit Class MultipassStateShaderEffectBase
    Inherits MultipassShaderEffectBase

#Region "Sampler"

    Public Shared ReadOnly StateTextureProperty As DependencyProperty =
        RegisterPixelShaderSamplerProperty(
            NameOf(StateTexture),
            GetType(MultipassStateShaderEffectBase),
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

#Region "Konstruktor"

    Protected Sub New(shaderDateiname As String)

        MyBase.New(shaderDateiname, StateTextureProperty)

    End Sub

#End Region

End Class