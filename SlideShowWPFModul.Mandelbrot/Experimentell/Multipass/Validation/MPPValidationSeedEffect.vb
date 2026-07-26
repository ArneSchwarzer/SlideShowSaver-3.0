Imports System.Windows.Media.Effects

Public Class MPPValidationSeedEffect
    Inherits MultipassStateShaderEffectBase

    Public Sub New()

        MyBase.New(
            "MPPValidationSeed.ps")

        StateTexture =
            Effect.ImplicitInput

    End Sub

End Class