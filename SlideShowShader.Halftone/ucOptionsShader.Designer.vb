<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucOptionsShader
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblShadername = New System.Windows.Forms.Label()
Me.lblShadername.Tag = "langKey=lblShadername"
        Me.lblNlblShaderName = New System.Windows.Forms.Label()
Me.lblNlblShaderName.Tag = "langKey=lblNlblShaderName"
        Me.grbAllgemeineOptionen = New System.Windows.Forms.GroupBox()
Me.grbAllgemeineOptionen.Tag = "langKey=grbAllgemeineOptionen"
        Me.chkRasterZufall = New System.Windows.Forms.CheckBox()
Me.chkRasterZufall.Tag = "langKey=chkRasterZufall"
        Me.lblGamma = New System.Windows.Forms.Label()
Me.lblGamma.Tag = "langKey=lblGamma"
        Me.lblRaster = New System.Windows.Forms.Label()
Me.lblRaster.Tag = "langKey=lblRaster"
        Me.cmbPosterise = New System.Windows.Forms.ComboBox()
Me.cmbPosterise.Tag = "langKey=cmbPosterise"
        Me.lblNPosterise = New System.Windows.Forms.Label()
Me.lblNPosterise.Tag = "langKey=lblNPosterise"
        Me.lblNGamma = New System.Windows.Forms.Label()
Me.lblNGamma.Tag = "langKey=lblNGamma"
        Me.trkGamma = New System.Windows.Forms.TrackBar()
        Me.trkRaster = New System.Windows.Forms.TrackBar()
        Me.rdoModusHTCMYK = New System.Windows.Forms.RadioButton()
Me.rdoModusHTCMYK.Tag = "langKey=rdoModusHTCMYK"
        Me.rdoModusZufall = New System.Windows.Forms.RadioButton()
Me.rdoModusZufall.Tag = "langKey=rdoModusZufall"
        Me.rdoModusHTSW = New System.Windows.Forms.RadioButton()
Me.rdoModusHTSW.Tag = "langKey=rdoModusHTSW"
        Me.rdoModusPixelArt = New System.Windows.Forms.RadioButton()
Me.rdoModusPixelArt.Tag = "langKey=rdoModusPixelArt"
        Me.lblNRaster = New System.Windows.Forms.Label()
Me.lblNRaster.Tag = "langKey=lblNRaster"
        Me.lblNModus = New System.Windows.Forms.Label()
Me.lblNModus.Tag = "langKey=lblNModus"
        Me.grbHalftoneOptionen = New System.Windows.Forms.GroupBox()
Me.grbHalftoneOptionen.Tag = "langKey=grbHalftoneOptionen"
        Me.lblPapierintensität = New System.Windows.Forms.Label()
        Me.trkPapierIntensität = New System.Windows.Forms.TrackBar()
        Me.lblNPapierIntensität = New System.Windows.Forms.Label()
        Me.chkPapierTextur = New System.Windows.Forms.CheckBox()
Me.chkPapierTextur.Tag = "langKey=chkPapierTextur"
        Me.cmbDotsWinkel = New System.Windows.Forms.ComboBox()
Me.cmbDotsWinkel.Tag = "langKey=cmbDotsWinkel"
        Me.lblNDotsWinkel = New System.Windows.Forms.Label()
Me.lblNDotsWinkel.Tag = "langKey=lblNDotsWinkel"
        Me.lblDotsMin = New System.Windows.Forms.Label()
Me.lblDotsMin.Tag = "langKey=lblDotsMin"
        Me.trkDotsMin = New System.Windows.Forms.TrackBar()
        Me.lblNDotsMin = New System.Windows.Forms.Label()
Me.lblNDotsMin.Tag = "langKey=lblNDotsMin"
        Me.lblDotsMax = New System.Windows.Forms.Label()
Me.lblDotsMax.Tag = "langKey=lblDotsMax"
        Me.trkDotsMax = New System.Windows.Forms.TrackBar()
        Me.lblNDotsMax = New System.Windows.Forms.Label()
Me.lblNDotsMax.Tag = "langKey=lblNDotsMax"
        Me.btnDefaults = New System.Windows.Forms.Button()
Me.btnDefaults.Tag = "langKey=btnDefaults"
        Me.grbAllgemeineOptionen.SuspendLayout()
        CType(Me.trkGamma, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkRaster, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grbHalftoneOptionen.SuspendLayout()
        CType(Me.trkPapierIntensität, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkDotsMin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkDotsMax, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblShadername.Location = New System.Drawing.Point(264, 22)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(298, 41)
        Me.lblShadername.TabIndex = 19
        Me.lblShadername.Text = "PixelArt && Halftone"
        '
        'lblNlblShaderName
        '
        Me.lblNlblShaderName.AutoSize = True
        Me.lblNlblShaderName.Location = New System.Drawing.Point(32, 22)
        Me.lblNlblShaderName.Name = "lblNlblShaderName"
        Me.lblNlblShaderName.Size = New System.Drawing.Size(110, 41)
        Me.lblNlblShaderName.TabIndex = 18
        Me.lblNlblShaderName.Text = "Shader"
        '
        'grbAllgemeineOptionen
        '
        Me.grbAllgemeineOptionen.Controls.Add(Me.chkRasterZufall)
        Me.grbAllgemeineOptionen.Controls.Add(Me.lblGamma)
        Me.grbAllgemeineOptionen.Controls.Add(Me.lblRaster)
        Me.grbAllgemeineOptionen.Controls.Add(Me.cmbPosterise)
        Me.grbAllgemeineOptionen.Controls.Add(Me.lblNPosterise)
        Me.grbAllgemeineOptionen.Controls.Add(Me.lblNGamma)
        Me.grbAllgemeineOptionen.Controls.Add(Me.trkGamma)
        Me.grbAllgemeineOptionen.Controls.Add(Me.trkRaster)
        Me.grbAllgemeineOptionen.Controls.Add(Me.rdoModusHTCMYK)
        Me.grbAllgemeineOptionen.Controls.Add(Me.rdoModusZufall)
        Me.grbAllgemeineOptionen.Controls.Add(Me.rdoModusHTSW)
        Me.grbAllgemeineOptionen.Controls.Add(Me.rdoModusPixelArt)
        Me.grbAllgemeineOptionen.Controls.Add(Me.lblNRaster)
        Me.grbAllgemeineOptionen.Controls.Add(Me.lblNModus)
        Me.grbAllgemeineOptionen.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Italic)
        Me.grbAllgemeineOptionen.Location = New System.Drawing.Point(23, 80)
        Me.grbAllgemeineOptionen.Name = "grbAllgemeineOptionen"
        Me.grbAllgemeineOptionen.Size = New System.Drawing.Size(855, 448)
        Me.grbAllgemeineOptionen.TabIndex = 20
        Me.grbAllgemeineOptionen.TabStop = False
        Me.grbAllgemeineOptionen.Text = "Allgemeine Optionen"
        '
        'chkRasterZufall
        '
        Me.chkRasterZufall.AutoSize = True
        Me.chkRasterZufall.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.chkRasterZufall.Location = New System.Drawing.Point(248, 270)
        Me.chkRasterZufall.Name = "chkRasterZufall"
        Me.chkRasterZufall.Size = New System.Drawing.Size(315, 45)
        Me.chkRasterZufall.TabIndex = 13
        Me.chkRasterZufall.Text = "Rastergröße Zufällig"
        Me.chkRasterZufall.UseVisualStyleBackColor = True
        '
        'lblGamma
        '
        Me.lblGamma.AutoSize = True
        Me.lblGamma.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblGamma.Location = New System.Drawing.Point(745, 386)
        Me.lblGamma.MaximumSize = New System.Drawing.Size(90, 41)
        Me.lblGamma.MinimumSize = New System.Drawing.Size(90, 41)
        Me.lblGamma.Name = "lblGamma"
        Me.lblGamma.Size = New System.Drawing.Size(90, 41)
        Me.lblGamma.TabIndex = 12
        Me.lblGamma.Text = "1.4"
        Me.lblGamma.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblRaster
        '
        Me.lblRaster.AutoSize = True
        Me.lblRaster.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblRaster.Location = New System.Drawing.Point(745, 214)
        Me.lblRaster.MaximumSize = New System.Drawing.Size(90, 41)
        Me.lblRaster.MinimumSize = New System.Drawing.Size(90, 41)
        Me.lblRaster.Name = "lblRaster"
        Me.lblRaster.Size = New System.Drawing.Size(90, 41)
        Me.lblRaster.TabIndex = 11
        Me.lblRaster.Text = "16 px"
        Me.lblRaster.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'cmbPosterise
        '
        Me.cmbPosterise.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.cmbPosterise.FormattingEnabled = True
        Me.cmbPosterise.Items.AddRange(New Object() {"8 Farben (2 pro Kanal)", "27 Farben (3 pro Kanal)", "64 Farben (4 pro Kanal)", "125 Farben (5 pro Kanal)", "216 Farben (6 pro Kanal)", "343 Farben (7 pro Kanal)", "512 Farben (8 pro Kanal)"})
        Me.cmbPosterise.Location = New System.Drawing.Point(248, 328)
        Me.cmbPosterise.Name = "cmbPosterise"
        Me.cmbPosterise.Size = New System.Drawing.Size(595, 49)
        Me.cmbPosterise.TabIndex = 10
        '
        'lblNPosterise
        '
        Me.lblNPosterise.AutoSize = True
        Me.lblNPosterise.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNPosterise.Location = New System.Drawing.Point(10, 330)
        Me.lblNPosterise.Name = "lblNPosterise"
        Me.lblNPosterise.Size = New System.Drawing.Size(108, 41)
        Me.lblNPosterise.TabIndex = 9
        Me.lblNPosterise.Text = "Farben"
        '
        'lblNGamma
        '
        Me.lblNGamma.AutoSize = True
        Me.lblNGamma.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNGamma.Location = New System.Drawing.Point(13, 386)
        Me.lblNGamma.Name = "lblNGamma"
        Me.lblNGamma.Size = New System.Drawing.Size(121, 41)
        Me.lblNGamma.TabIndex = 8
        Me.lblNGamma.Text = "Gamma"
        '
        'trkGamma
        '
        Me.trkGamma.AutoSize = False
        Me.trkGamma.Location = New System.Drawing.Point(248, 386)
        Me.trkGamma.Maximum = 22
        Me.trkGamma.Minimum = 8
        Me.trkGamma.Name = "trkGamma"
        Me.trkGamma.Size = New System.Drawing.Size(463, 49)
        Me.trkGamma.TabIndex = 7
        Me.trkGamma.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkGamma.Value = 14
        '
        'trkRaster
        '
        Me.trkRaster.AutoSize = False
        Me.trkRaster.Location = New System.Drawing.Point(248, 214)
        Me.trkRaster.Maximum = 6
        Me.trkRaster.Minimum = 1
        Me.trkRaster.Name = "trkRaster"
        Me.trkRaster.Size = New System.Drawing.Size(469, 49)
        Me.trkRaster.TabIndex = 6
        Me.trkRaster.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkRaster.Value = 4
        '
        'rdoModusHTCMYK
        '
        Me.rdoModusHTCMYK.AutoSize = True
        Me.rdoModusHTCMYK.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.rdoModusHTCMYK.Location = New System.Drawing.Point(581, 92)
        Me.rdoModusHTCMYK.Name = "rdoModusHTCMYK"
        Me.rdoModusHTCMYK.Size = New System.Drawing.Size(268, 45)
        Me.rdoModusHTCMYK.TabIndex = 5
        Me.rdoModusHTCMYK.TabStop = True
        Me.rdoModusHTCMYK.Text = "Halftone (CMYK)"
        Me.rdoModusHTCMYK.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.rdoModusHTCMYK.UseVisualStyleBackColor = True
        '
        'rdoModusZufall
        '
        Me.rdoModusZufall.AutoSize = True
        Me.rdoModusZufall.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.rdoModusZufall.Location = New System.Drawing.Point(248, 137)
        Me.rdoModusZufall.Name = "rdoModusZufall"
        Me.rdoModusZufall.Size = New System.Drawing.Size(146, 45)
        Me.rdoModusZufall.TabIndex = 4
        Me.rdoModusZufall.TabStop = True
        Me.rdoModusZufall.Text = "Zufällig"
        Me.rdoModusZufall.UseVisualStyleBackColor = True
        '
        'rdoModusHTSW
        '
        Me.rdoModusHTSW.AutoSize = True
        Me.rdoModusHTSW.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.rdoModusHTSW.Location = New System.Drawing.Point(248, 92)
        Me.rdoModusHTSW.Name = "rdoModusHTSW"
        Me.rdoModusHTSW.Size = New System.Drawing.Size(232, 45)
        Me.rdoModusHTSW.TabIndex = 3
        Me.rdoModusHTSW.TabStop = True
        Me.rdoModusHTSW.Text = "Halftone (SW)"
        Me.rdoModusHTSW.UseVisualStyleBackColor = True
        '
        'rdoModusPixelArt
        '
        Me.rdoModusPixelArt.AutoSize = True
        Me.rdoModusPixelArt.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.rdoModusPixelArt.Location = New System.Drawing.Point(248, 47)
        Me.rdoModusPixelArt.Name = "rdoModusPixelArt"
        Me.rdoModusPixelArt.Size = New System.Drawing.Size(149, 45)
        Me.rdoModusPixelArt.TabIndex = 2
        Me.rdoModusPixelArt.TabStop = True
        Me.rdoModusPixelArt.Text = "PixelArt"
        Me.rdoModusPixelArt.UseVisualStyleBackColor = True
        '
        'lblNRaster
        '
        Me.lblNRaster.AutoSize = True
        Me.lblNRaster.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNRaster.Location = New System.Drawing.Point(9, 214)
        Me.lblNRaster.Name = "lblNRaster"
        Me.lblNRaster.Size = New System.Drawing.Size(178, 41)
        Me.lblNRaster.TabIndex = 1
        Me.lblNRaster.Text = "Rastergröße"
        '
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNModus.Location = New System.Drawing.Point(9, 43)
        Me.lblNModus.Name = "lblNModus"
        Me.lblNModus.Size = New System.Drawing.Size(111, 41)
        Me.lblNModus.TabIndex = 0
        Me.lblNModus.Text = "Modus"
        '
        'grbHalftoneOptionen
        '
        Me.grbHalftoneOptionen.Controls.Add(Me.lblPapierintensität)
        Me.grbHalftoneOptionen.Controls.Add(Me.trkPapierIntensität)
        Me.grbHalftoneOptionen.Controls.Add(Me.lblNPapierIntensität)
        Me.grbHalftoneOptionen.Controls.Add(Me.chkPapierTextur)
        Me.grbHalftoneOptionen.Controls.Add(Me.cmbDotsWinkel)
        Me.grbHalftoneOptionen.Controls.Add(Me.lblNDotsWinkel)
        Me.grbHalftoneOptionen.Controls.Add(Me.lblDotsMin)
        Me.grbHalftoneOptionen.Controls.Add(Me.trkDotsMin)
        Me.grbHalftoneOptionen.Controls.Add(Me.lblNDotsMin)
        Me.grbHalftoneOptionen.Controls.Add(Me.lblDotsMax)
        Me.grbHalftoneOptionen.Controls.Add(Me.trkDotsMax)
        Me.grbHalftoneOptionen.Controls.Add(Me.lblNDotsMax)
        Me.grbHalftoneOptionen.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Italic)
        Me.grbHalftoneOptionen.Location = New System.Drawing.Point(23, 534)
        Me.grbHalftoneOptionen.Name = "grbHalftoneOptionen"
        Me.grbHalftoneOptionen.Size = New System.Drawing.Size(855, 472)
        Me.grbHalftoneOptionen.TabIndex = 21
        Me.grbHalftoneOptionen.TabStop = False
        Me.grbHalftoneOptionen.Text = "Halftone && CMYK Optionen"
        '
        'lblPapierintensität
        '
        Me.lblPapierintensität.AutoSize = True
        Me.lblPapierintensität.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblPapierintensität.Location = New System.Drawing.Point(744, 317)
        Me.lblPapierintensität.MaximumSize = New System.Drawing.Size(90, 41)
        Me.lblPapierintensität.MinimumSize = New System.Drawing.Size(90, 41)
        Me.lblPapierintensität.Name = "lblPapierintensität"
        Me.lblPapierintensität.Size = New System.Drawing.Size(90, 41)
        Me.lblPapierintensität.TabIndex = 23
        Me.lblPapierintensität.Text = "30 %"
        Me.lblPapierintensität.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkPapierIntensität
        '
        Me.trkPapierIntensität.AutoSize = False
        Me.trkPapierIntensität.Location = New System.Drawing.Point(247, 317)
        Me.trkPapierIntensität.Maximum = 50
        Me.trkPapierIntensität.Minimum = 5
        Me.trkPapierIntensität.Name = "trkPapierIntensität"
        Me.trkPapierIntensität.Size = New System.Drawing.Size(463, 49)
        Me.trkPapierIntensität.TabIndex = 22
        Me.trkPapierIntensität.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkPapierIntensität.Value = 30
        '
        'lblNPapierIntensität
        '
        Me.lblNPapierIntensität.AutoSize = True
        Me.lblNPapierIntensität.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNPapierIntensität.Location = New System.Drawing.Point(13, 317)
        Me.lblNPapierIntensität.MaximumSize = New System.Drawing.Size(225, 0)
        Me.lblNPapierIntensität.Name = "lblNPapierIntensität"
        Me.lblNPapierIntensität.Size = New System.Drawing.Size(185, 82)
        Me.lblNPapierIntensität.TabIndex = 21
        Me.lblNPapierIntensität.Text = "Papiertextur Intensität"
        '
        'chkPapierTextur
        '
        Me.chkPapierTextur.AutoSize = True
        Me.chkPapierTextur.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.chkPapierTextur.Location = New System.Drawing.Point(248, 257)
        Me.chkPapierTextur.Name = "chkPapierTextur"
        Me.chkPapierTextur.Size = New System.Drawing.Size(209, 45)
        Me.chkPapierTextur.TabIndex = 20
        Me.chkPapierTextur.Text = "Papiertextur"
        Me.chkPapierTextur.UseVisualStyleBackColor = True
        '
        'cmbDotsWinkel
        '
        Me.cmbDotsWinkel.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.cmbDotsWinkel.FormattingEnabled = True
        Me.cmbDotsWinkel.Items.AddRange(New Object() {"C: 15° | M: 75° | Y: 0° | K: 45° (klassisch)", "C: 15° | M: 45° | Y: 0° | K: 75° (alternativ)"})
        Me.cmbDotsWinkel.Location = New System.Drawing.Point(248, 164)
        Me.cmbDotsWinkel.Name = "cmbDotsWinkel"
        Me.cmbDotsWinkel.Size = New System.Drawing.Size(595, 49)
        Me.cmbDotsWinkel.TabIndex = 19
        '
        'lblNDotsWinkel
        '
        Me.lblNDotsWinkel.AutoSize = True
        Me.lblNDotsWinkel.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNDotsWinkel.Location = New System.Drawing.Point(9, 164)
        Me.lblNDotsWinkel.Name = "lblNDotsWinkel"
        Me.lblNDotsWinkel.Size = New System.Drawing.Size(178, 41)
        Me.lblNDotsWinkel.TabIndex = 18
        Me.lblNDotsWinkel.Text = "Dots Winkel"
        '
        'lblDotsMin
        '
        Me.lblDotsMin.AutoSize = True
        Me.lblDotsMin.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblDotsMin.Location = New System.Drawing.Point(745, 109)
        Me.lblDotsMin.MaximumSize = New System.Drawing.Size(90, 41)
        Me.lblDotsMin.MinimumSize = New System.Drawing.Size(90, 41)
        Me.lblDotsMin.Name = "lblDotsMin"
        Me.lblDotsMin.Size = New System.Drawing.Size(90, 41)
        Me.lblDotsMin.TabIndex = 17
        Me.lblDotsMin.Text = "5 %"
        Me.lblDotsMin.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkDotsMin
        '
        Me.trkDotsMin.AutoSize = False
        Me.trkDotsMin.Location = New System.Drawing.Point(248, 109)
        Me.trkDotsMin.Name = "trkDotsMin"
        Me.trkDotsMin.Size = New System.Drawing.Size(463, 49)
        Me.trkDotsMin.TabIndex = 16
        Me.trkDotsMin.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkDotsMin.Value = 5
        '
        'lblNDotsMin
        '
        Me.lblNDotsMin.AutoSize = True
        Me.lblNDotsMin.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNDotsMin.Location = New System.Drawing.Point(9, 109)
        Me.lblNDotsMin.Name = "lblNDotsMin"
        Me.lblNDotsMin.Size = New System.Drawing.Size(220, 41)
        Me.lblNDotsMin.TabIndex = 15
        Me.lblNDotsMin.Text = "Dots MinGröße"
        '
        'lblDotsMax
        '
        Me.lblDotsMax.AutoSize = True
        Me.lblDotsMax.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblDotsMax.Location = New System.Drawing.Point(744, 54)
        Me.lblDotsMax.MaximumSize = New System.Drawing.Size(90, 41)
        Me.lblDotsMax.MinimumSize = New System.Drawing.Size(90, 41)
        Me.lblDotsMax.Name = "lblDotsMax"
        Me.lblDotsMax.Size = New System.Drawing.Size(90, 41)
        Me.lblDotsMax.TabIndex = 14
        Me.lblDotsMax.Text = "40 %"
        Me.lblDotsMax.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkDotsMax
        '
        Me.trkDotsMax.AutoSize = False
        Me.trkDotsMax.Location = New System.Drawing.Point(247, 54)
        Me.trkDotsMax.Maximum = 50
        Me.trkDotsMax.Minimum = 20
        Me.trkDotsMax.Name = "trkDotsMax"
        Me.trkDotsMax.Size = New System.Drawing.Size(463, 49)
        Me.trkDotsMax.TabIndex = 13
        Me.trkDotsMax.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkDotsMax.Value = 40
        '
        'lblNDotsMax
        '
        Me.lblNDotsMax.AutoSize = True
        Me.lblNDotsMax.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNDotsMax.Location = New System.Drawing.Point(13, 54)
        Me.lblNDotsMax.Name = "lblNDotsMax"
        Me.lblNDotsMax.Size = New System.Drawing.Size(225, 41)
        Me.lblNDotsMax.TabIndex = 0
        Me.lblNDotsMax.Text = "Dots MaxGröße"
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(697, 22)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(181, 52)
        Me.btnDefaults.TabIndex = 22
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.grbHalftoneOptionen)
        Me.Controls.Add(Me.grbAllgemeineOptionen)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNlblShaderName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(5, 6, 5, 6)
        Me.Name = "ucOptionsShader"
        Me.Size = New System.Drawing.Size(890, 1020)
        Me.grbAllgemeineOptionen.ResumeLayout(False)
        Me.grbAllgemeineOptionen.PerformLayout()
        CType(Me.trkGamma, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkRaster, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grbHalftoneOptionen.ResumeLayout(False)
        Me.grbHalftoneOptionen.PerformLayout()
        CType(Me.trkPapierIntensität, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkDotsMin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkDotsMax, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As System.Windows.Forms.Label
    Friend WithEvents lblNlblShaderName As System.Windows.Forms.Label
    Friend WithEvents grbAllgemeineOptionen As Forms.GroupBox
    Friend WithEvents lblNModus As Forms.Label
    Friend WithEvents grbHalftoneOptionen As Forms.GroupBox
    Friend WithEvents lblNRaster As Forms.Label
    Friend WithEvents rdoModusHTCMYK As Forms.RadioButton
    Friend WithEvents rdoModusZufall As Forms.RadioButton
    Friend WithEvents rdoModusHTSW As Forms.RadioButton
    Friend WithEvents rdoModusPixelArt As Forms.RadioButton
    Friend WithEvents trkRaster As Forms.TrackBar
    Friend WithEvents lblNGamma As Forms.Label
    Friend WithEvents trkGamma As Forms.TrackBar
    Friend WithEvents cmbPosterise As Forms.ComboBox
    Friend WithEvents lblNPosterise As Forms.Label
    Friend WithEvents lblGamma As Forms.Label
    Friend WithEvents lblRaster As Forms.Label
    Friend WithEvents chkRasterZufall As Forms.CheckBox
    Friend WithEvents btnDefaults As Forms.Button
    Friend WithEvents lblDotsMin As Forms.Label
    Friend WithEvents trkDotsMin As Forms.TrackBar
    Friend WithEvents lblNDotsMin As Forms.Label
    Friend WithEvents cmbDotsWinkel As Forms.ComboBox
    Friend WithEvents lblNDotsWinkel As Forms.Label
    Friend WithEvents chkPapierTextur As Forms.CheckBox
    Friend WithEvents lblPapierintensität As Forms.Label
    Friend WithEvents trkPapierIntensität As Forms.TrackBar
    Friend WithEvents lblNPapierIntensität As Forms.Label
    Friend WithEvents lblDotsMax As Forms.Label
    Friend WithEvents trkDotsMax As Forms.TrackBar
    Friend WithEvents lblNDotsMax As Forms.Label
End Class