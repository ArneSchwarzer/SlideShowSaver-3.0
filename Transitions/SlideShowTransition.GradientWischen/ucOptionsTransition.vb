Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTransition.GradientWischen.TransitionMain_GradientWischen.TransitionMain
Imports System.ComponentModel

Public Class ucOptionsTransition
    Inherits System.Windows.Forms.UserControl

    'Variablendeklaration
    Private aktuelleSettings As SlideShowTransitionSettings_GradientWischen

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Steuerelemente des Optionsdialogs.

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
            tbtNW.CheckChanged,
            tbtZOut.CheckChanged,
            tbtZIn.CheckChanged
        'Aktualisiert die Richtungswahl.

        RichtungStringBauen()

    End Sub

    Private Sub trkGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkGeschwindigkeit.ValueChanged
        'Behandelt die Transitionsgeschwindigkeit.

        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString() & " s"

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.geschwindigkeit = (trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value

        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Geschwindigkeit",
                        aktuelleSettings.geschwindigkeit.ToString())

    End Sub

    Private Sub trkBreite_ValueChanged(sender As Object, e As EventArgs) Handles trkBreite.ValueChanged
        'Behandelt die Breite des weichen Übergangs.

        lblBreiteProzent.Text = trkBreite.Value.ToString() & " %"

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.breite = trkBreite.Value

        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Breite", aktuelleSettings.breite.ToString())

    End Sub

    Private Sub RichtungStringBauen()
        'Ermittelt die aktuell markierten Richtungen,
        'aktualisiert UI und lokale Settings und speichert
        'die Auswahl bei Benutzeränderungen per Direct Commit.

        Dim richtung As New List(Of String)
        Dim richtungsString As String

        If tbtN.Checked Then
            richtung.Add("N")
        End If

        If tbtNO.Checked Then
            richtung.Add("NO")
        End If

        If tbtO.Checked Then
            richtung.Add("O")
        End If

        If tbtSO.Checked Then
            richtung.Add("SO")
        End If

        If tbtS.Checked Then
            richtung.Add("S")
        End If

        If tbtSW.Checked Then
            richtung.Add("SW")
        End If

        If tbtW.Checked Then
            richtung.Add("W")
        End If

        If tbtNW.Checked Then
            richtung.Add("NW")
        End If

        If tbtZOut.Checked Then
            richtung.Add("ZOut")
        End If

        If tbtZIn.Checked Then
            richtung.Add("ZIn")
        End If

        lblKeineRichtung.Visible = richtung.Count = 0

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.richtungen = New List(Of String)(richtung)

        richtungsString = String.Join(";", richtung)

        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Richtungen", richtungsString)

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SetttingsInbox

        aktuelleSettings = GetSettings(Of SlideShowTransitionSettings_GradientWischen)(nameTransition)

    End Sub

    Private Sub IniOrReinitialise()
        'Initialisiert sämtliche Controls aus aktuelleSettings.

        tbtN.Checked = False
        tbtNO.Checked = False
        tbtO.Checked = False
        tbtSO.Checked = False
        tbtS.Checked = False
        tbtSW.Checked = False
        tbtW.Checked = False
        tbtNW.Checked = False
        tbtZOut.Checked = False
        tbtZIn.Checked = False

        If aktuelleSettings.richtungen IsNot Nothing Then

            For Each richtung As String In aktuelleSettings.richtungen

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

                    Case "ZOut"
                        tbtZOut.Checked = True

                    Case "ZIn"
                        tbtZIn.Checked = True

                End Select

            Next

        End If

        If aktuelleSettings.richtungen Is Nothing Then

            lblKeineRichtung.Visible = True

        Else

            lblKeineRichtung.Visible = aktuelleSettings.richtungen.Count = 0

        End If

        trkBreite.Value = aktuelleSettings.breite
        lblBreiteProzent.Text = aktuelleSettings.breite.ToString() & " %"

        trkGeschwindigkeit.Value = (trkGeschwindigkeit.Maximum + 1) - aktuelleSettings.geschwindigkeit
        lblGeschwindigkeit.Text = aktuelleSettings.geschwindigkeit.ToString() & " s"

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Stellt die Defaultwerte wieder her und speichert
        'diese gemäß Direct-Commit-Architektur explizit.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetTransitionDefaultSettings()

        aktuelleSettings.geschwindigkeit = CInt(defaults("Geschwindigkeit"))
        aktuelleSettings.richtungen = SplitSemicolonList(defaults("Richtungen"))
        aktuelleSettings.breite = CInt(defaults("Breite"))

        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Geschwindigkeit", defaults("Geschwindigkeit"))
        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Richtungen", defaults("Richtungen"))
        WriteToRegistry(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Breite", defaults("Breite"))

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
