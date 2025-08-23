<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMessageBildauswahl
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMessageBildauswahl))
        Me.picLogo = New System.Windows.Forms.PictureBox()
Me.picLogo.Tag = "langKey=picLogo"
        Me.btnOK = New System.Windows.Forms.Button()
Me.btnOK.Tag = "langKey=btnOK"
        Me.lblTitel = New System.Windows.Forms.Label()
Me.lblTitel.Tag = "langKey=lblTitel"
        Me.rtxMessage = New System.Windows.Forms.RichTextBox()
Me.rtxMessage.Tag = "langKey=rtxMessage"
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picLogo
        '
        Me.picLogo.Image = Global.SlideShowBildauswahl.My.Resources.Resources.Flying_Kitchen_Aid_Logo_Transparent
        Me.picLogo.Location = New System.Drawing.Point(699, 32)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New System.Drawing.Size(235, 223)
        Me.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogo.TabIndex = 0
        Me.picLogo.TabStop = False
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(380, 537)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(168, 86)
        Me.btnOK.TabIndex = 1
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'lblTitel
        '
        Me.lblTitel.AutoSize = True
        Me.lblTitel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitel.Location = New System.Drawing.Point(47, 32)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(450, 48)
        Me.lblTitel.TabIndex = 2
        Me.lblTitel.Text = "Uuups... ein Tag-Konflikt!"
        '
        'rtxMessage
        '
        Me.rtxMessage.BackColor = System.Drawing.SystemColors.Control
        Me.rtxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxMessage.Font = New System.Drawing.Font("Segoe UI", 10.5!)
        Me.rtxMessage.Location = New System.Drawing.Point(55, 102)
        Me.rtxMessage.Name = "rtxMessage"
        Me.rtxMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
        Me.rtxMessage.Size = New System.Drawing.Size(638, 361)
        Me.rtxMessage.TabIndex = 4
        Me.rtxMessage.Text = resources.GetString("rtxMessage.Text")
        '
        'frmMessageBildauswahl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(971, 645)
        Me.Controls.Add(Me.rtxMessage)
        Me.Controls.Add(Me.lblTitel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.picLogo)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmMessageBildauswahl"
        Me.Text = "Tag-Konflikt"
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents picLogo As Windows.Forms.PictureBox
    Friend WithEvents btnOK As Windows.Forms.Button
    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents rtxMessage As Windows.Forms.RichTextBox
End Class