Imports System.IO

Public Class ucLanguageSelector
    Inherits UserControl
    Private Sub ucLanguageSelector_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim csvPfad As String = Path.Combine(Application.StartupPath, "Languages", "translations.csv")
        cboSprache.Items.AddRange(New String() {"de", "en", "fr", "hi", "pl", "ru", "zh"})
        cboSprache.SelectedItem = LanguageManager.CurrentLanguage
        LanguageManager.LoadTranslations(csvPfad)
        SetLanguage()
    End Sub

    Private Sub cboSprache_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSprache.SelectedIndexChanged
        LanguageManager.CurrentLanguage = cboSprache.SelectedItem.ToString()
        SetLanguage()
    End Sub

    Public Sub SetLanguage()
        Dim font As Font
        Select Case LanguageManager.CurrentLanguage
            Case "zh"
                font = New Font("Microsoft YaHei", 10)
            Case "hi"
                font = New Font("Nirmala UI", 10)
            Case "ru"
                font = New Font("Segoe UI", 10)
            Case Else
                font = New Font("Segoe UI", 10)
        End Select

        For Each ctrl As Control In Me.Controls
            ctrl.Font = font
            If Not String.IsNullOrEmpty(ctrl.Tag) Then
                ctrl.Text = LanguageManager.Translate(ctrl.Name.ToString())
            End If
        Next
    End Sub
End Class
