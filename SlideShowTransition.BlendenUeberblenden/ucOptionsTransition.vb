Imports System.Drawing
Imports System.Windows.Forms
Imports Transition_Überblenden.TransitionMain
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling
Imports SlideShowTools

Public Class ucOptionsTransition
    Inherits UserControl

#Region "Variablendeklaration"
    'Variablendeklaration

    Private aktuelleSettings As TransitionSettings_FadeCrossfade

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

#End Region

#Region "Initialisierung"

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert das Options-Control.

        CheckYourMail()
        InitialisiereControls()

    End Sub

    Private Sub InitialisiereControls()
        'Initialisiert sämtliche Steuerelemente aus den
        'aktuellen Settings ohne Direct Commit.

        wirdInitialisiert = True

        Try

            picFarbton.BackColor = aktuelleSettings.Farbton
            chkZufallsfarbe.Checked = aktuelleSettings.Zufallsfarbe
            chkMorphing.Checked = aktuelleSettings.Morphing

            Select Case aktuelleSettings.Modus

                Case "Fade"

                    rdoBlenden.Checked = True

                Case "Crossfade"

                    rdoÜberblenden.Checked = True

                Case "Zufall"

                    rdoZufall.Checked = True

            End Select

            trkGeschwindigkeit.Value = (trkGeschwindigkeit.Maximum + 1) - aktuelleSettings.Geschwindigkeit

            AktualisiereGeschwindigkeitText()
            AktualisiereOptionsStatus()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

#End Region

#Region "Optionsstatus"

    Private Sub AktualisiereOptionsStatus()
        'Aktualisiert Enabled-Status und Farbdarstellung
        'abhängig von Modus, Morphing und Zufallsfarbe.

        Dim fadeOptionenAktiv As Boolean

        fadeOptionenAktiv = rdoBlenden.Checked OrElse rdoZufall.Checked

        If Not fadeOptionenAktiv Then

            chkMorphing.Enabled = False
            chkZufallsfarbe.Enabled = False
            lblNpicFarbton.Enabled = False
            picFarbton.BackColor = Color.Transparent
            picFarbton.Enabled = False

            Exit Sub

        End If

        chkMorphing.Enabled = True

        If Not chkMorphing.Checked Then

            chkZufallsfarbe.Enabled = False
            lblNpicFarbton.Enabled = False
            picFarbton.BackColor = HintergrundFarbeSaver
            picFarbton.Enabled = False

            Exit Sub

        End If

        chkZufallsfarbe.Enabled = True

        If chkZufallsfarbe.Checked Then

            picFarbton.BackColor = Color.Transparent
            lblNpicFarbton.Enabled = False
            picFarbton.Enabled = False

        Else

            picFarbton.BackColor = aktuelleSettings.Farbton
            lblNpicFarbton.Enabled = True
            picFarbton.Enabled = True

        End If

    End Sub

#End Region

#Region "Modus"

    Private Sub rdoBlenden_CheckedChanged(sender As Object, e As EventArgs) Handles rdoBlenden.CheckedChanged

        If Not rdoBlenden.Checked Then
            Exit Sub
        End If

        AktualisiereOptionsStatus()

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.Modus = "Fade"

        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", aktuelleSettings.Modus)

    End Sub

    Private Sub rdoÜberblenden_CheckedChanged(sender As Object, e As EventArgs) Handles rdoÜberblenden.CheckedChanged

        If Not rdoÜberblenden.Checked Then
            Exit Sub
        End If

        AktualisiereOptionsStatus()

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.Modus = "Crossfade"

        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", aktuelleSettings.Modus)

    End Sub

    Private Sub rdoZufall_CheckedChanged(sender As Object, e As EventArgs) Handles rdoZufall.CheckedChanged

        If Not rdoZufall.Checked Then
            Exit Sub
        End If

        AktualisiereOptionsStatus()

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.Modus = "Zufall"

        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", aktuelleSettings.Modus)

    End Sub

#End Region

#Region "Fade-Einstellungen"

    Private Sub chkZufallsfarbe_CheckedChanged(sender As Object, e As EventArgs) Handles chkZufallsfarbe.CheckedChanged

        AktualisiereOptionsStatus()

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.Zufallsfarbe = chkZufallsfarbe.Checked

        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Zufallsfarbe",
                        aktuelleSettings.Zufallsfarbe.ToString())

    End Sub

    Private Sub chkMorphing_CheckedChanged(sender As Object, e As EventArgs) Handles chkMorphing.CheckedChanged

        AktualisiereOptionsStatus()

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.Morphing = chkMorphing.Checked

        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Morphing", aktuelleSettings.Morphing.ToString())

    End Sub

    Private Sub picFarbton_Click(
        sender As Object,
        e As EventArgs) _
        Handles picFarbton.Click
        'Öffnet die Farbauswahl für den Morph-Farbton.

        Dim bg As Color
        Dim inv As Color
        Dim custom(15) As Integer

        If wurdeBereinigt Then
            Exit Sub
        End If

        bg = HintergrundFarbeSaver
        inv = InvertSDColor(HintergrundFarbeSaver)

        custom(0) = ColorTranslator.ToOle(bg)
        custom(1) = ColorTranslator.ToOle(inv)

        Using dlg As New ColorDialog()

            dlg.AllowFullOpen = True
            dlg.AnyColor = True
            dlg.FullOpen = True
            dlg.Color = aktuelleSettings.Farbton
            dlg.CustomColors = custom

            If dlg.ShowDialog() <> DialogResult.OK Then

                Exit Sub

            End If

            aktuelleSettings.Farbton = dlg.Color

            picFarbton.BackColor = aktuelleSettings.Farbton

            WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Farbton",
                            ColorToString(aktuelleSettings.Farbton))

        End Using

    End Sub

#End Region

#Region "Geschwindigkeit"

    Private Sub trkGeschwindigkeit_ValueChanged(sender As Object, e As EventArgs) Handles trkGeschwindigkeit.ValueChanged

        AktualisiereGeschwindigkeitText()

        If wirdInitialisiert OrElse
           wurdeBereinigt Then

            Exit Sub

        End If

        aktuelleSettings.Geschwindigkeit = (trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value

        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Geschwindigkeit",
                        aktuelleSettings.Geschwindigkeit.ToString())

    End Sub

    Private Sub AktualisiereGeschwindigkeitText()
        'Aktualisiert die lesbare Daueranzeige.

        lblGeschwindigkeit.Text = ((trkGeschwindigkeit.Maximum + 1) - trkGeschwindigkeit.Value).ToString() & " s"

    End Sub

#End Region

#Region "Defaults"

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Übernimmt die Defaultwerte, speichert sie explizit
        'und initialisiert anschließend die Controls neu.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetTransitionDefaultSettings()

        aktuelleSettings.Farbton = ColorHandling.StringToColor(defaults("Farbton"))
        aktuelleSettings.Zufallsfarbe = defaults("Zufallsfarbe") = "True"
        aktuelleSettings.Morphing = defaults("Morphing") = "True"
        aktuelleSettings.Modus = defaults("Modus")
        aktuelleSettings.Geschwindigkeit = CInt(defaults("Geschwindigkeit"))

        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Farbton", defaults("Farbton"))
        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Zufallsfarbe", defaults("Zufallsfarbe"))
        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Morphing", defaults("Morphing"))
        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", defaults("Modus"))
        WriteToRegistry(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Geschwindigkeit", defaults("Geschwindigkeit"))

        InitialisiereControls()

    End Sub

#End Region

#Region "Settings"

    Private Sub CheckYourMail()
        'Liest aktuelleSettings aus der SettingsInbox.

        aktuelleSettings = GetSettings(Of TransitionSettings_FadeCrossfade)(nameTransition)

    End Sub

#End Region

#Region "Bereinigung"

    Private Sub ucOptionsTransition_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert weitere Aktionen nach der Freigabe.

        wurdeBereinigt = True

    End Sub

#End Region

End Class