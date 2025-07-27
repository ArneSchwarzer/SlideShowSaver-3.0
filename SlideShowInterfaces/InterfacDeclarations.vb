Imports System.Drawing
Imports System.Windows.Forms
Imports System.Windows.Media.Imaging
Imports SlideShowInterfaces.InfoHandling


Public Class InterfaceDeclarations

    ' =========================================================
    ' INTERFACE: ISlideShowModul
    ' =========================================================

    ''' <summary>
    ''' Dieses Interface definiert die Struktur eines Bildschirmschoner-Moduls.
    ''' 
    ''' 
    ''' Wichtige Hinweis zu Modul-Implementierung und Eventweiterleitung:
    '''
    '''Neben den in diesem Interface vorgegebenen Methoden sollte ein Modul auch immer
    '''eine Struktur ModulSettings_Modulname sowie diese beiden Methoden anbieten:
    '''
    ''' Private Function GetModulDefaultSettings() as Dictionary(Of String, String)
    ''' Private Sub ReadModulSettingsFromRegistryOrDefault()
    ''' 
    ''' Die frmModulMain sollte Tastatur- oder Mauseingaben standardmäßig über
    ''' SlideShowTools.KeyAndMouseHandling an das Hauptprogramm weiterleiten.
    ''' </summary>

    Public Interface ISlideShowModul

        'Eigenschaften
        ReadOnly Property ModulName As String
        ReadOnly Property ModulBeschreibung As String
        ReadOnly Property ModulVersion As Version
        ReadOnly Property ModulNutztSlideShowBildauswahl As Boolean
        ReadOnly Property ModulNutztTransitions As Boolean
        ReadOnly Property ModulNutztShader As Boolean

        'Events
        Event ModulStateChanged(newState As String)

        'Modulsteuerung
        Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing)
        Sub StopModul()
        Sub PauseModusModul()

        'Optionen/Dialoghandling
        Function GetModulOptionsDialog() As UserControl

        'Info-Kommunikation
        Sub CheckYourSettings()

    End Interface


    ' =========================================================
    ' INTERFACE: ISlideShowTransition
    ' =========================================================

    ''' <summary>
    ''' Dieses Interface definiert die Struktur eines Bildübergangs (Transition) von einem
    ''' Bild zum nächsten.
    ''' 
    ''' 
    ''' Wichtige Hinweis zur Transition-Implementierung:
    '''
    '''Neben den in diesem Interface vorgegebenen Methoden sollte eine Transition auch immer
    '''eine Struktur TransitionSettings_TransitionName sowie diese beiden Methoden anbieten:
    '''
    ''' Private Function GetTransitionDefaultSettings() as Dictionary(Of String, String)
    ''' Private Sub ReaTransitionSettingsFromRegistryOrDefault()
    ''' 
    ''' </summary>

    Public Interface ISlideShowTransition

        'Eigenschaften
        ReadOnly Property TransitionName As String
        ReadOnly Property TransitionKurzBeschreibung As String
        ReadOnly Property TransitionVersion As Version

        'Events
        Event TransitionIsRunning(state As Boolean)
        Event TransitionFrameIstFertig(bitmap As RenderTargetBitmap)

        'Ausführung
        Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As BitmapImage, picBoxModeNew As PictureBoxSizeMode, clientSize As Size, Optional durationMs As Integer = 0)
        Sub StopTransition()

        'Optionen/Dialoghandling
        Function GetTransitionOptionsDialog() As UserControl

    End Interface


    ' =========================================================
    ' INTERFACE: ISlideShowShader
    ' =========================================================

    ''' <summary>
    ''' Dieses Interface definiert die Struktur eines Shaders.
    ''' 
    ''' 
    ''' Wichtige Hinweis zur Shader-Implementierung:
    '''
    '''Neben den in diesem Interface vorgegebenen Methoden sollte ein Shader auch immer eine Struktur
    '''ShaderSettings_ShaderName sowie diese beiden Methoden anbieten:
    '''
    ''' Private Function GetShaderDefaultSettings() as Dictionary(Of String, String)
    ''' Private Sub ReadShaderSettingsFromRegistryOrDefault()
    ''' 
    ''' </summary>

    Public Interface ISlideShowShader

        'Eigenschaften
        ReadOnly Property ShaderName As String
        ReadOnly Property ShaderKurzBeschreibung As String
        ReadOnly Property ShaderVersion As Version

        'Ausführung
        Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image

        'Optionen/Dialoghandling
        Function GetShaderOptionsDialog() As UserControl

    End Interface


    ' =========================================================
    ' INTERFACE: ISlideShowTransitionCommunication
    ' =========================================================

    ''' <summary>
    ''' Dieses Interface definiert die Kommunikation zwischen einer ucOptionsModul
    ''' und der frmOptionsMain, um auf derer TabPage "Transition" das korrekte ucOptionsTransition zu
    ''' laden.
    ''' </summary>

    Public Interface ISlideShowTransitionCommunication

        'Event
        Event PleaseChangeToTransition(sender As Object, transitionName As String)

    End Interface


    ' =========================================================
    ' INTERFACE: ISlideShowShaderCommunication
    ' =========================================================

    ''' <summary>
    ''' Dieses Interface definiert die Kommunikation zwischen einer ucOptionsModul
    ''' und der frmOptionsMain, um auf derer TabPage "Shader" das korrekte ucOptionsShader zu
    ''' laden.
    ''' </summary>

    Public Interface ISlideShowShaderCommunication

        'Event
        Event PleaseChangeToShader(shaderName As String)

    End Interface

End Class
