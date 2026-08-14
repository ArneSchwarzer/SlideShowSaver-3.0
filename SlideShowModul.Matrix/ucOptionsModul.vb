Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.CheckedListBoxHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools
Imports System.Windows.Forms

Public Class ucOptionsModul

#Region "Variablendeklaration"
    'Variablendeklaration

    Private aktuelleSettings As ModulMain.ModulSettings_Matrix

    Private wirdInitialisiert As Boolean
    Private wurdeBereinigt As Boolean

#End Region

#Region "Initialisierung"

    Private Sub ucOptionsModul_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert das UC und seine Steuerelemente.

        CheckYourMail()
        InitialisiereControls()

    End Sub

    Private Sub InitialisiereControls()
        'Initialisiert sämtliche Controls aus aktuelleSettings,
        'ohne dabei Direct Commit auszulösen.

        wirdInitialisiert = True

        Try

            lstHiglightTexte.Items.Clear()

            If aktuelleSettings.HighlightTexte IsNot Nothing Then

                For Each item As String In aktuelleSettings.HighlightTexte

                    lstHiglightTexte.Items.Add(item)

                Next

            End If

            lstHiglightTexte.Sorted = True

            If aktuelleSettings.SzenendauerSekunden < 5 Then

                trkSzenendauer.Value = 25

            Else

                trkSzenendauer.Value = aktuelleSettings.SzenendauerSekunden

            End If

            AktualisiereSzenendauerText()
            AktualisiereHighlightButtonStatus()

        Finally

            wirdInitialisiert = False

        End Try

    End Sub

#End Region

#Region "Highlighttexte"

    Private Sub btnHighlighttextHinzufügen_Click(sender As Object, e As EventArgs) Handles btnHighlighttextHinzufügen.Click
        'Fügt einen neuen Highlighttext hinzu.

        Dim rueckgabeText As String

        If wurdeBereinigt Then
            Exit Sub
        End If

        Using myInputBox As New frmInputModul()

            If myInputBox.ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            rueckgabeText = myInputBox.rueckgabeText

        End Using

        lstHiglightTexte.Items.Add(rueckgabeText)

        AktualisiereHighlightButtonStatus()

        CheckedListBoxHandling.SaveListBoxToRegistry(lstHiglightTexte, ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH &
                                                     "Highlighttexte")

    End Sub

    Private Sub btnHighlighttextLöschen_Click(sender As Object, e As EventArgs) Handles btnHighlighttextLöschen.Click

        If wurdeBereinigt Then
            Exit Sub
        End If

        If lstHiglightTexte.SelectedIndex < 0 Then
            Exit Sub
        End If

        lstHiglightTexte.Items.Remove(lstHiglightTexte.SelectedItem)

        AktualisiereHighlightButtonStatus()

        CheckedListBoxHandling.SaveListBoxToRegistry(lstHiglightTexte, ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH &
                                                     "Highlighttexte")

    End Sub

    Private Sub btnHighlighttextListeLöschen_Click(sender As Object, e As EventArgs) Handles btnHighlighttextListeLöschen.Click

        If wurdeBereinigt Then
            Exit Sub
        End If

        lstHiglightTexte.Items.Clear()

        AktualisiereHighlightButtonStatus()

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH & "Highlighttexte", "")

    End Sub

    Private Sub lstHiglightTexte_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstHiglightTexte.SelectedIndexChanged

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        AktualisiereHighlightButtonStatus()

    End Sub

    Private Sub AktualisiereHighlightButtonStatus()

        If lstHiglightTexte.Items.Count > 0 Then

            btnHighlighttextListeLöschen.Enabled = True
            btnHighlighttextLöschen.Enabled = lstHiglightTexte.SelectedIndex >= 0

        Else

            btnHighlighttextListeLöschen.Enabled = False
            btnHighlighttextLöschen.Enabled = False

        End If

    End Sub

#End Region

#Region "Szenendauer"

    Private Sub trkSzenendauer_ValueChanged(sender As Object, e As EventArgs) Handles trkSzenendauer.ValueChanged

        AktualisiereSzenendauerText()

        If wirdInitialisiert OrElse wurdeBereinigt Then

            Exit Sub

        End If

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH & "SzenendauerSekunden", trkSzenendauer.Value.ToString())

    End Sub

    Private Sub AktualisiereSzenendauerText()

        lblSzenendauer.Text = trkSzenendauer.Value.ToString() & " s"

    End Sub

#End Region

#Region "Defaults"

    Private Sub btnDefaults_Click(sender As Object, e As EventArgs) Handles btnDefaults.Click
        'Übernimmt die Defaultwerte und speichert sie explizit.

        Dim defaults As Dictionary(Of String, String)

        If wurdeBereinigt Then
            Exit Sub
        End If

        defaults = ModulMain.GetModulDefaults()

        aktuelleSettings.HighlightTexte = SplitSemicolonList(defaults("Highlighttexte"))
        aktuelleSettings.SzenendauerSekunden = CInt(defaults("SzenendauerSekunden"))

        WriteToRegistry(ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH & "Highlighttexte", defaults("Highlighttexte"))
        WriteToRegistry(ModulMain.SLIDESHOWMODUL_MATRIX_FULLPATH & "SzenendauerSekunden",
                        defaults("SzenendauerSekunden"))

        InitialisiereControls()

    End Sub

#End Region

#Region "Settings"

    Private Sub CheckYourMail()

        aktuelleSettings = GetSettings(Of ModulMain.ModulSettings_Matrix)(ModulMain.nameModul)

    End Sub

#End Region

#Region "Bereinigung"

    Private Sub ucOptionsModul_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed

        wurdeBereinigt = True

    End Sub

#End Region

End Class