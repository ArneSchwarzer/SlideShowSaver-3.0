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
        Me.lblTitel = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lblBewertung = New System.Windows.Forms.Label()
        Me.slbBewertung = New StarControlLibrary.SterneAnzeigeLabel()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.lblTags = New System.Windows.Forms.Label()
        Me.lblAutor = New System.Windows.Forms.Label()
        Me.lblErstellungsdatum = New System.Windows.Forms.Label()
        Me.lblDateipfad = New System.Windows.Forms.Label()
        Me.lblDateiname = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lblBlende = New System.Windows.Forms.Label()
        Me.lblISO = New System.Windows.Forms.Label()
        Me.lblVerschlusszeit = New System.Windows.Forms.Label()
        Me.lblObjektiv = New System.Windows.Forms.Label()
        Me.lblBrennweite = New System.Windows.Forms.Label()
        Me.lblKamera = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblTitel
        '
        Me.lblTitel.AutoSize = True
        Me.lblTitel.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitel.Location = New System.Drawing.Point(1, 9)
        Me.lblTitel.MinimumSize = New System.Drawing.Size(1457, 0)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(1457, 72)
        Me.lblTitel.TabIndex = 0
        Me.lblTitel.Text = "Bildinformationen"
        Me.lblTitel.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox1.Controls.Add(Me.lblBewertung)
        Me.GroupBox1.Controls.Add(Me.slbBewertung)
        Me.GroupBox1.Controls.Add(Me.Label16)
        Me.GroupBox1.Controls.Add(Me.lblTags)
        Me.GroupBox1.Controls.Add(Me.lblAutor)
        Me.GroupBox1.Controls.Add(Me.lblErstellungsdatum)
        Me.GroupBox1.Controls.Add(Me.lblDateipfad)
        Me.GroupBox1.Controls.Add(Me.lblDateiname)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Location = New System.Drawing.Point(13, 136)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(903, 538)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        '
        'lblBewertung
        '
        Me.lblBewertung.AutoSize = True
        Me.lblBewertung.Location = New System.Drawing.Point(286, 315)
        Me.lblBewertung.Name = "lblBewertung"
        Me.lblBewertung.Size = New System.Drawing.Size(30, 41)
        Me.lblBewertung.TabIndex = 13
        Me.lblBewertung.Text = "-"
        '
        'slbBewertung
        '
        Me.slbBewertung.BackColor = System.Drawing.Color.Black
        Me.slbBewertung.Bewertung = 0
        Me.slbBewertung.Location = New System.Drawing.Point(293, 326)
        Me.slbBewertung.Name = "slbBewertung"
        Me.slbBewertung.Size = New System.Drawing.Size(150, 30)
        Me.slbBewertung.TabIndex = 12
        Me.slbBewertung.Text = "SterneAnzeigeLabel1"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(17, 315)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(161, 41)
        Me.Label16.TabIndex = 10
        Me.Label16.Text = "Bewertung"
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
        Me.lblDateipfad.Size = New System.Drawing.Size(579, 164)
        Me.lblDateipfad.TabIndex = 6
        Me.lblDateipfad.Text = "C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Module C:\U" &
    "sers\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Module"
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
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(17, 411)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(243, 41)
        Me.Label8.TabIndex = 4
        Me.Label8.Text = "Verwendete Tags"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(17, 363)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(92, 41)
        Me.Label7.TabIndex = 3
        Me.Label7.Text = "Autor"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(17, 267)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(247, 41)
        Me.Label6.TabIndex = 2
        Me.Label6.Text = "Erstellungsdatum"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(17, 92)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(147, 41)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "Dateipfad"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(17, 34)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(161, 41)
        Me.Label4.TabIndex = 0
        Me.Label4.Text = "Dateiname"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 101)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(87, 41)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Datei"
        '
        'GroupBox2
        '
        Me.GroupBox2.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox2.Controls.Add(Me.lblBlende)
        Me.GroupBox2.Controls.Add(Me.lblISO)
        Me.GroupBox2.Controls.Add(Me.lblVerschlusszeit)
        Me.GroupBox2.Controls.Add(Me.lblObjektiv)
        Me.GroupBox2.Controls.Add(Me.lblBrennweite)
        Me.GroupBox2.Controls.Add(Me.lblKamera)
        Me.GroupBox2.Controls.Add(Me.Label15)
        Me.GroupBox2.Controls.Add(Me.Label13)
        Me.GroupBox2.Controls.Add(Me.Label12)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Controls.Add(Me.Label10)
        Me.GroupBox2.Controls.Add(Me.Label9)
        Me.GroupBox2.Location = New System.Drawing.Point(949, 136)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(485, 538)
        Me.GroupBox2.TabIndex = 3
        Me.GroupBox2.TabStop = False
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
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(7, 311)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(109, 41)
        Me.Label15.TabIndex = 6
        Me.Label15.Text = "Blende"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(7, 407)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(65, 41)
        Me.Label13.TabIndex = 4
        Me.Label13.Text = "ISO"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(7, 359)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(202, 41)
        Me.Label12.TabIndex = 3
        Me.Label12.Text = "Verschlusszeit"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(7, 175)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(128, 41)
        Me.Label11.TabIndex = 2
        Me.Label11.Text = "Objektiv"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(7, 255)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(166, 41)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "Brennweite"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(7, 34)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(117, 41)
        Me.Label9.TabIndex = 0
        Me.Label9.Text = "Kamera"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(942, 101)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(117, 41)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Kamera"
        '
        'frmPictureInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(1459, 698)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.lblTitel)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.ForeColor = System.Drawing.Color.Snow
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmPictureInfo"
        Me.Opacity = 0.5R
        Me.ShowInTaskbar = False
        Me.Text = "7"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents GroupBox1 As Windows.Forms.GroupBox
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents GroupBox2 As Windows.Forms.GroupBox
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents lblDateiname As Windows.Forms.Label
    Friend WithEvents lblDateipfad As Windows.Forms.Label
    Friend WithEvents Label8 As Windows.Forms.Label
    Friend WithEvents Label7 As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents lblTags As Windows.Forms.Label
    Friend WithEvents lblAutor As Windows.Forms.Label
    Friend WithEvents lblErstellungsdatum As Windows.Forms.Label
    Friend WithEvents Label15 As Windows.Forms.Label
    Friend WithEvents Label13 As Windows.Forms.Label
    Friend WithEvents Label12 As Windows.Forms.Label
    Friend WithEvents Label11 As Windows.Forms.Label
    Friend WithEvents Label10 As Windows.Forms.Label
    Friend WithEvents Label9 As Windows.Forms.Label
    Friend WithEvents lblBlende As Windows.Forms.Label
    Friend WithEvents lblISO As Windows.Forms.Label
    Friend WithEvents lblVerschlusszeit As Windows.Forms.Label
    Friend WithEvents lblObjektiv As Windows.Forms.Label
    Friend WithEvents lblBrennweite As Windows.Forms.Label
    Friend WithEvents lblKamera As Windows.Forms.Label
    Friend WithEvents Label16 As Windows.Forms.Label
    Friend WithEvents slbBewertung As StarControlLibrary.SterneAnzeigeLabel
    Friend WithEvents lblBewertung As Windows.Forms.Label
End Class
