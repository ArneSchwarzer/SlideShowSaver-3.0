Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowLogging.LogHandling

Public Class frmModulMain
    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim uc As UserControl

        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.Text = "Modul Matrix"
        Me.TopMost = False

        uc = New ucUnderConstruction()
        uc.Dock = DockStyle.None
        uc.Top = (Me.Height - uc.Height) \ 2
        uc.Left = (Me.Width - uc.Width) \ 2
        Me.Controls.Add(uc)
    End Sub
    Private Sub frmModulMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        LogDebug("SlideShowModul Matrix hat den Key: " & e.KeyValue.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardKeyDown(Me, e)
    End Sub

    Private Sub frmModulMain_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        LogDebug("SlideShowModul Matrix hat den MouseButton: " & e.Button.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardMouseDown(Me, e)
    End Sub
End Class