Imports System.Drawing

Imports SlideShowTools.ToolTipHandling
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowLoader
Imports SlideShowTools.ListHandling


Public Class ucOptionsModul

    'Variablendeklaration

    Private Shared minuten As Integer
    Private Shared sekunden As Integer
    
    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim transitionInfos As List(Of SlideShowTransitionInfo)
        Dim shaderInfos As List(Of SlideShowShaderInfo)
        Dim markierteTransitions As List(Of String)
        Dim markierteShader As List(Of String)

        'Initialisieren
        transitionInfos = TransitionListLoader.LadeTransitionInfoListe()
        shaderInfos = ShaderListLoader.LadeShaderInfoListe
        markierteTransitions = ModulMain.aktuelleSettings.Transitionseffekte
        markierteShader = ModulMain.aktuelleSettings.Shader

        'cmbBildauswahl
        cmbBildauswahl.SelectedItem = ModulMain.aktuelleSettings.Bildauswahl

        'clbTransitions
        clbTransitions.Items.Clear()
        clbTransitions.Items.Add("Direkter Übergang (Cut)")
        For Each transitionInfo In transitionInfos
            clbTransitions.Items.Add(transitionInfo)
        Next

        EnableToolTipsForCLB(clbTransitions)

        SetCheckedItemsByName(Of SlideShowTransitionInfo)(
            clbTransitions,
            JoinSemicolonList(markierteTransitions),
            Function(m) m.TransitionName
            )

        'cmbEffektauswahl
        If clbTransitions.Items.Count = 0 Then
            cmbEffektauswahl.SelectedIndex = 0 'Zufällig bei Start
            cmbEffektauswahl.Enabled = False
            lblNcmbEffektauswahl.Enabled = False
            clbTransitions.Enabled = False
        ElseIf clbTransitions.CheckedItems.Count = 1 Then
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

        'clbShader
        clbShader.Items.Clear()
        clbShader.Items.Add("Originalbild")
        For Each shaderInfo In shaderInfos
            clbShader.Items.Add(shaderInfo)
        Next

        EnableToolTipsForCLB(clbShader)

        SetCheckedItemsByName(Of SlideShowShaderInfo)(
            clbShader,
            JoinSemicolonList(markierteShader),
            Function(m) m.ShaderName
            )

        'cmbShader
        If clbShader.Items.Count = 0 Then
            cmbShaderauswahl.SelectedIndex = 0 'Zufällig bei Start
            cmbShaderauswahl.Enabled = False
            lblNcmbShaderauswahl.Enabled = False
            clbShader.Enabled = False
        ElseIf clbShader.CheckedItems.Count = 1 Then
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

        'chkBildInfoAnzeigen
        chkBildinformationen.Checked = ModulMain.aktuelleSettings.BildInfoAnzeigen

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

    End Sub
End Class
