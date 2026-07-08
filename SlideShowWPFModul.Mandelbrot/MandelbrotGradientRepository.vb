Imports System.Globalization
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Xml.Linq

Public Class MandelbrotGradientRepository

    Public Shared Function LadeGradienten(xmlPfad As String) As List(Of MandelbrotGradient)

        Dim result As New List(Of MandelbrotGradient)

        If Not IO.File.Exists(xmlPfad) Then Return result

        Dim doc As XDocument = XDocument.Load(xmlPfad)

        For Each gradientNode As XElement In doc.Root.Elements("Gradient")

            Dim gradient As New MandelbrotGradient With {
                .Name = CStr(gradientNode.Attribute("Name"))
            }

            For Each stopNode As XElement In gradientNode.Elements("Stop")

                gradient.Stops.Add(New MandelbrotGradientStop With {
                    .Position = Double.Parse(CStr(stopNode.Attribute("Position")), CultureInfo.InvariantCulture),
                    .Farbe = CType(ColorConverter.ConvertFromString(CStr(stopNode.Attribute("Color"))), Color)
                })

            Next

            gradient.Stops = gradient.Stops.OrderBy(Function(s) s.Position).ToList()
            result.Add(gradient)

        Next

        Return result

    End Function

    Public Shared Function ErzeugeGradientBrush(gradient As MandelbrotGradient,
                                                Optional breite As Integer = 1024) As ImageBrush

        If gradient Is Nothing OrElse gradient.Stops.Count = 0 Then
            Throw New ArgumentException("Gradient ist leer.")
        End If

        Dim wb As New WriteableBitmap(breite, 1, 96, 96, PixelFormats.Bgra32, Nothing)
        Dim pixels(breite - 1) As Integer

        For x As Integer = 0 To breite - 1
            Dim t As Double = x / CDbl(breite - 1)
            Dim c As Color = SampleGradient(gradient, t)

            pixels(x) =
                (CInt(c.A) << 24) Or
                (CInt(c.R) << 16) Or
                (CInt(c.G) << 8) Or
                CInt(c.B)
        Next

        wb.WritePixels(New Int32Rect(0, 0, breite, 1), pixels, breite * 4, 0)
        wb.Freeze()

        Dim brush As New ImageBrush(wb) With {
            .Stretch = Stretch.Fill,
            .TileMode = TileMode.None
        }

        brush.Freeze()
        Return brush

    End Function

    Private Shared Function SampleGradient(gradient As MandelbrotGradient, t As Double) As Color

        t = Math.Max(0.0, Math.Min(1.0, t))

        Dim stops = gradient.Stops

        If t <= stops.First().Position Then Return stops.First().Farbe
        If t >= stops.Last().Position Then Return stops.Last().Farbe

        For i As Integer = 0 To stops.Count - 2

            Dim a = stops(i)
            Dim b = stops(i + 1)

            If t >= a.Position AndAlso t <= b.Position Then

                Dim localT As Double = (t - a.Position) / (b.Position - a.Position)

                Return Color.FromArgb(
                    CByte(Lerp(a.Farbe.A, b.Farbe.A, localT)),
                    CByte(Lerp(a.Farbe.R, b.Farbe.R, localT)),
                    CByte(Lerp(a.Farbe.G, b.Farbe.G, localT)),
                    CByte(Lerp(a.Farbe.B, b.Farbe.B, localT))
                )

            End If

        Next

        Return stops.Last().Farbe

    End Function

    Private Shared Function Lerp(a As Double, b As Double, t As Double) As Double
        Return a + (b - a) * t
    End Function

End Class