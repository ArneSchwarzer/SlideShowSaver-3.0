Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowLogging.LogHandling
Public Class frmModulMain

    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.TopMost = False

        'Erst einmal den SplashScreen justieren & Labelfarben setzen
        Dim colorLabels As Color = Color.DarkViolet
        Label1.ForeColor = colorLabels
        Label2.ForeColor = colorLabels
        Label3.ForeColor = colorLabels
        Panel1.Left = (Me.ClientSize.Width - Panel1.Width) \ 2
        Panel1.Top = (Me.ClientSize.Height - Panel1.Height) \ 2
    End Sub

    Private Sub frmModulMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        LogDebug("SlideShowModul Mandelbrot hat den Key: " & e.KeyValue.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardKeyDown(Me, e)
    End Sub

    Private Sub frmModulMain_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        LogDebug("SlideShowModul Mandelbrot hat den MouseButton: " & e.Button.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardMouseDown(Me, e)
    End Sub
End Class