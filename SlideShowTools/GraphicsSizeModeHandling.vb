Imports System.Drawing
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Public Class GraphicsSizeModeHandling

    Public Shared Function GetDrawRectangle(imageSize As System.Drawing.Size, containerRect As Rectangle, mode As PictureBoxSizeMode) As Rectangle
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

    Public Shared Function ErzeugeGerahmtesBild(bild As BitmapImage,
                                             modus As PictureBoxSizeMode,
                                             zielgröße As System.Drawing.Size) As RenderTargetBitmap
        'Erstellt ein Bitmap mit dem Bild gemäß SizeMode mit schwarzem Rahmen in der Zielgröße

        Dim drawingVisual As New DrawingVisual()

        Using dc As DrawingContext = drawingVisual.RenderOpen()
            ' Bildgröße berechnen anhand SizeMode
            Dim bildgröße As New System.Drawing.Size(bild.PixelWidth, bild.PixelHeight)
            Dim quellRect As New Rectangle(New System.Drawing.Point(0, 0), zielgröße)
            Dim zielRectangle As Rectangle = GraphicsSizeModeHandling.GetDrawRectangle(bildgröße, quellRect, modus)
            Dim zielRect As New Rect(zielRectangle.Left, zielRectangle.Top, zielRectangle.Width, zielRectangle.Height)
            Dim gesamterBereich As New Rect(0.0, 0.0, zielgröße.Width, zielgröße.Height)

            dc.DrawRectangle(Media.Brushes.Black, Nothing, gesamterBereich)
            dc.DrawImage(bild, zielRect)

        End Using

        ' Rendern in RenderTargetBitmap
        Dim bmp As New RenderTargetBitmap(CInt(zielgröße.Width), CInt(zielgröße.Height), 96, 96, PixelFormats.Pbgra32)
        bmp.Render(drawingVisual)

        Return bmp
    End Function

End Class
