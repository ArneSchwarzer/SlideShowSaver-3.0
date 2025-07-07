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
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lblModulname = New System.Windows.Forms.Label()
        Me.lblHierEntsteht = New System.Windows.Forms.Label()
        Me.lblUnderConstruction = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Transparent
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Controls.Add(Me.lblModulname)
        Me.Panel1.Controls.Add(Me.lblHierEntsteht)
        Me.Panel1.Controls.Add(Me.lblUnderConstruction)
        Me.Panel1.Location = New System.Drawing.Point(2, 2)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1552, 1118)
        Me.Panel1.TabIndex = 6
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.Modul_SlideShowSaver_3._0.My.Resources.Resources.SlideShowSaver_Splash
        Me.PictureBox1.Location = New System.Drawing.Point(0, 435)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.PictureBox1.MinimumSize = New System.Drawing.Size(1549, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(1549, 616)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'lblModulname
        '
        Me.lblModulname.AutoSize = True
        Me.lblModulname.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold)
        Me.lblModulname.ForeColor = System.Drawing.Color.Red
        Me.lblModulname.Location = New System.Drawing.Point(0, 257)
        Me.lblModulname.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblModulname.MinimumSize = New System.Drawing.Size(1549, 0)
        Me.lblModulname.Name = "lblModulname"
        Me.lblModulname.Size = New System.Drawing.Size(1549, 96)
        Me.lblModulname.TabIndex = 5
        Me.lblModulname.Tag = "langKey=lblModulname"
        Me.lblModulname.Text = "SlideShowSaver 3.0"
        Me.lblModulname.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblHierEntsteht
        '
        Me.lblHierEntsteht.AutoSize = True
        Me.lblHierEntsteht.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblHierEntsteht.ForeColor = System.Drawing.Color.Red
        Me.lblHierEntsteht.Location = New System.Drawing.Point(0, 132)
        Me.lblHierEntsteht.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblHierEntsteht.MinimumSize = New System.Drawing.Size(1549, 0)
        Me.lblHierEntsteht.Name = "lblHierEntsteht"
        Me.lblHierEntsteht.Size = New System.Drawing.Size(1549, 72)
        Me.lblHierEntsteht.TabIndex = 5
        Me.lblHierEntsteht.Tag = "langKey=lblHierEntsteht"
        Me.lblHierEntsteht.Text = "Hier entsteht gerade das neue Bildschirmschoner-Modul"
        Me.lblHierEntsteht.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'lblUnderConstruction
        '
        Me.lblUnderConstruction.AutoSize = True
        Me.lblUnderConstruction.BackColor = System.Drawing.Color.Transparent
        Me.lblUnderConstruction.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblUnderConstruction.ForeColor = System.Drawing.Color.Red
        Me.lblUnderConstruction.Location = New System.Drawing.Point(0, 42)
        Me.lblUnderConstruction.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblUnderConstruction.MinimumSize = New System.Drawing.Size(1549, 0)
        Me.lblUnderConstruction.Name = "lblUnderConstruction"
        Me.lblUnderConstruction.Size = New System.Drawing.Size(1549, 72)
        Me.lblUnderConstruction.TabIndex = 5
        Me.lblUnderConstruction.Tag = "langKey=lblUnderConstruction"
        Me.lblUnderConstruction.Text = "! Under Construction !"
        Me.lblUnderConstruction.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'ucUnderConstruction
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(14.0!, 29.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Transparent
        Me.Controls.Add(Me.Panel1)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "ucUnderConstruction"
        Me.Size = New System.Drawing.Size(1561, 1124)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents PictureBox1 As Windows.Forms.PictureBox
    Friend WithEvents lblModulname As Windows.Forms.Label
    Friend WithEvents lblHierEntsteht As Windows.Forms.Label
    Friend WithEvents lblUnderConstruction As Windows.Forms.Label
End Class