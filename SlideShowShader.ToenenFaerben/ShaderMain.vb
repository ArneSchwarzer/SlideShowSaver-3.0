Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ColorHandling

Public Class ShaderMain
    Implements ISlideShowShader

    ' === Shaderinformationen ===
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return "Tönen und Färben"
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

    ' === Interne Settings ===
    Private aktuelleSettings As New ShaderSettings
    Public Const SLIDESHOWSHADER_FULLPATH As String = SLIDESHOWSHADER_PATH & "Tönen und Färben\"

    ' === Shader ausführen ===
    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Färbt oder tönt das Bild

        Dim bmp As New Bitmap(baseImage.Width, baseImage.Height)
        Dim rnd As New Random

        LiesAktuelleShaderSettingsEin()

        'Farbton setzen (falls Zufallsfarbe = False ist er bereits korrekt gesetzt)
        If aktuelleSettings.Zufallsfarbe Then
            aktuelleSettings.Farbton = SetzeZufallsFarbe()
        End If

        'Falls aktueller Modus = Zufällig, dann den tatsächlichen Modus wählen
        If aktuelleSettings.Modus = "Zufällig" Then
            aktuelleSettings.Modus = If(rnd.Next(1) = 0, "Färben", "Tönen")
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

    ' === Hilfsfunktionen ===
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

    ' === Dialog & Optionen ===
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Liefert den Options-Dialog des Shaders

        Return New ucOptionsShader

    End Function

    Public Function MemorizeShaderSettings(uc As UserControl) As Object Implements ISlideShowShader.MemorizeShaderSettings
        'Dim optionsUC = TryCast(uc, ucOptionsShader)
        'If optionsUC IsNot Nothing Then
        'Return optionsUC.GetSettings()
        'End If
        'Return Nothing
    End Function

    Public Sub ApplyShaderSettings(settings As Object) Implements ISlideShowShader.ApplyShaderSettings
        'If settings IsNot Nothing AndAlso TypeOf settings Is ShaderSettings Then
        '    aktuelleSettings = DirectCast(settings, ShaderSettings)
        'End If
    End Sub

    Public Sub GetShaderSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowShader.GetShaderSettings
        'Dim optionsUC = TryCast(uc, ucOptionsShader)
        'If optionsUC IsNot Nothing AndAlso restoreSettings IsNot Nothing Then
        '    optionsUC.SetzeSettings(DirectCast(restoreSettings, ShaderSettings))
        'End If
    End Sub

    Public Sub GetShaderRegistryOrDefaultSettings(uc As UserControl) Implements ISlideShowShader.GetShaderRegistryOrDefaultSettings
        'Dim optionsUC = TryCast(uc, ucOptionsShader)
        'If optionsUC IsNot Nothing Then
        '    ' Noch kein Registry-Zugriff, daher Defaults verwenden
        '    optionsUC.SetzeSettings(New ShaderSettings With {
        '        .Farbton = Color.Sienna,
        '        .Intensitaet = 64,
        '        .Modus = ShaderModus.Zufaellig
        '    })
        'End If
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowShader.CheckYourSettings
        ' Kein Prüfbedarf für diesen Shader
    End Sub

    Public Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
        Dim defaultShaderSettings As New Dictionary(Of String, String)
        'Liefert die Default-Werte des Moduls

        defaultShaderSettings("Farbton") = "112, 66, 20, 255"
        defaultShaderSettings("Zufallsfarbe") = "False"
        defaultShaderSettings("Intensität") = "35"
        defaultShaderSettings("Modus") = "Tönen"

        Return defaultShaderSettings

    End Function

    Private Sub LiesAktuelleShaderSettingsEin()
        Dim defaults As Dictionary(Of String, String) = GetShaderDefaultSettings()

        aktuelleSettings.Farbton = StringToColor(ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Farbton", defaults))
        aktuelleSettings.Zufallsfarbe = CBool(ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Zufallsfarbe", defaults))
        aktuelleSettings.Intensitaet = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Intensität", defaults))
        aktuelleSettings.Modus = ReadFromRegOrDefaults(SLIDESHOWSHADER_FULLPATH & "Modus", defaults)

    End Sub

    ' === Settings-Struktur ===

    Public Structure ShaderSettings
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

End Class
