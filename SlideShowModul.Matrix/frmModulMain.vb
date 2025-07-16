Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowTools.ScreenHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowLogging.LogHandling
Imports Modul_Matrix.ModulMain


Public Class frmModulMain

#Region "Variablendeklaration"
    'Variablendeklaration
    Private Shared aktuelleSettings As ModulSettings_Matrix
#End Region

    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Form und ihre Steuerelemente

        Dim uc As UserControl

        'Form Initialisieren
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.Text = "Modul Matrix"
        Me.TopMost = False

        'Eventhandler
        AddHandler YouHaveMail_Matrix, AddressOf CheckYourMail

        'AktuelleSettings einlesen
        CheckYourMail()

        'UC Under Constuction einlesen
        uc = New ucUnderConstruction()

        uc.Top = (Me.ClientSize.Height - uc.Height) \ 2
        uc.Left = (Me.ClientSize.Width - uc.Width) \ 2
        Me.Controls.Add(uc)

    End Sub

    Private Sub frmModulMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'Initiale Anzeige der Form

        'Jetzt die Form sichtbar machen
        Me.WindowState = FormWindowState.Maximized

    End Sub

    Private Sub frmModulMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        LogDebug("SlideShowModul Matrix hat den Key: " & e.KeyValue.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardKeyDown(Me, e)
    End Sub

    Private Sub frmModulMain_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        LogDebug("SlideShowModul Matrix hat den MouseButton: " & e.Button.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardMouseDown(Me, e)
    End Sub

    Private Sub CheckYourMail()
        'Liest die aktuellen Settings aus der SettingsInbox ein

        GetSettings(Of ModulSettings_Matrix)(nameModul)

    End Sub
End Class