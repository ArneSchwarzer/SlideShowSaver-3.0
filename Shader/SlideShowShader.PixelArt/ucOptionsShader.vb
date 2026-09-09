Imports SlideShowShader.PixelArt.ShaderMain
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ucOptionsShader
    Inherits System.Windows.Forms.UserControl

#Region "Variablendeklaration"

    Private aktuelleSettings As ShaderSettings_PixelArt

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

#End Region

#Region "Initialisierung"

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert das Options-Control.

        CheckYourMail()

        Try

            IniOrReinitialize()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox.

        aktuelleSettings = GetSettings(Of ShaderSettings_PixelArt)(nameShader)

    End Sub

    Private Sub IniOrReinitialize()
        'Initialisiert oder reinitialisiert sämtliche Controls
        'anhand der aktuellen Settings.

        trkRaster.Value = aktuelleSettings.raster
        lblRaster.Text = Math.Pow(2, aktuelleSettings.raster).ToString() & " px"
        chkRasterZufall.Checked = aktuelleSettings.rasterZufall

        AktualisiereRasterControls()

        cmbFarbraum.SelectedIndex = aktuelleSettings.farbraum - 2
        chkFarbraumZufall.Checked = aktuelleSettings.farbraumZufall

        AktualisiereFarbraumControls()

    End Sub

#End Region

#Region "Control-Logik"

    Private Sub AktualisiereRasterControls()
        'Aktiviert oder deaktiviert die manuellen Raster-Controls.

        Dim manuelleAuswahlAktiv As Boolean

        manuelleAuswahlAktiv = Not chkRasterZufall.Checked

        trkRaster.Enabled = manuelleAuswahlAktiv
        lblNRaster.Enabled = manuelleAuswahlAktiv
        lblRaster.Enabled = manuelleAuswahlAktiv

    End Sub

    Private Sub AktualisiereFarbraumControls()
        'Aktiviert oder deaktiviert die manuelle Farbraum-Auswahl.

        Dim manuelleAuswahlAktiv As Boolean

        manuelleAuswahlAktiv = Not chkFarbraumZufall.Checked
        lblNFarbraum.Enabled = manuelleAuswahlAktiv
        cmbFarbraum.Enabled = manuelleAuswahlAktiv

    End Sub

#End Region

#Region "Direct Commit"

    Private Sub chkRasterZufall_CheckedChanged(sender As Object, e As EventArgs) Handles chkRasterZufall.CheckedChanged
        'Behandelt die Zufallsauswahl der Rastergröße.

        AktualisiereRasterControls()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.rasterZufall = chkRasterZufall.Checked

        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "RasterZufall", aktuelleSettings.rasterZufall.ToString())

    End Sub

    Private Sub cmbFarbraum_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFarbraum.SelectedIndexChanged
        'Behandelt die Anzahl der Posterise-Farbstufen.

        Dim farbraum As Integer

        If cmbFarbraum.SelectedIndex < 0 Then
            Exit Sub
        End If

        farbraum = cmbFarbraum.SelectedIndex + 2

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.farbraum = farbraum

        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Farbraum", farbraum.ToString())

    End Sub

    Private Sub trkRaster_ValueChanged(sender As Object, e As EventArgs) Handles trkRaster.ValueChanged
        'Behandelt die Rastergröße.

        lblRaster.Text = Math.Pow(2, trkRaster.Value).ToString() & " px"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.raster = trkRaster.Value

        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Raster", aktuelleSettings.raster.ToString())

    End Sub

    Private Sub chkFarbraumZufall_CheckedChanged(sender As Object, e As EventArgs) Handles chkFarbraumZufall.CheckedChanged
        'Behandelt die Zufallsauswahl des Farbraums.

        AktualisiereFarbraumControls()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.farbraumZufall = chkFarbraumZufall.Checked

        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "FarbraumZufall", aktuelleSettings.farbraumZufall.ToString())

    End Sub

#End Region

#Region "Defaults"

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Stellt sämtliche Defaultwerte wieder her und
        'speichert sie explizit per Direct Commit.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetShaderDefaultSettings()

        aktuelleSettings.raster = CInt(defaults("Raster"))
        aktuelleSettings.rasterZufall = CBool(defaults("RasterZufall"))
        aktuelleSettings.farbraum = CInt(defaults("Farbraum"))
        aktuelleSettings.farbraumZufall = CBool(defaults("FarbraumZufall"))

        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Raster", defaults("Raster"))
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "RasterZufall", defaults("RasterZufall"))
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Farbraum", defaults("Farbraum"))
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "FarbraumZufall", defaults("FarbraumZufall"))

        wirdInitialisiert = True

        Try

            IniOrReinitialize()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

#End Region

#Region "Bereinigung"

    Private Sub ucOptionsShader_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert Direct-Commit-Aktionen nach der Freigabe.

        wurdeBereinigt = True

    End Sub

#End Region

End Class