Imports SlideShowInterfaces.InterfaceDeclarations
Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowTools.ConversionHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowLogging.LogHandling

Public Class ModulMain
    Implements ISlideShowModul

#Region "Variablendeklaration"
    'Variablendeklaration

    'Settings
    Private aktuelleSettings As ModulSettings_Mandelbrot
    Public Const SLIDESHOWMODUL_MANDELBROT_FULLPATH = SLIDESHOWMODULBASE_PATH & "Mandelbrot\"
    Public Const nameModul As String = "Mandelbrot"

    'Sonstiges
    Public mandelbrotScreen As wpfModulMain
#End Region

#Region "Structures & Enums"
    ' === Settings-Structure ===
    Public Structure ModulSettings_Mandelbrot
        Dim Gradienten As List(Of String)
        Dim GradientAnimieren As Boolean
        Dim KoordinatenAnzeigen As Boolean
    End Structure
#End Region

    'Moduleigenschaften
    Public ReadOnly Property ModulName As String Implements ISlideShowModul.ModulName
        Get
            Return nameModul
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

#Region "Eventdeklaration"
    ' === Events ===
    Public Event ModulStateChanged(newState As String) Implements ISlideShowModul.ModulStateChanged
    Public Shared Event YouHaveMail_Mandelbrot()
#End Region


    'Start, Stopp & Pause
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        'Modul anzeigen

        CheckYourSettings()

        Try
            mandelbrotScreen = New wpfModulMain()
        Catch ex As Exception
            LogError("Modul Mandelbrot - ModulMain.StartModul(): Das Modul konnte nicht geladen werden:" & ex.ToString)
        End Try

        mandelbrotScreen.WindowState = System.Windows.WindowState.Maximized
        mandelbrotScreen.Show()

        RaiseEvent ModulStateChanged("Running")

    End Sub

    Public Sub StopModul() Implements ISlideShowModul.StopModul
        'Aufräumen und Modul beenden

        If mandelbrotScreen IsNot Nothing Then
            mandelbrotScreen.Close()
            mandelbrotScreen = Nothing
        End If

        RaiseEvent ModulStateChanged("Stopped")

    End Sub

    Public Sub PauseModusModul() Implements ISlideShowModul.PauseModusModul
        ' Noch nicht implementiert
    End Sub

    'Optionen und Optionsdialog
    Public Function GetModulOptionsDialog() As UserControl Implements ISlideShowModul.GetModulOptionsDialog
        'Holt sich die aktuellen Settings, speichert sie in der Settings-Inbox und gibt dann das ucOptionsModul zurück

        'Aktuelle Settings auslesen und zwischenspeichern
        ReadModuleSettingsFromRegistryOrDefaults()
        StoreSettings(ModulName, aktuelleSettings)

        'ucOptionsModul ausgeben
        Return New ucOptionsModul()

    End Function

    'Info-Kommunikation
    Public Sub CheckYourSettings() Implements ISlideShowModul.CheckYourSettings
        ' Initialisierung und Re-Initalisierung der Optionen aus der Registry

        'aktuelleSettings einlesen und gleich auch in der SettingsInbox bereitstellen
        ReadModuleSettingsFromRegistryOrDefaults()
        StoreSettings(nameModul, aktuelleSettings)

        'Der Instanz mandelbrotScreen auch Bescheid geben
        RaiseEvent YouHaveMail_Mandelbrot()

    End Sub

    'Private Funktionen
    Private Function GetModulDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Werte des Moduls

        Dim defaults As New Dictionary(Of String, String)

        defaults("Gradienten") = "Regenbogen"
        defaults("GradientAnimieren") = "True"
        defaults("KoordinatenAnzeigen") = "False"

        Return defaults

    End Function

    Private Sub ReadModuleSettingsFromRegistryOrDefaults()
        'Füllt die Struktur mit den Settings

        Dim defaults As New Dictionary(Of String, String)
        Dim tmpRegVal As String

        defaults = GetModulDefaultSettings()

        'Farbverlauf
        tmpRegVal = ReadFromRegOrDefaults(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Gradienten", defaults)
        aktuelleSettings.Gradienten = SplitSemicolonList(tmpRegVal)

        'Gradient Animieren
        aktuelleSettings.GradientAnimieren = CBool(ReadFromRegOrDefaults(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "GradientAnimieren", defaults))

        'Koordinaten Anzeigen
        aktuelleSettings.KoordinatenAnzeigen = CBool(ReadFromRegOrDefaults(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "KoordinatenAnzeigen", defaults))

    End Sub
End Class
