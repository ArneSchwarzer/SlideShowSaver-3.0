Imports System.Drawing

Public Class ucUnderConstruction
    Private Sub ucUnderConstruction_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Modul-Name und Labelfarben setzen 
        Dim colorLabels As Color = Color.Red
        lblUnderConstruction.ForeColor = colorLabels
        lblHierEntsteht.ForeColor = colorLabels
        lblModulname.ForeColor = colorLabels
        lblModulname.Text = "SlideShowSaver 3.0"
    End Sub
End Class
