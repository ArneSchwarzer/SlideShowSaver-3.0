Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging
Imports SlideShowLogging.LogHandling
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling
Imports System.Drawing
Imports System.Windows.Media
Imports System.Windows.Forms
Imports System.Windows.Media.Animation
Imports SlideShowShader.PixelArt.My.Resources

Public Class ShaderMain
    Implements ISlideShowShader

    'Variablendeklarationen

    'Allgemeines
    Public aktuelleSettings As ShaderSettings_PixelArt
    Public Const SLIDESHOWSHADER_PIXELART_FULLPATH As String = SLIDESHOWSHADER_PATH & "PixelArt\"
    Public Const nameShader As String = "PixelArt"

    'Für den Shader
    Dim inputBmp As System.Drawing.Bitmap = Nothing
    Dim outputBmp As System.Drawing.Bitmap = Nothing
    Dim cellSize As Single = 0.0F
    Dim levels As Integer = 0

    Structure ShaderSettings_PixelArt
        Public modus As String
        'Die Potenz zur Basis 2 wird gespeichert
        Public raster As Integer
        Public rasterZufall As Boolean
        'Der Faktor pro Kanal wird gespeichert
        Public farbraum As Integer
        Public farbraumZufall As Boolean
    End Structure

    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return nameShader
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Die 80er Jahre haben angerufen. Sie wollen ihre Pixel zurück."
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

        'Zufallssettings & Rastergröße auflösen
        If aktuelleSettings.rasterZufall Then
            aktuelleSettings.raster = rnd.Next(6) + 1
        End If

        If aktuelleSettings.farbraumZufall Then
            aktuelleSettings.farbraum = rnd.Next(7) + 2
        End If

        cellSize = CInt(Math.Pow(2, aktuelleSettings.raster))
        levels = aktuelleSettings.farbraum
        inputBmp = CType(baseImage, System.Drawing.Bitmap)

        outputBmp = WpfShaderRunner.ApplyPixelArtEffect(inputBmp, cellSize, levels)

        Return CType(outputBmp, Image)
    End Function

    'Dialog & Optionen
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
        defaults("Raster") = "4"
        defaults("RasterZufall") = "False"
        'Der Faktor pro Kanal wird gespeichert - d.h. 6 x 6 x 6 = 216 Farben
        defaults("Farbraum") = "6"
        defaults("FarbraumZufall") = "False"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Holt die Settings aus der Registry (oder aus Default-Werten) und legt sie in aktuelleSettings ab.

        Dim defaults As Dictionary(Of String, String) = GetShaderDefaultSettings()

        aktuelleSettings.modus = ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Modus", defaults)
        'Die Potenz zur Basis 2 wird gespeichert!
        aktuelleSettings.raster = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Raster", defaults))
        aktuelleSettings.rasterZufall = CBool(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "RasterZufall", defaults))
        'Der Faktor pro Kanal wird gespeichert
        aktuelleSettings.farbraum = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Farbraum", defaults))
        aktuelleSettings.farbraumZufall = CBool(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "FarbraumZufall", defaults))

    End Sub

End Class
