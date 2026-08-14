Imports SlideShowShader.Invertieren.ShaderMain
Imports SlideShowLogging.LogHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ucOptionsShader

    'Variablendeklarationen
    Private aktuelleSettings As ShaderSettings_Invertieren

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert das Options-Control.

        CheckYourMail()

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub Modus_CheckedChanged(sender As Object, e As EventArgs) _
    Handles rdoFarbe.CheckedChanged,
            rdoWS.CheckedChanged,
            rdoZufall.CheckedChanged
        'Übernimmt den vom Benutzer gewählten Invertierungsmodus.

        Dim modus As String

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        If rdoFarbe.Checked Then

            modus = "Farbe"

        ElseIf rdoWS.Checked Then

            modus = "Weiss-Schwarz"

        ElseIf rdoZufall.Checked Then

            modus = "Zufall"

        Else

            Exit Sub

        End If

        aktuelleSettings.Modus = modus

        WriteToRegistry(SLIDESHOWSHADER_INVERTIEREN_FULLPATH & "Modus", modus)

    End Sub

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox aus

        aktuelleSettings = GetSettings(Of ShaderSettings_Invertieren)(nameShader)
    End Sub

    Private Sub IniOrReinitialise()
        'Setzt die Controls entsprechend aktuelleSettings.

        Select Case aktuelleSettings.Modus

            Case "Farbe"

                rdoFarbe.Checked = True

            Case "Weiss-Schwarz"

                rdoWS.Checked = True

            Case "Zufall"

                rdoZufall.Checked = True

        End Select

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Stellt die Defaultwerte wieder her.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetShaderDefaultSettings()

        aktuelleSettings.Modus = defaults("Modus")

        WriteToRegistry(SLIDESHOWSHADER_INVERTIEREN_FULLPATH & "Modus", defaults("Modus"))

        wirdInitialisiert = True

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub ucOptionsShader_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert Registrywrites nach der Freigabe.

        wurdeBereinigt = True

    End Sub

End Class
