Imports System
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Shapes

Public Class MPPValidationOutputNode
    Inherits MultipassRenderNodeBase

#Region "Klassenvariablen"

    Private ReadOnly outputEffectIntern As MPPValidationOutputEffect

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

        outputEffectIntern =
            New MPPValidationOutputEffect()

        renderRectangle.Fill =
            Brushes.White

        renderRectangle.Effect =
            outputEffectIntern

    End Sub

#End Region

#Region "Öffentliche Methoden"

    Public Sub SetzeEingang(
        inputBrush As Brush)

        If inputBrush Is Nothing Then

            Throw New ArgumentNullException(
                NameOf(inputBrush))

        End If

        outputEffectIntern.StateTexture =
            inputBrush

        RenderRectangle.InvalidateVisual()
        Container.InvalidateVisual()

    End Sub

#End Region

End Class