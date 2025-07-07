Imports SlideShowMain.SaverMain
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ToolTipHandling
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowLoader
Imports SlideShowBildauswahl
Imports SlideShowTools.CursorHandling
Imports SlideShowLogging
Imports SlideShowTools
Imports StarControlLibrary
Imports SlideShowSprachen
Imports System.TimeZoneInfo

Public Class frmOptionsMain

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

    'Shader
    Private shaderList As List(Of SlideShowShaderInfo)
    Private markierteShader As String

    'Sonstiges
    Private uc As UserControl
    Private modulSettingsZwischenspeicher As New Dictionary(Of String, Object)


    Private Sub frmOptionsMain_Load(sender As Object, e As EventArgs) Handles Me.Load
#Region "Header frmOptionMain_Load"
        Dim defaultsMain As New Dictionary(Of String, String)
        Dim anzahlMarkierteModule As Integer

        AddHandler activeModule.PleaseChangeToShader, AddressOf Modul_BitteWechseleZuShader
        AddHandler activeModule.PleaseChangeToTransition, AddressOf Modul_BitteWechseleZuTransition

        defaultsMain = GetMainDefaultSettings()

        Me.TopMost = True
        Me.BringToFront()

        isInputLocked = True
        optionsDialogIsActive = True

        ' Weil Cursor.Hide ein Stack ist...
        CursorPowerShow()


        ' Modul-Liste laden
        Dim moduleInfos = ModulListLoader.LadeModulInfoListe()
        clbModule.Items.Clear()

        ' Transition- und Shader-Liste vorbereiten
        transitionList = TransitionListLoader.LadeTransitionInfoListe()
        shaderList = ShaderListLoader.LadeShaderInfoListe()

#End Region
        ' --- Steuerelemente Initialisieren---
        ' Trackbar trkDauerModulwechsel und dazugehöriges Label lblDauerModulwechsel
#Region "Trackbar Initialisierung"
        trkDauerModulwechsel.Minimum = 1
        trkDauerModulwechsel.Maximum = 60
        trkDauerModulwechsel.Value = Integer.Parse(ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulDauer", defaultsMain))
        If trkDauerModulwechsel.Value = 60 Then
            lblDauerModuswechsel.Text = "1 h"
        Else
            lblDauerModuswechsel.Text = trkDauerModulwechsel.Value.ToString & " m"
        End If
#End Region
        ' Modulliste clbModule
#Region "clbModule Initialisierung"

        clbModule.Items.Clear()
        For Each modulInfo In moduleInfos
            clbModule.Items.Add(modulInfo)
        Next

        EnableToolTipsForCLB(clbModule)

        markierteModule = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulAktivListe", defaultsMain)
        SetCheckedItemsByName(Of SlideShowModulInfo)(
            clbModule,
            markierteModule,
            Function(m) m.ModulName
            )

        clbModule.Sorted = True
#End Region

        'Combobox cmbModulwechsel
        cmbModulwechsel.SelectedItem = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulReihenfolge", defaultsMain)
        If cmbModulwechsel.SelectedIndex = 0 Then 'Zufällig bei Start
            lblNtrkDauerModulwechsel.Enabled = False
            lblDauerModuswechsel.Enabled = False
            trkDauerModulwechsel.Enabled = False
        Else
            lblNtrkDauerModulwechsel.Enabled = True
            lblDauerModuswechsel.Enabled = True
            trkDauerModulwechsel.Enabled = True
        End If

        'Label lblKeineModule
#Region "Label 'Keine Module'"
        anzahlMarkierteModule = clbModule.CheckedItems.Count
        KeineModuleLabelLogik(anzahlMarkierteModule)
#End Region

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

            markierteTransitionen = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulTransitionListe", defaultsMain)
            SetCheckedItemsByName(Of SlideShowTransitionInfo)(
            clbTransitionsModule,
            markierteTransitionen,
            Function(m) m.TransitionName
            )

            clbTransitionsModule.Sorted = True
        End If

        'Combobox cmbTransitionsReihenfolge
        cmbTransitionsReihenfolge.SelectedItem = ReadFromRegOrDefaults(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", defaultsMain)

        'Checkbox MultiMonitor Support
        chkMultiMonitor.Visible = False 'Solange noch kein MultiMonitor Support implementiert ist
        chkMultiMonitor.Checked = False

        'Sprachauswahl-Leiste laden
        uc = New ucFlaggenstreifen()
        uc.Dock = DockStyle.Fill
        pnlLanguages.Controls.Add(uc)

        'Tabpages - Da bei Aufruf des Dialogs noch kein Modul ausgewählt ist, erst einmal alle ausblenden
        tabOptions.TabPages.Remove(tpBildauswahl)
        tabOptions.TabPages.Remove(tpModul)
        tabOptions.TabPages.Remove(tpShader)
        tabOptions.TabPages.Remove(tpTransitions)

    End Sub

    Private Sub trkDauerModulwechsel_ValueChanged(sender As Object, e As EventArgs) Handles trkDauerModulwechsel.ValueChanged

        If trkDauerModulwechsel.Value = 60 Then
            lblDauerModuswechsel.Text = "1 h"
        Else
            lblDauerModuswechsel.Text = trkDauerModulwechsel.Value.ToString & " m"
        End If

    End Sub

    Private Sub clbModule_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbModule.ItemCheck
        Try
            ' BeginInvoke wartet auf aktualisierten CheckedState – kein Korrekturterm nötig!
            BeginInvoke(Sub()
                            Dim anzahlMarkierteModule As Integer = clbModule.CheckedItems.Count
                            KeineModuleLabelLogik(anzahlMarkierteModule)
                        End Sub)
        Catch ex As Exception
            LogHandling.LogError("Fehler in clbModule_ItemCheck: " & ex.Message.ToString)
        End Try
    End Sub

    Private Sub btnAbbrechen_Click(sender As Object, e As EventArgs) Handles btnAbbrechen.Click
        Me.Close()
    End Sub

    Private Sub clbModule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbModule.SelectedIndexChanged
        'Wechselt den Inhalt der tpModul gemäß dem gerade selektieren Modul
        Dim currentSettings As New Object
        Dim modulName As String
        Dim restoreSettings As Object = Nothing

        Try
            ' Vorheriges Modul sichern
            If aktuellGeladenesModul IsNot Nothing Then
                uc = TryCast(tpModul.Controls(0), UserControl)
                If uc IsNot Nothing Then
                    currentSettings = aktuellGeladenesModul.MemorizeModulSettings(uc)
                    modulSettingsZwischenspeicher(aktuellGeladenesModul.ModulName) = currentSettings
                End If
                aktuellGeladenesModul.StopModul()
                aktuellGeladenesModul = Nothing
                tpModul.Controls.Clear()
            End If

            ' Neues Modul laden
            If clbModule.SelectedItem IsNot Nothing Then
                modulName = clbModule.SelectedItem.ToString()
                aktuellGeladenesModul = ModulByNameLoader.LadeModulNachName(modulName)

                If aktuellGeladenesModul IsNot Nothing Then
                    uc = aktuellGeladenesModul.GetModulOptionsDialog()
                    uc.Dock = DockStyle.Fill
                    tpModul.Controls.Add(uc)

                    'TabPages aktivieren
                    If Not tabOptions.TabPages.Contains(tpModul) Then tabOptions.TabPages.Add(tpModul)

                    'Einstellungen anwenden
                    If modulSettingsZwischenspeicher.ContainsKey(modulName) Then
                        restoreSettings = modulSettingsZwischenspeicher(modulName)
                        aktuellGeladenesModul.GetModulSettings(uc, restoreSettings)
                    Else
                        aktuellGeladenesModul.GetModulRegistryOrDefaultSettings(uc)
                    End If

                    'Ggf. Bildauswahl aktivieren...
                    If aktuellGeladenesModul.ModulNutztSlideShowBildauswahl AndAlso Not tabOptions.TabPages.Contains(tpBildauswahl) Then
                        uc = New ucOptionsBildauswahl()
                        uc.Dock = DockStyle.Fill
                        tpBildauswahl.Controls.Add(uc)
                        tabOptions.TabPages.Add(tpBildauswahl)
                    End If
                    '...oder deaktivieren
                    If aktuellGeladenesModul.ModulNutztSlideShowBildauswahl = False Then
                        tpBildauswahl.Controls.Clear()
                        tabOptions.TabPages.Remove(tpBildauswahl)
                    End If

                    'Ggf. Shader deaktivieren (aktivieren erfolgt über Modul_BitteWechseleZuShader()
                    If aktuellGeladenesModul.ModulNutztShader = False Then
                        tabOptions.TabPages.Remove(tpShader)
                    End If

                    'Ggf. Transitions deaktivieren (aktivieren erfolgt über Modul_BitteWechseleZUTransition()
                    If aktuellGeladenesModul.ModulNutztTransitions = False Then
                        tabOptions.TabPages.Remove(tpTransitions)
                    End If

                End If
            End If
        Catch ex As Exception
            LogHandling.LogError("Fehler in clbModule_SelectedIndexChanged: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub KeineModuleLabelLogik(anzahlMarkierteModule As Integer)
        If clbModule.Items.Count = 0 Then
            lblKeineModule.Text = "Keine Module geladen, spiele Bouncing Logo"
            lblKeineModule.Visible = True
            cmbModulwechsel.SelectedIndex = 0
            cmbModulwechsel.Enabled = False
            lblNcmbModulWechsel.Enabled = False
            trkDauerModulwechsel.Visible = False
            lblNtrkDauerModulwechsel.Visible = False
            lblDauerModuswechsel.Visible = False

        ElseIf anzahlMarkierteModule <= 1 Then
            If anzahlMarkierteModule = 0 Then
                lblKeineModule.Text = "Keine Module ausgewählt, spiele Bouncing Logo"
                lblKeineModule.Visible = True
            Else
                lblKeineModule.Visible = False
            End If

            cmbModulwechsel.SelectedIndex = 0
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
        Dim uc As UserControl
        Dim bildauswahlSettings As New Dictionary(Of String, String)
        Dim restoreSettings As Object = Nothing
        Dim modul As ISlideShowModul = Nothing

#Region "Main Settings speichern"
        ' === Main Settings ===
        markierteModule = GetCheckedItemsAsString(clbModule)
        markierteTransitionen = GetCheckedItemsAsString(clbTransitionsModule)

        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulDauer", trkDauerModulwechsel.Value.ToString)
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulReihenfolge", cmbModulwechsel.SelectedItem.ToString)
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulAktivListe", markierteModule)
        WriteToRegistry(SLIDESHOWMAIN_PATH & "MultiMonitor", chkMultiMonitor.Checked.ToString)
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionListe", markierteTransitionen)
        WriteToRegistry(SLIDESHOWMAIN_PATH & "ModulTransitionReihenfolge", cmbTransitionsReihenfolge.SelectedItem.ToString)

        LogHandling.LogDebug("Registry-Einträge für Main-Settings geschrieben.")

#End Region
        'Bildauswahl Settings auf DirectCommit umgestellt

        'Modul Settings auf DirectCommit umgestellt

        ' === Transition Settings ===

        ' === Shader Settings ===

        ' === ...und Schluss ===
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub frmOptionsMain_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        isInputLocked = False
        optionsDialogIsActive = False
        tpBildauswahl.Controls.Clear()
        Cursor.Hide()
    End Sub

    Private Sub cmbModulwechsel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbModulwechsel.SelectedIndexChanged
        If cmbModulwechsel.SelectedIndex = 0 Then 'Zufällig bei Start --> Kein Modulwechsel während der Laufzeit
            lblNtrkDauerModulwechsel.Enabled = False
            lblDauerModuswechsel.Enabled = False
            trkDauerModulwechsel.Enabled = False
        Else
            lblNtrkDauerModulwechsel.Enabled = True
            lblDauerModuswechsel.Enabled = True
            trkDauerModulwechsel.Enabled = True
        End If
    End Sub

    Private Sub Modul_BitteWechseleZuShader(shaderName As String)
        If activeModule.ModulNutztShader Then
            Try
                ' Vorhandene Controls in tpShader löschen
                If tpShader.Controls.Count > 0 Then
                    Dim altesUC As Control = tpShader.Controls(0)
                    tpShader.Controls.Remove(altesUC)
                    altesUC.Dispose()
                    altesUC = Nothing
                End If

                ' Shader laden (Dummy-Instanz)
                Dim shaderInstanz As ISlideShowShader = ShaderByNameLoader.LadeShaderNachName(shaderName)

                If shaderInstanz IsNot Nothing Then
                    Dim ucShader As UserControl = shaderInstanz.GetShaderOptionsDialog()

                    If ucShader IsNot Nothing Then
                        ucShader.Dock = DockStyle.Fill
                        tpShader.Controls.Add(ucShader)

                        ' Shader-Einstellungen aus Registry oder Default laden
                        shaderInstanz.GetShaderRegistryOrDefaultSettings(ucShader)
                    Else
                        LogHandling.LogWarn("Shader '" & shaderName & "' hat kein Options-UserControl geliefert.")
                    End If
                Else
                    LogHandling.LogWarn("Shader '" & shaderName & "' konnte nicht geladen werden.")
                End If

            Catch ex As Exception
                LogHandling.LogError("Fehler beim Umschalten auf Shader '" & shaderName & "': " & ex.Message)
            End Try

            If Not tabOptions.TabPages.Contains(tpShader) Then
                tabOptions.TabPages.Add(tpShader)
            End If

        End If

    End Sub

    Private Sub Modul_BitteWechseleZuTransition(transitionName As String)

        Try
            ' Vorhandene Controls in tpTransition löschen
            If tpTransitions.Controls.Count > 0 Then
                Dim altesUC As Control = tpTransitions.Controls(0)
                tpTransitions.Controls.Remove(altesUC)
                altesUC.Dispose()
                altesUC = Nothing
            End If

            ' Transition laden (Dummy-Instanz)
            Dim transitionInstanz As ISlideShowTransition = TransitionByNameLoader.LadeTransitionNachName(transitionName)

            If transitionInstanz IsNot Nothing Then
                Dim ucTransition As UserControl = transitionInstanz.GetTransitionOptionsDialog()

                If ucTransition IsNot Nothing Then
                    ucTransition.Dock = DockStyle.Fill
                    tpTransitions.Controls.Add(ucTransition)

                    ' Einstellungen aus Registry oder Default laden
                    transitionInstanz.GetTransitionRegistryOrDefaultSettings(ucTransition)
                Else
                    LogHandling.LogWarn("Transition '" & transitionName & "' hat kein Options-UserControl geliefert.")
                End If
            Else
                LogHandling.LogWarn("Transition '" & transitionName & "' konnte nicht geladen werden.")
            End If

        Catch ex As Exception
            LogHandling.LogError("Fehler beim Umschalten auf Transition '" & transitionName & "': " & ex.Message)
        End Try

        If Not tabOptions.TabPages.Contains(tpTransitions) Then
            tabOptions.TabPages.Add(tpTransitions)
        End If


    End Sub

    Private Sub clbTransitionsModule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbTransitionsModule.SelectedIndexChanged

        If clbTransitionsModule.SelectedItem IsNot Nothing Then
            Modul_BitteWechseleZuTransition(clbTransitionsModule.SelectedItem.ToString)
        End If

    End Sub
End Class