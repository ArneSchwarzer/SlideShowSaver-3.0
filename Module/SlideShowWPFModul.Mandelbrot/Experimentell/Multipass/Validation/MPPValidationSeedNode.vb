Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Shapes

Public Class MPPValidationSeedNode
    Inherits MultipassRenderNodeBase

#Region "Klassenvariablen"

    Private ReadOnly seedEffectIntern As MPPValidationSeedEffect

#End Region

#Region "Konstruktor"

    Public Sub New(
        nodeName As String,
        container As Grid,
        renderRectangle As Rectangle)

        MyBase.New(
            nodeName,
            container,
            renderRectangle)

        seedEffectIntern =
            New MPPValidationSeedEffect()

        renderRectangle.Fill =
            Brushes.White

        renderRectangle.Effect =
            seedEffectIntern

    End Sub

#End Region

End Class