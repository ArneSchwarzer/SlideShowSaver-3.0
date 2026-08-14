Imports SlideShowShader.Nachtsicht.ShaderMain
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ucOptionsShader

    Private aktuelleSettings As ShaderSettings_Nachtsicht

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load

        CheckYourMail()

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub chkBNDOverlay_CheckedChanged(sender As Object, e As EventArgs) Handles chkBNDOverlay.CheckedChanged

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.BNDOverlayAktiv = chkBNDOverlay.Checked

        WriteToRegistry(SLIDESHOWSHADER_NACHTSICHT_FULLPATH & "BNDOverlayAktiv",
                        aktuelleSettings.BNDOverlayAktiv.ToString())

    End Sub

    Private Sub CheckYourMail()

        aktuelleSettings = GetSettings(Of ShaderSettings_Nachtsicht)(nameShader)

    End Sub

    Private Sub IniOrReinitialise()

        chkBNDOverlay.Checked = aktuelleSettings.BNDOverlayAktiv

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetShaderDefaultSettings()

        aktuelleSettings.BNDOverlayAktiv =
            String.Equals(
                defaults("BNDOverlayAktiv"),
                "True",
                StringComparison.OrdinalIgnoreCase)

        WriteToRegistry(SLIDESHOWSHADER_NACHTSICHT_FULLPATH & "BNDOverlayAktiv", defaults("BNDOverlayAktiv"))

        wirdInitialisiert = True

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub ucOptionsShader_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        GC.SuppressFinalize(Me)

    End Sub

End Class