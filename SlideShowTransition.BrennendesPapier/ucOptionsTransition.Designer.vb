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
        Me.btnDefaults = New System.Windows.Forms.Button()
        Me.lblTransitionsname = New System.Windows.Forms.Label()
        Me.lblNlblTransitionname = New System.Windows.Forms.Label()
        Me.lblGeschwindigkeit = New System.Windows.Forms.Label()
        Me.trkDauer = New System.Windows.Forms.TrackBar()
        Me.lblNtrbDauer = New System.Windows.Forms.Label()
        Me.chkPartikel = New System.Windows.Forms.CheckBox()
        Me.lblNModus = New System.Windows.Forms.Label()
        Me.clbModus = New System.Windows.Forms.CheckedListBox()
        Me.lblKeineRichtungInfo = New System.Windows.Forms.Label()
        Me.lblWertBrandkante = New System.Windows.Forms.Label()
        Me.trkBrandkantenbreite = New System.Windows.Forms.TrackBar()
        Me.lblNtrkBrandkantenbreite = New System.Windows.Forms.Label()
        Me.chkZufallsdauer = New System.Windows.Forms.CheckBox()
        Me.grbFBM = New System.Windows.Forms.GroupBox()
        Me.lblNtrkFBMPersistenz = New System.Windows.Forms.Label()
        Me.lblNtrkGrundfrequenz = New System.Windows.Forms.Label()
        Me.trkFBMGrundfrequenz = New System.Windows.Forms.TrackBar()
        Me.lblNtrkFBMOktaven = New System.Windows.Forms.Label()
        Me.lblNtrkFBMStaerke = New System.Windows.Forms.Label()
        Me.trkFBMOktaven = New System.Windows.Forms.TrackBar()
        Me.trkFBMPersistenz = New System.Windows.Forms.TrackBar()
        Me.trkFBMStaerke = New System.Windows.Forms.TrackBar()
        Me.lblFBMGrundfrequenz = New System.Windows.Forms.Label()
        Me.lblFBMOktaven = New System.Windows.Forms.Label()
        Me.lblFBMPersistenz = New System.Windows.Forms.Label()
        Me.lblFBMStaerke = New System.Windows.Forms.Label()
        CType(Me.trkDauer, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkBrandkantenbreite, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grbFBM.SuspendLayout()
        CType(Me.trkFBMGrundfrequenz, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkFBMOktaven, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkFBMPersistenz, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkFBMStaerke, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(696, 22)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(174, 52)
        Me.btnDefaults.TabIndex = 54
        Me.btnDefaults.Tag = "langKey=btnDefaults"
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'lblTransitionsname
        '
        Me.lblTransitionsname.AutoSize = True
        Me.lblTransitionsname.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblTransitionsname.Location = New System.Drawing.Point(276, 28)
        Me.lblTransitionsname.Name = "lblTransitionsname"
        Me.lblTransitionsname.Size = New System.Drawing.Size(280, 41)
        Me.lblTransitionsname.TabIndex = 53
        Me.lblTransitionsname.Tag = "langKey=lblTransitionsname"
        Me.lblTransitionsname.Text = "Brennendes Papier"
        '
        'lblNlblTransitionname
        '
        Me.lblNlblTransitionname.AutoSize = True
        Me.lblNlblTransitionname.Location = New System.Drawing.Point(8, 28)
        Me.lblNlblTransitionname.Name = "lblNlblTransitionname"
        Me.lblNlblTransitionname.Size = New System.Drawing.Size(151, 41)
        Me.lblNlblTransitionname.TabIndex = 52
        Me.lblNlblTransitionname.Tag = "langKey=lblNlblTransitionname"
        Me.lblNlblTransitionname.Text = "Übergang"
        '
        'lblGeschwindigkeit
        '
        Me.lblGeschwindigkeit.AutoSize = True
        Me.lblGeschwindigkeit.Location = New System.Drawing.Point(771, 540)
        Me.lblGeschwindigkeit.MinimumSize = New System.Drawing.Size(99, 0)
        Me.lblGeschwindigkeit.Name = "lblGeschwindigkeit"
        Me.lblGeschwindigkeit.Size = New System.Drawing.Size(99, 41)
        Me.lblGeschwindigkeit.TabIndex = 57
        Me.lblGeschwindigkeit.Tag = "langKey=lblGeschwindigkeit"
        Me.lblGeschwindigkeit.Text = "15 s"
        Me.lblGeschwindigkeit.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkDauer
        '
        Me.trkDauer.AutoSize = False
        Me.trkDauer.Location = New System.Drawing.Point(283, 540)
        Me.trkDauer.Maximum = 30
        Me.trkDauer.Minimum = 4
        Me.trkDauer.Name = "trkDauer"
        Me.trkDauer.Size = New System.Drawing.Size(482, 57)
        Me.trkDauer.TabIndex = 56
        Me.trkDauer.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkDauer.Value = 15
        '
        'lblNtrbDauer
        '
        Me.lblNtrbDauer.AutoSize = True
        Me.lblNtrbDauer.Location = New System.Drawing.Point(15, 540)
        Me.lblNtrbDauer.Name = "lblNtrbDauer"
        Me.lblNtrbDauer.Size = New System.Drawing.Size(97, 41)
        Me.lblNtrbDauer.TabIndex = 55
        Me.lblNtrbDauer.Tag = "langKey=lblNtrbDauer"
        Me.lblNtrbDauer.Text = "Dauer"
        '
        'chkPartikel
        '
        Me.chkPartikel.AutoSize = True
        Me.chkPartikel.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.chkPartikel.Location = New System.Drawing.Point(22, 492)
        Me.chkPartikel.Name = "chkPartikel"
        Me.chkPartikel.Size = New System.Drawing.Size(146, 45)
        Me.chkPartikel.TabIndex = 58
        Me.chkPartikel.Tag = "langKey=chkPartikel"
        Me.chkPartikel.Text = "Partikel"
        Me.chkPartikel.UseVisualStyleBackColor = True
        '
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Location = New System.Drawing.Point(17, 139)
        Me.lblNModus.Name = "lblNModus"
        Me.lblNModus.Size = New System.Drawing.Size(111, 41)
        Me.lblNModus.TabIndex = 60
        Me.lblNModus.Tag = "langKey=lblNModus"
        Me.lblNModus.Text = "Modus"
        Me.lblNModus.UseMnemonic = False
        '
        'clbModus
        '
        Me.clbModus.FormattingEnabled = True
        Me.clbModus.Items.AddRange(New Object() {"Feuer", "Blitze", "Säure", "Magie"})
        Me.clbModus.Location = New System.Drawing.Point(283, 139)
        Me.clbModus.Name = "clbModus"
        Me.clbModus.Size = New System.Drawing.Size(587, 180)
        Me.clbModus.TabIndex = 59
        Me.clbModus.Tag = "langKey=clbModus"
        '
        'lblKeineRichtungInfo
        '
        Me.lblKeineRichtungInfo.AutoSize = True
        Me.lblKeineRichtungInfo.Location = New System.Drawing.Point(276, 322)
        Me.lblKeineRichtungInfo.MaximumSize = New System.Drawing.Size(587, 0)
        Me.lblKeineRichtungInfo.Name = "lblKeineRichtungInfo"
        Me.lblKeineRichtungInfo.Size = New System.Drawing.Size(486, 82)
        Me.lblKeineRichtungInfo.TabIndex = 61
        Me.lblKeineRichtungInfo.Tag = "langKey=lblKeineRichtungInfo"
        Me.lblKeineRichtungInfo.Text = "Kein Modus ausgewählt, verwende zufälligen Modus"
        '
        'lblWertBrandkante
        '
        Me.lblWertBrandkante.AutoSize = True
        Me.lblWertBrandkante.Location = New System.Drawing.Point(773, 423)
        Me.lblWertBrandkante.MinimumSize = New System.Drawing.Size(99, 0)
        Me.lblWertBrandkante.Name = "lblWertBrandkante"
        Me.lblWertBrandkante.Size = New System.Drawing.Size(99, 41)
        Me.lblWertBrandkante.TabIndex = 64
        Me.lblWertBrandkante.Tag = "langKey=lblWertBrandkante"
        Me.lblWertBrandkante.Text = "35"
        Me.lblWertBrandkante.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkBrandkantenbreite
        '
        Me.trkBrandkantenbreite.AutoSize = False
        Me.trkBrandkantenbreite.LargeChange = 10
        Me.trkBrandkantenbreite.Location = New System.Drawing.Point(285, 423)
        Me.trkBrandkantenbreite.Maximum = 100
        Me.trkBrandkantenbreite.Minimum = 10
        Me.trkBrandkantenbreite.Name = "trkBrandkantenbreite"
        Me.trkBrandkantenbreite.Size = New System.Drawing.Size(521, 57)
        Me.trkBrandkantenbreite.TabIndex = 63
        Me.trkBrandkantenbreite.TickFrequency = 10
        Me.trkBrandkantenbreite.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkBrandkantenbreite.Value = 35
        '
        'lblNtrkBrandkantenbreite
        '
        Me.lblNtrkBrandkantenbreite.AutoSize = True
        Me.lblNtrkBrandkantenbreite.Location = New System.Drawing.Point(17, 423)
        Me.lblNtrkBrandkantenbreite.Name = "lblNtrkBrandkantenbreite"
        Me.lblNtrkBrandkantenbreite.Size = New System.Drawing.Size(262, 41)
        Me.lblNtrkBrandkantenbreite.TabIndex = 62
        Me.lblNtrkBrandkantenbreite.Tag = "langKey=lblNtrkBrandkantenbreite"
        Me.lblNtrkBrandkantenbreite.Text = "Brandkantenbreite"
        '
        'chkZufallsdauer
        '
        Me.chkZufallsdauer.AutoSize = True
        Me.chkZufallsdauer.Location = New System.Drawing.Point(24, 604)
        Me.chkZufallsdauer.Name = "chkZufallsdauer"
        Me.chkZufallsdauer.Size = New System.Drawing.Size(250, 45)
        Me.chkZufallsdauer.TabIndex = 65
        Me.chkZufallsdauer.Text = "Zufällige Dauer"
        Me.chkZufallsdauer.UseVisualStyleBackColor = True
        '
        'grbFBM
        '
        Me.grbFBM.Controls.Add(Me.lblFBMStaerke)
        Me.grbFBM.Controls.Add(Me.lblFBMPersistenz)
        Me.grbFBM.Controls.Add(Me.lblFBMOktaven)
        Me.grbFBM.Controls.Add(Me.lblFBMGrundfrequenz)
        Me.grbFBM.Controls.Add(Me.trkFBMStaerke)
        Me.grbFBM.Controls.Add(Me.trkFBMPersistenz)
        Me.grbFBM.Controls.Add(Me.trkFBMOktaven)
        Me.grbFBM.Controls.Add(Me.lblNtrkFBMStaerke)
        Me.grbFBM.Controls.Add(Me.lblNtrkFBMPersistenz)
        Me.grbFBM.Controls.Add(Me.lblNtrkGrundfrequenz)
        Me.grbFBM.Controls.Add(Me.trkFBMGrundfrequenz)
        Me.grbFBM.Controls.Add(Me.lblNtrkFBMOktaven)
        Me.grbFBM.Location = New System.Drawing.Point(24, 655)
        Me.grbFBM.Name = "grbFBM"
        Me.grbFBM.Size = New System.Drawing.Size(848, 341)
        Me.grbFBM.TabIndex = 66
        Me.grbFBM.TabStop = False
        Me.grbFBM.Text = "FBM"
        '
        'lblNtrkFBMPersistenz
        '
        Me.lblNtrkFBMPersistenz.AutoSize = True
        Me.lblNtrkFBMPersistenz.Location = New System.Drawing.Point(30, 192)
        Me.lblNtrkFBMPersistenz.Name = "lblNtrkFBMPersistenz"
        Me.lblNtrkFBMPersistenz.Size = New System.Drawing.Size(150, 41)
        Me.lblNtrkFBMPersistenz.TabIndex = 67
        Me.lblNtrkFBMPersistenz.Text = "Persistenz"
        '
        'lblNtrkGrundfrequenz
        '
        Me.lblNtrkGrundfrequenz.AutoSize = True
        Me.lblNtrkGrundfrequenz.Location = New System.Drawing.Point(30, 46)
        Me.lblNtrkGrundfrequenz.Name = "lblNtrkGrundfrequenz"
        Me.lblNtrkGrundfrequenz.Size = New System.Drawing.Size(218, 41)
        Me.lblNtrkGrundfrequenz.TabIndex = 3
        Me.lblNtrkGrundfrequenz.Text = "Grundfrequenz"
        '
        'trkFBMGrundfrequenz
        '
        Me.trkFBMGrundfrequenz.AutoSize = False
        Me.trkFBMGrundfrequenz.Location = New System.Drawing.Point(271, 46)
        Me.trkFBMGrundfrequenz.Maximum = 40
        Me.trkFBMGrundfrequenz.Minimum = 2
        Me.trkFBMGrundfrequenz.Name = "trkFBMGrundfrequenz"
        Me.trkFBMGrundfrequenz.Size = New System.Drawing.Size(511, 60)
        Me.trkFBMGrundfrequenz.TabIndex = 66
        Me.trkFBMGrundfrequenz.Value = 16
        '
        'lblNtrkFBMOktaven
        '
        Me.lblNtrkFBMOktaven.AutoSize = True
        Me.lblNtrkFBMOktaven.Location = New System.Drawing.Point(30, 119)
        Me.lblNtrkFBMOktaven.Name = "lblNtrkFBMOktaven"
        Me.lblNtrkFBMOktaven.Size = New System.Drawing.Size(128, 41)
        Me.lblNtrkFBMOktaven.TabIndex = 1
        Me.lblNtrkFBMOktaven.Text = "Oktaven"
        '
        'lblNtrkFBMStaerke
        '
        Me.lblNtrkFBMStaerke.AutoSize = True
        Me.lblNtrkFBMStaerke.Location = New System.Drawing.Point(30, 265)
        Me.lblNtrkFBMStaerke.Name = "lblNtrkFBMStaerke"
        Me.lblNtrkFBMStaerke.Size = New System.Drawing.Size(99, 41)
        Me.lblNtrkFBMStaerke.TabIndex = 68
        Me.lblNtrkFBMStaerke.Text = "Stärke"
        '
        'trkFBMOktaven
        '
        Me.trkFBMOktaven.AutoSize = False
        Me.trkFBMOktaven.Location = New System.Drawing.Point(259, 119)
        Me.trkFBMOktaven.Maximum = 8
        Me.trkFBMOktaven.Minimum = 1
        Me.trkFBMOktaven.Name = "trkFBMOktaven"
        Me.trkFBMOktaven.Size = New System.Drawing.Size(523, 60)
        Me.trkFBMOktaven.TabIndex = 69
        Me.trkFBMOktaven.Value = 5
        '
        'trkFBMPersistenz
        '
        Me.trkFBMPersistenz.AutoSize = False
        Me.trkFBMPersistenz.Location = New System.Drawing.Point(261, 192)
        Me.trkFBMPersistenz.Maximum = 90
        Me.trkFBMPersistenz.Minimum = 10
        Me.trkFBMPersistenz.Name = "trkFBMPersistenz"
        Me.trkFBMPersistenz.Size = New System.Drawing.Size(521, 65)
        Me.trkFBMPersistenz.TabIndex = 70
        Me.trkFBMPersistenz.Value = 60
        '
        'trkFBMStaerke
        '
        Me.trkFBMStaerke.AutoSize = False
        Me.trkFBMStaerke.Location = New System.Drawing.Point(259, 265)
        Me.trkFBMStaerke.Maximum = 100
        Me.trkFBMStaerke.Name = "trkFBMStaerke"
        Me.trkFBMStaerke.Size = New System.Drawing.Size(523, 75)
        Me.trkFBMStaerke.TabIndex = 71
        Me.trkFBMStaerke.Value = 40
        '
        'lblFBMGrundfrequenz
        '
        Me.lblFBMGrundfrequenz.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFBMGrundfrequenz.AutoSize = True
        Me.lblFBMGrundfrequenz.Location = New System.Drawing.Point(791, 46)
        Me.lblFBMGrundfrequenz.Name = "lblFBMGrundfrequenz"
        Me.lblFBMGrundfrequenz.Size = New System.Drawing.Size(57, 41)
        Me.lblFBMGrundfrequenz.TabIndex = 72
        Me.lblFBMGrundfrequenz.Text = "8.0"
        Me.lblFBMGrundfrequenz.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblFBMOktaven
        '
        Me.lblFBMOktaven.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFBMOktaven.AutoSize = True
        Me.lblFBMOktaven.Location = New System.Drawing.Point(814, 119)
        Me.lblFBMOktaven.Name = "lblFBMOktaven"
        Me.lblFBMOktaven.Size = New System.Drawing.Size(34, 41)
        Me.lblFBMOktaven.TabIndex = 73
        Me.lblFBMOktaven.Text = "5"
        Me.lblFBMOktaven.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblFBMPersistenz
        '
        Me.lblFBMPersistenz.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFBMPersistenz.AutoSize = True
        Me.lblFBMPersistenz.Location = New System.Drawing.Point(791, 192)
        Me.lblFBMPersistenz.Name = "lblFBMPersistenz"
        Me.lblFBMPersistenz.Size = New System.Drawing.Size(57, 41)
        Me.lblFBMPersistenz.TabIndex = 74
        Me.lblFBMPersistenz.Text = "0.6"
        Me.lblFBMPersistenz.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblFBMStaerke
        '
        Me.lblFBMStaerke.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFBMStaerke.AutoSize = True
        Me.lblFBMStaerke.Location = New System.Drawing.Point(791, 265)
        Me.lblFBMStaerke.Name = "lblFBMStaerke"
        Me.lblFBMStaerke.Size = New System.Drawing.Size(57, 41)
        Me.lblFBMStaerke.TabIndex = 75
        Me.lblFBMStaerke.Text = "0.4"
        Me.lblFBMStaerke.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.grbFBM)
        Me.Controls.Add(Me.chkZufallsdauer)
        Me.Controls.Add(Me.lblWertBrandkante)
        Me.Controls.Add(Me.trkBrandkantenbreite)
        Me.Controls.Add(Me.lblNtrkBrandkantenbreite)
        Me.Controls.Add(Me.lblKeineRichtungInfo)
        Me.Controls.Add(Me.lblNModus)
        Me.Controls.Add(Me.clbModus)
        Me.Controls.Add(Me.chkPartikel)
        Me.Controls.Add(Me.lblGeschwindigkeit)
        Me.Controls.Add(Me.trkDauer)
        Me.Controls.Add(Me.lblNtrbDauer)
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.lblTransitionsname)
        Me.Controls.Add(Me.lblNlblTransitionname)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkDauer, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkBrandkantenbreite, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grbFBM.ResumeLayout(False)
        Me.grbFBM.PerformLayout()
        CType(Me.trkFBMGrundfrequenz, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkFBMOktaven, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkFBMPersistenz, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkFBMStaerke, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnDefaults As Windows.Forms.Button
    Friend WithEvents lblTransitionsname As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionname As Windows.Forms.Label
    Friend WithEvents lblGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents trkDauer As Windows.Forms.TrackBar
    Friend WithEvents lblNtrbDauer As Windows.Forms.Label
    Friend WithEvents chkPartikel As Windows.Forms.CheckBox
    Friend WithEvents lblNModus As Windows.Forms.Label
    Friend WithEvents clbModus As Windows.Forms.CheckedListBox
    Friend WithEvents lblKeineRichtungInfo As Windows.Forms.Label
    Friend WithEvents lblWertBrandkante As Windows.Forms.Label
    Friend WithEvents trkBrandkantenbreite As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkBrandkantenbreite As Windows.Forms.Label
    Friend WithEvents chkZufallsdauer As Windows.Forms.CheckBox
    Friend WithEvents grbFBM As Windows.Forms.GroupBox
    Friend WithEvents lblNtrkFBMOktaven As Windows.Forms.Label
    Friend WithEvents lblNtrkGrundfrequenz As Windows.Forms.Label
    Friend WithEvents trkFBMGrundfrequenz As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkFBMPersistenz As Windows.Forms.Label
    Friend WithEvents trkFBMOktaven As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkFBMStaerke As Windows.Forms.Label
    Friend WithEvents trkFBMPersistenz As Windows.Forms.TrackBar
    Friend WithEvents trkFBMStaerke As Windows.Forms.TrackBar
    Friend WithEvents lblFBMPersistenz As Windows.Forms.Label
    Friend WithEvents lblFBMOktaven As Windows.Forms.Label
    Friend WithEvents lblFBMGrundfrequenz As Windows.Forms.Label
    Friend WithEvents lblFBMStaerke As Windows.Forms.Label
End Class
