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
    Private Shared aktuelleSettings As ShaderSettings_Invertieren

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
    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        Dim bmp As New Bitmap(baseImage.Width, baseImage.Height)
        Dim attributes As New ImageAttributes()
        Dim rnd As New Random
        Dim rndTest As Integer

        If clientSize = Nothing Then clientSize = baseImage.Size

        'Invertierungsmatrix
        Dim inversMatrix As New ColorMatrix(New Single()() {
        New Single() {-1, 0, 0, 0, 0},
        New Single() {0, -1, 0, 0, 0},
        New Single() {0, 0, -1, 0, 0},
        New Single() {0, 0, 0, 1, 0},
        New Single() {1, 1, 1, 0, 1}})

        ' Farbmatrix zur Umwandlung in Greyscale (nach europäischem Stil 😉)
        Dim greyscaleMatrix As New ColorMatrix(New Single()() {
        New Single() {0.299F, 0.299F, 0.299F, 0, 0},
        New Single() {0.587F, 0.587F, 0.587F, 0, 0},
        New Single() {0.114F, 0.114F, 0.114F, 0, 0},
        New Single() {0, 0, 0, 1, 0},
        New Single() {0, 0, 0, 0, 1}
    })

        ReadShaderSettingsFromRegistryOrDefaults()

        'Wenn Modus Zufall gesetzt ist, dann Modus auswürfeln
        If aktuelleSettings.Modus = "Zufall" Then
            rndTest = rnd.Next(2)

            If rndTest = 0 Then
                aktuelleSettings.Modus = "Farbe"
            Else
                aktuelleSettings.Modus = "Weiss-Schwarz"
            End If

        End If

        'Falls Modus "Weiss-Schwarz", Bild vor dem Invertieren erst in ein Greyscale wandeln
        If aktuelleSettings.Modus = "Weiss-Schwarz" Then

            attributes.SetColorMatrix(greyscaleMatrix)

            Using g As Graphics = Graphics.FromImage(bmp)
                g.DrawImage(baseImage,
                        New Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, baseImage.Width, baseImage.Height,
                        GraphicsUnit.Pixel,
                        attributes)
            End Using

            baseImage = bmp
        End If

        'Invertieren
        attributes.SetColorMatrix(inversMatrix)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.DrawImage(baseImage,
                        New Rectangle(0, 0, bmp.Width, bmp.Height),
                        0, 0, baseImage.Width, baseImage.Height,
                        GraphicsUnit.Pixel,
                        attributes)
        End Using

        'Ergebnis liefern
        Return bmp

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
    Public Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
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

End Class
