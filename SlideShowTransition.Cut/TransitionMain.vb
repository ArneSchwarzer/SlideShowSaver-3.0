Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Drawing.Drawing2D
Imports SlideShowTools.GraphicsSizeModeHandling
Imports System.Runtime.Remoting.Messaging
Imports System.Windows.Media.Imaging
Imports System.Windows.Controls
Imports SlideShowTools.WPFHandling
Imports SlideShowTools


Public Class TransitionMain
    Implements ISlideShowTransition

    'Variablendeklaration
    Private newImg As BitmapImage
    Private newRTImg As RenderTargetBitmap
    Private newPicBoxSM As PictureBoxSizeMode
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
    Public Event FrameIstFertig As ISlideShowTransition.FrameIstFertigEventHandler Implements ISlideShowTransition.FrameIstFertig

    Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode,
                         newImage As BitmapImage, picBoxModeNew As PictureBoxSizeMode,
                         clientSize As Size, Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition

        Dim sizeWPF As New Windows.Size(clientSize.Width, clientSize.Height)

        'Lohnt eigentlich gar nicht...
        RaiseEvent TransitionIsRunning(True)

        newRTImg = ConvertBitmapImageToRenderTargetBitmap(newImage, sizeWPF)

        'Das Bild direkt als fertig zurückgeben
        RaiseEvent FrameIstFertig(newRTImg)
        RaiseEvent TransitionIsRunning(False)

    End Sub


    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Nichts zu tun, Frame wurde bereits geliefert

        RaiseEvent TransitionIsRunning(False)

    End Sub

    'Optionen & OptionsDialog
    Public Function GetTransitionOptionsDialog() As Windows.Forms.UserControl Implements ISlideShowTransition.GetTransitionOptionsDialog
        Return New ucOptonsTransition
    End Function

End Class
