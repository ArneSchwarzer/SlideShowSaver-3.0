Imports System.IO
Imports SlideShowLogging
Imports SlideShowInterfaces.InfoHandling

Public Module LUTListLoader

    ' *** Variablen am Anfang ***

    ' Dokumente: ...\SlideShowSaver 3.0\Shader\LUTShader
    Private ReadOnly DocRoot As String =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                     "SlideShowSaver 3.0\Shader\LUTShader")

    ' Installation/Debug: <EXE>\Shader\LUTShader
    Private ReadOnly InstRoot As String =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shader\LUTShader")

    Public Function LUTListLoader() As List(Of LutInfo)
        Dim result As New List(Of LutInfo)
        Dim roots = New String() {InstRoot, DocRoot}
        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each root In roots
            If Not Directory.Exists(root) Then Continue For

            For Each pattern In New String() {"*.cube", "*.png", "*.lut"}
                Dim files As IEnumerable(Of String) = Enumerable.Empty(Of String)()
                Try
                    files = Directory.EnumerateFiles(root, pattern, SearchOption.TopDirectoryOnly)
                Catch ex As Exception
                    LogHandling.LogError($"LUTListLoader: Konnte Dateien in '{root}' nicht auflisten: {ex.Message}")
                End Try

                For Each fp In files
                    If Not seen.Add(fp) Then Continue For
                    Try
                        Dim info As LutInfo = LUTSniffer.SniffFile(fp)
                        If info.Type <> LutType.Unknown Then
                            result.Add(info)
                            LogHandling.LogDebug($"Gefundene LUT: {fp}")
                        End If
                    Catch ex As Exception
                        LogHandling.LogError($"LUTListLoader: Fehler beim Sniffen '{fp}': {ex.Message}")
                    End Try
                Next
            Next
        Next

        result.Sort(Function(a, b) StringComparer.CurrentCultureIgnoreCase.Compare(a.DisplayName, b.DisplayName))
        Return result
    End Function

End Module
