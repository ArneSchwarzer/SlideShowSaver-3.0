Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging.LogHandling
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.WPFHandling
Imports System.Windows.Media.Imaging
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Threading
Imports SlideShowTools

Public Class ShaderMain
    Implements ISlideShowShader

#Region "Variablendeklaration"
    'Variablendeklaration
    Public Shared SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH = SLIDESHOWSHADER_PATH & "Tönen und Färben\"
    Private aktuelleSettings As New ShaderSettings_ToenenFaerben
    Public Shared nameShader As String = "Tönen und Färben"

    'Animation
    Private FPS As Integer = 120
    Private startTime As DateTime
    Private dauerInMS As Integer
    Private Shared alteFarbe As System.Drawing.Color
    Private Shared neueFarbe As System.Drawing.Color
    Private Shared intensitaet As Integer

    'Rendering
    Private Shared drawAction As Action(Of DrawingContext)
    Private Shared frameTimer As DispatcherTimer
    Private Shared grundBild As Image
    Private Shared renderGrundBild As BitmapSource
    Private Shared renderRectangle As Windows.Rect
    Private Shared bm As BitmapImage

    'Settings-Struktur
    Public Structure ShaderSettings_ToenenFaerben
        Public Property Farbton As System.Drawing.Color
        Public Property Zufallsfarbe As Boolean
        Public Property Intensitaet As Integer
        Public Property Modus As ShaderModus
        Public Property Animationsmodus As AnimationsModus
        Public Property Geschwindigkeit As Integer
    End Structure

    Private Structure HSL
        Public Hue As Double        ' 0–360
        Public Saturation As Double ' 0–1
        Public Lightness As Double  ' 0–1
    End Structure

    Public Enum ShaderModus
        Toenen = 0
        Faerben = 1
        Zufaellig = 2
    End Enum

    Public Enum AnimationsModus
        Statisch = 0
        Animiert = 1
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

    'Events
    Event ShaderFrameIstFertig(bitmap As RenderTargetBitmap) Implements ISlideShowShader.ShaderFrameIstFertig

    'Shader ausführen
    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As System.Drawing.Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Färbt oder tönt das Bild

        Dim bmp As New Bitmap(baseImage.Width, baseImage.Height)
        Dim rtb As RenderTargetBitmap
        Dim rnd As New Random
        Dim farbe As System.Drawing.Color = aktuelleSettings.Farbton
        Dim overlayColor As System.Drawing.Color

        If clientSize = Nothing Then clientSize = baseImage.Size

        'Wesentliche Variablen in Klassenvariablen überführen
        grundBild = baseImage

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

        'Falls aktueller AnimationsModus = Zufällig, dann den tatsächlichen Modus wählen
        If aktuelleSettings.Animationsmodus = AnimationsModus.Zufaellig Then
            aktuelleSettings.Animationsmodus = If(rnd.Next(2) = 0, AnimationsModus.Statisch, AnimationsModus.Animiert)
        End If

        'Für Modus "Färben" das Bild erst in Graustufen wandeln
        Using g As Graphics = Graphics.FromImage(bmp)
            If aktuelleSettings.Modus = ShaderModus.Faerben Then
                ' Bild in Graustufen umwandeln
                Using grayImage As Image = ConvertToGrayscale(grundBild)
                    g.DrawImage(grayImage, New Rectangle(0, 0, bmp.Width, bmp.Height))
                End Using
                grundBild = ConvertToGrayscale(grundBild)
            Else
                g.DrawImage(grundBild, New Rectangle(0, 0, bmp.Width, bmp.Height))
            End If

            ' Farbübergabe vorbereiten
            intensitaet = CInt(255 * (aktuelleSettings.Intensitaet / 100))
            overlayColor = System.Drawing.Color.FromArgb(intensitaet, farbe.R, farbe.G, farbe.B)

            Using brush As New SolidBrush(overlayColor)
                g.FillRectangle(brush, 0, 0, bmp.Width, bmp.Height)
            End Using

        End Using

        If aktuelleSettings.Animationsmodus = AnimationsModus.Statisch Then
            'Ergebnis sowohl als Event als auch als Bitmap liefert
            bm = ConvertImageToBitmapImage(bmp)
            rtb = ErzeugeGerahmtesBild(bm, PictureBoxSizeMode.Zoom, clientSize)
            RaiseEvent ShaderFrameIstFertig(rtb)

            Return bmp

        Else
            'Animationsloop starten und erstes Bild als Bitmap zurück liefern

            alteFarbe = overlayColor
            neueFarbe = SetzeZufallsFarbe()
            Try
                renderGrundBild = ConvertImageToBitmapImage(bmp)
            Catch ex As Exception
                LogError("Shader T&F - ShaderMain.RunShader: ConverImageToBitmapImage() fehlgeschlagen: " & ex.Message)
            End Try

            renderRectangle = New Windows.Rect(0, 0, clientSize.Width, clientSize.Height)
            renderSize = New Windows.Size(clientSize.Width, clientSize.Height)

            ' Animation starten 
            dauerInMS = 1000 * aktuelleSettings.Geschwindigkeit
            startTime = DateTime.Now

            StartRenderLoop(AddressOf DrawShaderFrame)

            Return bmp

        End If

    End Function

    Public Sub StopShader() Implements ISlideShowShader.StopShader
        'Aufräumen
        StopRenderLoop()
    End Sub

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

        Dim defaultShaderSettings_ToenenFaerben As New Dictionary(Of String, String)

        defaultShaderSettings_ToenenFaerben("Farbton") = "112, 66, 20, 255"
        defaultShaderSettings_ToenenFaerben("Zufallsfarbe") = "False"
        defaultShaderSettings_ToenenFaerben("Intensität") = "12"
        defaultShaderSettings_ToenenFaerben("Modus") = "Tönen"
        defaultShaderSettings_ToenenFaerben("Animationsmodus") = "Animiert"
        defaultShaderSettings_ToenenFaerben("Geschwindigkeit") = "15"

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
        Select Case ReadFromRegOrDefaults(SLIDESHOWSHADER_TOENENFAERBEN_FULLPATH & "AnimationsModus", defaults)
            Case "Statisch"
                aktuelleSettings.Animationsmodus = AnimationsModus.Statisch
            Case "Animiert"
                aktuelleSettings.Animationsmodus = AnimationsModus.Animiert
            Case "Zufall"
                aktuelleSettings.Animationsmodus = AnimationsModus.Zufaellig
        End Select
        aktuelleSettings.Geschwindigkeit = CInt(ReadFromRegOrDefaults("Geschwindigkeit", defaults))

    End Sub

    'Farben & Konversionen
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

    Private Function SetzeZufallsFarbe() As System.Drawing.Color
        'Sucht eine zufällige Farbe aus der Liste der benannten Farben (Systemfarben werden ignoriert)

        Dim zufallsFarbe As System.Drawing.Color
        Dim rnd As New Random()
        Dim knownColors = [Enum].GetValues(GetType(KnownColor))
        Dim echteFarben = knownColors.Cast(Of KnownColor)().
                Where(Function(kc) Not System.Drawing.Color.FromKnownColor(kc).IsSystemColor).ToList()
        Dim colorName = echteFarben(rnd.Next(echteFarben.Count))

        zufallsFarbe = System.Drawing.Color.FromKnownColor(colorName)

        Return zufallsFarbe

    End Function

    Private Function RgbToHsl(color As System.Drawing.Color) As HSL
        Dim r As Double = color.R / 255.0
        Dim g As Double = color.G / 255.0
        Dim b As Double = color.B / 255.0

        Dim max As Double = Math.Max(r, Math.Max(g, b))
        Dim min As Double = Math.Min(r, Math.Min(g, b))
        Dim h, s, l As Double
        h = 0 : s = 0 : l = (max + min) / 2

        If max = min Then
            h = 0 : s = 0 ' Graustufen
        Else
            Dim d As Double = max - min
            s = If(l > 0.5, d / (2.0 - max - min), d / (max + min))

            Select Case max
                Case r : h = (g - b) / d + If(g < b, 6, 0)
                Case g : h = (b - r) / d + 2
                Case b : h = (r - g) / d + 4
            End Select
            h *= 60
        End If

        Return New HSL With {.Hue = h, .Saturation = s, .Lightness = l}
    End Function

    Private Function HslToRgb(hsl As HSL) As System.Drawing.Color
        Dim r, g, b As Double
        Dim h As Double = hsl.Hue / 360.0
        Dim s As Double = hsl.Saturation
        Dim l As Double = hsl.Lightness

        If s = 0 Then
            r = l : g = l : b = l
        Else
            Dim q As Double = If(l < 0.5, l * (1 + s), l + s - l * s)
            Dim p As Double = 2 * l - q
            r = HueToRgb(p, q, h + 1.0 / 3.0)
            g = HueToRgb(p, q, h)
            b = HueToRgb(p, q, h - 1.0 / 3.0)
        End If

        Return System.Drawing.Color.FromArgb(255, CInt(r * 255), CInt(g * 255), CInt(b * 255))
    End Function

    Private Function HueToRgb(p As Double, q As Double, t As Double) As Double
        If t < 0 Then t += 1
        If t > 1 Then t -= 1
        If t < 1.0 / 6.0 Then Return p + (q - p) * 6 * t
        If t < 1.0 / 2.0 Then Return q
        If t < 2.0 / 3.0 Then Return p + (q - p) * (2.0 / 3.0 - t) * 6
        Return p
    End Function

    'Animation
    Public Function InterpolateColorHueRadial(startColor As System.Drawing.Color, endColor As System.Drawing.Color, t As Double) As System.Drawing.Color
        If t <= 0 Then Return startColor
        If t >= 1 Then Return endColor

        Dim hslStart As HSL = RgbToHsl(startColor)
        Dim hslEnd As HSL = RgbToHsl(endColor)

        ' Hue über kürzesten Kreisweg interpolieren
        Dim deltaHue As Double = ((hslEnd.Hue - hslStart.Hue + 540) Mod 360) - 180
        Dim hueInterp As Double = (hslStart.Hue + t * deltaHue + 360) Mod 360

        ' Linear für Sättigung und Helligkeit (optional: easing)
        Dim satInterp As Double = hslStart.Saturation + t * (hslEnd.Saturation - hslStart.Saturation)
        Dim lightInterp As Double = hslStart.Lightness + t * (hslEnd.Lightness - hslStart.Lightness)

        Dim hslResult As New HSL With {
        .Hue = hueInterp,
        .Saturation = satInterp,
        .Lightness = lightInterp
    }

        Return HslToRgb(hslResult)
    End Function

    Private Sub DrawShaderFrame(dc As DrawingContext)

        Dim grundBildIS As ImageSource

        If grundBild Is Nothing Then Exit Sub
        grundBildIS = ConvertImageToImageSource(grundBild)

        ' Fortschritt der Animation berechnen
        Dim elapsed As Double = (DateTime.Now - startTime).TotalMilliseconds
        Dim progress As Double = Math.Min(1.0, elapsed / dauerInMS)

        ' Interpolierte Farbe
        Dim aktuelleFarbe As System.Drawing.Color = InterpolateColorHueRadial(alteFarbe, neueFarbe, progress)

        ' Effekt-Zielrechteck berechnen (nicht auf gesamten Bildschirm!)
        Dim imageSize As System.Drawing.Size = grundBild.Size
        Dim containerRect As New Rectangle(0, 0, CInt(renderSize.Width), CInt(renderSize.Height))
        Dim effektRectangle As Rectangle = GraphicsSizeModeHandling.GetDrawRectangle(imageSize, containerRect, PictureBoxSizeMode.Zoom)
        Dim effektRect As New Rect(effektRectangle.Left, effektRectangle.Top, effektRectangle.Width, effektRectangle.Height)

        ' Overlay erstellen
        Dim overlayBrush As New SolidColorBrush(Windows.Media.Color.FromArgb(intensitaet, aktuelleFarbe.R, aktuelleFarbe.G, aktuelleFarbe.B))
        dc.DrawImage(grundBildIS, effektRect)
        dc.DrawRectangle(overlayBrush, Nothing, effektRect)

        ' Wenn fertig, Farbzyklus neu starten
        If progress >= 1.0 Then
            alteFarbe = neueFarbe
            neueFarbe = SetzeZufallsFarbe()
            startTime = DateTime.Now
        End If
    End Sub


    Public Sub StartRenderLoop(drawActionInput As Action(Of DrawingContext))

        drawAction = drawActionInput

        StopRenderLoop()

        frameTimer = New DispatcherTimer()
        AddHandler frameTimer.Tick, AddressOf OnFrameTick
        frameTimer.Interval = TimeSpan.FromMilliseconds(1000 \ FPS)
        frameTimer.Start()

    End Sub

    Public Sub StopRenderLoop()
        If frameTimer IsNot Nothing Then
            frameTimer.Stop()
            RemoveHandler frameTimer.Tick, AddressOf OnFrameTick
            frameTimer = Nothing
        End If
    End Sub

    Private Sub OnFrameTick(sender As Object, e As EventArgs)
        If drawAction Is Nothing Then Exit Sub

        ' Neues Visual für diesen Frame
        Dim drawingVisual As New DrawingVisual()
        Using dc As DrawingContext = drawingVisual.RenderOpen()
            ' Hintergrund löschen
            dc.DrawRectangle(Windows.Media.Brushes.Black, Nothing, New Rect(0, 0, renderSize.Width, renderSize.Height))

            ' Effekt zeichnen
            drawAction.Invoke(dc)
        End Using

        ' Rendern in RTB
        Dim rtb As New RenderTargetBitmap(CInt(renderSize.Width),
                                          CInt(renderSize.Height),
                                          96, 96,
                                          PixelFormats.Pbgra32)
        rtb.Render(drawingVisual)

        ' Ergebnis-Event feuern
        RaiseEvent ShaderFrameIstFertig(rtb)
    End Sub

End Class
