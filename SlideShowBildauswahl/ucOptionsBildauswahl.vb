Imports System.Windows.Forms
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools

Public Class ucOptionsBildauswahl
#Region "Variablendeklaration"
    'Settings
    Private aktuelleSettings As SettingsBildauswahl

    'Initialisierung
    Private initialisierungLaeuft As Boolean
#End Region

    Private Sub ucOptionsBildauswahl_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Settings einlesen und setzten
        CheckYourMail()

        'Steuerelemente initialisieren
        IniOrReinitialise()

    End Sub

    Private Sub rdo18_CheckedChanged(sender As Object, e As EventArgs) Handles rdo18.CheckedChanged
        'Behandelt die Auswahl der Altersfreigabe 18+.

        If initialisierungLaeuft OrElse Not rdo18.Checked Then
            Exit Sub
        End If

        SpeichereAltersfreigabe("18+")

    End Sub

    Private Sub rdoAkt_CheckedChanged(sender As Object, e As EventArgs) Handles rdoAkt.CheckedChanged
        'Behandelt die Auswahl der Altersfreigabe Akt.

        If initialisierungLaeuft OrElse Not rdoAkt.Checked Then
            Exit Sub
        End If

        SpeichereAltersfreigabe("Akt")

    End Sub

    Private Sub rdoLingerie_CheckedChanged(sender As Object, e As EventArgs) Handles rdoLingerie.CheckedChanged
        'Behandelt die Auswahl der Altersfreigabe Lingerie.

        If initialisierungLaeuft OrElse Not rdoLingerie.Checked Then
            Exit Sub
        End If

        SpeichereAltersfreigabe("Lingerie")

    End Sub

    Private Sub rdoJugendfrei_CheckedChanged(sender As Object, e As EventArgs) Handles rdoJugendfrei.CheckedChanged
        'Behandelt die Auswahl der Altersfreigabe Jugendfrei.

        If initialisierungLaeuft OrElse Not rdoJugendfrei.Checked Then
            Exit Sub
        End If

        SpeichereAltersfreigabe("Jugendfrei")

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
        'Schreibt ein neues Tag in die Whitelist.
        'Bekannte Altersfreigabe-Tags werden gegen die aktuelle Freigabe geprüft.

        Dim myTag As String
        Dim erforderlicheAltersfreigabe As String
        Dim myMessageBox As frmMessageBildauswahl
        Dim myInputBox As frmInputBildauswahl
        Dim warnDialog As frmAltersfreigabeWarnung
        Dim warnErgebnis As frmAltersfreigabeWarnung.AltersfreigabeWarnungErgebnis

        myTag = String.Empty
        erforderlicheAltersfreigabe = Nothing
        myMessageBox = Nothing
        myInputBox = New frmInputBildauswahl()
        warnDialog = Nothing

        Using myInputBox

            If myInputBox.ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            myTag =
            If(
                myInputBox.rueckgabeTag,
                String.Empty).
            Trim()

        End Using

        If String.IsNullOrWhiteSpace(myTag) Then
            Exit Sub
        End If

        'Gegen die sichtbare Benutzer-Blacklist prüfen.
        If ListBoxEnthaeltTag(lstBlackList, myTag) Then

            myMessageBox =
            New frmMessageBildauswahl(
                myTag,
                False)

            Using myMessageBox
                myMessageBox.ShowDialog()
            End Using

            Exit Sub

        End If

        'Bekannte Altersfreigabe-Tags gegen die aktuelle Freigabe prüfen.
        If Not IstWhitelistTagMitAltersfreigabeKompatibel(myTag, aktuelleSettings.Altersfreigabe,
                                                          erforderlicheAltersfreigabe) Then

            warnDialog =
            New frmAltersfreigabeWarnung(
                myTag,
                aktuelleSettings.Altersfreigabe,
                erforderlicheAltersfreigabe)

            Using warnDialog

                warnDialog.ShowDialog()

                warnErgebnis = warnDialog.Ergebnis

            End Using

            Select Case warnErgebnis

                Case frmAltersfreigabeWarnung.
                AltersfreigabeWarnungErgebnis.Abbrechen

                    Exit Sub

                Case frmAltersfreigabeWarnung.
                AltersfreigabeWarnungErgebnis.AltersfreigabeAnpassen

                    SetzeAltersfreigabeUndRadioButton(erforderlicheAltersfreigabe)

                Case frmAltersfreigabeWarnung.
                AltersfreigabeWarnungErgebnis.TagTrotzdemHinzufuegen

                    'Das Tag wird gespeichert.
                    'Die bestehende Altersfreigabe bleibt unverändert.

            End Select

        End If

        If Not ListBoxEnthaeltTag(lstWhiteList, myTag) Then

            lstWhiteList.Items.Add(myTag)

        End If

        AktualisiereButtonStatus()

        CheckedListBoxHandling.SaveListBoxToRegistry(lstWhiteList, SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags")

    End Sub

    Private Sub btnBlackListHinzufügen_Click(sender As Object, e As EventArgs) Handles btnBlackListHinzufügen.Click
        'Schreibt ein neues Tag in die Benutzer-Blacklist.
        'Altersfreigabe-Tags werden ausschließlich durch die RadioButtons verwaltet.

        Dim myTag As String
        Dim myMessageBox As frmMessageBildauswahl
        Dim myInputBox As frmInputBildauswahl

        myTag = String.Empty
        myMessageBox = Nothing
        myInputBox = New frmInputBildauswahl()

        Using myInputBox

            If myInputBox.ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            myTag =
            If(
                myInputBox.rueckgabeTag,
                String.Empty).
            Trim()

        End Using

        If String.IsNullOrWhiteSpace(myTag) Then
            Exit Sub
        End If

        If IstAltersfreigabeTag(myTag) Then

            MessageBox.Show(
            "Das Tag """ &
            myTag &
            """ wird intern durch die Altersfreigabe verwaltet." &
            Environment.NewLine &
            Environment.NewLine &
            "Bitte ändere die gewünschte Altersfreigabe über die dafür vorgesehenen Optionsfelder.",
            "Altersfreigabe",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information)

            Exit Sub

        End If

        If ListBoxEnthaeltTag(lstWhiteList, myTag) Then

            myMessageBox =
            New frmMessageBildauswahl(
                myTag,
                True)

            Using myMessageBox
                myMessageBox.ShowDialog()
            End Using

            Exit Sub

        End If

        If Not ListBoxEnthaeltTag(lstBlackList, myTag) Then

            lstBlackList.Items.Add(myTag)

        End If

        AktualisiereButtonStatus()

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
        'Initialisiert alle Steuerelemente anhand der aktuellen Settings.

        Dim item As String

        initialisierungLaeuft = True

        Try

            lstVerzeichnisse.Items.Clear()
            lstWhiteList.Items.Clear()
            lstBlackList.Items.Clear()

            'Verzeichnisse
            If aktuelleSettings.Verzeichnisse IsNot Nothing Then

                For Each item In aktuelleSettings.Verzeichnisse

                    If Not String.IsNullOrWhiteSpace(item) Then
                        lstVerzeichnisse.Items.Add(item)
                    End If

                Next

            End If

            'Whitelist
            If aktuelleSettings.WhiteListTags IsNot Nothing Then

                For Each item In aktuelleSettings.WhiteListTags

                    If Not String.IsNullOrWhiteSpace(item) AndAlso Not ListBoxEnthaeltTag(lstWhiteList, item) Then

                        lstWhiteList.Items.Add(item.Trim())

                    End If

                Next

            End If

            'Sichtbare Benutzer-Blacklist
            aktuelleSettings.BlackListTags = BereinigeBenutzerBlacklist(aktuelleSettings.BlackListTags)

            For Each item In aktuelleSettings.BlackListTags

                If Not String.IsNullOrWhiteSpace(item) AndAlso Not ListBoxEnthaeltTag(lstBlackList, item) Then

                    lstBlackList.Items.Add(item.Trim())

                End If

            Next

            'Bewertung
            sbcBewertung.Bewertung = aktuelleSettings.Bewertung

            sbcBewertung.EndInitialization()

            'Altersfreigabe
            Select Case aktuelleSettings.Altersfreigabe

                Case "18+"

                    rdo18.Checked = True

                Case "Akt"

                    rdoAkt.Checked = True

                Case "Lingerie"

                    rdoLingerie.Checked = True

                Case "Jugendfrei"

                    rdoJugendfrei.Checked = True

                Case Else

                    aktuelleSettings.Altersfreigabe = "Jugendfrei"
                    rdoJugendfrei.Checked = True

            End Select

            AktualisiereButtonStatus()

        Finally

            initialisierungLaeuft = False

        End Try

    End Sub

    Private Sub AktualisiereButtonStatus()
        'Aktualisiert die Aktivierung der Listenbuttons.

        btnVerzeichnisseLöschen.Enabled = (lstVerzeichnisse.SelectedIndex >= 0)
        btnVerzeichnisseListeLöschen.Enabled = (lstVerzeichnisse.Items.Count > 0)

        btnWhiteListLöschen.Enabled = (lstWhiteList.SelectedIndex >= 0)
        btnWhiteListListeLöschen.Enabled = (lstWhiteList.Items.Count > 0)

        btnBlackListLöschen.Enabled = (lstBlackList.SelectedIndex >= 0)
        btnBlackListListeLöschen.Enabled = (lstBlackList.Items.Count > 0)

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Lädt die Defaultwerte, aktualisiert die UI und speichert sie direkt.

        Dim defaults As Dictionary(Of String, String)

        defaults = GetBildauswahlDefaultSettings()

        aktuelleSettings.Verzeichnisse = SplitSemicolonList(defaults("Verzeichnisse"))
        aktuelleSettings.WhiteListTags = SplitSemicolonList(defaults("WhiteListTags"))
        aktuelleSettings.BlackListTags = BereinigeBenutzerBlacklist(SplitSemicolonList(defaults("BlackListTags")))
        aktuelleSettings.Altersfreigabe = defaults("Altersfreigabe")
        aktuelleSettings.Bewertung = CInt(defaults("Bewertung"))

        IniOrReinitialise()

        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Verzeichnisse", String.Join(";", aktuelleSettings.Verzeichnisse))
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "WhiteListTags", String.Join(";", aktuelleSettings.WhiteListTags))
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "BlackListTags", String.Join(";", aktuelleSettings.BlackListTags))
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe", aktuelleSettings.Altersfreigabe)
        WriteToRegistry(SLIDESHOWBILDAUSWAHL_PATH & "Bewertung", aktuelleSettings.Bewertung.ToString())

    End Sub
    Private Sub SpeichereAltersfreigabe(altersfreigabe As String)
        'Speichert die aktuelle Altersfreigabe im Settingsobjekt und in der Registry.

        aktuelleSettings.Altersfreigabe = altersfreigabe

        WriteToRegistry(
            SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe",
            aktuelleSettings.Altersfreigabe)

    End Sub

    Private Sub SetzeAltersfreigabeUndRadioButton(altersfreigabe As String)
        'Setzt die Altersfreigabe und den zugehörigen RadioButton konsistent.

        initialisierungLaeuft = True

        Try

            Select Case altersfreigabe

                Case "18+"

                    aktuelleSettings.Altersfreigabe = "18+"
                    rdo18.Checked = True

                Case "Akt"

                    aktuelleSettings.Altersfreigabe = "Akt"
                    rdoAkt.Checked = True

                Case "Lingerie"

                    aktuelleSettings.Altersfreigabe = "Lingerie"
                    rdoLingerie.Checked = True

                Case "Jugendfrei"

                    aktuelleSettings.Altersfreigabe = "Jugendfrei"
                    rdoJugendfrei.Checked = True

                Case Else

                    aktuelleSettings.Altersfreigabe = "Jugendfrei"
                    rdoJugendfrei.Checked = True

            End Select

        Finally

            initialisierungLaeuft = False

        End Try

        WriteToRegistry(
            SLIDESHOWBILDAUSWAHL_PATH & "Altersfreigabe",
            aktuelleSettings.Altersfreigabe)

    End Sub

    Private Function ListBoxEnthaeltTag(listBox As ListBox, tag As String) As Boolean
        'Prüft case-insensitiv, ob ein Tag bereits in einer ListBox enthalten ist.

        Dim item As Object
        Dim vorhandenesTag As String

        If listBox Is Nothing OrElse String.IsNullOrWhiteSpace(tag) Then

            Return False

        End If

        For Each item In listBox.Items

            vorhandenesTag = Convert.ToString(item)

            If String.Equals(
                vorhandenesTag,
                tag,
                StringComparison.OrdinalIgnoreCase) Then

                Return True

            End If

        Next

        Return False

    End Function

End Class

