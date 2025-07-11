Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowShader.TönenFärben.ShaderMain

Public Class ucOptionsShader
    Inherits UserControl

    'Variablendeklaration
    Private aktuelleSettings As ShaderSettings

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim defaults As New Dictionary(Of String, String)
        Dim registryTempWert As String

        defaults = ShaderMain.GetShaderDefaultSettings()

        'Farbton picFarbton setzen
        picFarbton.BackColor = StringToColor(ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Farbton", defaults))
        aktuelleSettings.Farbton = picFarbton.BackColor

        'trbIntensität setzen
        trbIntensität.Value = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Intensität", defaults))
        aktuelleSettings.Intensitaet = trbIntensität.Value

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

        lblInensität.Text = trbIntensität.Value & " %"
        aktuelleSettings.Intensitaet = trbIntensität.Value
        WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Intensität", trbIntensität.Value.ToString)

    End Sub

    Private Sub rbTönen_CheckedChanged(sender As Object, e As EventArgs) Handles rbTönen.CheckedChanged

        If rbTönen.Checked = True Then
            aktuelleSettings.Modus = ShaderModus.Toenen
            WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Modus", "Tönen")
        End If

    End Sub

    Private Sub rbFärben_CheckedChanged(sender As Object, e As EventArgs) Handles rbFärben.CheckedChanged

        If rbFärben.Checked = True Then
            aktuelleSettings.Modus = ShaderModus.Faerben
            WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Modus", "Färben")
        End If

    End Sub

    Private Sub rbZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rbZufall.CheckedChanged

        If rbZufall.Checked = True Then
            aktuelleSettings.Modus = ShaderModus.Zufaellig
            WriteToRegistry(SLIDESHOWSHADER_FULLPATH & "Modus", "Zufall")
        End If

    End Sub

End Class
