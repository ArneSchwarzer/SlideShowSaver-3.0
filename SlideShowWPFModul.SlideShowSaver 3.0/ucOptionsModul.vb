Imports System.Windows.Forms
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLoader
Imports SlideShowLogging
Imports SlideShowTools
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.ToolTipHandling
Imports SlideShowWPFModul.SlideShowSaver_3._0.ModulMain

Public Class ucOptionsModul
    Implements ISlideShowTransitionCommunication
    Implements ISlideShowShaderCommunication

#Region "Variablendeklaration"
    'Variablendeklaration
    Private aktuelleSettings As SettingsModul_SSS
    Private transitionInfos As List(Of SlideShowTransitionInfo)
    Private shaderInfos As List(Of SlideShowShaderInfo)
    Private markierteTransitions As List(Of String)
    Private markierteShader As List(Of String)

    Private Shared minuten As Integer
    Private Shared sekunden As Integer
#End Region

#Region "Events"
    'Events
    Public Event PleaseChangeToTransition(sender As Object, transitionName As String) Implements ISlideShowTransitionCommunication.PleaseChangeToTransition
    Public Event PleaseChangeToShader(shaderName As String) Implements ISlideShowShaderCommunication.PleaseChangeToShader
#End Region

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Settings aus dem Zwischenspeicher holen
        CheckYourMail()

        'Steuerelemente Initialisieren
        IniAndReinitialise()

    End Sub

    Private Sub trkAnzeigedauer_ValueChanged(sender As Object, e As EventArgs) Handles trkAnzeigedauer.ValueChanged

        'Label aktualisieren
        minuten = trkAnzeigedauer.Value \ 60
        sekunden = trkAnzeigedauer.Value Mod 60
        If minuten > 0 Then
            lblAnzeigedauer.Text = minuten.ToString & "m"
            If sekunden <> 0 Then
                lblAnzeigedauer.Text = lblAnzeigedauer.Text & " " & sekunden.ToString & "s"
            End If
        Else
            lblAnzeigedauer.Text = sekunden.ToString & "s"
        End If

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Anzeigedauer", trkAnzeigedauer.Value.ToString)

    End Sub

    Private Sub clbShader_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbShader.SelectedIndexChanged
        'Beauftragt fmrOptionsMain den ucOptionsShader zu wechseln

        If clbShader.SelectedItem IsNot Nothing Then
            RaiseEvent PleaseChangeToShader(clbShader.SelectedItem.ToString)
        End If

    End Sub

    Private Sub clbTransitions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbTransitions.SelectedIndexChanged

        If clbTransitions.SelectedItem IsNot Nothing Then
            RaiseEvent PleaseChangeToTransition(clbTransitions, clbTransitions.SelectedItem.ToString)
        End If

    End Sub

    Private Sub chkBildinformationen_CheckedChanged(sender As Object, e As EventArgs) Handles chkBildinformationen.CheckedChanged

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "BildInfoAnzeigen", chkBildinformationen.Checked.ToString)

    End Sub

    Private Sub cmbBildauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBildauswahl.SelectedIndexChanged

        If cmbBildauswahl.SelectedItem = "Zufallsbild" Then
            chkPräsentationsschirm.Enabled = False
        Else
            chkPräsentationsschirm.Enabled = True
        End If
        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Bildauswahl", cmbBildauswahl.SelectedItem.ToString)

    End Sub

    Private Sub chkPräsentationsschirm_CheckedChanged(sender As Object, e As EventArgs) Handles chkPräsentationsschirm.CheckedChanged

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Präsentationsschirm", chkPräsentationsschirm.Checked.ToString)

    End Sub

    Private Sub cmbEffektauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbEffektauswahl.SelectedIndexChanged

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "TransitionsReihenfolge", cmbEffektauswahl.SelectedItem.ToString)

    End Sub

    Private Sub cmbShaderauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbShaderauswahl.SelectedIndexChanged

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "ShaderReihenfolge", cmbShaderauswahl.SelectedItem.ToString)

    End Sub

    Private Sub clbTransitions_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbTransitions.ItemCheck
        Try
            BeginInvoke(Sub()
                            If clbTransitions.CheckedItems.Count = 1 Then
                                cmbEffektauswahl.SelectedIndex = 0 'Zufällig bei Start
                                cmbEffektauswahl.Enabled = False
                                lblNcmbEffektauswahl.Enabled = False
                                clbTransitions.Enabled = True
                            Else
                                cmbEffektauswahl.Enabled = True
                                lblNcmbEffektauswahl.Enabled = True
                                clbTransitions.Enabled = True
                            End If

                            'DirectCommit
                            CheckedListBoxHandling.SaveListBoxToRegistry(clbTransitions, ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Transitionseffekte")

                        End Sub)
        Catch ex As Exception
            LogHandling.LogWarn("SSS 3.0: ucOptionsModul.clbTransitions_ItemCheck() - Problem: " & ex.ToString)
        End Try
    End Sub

    Private Sub clbShader_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbShader.ItemCheck
        Try
            BeginInvoke(Sub()
                            If clbShader.CheckedItems.Count = 1 Then
                                cmbShaderauswahl.SelectedIndex = 0 'Zufällig bei Start
                                cmbShaderauswahl.Enabled = False
                                lblNcmbShaderauswahl.Enabled = False
                                clbShader.Enabled = True
                            Else
                                cmbShaderauswahl.Enabled = True
                                lblNcmbShaderauswahl.Enabled = True
                                clbShader.Enabled = True
                            End If

                            'DirectCommit
                            CheckedListBoxHandling.SaveListBoxToRegistry(clbShader, ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Shader")

                        End Sub)
        Catch ex As Exception
            LogHandling.LogWarn("SSS 3.0: ucOptionsModul.clbShader_ItemCheck() - Problem: " & ex.ToString)
        End Try
    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SettingsInbox ein.

        aktuelleSettings = GetSettings(Of SettingsModul_SSS)(nameModul)

    End Sub

    Private Sub IniAndReinitialise()

        'Initialisieren
        transitionInfos = TransitionListLoader.LadeTransitionInfoListe()
        shaderInfos = ShaderListLoader.LadeShaderInfoListe()
        markierteTransitions = aktuelleSettings.Transitionseffekte
        markierteShader = aktuelleSettings.Shader

#Region "cmbBildauswahl Initialisieren"
        'cmbBildauswahl
        cmbBildauswahl.SelectedItem = aktuelleSettings.Bildauswahl
        If aktuelleSettings.Bildauswahl = "Zufallsbild" Then
            chkPräsentationsschirm.Enabled = False
        Else
            chkPräsentationsschirm.Enabled = True
        End If
#End Region

#Region "chkPräsentationsschirm Initialisieren"
        'chkPräsentationsschirm
        chkPräsentationsschirm.Checked = aktuelleSettings.Präsentationsschirm
#End Region

#Region "clbTransitions Initialisierung"
        'clbTransitions
        clbTransitions.Items.Clear()
        For Each transitionInfo In transitionInfos
            clbTransitions.Items.Add(transitionInfo)
        Next

        EnableToolTipsForCLB(clbTransitions)

        SetCheckedItemsByName(Of SlideShowTransitionInfo)(
            clbTransitions,
            JoinSemicolonList(markierteTransitions),
            Function(m) m.TransitionName
            )

        If clbTransitions.Items.Count = 1 Then
            clbTransitions.SetItemCheckState(0, CheckState.Checked)
        End If
#End Region

#Region "cmbEffektauswahl Initialisieren"
        'cmbEffektauswahl
        If clbTransitions.CheckedItems.Count = 1 Then
            cmbEffektauswahl.SelectedIndex = 0 'Zufällig bei Start
            cmbEffektauswahl.Enabled = False
            lblNcmbEffektauswahl.Enabled = False
            clbTransitions.Enabled = True
        Else
            cmbEffektauswahl.SelectedItem = aktuelleSettings.TransitionsReihenfolge
            cmbEffektauswahl.Enabled = True
            lblNcmbEffektauswahl.Enabled = True
            clbTransitions.Enabled = True
        End If
#End Region

#Region "clbShader Initialisieren"
        'clbShader
        clbShader.Items.Clear()
        For Each shaderInfo In shaderInfos
            clbShader.Items.Add(shaderInfo)
        Next

        EnableToolTipsForCLB(clbShader)

        SetCheckedItemsByName(Of SlideShowShaderInfo)(
            clbShader,
            JoinSemicolonList(markierteShader),
            Function(m) m.ShaderName
            )
#End Region

#Region "cmbShader Initialisieren"
        'cmbShader
        If clbShader.CheckedItems.Count = 1 Then
            cmbShaderauswahl.SelectedIndex = 0 'Zufällig bei Start
            cmbShaderauswahl.Enabled = False
            lblNcmbShaderauswahl.Enabled = False
            clbShader.Enabled = True
        Else
            cmbShaderauswahl.SelectedItem = aktuelleSettings.ShaderReihenfolge
            cmbShaderauswahl.Enabled = True
            lblNcmbShaderauswahl.Enabled = True
            clbShader.Enabled = True
        End If
#End Region

#Region "trbAnzeigedauer Initialisieren"
        'trbAnzeigedauer
        trkAnzeigedauer.Value = aktuelleSettings.Anzeigedauer

        minuten = trkAnzeigedauer.Value \ 60
        sekunden = trkAnzeigedauer.Value Mod 60
        If minuten > 0 Then
            lblAnzeigedauer.Text = minuten.ToString & "m"
            If sekunden <> 0 Then
                lblAnzeigedauer.Text = lblAnzeigedauer.Text & " " & sekunden.ToString & "s"
            End If
        Else
            lblAnzeigedauer.Text = sekunden.ToString & "s"
        End If
#End Region

#Region "chkBildInfoAnzeigen"
        'chkBildInfoAnzeigen
        chkBildinformationen.Checked = aktuelleSettings.BildInfoAnzeigen
#End Region
    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Liest die Defaultwerte ein und setzt die Steuerelemente entsprechend

        Dim defaults As Dictionary(Of String, String)

        'Defaults einlesen
        defaults = GetModulDefaultSettings()

        'AktuelleSettingsAktualisieren
        aktuelleSettings.Bildauswahl = defaults("Bildauswahl")
        aktuelleSettings.Präsentationsschirm = CBool(defaults("Präsentationsschirm"))
        aktuelleSettings.Anzeigedauer = CInt(defaults("Anzeigedauer"))
        aktuelleSettings.Transitionseffekte = SplitSemicolonList(defaults("Transitionseffekte"))
        aktuelleSettings.TransitionsReihenfolge = defaults("TransitionsReihenfolge")
        aktuelleSettings.Shader = SplitSemicolonList(defaults("Shader"))
        aktuelleSettings.ShaderReihenfolge = defaults("ShaderReihenfolge")
        aktuelleSettings.BildInfoAnzeigen = CBool(defaults("BildInfoAnzeigen"))

        'Steuerelemente Setzen
        IniAndReinitialise()

    End Sub
End Class

