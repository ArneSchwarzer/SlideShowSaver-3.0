Public Class LanguageManager
    Public Shared CurrentLanguage As String = "de"
    Private Shared translations As New Dictionary(Of String, Dictionary(Of String, String))

    Public Shared Sub LoadTranslations(filePath As String)
        translations.Clear()
        If Not System.IO.File.Exists(filePath) Then Exit Sub

        Dim lines = IO.File.ReadAllLines(filePath)
        For Each line In lines
            Dim parts = line.Split(";"c)
            If parts.Length >= 2 Then
                Dim key = parts(0)
                Dim values As New Dictionary(Of String, String)
                If parts.Length > 1 Then values("de") = parts(1)
                If parts.Length > 2 Then values("en") = parts(2)
                If parts.Length > 3 Then values("fr") = parts(3)
                If parts.Length > 4 Then values("hi") = parts(4)
                If parts.Length > 5 Then values("pl") = parts(5)
                If parts.Length > 6 Then values("ru") = parts(6)
                If parts.Length > 7 Then values("zh") = parts(7)
                translations(key) = values
            End If
        Next
    End Sub

    Public Shared Function Translate(key As String) As String
        If translations.ContainsKey(key) AndAlso translations(key).ContainsKey(CurrentLanguage) Then
            Return translations(key)(CurrentLanguage)
        Else
            Return $"[{key}]"
        End If
    End Function
End Class
