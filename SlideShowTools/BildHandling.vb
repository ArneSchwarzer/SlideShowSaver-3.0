Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports MetadataExtractor
Imports MetadataExtractor.Formats.Exif
Imports SlideShowLogging

Public NotInheritable Class BildHandling

#Region "Öffentliche Methoden"

    ''' <summary>
    ''' Lädt ein Bild vollständig in den Arbeitsspeicher, ohne die Quelldatei
    ''' anschließend gesperrt zu halten.
    ''' </summary>
    ''' <param name="bildPfad">
    ''' Vollständiger Pfad der zu ladenden Bilddatei.
    ''' </param>
    ''' <param name="ausrichtungKorrigieren">
    ''' Gibt an, ob bei JPEG-Dateien die EXIF-Ausrichtung berücksichtigt wird.
    ''' </param>
    Public Shared Function LadeBild(
        bildPfad As String,
        Optional ausrichtungKorrigieren As Boolean = True
    ) As Image

        Dim dateiStream As FileStream
        Dim quellBild As Image
        Dim geladenesBild As Bitmap

        dateiStream = Nothing
        quellBild = Nothing
        geladenesBild = Nothing

        If String.IsNullOrWhiteSpace(bildPfad) Then
            Return Nothing
        End If

        If Not File.Exists(bildPfad) Then

            LogHandling.LogWarn(
                "Bilddatei nicht gefunden: " &
                bildPfad)

            Return Nothing

        End If

        Try

            dateiStream =
                New FileStream(
                    bildPfad,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite Or FileShare.Delete)

            quellBild = Image.FromStream(dateiStream, True, True)

            ' Eine unabhängige Bitmapkopie erzeugen, damit weder der Stream
            ' noch die Quelldatei nach der Rückgabe geöffnet bleiben.
            geladenesBild = New Bitmap(quellBild)

            If ausrichtungKorrigieren AndAlso
               FileHandling.IstJpegDatei(bildPfad) Then

                KorrigiereBildausrichtung(
                    geladenesBild,
                    bildPfad)

            End If

            Return geladenesBild

        Catch ex As Exception

            If geladenesBild IsNot Nothing Then
                geladenesBild.Dispose()
                geladenesBild = Nothing
            End If

            LogHandling.LogError(
                "Bild """ &
                bildPfad &
                """ konnte nicht geladen werden: " &
                ex.ToString())

            Return Nothing

        Finally

            If quellBild IsNot Nothing Then
                quellBild.Dispose()
            End If

            If dateiStream IsNot Nothing Then
                dateiStream.Dispose()
            End If

        End Try

    End Function

    ''' <summary>
    ''' Korrigiert die Darstellung eines Bildes anhand des in der JPEG-Datei
    ''' gespeicherten EXIF-Orientierungswertes.
    ''' </summary>
    ''' <remarks>
    ''' Das übergebene Bild wird direkt verändert.
    ''' </remarks>
    Public Shared Function KorrigiereBildausrichtung(
        bild As Image,
        bildPfad As String
    ) As Image

        Dim ausrichtung As Integer
        Dim verzeichnisse As IReadOnlyList(Of MetadataExtractor.Directory)
        Dim exifVerzeichnis As ExifIfd0Directory

        ausrichtung = 1
        verzeichnisse = Nothing
        exifVerzeichnis = Nothing

        If bild Is Nothing Then
            Return Nothing
        End If

        If String.IsNullOrWhiteSpace(bildPfad) Then
            Return bild
        End If

        If Not FileHandling.IstJpegDatei(bildPfad) Then
            Return bild
        End If

        Try

            verzeichnisse =
                ImageMetadataReader.ReadMetadata(
                    bildPfad)

            exifVerzeichnis =
                verzeichnisse.
                OfType(Of ExifIfd0Directory)().
                FirstOrDefault()

            If exifVerzeichnis IsNot Nothing AndAlso
               exifVerzeichnis.ContainsTag(
                   ExifDirectoryBase.TagOrientation) Then

                ausrichtung =
                    exifVerzeichnis.GetInt32(
                        ExifDirectoryBase.TagOrientation)

            End If

        Catch ex As Exception

            LogHandling.LogWarn(
                "Die Bildausrichtung von """ &
                bildPfad &
                """ konnte nicht ausgelesen werden: " &
                ex.Message)

            Return bild

        End Try

        Select Case ausrichtung

            Case 1
                ' Keine Änderung erforderlich

            Case 2
                bild.RotateFlip(RotateFlipType.RotateNoneFlipX)

            Case 3
                bild.RotateFlip(RotateFlipType.Rotate180FlipNone)

            Case 4
                bild.RotateFlip(RotateFlipType.Rotate180FlipX)

            Case 5
                bild.RotateFlip(RotateFlipType.Rotate90FlipX)

            Case 6
                bild.RotateFlip(RotateFlipType.Rotate90FlipNone)

            Case 7
                bild.RotateFlip(RotateFlipType.Rotate270FlipX)

            Case 8
                bild.RotateFlip(RotateFlipType.Rotate270FlipNone)

            Case Else

                LogHandling.LogWarn(
                    "Unbekannter EXIF-Orientierungswert " &
                    ausrichtung.ToString() &
                    " in der Bilddatei """ &
                    bildPfad &
                    """.")

        End Select

        Return bild

    End Function

    ''' <summary>
    ''' Erzeugt eine neue Bildinstanz, die um den angegebenen Winkel gedreht ist.
    ''' Der vollständige Bildinhalt bleibt erhalten; neu entstehende Randbereiche
    ''' sind transparent.
    ''' </summary>
    Public Shared Function DreheBild(
        bild As Image,
        winkel As Single
    ) As Image

        Dim originalBreite As Integer
        Dim originalHoehe As Integer
        Dim neueBreite As Integer
        Dim neueHoehe As Integer
        Dim cosinus As Double
        Dim sinus As Double
        Dim winkelRadiant As Double
        Dim mittelpunktX As Single
        Dim mittelpunktY As Single
        Dim gedrehtesBild As Bitmap
        Dim horizontaleAufloesung As Single
        Dim vertikaleAufloesung As Single

        If bild Is Nothing Then
            Return Nothing
        End If

        winkel = winkel Mod 360.0F

        If winkel < 0.0F Then
            winkel += 360.0F
        End If

        If Math.Abs(winkel) < Single.Epsilon Then
            Return New Bitmap(bild)
        End If

        originalBreite = bild.Width
        originalHoehe = bild.Height

        winkelRadiant =
            winkel *
            Math.PI /
            180.0

        cosinus =
            Math.Abs(
                Math.Cos(
                    winkelRadiant))

        sinus =
            Math.Abs(
                Math.Sin(
                    winkelRadiant))

        ' Ceiling verhindert, dass Randpixel durch Abrundung abgeschnitten werden.
        neueBreite =
            CInt(
                Math.Ceiling(
                    originalBreite * cosinus +
                    originalHoehe * sinus))

        neueHoehe =
            CInt(
                Math.Ceiling(
                    originalBreite * sinus +
                    originalHoehe * cosinus))

        gedrehtesBild =
            New Bitmap(
                neueBreite,
                neueHoehe,
                PixelFormat.Format32bppArgb)

        horizontaleAufloesung = bild.HorizontalResolution
        vertikaleAufloesung = bild.VerticalResolution

        If horizontaleAufloesung > 0.0F AndAlso
           vertikaleAufloesung > 0.0F Then

            gedrehtesBild.SetResolution(
                horizontaleAufloesung,
                vertikaleAufloesung)

        End If

        mittelpunktX = CSng(neueBreite / 2.0F)
        mittelpunktY = CSng(neueHoehe / 2.0F)

        Using grafik As Graphics = Graphics.FromImage(gedrehtesBild)

            grafik.SmoothingMode = SmoothingMode.AntiAlias
            grafik.InterpolationMode = InterpolationMode.HighQualityBicubic
            grafik.PixelOffsetMode = PixelOffsetMode.HighQuality
            grafik.CompositingQuality = CompositingQuality.HighQuality
            grafik.Clear(Color.Transparent)
            grafik.TranslateTransform(mittelpunktX, mittelpunktY)
            grafik.RotateTransform(winkel)
            grafik.TranslateTransform(CSng(-originalBreite / 2.0F), CSng(-originalHoehe / 2.0F))
            grafik.DrawImage(bild, New Rectangle(0, 0, originalBreite, originalHoehe))

        End Using

        Return gedrehtesBild

    End Function

#End Region

End Class