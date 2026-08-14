Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools
Imports SlideShowTools.MetaDataHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling

Public Class ShaderMain
    Implements ISlideShowShader

#Region "Variablendeklaration und Strukturen"

    'Verwaltung
    Public Const nameShader As String = "Nachtsicht"
    Public Const SLIDESHOWSHADER_NACHTSICHT_FULLPATH As String = SLIDESHOWSHADER_PATH & "Nachtsicht\"

    Private aktuelleSettings As ShaderSettings_Nachtsicht

    Private wurdeBereinigt As Boolean

    Private ReadOnly rnd As New Random()

    'Settings
    Public Structure ShaderSettings_Nachtsicht

        Public BNDOverlayAktiv As Boolean

    End Structure

    'Interne Daten des optionalen BND-Overlays
    Private Structure BNDOverlayInformationen

        Public Aktenzeichen As String
        Public Datum As String
        Public Dateipfad As String

        Public GeoPosition As String
        Public Schlagworte As String

        Public Geheimstufe As String
        Public GeheimStufeFarbe As System.Drawing.Brush

        Public KameraCode As String

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

            Return "SIE beobachten Dich! Überall haben SIE ihre Nachtsichtkameras installiert, " &
                   "um Informationen über Dich zu sammeln!"

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
        'Erzeugt aus dem Quellbild eine Nachtsichtkamera-Darstellung
        'und ergänzt optional das BND-Informations-Overlay.

        Dim zielSize As Size
        Dim resultImage As Bitmap
        Dim gfx As Graphics

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(ShaderMain))

        End If

        If baseImage Is Nothing Then

            Throw New ArgumentNullException(NameOf(baseImage))

        End If

        zielSize = clientSize

        If zielSize.IsEmpty Then

            zielSize = baseImage.Size

        End If

        If zielSize.Width <= 0 OrElse zielSize.Height <= 0 Then

            Throw New ArgumentOutOfRangeException(NameOf(clientSize),
                                                  "Die Zielgröße des Nachtsicht-Shaders ist ungültig.")

        End If

        ReadShaderSettingsFromRegistryOrDefaults()

        resultImage = New Bitmap(zielSize.Width, zielSize.Height, Imaging.PixelFormat.Format32bppArgb)

        gfx = Nothing

        Try

            gfx = Graphics.FromImage(resultImage)
            gfx.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            gfx.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            gfx.Clear(HintergrundFarbeSaver)

            RenderNachtsichtkamera(gfx, baseImage, zielSize)

            If aktuelleSettings.BNDOverlayAktiv Then

                RenderBNDOverlay(gfx, imagePath, zielSize)

            End If

            Return resultImage

        Catch

            resultImage.Dispose()

            Throw

        Finally

            If gfx IsNot Nothing Then

                gfx.Dispose()
                gfx = Nothing

            End If

        End Try

    End Function

#End Region

#Region "Nachtsichtkamera"

    Private Sub RenderNachtsichtkamera(gfx As Graphics, baseImage As Image, zielSize As Size)
        'Erzeugt den eigentlichen Nachtsichtkamera-Effekt:
        'Grünfilter, Noise, Scanlines und Vignette.

        Dim colorMatrix As Imaging.ColorMatrix
        Dim imageAttributes As Imaging.ImageAttributes

        Dim noiseOverlay As Image
        Dim scanlineOverlay As Image
        Dim vignetteOverlay As Image

        Dim noiseBrush As TextureBrush
        Dim scanlineBrush As TextureBrush

        Dim zielRect As Rectangle
        Dim fullRect As Rectangle

        imageAttributes = Nothing

        noiseOverlay = Nothing
        scanlineOverlay = Nothing
        vignetteOverlay = Nothing

        noiseBrush = Nothing
        scanlineBrush = Nothing

        colorMatrix =
            New Imaging.ColorMatrix(
                New Single()() {
                    New Single() {0.05F, 0.1F, 0.05F, 0, 0},
                    New Single() {0.9F, 1.0F, 0.9F, 0, 0},
                    New Single() {0.05F, 0.1F, 0.05F, 0, 0},
                    New Single() {0, 0, 0, 1, 0},
                    New Single() {0, 0.05F, 0, 0, 1}
                })

        fullRect = New Rectangle(Point.Empty, zielSize)

        Try

            imageAttributes = New Imaging.ImageAttributes()
            imageAttributes.SetColorMatrix(colorMatrix)

            zielRect =
                GraphicsSizeModeHandling.GetDrawRectangle(
                    baseImage.Size,
                    fullRect,
                    PictureBoxSizeMode.Zoom)

            'Basisbild mit Nachtsicht-Farbmatrix
            gfx.DrawImage(
                baseImage,
                zielRect,
                0,
                0,
                baseImage.Width,
                baseImage.Height,
                GraphicsUnit.Pixel,
                imageAttributes)

            'Noise-Textur erzeugen
            noiseOverlay = TextureHandling.GenerateNoiseTexture(512, 512, 32)

            If noiseOverlay IsNot Nothing Then

                noiseBrush = New TextureBrush(noiseOverlay)
                noiseBrush.WrapMode = Drawing2D.WrapMode.Tile

                gfx.FillRectangle(noiseBrush, fullRect)

            End If

            'Scanlines erzeugen
            scanlineOverlay = TextureHandling.GenerateScanlineTexture(8, zielSize.Height, 23)

            If scanlineOverlay IsNot Nothing Then

                scanlineBrush = New TextureBrush(scanlineOverlay)
                scanlineBrush.WrapMode = Drawing2D.WrapMode.Tile

                gfx.FillRectangle(scanlineBrush, fullRect)

            End If

            'Vignette als eigene Instanz verwenden.
            If My.Resources.vignetteTexture IsNot Nothing Then

                vignetteOverlay = DirectCast(My.Resources.vignetteTexture.Clone(), Image)

                gfx.DrawImage(vignetteOverlay, fullRect)

            End If

        Finally

            If noiseBrush IsNot Nothing Then

                noiseBrush.Dispose()
                noiseBrush = Nothing

            End If

            If scanlineBrush IsNot Nothing Then

                scanlineBrush.Dispose()
                scanlineBrush = Nothing

            End If

            If noiseOverlay IsNot Nothing Then

                noiseOverlay.Dispose()
                noiseOverlay = Nothing

            End If

            If scanlineOverlay IsNot Nothing Then

                scanlineOverlay.Dispose()
                scanlineOverlay = Nothing

            End If

            If vignetteOverlay IsNot Nothing Then

                vignetteOverlay.Dispose()
                vignetteOverlay = Nothing

            End If

            If imageAttributes IsNot Nothing Then

                imageAttributes.Dispose()
                imageAttributes = Nothing

            End If

        End Try

    End Sub

#End Region

#Region "BND-Overlay"

    Private Sub RenderBNDOverlay(gfx As Graphics, imagePath As String, zielSize As Size)
        'Ermittelt die Informationen des optionalen BND-Overlays
        'und zeichnet Rahmen, Metadaten und Kamerainformationen.

        Dim informationen As BNDOverlayInformationen

        Dim overlayFont As Font
        Dim geheimFont As Font

        Dim overlayBrush As System.Drawing.Brush

        Dim zeilen As List(Of String)

        Dim hoeheNaechsterText As Integer
        Dim breiteFuerTextumbruch As Integer

        Dim rechteSpalteX As Integer

        Dim aktenzeichenLabelBreite As Single
        Dim geoLabelBreite As Single

        overlayFont = Nothing
        geheimFont = Nothing

        informationen = ErmittleBNDOverlayInformationen(imagePath)

        overlayBrush = System.Drawing.Brushes.LimeGreen

        Try

            overlayFont = New Font("Lucida Console", 12, FontStyle.Bold)
            geheimFont = New Font("Lucida Console", 18, FontStyle.Bold)

            ZeichneRahmen(gfx, informationen.KameraCode, overlayFont, zielSize)

            aktenzeichenLabelBreite = gfx.MeasureString("Aktenzeichen: ", overlayFont).Width

            geoLabelBreite = gfx.MeasureString("Geo-Position: ", overlayFont).Width

            rechteSpalteX = Math.Max(10, zielSize.Width - 950)

            ' -------------------------------------------------
            ' Aktenzeichen
            ' -------------------------------------------------

            DrawText(gfx, "Aktenzeichen: ", overlayFont, overlayBrush, 10, 20)

            hoeheNaechsterText = 22

            breiteFuerTextumbruch = CInt(((zielSize.Width - gfx.MeasureString(informationen.Geheimstufe,
                                                                              geheimFont).Width) / 2.0) _
                                            - (10 + aktenzeichenLabelBreite) - 10)

            breiteFuerTextumbruch = Math.Max(1, breiteFuerTextumbruch)

            zeilen = TextKorrektTeilen(gfx, informationen.Aktenzeichen, overlayFont, breiteFuerTextumbruch)

            For Each zeile As String In zeilen

                DrawText(gfx, zeile, overlayFont, overlayBrush, CInt(10 + aktenzeichenLabelBreite),
                         hoeheNaechsterText)

                hoeheNaechsterText += CInt(gfx.MeasureString(zeile, overlayFont).Height)

            Next

            ' -------------------------------------------------
            ' Datum
            ' -------------------------------------------------

            DrawText(gfx, "Datum: ", overlayFont, overlayBrush, 10, hoeheNaechsterText)

            DrawText(gfx, informationen.Datum, overlayFont, overlayBrush, CInt(10 + aktenzeichenLabelBreite),
                     hoeheNaechsterText)

            ' -------------------------------------------------
            ' Geo-Position
            ' -------------------------------------------------

            DrawText(gfx, "Geo-Position: ", overlayFont, overlayBrush, rechteSpalteX, 20)

            DrawText(gfx, informationen.GeoPosition, overlayFont, overlayBrush, CInt(rechteSpalteX + geoLabelBreite), 20)

            hoeheNaechsterText = 20 + CInt(gfx.MeasureString("Geo-Position: ", overlayFont).Height)

            ' -------------------------------------------------
            ' Schlagworte
            ' -------------------------------------------------

            DrawText(gfx, "Schlagworte: ", overlayFont, overlayBrush, rechteSpalteX, hoeheNaechsterText)

            breiteFuerTextumbruch = CInt(zielSize.Width - (rechteSpalteX + geoLabelBreite) - 10)

            breiteFuerTextumbruch = Math.Max(1, breiteFuerTextumbruch)

            zeilen = TextKorrektTeilen(gfx, informationen.Schlagworte, overlayFont, breiteFuerTextumbruch)

            For Each zeile As String In zeilen

                DrawText(gfx, zeile, overlayFont, overlayBrush, CInt(rechteSpalteX + geoLabelBreite),
                         hoeheNaechsterText)

                hoeheNaechsterText += CInt(gfx.MeasureString(zeile, overlayFont).Height)

            Next

            ' -------------------------------------------------
            ' Geheimhaltungsstufe
            ' -------------------------------------------------

            DrawText(gfx, "Geheimhaltungsstufe", overlayFont, overlayBrush, CInt((zielSize.Width -
                     gfx.MeasureString("Geheimhaltungsstufe", overlayFont).Width) / 2.0), 60)

            DrawText(gfx, informationen.Geheimstufe, geheimFont, informationen.GeheimStufeFarbe,
                     CInt((zielSize.Width - gfx.MeasureString(informationen.Geheimstufe, geheimFont).Width) / 2.0),
                     105)

            ' -------------------------------------------------
            ' Bibliothekarischer Hinweis / Ablageort
            ' -------------------------------------------------

            DrawText(gfx, "Bibliothekarischer Hinweis:" & vbCrLf & "Ablageort: " & informationen.Dateipfad,
                     overlayFont, overlayBrush, 10, Math.Max(10, zielSize.Height - 85))

            ' -------------------------------------------------
            ' Kamera-Code
            ' -------------------------------------------------

            DrawText(gfx, informationen.KameraCode, overlayFont, overlayBrush, CInt((zielSize.Width -
                     gfx.MeasureString(informationen.KameraCode, overlayFont).Width) / 2.0), 0)

        Finally

            If overlayFont IsNot Nothing Then

                overlayFont.Dispose()
                overlayFont = Nothing

            End If

            If geheimFont IsNot Nothing Then

                geheimFont.Dispose()
                geheimFont = Nothing

            End If

        End Try

    End Sub

    Private Function ErmittleBNDOverlayInformationen(
        imagePath As String) As BNDOverlayInformationen
        'Ermittelt sämtliche Informationen, die für das BND-Overlay
        'benötigt werden. Ohne gültigen Bildpfad werden sichere
        'Ersatzwerte geliefert.

        Dim informationen As BNDOverlayInformationen
        Dim metadaten As Metadata
        Dim stichworte As List(Of String)
        Dim extension As String
        Dim locationHelper As LocationHandling
        Dim dateiIstGueltig As Boolean

        informationen.Aktenzeichen = "Unbekannt"
        informationen.Datum = "Unbekannt"
        informationen.Dateipfad = "Unbekannt"
        informationen.GeoPosition = "Unbekannt"
        informationen.Schlagworte = "-"
        informationen.Geheimstufe = "VS-Nur für den Dienstgebrauch"
        informationen.GeheimStufeFarbe = System.Drawing.Brushes.LimeGreen
        informationen.KameraCode = GeneriereKameracode()

        stichworte = New List(Of String)()
        locationHelper = Nothing

        dateiIstGueltig = Not String.IsNullOrWhiteSpace(imagePath) AndAlso File.Exists(imagePath)

        If Not dateiIstGueltig Then

            Return informationen

        End If

        informationen.Dateipfad = imagePath
        informationen.Aktenzeichen = Path.GetFileNameWithoutExtension(imagePath)
        informationen.Datum = File.GetCreationTime(imagePath).ToString("dd.MM.yyyy HH:mm:ss",
                                                                       CultureInfo.CurrentCulture)

        extension = Path.GetExtension(imagePath)

        If Not String.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase) AndAlso
           Not String.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase) Then

            Return informationen

        End If

        metadaten = ExtractMetadataFromImage(imagePath)

        ' -------------------------------------------------
        ' Keywords
        ' -------------------------------------------------

        If metadaten.Keywords IsNot Nothing Then

            stichworte = New List(Of String)(metadaten.Keywords)

        End If

        If stichworte.Count = 0 Then

            stichworte.Add("-")

        End If

        ' -------------------------------------------------
        ' Geo-Position
        ' -------------------------------------------------

        If metadaten.geographicLongitude <> 0 AndAlso
           metadaten.geographicLatitude <> 0 Then

            informationen.GeoPosition =
                metadaten.geographicLatitude.ToString(
                    CultureInfo.CurrentCulture) &
                ", " &
                metadaten.geographicLongitude.ToString(
                    CultureInfo.CurrentCulture)

        Else

            locationHelper = New LocationHandling()

            informationen.GeoPosition = locationHelper.DetermineGPSLocation(stichworte)

            If String.IsNullOrWhiteSpace(informationen.GeoPosition) Then

                informationen.GeoPosition = "Unbekannt"

            End If

        End If

        ' -------------------------------------------------
        ' Geheimhaltungsstufe
        ' -------------------------------------------------

        If stichworte.Any(
            Function(keyword)
                Return String.Equals(
                    keyword.Trim(),
                    "18+",
                    StringComparison.OrdinalIgnoreCase)
            End Function) Then

            informationen.Geheimstufe = "Streng Geheim"
            informationen.GeheimStufeFarbe = System.Drawing.Brushes.Orange

        ElseIf stichworte.Any(
            Function(keyword)
                Return String.Equals(
                    keyword.Trim(),
                    "Akt",
                    StringComparison.OrdinalIgnoreCase)
            End Function) Then

            informationen.Geheimstufe = "Geheim"

        ElseIf stichworte.Any(
            Function(keyword)
                Return String.Equals(
                    keyword.Trim(),
                    "Lingerie",
                    StringComparison.OrdinalIgnoreCase)
            End Function) Then

            informationen.Geheimstufe = "VS-Vertraulich"

        End If

        stichworte.Sort()

        informationen.Schlagworte = String.Join(" | ", stichworte)

        Return informationen

    End Function

    Private Function GeneriereKameracode() As String
        'Erzeugt einen zufälligen fiktiven Kamera-Code.

        Dim teil1 As String
        Dim teil2 As String
        Dim teil3 As String

        teil1 = ""
        teil2 = ""
        teil3 = ""

        For i As Integer = 1 To 4

            If rnd.NextDouble() < 0.5 Then

                teil1 &= Chr(rnd.Next(48, 58))

            Else

                teil1 &= Chr(rnd.Next(65, 91))

            End If

        Next

        teil2 = rnd.Next(100, 1000).ToString()
        teil3 = rnd.Next(10, 100).ToString("00")

        Return "Überwachungskamera #" & teil1 & "-" & teil2 & "-" & teil3

    End Function

    Private Sub ZeichneRahmen(gfx As Graphics, titelText As String, overlayFont As Font, zielSize As Size)
        'Zeichnet den grünen Überwachungsrahmen um die
        'tatsächliche Zielgröße des Shaderbildes.

        Dim titelSize As SizeF

        Dim titelbreite As Integer
        Dim bildbreite As Integer
        Dim bildhoehe As Integer

        Dim rahmenStift As Pen
        Dim korrekturStift As Pen

        rahmenStift = Nothing
        korrekturStift = Nothing

        titelSize = gfx.MeasureString(titelText, overlayFont)

        titelbreite = CInt(titelSize.Width)
        bildbreite = zielSize.Width

        bildhoehe = zielSize.Height

        Try

            rahmenStift = New Pen(System.Drawing.Color.LimeGreen, 10)

            'Links oben bis zur Titellücke
            gfx.DrawLine(rahmenStift, 0, 10, (bildbreite - titelbreite) \ 2, 10)

            'Rechts oben nach der Titellücke
            gfx.DrawLine(rahmenStift, (bildbreite + titelbreite) \ 2, 10, bildbreite - 1, 10)

            'Links
            gfx.DrawLine(rahmenStift, 0, 10, 0, bildhoehe - 1)

            'Rechts
            gfx.DrawLine(rahmenStift, bildbreite - 1, 10, bildbreite - 1, bildhoehe - 1)

            'Unten
            gfx.DrawLine(rahmenStift, 0, bildhoehe - 1, bildbreite - 1, bildhoehe - 1)

            'Oberen Rand korrigieren
            korrekturStift = New Pen(System.Drawing.Color.Black, 10)

            gfx.DrawLine(korrekturStift, 0, 0, bildbreite, 0)

        Finally

            If rahmenStift IsNot Nothing Then

                rahmenStift.Dispose()
                rahmenStift = Nothing

            End If

            If korrekturStift IsNot Nothing Then

                korrekturStift.Dispose()
                korrekturStift = Nothing

            End If

        End Try

    End Sub

#End Region

#Region "Textdarstellung"

    Private Sub DrawText(gfx As Graphics, text As String, font As Font, brush As System.Drawing.Brush,
                         x As Integer, y As Integer)
        'Zeichnet einen Text mit halbtransparentem schwarzen Hintergrund.

        Dim textSize As SizeF
        Dim backgroundBrush As SolidBrush

        backgroundBrush = Nothing

        textSize = gfx.MeasureString(text, font)

        Try

            backgroundBrush =
                New SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0))

            gfx.FillRectangle(backgroundBrush, x - 4, y - 2, textSize.Width + 8, textSize.Height + 4)

            gfx.DrawString(text, font, brush, x, y)

        Finally

            If backgroundBrush IsNot Nothing Then

                backgroundBrush.Dispose()
                backgroundBrush = Nothing

            End If

        End Try

    End Sub

    Private Function TextKorrektTeilen(gfx As Graphics, zuTrennenderText As String, zuTrennenderFont As Font,
                                       maxWidth As Integer) As List(Of String)
        'Teilt Text anhand der verfügbaren Pixelbreite
        'unter Berücksichtigung sinnvoller Trennzeichen.

        Dim breakCharsBefore As Char()
        Dim breakCharsAfter As Char()
        Dim breakAndDeleteChars As Char()
        Dim breakCharsCountDependent As Char()

        Dim resultList As List(Of String)

        Dim cutText As String
        Dim cuttingBlock As String
        Dim restText As String

        Dim trennTyp As String
        Dim trennIndex As Integer

        Dim i As Integer
        Dim ch As Char

        Dim breakCharsCountDependentCountIsOdd As Boolean

        breakCharsBefore = New Char() {"("c, "{"c, "["c}
        breakCharsAfter = New Char() {")"c, "}"c, "]"c, "."c, "-"c, "\"c, "/"c, "|"c}
        breakAndDeleteChars = New Char() {" "c}
        breakCharsCountDependent = New Char() {""""c, "'"c}

        resultList = New List(Of String)()

        cutText = ""
        cuttingBlock = ""

        restText = If(zuTrennenderText, "").Trim()

        breakCharsCountDependentCountIsOdd = True

        If restText = "" OrElse maxWidth <= 0 Then

            resultList.Add(restText)

            Return resultList

        End If

        Do While gfx.MeasureString(restText, zuTrennenderFont).Width > maxWidth

            trennTyp = ""
            trennIndex = -1

            i = 1

            'Den maximal passenden CuttingBlock ermitteln.
            Do While i <= restText.Length

                cuttingBlock = restText.Substring(0, i)

                If gfx.MeasureString(cuttingBlock, zuTrennenderFont).Width > maxWidth Then

                    Exit Do

                End If

                i += 1

            Loop

            cuttingBlock = restText.Substring(0, Math.Max(1, i - 1))

            'Von hinten nach einem sinnvollen Trennzeichen suchen.
            For j As Integer = cuttingBlock.Length - 1 To 0 Step -1

                ch = cuttingBlock(j)

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

                    If breakCharsCountDependentCountIsOdd Then

                        trennIndex = j

                        breakCharsCountDependentCountIsOdd = False

                    Else

                        trennIndex = j + 1

                        breakCharsCountDependentCountIsOdd = True

                    End If

                    Exit For

                ElseIf breakAndDeleteChars.Contains(ch) Then

                    trennIndex = j
                    trennTyp = "Delete"

                    Exit For

                End If

            Next

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

                    If trennTyp = "Delete" AndAlso
                       restText.Length > 0 Then

                        restText = restText.Substring(1)

                    End If

                Else

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

#End Region

#Region "Optionen und Settings"

    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Liest die aktuellen Settings ein und stellt sie
        'dem Options-Control über die SettingsInbox bereit.

        ReadShaderSettingsFromRegistryOrDefaults()

        StoreSettings(nameShader, aktuelleSettings)

        Return New ucOptionsShader()

    End Function

    Friend Shared Function GetShaderDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Default-Settings des Nachtsicht-Shaders.

        Dim defaults As New Dictionary(Of String, String)

        defaults("BNDOverlayAktiv") = "True"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Liest die aktuellen Shadereinstellungen aus der Registry
        'oder verwendet die definierten Defaultwerte.

        Dim defaults As Dictionary(Of String, String)
        Dim overlayAktiv As String

        defaults = GetShaderDefaultSettings()

        overlayAktiv = ReadFromRegOrDefaults(SLIDESHOWSHADER_NACHTSICHT_FULLPATH & "BNDOverlayAktiv", defaults)

        aktuelleSettings.BNDOverlayAktiv =
            String.Equals(
                overlayAktiv,
                "True",
                StringComparison.OrdinalIgnoreCase)

    End Sub

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