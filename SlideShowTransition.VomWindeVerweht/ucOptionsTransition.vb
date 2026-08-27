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

        Select Case aktuelleSettings.partikelGroessenModus

            Case "Zufällig"

                rbZufallsFestePGroesse.Checked = True

            Case "Gemischt"

                rbGemischtePartikel.Checked = True

            Case "Zufallsmodus"

                rbZufallsPGroessenModus.Checked = True

            Case Else

                rbManuellePGroesse.Checked = True

        End Select

        lblPartikelGroesse.Text = ErmittlePartikelGroesseAusTrackBar().ToString() & " px"

        trkWindstaerke.Value = Math.Max(trkWindstaerke.Minimum, Math.Min(trkWindstaerke.Maximum,
                                        aktuelleSettings.windStaerke))

        chkWindstaerkeZufall.Checked = aktuelleSettings.windStaerkeZufall

        lblWindstaerke.Text = "Bft " & aktuelleSettings.windStaerke.ToString()

        trkDauerAbrisskante.Value = Math.Max(trkDauerAbrisskante.Minimum, Math.Min(trkDauerAbrisskante.Maximum,
                                             aktuelleSettings.dauerAbrisskante))

        lblDauerAbrisskante.Text = aktuelleSettings.dauerAbrisskante.ToString() & " s"

        Select Case aktuelleSettings.schwerkraftModus
            Case "An"
                rbSchwerkraftAn.Checked = True
            Case "Aus"
                rbSchwerkraftAus.Checked = True
            Case Else
                rbSchwerkraftZufällig.Checked = True
        End Select

        AktualisiereAbhaengigeControls()

    End Sub

    Private Sub AktualisiereAbhaengigeControls()
        'Aktualisiert sämtliche Controls, deren Zustand von
        'anderen Einstellungen abhängt.

        trkPartikelGroesse.Enabled = rbManuellePGroesse.Checked
        lblPartikelGroesse.Enabled = rbManuellePGroesse.Checked
        lblNtrkParikelGroesse.Enabled = rbManuellePGroesse.Checked

        trkWindstaerke.Enabled = Not chkWindstaerkeZufall.Checked
        lblWindstaerke.Enabled = Not chkWindstaerkeZufall.Checked

        AktualisiereFlauteWarnung()

    End Sub

    Private Sub AktualisiereFlauteWarnung()
        'Warnt den Benutzer vor einer fest eingestellten
        'Windstärke von Bft 0 (Flaute).
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

    Private Sub trkPartikelGroesse_ValueChanged(sender As Object, e As EventArgs)
        'Speichert die gewählte Partikelgröße per Direct Commit.

        lblPartikelGroesse.Text = ErmittlePartikelGroesseAusTrackBar().ToString() & " px"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.partikelGroesse = ErmittlePartikelGroesseAusTrackBar()

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "PartikelGroesse",
                        aktuelleSettings.partikelGroesse.ToString())

    End Sub

    Private Sub PartikelGroessenModus_CheckedChanged(sender As Object, e As EventArgs) _
    Handles rbManuellePGroesse.CheckedChanged,
            rbZufallsFestePGroesse.CheckedChanged,
            rbGemischtePartikel.CheckedChanged,
            rbZufallsPGroessenModus.CheckedChanged

        AktualisiereAbhaengigeControls()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        If rbManuellePGroesse.Checked Then

            aktuelleSettings.partikelGroessenModus = "Manuell"

        ElseIf rbZufallsFestePGroesse.Checked Then

            aktuelleSettings.partikelGroessenModus = "Zufällig"

        ElseIf rbGemischtePartikel.Checked Then

            aktuelleSettings.partikelGroessenModus = "Gemischt"

        ElseIf rbZufallsPGroessenModus.Checked Then

            aktuelleSettings.partikelGroessenModus = "Zufallsmodus"

        Else

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "PartikelGroessenModus",
                        aktuelleSettings.partikelGroessenModus)

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

    Private Sub trkDauerAbrisskante_ValueChanged(sender As Object, e As EventArgs) _
        Handles trkDauerAbrisskante.ValueChanged

        lblDauerAbrisskante.Text = trkDauerAbrisskante.Value.ToString() & " s"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.dauerAbrisskante = trkDauerAbrisskante.Value

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "DauerAbrisskante",
                        aktuelleSettings.dauerAbrisskante.ToString())

    End Sub

    Private Sub Schwerkraft_CheckedChanged(sender As Object, e As EventArgs) _
    Handles rbSchwerkraftAn.CheckedChanged,
            rbSchwerkraftAus.CheckedChanged,
            rbSchwerkraftZufällig.CheckedChanged

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        If rbSchwerkraftAn.Checked Then

            aktuelleSettings.schwerkraftModus = "An"

        ElseIf rbSchwerkraftAus.Checked Then

            aktuelleSettings.schwerkraftModus = "Aus"

        ElseIf rbSchwerkraftZufällig.Checked Then

            aktuelleSettings.schwerkraftModus = "Zufällig"

        Else

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "SchwerkraftModus",
                    aktuelleSettings.schwerkraftModus)

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
        aktuelleSettings.partikelGroessenModus = defaults("PartikelGroessenModus")
        aktuelleSettings.windStaerke = CInt(defaults("WindStaerke"))
        aktuelleSettings.windStaerkeZufall = CBool(defaults("WindStaerkeZufall"))
        aktuelleSettings.dauerAbrisskante = CInt(defaults("DauerAbrisskante"))
        aktuelleSettings.schwerkraftModus = defaults("SchwerkraftModus")


        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "PartikelGroesse",
                        defaults("PartikelGroesse"))
        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "PartikelGroessenModus",
                        defaults("PartikelGroessenModus"))
        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "WindStaerke", defaults("WindStaerke"))
        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "WindStaerkeZufall",
                        defaults("WindStaerkeZufall"))
        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "DauerAbrisskante",
                        defaults("DauerAbrisskante"))
        WriteToRegistry(SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH & "SchwerkraftModus",
                        defaults("SchwerkraftModus"))

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