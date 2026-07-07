Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowWPFModul.Mandelbrot.ModulMain
Imports System.Windows.Forms
Imports SlideShowLogging
Imports SlideShowTools

Public Class ucOptionsModul

#Region "Variablendeklaration"
    'Variablendeklaration
    Private Shared aktuelleSettings As ModulSettings_Mandelbrot

#End Region

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load

        CheckYourMail()

        clbGradienten.Items.Clear()

        Dim alleGradienten As String() = {
        "Regenbogen",
        "Zebra",
        "Joker",
        "Wakanda",
        "Weihnachten"
    }

        Dim aktiveGradienten As New HashSet(Of String)(
        aktuelleSettings.Gradienten,
        StringComparer.OrdinalIgnoreCase)

        For Each gradientName As String In alleGradienten
            clbGradienten.Items.Add(gradientName, aktiveGradienten.Contains(gradientName))
        Next

        If clbGradienten.CheckedItems.Count = 0 AndAlso clbGradienten.Items.Count > 0 Then
            clbGradienten.SetItemCheckState(0, CheckState.Checked)
        End If

        chkGradientAnimieren.Checked = aktuelleSettings.GradientAnimieren
        chkKoordinatenAnzeigen.Checked = aktuelleSettings.KoordinatenAnzeigen

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

    Private Sub clbGradienten_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbGradienten.ItemCheck
        Try
            BeginInvoke(Sub()
                            'DirectCommit
                            CheckedListBoxHandling.SaveListBoxToRegistry(clbGradienten, ModulMain.SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Gradienten")

                        End Sub)
        Catch ex As Exception
            LogHandling.LogWarn("SSS 3.0: ucOptionsModul.clbGradienten_ItemCheck() - Problem: " & ex.ToString)
        End Try
    End Sub

    Private Sub CheckYourMail()
        'Initialisiert die aktuelleSettings des UCs per SlideShowTools.SettingsHandling.SettingsInbox.

        aktuelleSettings = GetSettings(Of ModulSettings_Mandelbrot)(nameModul)

    End Sub

End Class
