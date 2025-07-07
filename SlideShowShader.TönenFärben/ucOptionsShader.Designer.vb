<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsShader
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.lblShadername = New System.Windows.Forms.Label()
        Me.lblNlblShaderName = New System.Windows.Forms.Label()
        Me.lblNpicFarbton = New System.Windows.Forms.Label()
        Me.picFarbton = New System.Windows.Forms.PictureBox()
        Me.lblNtrbIntensität = New System.Windows.Forms.Label()
        Me.trbIntensität = New System.Windows.Forms.TrackBar()
        Me.lblInensität = New System.Windows.Forms.Label()
        Me.cdFarbton = New System.Windows.Forms.ColorDialog()
        Me.rbTönen = New System.Windows.Forms.RadioButton()
        Me.rbFärben = New System.Windows.Forms.RadioButton()
        Me.rbZufall = New System.Windows.Forms.RadioButton()
        Me.lblNModus = New System.Windows.Forms.Label()
        CType(Me.picFarbton, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trbIntensität, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.lblNlblShaderName.Location = New System.Drawing.Point(27, 30)
        Me.lblNlblShaderName.Name = "lblNlblShaderName"
        Me.lblNlblShaderName.Size = New System.Drawing.Size(110, 41)
        Me.lblNlblShaderName.TabIndex = 19
        Me.lblNlblShaderName.Text = "Shader"
        '
        'lblNpicFarbton
        '
        Me.lblNpicFarbton.AutoSize = True
        Me.lblNpicFarbton.Location = New System.Drawing.Point(27, 130)
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
        Me.lblNtrbIntensität.Location = New System.Drawing.Point(34, 213)
        Me.lblNtrbIntensität.Name = "lblNtrbIntensität"
        Me.lblNtrbIntensität.Size = New System.Drawing.Size(141, 41)
        Me.lblNtrbIntensität.TabIndex = 23
        Me.lblNtrbIntensität.Text = "Intensität"
        '
        'trbIntensität
        '
        Me.trbIntensität.Location = New System.Drawing.Point(289, 213)
        Me.trbIntensität.Maximum = 100
        Me.trbIntensität.Minimum = 1
        Me.trbIntensität.Name = "trbIntensität"
        Me.trbIntensität.Size = New System.Drawing.Size(453, 101)
        Me.trbIntensität.TabIndex = 24
        Me.trbIntensität.Value = 35
        '
        'lblInensität
        '
        Me.lblInensität.AutoSize = True
        Me.lblInensität.Location = New System.Drawing.Point(762, 213)
        Me.lblInensität.MinimumSize = New System.Drawing.Size(99, 0)
        Me.lblInensität.Name = "lblInensität"
        Me.lblInensität.Size = New System.Drawing.Size(99, 41)
        Me.lblInensität.TabIndex = 25
        Me.lblInensität.Text = "35 %"
        Me.lblInensität.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'rbTönen
        '
        Me.rbTönen.AutoSize = True
        Me.rbTönen.Location = New System.Drawing.Point(276, 338)
        Me.rbTönen.Name = "rbTönen"
        Me.rbTönen.Size = New System.Drawing.Size(130, 45)
        Me.rbTönen.TabIndex = 26
        Me.rbTönen.TabStop = True
        Me.rbTönen.Text = "Tönen"
        Me.rbTönen.UseVisualStyleBackColor = True
        '
        'rbFärben
        '
        Me.rbFärben.AutoSize = True
        Me.rbFärben.Location = New System.Drawing.Point(276, 389)
        Me.rbFärben.Name = "rbFärben"
        Me.rbFärben.Size = New System.Drawing.Size(139, 45)
        Me.rbFärben.TabIndex = 27
        Me.rbFärben.TabStop = True
        Me.rbFärben.Text = "Färben"
        Me.rbFärben.UseVisualStyleBackColor = True
        '
        'rbZufall
        '
        Me.rbZufall.AutoSize = True
        Me.rbZufall.Location = New System.Drawing.Point(276, 440)
        Me.rbZufall.Name = "rbZufall"
        Me.rbZufall.Size = New System.Drawing.Size(146, 45)
        Me.rbZufall.TabIndex = 28
        Me.rbZufall.TabStop = True
        Me.rbZufall.Text = "Zufällig"
        Me.rbZufall.UseVisualStyleBackColor = True
        '
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Location = New System.Drawing.Point(34, 338)
        Me.lblNModus.Name = "lblNModus"
        Me.lblNModus.Size = New System.Drawing.Size(111, 41)
        Me.lblNModus.TabIndex = 29
        Me.lblNModus.Text = "Modus"
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblNModus)
        Me.Controls.Add(Me.rbZufall)
        Me.Controls.Add(Me.rbFärben)
        Me.Controls.Add(Me.rbTönen)
        Me.Controls.Add(Me.lblInensität)
        Me.Controls.Add(Me.trbIntensität)
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
        CType(Me.trbIntensität, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNlblShaderName As Windows.Forms.Label
    Friend WithEvents lblNpicFarbton As Windows.Forms.Label
    Friend WithEvents picFarbton As Windows.Forms.PictureBox
    Friend WithEvents lblNtrbIntensität As Windows.Forms.Label
    Friend WithEvents trbIntensität As Windows.Forms.TrackBar
    Friend WithEvents lblInensität As Windows.Forms.Label
    Friend WithEvents cdFarbton As Windows.Forms.ColorDialog
    Friend WithEvents rbTönen As Windows.Forms.RadioButton
    Friend WithEvents rbFärben As Windows.Forms.RadioButton
    Friend WithEvents rbZufall As Windows.Forms.RadioButton
    Friend WithEvents lblNModus As Windows.Forms.Label
End Class

Me.lblShadername.Tag = "langKey=lblShadername"
Me.lblNlblShaderName.Tag = "langKey=lblNlblShaderName"
Me.lblNpicFarbton.Tag = "langKey=lblNpicFarbton"
Me.lblNModus.Tag = "langKey=lblNModus"
Me.rbZufall.Tag = "langKey=rbZufall"
Me.picFarbton.Tag = "langKey=picFarbton"