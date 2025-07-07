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
        Me.panPanel.Location = New System.Drawing.Point(4, 4)
        Me.panPanel.Name = "panPanel"
        Me.panPanel.Size = New System.Drawing.Size(1552, 1117)
        Me.panPanel.TabIndex = 6
        '
        'picLogo
        '
        Me.picLogo.Image = Global.Modul_Mandelbrot.My.Resources.Resources.mandelbrot_SpashScreen
        Me.picLogo.Location = New System.Drawing.Point(264, 394)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New System.Drawing.Size(986, 615)
        Me.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogo.TabIndex = 5
        Me.picLogo.TabStop = False
        '
        'lblModulname
        '
        Me.lblModulname.AutoSize = True
        Me.lblModulname.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblModulname.ForeColor = System.Drawing.Color.Blue
        Me.lblModulname.Location = New System.Drawing.Point(488, 256)
        Me.lblModulname.Name = "lblModulname"
        Me.lblModulname.Size = New System.Drawing.Size(442, 96)
        Me.lblModulname.TabIndex = 5
        Me.lblModulname.Tag = "langKey=lblModulname"
        Me.lblModulname.Text = "Mandelbrot"
        '
        'lblHierEntsteht
        '
        Me.lblHierEntsteht.AutoSize = True
        Me.lblHierEntsteht.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblHierEntsteht.ForeColor = System.Drawing.Color.Blue
        Me.lblHierEntsteht.Location = New System.Drawing.Point(37, 154)
        Me.lblHierEntsteht.Name = "lblHierEntsteht"
        Me.lblHierEntsteht.Size = New System.Drawing.Size(1456, 72)
        Me.lblHierEntsteht.TabIndex = 5
        Me.lblHierEntsteht.Tag = "langKey=lblHierEntsteht"
        Me.lblHierEntsteht.Text = "Hier entsteht gerade das neue Bildschirmschoner-Modul"
        '
        'lblUnderConstruction
        '
        Me.lblUnderConstruction.AutoSize = True
        Me.lblUnderConstruction.BackColor = System.Drawing.Color.Transparent
        Me.lblUnderConstruction.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblUnderConstruction.ForeColor = System.Drawing.Color.Blue
        Me.lblUnderConstruction.Location = New System.Drawing.Point(438, 69)
        Me.lblUnderConstruction.Name = "lblUnderConstruction"
        Me.lblUnderConstruction.Size = New System.Drawing.Size(593, 72)
        Me.lblUnderConstruction.TabIndex = 5
        Me.lblUnderConstruction.Tag = "langKey=lblUnderConstruction"
        Me.lblUnderConstruction.Text = "! Under Construction !"
        '
        'ucUnderConstruction
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(14.0!, 29.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.panPanel)
        Me.Name = "ucUnderConstruction"
        Me.Size = New System.Drawing.Size(1561, 1124)
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