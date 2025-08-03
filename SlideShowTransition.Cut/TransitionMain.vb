Imports System.Drawing
Imports System.Windows.Forms
Imports System.Windows.Media.Imaging
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.ImageConversionHandling


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
    Public Event TransitionFrameIstFertig As ISlideShowTransition.TransitionFrameIstFertigEventHandler Implements ISlideShowTransition.TransitionFrameIstFertig

    Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode,
                         newImage As BitmapImage, picBoxModeNew As PictureBoxSizeMode,
                         clientSize As Size, Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition

        Dim sizeWPF As New Windows.Size(clientSize.Width, clientSize.Height)

        'Lohnt eigentlich gar nicht...
        RaiseEvent TransitionIsRunning(True)

        newRTImg = ConvertBitmapImageToRenderTargetBitmap(newImage, sizeWPF)

        'Dem Modul 1/2 Sekunde Zeit zum aufholen geben und dann das Bild direkt als fertig zurückgeben
        System.Threading.Thread.Sleep(500)

        RaiseEvent TransitionFrameIstFertig(newRTImg)
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
