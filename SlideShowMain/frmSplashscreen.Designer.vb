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
        Me.grpSplashscreen = New System.Windows.Forms.GroupBox()
        Me.lblJahr = New System.Windows.Forms.Label()
        Me.lblAutor = New System.Windows.Forms.Label()
        Me.lblMarketingslogan = New System.Windows.Forms.Label()
        Me.picLogo = New System.Windows.Forms.PictureBox()
        Me.lblTitel = New System.Windows.Forms.Label()
        Me.grpSplashscreen.SuspendLayout()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'grpSplashscreen
        '
        Me.grpSplashscreen.Controls.Add(Me.lblJahr)
        Me.grpSplashscreen.Controls.Add(Me.lblAutor)
        Me.grpSplashscreen.Controls.Add(Me.lblMarketingslogan)
        Me.grpSplashscreen.Controls.Add(Me.picLogo)
        Me.grpSplashscreen.Controls.Add(Me.lblTitel)
        Me.grpSplashscreen.Location = New System.Drawing.Point(16, 29)
        Me.grpSplashscreen.Margin = New System.Windows.Forms.Padding(4)
        Me.grpSplashscreen.Name = "grpSplashscreen"
        Me.grpSplashscreen.Padding = New System.Windows.Forms.Padding(4)
        Me.grpSplashscreen.Size = New System.Drawing.Size(860, 437)
        Me.grpSplashscreen.TabIndex = 1
        Me.grpSplashscreen.TabStop = False
        Me.grpSplashscreen.Tag = "langKey=GroupBox1"
        '
        'lblJahr
        '
        Me.lblJahr.AutoSize = True
        Me.lblJahr.Location = New System.Drawing.Point(39, 381)
        Me.lblJahr.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblJahr.Name = "lblJahr"
        Me.lblJahr.Size = New System.Drawing.Size(82, 41)
        Me.lblJahr.TabIndex = 4
        Me.lblJahr.Tag = "langKey=lblJahr"
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
        Me.lblAutor.Tag = "langKey=lblAutor"
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
        Me.lblMarketingslogan.Tag = "langKey=lblMarketingslogan"
        Me.lblMarketingslogan.Text = "Der modulare Screensaver speziell für Diashows!"
        '
        'picLogo
        '
        Me.picLogo.Image = Global.SlideShowMain.My.Resources.Resources.Flying_Kitchen_Aid_Logo_Transparent
        Me.picLogo.Location = New System.Drawing.Point(548, 134)
        Me.picLogo.Margin = New System.Windows.Forms.Padding(4)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New System.Drawing.Size(311, 300)
        Me.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogo.TabIndex = 1
        Me.picLogo.TabStop = False
        Me.picLogo.Tag = "langKey=PictureBox1"
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
        Me.lblTitel.Tag = "langKey=lblTitel"
        Me.lblTitel.Text = "SlideShowSaver 3.0"
        Me.lblTitel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmSplashscreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(898, 495)
        Me.ControlBox = False
        Me.Controls.Add(Me.grpSplashscreen)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSplashscreen"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Splashscreen"
        Me.grpSplashscreen.ResumeLayout(False)
        Me.grpSplashscreen.PerformLayout()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents grpSplashscreen As GroupBox
    Friend WithEvents lblTitel As Label
    Friend WithEvents lblMarketingslogan As Label
    Friend WithEvents picLogo As PictureBox
    Friend WithEvents lblJahr As Label
    Friend WithEvents lblAutor As Label
End Class