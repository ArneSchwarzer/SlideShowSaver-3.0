Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTransition.GradientWischen.TransitionMain_GradientWischen.TransitionMain
Imports System.ComponentModel

Public Class ucOptionsTransition
    Inherits System.Windows.Forms.UserControl

    'Variablendeklaration
    Private aktuelleSettings As SlideShowTransitionSettings_GradientWischen

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente der ucOptionsTransition

        'Aktuelle Settings abholen
        CheckYourMail()

        'Steuerelemente initialisieren

        'Ankerpunkte
        tbtN.Checked = False
        tbtNO.Checked = False
        tbtO.Checked = False
        tbtSO.Checked = False
        tbtS.Checked = False
        tbtSW.Checked = False
        tbtW.Checked = False
        tbtNW.Checked = False
        tbtZOut.Checked = False
        tbtZIn.Checked = False

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
                Case "ZOut"
                    tbtZOut.Checked = True
                Case "ZIn"
                    tbtZIn.Checked = True
            End Select

        Next

        'Label "Kein Ankerpunkt ausgewählt"
        If aktuelleSettings.richtungen.Count = 0 Then
            lblKeineRichtung.Visible = True
        Else
            lblKeineRichtung.Visible = False
        End If

        'Breite
        trkBreite.Value = aktuelleSettings.breite

        'Geschwindigkeit
        trkGeschwindigkeit.Value = (trkGeschwindigkeit.Maximum + 1) - aktuelleSettings.geschwindigkeit
        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString & " s"

    End Sub

    Private Sub tbtNW_CheckChanged(sender As Object, e As EventArgs) Handles tbtNW.CheckChanged
        'Button Nord-West

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtN_CheckChanged(sender As Object, e As EventArgs) Handles tbtN.CheckChanged
        'Button Nord

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtNO_CheckChanged(sender As Object, e As EventArgs) Handles tbtNO.CheckChanged
        'Button Nord-Ost

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtO_CheckChanged(sender As Object, e As EventArgs) Handles tbtO.CheckChanged
        'Button Ost

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtSO_CheckChanged(sender As Object, e As EventArgs) Handles tbtSO.CheckChanged
        'Button Süd-Ost

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtS_CheckChanged(sender As Object, e As EventArgs) Handles tbtS.CheckChanged
        'Button Süd

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtSW_CheckChanged(sender As Object, e As EventArgs) Handles tbtSW.CheckChanged
        'Button Süd-West

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtW_CheckChanged(sender As Object, e As EventArgs) Handles tbtW.CheckChanged
        'Button West

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtZOut_CheckChanged(sender As Object, e As EventArgs) Handles tbtZOut.CheckChanged
        'Button Zentrum nach Außen

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub tbtZIn_CheckChanged(sender As Object, e As EventArgs) Handles tbtZIn.CheckChanged
        'Button Zentrum nach Außen

        'DirectCommit
        RichtungStringBauen()

    End Sub

    Private Sub trkGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkGeschwindigkeit.ValueChanged
        'TrackBar Geschwindigkeit

        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString & " s"

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Geschwindigkeit", ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString)

    End Sub

    Private Sub trkBreite_ValueChanged(sender As Object, e As EventArgs) Handles trkBreite.ValueChanged
        'TrackBar Breite

        'Direct Commit
        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Breite", trkBreite.Value.ToString)

    End Sub

    Private Sub RichtungStringBauen()
        'Setzt den Richtungsstring zusammen, speichert ihn in die Registry und setzt auch gleich das
        'lblanker entsprechend.

        Dim richtungsString As String = ""
        Dim richtung As New List(Of String)

        If tbtN.Checked Then richtung.Add("N")
        If tbtNO.Checked Then richtung.Add("NO")
        If tbtO.Checked Then richtung.Add("O")
        If tbtSO.Checked Then richtung.Add("SO")
        If tbtS.Checked Then richtung.Add("S")
        If tbtSW.Checked Then richtung.Add("SW")
        If tbtW.Checked Then richtung.Add("W")
        If tbtNW.Checked Then richtung.Add("NW")
        If tbtZOut.Checked Then richtung.Add("ZOut")
        If tbtZIn.Checked Then richtung.Add("ZIn")


        richtungsString = String.Join(";", richtung)

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Richtungen", richtungsString)

        'Label ein- oder ausschalten
        If richtung.Count = 0 Then
            lblKeineRichtung.Visible = True
        Else
            lblKeineRichtung.Visible = False
        End If

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SetttingsInbox

        aktuelleSettings = GetSettings(Of SlideShowTransitionSettings_GradientWischen)(nameTransition)

    End Sub

End Class
