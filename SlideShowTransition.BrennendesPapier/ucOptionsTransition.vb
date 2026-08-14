Imports System.Globalization
Imports System.Windows.Forms
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTransition.BrennendesPapier.TransitionMain

Public Class ucOptionsTransition
    Inherits System.Windows.Forms.UserControl

#Region "Variablendeklaration"

    Private aktuelleSettings As SlideShowTransitionSettings_BrennendesPapier

    Private wirdInitialisiert As Boolean = True
    Private wurdeBereinigt As Boolean

#End Region

#Region "Initialisierung"

    Private Sub ucOptionsTransition_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert sämtliche Steuerelemente aus den
        'aktuellen Transitionseinstellungen.

        CheckYourMail()

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SettingsInbox.

        aktuelleSettings = GetSettings(Of SlideShowTransitionSettings_BrennendesPapier)(nameTransition)

    End Sub

    Private Sub IniOrReinitialise()
        'Initialisiert sämtliche Controls aus aktuelleSettings.

        Dim index As Integer
        Dim modus As String

        For index = 0 To clbModus.Items.Count - 1

            clbModus.SetItemChecked(index, False)

        Next

        If aktuelleSettings.modi IsNot Nothing Then

            For Each modus In aktuelleSettings.modi

                For index = 0 To clbModus.Items.Count - 1

                    If String.Equals(
                        clbModus.Items(index).ToString(),
                        modus,
                        StringComparison.OrdinalIgnoreCase) Then

                        clbModus.SetItemChecked(index, True)

                        Exit For

                    End If

                Next

            Next

        End If

        If aktuelleSettings.modi Is Nothing Then

            lblKeineRichtungInfo.Visible = True

        Else

            lblKeineRichtungInfo.Visible = aktuelleSettings.modi.Count = 0

        End If

        chkPartikel.Checked = aktuelleSettings.partikel

        trkDauer.Value = Math.Max(trkDauer.Minimum, Math.Min(trkDauer.Maximum, aktuelleSettings.dauer))
        lblGeschwindigkeit.Text = aktuelleSettings.dauer.ToString() & " s"

        chkZufallsdauer.Checked = aktuelleSettings.zufallsdauer
        trkDauer.Enabled = Not aktuelleSettings.zufallsdauer
        lblGeschwindigkeit.Enabled = Not aktuelleSettings.zufallsdauer

        trkBrandkantenbreite.Value = Math.Max(trkBrandkantenbreite.Minimum,
                                              Math.Min(trkBrandkantenbreite.Maximum,
                                                       aktuelleSettings.brandkantenbreite))
        lblWertBrandkante.Text = aktuelleSettings.brandkantenbreite.ToString()

        trkFBMGrundfrequenz.Value = Math.Max(trkFBMGrundfrequenz.Minimum, Math.Min(trkFBMGrundfrequenz.Maximum,
                                             CInt(Math.Round(aktuelleSettings.fbmGrundfrequenz * 2.0))))
        lblFBMGrundfrequenz.Text = aktuelleSettings.fbmGrundfrequenz.ToString("0.0")

        trkFBMOktaven.Value = Math.Max(trkFBMOktaven.Minimum, Math.Min(trkFBMOktaven.Maximum, aktuelleSettings.fbmOktaven))
        lblFBMOktaven.Text = aktuelleSettings.fbmOktaven.ToString()

        trkFBMPersistenz.Value = Math.Max(trkFBMPersistenz.Minimum, Math.Min(trkFBMPersistenz.Maximum,
                                          CInt(Math.Round(aktuelleSettings.fbmPersistenz * 100.0))))
        lblFBMPersistenz.Text = aktuelleSettings.fbmPersistenz.ToString("0.00")

        trkFBMStaerke.Value = Math.Max(trkFBMStaerke.Minimum, Math.Min(trkFBMStaerke.Maximum, CInt(Math.Round(
                                       aktuelleSettings.fbmStaerke * 100.0))))
        lblFBMStaerke.Text = aktuelleSettings.fbmStaerke.ToString("0.00")

    End Sub

#End Region

#Region "Direct Commit"

    Private Sub clbModus_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbModus.ItemCheck
        'Speichert die aktuell gewählten Modi per Direct Commit.

        Dim modi As New List(Of String)
        Dim index As Integer
        Dim checkedState As Boolean
        Dim modiString As String

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        For index = 0 To clbModus.Items.Count - 1

            If index = e.Index Then

                checkedState = e.NewValue = CheckState.Checked

            Else

                checkedState = clbModus.GetItemChecked(index)

            End If

            If checkedState Then

                modi.Add(clbModus.Items(index).ToString())

            End If

        Next

        aktuelleSettings.modi = New List(Of String)(modi)

        lblKeineRichtungInfo.Visible = modi.Count = 0

        modiString = String.Join(";", modi)

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Modi", modiString)

    End Sub

    Private Sub chkPartikel_CheckedChanged(sender As Object, e As EventArgs) Handles chkPartikel.CheckedChanged
        'Speichert die Partikel-Einstellung per Direct Commit.

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.partikel = chkPartikel.Checked

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Partikel",
                        aktuelleSettings.partikel.ToString())

    End Sub

    Private Sub trkDauer_ValueChanged(sender As Object, e As EventArgs) Handles trkDauer.ValueChanged
        'Speichert die gewünschte Übergangsdauer.

        lblGeschwindigkeit.Text = trkDauer.Value.ToString() & " s"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.dauer = trkDauer.Value

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Dauer", aktuelleSettings.dauer.ToString())

    End Sub

    Private Sub trkFBMGrundfrequenz_ValueChanged(sender As Object, e As EventArgs) _
        Handles trkFBMGrundfrequenz.ValueChanged

        Dim wert As Double

        wert = trkFBMGrundfrequenz.Value / 2.0

        lblFBMGrundfrequenz.Text = wert.ToString("0.0")

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.fbmGrundfrequenz = wert

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "FBMGrundfrequenz",
                        aktuelleSettings.fbmGrundfrequenz.ToString(CultureInfo.InvariantCulture))

    End Sub

    Private Sub trkFBMOktaven_ValueChanged(sender As Object, e As EventArgs) Handles trkFBMOktaven.ValueChanged

        lblFBMOktaven.Text = trkFBMOktaven.Value.ToString()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.fbmOktaven = trkFBMOktaven.Value

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "FBMOktaven",
                        aktuelleSettings.fbmOktaven.ToString())

    End Sub

    Private Sub trkFBMPersistenz_ValueChanged(sender As Object, e As EventArgs) Handles trkFBMPersistenz.ValueChanged

        Dim wert As Double

        wert = trkFBMPersistenz.Value / 100.0
        lblFBMPersistenz.Text = wert.ToString("0.00")

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.fbmPersistenz = wert

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "FBMPersistenz",
                        aktuelleSettings.fbmPersistenz.ToString(CultureInfo.InvariantCulture))

    End Sub

    Private Sub trkFBMStaerke_ValueChanged(sender As Object, e As EventArgs) Handles trkFBMStaerke.ValueChanged

        Dim wert As Double

        wert = trkFBMStaerke.Value / 100.0

        lblFBMStaerke.Text = wert.ToString("0.00")

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.fbmStaerke = wert

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "FBMStaerke",
                        aktuelleSettings.fbmStaerke.ToString(CultureInfo.InvariantCulture))

    End Sub

    Private Sub chkZufallsdauer_CheckedChanged(sender As Object, e As EventArgs) Handles chkZufallsdauer.CheckedChanged
        'Aktiviert oder deaktiviert eine zufällige Übergangsdauer.

        trkDauer.Enabled = Not chkZufallsdauer.Checked
        lblGeschwindigkeit.Enabled = Not chkZufallsdauer.Checked

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.zufallsdauer = chkZufallsdauer.Checked

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Zufallsdauer",
                        aktuelleSettings.zufallsdauer.ToString())

    End Sub

    Private Sub trkBrandkantenbreite_ValueChanged(sender As Object, e As EventArgs) Handles trkBrandkantenbreite.ValueChanged
        'Speichert die gewünschte Breite der Brandkante.

        lblWertBrandkante.Text = trkBrandkantenbreite.Value.ToString()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.brandkantenbreite = trkBrandkantenbreite.Value

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Brandkantenbreite",
                        aktuelleSettings.brandkantenbreite.ToString())

    End Sub

#End Region

#Region "Defaultwerte"

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Stellt die Defaultwerte wieder her und speichert sie
        'gemäß Direct-Commit-Architektur explizit.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = GetTransitionDefaultSettings()


        aktuelleSettings.dauer = CInt(defaults("Dauer"))
        aktuelleSettings.zufallsdauer = CBool(defaults("Zufallsdauer"))
        aktuelleSettings.modi = SplitSemicolonList(defaults("Modi"))
        aktuelleSettings.partikel = CBool(defaults("Partikel"))
        aktuelleSettings.brandkantenbreite = CInt(defaults("Brandkantenbreite"))

        aktuelleSettings.fbmGrundfrequenz = Double.Parse(defaults("FBMGrundfrequenz"), CultureInfo.InvariantCulture)
        aktuelleSettings.fbmOktaven = CInt(defaults("FBMOktaven"))
        aktuelleSettings.fbmPersistenz = Double.Parse(defaults("FBMPersistenz"), CultureInfo.InvariantCulture)
        aktuelleSettings.fbmStaerke = Double.Parse(defaults("FBMStaerke"), CultureInfo.InvariantCulture)

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Dauer", defaults("Dauer"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Zufallsdauer", defaults("Zufallsdauer"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Modi", defaults("Modi"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Partikel", defaults("Partikel"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Brandkantenbreite", defaults("Brandkantenbreite"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "FBMGrundfrequenz", defaults("FBMGrundfrequenz"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "FBMOktaven", defaults("FBMOktaven"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "FBMPersistenz", defaults("FBMPersistenz"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "FBMStaerke", defaults("FBMStaerke"))

        wirdInitialisiert = True

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

#End Region

#Region "Bereinigung"

    Private Sub ucOptionsTransition_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert Direct-Commit-Aktionen nach der Freigabe.

        wurdeBereinigt = True

    End Sub

#End Region

End Class