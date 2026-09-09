Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowShader.TönenFärben.ShaderMain
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ucOptionsShader
    Inherits UserControl

#Region "Variablendeklaration"

    Private aktuelleSettings As ShaderSettings_ToenenFaerben

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

#End Region

#Region "Initialisierung"

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente des Options-Control.

        CheckYourMail()

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox.

        aktuelleSettings = GetSettings(Of ShaderSettings_ToenenFaerben)(nameShader)

    End Sub

    Private Sub IniOrReinitialise()
        'Setzt sämtliche Controls entsprechend aktuelleSettings.

        picFarbton.BackColor = aktuelleSettings.Farbton
        chkZufallsfarbe.Checked = aktuelleSettings.Zufallsfarbe

        AktualisiereFarbtonControls()

        trkIntensität.Value = aktuelleSettings.Intensitaet
        lblIntensität.Text = aktuelleSettings.Intensitaet.ToString() & " %"

        Select Case aktuelleSettings.Modus

            Case ShaderModus.Toenen

                rdoTönen.Checked = True

            Case ShaderModus.Faerben

                rdoFärben.Checked = True

            Case ShaderModus.Zufaellig

                rdoZufall.Checked = True

        End Select

    End Sub

#End Region

#Region "Control-Logik"

    Private Sub AktualisiereFarbtonControls()
        'Aktiviert oder deaktiviert die manuelle Farbwahl.

        Dim manuelleFarbwahlAktiv As Boolean

        manuelleFarbwahlAktiv = Not chkZufallsfarbe.Checked

        lblNpicFarbton.Enabled = manuelleFarbwahlAktiv

        picFarbton.Enabled = manuelleFarbwahlAktiv

        If manuelleFarbwahlAktiv Then

            picFarbton.BackColor = aktuelleSettings.Farbton

        Else

            picFarbton.BackColor = Color.Transparent

        End If

    End Sub

#End Region

#Region "Direct Commit"

    Private Sub picFarbton_Click(sender As Object, e As EventArgs) Handles picFarbton.Click
        'Öffnet die Farbauswahl für den manuellen Farbton.

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        Using dlg As New ColorDialog()

            dlg.Color = aktuelleSettings.Farbton
            dlg.AllowFullOpen = True
            dlg.AnyColor = True
            dlg.FullOpen = True

            If dlg.ShowDialog() = DialogResult.OK Then

                aktuelleSettings.Farbton = dlg.Color

                picFarbton.BackColor = dlg.Color

                WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Farbton", ColorToString(dlg.Color))

            End If

        End Using

    End Sub

    Private Sub trkIntensität_ValueChanged(sender As Object, e As EventArgs) Handles trkIntensität.ValueChanged
        'Behandelt die Intensität des Farb-Overlays.

        lblIntensität.Text = trkIntensität.Value.ToString() & " %"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.Intensitaet = trkIntensität.Value

        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Intensität",
                        aktuelleSettings.Intensitaet.ToString())

    End Sub

    Private Sub Modus_CheckedChanged(sender As Object, e As EventArgs) _
        Handles rdoTönen.CheckedChanged,
                rdoFärben.CheckedChanged,
                rdoZufall.CheckedChanged
        'Übernimmt den gewählten Shadermodus.

        Dim modusString As String

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        If rdoTönen.Checked Then

            aktuelleSettings.Modus = ShaderModus.Toenen

            modusString = "Tönen"

        ElseIf rdoFärben.Checked Then

            aktuelleSettings.Modus = ShaderModus.Faerben

            modusString = "Färben"

        ElseIf rdoZufall.Checked Then

            aktuelleSettings.Modus = ShaderModus.Zufaellig

            modusString = "Zufall"

        Else

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Modus", modusString)

    End Sub

    Private Sub chkZufallsfarbe_CheckedChanged(sender As Object, e As EventArgs) _
        Handles chkZufallsfarbe.CheckedChanged
        'Behandelt die automatische Farbauswahl.

        AktualisiereFarbtonControls()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.Zufallsfarbe = chkZufallsfarbe.Checked

        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Zufallsfarbe",
                        aktuelleSettings.Zufallsfarbe.ToString())

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

        aktuelleSettings.Farbton = StringToColor(defaults("Farbton"))
        aktuelleSettings.Zufallsfarbe = CBool(defaults("Zufallsfarbe"))
        aktuelleSettings.Intensitaet = CInt(defaults("Intensität"))

        Select Case defaults("Modus")

            Case "Tönen"

                aktuelleSettings.Modus = ShaderModus.Toenen

            Case "Färben"

                aktuelleSettings.Modus = ShaderModus.Faerben

            Case "Zufall"

                aktuelleSettings.Modus = ShaderModus.Zufaellig

            Case Else

                aktuelleSettings.Modus = ShaderModus.Toenen

        End Select

        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Farbton", defaults("Farbton"))
        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Zufallsfarbe", defaults("Zufallsfarbe"))
        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Intensität", defaults("Intensität"))
        WriteToRegistry(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Modus", defaults("Modus"))

        wirdInitialisiert = True

        Try

            IniOrReinitialise()

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