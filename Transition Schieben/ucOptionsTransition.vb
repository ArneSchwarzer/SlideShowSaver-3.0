Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports Transition_Schieben.TransitionMain

Public Class ucOptionsTransition

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim defaults As Dictionary(Of String, String) = GetTransitionDefaultSettings()
        Dim richtungen As New List(Of String)

        'Steuerelemente initialisieren
        richtungen = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", defaults))

        For Each richtung In richtungen

            Select Case richtung
                Case "N"
                    tbtVN.Checked = True
                Case "NO"
                    tbtVNO.Checked = True
                Case "O"
                    tbtVO.Checked = True
                Case "SO"
                    tbtVSO.Checked = True
                Case "S"
                    tbtVS.Checked = True
                Case "SW"
                    tbtVSW.Checked = True
                Case "W"
                    tbtVW.Checked = True
                Case "NW"
                    tbtVNW.Checked = True
            End Select

        Next

        'Geschwindigkeit
        trbGeschwindigkeit.Value = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", defaults)

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

    Private Sub tbtVNW_CheckChanged(sender As Object, e As EventArgs) Handles tbtVNW.CheckChanged
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtVN_CheckChanged(sender As Object, e As EventArgs) Handles tbtVN.CheckChanged
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtVNO_CheckChanged(sender As Object, e As EventArgs) Handles tbtVNO.CheckChanged
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtVO_CheckChanged(sender As Object, e As EventArgs) Handles tbtVO.CheckChanged
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtVSO_CheckChanged(sender As Object, e As EventArgs) Handles tbtVSO.CheckChanged
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtVS_CheckChanged(sender As Object, e As EventArgs) Handles tbtVS.CheckChanged
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtVSW_CheckChanged(sender As Object, e As EventArgs) Handles tbtVSW.CheckChanged
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub tbtVW_CheckChanged(sender As Object, e As EventArgs) Handles tbtVW.CheckChanged
        WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", RichtungsStringBauen())
    End Sub

    Private Sub rbSchieben_CheckedChanged(sender As Object, e As EventArgs) Handles rbSchieben.CheckedChanged
        If rbSchieben.Checked Then
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Schieben")
        End If
    End Sub

    Private Sub rbWischen_CheckedChanged(sender As Object, e As EventArgs) Handles rbWischen.CheckedChanged
        If rbWischen.Checked Then
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Wischen")
        End If
    End Sub

    Private Sub rbZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rbZufall.CheckedChanged
        If rbZufall.Checked Then
            WriteToRegistry(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", "Zufällig")
        End If
    End Sub

    Private Function RichtungsStringBauen() As String
        'Setzt den Richtungsstring zusammen und setzt auch gleich das lblRichtungen entsprechend.

        Dim richtungsString As String = Nothing
        Dim richtungen As New List(Of String)

        If tbtVN.Checked Then richtungen.Add("N")
        If tbtVNO.Checked Then richtungen.Add("NO")
        If tbtVO.Checked Then richtungen.Add("O")
        If tbtVSO.Checked Then richtungen.Add("SO")
        If tbtVS.Checked Then richtungen.Add("S")
        If tbtVSW.Checked Then richtungen.Add("SW")
        If tbtVW.Checked Then richtungen.Add("W")
        If tbtVNW.Checked Then richtungen.Add("NW")

        richtungsString = String.Join(";", richtungen)

        If richtungsString Is Nothing Then
            lblKeineRichtungInfo.Visible = True
        Else
            lblKeineRichtungInfo.Visible = False
        End If

        Return richtungsString

    End Function
End Class
