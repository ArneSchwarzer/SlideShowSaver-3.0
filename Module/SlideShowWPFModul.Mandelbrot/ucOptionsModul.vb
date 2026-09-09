Imports System.Windows.Forms
Imports SlideShowLogging
Imports SlideShowTools
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowWPFModul.Mandelbrot.ModulMain

Public Class ucOptionsModul

#Region "Variablendeklaration"
    'Variablendeklaration

    Private aktuelleSettings As ModulSettings_Mandelbrot

    Private wirdInitialisiert As Boolean
    Private wurdeBereinigt As Boolean

#End Region

#Region "Initialisierung"

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load

        CheckYourMail()
        InitialisiereControls()

    End Sub

    Private Sub InitialisiereControls()
        'Initialisiert sämtliche Controls aus aktuelleSettings,
        'ohne Direct Commit auszulösen.

        Dim alleGradienten As List(Of SlideShowGradient)
        Dim aktiveGradienten As HashSet(Of String)

        wirdInitialisiert = True

        Try

            alleGradienten = GradientenHandling.LadeGradienten()

            If aktuelleSettings.Gradienten IsNot Nothing Then

                aktiveGradienten =
                New HashSet(Of String)(
                    aktuelleSettings.Gradienten,
                    StringComparer.OrdinalIgnoreCase)

            Else

                aktiveGradienten =
                New HashSet(Of String)(
                    StringComparer.OrdinalIgnoreCase)

            End If

            clbGradienten.Items.Clear()

            For Each gradient As SlideShowGradient In alleGradienten

                clbGradienten.Items.Add(gradient.Name, aktiveGradienten.Contains(gradient.Name))

            Next

            If clbGradienten.CheckedItems.Count = 0 AndAlso clbGradienten.Items.Count > 0 Then

                clbGradienten.SetItemCheckState(0, CheckState.Checked)

            End If

            clbGradienten.Sorted = True

            chkGradientAnimieren.Checked = aktuelleSettings.GradientAnimieren
            chkKoordinatenAnzeigen.Checked = aktuelleSettings.KoordinatenAnzeigen
            chkRotation.Checked = aktuelleSettings.Rotation

            trkZoomgeschwindigkeit.Value = aktuelleSettings.Zoomgeschwindigkeit

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

#End Region

#Region "Direct Commit"

    Private Sub chkGradientAnimieren_CheckedChanged(sender As Object, e As EventArgs) Handles chkGradientAnimieren.CheckedChanged
        'Speichert die Einstellung zur Gradientenanimation.

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "GradientAnimieren",
                        chkGradientAnimieren.Checked.ToString())

    End Sub

    Private Sub chkKoordinatenAnzeigen_CheckedChanged(sender As Object, e As EventArgs) Handles chkKoordinatenAnzeigen.CheckedChanged
        'Speichert die Einstellung zur Koordinatenanzeige.

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "KoordinatenAnzeigen",
                        chkKoordinatenAnzeigen.Checked.ToString())

    End Sub

    Private Sub clbGradienten_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbGradienten.ItemCheck
        'Speichert die aktive Gradientenauswahl nach Abschluss
        'der Zustandsänderung des CheckedListBox-Eintrags.

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        Try

            BeginInvoke(New Action(AddressOf SpeichereGradientenauswahl))

        Catch ex As InvalidOperationException

            LogHandling.LogWarn("Modul Mandelbrot - ucOptionsModul.clbGradienten_ItemCheck(): " &
                                "Die Gradientenauswahl konnte nicht gespeichert werden: " & ex.ToString())

        End Try

    End Sub

    Private Sub trkZoomgeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkZoomgeschwindigkeit.ValueChanged
        'Speichert die Zoomgeschwindigkeit.

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Zoomgeschwindigkeit",
                        trkZoomgeschwindigkeit.Value.ToString())

    End Sub

    Private Sub chkRotation_CheckedChanged(sender As Object, e As EventArgs) Handles chkRotation.CheckedChanged
        'Speichert die Einstellung zur Kamerarotation.

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Rotation", chkRotation.Checked.ToString())

    End Sub

    Private Sub SpeichereGradientenauswahl()
        'Speichert die CheckedListBox, nachdem der neue
        'CheckState vollständig übernommen wurde.

        If wirdInitialisiert OrElse wurdeBereinigt OrElse IsDisposed OrElse Disposing Then

            Exit Sub

        End If

        CheckedListBoxHandling.SaveListBoxToRegistry(clbGradienten, SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Gradienten")

    End Sub

#End Region

#Region "Settings"

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Übernimmt die Defaultwerte, speichert sie und
        'initialisiert anschließend die Controls neu.

        Dim defaults As Dictionary(Of String, String)

        defaults = ModulMain.GetModulDefaultSettings()

        aktuelleSettings.Gradienten = SplitSemicolonList(defaults("Gradienten"))
        aktuelleSettings.GradientAnimieren = CBool(defaults("GradientAnimieren"))
        aktuelleSettings.KoordinatenAnzeigen = CBool(defaults("KoordinatenAnzeigen"))
        aktuelleSettings.Zoomgeschwindigkeit = CInt(defaults("Zoomgeschwindigkeit"))
        aktuelleSettings.Rotation = CBool(defaults("Rotation"))

        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Gradienten", defaults("Gradienten"))
        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "GradientAnimieren", defaults("GradientAnimieren"))
        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "KoordinatenAnzeigen", defaults("KoordinatenAnzeigen"))
        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Zoomgeschwindigkeit", defaults("Zoomgeschwindigkeit"))
        WriteToRegistry(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Rotation", defaults("Rotation"))

        InitialisiereControls()

    End Sub

    Private Sub CheckYourMail()
        'Initialisiert die aktuellen Settings über die SettingsInbox.

        aktuelleSettings = GetSettings(Of ModulSettings_Mandelbrot)(nameModul)

    End Sub

#End Region

#Region "Bereinigung"

    Private Sub ucOptionsModul_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert nach der Freigabe weitere verzögerte Direct-Commit-Aktionen.

        wurdeBereinigt = True

    End Sub

#End Region

End Class