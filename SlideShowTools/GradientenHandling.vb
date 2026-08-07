Imports System.Globalization
Imports System.IO
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Xml.Linq

Public NotInheritable Class GradientenHandling

#Region "Konstanten"

    Private Const gradientenDateiname As String = "SlideShowGradienten.xml"

#End Region

#Region "Konstruktor"

    Private Sub New()
        'Statische Hilfsklasse
    End Sub

#End Region

#Region "Pfad- und Dateiverwaltung"

    Public Shared Function GetGradientenPfad() As String
        'Liefert den zentralen Pfad der Gradientenbibliothek.

        Dim basisPfad As String

        basisPfad = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        Return Path.Combine(basisPfad, "SlideShowSaver 3.0", gradientenDateiname)

    End Function

#End Region

#Region "XML laden"

    Public Shared Function LadeGradienten(Optional xmlPfad As String = Nothing) As List(Of SlideShowGradient)
        'Lädt eine Gradientenbibliothek aus einer XML-Datei.

        Dim result As New List(Of SlideShowGradient)
        Dim doc As XDocument
        Dim gradient As SlideShowGradient
        Dim gradientName As String

        If String.IsNullOrWhiteSpace(xmlPfad) Then

            xmlPfad = GetGradientenPfad()

        End If

        If Not File.Exists(xmlPfad) Then

            Return result

        End If

        doc = XDocument.Load(xmlPfad)

        If doc.Root Is Nothing Then
            Return result
        End If

        For Each gradientNode As XElement In
            doc.Root.Elements("Gradient")

            gradientName = CStr(gradientNode.Attribute("Name"))

            If String.IsNullOrWhiteSpace(gradientName) Then

                Continue For

            End If

            gradient = New SlideShowGradient() With {.Name = gradientName.Trim()}

            LadeGradientStops(gradientNode, gradient)

            If gradient.Stops.Count > 0 Then

                gradient.Stops =
                    gradient.Stops.
                    OrderBy(
                        Function(stopElement)
                            Return stopElement.Position
                        End Function).
                    ToList()

                result.Add(gradient)

            End If

        Next

        Return result

    End Function

    Private Shared Sub LadeGradientStops(gradientNode As XElement, gradient As SlideShowGradient)
        'Lädt und validiert die Stops eines Gradienten.

        Dim position As Double
        Dim farbe As Color
        Dim positionText As String
        Dim farbeText As String

        For Each stopNode As XElement In gradientNode.Elements("Stop")

            positionText = CStr(stopNode.Attribute("Position"))

            farbeText = CStr(stopNode.Attribute("Color"))

            If Not Double.TryParse(positionText, NumberStyles.Float, CultureInfo.InvariantCulture, position) Then

                Continue For

            End If

            If position < 0.0 OrElse position > 1.0 Then

                Continue For

            End If

            If Not TryParseColor(farbeText, farbe) Then

                Continue For

            End If

            gradient.Stops.Add(New SlideShowGradientStop() With {.Position = position, .Farbe = farbe})

        Next

    End Sub

    Private Shared Function TryParseColor(farbeText As String, ByRef farbe As Color) As Boolean
        'Konvertiert einen XML-Farbwert in eine WPF-Color.

        Dim konvertierterWert As Object

        If String.IsNullOrWhiteSpace(farbeText) Then

            Return False

        End If

        Try

            konvertierterWert = ColorConverter.ConvertFromString(farbeText)

            If konvertierterWert Is Nothing Then
                Return False
            End If

            farbe = CType(konvertierterWert, Color)

            Return True

        Catch ex As FormatException

            Return False

        Catch ex As NotSupportedException

            Return False

        End Try

    End Function

#End Region

#Region "WPF-Gradiententextur"

    Public Shared Function ErzeugeGradientBrush(gradient As SlideShowGradient, Optional breite As Integer = 1024) As ImageBrush
        'Erzeugt eine horizontale WPF-Textur aus einem Gradient.

        Dim wb As WriteableBitmap
        Dim pixels() As Integer
        Dim t As Double
        Dim farbe As Color
        Dim brush As ImageBrush

        If gradient Is Nothing Then

            Throw New ArgumentNullException(NameOf(gradient))

        End If

        If gradient.Stops Is Nothing OrElse gradient.Stops.Count = 0 Then

            Throw New ArgumentException("Der Gradient besitzt keine Farbstopps.", NameOf(gradient))

        End If

        If breite < 2 Then

            Throw New ArgumentOutOfRangeException(NameOf(breite), "Die Gradiententextur muss mindestens zwei Pixel breit sein.")

        End If

        wb =
            New WriteableBitmap(
                breite,
                1,
                96,
                96,
                PixelFormats.Bgra32,
                Nothing)

        ReDim pixels(breite - 1)

        For x As Integer = 0 To breite - 1

            t = x / CDbl(breite - 1)

            farbe = SampleGradient(gradient, t)

            pixels(x) =
                (CInt(farbe.A) << 24) Or
                (CInt(farbe.R) << 16) Or
                (CInt(farbe.G) << 8) Or
                CInt(farbe.B)

        Next

        wb.WritePixels(
            New Int32Rect(
                0,
                0,
                breite,
                1),
            pixels,
            breite * 4,
            0)

        wb.Freeze()

        brush =
            New ImageBrush(
                wb) With {
                .Stretch = Stretch.Fill,
                .TileMode = TileMode.None
            }

        brush.Freeze()

        Return brush

    End Function

    Private Shared Function SampleGradient(gradient As SlideShowGradient, position As Double) As Color
        'Ermittelt eine interpolierte Farbe des Gradienten.

        Dim stopA As SlideShowGradientStop
        Dim stopB As SlideShowGradientStop
        Dim abstand As Double
        Dim lokalePosition As Double

        position = Math.Max(0.0, Math.Min(1.0, position))

        If position <= gradient.Stops.First().Position Then

            Return gradient.Stops.First().Farbe

        End If

        If position >= gradient.Stops.Last().Position Then

            Return gradient.Stops.Last().Farbe

        End If

        For index As Integer = 0 To gradient.Stops.Count - 2

            stopA = gradient.Stops(index)
            stopB = gradient.Stops(index + 1)

            If position < stopA.Position OrElse position > stopB.Position Then

                Continue For

            End If

            abstand = stopB.Position - stopA.Position

            If abstand <= 0.0 Then

                Return stopB.Farbe

            End If

            lokalePosition = (position - stopA.Position) / abstand

            Return Color.FromArgb(
                CByte(Lerp(stopA.Farbe.A, stopB.Farbe.A, lokalePosition)),
                CByte(Lerp(stopA.Farbe.R, stopB.Farbe.R, lokalePosition)),
                CByte(Lerp(stopA.Farbe.G, stopB.Farbe.G, lokalePosition)),
                CByte(Lerp(stopA.Farbe.B, stopB.Farbe.B, lokalePosition)))

        Next

        Return gradient.Stops.Last().Farbe

    End Function

    Private Shared Function Lerp(startwert As Double, endwert As Double, position As Double) As Double

        Return startwert + (endwert - startwert) * position

    End Function

#End Region

End Class