<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucUnderConstruction
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
        Me.panPanel = New System.Windows.Forms.Panel()
        Me.picLogo = New System.Windows.Forms.PictureBox()
Me.picLogo.Tag = "langKey=picLogo"
        Me.lblModulname = New System.Windows.Forms.Label()
        Me.lblHierEntsteht = New System.Windows.Forms.Label()
        Me.lblUnderConstruction = New System.Windows.Forms.Label()
        Me.panPanel.SuspendLayout()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panPanel
        '
        Me.panPanel.BackColor = System.Drawing.Color.Transparent
        Me.panPanel.Controls.Add(Me.picLogo)
        Me.panPanel.Controls.Add(Me.lblModulname)
        Me.panPanel.Controls.Add(Me.lblHierEntsteht)
        Me.panPanel.Controls.Add(Me.lblUnderConstruction)
        Me.panPanel.Location = New System.Drawing.Point(5, 6)
        Me.panPanel.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.panPanel.MaximumSize = New System.Drawing.Size(1024, 768)
        Me.panPanel.MinimumSize = New System.Drawing.Size(1024, 768)
        Me.panPanel.Name = "panPanel"
        Me.panPanel.Size = New System.Drawing.Size(1024, 768)
        Me.panPanel.TabIndex = 6
        '
        'picLogo
        '
        Me.picLogo.BackgroundImage = Global.Modul_Matrix.My.Resources.Resources.Matrix_SplashScreen
        Me.picLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.picLogo.Location = New System.Drawing.Point(0, 317)
        Me.picLogo.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.picLogo.MaximumSize = New System.Drawing.Size(1024, 768)
        Me.picLogo.MinimumSize = New System.Drawing.Size(1024, 0)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New System.Drawing.Size(1024, 414)
        Me.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogo.TabIndex = 5
        Me.picLogo.TabStop = False
        '
        'lblModulname
        '
        Me.lblModulname.AutoSize = True
        Me.lblModulname.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblModulname.ForeColor = System.Drawing.Color.Blue
        Me.lblModulname.Location = New System.Drawing.Point(0, 178)
        Me.lblModulname.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblModulname.MaximumSize = New System.Drawing.Size(1024, 768)
        Me.lblModulname.MinimumSize = New System.Drawing.Size(1024, 0)
        Me.lblModulname.Name = "lblModulname"
        Me.lblModulname.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.lblModulname.Size = New System.Drawing.Size(1024, 96)
        Me.lblModulname.TabIndex = 5
        Me.lblModulname.Tag = "langKey=lblModulname"
        Me.lblModulname.Text = "Matrix"
        Me.lblModulname.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblHierEntsteht
        '
        Me.lblHierEntsteht.AutoSize = True
        Me.lblHierEntsteht.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblHierEntsteht.ForeColor = System.Drawing.Color.Blue
        Me.lblHierEntsteht.Location = New System.Drawing.Point(0, 100)
        Me.lblHierEntsteht.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblHierEntsteht.MaximumSize = New System.Drawing.Size(1024, 768)
        Me.lblHierEntsteht.MinimumSize = New System.Drawing.Size(1024, 0)
        Me.lblHierEntsteht.Name = "lblHierEntsteht"
        Me.lblHierEntsteht.Size = New System.Drawing.Size(1024, 48)
        Me.lblHierEntsteht.TabIndex = 5
        Me.lblHierEntsteht.Tag = "langKey=lblHierEntsteht"
        Me.lblHierEntsteht.Text = "Hier entsteht gerade das neue Bildschirmschoner-Modul"
        Me.lblHierEntsteht.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblUnderConstruction
        '
        Me.lblUnderConstruction.BackColor = System.Drawing.Color.Transparent
        Me.lblUnderConstruction.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblUnderConstruction.ForeColor = System.Drawing.Color.Blue
        Me.lblUnderConstruction.Location = New System.Drawing.Point(0, 13)
        Me.lblUnderConstruction.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblUnderConstruction.MaximumSize = New System.Drawing.Size(1024, 768)
        Me.lblUnderConstruction.MinimumSize = New System.Drawing.Size(1024, 0)
        Me.lblUnderConstruction.Name = "lblUnderConstruction"
        Me.lblUnderConstruction.Size = New System.Drawing.Size(1024, 68)
        Me.lblUnderConstruction.TabIndex = 5
        Me.lblUnderConstruction.Tag = "langKey=lblUnderConstruction"
        Me.lblUnderConstruction.Text = "! Under Construction !"
        Me.lblUnderConstruction.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'ucUnderConstruction
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.panPanel)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.MaximumSize = New System.Drawing.Size(1024, 768)
        Me.MinimumSize = New System.Drawing.Size(1024, 768)
        Me.Name = "ucUnderConstruction"
        Me.Size = New System.Drawing.Size(1024, 768)
        Me.panPanel.ResumeLayout(False)
        Me.panPanel.PerformLayout()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents panPanel As Windows.Forms.Panel
    Friend WithEvents picLogo As Windows.Forms.PictureBox
    Friend WithEvents lblModulname As Windows.Forms.Label
    Friend WithEvents lblHierEntsteht As Windows.Forms.Label
    Friend WithEvents lblUnderConstruction As Windows.Forms.Label
End Class