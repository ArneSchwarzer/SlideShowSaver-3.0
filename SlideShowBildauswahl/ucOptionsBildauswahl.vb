Imports System.Windows.Forms
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools

Public Class ucOptionsBildauswahl
    'Variablendeklaration

    'Settings
    Private aktuelleSettings As SettingsBildauswahl


    Private Sub ucOptionsBildauswahl_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Settings einlesen und setzten
        CheckYourMail()

        'Steuerelemente initialisieren
        IniOrReinitialise()

    End Sub

    Private Sub rdo18_CheckedChanged(sender As Object, e As EventArgs) Handles rdo18.CheckedChanged
        'Behandelt den RadioButton rdo18

        'Blacklist Tags setzen/löschen
        lstBlackList.Items.Remove("18+")
        lstBlackList.Items.Remove("Akt")
        lstBlackList.Items.Remove("Lingerie")

        'DirectCommit
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", "18+")
        CheckedListBoxHandling.SaveListBoxToRegistry(lstBlackList, SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags")

    End Sub

    Private Sub rdoAkt_CheckedChanged(sender As Object, e As EventArgs) Handles rdoAkt.CheckedChanged
        'Behandelt den RadioButton rdo Akt

        'Blacklist Tags setzen/löschen
        If Not lstBlackList.Items.Contains("18+") Then
            lstBlackList.Items.Add("18+")
        End If
        lstBlackList.Items.Remove("Akt")
        lstBlackList.Items.Remove("Lingerie")

        'DirectCommit
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", "Akt")
        CheckedListBoxHandling.SaveListBoxToRegistry(lstBlackList, SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags")

    End Sub

    Private Sub rdoLingerie_CheckedChanged(sender As Object, e As EventArgs) Handles rdoLingerie.CheckedChanged
        'Behandelt den RadioButton rdoLingerie

        'Blacklist Tags setzen/löschen
        If Not lstBlackList.Items.Contains("18+") Then
            lstBlackList.Items.Add("18+")
        End If
        If Not lstBlackList.Items.Contains("Akt") Then
            lstBlackList.Items.Add("Akt")
        End If
        lstBlackList.Items.Remove("Lingerie")

        'DirectCommit
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", "Lingerie")
        CheckedListBoxHandling.SaveListBoxToRegistry(lstBlackList, SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags")

    End Sub

    Private Sub rdoJugendfrei_CheckedChanged(sender As Object, e As EventArgs) Handles rdoJugendfrei.CheckedChanged
        'Behandelt den RadioButton rdoJugendfrei

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

        'DirectCommit
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", "Jugendfrei")
        CheckedListBoxHandling.SaveListBoxToRegistry(lstBlackList, SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags")

    End Sub

    Private Sub lstVerzeichnisse_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstVerzeichnisse.SelectedIndexChanged
        'Buttons gemäß selektierem Eintrag in lstVerzeichnisse aktivieren/deaktivieren

        btnVerzeichnisseLöschen.Enabled = (lstVerzeichnisse.SelectedIndex >= 0)

    End Sub

    Private Sub lstWhiteList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstWhiteList.SelectedIndexChanged
        'Buttons gemäß selektierem Eintrag in lstWhiteList aktivieren/deaktivieren

        btnWhiteListLöschen.Enabled = (lstWhiteList.SelectedIndex >= 0)

    End Sub

    Private Sub lstBlackList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstBlackList.SelectedIndexChanged
        'Buttons gemäß selektierem Eintrag in lstBlackList aktivieren/deaktivieren

        btnBlackListLöschen.Enabled = (lstBlackList.SelectedIndex >= 0)

    End Sub

    Private Sub btnVerzeichnisHinzufügen_Click(sender As Object, e As EventArgs) Handles btnVerzeichnisHinzufügen.Click
        'Behandelt btnVerzeichnisHinzufügen

        'Ruft den Dialog 'Ordner auswählen" aus.
        Using dlg As New FolderBrowserDialog()
            dlg.Description = "Bitte wähle ein Verzeichnis aus"
            dlg.ShowNewFolderButton = False

            If dlg.ShowDialog() = DialogResult.OK Then
                Dim ausgewählterPfad As String = dlg.SelectedPath
                If Not lstVerzeichnisse.Items.Contains(ausgewählterPfad) Then
                    lstVerzeichnisse.Items.Add(ausgewählterPfad)
                End If
            End If
        End Using

        'Buttons setzen
        If lstVerzeichnisse.Items.Count > 0 Then
            btnVerzeichnisseListeLöschen.Enabled = True
            btnVerzeichnisseLöschen.Enabled = (lstVerzeichnisse.SelectedIndex >= 0)
        Else
            btnVerzeichnisseListeLöschen.Enabled = False
            btnVerzeichnisseLöschen.Enabled = False
        End If

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(lstVerzeichnisse, SLIDESHOWBILDAUSWAHL_PATH & "Verzeichnisse")

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

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(lstWhiteList, SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags")

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

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(lstBlackList, SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags")

    End Sub

    Private Sub btnVerzeichnisseLöschen_Click(sender As Object, e As EventArgs) Handles btnVerzeichnisseLöschen.Click
        'Löscht einen Eintrage aus der Verzeichnis-Liste

        If lstVerzeichnisse.SelectedIndex < 0 Then
            Exit Sub
        End If

        lstVerzeichnisse.Items.Remove(lstVerzeichnisse.SelectedItem)

        'Button setzen
        If lstVerzeichnisse.Items.Count > 0 Then
            btnVerzeichnisseListeLöschen.Enabled = True
            btnVerzeichnisseLöschen.Enabled = (lstVerzeichnisse.SelectedIndex >= 0)
        Else
            btnVerzeichnisseListeLöschen.Enabled = False
            btnVerzeichnisseLöschen.Enabled = False
        End If

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(lstVerzeichnisse, SLIDESHOWBILDAUSWAHL_PATH & "Verzeichnisse")

    End Sub

    Private Sub btnWhiteListLöschen_Click(sender As Object, e As EventArgs) Handles btnWhiteListLöschen.Click
        'Löscht einen Eintrag aus der WhiteList

        If lstWhiteList.SelectedIndex < 0 Then
            Exit Sub
        End If

        lstWhiteList.Items.Remove(lstWhiteList.SelectedItem)

        'Buttons setzen
        If lstWhiteList.Items.Count > 0 Then
            btnWhiteListListeLöschen.Enabled = True
            btnWhiteListLöschen.Enabled = (lstWhiteList.SelectedIndex >= 0)
        Else
            btnWhiteListLöschen.Enabled = False
            btnWhiteListListeLöschen.Enabled = False
        End If

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(lstWhiteList, SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags")

    End Sub

    Private Sub btnBlackListLöschen_Click(sender As Object, e As EventArgs) Handles btnBlackListLöschen.Click
        'Löscht einen Eintrag aus der BlackList

        If lstBlackList.SelectedIndex < 0 Then
            Exit Sub
        End If

        lstBlackList.Items.Remove(lstBlackList.SelectedItem)

        'Buttons setzen
        If lstBlackList.Items.Count > 0 Then
            btnBlackListListeLöschen.Enabled = True
            btnBlackListLöschen.Enabled = (lstBlackList.SelectedIndex >= 0)
        Else
            btnBlackListLöschen.Enabled = False
            btnBlackListListeLöschen.Enabled = False
        End If

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(lstBlackList, SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags")

    End Sub

    Private Sub btnBlackListListeLöschen_Click(sender As Object, e As EventArgs) Handles btnBlackListListeLöschen.Click
        'Löscht alle Einträge aus der BlackList

        lstBlackList.Items.Clear()
        btnBlackListLöschen.Enabled = False
        btnBlackListListeLöschen.Enabled = False

        'DirectCommit
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags", "")

    End Sub

    Private Sub btnVerzeichnisseListeLöschen_Click(sender As Object, e As EventArgs) Handles btnVerzeichnisseListeLöschen.Click
        'Löscht alle Einträge aus der Verzeichnisliste

        lstVerzeichnisse.Items.Clear()
        btnVerzeichnisseListeLöschen.Enabled = False
        btnVerzeichnisseLöschen.Enabled = False

        'DirectCommit
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Verzeichnisse", "")

    End Sub

    Private Sub btnWhiteListListeLöschen_Click(sender As Object, e As EventArgs) Handles btnWhiteListListeLöschen.Click
        'Löscht alle Einträge aus der WhiteList

        lstWhiteList.Items.Clear()
        btnWhiteListLöschen.Enabled = False
        btnWhiteListListeLöschen.Enabled = False

        'DirectCommit
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags", "")

    End Sub

    Private Sub sbcBewertung_BewertungGeaendert(sender As Object, neueBewertung As Integer) Handles sbcBewertung.BewertungGeaendert
        'Behandelt das SterneBewertungsControl

        'DirectCommit
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Bewertung", sbcBewertung.Bewertung.ToString)

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuelleSettings aus der SettingsInbox aus

        aktuelleSettings = GetSettings(Of SettingsBildauswahl)("Bildauswahl")

    End Sub

    Private Sub IniOrReinitialise()
        'lstVerzeichnisse
        For Each item In aktuelleSettings.Verzeichnisse
            lstVerzeichnisse.Items.Add(item)
        Next
        btnVerzeichnisseLöschen.Enabled = False
        If lstVerzeichnisse.Items.Count = 0 Then
            btnVerzeichnisseListeLöschen.Enabled = False
        End If

        'White-List
        For Each item In aktuelleSettings.WhiteListTags
            lstWhiteList.Items.Add(item)
        Next
        btnWhiteListLöschen.Enabled = False
        If lstWhiteList.Items.Count = 0 Then
            btnWhiteListListeLöschen.Enabled = False
        End If

        'Black-List
        For Each item In aktuelleSettings.BlackListTags
            lstBlackList.Items.Add(item)
        Next
        btnBlackListLöschen.Enabled = False
        If lstBlackList.Items.Count = 0 Then
            btnBlackListListeLöschen.Enabled = False
        End If

        'SterneBewertungControl setzten und dann dessen Eventhandling einschalten.
        sbcBewertung.Bewertung = aktuelleSettings.Bewertung
        sbcBewertung.EndInitialization()

        'Altersfreigabe - RadioButtons & Black-List Einträge
        Select Case aktuelleSettings.Altersfreigabe
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

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Default-Werte einlesen und Steuerelemente entsprechend setzen

        Dim defaults As Dictionary(Of String, String)

        'Defaults einlesen
        defaults = GetBildauswahlDefaultSettings()

        'AktuelleSettings aktualisieren
        aktuelleSettings.Verzeichnisse = SplitSemicolonList(defaults("Verzeichnisse"))
        aktuelleSettings.WhiteListTags = SplitSemicolonList(defaults("WhiteListTags"))
        aktuelleSettings.BlackListTags = SplitSemicolonList(defaults("BlackListTags"))
        aktuelleSettings.Altersfreigabe = defaults("Altersfreigabe")
        aktuelleSettings.Bewertung = CInt(defaults("Bewertung"))

        'Listen leeren
        lstVerzeichnisse.Items.Clear()
        lstWhiteList.Items.Clear()
        lstBlackList.Items.Clear()

        'Steuerelemente setzen
        IniOrReinitialise()

    End Sub
End Class

