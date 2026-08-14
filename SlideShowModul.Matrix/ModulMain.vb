Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.ListHandling
Imports SlideShowLogging

Public Class ModulMain
    Implements ISlideShowModul

#Region "Variablendeklaration"
    'Variablendeklaration

    Private matrixScreen As frmModulMain
    Private aktuelleSettings As ModulSettings_Matrix

    Private wurdeBereinigt As Boolean

    Public Shared nameModul As String = "Matrix"
    Public Shared SLIDESHOWMODUL_MATRIX_FULLPATH As String = SLIDESHOWMODULBASE_PATH & "Matrix\"

#End Region

#Region "Strukturen, Enums etc."

    Public Structure ModulSettings_Matrix

        Public HighlightTexte As List(Of String)
        Public SzenendauerSekunden As Integer

    End Structure

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property ModulName As String Implements ISlideShowModul.ModulName

        Get
            Return nameModul
        End Get

    End Property

    Public ReadOnly Property ModulBeschreibung As String Implements ISlideShowModul.ModulBeschreibung

        Get
            Return "The Matrix has you..."
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
            Return False
        End Get

    End Property

    Public ReadOnly Property ModulNutztShader As Boolean Implements ISlideShowModul.ModulNutztShader

        Get
            Return False
        End Get

    End Property

#End Region

#Region "Events"

    Public Event ModulStateChanged(newState As String) Implements ISlideShowModul.ModulStateChanged

    Public Event ModulIstDarstellungsbereit() Implements ISlideShowModul.ModulIstDarstellungsbereit

#End Region

#Region "Start / Stop / Pause"

    Public Sub StartModul(
        targetScreen As Screen,
        Optional isPreview As Boolean = False,
        Optional targetHandle As IntPtr = Nothing) _
        Implements ISlideShowModul.StartModul
        'Startet das Matrix-Modul als neue Fensterinstanz.

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ModulMain))

        End If

        BeendeUndBereinigeMatrixFenster("Modul Matrix - ModulMain.StartModul(): " &
                                        "Fehler beim Beenden einer vorhandenen Fensterinstanz: ")

        CheckYourSettings()

        Try

            matrixScreen = New frmModulMain(aktuelleSettings)

            AddHandler matrixScreen.DarstellungIstBereit, AddressOf MatrixScreen_DarstellungIstBereit
            AddHandler matrixScreen.FormClosed, AddressOf MatrixScreen_FormClosed

            matrixScreen.WindowState = FormWindowState.Normal

            matrixScreen.Show()

            RaiseEvent ModulStateChanged("Running")

        Catch ex As Exception

            LogHandling.LogError("Modul Matrix - ModulMain.StartModul(): Fehler beim Laden des Moduls: " &
                                 ex.ToString())

            BeendeUndBereinigeMatrixFenster("Modul Matrix - ModulMain.StartModul(): " &
                                            "Fehler beim Aufräumen nach einem fehlgeschlagenen Start: ")

            RaiseEvent ModulStateChanged("Error")

            Throw

        End Try

    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Räumt auf und beendet das Modul.

        BeendeUndBereinigeMatrixFenster("Modul Matrix - ModulMain.StopModul(): " &
                                        "Fehler beim Beenden und Bereinigen des Modulfensters: ")

        RaiseEvent ModulStateChanged("Stopped")

    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        'Wird für dieses Modul nicht benötigt.

    End Sub

#End Region

#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
        'Beendet das Modul endgültig und gibt gehaltene Ressourcen frei.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        BeendeUndBereinigeMatrixFenster("Modul Matrix - ModulMain.Dispose(): " &
                                        "Fehler beim Beenden und Bereinigen des Modulfensters: ")

        GC.SuppressFinalize(Me)

    End Sub

#End Region

#Region "Eventweiterleitung"

    Private Sub MatrixScreen_DarstellungIstBereit()
        'Leitet die Darstellungsbereitschaft an das Framework weiter.

        RaiseEvent ModulIstDarstellungsbereit()

    End Sub

    Private Sub MatrixScreen_FormClosed(sender As Object, e As FormClosedEventArgs)
        'Löst alle Verbindungen zu einem selbständig geschlossenen Fenster.

        Dim geschlossenesFenster As frmModulMain

        geschlossenesFenster = TryCast(sender, frmModulMain)

        If geschlossenesFenster Is Nothing Then
            Exit Sub
        End If

        RemoveHandler geschlossenesFenster.DarstellungIstBereit, AddressOf MatrixScreen_DarstellungIstBereit
        RemoveHandler geschlossenesFenster.FormClosed, AddressOf MatrixScreen_FormClosed

        If ReferenceEquals(matrixScreen, geschlossenesFenster) Then

            matrixScreen = Nothing

        End If

    End Sub

#End Region

#Region "Optionen / Optionsdialog"

    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog
        'Liest die aktuellen Settings ein und liefert das Options-Control.

        ReadModulSettingsFromRegistryOrDefaults()

        StoreSettings(nameModul, aktuelleSettings)

        Return New ucOptionsModul()

    End Function

#End Region

#Region "Settings-Kommunikation"

    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        'Initialisiert oder aktualisiert die Moduleinstellungen.

        ReadModulSettingsFromRegistryOrDefaults()

        StoreSettings(nameModul, aktuelleSettings)

        If matrixScreen IsNot Nothing Then

            matrixScreen.AktualisiereSettings(aktuelleSettings)

        End If

    End Sub

#End Region

#Region "Private Methoden"

    Private Sub BeendeUndBereinigeMatrixFenster(fehlerPraefix As String)
        'Beendet das aktuelle Matrix-Fenster und löst alle
        'von ModulMain registrierten Verbindungen.

        Dim zuBeendendesFenster As frmModulMain

        zuBeendendesFenster = matrixScreen

        matrixScreen = Nothing

        If zuBeendendesFenster Is Nothing Then
            Exit Sub
        End If

        Try

            RemoveHandler zuBeendendesFenster.DarstellungIstBereit, AddressOf MatrixScreen_DarstellungIstBereit
            RemoveHandler zuBeendendesFenster.FormClosed, AddressOf MatrixScreen_FormClosed

            zuBeendendesFenster.Close()
            zuBeendendesFenster.Dispose()

        Catch ex As Exception

            LogHandling.LogError(fehlerPraefix & ex.ToString())

        End Try

    End Sub

    Friend Shared Function GetModulDefaults() As Dictionary(Of String, String)
        'Liefert die Default-Einstellungen des Moduls.

        Dim defaults As New Dictionary(Of String, String)

        defaults("Highlighttexte") = ""
        defaults("SzenendauerSekunden") = "20"

        Return defaults

    End Function

    Private Sub ReadModulSettingsFromRegistryOrDefaults()
        'Setzt aktuelle Settings entweder per Registry
        'oder per Defaultwerten.

        Dim temRegVal As String
        Dim defaults As Dictionary(Of String, String)

        defaults = GetModulDefaults()

        temRegVal = ReadFromRegOrDefaults(SLIDESHOWMODUL_MATRIX_FULLPATH & "Highlighttexte", defaults)

        aktuelleSettings.HighlightTexte = SplitSemicolonList(temRegVal)
        aktuelleSettings.SzenendauerSekunden = CInt(ReadFromRegOrDefaults(SLIDESHOWMODUL_MATRIX_FULLPATH &
                    "SzenendauerSekunden", defaults))

    End Sub

#End Region

End Class