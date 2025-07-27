Imports SlideShowShader.Invertieren.ShaderMain
Imports SlideShowLogging.LogHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ucOptionsShader

    'Variablendeklarationen
    Private aktuelleSettings As ShaderSettings_Invertieren

    Private Sub ucOptionsShader_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisieren der Form und ihrer Steuerelemente

        'Aktuelle Settings einlesen
        CheckYourMail()

        'RadioButtons setzen
        Select Case aktuelleSettings.Modus
            Case "Farbe"
                rdoFarbe.Checked = True
            Case "Weiss-Schwarz"
                rdoWS.Checked = True
            Case "Zufall"
                rdoZufall.Checked = True
        End Select

    End Sub

    Private Sub rdoFarbe_CheckedChanged(sender As Object, e As EventArgs) Handles rdoFarbe.CheckedChanged
        'Behandelt rdoFarbe

        If rdoFarbe.Checked Then
            WriteToRegistry(SLIDESHOWSHADER_INVERTIEREN_FULLPATH & "Modus", "Farbe")
        End If
    End Sub

    Private Sub rdoWS_CheckedChanged(sender As Object, e As EventArgs) Handles rdoWS.CheckedChanged
        'Behandelt rdoWS

        If rdoWS.Checked Then
            WriteToRegistry(SLIDESHOWSHADER_INVERTIEREN_FULLPATH & "Modus", "Weiss-Schwarz")
        End If
    End Sub

    Private Sub rdoZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rdoZufall.CheckedChanged
        'Behandelt rdoZufall

        If rdoZufall.Checked Then
            WriteToRegistry(SLIDESHOWSHADER_INVERTIEREN_FULLPATH & "Modus", "Zufall")
        End If
    End Sub

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox aus

        aktuelleSettings = GetSettings(Of ShaderSettings_Invertieren)(nameShader)
    End Sub
End Class
