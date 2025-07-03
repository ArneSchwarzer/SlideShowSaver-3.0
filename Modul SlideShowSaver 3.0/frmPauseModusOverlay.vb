Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowTools.CursorHandling
Imports SlideShowBildauswahl.BildauswahlMain
Imports TagLib

Public Class frmPauseModusOverlay

    'Variablendeklaration
    Private anzeigeListe As New List(Of String)
    Private indexListe As Integer
    Private bildPfad As String
    Private bild As Image
    Private meineInstanz As ModulMain = TryCast(ModulMain.activeModuleInstanz, ModulMain)
    Private pauseInfoScreen As frmPictureInfo = Nothing

    'Hier ggf. eine Variable um die Liste "Markierte Fotos.xml" zwischenzuspeichern und manipulieren zu können.

    Private Sub frmPauseModusOverlay_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim tagLibFile As TagLib.Jpeg.File
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

        If meineInstanz IsNot Nothing Then
            anzeigeListe = meineInstanz.sssScreen.listeDerZuletztAngezeigtenBilder
        End If

        indexListe = anzeigeListe.Count
        bildPfad = anzeigeListe(indexListe - 1)

        'Hier die Liste „Markierte Fotos.xml“ aus dem Unterverzeichnis „/SlideShowSaver 3.0/Module/SlideShowSaver“
        'des Dokumenten-Ordners des Benutzers laden. Prüfen, ob "bildPfad" in der Liste vorkommt, falls ja
        'chkPauseMarkPicture auf checked setzten, sonst auf unchecked.

        lblPauseAnzahl.Text = "Bild " & indexListe & " von " & anzeigeListe.Count
        lblOptionsDialogDisabled.Visible = False

        tagLibFile = TagLib.File.Create(bildPfad)
        sbcBewerten.Bewertung = tagLibFile.ImageTag.Rating
        tagLibFile.Dispose()

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

        'Hier die geladene Liste „Markierte Fotos.xml“ aus dem Unterverzeichnis „/SlideShowSaver 3.0/Module/SlideShowSaver“
        'des Dokumenten-Ordners des Benutzers wieder speichern.

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
            taglibFile = TagLib.File.Create(bildPfad)
            taglibFile.ImageTag.Rating = sbcBewerten.Bewertung
            taglibFile.Dispose()
        End If

    End Sub

    Private Sub chkPauseMarkPicture_CheckStateChanged(sender As Object, e As EventArgs) Handles chkPauseMarkPicture.CheckStateChanged

        'Hier die Werte in der Liste "Markierte Fotos.xml" anpassen:
        '
        'Wenn ein Bild in der Liste ist und der Checked-Status ist auf "unmarkiert" gesetzt worden, dann das Bild aus Liste löschen.
        'Wenn ein Bild nicht in der Liste ist und der Checked-Statur ist auf "markiert" gesetzt worden, dann das Bild in die Liste eintragen.

    End Sub

    Private Sub btnPauseBack_Click(sender As Object, e As EventArgs) Handles btnPauseBack.Click

        indexListe -= 1

        If indexListe <= 1 Then
            indexListe = 1
            btnPauseBack.BackgroundImage = My.Resources.Zurück_grau_Transparent
            btnPauseBack.BackgroundImageLayout = ImageLayout.Zoom
            btnPauseBack.Enabled = False
        End If

        lblPauseAnzahl.Text = "Bild " & indexListe & " von " & anzeigeListe.Count
        btnPauseForward.BackgroundImage = My.Resources.Vor_Transparent
        btnPauseForward.BackgroundImageLayout = ImageLayout.Zoom
        btnPauseForward.Enabled = True

        bildPfad = anzeigeListe(indexListe - 1)

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

        lblPauseAnzahl.Text = "Bild " & indexListe & " von " & anzeigeListe.Count
        btnPauseBack.BackgroundImage = My.Resources.Zurück_Transparent
        btnPauseBack.BackgroundImageLayout = ImageLayout.Zoom
        btnPauseBack.Enabled = True

        bildPfad = anzeigeListe(indexListe - 1)

        If meineInstanz IsNot Nothing Then
            bild = GetPictureByName(bildPfad)
            meineInstanz.sssScreen.picBildAnzeige.Image = bild
            meineInstanz.sssScreen.Refresh()
        End If

        pauseInfoScreen.RefreshLabels(bildPfad)
        pauseInfoScreen.Refresh()

    End Sub

End Class