Imports System.Windows.Forms
Imports System.Drawing

Public Class FormsHandling
    Public Shared Sub InitialFormPreparation(ByVal theForm As Form, bgColor As Color)
        'Übernimmt einfache Standard-Initialisierungen für den .Load() Event einer Form

        theForm.BackColor = bgColor
        theForm.FormBorderStyle = FormBorderStyle.None
        theForm.KeyPreview = True
        theForm.TopMost = False

    End Sub

End Class
