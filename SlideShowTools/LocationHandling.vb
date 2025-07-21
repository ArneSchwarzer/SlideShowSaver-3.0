' LocationHandling.vb
' Klasse zur Verarbeitung geographischer Informationen basierend auf XML-Datenbank
' Unterstützt Erkennung von Orten in Tags, Rückgabe von GPS-Koordinaten, Vervollständigung etc.

Imports System.IO
Imports System.Xml.Linq
Imports SlideShowLogging.LogHandling

Public Class LocationHandling
    Private Const xmlPfad As String = "Orte\GeoNames_StadtListe_20000.xml"
    Private xmlDaten As XDocument

    Public Sub New()
        Try
            If IO.File.Exists(xmlPfad) Then
                xmlDaten = XDocument.Load(xmlPfad)
            Else
                LogError("SlideShowTools - LocationHandling.Sub New(): Fehler beim Einlesen der GeoNames_StadtListe_20000.xml")
            End If
        Catch ex As Exception
            LogError("SlideShowTools - LocationHandling.Sub New(): Fehler beim Einlesen der GeoNames_StadtListe_20000.xml" & ex.ToString)
        End Try

    End Sub

    Public Function CreateLocationString(tags As List(Of String)) As List(Of String)
        Dim result As New List(Of String)

        For Each tag In tags
            Dim treffer = (From ort In xmlDaten.<Orte>.<Ort>
                           Where String.Equals(ort.<Stadt>.Value.Trim(), tag.Trim(), StringComparison.OrdinalIgnoreCase) _
               Or String.Equals(ort.<Landname>.Value.Trim(), tag.Trim(), StringComparison.OrdinalIgnoreCase)
                           Select ort).FirstOrDefault()

            If treffer IsNot Nothing Then
                If Not result.Contains(treffer.<Stadt>.Value) Then result.Add(treffer.<Stadt>.Value)
                If Not result.Contains(treffer.<Landname>.Value) Then result.Add(treffer.<Landname>.Value)
                If Not result.Contains(treffer.<Kontinent>.Value) Then result.Add(treffer.<Kontinent>.Value)
            End If
        Next

        Return result
    End Function

    Public Function DetermineGPSLocation(tags As List(Of String)) As String
        For Each tag In tags
            Dim treffer = (From ort In xmlDaten.<Orte>.<Ort>
                           Where String.Equals(ort.<Stadt>.Value.Trim(), tag.Trim(), StringComparison.OrdinalIgnoreCase)
                           Select ort).FirstOrDefault()
            If treffer IsNot Nothing Then
                Return $"{treffer.<Latitude>.Value}, {treffer.<Longitude>.Value}"
            Else
                ' LogDebug("SlideShowTools - LocationHandling.DetermineGPSLocation(): Tag '" & tag & "' konnte nicht in der Ortsdatenbank gefunden werden")
            End If

        Next
        Return ""
    End Function

    ''' <summary>
    ''' Gibt den Kontinent für ein übergebenes Land zurück (erster Treffer).
    ''' </summary>
    ''' <param name="country">Landname</param>
    ''' <returns>Kontinentname oder leerer String</returns>
    Public Function GetContinent(country As String) As String
        Dim treffer = (From ort In xmlDaten.<Orte>.<Ort>
                       Where String.Equals(ort.<Landname>.Value.Trim(), country.Trim(), StringComparison.OrdinalIgnoreCase)
                       Select ort.<Kontinent>.Value).FirstOrDefault()
        Return If(treffer, "")
    End Function

    ''' <summary>
    ''' Gibt das Land zu einer gegebenen Stadt zurück (erster Treffer).
    ''' </summary>
    ''' <param name="city">Stadtname</param>
    ''' <returns>Landname oder leerer String</returns>
    Public Function GetCountry(city As String) As String
        Dim treffer = (From ort In xmlDaten.<Orte>.<Ort>
                       Where String.Equals(ort.<Stadt>.Value.Trim(), city.Trim(), StringComparison.OrdinalIgnoreCase)
                       Select ort.<Landname>.Value).FirstOrDefault()
        Return If(treffer, "")
    End Function

    ''' <summary>
    ''' Führt zwei Ortsdatenbanken (userXML und defaultXML) zusammen, ohne Duplikate.
    ''' </summary>
    ''' <param name="userXmlPath">Pfad zur benutzerdefinierten XML-Datei</param>
    ''' <param name="defaultXmlPath">Pfad zur Standard-XML-Datei</param>
    ''' <returns>Anzahl der neu hinzugefügten Orte</returns>
    Public Function MergeLocations(userXmlPath As String, defaultXmlPath As String) As Integer
        Dim userDoc As XDocument = XDocument.Load(userXmlPath)
        Dim defaultDoc As XDocument = XDocument.Load(defaultXmlPath)
        Dim neueOrte As Integer = 0

        Dim userOrte = userDoc.<Orte>.<Ort>.Select(Function(o) o.<Stadt>.Value & "|" & o.<Landname>.Value).ToHashSet()

        For Each ort In defaultDoc.<Orte>.<Ort>
            Dim schluessel = ort.<Stadt>.Value & "|" & ort.<Landname>.Value
            If Not userOrte.Contains(schluessel) Then
                userDoc.<Orte>.First().Add(ort)
                userOrte.Add(schluessel)
                neueOrte += 1
            End If
        Next

        userDoc.Save(userXmlPath)
        Return neueOrte
    End Function


End Class
