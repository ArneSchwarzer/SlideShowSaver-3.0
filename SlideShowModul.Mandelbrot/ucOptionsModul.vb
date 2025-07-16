Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.RegistryHandling
Imports Modul_Mandelbrot.ModulMain

Public Class ucOptionsModul

#Region "Variablendeklaration"
    'Variablendeklaration
    Private Shared aktuelleSettings As ModulSettings_Mandelbrot
#End Region

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Inititalisiert das UC und seine Steuerelemente

        'Settings abholen
        CheckYourMail()

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
        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Farbverlauf", cmbGradient.SelectedItem.ToString)

    End Sub

    Private Sub chkGradientAnimieren_CheckedChanged(sender As Object, e As EventArgs) Handles chkGradientAnimieren.CheckedChanged
        'Behandelt chkGradientAnimieren

        'DirectCommit
        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "GradientAnimieren", chkGradientAnimieren.Checked.ToString)

    End Sub

    Private Sub chkKoordinatenAnzeigen_CheckedChanged(sender As Object, e As EventArgs) Handles chkKoordinatenAnzeigen.CheckedChanged
        'Behandelt chkKoordinatenAnzeigen

        'DirectCommit
        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "KoordinatenAnzeigen", chkKoordinatenAnzeigen.Checked.ToString)

    End Sub

    Private Sub CheckYourMail()
        'Initialisiert die aktuelleSettings des UCs per SlideShowTools.SettingsHandling.SettingsInbox.

        aktuelleSettings = GetSettings(Of ModulSettings_Mandelbrot)(nameModul)

    End Sub

End Class
