Imports System.Windows.Forms
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Public Class ucOptionsBildauswahl
    'Variablendeklaration

    'Settings
    Private defaults As Dictionary(Of String, String)
    Private verzeichnisseByReg As String
    Private verzeichnisListe As List(Of String)
    Private whitelistByReg As String
    Private whitelistListe As List(Of String)
    Private blacklistByReg As String
    Private blacklistListe As List(Of String)
    Private altersfreigabe As String
    Private bewertung As Integer

    Private Sub ucOptionsBildauswahl_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Settings einlesen und setzten
        defaults = GetBildauswahlDefaultSettings()

        verzeichnissebyReg = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Verzeichnisse", defaults)
        verzeichnisListe = SplitSemicolonList(verzeichnisseByReg)
        For Each item In verzeichnisListe
            lstVerzeichnisse.Items.Add(item)
        Next
        btnVerzeichnisseLöschen.Enabled = False
        If lstVerzeichnisse.Items.Count = 0 Then
            btnVerzeichnisseListeLöschen.Enabled = False
        End If

        whitelistByReg = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags", defaults)
        whitelistListe = SplitSemicolonList(whitelistByReg)
        For Each item In whitelistListe
            lstWhiteList.Items.Add(item)
        Next
        btnWhiteListLöschen.Enabled = False
        If lstWhiteList.Items.Count = 0 Then
            btnWhiteListListeLöschen.Enabled = False
        End If

        blacklistByReg = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags", defaults)
        blacklistListe = SplitSemicolonList(blacklistByReg)
        For Each item In blacklistListe
            lstBlackList.Items.Add(item)
        Next
        btnBlackListLöschen.Enabled = False
        If lstBlackList.Items.Count = 0 Then
            btnBlackListListeLöschen.Enabled = False
        End If

        bewertung = CInt(ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Bewertung", defaults))
        sbcBewertung.Bewertung = bewertung

        altersfreigabe = ReadFromRegOrDefaults(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", defaults)
        Select Case altersfreigabe
            Case "18+"
                rdo18.Checked = True
                'Blacklist Tags setzen/löschen
                lstBlackList.Items.Remove("18+")
                lstBlackList.Items.Remove("Akt")
                lstBlackList.Items.Remove("Lingerie")
            Case "Akt"
                rdoAkt.Checked = True
                'Blacklist Tags setzen/löschen
                If Not lstBlackList.Items.Contains("18+") Then
                    lstBlackList.Items.Add("18+")
                End If
                lstBlackList.Items.Remove("Akt")
                lstBlackList.Items.Remove("Lingerie")
            Case "Lingerie"
                rdoLingerie.Checked = True
                'Blacklist Tags setzen/löschen
                If Not lstBlackList.Items.Contains("18+") Then
                    lstBlackList.Items.Add("18+")
                End If
                If Not lstBlackList.Items.Contains("Akt") Then
                    lstBlackList.Items.Add("Akt")
                End If
                lstBlackList.Items.Remove("Lingerie")
            Case "Jugendfrei"
                rdoJugendfrei.Checked = True
                'Blacklist Tags setzen/löschen
                If Not lstBlackList.Items.Contains("18+") Then
                    lstBlackList.Items.Add("18+")
                End If
                If Not lstBlackList.Items.Contains("Akt") Then
                    lstBlackList.Items.Add("Akt")
                End If
                If Not lstBlackList.Items.Contains("Lingerie") Then
                    lstBlackList.Items.Add("Lingerie")
                End If
        End Select

    End Sub

    Private Sub rdo18_CheckedChanged(sender As Object, e As EventArgs) Handles rdo18.CheckedChanged
        'Blacklist Tags setzen/löschen
        lstBlackList.Items.Remove("18+")
        lstBlackList.Items.Remove("Akt")
        lstBlackList.Items.Remove("Lingerie")
    End Sub

    Private Sub rdoAkt_CheckedChanged(sender As Object, e As EventArgs) Handles rdoAkt.CheckedChanged
        'Blacklist Tags setzen/löschen
        If Not lstBlackList.Items.Contains("18+") Then
            lstBlackList.Items.Add("18+")
        End If
        lstBlackList.Items.Remove("Akt")
        lstBlackList.Items.Remove("Lingerie")
    End Sub

    Private Sub rdoLingerie_CheckedChanged(sender As Object, e As EventArgs) Handles rdoLingerie.CheckedChanged
        'Blacklist Tags setzen/löschen
        If Not lstBlackList.Items.Contains("18+") Then
            lstBlackList.Items.Add("18+")
        End If
        If Not lstBlackList.Items.Contains("Akt") Then
            lstBlackList.Items.Add("Akt")
        End If
        lstBlackList.Items.Remove("Lingerie")
    End Sub

    Private Sub rdoJugendfrei_CheckedChanged(sender As Object, e As EventArgs) Handles rdoJugendfrei.CheckedChanged
        'Blacklist Tags setzen/löschen
        If Not lstBlackList.Items.Contains("18+") Then
            lstBlackList.Items.Add("18+")
        End If
        If Not lstBlackList.Items.Contains("Akt") Then
            lstBlackList.Items.Add("Akt")
        End If
        If Not lstBlackList.Items.Contains("Lingerie") Then
            lstBlackList.Items.Add("Lingerie")
        End If
    End Sub

    Private Sub lstVerzeichnisse_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstVerzeichnisse.SelectedIndexChanged
        btnVerzeichnisseLöschen.Enabled = (lstVerzeichnisse.SelectedIndex >= 0)
    End Sub

    Private Sub lstWhiteList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstWhiteList.SelectedIndexChanged
        btnWhiteListLöschen.Enabled = (lstWhiteList.SelectedIndex >= 0)
    End Sub

    Private Sub lstBlackList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstBlackList.SelectedIndexChanged
        btnBlackListLöschen.Enabled = (lstBlackList.SelectedIndex >= 0)
    End Sub

    Private Sub btnVerzeichnisHinzufügen_Click(sender As Object, e As EventArgs) Handles btnVerzeichnisHinzufügen.Click


        Using dlg As New FolderBrowserDialog()
            dlg.Description = "Bitte wähle ein Verzeichnis aus"
            dlg.ShowNewFolderButton = True

            If dlg.ShowDialog() = DialogResult.OK Then
                Dim ausgewählterPfad As String = dlg.SelectedPath
                If Not lstVerzeichnisse.Items.Contains(ausgewählterPfad) Then
                    lstVerzeichnisse.Items.Add(ausgewählterPfad)
                End If
            End If
        End Using
        If lstVerzeichnisse.Items.Count > 0 Then
            btnVerzeichnisseListeLöschen.Enabled = True
            btnVerzeichnisseLöschen.Enabled = (lstVerzeichnisse.SelectedIndex >= 0)
        Else
            btnVerzeichnisseListeLöschen.Enabled = False
            btnVerzeichnisseLöschen.Enabled = False
        End If
    End Sub

    Private Sub btnWhiteListHinzufügen_Click(sender As Object, e As EventArgs) Handles btnWhiteListHinzufügen.Click
        'Schreibt ein neues Tag in die Whitelist. Bei einem Tag-Konflikt mit Einträgen
        'aus der BlackList wird eine Warnung ausgegeben

        Dim myTag As String = ""
        Dim myMessageBox As frmMessageBildauswahl
        Dim myInputBox As New frmInputBildauswahl

        'InputBox aufrufen
        If myInputBox.ShowDialog() = DialogResult.OK Then

            myTag = myInputBox.rueckgabeTag

            'Gegen Blacklist-Einträge gegenprüfen
            If lstBlackList.Items.Contains(myTag) Then
                myMessageBox = New frmMessageBildauswahl(myTag, False)
                myMessageBox.Show()
                Exit Sub
            End If

            'In die Liste eintragen
            If Not lstWhiteList.Items.Contains(myTag) Then
                lstWhiteList.Items.Add(myTag)
            End If

            'Button-Status setzen
            If lstWhiteList.Items.Count > 0 Then
                btnWhiteListListeLöschen.Enabled = True
                btnWhiteListLöschen.Enabled = (lstWhiteList.SelectedIndex >= 0)
            Else
                btnWhiteListLöschen.Enabled = False
                btnWhiteListListeLöschen.Enabled = False
            End If

        End If

    End Sub

    Private Sub btnBlackListHinzufügen_Click(sender As Object, e As EventArgs) Handles btnBlackListHinzufügen.Click
        'Schreibt ein neues Tag in die Blacklist. Bei einem Tag-Konflikt mit Einträgen
        'aus der WhiteList wird eine Warnung ausgegeben

        Dim myTag As String = ""
        Dim myMessageBox As frmMessageBildauswahl
        Dim myInputBox As New frmInputBildauswahl

        'InputBox aufrufen
        If myInputBox.ShowDialog() = DialogResult.OK Then

            myTag = myInputBox.rueckgabeTag


            'Gegen WhiteListlist-Einträge gegenprüfen
            If lstWhiteList.Items.Contains(myTag) Then
                myMessageBox = New frmMessageBildauswahl(myTag, True)
                myMessageBox.Show()
                Exit Sub
            End If

            'In die Liste eintragen
            If Not lstBlackList.Items.Contains(myTag) Then
                lstBlackList.Items.Add(myTag)
            End If

            'Button-Status setzen
            If lstBlackList.Items.Count > 0 Then
                btnBlackListListeLöschen.Enabled = True
                btnBlackListLöschen.Enabled = (lstBlackList.SelectedIndex >= 0)
            Else
                btnBlackListLöschen.Enabled = False
                btnBlackListListeLöschen.Enabled = False
            End If

        End If

    End Sub

    Private Sub btnVerzeichnisseLöschen_Click(sender As Object, e As EventArgs) Handles btnVerzeichnisseLöschen.Click
        If lstVerzeichnisse.SelectedIndex < 0 Then
            Exit Sub
        End If
        lstVerzeichnisse.Items.Remove(lstVerzeichnisse.SelectedItem)
        If lstVerzeichnisse.Items.Count > 0 Then
            btnVerzeichnisseListeLöschen.Enabled = True
            btnVerzeichnisseLöschen.Enabled = (lstVerzeichnisse.SelectedIndex >= 0)
        Else
            btnVerzeichnisseListeLöschen.Enabled = False
            btnVerzeichnisseLöschen.Enabled = False
        End If
    End Sub

    Private Sub btnWhiteListLöschen_Click(sender As Object, e As EventArgs) Handles btnWhiteListLöschen.Click
        If lstWhiteList.SelectedIndex < 0 Then
            Exit Sub
        End If
        lstWhiteList.Items.Remove(lstWhiteList.SelectedItem)
        If lstWhiteList.Items.Count > 0 Then
            btnWhiteListListeLöschen.Enabled = True
            btnWhiteListLöschen.Enabled = (lstWhiteList.SelectedIndex >= 0)
        Else
            btnWhiteListLöschen.Enabled = False
            btnWhiteListListeLöschen.Enabled = False
        End If
    End Sub

    Private Sub btnBlackListLöschen_Click(sender As Object, e As EventArgs) Handles btnBlackListLöschen.Click
        If lstBlackList.SelectedIndex < 0 Then
            Exit Sub
        End If
        lstBlackList.Items.Remove(lstBlackList.SelectedItem)
        If lstBlackList.Items.Count > 0 Then
            btnBlackListListeLöschen.Enabled = True
            btnBlackListLöschen.Enabled = (lstBlackList.SelectedIndex >= 0)
        Else
            btnBlackListLöschen.Enabled = False
            btnBlackListListeLöschen.Enabled = False
        End If
    End Sub

    Private Sub btnBlackListListeLöschen_Click(sender As Object, e As EventArgs) Handles btnBlackListListeLöschen.Click
        lstBlackList.Items.Clear()
        btnBlackListLöschen.Enabled = False
        btnBlackListListeLöschen.Enabled = False
    End Sub

    Private Sub btnVerzeichnisseListeLöschen_Click(sender As Object, e As EventArgs) Handles btnVerzeichnisseListeLöschen.Click
        lstVerzeichnisse.Items.Clear()
        btnVerzeichnisseListeLöschen.Enabled = False
        btnVerzeichnisseLöschen.Enabled = False
    End Sub

    Private Sub btnWhiteListListeLöschen_Click(sender As Object, e As EventArgs) Handles btnWhiteListListeLöschen.Click
        lstWhiteList.Items.Clear()
        btnWhiteListLöschen.Enabled = False
        btnWhiteListListeLöschen.Enabled = False
    End Sub
End Class
