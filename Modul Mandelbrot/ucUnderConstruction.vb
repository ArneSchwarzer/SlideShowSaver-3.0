Imports System.Drawing

Public Class ucUnderConstruction
    Private Sub ucUnderConstruction_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Erst einmal den SplashScreen justieren & Labelfarben setzen

        Dim colorLabels As Color = Color.DarkViolet

        lblUnderConstruction.ForeColor = colorLabels
        lblHierEntsteht.ForeColor = colorLabels
        lblModulname.ForeColor = colorLabels

        panPanel.Left = (Me.ClientSize.Width - panPanel.Width) \ 2
        panPanel.Top = (Me.ClientSize.Height - panPanel.Height) \ 2

    End Sub
End Class
