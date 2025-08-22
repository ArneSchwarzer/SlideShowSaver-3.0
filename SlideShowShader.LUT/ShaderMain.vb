Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowLoader.LUTByNameLoader
Imports SlideShowLogging
Imports SlideShowTools.SettingsHandling
Imports System.Drawing
Imports SlideShowTools.ListHandling
Imports System.Windows.Forms
Imports SlideShowTools.LUTHandling

Public Class ShaderMain
    Implements ISlideShowShader

    'Variablendeklaration
    'Allgemeines
    Private aktuelleSettings As ShaderSettings_LUT
    Public Const SLIDESHOWSHADER_LUT_FULLPATH As String = SLIDESHOWSHADER_PATH & "LUT\"
    Public Const nameShader As String = "LUT"

    'LUT Verarbeitung
    Private currentLUT As LutInfo

    Public Structure ShaderSettings_LUT
        Public LUTs As List(Of String)
        Public intensitaet As Integer
    End Structure

    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return nameShader
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Wendet Color-Lookup-Tabellen (LUTs) auf das Bild an"
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Der eigentliche Shader
        Dim rnd As New Random

        'Aktuelle Settings abholen und in SettingsInbox speichern
        ReadShaderSettingsFromRegistryOrDefaults()
        StoreSettings(nameShader, aktuelleSettings)

        'LUT aus der Liste der LUTs aussuchen
        currentLUT = LUTByNameLoader(aktuelleSettings.LUTs(rnd.Next(aktuelleSettings.LUTs.Count)))

        Return WpfShaderRunner.ApplyLutEffect(baseImage, currentLUT, CSng(aktuelleSettings.intensitaet))

    End Function

    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Liefert den Options-Dialog des Shaders

        'Aktuelle Settings abholen und in SettingsInbox speichern
        ReadShaderSettingsFromRegistryOrDefaults()
        StoreSettings(nameShader, aktuelleSettings)

        Return New ucOptionsShader
    End Function

    'Private Metohden

    'Settings
    Public Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Werte des Shaders

        Dim defaults As New Dictionary(Of String, String)

        'Die Potenz zur Basis 2 wird gespeichert - d.h. 2^4 = 16 px
        defaults("LUTs") = ""
        defaults("Intensität") = "33"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Holt die Settings aus der Registry (oder aus Default-Werten) und legt sie in aktuelleSettings ab.

        Dim defaults As Dictionary(Of String, String) = GetShaderDefaultSettings()

        aktuelleSettings.LUTs = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWSHADER_LUT_FULLPATH & "LUTs", defaults))
        aktuelleSettings.intensitaet = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_LUT_FULLPATH & "Intensität", defaults))

    End Sub
End Class
