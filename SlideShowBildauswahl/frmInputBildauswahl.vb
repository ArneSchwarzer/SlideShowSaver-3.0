Imports System.Windows.Forms

Public Class frmInputBildauswahl

    Public Property rueckgabeTag As String

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click

        rueckgabeTag = txtTag.Text
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub frmInputBildauswahl_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.TopMost = True
        Me.AcceptButton = btnOK
        Me.CancelButton = btnCancel

        txtTag.Select()
        txtTag.SelectionStart = txtTag.TextLength

    End Sub

End Class