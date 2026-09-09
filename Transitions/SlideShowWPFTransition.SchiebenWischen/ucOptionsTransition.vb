Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowWPFTransition.SchiebenWischen.TransitionMain_SuW.TransitionMain

Public Class ucOptionsTransition

    'Variablendeklaration
    Private aktuelleSettings As SlideShowTransitionSettings_SuW

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load

        CheckYourMail()

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub Richtung_CheckChanged(sender As Object, e As EventArgs) _
    Handles tbtN.CheckChanged,
            tbtNO.CheckChanged,
            tbtO.CheckChanged,
            tbtSO.CheckChanged,
            tbtS.CheckChanged,
            tbtSW.CheckChanged,
            tbtW.CheckChanged,
            tbtNW.CheckChanged

        RichtungsStringBauen()

    End Sub

    Private Sub Modus_CheckedChanged(sender As Object, e As EventArgs) _
    Handles rbSchieben.CheckedChanged,
            rbWischen.CheckedChanged,
            rbZufall.CheckedChanged

        Dim modus As String

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        If rbSchieben.Checked Then

            modus = "Schieben"

        ElseIf rbWischen.Checked Then

            modus = "Wischen"

        ElseIf rbZufall.Checked Then

            modus = "Zufällig"

        Else

            Exit Sub

        End If

        aktuelleSettings.modus = modus

        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", modus)

    End Sub

    Private Sub trkGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkGeschwindigkeit.ValueChanged

        Dim geschwindigkeit As Integer

        geschwindigkeit = (trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value

        lblGeschwindigkeit.Text = geschwindigkeit.ToString() & " s"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.geschwindigkeit = geschwindigkeit

        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", geschwindigkeit.ToString())

    End Sub

    Private Sub RichtungsStringBauen()

        Dim richtungsString As String
        Dim richtungen As New List(Of String)

        If tbtN.Checked Then
            richtungen.Add("N")
        End If

        If tbtNO.Checked Then
            richtungen.Add("NO")
        End If

        If tbtO.Checked Then
            richtungen.Add("O")
        End If

        If tbtSO.Checked Then
            richtungen.Add("SO")
        End If

        If tbtS.Checked Then
            richtungen.Add("S")
        End If

        If tbtSW.Checked Then
            richtungen.Add("SW")
        End If

        If tbtW.Checked Then
            richtungen.Add("W")
        End If

        If tbtNW.Checked Then
            richtungen.Add("NW")
        End If

        lblKeineRichtungInfo.Visible = richtungen.Count = 0

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.richtungen = New List(Of String)(richtungen)

        richtungsString = String.Join(";", richtungen)

        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", richtungsString)

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SetttingsInbox

        aktuelleSettings = GetSettings(Of SlideShowTransitionSettings_SuW)(nameTransition)

    End Sub

    Private Sub IniOrReinitialise()
        'Richtungsbuttons
        tbtN.Checked = False
        tbtNO.Checked = False
        tbtO.Checked = False
        tbtSO.Checked = False
        tbtS.Checked = False
        tbtSW.Checked = False
        tbtW.Checked = False
        tbtNW.Checked = False

        For Each richtung In aktuelleSettings.richtungen

            Select Case richtung
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
            End Select

        Next

        'Label "Keine Richtung Ausgewählt"
        If aktuelleSettings.richtungen.Count = 0 Then
            lblKeineRichtungInfo.Visible = True
        Else
            lblKeineRichtungInfo.Visible = False
        End If

        'Geschwindigkeit
        trkGeschwindigkeit.Value = (trkGeschwindigkeit.Maximum + 1) - aktuelleSettings.geschwindigkeit
        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString & " s"

        'Modus
        Select Case aktuelleSettings.modus
            Case "Schieben"
                rbSchieben.Checked = True
            Case "Wischen"
                rbWischen.Checked = True
            Case "Zufällig"
                rbZufall.Checked = True
        End Select

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetTransitionDefaultSettings()

        aktuelleSettings.geschwindigkeit = CInt(defaults("Geschwindigkeit"))
        aktuelleSettings.richtungen = SplitSemicolonList(defaults("Richtungen"))
        aktuelleSettings.modus = defaults("Modus")

        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", defaults("Geschwindigkeit"))
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", defaults("Richtungen"))
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", defaults("Modus"))

        wirdInitialisiert = True

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub ucOptionsTransition_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed

        wurdeBereinigt = True

    End Sub

End Class
