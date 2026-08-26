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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucOptionsTransition))
        Me.lblTransitonName = New System.Windows.Forms.Label()
        Me.lblNlblTransitionName = New System.Windows.Forms.Label()
        Me.chkWindstaerkeZufall = New System.Windows.Forms.CheckBox()
        Me.trkWindstaerke = New System.Windows.Forms.TrackBar()
        Me.lblNtrkWindstaerke = New System.Windows.Forms.Label()
        Me.lblWindstaerke = New System.Windows.Forms.Label()
        Me.btnDefaults = New System.Windows.Forms.Button()
        Me.lblFlauteWarnung = New System.Windows.Forms.Label()
        Me.lblDauerAbrisskante = New System.Windows.Forms.Label()
        Me.lblNtrkDauerAbrisskante = New System.Windows.Forms.Label()
        Me.trkDauerAbrisskante = New System.Windows.Forms.TrackBar()
        Me.grpSchwerkraft = New System.Windows.Forms.GroupBox()
        Me.rbSchwerkraftZufällig = New System.Windows.Forms.RadioButton()
        Me.rbSchwerkraftAn = New System.Windows.Forms.RadioButton()
        Me.rbSchwerkraftAus = New System.Windows.Forms.RadioButton()
        Me.grbPartikelgröße = New System.Windows.Forms.GroupBox()
        Me.rbManuellePGroesse = New System.Windows.Forms.RadioButton()
        Me.lblPartikelGroesse = New System.Windows.Forms.Label()
        Me.lblNtrkParikelGroesse = New System.Windows.Forms.Label()
        Me.trkPartikelGroesse = New System.Windows.Forms.TrackBar()
        Me.rbZufallsFestePGroesse = New System.Windows.Forms.RadioButton()
        Me.rbGemischtePartikel = New System.Windows.Forms.RadioButton()
        Me.rbZufallsPGroessenModus = New System.Windows.Forms.RadioButton()
        CType(Me.trkWindstaerke, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkDauerAbrisskante, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpSchwerkraft.SuspendLayout()
        Me.grbPartikelgröße.SuspendLayout()
        CType(Me.trkPartikelGroesse, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.lblWindstaerke.Location = New System.Drawing.Point(769, 338)
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
        Me.btnDefaults.Location = New System.Drawing.Point(689, 23)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(174, 52)
        Me.btnDefaults.TabIndex = 55
        Me.btnDefaults.Tag = "langKey=btnDefaults"
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'lblFlauteWarnung
        '
        Me.lblFlauteWarnung.AutoSize = True
        Me.lblFlauteWarnung.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFlauteWarnung.ForeColor = System.Drawing.Color.Red
        Me.lblFlauteWarnung.Location = New System.Drawing.Point(41, 463)
        Me.lblFlauteWarnung.MaximumSize = New System.Drawing.Size(840, 0)
        Me.lblFlauteWarnung.Name = "lblFlauteWarnung"
        Me.lblFlauteWarnung.Size = New System.Drawing.Size(809, 205)
        Me.lblFlauteWarnung.TabIndex = 56
        Me.lblFlauteWarnung.Text = resources.GetString("lblFlauteWarnung.Text")
        '
        'lblDauerAbrisskante
        '
        Me.lblDauerAbrisskante.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblDauerAbrisskante.AutoSize = True
        Me.lblDauerAbrisskante.Location = New System.Drawing.Point(769, 687)
        Me.lblDauerAbrisskante.MaximumSize = New System.Drawing.Size(94, 0)
        Me.lblDauerAbrisskante.MinimumSize = New System.Drawing.Size(94, 0)
        Me.lblDauerAbrisskante.Name = "lblDauerAbrisskante"
        Me.lblDauerAbrisskante.Size = New System.Drawing.Size(94, 41)
        Me.lblDauerAbrisskante.TabIndex = 59
        Me.lblDauerAbrisskante.Text = "7 s"
        Me.lblDauerAbrisskante.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblNtrkDauerAbrisskante
        '
        Me.lblNtrkDauerAbrisskante.AutoSize = True
        Me.lblNtrkDauerAbrisskante.Location = New System.Drawing.Point(41, 687)
        Me.lblNtrkDauerAbrisskante.MaximumSize = New System.Drawing.Size(222, 0)
        Me.lblNtrkDauerAbrisskante.MinimumSize = New System.Drawing.Size(222, 0)
        Me.lblNtrkDauerAbrisskante.Name = "lblNtrkDauerAbrisskante"
        Me.lblNtrkDauerAbrisskante.Size = New System.Drawing.Size(222, 82)
        Me.lblNtrkDauerAbrisskante.TabIndex = 58
        Me.lblNtrkDauerAbrisskante.Text = "Dauer Abrisskante"
        '
        'trkDauerAbrisskante
        '
        Me.trkDauerAbrisskante.AutoSize = False
        Me.trkDauerAbrisskante.Location = New System.Drawing.Point(271, 687)
        Me.trkDauerAbrisskante.Maximum = 30
        Me.trkDauerAbrisskante.Minimum = 5
        Me.trkDauerAbrisskante.Name = "trkDauerAbrisskante"
        Me.trkDauerAbrisskante.Size = New System.Drawing.Size(495, 61)
        Me.trkDauerAbrisskante.TabIndex = 57
        Me.trkDauerAbrisskante.TabStop = False
        Me.trkDauerAbrisskante.Value = 7
        '
        'grpSchwerkraft
        '
        Me.grpSchwerkraft.Controls.Add(Me.rbSchwerkraftZufällig)
        Me.grpSchwerkraft.Controls.Add(Me.rbSchwerkraftAn)
        Me.grpSchwerkraft.Controls.Add(Me.rbSchwerkraftAus)
        Me.grpSchwerkraft.Location = New System.Drawing.Point(41, 792)
        Me.grpSchwerkraft.Name = "grpSchwerkraft"
        Me.grpSchwerkraft.Size = New System.Drawing.Size(814, 112)
        Me.grpSchwerkraft.TabIndex = 69
        Me.grpSchwerkraft.TabStop = False
        Me.grpSchwerkraft.Text = "Schwerkraft"
        '
        'rbSchwerkraftZufällig
        '
        Me.rbSchwerkraftZufällig.AutoSize = True
        Me.rbSchwerkraftZufällig.Location = New System.Drawing.Point(553, 46)
        Me.rbSchwerkraftZufällig.Name = "rbSchwerkraftZufällig"
        Me.rbSchwerkraftZufällig.Size = New System.Drawing.Size(146, 45)
        Me.rbSchwerkraftZufällig.TabIndex = 2
        Me.rbSchwerkraftZufällig.TabStop = True
        Me.rbSchwerkraftZufällig.Text = "Zufällig"
        Me.rbSchwerkraftZufällig.UseVisualStyleBackColor = True
        '
        'rbSchwerkraftAn
        '
        Me.rbSchwerkraftAn.AutoSize = True
        Me.rbSchwerkraftAn.Location = New System.Drawing.Point(6, 46)
        Me.rbSchwerkraftAn.Name = "rbSchwerkraftAn"
        Me.rbSchwerkraftAn.Size = New System.Drawing.Size(245, 45)
        Me.rbSchwerkraftAn.TabIndex = 1
        Me.rbSchwerkraftAn.TabStop = True
        Me.rbSchwerkraftAn.Text = "An (Bild hängt)"
        Me.rbSchwerkraftAn.UseVisualStyleBackColor = True
        '
        'rbSchwerkraftAus
        '
        Me.rbSchwerkraftAus.AutoSize = True
        Me.rbSchwerkraftAus.Location = New System.Drawing.Point(276, 46)
        Me.rbSchwerkraftAus.Name = "rbSchwerkraftAus"
        Me.rbSchwerkraftAus.Size = New System.Drawing.Size(245, 45)
        Me.rbSchwerkraftAus.TabIndex = 0
        Me.rbSchwerkraftAus.TabStop = True
        Me.rbSchwerkraftAus.Text = "Aus (Top View)"
        Me.rbSchwerkraftAus.UseVisualStyleBackColor = True
        '
        'grbPartikelgröße
        '
        Me.grbPartikelgröße.Controls.Add(Me.rbZufallsPGroessenModus)
        Me.grbPartikelgröße.Controls.Add(Me.rbGemischtePartikel)
        Me.grbPartikelgröße.Controls.Add(Me.rbZufallsFestePGroesse)
        Me.grbPartikelgröße.Controls.Add(Me.lblPartikelGroesse)
        Me.grbPartikelgröße.Controls.Add(Me.lblNtrkParikelGroesse)
        Me.grbPartikelgröße.Controls.Add(Me.trkPartikelGroesse)
        Me.grbPartikelgröße.Controls.Add(Me.rbManuellePGroesse)
        Me.grbPartikelgröße.Location = New System.Drawing.Point(41, 81)
        Me.grbPartikelgröße.Name = "grbPartikelgröße"
        Me.grbPartikelgröße.Size = New System.Drawing.Size(814, 239)
        Me.grbPartikelgröße.TabIndex = 70
        Me.grbPartikelgröße.TabStop = False
        Me.grbPartikelgröße.Text = "Partikelgröße"
        '
        'rbManuellePGroesse
        '
        Me.rbManuellePGroesse.AutoSize = True
        Me.rbManuellePGroesse.Location = New System.Drawing.Point(23, 47)
        Me.rbManuellePGroesse.Name = "rbManuellePGroesse"
        Me.rbManuellePGroesse.Size = New System.Drawing.Size(155, 45)
        Me.rbManuellePGroesse.TabIndex = 0
        Me.rbManuellePGroesse.TabStop = True
        Me.rbManuellePGroesse.Text = "Manuell"
        Me.rbManuellePGroesse.UseVisualStyleBackColor = True
        '
        'lblPartikelGroesse
        '
        Me.lblPartikelGroesse.AutoSize = True
        Me.lblPartikelGroesse.Location = New System.Drawing.Point(715, 163)
        Me.lblPartikelGroesse.MaximumSize = New System.Drawing.Size(106, 0)
        Me.lblPartikelGroesse.MinimumSize = New System.Drawing.Size(106, 0)
        Me.lblPartikelGroesse.Name = "lblPartikelGroesse"
        Me.lblPartikelGroesse.Size = New System.Drawing.Size(106, 41)
        Me.lblPartikelGroesse.TabIndex = 29
        Me.lblPartikelGroesse.Text = "4 px"
        Me.lblPartikelGroesse.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblNtrkParikelGroesse
        '
        Me.lblNtrkParikelGroesse.AutoSize = True
        Me.lblNtrkParikelGroesse.Location = New System.Drawing.Point(-1, 163)
        Me.lblNtrkParikelGroesse.Name = "lblNtrkParikelGroesse"
        Me.lblNtrkParikelGroesse.Size = New System.Drawing.Size(192, 41)
        Me.lblNtrkParikelGroesse.TabIndex = 28
        Me.lblNtrkParikelGroesse.Text = "Partikelgröße"
        '
        'trkPartikelGroesse
        '
        Me.trkPartikelGroesse.AutoSize = False
        Me.trkPartikelGroesse.Location = New System.Drawing.Point(229, 163)
        Me.trkPartikelGroesse.Maximum = 7
        Me.trkPartikelGroesse.Name = "trkPartikelGroesse"
        Me.trkPartikelGroesse.Size = New System.Drawing.Size(495, 55)
        Me.trkPartikelGroesse.TabIndex = 27
        Me.trkPartikelGroesse.Value = 2
        '
        'rbZufallsFestePGroesse
        '
        Me.rbZufallsFestePGroesse.AutoSize = True
        Me.rbZufallsFestePGroesse.Location = New System.Drawing.Point(376, 47)
        Me.rbZufallsFestePGroesse.Name = "rbZufallsFestePGroesse"
        Me.rbZufallsFestePGroesse.Size = New System.Drawing.Size(323, 45)
        Me.rbZufallsFestePGroesse.TabIndex = 30
        Me.rbZufallsFestePGroesse.TabStop = True
        Me.rbZufallsFestePGroesse.Text = "Zufällige feste Größe"
        Me.rbZufallsFestePGroesse.UseVisualStyleBackColor = True
        '
        'rbGemischtePartikel
        '
        Me.rbGemischtePartikel.AutoSize = True
        Me.rbGemischtePartikel.Location = New System.Drawing.Point(23, 98)
        Me.rbGemischtePartikel.Name = "rbGemischtePartikel"
        Me.rbGemischtePartikel.Size = New System.Drawing.Size(295, 45)
        Me.rbGemischtePartikel.TabIndex = 31
        Me.rbGemischtePartikel.TabStop = True
        Me.rbGemischtePartikel.Text = "Gemischte Größen"
        Me.rbGemischtePartikel.UseVisualStyleBackColor = True
        '
        'rbZufallsPGroessenModus
        '
        Me.rbZufallsPGroessenModus.AutoSize = True
        Me.rbZufallsPGroessenModus.Location = New System.Drawing.Point(376, 98)
        Me.rbZufallsPGroessenModus.Name = "rbZufallsPGroessenModus"
        Me.rbZufallsPGroessenModus.Size = New System.Drawing.Size(370, 45)
        Me.rbZufallsPGroessenModus.TabIndex = 32
        Me.rbZufallsPGroessenModus.TabStop = True
        Me.rbZufallsPGroessenModus.Text = "Zufälliger Größenmodus"
        Me.rbZufallsPGroessenModus.UseVisualStyleBackColor = True
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.grbPartikelgröße)
        Me.Controls.Add(Me.grpSchwerkraft)
        Me.Controls.Add(Me.lblDauerAbrisskante)
        Me.Controls.Add(Me.lblNtrkDauerAbrisskante)
        Me.Controls.Add(Me.trkDauerAbrisskante)
        Me.Controls.Add(Me.lblFlauteWarnung)
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.lblWindstaerke)
        Me.Controls.Add(Me.lblNtrkWindstaerke)
        Me.Controls.Add(Me.trkWindstaerke)
        Me.Controls.Add(Me.chkWindstaerkeZufall)
        Me.Controls.Add(Me.lblTransitonName)
        Me.Controls.Add(Me.lblNlblTransitionName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkWindstaerke, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkDauerAbrisskante, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpSchwerkraft.ResumeLayout(False)
        Me.grpSchwerkraft.PerformLayout()
        Me.grbPartikelgröße.ResumeLayout(False)
        Me.grbPartikelgröße.PerformLayout()
        CType(Me.trkPartikelGroesse, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblTransitonName As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionName As Windows.Forms.Label
    Friend WithEvents chkWindstaerkeZufall As Windows.Forms.CheckBox
    Friend WithEvents trkWindstaerke As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkWindstaerke As Windows.Forms.Label
    Friend WithEvents lblWindstaerke As Windows.Forms.Label
    Friend WithEvents btnDefaults As Windows.Forms.Button
    Friend WithEvents lblFlauteWarnung As Windows.Forms.Label
    Friend WithEvents lblDauerAbrisskante As Windows.Forms.Label
    Friend WithEvents lblNtrkDauerAbrisskante As Windows.Forms.Label
    Friend WithEvents trkDauerAbrisskante As Windows.Forms.TrackBar
    Friend WithEvents grpSchwerkraft As Windows.Forms.GroupBox
    Friend WithEvents rbSchwerkraftZufällig As Windows.Forms.RadioButton
    Friend WithEvents rbSchwerkraftAn As Windows.Forms.RadioButton
    Friend WithEvents rbSchwerkraftAus As Windows.Forms.RadioButton
    Friend WithEvents grbPartikelgröße As Windows.Forms.GroupBox
    Friend WithEvents rbZufallsPGroessenModus As Windows.Forms.RadioButton
    Friend WithEvents rbGemischtePartikel As Windows.Forms.RadioButton
    Friend WithEvents rbZufallsFestePGroesse As Windows.Forms.RadioButton
    Friend WithEvents lblPartikelGroesse As Windows.Forms.Label
    Friend WithEvents lblNtrkParikelGroesse As Windows.Forms.Label
    Friend WithEvents trkPartikelGroesse As Windows.Forms.TrackBar
    Friend WithEvents rbManuellePGroesse As Windows.Forms.RadioButton
End Class
