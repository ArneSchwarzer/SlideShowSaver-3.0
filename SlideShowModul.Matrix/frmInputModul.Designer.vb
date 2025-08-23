<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInputModul
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
        Me.btnCancel = New System.Windows.Forms.Button()
Me.btnCancel.Tag = "langKey=btnCancel"
        Me.lblNtxtTag = New System.Windows.Forms.Label()
Me.lblNtxtTag.Tag = "langKey=lblNtxtTag"
        Me.txtTag = New System.Windows.Forms.TextBox()
        Me.lblTitel = New System.Windows.Forms.Label()
Me.lblTitel.Tag = "langKey=lblTitel"
        Me.btnOK = New System.Windows.Forms.Button()
Me.btnOK.Tag = "langKey=btnOK"
        Me.picLogo = New System.Windows.Forms.PictureBox()
Me.picLogo.Tag = "langKey=picLogo"
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(544, 263)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(218, 71)
        Me.btnCancel.TabIndex = 14
        Me.btnCancel.Text = "Abbrechen"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lblNtxtTag
        '
        Me.lblNtxtTag.AutoSize = True
        Me.lblNtxtTag.Location = New System.Drawing.Point(34, 108)
        Me.lblNtxtTag.Name = "lblNtxtTag"
        Me.lblNtxtTag.Size = New System.Drawing.Size(160, 41)
        Me.lblNtxtTag.TabIndex = 13
        Me.lblNtxtTag.Text = "Neuer Text"
        '
        'txtTag
        '
        Me.txtTag.Location = New System.Drawing.Point(206, 105)
        Me.txtTag.Name = "txtTag"
        Me.txtTag.Size = New System.Drawing.Size(317, 47)
        Me.txtTag.TabIndex = 12
        '
        'lblTitel
        '
        Me.lblTitel.AutoSize = True
        Me.lblTitel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitel.Location = New System.Drawing.Point(27, 31)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(407, 48)
        Me.lblTitel.TabIndex = 11
        Me.lblTitel.Text = "Highlighttext einfügen"
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(355, 263)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(168, 71)
        Me.btnOK.TabIndex = 10
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'picLogo
        '
        Me.picLogo.Image = Global.Modul_Matrix.My.Resources.Resources.Flying_KitchenAid_Logo_Matrix_Transparent
        Me.picLogo.Location = New System.Drawing.Point(555, 31)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New System.Drawing.Size(207, 207)
        Me.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogo.TabIndex = 9
        Me.picLogo.TabStop = False
        '
        'frmModulInput
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
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "frmModulInput"
        Me.Text = "Highlighttext eingeben"
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnCancel As Windows.Forms.Button
    Friend WithEvents lblNtxtTag As Windows.Forms.Label
    Friend WithEvents txtTag As Windows.Forms.TextBox
    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents btnOK As Windows.Forms.Button
    Friend WithEvents picLogo As Windows.Forms.PictureBox
End Class
