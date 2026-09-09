Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowTools.ScreenHandling
Imports SlideShowLogging.LogHandling
Imports Modul_Matrix.ModulMain

Public Class frmModulMain

#Region "Variablendeklaration"
    'Variablendeklaration

    Private aktuelleSettings As ModulSettings_Matrix

    Private wirdGeschlossen As Boolean
    Private darstellungsbereitschaftWurdeGemeldet As Boolean

#End Region

#Region "Events"

    Public Event DarstellungIstBereit()

#End Region

#Region "Konstruktor"

    Public Sub New(settings As ModulSettings_Matrix)

        InitializeComponent()
        AktualisiereSettingsIntern(settings)

    End Sub

#End Region

#Region "Initialisierung"

    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Form und ihre Steuerelemente.

        Dim uc As UserControl

        FormsHandling.InitialFormPreparation(Me, Color.Black)

        Me.Text = "Modul Matrix"
        Me.TopMost = False

        Me.Top = 0
        Me.Left = 0

        uc = New ucUnderConstruction()

        uc.Left = (GetNativeScreenResolution().Width - uc.Width) \ 2
        uc.Top = (GetNativeScreenResolution().Height - uc.Height) \ 2

        Me.Controls.Add(uc)

    End Sub

    Private Sub frmModulMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'Maximiert das Modul und plant die Meldung
        'der Darstellungsbereitschaft ein.

        Me.WindowState = FormWindowState.Maximized

        If wirdGeschlossen OrElse IsDisposed OrElse Disposing Then

            Exit Sub

        End If

        Try

            BeginInvoke(New MethodInvoker(AddressOf MeldeDarstellungsbereitschaft))

        Catch ex As InvalidOperationException

            LogDebug("Modul Matrix - frmModulMain_Shown(): Die Darstellungsbereitschaft konnte nicht mehr " &
                     "eingeplant werden: " & ex.ToString())

        End Try

    End Sub

#End Region

#Region "Darstellungsbereitschaft"

    Private Sub MeldeDarstellungsbereitschaft()
        'Meldet nach abgeschlossenem Layout- und Zeichenzyklus,
        'dass das Modul vollständig dargestellt werden kann.

        If wirdGeschlossen OrElse IsDisposed OrElse Disposing OrElse darstellungsbereitschaftWurdeGemeldet Then

            Exit Sub

        End If

        Me.Refresh()

        darstellungsbereitschaftWurdeGemeldet = True

        RaiseEvent DarstellungIstBereit()

    End Sub

#End Region

#Region "Settings"

    Public Sub AktualisiereSettings(settings As ModulSettings_Matrix)
        'Übernimmt aktualisierte Moduleinstellungen.

        If wirdGeschlossen OrElse IsDisposed OrElse Disposing Then

            Exit Sub

        End If

        AktualisiereSettingsIntern(settings)

    End Sub

    Private Sub AktualisiereSettingsIntern(settings As ModulSettings_Matrix)
        'Übernimmt die Settings und kopiert veränderbare Listen.

        aktuelleSettings = settings

        If settings.HighlightTexte IsNot Nothing Then

            aktuelleSettings.HighlightTexte = New List(Of String)(settings.HighlightTexte)

        Else

            aktuelleSettings.HighlightTexte = New List(Of String)()

        End If

    End Sub

#End Region

#Region "Tastatur und Maus"

    Private Sub frmModulMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        LogDebug("SlideShowModul Matrix hat den Key: " & e.KeyValue.ToString() &
                 " empfangen. Leite weiter an Eventhandler.")

        ForwardKeyDown(Me, e)

    End Sub

    Private Sub frmModulMain_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown

        LogDebug("SlideShowModul Matrix hat den MouseButton: " & e.Button.ToString() &
                 " empfangen. Leite weiter an Eventhandler.")

        ForwardMouseDown(Me, e)

    End Sub

#End Region

#Region "Lebenszyklus"

    Private Sub frmModulMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        wirdGeschlossen = True

    End Sub

#End Region

End Class