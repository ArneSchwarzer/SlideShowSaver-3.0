Imports System.IO
Imports System.Threading
Imports SlideShowLogging

Public NotInheritable Class FileHandling

#Region "Strukturen und Enumerationen"

    Public Enum VerzeichnisSuchmodus
        AlleVerzeichnisse
        NurBlattverzeichnisse
    End Enum

#End Region

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Ermittelt alle Verzeichnisse, die gemäß dem angegebenen Suchmodus
    ''' auf unmittelbar enthaltene Dateien geprüft werden sollen.
    ''' </summary>
    ''' <param name="startVerzeichnisse">
    ''' Die Ausgangsverzeichnisse der Suche.
    ''' </param>
    ''' <param name="suchmodus">
    ''' Legt fest, ob alle Verzeichnisse oder ausschließlich Blattverzeichnisse
    ''' zurückgegeben werden.
    ''' </param>
    ''' <param name="cancellationToken">
    ''' Ermöglicht den kontrollierten Abbruch einer laufenden Suche.
    ''' </param>
    Public Shared Function ErmittleVerzeichnisse(
        startVerzeichnisse As IEnumerable(Of String),
        suchmodus As VerzeichnisSuchmodus,
        Optional cancellationToken As CancellationToken = Nothing
    ) As List(Of String)

        Dim ergebnis As New List(Of String)
        Dim bekannteVerzeichnisse As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim nochZuPruefen As New Stack(Of String)
        Dim startVerzeichnis As String
        Dim aktuellesVerzeichnis As String
        Dim unterverzeichnisse As String()
        Dim normalisierterPfad As String
        Dim verzeichnisIstBlatt As Boolean

        If startVerzeichnisse Is Nothing Then
            Return ergebnis
        End If

        For Each startVerzeichnis In startVerzeichnisse

            If cancellationToken.IsCancellationRequested Then
                Exit For
            End If

            If String.IsNullOrWhiteSpace(startVerzeichnis) Then
                Continue For
            End If

            Try

                normalisierterPfad = Path.GetFullPath(startVerzeichnis.Trim())

            Catch ex As Exception

                LogHandling.LogError("Ungültiger Verzeichnispfad """ & startVerzeichnis & """: " & ex.Message)

                Continue For

            End Try

            If Not Directory.Exists(normalisierterPfad) Then

                LogHandling.LogError("Verzeichnis nicht gefunden: " & normalisierterPfad)

                Continue For

            End If

            If bekannteVerzeichnisse.Add(normalisierterPfad) Then
                nochZuPruefen.Push(normalisierterPfad)
            End If

        Next

        Do While nochZuPruefen.Count > 0

            If cancellationToken.IsCancellationRequested Then
                Exit Do
            End If

            aktuellesVerzeichnis = nochZuPruefen.Pop()
            unterverzeichnisse = Nothing

            Try

                unterverzeichnisse =
                    Directory.GetDirectories(
                        aktuellesVerzeichnis,
                        "*",
                        SearchOption.TopDirectoryOnly)

            Catch ex As UnauthorizedAccessException

                LogHandling.LogWarn(
                    "Keine Zugriffsberechtigung für das Verzeichnis """ &
                    aktuellesVerzeichnis &
                    """: " &
                    ex.Message)

                Continue Do

            Catch ex As IOException

                LogHandling.LogWarn(
                    "Das Verzeichnis """ &
                    aktuellesVerzeichnis &
                    """ konnte nicht gelesen werden: " &
                    ex.Message)

                Continue Do

            Catch ex As Exception

                LogHandling.LogError(
                    "Fehler beim Lesen des Verzeichnisses """ &
                    aktuellesVerzeichnis &
                    """: " &
                    ex.Message)

                Continue Do

            End Try

            verzeichnisIstBlatt = (unterverzeichnisse.Length = 0)

            Select Case suchmodus

                Case VerzeichnisSuchmodus.AlleVerzeichnisse

                    ergebnis.Add(aktuellesVerzeichnis)

                Case VerzeichnisSuchmodus.NurBlattverzeichnisse

                    If verzeichnisIstBlatt Then
                        ergebnis.Add(aktuellesVerzeichnis)
                    End If

            End Select

            For Each unterverzeichnis In unterverzeichnisse

                If cancellationToken.IsCancellationRequested Then
                    Exit For
                End If

                If bekannteVerzeichnisse.Add(unterverzeichnis) Then
                    nochZuPruefen.Push(unterverzeichnis)
                End If

            Next

        Loop

        Return ergebnis

    End Function

    ''' <summary>
    ''' Ermittelt alle unmittelbar in einem Verzeichnis liegenden Dateien,
    ''' deren Dateiendung in der übergebenen Endungsliste enthalten ist.
    ''' Unterverzeichnisse werden nicht durchsucht.
    ''' </summary>
    Public Shared Function ErmittleDateienImVerzeichnis(
        verzeichnis As String,
        erlaubteEndungen As IEnumerable(Of String),
        Optional cancellationToken As CancellationToken = Nothing
    ) As List(Of String)

        Dim ergebnis As New List(Of String)
        Dim normalisierteEndungen As HashSet(Of String)
        Dim dateien As String()
        Dim datei As String
        Dim dateiEndung As String

        If cancellationToken.IsCancellationRequested Then
            Return ergebnis
        End If

        If String.IsNullOrWhiteSpace(verzeichnis) Then
            Return ergebnis
        End If

        If Not Directory.Exists(verzeichnis) Then
            Return ergebnis
        End If

        normalisierteEndungen = NormalisiereDateiendungen(erlaubteEndungen)

        If normalisierteEndungen.Count = 0 Then
            Return ergebnis
        End If

        Try

            dateien =
                Directory.GetFiles(
                    verzeichnis,
                    "*",
                    SearchOption.TopDirectoryOnly)

        Catch ex As UnauthorizedAccessException

            LogHandling.LogWarn(
                "Keine Zugriffsberechtigung für Dateien im Verzeichnis """ &
                verzeichnis &
                """: " &
                ex.Message)

            Return ergebnis

        Catch ex As IOException

            LogHandling.LogWarn(
                "Dateien im Verzeichnis """ &
                verzeichnis &
                """ konnten nicht gelesen werden: " &
                ex.Message)

            Return ergebnis

        Catch ex As Exception

            LogHandling.LogError(
                "Fehler beim Lesen der Dateien im Verzeichnis """ &
                verzeichnis &
                """: " &
                ex.Message)

            Return ergebnis

        End Try

        For Each datei In dateien

            If cancellationToken.IsCancellationRequested Then
                Exit For
            End If

            dateiEndung = Path.GetExtension(datei)

            If normalisierteEndungen.Contains(dateiEndung) Then
                ergebnis.Add(datei)
            End If

        Next

        Return ergebnis

    End Function

    ''' <summary>
    ''' Prüft, ob die Dateiendung des angegebenen Pfades einer der
    ''' übergebenen Endungen entspricht.
    ''' </summary>
    Public Shared Function HatDateiendung(
        dateiPfad As String,
        erlaubteEndungen As IEnumerable(Of String)
    ) As Boolean

        Dim normalisierteEndungen As HashSet(Of String)
        Dim dateiEndung As String

        If String.IsNullOrWhiteSpace(dateiPfad) Then
            Return False
        End If

        normalisierteEndungen = NormalisiereDateiendungen(erlaubteEndungen)

        If normalisierteEndungen.Count = 0 Then
            Return False
        End If

        dateiEndung = Path.GetExtension(dateiPfad)

        Return normalisierteEndungen.Contains(dateiEndung)

    End Function

    ''' <summary>
    ''' Prüft, ob der angegebene Pfad die Dateiendung .jpg oder .jpeg besitzt.
    ''' </summary>
    Public Shared Function IstJpegDatei(dateiPfad As String) As Boolean

        Dim dateiEndung As String

        If String.IsNullOrWhiteSpace(dateiPfad) Then
            Return False
        End If

        dateiEndung = Path.GetExtension(dateiPfad)

        Return String.Equals(
            dateiEndung,
            ".jpg",
            StringComparison.OrdinalIgnoreCase) OrElse
            String.Equals(
                dateiEndung,
                ".jpeg",
                StringComparison.OrdinalIgnoreCase)

    End Function

#End Region

#Region "Private Methoden"

    Private Shared Function NormalisiereDateiendungen(
        dateiendungen As IEnumerable(Of String)
    ) As HashSet(Of String)

        Dim ergebnis As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim dateiendung As String
        Dim normalisierteEndung As String

        If dateiendungen Is Nothing Then
            Return ergebnis
        End If

        For Each dateiendung In dateiendungen

            If String.IsNullOrWhiteSpace(dateiendung) Then
                Continue For
            End If

            normalisierteEndung = dateiendung.Trim()

            If Not normalisierteEndung.StartsWith(".", StringComparison.Ordinal) Then
                normalisierteEndung = "." & normalisierteEndung
            End If

            ergebnis.Add(normalisierteEndung)

        Next

        Return ergebnis

    End Function

#End Region

End Class