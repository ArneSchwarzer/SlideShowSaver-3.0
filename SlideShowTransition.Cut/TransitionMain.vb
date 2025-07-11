Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Drawing.Drawing2D
Imports SlideShowTools.GraphicsSizeModeHandling
Imports System.Runtime.Remoting.Messaging

Public Class TransitionMain
    Implements ISlideShowTransition

    'Variablendeklaration
    Private newImage As Image
    Private newPicBoxSizeMode As PictureBoxSizeMode
    Private targetGrapics As Graphics
    Private clientSize As Size

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
    Public Event PleaseChangeToShader As ISlideShowTransition.PleaseChangeToShaderEventHandler Implements ISlideShowTransition.PleaseChangeToShader

    Public Sub RunTransition(oldImage As System.Drawing.Image, picBoxModeOld As Windows.Forms.PictureBoxSizeMode, newImage As System.Drawing.Image, picBoxModeNew As Windows.Forms.PictureBoxSizeMode, targetGraphics As System.Drawing.Graphics, Optional clientSize As System.Drawing.Size = Nothing, Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition
        'Formal noch einmal TransitionIsRunning werfen.
        RaiseEvent TransitionIsRunning("Running")

        'Parameter in interne Variablen überführen
        Me.newImage = newImage
        Me.newPicBoxSizeMode = picBoxModeNew
        Me.targetGrapics = targetGraphics
        Me.clientSize = clientSize

        'Gleich zum Ende der Transition.
        StopTransition()

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Zeichnet newImage auf den Zeichenbereich und gut
        Dim drawRect As Rectangle
        Dim targetRect As Rectangle
        Dim bmp As New Bitmap(clientSize.Width, clientSize.Height)

        'Grafik vorbereiten
        targetRect = New Rectangle(0, 0, clientSize.Width, clientSize.Height)
        drawRect = GetDrawRectangle(newImage.Size, targetRect, newPicBoxSizeMode)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.Clear(Color.Black)

            ' Endbild gnadenlos auf den Zeichenbereich malen...
            g.DrawImage(newImage, drawRect)
        End Using

        RaiseEvent TransitionIsRunning("Stopped")

    End Sub

    Public Sub ApplyTransitionSettings(settings As Object) Implements ISlideShowTransition.ApplyTransitionSettings
        Throw New NotImplementedException()
    End Sub

    Public Sub GetTransitionSettings(uc As Windows.Forms.UserControl, restoreSettings As Object) Implements ISlideShowTransition.GetTransitionSettings
        Throw New NotImplementedException()
    End Sub

    Public Sub GetTransitionRegistryOrDefaultSettings(uc As Windows.Forms.UserControl) Implements ISlideShowTransition.GetTransitionRegistryOrDefaultSettings
        Throw New NotImplementedException()
    End Sub

    Public Sub AttentionShaderGewechselt(shaderName As String) Implements ISlideShowTransition.AttentionShaderGewechselt
        Throw New NotImplementedException()
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowTransition.CheckYourSettings
        Throw New NotImplementedException()
    End Sub

    Public Function GetTransitionOptionsDialog() As Windows.Forms.UserControl Implements ISlideShowTransition.GetTransitionOptionsDialog
        Return New ucOptonsTransition
    End Function

    Public Function MemorizeTransitionSettings(uc As Windows.Forms.UserControl) As Object Implements ISlideShowTransition.MemorizeTransitionSettings
        Throw New NotImplementedException()
    End Function

End Class
