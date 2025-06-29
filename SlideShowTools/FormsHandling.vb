Imports System.Windows.Forms
Imports System.Drawing

Public Class FormsHandling
    Public Shared Sub InitialFormPreparation(ByVal theForm As Form, bgColor As Color)
        theForm.BackColor = bgColor
        theForm.FormBorderStyle = FormBorderStyle.None
        theForm.KeyPreview = True
        theForm.TopMost = False

        'temporär für Testzwecke
        theForm.WindowState = FormWindowState.Maximized

    End Sub
End Class
