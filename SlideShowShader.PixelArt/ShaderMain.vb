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
    Public Const nameShader As String = "PixelArt und Halftone"


    Structure ShaderSettings_PixelArt
        Public modus As String
        'Die Potenz zur Basis 2 wird gespeichert
        Public raster As Integer
        Public rasterZufall As Boolean
        'Der Faktor pro Kanal wird gespeichert
        Public posterise As Integer
        Public gamma As Single
        Public dotsMax As Integer
        Public dotsMin As Integer
        'kleiner Missbrauch von "Color" als CMYK-Datenstruktur: C = B, M = R, Y = G, K = A
        Public dotsWinkel As System.Drawing.Color
        Public papier As Boolean
        Public papierFaktor As Integer
    End Structure

    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return nameShader
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Stellt das Bild als PixelArt oder als 4-Farb bzw. SW Druck dar"
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Der eigentliche Shader

        'Aktuelle Settings abholen und in SettingsInbox speichern
        ReadShaderSettingsFromRegistryOrDefaults()
        StoreSettings(nameShader, aktuelleSettings)

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

        defaults("Modus") = "Zufall"
        'Die Potenz zur Basis 2 wird gespeichert - d.h. 2^4 = 16 px
        defaults("Raster") = "4"
        defaults("RasterZufall") = "False"
        'Der Faktor pro Kanal wird gespeichert - d.h. 6 x 6 x 6 = 216 Farben
        defaults("Posterise") = "6"
        defaults("Gamma") = "1,6"
        defaults("DotsMax") = "45"
        defaults("DotsMin") = "6"
        'kleiner Missbrauch von "Color" als CMYK-Datenstruktur: C = B, M = R, Y = G, K = A
        defaults("DotsWinkel") = "75,0,15,45"
        defaults("Papier") = "True"
        defaults("PapierFaktor") = "33"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Holt die Settings aus der Registry (oder aus Default-Werten) und legt sie in aktuelleSettings ab.

        Dim defaults As Dictionary(Of String, String) = GetShaderDefaultSettings()

        aktuelleSettings.modus = ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Modus", defaults)
        'Die Potenz zur Basis 2 wird gespeichert!
        aktuelleSettings.raster = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Raster", defaults))
        If ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "RasterZufall", defaults) = "True" Then
            aktuelleSettings.rasterZufall = True
        Else
            aktuelleSettings.rasterZufall = False
        End If
        'Der Faktor pro Kanal wird gespeichert
        aktuelleSettings.posterise = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Posterise", defaults))

        'Gleitkommazahlen sind doof...
        Dim sGamma As String = ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Gamma", defaults)
        Dim g As Single
        If Not Single.TryParse(sGamma, Globalization.NumberStyles.Float, Globalization.CultureInfo.CurrentCulture, g) Then
            Single.TryParse(sGamma, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, g)
        End If
        aktuelleSettings.gamma = g

        aktuelleSettings.dotsMax = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "DotsMax", defaults))
        aktuelleSettings.dotsMin = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "DotsMin", defaults))
        'kleiner Missbrauch von "Color" als CMYK-Datenstruktur: C = B, M = R, Y = G, K = A
        aktuelleSettings.dotsWinkel = StringToColor(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "DotsWinkel", defaults))
        If ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Papier", defaults) = "True" Then
            aktuelleSettings.papier = True
        Else
            aktuelleSettings.papier = False
        End If
        aktuelleSettings.papierFaktor = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "PapierFaktor", defaults))

    End Sub
End Class
