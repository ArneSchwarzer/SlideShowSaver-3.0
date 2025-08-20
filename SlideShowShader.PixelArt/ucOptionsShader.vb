Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowShader.PixelArt.ShaderMain
Imports System.ComponentModel

Public Class ucOptionsShader
    Inherits System.Windows.Forms.UserControl

    'Variablendeklaration

    Private aktuelleSettings As ShaderSettings_PixelArt

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        CheckYourMail()
        IniOrReinitialize()
    End Sub

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox aus

        aktuelleSettings = GetSettings(Of ShaderSettings_PixelArt)(nameShader)

    End Sub

    Private Sub IniOrReinitialize()
        'Initialisiert oder Re-Initialisiert die Steuerelemente

        'Trackbar "Raster"
        trkRaster.Value = aktuelleSettings.raster
        lblRaster.Text = Math.Pow(2, aktuelleSettings.raster).ToString & " px"

        'Checkbox "Raster Zufall"
        chkRasterZufall.Checked = aktuelleSettings.rasterZufall
        trkRaster.Enabled = Not aktuelleSettings.rasterZufall
        lblNRaster.Enabled = Not aktuelleSettings.rasterZufall
        lblRaster.Enabled = Not aktuelleSettings.rasterZufall

        'Combobox "Farbraum"
        cmbFarbraum.SelectedIndex = aktuelleSettings.farbraum - 2

        'Checkbox "Farbraum Zufall"
        chkFarbraumZufall.Checked = aktuelleSettings.farbraumZufall
        lblNFarbraum.Enabled = Not aktuelleSettings.farbraumZufall
        cmbFarbraum.Enabled = Not aktuelleSettings.farbraumZufall

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Lädt und setzt die Default-Werte für die Dialogbox

        Dim defaults As Dictionary(Of String, String)

        'Defaultwerte einlesen
        defaults = ShaderMain.GetShaderDefaultSettings()


        'AktuelleSettings aktualisieren
        'Die Potenz zur Basis 2 wird gespeichert!
        aktuelleSettings.raster = CInt(defaults("Raster"))
        aktuelleSettings.rasterZufall = CBool(defaults("RasterZufall"))
        'Der Faktor pro Kanal wird gespeichert
        aktuelleSettings.farbraum = CInt(defaults("Farbraum"))
        aktuelleSettings.farbraumZufall = CBool(defaults("FarbraumZufall"))

        'Steuerelemente re-initialisieren
        IniOrReinitialize()

    End Sub

    Private Sub chkRasterZufall_CheckedChanged(sender As Object, e As EventArgs) Handles chkRasterZufall.CheckedChanged
        'Behandelt die Checkbox "Raster Zufall"

        trkRaster.Enabled = (Not chkRasterZufall.Checked)
        lblNRaster.Enabled = (Not chkRasterZufall.Checked)
        lblRaster.Enabled = (Not chkRasterZufall.Checked)

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "RasterZufall", chkRasterZufall.Checked.ToString)

    End Sub

    Private Sub cmbFarbraum_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFarbraum.SelectedIndexChanged
        'Behandelt die ComboBox "Farben (Posterise)"

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Farbraum", (cmbFarbraum.SelectedIndex + 2).ToString)

    End Sub

    Private Sub trkRaster_ValueChanged(sender As Object, e As EventArgs) Handles trkRaster.ValueChanged
        'Behandelt TrackBar "Rastergröße"

        lblRaster.Text = Math.Pow(2, trkRaster.Value).ToString & " px"

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Raster", trkRaster.Value.ToString)

    End Sub

    Private Sub chkFarbraumZufall_CheckedChanged(sender As Object, e As EventArgs) Handles chkFarbraumZufall.CheckedChanged
        'Behandelt die Checkbox "Farbraum Zufall"

        lblNFarbraum.Enabled = (Not chkFarbraumZufall.Checked)
        cmbFarbraum.Enabled = (Not chkFarbraumZufall.Checked)

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "FarbraumZufall", chkFarbraumZufall.Checked.ToString)

    End Sub

End Class
