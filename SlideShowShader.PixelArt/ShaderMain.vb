Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ShaderMain
    Implements ISlideShowShader

#Region "Variablendeklaration und Strukturen"

    'Allgemeines
    Public Const SLIDESHOWSHADER_PIXELART_FULLPATH As String = SLIDESHOWSHADER_PATH & "PixelArt\"
    Public Const nameShader As String = "PixelArt"

    Private aktuelleSettings As ShaderSettings_PixelArt

    Private ReadOnly rnd As New Random()

    Private shaderRunner As WpfShaderRunner

    Private wurdeBereinigt As Boolean

    Public Structure ShaderSettings_PixelArt

        'Die Potenz zur Basis 2 wird gespeichert.
        Public raster As Integer
        Public rasterZufall As Boolean

        'Die Anzahl der Stufen pro Farbkanal wird gespeichert.
        Public farbraum As Integer
        Public farbraumZufall As Boolean

    End Structure

#End Region

#Region "Eigenschaften"

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

#End Region

#Region "Shader-Ausführung"

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "",
                              Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Wendet den PixelArt-HLSL-Shader auf das Quellbild an.

        Dim effektivesRaster As Integer
        Dim effektiverFarbraum As Integer

        Dim cellSize As Single
        Dim levels As Integer

        Dim inputBmp As Bitmap
        Dim inputBmpIstEigeneKopie As Boolean

        Dim outputBmp As Bitmap

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ShaderMain))

        End If

        If baseImage Is Nothing Then

            Throw New ArgumentNullException(NameOf(baseImage))

        End If

        ReadShaderSettingsFromRegistryOrDefaults()

        StoreSettings(nameShader, aktuelleSettings)

        effektivesRaster = aktuelleSettings.raster
        effektiverFarbraum = aktuelleSettings.farbraum

        If aktuelleSettings.rasterZufall Then

            effektivesRaster = rnd.Next(1, 7)

        End If

        If aktuelleSettings.farbraumZufall Then

            effektiverFarbraum = rnd.Next(2, 9)

        End If

        cellSize = CSng(Math.Pow(2, effektivesRaster))
        levels = effektiverFarbraum

        inputBmp = TryCast(baseImage, Bitmap)

        inputBmpIstEigeneKopie = False

        If inputBmp Is Nothing Then

            inputBmp = New Bitmap(baseImage)

            inputBmpIstEigeneKopie = True

        End If

        Try

            If shaderRunner Is Nothing Then

                shaderRunner = New WpfShaderRunner()

            End If

            outputBmp = shaderRunner.ApplyPixelArtEffect(inputBmp, cellSize, levels)

            If outputBmp Is Nothing Then

                Throw New InvalidOperationException("Der PixelArt-Shader konnte kein Ergebnisbild erzeugen.")

            End If

            Return outputBmp

        Finally

            If inputBmpIstEigeneKopie AndAlso inputBmp IsNot Nothing Then

                inputBmp.Dispose()
                inputBmp = Nothing

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
        'Liefert die Default-Werte des PixelArt-Shaders.

        Dim defaults As New Dictionary(Of String, String)

        'Die Potenz zur Basis 2 wird gespeichert.
        '2^4 = 16 Pixel.
        defaults("Raster") = "4"
        defaults("RasterZufall") = "False"

        'Anzahl der Stufen pro Farbkanal.
        '6 × 6 × 6 = 216 Farben.
        defaults("Farbraum") = "6"
        defaults("FarbraumZufall") = "False"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Liest die aktuellen Shadereinstellungen aus der Registry
        'oder verwendet die definierten Defaultwerte.

        Dim defaults As Dictionary(Of String, String)

        defaults = GetShaderDefaultSettings()

        aktuelleSettings.raster = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Raster",
                                                             defaults))

        aktuelleSettings.rasterZufall = CBool(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH &
                                                                    "RasterZufall", defaults))

        aktuelleSettings.farbraum = CInt(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH & "Farbraum",
                                                               defaults))

        aktuelleSettings.farbraumZufall = CBool(ReadFromRegOrDefaults(SLIDESHOWSHADER_PIXELART_FULLPATH &
                                                                      "FarbraumZufall", defaults))

    End Sub

#End Region

#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
        'Gibt sämtliche vom PixelArt-Shader gehaltenen
        'Ressourcen endgültig frei.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        If shaderRunner IsNot Nothing Then

            shaderRunner.Dispose()
            shaderRunner = Nothing

        End If

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class