' LanguageCSVManager.vb
Imports System.IO
Imports System.Reflection
Imports System.Text

Public Module LanguageCSVManager

    Public Property AktuelleSprache As String = "DE"

    ' Cache: (lang|form) -> (key -> text)
    Private ReadOnly _cache As New Dictionary(Of String, Dictionary(Of String, String))(StringComparer.OrdinalIgnoreCase)

    Public Function HoleText(formName As String, lang As String, key As String, Optional sourceAsm As Assembly = Nothing) As String
        If String.IsNullOrWhiteSpace(key) Then Return Nothing
        Dim table = LadeTabelle(formName, lang, sourceAsm)
        Dim txt As String = Nothing
        If table IsNot Nothing AndAlso table.TryGetValue(key, txt) Then Return txt
        Return Nothing
    End Function

    Public Function LadeTabelle(formName As String, lang As String, Optional sourceAsm As Assembly = Nothing) As Dictionary(Of String, String)
        If String.IsNullOrWhiteSpace(formName) Then formName = "Unnamed"
        If String.IsNullOrWhiteSpace(lang) Then lang = AktuelleSprache

        Dim cacheKey = $"{lang}|{formName}"
        Dim table As Dictionary(Of String, String) = Nothing
        If _cache.TryGetValue(cacheKey, table) Then Return table

        If sourceAsm Is Nothing Then sourceAsm = Assembly.GetCallingAssembly()

        ' Wir suchen nach einem Embedded-Resource-Namen, der auf "Lang/<ISO>/<Form>.csv" endet
        ' (LogicalName siehe .vbproj oben)
        Dim wantedSuffix = $"Lang/{lang}/{formName}.csv".ToLowerInvariant()

        Dim resName As String = Nothing
        For Each n In sourceAsm.GetManifestResourceNames()
            If n.ToLowerInvariant().EndsWith(wantedSuffix) Then
                resName = n : Exit For
            End If
        Next

        ' DEBUG-Fallback: aus Ordner lesen (praktisch beim Übersetzen/Feintuning)
#If DEBUG Then
        If resName Is Nothing Then
            Dim prob1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lang", lang, formName & ".csv")
            If File.Exists(prob1) Then
                table = LadeCsvAusStream(File.OpenRead(prob1))
                _cache(cacheKey) = table
                Return table
            End If
        End If
#End If

        If resName Is Nothing Then
            table = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            _cache(cacheKey) = table
            Return table
        End If

        Using s = sourceAsm.GetManifestResourceStream(resName)
            table = LadeCsvAusStream(s)
        End Using

        _cache(cacheKey) = table
        Return table
    End Function

    Private Function LadeCsvAusStream(str As Stream) As Dictionary(Of String, String)
        Dim dict As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Using sr As New StreamReader(str, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False, throwOnInvalidBytes:=True))
            Dim headerProcessed As Boolean = False
            While Not sr.EndOfStream
                Dim record = LeseCsvDatensatz(sr)
                If record Is Nothing OrElse record.Count = 0 Then Continue While

                If Not headerProcessed Then
                    headerProcessed = True
                    Dim h = record(0).Trim().ToLowerInvariant()
                    If h = "key" OrElse h = "tag" Then Continue While ' Kopfzeile weg
                End If

                If record.Count >= 2 Then
                    Dim key = record(0).Trim()
                    Dim text = record(1)
                    If key.Length > 0 Then dict(key) = text
                End If
            End While
        End Using
        Return dict
    End Function

    ' Liest genau EINEN CSV-Datensatz (inkl. eingebetteter Newlines) aus dem Stream
    Private Function LeseCsvDatensatz(sr As StreamReader) As List(Of String)
        If sr.EndOfStream Then Return Nothing
        Dim line = sr.ReadLine()
        If line Is Nothing Then Return Nothing

        Dim sb As New StringBuilder(line)
        Dim quoteCount As Integer = CountQuotes(line)
        While (quoteCount Mod 2) = 1 AndAlso Not sr.EndOfStream
            Dim nxt = sr.ReadLine()
            sb.AppendLine().Append(nxt)
            quoteCount += CountQuotes(nxt)
        End While
        Dim rec = sb.ToString()

        ' Trennzeichen raten (Semikolon/Komma)
        Dim sep As Char = If(rec.Count(Function(ch) ch = ";"c) >= rec.Count(Function(ch) ch = ","c), ";"c, ","c)

        Dim fields As New List(Of String)
        Dim cur As New StringBuilder()
        Dim inside As Boolean = False
        For Each ch In rec
            If ch = """"c Then
                inside = Not inside
            ElseIf ch = sep AndAlso Not inside Then
                fields.Add(Unquote(cur.ToString())) : cur.Clear()
            Else
                cur.Append(ch)
            End If
        Next
        fields.Add(Unquote(cur.ToString()))
        Return fields
    End Function

    Private Function CountQuotes(s As String) As Integer
        Dim n As Integer = 0
        For Each ch In s
            If ch = """"c Then
                n += 1
            End If
        Next
        Return n
  End Function

    Private Function Unquote(s As String) As String
        s = s.Trim()
        If s.StartsWith("""") AndAlso s.EndsWith("""") Then
            s = s.Substring(1, s.Length - 2).Replace("""""", """")
        End If
        ' In CSV gespeichertes \n in echte Umbrüche wandeln:
        s = s.Replace("\r\n", vbLf).Replace("\n", vbLf).Replace(vbCr, vbLf)
        Return s
    End Function

End Module
