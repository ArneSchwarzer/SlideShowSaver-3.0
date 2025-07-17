Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports Transition_Schieben.TransitionMain

Public Class ucOptionsTransition

    'Variablendeklaration
    Private aktuelleSettings As SlideShowTransitionSettings_SuW

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente der ucOptionsTransition

        'Aktuelle Settings abholen
        CheckYourMail()

        'Steuerelemente initialisieren

        'Richtungsbuttons
        tbtN.Checked = False
        tbtNO.Checked = False
        tbtO.Checked = False
        tbtSO.Checked = False
        tbtS.Checked = False
        tbtSW.Checked = False
        tbtW.Checked = False
        tbtNW.Checked = False

        For Each richtung In aktuelleSettings.richtungen

            Select Case richtung
                Case "N"
                    tbtN.Checked = True
                Case "NO"
                    tbtNO.Checked = True
                Case "O"
                    tbtO.Checked = True
                Case "SO"
                    tbtSO.Checked = True
                Case "S"
                    tbtS.Checked = True
                Case "SW"
                    tbtSW.Checked = True
                Case "W"
                    tbtW.Checked = True
                Case "NW"
                    tbtNW.Checked = True
            End Select

        Next

        'Label "Keine Richtung Ausgewählt"
        If aktuelleSettings.richtungen.Count = 0 Then
            lblKeineRichtungInfo.Visible = True
        Else
            lblKeineRichtungInfo.Visible = False
        End If

        'Geschwindigkeit
        trkGeschwindigkeit.Value = aktuelleSettings.geschwindigkeit
        lblGeschwindigkeit.Text = (11 - trkGeschwindigkeit.Value).ToString & " s"

        'FPS
        nudFPS.Value = aktuelleSettings.FPS

        'Modus
        Select Case aktuelleSettings.modus
            Case "Schieben"
                rbSchieben.Checked = True
            Case "Wischen"
                rbWischen.Checked = True
            Case "Zufällig"
                rbZufall.Checked = True
        End Select

    End Sub

    Private Sub tbtNW_CheckChanged(sender As Object, e As EventArgs) Handles tbtNW.CheckChanged
        'Button Nord-West

        'DirectCommit
        RichtungsStringBauen()

    End Sub

    Private Sub tbtN_CheckChanged(sender As Object, e As EventArgs) Handles tbtN.CheckChanged
        'Button Nord

        'DirectCommit
        RichtungsStringBauen()

    End Sub

    Private Sub tbtNO_CheckChanged(sender As Object, e As EventArgs) Handles tbtNO.CheckChanged
        'Button Nord-Ost

        'DirectCommit
        RichtungsStringBauen()

    End Sub

    Private Sub tbtO_CheckChanged(sender As Object, e As EventArgs) Handles tbtO.CheckChanged
        'Button Ost

        'DirectCommit
        RichtungsStringBauen()

    End Sub

    Private Sub tbtSO_CheckChanged(sender As Object, e As EventArgs) Handles tbtSO.CheckChanged
        'Button Süd-Ost

        'DirectCommit
        RichtungsStringBauen()

    End Sub

    Private Sub tbtS_CheckChanged(sender As Object, e As EventArgs) Handles tbtS.CheckChanged
        'Button Süd

        'DirectCommit
        RichtungsStringBauen()

    End Sub

    Private Sub tbtSW_CheckChanged(sender As Object, e As EventArgs) Handles tbtSW.CheckChanged
        'Button Süd-West

        'DirectCommit
        RichtungsStringBauen()

    End Sub

    Private Sub tbtW_CheckChanged(sender As Object, e As EventArgs) Handles tbtW.CheckChanged
        'Button West

        'DirectCommit
        RichtungsStringBauen()

    End Sub

    Private Sub rbSchieben_CheckedChanged(sender As Object, e As EventArgs) Handles rbSchieben.CheckedChanged
        'RadioButton Schieben

        If rbSchieben.Checked Then
            'DirectCommit
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Schieben")
        End If

    End Sub

    Private Sub rbWischen_CheckedChanged(sender As Object, e As EventArgs) Handles rbWischen.CheckedChanged
        'RadioButton Wischen

        If rbWischen.Checked Then
            'DirectCommit
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Wischen")
        End If

    End Sub

    Private Sub rbZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rbZufall.CheckedChanged
        'RadioButton Zufall

        If rbZufall.Checked Then
            'DirectCommit
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Zufällig")
        End If

    End Sub

    Private Sub trkGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkGeschwindigkeit.ValueChanged
        'TrackBar Geschwindigkeit

        lblGeschwindigkeit.Text = (11 - trkGeschwindigkeit.Value).ToString & " s"

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", trkGeschwindigkeit.Value.ToString)

    End Sub

    Private Sub RichtungsStringBauen()
        'Setzt den Richtungsstring zusammen, speichert ihn in die Registry und setzt auch gleich das
        'lblRichtungen entsprechend.

        Dim richtungsString As String = ""
        Dim richtungen As New List(Of String)

        If tbtN.Checked Then richtungen.Add("N")
        If tbtNO.Checked Then richtungen.Add("NO")
        If tbtO.Checked Then richtungen.Add("O")
        If tbtSO.Checked Then richtungen.Add("SO")
        If tbtS.Checked Then richtungen.Add("S")
        If tbtSW.Checked Then richtungen.Add("SW")
        If tbtW.Checked Then richtungen.Add("W")
        If tbtNW.Checked Then richtungen.Add("NW")

        richtungsString = String.Join(";", richtungen)

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", richtungsString)

        'Label ein- oder ausschalten
        If richtungen.Count = 0 Then
            lblKeineRichtungInfo.Visible = True
        Else
            lblKeineRichtungInfo.Visible = False
        End If

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SetttingsInbox

        aktuelleSettings = GetSettings(Of SlideShowTransitionSettings_SuW)(nameTransition)

    End Sub

    Private Sub nudFPS_ValueChanged(sender As Object, e As EventArgs) Handles nudFPS.ValueChanged
        'Behandelt den NumericUpAndDown FPS

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "FPS", nudFPS.Value.ToString)

    End Sub
End Class
