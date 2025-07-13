Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowTools.ConversionHandling
Imports SlideShowTools.SettingsHandling
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
    Public Event PleaseChangeToTransition(sender As Object, transitionName As String) Implements ISlideShowModul.PleaseChangeToTransition

    ' === Lokale Variablen ===

    'Settings
    Private aktuelleSettings As ModulSettings_Mandelbrot
    Public Const SLIDESHOWMODUL_MANDELBROT_FULLPATH = SLIDESHOWMODULBASE_PATH & "Mandelbrot\"
    Public Const nameModul As String = "Mandelbrot"

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
        ' SlpashScreen anzeigen

        mandelbrotScreen = New frmModulMain()
        mandelbrotScreen.Show()

        RaiseEvent ModulStateChanged("Running")

    End Sub

    ' === Beenden ===
    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Aufräumen und Modul beenden

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
        'Holt sich die aktuellen Settings, speichert sie in der Settings-Inbox und gibt dann das ucOptionsModul zurück

        'Aktuelle Settings auslesen und zwischenspeichern
        ReadModuleSettingsFromRegistryOrDefaults()
        StoreSettings(ModulName, aktuelleSettings)

        'ucOptionsModul ausgeben
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

    Public Sub GetModulRegistryOrDefaultSettings(uc As UserControl) Implements ISlideShowModul.GetModulRegistryOrDefaultSettings
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

    Public Sub AttentionTransitionGewechselt(sender As Object, transitionName As String) Implements ISlideShowModul.AttentionTransitionGewechselt
        ' nicht verwendet
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        ' Initialisierung und Re-Initalisierung der Optionen aus der Registry
    End Sub

    'Private Funktionen

    Private Function GetModulDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Werte des Moduls

        Dim defaults As New Dictionary(Of String, String)

        defaults("Farbverlauf") = "Regenbogen"
        defaults("GradientAnimieren") = "True"
        defaults("KoordinatenAnzeigen") = "False"

        Return defaults

    End Function

    Private Sub ReadModuleSettingsFromRegistryOrDefaults()
        'Füllt die Struktur mit den Settings

        Dim defaults As New Dictionary(Of String, String)

        defaults = GetModulDefaultSettings()

        'Farbverlauf
        aktuelleSettings.Farbverlauf = ReadFromRegOrDefaults(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Farbverlauf", defaults)

        'Gradient Animieren
        aktuelleSettings.GradientAnimieren = CBool(ReadFromRegOrDefaults(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "GradientAnimieren", defaults))

        'Koordinaten Anzeigen
        aktuelleSettings.KoordinatenAnzeigen = CBool(ReadFromRegOrDefaults(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "KoordinatenAnzeigen", defaults))

    End Sub
End Class
