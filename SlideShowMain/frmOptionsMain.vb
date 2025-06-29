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
        defaultsMain = GetMainDefaultSettings()

        Me.TopMost = True
        isInputLocked = True
        ' Weil Cursor.Hide ein Stack ist...
        optionsDialogIsActive = True
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
        If cmbModulwechsel.SelectedIndex = 0 Then
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
        KeineModuleLabelLogik()
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

        'Tabpages - Da bei Aufruf des Dialogs noch kein Modul ausgewählt ist, erst einmal alle ausblenden

        'Bildauswahl vorladen
        uc = New ucOptionsBildauswahl()
        uc.Dock = DockStyle.Fill
        tpBildauswahl.Controls.Add(uc)

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
        Dim anzahlMarkierteModule As Integer

        Try
            anzahlMarkierteModule = clbModule.CheckedItems.Count
            If e.NewValue = CheckState.Unchecked Then
                anzahlMarkierteModule -= 1
            ElseIf e.NewValue = CheckState.Checked Then
                anzahlMarkierteModule += 1
            End If

            'Label lblKeineModule
            KeineModuleLabelLogik()
        Catch ex As Exception
            LogHandling.LogError("Fehler in clbModule_ItemCheck: " & ex.Message.ToString)
        End Try


    End Sub

    Private Sub btnAbbrechen_Click(sender As Object, e As EventArgs) Handles btnAbbrechen.Click
        Me.Close()
    End Sub

    Private Sub clbModule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbModule.SelectedIndexChanged
        Try
            ' Vorheriges Modul sichern
            If aktuellGeladenesModul IsNot Nothing Then
                uc = TryCast(tpModul.Controls(0), UserControl)
                If uc IsNot Nothing Then
                    Dim currentSettings = aktuellGeladenesModul.MemorizeModulSettings(uc)
                    modulSettingsZwischenspeicher(aktuellGeladenesModul.ModulName) = currentSettings
                End If
                aktuellGeladenesModul.StopModul()
                aktuellGeladenesModul = Nothing
                tpModul.Controls.Clear()
            End If

            ' Neues Modul laden
            If clbModule.SelectedItem IsNot Nothing Then
                Dim modulName As String = clbModule.SelectedItem.ToString()
                aktuellGeladenesModul = ModulByNameLoader.LadeModulNachName(modulName)

                If aktuellGeladenesModul IsNot Nothing Then
                    uc = aktuellGeladenesModul.GetModulOptionsDialog()
                    uc.Dock = DockStyle.Fill
                    tpModul.Controls.Add(uc)

                    'TabPages aktivieren
                    If Not tabOptions.TabPages.Contains(tpModul) Then tabOptions.TabPages.Add(tpModul)

                    'Einstellungen anwenden
                    Dim restoreSettings As Object = Nothing
                    If modulSettingsZwischenspeicher.ContainsKey(modulName) Then
                        restoreSettings = modulSettingsZwischenspeicher(modulName)

                        aktuellGeladenesModul.GetModulSettings(uc, restoreSettings)
                    Else
                        aktuellGeladenesModul.GetModulDefaultSettings(uc)
                    End If

                    'Ggf. Bildauswahl aktivieren...
                    If aktuellGeladenesModul.ModulNutztSlideShowBildauswahl AndAlso Not tabOptions.TabPages.Contains(tpBildauswahl) Then
                        tabOptions.TabPages.Add(tpBildauswahl)
                    End If
                    '...oder deaktivieren
                    If aktuellGeladenesModul.ModulNutztSlideShowBildauswahl = False Then
                        tabOptions.TabPages.Remove(tpBildauswahl)
                    End If


                End If
            End If
        Catch ex As Exception
            LogHandling.LogError("Fehler in clbModule_SelectedIndexChanged: " & ex.Message.ToString)
        End Try

    End Sub

    Private Sub KeineModuleLabelLogik()
        Dim anzahlMarkierteModule As Integer = clbModule.CheckedItems.Count

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


        ' === Bildauswahl Settings ===
        uc = TryCast(tpBildauswahl.Controls(0), UserControl)
        If uc IsNot Nothing Then
            bildauswahlSettings = ConversionHandling.UserControlZuDictionary(uc)
        End If

        BildauswahlMain.WriteBildauswahlSettingsToRegistry(bildauswahlSettings)


        ' === Modul Setting ===

        ' Sicherstellen, dass das aktive Modul seine Settings vorher aktualisiert
        If aktuellGeladenesModul IsNot Nothing Then
            uc = TryCast(tpModul.Controls(0), UserControl)
            If uc IsNot Nothing Then
                Dim currentSettings = aktuellGeladenesModul.MemorizeModulSettings(uc)
                modulSettingsZwischenspeicher(aktuellGeladenesModul.ModulName) = currentSettings
            End If
        End If

        ' Speicher-Schleife: Alle bekannten Module anwenden lassen
        For Each modulName In modulSettingsZwischenspeicher.Keys
            Dim modul = ModulByNameLoader.LadeModulNachName(modulName)
            If modul IsNot Nothing Then
                uc = modul.GetModulOptionsDialog()
                Dim restoreSettings As Object = Nothing
                If modulSettingsZwischenspeicher.ContainsKey(modulName) Then
                    restoreSettings = modulSettingsZwischenspeicher(modulName)
                    aktuellGeladenesModul.GetModulSettings(uc, restoreSettings)
                Else
                    aktuellGeladenesModul.GetModulDefaultSettings(uc)
                End If
                If uc IsNot Nothing Then
                    modul.ApplyModulSettings(modulSettingsZwischenspeicher(modulName))
                End If
            End If
        Next

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
        If cmbModulwechsel.SelectedIndex = 0 Then
            lblNtrkDauerModulwechsel.Enabled = False
            lblDauerModuswechsel.Enabled = False
            trkDauerModulwechsel.Enabled = False
        Else
            lblNtrkDauerModulwechsel.Enabled = True
            lblDauerModuswechsel.Enabled = True
            trkDauerModulwechsel.Enabled = True
        End If
    End Sub
End Class