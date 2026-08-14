Imports System.Drawing.Imaging
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ShaderMain
    Implements ISlideShowShader

    'Variablendeklaration
#Region "Variablendeklartion"
    'Internes und Verwaltung
    Public Const SLIDESHOWSHADER_INVERTIEREN_FULLPATH As String = SLIDESHOWSHADER_PATH & "Invertieren\"
    Public Const nameShader As String = "Invertieren"

    Private aktuelleSettings As ShaderSettings_Invertieren

    Private wurdeBereinigt As Boolean
    Private ReadOnly rnd As New Random()

    Structure ShaderSettings_Invertieren
        Public Property Modus As String
    End Structure
#End Region

    'Eigenschaften
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return nameShader
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Invertiert die Farben von Bildern"
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    'Start
    Public Function RunShader(
    baseImage As Image,
    Optional imagePath As String = "",
    Optional clientSize As Size = Nothing) As Image _
    Implements ISlideShowShader.RunShader
        'Invertiert das übergebene Bild entsprechend den aktuellen Einstellungen.

        Dim effektiverModus As String

        Dim ergebnisBitmap As Bitmap
        Dim zwischenBitmap As Bitmap

        Dim inversAttributes As ImageAttributes
        Dim greyscaleAttributes As ImageAttributes

        Dim inversMatrix As ColorMatrix
        Dim greyscaleMatrix As ColorMatrix

        Dim quellBild As Image

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ShaderMain))

        End If

        If baseImage Is Nothing Then

            Throw New ArgumentNullException(NameOf(baseImage))

        End If

        zwischenBitmap = Nothing
        inversAttributes = Nothing
        greyscaleAttributes = Nothing

        ReadShaderSettingsFromRegistryOrDefaults()

        effektiverModus = aktuelleSettings.Modus

        If effektiverModus = "Zufall" Then

            effektiverModus = If(rnd.Next(2) = 0, "Farbe", "Weiss-Schwarz")

        End If

        inversMatrix =
        New ColorMatrix(
            New Single()() {
                New Single() {-1, 0, 0, 0, 0},
                New Single() {0, -1, 0, 0, 0},
                New Single() {0, 0, -1, 0, 0},
                New Single() {0, 0, 0, 1, 0},
                New Single() {1, 1, 1, 0, 1}
            })

        greyscaleMatrix =
        New ColorMatrix(
            New Single()() {
                New Single() {0.299F, 0.299F, 0.299F, 0, 0},
                New Single() {0.587F, 0.587F, 0.587F, 0, 0},
                New Single() {0.114F, 0.114F, 0.114F, 0, 0},
                New Single() {0, 0, 0, 1, 0},
                New Single() {0, 0, 0, 0, 1}
            })

        ergebnisBitmap = New Bitmap(baseImage.Width, baseImage.Height)

        Try

            quellBild = baseImage

            If effektiverModus = "Weiss-Schwarz" Then

                zwischenBitmap = New Bitmap(baseImage.Width, baseImage.Height)

                greyscaleAttributes = New ImageAttributes()

                greyscaleAttributes.SetColorMatrix(greyscaleMatrix)

                Using graphics As Graphics = Graphics.FromImage(zwischenBitmap)

                    graphics.DrawImage(
                        baseImage,
                        New Rectangle(
                            0,
                            0,
                            zwischenBitmap.Width,
                            zwischenBitmap.Height),
                        0,
                        0,
                        baseImage.Width,
                        baseImage.Height,
                        GraphicsUnit.Pixel,
                        greyscaleAttributes)

                End Using

                quellBild = zwischenBitmap

            End If

            inversAttributes = New ImageAttributes()
            inversAttributes.SetColorMatrix(inversMatrix)

            Using graphics As Graphics = Graphics.FromImage(ergebnisBitmap)

                graphics.DrawImage(
                    quellBild,
                    New Rectangle(
                        0,
                        0,
                        ergebnisBitmap.Width,
                        ergebnisBitmap.Height),
                    0,
                    0,
                    quellBild.Width,
                    quellBild.Height,
                    GraphicsUnit.Pixel,
                    inversAttributes)

            End Using

            Return ergebnisBitmap

        Catch

            ergebnisBitmap.Dispose()
            Throw

        Finally

            If inversAttributes IsNot Nothing Then

                inversAttributes.Dispose()

            End If

            If greyscaleAttributes IsNot Nothing Then

                greyscaleAttributes.Dispose()

            End If

            If zwischenBitmap IsNot Nothing Then

                zwischenBitmap.Dispose()

            End If

        End Try

    End Function

    'Optionsdialog
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Liest die aktuelleSettings ein, speichert sie in der SettingsInbox und liefert dann den Optionsdialog

        ReadShaderSettingsFromRegistryOrDefaults()
        StoreSettings(nameShader, aktuelleSettings)

        Return New ucOptionsShader()

    End Function

    'Private Funktionen

    'Settings & Defaults
    Friend Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Werte des Shaders

        Dim defaults As New Dictionary(Of String, String)

        defaults("Modus") = "Farbe"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Holt die Settings aus der Registry (oder aus Default-Werten) und legt sie in aktuelleSettings ab.

        Dim defaults As Dictionary(Of String, String) = GetShaderDefaultSettings()

        Select Case ReadFromRegOrDefaults(SLIDESHOWSHADER_INVERTIEREN_FULLPATH & "Modus", defaults)
            Case "Farbe"
                aktuelleSettings.Modus = "Farbe"
            Case "Weiss-Schwarz"
                aktuelleSettings.Modus = "Weiss-Schwarz"
            Case "Zufall"
                aktuelleSettings.Modus = "Zufall"
        End Select

    End Sub

    'Bereinigen udn Dispose
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
