<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucLanguageSelector
    Inherits System.Windows.Forms.UserControl

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblSprache = New System.Windows.Forms.Label()
        Me.cboSprache = New System.Windows.Forms.ComboBox()
        Me.TITLE = New System.Windows.Forms.Label()
        Me.BTN_OK = New System.Windows.Forms.Button()
        Me.BTN_CANCEL = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblSprache
        '
        Me.lblSprache.AutoSize = True
        Me.lblSprache.Location = New System.Drawing.Point(26, 104)
        Me.lblSprache.Name = "lblSprache"
        Me.lblSprache.Size = New System.Drawing.Size(226, 41)
        Me.lblSprache.TabIndex = 0
        Me.lblSprache.Tag = "LBL_LANGUAGE"
        Me.lblSprache.Text = "Sprache wählen"
        '
        'cboSprache
        '
        Me.cboSprache.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSprache.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cboSprache.FormattingEnabled = True
        Me.cboSprache.Location = New System.Drawing.Point(278, 104)
        Me.cboSprache.Name = "cboSprache"
        Me.cboSprache.Size = New System.Drawing.Size(121, 49)
        Me.cboSprache.TabIndex = 1
        '
        'TITLE
        '
        Me.TITLE.AutoSize = True
        Me.TITLE.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TITLE.Location = New System.Drawing.Point(71, 26)
        Me.TITLE.Name = "TITLE"
        Me.TITLE.Size = New System.Drawing.Size(276, 41)
        Me.TITLE.TabIndex = 2
        Me.TITLE.Tag = "TITLE"
        Me.TITLE.Text = "Bildschirmschoner"
        '
        'BTN_OK
        '
        Me.BTN_OK.Location = New System.Drawing.Point(51, 218)
        Me.BTN_OK.Name = "BTN_OK"
        Me.BTN_OK.Size = New System.Drawing.Size(134, 62)
        Me.BTN_OK.TabIndex = 3
        Me.BTN_OK.Tag = "BTN_OK"
        Me.BTN_OK.Text = "Button1"
        Me.BTN_OK.UseVisualStyleBackColor = True
        '
        'BTN_CANCEL
        '
        Me.BTN_CANCEL.Location = New System.Drawing.Point(237, 218)
        Me.BTN_CANCEL.Name = "BTN_CANCEL"
        Me.BTN_CANCEL.Size = New System.Drawing.Size(134, 62)
        Me.BTN_CANCEL.TabIndex = 4
        Me.BTN_CANCEL.Text = "Button2"
        Me.BTN_CANCEL.UseVisualStyleBackColor = True
        '
        'ucLanguageSelector
        '
        Me.Controls.Add(Me.BTN_CANCEL)
        Me.Controls.Add(Me.BTN_OK)
        Me.Controls.Add(Me.TITLE)
        Me.Controls.Add(Me.cboSprache)
        Me.Controls.Add(Me.lblSprache)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "ucLanguageSelector"
        Me.Size = New System.Drawing.Size(439, 357)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblSprache As Label
    Friend WithEvents cboSprache As ComboBox
    Friend WithEvents TITLE As Label
    Friend WithEvents BTN_OK As Button
    Friend WithEvents BTN_CANCEL As Button
End Class

Me.lblSprache.Tag = "langKey=lblSprache"
Me.TITLE.Tag = "langKey=TITLE"
Me.BTN_OK.Tag = "langKey=BTN_OK"
Me.BTN_CANCEL.Tag = "langKey=BTN_CANCEL"
Me.cboSprache.Tag = "langKey=cboSprache"