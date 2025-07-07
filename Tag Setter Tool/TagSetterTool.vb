' TagSetterTool.vb
' VB.NET-Konsolenanwendung zum automatischen Setzen und Aktualisieren von .Tag auf sprachrelevanten Steuerelementen in *.Designer.vb-Dateien, inklusive einfachem Logging

Imports System.IO
Imports System.Text.RegularExpressions

Module TagSetterTool

    Sub Main()
        ' Alle unterstützten Steuerelementtypen für Mehrsprachigkeit
        Dim steuerbareTypen As String() = {
            "Label", "GroupBox", "RadioButton", "RichTextBox", "Button",
            "CheckBox", "LinkLabel", "TabPage", "TabControl", "ListBox",
            "ComboBox", "CheckedListBox", "PictureBox"
        }

        Console.WriteLine("Pfad zum Projektordner (z. B. C:\\Projekte\\SlideShowSaver):")
        Dim basisPfad As String = Console.ReadLine()

        If Not Directory.Exists(basisPfad) Then
            Console.WriteLine("Pfad ungültig.")
            Return
        End If

        Dim logPfad As String = Path.Combine(basisPfad, "TagSetterTool_Log.txt")
        Dim logEintraege As New List(Of String)

        Dim dateien = Directory.GetFiles(basisPfad, "*.Designer.vb", SearchOption.AllDirectories)
        Dim anzahlGeaendert As Integer = 0

        For Each datei In dateien
            Dim originalCode As String = File.ReadAllText(datei)
            Dim bearbeitetCode As String = originalCode
            Dim dateiGeaendert As Boolean = False

            ' Tags setzen oder aktualisieren für Standard-Controls
            For Each typ In steuerbareTypen
                Dim regex = New Regex("(Me\.([a-zA-Z0-9_]+)\s*=\s*New\s*System\.Windows\.Forms\." & typ & ")", RegexOptions.Multiline)
                For Each match In regex.Matches(bearbeitetCode)
                    Dim ctrlName As String = match.Groups(2).Value
                    Dim tagRegex = New Regex("Me\." & Regex.Escape(ctrlName) & "\.Tag\s*=\s*""langKey=.+?""")

                    If tagRegex.IsMatch(bearbeitetCode) Then
                        bearbeitetCode = tagRegex.Replace(bearbeitetCode, "Me." & ctrlName & ".Tag = ""langKey=" & ctrlName & """")
                        logEintraege.Add("[Update] " & Path.GetFileName(datei) & ": langKey für " & ctrlName & " aktualisiert.")
                    Else
                        Dim tagSet As String = "Me." & ctrlName & ".Tag = ""langKey=" & ctrlName & """"
                        If Not bearbeitetCode.Contains(tagSet) Then
                            bearbeitetCode &= vbCrLf & tagSet
                            logEintraege.Add("[Neu] " & Path.GetFileName(datei) & ": langKey für " & ctrlName & " hinzugefügt.")
                        End If
                    End If
                    dateiGeaendert = True
                    anzahlGeaendert += 1
                Next
            Next

            ' Sonderfall: SterneBewertungsControl
            Dim regexSterne = New Regex("(Me\.([a-zA-Z0-9_]+)\s*=\s*New\s*SlideShowControls\.SterneBewertungsControl)", RegexOptions.Multiline)
            For Each match In regexSterne.Matches(bearbeitetCode)
                Dim ctrlName As String = match.Groups(2).Value
                Dim styleSet As String = "Me." & ctrlName & ".Style = SlideShowControls.SterneBewertungsControl.Styles.STYLE_KLINGON"
                If Not bearbeitetCode.Contains(styleSet) Then
                    bearbeitetCode &= vbCrLf & styleSet
                    logEintraege.Add("[Style] " & Path.GetFileName(datei) & ": Style für " & ctrlName & " gesetzt.")
                    dateiGeaendert = True
                    anzahlGeaendert += 1
                End If
            Next

            If dateiGeaendert AndAlso bearbeitetCode <> originalCode Then
                File.WriteAllText(datei, bearbeitetCode)
                Console.WriteLine("Bearbeitet: " & Path.GetFileName(datei))
            End If
        Next

        ' Logdatei schreiben
        If logEintraege.Count > 0 Then
            File.WriteAllLines(logPfad, logEintraege)
            Console.WriteLine("Logdatei geschrieben: " & logPfad)
        End If

        Console.WriteLine("Fertig. Insgesamt " & anzahlGeaendert & " Einträge geändert oder aktualisiert.")
        Console.WriteLine("Taste drücken zum Beenden.")
        Console.ReadKey()
    End Sub

End Module
