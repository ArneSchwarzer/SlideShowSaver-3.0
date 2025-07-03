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
        Me.components = New System.ComponentModel.Container()
        Me.lblPauseAnzahl = New System.Windows.Forms.Label()
        Me.btnPauseBack = New System.Windows.Forms.Button()
        Me.btnPausePause = New System.Windows.Forms.Button()
        Me.btnPauseForward = New System.Windows.Forms.Button()
        Me.chkPauseMarkPicture = New System.Windows.Forms.CheckBox()
        Me.chkBewerten = New System.Windows.Forms.CheckBox()
        Me.sbcBewerten = New StarControlLibrary.SterneBewertungControl()
        Me.lblOptionsDialogDisabled = New System.Windows.Forms.Label()
        Me.tmrWarnLabelAnzeige = New System.Windows.Forms.Timer(Me.components)
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
        'chkBewerten
        '
        Me.chkBewerten.AutoSize = True
        Me.chkBewerten.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkBewerten.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.chkBewerten.Location = New System.Drawing.Point(32, 293)
        Me.chkBewerten.Margin = New System.Windows.Forms.Padding(4)
        Me.chkBewerten.Name = "chkBewerten"
        Me.chkBewerten.Size = New System.Drawing.Size(172, 40)
        Me.chkBewerten.TabIndex = 6
        Me.chkBewerten.Text = "Bewerten"
        Me.chkBewerten.UseVisualStyleBackColor = True
        '
        'sbcBewerten
        '
        Me.sbcBewerten.Bewertung = 0
        Me.sbcBewerten.Location = New System.Drawing.Point(371, 293)
        Me.sbcBewerten.Name = "sbcBewerten"
        Me.sbcBewerten.Size = New System.Drawing.Size(195, 40)
        Me.sbcBewerten.TabIndex = 7
        Me.sbcBewerten.Text = "SterneBewertungControl1"
        '
        'lblOptionsDialogDisabled
        '
        Me.lblOptionsDialogDisabled.AutoSize = True
        Me.lblOptionsDialogDisabled.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblOptionsDialogDisabled.ForeColor = System.Drawing.Color.Red
        Me.lblOptionsDialogDisabled.Location = New System.Drawing.Point(18, 409)
        Me.lblOptionsDialogDisabled.MaximumSize = New System.Drawing.Size(580, 0)
        Me.lblOptionsDialogDisabled.Name = "lblOptionsDialogDisabled"
        Me.lblOptionsDialogDisabled.Size = New System.Drawing.Size(575, 82)
        Me.lblOptionsDialogDisabled.TabIndex = 8
        Me.lblOptionsDialogDisabled.Text = "Im PauseModus ist kein Options-Dialog  aufrufbar"
        Me.lblOptionsDialogDisabled.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'tmrWarnLabelAnzeige
        '
        Me.tmrWarnLabelAnzeige.Interval = 3000
        '
        'frmPauseModusOverlay
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(599, 499)
        Me.Controls.Add(Me.lblOptionsDialogDisabled)
        Me.Controls.Add(Me.sbcBewerten)
        Me.Controls.Add(Me.chkBewerten)
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
    Friend WithEvents chkBewerten As Windows.Forms.CheckBox
    Friend WithEvents sbcBewerten As StarControlLibrary.SterneBewertungControl
    Friend WithEvents lblOptionsDialogDisabled As Windows.Forms.Label
    Friend WithEvents tmrWarnLabelAnzeige As Windows.Forms.Timer
End Class
