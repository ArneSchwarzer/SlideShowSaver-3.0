Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ShaderMain
    Implements ISlideShowShader

#Region "Variablendeklaration und Strukturen"

    'Allgemeines
    Public Const SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH As String = SLIDESHOWSHADER_PATH & "Tönen und Färben\"
    Public Const nameShader As String = "Tönen und Färben"

    Private aktuelleSettings As ShaderSettings_ToenenFaerben

    Private ReadOnly rnd As New Random()

    Private wurdeBereinigt As Boolean

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

#Region "Eigenschaften"

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

#End Region

#Region "Shader-Ausführung"

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "",
                              Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Tönt oder färbt das Quellbild entsprechend den aktuellen Einstellungen.

        Dim resultImage As Bitmap

        Dim effektiveFarbe As Color
        Dim effektiverModus As ShaderModus

        Dim intensitaet As Integer
        Dim overlayColor As Color

        Dim grayImage As Image
        Dim graphics As Graphics

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ShaderMain))

        End If

        If baseImage Is Nothing Then

            Throw New ArgumentNullException(NameOf(baseImage))

        End If

        ReadShaderSettingsFromRegistryOrDefaults()

        StoreSettings(nameShader, aktuelleSettings)

        effektiveFarbe = aktuelleSettings.Farbton
        effektiverModus = aktuelleSettings.Modus

        If aktuelleSettings.Zufallsfarbe Then

            effektiveFarbe = SetzeZufallsFarbe()

        End If

        If effektiverModus = ShaderModus.Zufaellig Then

            effektiverModus = If(rnd.Next(2) = 0, ShaderModus.Toenen, ShaderModus.Faerben)

        End If

        intensitaet = CInt(255 * (aktuelleSettings.Intensitaet / 100.0))
        intensitaet = Math.Max(0, Math.Min(255, intensitaet))

        overlayColor =
            Color.FromArgb(
                intensitaet,
                effektiveFarbe.R,
                effektiveFarbe.G,
                effektiveFarbe.B)

        resultImage =
            New Bitmap(
                baseImage.Width,
                baseImage.Height,
                Imaging.PixelFormat.Format32bppArgb)

        grayImage = Nothing

        graphics = Nothing

        Try

            graphics = Graphics.FromImage(resultImage)

            If effektiverModus = ShaderModus.Faerben Then

                grayImage = ConvertToGrayscale(baseImage)
                graphics.DrawImage(grayImage, New Rectangle(0, 0, resultImage.Width, resultImage.Height))

            Else

                graphics.DrawImage(baseImage, New Rectangle(0, 0, resultImage.Width, resultImage.Height))

            End If

            Using brush As New SolidBrush(overlayColor)

                graphics.FillRectangle(brush, 0, 0, resultImage.Width, resultImage.Height)

            End Using

            Return resultImage

        Catch

            resultImage.Dispose()
            Throw

        Finally

            If graphics IsNot Nothing Then

                graphics.Dispose()
                graphics = Nothing

            End If

            If grayImage IsNot Nothing Then

                grayImage.Dispose()
                grayImage = Nothing

            End If

        End Try

    End Function

#End Region

#Region "Dialog und Optionen"

    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Liest die aktuellen Settings ein und stellt sie dem
        'Options-Control über die SettingsInbox bereit.

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ShaderMain))

        End If

        ReadShaderSettingsFromRegistryOrDefaults()

        StoreSettings(nameShader, aktuelleSettings)

        Return New ucOptionsShader()

    End Function

#End Region

#Region "Settings und Defaultwerte"

    Friend Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Werte des Shaders.

        Dim defaults As New Dictionary(Of String, String)

        defaults("Farbton") = "112, 66, 20, 255"
        defaults("Zufallsfarbe") = "False"
        defaults("Intensität") = "12"
        defaults("Modus") = "Tönen"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Liest die aktuellen Shadereinstellungen aus der Registry
        'oder verwendet die definierten Defaultwerte.

        Dim defaults As Dictionary(Of String, String)
        Dim modusString As String

        defaults = GetShaderDefaultSettings()

        aktuelleSettings.Farbton = StringToColor(ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH &
                                                                       "Farbton", defaults))
        aktuelleSettings.Zufallsfarbe = CBool(ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH &
                                                                    "Zufallsfarbe", defaults))
        aktuelleSettings.Intensitaet = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH &
                                                                  "Intensität", defaults))

        modusString = ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "Modus", defaults)

        Select Case modusString

            Case "Tönen"

                aktuelleSettings.Modus = ShaderModus.Toenen

            Case "Färben"

                aktuelleSettings.Modus = ShaderModus.Faerben

            Case "Zufall"

                aktuelleSettings.Modus = ShaderModus.Zufaellig

            Case Else

                aktuelleSettings.Modus = ShaderModus.Toenen

        End Select

    End Sub

#End Region

#Region "Farben und Konvertierungen"

    Private Function ConvertToGrayscale(src As Image) As Image
        'Wandelt das Quellbild in ein Graustufenbild um.

        Dim grayBmp As Bitmap

        Dim colorMatrix As Imaging.ColorMatrix
        Dim attributes As Imaging.ImageAttributes

        grayBmp = New Bitmap(src.Width, src.Height, Imaging.PixelFormat.Format32bppArgb)

        attributes = Nothing

        colorMatrix =
            New Imaging.ColorMatrix(
                New Single()() {
                    New Single() {0.299F, 0.299F, 0.299F, 0, 0},
                    New Single() {0.587F, 0.587F, 0.587F, 0, 0},
                    New Single() {0.114F, 0.114F, 0.114F, 0, 0},
                    New Single() {0, 0, 0, 1, 0},
                    New Single() {0, 0, 0, 0, 1}
                })

        Try

            attributes = New Imaging.ImageAttributes()
            attributes.SetColorMatrix(colorMatrix)

            Using graphics As Graphics = Graphics.FromImage(grayBmp)

                graphics.DrawImage(
                    src,
                    New Rectangle(
                        0,
                        0,
                        grayBmp.Width,
                        grayBmp.Height),
                    0,
                    0,
                    src.Width,
                    src.Height,
                    GraphicsUnit.Pixel,
                    attributes)

            End Using

            Return grayBmp

        Catch

            grayBmp.Dispose()
            Throw

        Finally

            If attributes IsNot Nothing Then

                attributes.Dispose()
                attributes = Nothing

            End If

        End Try

    End Function

    Private Function SetzeZufallsFarbe() As Color
        'Sucht eine zufällige benannte Farbe aus.
        'Systemfarben werden ausgeschlossen.

        Dim knownColors As Array
        Dim echteFarben As List(Of KnownColor)

        Dim colorName As KnownColor
        Dim zufallsFarbe As Color

        knownColors = [Enum].GetValues(GetType(KnownColor))

        echteFarben =
            knownColors.
            Cast(Of KnownColor)().
            Where(
                Function(kc)

                    Return Not Color.FromKnownColor(
                        kc).
                    IsSystemColor

                End Function).
            ToList()

        colorName = echteFarben(rnd.Next(echteFarben.Count))

        zufallsFarbe = Color.FromKnownColor(colorName)

        Return zufallsFarbe

    End Function

#End Region

#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
        'Markiert die Shaderinstanz als endgültig freigegeben.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class