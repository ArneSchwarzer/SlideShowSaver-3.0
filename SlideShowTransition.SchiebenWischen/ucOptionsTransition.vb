Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports Transition_Schieben.TransitionMain

Public Class ucOptionsTransition

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente der ucOptionsTransition
        Dim defaults As Dictionary(Of String, String) = GetTransitionDefaultSettings()
        Dim richtungen As New List(Of String)

        'Steuerelemente initialisieren

        'Richtungsbuttons
        richtungen = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", defaults))

        For Each richtung In richtungen

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

        'Geschwindigkeit
        trbGeschwindigkeit.Value = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", defaults)
        lblGeschwindigkeit.Text = (11 - trbGeschwindigkeit.Value).ToString & " s"

        'Modus
        Select Case ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", defaults)
            Case "Schieben"
                rbSchieben.Checked = True
            Case "Wischen"
                rbWischen.Checked = True
            Case "Zufällig"
                rbZufall.Checked = True
        End Select

    End Sub

    Private Sub tbtNW_CheckChanged(sender As Object, e As EventArgs)
        'Button Nord-West

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtN_CheckChanged(sender As Object, e As EventArgs)
        'Button Nord

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtNO_CheckChanged(sender As Object, e As EventArgs)
        'Button Nord-Ost

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtO_CheckChanged(sender As Object, e As EventArgs)
        'Button Ost

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtSO_CheckChanged(sender As Object, e As EventArgs)
        'Button Süd-Ost

        'DirectCommit

        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtS_CheckChanged(sender As Object, e As EventArgs)
        'Button Süd

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtSW_CheckChanged(sender As Object, e As EventArgs)
        'Button Süd-West

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtW_CheckChanged(sender As Object, e As EventArgs)
        'Button West

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub rbSchieben_CheckedChanged(sender As Object, e As EventArgs) Handles rbSchieben.CheckedChanged
        'RadioButton Schieben

        If rbSchieben.Checked Then
            'DirectCommit
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Schieben")
        End If
    End Sub

    Private Sub rbWischen_CheckedChanged(sender As Object, e As EventArgs) Handles rbWischen.CheckedChanged
        'RadioButton Wischen

        If rbWischen.Checked Then
            'DirectCommit
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Wischen")
        End If
    End Sub

    Private Sub rbZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rbZufall.CheckedChanged
        'RadioButton Zufall

        If rbZufall.Checked Then
            'DirectCommit
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Zufällig")
        End If
    End Sub

    Private Sub trbGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trbGeschwindigkeit.ValueChanged
        'TrackBar Geschwindigkeit

        lblGeschwindigkeit.Text = (11 - trbGeschwindigkeit.Value).ToString & " s"

        'DirectCommit
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", trbGeschwindigkeit.Value.ToString)

    End Sub

    Private Function RichtungsStringBauen() As String
        'Setzt den Richtungsstring zusammen und setzt auch gleich das lblRichtungen entsprechend.

        Dim richtungsString As String = Nothing
        Dim richtungen As New List(Of String)

        If tbtN.Checked Then richtungen.Add("N")
        If tbtNO.Checked Then richtungen.Add("NO")
        If tbtO.Checked Then richtungen.Add("O")
        If tbtSO.Checked Then richtungen.Add("SO")
        If tbtS.Checked Then richtungen.Add("S")
        If tbtSW.Checked Then richtungen.Add("SW")
        If tbtW.Checked Then richtungen.Add("W")
        If tbtNW.Checked Then richtungen.Add("NW")

        richtungsString = String.Join(";", richtungen)

        'Label ein- oder ausschalten
        If richtungsString Is Nothing Then
            lblKeineRichtungInfo.Visible = True
        Else
            lblKeineRichtungInfo.Visible = False
        End If

        Return richtungsString

    End Function

End Class
