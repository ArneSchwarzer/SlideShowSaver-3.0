Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports SlideShowTools.MataDataHandling
Imports SlideShowLogging
Imports SlideShowLogging.LogHandling
Imports SlideShowTools.KeyAndMouseHandling

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

        Dim metadaten As Metadata
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

            metadaten = ExtractMetadataFromImage(bildPfad)

            'Dateiname und Pfad
            lblDateiname.Text = Path.GetFileNameWithoutExtension(bildPfad)
            lblDateipfad.Text = bildPfad
            lblBewertung.Visible = False
            slbBewertung.Visible = True

            'Erstellungsdatum
            If Not String.IsNullOrEmpty(metadaten.CreatedDate.ToString) Then
                lblErstellungsdatum.Text = metadaten.CreatedDate.ToString("dd.MM.yyyy HH:mm:ss")
            Else
                lblErstellungsdatum.Text = "-"
            End If

            'Blende
            If Not String.IsNullOrEmpty(metadaten.FNumber) Then
                lblBlende.Text = metadaten.FNumber
            Else
                lblBlende.Text = "-"
            End If

            'Verschlusszeit
            If Not String.IsNullOrEmpty(metadaten.ExposureTime) Then
                lblVerschlusszeit.Text = metadaten.ExposureTime
            Else
                lblVerschlusszeit.Text = "-"
            End If

            'ISO
            If metadaten.ISO > 0 Then
                lblISO.Text = metadaten.ISO.ToString()
            Else
                lblISO.Text = "-"
            End If

            'Brennweite
            If Not String.IsNullOrEmpty(metadaten.FocalLength) Then
                lblBrennweite.Text = metadaten.FocalLength
            Else
                lblBrennweite.Text = "-"
            End If

            'Kamera (Modell)
            If Not String.IsNullOrEmpty(metadaten.CameraModel) Then
                lblKamera.Text = metadaten.CameraModel
            Else
                lblKamera.Text = "-"
            End If

            'Objektiv (sofern vorhanden)
            If Not String.IsNullOrEmpty(metadaten.LensModel) Then
                lblObjektiv.Text = metadaten.LensModel
            Else
                lblObjektiv.Text = "-"
            End If

            'Aurtor
            If Not String.IsNullOrEmpty(metadaten.Author) Then
                Dim worte = metadaten.Author.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)

                ' Wir suchen das Muster A B A B (oder mehrfache Wiederholung)
                If worte.Length >= 2 AndAlso worte.Length Mod 2 = 0 Then
                    Dim halb = worte.Length \ 2
                    Dim ersterTeil = String.Join(" ", worte.Take(halb))
                    Dim zweiterTeil = String.Join(" ", worte.Skip(halb))
                    If ersterTeil = zweiterTeil Then
                        lblAutor.Text = ersterTeil
                    Else
                        lblAutor.Text = metadaten.Author ' Keine Wiederholung → zeige Original
                    End If
                Else
                    lblAutor.Text = metadaten.Author ' ungerade Anzahl → keine Dopplung möglich
                End If
            Else
                lblAutor.Text = "-"
            End If

            'Tags
            If metadaten.Keywords IsNot Nothing AndAlso metadaten.Keywords.Count > 0 Then
                lblTags.Text = String.Join(" | ", metadaten.Keywords)
            Else
                lblTags.Text = "-"
            End If

            'Bewertung
            If metadaten.Rating >= 0 Then
                slbBewertung.Bewertung = metadaten.Rating
            Else
                slbBewertung.Bewertung = 0
            End If
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