Imports System.Drawing

Public Class ucUnderConstruction
    Private Sub ucUnderConstruction_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Erst einmal den SplashScreen justieren & Labelfarben setzen

        Dim colorLabels As Color = Color.Lime

        lblUnderConstruction.ForeColor = colorLabels
        lblHierEntsteht.ForeColor = colorLabels
        lblModulname.ForeColor = colorLabels

        panPanel.Left = (Me.ClientSize.Width - panPanel.Width) \ 2
        panPanel.Top = (Me.ClientSize.Height - panPanel.Height) \ 2

        lblUnderConstruction.Left = (panPanel.Width - lblUnderConstruction.Width) \ 2
        lblHierEntsteht.Left = (panPanel.Width - lblHierEntsteht.Width) \ 2
        lblModulname.Left = (panPanel.Width - lblModulname.Width) \ 2

    End Sub
End Class
