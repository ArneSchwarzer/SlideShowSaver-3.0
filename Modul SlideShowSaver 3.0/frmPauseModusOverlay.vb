Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools.CursorHandling
Imports SlideShowTools.XmlHandling
Imports SlideShowBildauswahl.BildauswahlMain
Imports TagLib
Imports System.IO
Imports SlideShowTools
Imports SlideShowLogging

Public Class frmPauseModusOverlay

    'Variablendeklaration
    Private anzeigeListe As New List(Of String)
    Private indexListe As Integer
    Private bildPfad As String
    Private bild As Image
    Private meineInstanz As ModulMain = TryCast(ModulMain.activeModuleInstanz, ModulMain)
    Private pauseInfoScreen As frmPictureInfo = Nothing
    Private tagLibFile As TagLib.Jpeg.File
    Private xmlPfad As String = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SlideShowSaver 3.0\Module\SlideShowSaver 3.0\Markierte Fotos.xml"
            )
    Private markierteFotos As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

    Private Sub frmPauseModusOverlay_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim screen As Screen = Screen.FromControl(Me)

        Me.Top = 0
        Me.Left = (screen.Bounds.Width - Me.Width) \ 2
        Me.Size = New Size(Me.Size.Width, Me.Size.Height - 82)

        'Eigene sssInfo aufrufen, um die komplette Herrschaft zu erlangen.
        If meineInstanz.sssInfo IsNot Nothing Then
            meineInstanz.sssInfo.Close()
            meineInstanz.sssInfo.Dispose()
            meineInstanz.sssInfo = Nothing
        End If

        If pauseInfoScreen Is Nothing Then
            pauseInfoScreen = New frmPictureInfo
            pauseInfoScreen.Show()
            pauseInfoScreen.BringToFront()
        End If

        Me.TopMost = True
        Me.KeyPreview = True
        Me.Focus()
        Me.Activate()
        Me.BringToFront()

        'Weil Cursor.Hide() ein Stack ist und ich ein Feigling bin...
        CursorPowerShow()

        'Listen und Variablen setzen
        If meineInstanz IsNot Nothing Then
            anzeigeListe = meineInstanz.sssScreen.listeDerZuletztAngezeigtenBilder
        End If

        indexListe = anzeigeListe.Count
        bildPfad = anzeigeListe(indexListe - 1)

        markierteFotos = New HashSet(Of String)(XmlHandling.LadeWerteliste(xmlPfad, "Foto", "Pfad"))

        'ChkPauseMarkPicture setzen
        If markierteFotos.Contains(bildPfad) Then
            chkPauseMarkPicture.Checked = True
        Else
            chkPauseMarkPicture.Checked = False
        End If

        'Labels anpassen
        lblPauseAnzahl.Text = "Bild " & indexListe & " von " & anzeigeListe.Count
        lblOptionsDialogDisabled.Visible = False

        'Bei TagLib-Dateien immer ein bischen vorsichtig sein.
        Try
            tagLibFile = TagLib.File.Create(bildPfad)
            sbcBewerten.Bewertung = tagLibFile.ImageTag.Rating
            tagLibFile.Dispose()
        Catch ex As Exception
            LogHandling.LogError("SlideShowSaver 3.0\PauseOverlay: Fehler beim Erstellen von tagLibFile: " & ex.ToString)
        End Try

        'Steuerelemente & Timer einrichten
        sbcBewerten.Visible = False
        chkBewerten.Checked = False

        btnPauseForward.BackgroundImage = My.Resources.Vor_grau_Transparent
        btnPauseForward.BackgroundImageLayout = ImageLayout.Zoom
        btnPauseForward.Enabled = False

        tmrWarnLabelAnzeige.Stop()

        'Hier ggf. Transition stoppen, falls "Running". Aber erst, sobald die ersten Transitionen implementiert sind

        If meineInstanz IsNot Nothing Then
            meineInstanz.sssScreen.tmrModul.Stop()

            bild = GetPictureByName(bildPfad)
            meineInstanz.sssScreen.picBildAnzeige.Image = bild
            meineInstanz.sssScreen.picBildAnzeige.Refresh()
        End If

        pauseInfoScreen.RefreshLabels(bildPfad)
        pauseInfoScreen.Refresh()

    End Sub

    Private Sub btnPausePause_Click(sender As Object, e As EventArgs) Handles btnPausePause.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub frmPauseModusOverlay_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If e.KeyCode = Keys.Escape Then
            If Not lblOptionsDialogDisabled.Visible Then
                Me.Size = New Size(Me.Size.Width, Me.Size.Height + 82)
            End If
            lblOptionsDialogDisabled.Visible = True
            tmrWarnLabelAnzeige.Start()
        ElseIf e.KeyCode = Keys.Left Then
            btnPauseBack.PerformClick()
        ElseIf e.KeyCode = Keys.Right Then
            btnPauseForward.PerformClick()
        ElseIf e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.M Then
            If chkPauseMarkPicture.Checked = True Then
                chkPauseMarkPicture.Checked = False
            Else
                chkPauseMarkPicture.Checked = True
            End If
        ElseIf e.KeyCode = Keys.P Then
            Me.Close()
        End If

    End Sub

    Private Sub frmPauseModusOverlay_Closed(sender As Object, e As EventArgs) Handles Me.Closed

        'Liste der markierten Fotos wieder zurückspeichern
        XmlHandling.SpeichereWerteliste(xmlPfad, markierteFotos, "Markierungen", "Foto", "Pfad")

        If pauseInfoScreen IsNot Nothing Then
            pauseInfoScreen.Close()
            pauseInfoScreen.Dispose()
            pauseInfoScreen = Nothing
        End If

        Cursor.Hide()

    End Sub

    Private Sub frmPauseModusOverlay_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown

        If e.Button = MouseButtons.Right Then
            If Not lblOptionsDialogDisabled.Visible Then
                Me.Size = New Size(Me.Size.Width, Me.Size.Height + 82)
            End If
            lblOptionsDialogDisabled.Visible = True
            tmrWarnLabelAnzeige.Start()
        End If
    End Sub

    Private Sub tmrWarnLabelAnzeige_Tick(sender As Object, e As EventArgs) Handles tmrWarnLabelAnzeige.Tick
        lblOptionsDialogDisabled.Visible = False
        Me.Size = New Size(Me.Size.Width, Me.Size.Height - 82)
        tmrWarnLabelAnzeige.Stop()
    End Sub

    Private Sub chkBewerten_CheckStateChanged(sender As Object, e As EventArgs) Handles chkBewerten.CheckStateChanged
        If chkBewerten.Checked = True Then
            sbcBewerten.Visible = True
        Else
            sbcBewerten.Visible = False
        End If
    End Sub

    Private Sub sbcBewerten_BewertungGeaendert(sender As Object, neueBewertung As Integer) Handles sbcBewerten.BewertungGeaendert
        Dim taglibFile As TagLib.Jpeg.File

        If Not String.IsNullOrEmpty(bildPfad) Then
            Try
                ' Bild aus der PictureBox entfernen
                If meineInstanz IsNot Nothing AndAlso meineInstanz.sssScreen.picBildAnzeige.Image IsNot Nothing Then
                    meineInstanz.sssScreen.picBildAnzeige.Image.Dispose()
                    meineInstanz.sssScreen.picBildAnzeige.Image = Nothing
                End If

                ' Tag schreiben
                taglibFile = TagLib.File.Create(bildPfad)
                taglibFile.ImageTag.Rating = sbcBewerten.Bewertung
                taglibFile.Save()
                taglibFile.Dispose()

                ' Bild wieder neu einladen (nach dem Speichern)
                bild = GetPictureByName(bildPfad)
                meineInstanz.sssScreen.picBildAnzeige.Image = bild
                meineInstanz.sssScreen.Refresh()

            Catch ex As Exception
                LogHandling.LogError("SlideShowSaver 3.0\PauseOverlay: Fehler beim Setzen der Bewertung für Bild " & bildPfad & ": " & ex.ToString)
            End Try
        End If

    End Sub


    Private Sub chkPauseMarkPicture_CheckStateChanged(sender As Object, e As EventArgs) Handles chkPauseMarkPicture.CheckStateChanged

        'Hier die Werte in der Liste "Markierte Fotos.xml" anpassen
        If chkPauseMarkPicture.CheckState Then
            markierteFotos.Add(bildPfad)
        Else
            markierteFotos.Remove(bildPfad)
        End If

    End Sub

    Private Sub btnPauseBack_Click(sender As Object, e As EventArgs) Handles btnPauseBack.Click

        indexListe -= 1

        If indexListe <= 1 Then
            indexListe = 1
            btnPauseBack.BackgroundImage = My.Resources.Zurück_grau_Transparent
            btnPauseBack.BackgroundImageLayout = ImageLayout.Zoom
            btnPauseBack.Enabled = False
        End If

        'Bei TagLib-Dateien immer ein bischen vorsichtig sein.
        Try
            tagLibFile = TagLib.File.Create(bildPfad)
            sbcBewerten.Bewertung = tagLibFile.ImageTag.Rating
            tagLibFile.Dispose()
        Catch ex As Exception
            LogHandling.LogError("SlideShowSaver 3.0\PauseOverlay: Fehler beim Erstellen von tagLibFile: " & ex.ToString)
        End Try

        'Zum Schutz vor unabsichtlichem Ändern der Bewertung eines Bildes
        chkBewerten.Checked = False
        sbcBewerten.Visible = False

        'Restliche Controls anpassen
        lblPauseAnzahl.Text = "Bild " & indexListe & " von " & anzeigeListe.Count
        btnPauseForward.BackgroundImage = My.Resources.Vor_Transparent
        btnPauseForward.BackgroundImageLayout = ImageLayout.Zoom
        btnPauseForward.Enabled = True

        bildPfad = anzeigeListe(indexListe - 1)

        'Markiert Status setzen
        If markierteFotos.Contains(bildPfad) Then
            chkPauseMarkPicture.Checked = True
        Else
            chkPauseMarkPicture.Checked = False
        End If

        'Bild anzeigen
        If meineInstanz IsNot Nothing Then
            bild = GetPictureByName(bildPfad)
            meineInstanz.sssScreen.picBildAnzeige.Image = bild
            meineInstanz.sssScreen.Refresh()
        End If

        pauseInfoScreen.RefreshLabels(bildPfad)
        pauseInfoScreen.Refresh()

    End Sub

    Private Sub btnPauseForward_Click(sender As Object, e As EventArgs) Handles btnPauseForward.Click

        indexListe += 1

        If indexListe >= anzeigeListe.Count Then
            indexListe = anzeigeListe.Count
            btnPauseForward.BackgroundImage = My.Resources.Vor_grau_Transparent
            btnPauseForward.BackgroundImageLayout = ImageLayout.Zoom
            btnPauseForward.Enabled = False
        End If

        'Bei TagLib-Dateien immer ein bischen vorsichtig sein.
        Try
            tagLibFile = TagLib.File.Create(bildPfad)
            sbcBewerten.Bewertung = tagLibFile.ImageTag.Rating
            tagLibFile.Dispose()
        Catch ex As Exception
            LogHandling.LogError("SlideShowSaver 3.0\PauseOverlay: Fehler beim Erstellen von tagLibFile: " & ex.ToString)
        End Try

        'Zum Schutz vor unabsichtlichem Ändern der Bewertung eines Bildes
        chkBewerten.Checked = False
        sbcBewerten.Visible = False

        'Restliche Controls anpassen
        lblPauseAnzahl.Text = "Bild " & indexListe & " von " & anzeigeListe.Count
        btnPauseBack.BackgroundImage = My.Resources.Zurück_Transparent
        btnPauseBack.BackgroundImageLayout = ImageLayout.Zoom
        btnPauseBack.Enabled = True

        bildPfad = anzeigeListe(indexListe - 1)

        'Markiert Status setzen
        If markierteFotos.Contains(bildPfad) Then
            chkPauseMarkPicture.Checked = True
        Else
            chkPauseMarkPicture.Checked = False
        End If

        'Bild anzeigen
        If meineInstanz IsNot Nothing Then
            bild = GetPictureByName(bildPfad)
            meineInstanz.sssScreen.picBildAnzeige.Image = bild
            meineInstanz.sssScreen.Refresh()
        End If

        pauseInfoScreen.RefreshLabels(bildPfad)
        pauseInfoScreen.Refresh()

    End Sub

End Class