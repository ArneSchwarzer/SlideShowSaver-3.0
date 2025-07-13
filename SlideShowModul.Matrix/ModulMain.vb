Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ConversionHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.ListHandling

Public Class ModulMain
    Implements ISlideShowModul

    ' === INTERN ===
    Private matrixScreen As frmModulMain

    Private Shared aktuelleSettings As ModulSettings_Matrix
    Public Shared nameModul As String = "Matrix"
    Public Shared SLIDESHOWMODUL_MATRIX_FULLPATH As String = SLIDESHOWMODULBASE_PATH & "Matrix\"

    ' === OPTIONEN-STRUKTUR ===
    Public Structure ModulSettings_Matrix
        Public HighlightTexte As List(Of String)
        Public SzenendauerSekunden As Integer
    End Structure

    ' === METADATEN ===
    Public ReadOnly Property ModulName As String Implements ISlideShowModul.ModulName
        Get
            Return "Matrix"
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

    ' === EVENTS ===
    Public Event ModulStateChanged(newState As String) Implements ISlideShowModul.ModulStateChanged
    Public Event PleaseChangeToShader(shaderName As String) Implements ISlideShowModul.PleaseChangeToShader
    Public Event PleaseChangeToTransition(sender As Object, transitionName As String) Implements ISlideShowModul.PleaseChangeToTransition

    ' === START/STOP ===
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        'Started das Modul als Instanz

        matrixScreen = New frmModulMain()
        matrixScreen.Show()

        RaiseEvent ModulStateChanged("Running")

    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Räumt auf und beendet das Modul

        If matrixScreen IsNot Nothing Then
            matrixScreen.Close()
            matrixScreen.Dispose()
            matrixScreen = Nothing
        End If

        RaiseEvent ModulStateChanged("Stopped")

    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        ' Wird für dieses Modul nicht benötigt
    End Sub

    ' === OPTIONEN ===
    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog
        'Liest die aktuellen Settings ein, speichert sie in SettingsInbox und liefert dann das ucOptionsModul

        'Aktuelle Settings einlesen und abspeichern
        ReadModulSettingsFromRegistryOrDefaults()
        StoreSettings(nameModul, aktuelleSettings)

        'Und jetzt die ucOptionsModul ausgeben
        Return New ucOptionsModul()

    End Function

    Public Function MemorizeModulSettings(uc As UserControl) As Object Implements ISlideShowModul.MemorizeModulSettings
        'Keine Funktion
    End Function

    Public Sub ApplyModulSettings(settings As Object) Implements ISlideShowModul.ApplyModulSettings
        'Keine Funktion
    End Sub

    Public Sub GetModulSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowModul.GetModulSettings
        'Keine Funktion
    End Sub

    Public Sub GetModulRegistryOrDefaultSettings(uc As UserControl) Implements ISlideShowModul.GetModulRegistryOrDefaultSettings
        'Keine Funktion
    End Sub

    ' === INFO ===
    Public Sub AttentionShaderGewechselt(shaderName As String) Implements ISlideShowModul.AttentionShaderGewechselt
        ' Wird nicht verwendet
    End Sub

    Public Sub AttentionTransitionGewechselt(sender As Object, transitionName As String) Implements ISlideShowModul.AttentionTransitionGewechselt
        ' Wird nicht verwendet
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        ' Initialisierung und Re-Initalisierung der Optionen aus der Registry
    End Sub

    Private Function GetModulDefaults() As Dictionary(Of String, String)
        'Liefert die Default-Einstellungen des Moduls

        Dim defaults As New Dictionary(Of String, String)

        defaults("Highlighttexte") = ""
        defaults("SzenendauerSekunden") = "20"

        Return defaults

    End Function

    Private Sub ReadModulSettingsFromRegistryOrDefaults()
        'Setzt aktuelle Settings entweder per Registry oder per Defaultwerten

        Dim temRegVal As String
        Dim defaults As Dictionary(Of String, String)

        defaults = GetModulDefaults()

        temRegVal = ReadFromRegOrDefaults(SLIDESHOWMODUL_MATRIX_FULLPATH & "Highlighttexte", defaults)
        aktuelleSettings.HighlightTexte = SplitSemicolonList(temRegVal)

        aktuelleSettings.SzenendauerSekunden = CInt(ReadFromRegOrDefaults(SLIDESHOWMODUL_MATRIX_FULLPATH & "SzenendauerSekunden", defaults))

    End Sub
End Class
