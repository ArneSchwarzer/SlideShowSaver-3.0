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
    Private Shared bildPfad As String
    Private Shared aktuellesVerzeichnis As New List(Of String)
    Private Shared aktuellesVerzeichnisCounter As Integer
    Private Shared listeDerZuletztAngezeigtenBilder As New List(Of String)

    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.TopMost = False

        'picBildAnzeige initialisieren
        picBildAnzeige.Dock = DockStyle.Fill
        picBildAnzeige.SizeMode = PictureBoxSizeMode.Zoom ' oder StretchImage, wenn du willst, dass das Bild verzerrt wird
        picBildAnzeige.BackColor = Color.Transparent

        'erstes Bild Laden
        If ModulMain.aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then
            Do
                aktuellesVerzeichnis = GetPicturesByDirectory()
            Loop Until aktuellesVerzeichnis.Count > 0

            aktuellesVerzeichnisCounter = 0
            bildPfad = aktuellesVerzeichnis(aktuellesVerzeichnisCounter)
        Else
            bildPfad = GetPictures(1).Item(0)
        End If

        aktuellesBild = GetPictureByName(bildPfad)

        'Shader anwenden, sobald implementiert
        'aktuellesBild = aktuellerShader.RunShader(aktuellesBild)

        picBildAnzeige.Image = aktuellesBild
        picBildAnzeige.Refresh()

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

        'Liste der letzten 10 Bilder befüllen und ggf. das erste Element wieder aus der Liste löschen.
        listeDerZuletztAngezeigtenBilder.Add(bildPfad)
        If listeDerZuletztAngezeigtenBilder.Count > 10 Then
            listeDerZuletztAngezeigtenBilder.RemoveAt(0)
        End If

        'Neues Bild laden. Wichtig später für Transitionen, um selbigen auch 2 Bilder zur Verfügung stellen zu können.
        If ModulMain.aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then

            aktuellesVerzeichnisCounter += 1

            If aktuellesVerzeichnisCounter = aktuellesVerzeichnis.Count Then

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

        'Jetzt die Transition aufrufen, sobald implementiert
        'aktuelleTransition.RunTransition(aktuellesBild, neuesBild)

        aktuellesBild = neuesBild
        picBildAnzeige.Image = aktuellesBild
        picBildAnzeige.Refresh()

    End Sub
End Class