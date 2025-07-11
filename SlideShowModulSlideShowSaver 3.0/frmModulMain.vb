Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools
Imports SlideShowTools.KeyAndMouseHandling
Imports SlideShowLogging.LogHandling
Imports SlideShowBildauswahl.BildauswahlMain
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowLoader
Imports SlideShowLogging
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling

Public Class frmModulMain
    'Variablendeklaration
    Private Shared aktuellesBild As Image
    Private Shared neuesBild As Image
    Public Shared bildPfad As String = Nothing
    Private Shared initialePfade As New List(Of String)
    Private Shared aktuellesVerzeichnis As New List(Of String)
    Private Shared aktuellesVerzeichnisCounter As Integer
    Public Shared listeDerZuletztAngezeigtenBilder As New List(Of String)

    Public listOfAvailableTransitions As List(Of SlideShowTransitionInfo)
    Public listOfEnabledTransitions As List(Of String)
    Private aktiveTransition As ISlideShowTransition = Nothing

    Public listOfAvailableShaders As List(Of SlideShowShaderInfo)
    Public listOfEnabledShaders As List(Of String)
    Private aktiverShader As ISlideShowShader = Nothing

    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.Text = "Modul SlideShowSaver 3.0"
        Me.TopMost = False

        'picBildAnzeige initialisieren
        picBildAnzeige.Dock = DockStyle.Fill
        picBildAnzeige.SizeMode = PictureBoxSizeMode.Zoom
        picBildAnzeige.BackColor = Color.Transparent

    End Sub

    Private Sub frmModulMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim zuLadenderShader As String
        Dim rnd As New Random

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

        'Liste der legalen Shader erstellen
        LegitimeShaderListeErstellen()

        'Aktiven Shader laden
        'If ModulMain.aktuelleSettings.ShaderReihenfolge = "In Reihenfolge" Then
        '    If aktiverShader Is Nothing Then
        '        zuLadenderShader = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader")
        '        listOfEnabledShaders.Sort()
        '        aktiverShader = ShaderByNameLoader.LadeShaderNachName(GetNextAlphabeticItemName(listOfEnabledShaders, zuLadenderShader))
        '    End If
        'Else
        '    If aktiverShader Is Nothing Then
        '        zuLadenderShader = listOfEnabledShaders(Rnd.next(listOfEnabledShaders.Count))
        '        aktiverShader = ShaderByNameLoader.LadeShaderNachName(zuLadenderShader)
        '    End If
        'End If

        'aktiverShader = ShaderByNameLoader.LadeShaderNachName("Tönen und Färben")
        ''Shader anwenden
        'If aktiverShader IsNot Nothing Then
        '    aktuellesBild = aktiverShader.RunShader(aktuellesBild)
        '    neuesBild = aktiverShader.RunShader(neuesBild)
        'End If

        'Transition laden (erst einmal gefaked)
        aktiveTransition = TransitionByNameLoader.LadeTransitionNachName("Schieben & Wischen")

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
        Dim nextShaderName As String = Nothing
        Dim rnd As New Random
        Dim gfx As Graphics = Graphics.FromHwnd(picBildAnzeige.Handle)

        'Jetzt die Transition aufrufen
        aktiveTransition.RunTransition(aktuellesBild, PictureBoxSizeMode.Zoom, neuesBild, PictureBoxSizeMode.Zoom, gfx)

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

        'Jetzt Shader laden und anwenden
        'If ModulMain.aktuelleSettings.ShaderReihenfolge = "In Reihenfolge" Then
        '    WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader", aktiverShader.ShaderName)
        '    nextShaderName = GetNextAlphabeticItemName(listOfEnabledShaders, aktiverShader.ShaderName)
        '    aktiverShader = ShaderByNameLoader.LadeShaderNachName(nextShaderName)
        'ElseIf ModulMain.aktuelleSettings.ShaderReihenfolge = "Zufällig" Then
        '    nextShaderName = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
        'End If

        'aktiverShader.RunShader(neuesBild)

    End Sub

    Public Sub LegitimeShaderListeErstellen()
        'Aktualisiert die Liste der Shader, die das Modul SlideShowSaver 3.0 aktuell anzeigen darf

        Dim enabledShadersRegVal As String
        Dim tempList As New List(Of String)
        Dim defaults As New Dictionary(Of String, String)

        ' Listen der Module und aktivierten Module neu Laden, gegeneinander abgleichen.
        defaults = ModulMain.GetModulDefaultSettings()

        listOfAvailableShaders = ShaderListLoader.LadeShaderInfoListe()
        enabledShadersRegVal = ReadFromRegOrDefaults(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Shader", defaults)

        tempList =
                enabledShadersRegVal.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries).
                Select(Function(s) s.Trim()).
                Where(Function(name) listOfAvailableShaders.Any(
                    Function(shader) shader.ShaderName.Equals(name, StringComparison.OrdinalIgnoreCase))).
                Distinct(StringComparer.OrdinalIgnoreCase).
                OrderBy(Function(s) s).
                ToList()

        listOfEnabledShaders = tempList

    End Sub

End Class