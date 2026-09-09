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
    Public Const SLIDESHOWSHADER_LUT_FULLPATH As String = SLIDESHOWSHADER_PATH & "LUT\"
    Public Const nameShader As String = "LUT"

    Private aktuelleSettings As ShaderSettings_LUT

    Private ReadOnly rnd As New Random()
    Private shaderRunner As WpfShaderRunner

    Private wurdeBereinigt As Boolean

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

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "",
                              Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Wendet eine zufällig ausgewählte aktivierte LUT auf das Bild an.

        Dim lutName As String
        Dim intensitaet As Single

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ShaderMain))

        End If

        If baseImage Is Nothing Then

            Throw New ArgumentNullException(NameOf(baseImage))

        End If

        ReadShaderSettingsFromRegistryOrDefaults()

        StoreSettings(nameShader, aktuelleSettings)

        If aktuelleSettings.LUTs Is Nothing OrElse aktuelleSettings.LUTs.Count = 0 Then

            Return DirectCast(baseImage.Clone(), Image)

        End If

        lutName = aktuelleSettings.LUTs(rnd.Next(aktuelleSettings.LUTs.Count))

        currentLUT = LUTByNameLoader(lutName)

        intensitaet = CSng(aktuelleSettings.intensitaet / 100.0F)

        If shaderRunner Is Nothing Then

            shaderRunner = New WpfShaderRunner()

        End If

        Return shaderRunner.ApplyLutEffect(baseImage, currentLUT, intensitaet)

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
    Friend Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
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

    'Bereinigen und Dispose
#Region "IDisposable"

    Public Sub Dispose() _
    Implements IDisposable.Dispose
        'Gibt sämtliche vom LUT-Shader gehaltenen Ressourcen frei.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt =
        True

        If shaderRunner IsNot Nothing Then

            shaderRunner.Dispose()
            shaderRunner = Nothing

        End If

        currentLUT =
        Nothing

        GC.SuppressFinalize(
        Me)

    End Sub

#End Region

End Class
