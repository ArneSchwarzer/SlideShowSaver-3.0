Imports System.Drawing

Imports SlideShowTools.ToolTipHandling
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools.RegistryHandling
Imports Modul_SlideShowSaver_3
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLoader
Imports SlideShowTools.ListHandling
Imports System.Windows.Forms
Imports SlideShowInterfaces
Imports SlideShowTools



Public Class ucOptionsModul

    'Variablendeklaration
    Private transitionInfos As List(Of SlideShowTransitionInfo)
    Private shaderInfos As List(Of SlideShowShaderInfo)
    Private markierteTransitions As List(Of String)
    Private markierteShader As List(Of String)

    Private meineInstanz As ModulMain = TryCast(ModulMain.activeModuleInstanz, ModulMain)

    Private Shared minuten As Integer
    Private Shared sekunden As Integer

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load
#Region "ucOptionsModul.Load Header"
        'Initialisieren
        transitionInfos = TransitionListLoader.LadeTransitionInfoListe()
        shaderInfos = ShaderListLoader.LadeShaderInfoListe()
        markierteTransitions = ModulMain.aktuelleSettings.Transitionseffekte
        markierteShader = ModulMain.aktuelleSettings.Shader
#End Region

#Region "cmbBildauswahl Initialisieren"
        'cmbBildauswahl
        cmbBildauswahl.SelectedItem = ModulMain.aktuelleSettings.Bildauswahl
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

        'cmbEffektauswahl
        If clbTransitions.CheckedItems.Count = 1 Then
            cmbEffektauswahl.SelectedIndex = 0 'Zufällig bei Start
            cmbEffektauswahl.Enabled = False
            lblNcmbEffektauswahl.Enabled = False
            clbTransitions.Enabled = True
        Else
            cmbEffektauswahl.SelectedItem = ModulMain.aktuelleSettings.TransitionsReihenfolge
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
            cmbShaderauswahl.SelectedItem = ModulMain.aktuelleSettings.ShaderReihenfolge
            cmbShaderauswahl.Enabled = True
            lblNcmbShaderauswahl.Enabled = True
            clbShader.Enabled = True
        End If
#End Region

#Region "trbAnzeigedauer Initialisieren"
        'trbAnzeigedauer
        trbAnzeigedauer.Value = ModulMain.aktuelleSettings.Anzeigedauer

        minuten = trbAnzeigedauer.Value \ 60
        sekunden = trbAnzeigedauer.Value Mod 60
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
        chkBildinformationen.Checked = ModulMain.aktuelleSettings.BildInfoAnzeigen
#End Region

    End Sub

    Private Sub trbAnzeigedauer_ValueChanged(sender As Object, e As EventArgs) Handles trbAnzeigedauer.ValueChanged

        'Label aktualisieren
        minuten = trbAnzeigedauer.Value \ 60
        sekunden = trbAnzeigedauer.Value Mod 60
        If minuten > 0 Then
            lblAnzeigedauer.Text = minuten.ToString & "m"
            If sekunden <> 0 Then
                lblAnzeigedauer.Text = lblAnzeigedauer.Text & " " & sekunden.ToString & "s"
            End If
        Else
            lblAnzeigedauer.Text = sekunden.ToString & "s"
        End If

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Anzeigedauer", trbAnzeigedauer.Value.ToString)

    End Sub

    Private Sub clbShader_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbShader.SelectedIndexChanged
        'Beauftragt fmrOptionsMain den ucOptionsShader zu wechseln

        If clbShader.SelectedItem IsNot Nothing Then
            meineInstanz.AttentionShaderGewechselt(clbShader.SelectedItem.ToString)
        End If

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(clbShader, ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Shader")

    End Sub

    Private Sub clbTransitions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbTransitions.SelectedIndexChanged

        If clbTransitions.SelectedItem IsNot Nothing Then
            meineInstanz.AttentionTransitionGewechselt(clbTransitions, clbTransitions.SelectedItem.ToString)
        End If

        'DirectCommit
        CheckedListBoxHandling.SaveListBoxToRegistry(clbTransitions, ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Transitionseffekte")

    End Sub

    Private Sub chkBildinformationen_CheckedChanged(sender As Object, e As EventArgs) Handles chkBildinformationen.CheckedChanged

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "BildInfoAnzeigen", chkBildinformationen.Checked.ToString)

    End Sub

    Private Sub cmbBildauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBildauswahl.SelectedIndexChanged

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Bildauswahl", cmbBildauswahl.SelectedItem.ToString)

    End Sub

    Private Sub cmbEffektauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbEffektauswahl.SelectedIndexChanged

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "TransitionsReihenfolge", cmbEffektauswahl.SelectedItem.ToString)

    End Sub

    Private Sub cmbShaderauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbShaderauswahl.SelectedIndexChanged

        'DirectCommit
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "ShaderReihenfolge", cmbShaderauswahl.SelectedItem.ToString)

    End Sub

End Class
