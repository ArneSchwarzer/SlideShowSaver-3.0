Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowLogging.LogHandling
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowInterfaces.InterfaceDeclarations

Public Class frmModulMain
    'Variablendeklaration
    Private Shared aktuellesBild As Image
    Private Shared neuesBild As Image
    Public Shared bildPfad As String = Nothing
    Private Shared initialePfade As New List(Of String)
    Private Shared aktuellesVerzeichnis As New List(Of String)
    Private Shared aktuellesVerzeichnisCounter As Integer
    Public Shared listeDerZuletztAngezeigtenBilder As New List(Of String)

    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.TopMost = False

        'Bitte warten Label vorbereiten
        lblInitializing.Left = (Me.Width - lblInitializing.Width) \ 2
        lblInitializing.Top = (Me.Height - lblInitializing.Height) \ 2


        'picBildAnzeige initialisieren
        picBildAnzeige.Dock = DockStyle.Fill
        picBildAnzeige.SizeMode = PictureBoxSizeMode.Zoom ' oder StretchImage, wenn du willst, dass das Bild verzerrt wird
        picBildAnzeige.BackColor = Color.Transparent

    End Sub

    Private Sub frmModulMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        'Bitte warten Label anzeigen
        lblInitializing.Visible = True

        'erstes Bild Laden
        If ModulMain.aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then
            Do
                aktuellesVerzeichnis = GetPicturesByDirectory()
            Loop Until aktuellesVerzeichnis.Count > 1 'Das initiale Verzeichnis mus mindestens 2 legitime Bilder haben, später reicht auch eines
            initialePfade = aktuellesVerzeichnis 'Nur die ersten beiden Bilder werden gebraucht, aber sonst gibt es eine "Out of Bounds"-Exception
            initialePfade(0) = aktuellesVerzeichnis(0)
            initialePfade(1) = aktuellesVerzeichnis(1)
            aktuellesVerzeichnisCounter = 1
        Else
            initialePfade = GetPictures(2)
        End If


        aktuellesBild = GetPictureByName(initialePfade(0))
        neuesBild = GetPictureByName(initialePfade(1))

        listeDerZuletztAngezeigtenBilder.Add(initialePfade(0))
        listeDerZuletztAngezeigtenBilder.Add(initialePfade(1))

        'Shader anwenden, sobald implementiert
        'aktuellesBild = aktuellerShader.RunShader(aktuellesBild)

        lblInitializing.Visible = False
        picBildAnzeige.Image = aktuellesBild
        picBildAnzeige.Refresh()

        'Bildinfo aktualisieren
        If ModulMain.aktuelleSettings.BildInfoAnzeigen Then
            ModulMain.sssInfo.RefreshLabels(initialePfade(0))
        End If
    End Sub

    Private Sub frmModulMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        LogDebug("SlideShowModul SSS 3.0 hat den Key: " & e.KeyValue.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardKeyDown(Me, e)
    End Sub

    Private Sub frmModulMain_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        LogDebug("SlideShowModul SSS 3.0 hat den MouseButton: " & e.Button.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardMouseDown(Me, e)
    End Sub

    Private Sub tmrModul_Tick(sender As Object, e As EventArgs) Handles tmrModul.Tick

        'Jetzt die Transition aufrufen, sobald implementiert
        'aktuelleTransition.RunTransition(aktuellesBild, neuesBild)

        aktuellesBild = neuesBild
        picBildAnzeige.Image = aktuellesBild
        picBildAnzeige.Refresh()

        If ModulMain.aktuelleSettings.BildInfoAnzeigen Then
            If bildPfad IsNot Nothing Then
                ModulMain.sssInfo.RefreshLabels(bildPfad)
            Else
                ModulMain.sssInfo.RefreshLabels(initialePfade(1))
            End If
            ModulMain.sssInfo.Refresh()
        End If

        'Liste der letzten 10 Bilder befüllen und ggf. das erste Element wieder aus der Liste löschen.
        If bildPfad IsNot Nothing Then
            listeDerZuletztAngezeigtenBilder.Add(bildPfad)
            If listeDerZuletztAngezeigtenBilder.Count > 10 Then
                listeDerZuletztAngezeigtenBilder.RemoveAt(0)
            End If
        End If

        'Neues Bild laden. Da dies nach dem Anzeigen des neuen Bildes geschieht, hat ein evtl. Shader einen tmrMain-Tick Zeit...
        If ModulMain.aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then

            aktuellesVerzeichnisCounter += 1

            If aktuellesVerzeichnisCounter >= aktuellesVerzeichnis.Count Then

                aktuellesVerzeichnisCounter = 0
                Do
                    aktuellesVerzeichnis = GetPicturesByDirectory()
                Loop Until aktuellesVerzeichnis.Count > 0

            End If

            bildPfad = aktuellesVerzeichnis(aktuellesVerzeichnisCounter)

        Else

            bildPfad = GetPictures(1).Item(0)

        End If

        neuesBild = GetPictureByName(bildPfad)
        'Jetzt Shader anwenden, sobald implementiert
        'aktuellerShader.RunShader(neuesBild)

    End Sub

End Class