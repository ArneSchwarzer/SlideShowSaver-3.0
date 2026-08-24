Imports System.Windows.Forms
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTransition.VomWindeVerweht.TransitionMain

Public Class ucOptionsTransition
    Inherits UserControl

#Region "Variablendeklaration"

    Private aktuelleSettings As TransitionSettings_VomWindeVerweht

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

#End Region

#Region "Initialisierung"

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert sämtliche Controls aus den aktuellen
        'Transitionseinstellungen.

        CheckYourMail()

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SettingsInbox.

        aktuelleSettings = GetSettings(Of TransitionSettings_VomWindeVerweht)(nameTransition)

    End Sub

    Private Sub IniOrReinitialise()
        'Überträgt aktuelleSettings auf sämtliche Controls.

        trkPartikelGroesse.Value = Math.Max(trkPartikelGroesse.Minimum, Math.Min(trkPartikelGroesse.Maximum,
                                            ErmittleTrackBarWertAusPartikelGroesse(aktuelleSettings.partikelGroesse)))

        chkPartikelGroesseZufall.Checked = aktuelleSettings.partikelGroesseZufall

        trkWindstaerke.Value = Math.Max(trkWindstaerke.Minimum, Math.Min(trkWindstaerke.Maximum,
                                        aktuelleSettings.windStaerke))

        chkWindstaerkeZufall.Checked = aktuelleSettings.windStaerkeZufall

        lblPartikelGroesse.Text = ErmittlePartikelGroesseAusTrackBar().ToString() & " px"

        lblWindstaerke.Text = "Bft " & aktuelleSettings.windStaerke.ToString()

        AktualisiereAbhaengigeControls()

    End Sub

    Private Sub AktualisiereAbhaengigeControls()
        'Aktualisiert sämtliche Controls, deren Zustand von
        'anderen Einstellungen abhängt.

        trkPartikelGroesse.Enabled = Not chkPartikelGroesseZufall.Checked
        lblPartikelGroesse.Enabled = Not chkPartikelGroesseZufall.Checked

        trkWindstaerke.Enabled = Not chkWindstaerkeZufall.Checked
        lblWindstaerke.Enabled = Not chkWindstaerkeZufall.Checked

        AktualisiereFlauteWarnung()

    End Sub

    Private Sub AktualisiereFlauteWarnung()
        'Warnt den Benutzer vor einer fest eingestellten
        'Windstärke von Bft 0.
        '
        'Bei zufälliger Windstärke ist die Warnung nicht nötig,
        'da der Zufallgenerator keine Flaute auswählen wird.
        If Not chkWindstaerkeZufall.Checked AndAlso trkWindstaerke.Value = 0 Then

            lblFlauteWarnung.Visible = True
            grpSchwerkraft.Top = 792
            lblNtrkDauerAbrisskante.Top = 687
            trkDauerAbrisskante.Top = 687
            lblDauerAbrisskante.Top = 687

        Else

            lblFlauteWarnung.Visible = False
            grpSchwerkraft.Top = 579
            lblNtrkDauerAbrisskante.Top = 474
            trkDauerAbrisskante.Top = 474
            lblDauerAbrisskante.Top = 474

        End If


    End Sub

    Private Function ErmittleTrackBarWertAusPartikelGroesse(partikelGroesse As Integer) As Integer

        Return CInt(Math.Round(Math.Log(partikelGroesse, 2)))

    End Function

    Private Function ErmittlePartikelGroesseAusTrackBar() As Integer

        Return 1 << trkPartikelGroesse.Value

    End Function

#End Region

#Region "Direct Commit"

    Private Sub trkPartikelGroesse_ValueChanged(sender As Object, e As EventArgs) Handles trkPartikelGroesse.ValueChanged
        'Speichert die gewählte Partikelgröße per Direct Commit.

        lblPartikelGroesse.Text = ErmittlePartikelGroesseAusTrackBar().ToString() & " px"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.partikelGroesse = ErmittlePartikelGroesseAusTrackBar()

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "PartikelGroesse",
                        aktuelleSettings.partikelGroesse.ToString())

    End Sub

    Private Sub chkPartikelGroesseZufall_CheckedChanged(sender As Object, e As EventArgs) _
        Handles chkPartikelGroesseZufall.CheckedChanged
        'Speichert den Zufallsmodus für die Partikelgröße
        'per Direct Commit.

        AktualisiereAbhaengigeControls()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.partikelGroesseZufall = chkPartikelGroesseZufall.Checked

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "PartikelGroesseZufall",
                        aktuelleSettings.partikelGroesseZufall.ToString())

    End Sub

    Private Sub trkWindstaerke_ValueChanged(sender As Object, e As EventArgs) Handles trkWindstaerke.ValueChanged
        'Speichert die gewählte Windstärke per Direct Commit.

        lblWindstaerke.Text = "Bft " & trkWindstaerke.Value.ToString()

        AktualisiereFlauteWarnung()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.windStaerke = trkWindstaerke.Value

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "WindStaerke",
                        aktuelleSettings.windStaerke.ToString())

    End Sub

    Private Sub chkWindstaerkeZufall_CheckedChanged(sender As Object, e As EventArgs) _
        Handles chkWindstaerkeZufall.CheckedChanged

        'Speichert den Zufallsmodus für die Windstärke
        'per Direct Commit.

        AktualisiereAbhaengigeControls()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.windStaerkeZufall = chkWindstaerkeZufall.Checked

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "WindStaerkeZufall",
                        aktuelleSettings.windStaerkeZufall.ToString())

    End Sub

#End Region

#Region "Defaultwerte"

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Stellt sämtliche Defaultwerte wieder her und speichert
        'sie gemäß Direct-Commit-Architektur explizit.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetTransitionDefaultSettings()

        aktuelleSettings.partikelGroesse = CInt(defaults("PartikelGroesse"))
        aktuelleSettings.partikelGroesseZufall = CBool(defaults("PartikelGroesseZufall"))
        aktuelleSettings.windStaerke = CInt(defaults("WindStaerke"))
        aktuelleSettings.windStaerkeZufall = CBool(defaults("WindStaerkeZufall"))

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "PartikelGroesse", defaults("PartikelGroesse"))
        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "PartikelGroesseZufall",
                        defaults("PartikelGroesseZufall"))
        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "WindStaerke", defaults("WindStaerke"))
        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "WindStaerkeZufall", defaults("WindStaerkeZufall"))

        wirdInitialisiert = True

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

#End Region

#Region "Bereinigung"

    Private Sub ucOptionsTransition_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert Direct-Commit-Aktionen nach der Freigabe.

        wurdeBereinigt = True

    End Sub

#End Region

End Class