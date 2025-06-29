Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Windows.Forms
Imports System.Drawing

Public Class ModulMain
    Implements ISlideShowModul

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

    ' === Private Felder ===
    Private sssScreen As frmModulMain
    Private zwischenspeicherSettings As ModulSettings_SlideShowSaver_3_0

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

    ' === Start/Stop ===
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        sssScreen = New frmModulMain()
        sssScreen.Show()

        RaiseEvent ModulStateChanged("Running")
    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        If sssScreen IsNot Nothing AndAlso Not sssScreen.IsDisposed Then
            sssScreen.Close()
            sssScreen.Dispose()
            sssScreen = Nothing
        End If
        RaiseEvent ModulStateChanged("Stopped")
    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        ' Keine Pausefunktion in diesem Beispiel
    End Sub

    ' === Optionen / Settings ===

    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog
        Return New ucOptionsModul()
    End Function

    Public Function MemorizeModulSettings(uc As UserControl) As Object Implements ISlideShowModul.MemorizeModulSettings
        Dim dict = SlideShowTools.ConversionHandling.UserControlZuDictionary(uc)
        zwischenspeicherSettings = SlideShowTools.ConversionHandling.DictionaryZuStruktur(Of ModulSettings_SlideShowSaver_3_0)(dict)
        Return zwischenspeicherSettings
    End Function

    Public Sub ApplyModulSettings(settings As Object) Implements ISlideShowModul.ApplyModulSettings
        ' Optional: hier in Registry speichern
        If TypeOf settings Is ModulSettings_SlideShowSaver_3_0 Then
            zwischenspeicherSettings = CType(settings, ModulSettings_SlideShowSaver_3_0)
        End If
    End Sub

    Public Sub GetModulSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowModul.GetModulSettings
        If TypeOf restoreSettings Is ModulSettings_SlideShowSaver_3_0 Then
            Dim dict = SlideShowTools.ConversionHandling.StrukturZuDictionary(CType(restoreSettings, ModulSettings_SlideShowSaver_3_0))
            SlideShowTools.ConversionHandling.DictionaryZuUserControl(uc, dict)
        End If
    End Sub

    Public Sub GetModulDefaultSettings(uc As UserControl) Implements ISlideShowModul.GetModulDefaultSettings
        Dim defaults As New ModulSettings_SlideShowSaver_3_0 With {
            .Bildauswahl = "Zufallsbild",
            .Anzeigedauer = 20,
            .Transitionseffekte = New List(Of String),
            .TransitionsReihenfolge = "Zufällig bei Start",
            .Shader = New List(Of String),
            .ShaderReihenfolge = "Zufällig bei Start",
            .BildInfoAnzeigen = False
        }

        Dim dict = SlideShowTools.ConversionHandling.StrukturZuDictionary(defaults)
        SlideShowTools.ConversionHandling.DictionaryZuUserControl(uc, dict)
    End Sub

    ' === Info-Kommunikation ===

    Public Sub AttentionShaderGewechselt(shaderName As String) Implements ISlideShowModul.AttentionShaderGewechselt
        ' Noch keine Reaktion nötig
    End Sub

    Public Sub AttentionTransitionGewechselt(transitionName As String) Implements ISlideShowModul.AttentionTransitionGewechselt
        ' Noch keine Reaktion nötig
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        ' Initialisierung und Re-Initalisierung der Optionen aus der Registry
    End Sub

    ' === Interne Funktionen ===

End Class
