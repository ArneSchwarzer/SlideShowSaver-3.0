<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsTransition
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
        Me.lblNlblTransitionname = New System.Windows.Forms.Label()
        Me.lblNoOptions = New System.Windows.Forms.Label()
        Me.tbtVNW = New MyControlsLibrary.ToggleButton()
        Me.tbtVN = New MyControlsLibrary.ToggleButton()
        Me.tbtVNO = New MyControlsLibrary.ToggleButton()
        Me.tbtVW = New MyControlsLibrary.ToggleButton()
        Me.tbtVO = New MyControlsLibrary.ToggleButton()
        Me.tbtVSW = New MyControlsLibrary.ToggleButton()
        Me.tbtVS = New MyControlsLibrary.ToggleButton()
        Me.tbtVSO = New MyControlsLibrary.ToggleButton()
        Me.lblKeineRichtungInfo = New System.Windows.Forms.Label()
        Me.lblNtrbGeschwindigkeit = New System.Windows.Forms.Label()
        Me.trbGeschwindigkeit = New System.Windows.Forms.TrackBar()
        Me.lblNModus = New System.Windows.Forms.Label()
        Me.rbZufall = New System.Windows.Forms.RadioButton()
        Me.rbWischen = New System.Windows.Forms.RadioButton()
        Me.rbSchieben = New System.Windows.Forms.RadioButton()
        CType(Me.trbGeschwindigkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblShadername.Location = New System.Drawing.Point(325, 26)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(304, 41)
        Me.lblShadername.TabIndex = 17
        Me.lblShadername.Text = "Schieben && Wischen"
        '
        'lblNlblTransitionname
        '
        Me.lblNlblTransitionname.AutoSize = True
        Me.lblNlblTransitionname.Location = New System.Drawing.Point(34, 26)
        Me.lblNlblTransitionname.Name = "lblNlblTransitionname"
        Me.lblNlblTransitionname.Size = New System.Drawing.Size(151, 41)
        Me.lblNlblTransitionname.TabIndex = 16
        Me.lblNlblTransitionname.Text = "Übergang"
        '
        'lblNoOptions
        '
        Me.lblNoOptions.AutoSize = True
        Me.lblNoOptions.Location = New System.Drawing.Point(34, 144)
        Me.lblNoOptions.Name = "lblNoOptions"
        Me.lblNoOptions.Size = New System.Drawing.Size(136, 41)
        Me.lblNoOptions.TabIndex = 18
        Me.lblNoOptions.Text = "Richtung"
        '
        'tbtVNW
        '
        Me.tbtVNW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtVNW.Checked = False
        Me.tbtVNW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtVNW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtVNW.Location = New System.Drawing.Point(332, 144)
        Me.tbtVNW.Name = "tbtVNW"
        Me.tbtVNW.Size = New System.Drawing.Size(75, 75)
        Me.tbtVNW.TabIndex = 19
        Me.tbtVNW.Text = "î"
        Me.tbtVNW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtVNW.UseVisualStyleBackColor = False
        '
        'tbtVN
        '
        Me.tbtVN.BackColor = System.Drawing.SystemColors.Control
        Me.tbtVN.Checked = False
        Me.tbtVN.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtVN.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtVN.Location = New System.Drawing.Point(495, 144)
        Me.tbtVN.Name = "tbtVN"
        Me.tbtVN.Size = New System.Drawing.Size(75, 75)
        Me.tbtVN.TabIndex = 20
        Me.tbtVN.Text = "ê"
        Me.tbtVN.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtVN.UseVisualStyleBackColor = False
        '
        'tbtVNO
        '
        Me.tbtVNO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtVNO.Checked = False
        Me.tbtVNO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtVNO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtVNO.Location = New System.Drawing.Point(658, 144)
        Me.tbtVNO.Name = "tbtVNO"
        Me.tbtVNO.Size = New System.Drawing.Size(75, 75)
        Me.tbtVNO.TabIndex = 21
        Me.tbtVNO.Text = "í"
        Me.tbtVNO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtVNO.UseVisualStyleBackColor = False
        '
        'tbtVW
        '
        Me.tbtVW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtVW.Checked = False
        Me.tbtVW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtVW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtVW.Location = New System.Drawing.Point(332, 270)
        Me.tbtVW.Name = "tbtVW"
        Me.tbtVW.Size = New System.Drawing.Size(75, 75)
        Me.tbtVW.TabIndex = 22
        Me.tbtVW.Text = "è"
        Me.tbtVW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtVW.UseVisualStyleBackColor = False
        '
        'tbtVO
        '
        Me.tbtVO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtVO.Checked = False
        Me.tbtVO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtVO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtVO.Location = New System.Drawing.Point(658, 270)
        Me.tbtVO.Name = "tbtVO"
        Me.tbtVO.Size = New System.Drawing.Size(75, 75)
        Me.tbtVO.TabIndex = 23
        Me.tbtVO.Text = "ç"
        Me.tbtVO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtVO.UseVisualStyleBackColor = False
        '
        'tbtVSW
        '
        Me.tbtVSW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtVSW.Checked = False
        Me.tbtVSW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtVSW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtVSW.Location = New System.Drawing.Point(332, 396)
        Me.tbtVSW.Name = "tbtVSW"
        Me.tbtVSW.Size = New System.Drawing.Size(75, 75)
        Me.tbtVSW.TabIndex = 24
        Me.tbtVSW.Text = "ì"
        Me.tbtVSW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtVSW.UseVisualStyleBackColor = False
        '
        'tbtVS
        '
        Me.tbtVS.BackColor = System.Drawing.SystemColors.Control
        Me.tbtVS.Checked = False
        Me.tbtVS.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtVS.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtVS.Location = New System.Drawing.Point(495, 396)
        Me.tbtVS.Name = "tbtVS"
        Me.tbtVS.Size = New System.Drawing.Size(75, 75)
        Me.tbtVS.TabIndex = 25
        Me.tbtVS.Text = "é"
        Me.tbtVS.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtVS.UseVisualStyleBackColor = False
        '
        'tbtVSO
        '
        Me.tbtVSO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtVSO.Checked = False
        Me.tbtVSO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtVSO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtVSO.Location = New System.Drawing.Point(658, 396)
        Me.tbtVSO.Name = "tbtVSO"
        Me.tbtVSO.Size = New System.Drawing.Size(75, 75)
        Me.tbtVSO.TabIndex = 26
        Me.tbtVSO.Text = "ë"
        Me.tbtVSO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtVSO.UseVisualStyleBackColor = False
        '
        'lblKeineRichtungInfo
        '
        Me.lblKeineRichtungInfo.AutoSize = True
        Me.lblKeineRichtungInfo.Location = New System.Drawing.Point(325, 485)
        Me.lblKeineRichtungInfo.MaximumSize = New System.Drawing.Size(600, 0)
        Me.lblKeineRichtungInfo.Name = "lblKeineRichtungInfo"
        Me.lblKeineRichtungInfo.Size = New System.Drawing.Size(527, 82)
        Me.lblKeineRichtungInfo.TabIndex = 27
        Me.lblKeineRichtungInfo.Text = "Keine Richtung ausgewählt, verwende zufällige Richtung"
        '
        'lblNtrbGeschwindigkeit
        '
        Me.lblNtrbGeschwindigkeit.AutoSize = True
        Me.lblNtrbGeschwindigkeit.Location = New System.Drawing.Point(41, 606)
        Me.lblNtrbGeschwindigkeit.Name = "lblNtrbGeschwindigkeit"
        Me.lblNtrbGeschwindigkeit.Size = New System.Drawing.Size(236, 41)
        Me.lblNtrbGeschwindigkeit.TabIndex = 28
        Me.lblNtrbGeschwindigkeit.Text = "Geschwindigkeit"
        '
        'trbGeschwindigkeit
        '
        Me.trbGeschwindigkeit.Location = New System.Drawing.Point(332, 606)
        Me.trbGeschwindigkeit.Maximum = 100
        Me.trbGeschwindigkeit.Minimum = 1
        Me.trbGeschwindigkeit.Name = "trbGeschwindigkeit"
        Me.trbGeschwindigkeit.Size = New System.Drawing.Size(520, 101)
        Me.trbGeschwindigkeit.TabIndex = 29
        Me.trbGeschwindigkeit.Value = 50
        '
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Location = New System.Drawing.Point(41, 724)
        Me.lblNModus.Name = "lblNModus"
        Me.lblNModus.Size = New System.Drawing.Size(111, 41)
        Me.lblNModus.TabIndex = 33
        Me.lblNModus.Text = "Modus"
        '
        'rbZufall
        '
        Me.rbZufall.AutoSize = True
        Me.rbZufall.Location = New System.Drawing.Point(332, 826)
        Me.rbZufall.Name = "rbZufall"
        Me.rbZufall.Size = New System.Drawing.Size(146, 45)
        Me.rbZufall.TabIndex = 32
        Me.rbZufall.TabStop = True
        Me.rbZufall.Text = "Zufällig"
        Me.rbZufall.UseVisualStyleBackColor = True
        '
        'rbWischen
        '
        Me.rbWischen.AutoSize = True
        Me.rbWischen.Location = New System.Drawing.Point(332, 775)
        Me.rbWischen.Name = "rbWischen"
        Me.rbWischen.Size = New System.Drawing.Size(161, 45)
        Me.rbWischen.TabIndex = 31
        Me.rbWischen.TabStop = True
        Me.rbWischen.Text = "Wischen"
        Me.rbWischen.UseVisualStyleBackColor = True
        '
        'rbSchieben
        '
        Me.rbSchieben.AutoSize = True
        Me.rbSchieben.Location = New System.Drawing.Point(332, 724)
        Me.rbSchieben.Name = "rbSchieben"
        Me.rbSchieben.Size = New System.Drawing.Size(170, 45)
        Me.rbSchieben.TabIndex = 30
        Me.rbSchieben.TabStop = True
        Me.rbSchieben.Text = "Schieben"
        Me.rbSchieben.UseVisualStyleBackColor = True
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblNModus)
        Me.Controls.Add(Me.rbZufall)
        Me.Controls.Add(Me.rbWischen)
        Me.Controls.Add(Me.rbSchieben)
        Me.Controls.Add(Me.trbGeschwindigkeit)
        Me.Controls.Add(Me.lblNtrbGeschwindigkeit)
        Me.Controls.Add(Me.lblKeineRichtungInfo)
        Me.Controls.Add(Me.tbtVSO)
        Me.Controls.Add(Me.tbtVS)
        Me.Controls.Add(Me.tbtVSW)
        Me.Controls.Add(Me.tbtVO)
        Me.Controls.Add(Me.tbtVW)
        Me.Controls.Add(Me.tbtVNO)
        Me.Controls.Add(Me.tbtVN)
        Me.Controls.Add(Me.tbtVNW)
        Me.Controls.Add(Me.lblNoOptions)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNlblTransitionname)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trbGeschwindigkeit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionname As Windows.Forms.Label
    Friend WithEvents lblNoOptions As Windows.Forms.Label
    Friend WithEvents tbtVNW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtVN As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtVNO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtVW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtVO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtVSW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtVS As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtVSO As MyControlsLibrary.ToggleButton
    Friend WithEvents lblKeineRichtungInfo As Windows.Forms.Label
    Friend WithEvents lblNtrbGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents trbGeschwindigkeit As Windows.Forms.TrackBar
    Friend WithEvents lblNModus As Windows.Forms.Label
    Friend WithEvents rbZufall As Windows.Forms.RadioButton
    Friend WithEvents rbWischen As Windows.Forms.RadioButton
    Friend WithEvents rbSchieben As Windows.Forms.RadioButton
End Class
