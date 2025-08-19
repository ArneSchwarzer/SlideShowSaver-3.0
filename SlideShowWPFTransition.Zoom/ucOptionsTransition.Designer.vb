<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucOptionsTransition
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
        Me.lblNlblTransitionname = New System.Windows.Forms.Label()
        Me.lblNAnkerpunkt = New System.Windows.Forms.Label()
        Me.lblKeinAnkerpunkt = New System.Windows.Forms.Label()
        Me.lblNtrbGeschwindigkeit = New System.Windows.Forms.Label()
        Me.trkGeschwindigkeit = New System.Windows.Forms.TrackBar()
        Me.lblGeschwindigkeit = New System.Windows.Forms.Label()
        Me.tbtNW = New MyControlsLibrary.ToggleButton()
        Me.tbtN = New MyControlsLibrary.ToggleButton()
        Me.tbtNO = New MyControlsLibrary.ToggleButton()
        Me.tbtW = New MyControlsLibrary.ToggleButton()
        Me.tbtO = New MyControlsLibrary.ToggleButton()
        Me.tbtSW = New MyControlsLibrary.ToggleButton()
        Me.tbtS = New MyControlsLibrary.ToggleButton()
        Me.tbtSO = New MyControlsLibrary.ToggleButton()
        Me.tbtZ = New MyControlsLibrary.ToggleButton()
        Me.chkGleicherAnkerpunkt = New System.Windows.Forms.CheckBox()
        Me.btnDefaults = New System.Windows.Forms.Button()
        CType(Me.trkGeschwindigkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblShadername.Location = New System.Drawing.Point(325, 26)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(99, 41)
        Me.lblShadername.TabIndex = 17
        Me.lblShadername.Text = "Zoom"
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
        'lblNAnkerpunkt
        '
        Me.lblNAnkerpunkt.AutoSize = True
        Me.lblNAnkerpunkt.Location = New System.Drawing.Point(34, 144)
        Me.lblNAnkerpunkt.Name = "lblNAnkerpunkt"
        Me.lblNAnkerpunkt.Size = New System.Drawing.Size(188, 41)
        Me.lblNAnkerpunkt.TabIndex = 18
        Me.lblNAnkerpunkt.Text = "Ankerpunkte"
        '
        'lblKeinAnkerpunkt
        '
        Me.lblKeinAnkerpunkt.AutoSize = True
        Me.lblKeinAnkerpunkt.Location = New System.Drawing.Point(325, 485)
        Me.lblKeinAnkerpunkt.MaximumSize = New System.Drawing.Size(600, 0)
        Me.lblKeinAnkerpunkt.Name = "lblKeinAnkerpunkt"
        Me.lblKeinAnkerpunkt.Size = New System.Drawing.Size(547, 82)
        Me.lblKeinAnkerpunkt.TabIndex = 27
        Me.lblKeinAnkerpunkt.Text = "Kein Ankerpunkt ausgewählt, verwende zufälligen Ankerpunkt"
        '
        'lblNtrbGeschwindigkeit
        '
        Me.lblNtrbGeschwindigkeit.AutoSize = True
        Me.lblNtrbGeschwindigkeit.Location = New System.Drawing.Point(34, 728)
        Me.lblNtrbGeschwindigkeit.Name = "lblNtrbGeschwindigkeit"
        Me.lblNtrbGeschwindigkeit.Size = New System.Drawing.Size(236, 41)
        Me.lblNtrbGeschwindigkeit.TabIndex = 28
        Me.lblNtrbGeschwindigkeit.Text = "Geschwindigkeit"
        '
        'trkGeschwindigkeit
        '
        Me.trkGeschwindigkeit.AutoSize = False
        Me.trkGeschwindigkeit.Location = New System.Drawing.Point(325, 728)
        Me.trkGeschwindigkeit.Maximum = 30
        Me.trkGeschwindigkeit.Minimum = 1
        Me.trkGeschwindigkeit.Name = "trkGeschwindigkeit"
        Me.trkGeschwindigkeit.Size = New System.Drawing.Size(456, 53)
        Me.trkGeschwindigkeit.TabIndex = 29
        Me.trkGeschwindigkeit.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkGeschwindigkeit.Value = 15
        '
        'lblGeschwindigkeit
        '
        Me.lblGeschwindigkeit.AutoSize = True
        Me.lblGeschwindigkeit.Location = New System.Drawing.Point(788, 728)
        Me.lblGeschwindigkeit.MinimumSize = New System.Drawing.Size(71, 0)
        Me.lblGeschwindigkeit.Name = "lblGeschwindigkeit"
        Me.lblGeschwindigkeit.Size = New System.Drawing.Size(71, 41)
        Me.lblGeschwindigkeit.TabIndex = 34
        Me.lblGeschwindigkeit.Text = "15 s"
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
        Me.tbtNW.Text = "l"
        Me.tbtNW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtNW.UseVisualStyleBackColor = False
        '
        'tbtN
        '
        Me.tbtN.BackColor = System.Drawing.SystemColors.Control
        Me.tbtN.Checked = False
        Me.tbtN.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtN.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtN.Location = New System.Drawing.Point(553, 144)
        Me.tbtN.Name = "tbtN"
        Me.tbtN.Size = New System.Drawing.Size(75, 75)
        Me.tbtN.TabIndex = 36
        Me.tbtN.Text = "l"
        Me.tbtN.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtN.UseVisualStyleBackColor = False
        '
        'tbtNO
        '
        Me.tbtNO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtNO.Checked = False
        Me.tbtNO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtNO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtNO.Location = New System.Drawing.Point(774, 144)
        Me.tbtNO.Name = "tbtNO"
        Me.tbtNO.Size = New System.Drawing.Size(75, 75)
        Me.tbtNO.TabIndex = 37
        Me.tbtNO.Text = "l"
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
        Me.tbtW.Text = "l"
        Me.tbtW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtW.UseVisualStyleBackColor = False
        '
        'tbtO
        '
        Me.tbtO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtO.Checked = False
        Me.tbtO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtO.Location = New System.Drawing.Point(775, 270)
        Me.tbtO.Name = "tbtO"
        Me.tbtO.Size = New System.Drawing.Size(75, 75)
        Me.tbtO.TabIndex = 39
        Me.tbtO.Text = "l"
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
        Me.tbtSW.Text = "l"
        Me.tbtSW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtSW.UseVisualStyleBackColor = False
        '
        'tbtS
        '
        Me.tbtS.BackColor = System.Drawing.SystemColors.Control
        Me.tbtS.Checked = False
        Me.tbtS.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtS.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtS.Location = New System.Drawing.Point(553, 396)
        Me.tbtS.Name = "tbtS"
        Me.tbtS.Size = New System.Drawing.Size(75, 75)
        Me.tbtS.TabIndex = 41
        Me.tbtS.Text = "l"
        Me.tbtS.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtS.UseVisualStyleBackColor = False
        '
        'tbtSO
        '
        Me.tbtSO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtSO.Checked = False
        Me.tbtSO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtSO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtSO.Location = New System.Drawing.Point(775, 396)
        Me.tbtSO.Name = "tbtSO"
        Me.tbtSO.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tbtSO.Size = New System.Drawing.Size(75, 75)
        Me.tbtSO.TabIndex = 42
        Me.tbtSO.Text = "l"
        Me.tbtSO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtSO.UseVisualStyleBackColor = False
        '
        'tbtZ
        '
        Me.tbtZ.BackColor = System.Drawing.SystemColors.Control
        Me.tbtZ.Checked = False
        Me.tbtZ.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtZ.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtZ.Location = New System.Drawing.Point(553, 270)
        Me.tbtZ.Name = "tbtZ"
        Me.tbtZ.Size = New System.Drawing.Size(75, 75)
        Me.tbtZ.TabIndex = 43
        Me.tbtZ.Text = "l"
        Me.tbtZ.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtZ.UseVisualStyleBackColor = False
        '
        'chkGleicherAnkerpunkt
        '
        Me.chkGleicherAnkerpunkt.AutoSize = True
        Me.chkGleicherAnkerpunkt.Location = New System.Drawing.Point(332, 586)
        Me.chkGleicherAnkerpunkt.MaximumSize = New System.Drawing.Size(550, 0)
        Me.chkGleicherAnkerpunkt.MinimumSize = New System.Drawing.Size(0, 90)
        Me.chkGleicherAnkerpunkt.Name = "chkGleicherAnkerpunkt"
        Me.chkGleicherAnkerpunkt.Size = New System.Drawing.Size(550, 90)
        Me.chkGleicherAnkerpunkt.TabIndex = 44
        Me.chkGleicherAnkerpunkt.Text = "Gleichen Ankerpunkt für Einblenden und Ausblenden verwenden"
        Me.chkGleicherAnkerpunkt.TextAlign = System.Drawing.ContentAlignment.TopLeft
        Me.chkGleicherAnkerpunkt.UseVisualStyleBackColor = True
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(671, 19)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(188, 54)
        Me.btnDefaults.TabIndex = 45
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.chkGleicherAnkerpunkt)
        Me.Controls.Add(Me.tbtZ)
        Me.Controls.Add(Me.tbtSO)
        Me.Controls.Add(Me.tbtS)
        Me.Controls.Add(Me.tbtSW)
        Me.Controls.Add(Me.tbtO)
        Me.Controls.Add(Me.tbtW)
        Me.Controls.Add(Me.tbtNO)
        Me.Controls.Add(Me.tbtN)
        Me.Controls.Add(Me.tbtNW)
        Me.Controls.Add(Me.lblGeschwindigkeit)
        Me.Controls.Add(Me.trkGeschwindigkeit)
        Me.Controls.Add(Me.lblNtrbGeschwindigkeit)
        Me.Controls.Add(Me.lblKeinAnkerpunkt)
        Me.Controls.Add(Me.lblNAnkerpunkt)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNlblTransitionname)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkGeschwindigkeit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionname As Windows.Forms.Label
    Friend WithEvents lblNAnkerpunkt As Windows.Forms.Label
    Friend WithEvents lblKeinAnkerpunkt As Windows.Forms.Label
    Friend WithEvents lblNtrbGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents trkGeschwindigkeit As Windows.Forms.TrackBar
    Friend WithEvents lblGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents tbtNW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtN As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtNO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtSW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtS As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtSO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtZ As MyControlsLibrary.ToggleButton
    Friend WithEvents chkGleicherAnkerpunkt As Forms.CheckBox
    Friend WithEvents btnDefaults As Forms.Button
End Class
