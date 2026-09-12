Imports System.IO
Imports System.Text
Imports MetadataExtractor
Imports MetadataExtractor.Formats.Exif
Imports MetadataExtractor.Formats.Iptc
Imports MetadataExtractor.Formats.Xmp
Imports System.Drawing
Imports System.Drawing.Imaging

Public Class MetaDataHandling

    Public Structure Metadata
        Public Author As String
        Public Rating As Integer
        Public Keywords As List(Of String)
        Public CreatedDate As DateTime
        Public CameraModel As String
        Public LensModel As String
        Public ExposureTime As String
        Public FNumber As String
        Public ISO As Integer
        Public FocalLength As String
        Public geographicLatitude As Double
        Public geographicLongitude As Double
    End Structure

    Public Shared Function ExtractMetadataFromImage(imagePath As String) As Metadata
        Dim metadata As New Metadata()
        metadata.Keywords = New List(Of String)()

        If Not File.Exists(imagePath) Then Return metadata

        Dim ext As String = Path.GetExtension(imagePath).ToLowerInvariant()
        If Not (ext.EndsWith(".jpg") OrElse ext.EndsWith(".jpeg")) Then Return metadata

        Try
            Dim directories = ImageMetadataReader.ReadMetadata(imagePath)

            Dim iptcDirectory = directories.OfType(Of IptcDirectory)().FirstOrDefault()
            Dim xmpDirectory = directories.OfType(Of XmpDirectory)().FirstOrDefault()
            Dim exifSubIfdDirectory = directories.OfType(Of ExifSubIfdDirectory)().FirstOrDefault()
            Dim exifIfd0Directory = directories.OfType(Of ExifIfd0Directory)().FirstOrDefault() ' FIX: für Camera Model
            Dim gpsDirectory = directories.OfType(Of GpsDirectory)().FirstOrDefault()

            ' --- XMP: Rating & (optional) Autor ---
            If xmpDirectory IsNot Nothing AndAlso xmpDirectory.XmpMeta IsNot Nothing Then
                Dim ratingStr As String = xmpDirectory.XmpMeta.GetPropertyString("http://ns.adobe.com/xap/1.0/", "Rating")
                Dim ratingVal As Integer
                If Integer.TryParse(ratingStr, ratingVal) Then
                    metadata.Rating = ratingVal
                End If

                ' Versuche zuerst dc:creator (falls vorhanden)
                Dim dcNs As String = "http://purl.org/dc/elements/1.1/"
                Dim creatorCount As Integer = xmpDirectory.XmpMeta.CountArrayItems(dcNs, "creator")
                If creatorCount > 0 Then
                    Dim item = xmpDirectory.XmpMeta.GetArrayItem(dcNs, "creator", 1)
                    If item IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(item.Value) Then
                        metadata.Author = item.Value
                    End If
                End If
            End If

            ' --- IPTC: Keywords & Autor (Fallback) ---
            If iptcDirectory IsNot Nothing Then
                Dim iptcKeywords = iptcDirectory.GetKeywords()
                If iptcKeywords IsNot Nothing AndAlso iptcKeywords.Any() Then
                    metadata.Keywords.AddRange(iptcKeywords)
                End If

                If String.IsNullOrWhiteSpace(metadata.Author) Then
                    Dim byLine = iptcDirectory.GetDescription(IptcDirectory.TagByLine)
                    If Not String.IsNullOrWhiteSpace(byLine) Then metadata.Author = byLine
                End If
            End If

            ' --- XMP: Keywords (Fallback, wenn IPTC leer oder ergänzend) ---
            If xmpDirectory IsNot Nothing AndAlso xmpDirectory.XmpMeta IsNot Nothing Then
                ' dc:subject
                Dim dcNs As String = "http://purl.org/dc/elements/1.1/"
                Dim subjCount As Integer = xmpDirectory.XmpMeta.CountArrayItems(dcNs, "subject")
                For i As Integer = 1 To subjCount
                    Dim it = xmpDirectory.XmpMeta.GetArrayItem(dcNs, "subject", i)
                    If it IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(it.Value) Then
                        metadata.Keywords.Add(it.Value)
                    End If
                Next

                ' lr:hierarchicalSubject (Lightroom) – HIERARCHIE AUFTEILEN STATT PFAD SPEICHERN
                Dim lrNs As String = "http://ns.adobe.com/lightroom/1.0/"
                Dim hCount As Integer = xmpDirectory.XmpMeta.CountArrayItems(lrNs, "hierarchicalSubject")
                For i As Integer = 1 To hCount
                    Dim it = xmpDirectory.XmpMeta.GetArrayItem(lrNs, "hierarchicalSubject", i)
                    If it IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(it.Value) Then
                        ' z.B. "Orte|Europa|Deutschland" -> "Orte", "Europa", "Deutschland"
                        For Each part In it.Value.Split("|"c)
                            Dim seg As String = If(part, String.Empty).Trim()
                            If seg.Length > 0 Then metadata.Keywords.Add(seg)
                        Next
                    End If
                Next
            End If

            ' Deduplizieren & sortieren
            ' --- Keywords normalisieren, deduplizieren, sortieren ---
            If metadata.Keywords IsNot Nothing AndAlso metadata.Keywords.Count > 0 Then
                Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Dim cleaned As New List(Of String)

                For Each raw In metadata.Keywords
                    Dim k As String = NormalizeKeyword(raw)
                    If k.Length > 0 AndAlso seen.Add(k) Then
                        cleaned.Add(k)
                    End If
                Next

                metadata.Keywords = cleaned.
                    OrderBy(Function(s) s, StringComparer.OrdinalIgnoreCase).
                    ToList()
            End If


            ' --- Kamera / EXIF ---
            ' FIX: Kameramodell aus ExifIfd0Directory, NICHT aus ExifSubIfdDirectory
            If exifIfd0Directory IsNot Nothing Then
                Dim modelDesc = exifIfd0Directory.GetDescription(ExifDirectoryBase.TagModel)
                If Not String.IsNullOrWhiteSpace(modelDesc) Then metadata.CameraModel = modelDesc
            End If

            If exifSubIfdDirectory IsNot Nothing Then
                ' Datum – mit Fallbacks
                Dim dt As DateTime
                If exifSubIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, dt) Then
                    metadata.CreatedDate = dt
                ElseIf exifSubIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeDigitized, dt) Then
                    metadata.CreatedDate = dt
                End If

                metadata.LensModel = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagLensModel)
                metadata.ExposureTime = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagExposureTime)
                metadata.FNumber = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagFNumber)

                Dim isoVal As Integer
                If exifSubIfdDirectory.TryGetInt32(ExifDirectoryBase.TagIsoEquivalent, isoVal) Then
                    metadata.ISO = isoVal
                End If

                metadata.FocalLength = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagFocalLength)
            End If

            ' --- GPS ---
            If gpsDirectory IsNot Nothing AndAlso gpsDirectory.GetGeoLocation() IsNot Nothing Then
                metadata.geographicLatitude = gpsDirectory.GetGeoLocation().Latitude
                metadata.geographicLongitude = gpsDirectory.GetGeoLocation().Longitude
            End If

        Catch ex As Exception
            ' optional: Logging
        End Try

        Return metadata
    End Function

    Private Shared Function NormalizeKeyword(input As String) As String
        If String.IsNullOrWhiteSpace(input) Then Return String.Empty

        ' Trim, Mehrfach-Spaces zu einem Space, Unicode vereinheitlichen (NFC)
        Dim s As String = input.Trim()
        s = System.Text.RegularExpressions.Regex.Replace(s, "\s+", " ")
        s = s.Normalize(NormalizationForm.FormC)

        Return s
    End Function

End Class
