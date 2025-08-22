Imports System.IO
Imports SlideShowLogging
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowLoader

Public Module LUTByNameLoader

    ' *** Variablen am Anfang ***
    Private ReadOnly _docRoot As String =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                     "SlideShowSaver 3.0\Shaders\LUTShader")

    Private ReadOnly _instRoots As String() = {
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders\LUTShader"),
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shader\LUTShader")
    }

    ''' <summary>
    ''' Holt eine LUT per Anzeigename (Dateiname) – bevorzugt aus dem Benutzerordner.
    ''' Fällt zurück auf Installationsordner. Falls <paramref name="lutDisplayNameOrPath"/> ein existierender Pfad ist,
    ''' wird genau diese Datei gesnifft und zurückgegeben.
    ''' </summary>
    Public Function LUTByNameLoader(lutDisplayNameOrPath As String) As LutInfo
        If String.IsNullOrWhiteSpace(lutDisplayNameOrPath) Then
            Return NotFound("Kein Name angegeben.")
        End If

        ' 1) Direkter Pfad?
        If File.Exists(lutDisplayNameOrPath) Then
            Try
                Return LUTSniffer.SniffFile(lutDisplayNameOrPath)
            Catch ex As Exception
                LogHandling.LogError($"LUTByNameLoader: Sniffen per Pfad fehlgeschlagen: {ex.Message}")
                Return NotFound("Sniffen per Pfad fehlgeschlagen.")
            End Try
        End If

        Dim nameOnly As String = Path.GetFileName(lutDisplayNameOrPath)
        Dim nameNoExt As String = Path.GetFileNameWithoutExtension(lutDisplayNameOrPath)

        ' 2) Liste laden und nach Name suchen (Benutzerordner hat Vorrang)
        Dim all As List(Of LutInfo) = LUTListLoader.LUTListLoader()

        ' harte Priorisierung: erst exakter Filename (mit Ext), UserRoot vor InstallRoot
        Dim candidates = all.Where(Function(li) li.LUTName.Equals(nameOnly, StringComparison.CurrentCultureIgnoreCase)).ToList()
        Dim chosen As LutInfo
        If candidates.Count > 0 Then
            chosen = PreferUserFolder(candidates)
            Return chosen
        End If

        ' ansonsten: Vergleich ohne Extension
        candidates = all.Where(Function(li) Path.GetFileNameWithoutExtension(li.LUTName).Equals(nameNoExt, StringComparison.CurrentCultureIgnoreCase)).ToList()
        If candidates.Count > 0 Then
            chosen = PreferUserFolder(candidates)
            Return chosen
        End If

        Return NotFound($"Keine LUT gefunden zu '{lutDisplayNameOrPath}'.")
    End Function

    ' --- Helpers ---

    Private Function PreferUserFolder(list As List(Of LutInfo)) As LutInfo
        ' Nutzerordner bevorzugen
        Dim user = list.FirstOrDefault(Function(li) li.FullPath.StartsWith(_docRoot, StringComparison.OrdinalIgnoreCase))
        If user.FullPath IsNot Nothing Then Return user
        Return list(0)
    End Function

    Private Function NotFound(msg As String) As LutInfo
        Return New LutInfo With {
            .FullPath = "",
            .LUTName = "",
            .LUTBeschreibung = "",
            .Type = LutType.Unknown,
            .SizeN = 0,
            .ErrorMessage = msg
        }
    End Function

End Module
