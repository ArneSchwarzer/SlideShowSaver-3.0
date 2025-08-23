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

    'Loader-Logik
    Private modulListLoader As New ModulListLoader()
    Private modulByNameLoader As New ModulByNameLoader()
    Private aktuellGeladenesModul As ISlideShowModul = Nothing

    'Module
    Private moduleList As List(Of SlideShowModulInfo)
    Private markierteModule As String

    'Transitionen
    Private transitionList As List(Of SlideShowTransitionInfo)
    Private markierteTransitionen As String
    Private aktuellGeladeneTransition As ISlideShowTransition = Nothing

    'Shader
    Private shaderList As List(Of SlideShowShaderInfo)
    Private markierteShader As String
    Private aktuellGedandenerShader As ISlideShowShader = Nothing

    'TabOptions-Logik
    Private tpModulIstSichtbar As Boolean = False
    Private tpBildauswahlIstSichtbar As Boolean = False
    Private tpTransitionsIstSichtbar As Boolean = False
    Private tpShaderIstSichtbar As Boolean = False

    'Sonstiges
    Private uc As UserControl
    Private aktuelleSettings As SettingsMain
#End Region

    Private Sub frmOptionsMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.TopMost = True
        Me.BringToFront()

        isInputLocked = True
        optionsDialogIsActive = True

        ' Weil Cursor.Hide ein Stack ist...
        CursorPowerShow()

        'Aktuelle Settings einlesen
        CheckYourMail()

        'Steuerelemente initialisieren
        IniOrReinitialise()

#Region "Sprachauswahl-Leiste laden"
        'Sprachauswahl-Leiste laden
        uc = New ucFlaggenstreifen()
        uc.Dock = DockStyle.Fill
        pnlLanguages.Controls.Add(uc)
#End Region

#Region "TabPages Initialisieren"
        'Tabpages - Da bei Aufruf des Dialogs noch kein Modul ausgewählt ist, erst einmal alle ausblenden

        StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar, tpShaderIstSichtbar)
#End Region

    End Sub

    Private Sub trkDauerModulwechsel_ValueChanged(sender As Object, e As EventArgs) Handles trkDauerModulwechsel.ValueChanged

        If trkDauerModulwechsel.Value = 60 Then
            lblDauerModuswechsel.Text = "1 h"
        Else
            lblDauerModuswechsel.Text = trkDauerModulwechsel.Value.ToString & " m"
        End If

        'DirectCommit
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulDauer", trkDauerModulwechsel.Value.ToString)

    End Sub

    Private Sub clbModule_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbModule.ItemCheck
        Try
            ' BeginInvoke sorgt dafür, dass der Code erst ausgeführt wird,
            ' nachdem der Checked-Zustand aktualisiert wurde
            BeginInvoke(Sub()
                            Dim anzahlMarkierteModule As Integer = clbModule.CheckedItems.Count
                            Dim clb As CheckedListBox = DirectCast(sender, CheckedListBox)

                            KeineModuleLabelLogik(anzahlMarkierteModule)

                            'DirectCommit
                            CheckedListBoxHandling.SaveListBoxToRegistry(clb, SLIDESHOWMAIN_PATH & "ModulAktivListe")

                        End Sub)
        Catch ex As Exception
            LogHandling.LogError("Fehler in clbModule_ItemCheck: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub clbModule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbModule.SelectedIndexChanged
        'Wechselt den Inhalt der tpModul gemäß dem gerade selektieren Modul
        Dim modulName As String

        'Handler vom vorherigen Modul entfernen
        Try
            Dim ucTransition = TryCast(tpModul.Controls(0), ISlideShowTransitionCommunication)
            If ucTransition IsNot Nothing Then
                RemoveHandler ucTransition.PleaseChangeToTransition, AddressOf Modul_BitteWechseleZuTransition
            End If

            Dim ucShader = TryCast(tpModul.Controls(0), ISlideShowShaderCommunication)
            If ucShader IsNot Nothing Then
                RemoveHandler ucShader.PleaseChangeToShader, AddressOf Modul_BitteWechseleZuShader
            End If
        Catch ex As Exception
            LogHandling.LogError("SaverMain - frmOptionsMain.clbModule.SelectedIndexChanged: Fehler beim Entfernen alter Handler: " & ex.ToString)
        End Try

        Try

            'Vorheriges Modul entladen
            If aktuellGeladenesModul IsNot Nothing Then
                aktuellGeladenesModul = Nothing
                tpModul.Controls.Clear()
            End If

            ' Neues Modul laden
            If clbModule.SelectedItem IsNot Nothing Then

                'Modul als Dummy-Instanz laden
                modulName = clbModule.SelectedItem.ToString()
                aktuellGeladenesModul = ModulByNameLoader.LadeModulNachName(modulName)

                'TabPages aktivieren
                tpModulIstSichtbar = True

                'Ggf. Bildauswahl aktivieren oder deaktivieren
                If aktuellGeladenesModul.ModulNutztSlideShowBildauswahl Then
                    tpBildauswahlIstSichtbar = True
                Else
                    tpBildauswahlIstSichtbar = False
                End If

                'Ggf. Shader deaktivieren (aktivieren erfolgt über Modul_BitteWechseleZuShader()
                If aktuellGeladenesModul.ModulNutztShader = False Then
                    tpShaderIstSichtbar = False
                End If

                'Ggf. Transitions deaktivieren (aktivieren erfolgt über Modul_BitteWechseleZUTransition()
                If aktuellGeladenesModul.ModulNutztTransitions = False And clbTransitionsModule.SelectedIndex < 0 Then
                    tpTransitionsIstSichtbar = False
                End If

            End If

            StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar, tpShaderIstSichtbar)

            'Dummny Instanz wird beim Schließen der Form gelöscht!

        Catch ex As Exception
            LogHandling.LogError("Fehler in clbModule_SelectedIndexChanged: " & ex.Message.ToString)
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

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        'Da eine DirectCommit-Architektur vorliegt, ist keine 'Sonderbehandlung' des OK-Buttons notwendig
        Close()
    End Sub

    Private Sub frmOptionsMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        isInputLocked = False
        optionsDialogIsActive = False
        tpBildauswahl.Controls.Clear()
        Cursor.Hide()
    End Sub

    Private Sub cmbModulwechsel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbModulwechsel.SelectedIndexChanged
        If cmbModulwechsel.SelectedIndex = 0 Or cmbModulwechsel.SelectedIndex = 2 Then '"Zufällig bei Start" oder "In Reihenfolge bei Start" --> Kein Modulwechsel während der Laufzeit
            lblNtrkDauerModulwechsel.Enabled = False
            lblDauerModuswechsel.Enabled = False
            trkDauerModulwechsel.Enabled = False
        Else
            lblNtrkDauerModulwechsel.Enabled = True
            lblDauerModuswechsel.Enabled = True
            trkDauerModulwechsel.Enabled = True
        End If

        'DirectCommit
        If cmbModulwechsel.SelectedIndex >= 0 Then
            WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulReihenfolge", cmbModulwechsel.SelectedItem.ToString)
        End If

    End Sub

    Private Sub Modul_BitteWechseleZuShader(shaderName As String)

        If activeModule.ModulNutztShader Then
            Try
                'Vorhandene Controls in tpShader löschen
                If tpShader.Controls.Count > 0 Then
                    uc = tpShader.Controls(0)
                    tpShader.Controls.Remove(uc)
                    uc.Dispose()
                    uc = Nothing
                End If

                'Shader laden (Dummy-Instanz)
                aktuellGedandenerShader = ShaderByNameLoader.LadeShaderNachName(shaderName)

                'Shader TabPage anzeigen
                tpShaderIstSichtbar = True
                StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar, tpShaderIstSichtbar)

                'Dummy-Instanz wird erst bei .Closed() freigegeben

            Catch ex As Exception
                LogHandling.LogError("Fehler beim Umschalten auf Shader '" & shaderName & "': " & ex.Message)
            End Try

        End If

    End Sub

    Private Sub Modul_BitteWechseleZuTransition(sender As Object, transitionName As String)

        Try
            ' Vorhandene Controls in tpTransition löschen
            If tpTransitions.Controls.Count > 0 Then
                uc = tpTransitions.Controls(0)
                tpTransitions.Controls.Remove(uc)
                uc.Dispose()
                uc = Nothing
            End If

            ' Transition laden (Dummy-Instanz)
            aktuellGeladeneTransition = TransitionByNameLoader.LadeTransitionNachName(transitionName)

            If aktuellGeladeneTransition IsNot Nothing Then
                'TabPage tpTransitions einschalten
                tpTransitionsIstSichtbar = True
                StelleTabPagesZusammen(tpModulIstSichtbar, tpBildauswahlIstSichtbar, tpTransitionsIstSichtbar, tpShaderIstSichtbar)

                'Falls von Modul aufgerufen, SelectedIndex in cblTransitionsModule löschen
                If sender IsNot clbTransitionsModule AndAlso clbTransitionsModule.SelectedItem?.ToString() <> transitionName Then
                    clbTransitionsModule.SelectedIndex = -1
                End If


            Else
                LogHandling.LogWarn("Transition '" & transitionName & "' hat kein Options-UserControl geliefert.")
            End If

            'Dummy Instanz wird erst bei .Close() wieder freigegeben

        Catch ex As Exception
            LogHandling.LogError("Fehler beim Umschalten auf Transition '" & transitionName & "': " & ex.Message)
        End Try

    End Sub

    Private Sub clbTransitionsModule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbTransitionsModule.SelectedIndexChanged

        If clbTransitionsModule.SelectedItem IsNot Nothing Then
            Modul_BitteWechseleZuTransition(clbTransitionsModule, clbTransitionsModule.SelectedItem.ToString)
        End If

    End Sub

    Private Sub chkMultiMonitor_CheckedChanged(sender As Object, e As EventArgs) Handles chkMultiMonitor.CheckedChanged

        'DirectCommit
        WriteToRegistry(SLIDESHOWMAIN_PATH & "MultiMonitor", chkMultiMonitor.Checked.ToString)

    End Sub

    Private Sub clbTransitionsModule_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbTransitionsModule.ItemCheck
        'DirectCommit für chlbModule sobald ein Eintrag gechecked/ungeschecked wird.

        ' BeginInvoke sorgt dafür, dass der Code erst ausgeführt wird,
        ' nachdem der Checked-Zustand aktualisiert wurde
        Dim clb As CheckedListBox = DirectCast(sender, CheckedListBox)

        BeginInvoke(New MethodInvoker(Sub()
                                          CheckedListBoxHandling.SaveListBoxToRegistry(clb, SLIDESHOWMAIN_PATH & "ModulTransitionListe")

                                          If clbTransitionsModule.CheckedItems.Count <= 1 Then
                                              cmbTransitionsReihenfolge.Enabled = False
                                              lblNcmbAbspielmodusTransitionsModule.Enabled = False
                                          Else
                                              cmbTransitionsReihenfolge.Enabled = True
                                              lblNcmbAbspielmodusTransitionsModule.Enabled = True
                                          End If

                                      End Sub))

    End Sub

    Private Sub StelleTabPagesZusammen(modulSichtbar As Boolean, bildlauswahlSichtbar As Boolean, transitionSichtbar As Boolean, shaderSichtbar As Boolean)
        'Zeigt die TabPages in der korrekten Reihenfolge an, weil das "%)=§$-TabOptions Steuerelement dafür ja leider
        'zu blöde ist.
        Dim aufrufendeTabPage As TabPage

        aufrufendeTabPage = tabOptions.SelectedTab

        'TabPages außer tpAllgemein ausschalten
        If aufrufendeTabPage IsNot tpModul Then
            tabOptions.TabPages.Remove(tpModul)
        End If
        tabOptions.TabPages.Remove(tpBildauswahl)
        tabOptions.TabPages.Remove(tpTransitions)
        tabOptions.TabPages.Remove(tpShader)

        'Und nun in der korrekten Reihenfolge wieder einschalten
        If aufrufendeTabPage IsNot tpModul Then
            If modulSichtbar Then

                ' Neues Modul UC laden
                If aktuellGeladenesModul IsNot Nothing Then
                    uc = aktuellGeladenesModul.GetModulOptionsDialog()
                    uc.Dock = DockStyle.Fill
                    tpModul.Controls.Add(uc)

                    tabOptions.TabPages.Add(tpModul)
                End If

                'Handler für Transitionen und Shader hinzufügen (RemoveHandler des Vorgänger-Moduls hat bereits
                'in clbModule.SelectedIndexChanged() stattgefunden.
                Dim ucTransition = TryCast(tpModul.Controls(0), ISlideShowTransitionCommunication)
                If ucTransition IsNot Nothing Then
                    AddHandler ucTransition.PleaseChangeToTransition, AddressOf Modul_BitteWechseleZuTransition
                End If

                Dim ucShader = TryCast(tpModul.Controls(0), ISlideShowShaderCommunication)
                If ucShader IsNot Nothing Then
                    AddHandler ucShader.PleaseChangeToShader, AddressOf Modul_BitteWechseleZuShader
                End If

            Else
                tabOptions.TabPages.Remove(tpModul)
            End If
        End If


        If bildlauswahlSichtbar Then
            uc = New ucOptionsBildauswahl()
            uc.Dock = DockStyle.Fill
            tpBildauswahl.Controls.Add(uc)

            tabOptions.TabPages.Add(tpBildauswahl)
        Else
            tabOptions.TabPages.Remove(tpBildauswahl)
        End If

        If transitionSichtbar Then

            If aktuellGeladeneTransition IsNot Nothing Then
                uc = aktuellGeladeneTransition.GetTransitionOptionsDialog()

                If uc IsNot Nothing Then
                    uc.Dock = DockStyle.Fill
                    tpTransitions.Controls.Add(uc)

                    'TabPage tpTransitions einschalten
                    tabOptions.TabPages.Add(tpTransitions)
                Else
                    LogHandling.LogWarn("Transition '" & aktuellGeladeneTransition.TransitionName & "' hat kein Options-UserControl geliefert.")
                End If
            End If

        Else
            tabOptions.TabPages.Remove(tpTransitions)
        End If

        If shaderSichtbar Then

            'Neues Shader UC laden
            If aktuellGedandenerShader IsNot Nothing Then
                uc = aktuellGedandenerShader.GetShaderOptionsDialog()

                If uc IsNot Nothing Then
                    uc.Dock = DockStyle.Fill
                    tpShader.Controls.Add(uc)

                    'TabPage tpShader einschalten
                    tabOptions.TabPages.Add(tpShader)
                Else
                    LogHandling.LogWarn("Shader '" & aktuellGedandenerShader.ShaderName & "' hat kein Options-UserControl geliefert.")
                End If
            End If
        Else
            tabOptions.TabPages.Remove(tpShader)
        End If

        tabOptions.SelectedTab = aufrufendeTabPage

    End Sub

    Private Sub cmbTransitionsReihenfolge_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbTransitionsReihenfolge.SelectedIndexChanged

        'DirectCommit
        If cmbModulwechsel.SelectedIndex >= 0 Then
            WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", cmbModulwechsel.SelectedItem.ToString)
        End If

    End Sub

    Private Sub frmOptionsMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        'Bei DirectCommit entspricht jedes Closed auch einem Klick auf den klassischen "OK"-Button

        'Dummy-Instanzen wieder löschen
        aktuellGeladenesModul = Nothing
        aktuellGedandenerShader = Nothing
        aktuellGeladeneTransition = Nothing

        '...und Schluss
        Me.DialogResult = DialogResult.OK

    End Sub

    Private Sub CheckYourMail()
        'aktuelleSettings aus der SettingsInbox abholen

        aktuelleSettings = GetSettings(Of SettingsMain)("Main")

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

    Private Sub IniOrReinitialise()
        ' Modul-Liste laden
        Dim anzahlMarkierteModule As Integer
        Dim moduleInfos = ModulListLoader.LadeModulInfoListe()

        clbModule.Items.Clear()

        ' ModulTransition Liste vorbereiten
        transitionList = TransitionListLoader.LadeTransitionInfoListe()

#Region "Trackbar Initialisierung"
        ' Trackbar trkDauerModulwechsel und dazugehöriges Label lblDauerModulwechsel
        trkDauerModulwechsel.Minimum = 1
        trkDauerModulwechsel.Maximum = 60
        trkDauerModulwechsel.Value = aktuelleSettings.ModulDauer
        If trkDauerModulwechsel.Value = 60 Then
            lblDauerModuswechsel.Text = "1 h"
        Else
            lblDauerModuswechsel.Text = trkDauerModulwechsel.Value.ToString & " m"
        End If
#End Region

#Region "clbModule Initialisierung"
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
#End Region

#Region "cmbModulWechsel Initialisierung"
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
#End Region

#Region "Label 'Keine Module'"
        'Label lblKeineModule
        anzahlMarkierteModule = clbModule.CheckedItems.Count
        KeineModuleLabelLogik(anzahlMarkierteModule)
#End Region

#Region "clbTransitionsModule Initialisieren"
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

        'Solange die Funktion noch nicht implementiert ist
        lblNclbTransitionsModule.Enabled = False
        clbTransitionsModule.Enabled = False

#End Region

#Region "cmbTransitionsReihenfolge für Module Initialisieren"
        'Combobox cmbTransitionsReihenfolge
        cmbTransitionsReihenfolge.SelectedItem = aktuelleSettings.ModulTransitionReihenfolge

        If clbTransitionsModule.CheckedItems.Count <= 1 Then
            cmbTransitionsReihenfolge.Enabled = False
            lblNcmbAbspielmodusTransitionsModule.Enabled = False
        Else
            cmbTransitionsReihenfolge.Enabled = True
            lblNcmbAbspielmodusTransitionsModule.Enabled = True
        End If

        'Solange die Funktion noch nicht implementiert ist
        cmbTransitionsReihenfolge.Enabled = False
        lblNcmbAbspielmodusTransitionsModule.Enabled = False

#End Region

#Region "chkMultiMonitor Initialisieren"
        'Checkbox MultiMonitor Support

        'Solange noch kein MultiMonitor Support implementiert ist
        '
        'chkMultiMonitor.Checked = aktuelleSettings.MultiMonitor

        chkMultiMonitor.Enabled = False
        chkMultiMonitor.Checked = False
#End Region

#Region "picHintergrundfarbe Initialisieren"
        'Hintergrundfarbe für den Schoner

        picHintergrundfarbe.BackColor = aktuelleSettings.Hintergrundfarbe

#End Region

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Default-Werte einlesen und Steuerelemente setzen

        Dim defaults As Dictionary(Of String, String)

        'Defaults einlesen
        defaults = GetMainDefaultSettings()

        'AktuelleSettings aktualisieren
        aktuelleSettings.ModulDauer = CInt(defaults("ModulDauer"))
        aktuelleSettings.ModulReihenfolge = defaults("ModulReihenfolge")
        aktuelleSettings.ModulAktivListe = SplitSemicolonList(defaults("ModulAktivListe"))
        aktuelleSettings.MultiMonitor = CBool(defaults("MultiMonitor"))
        aktuelleSettings.ModulTransitionListe = SplitSemicolonList(defaults("ModulTransitionListe"))
        aktuelleSettings.ModulTransitionReihenfolge = defaults("ModulTransitionReihenfolge")
        aktuelleSettings.Hintergrundfarbe = StringToColor(defaults("Hintergrundfarbe"))

        'Steuerelemente setzen
        IniOrReinitialise()

    End Sub
End Class