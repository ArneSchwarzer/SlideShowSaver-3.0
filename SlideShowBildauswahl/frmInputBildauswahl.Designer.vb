<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInputBildauswahl
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
Me.lblTitel.Tag = "langKey=lblTitel"
        Me.btnOK = New System.Windows.Forms.Button()
Me.btnOK.Tag = "langKey=btnOK"
        Me.picLogo = New System.Windows.Forms.PictureBox()
Me.picLogo.Tag = "langKey=picLogo"
        Me.txtTag = New System.Windows.Forms.TextBox()
        Me.lblNtxtTag = New System.Windows.Forms.Label()
Me.lblNtxtTag.Tag = "langKey=lblNtxtTag"
        Me.btnCancel = New System.Windows.Forms.Button()
Me.btnCancel.Tag = "langKey=btnCancel"
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitel
        '
        Me.lblTitel.AutoSize = True
        Me.lblTitel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitel.Location = New System.Drawing.Point(21, 29)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(239, 48)
        Me.lblTitel.TabIndex = 5
        Me.lblTitel.Text = "Tag einfügen"
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(349, 261)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(168, 71)
        Me.btnOK.TabIndex = 4
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'picLogo
        '
        Me.picLogo.Image = Global.SlideShowBildauswahl.My.Resources.Resources.Flying_Kitchen_Aid_Logo_Transparent
        Me.picLogo.Location = New System.Drawing.Point(549, 29)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New System.Drawing.Size(207, 207)
        Me.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogo.TabIndex = 3
        Me.picLogo.TabStop = False
        '
        'txtTag
        '
        Me.txtTag.Location = New System.Drawing.Point(200, 103)
        Me.txtTag.Name = "txtTag"
        Me.txtTag.Size = New System.Drawing.Size(317, 47)
        Me.txtTag.TabIndex = 6
        '
        'lblNtxtTag
        '
        Me.lblNtxtTag.AutoSize = True
        Me.lblNtxtTag.Location = New System.Drawing.Point(28, 106)
        Me.lblNtxtTag.Name = "lblNtxtTag"
        Me.lblNtxtTag.Size = New System.Drawing.Size(156, 41)
        Me.lblNtxtTag.TabIndex = 7
        Me.lblNtxtTag.Text = "Neues Tag"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(538, 261)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(218, 71)
        Me.btnCancel.TabIndex = 8
        Me.btnCancel.Text = "Abbrechen"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmInputBildauswahl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(788, 364)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.lblNtxtTag)
        Me.Controls.Add(Me.txtTag)
        Me.Controls.Add(Me.lblTitel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.picLogo)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmInputBildauswahl"
        Me.Text = "Tag hinzufügen"
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents btnOK As Windows.Forms.Button
    Friend WithEvents picLogo As Windows.Forms.PictureBox
    Friend WithEvents txtTag As Windows.Forms.TextBox
    Friend WithEvents lblNtxtTag As Windows.Forms.Label
    Friend WithEvents btnCancel As Windows.Forms.Button
End Class