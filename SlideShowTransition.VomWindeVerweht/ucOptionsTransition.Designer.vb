<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsTransition
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
        Me.lblTransitonName = New System.Windows.Forms.Label()
        Me.lblNlblTransitionName = New System.Windows.Forms.Label()
        Me.chkPartikelGroesseZufall = New System.Windows.Forms.CheckBox()
        Me.chkWindstaerkeZufall = New System.Windows.Forms.CheckBox()
        Me.trkPartikelGroesse = New System.Windows.Forms.TrackBar()
        Me.trkWindstaerke = New System.Windows.Forms.TrackBar()
        Me.lblNtrkParikelGroesse = New System.Windows.Forms.Label()
        Me.lblPartikelGroesse = New System.Windows.Forms.Label()
        Me.lblNtrkWindstaerke = New System.Windows.Forms.Label()
        Me.lblWindstaerke = New System.Windows.Forms.Label()
        Me.btnDefaults = New System.Windows.Forms.Button()
        CType(Me.trkPartikelGroesse, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkWindstaerke, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTransitonName
        '
        Me.lblTransitonName.AutoSize = True
        Me.lblTransitonName.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTransitonName.Location = New System.Drawing.Point(264, 29)
        Me.lblTransitonName.Name = "lblTransitonName"
        Me.lblTransitonName.Size = New System.Drawing.Size(304, 41)
        Me.lblTransitonName.TabIndex = 20
        Me.lblTransitonName.Tag = "langKey=lblTransitonName"
        Me.lblTransitonName.Text = "Vom Winde verweht"
        '
        'lblNlblTransitionName
        '
        Me.lblNlblTransitionName.AutoSize = True
        Me.lblNlblTransitionName.Location = New System.Drawing.Point(34, 29)
        Me.lblNlblTransitionName.Name = "lblNlblTransitionName"
        Me.lblNlblTransitionName.Size = New System.Drawing.Size(151, 41)
        Me.lblNlblTransitionName.TabIndex = 19
        Me.lblNlblTransitionName.Tag = "langKey=lblNlblTransitionName"
        Me.lblNlblTransitionName.Text = "Übergang"
        '
        'chkPartikelGroesseZufall
        '
        Me.chkPartikelGroesseZufall.AutoSize = True
        Me.chkPartikelGroesseZufall.Location = New System.Drawing.Point(48, 240)
        Me.chkPartikelGroesseZufall.Name = "chkPartikelGroesseZufall"
        Me.chkPartikelGroesseZufall.Size = New System.Drawing.Size(345, 45)
        Me.chkPartikelGroesseZufall.TabIndex = 21
        Me.chkPartikelGroesseZufall.Text = "Zufällige Partikelgröße"
        Me.chkPartikelGroesseZufall.UseVisualStyleBackColor = True
        '
        'chkWindstaerkeZufall
        '
        Me.chkWindstaerkeZufall.AutoSize = True
        Me.chkWindstaerkeZufall.Location = New System.Drawing.Point(48, 404)
        Me.chkWindstaerkeZufall.Name = "chkWindstaerkeZufall"
        Me.chkWindstaerkeZufall.Size = New System.Drawing.Size(320, 45)
        Me.chkWindstaerkeZufall.TabIndex = 22
        Me.chkWindstaerkeZufall.Text = "Zufällige Windstärke"
        Me.chkWindstaerkeZufall.UseVisualStyleBackColor = True
        '
        'trkPartikelGroesse
        '
        Me.trkPartikelGroesse.AutoSize = False
        Me.trkPartikelGroesse.Location = New System.Drawing.Point(271, 184)
        Me.trkPartikelGroesse.Maximum = 100
        Me.trkPartikelGroesse.Minimum = 1
        Me.trkPartikelGroesse.Name = "trkPartikelGroesse"
        Me.trkPartikelGroesse.Size = New System.Drawing.Size(495, 55)
        Me.trkPartikelGroesse.TabIndex = 23
        Me.trkPartikelGroesse.Value = 20
        '
        'trkWindstaerke
        '
        Me.trkWindstaerke.AutoSize = False
        Me.trkWindstaerke.Location = New System.Drawing.Point(271, 338)
        Me.trkWindstaerke.Maximum = 12
        Me.trkWindstaerke.Name = "trkWindstaerke"
        Me.trkWindstaerke.Size = New System.Drawing.Size(495, 61)
        Me.trkWindstaerke.TabIndex = 24
        Me.trkWindstaerke.TabStop = False
        Me.trkWindstaerke.Value = 3
        '
        'lblNtrkParikelGroesse
        '
        Me.lblNtrkParikelGroesse.AutoSize = True
        Me.lblNtrkParikelGroesse.Location = New System.Drawing.Point(41, 184)
        Me.lblNtrkParikelGroesse.Name = "lblNtrkParikelGroesse"
        Me.lblNtrkParikelGroesse.Size = New System.Drawing.Size(192, 41)
        Me.lblNtrkParikelGroesse.TabIndex = 25
        Me.lblNtrkParikelGroesse.Text = "Partikelgröße"
        '
        'lblPartikelGroesse
        '
        Me.lblPartikelGroesse.AutoSize = True
        Me.lblPartikelGroesse.Location = New System.Drawing.Point(772, 184)
        Me.lblPartikelGroesse.Name = "lblPartikelGroesse"
        Me.lblPartikelGroesse.Size = New System.Drawing.Size(66, 41)
        Me.lblPartikelGroesse.TabIndex = 26
        Me.lblPartikelGroesse.Text = "100"
        Me.lblPartikelGroesse.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblNtrkWindstaerke
        '
        Me.lblNtrkWindstaerke.AutoSize = True
        Me.lblNtrkWindstaerke.Location = New System.Drawing.Point(41, 338)
        Me.lblNtrkWindstaerke.Name = "lblNtrkWindstaerke"
        Me.lblNtrkWindstaerke.Size = New System.Drawing.Size(167, 41)
        Me.lblNtrkWindstaerke.TabIndex = 27
        Me.lblNtrkWindstaerke.Text = "Windstärke"
        '
        'lblWindstaerke
        '
        Me.lblWindstaerke.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWindstaerke.AutoSize = True
        Me.lblWindstaerke.Location = New System.Drawing.Point(744, 338)
        Me.lblWindstaerke.MaximumSize = New System.Drawing.Size(94, 0)
        Me.lblWindstaerke.MinimumSize = New System.Drawing.Size(94, 0)
        Me.lblWindstaerke.Name = "lblWindstaerke"
        Me.lblWindstaerke.Size = New System.Drawing.Size(94, 41)
        Me.lblWindstaerke.TabIndex = 28
        Me.lblWindstaerke.Text = "Bft 3"
        Me.lblWindstaerke.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(664, 23)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(174, 52)
        Me.btnDefaults.TabIndex = 55
        Me.btnDefaults.Tag = "langKey=btnDefaults"
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.lblWindstaerke)
        Me.Controls.Add(Me.lblNtrkWindstaerke)
        Me.Controls.Add(Me.lblPartikelGroesse)
        Me.Controls.Add(Me.lblNtrkParikelGroesse)
        Me.Controls.Add(Me.trkWindstaerke)
        Me.Controls.Add(Me.trkPartikelGroesse)
        Me.Controls.Add(Me.chkWindstaerkeZufall)
        Me.Controls.Add(Me.chkPartikelGroesseZufall)
        Me.Controls.Add(Me.lblTransitonName)
        Me.Controls.Add(Me.lblNlblTransitionName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkPartikelGroesse, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkWindstaerke, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTransitonName As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionName As Windows.Forms.Label
    Friend WithEvents chkPartikelGroesseZufall As Windows.Forms.CheckBox
    Friend WithEvents chkWindstaerkeZufall As Windows.Forms.CheckBox
    Friend WithEvents trkPartikelGroesse As Windows.Forms.TrackBar
    Friend WithEvents trkWindstaerke As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkParikelGroesse As Windows.Forms.Label
    Friend WithEvents lblPartikelGroesse As Windows.Forms.Label
    Friend WithEvents lblNtrkWindstaerke As Windows.Forms.Label
    Friend WithEvents lblWindstaerke As Windows.Forms.Label
    Friend WithEvents btnDefaults As Windows.Forms.Button
End Class
