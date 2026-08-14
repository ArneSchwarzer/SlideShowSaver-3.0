Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowWPFTransition.Zoom.TransitionMain

Public Class ucOptionsTransition
    Inherits System.Windows.Forms.UserControl

    'Variablendeklaration
    Private aktuelleSettings As SlideShowTransitionSettings_Zoom

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente der ucOptionsTransition.

        CheckYourMail()

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub Ankerpunkt_CheckChanged(sender As Object, e As EventArgs) _
    Handles tbtN.CheckChanged,
            tbtNO.CheckChanged,
            tbtO.CheckChanged,
            tbtSO.CheckChanged,
            tbtS.CheckChanged,
            tbtSW.CheckChanged,
            tbtW.CheckChanged,
            tbtNW.CheckChanged,
            tbtZ.CheckChanged
        'Behandelt Änderungen der möglichen Ankerpunkte.

        AnkerStringBauen()

    End Sub

    Private Sub chkGleicherAnkerpunkt_CheckedChanged(sender As Object, e As EventArgs) _
    Handles chkGleicherAnkerpunkt.CheckedChanged
        'Behandelt die Einstellung für identische Ankerpunkte.

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.gleicherAnkerpunkt = chkGleicherAnkerpunkt.Checked

        WriteToRegistry(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "GleicherAnkerpunkt",
                        aktuelleSettings.gleicherAnkerpunkt.ToString())

    End Sub

    Private Sub trkGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkGeschwindigkeit.ValueChanged
        'Behandelt die Zoom-Geschwindigkeit.

        Dim geschwindigkeit As Integer

        geschwindigkeit = (trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value

        lblGeschwindigkeit.Text = geschwindigkeit.ToString() & " s"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.geschwindigkeit = geschwindigkeit

        WriteToRegistry(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Geschwindigkeit", geschwindigkeit.ToString())

    End Sub

    Private Sub AnkerStringBauen()
        'Ermittelt die ausgewählten Ankerpunkte,
        'aktualisiert UI und lokale Settings und speichert
        'bei echter Benutzereingabe per Direct Commit.

        Dim anker As New List(Of String)
        Dim ankerString As String

        If tbtN.Checked Then
            anker.Add("N")
        End If

        If tbtNO.Checked Then
            anker.Add("NO")
        End If

        If tbtO.Checked Then
            anker.Add("O")
        End If

        If tbtSO.Checked Then
            anker.Add("SO")
        End If

        If tbtS.Checked Then
            anker.Add("S")
        End If

        If tbtSW.Checked Then
            anker.Add("SW")
        End If

        If tbtW.Checked Then
            anker.Add("W")
        End If

        If tbtNW.Checked Then
            anker.Add("NW")
        End If

        If tbtZ.Checked Then
            anker.Add("Z")
        End If

        lblKeinAnkerpunkt.Visible = anker.Count = 0

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.ankerpunkte = New List(Of String)(anker)

        ankerString = String.Join(";", anker)

        WriteToRegistry(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Ankerpunkte", ankerString)

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SetttingsInbox

        aktuelleSettings = GetSettings(Of SlideShowTransitionSettings_Zoom)(nameTransition)

    End Sub

    Private Sub IniOrReinitialise()
        'Setzt die Controls gemäß aktuelleSettings.

        tbtN.Checked = False
        tbtNO.Checked = False
        tbtO.Checked = False
        tbtSO.Checked = False
        tbtS.Checked = False
        tbtSW.Checked = False
        tbtW.Checked = False
        tbtNW.Checked = False
        tbtZ.Checked = False

        If aktuelleSettings.ankerpunkte IsNot Nothing Then

            For Each ankerpunkt As String In aktuelleSettings.ankerpunkte

                Select Case ankerpunkt

                    Case "N"
                        tbtN.Checked = True

                    Case "NO"
                        tbtNO.Checked = True

                    Case "O"
                        tbtO.Checked = True

                    Case "SO"
                        tbtSO.Checked = True

                    Case "S"
                        tbtS.Checked = True

                    Case "SW"
                        tbtSW.Checked = True

                    Case "W"
                        tbtW.Checked = True

                    Case "NW"
                        tbtNW.Checked = True

                    Case "Z"
                        tbtZ.Checked = True

                End Select

            Next

        End If

        If aktuelleSettings.ankerpunkte Is Nothing Then

            lblKeinAnkerpunkt.Visible = True

        Else

            lblKeinAnkerpunkt.Visible = aktuelleSettings.ankerpunkte.Count = 0

        End If

        chkGleicherAnkerpunkt.Checked = aktuelleSettings.gleicherAnkerpunkt

        trkGeschwindigkeit.Value = (trkGeschwindigkeit.Maximum + 1) - aktuelleSettings.geschwindigkeit

        lblGeschwindigkeit.Text = aktuelleSettings.geschwindigkeit.ToString() & " s"

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Stellt die Defaultwerte wieder her und speichert
        'diese explizit gemäß Direct-Commit-Architektur.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetTransitionDefaultSettings()

        aktuelleSettings.geschwindigkeit = CInt(defaults("Geschwindigkeit"))
        aktuelleSettings.ankerpunkte = SplitSemicolonList(defaults("Ankerpunkte"))
        aktuelleSettings.gleicherAnkerpunkt = String.Equals(defaults("GleicherAnkerpunkt"), "True",
                                                            StringComparison.OrdinalIgnoreCase)

        WriteToRegistry(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Geschwindigkeit", defaults("Geschwindigkeit"))
        WriteToRegistry(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Ankerpunkte", defaults("Ankerpunkte"))
        WriteToRegistry(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "GleicherAnkerpunkt", defaults("GleicherAnkerpunkt"))

        wirdInitialisiert = True

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub ucOptionsTransition_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert Direct-Commit-Aktionen nach der Freigabe.

        wurdeBereinigt = True

    End Sub

End Class
