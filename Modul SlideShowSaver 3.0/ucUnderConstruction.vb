Imports System.Drawing

Public Class ucUnderConstruction
    Private Sub ucUnderConstruction_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Modul-Name und Labelfarben setzen 
        Dim colorLabels As Color = Color.Red
        Label1.ForeColor = colorLabels
        Label2.ForeColor = colorLabels
        Label3.ForeColor = colorLabels
        Label3.Text = "SlideShowSaver 3.0"
    End Sub
End Class
