Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowTools.ConversionHandling
Imports SlideShowTools.RegistryHandling

Public Class ModulMain

    Implements ISlideShowModul

    ' === Moduleigenschaften ===
    Public ReadOnly Property ModulName As String Implements ISlideShowModul.ModulName
        Get
            Return "Mandelbrot"
        End Get
    End Property

    Public ReadOnly Property ModulBeschreibung As String Implements ISlideShowModul.ModulBeschreibung
        Get
            Return "Zeigt animierte Mandelbrot-Fraktale mit Farbverlauf."
        End Get
    End Property

    Public ReadOnly Property ModulVersion As Version Implements ISlideShowModul.ModulVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    Public ReadOnly Property ModulNutztSlideShowBildauswahl As Boolean Implements ISlideShowModul.ModulNutztSlideShowBildauswahl
        Get
            Return False
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

    ' === Events ===
    Public Event ModulStateChanged(newState As String) Implements ISlideShowModul.ModulStateChanged
    Public Event PleaseChangeToShader(shaderName As String) Implements ISlideShowModul.PleaseChangeToShader
    Public Event PleaseChangeToTransition(transitionName As String) Implements ISlideShowModul.PleaseChangeToTransition

    ' === Lokale Variablen ===

    'Settings
    Private gradient As String
    Private gradientAnimieren As Boolean
    Private koordinatenAnzeigen As Boolean
    Private aktuelleSettings As ModulSettings_Mandelbrot
    Private zwischenspeicherSettings As ModulSettings_Mandelbrot
    Private dictMandelbrot As Dictionary(Of String, String)

    'Sonstiges
    Public mandelbrotScreen As frmModulMain



    ' === Settings-Structure ===
    Public Structure ModulSettings_Mandelbrot
        Dim Farbverlauf As String
        Dim GradientAnimieren As Boolean
        Dim KoordinatenAnzeigen As Boolean
    End Structure

    ' === Starten ===
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        ' SpashScreen anzeigen
        mandelbrotScreen = New frmModulMain()
        mandelbrotScreen.Show()

        RaiseEvent ModulStateChanged("Running")
    End Sub

    ' === Beenden ===
    Public Sub StopModul() Implements ISlideShowModul.StopModul
        If mandelbrotScreen IsNot Nothing Then
            mandelbrotScreen.Close()
            mandelbrotScreen.Dispose()
            mandelbrotScreen = Nothing
        End If
        RaiseEvent ModulStateChanged("Stopped")
    End Sub

    ' === Pausieren ===
    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        ' Noch nicht implementiert
    End Sub

    ' === Optionsdialog ===
    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog
        Return New ucOptionsModul()
    End Function

    Public Function MemorizeModulSettings(uc As UserControl) As Object Implements ISlideShowModul.MemorizeModulSettings
        Dim dict = UserControlZuDictionary(uc)
        zwischenspeicherSettings = DictionaryZuStruktur(Of ModulSettings_Mandelbrot)(dict)
        Return zwischenspeicherSettings
    End Function

    Public Sub ApplyModulSettings(settings As Object) Implements ISlideShowModul.ApplyModulSettings
        If TypeOf settings Is ModulSettings_Mandelbrot Then
            aktuelleSettings = CType(settings, ModulSettings_Mandelbrot)
        End If
    End Sub

    Public Sub GetModulSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowModul.GetModulSettings
        If restoreSettings Is Nothing Then Exit Sub
        Dim dict = StrukturZuDictionary(CType(restoreSettings, ModulSettings_Mandelbrot))
        DictionaryZuUserControl(uc, dict)
    End Sub

    Public Sub GetModulDefaultSettings(uc As UserControl) Implements ISlideShowModul.GetModulDefaultSettings
        Dim defaults As New ModulSettings_Mandelbrot With {
            .Farbverlauf = "Regenbogen",
            .GradientAnimieren = False,
            .KoordinatenAnzeigen = False
        }
        Dim dict = StrukturZuDictionary(defaults)
        DictionaryZuUserControl(uc, dict)
    End Sub

    ' === Info-Kommunikation ===
    Public Sub AttentionShaderGewechselt(shaderName As String) Implements ISlideShowModul.AttentionShaderGewechselt
        ' nicht verwendet
    End Sub

    Public Sub AttentionTransitionGewechselt(transitionName As String) Implements ISlideShowModul.AttentionTransitionGewechselt
        ' nicht verwendet
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        ' Initialisierung und Re-Initalisierung der Optionen aus der Registry
    End Sub


End Class
