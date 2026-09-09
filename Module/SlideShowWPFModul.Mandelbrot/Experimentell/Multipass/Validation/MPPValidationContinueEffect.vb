Imports System.Windows
Imports System.Windows.Media.Effects

Public Class MPPValidationContinueEffect
    Inherits MultipassStateShaderEffectBase

#Region "DependencyProperties"

    Public Shared ReadOnly PassIndexProperty As DependencyProperty =
        DependencyProperty.Register(
            NameOf(PassIndex),
            GetType(Double),
            GetType(MPPValidationContinueEffect),
            New UIPropertyMetadata(
                0.0,
                PixelShaderConstantCallback(0)))

    Public Shared ReadOnly MaxPassesProperty As DependencyProperty =
        DependencyProperty.Register(
            NameOf(MaxPasses),
            GetType(Double),
            GetType(MPPValidationContinueEffect),
            New UIPropertyMetadata(
                1.0,
                PixelShaderConstantCallback(1)))

#End Region

#Region "Eigenschaften"

    Public Property PassIndex As Double
        Get
            Return CDbl(
                GetValue(
                    PassIndexProperty))
        End Get
        Set(value As Double)

            SetValue(
                PassIndexProperty,
                value)

        End Set
    End Property

    Public Property MaxPasses As Double
        Get
            Return CDbl(
                GetValue(
                    MaxPassesProperty))
        End Get
        Set(value As Double)

            SetValue(
                MaxPassesProperty,
                value)

        End Set
    End Property

#End Region

#Region "Konstruktor"

    Public Sub New()

        MyBase.New(
            "MPPValidationContinue.ps")

        UpdateShaderValue(
            PassIndexProperty)

        UpdateShaderValue(
            MaxPassesProperty)

    End Sub

#End Region

End Class