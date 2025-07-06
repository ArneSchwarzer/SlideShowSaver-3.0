Imports System.Drawing
Imports System.Windows.Forms

Public Class GraphicsSizeModeHandling

    Public Shared Function GetDrawRectangle(imageSize As Size, containerRect As Rectangle, mode As PictureBoxSizeMode) As Rectangle
        Select Case mode
            Case PictureBoxSizeMode.Normal
                Return New Rectangle(containerRect.X, containerRect.Y, imageSize.Width, imageSize.Height)

            Case PictureBoxSizeMode.CenterImage
                Return New Rectangle(
                    containerRect.X + (containerRect.Width - imageSize.Width) \ 2,
                    containerRect.Y + (containerRect.Height - imageSize.Height) \ 2,
                    imageSize.Width,
                    imageSize.Height)

            Case PictureBoxSizeMode.StretchImage
                Return containerRect

            Case PictureBoxSizeMode.Zoom
                Dim ratioX As Double = containerRect.Width / imageSize.Width
                Dim ratioY As Double = containerRect.Height / imageSize.Height
                Dim ratio As Double = Math.Min(ratioX, ratioY)

                Dim newWidth As Integer = CInt(imageSize.Width * ratio)
                Dim newHeight As Integer = CInt(imageSize.Height * ratio)

                Dim posX As Integer = containerRect.X + (containerRect.Width - newWidth) \ 2
                Dim posY As Integer = containerRect.Y + (containerRect.Height - newHeight) \ 2

                Return New Rectangle(posX, posY, newWidth, newHeight)

            Case PictureBoxSizeMode.AutoSize
                Return New Rectangle(containerRect.X, containerRect.Y, imageSize.Width, imageSize.Height)

            Case Else
                Return containerRect
        End Select
    End Function

End Class
