Imports SlideShowTools
Imports System.Drawing

Public Class frmModulBitteWarten
    Private Sub frmModulBitteWarten_Load(sender As Object, e As EventArgs) Handles Me.Load
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.Text = "Modul SlideShowSaver 3.0"
        Me.TopMost = False

        'Bitte warten Label vorbereiten
        lblInitializing.BringToFront()
        lblInitializing.Left = (Me.Width - lblInitializing.Width) \ 2
        lblInitializing.Top = (Me.Height - lblInitializing.Height) \ 2

        'Bitte warten Label anzeigen
        lblInitializing.Visible = True
    End Sub
End Class