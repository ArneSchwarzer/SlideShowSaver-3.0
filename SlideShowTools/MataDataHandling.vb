Imports System.IO
Imports MetadataExtractor
Imports MetadataExtractor.Formats.Exif
Imports MetadataExtractor.Formats.Iptc
Imports MetadataExtractor.Formats.Xmp

Public Class MataDataHandling

    ' This class is responsible for handling metadata extraction from images.
    ' It provides methods to extract metadata such as author, rating, keywords, and camera settings.
    ' It also includes a structure to hold the metadata information.

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
        Public goeographicLatitude As Double
        Public geographicLongitude As Double
    End Structure

    Public Shared Function ExtractMetadataFromImage(imagePath As String) As Metadata

        Dim metadata As New Metadata()

        metadata.Keywords = New List(Of String)()

        If Not File.Exists(imagePath) Then
            ' Handle the case where the file does not exist
            Return metadata
        End If

        If Not Path.GetExtension(imagePath).ToLowerInvariant().EndsWith(".jpg") AndAlso
           Not Path.GetExtension(imagePath).ToLowerInvariant().EndsWith(".jpeg") Then
            ' Handle unsupported file formats
            Return metadata
        End If

        Try
            Dim directories = ImageMetadataReader.ReadMetadata(imagePath)
            Dim iptcDirectory = directories.OfType(Of IptcDirectory)().FirstOrDefault()
            Dim xmpDirectory = directories.OfType(Of XmpDirectory)().FirstOrDefault()
            Dim exifSubIfdDirectory = directories.OfType(Of ExifSubIfdDirectory)().FirstOrDefault()
            Dim gpsDirectory = directories.OfType(Of GpsDirectory)().FirstOrDefault()

            ' Extracting author and rating
            If xmpDirectory IsNot Nothing Then
                metadata.Rating = xmpDirectory.XmpMeta?.GetPropertyString("http://ns.adobe.com/xap/1.0/", "Rating")
                metadata.Author = xmpDirectory.XmpMeta?.GetPropertyString("http://ns.adobe.com/xap/1.0/", "CreatorTool")
            End If

            ' Extracting keywords
            If iptcDirectory IsNot Nothing Then
                metadata.Keywords.AddRange(iptcDirectory.GetKeywords())
                metadata.Keywords.Sort()

                If metadata.Author = String.Empty Then
                    metadata.Author = iptcDirectory.GetDescription(IptcDirectory.TagByLine)
                End If
            End If

            ' Extracting camera settings
            If exifSubIfdDirectory IsNot Nothing Then
                metadata.CreatedDate = exifSubIfdDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal)
                metadata.CameraModel = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagModel)
                metadata.LensModel = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagLensModel)
                metadata.ExposureTime = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagExposureTime)
                metadata.FNumber = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagFNumber)
                metadata.ISO = exifSubIfdDirectory.GetInt32(ExifDirectoryBase.TagIsoEquivalent)
                metadata.FocalLength = exifSubIfdDirectory.GetDescription(ExifDirectoryBase.TagFocalLength)
            End If

            'Extracting geographic coordinates if available
            If gpsDirectory IsNot Nothing Then
                If gpsDirectory.GetGeoLocation() IsNot Nothing Then
                    metadata.goeographicLatitude = gpsDirectory.GetGeoLocation().Latitude
                    metadata.geographicLongitude = gpsDirectory.GetGeoLocation().Longitude
                End If

            End If

        Catch ex As Exception
            ' Handle exceptions (e.g., file not found, unsupported format, etc.)
        End Try
        Return metadata
    End Function
End Class
