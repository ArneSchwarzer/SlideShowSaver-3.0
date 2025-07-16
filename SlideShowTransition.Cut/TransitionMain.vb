Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Drawing.Drawing2D
Imports SlideShowTools.GraphicsSizeModeHandling
Imports System.Runtime.Remoting.Messaging

Public Class TransitionMain
    Implements ISlideShowTransition

    'Variablendeklaration
    Private newImg As Image
    Private newPicBoxSM As PictureBoxSizeMode
    Private renderTarget As Graphics
    Private clntSize As Size

    Public ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName
        Get
            Return "Direkter Übergang"
        End Get
    End Property

    Public ReadOnly Property TransitionKurzBeschreibung As String Implements ISlideShowTransition.TransitionKurzBeschreibung
        Get
            Return "Wechselt sofort auf das nächste Bild"
        End Get
    End Property

    Public ReadOnly Property TransitionVersion As Version Implements ISlideShowTransition.TransitionVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    Public Event TransitionIsRunning As ISlideShowTransition.TransitionIsRunningEventHandler Implements ISlideShowTransition.TransitionIsRunning

    Public Sub RunTransition(oldImage As System.Drawing.Image, picBoxModeOld As Windows.Forms.PictureBoxSizeMode, newImage As System.Drawing.Image, picBoxModeNew As Windows.Forms.PictureBoxSizeMode, targetGraphics As System.Drawing.Graphics, Optional clientSize As System.Drawing.Size = Nothing, Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition
        'Im Normalfall braucht diese 'Transition' ja eh nicht zu laufen...

        'Formal noch einmal TransitionIsRunning werfen.
        RaiseEvent TransitionIsRunning(True)

        If durationMs <= 0 Then
            RaiseEvent TransitionIsRunning(False)
            Exit Sub
        End If

        'Parameter in interne Variablen überführen
        newImg = newImage
        newPicBoxSM = picBoxModeNew
        renderTarget = targetGraphics
        clntSize = clientSize

        If clntSize.IsEmpty Then
            clntSize = renderTarget.VisibleClipBounds.Size.ToSize()
        End If

        'Gleich zum Ende der Transition.
        StopTransition()

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Zeichnet newImage auf den Zeichenbereich und gut
        Dim drawRect As Rectangle
        Dim targetRect As Rectangle

        'Grafik vorbereiten
        targetRect = New Rectangle(0, 0, clntSize.Width, clntSize.Height)
        drawRect = GetDrawRectangle(newImg.Size, targetRect, newPicBoxSM)

        'Neues Bild direkt zeichnen
        Using g As Graphics = renderTarget
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.Clear(Color.Black)
            g.DrawImage(newImg, New Rectangle(0, 0, clntSize.Width, clntSize.Height))
        End Using

        renderTarget.DrawImage(newImg, 0, 0)

        RaiseEvent TransitionIsRunning(False)

    End Sub

    'Optionen & OptionsDialog
    Public Function GetTransitionOptionsDialog() As Windows.Forms.UserControl Implements ISlideShowTransition.GetTransitionOptionsDialog
        Return New ucOptonsTransition
    End Function

End Class
