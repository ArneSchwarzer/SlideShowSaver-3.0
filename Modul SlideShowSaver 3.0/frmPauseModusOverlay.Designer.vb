<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPauseModusOverlay
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
        Me.lblPauseAnzahl = New System.Windows.Forms.Label()
        Me.btnPauseBack = New System.Windows.Forms.Button()
        Me.btnPausePause = New System.Windows.Forms.Button()
        Me.btnPauseForward = New System.Windows.Forms.Button()
        Me.chkPauseMarkPicture = New System.Windows.Forms.CheckBox()
        Me.cmbBewertungKorrigieren = New System.Windows.Forms.ComboBox()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'lblPauseAnzahl
        '
        Me.lblPauseAnzahl.AutoSize = True
        Me.lblPauseAnzahl.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPauseAnzahl.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblPauseAnzahl.Location = New System.Drawing.Point(128, 23)
        Me.lblPauseAnzahl.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblPauseAnzahl.Name = "lblPauseAnzahl"
        Me.lblPauseAnzahl.Size = New System.Drawing.Size(314, 55)
        Me.lblPauseAnzahl.TabIndex = 0
        Me.lblPauseAnzahl.Text = "Bild 3 von 10"
        '
        'btnPauseBack
        '
        Me.btnPauseBack.BackgroundImage = Global.Modul_SlideShowSaver_3._0.My.Resources.Resources.Zurück_Transparent
        Me.btnPauseBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnPauseBack.Font = New System.Drawing.Font("Webdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPauseBack.Location = New System.Drawing.Point(25, 100)
        Me.btnPauseBack.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPauseBack.Name = "btnPauseBack"
        Me.btnPauseBack.Size = New System.Drawing.Size(150, 150)
        Me.btnPauseBack.TabIndex = 1
        Me.btnPauseBack.UseVisualStyleBackColor = True
        '
        'btnPausePause
        '
        Me.btnPausePause.BackgroundImage = Global.Modul_SlideShowSaver_3._0.My.Resources.Resources.Pause_Transparent
        Me.btnPausePause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnPausePause.Font = New System.Drawing.Font("Webdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPausePause.Location = New System.Drawing.Point(221, 100)
        Me.btnPausePause.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPausePause.Name = "btnPausePause"
        Me.btnPausePause.Size = New System.Drawing.Size(150, 150)
        Me.btnPausePause.TabIndex = 2
        Me.btnPausePause.UseVisualStyleBackColor = True
        '
        'btnPauseForward
        '
        Me.btnPauseForward.BackgroundImage = Global.Modul_SlideShowSaver_3._0.My.Resources.Resources.Vor_Transparent
        Me.btnPauseForward.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnPauseForward.Font = New System.Drawing.Font("Webdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPauseForward.Location = New System.Drawing.Point(416, 100)
        Me.btnPauseForward.Margin = New System.Windows.Forms.Padding(4)
        Me.btnPauseForward.Name = "btnPauseForward"
        Me.btnPauseForward.Size = New System.Drawing.Size(150, 150)
        Me.btnPauseForward.TabIndex = 3
        Me.btnPauseForward.UseVisualStyleBackColor = True
        '
        'chkPauseMarkPicture
        '
        Me.chkPauseMarkPicture.AutoSize = True
        Me.chkPauseMarkPicture.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkPauseMarkPicture.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.chkPauseMarkPicture.Location = New System.Drawing.Point(32, 365)
        Me.chkPauseMarkPicture.Margin = New System.Windows.Forms.Padding(4)
        Me.chkPauseMarkPicture.Name = "chkPauseMarkPicture"
        Me.chkPauseMarkPicture.Size = New System.Drawing.Size(179, 40)
        Me.chkPauseMarkPicture.TabIndex = 4
        Me.chkPauseMarkPicture.Text = "Markieren"
        Me.chkPauseMarkPicture.UseVisualStyleBackColor = True
        '
        'cmbBewertungKorrigieren
        '
        Me.cmbBewertungKorrigieren.BackColor = System.Drawing.Color.Black
        Me.cmbBewertungKorrigieren.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.cmbBewertungKorrigieren.FormattingEnabled = True
        Me.cmbBewertungKorrigieren.Location = New System.Drawing.Point(221, 287)
        Me.cmbBewertungKorrigieren.Name = "cmbBewertungKorrigieren"
        Me.cmbBewertungKorrigieren.Size = New System.Drawing.Size(345, 49)
        Me.cmbBewertungKorrigieren.TabIndex = 5
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.CheckBox1.Location = New System.Drawing.Point(32, 293)
        Me.CheckBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(172, 40)
        Me.CheckBox1.TabIndex = 6
        Me.CheckBox1.Text = "Bewerten"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'frmPauseModusOverlay
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(599, 438)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.cmbBewertungKorrigieren)
        Me.Controls.Add(Me.chkPauseMarkPicture)
        Me.Controls.Add(Me.btnPauseForward)
        Me.Controls.Add(Me.btnPausePause)
        Me.Controls.Add(Me.btnPauseBack)
        Me.Controls.Add(Me.lblPauseAnzahl)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmPauseModusOverlay"
        Me.Opacity = 0.5R
        Me.Text = "PauseModusControl"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblPauseAnzahl As Windows.Forms.Label
    Friend WithEvents btnPauseBack As Windows.Forms.Button
    Friend WithEvents btnPausePause As Windows.Forms.Button
    Friend WithEvents btnPauseForward As Windows.Forms.Button
    Friend WithEvents chkPauseMarkPicture As Windows.Forms.CheckBox
    Friend WithEvents cmbBewertungKorrigieren As Windows.Forms.ComboBox
    Friend WithEvents CheckBox1 As Windows.Forms.CheckBox
End Class
