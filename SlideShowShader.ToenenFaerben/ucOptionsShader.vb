Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowShader.TönenFärben.ShaderMain
Imports System.Diagnostics.Eventing.Reader

Public Class ucOptionsShader
    Inherits UserControl

    'Variablendeklaration
    Private aktuelleSettings As ShaderSettings

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente des ucOptionShader

        Dim defaults As New Dictionary(Of String, String)
        Dim registryTempWert As String

        defaults = ShaderMain.GetShaderDefaultSettings()

        'Farbton picFarbton setzen
        picFarbton.BackColor = StringToColor(ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Farbton", defaults))
        aktuelleSettings.Farbton = picFarbton.BackColor

        'chkZufallsfarbe setzen
        If ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Zufallsfarbe", defaults) = "True" Then
            chkZufallsfarbe.Checked = True
            lblNpicFarbton.Enabled = False
            picFarbton.Enabled = False
        Else
            chkZufallsfarbe.Checked = False
            lblNpicFarbton.Enabled = True
            picFarbton.Enabled = True
        End If

        'trbIntensität setzen
        trbIntensität.Value = aktuelleSettings.Intensitaet
        aktuelleSettings.Intensitaet = trbIntensität.Value
        lblIntensität.Text = trbIntensität.Value & " %"

        'Modus setzen
        registryTempWert = ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Modus", defaults)
        Select Case registryTempWert
            Case "Tönen"
                rbTönen.Checked = True
                aktuelleSettings.Modus = ShaderModus.Toenen
            Case "Färben"
                rbFärben.Checked = True
                aktuelleSettings.Modus = ShaderModus.Faerben
            Case "Zufall"
                rbZufall.Checked = True
                aktuelleSettings.Modus = ShaderModus.Zufaellig
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
                aktuelleSettings.Farbton = dlg.Color
                WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Farbton", ColorToString(dlg.Color))
            End If
        End Using

    End Sub

    Private Sub trbIntensität_ValueChanged(sender As Object, e As EventArgs) Handles trbIntensität.ValueChanged
        'Behandelt Trackbar Intensität

        lblIntensität.Text = trbIntensität.Value & " %"
        aktuelleSettings.Intensitaet = trbIntensität.Value
        WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Intensität", trbIntensität.Value.ToString)

    End Sub

    Private Sub rbTönen_CheckedChanged(sender As Object, e As EventArgs) Handles rbTönen.CheckedChanged
        'Behandelt RadioButton Tönen

        If rbTönen.Checked = True Then
            aktuelleSettings.Modus = ShaderModus.Toenen
            WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Modus", "Tönen")
        End If

    End Sub

    Private Sub rbFärben_CheckedChanged(sender As Object, e As EventArgs) Handles rbFärben.CheckedChanged
        'Behandelt RadioButton Färben

        If rbFärben.Checked = True Then
            aktuelleSettings.Modus = ShaderModus.Faerben
            WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Modus", "Färben")
        End If

    End Sub

    Private Sub rbZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rbZufall.CheckedChanged
        'Behandelt Radiobutton Zufall

        If rbZufall.Checked = True Then
            aktuelleSettings.Modus = ShaderModus.Zufaellig
            WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Modus", "Zufall")
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

        WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Zufallsfarbe", chkZufallsfarbe.Checked.ToString)

    End Sub

End Class
