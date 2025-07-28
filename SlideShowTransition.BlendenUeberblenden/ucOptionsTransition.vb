Imports System.Drawing
Imports System.Windows.Forms
Imports Transition_Überblenden.TransitionMain
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ucOptionsTransition
    Inherits UserControl

    'Variablendeklaration
    Private aktuelleSettings As TransitionSettings_FadeCrossfade

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente des ucOptionShader

        'Settings abholen
        CheckYourMail()

        'Farbton picFarbton setzen
        picFarbton.BackColor = aktuelleSettings.Farbton

        'chkZufallsfarbe setzen
        If aktuelleSettings.Zufallsfarbe Then
            chkZufallsfarbe.Checked = True
            picFarbton.BackColor = Color.Transparent
            lblNpicFarbton.Enabled = False
            picFarbton.Enabled = False
        Else
            chkZufallsfarbe.Checked = False
            picFarbton.BackColor = aktuelleSettings.Farbton
            lblNpicFarbton.Enabled = True
            picFarbton.Enabled = True
        End If

        'chkMorphing setzen
        If aktuelleSettings.Morphing Then
            chkMorphing.Checked = True
        Else
            chkMorphing.Checked = False
            chkZufallsfarbe.Enabled = False
            lblNpicFarbton.Enabled = False
            picFarbton.BackColor = Color.Black
            picFarbton.Enabled = False
        End If

        'Modus setzen
        Select Case aktuelleSettings.Modus
            Case "Blenden"
                rdoBlenden.Checked = True
            Case "Überblenden"
                rdoÜberblenden.Checked = True
                chkMorphing.Enabled = False
                chkZufallsfarbe.Enabled = False
                lblNpicFarbton.Enabled = False
                picFarbton.BackColor = Color.Transparent
                picFarbton.Enabled = False
            Case "Zufall"
                rdoZufall.Checked = True
        End Select

        'Geschwindigkeit
        trkGeschwindigkeit.Value = (trkGeschwindigkeit.Maximum + 1) - aktuelleSettings.geschwindigkeit
        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString & " s"

    End Sub

    Private Sub picFarbton_Click(sender As Object, e As EventArgs) Handles picFarbton.Click
        'Behandelt PictureBox Farbton

        Using dlg As New ColorDialog()

            dlg.Color = aktuelleSettings.Farbton
            dlg.AllowFullOpen = True
            dlg.AnyColor = True
            dlg.FullOpen = True

            If dlg.ShowDialog() = DialogResult.OK Then
                picFarbton.BackColor = dlg.Color
                aktuelleSettings.Farbton = dlg.Color
                WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Farbton", ColorToString(dlg.Color))
            End If
        End Using

    End Sub

    Private Sub rdoBlenden_CheckedChanged(sender As Object, e As EventArgs) Handles rdoBlenden.CheckedChanged
        'Behandelt RadioButton Blenden

        If rdoBlenden.Checked = True Then

            chkMorphing.Enabled = True

            If chkMorphing.Checked Then

                chkZufallsfarbe.Enabled = True

                If chkZufallsfarbe.Checked Then
                    picFarbton.BackColor = Color.Transparent
                    lblNpicFarbton.Enabled = False
                    picFarbton.Enabled = False
                Else
                    picFarbton.BackColor = aktuelleSettings.Farbton
                    lblNpicFarbton.Enabled = True
                    picFarbton.Enabled = True
                End If

            Else
                chkZufallsfarbe.Enabled = False
                lblNpicFarbton.Enabled = False
                picFarbton.BackColor = Color.Black
                picFarbton.Enabled = False
            End If

            WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", "Fade")
        End If

    End Sub

    Private Sub rdoÜberblenden_CheckedChanged(sender As Object, e As EventArgs) Handles rdoÜberblenden.CheckedChanged
        'Behandelt RadioButton Färben

        If rdoÜberblenden.Checked = True Then

            chkMorphing.Enabled = False
            chkZufallsfarbe.Enabled = False
            lblNpicFarbton.Enabled = False
            picFarbton.BackColor = Color.Transparent
            picFarbton.Enabled = False

            WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", "Crossfade")
        End If

    End Sub

    Private Sub rbZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rdoZufall.CheckedChanged
        'Behandelt Radiobutton Zufall

        If rdoZufall.Checked = True Then

            chkMorphing.Enabled = True

            If chkMorphing.Checked Then

                chkZufallsfarbe.Enabled = True

                If chkZufallsfarbe.Checked Then
                    picFarbton.BackColor = Color.Transparent
                    lblNpicFarbton.Enabled = False
                    picFarbton.Enabled = False
                Else
                    picFarbton.BackColor = aktuelleSettings.Farbton
                    lblNpicFarbton.Enabled = True
                    picFarbton.Enabled = True
                End If

            Else
                chkZufallsfarbe.Enabled = False
                lblNpicFarbton.Enabled = False
                picFarbton.BackColor = Color.Black
                picFarbton.Enabled = False
            End If

            WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", "Zufall")
        End If

    End Sub

    Private Sub chkZufallsfarbe_CheckedChanged(sender As Object, e As EventArgs) Handles chkZufallsfarbe.CheckedChanged
        'Behandelt Checkbox Zufallsfarbe

        ' BeginInvoke sorgt dafür, dass der Code erst ausgeführt wird,
        ' nachdem der Checked-Zustand aktualisiert wurde
        BeginInvoke(Sub()
                        If chkZufallsfarbe.Checked = True Then
                            picFarbton.BackColor = Color.Transparent
                            lblNpicFarbton.Enabled = False
                            picFarbton.Enabled = False
                        Else
                            picFarbton.BackColor = aktuelleSettings.Farbton
                            lblNpicFarbton.Enabled = True
                            picFarbton.Enabled = True
                        End If

                        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Zufallsfarbe", chkZufallsfarbe.Checked.ToString)
                    End Sub)

    End Sub

    Private Sub trkGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkGeschwindigkeit.ValueChanged
        'TrackBar Geschwindigkeit

        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString & " s"

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Geschwindigkeit", ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString)

    End Sub

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox aus

        aktuelleSettings = GetSettings(Of TransitionSettings_FadeCrossfade)(nameTransition)

    End Sub

    Private Sub chkMorphing_CheckedChanged(sender As Object, e As EventArgs) Handles chkMorphing.CheckedChanged
        BeginInvoke(Sub()
                        If chkMorphing.Checked Then

                            chkZufallsfarbe.Enabled = True

                            If chkZufallsfarbe.Checked Then
                                picFarbton.BackColor = Color.Transparent
                                lblNpicFarbton.Enabled = False
                                picFarbton.Enabled = False
                            Else
                                picFarbton.BackColor = aktuelleSettings.Farbton
                                lblNpicFarbton.Enabled = True
                                picFarbton.Enabled = True
                            End If

                        Else
                            chkZufallsfarbe.Enabled = False
                            lblNpicFarbton.Enabled = False
                            picFarbton.BackColor = Color.Black
                            picFarbton.Enabled = False
                        End If

                        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Morphing", chkMorphing.Checked.ToString)

                    End Sub)
    End Sub
End Class
