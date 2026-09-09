Imports System.Globalization
Imports System.IO
Imports System.Xml.Linq
Imports SlideShowLogging

Public Class MandelbrotZielRepository

#Region "Variablendeklaration"
    'Variablendeklaration

    Private ReadOnly xmlPfad As String
    Private ReadOnly geladeneZiele As List(Of MandelbrotZiel)

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property Ziele As IReadOnlyList(Of MandelbrotZiel)
        Get
            Return geladeneZiele.AsReadOnly()
        End Get
    End Property

#End Region

#Region "Konstruktor"

    Public Sub New(dateiPfad As String)

        xmlPfad = dateiPfad
        geladeneZiele = New List(Of MandelbrotZiel)()

    End Sub

#End Region

#Region "Laden"

    Public Function LadeZiele() As Boolean
        'Lädt und validiert sämtliche Mandelbrot-Ziele einmalig.

        Dim dokument As XDocument
        Dim ziel As MandelbrotZiel

        geladeneZiele.Clear()

        If String.IsNullOrWhiteSpace(xmlPfad) Then

            LogHandling.LogWarn("Modul Mandelbrot - MandelbrotZielRepository.LadeZiele(): " &
                                "Es wurde kein Pfad zur Zieldatei angegeben.")

            Return False

        End If

        If Not File.Exists(xmlPfad) Then

            LogHandling.LogWarn("Modul Mandelbrot - MandelbrotZielRepository.LadeZiele(): " &
                                "Die Zieldatei wurde nicht gefunden: " & xmlPfad)

            Return False

        End If

        Try

            dokument = XDocument.Load(xmlPfad)

            If dokument.Root Is Nothing Then

                LogHandling.LogWarn("Modul Mandelbrot - MandelbrotZielRepository.LadeZiele(): " &
                                    "Die Zieldatei besitzt kein Wurzelelement.")

                Return False

            End If

            For Each zielNode As XElement In
                dokument.Root.Elements("Target")

                ziel = ErzeugeZielAusXml(zielNode)

                If ziel IsNot Nothing Then

                    geladeneZiele.Add(ziel)

                End If

            Next

        Catch ex As Exception

            LogHandling.LogError("Modul Mandelbrot - MandelbrotZielRepository.LadeZiele(): " &
                                 "Die Mandelbrot-Ziele konnten nicht geladen werden: " & ex.ToString())

            geladeneZiele.Clear()

            Return False

        End Try

        If geladeneZiele.Count = 0 Then

            LogHandling.LogWarn("Modul Mandelbrot - MandelbrotZielRepository.LadeZiele(): " &
                                "Die Zieldatei enthält keine gültigen Mandelbrot-Ziele.")

            Return False

        End If

        LogHandling.LogInfo("Modul Mandelbrot: " & geladeneZiele.Count.ToString() &
                            " Mandelbrot-Ziele wurden geladen.")

        Return True

    End Function

    Private Function ErzeugeZielAusXml(zielNode As XElement) As MandelbrotZiel
        'Erzeugt ein validiertes Mandelbrot-Ziel aus einem XML-Element.

        Dim zielName As String
        Dim centerX As Double
        Dim centerY As Double
        Dim targetScale As Double

        zielName = CStr(zielNode.Attribute("Name"))

        If String.IsNullOrWhiteSpace(zielName) Then

            LogHandling.LogWarn("Modul Mandelbrot - MandelbrotZielRepository: Ein Ziel ohne Namen wurde übersprungen.")

            Return Nothing

        End If

        If Not TryLeseDouble(zielNode.Element("CenterX"), centerX) Then

            LogUngueltigesZiel(zielName, "CenterX")

            Return Nothing

        End If

        If Not TryLeseDouble(zielNode.Element("CenterY"), centerY) Then

            LogUngueltigesZiel(zielName, "CenterY")

            Return Nothing

        End If

        If Not TryLeseDouble(zielNode.Element("TargetScale"), targetScale) Then

            LogUngueltigesZiel(zielName, "TargetScale")

            Return Nothing

        End If

        If targetScale <= 0.0 Then

            LogHandling.LogWarn("Modul Mandelbrot - MandelbrotZielRepository: Ziel """ & zielName &
                                """ besitzt eine ungültige TargetScale: " & targetScale.ToString("G17",
                                CultureInfo.InvariantCulture))

            Return Nothing

        End If

        Return New MandelbrotZiel() With {
            .Name = zielName.Trim(),
            .CenterX = centerX,
            .CenterY = centerY,
            .TargetScale = targetScale
        }

    End Function

    Private Function TryLeseDouble(element As XElement, ByRef wert As Double) As Boolean
        'Liest einen Double-Wert kulturunabhängig aus einem XML-Element.

        If element Is Nothing Then
            Return False
        End If

        Return Double.TryParse(element.Value, NumberStyles.Float, CultureInfo.InvariantCulture, wert)

    End Function

    Private Sub LogUngueltigesZiel(zielName As String, elementName As String)
        'Protokolliert ein fehlendes oder ungültiges Zielelement.

        LogHandling.LogWarn("Modul Mandelbrot - MandelbrotZielRepository: Ziel """ & zielName &
                            """ besitzt keinen gültigen Wert für " & elementName & " und wurde übersprungen.")

    End Sub

#End Region

End Class