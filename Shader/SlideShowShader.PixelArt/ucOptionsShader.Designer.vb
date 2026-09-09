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
        Me.btnDefaults = New System.Windows.Forms.Button()
Me.btnDefaults.Tag = "langKey=btnDefaults"
        Me.chkRasterZufall = New System.Windows.Forms.CheckBox()
Me.chkRasterZufall.Tag = "langKey=chkRasterZufall"
        Me.lblRaster = New System.Windows.Forms.Label()
Me.lblRaster.Tag = "langKey=lblRaster"
        Me.cmbFarbraum = New System.Windows.Forms.ComboBox()
Me.cmbFarbraum.Tag = "langKey=cmbFarbraum"
        Me.lblNFarbraum = New System.Windows.Forms.Label()
Me.lblNFarbraum.Tag = "langKey=lblNFarbraum"
        Me.trkRaster = New System.Windows.Forms.TrackBar()
        Me.lblNRaster = New System.Windows.Forms.Label()
Me.lblNRaster.Tag = "langKey=lblNRaster"
        Me.chkFarbraumZufall = New System.Windows.Forms.CheckBox()
Me.chkFarbraumZufall.Tag = "langKey=chkFarbraumZufall"
        CType(Me.trkRaster, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblShadername.Location = New System.Drawing.Point(264, 22)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(132, 41)
        Me.lblShadername.TabIndex = 19
        Me.lblShadername.Text = "PixelArt"
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
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(697, 22)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(181, 52)
        Me.btnDefaults.TabIndex = 22
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'chkRasterZufall
        '
        Me.chkRasterZufall.AutoSize = True
        Me.chkRasterZufall.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.chkRasterZufall.Location = New System.Drawing.Point(271, 186)
        Me.chkRasterZufall.Name = "chkRasterZufall"
        Me.chkRasterZufall.Size = New System.Drawing.Size(315, 45)
        Me.chkRasterZufall.TabIndex = 28
        Me.chkRasterZufall.Text = "Rastergröße Zufällig"
        Me.chkRasterZufall.UseVisualStyleBackColor = True
        '
        'lblRaster
        '
        Me.lblRaster.AutoSize = True
        Me.lblRaster.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblRaster.Location = New System.Drawing.Point(768, 130)
        Me.lblRaster.MaximumSize = New System.Drawing.Size(90, 41)
        Me.lblRaster.MinimumSize = New System.Drawing.Size(90, 41)
        Me.lblRaster.Name = "lblRaster"
        Me.lblRaster.Size = New System.Drawing.Size(90, 41)
        Me.lblRaster.TabIndex = 27
        Me.lblRaster.Text = "16 px"
        Me.lblRaster.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'cmbFarbraum
        '
        Me.cmbFarbraum.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.cmbFarbraum.FormattingEnabled = True
        Me.cmbFarbraum.Items.AddRange(New Object() {"8 Farben (2 pro Kanal)", "27 Farben (3 pro Kanal)", "64 Farben (4 pro Kanal)", "125 Farben (5 pro Kanal)", "216 Farben (6 pro Kanal)", "343 Farben (7 pro Kanal)", "512 Farben (8 pro Kanal)"})
        Me.cmbFarbraum.Location = New System.Drawing.Point(271, 244)
        Me.cmbFarbraum.Name = "cmbFarbraum"
        Me.cmbFarbraum.Size = New System.Drawing.Size(595, 49)
        Me.cmbFarbraum.TabIndex = 26
        '
        'lblNFarbraum
        '
        Me.lblNFarbraum.AutoSize = True
        Me.lblNFarbraum.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNFarbraum.Location = New System.Drawing.Point(33, 246)
        Me.lblNFarbraum.Name = "lblNFarbraum"
        Me.lblNFarbraum.Size = New System.Drawing.Size(143, 41)
        Me.lblNFarbraum.TabIndex = 25
        Me.lblNFarbraum.Text = "Farbraum"
        '
        'trkRaster
        '
        Me.trkRaster.AutoSize = False
        Me.trkRaster.Location = New System.Drawing.Point(271, 130)
        Me.trkRaster.Maximum = 6
        Me.trkRaster.Minimum = 1
        Me.trkRaster.Name = "trkRaster"
        Me.trkRaster.Size = New System.Drawing.Size(469, 49)
        Me.trkRaster.TabIndex = 24
        Me.trkRaster.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkRaster.Value = 4
        '
        'lblNRaster
        '
        Me.lblNRaster.AutoSize = True
        Me.lblNRaster.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNRaster.Location = New System.Drawing.Point(32, 130)
        Me.lblNRaster.Name = "lblNRaster"
        Me.lblNRaster.Size = New System.Drawing.Size(178, 41)
        Me.lblNRaster.TabIndex = 23
        Me.lblNRaster.Text = "Rastergröße"
        '
        'chkFarbraumZufall
        '
        Me.chkFarbraumZufall.AutoSize = True
        Me.chkFarbraumZufall.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.chkFarbraumZufall.Location = New System.Drawing.Point(271, 320)
        Me.chkFarbraumZufall.Name = "chkFarbraumZufall"
        Me.chkFarbraumZufall.Size = New System.Drawing.Size(280, 45)
        Me.chkFarbraumZufall.TabIndex = 29
        Me.chkFarbraumZufall.Text = "Farbraum Zufällig"
        Me.chkFarbraumZufall.UseVisualStyleBackColor = True
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.chkFarbraumZufall)
        Me.Controls.Add(Me.chkRasterZufall)
        Me.Controls.Add(Me.lblRaster)
        Me.Controls.Add(Me.cmbFarbraum)
        Me.Controls.Add(Me.lblNFarbraum)
        Me.Controls.Add(Me.trkRaster)
        Me.Controls.Add(Me.lblNRaster)
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNlblShaderName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(5, 6, 5, 6)
        Me.Name = "ucOptionsShader"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkRaster, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As System.Windows.Forms.Label
    Friend WithEvents lblNlblShaderName As System.Windows.Forms.Label
    Friend WithEvents btnDefaults As Forms.Button
    Friend WithEvents chkRasterZufall As Forms.CheckBox
    Friend WithEvents lblRaster As Forms.Label
    Friend WithEvents cmbFarbraum As Forms.ComboBox
    Friend WithEvents lblNFarbraum As Forms.Label
    Friend WithEvents trkRaster As Forms.TrackBar
    Friend WithEvents lblNRaster As Forms.Label
    Friend WithEvents chkFarbraumZufall As Forms.CheckBox
End Class