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
        Dim Zoomgeschwindigkeit As Integer
        Dim Rotation As Boolean
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
            Return "Kurze Mandelbrotfraktal-Zooms (bis 3.5e-5)"
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
    Public Event ModulIstDarstellungsbereit() Implements ISlideShowModul.ModulIstDarstellungsbereit
    Public Shared Event YouHaveMail_Mandelbrot()
#End Region


    'Start, Stopp & Pause
    Public Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing) Implements ISlideShowModul.StartModul
        'Initialisiert und zeigt das Mandelbrot-Modul

        'Eine eventuell noch vorhandene Fensterinstanz
        'darf nicht wiederverwendet werden.
        If mandelbrotScreen IsNot Nothing Then

            Try

                RemoveHandler mandelbrotScreen.Closed, AddressOf MandelbrotScreen_Closed

                mandelbrotScreen.Close()

            Catch
                'Eine alte oder bereits geschlossene Fensterinstanz
                'ist nicht mehr verwendbar.
            Finally
                mandelbrotScreen = Nothing
            End Try

        End If

        CheckYourSettings()

        Try

            mandelbrotScreen = New wpfModulMain()

            AddHandler mandelbrotScreen.Closed, AddressOf MandelbrotScreen_Closed
            AddHandler mandelbrotScreen.DarstellungIstBereit, AddressOf MandelbrotScreen_DarstellungIstBereit

            mandelbrotScreen.WindowState = System.Windows.WindowState.Maximized
            mandelbrotScreen.Show()

            RaiseEvent ModulStateChanged("Running")

        Catch ex As Exception

            LogError(
            "Modul Mandelbrot - ModulMain.StartModul(): " &
            "Das Modul konnte nicht gestartet werden: " &
            ex.ToString())

            If mandelbrotScreen IsNot Nothing Then

                Try

                    RemoveHandler mandelbrotScreen.Closed, AddressOf MandelbrotScreen_Closed
                    RemoveHandler mandelbrotScreen.DarstellungIstBereit, AddressOf MandelbrotScreen_DarstellungIstBereit

                    mandelbrotScreen.Close()

                Catch
                    'Keine weitere Behandlung notwendig
                Finally
                    mandelbrotScreen = Nothing
                End Try

            End If

            RaiseEvent ModulStateChanged("Error")

            Throw

        End Try

    End Sub

    Public Sub StopModul() _
    Implements ISlideShowModul.StopModul
        'Räumt auf und beendet das Mandelbrot-Modul

        If mandelbrotScreen IsNot Nothing Then

            Try

                RemoveHandler mandelbrotScreen.Closed, AddressOf MandelbrotScreen_Closed
                RemoveHandler mandelbrotScreen.DarstellungIstBereit, AddressOf MandelbrotScreen_DarstellungIstBereit

                mandelbrotScreen.Close()

            Catch ex As Exception

                LogError(
                "Modul Mandelbrot - ModulMain.StopModul(): " &
                "Fehler beim Schließen des Modulfensters: " &
                ex.ToString())

            Finally

                mandelbrotScreen = Nothing

            End Try

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

    Private Sub MandelbrotScreen_DarstellungIstBereit()
        'Leitet die WPF-Darstellungsbereitschaft an das Framework weiter.

        RaiseEvent ModulIstDarstellungsbereit()

    End Sub

    Private Function GetModulDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Werte des Moduls

        Dim defaults As New Dictionary(Of String, String)

        defaults("Gradienten") = "Regenbogen"
        defaults("GradientAnimieren") = "True"
        defaults("KoordinatenAnzeigen") = "False"
        defaults("Zoomgeschwindigkeit") = "1"
        defaults("Rotation") = "True"

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

        'Zoomgeschwindigkeit
        aktuelleSettings.Zoomgeschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Zoomgeschwindigkeit", defaults))

        'Koordinaten Anzeigen
        aktuelleSettings.Rotation = CBool(ReadFromRegOrDefaults(SLIDESHOWMODUL_MANDELBROT_FULLPATH & "Rotation", defaults))
    End Sub

    Private Sub MandelbrotScreen_Closed(
    sender As Object,
    e As EventArgs)
        'Entfernt die Fensterreferenz auch bei internem Schließen.

        Dim geschlossenesFenster As wpfModulMain

        geschlossenesFenster =
            TryCast(sender, wpfModulMain)

        If geschlossenesFenster IsNot Nothing Then

            RemoveHandler geschlossenesFenster.Closed,
                AddressOf MandelbrotScreen_Closed

        End If

        If ReferenceEquals(
            mandelbrotScreen,
            geschlossenesFenster) Then

            mandelbrotScreen = Nothing

        End If

    End Sub

End Class
