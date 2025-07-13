Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InfoHandling


Public Class InterfaceDeclarations

    ' =========================================================
    ' INTERFACE: ISlideShowModul
    ' =========================================================

    ''' <summary>
    ''' Dieses Interface definiert die Struktur eines Bildschirmschoner-Moduls.
    ''' 
    ''' 
    ''' Hinweis zur Eventweiterleitung:
    ''' 
    ''' Module sollten Tastatur- oder Mauseingaben standardmäßig über
    ''' SlideShowTools.KeyAndMouseHandling an das Hauptprogramm weiterleiten.
    ''' </summary>


    Public Interface ISlideShowModul

        ' --- Eigenschaften ---
        ReadOnly Property ModulName As String
        ReadOnly Property ModulBeschreibung As String
        ReadOnly Property ModulVersion As Version
        ReadOnly Property ModulNutztSlideShowBildauswahl As Boolean
        ReadOnly Property ModulNutztTransitions As Boolean
        ReadOnly Property ModulNutztShader As Boolean

        ' --- Events ---
        Event ModulStateChanged(newState As String)
        Event PleaseChangeToShader(shaderName As String)
        Event PleaseChangeToTransition(sender As Object, transitionName As String)

        ' --- Modulsteuerung ---
        Sub StartModul(targetScreen As Screen, Optional isPreview As Boolean = False, Optional targetHandle As IntPtr = Nothing)
        Sub StopModul()
        Sub PauseModusModul()

        ' --- Optionen/Dialoghandling ---
        Function GetModulOptionsDialog() As UserControl
        Function MemorizeModulSettings(uc As UserControl) As Object
        Sub ApplyModulSettings(settings As Object)
        Sub GetModulSettings(uc As UserControl, restoreSettings As Object)
        Sub GetModulRegistryOrDefaultSettings(uc As UserControl)

        ' --- Info-Kommunikation ---
        Sub AttentionShaderGewechselt(shaderName As String)
        Sub AttentionTransitionGewechselt(sender As Object, transitionName As String)
        Sub CheckYourSettings()

    End Interface



    ' =========================================================
    ' INTERFACE: ISlideShowTransition
    ' =========================================================


    Public Interface ISlideShowTransition

        ' --- Eigenschaften ---
        ReadOnly Property TransitionName As String
        ReadOnly Property TransitionKurzBeschreibung As String
        ReadOnly Property TransitionVersion As Version

        ' --- Events ---
        Event TransitionIsRunning(state As Boolean)
        Event PleaseChangeToShader(shaderName As String)

        ' --- Ausführung ---
        Sub RunTransition(oldImage As Image, picBoxModeOld As PictureBoxSizeMode, newImage As Image, picBoxModeNew As PictureBoxSizeMode, targetGraphics As Graphics, Optional clientSize As Size = Nothing, Optional durationMs As Integer = 0)
        Sub StopTransition()

        ' --- Optionen/Dialoghandling ---
        Function GetTransitionOptionsDialog() As UserControl
        Function MemorizeTransitionSettings(uc As UserControl) As Object
        Sub ApplyTransitionSettings(settings As Object)
        Sub GetTransitionSettings(uc As UserControl, restoreSettings As Object)
        Sub GetTransitionRegistryOrDefaultSettings(uc As UserControl)

        ' --- Info-Kommunikation ---
        Sub AttentionShaderGewechselt(shaderName As String)
        Sub CheckYourSettings()

    End Interface



    ' =========================================================
    ' INTERFACE: ISlideShowShader
    ' =========================================================


    Public Interface ISlideShowShader

        ' --- Eigenschaften ---
        ReadOnly Property ShaderName As String
        ReadOnly Property ShaderKurzBeschreibung As String
        ReadOnly Property ShaderVersion As Version

        ' --- Ausführung ---
        Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image

        ' --- Optionen/Dialoghandling ---
        Function GetShaderOptionsDialog() As UserControl
        Function MemorizeShaderSettings(uc As UserControl) As Object
        Sub ApplyShaderSettings(settings As Object)
        Sub GetShaderSettings(uc As UserControl, restoreSettings As Object)
        Sub GetShaderRegistryOrDefaultSettings(uc As UserControl)

        ' --- Info-Kommunikation
        Sub CheckYourSettings()

    End Interface

End Class
