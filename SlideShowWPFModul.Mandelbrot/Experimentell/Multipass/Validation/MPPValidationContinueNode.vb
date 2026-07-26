Imports System
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Shapes

Public Class MPPValidationContinueNode
    Inherits MultipassRenderNodeBase

#Region "Klassenvariablen"

    Private ReadOnly continueEffectIntern As MPPValidationContinueEffect

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

        continueEffectIntern =
            New MPPValidationContinueEffect()

        renderRectangle.Fill =
            Brushes.White

        renderRectangle.Effect =
            continueEffectIntern

    End Sub

#End Region

#Region "Öffentliche Methoden"

    Public Sub SetzeEingang(inputBrush As Brush)

        If inputBrush Is Nothing Then
            Throw New ArgumentNullException(NameOf(inputBrush))
        End If

        continueEffectIntern.StateTexture = Nothing
        continueEffectIntern.StateTexture = inputBrush

        RenderRectangle.InvalidateVisual()
        Container.InvalidateVisual()

    End Sub

    Public Sub TrenneEingang()

        continueEffectIntern.StateTexture = Nothing

    End Sub

    Public Sub SetzePassInformation(passIndex As Integer, maxPasses As Integer)

        If passIndex < 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(passIndex))
        End If

        If maxPasses <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(maxPasses))
        End If

        continueEffectIntern.PassIndex = CDbl(passIndex)
        continueEffectIntern.MaxPasses = CDbl(maxPasses)

        RenderRectangle.InvalidateVisual()
        Container.InvalidateVisual()

    End Sub

#End Region

End Class