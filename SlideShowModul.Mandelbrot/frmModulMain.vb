Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowTools.ScreenHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowLogging.LogHandling
Imports Modul_Mandelbrot.ModulMain

Public Class frmModulMain

#Region "Variablendeklaration"
    'Variablendeklaration
    Private Shared aktuelleSettings As ModulSettings_Mandelbrot
#End Region

    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Form und ihre Steuerelemente

        Dim uc As UserControl

        'Form Initialisieren
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.Text = "Modul Mandelbrot"
        Me.TopMost = False

        'Handler definieren
        AddHandler YouHaveMail_Mandelbrot, AddressOf CheckYourMail

        'Settings einlesen
        CheckYourMail()

        'UserControl ucUnderConstruction
        uc = New ucUnderConstruction()

        uc.Top = (Me.ClientSize.Height - uc.Height) \ 2
        uc.Left = (Me.ClientSize.Width - uc.Width) \ 2
        Me.Controls.Add(uc)

    End Sub

    Private Sub frmModulMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'Initiales Anzeigen der Form

        Me.WindowState = FormWindowState.Maximized

    End Sub

    Private Sub frmModulMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        LogDebug("SlideShowModul Mandelbrot hat den Key: " & e.KeyValue.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardKeyDown(Me, e)
    End Sub

    Private Sub frmModulMain_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        LogDebug("SlideShowModul Mandelbrot hat den MouseButton: " & e.Button.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardMouseDown(Me, e)
    End Sub

    Private Sub CheckYourMail()
        'Holt die aktuelleSettings aus der SettingsHandling.SettingsInbox ab.

        aktuelleSettings = GetSettings(Of ModulSettings_Mandelbrot)(nameModul)

    End Sub

End Class