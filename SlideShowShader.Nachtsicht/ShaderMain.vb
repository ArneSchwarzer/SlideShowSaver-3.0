Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.TextureHandling
Imports SlideShowTools
Imports System.Text
Imports MetadataExtractor
Imports MetadataExtractor.Formats.Exif
Imports MetadataExtractor.Formats.Iptc
Imports TagLib

Public Class ShaderMain
    Implements ISlideShowShader

    'Variablendeklarationen

    'Texturen
    Private noiseOverlay As Image
    Private scanlineOverlay As Image
    Private vignetteOverlay As Image

    ' === Eigenschaften ===
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return "Nachtsicht"
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Simuliert eine Nachtsichtkamera mit grünem Farbton, Rauschen und Scanlines."
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    ' === Shader-Ausführung ===

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        ' --- Bild- und Zielgrößen ---
        Dim zielSize As Size = If(clientSize = Nothing, baseImage.Size, clientSize)
        Dim bildPfad As String = imagePath
        Dim resultImage As New Bitmap(zielSize.Width, zielSize.Height)
        Dim g As Graphics = Graphics.FromImage(resultImage)
        Dim ia As New Imaging.ImageAttributes()

        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        g.Clear(Color.Black)

        ' --- Nachtsicht-Farbfilter ---
        Dim colorMatrix As New Imaging.ColorMatrix(New Single()() {
        New Single() {0.05F, 0.1F, 0.05F, 0, 0},
        New Single() {0.9F, 1.0F, 0.9F, 0, 0},
        New Single() {0.05F, 0.1F, 0.05F, 0, 0},
        New Single() {0, 0, 0, 1, 0},
        New Single() {0, 0.05F, 0, 0, 1}
        })

        ia.SetColorMatrix(colorMatrix)

        'Overlay Texturen einrichten
        noiseOverlay = ConvertGrayscaleToAlphaMask(My.Resources.noiseTexture_2)
        scanlineOverlay = ConvertGrayscaleToAlphaMask(My.Resources.scanlineTexture_2)
        vignetteOverlay = My.Resources.vignetteTexture

        Debug.WriteLine("Alpha noise: " & noiseOverlay.PixelFormat.ToString())
        Debug.WriteLine("Alpha scanlines: " & scanlineOverlay.PixelFormat.ToString())
        Debug.WriteLine("Alpha vignette: " & vignetteOverlay.PixelFormat.ToString())

        ' --- Basisbild mittig einpassen ---
        Dim zielRect As Rectangle = GraphicsSizeModeHandling.GetDrawRectangle(baseImage.Size, New Rectangle(0, 0, zielSize.Width, zielSize.Height), PictureBoxSizeMode.Zoom)

        g.DrawImage(baseImage, zielRect, 0, 0, baseImage.Width, baseImage.Height, GraphicsUnit.Pixel, ia)


        ' --- Noise-Ebene ---
        If noiseOverlay IsNot Nothing Then
            Using noiseBrush As New TextureBrush(noiseOverlay)
                noiseBrush.WrapMode = Drawing2D.WrapMode.Tile
                g.FillRectangle(noiseBrush, New Rectangle(Point.Empty, zielSize))
            End Using
        End If

        ' --- Scanline-Ebene ---
        If scanlineOverlay IsNot Nothing Then
            Using scanBrush As New TextureBrush(scanlineOverlay)
                scanBrush.WrapMode = Drawing2D.WrapMode.Tile
                g.FillRectangle(scanBrush, New Rectangle(Point.Empty, zielSize))
            End Using
        End If

        ' --- Metadaten via TagLib ---
        Dim tagLibFile As TagLib.Jpeg.File
        Dim directories = ImageMetadataReader.ReadMetadata(bildPfad)
        Dim iptc = directories.OfType(Of IptcDirectory)().FirstOrDefault()

        'ToDo - Gegen fehlerhafte Bilder absichern
        tagLibFile = TagLib.File.Create(bildPfad)

        Dim aktenzeichen As String = Path.GetFileNameWithoutExtension(bildPfad)
        Dim datum As String = System.IO.File.GetCreationTime(bildPfad).ToString("dd.MM.yyyy")
        Dim dateipfad As String = bildPfad
        Dim geoPosition As String
        Dim keywords = iptc.GetStringArray(IptcDirectory.TagKeywords)
        Dim schlagworte As String
        Dim geheimstufe As String = "VS-Nur für den Dienstgebrauch" ' kann sich dynamisch ändern
        Dim geheimStufeFarbe As Brush = Brushes.LimeGreen

        If tagLibFile.ImageTag.Exif.GPSIFD.ToString <> "" Then
            geoPosition = tagLibFile.ImageTag.Exif.GPSIFD.ToString
        Else
            geoPosition = "Unbekannt"
        End If

        schlagworte = If(keywords IsNot Nothing, String.Join(" | ", keywords), "")

        If schlagworte.Contains("18+") Then
            geheimstufe = "Streng Geheim"
            geheimStufeFarbe = Brushes.Orange
        ElseIf schlagworte.Contains("Akt") Then
            geheimstufe = "Geheim"
        ElseIf schlagworte.Contains("Lingerie") Then
            geheimstufe = "VS-Vertraulich"
        End If

        ' --- Text-Overlay-Stil ---
        Dim overlayFont As New Font("Lucida Console", 12, FontStyle.Bold)
        Dim geheimFont As New Font("Lucida Console", 18, FontStyle.Bold)
        Dim overlayBrush As Brush = Brushes.LimeGreen
        Dim backgroundBrush As New SolidBrush(Color.FromArgb(128, Color.Black))


        ' Positionen berechnen
        DrawText(g, "Aktenzeichen: " & aktenzeichen, overlayFont, overlayBrush, 10, 10)
        DrawText(g, "Datum: " & datum, overlayFont, overlayBrush, 10, 40)
        DrawText(g, "Geo-Position: " & geoPosition, overlayFont, overlayBrush, resultImage.Width - 500, 10)
        DrawText(g, "Schlagworte: " & schlagworte, overlayFont, overlayBrush, resultImage.Width - 500, 40)
        DrawText(g, geheimstufe, geheimFont, geheimStufeFarbe, (resultImage.Width - 300) \ 2, 10)
        DrawText(g, "Bibliothekarischer Hinweis:" & vbCrLf & "Ablageort: " & dateipfad, overlayFont, overlayBrush, 10, resultImage.Height - 80)

        ' --- Noise-Ebene ---
        If noiseOverlay IsNot Nothing Then
            Using noiseBrush As New TextureBrush(noiseOverlay)
                noiseBrush.WrapMode = Drawing2D.WrapMode.Tile
                g.FillRectangle(noiseBrush, New Rectangle(Point.Empty, zielSize))
            End Using
        End If

        ' --- Scanline-Ebene ---
        If scanlineOverlay IsNot Nothing Then
            Using scanBrush As New TextureBrush(scanlineOverlay)
                scanBrush.WrapMode = Drawing2D.WrapMode.Tile
                g.FillRectangle(scanBrush, New Rectangle(Point.Empty, zielSize))
            End Using
        End If


        ' --- Vignette ---
        If vignetteOverlay IsNot Nothing Then
            g.DrawImage(vignetteOverlay, New Rectangle(Point.Empty, zielSize))
        End If


        ' --- Fertig ---
        g.Dispose()
        Return resultImage

    End Function


    ' Hilfsfunktion
    Sub DrawText(gfx As Graphics, text As String, font As Font, brush As Brush, x As Integer, y As Integer)
        Dim size = gfx.MeasureString(text, font)
        Dim backgroundBrush As New SolidBrush(Color.FromArgb(128, 0, 0, 0))

        gfx.FillRectangle(backgroundBrush, x - 4, y - 2, size.Width + 8, size.Height + 4)
        gfx.DrawString(text, font, brush, x, y)
    End Sub

    ' === Optionen/Dialog (nicht verwendet) ===
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Wird nicht verwendet
    End Function

    Public Function MemorizeShaderSettings(uc As UserControl) As Object Implements ISlideShowShader.MemorizeShaderSettings
        'Wird nicht verwendet
        Return Nothing
    End Function

    Public Sub ApplyShaderSettings(settings As Object) Implements ISlideShowShader.ApplyShaderSettings
        ' Keine Einstellungen
    End Sub

    Public Sub GetShaderSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowShader.GetShaderSettings
        ' Keine Einstellungen
    End Sub

    Public Sub GetShaderRegistryOrDefaultSettings(uc As UserControl) Implements ISlideShowShader.GetShaderRegistryOrDefaultSettings
        ' Keine Einstellungen
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowShader.CheckYourSettings
        ' Keine Einstellungen
    End Sub
End Class
