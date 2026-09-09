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

    Private minuten As Integer
    Private sekunden As Integer

    Private wirdInitialisiert As Boolean
    Private wurdeBereinigt As Boolean

#End Region

#Region "Events"
    'Events
    Public Event PleaseChangeToTransition(sender As Object, transitionName As String) Implements ISlideShowTransitionCommunication.PleaseChangeToTransition
    Public Event PleaseChangeToShader(shaderName As String) Implements ISlideShowShaderCommunication.PleaseChangeToShader
#End Region

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Lädt die Settings und initialisiert sämtliche Controls,
        'ohne Direct Commit oder Kommunikationsereignisse auszulösen.

        CheckYourMail()
        InitialisiereControls()

    End Sub

    Private Sub trkAnzeigedauer_ValueChanged(sender As Object, e As EventArgs) Handles trkAnzeigedauer.ValueChanged

        AktualisiereAnzeigedauerText()

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Anzeigedauer", trkAnzeigedauer.Value.ToString())

    End Sub

    Private Sub clbShader_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbShader.SelectedIndexChanged
        'Beauftragt frmOptionsMain, das Shader-Options-Control zu wechseln.

        If wirdInitialisiert OrElse wurdeBereinigt OrElse clbShader.SelectedItem Is Nothing Then

            Exit Sub

        End If

        RaiseEvent PleaseChangeToShader(clbShader.SelectedItem.ToString())

    End Sub

    Private Sub clbTransitions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles clbTransitions.SelectedIndexChanged
        'Beauftragt frmOptionsMain, das Transition-Options-Control zu wechseln.

        If wirdInitialisiert OrElse wurdeBereinigt OrElse clbTransitions.SelectedItem Is Nothing Then

            Exit Sub

        End If

        RaiseEvent PleaseChangeToTransition(clbTransitions, clbTransitions.SelectedItem.ToString())

    End Sub

    Private Sub chkBildinformationen_CheckedChanged(sender As Object, e As EventArgs) Handles chkBildinformationen.CheckedChanged

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "BildInfoAnzeigen", chkBildinformationen.Checked.ToString())

    End Sub

    Private Sub cmbBildauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBildauswahl.SelectedIndexChanged

        AktualisierePraesentationsschirmStatus()

        If wirdInitialisiert OrElse wurdeBereinigt OrElse cmbBildauswahl.SelectedItem Is Nothing Then

            Exit Sub

        End If

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Bildauswahl", cmbBildauswahl.SelectedItem.ToString())

    End Sub

    Private Sub chkPräsentationsschirm_CheckedChanged(sender As Object, e As EventArgs) Handles chkPräsentationsschirm.CheckedChanged

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Präsentationsschirm", chkPräsentationsschirm.Checked.ToString())

    End Sub

    Private Sub cmbEffektauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbEffektauswahl.SelectedIndexChanged

        If wirdInitialisiert OrElse wurdeBereinigt OrElse cmbEffektauswahl.SelectedItem Is Nothing Then

            Exit Sub

        End If

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "TransitionsReihenfolge", cmbEffektauswahl.SelectedItem.ToString())

    End Sub

    Private Sub cmbShaderauswahl_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbShaderauswahl.SelectedIndexChanged

        If wirdInitialisiert OrElse wurdeBereinigt OrElse cmbShaderauswahl.SelectedItem Is Nothing Then

            Exit Sub

        End If

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "ShaderReihenfolge", cmbShaderauswahl.SelectedItem.ToString())

    End Sub

    Private Sub clbTransitions_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbTransitions.ItemCheck

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        Try

            BeginInvoke(New Action(AddressOf SpeichereTransitionsAuswahl))

        Catch ex As InvalidOperationException

            LogHandling.LogWarn("SSS 3.0 - ucOptionsModul.clbTransitions_ItemCheck(): " &
                                "Die Transitionseinstellungen konnten nicht verarbeitet werden: " & ex.ToString())

        End Try

    End Sub

    Private Sub SpeichereTransitionsAuswahl()
        'Verarbeitet den vollständig übernommenen CheckState.

        If wirdInitialisiert OrElse wurdeBereinigt OrElse IsDisposed OrElse Disposing Then

            Exit Sub

        End If

        AktualisiereTransitionsAuswahlStatus()

        CheckedListBoxHandling.SaveListBoxToRegistry(clbTransitions, ModulMain.SLIDESHOWMODUL_SSS_FULLPATH &
                                                     "Transitionseffekte")

    End Sub

    Private Sub clbShader_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbShader.ItemCheck

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        Try

            BeginInvoke(New Action(AddressOf SpeichereShaderAuswahl))

        Catch ex As InvalidOperationException

            LogHandling.LogWarn("SSS 3.0 - ucOptionsModul.clbShader_ItemCheck(): " &
                                "Die Shadereinstellungen konnten nicht verarbeitet werden: " & ex.ToString())

        End Try

    End Sub

    Private Sub SpeichereShaderAuswahl()
        'Verarbeitet den vollständig übernommenen CheckState.

        If wirdInitialisiert OrElse wurdeBereinigt OrElse IsDisposed OrElse Disposing Then

            Exit Sub

        End If

        AktualisiereShaderAuswahlStatus()

        CheckedListBoxHandling.SaveListBoxToRegistry(clbShader, ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Shader")

    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SettingsInbox ein.

        aktuelleSettings = GetSettings(Of SettingsModul_SSS)(nameModul)

    End Sub

    Private Sub InitialisiereControls()
        'Initialisiert sämtliche Controls aus aktuelleSettings.

        wirdInitialisiert = True

        Try

            transitionInfos = TransitionListLoader.LadeTransitionInfoListe()
            shaderInfos = ShaderListLoader.LadeShaderInfoListe()

            If aktuelleSettings.Transitionseffekte IsNot Nothing Then

                markierteTransitions = New List(Of String)(aktuelleSettings.Transitionseffekte)

            Else

                markierteTransitions = New List(Of String)()

            End If

            If aktuelleSettings.Shader IsNot Nothing Then

                markierteShader = New List(Of String)(aktuelleSettings.Shader)

            Else

                markierteShader = New List(Of String)()

            End If

            'Bildauswahl
            cmbBildauswahl.SelectedItem = aktuelleSettings.Bildauswahl
            AktualisierePraesentationsschirmStatus()

            'Präsentationsschirm
            chkPräsentationsschirm.Checked = aktuelleSettings.Präsentationsschirm

            'Transitionen
            clbTransitions.Items.Clear()

            For Each transitionInfo As SlideShowTransitionInfo In transitionInfos

                clbTransitions.Items.Add(transitionInfo)

            Next

            EnableToolTipsForCLB(clbTransitions)

            SetCheckedItemsByName(Of SlideShowTransitionInfo)(clbTransitions, JoinSemicolonList(markierteTransitions),
                                                              Function(transitionInfo) transitionInfo.TransitionName)

            If clbTransitions.Items.Count = 1 Then

                clbTransitions.SetItemCheckState(0, CheckState.Checked)

            End If

            AktualisiereTransitionsAuswahlStatus()

            'Shader
            clbShader.Items.Clear()

            For Each shaderInfo As SlideShowShaderInfo In shaderInfos

                clbShader.Items.Add(shaderInfo)

            Next

            EnableToolTipsForCLB(clbShader)

            SetCheckedItemsByName(Of SlideShowShaderInfo)(clbShader, JoinSemicolonList(markierteShader),
                                                          Function(shaderInfo) shaderInfo.ShaderName)

            AktualisiereShaderAuswahlStatus()

            'Anzeigedauer
            trkAnzeigedauer.Value = aktuelleSettings.Anzeigedauer
            AktualisiereAnzeigedauerText()

            'Bildinformationen
            chkBildinformationen.Checked = aktuelleSettings.BildInfoAnzeigen

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

    Private Sub AktualisiereAnzeigedauerText()
        'Aktualisiert die formatierte Anzeige der Bilddauer.

        minuten = trkAnzeigedauer.Value \ 60
        sekunden = trkAnzeigedauer.Value Mod 60

        If minuten > 0 Then

            lblAnzeigedauer.Text = minuten.ToString() & "m"

            If sekunden <> 0 Then

                lblAnzeigedauer.Text &= " " & sekunden.ToString() & "s"

            End If

        Else

            lblAnzeigedauer.Text = sekunden.ToString() & "s"

        End If

    End Sub

    Private Sub AktualisierePraesentationsschirmStatus()
        'Aktiviert den Präsentationsschirm nur bei geeigneter Bildauswahl.

        chkPräsentationsschirm.Enabled =
                cmbBildauswahl.SelectedItem IsNot Nothing AndAlso
                cmbBildauswahl.SelectedItem.ToString() <> "Zufallsbild"

    End Sub

    Private Sub AktualisiereTransitionsAuswahlStatus()
        'Passt die Reihenfolgeauswahl an die Zahl aktiver Transitionen an.

        If clbTransitions.CheckedItems.Count <= 1 Then

            cmbEffektauswahl.SelectedIndex = 0
            cmbEffektauswahl.Enabled = False
            lblNcmbEffektauswahl.Enabled = False

        Else

            cmbEffektauswahl.SelectedItem = aktuelleSettings.TransitionsReihenfolge

            cmbEffektauswahl.Enabled = True
            lblNcmbEffektauswahl.Enabled = True

        End If

        clbTransitions.Enabled = True

    End Sub

    Private Sub AktualisiereShaderAuswahlStatus()
        'Passt die Reihenfolgeauswahl an die Zahl aktiver Shader an.

        If clbShader.CheckedItems.Count <= 1 Then

            cmbShaderauswahl.SelectedIndex = 0
            cmbShaderauswahl.Enabled = False
            lblNcmbShaderauswahl.Enabled = False

        Else

            cmbShaderauswahl.SelectedItem = aktuelleSettings.ShaderReihenfolge

            cmbShaderauswahl.Enabled = True
            lblNcmbShaderauswahl.Enabled = True

        End If

        clbShader.Enabled = True

    End Sub

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Übernimmt und speichert die Defaultwerte.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = ModulMain.GetModulDefaultSettings()

        aktuelleSettings.Bildauswahl = defaults("Bildauswahl")
        aktuelleSettings.Präsentationsschirm = CBool(defaults("Präsentationsschirm"))
        aktuelleSettings.Anzeigedauer = CInt(defaults("Anzeigedauer"))
        aktuelleSettings.Transitionseffekte = SplitSemicolonList(defaults("Transitionseffekte"))
        aktuelleSettings.TransitionsReihenfolge = defaults("TransitionsReihenfolge")
        aktuelleSettings.Shader = SplitSemicolonList(defaults("Shader"))
        aktuelleSettings.ShaderReihenfolge = defaults("ShaderReihenfolge")
        aktuelleSettings.BildInfoAnzeigen = CBool(defaults("BildInfoAnzeigen"))

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Bildauswahl", defaults("Bildauswahl"))
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Präsentationsschirm", defaults("Präsentationsschirm"))
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Anzeigedauer", defaults("Anzeigedauer"))
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Transitionseffekte", defaults("Transitionseffekte"))
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "TransitionsReihenfolge", defaults("TransitionsReihenfolge"))
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Shader", defaults("Shader"))
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "ShaderReihenfolge", defaults("ShaderReihenfolge"))
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "BildInfoAnzeigen", defaults("BildInfoAnzeigen"))

        InitialisiereControls()

    End Sub

    Private Sub ucOptionsModul_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        'Verhindert weitere verzögerte Aktionen nach der Freigabe.

        wurdeBereinigt = True

    End Sub

End Class

