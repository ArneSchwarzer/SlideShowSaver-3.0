<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPictureInfo
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.grpDatei = New System.Windows.Forms.GroupBox()
        Me.slbBewertung = New MyControlsLibrary.SterneAnzeigeLabel()
        Me.lblBewertung = New System.Windows.Forms.Label()
        Me.LblNBewertung = New System.Windows.Forms.Label()
        Me.lblTags = New System.Windows.Forms.Label()
        Me.lblAutor = New System.Windows.Forms.Label()
        Me.lblErstellungsdatum = New System.Windows.Forms.Label()
        Me.lblDateipfad = New System.Windows.Forms.Label()
        Me.lblDateiname = New System.Windows.Forms.Label()
        Me.lblNTags = New System.Windows.Forms.Label()
        Me.lblNAutor = New System.Windows.Forms.Label()
        Me.lblNErstellungsdatum = New System.Windows.Forms.Label()
        Me.lblNDateipfad = New System.Windows.Forms.Label()
        Me.lblNDateiname = New System.Windows.Forms.Label()
        Me.lblNDatei = New System.Windows.Forms.Label()
        Me.grpKamera = New System.Windows.Forms.GroupBox()
        Me.lblBlende = New System.Windows.Forms.Label()
        Me.lblISO = New System.Windows.Forms.Label()
        Me.lblVerschlusszeit = New System.Windows.Forms.Label()
        Me.lblObjektiv = New System.Windows.Forms.Label()
        Me.lblBrennweite = New System.Windows.Forms.Label()
        Me.lblKamera = New System.Windows.Forms.Label()
        Me.lblNBlende = New System.Windows.Forms.Label()
        Me.lblNISO = New System.Windows.Forms.Label()
        Me.lblNVerschlusszeit = New System.Windows.Forms.Label()
        Me.lblNObjektiv = New System.Windows.Forms.Label()
        Me.lblNBrennweite = New System.Windows.Forms.Label()
        Me.lblNlblKamera = New System.Windows.Forms.Label()
        Me.lblNKamera = New System.Windows.Forms.Label()
        Me.lblNBildinformationen = New System.Windows.Forms.Label()
        Me.grpDatei.SuspendLayout()
        Me.grpKamera.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpDatei
        '
        Me.grpDatei.BackColor = System.Drawing.Color.Transparent
        Me.grpDatei.Controls.Add(Me.slbBewertung)
        Me.grpDatei.Controls.Add(Me.lblBewertung)
        Me.grpDatei.Controls.Add(Me.LblNBewertung)
        Me.grpDatei.Controls.Add(Me.lblTags)
        Me.grpDatei.Controls.Add(Me.lblAutor)
        Me.grpDatei.Controls.Add(Me.lblErstellungsdatum)
        Me.grpDatei.Controls.Add(Me.lblDateipfad)
        Me.grpDatei.Controls.Add(Me.lblDateiname)
        Me.grpDatei.Controls.Add(Me.lblNTags)
        Me.grpDatei.Controls.Add(Me.lblNAutor)
        Me.grpDatei.Controls.Add(Me.lblNErstellungsdatum)
        Me.grpDatei.Controls.Add(Me.lblNDateipfad)
        Me.grpDatei.Controls.Add(Me.lblNDateiname)
        Me.grpDatei.Location = New System.Drawing.Point(13, 136)
        Me.grpDatei.Name = "grpDatei"
        Me.grpDatei.Size = New System.Drawing.Size(903, 538)
        Me.grpDatei.TabIndex = 1
        Me.grpDatei.TabStop = False
        '
        'slbBewertung
        '
        Me.slbBewertung.BackColor = System.Drawing.Color.Black
        Me.slbBewertung.Location = New System.Drawing.Point(293, 315)
        Me.slbBewertung.Name = "slbBewertung"
        Me.slbBewertung.Size = New System.Drawing.Size(221, 45)
        Me.slbBewertung.TabIndex = 14
        Me.slbBewertung.Text = "SterneAnzeigeLabel1"
        '
        'lblBewertung
        '
        Me.lblBewertung.AutoSize = True
        Me.lblBewertung.Location = New System.Drawing.Point(286, 315)
        Me.lblBewertung.Name = "lblBewertung"
        Me.lblBewertung.Size = New System.Drawing.Size(30, 41)
        Me.lblBewertung.TabIndex = 13
        Me.lblBewertung.Text = "-"
        Me.lblBewertung.Visible = False
        '
        'LblNBewertung
        '
        Me.LblNBewertung.AutoSize = True
        Me.LblNBewertung.Location = New System.Drawing.Point(17, 315)
        Me.LblNBewertung.Name = "LblNBewertung"
        Me.LblNBewertung.Size = New System.Drawing.Size(161, 41)
        Me.LblNBewertung.TabIndex = 10
        Me.LblNBewertung.Tag = "langKey=LblNBewertung"
        Me.LblNBewertung.Text = "Bewertung"
        '
        'lblTags
        '
        Me.lblTags.AutoSize = True
        Me.lblTags.Location = New System.Drawing.Point(286, 411)
        Me.lblTags.MaximumSize = New System.Drawing.Size(580, 123)
        Me.lblTags.Name = "lblTags"
        Me.lblTags.Size = New System.Drawing.Size(243, 41)
        Me.lblTags.TabIndex = 9
        Me.lblTags.Text = "Verwendete Tags"
        '
        'lblAutor
        '
        Me.lblAutor.AutoSize = True
        Me.lblAutor.Location = New System.Drawing.Point(286, 363)
        Me.lblAutor.MaximumSize = New System.Drawing.Size(580, 123)
        Me.lblAutor.Name = "lblAutor"
        Me.lblAutor.Size = New System.Drawing.Size(92, 41)
        Me.lblAutor.TabIndex = 8
        Me.lblAutor.Text = "Autor"
        '
        'lblErstellungsdatum
        '
        Me.lblErstellungsdatum.AutoSize = True
        Me.lblErstellungsdatum.Location = New System.Drawing.Point(286, 267)
        Me.lblErstellungsdatum.MaximumSize = New System.Drawing.Size(580, 123)
        Me.lblErstellungsdatum.Name = "lblErstellungsdatum"
        Me.lblErstellungsdatum.Size = New System.Drawing.Size(247, 41)
        Me.lblErstellungsdatum.TabIndex = 7
        Me.lblErstellungsdatum.Text = "Erstellungsdatum"
        '
        'lblDateipfad
        '
        Me.lblDateipfad.AutoSize = True
        Me.lblDateipfad.Location = New System.Drawing.Point(286, 92)
        Me.lblDateipfad.MaximumSize = New System.Drawing.Size(580, 205)
        Me.lblDateipfad.Name = "lblDateipfad"
        Me.lblDateipfad.Size = New System.Drawing.Size(147, 41)
        Me.lblDateipfad.TabIndex = 6
        Me.lblDateipfad.Text = "Dateipfad"
        '
        'lblDateiname
        '
        Me.lblDateiname.AutoSize = True
        Me.lblDateiname.Location = New System.Drawing.Point(286, 34)
        Me.lblDateiname.Name = "lblDateiname"
        Me.lblDateiname.Size = New System.Drawing.Size(161, 41)
        Me.lblDateiname.TabIndex = 5
        Me.lblDateiname.Text = "Dateiname"
        '
        'lblNTags
        '
        Me.lblNTags.AutoSize = True
        Me.lblNTags.Location = New System.Drawing.Point(17, 411)
        Me.lblNTags.Name = "lblNTags"
        Me.lblNTags.Size = New System.Drawing.Size(243, 41)
        Me.lblNTags.TabIndex = 4
        Me.lblNTags.Tag = "langKey=lblNTags"
        Me.lblNTags.Text = "Verwendete Tags"
        '
        'lblNAutor
        '
        Me.lblNAutor.AutoSize = True
        Me.lblNAutor.Location = New System.Drawing.Point(17, 363)
        Me.lblNAutor.Name = "lblNAutor"
        Me.lblNAutor.Size = New System.Drawing.Size(92, 41)
        Me.lblNAutor.TabIndex = 3
        Me.lblNAutor.Tag = "langKey=lblNAutor"
        Me.lblNAutor.Text = "Autor"
        '
        'lblNErstellungsdatum
        '
        Me.lblNErstellungsdatum.AutoSize = True
        Me.lblNErstellungsdatum.Location = New System.Drawing.Point(17, 267)
        Me.lblNErstellungsdatum.Name = "lblNErstellungsdatum"
        Me.lblNErstellungsdatum.Size = New System.Drawing.Size(247, 41)
        Me.lblNErstellungsdatum.TabIndex = 2
        Me.lblNErstellungsdatum.Tag = "langKey=lblNErstellungsdatum"
        Me.lblNErstellungsdatum.Text = "Erstellungsdatum"
        '
        'lblNDateipfad
        '
        Me.lblNDateipfad.AutoSize = True
        Me.lblNDateipfad.Location = New System.Drawing.Point(17, 92)
        Me.lblNDateipfad.Name = "lblNDateipfad"
        Me.lblNDateipfad.Size = New System.Drawing.Size(147, 41)
        Me.lblNDateipfad.TabIndex = 1
        Me.lblNDateipfad.Tag = "langKey=lblNDateipfad"
        Me.lblNDateipfad.Text = "Dateipfad"
        '
        'lblNDateiname
        '
        Me.lblNDateiname.AutoSize = True
        Me.lblNDateiname.Location = New System.Drawing.Point(17, 34)
        Me.lblNDateiname.Name = "lblNDateiname"
        Me.lblNDateiname.Size = New System.Drawing.Size(161, 41)
        Me.lblNDateiname.TabIndex = 0
        Me.lblNDateiname.Tag = "langKey=lblNDateiname"
        Me.lblNDateiname.Text = "Dateiname"
        '
        'lblNDatei
        '
        Me.lblNDatei.AutoSize = True
        Me.lblNDatei.Location = New System.Drawing.Point(12, 101)
        Me.lblNDatei.Name = "lblNDatei"
        Me.lblNDatei.Size = New System.Drawing.Size(87, 41)
        Me.lblNDatei.TabIndex = 2
        Me.lblNDatei.Tag = "langKey=lblNDatei"
        Me.lblNDatei.Text = "Datei"
        '
        'grpKamera
        '
        Me.grpKamera.BackColor = System.Drawing.Color.Transparent
        Me.grpKamera.Controls.Add(Me.lblBlende)
        Me.grpKamera.Controls.Add(Me.lblISO)
        Me.grpKamera.Controls.Add(Me.lblVerschlusszeit)
        Me.grpKamera.Controls.Add(Me.lblObjektiv)
        Me.grpKamera.Controls.Add(Me.lblBrennweite)
        Me.grpKamera.Controls.Add(Me.lblKamera)
        Me.grpKamera.Controls.Add(Me.lblNBlende)
        Me.grpKamera.Controls.Add(Me.lblNISO)
        Me.grpKamera.Controls.Add(Me.lblNVerschlusszeit)
        Me.grpKamera.Controls.Add(Me.lblNObjektiv)
        Me.grpKamera.Controls.Add(Me.lblNBrennweite)
        Me.grpKamera.Controls.Add(Me.lblNlblKamera)
        Me.grpKamera.Location = New System.Drawing.Point(949, 136)
        Me.grpKamera.Name = "grpKamera"
        Me.grpKamera.Size = New System.Drawing.Size(485, 538)
        Me.grpKamera.TabIndex = 3
        Me.grpKamera.TabStop = False
        '
        'lblBlende
        '
        Me.lblBlende.AutoSize = True
        Me.lblBlende.Location = New System.Drawing.Point(215, 311)
        Me.lblBlende.Name = "lblBlende"
        Me.lblBlende.Size = New System.Drawing.Size(109, 41)
        Me.lblBlende.TabIndex = 12
        Me.lblBlende.Text = "Blende"
        '
        'lblISO
        '
        Me.lblISO.AutoSize = True
        Me.lblISO.Location = New System.Drawing.Point(215, 407)
        Me.lblISO.Name = "lblISO"
        Me.lblISO.Size = New System.Drawing.Size(65, 41)
        Me.lblISO.TabIndex = 11
        Me.lblISO.Text = "ISO"
        '
        'lblVerschlusszeit
        '
        Me.lblVerschlusszeit.AutoSize = True
        Me.lblVerschlusszeit.Location = New System.Drawing.Point(215, 359)
        Me.lblVerschlusszeit.Name = "lblVerschlusszeit"
        Me.lblVerschlusszeit.Size = New System.Drawing.Size(202, 41)
        Me.lblVerschlusszeit.TabIndex = 10
        Me.lblVerschlusszeit.Text = "Verschlusszeit"
        '
        'lblObjektiv
        '
        Me.lblObjektiv.AutoSize = True
        Me.lblObjektiv.Location = New System.Drawing.Point(215, 175)
        Me.lblObjektiv.MaximumSize = New System.Drawing.Size(245, 82)
        Me.lblObjektiv.Name = "lblObjektiv"
        Me.lblObjektiv.Size = New System.Drawing.Size(128, 41)
        Me.lblObjektiv.TabIndex = 9
        Me.lblObjektiv.Text = "Objektiv"
        '
        'lblBrennweite
        '
        Me.lblBrennweite.AutoSize = True
        Me.lblBrennweite.Location = New System.Drawing.Point(215, 255)
        Me.lblBrennweite.Name = "lblBrennweite"
        Me.lblBrennweite.Size = New System.Drawing.Size(166, 41)
        Me.lblBrennweite.TabIndex = 8
        Me.lblBrennweite.Text = "Brennweite"
        '
        'lblKamera
        '
        Me.lblKamera.AutoSize = True
        Me.lblKamera.Location = New System.Drawing.Point(215, 34)
        Me.lblKamera.MaximumSize = New System.Drawing.Size(245, 0)
        Me.lblKamera.Name = "lblKamera"
        Me.lblKamera.Size = New System.Drawing.Size(117, 41)
        Me.lblKamera.TabIndex = 7
        Me.lblKamera.Text = "Kamera"
        '
        'lblNBlende
        '
        Me.lblNBlende.AutoSize = True
        Me.lblNBlende.Location = New System.Drawing.Point(7, 311)
        Me.lblNBlende.Name = "lblNBlende"
        Me.lblNBlende.Size = New System.Drawing.Size(109, 41)
        Me.lblNBlende.TabIndex = 6
        Me.lblNBlende.Tag = "langKey=lblNBlende"
        Me.lblNBlende.Text = "Blende"
        '
        'lblNISO
        '
        Me.lblNISO.AutoSize = True
        Me.lblNISO.Location = New System.Drawing.Point(7, 407)
        Me.lblNISO.Name = "lblNISO"
        Me.lblNISO.Size = New System.Drawing.Size(65, 41)
        Me.lblNISO.TabIndex = 4
        Me.lblNISO.Tag = "langKey=lblNISO"
        Me.lblNISO.Text = "ISO"
        '
        'lblNVerschlusszeit
        '
        Me.lblNVerschlusszeit.AutoSize = True
        Me.lblNVerschlusszeit.Location = New System.Drawing.Point(7, 359)
        Me.lblNVerschlusszeit.Name = "lblNVerschlusszeit"
        Me.lblNVerschlusszeit.Size = New System.Drawing.Size(202, 41)
        Me.lblNVerschlusszeit.TabIndex = 3
        Me.lblNVerschlusszeit.Tag = "langKey=lblNVerschlusszeit"
        Me.lblNVerschlusszeit.Text = "Verschlusszeit"
        '
        'lblNObjektiv
        '
        Me.lblNObjektiv.AutoSize = True
        Me.lblNObjektiv.Location = New System.Drawing.Point(7, 175)
        Me.lblNObjektiv.Name = "lblNObjektiv"
        Me.lblNObjektiv.Size = New System.Drawing.Size(128, 41)
        Me.lblNObjektiv.TabIndex = 2
        Me.lblNObjektiv.Tag = "lblNObjektiv"
        Me.lblNObjektiv.Text = "Objektiv"
        '
        'lblNBrennweite
        '
        Me.lblNBrennweite.AutoSize = True
        Me.lblNBrennweite.Location = New System.Drawing.Point(7, 255)
        Me.lblNBrennweite.Name = "lblNBrennweite"
        Me.lblNBrennweite.Size = New System.Drawing.Size(166, 41)
        Me.lblNBrennweite.TabIndex = 1
        Me.lblNBrennweite.Tag = "lblNBrennweite"
        Me.lblNBrennweite.Text = "Brennweite"
        '
        'lblNlblKamera
        '
        Me.lblNlblKamera.AutoSize = True
        Me.lblNlblKamera.Location = New System.Drawing.Point(7, 34)
        Me.lblNlblKamera.Name = "lblNlblKamera"
        Me.lblNlblKamera.Size = New System.Drawing.Size(117, 41)
        Me.lblNlblKamera.TabIndex = 0
        Me.lblNlblKamera.Tag = "langKey=lblNlblKamera"
        Me.lblNlblKamera.Text = "Kamera"
        '
        'lblNKamera
        '
        Me.lblNKamera.AutoSize = True
        Me.lblNKamera.Location = New System.Drawing.Point(942, 101)
        Me.lblNKamera.Name = "lblNKamera"
        Me.lblNKamera.Size = New System.Drawing.Size(117, 41)
        Me.lblNKamera.TabIndex = 4
        Me.lblNKamera.Tag = "langKey=lblNKamera"
        Me.lblNKamera.Text = "Kamera"
        '
        'lblNBildinformationen
        '
        Me.lblNBildinformationen.AutoSize = True
        Me.lblNBildinformationen.Font = New System.Drawing.Font("Segoe UI", 20.0!, System.Drawing.FontStyle.Bold)
        Me.lblNBildinformationen.Location = New System.Drawing.Point(0, 10)
        Me.lblNBildinformationen.MinimumSize = New System.Drawing.Size(1458, 0)
        Me.lblNBildinformationen.Name = "lblNBildinformationen"
        Me.lblNBildinformationen.Size = New System.Drawing.Size(1458, 81)
        Me.lblNBildinformationen.TabIndex = 5
        Me.lblNBildinformationen.Tag = "lblNBildinformationen"
        Me.lblNBildinformationen.Text = "Bildinformationen"
        Me.lblNBildinformationen.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'frmPictureInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(1459, 698)
        Me.Controls.Add(Me.lblNBildinformationen)
        Me.Controls.Add(Me.lblNKamera)
        Me.Controls.Add(Me.grpKamera)
        Me.Controls.Add(Me.lblNDatei)
        Me.Controls.Add(Me.grpDatei)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.ForeColor = System.Drawing.Color.Snow
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmPictureInfo"
        Me.Opacity = 0.5R
        Me.ShowInTaskbar = False
        Me.Text = "Modul SlideShowSaver 3.0 - Bildinfo Overlay"
        Me.TopMost = True
        Me.grpDatei.ResumeLayout(False)
        Me.grpDatei.PerformLayout()
        Me.grpKamera.ResumeLayout(False)
        Me.grpKamera.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents grpDatei As Windows.Forms.GroupBox
    Friend WithEvents lblNDateiname As Windows.Forms.Label
    Friend WithEvents lblNDatei As Windows.Forms.Label
    Friend WithEvents grpKamera As Windows.Forms.GroupBox
    Friend WithEvents lblNKamera As Windows.Forms.Label
    Friend WithEvents lblDateiname As Windows.Forms.Label
    Friend WithEvents lblDateipfad As Windows.Forms.Label
    Friend WithEvents lblNTags As Windows.Forms.Label
    Friend WithEvents lblNAutor As Windows.Forms.Label
    Friend WithEvents lblNErstellungsdatum As Windows.Forms.Label
    Friend WithEvents lblNDateipfad As Windows.Forms.Label
    Friend WithEvents lblTags As Windows.Forms.Label
    Friend WithEvents lblAutor As Windows.Forms.Label
    Friend WithEvents lblErstellungsdatum As Windows.Forms.Label
    Friend WithEvents lblNBlende As Windows.Forms.Label
    Friend WithEvents lblNISO As Windows.Forms.Label
    Friend WithEvents lblNVerschlusszeit As Windows.Forms.Label
    Friend WithEvents lblNObjektiv As Windows.Forms.Label
    Friend WithEvents lblNBrennweite As Windows.Forms.Label
    Friend WithEvents lblNlblKamera As Windows.Forms.Label
    Friend WithEvents lblBlende As Windows.Forms.Label
    Friend WithEvents lblISO As Windows.Forms.Label
    Friend WithEvents lblVerschlusszeit As Windows.Forms.Label
    Friend WithEvents lblObjektiv As Windows.Forms.Label
    Friend WithEvents lblBrennweite As Windows.Forms.Label
    Friend WithEvents lblKamera As Windows.Forms.Label
    Friend WithEvents LblNBewertung As Windows.Forms.Label
    Friend WithEvents lblBewertung As Windows.Forms.Label
    Friend WithEvents slbBewertung As MyControlsLibrary.SterneAnzeigeLabel
    Friend WithEvents lblNBildinformationen As Windows.Forms.Label
End Class