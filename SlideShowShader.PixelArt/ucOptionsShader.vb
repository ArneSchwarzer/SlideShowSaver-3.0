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

        'Radio Buttons "Modus"
        Select Case aktuelleSettings.modus
            Case "PixelArt"
                rdoModusPixelArt.Checked = True
                grbHalftoneOptionen.Enabled = False
            Case "HTSW"
                rdoModusHTSW.Checked = True
                grbHalftoneOptionen.Enabled = True
                lblNDotsWinkel.Enabled = False
                cmbDotsWinkel.Enabled = False
            Case "HTCMYK"
                rdoModusHTCMYK.Checked = True
                grbHalftoneOptionen.Enabled = True
                lblNDotsWinkel.Enabled = True
                cmbDotsWinkel.Enabled = True
            Case "Zufall"
                rdoModusZufall.Checked = True
                grbHalftoneOptionen.Enabled = True
                lblNDotsWinkel.Enabled = True
                cmbDotsWinkel.Enabled = True
        End Select

        'Trackbar "Raster"
        trkRaster.Value = aktuelleSettings.raster
        lblRaster.Text = Math.Pow(2, aktuelleSettings.raster).ToString & " px"

        'Checkbox "Raster Zufall"
        chkRasterZufall.Checked = aktuelleSettings.rasterZufall
        trkRaster.Enabled = Not aktuelleSettings.rasterZufall
        lblNRaster.Enabled = Not aktuelleSettings.rasterZufall
        lblRaster.Enabled = Not aktuelleSettings.rasterZufall

        'Combobox "Farbe" (Posterise)
        cmbPosterise.SelectedIndex = aktuelleSettings.posterise - 2

        'Trackbar "Gamma"
        trkGamma.Value = CInt(10 * aktuelleSettings.gamma)
        lblGamma.Text = aktuelleSettings.gamma.ToString

        'Trackbar "Dots MaxGröße"
        trkDotsMax.Value = aktuelleSettings.dotsMax
        lblDotsMax.Text = aktuelleSettings.dotsMax.ToString & " %"

        'Trackbar "Dots MinGröße"
        trkDotsMin.Value = aktuelleSettings.dotsMin
        lblDotsMin.Text = aktuelleSettings.dotsMin.ToString & " %"

        'ComboBox "Dots Winkel"
        If aktuelleSettings.dotsWinkel.R = 75 Then
            cmbDotsWinkel.SelectedIndex = 0
        Else
            cmbDotsWinkel.SelectedIndex = 1
        End If

        'Checkbox "Papiertextur"
        chkPapierTextur.Checked = aktuelleSettings.papier
        lblNPapierIntensität.Enabled = aktuelleSettings.papier
        trkPapierIntensität.Enabled = aktuelleSettings.papier
        lblPapierintensität.Enabled = aktuelleSettings.papier

        'Trackbar "Papiertextur Intensität"
        trkPapierIntensität.Value = aktuelleSettings.papierFaktor
        lblPapierintensität.Text = aktuelleSettings.papierFaktor.ToString & " %"

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Lädt und setzt die Default-Werte für die Dialogbox

        Dim defaults As Dictionary(Of String, String)

        'Defaultwerte einlesen
        defaults = ShaderMain.GetShaderDefaultSettings()


        'AktuelleSettings aktualisieren
        aktuelleSettings.modus = defaults("Modus")
        'Die Potenz zur Basis 2 wird gespeichert!
        aktuelleSettings.raster = CInt(defaults("Raster"))
        If defaults("RasterZufall") = "True" Then
            aktuelleSettings.rasterZufall = True
        Else
            aktuelleSettings.rasterZufall = False
        End If
        'Der Faktor pro Kanal wird gespeichert
        aktuelleSettings.posterise = CInt(defaults("Posterise"))

        'Gleitkommazahlen sind doof...
        Dim sGamma As String = defaults("Gamma")
        Dim g As Single
        If Not Single.TryParse(sGamma, Globalization.NumberStyles.Float, Globalization.CultureInfo.CurrentCulture, g) Then
            Single.TryParse(sGamma, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, g)
        End If
        aktuelleSettings.gamma = g

        aktuelleSettings.gamma = CSng(defaults("Gamma"))
        aktuelleSettings.dotsMax = CInt(defaults("DotsMax"))
        aktuelleSettings.dotsMin = CInt(defaults("DotsMin"))
        'kleiner Missbrauch von "Color" als CMYK-Datenstruktur: C = B, M = R, Y = G, K = A
        aktuelleSettings.dotsWinkel = StringToColor(defaults("DotsWinkel"))
        If defaults("Papier") = "True" Then
            aktuelleSettings.papier = True
        Else
            aktuelleSettings.papier = False
        End If
        aktuelleSettings.papierFaktor = CInt(defaults("PapierFaktor"))

        'Steuerelemente re-initialisieren
        IniOrReinitialize()

    End Sub

    Private Sub chkPapierTextur_CheckedChanged(sender As Object, e As EventArgs) Handles chkPapierTextur.CheckedChanged
        'Behandelt die Checkbox "Papiertextur"

        lblNPapierIntensität.Enabled = chkPapierTextur.Checked
        trkPapierIntensität.Enabled = chkPapierTextur.Checked
        lblPapierintensität.Enabled = chkPapierTextur.Checked

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Papier", chkPapierTextur.Checked.ToString)

    End Sub

    Private Sub chkRasterZufall_CheckedChanged(sender As Object, e As EventArgs) Handles chkRasterZufall.CheckedChanged
        'Behandelt die Checkbox "Raster Zufall"

        trkRaster.Enabled = (Not chkRasterZufall.Checked).ToString
        lblNRaster.Enabled = (Not chkRasterZufall.Checked).ToString
        lblRaster.Enabled = (Not chkRasterZufall.Checked).ToString

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "RasterZufall", chkRasterZufall.Checked.ToString)

    End Sub

    Private Sub cmbDotsWinkel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbDotsWinkel.SelectedIndexChanged
        'Behandelt die ComboBox "Dots Winkel"

        'Direct Commit
        'kleiner Missbrauch von "Color" als CMYK-Datenstruktur: C = B, M = R, Y = G, K = A
        If cmbDotsWinkel.SelectedIndex = 0 Then
            WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "DotsWinkel", "75,0,15,45")
        Else
            WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "DotsWinkel", "45,0,15,75")
        End If

    End Sub

    Private Sub cmbPosterise_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbPosterise.SelectedIndexChanged
        'Behandelt die ComboBox "Farben (Posterise)"

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Posterise", (cmbPosterise.SelectedIndex + 2).ToString)

    End Sub

    Private Sub rdoModusHTCMYK_CheckedChanged(sender As Object, e As EventArgs) Handles rdoModusHTCMYK.CheckedChanged
        'Behandelt Radio Button "Halftone (CMYK)"

        If rdoModusHTCMYK.Checked Then
            grbHalftoneOptionen.Enabled = True
            lblNDotsWinkel.Enabled = True
            cmbDotsWinkel.Enabled = True

            'Direct Commit
            WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Modus", "HTCMYK")
        End If

    End Sub

    Private Sub rdoModusHTSW_CheckedChanged(sender As Object, e As EventArgs) Handles rdoModusHTSW.CheckedChanged
        'Behandelt Radio Button "Halftone (SW)"

        If rdoModusHTSW.Checked Then
            grbHalftoneOptionen.Enabled = True
            lblNDotsWinkel.Enabled = False
            cmbDotsWinkel.Enabled = False

            'Direct Commit
            WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Modus", "HTSW")
        End If
    End Sub

    Private Sub rdoModusPixelArt_CheckedChanged(sender As Object, e As EventArgs) Handles rdoModusPixelArt.CheckedChanged
        'Behandelt Radio Button "PixelArt"

        If rdoModusPixelArt.Checked Then
            grbHalftoneOptionen.Enabled = False

            'Direct Commit
            WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Modus", "PixelArt")
        End If

    End Sub

    Private Sub rdoModusZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rdoModusZufall.CheckedChanged
        'Behandelt Radio Button "Zufällig"

        If rdoModusZufall.Checked Then
            grbHalftoneOptionen.Enabled = True
            lblNDotsWinkel.Enabled = True
            cmbDotsWinkel.Enabled = True

            'Direct Commit
            WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Modus", "Zufall")
        End If

    End Sub

    Private Sub trkDotsMax_ValueChanged(sender As Object, e As EventArgs) Handles trkDotsMax.ValueChanged
        'Behandelt TrackBar "Dots MaxGröße"

        lblDotsMax.Text = trkDotsMax.Value.ToString & " %"

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "DotsMax", trkDotsMax.Value.ToString)

    End Sub

    Private Sub trkDotsMin_ValueChanged(sender As Object, e As EventArgs) Handles trkDotsMin.ValueChanged
        'Behandelt Trackbar "Dots MinGröße"

        lblDotsMin.Text = trkDotsMin.Value.ToString & " %"

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "DotsMin", trkDotsMin.Value.ToString)

    End Sub

    Private Sub trkGamma_ValueChanged(sender As Object, e As EventArgs) Handles trkGamma.ValueChanged
        'Behandelt Trackbar "Gamma"

        lblGamma.Text = (trkGamma.Value / 10).ToString

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Gamma", (trkGamma.Value / 10.0F).ToString(Globalization.CultureInfo.InvariantCulture))

    End Sub

    Private Sub trkPapierIntensität_ValueChanged(sender As Object, e As EventArgs) Handles trkPapierIntensität.ValueChanged
        'Behandelt Trackbar "Papier Intensität"

        lblPapierintensität.Text = trkPapierIntensität.Value.ToString & " %"

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "PapierFaktor", trkPapierIntensität.Value.ToString)

    End Sub

    Private Sub trkRaster_ValueChanged(sender As Object, e As EventArgs) Handles trkRaster.ValueChanged
        'Behandelt TrackBar "Rastergröße"

        lblRaster.Text = Math.Pow(2, trkRaster.Value).ToString & " px"

        'Direct Commit
        WriteToRegistry(SLIDESHOWSHADER_PIXELART_FULLPATH & "Raster", trkRaster.Value.ToString)

    End Sub
End Class
