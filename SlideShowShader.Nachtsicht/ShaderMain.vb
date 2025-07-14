Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.TextureHandling
Imports SlideShowTools.LocationHandling
Imports SlideShowTools
Imports System.Text
Imports MetadataExtractor
Imports MetadataExtractor.Formats.Exif
Imports MetadataExtractor.Formats.Iptc
Imports TagLib
Imports TagLib.IFD.Entries
Imports System.Globalization
Imports MetadataExtractor.Formats

Public Class ShaderMain
    Implements ISlideShowShader

    'Variablendeklarationen

    'Texturen
    Private noiseOverlay As Image
    Private scanlineOverlay As Image
    Private vignetteOverlay As Image

    Private g As Graphics
    Private zielRect As Rectangle

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
        Dim ia As New Imaging.ImageAttributes()

        g = Graphics.FromImage(resultImage)
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
        noiseOverlay = TextureHandling.GenerateNoiseTexture(512, 512, 32)
        scanlineOverlay = TextureHandling.GenerateScanlineTexture(512, 512, 23)
        vignetteOverlay = My.Resources.vignetteTexture

        ' --- Basisbild mittig einpassen ---
        zielRect = GraphicsSizeModeHandling.GetDrawRectangle(baseImage.Size, New Rectangle(0, 0, zielSize.Width, zielSize.Height), PictureBoxSizeMode.Zoom)

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
        Dim aktenzeichen As String = Path.GetFileNameWithoutExtension(bildPfad)
        Dim datum As String = System.IO.File.GetCreationTime(bildPfad).ToString("dd.MM.yyyy")
        Dim dateipfad As String = bildPfad
        Dim geoPosition As String
        Dim locationHelper As LocationHandling
        Dim schlagworte As String
        Dim keywords As New List(Of String)
        Dim geheimstufe As String = "VS-Nur für den Dienstgebrauch" ' kann sich dynamisch ändern
        Dim geheimStufeFarbe As Brush = Brushes.LimeGreen
        Dim kameraCode As String

        If Path.GetExtension(bildPfad) = ".jpg" OrElse Path.GetExtension(bildPfad) = ".jpeg" Then
            Dim tagLibFile As TagLib.Jpeg.File
            Dim directories = ImageMetadataReader.ReadMetadata(bildPfad)
            Dim iptc = directories.OfType(Of IptcDirectory)().FirstOrDefault()

            tagLibFile = TagLib.File.Create(bildPfad)

            'Tags auslesen
            keywords.Clear()
            If tagLibFile.ImageTag.Keywords IsNot Nothing Then
                For Each keyword In tagLibFile.ImageTag.Keywords
                    keywords.Add(keyword)
                Next
            Else
                keywords.Add("-")
            End If

            'Geo-Daten holen. Erst in den Exif-Daten suchen, falls das nichts bringt LocationHelper bemühen.
            If HoleGpsKoordinaten(bildPfad) IsNot Nothing Then
                geoPosition = HoleGpsKoordinaten(bildPfad)
            Else
                locationHelper = New SlideShowTools.LocationHandling()

                geoPosition = locationHelper.DetermineGPSLocation(keywords)

                If String.IsNullOrWhiteSpace(geoPosition) Then
                    geoPosition = "Unbekannt"
                End If
            End If

            If keywords IsNot Nothing Then
                If keywords.Any(Function(k) String.Equals(k.Trim(), "18+", StringComparison.OrdinalIgnoreCase)) Then
                    geheimstufe = "Streng Geheim"
                    geheimStufeFarbe = Brushes.Orange
                ElseIf keywords.Any(Function(k) String.Equals(k.Trim(), "Akt", StringComparison.OrdinalIgnoreCase)) Then
                    geheimstufe = "Geheim"
                ElseIf keywords.Any(Function(k) String.Equals(k.Trim(), "Lingerie", StringComparison.OrdinalIgnoreCase)) Then
                    geheimstufe = "VS-Vertraulich"
                End If

                schlagworte = String.Join(" | ", keywords)
            Else
                schlagworte = ""
            End If
        End If

        kameraCode = GeneriereKameracode()


        ' --- Text-Overlay-Stil ---
        Dim overlayFont As New Font("Lucida Console", 12, FontStyle.Bold)
        Dim geheimFont As New Font("Lucida Console", 18, FontStyle.Bold)
        Dim overlayBrush As Brush = Brushes.LimeGreen
        Dim backgroundBrush As New SolidBrush(Color.FromArgb(128, Color.Black))
        Dim zeilen As List(Of String)
        Dim höheNächsterText As Integer

        'Positionen berechnen
        'Aktenzeichen
        DrawText(g, "Aktenzeichen: ", overlayFont, overlayBrush, 10, 15)
        höheNächsterText = 15
        zeilen = TextKorrektTeilen(g, aktenzeichen, overlayFont, 800)
        For Each zeile In zeilen
            DrawText(g, zeile, overlayFont, overlayBrush, 10 + g.MeasureString("Aktenzeichen: ", overlayFont).Width, höheNächsterText)
            höheNächsterText += g.MeasureString(zeile, overlayFont).Height
        Next

        'Datum
        DrawText(g, "Datum: ", overlayFont, overlayBrush, 10, höheNächsterText)
        DrawText(g, datum, overlayFont, overlayBrush, 10 + g.MeasureString("Aktenzeichen: ", overlayFont).Width, höheNächsterText)

        'Geo-Position
        DrawText(g, "Geo-Position: " & geoPosition, overlayFont, overlayBrush, resultImage.Width - 950, 15)
        höheNächsterText = 15 + g.MeasureString("Geo-Postition: ", overlayFont).Height

        'Tags
        DrawText(g, "Schlagworte: ", overlayFont, overlayBrush, resultImage.Width - 950, höheNächsterText)
        zeilen = TextKorrektTeilen(g, schlagworte, overlayFont, 550)
        For Each zeile In zeilen
            DrawText(g, zeile, overlayFont, overlayBrush, (resultImage.Width - 950) + g.MeasureString("Geo-Position: ", overlayFont).Width, höheNächsterText)
            höheNächsterText += g.MeasureString(zeile, overlayFont).Height
        Next

        'Geheimstufe
        DrawText(g, "Geheimhaltungsstufe", overlayFont, overlayBrush, (resultImage.Width - g.MeasureString("Geheimhaltunsgstufe", overlayFont).Width) \ 2, 60)
        DrawText(g, geheimstufe, geheimFont, geheimStufeFarbe, (resultImage.Width - g.MeasureString(geheimstufe, geheimFont).Width) \ 2, 105)

        'Bibleothekarischer Hinweis
        DrawText(g, "Bibliothekarischer Hinweis:" & vbCrLf & "Ablageort: " & dateipfad, overlayFont, overlayBrush, 10, resultImage.Height - 80)

        'Kamera
        DrawText(g, kameraCode, overlayFont, overlayBrush, (resultImage.Width - g.MeasureString(kameraCode, overlayFont).Width) \ 2, 0)

        'Rahmen zeichnen
        ZeichneRahmen(kameraCode, overlayFont)

        ' --- Vignette ---
        If vignetteOverlay IsNot Nothing Then
            g.DrawImage(vignetteOverlay, New Rectangle(Point.Empty, zielSize))
        End If

        ' --- Fertig ---
        g.Dispose()
        Return resultImage

    End Function

    ' Hilfsfunktion
    Private Sub DrawText(gfx As Graphics, text As String, font As Font, brush As Brush, x As Integer, y As Integer)
        Dim size = gfx.MeasureString(text, font)
        Dim backgroundBrush As New SolidBrush(Color.FromArgb(128, 0, 0, 0))

        gfx.FillRectangle(backgroundBrush, x - 4, y - 2, size.Width + 8, size.Height + 4)
        gfx.DrawString(text, font, brush, x, y)
    End Sub

    Private Function HoleGpsKoordinaten(pfadZurDatei As String) As String
        ' Variablendeklaration
        Dim verzeichnisse As IList(Of MetadataExtractor.Directory)
        Dim gpsDirectory As GpsDirectory
        Dim latLänge As Double
        Dim lonLänge As Double
        Dim latRef As String
        Dim lonRef As String
        Dim gpsString As String

        verzeichnisse = ImageMetadataReader.ReadMetadata(pfadZurDatei)

        gpsDirectory = verzeichnisse.OfType(Of GpsDirectory).FirstOrDefault()

        If gpsDirectory Is Nothing Then
            Return Nothing ' Keine GPS-Daten gefunden
        End If

        ' Koordinaten abrufen
        Dim location As GeoLocation = gpsDirectory.GetGeoLocation()

        If location IsNot Nothing Then
            latLänge = location.Latitude
            lonLänge = location.Longitude
            gpsString = latLänge.ToString("F6", CultureInfo.InvariantCulture) & ", " &
                        lonLänge.ToString("F6", CultureInfo.InvariantCulture)
            Return gpsString
        End If

        Return Nothing

    End Function

    Private Function GeneriereKameracode() As String
        Dim rnd As New Random()
        Dim teil1 As String = ""
        Dim teil2 As String = ""
        Dim teil3 As String = ""

        ' Teil 1: 4 Zeichen, Mischung aus Großbuchstaben und Zahlen
        For i = 1 To 4
            If rnd.NextDouble() < 0.5 Then
                teil1 &= Chr(rnd.Next(48, 58)) ' Ziffer 0–9
            Else
                teil1 &= Chr(rnd.Next(65, 91)) ' A–Z
            End If
        Next

        ' Teil 2 & 3: rein numerisch
        teil2 = rnd.Next(100, 1000).ToString()
        teil3 = rnd.Next(10, 100).ToString("00")

        Return $"Überwachungskamera #{teil1}-{teil2}-{teil3}"

    End Function

    Private Sub ZeichneRahmen(titelText As String, overlayFont As Font)
        Dim rahmenStift As New Pen(Color.LimeGreen, 10)
        Dim titelSize As SizeF = g.MeasureString(titelText, overlayFont)
        Dim lueckeBreite As Integer = CInt(titelSize.Width + 20)
        Dim bildbreite As Integer
        Dim titelbreite As Integer
        Dim bildhöhe As Integer

        bildbreite = Screen.PrimaryScreen.Bounds.Width
        bildhöhe = Screen.PrimaryScreen.Bounds.Height
        titelbreite = titelSize.Width


        ' Links oben bis zur Lücke
        g.DrawLine(rahmenStift, 0, 10, (bildbreite - titelbreite) \ 2, 10)

        ' Rechts oben nach der Lücke
        g.DrawLine(rahmenStift, (bildbreite + titelbreite) \ 2, 10, bildbreite - 1, 10)

        ' Links
        g.DrawLine(rahmenStift, 0, 0, 0, bildHöhe - 1)

        ' Rechts
        g.DrawLine(rahmenStift, bildBreite - 1, 0, bildBreite - 1, bildHöhe - 1)

        ' Unten
        g.DrawLine(rahmenStift, 0, bildHöhe - 1, bildBreite - 1, bildHöhe - 1)

    End Sub

    Public Function TextKorrektTeilen(gfx As Graphics,
                                  zuTrennenderText As String,
                                  zuTrennenderFont As Font,
                                  maxWidth As Integer) As List(Of String)

        ' --- Vordefinierte Trennzeichen ---
        Dim breakCharsBefore As Char() = {"("c, "{"c, "["c}
        Dim breakCharsAfter As Char() = {")"c, "}"c, "]"c, "."c, "-"c, "\"c, "/"c, "|"c}
        Dim breakAndDeleteChars As Char() = {" "c}
        Dim breakCharsCountDependent As Char() = {""""c, "'"c}
        Dim breakCharsCountDependetCountIsOdd As Boolean = True

        ' --- Initialisierung ---
        Dim resultList As New List(Of String)
        Dim cutText As String = ""
        Dim cuttingBlock As String = ""
        Dim restText As String = If(zuTrennenderText, "").Trim()

        If restText = "" OrElse maxWidth <= 0 Then
            resultList.Add(restText)
            Return resultList
        End If

        ' --- Hauptschleife ---
        Do While gfx.MeasureString(restText, zuTrennenderFont).Width > maxWidth
            Dim trennTyp As String = ""
            Dim trennIndex As Integer = -1

            ' Schritt 1: CuttingBlock suchen
            Dim i As Integer = 1
            Do While i <= restText.Length
                cuttingBlock = restText.Substring(0, i)
                If gfx.MeasureString(cuttingBlock, zuTrennenderFont).Width > maxWidth Then
                    Exit Do
                End If
                i += 1
            Loop
            cuttingBlock = restText.Substring(0, Math.Max(1, i - 1))

            ' Schritt 2: Trennzeichen im CuttingBlock rückwärts suchen
            For j As Integer = cuttingBlock.Length - 1 To 0 Step -1
                Dim ch As Char = cuttingBlock(j)

                If breakCharsBefore.Contains(ch) Then
                    trennIndex = j
                    trennTyp = "Before"
                    Exit For
                ElseIf breakCharsAfter.Contains(ch) Then
                    trennIndex = j + 1
                    trennTyp = "After"
                    Exit For
                ElseIf breakCharsCountDependent.Contains(ch) Then
                    trennTyp = "Count"
                    If breakCharsCountDependetCountIsOdd Then
                        trennIndex = j
                        breakCharsCountDependetCountIsOdd = False
                    Else
                        trennIndex = j + 1
                        breakCharsCountDependetCountIsOdd = True
                    End If
                    Exit For
                ElseIf breakAndDeleteChars.Contains(ch) Then
                    trennIndex = j
                    trennTyp = "Delete"
                    Exit For
                End If
            Next

            ' Schritt 3: Schneiden
            If trennIndex = -1 Then
                trennIndex = Math.Max(1, cuttingBlock.Length - 2)
                cutText = cuttingBlock.Substring(0, trennIndex) & "-"
                If restText.Length > trennIndex Then
                    restText = restText.Substring(trennIndex)
                Else
                    restText = ""
                End If
            Else
                If restText.Length >= trennIndex Then
                    cutText = restText.Substring(0, trennIndex)
                    restText = restText.Substring(trennIndex)
                    If trennTyp = "Delete" AndAlso restText.Length > 0 Then
                        restText = restText.Substring(1)
                    End If
                Else
                    ' Sicherheitshalber abbrechen
                    Exit Do
                End If
            End If

            resultList.Add(cutText)
            cutText = ""
            cuttingBlock = ""
        Loop

        If restText.Length > 0 Then
            resultList.Add(restText)
        End If

        Return resultList

    End Function


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
