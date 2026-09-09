Imports System.Windows.Forms

Public Class frmInputModul

#Region "Eigenschaften"

    Public Property rueckgabeText As String

#End Region

#Region "Initialisierung"

    Private Sub frmInputModul_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.TopMost = True

        Me.AcceptButton = btnOK
        Me.CancelButton = btnCancel

    End Sub

#End Region

#Region "Schaltflächen"

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click

        rueckgabeText = txtTag.Text

        Me.DialogResult = DialogResult.OK

        Me.Close()

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        Me.DialogResult = DialogResult.Cancel

        Me.Close()

    End Sub

#End Region

End Class