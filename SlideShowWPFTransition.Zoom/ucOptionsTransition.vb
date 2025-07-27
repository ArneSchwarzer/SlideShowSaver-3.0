Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowWPFTransition.Zoom.TransitionMain

Public Class ucOptionsTransition
    Inherits System.Windows.Forms.UserControl

    'Variablendeklaration
    Private aktuelleSettings As SlideShowTransitionSettings_Zoom

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
        tbtZ.Checked = False

        For Each ankerpunkt In aktuelleSettings.ankerpunkte

            Select Case ankerpunkt
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
                Case "Z"
                    tbtZ.Checked = True
            End Select

        Next

        'Label "Kein Ankerpunkt ausgewählt"
        If aktuelleSettings.ankerpunkte.Count = 0 Then
            lblKeinAnkerpunkt.Visible = True
        Else
            lblKeinAnkerpunkt.Visible = False
        End If

        'Geschwindigkeit
        trkGeschwindigkeit.Value = (trkGeschwindigkeit.Maximum + 1) - aktuelleSettings.geschwindigkeit
        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString & " s"

    End Sub

    Private Sub tbtNW_CheckChanged(sender As Object, e As EventArgs) Handles tbtNW.CheckChanged
        'Button Nord-West

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub tbtN_CheckChanged(sender As Object, e As EventArgs) Handles tbtN.CheckChanged
        'Button Nord

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub tbtNO_CheckChanged(sender As Object, e As EventArgs) Handles tbtNO.CheckChanged
        'Button Nord-Ost

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub tbtO_CheckChanged(sender As Object, e As EventArgs) Handles tbtO.CheckChanged
        'Button Ost

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub tbtSO_CheckChanged(sender As Object, e As EventArgs) Handles tbtSO.CheckChanged
        'Button Süd-Ost

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub tbtS_CheckChanged(sender As Object, e As EventArgs) Handles tbtS.CheckChanged
        'Button Süd

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub tbtSW_CheckChanged(sender As Object, e As EventArgs) Handles tbtSW.CheckChanged
        'Button Süd-West

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub tbtW_CheckChanged(sender As Object, e As EventArgs) Handles tbtW.CheckChanged
        'Button West

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub tbtZ_CheckChanged(sender As Object, e As EventArgs) Handles tbtZ.CheckChanged
        'Button West

        'DirectCommit
        AnkerStringBauen()

    End Sub

    Private Sub trkGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkGeschwindigkeit.ValueChanged
        'TrackBar Geschwindigkeit

        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString & " s"

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Geschwindigkeit", ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString)

    End Sub

    Private Sub AnkerStringBauen()
        'Setzt den Richtungsstring zusammen, speichert ihn in die Registry und setzt auch gleich das
        'lblanker entsprechend.

        Dim ankerString As String = ""
        Dim anker As New List(Of String)

        If tbtN.Checked Then anker.Add("N")
        If tbtNO.Checked Then anker.Add("NO")
        If tbtO.Checked Then anker.Add("O")
        If tbtSO.Checked Then anker.Add("SO")
        If tbtS.Checked Then anker.Add("S")
        If tbtSW.Checked Then anker.Add("SW")
        If tbtW.Checked Then anker.Add("W")
        If tbtNW.Checked Then anker.Add("NW")
        If tbtZ.Checked Then anker.Add("Z")

        ankerString = String.Join(";", anker)

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Ankerpunkte", ankerString)

        'Label ein- oder ausschalten
        If anker.Count = 0 Then
            lblKeinAnkerpunkt.Visible = True
        Else
            lblKeinAnkerpunkt.Visible = False
        End If

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SetttingsInbox

        aktuelleSettings = GetSettings(Of SlideShowTransitionSettings_Zoom)(nameTransition)

    End Sub

End Class
