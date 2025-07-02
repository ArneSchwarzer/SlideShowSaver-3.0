Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports SlideShowLogging.LogHandling
Imports TagLib
Imports SlideShowTools.ListHandling
Imports SlideShowLogging
Imports SlideShowTools.KeyAndMouseHandling
Imports MetadataExtractor
Imports MetadataExtractor.Formats.Exif
Imports MetadataExtractor.Formats.Iptc
Imports System.IO
Imports MetadataExtractor.Formats.Xmp

Public Class frmPictureInfo

    Private Sub frmPictureInfo_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim screen As Screen = Screen.FromControl(Me)
        Dim neueX As Integer = screen.Bounds.Right - Me.Width
        Dim neueY As Integer = screen.Bounds.Bottom - Me.Height

        Me.Location = New Point(neueX, neueY)

        Me.KeyPreview = True
        Me.Focus()
        Me.Activate()
        Me.BringToFront()

    End Sub

    Public Sub RefreshLabels(bildPfad As String)
        'Setzt die Labeltexte gemäß EXIF Daten, falls der Dateityp so etwas überhaupt bietet

        Dim endung As String = Path.GetExtension(bildPfad).ToLowerInvariant()

        If endung.ToLower = ".bmp" OrElse endung.ToLower = ".png" Then
            lblDateiname.Text = Path.GetFileNameWithoutExtension(bildPfad)
            lblDateipfad.Text = bildPfad
            lblBewertung.Visible = True
            slbBewertung.Visible = False
            lblErstellungsdatum.Text = "-"
            lblBlende.Text = "-"
            lblVerschlusszeit.Text = "-"
            lblISO.Text = "-"
            lblBrennweite.Text = "-"
            lblKamera.Text = "-"
            lblObjektiv.Text = "-"
            lblAutor.Text = "-"
            lblTags.Text = "-"
        Else
            Try
                Dim directories = ImageMetadataReader.ReadMetadata(bildPfad)
                Dim tagLibFile As TagLib.Jpeg.File

                tagLibFile = TagLib.File.Create(bildPfad)

                ' ========== Dateiname und Pfad ==========
                lblDateiname.Text = Path.GetFileNameWithoutExtension(bildPfad)
                lblDateipfad.Text = bildPfad
                lblBewertung.Visible = False
                slbBewertung.Visible = True
                ' ========== Erstellungsdatum, Blende, Verschlusszeit, ISO, Brennweite ==========
                Dim subIfd = directories.OfType(Of ExifSubIfdDirectory)().FirstOrDefault()
                If subIfd IsNot Nothing Then
                    lblErstellungsdatum.Text = subIfd.GetDescription(ExifDirectoryBase.TagDateTimeOriginal)
                    lblBlende.Text = subIfd.GetDescription(ExifDirectoryBase.TagFNumber)
                    lblVerschlusszeit.Text = subIfd.GetDescription(ExifDirectoryBase.TagExposureTime)
                    lblISO.Text = subIfd.GetDescription(ExifDirectoryBase.TagIsoEquivalent)
                    lblBrennweite.Text = subIfd.GetDescription(ExifDirectoryBase.TagFocalLength)
                End If

                ' ========== Kamera (Modell) ==========
                Dim ifd0 = directories.OfType(Of ExifIfd0Directory)().FirstOrDefault()
                If ifd0 IsNot Nothing Then
                    lblKamera.Text = ifd0.GetDescription(ExifDirectoryBase.TagModel)
                End If

                ' ========== Objektiv (sofern vorhanden) ==========
                Dim lens = subIfd?.GetDescription(ExifDirectoryBase.TagLensModel)
                If String.IsNullOrEmpty(lens) Then
                    lens = ifd0?.GetDescription(ExifDirectoryBase.TagLensModel)
                End If
                lblObjektiv.Text = If(lens, "unbekannt")

                ' ========== IPTC: Autor, Bewertung, Tags ==========
                Dim iptc = directories.OfType(Of IptcDirectory)().FirstOrDefault()
                If iptc IsNot Nothing Then
                    lblAutor.Text = iptc.GetDescription(IptcDirectory.TagByLine)

                    Dim rating As Integer
                    rating = tagLibFile.ImageTag.Rating
                    slbBewertung.Bewertung = rating

                    Dim keywords = iptc.GetStringArray(IptcDirectory.TagKeywords)
                    lblTags.Text = If(keywords IsNot Nothing, String.Join(" | ", keywords), "")
                End If

                ' ========== Fallback über XMP falls IPTC leer ==========
                Dim xmp = directories.OfType(Of XmpDirectory)().FirstOrDefault()
                If xmp IsNot Nothing Then
                    If String.IsNullOrEmpty(lblAutor.Text) Then
                        lblAutor.Text = xmp.XmpMeta.GetPropertyString("http://purl.org/dc/elements/1.1/", "creator")

                    End If
                End If

            Catch ex As Exception
                LogHandling.LogWarn("Die EXIF von " & bildPfad & " konnte nicht in die Form frmPictureInfo eingelesen werden: " & ex.Message)
            End Try
        End If

    End Sub

    Private Sub frmPictureInfo_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        LogDebug("SlideShowModul SSS 3.0\BildInfo hat den Key: " & e.KeyValue.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardKeyDown(Me, e)
    End Sub

    Private Sub frmPictureInfo_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        LogDebug("SlideShowModul SSS 3.0\BildInfo hat den MouseButton: " & e.Button.ToString & " empfangen. Leite Weiter an Eventhandler")
        ForwardMouseDown(Me, e)
    End Sub
End Class