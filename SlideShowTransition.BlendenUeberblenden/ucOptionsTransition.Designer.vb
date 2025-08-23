<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucOptionsTransition
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblTransitionname = New System.Windows.Forms.Label()
Me.lblTransitionname.Tag = "langKey=lblTransitionname"
        Me.lblNTransitionName = New System.Windows.Forms.Label()
Me.lblNTransitionName.Tag = "langKey=lblNTransitionName"
        Me.lblNpicFarbton = New System.Windows.Forms.Label()
Me.lblNpicFarbton.Tag = "langKey=lblNpicFarbton"
        Me.picFarbton = New System.Windows.Forms.PictureBox()
Me.picFarbton.Tag = "langKey=picFarbton"
        Me.cdFarbton = New System.Windows.Forms.ColorDialog()
        Me.lblNModus = New System.Windows.Forms.Label()
Me.lblNModus.Tag = "langKey=lblNModus"
        Me.chkZufallsfarbe = New System.Windows.Forms.CheckBox()
Me.chkZufallsfarbe.Tag = "langKey=chkZufallsfarbe"
        Me.pnlModus = New System.Windows.Forms.Panel()
        Me.rdoZufall = New System.Windows.Forms.RadioButton()
Me.rdoZufall.Tag = "langKey=rdoZufall"
        Me.rdoÜberblenden = New System.Windows.Forms.RadioButton()
        Me.rdoBlenden = New System.Windows.Forms.RadioButton()
Me.rdoBlenden.Tag = "langKey=rdoBlenden"
        Me.lblGeschwindigkeit = New System.Windows.Forms.Label()
Me.lblGeschwindigkeit.Tag = "langKey=lblGeschwindigkeit"
        Me.trkGeschwindigkeit = New System.Windows.Forms.TrackBar()
        Me.lblNtrbGeschwindigkeit = New System.Windows.Forms.Label()
Me.lblNtrbGeschwindigkeit.Tag = "langKey=lblNtrbGeschwindigkeit"
        Me.chkMorphing = New System.Windows.Forms.CheckBox()
Me.chkMorphing.Tag = "langKey=chkMorphing"
        Me.btnDefaults = New System.Windows.Forms.Button()
Me.btnDefaults.Tag = "langKey=btnDefaults"
        CType(Me.picFarbton, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlModus.SuspendLayout()
        CType(Me.trkGeschwindigkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTransitionname
        '
        Me.lblTransitionname.AutoSize = True
        Me.lblTransitionname.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTransitionname.Location = New System.Drawing.Point(272, 30)
        Me.lblTransitionname.Name = "lblTransitionname"
        Me.lblTransitionname.Size = New System.Drawing.Size(358, 41)
        Me.lblTransitionname.TabIndex = 20
        Me.lblTransitionname.Text = "Blenden && Überblenden"
        '
        'lblNTransitionName
        '
        Me.lblNTransitionName.AutoSize = True
        Me.lblNTransitionName.Location = New System.Drawing.Point(15, 30)
        Me.lblNTransitionName.Name = "lblNTransitionName"
        Me.lblNTransitionName.Size = New System.Drawing.Size(151, 41)
        Me.lblNTransitionName.TabIndex = 19
        Me.lblNTransitionName.Text = "Übergang"
        '
        'lblNpicFarbton
        '
        Me.lblNpicFarbton.AutoSize = True
        Me.lblNpicFarbton.Location = New System.Drawing.Point(15, 351)
        Me.lblNpicFarbton.Name = "lblNpicFarbton"
        Me.lblNpicFarbton.Size = New System.Drawing.Size(258, 41)
        Me.lblNpicFarbton.TabIndex = 21
        Me.lblNpicFarbton.Text = "Blende zu Farbton"
        '
        'picFarbton
        '
        Me.picFarbton.BackColor = System.Drawing.Color.Sienna
        Me.picFarbton.Location = New System.Drawing.Point(279, 342)
        Me.picFarbton.Name = "picFarbton"
        Me.picFarbton.Size = New System.Drawing.Size(582, 50)
        Me.picFarbton.TabIndex = 22
        Me.picFarbton.TabStop = False
        '
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Location = New System.Drawing.Point(15, 128)
        Me.lblNModus.Name = "lblNModus"
        Me.lblNModus.Size = New System.Drawing.Size(111, 41)
        Me.lblNModus.TabIndex = 29
        Me.lblNModus.Text = "Modus"
        '
        'chkZufallsfarbe
        '
        Me.chkZufallsfarbe.AutoSize = True
        Me.chkZufallsfarbe.Location = New System.Drawing.Point(279, 398)
        Me.chkZufallsfarbe.Name = "chkZufallsfarbe"
        Me.chkZufallsfarbe.Size = New System.Drawing.Size(203, 45)
        Me.chkZufallsfarbe.TabIndex = 30
        Me.chkZufallsfarbe.Text = "Zufallsfarbe"
        Me.chkZufallsfarbe.UseVisualStyleBackColor = True
        '
        'pnlModus
        '
        Me.pnlModus.Controls.Add(Me.rdoZufall)
        Me.pnlModus.Controls.Add(Me.rdoÜberblenden)
        Me.pnlModus.Controls.Add(Me.rdoBlenden)
        Me.pnlModus.Location = New System.Drawing.Point(264, 128)
        Me.pnlModus.Name = "pnlModus"
        Me.pnlModus.Size = New System.Drawing.Size(597, 164)
        Me.pnlModus.TabIndex = 31
        '
        'rdoZufall
        '
        Me.rdoZufall.AutoSize = True
        Me.rdoZufall.Location = New System.Drawing.Point(15, 97)
        Me.rdoZufall.Name = "rdoZufall"
        Me.rdoZufall.Size = New System.Drawing.Size(146, 45)
        Me.rdoZufall.TabIndex = 31
        Me.rdoZufall.TabStop = True
        Me.rdoZufall.Text = "Zufällig"
        Me.rdoZufall.UseVisualStyleBackColor = True
        '
        'rdoÜberblenden
        '
        Me.rdoÜberblenden.AutoSize = True
        Me.rdoÜberblenden.Location = New System.Drawing.Point(15, 46)
        Me.rdoÜberblenden.Name = "rdoÜberblenden"
        Me.rdoÜberblenden.Size = New System.Drawing.Size(223, 45)
        Me.rdoÜberblenden.TabIndex = 30
        Me.rdoÜberblenden.TabStop = True
        Me.rdoÜberblenden.Text = "Überblenden"
        Me.rdoÜberblenden.UseVisualStyleBackColor = True
        '
        'rdoBlenden
        '
        Me.rdoBlenden.AutoSize = True
        Me.rdoBlenden.Location = New System.Drawing.Point(15, -5)
        Me.rdoBlenden.Name = "rdoBlenden"
        Me.rdoBlenden.Size = New System.Drawing.Size(157, 45)
        Me.rdoBlenden.TabIndex = 29
        Me.rdoBlenden.TabStop = True
        Me.rdoBlenden.Text = "Blenden"
        Me.rdoBlenden.UseVisualStyleBackColor = True
        '
        'lblGeschwindigkeit
        '
        Me.lblGeschwindigkeit.AutoSize = True
        Me.lblGeschwindigkeit.Location = New System.Drawing.Point(790, 563)
        Me.lblGeschwindigkeit.MinimumSize = New System.Drawing.Size(71, 0)
        Me.lblGeschwindigkeit.Name = "lblGeschwindigkeit"
        Me.lblGeschwindigkeit.Size = New System.Drawing.Size(71, 41)
        Me.lblGeschwindigkeit.TabIndex = 37
        Me.lblGeschwindigkeit.Text = "15 s"
        Me.lblGeschwindigkeit.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkGeschwindigkeit
        '
        Me.trkGeschwindigkeit.AutoSize = False
        Me.trkGeschwindigkeit.Location = New System.Drawing.Point(279, 563)
        Me.trkGeschwindigkeit.Maximum = 30
        Me.trkGeschwindigkeit.Minimum = 1
        Me.trkGeschwindigkeit.Name = "trkGeschwindigkeit"
        Me.trkGeschwindigkeit.Size = New System.Drawing.Size(505, 57)
        Me.trkGeschwindigkeit.TabIndex = 36
        Me.trkGeschwindigkeit.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkGeschwindigkeit.Value = 15
        '
        'lblNtrbGeschwindigkeit
        '
        Me.lblNtrbGeschwindigkeit.AutoSize = True
        Me.lblNtrbGeschwindigkeit.Location = New System.Drawing.Point(15, 563)
        Me.lblNtrbGeschwindigkeit.Name = "lblNtrbGeschwindigkeit"
        Me.lblNtrbGeschwindigkeit.Size = New System.Drawing.Size(236, 41)
        Me.lblNtrbGeschwindigkeit.TabIndex = 35
        Me.lblNtrbGeschwindigkeit.Text = "Geschwindigkeit"
        '
        'chkMorphing
        '
        Me.chkMorphing.AutoSize = True
        Me.chkMorphing.Location = New System.Drawing.Point(279, 449)
        Me.chkMorphing.Name = "chkMorphing"
        Me.chkMorphing.Size = New System.Drawing.Size(219, 45)
        Me.chkMorphing.TabIndex = 39
        Me.chkMorphing.Text = "Morphphase"
        Me.chkMorphing.UseVisualStyleBackColor = True
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(678, 25)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(183, 51)
        Me.btnDefaults.TabIndex = 40
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.chkMorphing)
        Me.Controls.Add(Me.lblGeschwindigkeit)
        Me.Controls.Add(Me.pnlModus)
        Me.Controls.Add(Me.trkGeschwindigkeit)
        Me.Controls.Add(Me.lblNtrbGeschwindigkeit)
        Me.Controls.Add(Me.chkZufallsfarbe)
        Me.Controls.Add(Me.lblNModus)
        Me.Controls.Add(Me.picFarbton)
        Me.Controls.Add(Me.lblNpicFarbton)
        Me.Controls.Add(Me.lblTransitionname)
        Me.Controls.Add(Me.lblNTransitionName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.picFarbton, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlModus.ResumeLayout(False)
        Me.pnlModus.PerformLayout()
        CType(Me.trkGeschwindigkeit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTransitionname As Windows.Forms.Label
    Friend WithEvents lblNTransitionName As Windows.Forms.Label
    Friend WithEvents lblNpicFarbton As Windows.Forms.Label
    Friend WithEvents picFarbton As Windows.Forms.PictureBox
    Friend WithEvents cdFarbton As Windows.Forms.ColorDialog
    Friend WithEvents lblNModus As Windows.Forms.Label
    Friend WithEvents chkZufallsfarbe As Windows.Forms.CheckBox
    Friend WithEvents pnlModus As Windows.Forms.Panel
    Friend WithEvents rdoZufall As Windows.Forms.RadioButton
    Friend WithEvents rdoÜberblenden As Windows.Forms.RadioButton
    Friend WithEvents rdoBlenden As Windows.Forms.RadioButton
    Friend WithEvents lblGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents trkGeschwindigkeit As Windows.Forms.TrackBar
    Friend WithEvents lblNtrbGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents chkMorphing As Windows.Forms.CheckBox
    Friend WithEvents btnDefaults As Windows.Forms.Button
End Class
