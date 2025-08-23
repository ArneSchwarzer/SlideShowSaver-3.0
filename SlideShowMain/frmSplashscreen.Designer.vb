<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSplashscreen
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
Me.GroupBox1.Tag = "langKey=GroupBox1"
        Me.lblJahr = New System.Windows.Forms.Label()
Me.lblJahr.Tag = "langKey=lblJahr"
        Me.lblAutor = New System.Windows.Forms.Label()
Me.lblAutor.Tag = "langKey=lblAutor"
        Me.lblMarketingslogan = New System.Windows.Forms.Label()
Me.lblMarketingslogan.Tag = "langKey=lblMarketingslogan"
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
Me.PictureBox1.Tag = "langKey=PictureBox1"
        Me.lblTitel = New System.Windows.Forms.Label()
Me.lblTitel.Tag = "langKey=lblTitel"
        Me.GroupBox1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lblJahr)
        Me.GroupBox1.Controls.Add(Me.lblAutor)
        Me.GroupBox1.Controls.Add(Me.lblMarketingslogan)
        Me.GroupBox1.Controls.Add(Me.PictureBox1)
        Me.GroupBox1.Controls.Add(Me.lblTitel)
        Me.GroupBox1.Location = New System.Drawing.Point(16, 29)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(860, 437)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        '
        'lblJahr
        '
        Me.lblJahr.AutoSize = True
        Me.lblJahr.Location = New System.Drawing.Point(39, 381)
        Me.lblJahr.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblJahr.Name = "lblJahr"
        Me.lblJahr.Size = New System.Drawing.Size(82, 41)
        Me.lblJahr.TabIndex = 4
        Me.lblJahr.Text = "2025"
        '
        'lblAutor
        '
        Me.lblAutor.AutoSize = True
        Me.lblAutor.Location = New System.Drawing.Point(4, 340)
        Me.lblAutor.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblAutor.Name = "lblAutor"
        Me.lblAutor.Size = New System.Drawing.Size(257, 41)
        Me.lblAutor.TabIndex = 3
        Me.lblAutor.Text = "© Arne Schwarzer"
        '
        'lblMarketingslogan
        '
        Me.lblMarketingslogan.AutoSize = True
        Me.lblMarketingslogan.Location = New System.Drawing.Point(17, 134)
        Me.lblMarketingslogan.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMarketingslogan.MaximumSize = New System.Drawing.Size(607, 141)
        Me.lblMarketingslogan.Name = "lblMarketingslogan"
        Me.lblMarketingslogan.Size = New System.Drawing.Size(523, 82)
        Me.lblMarketingslogan.TabIndex = 2
        Me.lblMarketingslogan.Text = "Der modulare Screensaver speziell für Diashows!"
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.SlideShowMain.My.Resources.Resources.Flying_Kitchen_Aid_Logo_Transparent
        Me.PictureBox1.Location = New System.Drawing.Point(548, 134)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(311, 300)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'lblTitel
        '
        Me.lblTitel.AutoSize = True
        Me.lblTitel.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.lblTitel.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitel.Location = New System.Drawing.Point(8, 18)
        Me.lblTitel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(695, 96)
        Me.lblTitel.TabIndex = 0
        Me.lblTitel.Text = "SlideShowSaver 3.0"
        Me.lblTitel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmSplashscreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(898, 495)
        Me.ControlBox = False
        Me.Controls.Add(Me.GroupBox1)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSplashscreen"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Splashscreen"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lblTitel As Label
    Friend WithEvents lblMarketingslogan As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents lblJahr As Label
    Friend WithEvents lblAutor As Label
End Class