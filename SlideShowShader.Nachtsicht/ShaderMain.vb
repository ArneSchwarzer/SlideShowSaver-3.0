Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.TextureHandling
Imports SlideShowTools.LocationHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.RegistryHandling
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

#Region "Variablendeklaration"
    'Variablendeklarationen

    'Verwaltung
    Public Shared nameShader As String = "Nachtsicht"
    Private Shared aktuelleSettings As ShaderSettings_Nachtsicht
    Public Shared SLIDESHOWSHADER_NACHTSICHT_FULLPATH As String = SLIDESHOWSHADER_PATH & "Nachtsicht\"

    'Texturen
    Private noiseOverlay As Image
    Private scanlineOverlay As Image
    Private vignetteOverlay As Image

    'Grapics
    Private g As Graphics
    Private zielRect As Rectangle

#End Region

#Region "Structures, Enums etc."
    'Structures
    Public Structure ShaderSettings_Nachtsicht

    End Structure
#End Region

#Region "Eigenschaften"
    ' === Eigenschaften ===
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return nameShader
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Simuliert eine Nachtsichtkamera des BND mit grünem Farbton, Rauschen, Scanlines... und 'Geheiminformationen'."
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property
#End Region

    'Shader-Ausführung
    Public Function RunShader(baseImage As Image, Optional imagePath As String = "", Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Wandelt ein Bild in einen Überwachungsmonitor des BND...

        'Variablendeklarationen

        'Bild- und Zielgrößen
        Dim zielSize As Size = If(clientSize = Nothing, baseImage.Size, clientSize)
        Dim bildPfad As String = imagePath
        Dim resultImage As New Bitmap(zielSize.Width, zielSize.Height)
        Dim ia As New Imaging.ImageAttributes()

        'Nachtsicht-Farbfilter
        Dim colorMatrix As New Imaging.ColorMatrix(New Single()() {
        New Single() {0.05F, 0.1F, 0.05F, 0, 0},
        New Single() {0.9F, 1.0F, 0.9F, 0, 0},
        New Single() {0.05F, 0.1F, 0.05F, 0, 0},
        New Single() {0, 0, 0, 1, 0},
        New Single() {0, 0.05F, 0, 0, 1}
        })

        'Metadaten
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

        ' --- Text-Overlay-Stil ---
        Dim overlayFont As New Font("Lucida Console", 12, FontStyle.Bold)
        Dim geheimFont As New Font("Lucida Console", 18, FontStyle.Bold)
        Dim overlayBrush As Brush = Brushes.LimeGreen
        Dim backgroundBrush As New SolidBrush(Color.FromArgb(128, Color.Black))
        Dim zeilen As List(Of String)
        Dim höheNächsterText As Integer
        Dim breiteFuerTextumbruch As Integer

        'Initialisierungen

        'Zur Zeit keine aktuelleSettings zum initialisieren
        'CheckYourSettings()

        'Graphics initialisieren
        g = Graphics.FromImage(resultImage)
        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
        g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
        g.Clear(Color.Black)

        ia.SetColorMatrix(colorMatrix)

        'Eigentliche Shaderfunktion

        'Overlay Texturen einrichten
        noiseOverlay = TextureHandling.GenerateNoiseTexture(512, 512, 32)
        scanlineOverlay = TextureHandling.GenerateScanlineTexture(8, zielSize.Height, 23)
        vignetteOverlay = My.Resources.vignetteTexture

        'Basisbild mittig einpassen
        zielRect = GraphicsSizeModeHandling.GetDrawRectangle(baseImage.Size, New Rectangle(0, 0, zielSize.Width, zielSize.Height), PictureBoxSizeMode.Zoom)

        g.DrawImage(baseImage, zielRect, 0, 0, baseImage.Width, baseImage.Height, GraphicsUnit.Pixel, ia)

        'Noise-Ebene
        If noiseOverlay IsNot Nothing Then
            Using noiseBrush As New TextureBrush(noiseOverlay)
                noiseBrush.WrapMode = Drawing2D.WrapMode.Tile
                g.FillRectangle(noiseBrush, New Rectangle(Point.Empty, zielSize))
            End Using
        End If

        'Scanline-Ebene
        If scanlineOverlay IsNot Nothing Then
            Using scanBrush As New TextureBrush(scanlineOverlay)
                scanBrush.WrapMode = Drawing2D.WrapMode.Tile
                g.FillRectangle(scanBrush, New Rectangle(Point.Empty, zielSize))
            End Using
        End If

        'Metadaten via TagLib/MetaDataExtraktor und LocationHandling
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

                locationHelper = Nothing

            End If

            'Geheimstufe (Altersfreigabe) setzen und Schlagwort-String (Tags) vorbereiten
            If keywords IsNot Nothing Then
                If keywords.Any(Function(k) String.Equals(k.Trim(), "18+", StringComparison.OrdinalIgnoreCase)) Then
                    geheimstufe = "Streng Geheim"
                    geheimStufeFarbe = Brushes.Orange
                ElseIf keywords.Any(Function(k) String.Equals(k.Trim(), "Akt", StringComparison.OrdinalIgnoreCase)) Then
                    geheimstufe = "Geheim"
                ElseIf keywords.Any(Function(k) String.Equals(k.Trim(), "Lingerie", StringComparison.OrdinalIgnoreCase)) Then
                    geheimstufe = "VS-Vertraulich"
                End If

                keywords.Sort()
                schlagworte = String.Join(" | ", keywords)
            Else
                schlagworte = ""
            End If
        End If

        'Phantasie-Kamera Code erzeugen
        kameraCode = GeneriereKameracode()

        'Rahmen zeichnen
        ZeichneRahmen(kameraCode, overlayFont)

        'Positionen der Textboxen berechnen & zeichnen

        'Aktenzeichen (Dateiname)
        DrawText(g, "Aktenzeichen: ", overlayFont, overlayBrush, 10, 20)
        höheNächsterText = 20

        breiteFuerTextumbruch = (((resultImage.Width - g.MeasureString(geheimstufe, geheimFont).Width) \ 2) - (10 + g.MeasureString("Aktenzeichen: ", overlayFont).Width)) - 10

        zeilen = TextKorrektTeilen(g, aktenzeichen, overlayFont, breiteFuerTextumbruch)
        For Each zeile In zeilen
            DrawText(g, zeile, overlayFont, overlayBrush, 10 + g.MeasureString("Aktenzeichen: ", overlayFont).Width, höheNächsterText)
            höheNächsterText += g.MeasureString(zeile, overlayFont).Height
        Next

        'Datum
        DrawText(g, "Datum: ", overlayFont, overlayBrush, 10, höheNächsterText)
        DrawText(g, datum, overlayFont, overlayBrush, 10 + g.MeasureString("Aktenzeichen: ", overlayFont).Width, höheNächsterText)

        'Geo-Position
        DrawText(g, "Geo-Position: ", overlayFont, overlayBrush, resultImage.Width - 950, 20)
        DrawText(g, geoPosition, overlayFont, overlayBrush, (resultImage.Width - 950) + g.MeasureString("Geo-Position: ", overlayFont).Width, 20)
        höheNächsterText = 20 + g.MeasureString("Geo-Postition: ", overlayFont).Height

        'Schlagworte (Tags)
        DrawText(g, "Schlagworte: ", overlayFont, overlayBrush, resultImage.Width - 950, höheNächsterText)

        breiteFuerTextumbruch = (resultImage.Width - ((resultImage.Width - 950) + g.MeasureString("Geo-Position: ", overlayFont).Width)) - 10

        zeilen = TextKorrektTeilen(g, schlagworte, overlayFont, breiteFuerTextumbruch)
        For Each zeile In zeilen
            DrawText(g, zeile, overlayFont, overlayBrush, (resultImage.Width - 950) + g.MeasureString("Geo-Position: ", overlayFont).Width, höheNächsterText)
            höheNächsterText += g.MeasureString(zeile, overlayFont).Height
        Next

        'Geheimstufe
        DrawText(g, "Geheimhaltungsstufe", overlayFont, overlayBrush, (resultImage.Width - g.MeasureString("Geheimhaltunsgstufe", overlayFont).Width) \ 2, 60)
        DrawText(g, geheimstufe, geheimFont, geheimStufeFarbe, (resultImage.Width - g.MeasureString(geheimstufe, geheimFont).Width) \ 2, 105)

        'Bibleothekarischer Hinweis (Dateipfad)
        DrawText(g, "Bibliothekarischer Hinweis:" & vbCrLf & "Ablageort: " & dateipfad, overlayFont, overlayBrush, 10, resultImage.Height - 80)

        'Kamera
        DrawText(g, kameraCode, overlayFont, overlayBrush, (resultImage.Width - g.MeasureString(kameraCode, overlayFont).Width) \ 2, 0)

        'Vignette
        If vignetteOverlay IsNot Nothing Then
            g.DrawImage(vignetteOverlay, New Rectangle(Point.Empty, zielSize))
        End If

        'Fertig und raus...
        g.Dispose()
        Return resultImage

    End Function

    'Optionen/Dialog
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Lädt die aktuellen Settings und legt sie in SettingsInbox ab (z.Zt. noch ohne Funktion, da der Shader
        'aktuell keine Optionen bietet).

        'aktuelleSettings = ReadShaderSettingsFromRegistryOrDefaults()
        'StoreSettings(nameShader, aktuelleSettings)

        Return New ucOptionsShader()

    End Function

    'Private Methoden
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
        g.DrawLine(rahmenStift, 0, 10, 0, bildhöhe - 1)

        ' Rechts
        g.DrawLine(rahmenStift, bildbreite - 1, 10, bildbreite - 1, bildhöhe - 1)

        ' Unten
        g.DrawLine(rahmenStift, 0, bildhöhe - 1, bildbreite - 1, bildhöhe - 1)

        'Oberen Rand korrigieren
        rahmenStift = New Pen(Color.Black, 10)
        g.DrawLine(rahmenStift, 0, 0, bildbreite, 0)

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

    Private Function GetShaderDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Settings des Shaders als Dictionary. Zur Zeit ohne Funktion

        Dim defaults As New Dictionary(Of String, String)

        'defaults("Beispiel") = "Beispielinhalt"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Setzt aktuelleSettings auf die Registry oder auf Default-Werte. Zur Zeit ohne Funktion

        Dim defaults As New Dictionary(Of String, String)

        'aktuelleSettings.Beispiel = ReadFromRegistryOrDefaults(SLIDESHOWSHADER_NACHTSICHT_FULLPATH & "Beispiel", defaults)

    End Sub
End Class
