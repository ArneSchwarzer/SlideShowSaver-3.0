Imports System.IO
Imports System.Xml
Imports System.Xml.Linq
Imports SlideShowLogging.LogHandling

Public Class XmlHandling

    ''' <summary>
    ''' Lädt alle Werte eines bestimmten Attributs aus einer XML-Datei.
    ''' </summary>
    Public Shared Function LadeWerteliste(pfad As String, elementName As String, attributName As String) As List(Of String)
        Dim liste As New List(Of String)

        Try
            If Not File.Exists(pfad) Then Return liste

            Dim doc As XDocument = XDocument.Load(pfad)

            For Each eintrag In doc.Descendants(elementName)
                Dim attr As XAttribute = eintrag.Attribute(attributName)
                If attr IsNot Nothing Then liste.Add(attr.Value)
            Next

        Catch ex As Exception
            LogWarn("Fehler beim Laden der Werteliste aus XML: " & ex.Message)
        End Try

        Return liste
    End Function

    ''' <summary>
    ''' Speichert eine Liste von Strings als XML-Datei mit gegebenem Element- und Attributnamen.
    ''' </summary>
    Public Shared Sub SpeichereWerteliste(pfad As String, werte As IEnumerable(Of String), hauptElement As String, elementName As String, attributName As String)
        Try
            Dim doc As New XDocument(New XElement(hauptElement))

            For Each wert In werte
                doc.Root.Add(New XElement(elementName, New XAttribute(attributName, wert)))
            Next

            ' Sicherstellen, dass das Zielverzeichnis existiert
            Dim verzeichnis = Path.GetDirectoryName(pfad)
            If Not Directory.Exists(verzeichnis) Then
                Directory.CreateDirectory(verzeichnis)
            End If

            doc.Save(pfad)

        Catch ex As Exception
            LogWarn("Fehler beim Speichern der Werteliste als XML: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Prüft, ob ein bestimmter Wert in der XML-Werteliste bereits existiert.
    ''' </summary>
    Public Shared Function WertelisteEnthält(pfad As String, elementName As String, attributName As String, wertInhalt As String) As Boolean
            Try
                Dim liste = LadeWerteliste(pfad, elementName, attributName)
                Return liste.Contains(wertInhalt)
            Catch ex As Exception
                LogWarn("Fehler bei Prüfung auf vorhandenen Wert in XML: " & ex.Message)
                Return False
            End Try
        End Function

    ''' <summary>
    ''' Fügt einen Wert zur XML-Werteliste hinzu oder entfernt ihn, je nach Parameter.
    ''' </summary>
    Public Shared Sub WertInXmlEintragenOderEntfernen(pfad As String, hauptElement As String, elementName As String, attributName As String, wertInhalt As String, hinzufuegen As Boolean)
        Try
            Dim aktuelleListe = LadeWerteliste(pfad, elementName, attributName).ToList()

            If hinzufuegen Then
                If Not aktuelleListe.Contains(wertInhalt) Then aktuelleListe.Add(wertInhalt)
            Else
                aktuelleListe.Remove(wertInhalt)
            End If

            SpeichereWerteliste(pfad, aktuelleListe, hauptElement, elementName, attributName)

        Catch ex As Exception
            LogWarn("Fehler beim Hinzufügen/Entfernen von Wert in XML: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Zählt die Datensätze eines bestimmten Elementtyps in einer XML-Datei.
    ''' </summary>
    Public Shared Function XMLDatensaetzeCount(pfad As String, datensatzElementName As String) As Integer
        Try
            If Not File.Exists(pfad) Then
                LogInfo("MandelbrotZiele.xml unter " & pfad & " nicht gefunden.")
                Return 0
            End If

            Dim doc As XDocument = XDocument.Load(pfad)
            Return doc.Descendants(datensatzElementName).Count()

        Catch ex As Exception
            LogWarn("Fehler beim Zählen der XML-Datensätze: " & ex.Message)
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Liefert den XML-Datensatz mit dem angegebenen Index.
    ''' Index ist 0-basiert.
    ''' </summary>
    Public Shared Function XMLDatensatzPerIndex(pfad As String, datensatzElementName As String, index As Integer) As XElement
        Try
            If Not File.Exists(pfad) Then Return Nothing
            If index < 0 Then Return Nothing

            Dim doc As XDocument = XDocument.Load(pfad)

            Return doc.Descendants(datensatzElementName).
                       Skip(index).
                       FirstOrDefault()

        Catch ex As Exception
            LogWarn("Fehler beim Laden eines XML-Datensatzes per Index: " & ex.Message)
            Return Nothing
        End Try
    End Function

End Class


