Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.ConversionHandling

Public Class ModulMain
    Implements ISlideShowModul

    ' === INTERN ===
    Private matrixScreen As frmModulMain
    Private zwischenspeicherSettings As ModulSettings_Matrix
    Private highlightTexte As List(Of String)
    Private szenendauer As Integer

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
    Public Event PleaseChangeToTransition(transitionName As String) Implements ISlideShowModul.PleaseChangeToTransition

    ' === START/STOP ===
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        matrixScreen = New frmModulMain()
        matrixScreen.Show()
        RaiseEvent ModulStateChanged("Running")
    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
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
        Return New ucOptionsModul()
    End Function

    Public Function MemorizeModulSettings(uc As UserControl) As Object Implements ISlideShowModul.MemorizeModulSettings
        Dim dict = UserControlZuDictionary(uc)
        zwischenspeicherSettings = DictionaryZuStruktur(Of ModulSettings_Matrix)(dict)
        Return zwischenspeicherSettings
    End Function

    Public Sub ApplyModulSettings(settings As Object) Implements ISlideShowModul.ApplyModulSettings
        zwischenspeicherSettings = DirectCast(settings, ModulSettings_Matrix)
        ' Bei späterer Funktionalität können hier Ressourcen basierend auf Einstellungen geladen werden.
    End Sub

    Public Sub GetModulSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowModul.GetModulSettings
        Dim settings As ModulSettings_Matrix = DirectCast(restoreSettings, ModulSettings_Matrix)
        Dim dict = StrukturZuDictionary(settings)
        DictionaryZuUserControl(uc, dict)
    End Sub

    Public Sub GetModulRegistryOrDefaultSettings(uc As UserControl) Implements ISlideShowModul.GetModulRegistryOrDefaultSettings
        Dim defaults As New ModulSettings_Matrix With {
            .HighlightTexte = New List(Of String) From {"Follow the white rabbit", "Wake up, Neo"},
            .SzenendauerSekunden = 20
        }
        Dim dict = StrukturZuDictionary(defaults)
        DictionaryZuUserControl(uc, dict)
    End Sub

    ' === INFO ===
    Public Sub AttentionShaderGewechselt(shaderName As String) Implements ISlideShowModul.AttentionShaderGewechselt
        ' Wird nicht verwendet
    End Sub

    Public Sub AttentionTransitionGewechselt(transitionName As String) Implements ISlideShowModul.AttentionTransitionGewechselt
        ' Wird nicht verwendet
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        ' Initialisierung und Re-Initalisierung der Optionen aus der Registry
    End Sub
End Class
