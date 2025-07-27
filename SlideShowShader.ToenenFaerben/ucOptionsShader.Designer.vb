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
        Me.lblNlblShaderName = New System.Windows.Forms.Label()
        Me.lblNpicFarbton = New System.Windows.Forms.Label()
        Me.picFarbton = New System.Windows.Forms.PictureBox()
        Me.lblNtrbIntensität = New System.Windows.Forms.Label()
        Me.trkIntensität = New System.Windows.Forms.TrackBar()
        Me.lblIntensität = New System.Windows.Forms.Label()
        Me.cdFarbton = New System.Windows.Forms.ColorDialog()
        Me.lblNModus = New System.Windows.Forms.Label()
        Me.chkZufallsfarbe = New System.Windows.Forms.CheckBox()
        Me.pnlModus = New System.Windows.Forms.Panel()
        Me.rdoZufall = New System.Windows.Forms.RadioButton()
        Me.rdoFärben = New System.Windows.Forms.RadioButton()
        Me.rdoTönen = New System.Windows.Forms.RadioButton()
        CType(Me.picFarbton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkIntensität, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlModus.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblShadername.Location = New System.Drawing.Point(269, 30)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(241, 41)
        Me.lblShadername.TabIndex = 20
        Me.lblShadername.Text = "Tönen && Färben"
        '
        'lblNlblShaderName
        '
        Me.lblNlblShaderName.AutoSize = True
        Me.lblNlblShaderName.Location = New System.Drawing.Point(15, 30)
        Me.lblNlblShaderName.Name = "lblNlblShaderName"
        Me.lblNlblShaderName.Size = New System.Drawing.Size(110, 41)
        Me.lblNlblShaderName.TabIndex = 19
        Me.lblNlblShaderName.Text = "Shader"
        '
        'lblNpicFarbton
        '
        Me.lblNpicFarbton.AutoSize = True
        Me.lblNpicFarbton.Location = New System.Drawing.Point(15, 130)
        Me.lblNpicFarbton.Name = "lblNpicFarbton"
        Me.lblNpicFarbton.Size = New System.Drawing.Size(120, 41)
        Me.lblNpicFarbton.TabIndex = 21
        Me.lblNpicFarbton.Text = "Farbton"
        '
        'picFarbton
        '
        Me.picFarbton.BackColor = System.Drawing.Color.Sienna
        Me.picFarbton.Location = New System.Drawing.Point(276, 121)
        Me.picFarbton.Name = "picFarbton"
        Me.picFarbton.Size = New System.Drawing.Size(585, 50)
        Me.picFarbton.TabIndex = 22
        Me.picFarbton.TabStop = False
        '
        'lblNtrbIntensität
        '
        Me.lblNtrbIntensität.AutoSize = True
        Me.lblNtrbIntensität.Location = New System.Drawing.Point(15, 267)
        Me.lblNtrbIntensität.Name = "lblNtrbIntensität"
        Me.lblNtrbIntensität.Size = New System.Drawing.Size(141, 41)
        Me.lblNtrbIntensität.TabIndex = 23
        Me.lblNtrbIntensität.Text = "Intensität"
        '
        'trkIntensität
        '
        Me.trkIntensität.Location = New System.Drawing.Point(282, 267)
        Me.trkIntensität.Maximum = 100
        Me.trkIntensität.Minimum = 1
        Me.trkIntensität.Name = "trkIntensität"
        Me.trkIntensität.Size = New System.Drawing.Size(453, 101)
        Me.trkIntensität.TabIndex = 24
        Me.trkIntensität.Value = 34
        '
        'lblIntensität
        '
        Me.lblIntensität.AutoSize = True
        Me.lblIntensität.Location = New System.Drawing.Point(755, 267)
        Me.lblIntensität.MinimumSize = New System.Drawing.Size(99, 0)
        Me.lblIntensität.Name = "lblIntensität"
        Me.lblIntensität.Size = New System.Drawing.Size(99, 41)
        Me.lblIntensität.TabIndex = 25
        Me.lblIntensität.Text = "34 %"
        Me.lblIntensität.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Location = New System.Drawing.Point(15, 392)
        Me.lblNModus.Name = "lblNModus"
        Me.lblNModus.Size = New System.Drawing.Size(111, 41)
        Me.lblNModus.TabIndex = 29
        Me.lblNModus.Text = "Modus"
        '
        'chkZufallsfarbe
        '
        Me.chkZufallsfarbe.AutoSize = True
        Me.chkZufallsfarbe.Location = New System.Drawing.Point(276, 199)
        Me.chkZufallsfarbe.Name = "chkZufallsfarbe"
        Me.chkZufallsfarbe.Size = New System.Drawing.Size(203, 45)
        Me.chkZufallsfarbe.TabIndex = 30
        Me.chkZufallsfarbe.Text = "Zufallsfarbe"
        Me.chkZufallsfarbe.UseVisualStyleBackColor = True
        '
        'pnlModus
        '
        Me.pnlModus.Controls.Add(Me.rdoZufall)
        Me.pnlModus.Controls.Add(Me.rdoFärben)
        Me.pnlModus.Controls.Add(Me.rdoTönen)
        Me.pnlModus.Location = New System.Drawing.Point(257, 392)
        Me.pnlModus.Name = "pnlModus"
        Me.pnlModus.Size = New System.Drawing.Size(604, 164)
        Me.pnlModus.TabIndex = 31
        '
        'rdoZufall
        '
        Me.rdoZufall.AutoSize = True
        Me.rdoZufall.Location = New System.Drawing.Point(19, 100)
        Me.rdoZufall.Name = "rdoZufall"
        Me.rdoZufall.Size = New System.Drawing.Size(146, 45)
        Me.rdoZufall.TabIndex = 31
        Me.rdoZufall.TabStop = True
        Me.rdoZufall.Text = "Zufällig"
        Me.rdoZufall.UseVisualStyleBackColor = True
        '
        'rdoFärben
        '
        Me.rdoFärben.AutoSize = True
        Me.rdoFärben.Location = New System.Drawing.Point(19, 49)
        Me.rdoFärben.Name = "rdoFärben"
        Me.rdoFärben.Size = New System.Drawing.Size(139, 45)
        Me.rdoFärben.TabIndex = 30
        Me.rdoFärben.TabStop = True
        Me.rdoFärben.Text = "Färben"
        Me.rdoFärben.UseVisualStyleBackColor = True
        '
        'rdoTönen
        '
        Me.rdoTönen.AutoSize = True
        Me.rdoTönen.Location = New System.Drawing.Point(19, -2)
        Me.rdoTönen.Name = "rdoTönen"
        Me.rdoTönen.Size = New System.Drawing.Size(130, 45)
        Me.rdoTönen.TabIndex = 29
        Me.rdoTönen.TabStop = True
        Me.rdoTönen.Text = "Tönen"
        Me.rdoTönen.UseVisualStyleBackColor = True
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.pnlModus)
        Me.Controls.Add(Me.chkZufallsfarbe)
        Me.Controls.Add(Me.lblNModus)
        Me.Controls.Add(Me.lblIntensität)
        Me.Controls.Add(Me.trkIntensität)
        Me.Controls.Add(Me.lblNtrbIntensität)
        Me.Controls.Add(Me.picFarbton)
        Me.Controls.Add(Me.lblNpicFarbton)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNlblShaderName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsShader"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.picFarbton, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkIntensität, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlModus.ResumeLayout(False)
        Me.pnlModus.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNlblShaderName As Windows.Forms.Label
    Friend WithEvents lblNpicFarbton As Windows.Forms.Label
    Friend WithEvents picFarbton As Windows.Forms.PictureBox
    Friend WithEvents lblNtrbIntensität As Windows.Forms.Label
    Friend WithEvents trkIntensität As Windows.Forms.TrackBar
    Friend WithEvents lblIntensität As Windows.Forms.Label
    Friend WithEvents cdFarbton As Windows.Forms.ColorDialog
    Friend WithEvents lblNModus As Windows.Forms.Label
    Friend WithEvents chkZufallsfarbe As Windows.Forms.CheckBox
    Friend WithEvents pnlModus As Windows.Forms.Panel
    Friend WithEvents rdoZufall As Windows.Forms.RadioButton
    Friend WithEvents rdoFärben As Windows.Forms.RadioButton
    Friend WithEvents rdoTönen As Windows.Forms.RadioButton
End Class
