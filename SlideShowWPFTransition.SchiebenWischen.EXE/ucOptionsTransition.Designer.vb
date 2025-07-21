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
        Me.lblKeineRichtungInfo = New System.Windows.Forms.Label()
        Me.lblNtrbGeschwindigkeit = New System.Windows.Forms.Label()
        Me.trkGeschwindigkeit = New System.Windows.Forms.TrackBar()
        Me.lblNModus = New System.Windows.Forms.Label()
        Me.rbZufall = New System.Windows.Forms.RadioButton()
        Me.rbWischen = New System.Windows.Forms.RadioButton()
        Me.rbSchieben = New System.Windows.Forms.RadioButton()
        Me.lblGeschwindigkeit = New System.Windows.Forms.Label()
        Me.tbtNW = New MyControlsLibrary.ToggleButton()
        Me.tbtN = New MyControlsLibrary.ToggleButton()
        Me.tbtNO = New MyControlsLibrary.ToggleButton()
        Me.tbtW = New MyControlsLibrary.ToggleButton()
        Me.tbtO = New MyControlsLibrary.ToggleButton()
        Me.tbtSW = New MyControlsLibrary.ToggleButton()
        Me.tbtS = New MyControlsLibrary.ToggleButton()
        Me.tbtSO = New MyControlsLibrary.ToggleButton()
        Me.nudFPS = New System.Windows.Forms.NumericUpDown()
        Me.lblNnudFPS = New System.Windows.Forms.Label()
        CType(Me.trkGeschwindigkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudFPS, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.lblNoOptions.Size = New System.Drawing.Size(169, 41)
        Me.lblNoOptions.TabIndex = 18
        Me.lblNoOptions.Text = "Richtungen"
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
        'trkGeschwindigkeit
        '
        Me.trkGeschwindigkeit.Location = New System.Drawing.Point(332, 606)
        Me.trkGeschwindigkeit.Minimum = 1
        Me.trkGeschwindigkeit.Name = "trkGeschwindigkeit"
        Me.trkGeschwindigkeit.Size = New System.Drawing.Size(456, 101)
        Me.trkGeschwindigkeit.TabIndex = 29
        Me.trkGeschwindigkeit.Value = 3
        '
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Location = New System.Drawing.Point(41, 785)
        Me.lblNModus.Name = "lblNModus"
        Me.lblNModus.Size = New System.Drawing.Size(111, 41)
        Me.lblNModus.TabIndex = 33
        Me.lblNModus.Text = "Modus"
        '
        'rbZufall
        '
        Me.rbZufall.AutoSize = True
        Me.rbZufall.Location = New System.Drawing.Point(332, 887)
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
        Me.rbWischen.Location = New System.Drawing.Point(332, 836)
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
        Me.rbSchieben.Location = New System.Drawing.Point(332, 785)
        Me.rbSchieben.Name = "rbSchieben"
        Me.rbSchieben.Size = New System.Drawing.Size(170, 45)
        Me.rbSchieben.TabIndex = 30
        Me.rbSchieben.TabStop = True
        Me.rbSchieben.Text = "Schieben"
        Me.rbSchieben.UseVisualStyleBackColor = True
        '
        'lblGeschwindigkeit
        '
        Me.lblGeschwindigkeit.AutoSize = True
        Me.lblGeschwindigkeit.Location = New System.Drawing.Point(795, 606)
        Me.lblGeschwindigkeit.MinimumSize = New System.Drawing.Size(71, 0)
        Me.lblGeschwindigkeit.Name = "lblGeschwindigkeit"
        Me.lblGeschwindigkeit.Size = New System.Drawing.Size(71, 41)
        Me.lblGeschwindigkeit.TabIndex = 34
        Me.lblGeschwindigkeit.Text = "10 s"
        Me.lblGeschwindigkeit.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'tbtNW
        '
        Me.tbtNW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtNW.Checked = False
        Me.tbtNW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtNW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtNW.Location = New System.Drawing.Point(332, 144)
        Me.tbtNW.Name = "tbtNW"
        Me.tbtNW.Size = New System.Drawing.Size(75, 75)
        Me.tbtNW.TabIndex = 35
        Me.tbtNW.Text = "î"
        Me.tbtNW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtNW.UseVisualStyleBackColor = False
        '
        'tbtN
        '
        Me.tbtN.BackColor = System.Drawing.SystemColors.Control
        Me.tbtN.Checked = False
        Me.tbtN.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtN.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtN.Location = New System.Drawing.Point(495, 144)
        Me.tbtN.Name = "tbtN"
        Me.tbtN.Size = New System.Drawing.Size(75, 75)
        Me.tbtN.TabIndex = 36
        Me.tbtN.Text = "ê"
        Me.tbtN.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtN.UseVisualStyleBackColor = False
        '
        'tbtNO
        '
        Me.tbtNO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtNO.Checked = False
        Me.tbtNO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtNO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtNO.Location = New System.Drawing.Point(658, 144)
        Me.tbtNO.Name = "tbtNO"
        Me.tbtNO.Size = New System.Drawing.Size(75, 75)
        Me.tbtNO.TabIndex = 37
        Me.tbtNO.Text = "í"
        Me.tbtNO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtNO.UseVisualStyleBackColor = False
        '
        'tbtW
        '
        Me.tbtW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtW.Checked = False
        Me.tbtW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtW.Location = New System.Drawing.Point(332, 270)
        Me.tbtW.Name = "tbtW"
        Me.tbtW.Size = New System.Drawing.Size(75, 75)
        Me.tbtW.TabIndex = 38
        Me.tbtW.Text = "è"
        Me.tbtW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtW.UseVisualStyleBackColor = False
        '
        'tbtO
        '
        Me.tbtO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtO.Checked = False
        Me.tbtO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtO.Location = New System.Drawing.Point(658, 270)
        Me.tbtO.Name = "tbtO"
        Me.tbtO.Size = New System.Drawing.Size(75, 75)
        Me.tbtO.TabIndex = 39
        Me.tbtO.Text = "ç"
        Me.tbtO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtO.UseVisualStyleBackColor = False
        '
        'tbtSW
        '
        Me.tbtSW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtSW.Checked = False
        Me.tbtSW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtSW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtSW.Location = New System.Drawing.Point(332, 396)
        Me.tbtSW.Name = "tbtSW"
        Me.tbtSW.Size = New System.Drawing.Size(75, 75)
        Me.tbtSW.TabIndex = 40
        Me.tbtSW.Text = "ì"
        Me.tbtSW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtSW.UseVisualStyleBackColor = False
        '
        'tbtS
        '
        Me.tbtS.BackColor = System.Drawing.SystemColors.Control
        Me.tbtS.Checked = False
        Me.tbtS.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtS.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtS.Location = New System.Drawing.Point(495, 396)
        Me.tbtS.Name = "tbtS"
        Me.tbtS.Size = New System.Drawing.Size(75, 75)
        Me.tbtS.TabIndex = 41
        Me.tbtS.Text = "é"
        Me.tbtS.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtS.UseVisualStyleBackColor = False
        '
        'tbtSO
        '
        Me.tbtSO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtSO.Checked = False
        Me.tbtSO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtSO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtSO.Location = New System.Drawing.Point(658, 396)
        Me.tbtSO.Name = "tbtSO"
        Me.tbtSO.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tbtSO.Size = New System.Drawing.Size(75, 75)
        Me.tbtSO.TabIndex = 42
        Me.tbtSO.Text = "ë"
        Me.tbtSO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtSO.UseVisualStyleBackColor = False
        '
        'nudFPS
        '
        Me.nudFPS.Location = New System.Drawing.Point(332, 716)
        Me.nudFPS.Maximum = New Decimal(New Integer() {120, 0, 0, 0})
        Me.nudFPS.Minimum = New Decimal(New Integer() {5, 0, 0, 0})
        Me.nudFPS.Name = "nudFPS"
        Me.nudFPS.Size = New System.Drawing.Size(120, 47)
        Me.nudFPS.TabIndex = 43
        Me.nudFPS.Value = New Decimal(New Integer() {25, 0, 0, 0})
        '
        'lblNnudFPS
        '
        Me.lblNnudFPS.AutoSize = True
        Me.lblNnudFPS.Location = New System.Drawing.Point(48, 716)
        Me.lblNnudFPS.Name = "lblNnudFPS"
        Me.lblNnudFPS.Size = New System.Drawing.Size(66, 41)
        Me.lblNnudFPS.TabIndex = 44
        Me.lblNnudFPS.Text = "FPS"
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblNnudFPS)
        Me.Controls.Add(Me.nudFPS)
        Me.Controls.Add(Me.tbtSO)
        Me.Controls.Add(Me.tbtS)
        Me.Controls.Add(Me.tbtSW)
        Me.Controls.Add(Me.tbtO)
        Me.Controls.Add(Me.tbtW)
        Me.Controls.Add(Me.tbtNO)
        Me.Controls.Add(Me.tbtN)
        Me.Controls.Add(Me.tbtNW)
        Me.Controls.Add(Me.lblGeschwindigkeit)
        Me.Controls.Add(Me.lblNModus)
        Me.Controls.Add(Me.rbZufall)
        Me.Controls.Add(Me.rbWischen)
        Me.Controls.Add(Me.rbSchieben)
        Me.Controls.Add(Me.trkGeschwindigkeit)
        Me.Controls.Add(Me.lblNtrbGeschwindigkeit)
        Me.Controls.Add(Me.lblKeineRichtungInfo)
        Me.Controls.Add(Me.lblNoOptions)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNlblTransitionname)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkGeschwindigkeit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudFPS, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionname As Windows.Forms.Label
    Friend WithEvents lblNoOptions As Windows.Forms.Label
    Friend WithEvents lblKeineRichtungInfo As Windows.Forms.Label
    Friend WithEvents lblNtrbGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents trkGeschwindigkeit As Windows.Forms.TrackBar
    Friend WithEvents lblNModus As Windows.Forms.Label
    Friend WithEvents rbZufall As Windows.Forms.RadioButton
    Friend WithEvents rbWischen As Windows.Forms.RadioButton
    Friend WithEvents rbSchieben As Windows.Forms.RadioButton
    Friend WithEvents lblGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents tbtNW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtN As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtNO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtSW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtS As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtSO As MyControlsLibrary.ToggleButton
    Friend WithEvents nudFPS As Windows.Forms.NumericUpDown
    Friend WithEvents lblNnudFPS As Windows.Forms.Label
End Class
