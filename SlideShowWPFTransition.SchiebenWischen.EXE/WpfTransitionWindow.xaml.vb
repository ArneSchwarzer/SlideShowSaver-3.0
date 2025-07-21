Imports System.Windows.Media.Animation
Imports System.Windows.Media.Imaging
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Interop

Namespace SlideShowWPFTransition.SchiebenWischen

    Partial Public Class WpfTransitionWindow
        Inherits Window

        Public Sub StartTransition(oldBmp As Bitmap, newBmp As Bitmap, durationMs As Integer)
            ' Bilder einsetzen
            imgOld.Source = ConvertBitmapToImageSource(oldBmp)
            imgNew.Source = ConvertBitmapToImageSource(newBmp)

            ' Positionen
            Canvas.SetLeft(imgOld, 0)
            Canvas.SetTop(imgOld, 0)
            Canvas.SetLeft(imgNew, Me.Width)
            Canvas.SetTop(imgNew, 0)

            ' Animation für neues Bild
            Dim anim As New DoubleAnimation()
            anim.From = Me.Width
            anim.To = 0
            anim.Duration = TimeSpan.FromMilliseconds(durationMs)
            AddHandler anim.Completed, Sub() Me.Close()

            imgNew.BeginAnimation(Canvas.LeftProperty, anim)
        End Sub

        Private Function ConvertBitmapToImageSource(bmp As Bitmap) As BitmapSource
            Using memory = New MemoryStream()
                bmp.Save(memory, ImageFormat.Png)
                memory.Position = 0
                Dim bitmapImage As New BitmapImage()
                bitmapImage.BeginInit()
                bitmapImage.StreamSource = memory
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad
                bitmapImage.EndInit()
                Return bitmapImage
            End Using
        End Function
    End Class

End Namespace
