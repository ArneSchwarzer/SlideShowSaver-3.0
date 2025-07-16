Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowShader.TönenFärben.ShaderMain
Imports System.Diagnostics.Eventing.Reader

Public Class ucOptionsShader
    Inherits UserControl

    'Variablendeklaration
    Private aktuelleSettings As ShaderSettings_ToenenFaerben

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente des ucOptionShader

        'Settings abholen
        CheckYourMail()

        'Farbton picFarbton setzen
        picFarbton.BackColor = aktuelleSettings.Farbton

        'chkZufallsfarbe setzen
        If aktuelleSettings.Zufallsfarbe Then
            chkZufallsfarbe.Checked = True
            lblNpicFarbton.Enabled = False
            picFarbton.Enabled = False
        Else
            chkZufallsfarbe.Checked = False
            lblNpicFarbton.Enabled = True
            picFarbton.Enabled = True
        End If

        'trbIntensität setzen
        trkIntensität.Value = aktuelleSettings.Intensitaet
        lblIntensität.Text = trkIntensität.Value & " %"

        'Modus setzen
        Select Case aktuelleSettings.Modus
            Case ShaderModus.Toenen
                rbTönen.Checked = True
            Case ShaderModus.Faerben
                rbFärben.Checked = True
            Case ShaderModus.Zufaellig
                rbZufall.Checked = True
        End Select

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
                WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Farbton", ColorToString(dlg.Color))
            End If
        End Using

    End Sub

    Private Sub trkIntensität_ValueChanged(sender As Object, e As EventArgs) Handles trkIntensität.ValueChanged
        'Behandelt Trackbar Intensität

        lblIntensität.Text = trkIntensität.Value & " %"
        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Intensität", trkIntensität.Value.ToString)

    End Sub

    Private Sub rbTönen_CheckedChanged(sender As Object, e As EventArgs) Handles rbTönen.CheckedChanged
        'Behandelt RadioButton Tönen

        If rbTönen.Checked = True Then
            WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Modus", "Tönen")
        End If

    End Sub

    Private Sub rbFärben_CheckedChanged(sender As Object, e As EventArgs) Handles rbFärben.CheckedChanged
        'Behandelt RadioButton Färben

        If rbFärben.Checked = True Then
            WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Modus", "Färben")
        End If

    End Sub

    Private Sub rbZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rbZufall.CheckedChanged
        'Behandelt Radiobutton Zufall

        If rbZufall.Checked = True Then
            WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Modus", "Zufall")
        End If

    End Sub

    Private Sub chkZufallsfarbe_Leave(sender As Object, e As EventArgs) Handles chkZufallsfarbe.Leave
        'Behandelt Checkbox Zufallsfarbe

        If chkZufallsfarbe.Checked = True Then
            lblNpicFarbton.Enabled = False
            picFarbton.Enabled = False
        Else
            lblNpicFarbton.Enabled = True
            picFarbton.Enabled = True
        End If

        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Zufallsfarbe", chkZufallsfarbe.Checked.ToString)

    End Sub

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox aus

        aktuelleSettings = GetSettings(Of ShaderSettings_ToenenFaerben)(nameShader)

    End Sub
End Class
