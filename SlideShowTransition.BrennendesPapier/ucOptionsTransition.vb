Imports System.Drawing
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

        tcBP.DrawMode = TabDrawMode.OwnerDrawFixed

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

        'Modi
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

        'Zündmodus
        Select Case aktuelleSettings.zuendmodus
            Case "Brandherde"
                rbZuendModusBrandherde.Checked = True
            Case "Brand vom Rand"
                rbZuendmodusBrandRand.Checked = True
            Case Else
                rbZuendmodusZufall.Checked = True
        End Select

        'Effekte
        chkPartikel.Checked = aktuelleSettings.partikel
        chkGradient.Checked = aktuelleSettings.gradient
        chkVerzerrung.Checked = aktuelleSettings.verzerrung
        chkTextur.Checked = aktuelleSettings.textur

        'Tabpage Partikel
        Select Case aktuelleSettings.schwerkraft
            Case "An"
                rbSchwerkraftAn.Checked = True
            Case "Aus"
                rbSchwerkraftAus.Checked = True
            Case Else
                rbSchwerkraftZufällig.Checked = True
        End Select

        trkPartikelLebensdauer.Value = Math.Max(trkPartikelLebensdauer.Minimum, Math.Min(trkPartikelLebensdauer.Maximum,
                                                CInt(Math.Round(aktuelleSettings.partikelLebensdauer * 10.0))))
        lblPartikelLebensdauer.Text = aktuelleSettings.partikelLebensdauer.ToString("0.0") & " s"

        'Tabpage Gradient
        trkBrandkantenbreite.Value = Math.Max(trkBrandkantenbreite.Minimum, Math.Min(trkBrandkantenbreite.Maximum,
                                              aktuelleSettings.brandkantenbreite))
        lblWertBrandkante.Text = aktuelleSettings.brandkantenbreite.ToString()

        'Tabpage Verzerrung
        trkVerzerrungsbreite.Value = Math.Max(trkVerzerrungsbreite.Minimum, Math.Min(trkVerzerrungsbreite.Maximum,
                                              aktuelleSettings.verzerrungsbreite))
        lblVerzerrungsbreite.Text = aktuelleSettings.verzerrungsbreite.ToString()

        trkVerzerrungsstaerke.Value = Math.Max(trkVerzerrungsstaerke.Minimum, Math.Min(trkVerzerrungsstaerke.Maximum,
                                               aktuelleSettings.verzerrungsstaerke))
        lblVerzerrungEffektstaerke.Text = aktuelleSettings.verzerrungsstaerke.ToString()

        trkScherbengroesse.Value = Math.Max(trkScherbengroesse.Minimum, Math.Min(trkScherbengroesse.Maximum,
                                            aktuelleSettings.magieScherbengroesse))
        lblScherbengroesse.Text = aktuelleSettings.magieScherbengroesse.ToString()

        'Transitionsdauer
        trkDauer.Value = Math.Max(trkDauer.Minimum, Math.Min(trkDauer.Maximum, aktuelleSettings.dauer))
        lblGeschwindigkeit.Text = aktuelleSettings.dauer.ToString() & " s"

        chkZufallsdauer.Checked = aktuelleSettings.zufallsdauer
        trkDauer.Enabled = Not aktuelleSettings.zufallsdauer
        lblGeschwindigkeit.Enabled = Not aktuelleSettings.zufallsdauer

        'Abhängige Controls
        AktualisierePartikelAbhaengigeControls()
        AktualisiereEffektTabStatus()

        '################################################################
        '#                                                              #
        '# Test- und Konfigurationsbereich                              #
        '#                                                              #
        '# Diese Controls sind nur für Test- und Konfiguratinsarbeiten  #
        '# gedacht. Standardmäßig sind sie entsprechend Disabled und    #
        '# auf nicht sichtbar gesetzt.                                  #
        '################################################################

        If tcBP.TabPages.Contains(tpKonfig) Then

            tcBP.TabPages.Remove(tpKonfig)

        End If

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

    Private Sub AktualisiereEffektTabStatus()
        'Aktiviert oder deaktiviert die effektabhängigen
        'Optionsseiten.

        tpPartikel.Enabled = chkPartikel.Checked
        tpGradient.Enabled = chkGradient.Checked
        tpVerzerrung.Enabled = chkVerzerrung.Checked
        tpTextur.Enabled = chkTextur.Checked

        If tcBP.SelectedTab IsNot Nothing AndAlso Not tcBP.SelectedTab.Enabled Then

            For Each tabPage As TabPage In tcBP.TabPages

                If tabPage.Enabled Then

                    tcBP.SelectedTab = tabPage

                    Exit For

                End If

            Next

        End If

    End Sub

    Private Sub AktualisierePartikelAbhaengigeControls()
        'Aktiviert oder deaktiviert Einstellungen,
        'die nur bei eingeschalteten Partikeln sinnvoll sind.

        grpSchwerkraft.Enabled = chkPartikel.Checked

    End Sub

    Private Sub tcBP_DrawItem(sender As Object, e As DrawItemEventArgs) Handles tcBP.DrawItem

        Dim tabPage As TabPage
        Dim textColor As Color
        Dim textFlags As TextFormatFlags

        tabPage = tcBP.TabPages(e.Index)

        If tabPage.Enabled Then
            textColor = SystemColors.ControlText
        Else
            textColor = SystemColors.GrayText
        End If

        If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
            e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds)
        Else
            e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds)
        End If

        e.Graphics.FillRectangle(SystemBrushes.Control, e.Bounds)

        textFlags = TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.SingleLine

        TextRenderer.DrawText(e.Graphics, tabPage.Text, tcBP.Font, e.Bounds, textColor, textFlags)

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

    Private Sub rbSchwerkraft_CheckedChanged(sender As Object, e As EventArgs) _
        Handles rbSchwerkraftAn.CheckedChanged,
                rbSchwerkraftAus.CheckedChanged,
                rbSchwerkraftZufällig.CheckedChanged

        'Speichert den gewählten Schwerkraftmodus per Direct Commit.

        Dim schwerkraft As String

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        If Not DirectCast(sender, RadioButton).Checked Then
            Exit Sub
        End If

        If rbSchwerkraftAn.Checked Then

            schwerkraft = "An"

        ElseIf rbSchwerkraftAus.Checked Then

            schwerkraft = "Aus"

        Else

            schwerkraft = "Zufällig"

        End If

        aktuelleSettings.schwerkraft = schwerkraft

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Schwerkraft", aktuelleSettings.schwerkraft)

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

    Private Sub trkFBMGrundfrequenz_ValueChanged(sender As Object, e As EventArgs) Handles trkFBMGrundfrequenz.ValueChanged


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

    Private Sub rbZuendmodus_CheckedChanged(sender As Object, e As EventArgs) _
        Handles rbZuendModusBrandherde.CheckedChanged,
                rbZuendmodusBrandRand.CheckedChanged,
                rbZuendmodusZufall.CheckedChanged
        'Speichert den gewählten Zündmodus per Direct Commit.

        Dim zuendmodus As String

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        If Not DirectCast(sender, RadioButton).Checked Then
            Exit Sub
        End If

        If rbZuendModusBrandherde.Checked Then
            zuendmodus = "Brandherde"
        ElseIf rbZuendmodusBrandRand.Checked Then
            zuendmodus = "Brand vom Rand"
        Else
            zuendmodus = "Zufällig"
        End If

        aktuelleSettings.zuendmodus = zuendmodus

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Zuendmodus", aktuelleSettings.zuendmodus)

    End Sub

    Private Sub chkEffekt_CheckedChanged(sender As Object, e As EventArgs) _
        Handles chkPartikel.CheckedChanged,
                chkGradient.CheckedChanged,
                chkVerzerrung.CheckedChanged,
                chkTextur.CheckedChanged
        'Speichert die aktivierten Einzeleffekte und aktualisiert
        'die zugehörigen Optionsseiten.

        AktualisiereEffektTabStatus()
        AktualisierePartikelAbhaengigeControls()

        'Die OwnerDraw-Tabreiter müssen ihren geänderten
        'Enabled-Status unmittelbar neu darstellen.
        tcBP.Invalidate()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.partikel = chkPartikel.Checked
        aktuelleSettings.gradient = chkGradient.Checked
        aktuelleSettings.verzerrung = chkVerzerrung.Checked
        aktuelleSettings.textur = chkTextur.Checked

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Partikel", aktuelleSettings.partikel.ToString())
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Gradient", aktuelleSettings.gradient.ToString())
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Verzerrung", aktuelleSettings.verzerrung.ToString())
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Textur", aktuelleSettings.textur.ToString())

    End Sub

    Private Sub trkPartikelLebensdauer_ValueChanged(sender As Object, e As EventArgs) Handles trkPartikelLebensdauer.ValueChanged

        Dim wert As Double

        wert = trkPartikelLebensdauer.Value / 10.0

        lblPartikelLebensdauer.Text = wert.ToString("0.0") & " s"

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.partikelLebensdauer = wert

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "PartikelLebensdauer",
                        aktuelleSettings.partikelLebensdauer.ToString(CultureInfo.InvariantCulture))

    End Sub

    Private Sub trkVerzerrungsbreite_ValueChanged(sender As Object, e As EventArgs) Handles trkVerzerrungsbreite.ValueChanged

        lblVerzerrungsbreite.Text = trkVerzerrungsbreite.Value.ToString()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.verzerrungsbreite = trkVerzerrungsbreite.Value

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Verzerrungsbreite",
                        aktuelleSettings.verzerrungsbreite.ToString())

    End Sub

    Private Sub trkVerzerrungsstaerke_ValueChanged(sender As Object, e As EventArgs) Handles trkVerzerrungsstaerke.ValueChanged

        lblVerzerrungEffektstaerke.Text = trkVerzerrungsstaerke.Value.ToString()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.verzerrungsstaerke = trkVerzerrungsstaerke.Value

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Verzerrungsstaerke",
                        aktuelleSettings.verzerrungsstaerke.ToString())

    End Sub

    Private Sub trkScherbengroesse_ValueChanged(sender As Object, e As EventArgs) Handles trkScherbengroesse.ValueChanged

        lblScherbengroesse.Text = trkScherbengroesse.Value.ToString()

        If wirdInitialisiert OrElse wurdeBereinigt Then
            Exit Sub
        End If

        aktuelleSettings.magieScherbengroesse = trkScherbengroesse.Value

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "MagieScherbengroesse",
                        aktuelleSettings.magieScherbengroesse.ToString())

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
        aktuelleSettings.schwerkraft = defaults("Schwerkraft")
        aktuelleSettings.brandkantenbreite = CInt(defaults("Brandkantenbreite"))
        aktuelleSettings.zuendmodus = defaults("Zuendmodus")
        aktuelleSettings.gradient = CBool(defaults("Gradient"))
        aktuelleSettings.verzerrung = CBool(defaults("Verzerrung"))
        aktuelleSettings.textur = CBool(defaults("Textur"))
        aktuelleSettings.partikelLebensdauer = Double.Parse(defaults("PartikelLebensdauer"), CultureInfo.InvariantCulture)
        aktuelleSettings.verzerrungsbreite = CInt(defaults("Verzerrungsbreite"))
        aktuelleSettings.verzerrungsstaerke = CInt(defaults("Verzerrungsstaerke"))
        aktuelleSettings.magieScherbengroesse = CInt(defaults("MagieScherbengroesse"))


        aktuelleSettings.fbmGrundfrequenz = Double.Parse(defaults("FBMGrundfrequenz"), CultureInfo.InvariantCulture)
        aktuelleSettings.fbmOktaven = CInt(defaults("FBMOktaven"))
        aktuelleSettings.fbmPersistenz = Double.Parse(defaults("FBMPersistenz"), CultureInfo.InvariantCulture)
        aktuelleSettings.fbmStaerke = Double.Parse(defaults("FBMStaerke"), CultureInfo.InvariantCulture)

        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Dauer", defaults("Dauer"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Zufallsdauer", defaults("Zufallsdauer"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Modi", defaults("Modi"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Partikel", defaults("Partikel"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Schwerkraft", defaults("Schwerkraft"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Brandkantenbreite", defaults("Brandkantenbreite"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Zuendmodus", defaults("Zuendmodus"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Gradient", defaults("Gradient"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Verzerrung", defaults("Verzerrung"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Textur", defaults("Textur"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "PartikelLebensdauer", defaults("PartikelLebensdauer"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Verzerrungsbreite", defaults("Verzerrungsbreite"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "Verzerrungsstaerke", defaults("Verzerrungsstaerke"))
        WriteToRegistry(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH & "MagieScherbengroesse", defaults("MagieScherbengroesse"))

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