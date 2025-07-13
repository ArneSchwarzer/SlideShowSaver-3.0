Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.RegistryHandling

Public Class ucOptionsModul

    'Variablendeklaration
    Private Shared aktuelleSettings As ModulMain.ModulSettings_Mandelbrot

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Inititalisiert das UC und seine Steuerelemente

        'Settings abholen und aufräumen
        aktuelleSettings = GetSettings(Of ModulMain.ModulSettings_Mandelbrot)(ModulMain.nameModul)
        ClearSettings(ModulMain.nameModul)

        'Steuerelemente initialisieren

        'cmbGradient bestücken
        cmbGradient.SelectedItem = aktuelleSettings.Farbverlauf

        'chkGradientAnimieren
        chkGradientAnimieren.Checked = aktuelleSettings.GradientAnimieren

        'chkKoordinaten Anzeigen
        chkKoordinatenAnzeigen.Checked = aktuelleSettings.KoordinatenAnzeigen

    End Sub

    Private Sub cmbGradient_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbGradient.SelectedIndexChanged
        'Behandelt cmbGradient

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Farbverlauf", cmbGradient.SelectedItem.ToString)

    End Sub

    Private Sub chkGradientAnimieren_CheckedChanged(sender As Object, e As EventArgs) Handles chkGradientAnimieren.CheckedChanged
        'Behandelt chkGradientAnimieren

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_MANDELBROT_FULLPATH & "GradientAnimieren", chkGradientAnimieren.Checked.ToString)

    End Sub

    Private Sub chkKoordinatenAnzeigen_CheckedChanged(sender As Object, e As EventArgs) Handles chkKoordinatenAnzeigen.CheckedChanged
        'Behandelt chkKoordinatenAnzeigen

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_MANDELBROT_FULLPATH & "KoordinatenAnzeigen", chkKoordinatenAnzeigen.Checked.ToString)

    End Sub
End Class
