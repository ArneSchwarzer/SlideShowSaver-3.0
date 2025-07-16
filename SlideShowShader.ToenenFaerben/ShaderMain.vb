Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SettingsHandling

Public Class ShaderMain
    Implements ISlideShowShader

#Region "Variablendeklaration"
    'Variablendeklaration
    Public Shared SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH = SLIDESHOWSHADER_PATH & "Tönen und Färben\"
    Private aktuelleSettings As New ShaderSettings_ToenenFaerben
    Public Shared nameShader As String = "Tönen und Färben"

    'Settings-Struktur
    Public Structure ShaderSettings_ToenenFaerben
        Public Property Farbton As Color
        Public Property Zufallsfarbe As Boolean
        Public Property Intensitaet As Integer
        Public Property Modus As ShaderModus
    End Structure

    Public Enum ShaderModus
        Toenen = 0
        Faerben = 1
        Zufaellig = 2
    End Enum
#End Region

    'Eigenschaften
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return nameShader
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Legt eine transparente Farbe über das Bild (Tönen oder Färben)."
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    'Shader ausführen
    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Färbt oder tönt das Bild

        Dim bmp As New Bitmap(baseImage.Width, baseImage.Height)
        Dim rnd As New Random

        'Settings einlesen und in SettingsInbox speichern
        ReadShaderSettingsFromRegistryOrDefaults()
        StoreSettings(nameShader, aktuelleSettings)

        'Farbton setzen (falls Zufallsfarbe = False ist er bereits korrekt gesetzt)
        If aktuelleSettings.Zufallsfarbe Then
            aktuelleSettings.Farbton = SetzeZufallsFarbe()
        End If

        'Falls aktueller ShaderModus = Zufällig, dann den tatsächlichen Modus wählen
        If aktuelleSettings.Modus = ShaderModus.Zufaellig Then
            aktuelleSettings.Modus = If(rnd.Next(2) = 0, ShaderModus.Toenen, ShaderModus.Faerben)
        End If

        'Für Modus "Färben" das Bild erst in Graustufen wandeln
        Using g As Graphics = Graphics.FromImage(bmp)
            If aktuelleSettings.Modus = ShaderModus.Faerben Then
                ' Bild in Graustufen umwandeln
                Using grayImage As Image = ConvertToGrayscale(baseImage)
                    g.DrawImage(grayImage, New Rectangle(0, 0, bmp.Width, bmp.Height))
                End Using
            Else
                g.DrawImage(baseImage, New Rectangle(0, 0, bmp.Width, bmp.Height))
            End If

            ' Farbübergabe vorbereiten
            Dim farbe As Color = aktuelleSettings.Farbton
            Dim intensitaet As Integer
            Dim overlayColor As Color

            intensitaet = CInt(255 * (aktuelleSettings.Intensitaet / 100))
            overlayColor = Color.FromArgb(intensitaet, farbe.R, farbe.G, farbe.B)

            Using brush As New SolidBrush(overlayColor)
                g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height)
            End Using

        End Using

        Return bmp

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
    Public Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Werte des Shaders

        Dim defaultShaderSettings_ToenenFaerben As New Dictionary(Of String, String)

        defaultShaderSettings_ToenenFaerben("Farbton") = "112, 66, 20, 255"
        defaultShaderSettings_ToenenFaerben("Zufallsfarbe") = "False"
        defaultShaderSettings_ToenenFaerben("Intensität") = "12"
        defaultShaderSettings_ToenenFaerben("Modus") = "Tönen"

        Return defaultShaderSettings_ToenenFaerben

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Holt die Settings aus der Registry (oder aus Default-Werten) und legt sie in aktuelleSettings ab.

        Dim defaults As Dictionary(Of String, String) = GetShaderDefaultSettings()

        aktuelleSettings.Farbton = StringToColor(ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Farbton", defaults))
        aktuelleSettings.Zufallsfarbe = CBool(ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Zufallsfarbe", defaults))
        aktuelleSettings.Intensitaet = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Intensität", defaults))
        Select Case ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Modus", defaults)
            Case "Tönen"
                aktuelleSettings.Modus = ShaderModus.Toenen
            Case "Färben"
                aktuelleSettings.Modus = ShaderModus.Faerben
            Case "Zufall"
                aktuelleSettings.Modus = ShaderModus.Zufaellig
        End Select

    End Sub

    Private Function ConvertToGrayscale(src As Image) As Image
        'Wandelt das Bild in ein Graustufenbild

        Dim grayBmp As New Bitmap(src.Width, src.Height)
        Using g As Graphics = Graphics.FromImage(grayBmp)
            Dim cm As New Imaging.ColorMatrix(New Single()() {
                New Single() {0.299, 0.299, 0.299, 0, 0},
                New Single() {0.587, 0.587, 0.587, 0, 0},
                New Single() {0.114, 0.114, 0.114, 0, 0},
                New Single() {0, 0, 0, 1, 0},
                New Single() {0, 0, 0, 0, 1}})
            Dim ia As New Imaging.ImageAttributes()
            ia.SetColorMatrix(cm)
            g.DrawImage(src, New Rectangle(0, 0, grayBmp.Width, grayBmp.Height), 0, 0, src.Width, src.Height, GraphicsUnit.Pixel, ia)
        End Using
        Return grayBmp
    End Function

    Private Function SetzeZufallsFarbe() As Color
        'Sucht eine zufällige Farbe aus der Liste der benannten Farben (Systemfarben werden ignoriert)

        Dim zufallsFarbe As Color
        Dim rnd As New Random()
        Dim knownColors = [Enum].GetValues(GetType(KnownColor))
        Dim echteFarben = knownColors.Cast(Of KnownColor)().
                Where(Function(kc) Not Color.FromKnownColor(kc).IsSystemColor).ToList()
        Dim colorName = echteFarben(rnd.Next(echteFarben.Count))

        zufallsFarbe = Color.FromKnownColor(colorName)

        Return zufallsFarbe

    End Function

End Class
