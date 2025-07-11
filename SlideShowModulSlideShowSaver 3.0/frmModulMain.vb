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
Imports Modul_SlideShowSaver_3
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SharedDataHandling
Imports TagLib

Public Class frmModulMain
    'Variablendeklaration

    'Bildanzeige
    Private Shared aktuelleSettings As ModulMain.ModulSettings_SSS_3_0
    Private Shared aktuellesBild As Image
    Private Shared neuesBild As Image
    Public Shared bildPfad As String = Nothing
    Private Shared initialePfade As New List(Of String)
    Private Shared aktuellesVerzeichnis As New List(Of String)
    Private Shared aktuellesVerzeichnisCounter As Integer
    Public Shared listeDerZuletztAngezeigtenBilder As New List(Of String)

    'Transitionen
    Public listOfAvailableTransitions As List(Of SlideShowTransitionInfo)
    Public listOfEnabledTransitions As List(Of String)
    Private aktiveTransition As ISlideShowTransition = Nothing
    Private neueTransition As String = Nothing
    Private transitionIstAktiv As Boolean = False

    'Shader
    Public listOfAvailableShaders As List(Of SlideShowShaderInfo)
    Public listOfEnabledShaders As List(Of String)
    Private neuerShader As String
    Private aktiverShader As ISlideShowShader = Nothing

    'Sonstiges
    Private rnd As New Random

    Private Sub frmModuleMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Initialisiert die Form, Steuerelemente und Settings vor der Anzeige
        FormsHandling.InitialFormPreparation(Me, Color.Black)
        Me.Text = "Modul SlideShowSaver 3.0"
        Me.TopMost = False

        'picBildAnzeige initialisieren
        picBildAnzeige.Dock = DockStyle.Fill
        picBildAnzeige.SizeMode = PictureBoxSizeMode.Zoom
        picBildAnzeige.BackColor = Color.Transparent

        'Aktuelle Settings abholen
        aktuelleSettings = ModulMain.aktuelleSettings

        'Liste der legalen Transitionen und Shader erstellen
        LegitimeTransitionsListeErstellen()
        LegitimeShaderListeErstellen()

        'erstes Bild Laden = Aktueller Desktop
        If StartBildWurdeVerwendet Then
            aktuellesBild = GetCurrentScreen()
        Else
            aktuellesBild = StartBild
            StartBildWurdeVerwendet = True
        End If

    End Sub

    Private Sub frmModulMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'Startet die Anzeige der Bilder


        'erstes Bild zum Wechseln laden
        If aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then
            Do
                aktuellesVerzeichnis = GetPicturesByDirectory()
            Loop Until aktuellesVerzeichnis.Count >= 1
            aktuellesVerzeichnisCounter = 1
            bildPfad = aktuellesVerzeichnis(0)
            neuesBild = GetPictureByName(bildPfad)
        Else
            bildPfad = GetPictures(1).Item(0)
            neuesBild = GetPictureByName(bildPfad)

        End If

        'Transition aussuchen
        If listOfEnabledTransitions.Count > 0 Then
            Select Case aktuelleSettings.TransitionsReihenfolge
                Case "In Reihenfolge"
                    neueTransition = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition")
                    If neueTransition = Nothing Then neueTransition = ""
                    neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, neueTransition)
                Case "Zufällig"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                Case "Zufällig bei Start"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
                Case "In Reihenfolge bei Start"
                    neueTransition = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition")
                    If neueTransition = Nothing Then neueTransition = ""
                    neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, neueTransition)
                Case Else
                    'Entspricht "Zufällig bei Start"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))
            End Select

            If aktiveTransition IsNot Nothing Then
                'Sinnlos bei Initialisierung, aber gehört sich ja so. Und vielleicht gibt es ja doch noch 
                'Leichen im Speicher...
                RemoveHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
            End If

            aktiveTransition = TransitionByNameLoader.LadeTransitionNachName(neueTransition)

            If aktiveTransition IsNot Nothing Then
                'Handler hinzufügen
                AddHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
            End If
        End If

        'Aktiven Shader laden 
        If listOfEnabledShaders.Count > 0 Then
            Select Case aktuelleSettings.ShaderReihenfolge
                Case "In Reihenfolge"
                    neuerShader = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader")
                    If neuerShader = Nothing Then neuerShader = ""
                    neuerShader = GetNextAlphabeticItemName(listOfEnabledShaders, neuerShader)
                Case "Zufällig"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
                Case "Zufällig bei Start"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
                Case "In Reihenfolge bei Start"
                    neuerShader = ReadFromRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader")
                    If neuerShader = Nothing Then neuerShader = ""
                    neuerShader = GetNextAlphabeticItemName(listOfEnabledTransitions, neuerShader)
                Case Else
                    'Entspricht "Zufällig bei Start"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
            End Select

            aktiverShader = ShaderByNameLoader.LadeShaderNachName(neuerShader)

            'Shader neuesBild anwenden. Das erste Bild (d.h. der aktuelle Desktop) bekommt keinen Shader
            neuesBild = aktiverShader.RunShader(neuesBild)
        End If

        'Erstes Bild anzeigen
        picBildAnzeige.Image = aktuellesBild
        picBildAnzeige.Refresh()

        'Bildinfo wird nicht initialisiert, da das erste Bild ein Screenshot vom Desktop ist.

        'Timer starten
        tmrModul.Interval = aktuelleSettings.Anzeigedauer * 1000
        tmrModul.Start()

    End Sub

    Private Sub frmModulMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        'Leitet Tastatureingaben über SlideShowTools.KeyAndMouseHandling an das MCP weiter.

        LogDebug("SlideShowModul SSS 3.0 hat den Key: " & e.KeyValue.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardKeyDown(Me, e)

    End Sub

    Private Sub frmModulMain_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        'Leitet Maustasten über SlideShowTools.KeyAndMouseHandling an das MCP weiter.

        LogDebug("SlideShowModul SSS 3.0 hat den MouseButton: " & e.Button.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardMouseDown(Me, e)

    End Sub

    Private Sub tmrModul_Tick(sender As Object, e As EventArgs) Handles tmrModul.Tick
        'Startet nach Ablauf der Bildanzeigedauer die nächste Transition
        Dim picBoxGFX As Graphics

        If transitionIstAktiv Then Exit Sub

        'Falls der Benutzer in der Zwischenzeit an den Optionen 'rumgepfuscht hat
        LegitimeTransitionsListeErstellen()

        If listOfEnabledTransitions.Count > 0 Then
            'Transition aussuchen
            Select Case aktuelleSettings.TransitionsReihenfolge
                Case "In Reihenfolge"
                    WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzteTransition", aktiveTransition.TransitionName)
                    neueTransition = GetNextAlphabeticItemName(listOfEnabledTransitions, aktiveTransition.TransitionName)

                    If aktiveTransition IsNot Nothing Then
                        'Alte Eventhandler löschen
                        RemoveHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
                    End If

                    aktiveTransition = TransitionByNameLoader.LadeTransitionNachName(neueTransition)

                    If aktiveTransition IsNot Nothing Then
                        'Handler hinzufügen
                        AddHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
                    End If

                Case "Zufällig"
                    neueTransition = listOfEnabledTransitions(rnd.Next(listOfEnabledTransitions.Count))

                    If aktiveTransition IsNot Nothing Then
                        'Alte Eventhandler löschen
                        RemoveHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
                    End If

                    aktiveTransition = TransitionByNameLoader.LadeTransitionNachName(neueTransition)

                    If aktiveTransition IsNot Nothing Then
                        'Handler hinzufügen
                        AddHandler aktiveTransition.TransitionIsRunning, AddressOf Transition_TransitionIsRunning
                    End If

                Case "Zufällig bei Start"
                ' Keine Aktion notwendig
                Case "In Reihenfolge bei Start"
                    ' Keine Aktion notwendig
                Case Else
                    ' Keine Aktion notwendig (entspricht zufällig bei Start, in .Shown() behandelt)
            End Select

            'Transition starten
            picBoxGFX = Graphics.FromHwnd(picBildAnzeige.Handle)
            aktiveTransition.RunTransition(aktuellesBild, PictureBoxSizeMode.Zoom, neuesBild, PictureBoxSizeMode.Zoom, picBoxGFX)

            'Status setzen und Timer beenden 
            transitionIstAktiv = True
            tmrModul.Stop()

        Else
            'Dann muss der Timer halt selber ran...

            Bildwechsel()
            transitionIstAktiv = False

        End If


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

    Public Sub LegitimeTransitionsListeErstellen()
        'Aktualisiert die Liste der Transitionen, die das Modul SlideShowSaver 3.0 aktuell anzeigen darf

        Dim enabledTransitionsRegVal As String
        Dim tempList As New List(Of String)
        Dim defaults As New Dictionary(Of String, String)

        ' Listen der Module und aktivierten Module neu Laden, gegeneinander abgleichen.
        defaults = ModulMain.GetModulDefaultSettings()

        listOfAvailableTransitions = TransitionListLoader.LadeTransitionInfoListe()
        enabledTransitionsRegVal = ReadFromRegOrDefaults(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "Transitionseffekte", defaults)

        tempList =
                enabledTransitionsRegVal.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries).
                Select(Function(s) s.Trim()).
                Where(Function(name) listOfAvailableTransitions.Any(
                    Function(transition) transition.TransitionName.Equals(name, StringComparison.OrdinalIgnoreCase))).
                Distinct(StringComparer.OrdinalIgnoreCase).
                OrderBy(Function(s) s).
                ToList()

        listOfEnabledTransitions = tempList

    End Sub

    Private Sub Transition_TransitionIsRunning(state As Boolean)
        'Wechselt das aktuelle Bild nach erfolgreichem Abschluss der Transition und startet tmrModul 

        'Sofort raus, falls die Transition noch läuft
        If state Then Exit Sub

        'Hier beginnt die Bildanzeige
        Bildwechsel()

        'Status setzen und tmrModul starten
        transitionIstAktiv = False
        tmrModul.Interval = aktuelleSettings.Anzeigedauer * 1000
        tmrModul.Start()

    End Sub

    Private Sub Bildwechsel()
        'Eigentliche Anzeige des Bildes, Verwaltungsaufgaben und Auswahl des nächsten Bildes

        aktuellesBild = neuesBild
        picBildAnzeige.Image = aktuellesBild
        picBildAnzeige.Refresh()

        If aktuelleSettings.BildInfoAnzeigen Then
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

        'Neues Bild laden. 
        If aktuelleSettings.Bildauswahl = "Zufallsverzeichnis" Then
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

        'Falls der Benutzer in der Zwischenzeit an den Optionen 'rumgepfuscht hat
        LegitimeShaderListeErstellen()

        If listOfEnabledShaders.Count > 0 Then
            'Aktiven Shader laden & auf neuesBild anwenden
            Select Case aktuelleSettings.ShaderReihenfolge
                Case "In Reihenfolge"
                    WriteToRegistry(ModulMain.SLIDESHOWMODUL_SSS_FULLPATH & "LetzterShader", aktiverShader.ShaderName)
                    neuerShader = GetNextAlphabeticItemName(listOfEnabledShaders, aktiverShader.ShaderName)
                    aktiverShader = ShaderByNameLoader.LadeShaderNachName(neuerShader)
                Case "Zufällig"
                    neuerShader = listOfEnabledShaders(rnd.Next(listOfEnabledShaders.Count))
                    aktiverShader = ShaderByNameLoader.LadeShaderNachName(neuerShader)
                Case "Zufällig bei Start"
                'Keine Aktion notwendig.
                Case "In Reihenfolge bei Start"
                    'Keine Aktion notwendig.
                Case Else
                    'keine Aktion notwendig, entspricht "Zufällig bei Start"
            End Select

            neuesBild = aktiverShader.RunShader(neuesBild)
        End If

    End Sub

End Class