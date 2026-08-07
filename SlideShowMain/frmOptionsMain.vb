Imports SlideShowBildauswahl
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLoader
Imports SlideShowLogging
Imports SlideShowSprachen
Imports SlideShowTools
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.CursorHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.ToolTipHandling

Public Class frmOptionsMain

#Region "Variablendeklaration"
    'Variablendeklaration

    'Module
    Private markierteModule As String
    Private aktuellGeladenesModul As ISlideShowModul = Nothing

    'Transitionen
    Private transitionList As List(Of SlideShowTransitionInfo)
    Private markierteTransitionen As String
    Private aktuellGeladeneTransition As ISlideShowTransition = Nothing

    'Shader
    Private aktuellGeladenerShader As ISlideShowShader = Nothing

    'TabOptions-Logik
    Private tpModulIstSichtbar As Boolean = False
    Private tpBildauswahlIstSichtbar As Boolean = False
    Private tpTransitionsIstSichtbar As Boolean = False
    Private tpShaderIstSichtbar As Boolean = False

    'Gehostete Options-UserControls
    Private modulOptionsControl As UserControl
    Private transitionOptionsControl As UserControl
    Private shaderOptionsControl As UserControl
    Private bildauswahlOptionsControl As UserControl
    Private sprachenOptionsControl As UserControl

    'Formularbereinigung & Initialisierungssteuerung
    Private wirdInitialisiert As Boolean
    Private formularWurdeBereinigt As Boolean

    'Sonstiges
    Private aktuelleSettings As SettingsMain
#End Region

#Region "Formular-Lebenszyklus"

    Private Sub frmOptionsMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.TopMost = True
        Me.BringToFront()

        isInputLocked = True
        optionsDialogIsActive = True

        CursorPowerShow()

        CheckYourMail()

        wirdInitialisiert = True

        Try

            IniOrReinitialise()

            sprachenOptionsControl = New ucFlaggenstreifen()
            sprachenOptionsControl.Dock = DockStyle.Fill

            pnlLanguages.Controls.Add(sprachenOptionsControl)

            StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                   tpShaderIstSichtbar)

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub frmOptionsMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        'Bereinigt den Optionsdialog und gibt die Eingabesteuerung frei.

        BereinigeOptionsdialog()

        isInputLocked = False
        optionsDialogIsActive = False

        Me.DialogResult = DialogResult.OK

        Cursor.Hide()

    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        'Da eine DirectCommit-Architektur vorliegt, ist keine 'Sonderbehandlung' des OK-Buttons notwendig
        Close()
    End Sub

#End Region

#Region "Initialisierung und Settings"

    Private Sub CheckYourMail()
        'aktuelleSettings aus der SettingsInbox abholen

        aktuelleSettings = GetSettings(Of SettingsMain)("Main")

    End Sub

    Private Sub IniOrReinitialise()
        ' Modul-Liste laden
        Dim anzahlMarkierteModule As Integer
        Dim moduleInfos = ModulListLoader.LadeModulInfoListe()

        clbModule.Items.Clear()

        ' ModulTransition Liste vorbereiten
        transitionList = TransitionListLoader.LadeTransitionInfoListe()

        ' Trackbar trkDauerModulwechsel und dazugehöriges Label lblDauerModulwechsel
        trkDauerModulwechsel.Minimum = 1
        trkDauerModulwechsel.Maximum = 60
        trkDauerModulwechsel.Value = aktuelleSettings.ModulDauer
        If trkDauerModulwechsel.Value = 60 Then
            lblDauerModuswechsel.Text = "1 h"
        Else
            lblDauerModuswechsel.Text = trkDauerModulwechsel.Value.ToString & " m"
        End If

        ' Modulliste clbModule
        clbModule.Items.Clear()
        For Each modulInfo In moduleInfos
            clbModule.Items.Add(modulInfo)
        Next

        EnableToolTipsForCLB(clbModule)

        markierteModule = JoinSemicolonList(aktuelleSettings.ModulAktivListe)
        SetCheckedItemsByName(Of SlideShowModulInfo)(
            clbModule,
            markierteModule,
            Function(m) m.ModulName
            )

        clbModule.Sorted = True


        'Combobox cmbModulwechsel

        cmbModulwechsel.SelectedItem = aktuelleSettings.ModulReihenfolge

        If cmbModulwechsel.SelectedIndex = 0 OrElse cmbModulwechsel.SelectedIndex = 2 Then '"Zufällig bei Start" oder "In Reihenfolge bei Start"
            lblNtrkDauerModulwechsel.Enabled = False
            lblDauerModuswechsel.Enabled = False
            trkDauerModulwechsel.Enabled = False
        Else
            lblNtrkDauerModulwechsel.Enabled = True
            lblDauerModuswechsel.Enabled = True
            trkDauerModulwechsel.Enabled = True
        End If

        'Label lblKeineModule
        anzahlMarkierteModule = clbModule.CheckedItems.Count
        KeineModuleLabelLogik(anzahlMarkierteModule)

        'Checklistbox clbTransitionsModule
        If transitionList.Count = 0 Then
            clbTransitionsModule.Enabled = False
            lblNclbTransitionsModule.Enabled = False
            cmbTransitionsReihenfolge.Enabled = False
            lblNcmbAbspielmodusTransitionsModule.Enabled = False
            lblKeineTransitionsModule.Visible = True
        Else
            lblKeineTransitionsModule.Visible = False
            clbTransitionsModule.Items.Clear()
            For Each transition In transitionList
                clbTransitionsModule.Items.Add(transition)
            Next

            EnableToolTipsForCLB(clbTransitionsModule)

            markierteTransitionen = JoinSemicolonList(aktuelleSettings.ModulTransitionListe)
            SetCheckedItemsByName(Of SlideShowTransitionInfo)(
            clbTransitionsModule,
            markierteTransitionen,
            Function(m) m.TransitionName
            )

            clbTransitionsModule.Sorted = True
        End If

        'Combobox cmbTransitionsReihenfolge
        cmbTransitionsReihenfolge.SelectedItem = aktuelleSettings.ModulTransitionReihenfolge

        If clbTransitionsModule.CheckedItems.Count <= 1 Then
            cmbTransitionsReihenfolge.Enabled = False
            lblNcmbAbspielmodusTransitionsModule.Enabled = False
        Else
            cmbTransitionsReihenfolge.Enabled = True
            lblNcmbAbspielmodusTransitionsModule.Enabled = True
        End If

        'Checkbox MultiMonitor Support
        'Solange noch kein MultiMonitor Support implementiert ist
        '
        'chkMultiMonitor.Checked = aktuelleSettings.MultiMonitor

        chkMultiMonitor.Enabled = False
        chkMultiMonitor.Checked = False

        'Hintergrundfarbe für den Schoner

        picHintergrundfarbe.BackColor = aktuelleSettings.Hintergrundfarbe

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Default-Werte übernehmen, speichern und Steuerelemente aktualisieren

        Dim defaults As Dictionary(Of String, String)

        'Defaults einlesen
        defaults = SaverMain.GetMainDefaultSettings()

        'Aktuelle Settings aktualisieren
        aktuelleSettings.ModulDauer = CInt(defaults("ModulDauer"))
        aktuelleSettings.ModulReihenfolge = defaults("ModulReihenfolge")
        aktuelleSettings.ModulAktivListe = SplitSemicolonList(defaults("ModulAktivListe"))
        aktuelleSettings.MultiMonitor = CBool(defaults("MultiMonitor"))
        aktuelleSettings.ModulTransitionListe = SplitSemicolonList(defaults("ModulTransitionListe"))
        aktuelleSettings.ModulTransitionReihenfolge = defaults("ModulTransitionReihenfolge")
        aktuelleSettings.Hintergrundfarbe = StringToColor(defaults("Hintergrundfarbe"))

        'Defaults gemäß Direct-Commit-Architektur speichern
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulDauer", defaults("ModulDauer"))
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulReihenfolge", defaults("ModulReihenfolge"))
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulAktivListe", defaults("ModulAktivListe"))
        WriteToRegistry(SLIDESHOWMAIN_PATH & "MultiMonitor", defaults("MultiMonitor"))
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionListe", defaults("ModulTransitionListe"))
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", defaults("ModulTransitionReihenfolge"))
        WriteToRegistry(SLIDESHOWMAIN_PATH & "Hintergrundfarbe", defaults("Hintergrundfarbe"))

        'Steuerelemente neu setzen
        wirdInitialisiert = True

        Try

            IniOrReinitialise()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

#End Region

#Region "TabPage- und Options-Control-Verwaltung"

    Private Sub StelleTabPagesZusammen(modulSichtbar As Boolean, bildauswahlSichtbar As Boolean,
                                       transitionSichtbar As Boolean, shaderSichtbar As Boolean)
        'Zeigt die TabPages in der korrekten Reihenfolge an, weil das "%)=§$-TabOptions Steuerelement dafür ja leider
        'zu blöde ist.
        Dim aufrufendeTabPage As TabPage
        Dim transitionCommunication As ISlideShowTransitionCommunication
        Dim shaderCommunication As ISlideShowShaderCommunication

        aufrufendeTabPage = tabOptions.SelectedTab

        'TabPages außer tpAllgemein ausschalten
        If aufrufendeTabPage IsNot tpModul Then
            tabOptions.TabPages.Remove(tpModul)
        End If

        tabOptions.TabPages.Remove(tpBildauswahl)
        tabOptions.TabPages.Remove(tpTransitions)
        tabOptions.TabPages.Remove(tpShader)

        If aufrufendeTabPage IsNot tpModul Then

            If modulSichtbar AndAlso modulOptionsControl IsNot Nothing Then

                If Not tpModul.Controls.Contains(modulOptionsControl) Then

                    modulOptionsControl.Dock = DockStyle.Fill
                    tpModul.Controls.Add(modulOptionsControl)

                End If

                If Not tabOptions.TabPages.Contains(tpModul) Then

                    tabOptions.TabPages.Add(tpModul)

                End If

                EntferneModulOptionsHandler()

                transitionCommunication = TryCast(modulOptionsControl, ISlideShowTransitionCommunication)
                shaderCommunication = TryCast(modulOptionsControl, ISlideShowShaderCommunication)

                If transitionCommunication IsNot Nothing Then

                    AddHandler transitionCommunication.PleaseChangeToTransition, AddressOf Modul_BitteWechseleZuTransition

                End If

                If shaderCommunication IsNot Nothing Then

                    AddHandler shaderCommunication.PleaseChangeToShader, AddressOf Modul_BitteWechseleZuShader

                End If

            Else

                tabOptions.TabPages.Remove(tpModul)

            End If

        End If

        If bildauswahlSichtbar Then

            If bildauswahlOptionsControl Is Nothing Then

                bildauswahlOptionsControl = New ucOptionsBildauswahl()
                bildauswahlOptionsControl.Dock = DockStyle.Fill

            End If

            If Not tpBildauswahl.Controls.Contains(bildauswahlOptionsControl) Then

                tpBildauswahl.Controls.Add(bildauswahlOptionsControl)

            End If

            If Not tabOptions.TabPages.Contains(tpBildauswahl) Then

                tabOptions.TabPages.Add(tpBildauswahl)

            End If

        Else

            tabOptions.TabPages.Remove(tpBildauswahl)

        End If

        If transitionSichtbar AndAlso transitionOptionsControl IsNot Nothing Then

            If Not tpTransitions.Controls.Contains(transitionOptionsControl) Then

                transitionOptionsControl.Dock = DockStyle.Fill
                tpTransitions.Controls.Add(transitionOptionsControl)

            End If

            If Not tabOptions.TabPages.Contains(tpTransitions) Then

                tabOptions.TabPages.Add(tpTransitions)

            End If

        Else

            tabOptions.TabPages.Remove(tpTransitions)

        End If

        If shaderSichtbar AndAlso shaderOptionsControl IsNot Nothing Then

            If Not tpShader.Controls.Contains(shaderOptionsControl) Then

                shaderOptionsControl.Dock = DockStyle.Fill
                tpShader.Controls.Add(shaderOptionsControl)

            End If

            If Not tabOptions.TabPages.Contains(tpShader) Then

                tabOptions.TabPages.Add(tpShader)

            End If

        Else

            tabOptions.TabPages.Remove(tpShader)

        End If

        If aufrufendeTabPage IsNot Nothing AndAlso tabOptions.TabPages.Contains(aufrufendeTabPage) Then

            tabOptions.SelectedTab = aufrufendeTabPage

        ElseIf tabOptions.TabPages.Contains(tpAllgemein) Then

            tabOptions.SelectedTab = tpAllgemein

        ElseIf tabOptions.TabPages.Count > 0 Then

            tabOptions.SelectedIndex = 0

        End If

    End Sub

    Private Sub EntferneUndDisposeOptionsControl(ByRef optionsControl As UserControl, parentControl As Control)
        'Entfernt ein Options-Control aus seinem Host,
        'disposed es und löscht die Referenz.

        If optionsControl Is Nothing Then
            Exit Sub
        End If

        Try

            If parentControl IsNot Nothing AndAlso
               parentControl.Controls.Contains(optionsControl) Then

                parentControl.Controls.Remove(optionsControl)

            End If

            optionsControl.Dispose()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Freigeben eines Options-UserControls: " & ex.ToString())

        Finally

            optionsControl = Nothing

        End Try

    End Sub

    Private Sub EntferneModulOptionsHandler()
        'Entfernt die von frmOptionsMain registrierten
        'Kommunikationshandler des Modul-Options-Controls.

        Dim transitionCommunication As ISlideShowTransitionCommunication
        Dim shaderCommunication As ISlideShowShaderCommunication

        transitionCommunication = Nothing
        shaderCommunication = Nothing

        If modulOptionsControl Is Nothing Then
            Exit Sub
        End If

        transitionCommunication =
            TryCast(
                modulOptionsControl,
                ISlideShowTransitionCommunication)

        If transitionCommunication IsNot Nothing Then

            RemoveHandler transitionCommunication.PleaseChangeToTransition, AddressOf Modul_BitteWechseleZuTransition

        End If

        shaderCommunication =
            TryCast(
                modulOptionsControl,
                ISlideShowShaderCommunication)

        If shaderCommunication IsNot Nothing Then

            RemoveHandler shaderCommunication.PleaseChangeToShader, AddressOf Modul_BitteWechseleZuShader

        End If

    End Sub

#End Region

#Region "Modulauswahl und Moduloptionen"

    Private Sub clbModule_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbModule.ItemCheck

        If wirdInitialisiert OrElse formularWurdeBereinigt Then

            Exit Sub

        End If

        Try

            BeginInvoke(New Action(AddressOf SpeichereModulauswahl))

        Catch ex As InvalidOperationException

            LogHandling.LogError("Fehler beim Einplanen der Modulauswahl: " & ex.ToString())

        End Try

    End Sub

    Private Sub SpeichereModulauswahl()
        'Speichert die vollständig aktualisierte Modulauswahl.

        Dim anzahlMarkierteModule As Integer

        If wirdInitialisiert OrElse formularWurdeBereinigt OrElse IsDisposed OrElse Disposing Then

            Exit Sub

        End If

        anzahlMarkierteModule = clbModule.CheckedItems.Count

        KeineModuleLabelLogik(anzahlMarkierteModule)

        CheckedListBoxHandling.SaveListBoxToRegistry(clbModule, SLIDESHOWMAIN_PATH & "ModulAktivListe")

    End Sub

    Private Sub clbModule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbModule.SelectedIndexChanged
        'Wechselt die Dummyinstanz und den Optionsdialog
        'des aktuell ausgewählten Moduls.

        Dim modulName As String

        modulName = Nothing

        Try

            BeendeUndBereinigeDummyModul()

            If clbModule.SelectedItem Is Nothing Then

                tpModulIstSichtbar = False
                tpBildauswahlIstSichtbar = False
                tpTransitionsIstSichtbar = False
                tpShaderIstSichtbar = False

                StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                       tpShaderIstSichtbar)

                Exit Sub

            End If

            modulName = clbModule.SelectedItem.ToString()
            aktuellGeladenesModul = ModulByNameLoader.LadeModulNachName(modulName)

            If aktuellGeladenesModul Is Nothing Then

                tpModulIstSichtbar = False
                tpBildauswahlIstSichtbar = False
                tpTransitionsIstSichtbar = False
                tpShaderIstSichtbar = False

                LogHandling.LogWarn("Das Modul """ & modulName & """ konnte für den Optionsdialog nicht " &
                                    "geladen werden.")

                StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                       tpShaderIstSichtbar)

                Exit Sub

            End If

            modulOptionsControl = aktuellGeladenesModul.GetModulOptionsDialog()

            tpModulIstSichtbar = (modulOptionsControl IsNot Nothing)

            tpBildauswahlIstSichtbar = aktuellGeladenesModul.ModulNutztSlideShowBildauswahl

            If Not aktuellGeladenesModul.ModulNutztShader Then

                tpShaderIstSichtbar = False
                BeendeUndBereinigeDummyShader()

            End If

            If Not aktuellGeladenesModul.ModulNutztTransitions AndAlso
                    clbTransitionsModule.SelectedIndex < 0 Then

                tpTransitionsIstSichtbar = False
                BeendeUndBereinigeDummyTransition()

            End If

            StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                   tpShaderIstSichtbar)

        Catch ex As Exception

            LogHandling.LogError("Fehler in clbModule_SelectedIndexChanged: " & ex.ToString())

            BeendeUndBereinigeDummyModul()
            BeendeUndBereinigeDummyTransition()
            BeendeUndBereinigeDummyShader()

            tpModulIstSichtbar = False
            tpBildauswahlIstSichtbar = False
            tpTransitionsIstSichtbar = False
            tpShaderIstSichtbar = False

            StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                   tpShaderIstSichtbar)

        End Try

    End Sub

    Private Sub KeineModuleLabelLogik(anzahlMarkierteModule As Integer)
        If clbModule.Items.Count = 0 Then

            lblKeineModule.Text = "Keine Module geladen, spiele Bouncing Logo"
            lblKeineModule.Visible = True
            cmbModulwechsel.SelectedIndex = -1
            cmbModulwechsel.Enabled = False
            lblNcmbModulWechsel.Enabled = False
            trkDauerModulwechsel.Visible = False
            lblNtrkDauerModulwechsel.Visible = False
            lblDauerModuswechsel.Visible = False

        ElseIf anzahlMarkierteModule <= 1 Then

            If anzahlMarkierteModule = 0 Then
                lblKeineModule.Text = "Keine Module ausgewählt, spiele Bouncing Logo"
                lblKeineModule.Visible = True
                cmbModulwechsel.SelectedIndex = -1
            Else
                lblKeineModule.Visible = False
            End If

            If cmbModulwechsel.SelectedIndex = 0 Or cmbModulwechsel.SelectedIndex = 1 Then 'Falls "Zufällig bei Start" oder "Zufällig" --> "Zufällig bei Start"
                cmbModulwechsel.SelectedIndex = 0
            Else 'Sonst "In Reihenfolge bei Start"
                cmbModulwechsel.SelectedIndex = 2
            End If
            cmbModulwechsel.Enabled = False
            lblNcmbModulWechsel.Enabled = False
            trkDauerModulwechsel.Visible = False
            lblNtrkDauerModulwechsel.Visible = False
            lblDauerModuswechsel.Visible = False

        Else
            lblKeineModule.Visible = False
            cmbModulwechsel.Enabled = True
            lblNcmbModulWechsel.Enabled = True
            trkDauerModulwechsel.Visible = True
            lblNtrkDauerModulwechsel.Visible = True
            lblDauerModuswechsel.Visible = True

            If trkDauerModulwechsel.Value = 60 Then
                lblDauerModuswechsel.Text = "1 h"
            Else
                lblDauerModuswechsel.Text = trkDauerModulwechsel.Value.ToString & " m"
            End If
        End If

    End Sub

#End Region

#Region "Transitionauswahl und Transitionoptionen"

    Private Sub Modul_BitteWechseleZuTransition(sender As Object, transitionName As String)
        'Lädt die gewünschte Transition samt Options-Control.

        Try

            BeendeUndBereinigeDummyTransition()

            aktuellGeladeneTransition = TransitionByNameLoader.LadeTransitionNachName(transitionName)

            If aktuellGeladeneTransition Is Nothing Then

                tpTransitionsIstSichtbar = False

                LogHandling.LogWarn("Die Transition """ & transitionName & """ konnte für den Optionsdialog " &
                                    "nicht geladen werden.")

                StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                       tpShaderIstSichtbar)

                Exit Sub

            End If

            transitionOptionsControl = aktuellGeladeneTransition.GetTransitionOptionsDialog()
            tpTransitionsIstSichtbar = (transitionOptionsControl IsNot Nothing)

            StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                   tpShaderIstSichtbar)

            If sender IsNot clbTransitionsModule AndAlso
                clbTransitionsModule.SelectedItem?.ToString() <> transitionName Then

                clbTransitionsModule.SelectedIndex = -1

            End If

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Umschalten auf Transition """ & transitionName & """: " & ex.ToString())

            BeendeUndBereinigeDummyTransition()

            tpTransitionsIstSichtbar = False

            StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                   tpShaderIstSichtbar)

        End Try

    End Sub

    Private Sub clbTransitionsModule_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbTransitionsModule.ItemCheck

        If wirdInitialisiert OrElse formularWurdeBereinigt Then

            Exit Sub

        End If

        Try

            BeginInvoke(New Action(AddressOf SpeichereModulTransitionsAuswahl))

        Catch ex As InvalidOperationException

            LogHandling.LogError("Fehler beim Einplanen der Modul-Transitionsauswahl: " & ex.ToString())

        End Try

    End Sub

    Private Sub SpeichereModulTransitionsAuswahl()
        'Speichert die vollständig aktualisierte Transitionauswahl.

        If wirdInitialisiert OrElse formularWurdeBereinigt OrElse IsDisposed OrElse Disposing Then

            Exit Sub

        End If

        CheckedListBoxHandling.SaveListBoxToRegistry(clbTransitionsModule, SLIDESHOWMAIN_PATH &
                                                     "ModulTransitionListe")

        If clbTransitionsModule.CheckedItems.Count <= 1 Then

            cmbTransitionsReihenfolge.Enabled = False
            lblNcmbAbspielmodusTransitionsModule.Enabled = False

        Else

            cmbTransitionsReihenfolge.Enabled = True
            lblNcmbAbspielmodusTransitionsModule.Enabled = True

        End If

    End Sub

    Private Sub clbTransitionsModule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbTransitionsModule.SelectedIndexChanged

        If clbTransitionsModule.SelectedItem IsNot Nothing Then
            Modul_BitteWechseleZuTransition(clbTransitionsModule, clbTransitionsModule.SelectedItem.ToString)
        End If

    End Sub

    Private Sub cmbTransitionsReihenfolge_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTransitionsReihenfolge.SelectedIndexChanged

        If wirdInitialisiert OrElse formularWurdeBereinigt OrElse cmbTransitionsReihenfolge.SelectedIndex < 0 Then

            Exit Sub

        End If

        'DirectCommit
        If cmbTransitionsReihenfolge.SelectedIndex >= 0 Then
            WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", cmbTransitionsReihenfolge.SelectedItem.ToString)
        End If

    End Sub

#End Region

#Region "Shaderauswahl und Shaderoptionen"

    Private Sub Modul_BitteWechseleZuShader(shaderName As String)
        'Lädt den gewünschten Shader samt Options-Control.

        If aktuellGeladenesModul Is Nothing OrElse Not aktuellGeladenesModul.ModulNutztShader Then

            Exit Sub

        End If

        Try

            BeendeUndBereinigeDummyShader()

            aktuellGeladenerShader = ShaderByNameLoader.LadeShaderNachName(shaderName)

            If aktuellGeladenerShader Is Nothing Then

                tpShaderIstSichtbar = False

                LogHandling.LogWarn("Der Shader """ & shaderName & """ konnte für den Optionsdialog nicht " &
                                    "geladen werden.")

                StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                       tpShaderIstSichtbar)

                Exit Sub

            End If

            shaderOptionsControl = aktuellGeladenerShader.GetShaderOptionsDialog()
            tpShaderIstSichtbar = (shaderOptionsControl IsNot Nothing)

            StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                   tpShaderIstSichtbar)

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Umschalten auf Shader """ & shaderName & """: " & ex.ToString())

            BeendeUndBereinigeDummyShader()

            tpShaderIstSichtbar = False

            StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar,
                                   tpShaderIstSichtbar)

        End Try

    End Sub

#End Region

#Region "Allgemeine Optionen und Direct Commit"

    Private Sub trkDauerModulwechsel_ValueChanged(sender As Object, e As EventArgs) Handles trkDauerModulwechsel.ValueChanged

        If trkDauerModulwechsel.Value = 60 Then

            lblDauerModuswechsel.Text = "1 h"

        Else

            lblDauerModuswechsel.Text = trkDauerModulwechsel.Value.ToString() & " m"

        End If

        If wirdInitialisiert OrElse formularWurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulDauer", trkDauerModulwechsel.Value.ToString())

    End Sub

    Private Sub cmbModulwechsel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbModulwechsel.SelectedIndexChanged

        If cmbModulwechsel.SelectedIndex = 0 OrElse cmbModulwechsel.SelectedIndex = 2 Then

            lblNtrkDauerModulwechsel.Enabled = False
            lblDauerModuswechsel.Enabled = False
            trkDauerModulwechsel.Enabled = False

        Else

            lblNtrkDauerModulwechsel.Enabled = True
            lblDauerModuswechsel.Enabled = True
            trkDauerModulwechsel.Enabled = True

        End If

        If wirdInitialisiert OrElse formularWurdeBereinigt OrElse cmbModulwechsel.SelectedIndex < 0 Then

            Exit Sub

        End If

        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulReihenfolge", cmbModulwechsel.SelectedItem.ToString())

    End Sub

    Private Sub chkMultiMonitor_CheckedChanged(sender As Object, e As EventArgs) Handles chkMultiMonitor.CheckedChanged

        If wirdInitialisiert OrElse formularWurdeBereinigt Then

            Exit Sub

        End If

        'DirectCommit
        WriteToRegistry(SLIDESHOWMAIN_PATH & "MultiMonitor", chkMultiMonitor.Checked.ToString)

    End Sub

    Private Sub picHintergrundfarbe_Click(sender As Object, e As EventArgs) Handles picHintergrundfarbe.Click
        'Behandelt PictureBox Hintergrundfarbe

        Using dlg As New ColorDialog()

            dlg.Color = aktuelleSettings.Hintergrundfarbe
            dlg.AllowFullOpen = True
            dlg.AnyColor = True
            dlg.FullOpen = True

            If dlg.ShowDialog() = DialogResult.OK Then
                picHintergrundfarbe.BackColor = dlg.Color
                aktuelleSettings.Hintergrundfarbe = dlg.Color
                WriteToRegistry(SLIDESHOWMAIN_PATH & "Hintergrundfarbe", ColorToString(dlg.Color))
            End If
        End Using
    End Sub

#End Region

#Region "Bereinigung und Dispose"

    Private Sub BeendeUndBereinigeDummyModul()
        'Gibt die ausschließlich für den Optionsdialog geladene
        'Modulinstanz und ihr Options-Control frei.

        EntferneModulOptionsHandler()
        EntferneUndDisposeOptionsControl(modulOptionsControl, tpModul)

        If aktuellGeladenesModul Is Nothing Then
            Exit Sub
        End If

        Try

            aktuellGeladenesModul.Dispose()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Freigeben der Modul-Dummyinstanz: " & ex.ToString())

        Finally

            aktuellGeladenesModul = Nothing

        End Try

    End Sub

    Private Sub BeendeUndBereinigeDummyTransition()
        'Gibt die ausschließlich für den Optionsdialog geladene
        'Transitioninstanz und ihr Options-Control frei.

        EntferneUndDisposeOptionsControl(transitionOptionsControl, tpTransitions)

        If aktuellGeladeneTransition Is Nothing Then
            Exit Sub
        End If

        Try

            aktuellGeladeneTransition.Dispose()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Freigeben der Transition-Dummyinstanz: " & ex.ToString())

        Finally

            aktuellGeladeneTransition = Nothing

        End Try

    End Sub

    Private Sub BeendeUndBereinigeDummyShader()
        'Gibt die ausschließlich für den Optionsdialog geladene
        'Shaderinstanz und ihr Options-Control frei.

        EntferneUndDisposeOptionsControl(shaderOptionsControl, tpShader)

        If aktuellGeladenerShader Is Nothing Then
            Exit Sub
        End If

        Try

            aktuellGeladenerShader.Dispose()

        Catch ex As Exception

            LogHandling.LogError("Fehler beim Freigeben der Shader-Dummyinstanz: " & ex.ToString())

        Finally

            aktuellGeladenerShader = Nothing

        End Try

    End Sub

    Private Sub BereinigeOptionsdialog()
        'Gibt sämtliche vom Optionsdialog besessenen
        'Dummyinstanzen und UserControls frei.

        If formularWurdeBereinigt Then
            Exit Sub
        End If

        formularWurdeBereinigt = True

        BeendeUndBereinigeDummyShader()
        BeendeUndBereinigeDummyTransition()
        BeendeUndBereinigeDummyModul()

        EntferneUndDisposeOptionsControl(bildauswahlOptionsControl, tpBildauswahl)
        EntferneUndDisposeOptionsControl(sprachenOptionsControl, pnlLanguages)

    End Sub

    Private Sub frmOptionsMain_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert weitere verzögerte Aktionen nach der Formularfreigabe.

        formularWurdeBereinigt = True

    End Sub

#End Region

End Class