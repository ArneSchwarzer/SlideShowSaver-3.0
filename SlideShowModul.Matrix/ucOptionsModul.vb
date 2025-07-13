Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowBildauswahl
Imports SlideShowTools
Imports System.Windows.Forms

Public Class ucOptionsModul

    'Variablendeklaration
    Private Shared aktuelleSettings As ModulMain.ModulSettings_Matrix

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert das UC und seine Steuerelemente

        'Aktuelle Settings aus der SettingsInbox abholen
        GetSettings(Of ModulMain.ModulSettings_Matrix)(ModulMain.nameModul)
        ClearSettings(ModulMain.nameModul)

        'Steuerelemente Initialisieren

        'lstHighlighttexte
        For Each item In aktuelleSettings.HighlightTexte
            lstHiglightTexte.Items.Add(item)
        Next

        'trbSzenendauerSekunden
        trbSzenendauer.Value = aktuelleSettings.SzenendauerSekunden

    End Sub

    Private Sub btnHighlighttextHinzufügen_Click(sender As Object, e As EventArgs) Handles btnHighlighttextHinzufügen.Click
        'Schreibt ein neues Tag in die Whitelist. 

        Dim myTag As String = ""
        Dim myInputBox As New frmInputModul

        'InputBox aufrufen
        If myInputBox.ShowDialog() = DialogResult.OK Then

            myTag = myInputBox.rueckgabeText

            'In die Liste eintragen (Doppeleinträge sind möglich, erhöht die Chancen dargestellt zu werden)
            lstHiglightTexte.Items.Add(myTag)


            'Button-Status setzen
            If lstHiglightTexte.Items.Count > 0 Then
                btnHighlighttextListeLöschen.Enabled = True
                btnHighlighttextLöschen.Enabled = (lstHiglightTexte.SelectedIndex >= 0)
            Else
                btnHighlighttextLöschen.Enabled = False
                btnHighlighttextListeLöschen.Enabled = False
            End If

        End If

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(lstHiglightTexte, ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH & "Highlighttexte")

    End Sub

    Private Sub btnHighlighttextLöschen_Click(sender As Object, e As EventArgs) Handles btnHighlighttextLöschen.Click
        If lstHiglightTexte.SelectedIndex < 0 Then
            Exit Sub
        End If

        lstHiglightTexte.Items.Remove(lstHiglightTexte.SelectedItem)

        If lstHiglightTexte.Items.Count > 0 Then
            btnHighlighttextListeLöschen.Enabled = True
            btnHighlighttextLöschen.Enabled = (lstHiglightTexte.SelectedIndex >= 0)
        Else
            btnHighlighttextListeLöschen.Enabled = False
            btnHighlighttextLöschen.Enabled = False
        End If

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(lstHiglightTexte, ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH & "Highlighttexte")

    End Sub

    Private Sub btnHighlighttextListeLöschen_Click(sender As Object, e As EventArgs) Handles btnHighlighttextListeLöschen.Click

        lstHiglightTexte.Items.Clear()
        btnHighlighttextListeLöschen.Enabled = False
        btnHighlighttextLöschen.Enabled = False

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH & "Highlighttexte", "")

    End Sub
End Class
