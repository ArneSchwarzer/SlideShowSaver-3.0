' TagSetterTool.vb
' VB.NET-Konsolenanwendung zum Setzen und Aktualisieren von .Tag auf sprachrelevanten Steuerelementen in *.Designer.vb-Dateien
' sowie vollständige Ausgabe aller relevanten Controls und ihrer Tags in eine Logdatei (für Übersetzungs-CSV)

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
            Dim logEintraegeDatei As New List(Of String)

            logEintraegeDatei.Add("=== " & datei & " ===")

            ' Tags setzen oder erfassen für Standard-Controls
            For Each typ In steuerbareTypen
                Dim regex = New Regex("(Me\.([a-zA-Z0-9_]+)\s*=\s*New\s*System\.Windows\.Forms\." & typ & ")", RegexOptions.Multiline)
                For Each match In regex.Matches(bearbeitetCode)
                    Dim ctrlName As String = match.Groups(2).Value
                    Dim tagRegex = New Regex("Me\." & Regex.Escape(ctrlName) & "\.Tag\s*=\s*""langKey=.+?""")
                    Dim tagZeile = "Me." & ctrlName & ".Tag = ""langKey=" & ctrlName & """"

                    If tagRegex.IsMatch(bearbeitetCode) Then
                        ' existierende Tag-Zuweisung gefunden – ggf. aktualisieren
                        If Not tagRegex.Match(bearbeitetCode).Value.Contains(ctrlName) Then
                            bearbeitetCode = tagRegex.Replace(bearbeitetCode, tagZeile)
                            logEintraegeDatei.Add("[Update] " & ctrlName & " => " & tagZeile)
                            dateiGeaendert = True
                            anzahlGeaendert += 1
                        Else
                            logEintraegeDatei.Add("[OK]     " & ctrlName & " => " & tagRegex.Match(bearbeitetCode).Value.Trim())
                        End If
                    Else
                        ' kein Tag vorhanden – neuen einfügen direkt nach Konstruktion
                        Dim konstruktion = match.Value
                        Dim pos = bearbeitetCode.IndexOf(konstruktion)
                        If pos >= 0 Then
                            Dim insertPos = bearbeitetCode.IndexOf(vbCrLf, pos) + 2
                            bearbeitetCode = bearbeitetCode.Insert(insertPos, tagZeile & vbCrLf)
                            logEintraegeDatei.Add("[Neu]    " & ctrlName & " => " & tagZeile)
                            dateiGeaendert = True
                            anzahlGeaendert += 1
                        End If
                    End If
                Next
            Next

            ' Sonderfall: SterneBewertungsControl
            Dim regexSterne = New Regex("(Me\.([a-zA-Z0-9_]+)\s*=\s*New\s*SlideShowControls\.SterneBewertungsControl)", RegexOptions.Multiline)
            For Each match In regexSterne.Matches(bearbeitetCode)
                Dim ctrlName As String = match.Groups(2).Value
                Dim styleSet As String = "Me." & ctrlName & ".Style = SlideShowControls.SterneBewertungsControl.Styles.STYLE_KLINGON"
                If Not bearbeitetCode.Contains(styleSet) Then
                    Dim konstruktion = match.Value
                    Dim pos = bearbeitetCode.IndexOf(konstruktion)
                    If pos >= 0 Then
                        Dim insertPos = bearbeitetCode.IndexOf(vbCrLf, pos) + 2
                        bearbeitetCode = bearbeitetCode.Insert(insertPos, styleSet & vbCrLf)
                        logEintraegeDatei.Add("[Style]  " & ctrlName & " => " & styleSet)
                        dateiGeaendert = True
                        anzahlGeaendert += 1
                    End If
                Else
                    logEintraegeDatei.Add("[Style✓] " & ctrlName & " => vorhanden")
                End If
            Next

            If logEintraegeDatei.Count > 1 Then
                logEintraege.AddRange(logEintraegeDatei)
                logEintraege.Add(String.Empty)
            End If

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