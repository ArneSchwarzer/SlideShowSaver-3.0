' SprachSpezialHandling.vb – aktualisiert
Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowSprachen.LanguageHelper

Public Class LanguageSpecialHandling

    Public Shared Sub WendeSpracheAufAlleControls(root As Control, iso As String)
        If root Is Nothing Then Exit Sub

        ' Fonts pro Sprache (aus deiner SprachenListe)
        Dim info = SprachenListe.FirstOrDefault(Function(x) x.ISOCode.Equals(iso, StringComparison.OrdinalIgnoreCase))
        If info.Font IsNot Nothing Then ApplyFontRecursive(root, info.Font)

        ' Dateiname/Key: der Name des Root-Controls (Form oder UC)
        Dim formName As String = If(root.FindForm() IsNot Nothing, root.FindForm().Name, root.Name)

        ' Sprache setzen
        LanguageCSVManager.AktuelleSprache = iso

        ' Rekursiv lokalisieren
        LocalizeTree(root, formName, iso)

        ' Klingon-Spezialstil
        If iso.Equals("TLH", StringComparison.OrdinalIgnoreCase) Then
            ApplyKlingonStyle(root)
        End If
    End Sub

    Private Shared Sub LocalizeTree(c As Control, formName As String, iso As String)
        ' Schlüssel: Tag bevorzugen (langKey=XYZ), sonst Name
        Dim key As String = Nothing
        Dim tagText As String = TryCast(c.Tag, String)
        If Not String.IsNullOrWhiteSpace(tagText) AndAlso tagText.Contains("langKey=") Then
            key = tagText.Split(";"c).FirstOrDefault(Function(t) t.StartsWith("langKey="))
            If key IsNot Nothing Then key = key.Substring("langKey=".Length)
        Else
            key = c.Name
        End If

        If Not String.IsNullOrWhiteSpace(key) Then
            Dim txt = LanguageCSVManager.HoleText(formName, iso, key, c.[GetType]().Assembly)
            If txt IsNot Nothing Then
                If TypeOf c Is ComboBox Then
                    ApplyComboBoxItems(DirectCast(c, ComboBox), txt)
                Else
                    c.Text = txt
                End If
            End If
        End If

        For Each child As Control In c.Controls
            LocalizeTree(child, formName, iso)
        Next
    End Sub

    Private Shared Sub ApplyFontRecursive(c As Control, f As Font)
        Try : c.Font = f : Catch : End Try
        For Each ch As Control In c.Controls
            ApplyFontRecursive(ch, f)
        Next
    End Sub

    Private Shared Function SplitLines(text As String) As String()
        If String.IsNullOrEmpty(text) Then Return Array.Empty(Of String)()
        ' \r\n, \r und Literal "\n" vereinheitlichen
        Dim t = text.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Replace("\n", vbLf)
        Return t.Split(New String() {vbLf}, StringSplitOptions.None)
    End Function

    Private Shared Sub ApplyComboBoxItems(cmb As ComboBox, multiline As String)
        Dim old = cmb.SelectedIndex
        cmb.BeginUpdate()
        Try
            cmb.Items.Clear()
            For Each line In SplitLines(multiline)
                cmb.Items.Add(line)
            Next

            ' Auswahl wiederherstellen
            If old >= 0 AndAlso old < cmb.Items.Count Then
                cmb.SelectedIndex = old
            ElseIf cmb.Items.Count > 0 Then
                cmb.SelectedIndex = 0   ' oder -1, je nach gewünschtem Verhalten
            End If
        Finally
            cmb.EndUpdate()
        End Try
    End Sub


    Private Shared Sub ApplyKlingonStyle(root As Control)
        ' deine bereits existierende Speziallogik (Logos, Farben etc.)
    End Sub

End Class
