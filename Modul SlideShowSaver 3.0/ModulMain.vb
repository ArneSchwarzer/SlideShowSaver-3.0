Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling

Public Class ModulMain
    Implements ISlideShowModul

    ' Variablen, Konstanten, ENUMs etc. deklarieren.
    Public Shared aktuelleSettings As ModulSettings_SlideShowSaver_3_0
    Private Const SLIDESHOWMODULFULL_PATH As String = SLIDESHOWMODULBASE_PATH & "SlideShowSaver 3.0\"
    Public Shared sssScreen As frmModulMain
    Public Shared sssPause As frmPauseModusOverlay
    Public Shared sssInfo As frmPictureInfo
    Private zwischenspeicherSettings As ModulSettings_SlideShowSaver_3_0
    Public Shared pauseIsActive As Boolean = False


    'Übersetzungstabelle UC <--> Settings
    Private Shared ReadOnly translationTable As New Dictionary(Of String, String) From {
        {"chkBildinformationen", "BildInfoAnzeigen"},
        {"cmbBildauswahl", "Bildauswahl"},
        {"trbAnzeigedauer", "Anzeigedauer"},
        {"clbShader", "Shader"},
        {"cmbShaderauswahl", "ShaderReihenfolge"},
        {"clbTransitions", "Transitionseffekte"},
        {"cmbEffektauswahl", "TransitionsReihenfolge"}
    }

    ' === Struktur für die Moduloptionen ===
    Public Structure ModulSettings_SlideShowSaver_3_0
        Public Bildauswahl As String
        Public Anzeigedauer As Integer
        Public Transitionseffekte As List(Of String)
        Public TransitionsReihenfolge As String
        Public Shader As List(Of String)
        Public ShaderReihenfolge As String
        Public BildInfoAnzeigen As Boolean
    End Structure

    ' === Modul-Metadaten ===
    Public ReadOnly Property ModulName As String Implements ISlideShowModul.ModulName
        Get
            Return "SlideShowSaver 3.0"
        End Get
    End Property

    Public ReadOnly Property ModulBeschreibung As String Implements ISlideShowModul.ModulBeschreibung
        Get
            Return "Die klassische Diashow im neuen Gewand."
        End Get
    End Property

    Public ReadOnly Property ModulVersion As Version Implements ISlideShowModul.ModulVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    Public ReadOnly Property ModulNutztSlideShowBildauswahl As Boolean Implements ISlideShowModul.ModulNutztSlideShowBildauswahl
        Get
            Return True
        End Get
    End Property

    Public ReadOnly Property ModulNutztTransitions As Boolean Implements ISlideShowModul.ModulNutztTransitions
        Get
            Return True
        End Get
    End Property

    Public ReadOnly Property ModulNutztShader As Boolean Implements ISlideShowModul.ModulNutztShader
        Get
            Return True
        End Get
    End Property

    ' === Events ===
    Public Event ModulStateChanged(newState As String) Implements ISlideShowModul.ModulStateChanged
    Public Event PleaseChangeToShader(shaderName As String) Implements ISlideShowModul.PleaseChangeToShader
    Public Event PleaseChangeToTransition(transitionName As String) Implements ISlideShowModul.PleaseChangeToTransition


    ' === Start/Stop/Pause ===
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        'Initialisiert und startet das eigentliche Modul

        sssScreen = New frmModulMain()
        CheckYourSettings()
        RaiseEvent ModulStateChanged("Running")

        sssScreen.Show()
        'ZStackingSSS()

    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Räumt auf und meldet, das der Schoner ordungsgemäß beendet wurde

        If sssInfo IsNot Nothing AndAlso Not sssInfo.IsDisposed Then
            sssInfo.Close()
            sssInfo.Dispose()
            sssInfo = Nothing
        End If

        If sssPause IsNot Nothing AndAlso Not sssPause.IsDisposed Then
            sssPause.Close()
            sssPause.Dispose()
            sssPause = Nothing
        End If

        If sssScreen IsNot Nothing AndAlso Not sssScreen.IsDisposed Then
            sssScreen.Close()
            sssScreen.Dispose()
            sssScreen = Nothing
        End If

        RaiseEvent ModulStateChanged("Stopped")

    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        ' Startet den Pause-Modus des Moduls

        sssScreen.tmrModul.Stop()

        If sssPause Is Nothing Then
            sssPause = New frmPauseModusOverlay()
        End If

        pauseIsActive = True
        sssPause.Show()
        RaiseEvent ModulStateChanged("Pause")

    End Sub

    ' === Optionen / Settings ===

    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog
        'Liefert der frmOptionsMain das leere UC zum Einbau in die Modul-Tabpage.

        Return New ucOptionsModul()
    End Function

    Public Function MemorizeModulSettings(uc As UserControl) As Object Implements ISlideShowModul.MemorizeModulSettings
        'Liest die aktuellen Werte des 'eigenen' aus der frmOptionsMain aus und liefert diese als Structure zurück
        'an die Form, damit diese sie in den internen Zwischenspeicher der frmOptionsMain einlagern kann. Benötigt,
        'falls der Benutzer während einer Sitzung innerhalb von frmOptionsMain zwischen diversen Modulen
        'wechselt.

        Dim dict = SlideShowTools.ConversionHandling.UserControlZuDictionary(uc)
        Dim translatedDict As New Dictionary(Of String, String)

        'Wandlung der Dictionary-Keys von UC-Namen zu Registry/ModulSettings_SlideShowSaver_3_0-Namen
        For Each schluessel In dict.Keys
            translatedDict.Item(translationTable(schluessel)) = dict(schluessel)
        Next

        'Abgeschaltet bis der ""$§%"§$-Fehler in ConversionsHandling.DictionaryZuStruktur() gefunden wurde
        'zwischenspeicherSettings = SlideShowTools.ConversionHandling.DictionaryZuStruktur(Of ModulSettings_SlideShowSaver_3_0)(translatedDict)
        zwischenspeicherSettings.Bildauswahl = translatedDict("Bildauswahl")
        zwischenspeicherSettings.Anzeigedauer = CInt(translatedDict("Anzeigedauer"))
        zwischenspeicherSettings.Transitionseffekte = SplitSemicolonList(translatedDict("Transitionseffekte"))
        zwischenspeicherSettings.TransitionsReihenfolge = translatedDict("TransitionsReihenfolge")
        zwischenspeicherSettings.Shader = SplitSemicolonList(translatedDict("Shader"))
        zwischenspeicherSettings.ShaderReihenfolge = translatedDict("ShaderReihenfolge")
        zwischenspeicherSettings.BildInfoAnzeigen = translatedDict("BildInfoAnzeigen")

        Return zwischenspeicherSettings

    End Function

    Public Sub ApplyModulSettings(settings As Object) Implements ISlideShowModul.ApplyModulSettings
        'Schreibt die Settings nach Beenden der frmOptionsMain in die Registry

        'Cast weil "settings" (als Teil der generischen Interface-Deklaration) vom Typ Object ist.
        If TypeOf settings Is ModulSettings_SlideShowSaver_3_0 Then
            zwischenspeicherSettings = CType(settings, ModulSettings_SlideShowSaver_3_0)
        End If

        '...und ab dafür...
        WriteToRegistry(SLIDESHOWMODULFULL_PATH & "Bildauswahl", zwischenspeicherSettings.Bildauswahl)
        WriteToRegistry(SLIDESHOWMODULFULL_PATH & "Anzeigedauer", zwischenspeicherSettings.Anzeigedauer.ToString)
        WriteToRegistry(SLIDESHOWMODULFULL_PATH & "Transitionseffekte", JoinSemicolonList(zwischenspeicherSettings.Transitionseffekte))
        WriteToRegistry(SLIDESHOWMODULFULL_PATH & "TransitionsReihenfolge", zwischenspeicherSettings.TransitionsReihenfolge)
        WriteToRegistry(SLIDESHOWMODULFULL_PATH & "Shader", JoinSemicolonList(zwischenspeicherSettings.Shader))
        WriteToRegistry(SLIDESHOWMODULFULL_PATH & "ShaderReihenfolge", zwischenspeicherSettings.ShaderReihenfolge)
        WriteToRegistry(SLIDESHOWMODULFULL_PATH & "BildInfoAnzeigen", zwischenspeicherSettings.BildInfoAnzeigen.ToString)

    End Sub

    Public Sub GetModulSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowModul.GetModulSettings
#Region "Kommentar"
        'Übernimmt kurzzeitig die Herrschaft über die 'eigene' UC auf der tpModul, um die aus dem Zwischenspeicher
        'der frmOptionsMain gelieferten Werte zurückzuschreiben. Benötigt, falls der Benutzer während einer Sitzung
        'innerhalb von frmOptionsMain zwischen diversen Modulen wechselt.
        '
        'ACHTUNG!!!
        '
        'Die verwendete Hilfsfunktion "DictionaryZuUserControl" setzt nur Werte für 'Standard'-Controls (TextBox,
        'ComboBox, CheckBox, RadioButton, TrackBar, ListBox, CheckedListBox (Haken bei Einträgen
        'setzen, NICHT CheckedListBox mit Einträgen befüllen). Panels und GroupBoxen werden dabei rekursiv
        'durchlaufen. 
        'Alle anderen Control-Typen müssen danach ggf. noch in dieser Routine 'manuell' behandelt werden.
#End Region

        Dim translatedDic As New Dictionary(Of String, String)
        Dim dict As New Dictionary(Of String, String)

        If TypeOf restoreSettings Is ModulSettings_SlideShowSaver_3_0 Then
            dict = SlideShowTools.ConversionHandling.StrukturZuDictionary(CType(restoreSettings, ModulSettings_SlideShowSaver_3_0))

            'Wandlung der Dictionary-Keys von Registry/ModulSettings_SlideShowSaver_3_0-Namen zu UC-Namen
            For Each keyValuePair As KeyValuePair(Of String, String) In translationTable
                'dict:             cmbStimmung | "Mir doch egal"
                'translationTable: cmbStimmung | Stimmung
                'translatedDic:    Stimmung    | "Mir doch egal"

                If dict.ContainsKey(keyValuePair.Key) Then
                    translatedDic(keyValuePair.Value) = dict(keyValuePair.Key)
                End If
            Next


            'Werte im UserControl setzen
            SlideShowTools.ConversionHandling.DictionaryZuUserControl(uc, translatedDic)

        End If
    End Sub

    Public Sub GetModulRegistryOrDefaultSettings(uc As UserControl) Implements ISlideShowModul.GetModulRegistryOrDefaultSettings
        'Übernimmt kurzzeitig die Herrschaft über die 'eigene' UC auf der tpModul von frmOptionsMain, um die in
        'der Registry gespeicherten Werte (oder ggf. die Defaultwerte) in die UC zu schreiben. Benötigt beim
        'initialisieren der frmOptionsMain.
        '
        'ACHTUNG!!!
        '
        'Die verwendete Hilfsfunktion "DictionaryZuUserControl" setzt nur Werte für 'Standard'-Controls (TextBox,
        'ComboBox, CheckBox, RadioButton, TrackBar, ListBox, CheckedListBox (Haken bei Einträgen
        'setzen, NICHT CheckedListBox mit Einträgen befüllen). Panels und GroupBoxen werden dabei rekursiv
        'durchlaufen. 
        'Alle anderen Control-Typen müssen danach ggf. noch in dieser Routine "manuell" befüllt werden.

        Dim dict As New Dictionary(Of String, String)

        ReadModulSettingsFromRegistry()
        dict = SlideShowTools.ConversionHandling.StrukturZuDictionary(aktuelleSettings)
        SlideShowTools.ConversionHandling.DictionaryZuUserControl(uc, dict)

    End Sub

    Public Function GetModulDefaultSettings() As Dictionary(Of String, String)
        Dim defaultModulSettings As New Dictionary(Of String, String)
        'Liefert die Default-Werte des Moduls

        defaultModulSettings("Bildauswahl") = "Zufallsbild"
        defaultModulSettings("Anzeigedauer") = "20"
        defaultModulSettings("Transistionseffekte") = ""
        defaultModulSettings("TransitionsReihenfolge") = "Zufällig bei Start"
        defaultModulSettings("Shader") = ""
        defaultModulSettings("ShaderReihenfolge") = "Zufällig bei Start"
        defaultModulSettings("BildInfoAnzeigen") = "False"

        Return defaultModulSettings
    End Function

    Public Sub ReadModulSettingsFromRegistry()
        'Liest die Settings des Moduls aus der Registry. Falls diese nicht gesetzt sind, werden Default-Werte ausgegeben.

        Dim defaults As New Dictionary(Of String, String)
        Dim tempRegVal As String

        defaults = GetModulDefaultSettings()

        aktuelleSettings.Bildauswahl = ReadFromRegOrDefaults(SLIDESHOWMODULFULL_PATH & "Bildauswahl", defaults)
        aktuelleSettings.Anzeigedauer = CInt(ReadFromRegOrDefaults(SLIDESHOWMODULFULL_PATH & "Anzeigedauer", defaults))
        'Temporäre Sicherheitsmaßnahme
        If aktuelleSettings.Anzeigedauer < 5 Then
            aktuelleSettings.Anzeigedauer = 20 'Auf Default setzen
        End If
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWMODULFULL_PATH & "Transitionseffekte", defaults)
        aktuelleSettings.Transitionseffekte = SplitSemicolonList(tempRegVal)
        aktuelleSettings.TransitionsReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMODULFULL_PATH & "TransitionsReihenfolge", defaults)
        tempRegVal = ReadFromRegOrDefaults(SLIDESHOWMODULFULL_PATH & "Shader", defaults)
        aktuelleSettings.Shader = SplitSemicolonList(tempRegVal)
        aktuelleSettings.ShaderReihenfolge = ReadFromRegOrDefaults(SLIDESHOWMODULFULL_PATH & "ShaderReihenfolge", defaults)
        If ReadFromRegOrDefaults(SLIDESHOWMODULFULL_PATH & "BildInfoAnzeigen", defaults) = "True" Then
            aktuelleSettings.BildInfoAnzeigen = True
        Else
            aktuelleSettings.BildInfoAnzeigen = False
        End If

    End Sub


    ' === Info-Kommunikation ===

    Public Sub AttentionShaderGewechselt(shaderName As String) Implements ISlideShowModul.AttentionShaderGewechselt
        ' Noch keine Reaktion nötig
    End Sub

    Public Sub AttentionTransitionGewechselt(transitionName As String) Implements ISlideShowModul.AttentionTransitionGewechselt
        ' Noch keine Reaktion nötig
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        ' Re-Initalisierung der Optionen aus der Registry nachdem das MCP eine Änderung gemeldet hat.

        ReadModulSettingsFromRegistry()

        If aktuelleSettings.BildInfoAnzeigen Then
            sssInfo = New frmPictureInfo()
            sssInfo.Show()
        ElseIf sssInfo IsNot Nothing Then
            sssInfo.Close()
            sssInfo.Dispose()
            sssInfo = Nothing
        End If

        'ZStackingSSS()

        'Timer gemäß der neuen Anzeigedauer setzen. Stop/Start, um die
        'Änderungen sofort wirken zu lassen (falls z.B. die Anzeigedauer von 2m auf 20s zurückgesetzt wurde,
        'möchte der Benutzer keine 2 Minuten warten, bis die Änderung greift).

        sssScreen.tmrModul.Stop()
        If aktuelleSettings.Anzeigedauer > 0 Then
            sssScreen.tmrModul.Interval = aktuelleSettings.Anzeigedauer * 1000
        Else
            sssScreen.tmrModul.Interval = 20 * 1000 'Defaultwert, falls beim Lesen aus der Registry etwas falsch gelaufen ist
        End If
        sssScreen.tmrModul.Start()

    End Sub

    ' === Interne Funktionen ===

    Public Sub ZStackingSSS()
        'Sortiert die Fenster von SlideShowSaver 3.0 
        If sssScreen IsNot Nothing Then
            If Not sssScreen.Visible Then
                sssScreen.Show()
            End If
            sssScreen.BringToFront()
        End If

        If aktuelleSettings.BildInfoAnzeigen AndAlso sssInfo IsNot Nothing Then
            If Not sssInfo.Visible Then
                sssInfo.Show()
            End If
            sssInfo.BringToFront()
        End If

        If pauseIsActive AndAlso sssPause IsNot Nothing Then
            If Not sssPause.Visible Then
                sssPause.Show()
            End If
            sssInfo.BringToFront()
        End If

    End Sub
End Class
