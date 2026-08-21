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
        Me.lblNModus = New System.Windows.Forms.Label()
        Me.clbModus = New System.Windows.Forms.CheckedListBox()
        Me.lblKeineRichtungInfo = New System.Windows.Forms.Label()
        Me.chkZufallsdauer = New System.Windows.Forms.CheckBox()
        Me.tcBP = New System.Windows.Forms.TabControl()
        Me.tpPartikel = New System.Windows.Forms.TabPage()
        Me.lblPartikelLebensdauer = New System.Windows.Forms.Label()
        Me.trkPartikelLebensdauer = New System.Windows.Forms.TrackBar()
        Me.lblNtrkPartikelLebensdauer = New System.Windows.Forms.Label()
        Me.grpSchwerkraft = New System.Windows.Forms.GroupBox()
        Me.rbSchwerkraftZufällig = New System.Windows.Forms.RadioButton()
        Me.rbSchwerkraftAn = New System.Windows.Forms.RadioButton()
        Me.rbSchwerkraftAus = New System.Windows.Forms.RadioButton()
        Me.tpGradient = New System.Windows.Forms.TabPage()
        Me.lblWertBrandkante = New System.Windows.Forms.Label()
        Me.trkBrandkantenbreite = New System.Windows.Forms.TrackBar()
        Me.lblNtrkBrandkantenbreite = New System.Windows.Forms.Label()
        Me.tpVerzerrung = New System.Windows.Forms.TabPage()
        Me.lblScherbengroesse = New System.Windows.Forms.Label()
        Me.trkScherbengroesse = New System.Windows.Forms.TrackBar()
        Me.lblNtrkScherbengroesse = New System.Windows.Forms.Label()
        Me.lblVerzerrungEffektstaerke = New System.Windows.Forms.Label()
        Me.trkVerzerrungsstaerke = New System.Windows.Forms.TrackBar()
        Me.lblNtrkVerzerrungEffektstärke = New System.Windows.Forms.Label()
        Me.lblVerzerrungsbreite = New System.Windows.Forms.Label()
        Me.trkVerzerrungsbreite = New System.Windows.Forms.TrackBar()
        Me.lblNtrkVerzerrungsbreite = New System.Windows.Forms.Label()
        Me.tpTextur = New System.Windows.Forms.TabPage()
        Me.tpKonfig = New System.Windows.Forms.TabPage()
        Me.lblFBMStaerke = New System.Windows.Forms.Label()
        Me.lblFBMPersistenz = New System.Windows.Forms.Label()
        Me.lblFBMOktaven = New System.Windows.Forms.Label()
        Me.lblFBMGrundfrequenz = New System.Windows.Forms.Label()
        Me.trkFBMStaerke = New System.Windows.Forms.TrackBar()
        Me.trkFBMPersistenz = New System.Windows.Forms.TrackBar()
        Me.trkFBMOktaven = New System.Windows.Forms.TrackBar()
        Me.lblNtrkFBMStaerke = New System.Windows.Forms.Label()
        Me.lblNtrkFBMPersistenz = New System.Windows.Forms.Label()
        Me.lblNtrkGrundfrequenz = New System.Windows.Forms.Label()
        Me.trkFBMGrundfrequenz = New System.Windows.Forms.TrackBar()
        Me.lblNtrkFBMOktaven = New System.Windows.Forms.Label()
        Me.grbZuendmodus = New System.Windows.Forms.GroupBox()
        Me.rbZuendmodusZufall = New System.Windows.Forms.RadioButton()
        Me.rbZuendModusBrandherde = New System.Windows.Forms.RadioButton()
        Me.rbZuendmodusBrandRand = New System.Windows.Forms.RadioButton()
        Me.grbEffekte = New System.Windows.Forms.GroupBox()
        Me.chkTextur = New System.Windows.Forms.CheckBox()
        Me.chkVerzerrung = New System.Windows.Forms.CheckBox()
        Me.chkGradient = New System.Windows.Forms.CheckBox()
        Me.chkPartikel = New System.Windows.Forms.CheckBox()
        CType(Me.trkDauer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tcBP.SuspendLayout()
        Me.tpPartikel.SuspendLayout()
        CType(Me.trkPartikelLebensdauer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpSchwerkraft.SuspendLayout()
        Me.tpGradient.SuspendLayout()
        CType(Me.trkBrandkantenbreite, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpVerzerrung.SuspendLayout()
        CType(Me.trkScherbengroesse, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkVerzerrungsstaerke, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkVerzerrungsbreite, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpKonfig.SuspendLayout()
        CType(Me.trkFBMStaerke, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkFBMPersistenz, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkFBMOktaven, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkFBMGrundfrequenz, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grbZuendmodus.SuspendLayout()
        Me.grbEffekte.SuspendLayout()
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
        Me.lblGeschwindigkeit.Location = New System.Drawing.Point(772, 870)
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
        Me.trkDauer.Location = New System.Drawing.Point(284, 870)
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
        Me.lblNtrbDauer.Location = New System.Drawing.Point(16, 870)
        Me.lblNtrbDauer.Name = "lblNtrbDauer"
        Me.lblNtrbDauer.Size = New System.Drawing.Size(97, 41)
        Me.lblNtrbDauer.TabIndex = 55
        Me.lblNtrbDauer.Tag = "langKey=lblNtrbDauer"
        Me.lblNtrbDauer.Text = "Dauer"
        '
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Location = New System.Drawing.Point(18, 108)
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
        Me.clbModus.Location = New System.Drawing.Point(284, 108)
        Me.clbModus.Name = "clbModus"
        Me.clbModus.Size = New System.Drawing.Size(587, 136)
        Me.clbModus.TabIndex = 59
        Me.clbModus.Tag = "langKey=clbModus"
        '
        'lblKeineRichtungInfo
        '
        Me.lblKeineRichtungInfo.AutoSize = True
        Me.lblKeineRichtungInfo.Location = New System.Drawing.Point(277, 247)
        Me.lblKeineRichtungInfo.MaximumSize = New System.Drawing.Size(587, 0)
        Me.lblKeineRichtungInfo.Name = "lblKeineRichtungInfo"
        Me.lblKeineRichtungInfo.Size = New System.Drawing.Size(553, 41)
        Me.lblKeineRichtungInfo.TabIndex = 61
        Me.lblKeineRichtungInfo.Tag = "langKey=lblKeineRichtungInfo"
        Me.lblKeineRichtungInfo.Text = "Kein Modus ausgewählt → Zufallsmodus"
        '
        'chkZufallsdauer
        '
        Me.chkZufallsdauer.AutoSize = True
        Me.chkZufallsdauer.Location = New System.Drawing.Point(25, 934)
        Me.chkZufallsdauer.Name = "chkZufallsdauer"
        Me.chkZufallsdauer.Size = New System.Drawing.Size(250, 45)
        Me.chkZufallsdauer.TabIndex = 65
        Me.chkZufallsdauer.Text = "Zufällige Dauer"
        Me.chkZufallsdauer.UseVisualStyleBackColor = True
        '
        'tcBP
        '
        Me.tcBP.Controls.Add(Me.tpPartikel)
        Me.tcBP.Controls.Add(Me.tpGradient)
        Me.tcBP.Controls.Add(Me.tpVerzerrung)
        Me.tcBP.Controls.Add(Me.tpTextur)
        Me.tcBP.Controls.Add(Me.tpKonfig)
        Me.tcBP.Location = New System.Drawing.Point(15, 501)
        Me.tcBP.Name = "tcBP"
        Me.tcBP.SelectedIndex = 0
        Me.tcBP.Size = New System.Drawing.Size(856, 349)
        Me.tcBP.TabIndex = 68
        '
        'tpPartikel
        '
        Me.tpPartikel.Controls.Add(Me.lblPartikelLebensdauer)
        Me.tpPartikel.Controls.Add(Me.trkPartikelLebensdauer)
        Me.tpPartikel.Controls.Add(Me.lblNtrkPartikelLebensdauer)
        Me.tpPartikel.Controls.Add(Me.grpSchwerkraft)
        Me.tpPartikel.Location = New System.Drawing.Point(10, 59)
        Me.tpPartikel.Name = "tpPartikel"
        Me.tpPartikel.Padding = New System.Windows.Forms.Padding(3)
        Me.tpPartikel.Size = New System.Drawing.Size(836, 280)
        Me.tpPartikel.TabIndex = 0
        Me.tpPartikel.Text = "Partikel"
        Me.tpPartikel.UseVisualStyleBackColor = True
        '
        'lblPartikelLebensdauer
        '
        Me.lblPartikelLebensdauer.AutoSize = True
        Me.lblPartikelLebensdauer.Location = New System.Drawing.Point(750, 134)
        Me.lblPartikelLebensdauer.MaximumSize = New System.Drawing.Size(80, 0)
        Me.lblPartikelLebensdauer.MinimumSize = New System.Drawing.Size(80, 0)
        Me.lblPartikelLebensdauer.Name = "lblPartikelLebensdauer"
        Me.lblPartikelLebensdauer.Size = New System.Drawing.Size(80, 41)
        Me.lblPartikelLebensdauer.TabIndex = 86
        Me.lblPartikelLebensdauer.Tag = "langKey=lblWertBrandkante"
        Me.lblPartikelLebensdauer.Text = "3,2 s"
        Me.lblPartikelLebensdauer.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkPartikelLebensdauer
        '
        Me.trkPartikelLebensdauer.AutoSize = False
        Me.trkPartikelLebensdauer.LargeChange = 10
        Me.trkPartikelLebensdauer.Location = New System.Drawing.Point(258, 134)
        Me.trkPartikelLebensdauer.Maximum = 50
        Me.trkPartikelLebensdauer.Minimum = 10
        Me.trkPartikelLebensdauer.Name = "trkPartikelLebensdauer"
        Me.trkPartikelLebensdauer.Size = New System.Drawing.Size(483, 57)
        Me.trkPartikelLebensdauer.TabIndex = 85
        Me.trkPartikelLebensdauer.TickFrequency = 10
        Me.trkPartikelLebensdauer.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkPartikelLebensdauer.Value = 32
        '
        'lblNtrkPartikelLebensdauer
        '
        Me.lblNtrkPartikelLebensdauer.AutoSize = True
        Me.lblNtrkPartikelLebensdauer.Location = New System.Drawing.Point(8, 134)
        Me.lblNtrkPartikelLebensdauer.Name = "lblNtrkPartikelLebensdauer"
        Me.lblNtrkPartikelLebensdauer.Size = New System.Drawing.Size(188, 41)
        Me.lblNtrkPartikelLebensdauer.TabIndex = 84
        Me.lblNtrkPartikelLebensdauer.Tag = "langKey=lblNtrkBrandkantenbreite"
        Me.lblNtrkPartikelLebensdauer.Text = "Lebensdauer"
        '
        'grpSchwerkraft
        '
        Me.grpSchwerkraft.Controls.Add(Me.rbSchwerkraftZufällig)
        Me.grpSchwerkraft.Controls.Add(Me.rbSchwerkraftAn)
        Me.grpSchwerkraft.Controls.Add(Me.rbSchwerkraftAus)
        Me.grpSchwerkraft.Location = New System.Drawing.Point(16, 16)
        Me.grpSchwerkraft.Name = "grpSchwerkraft"
        Me.grpSchwerkraft.Size = New System.Drawing.Size(814, 112)
        Me.grpSchwerkraft.TabIndex = 68
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
        'tpGradient
        '
        Me.tpGradient.Controls.Add(Me.lblWertBrandkante)
        Me.tpGradient.Controls.Add(Me.trkBrandkantenbreite)
        Me.tpGradient.Controls.Add(Me.lblNtrkBrandkantenbreite)
        Me.tpGradient.Location = New System.Drawing.Point(10, 59)
        Me.tpGradient.Name = "tpGradient"
        Me.tpGradient.Padding = New System.Windows.Forms.Padding(3)
        Me.tpGradient.Size = New System.Drawing.Size(836, 280)
        Me.tpGradient.TabIndex = 1
        Me.tpGradient.Text = "Gradient"
        Me.tpGradient.UseVisualStyleBackColor = True
        '
        'lblWertBrandkante
        '
        Me.lblWertBrandkante.AutoSize = True
        Me.lblWertBrandkante.Location = New System.Drawing.Point(760, 25)
        Me.lblWertBrandkante.MaximumSize = New System.Drawing.Size(70, 0)
        Me.lblWertBrandkante.MinimumSize = New System.Drawing.Size(70, 0)
        Me.lblWertBrandkante.Name = "lblWertBrandkante"
        Me.lblWertBrandkante.Size = New System.Drawing.Size(70, 41)
        Me.lblWertBrandkante.TabIndex = 83
        Me.lblWertBrandkante.Tag = "langKey=lblWertBrandkante"
        Me.lblWertBrandkante.Text = "35"
        Me.lblWertBrandkante.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkBrandkantenbreite
        '
        Me.trkBrandkantenbreite.AutoSize = False
        Me.trkBrandkantenbreite.LargeChange = 10
        Me.trkBrandkantenbreite.Location = New System.Drawing.Point(276, 25)
        Me.trkBrandkantenbreite.Maximum = 100
        Me.trkBrandkantenbreite.Minimum = 10
        Me.trkBrandkantenbreite.Name = "trkBrandkantenbreite"
        Me.trkBrandkantenbreite.Size = New System.Drawing.Size(465, 57)
        Me.trkBrandkantenbreite.TabIndex = 82
        Me.trkBrandkantenbreite.TickFrequency = 10
        Me.trkBrandkantenbreite.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkBrandkantenbreite.Value = 35
        '
        'lblNtrkBrandkantenbreite
        '
        Me.lblNtrkBrandkantenbreite.AutoSize = True
        Me.lblNtrkBrandkantenbreite.Location = New System.Drawing.Point(8, 25)
        Me.lblNtrkBrandkantenbreite.Name = "lblNtrkBrandkantenbreite"
        Me.lblNtrkBrandkantenbreite.Size = New System.Drawing.Size(262, 41)
        Me.lblNtrkBrandkantenbreite.TabIndex = 81
        Me.lblNtrkBrandkantenbreite.Tag = "langKey=lblNtrkBrandkantenbreite"
        Me.lblNtrkBrandkantenbreite.Text = "Brandkantenbreite"
        '
        'tpVerzerrung
        '
        Me.tpVerzerrung.Controls.Add(Me.lblScherbengroesse)
        Me.tpVerzerrung.Controls.Add(Me.trkScherbengroesse)
        Me.tpVerzerrung.Controls.Add(Me.lblNtrkScherbengroesse)
        Me.tpVerzerrung.Controls.Add(Me.lblVerzerrungEffektstaerke)
        Me.tpVerzerrung.Controls.Add(Me.trkVerzerrungsstaerke)
        Me.tpVerzerrung.Controls.Add(Me.lblNtrkVerzerrungEffektstärke)
        Me.tpVerzerrung.Controls.Add(Me.lblVerzerrungsbreite)
        Me.tpVerzerrung.Controls.Add(Me.trkVerzerrungsbreite)
        Me.tpVerzerrung.Controls.Add(Me.lblNtrkVerzerrungsbreite)
        Me.tpVerzerrung.Location = New System.Drawing.Point(10, 59)
        Me.tpVerzerrung.Name = "tpVerzerrung"
        Me.tpVerzerrung.Padding = New System.Windows.Forms.Padding(3)
        Me.tpVerzerrung.Size = New System.Drawing.Size(836, 280)
        Me.tpVerzerrung.TabIndex = 2
        Me.tpVerzerrung.Text = "Verzerrung"
        Me.tpVerzerrung.UseVisualStyleBackColor = True
        '
        'lblScherbengroesse
        '
        Me.lblScherbengroesse.AutoSize = True
        Me.lblScherbengroesse.Location = New System.Drawing.Point(755, 178)
        Me.lblScherbengroesse.MaximumSize = New System.Drawing.Size(70, 0)
        Me.lblScherbengroesse.MinimumSize = New System.Drawing.Size(70, 0)
        Me.lblScherbengroesse.Name = "lblScherbengroesse"
        Me.lblScherbengroesse.Size = New System.Drawing.Size(70, 41)
        Me.lblScherbengroesse.TabIndex = 92
        Me.lblScherbengroesse.Tag = "langKey=lblWertBrandkante"
        Me.lblScherbengroesse.Text = "14"
        Me.lblScherbengroesse.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkScherbengroesse
        '
        Me.trkScherbengroesse.AutoSize = False
        Me.trkScherbengroesse.LargeChange = 10
        Me.trkScherbengroesse.Location = New System.Drawing.Point(271, 178)
        Me.trkScherbengroesse.Maximum = 30
        Me.trkScherbengroesse.Minimum = 5
        Me.trkScherbengroesse.Name = "trkScherbengroesse"
        Me.trkScherbengroesse.Size = New System.Drawing.Size(465, 57)
        Me.trkScherbengroesse.TabIndex = 91
        Me.trkScherbengroesse.TickFrequency = 10
        Me.trkScherbengroesse.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkScherbengroesse.Value = 14
        '
        'lblNtrkScherbengroesse
        '
        Me.lblNtrkScherbengroesse.AutoSize = True
        Me.lblNtrkScherbengroesse.Location = New System.Drawing.Point(3, 178)
        Me.lblNtrkScherbengroesse.MaximumSize = New System.Drawing.Size(262, 0)
        Me.lblNtrkScherbengroesse.Name = "lblNtrkScherbengroesse"
        Me.lblNtrkScherbengroesse.Size = New System.Drawing.Size(228, 82)
        Me.lblNtrkScherbengroesse.TabIndex = 90
        Me.lblNtrkScherbengroesse.Tag = "langKey=lblNtrkBrandkantenbreite"
        Me.lblNtrkScherbengroesse.Text = "Scherbengröße (nur für Magie)"
        '
        'lblVerzerrungEffektstaerke
        '
        Me.lblVerzerrungEffektstaerke.AutoSize = True
        Me.lblVerzerrungEffektstaerke.Location = New System.Drawing.Point(755, 103)
        Me.lblVerzerrungEffektstaerke.MaximumSize = New System.Drawing.Size(70, 0)
        Me.lblVerzerrungEffektstaerke.MinimumSize = New System.Drawing.Size(70, 0)
        Me.lblVerzerrungEffektstaerke.Name = "lblVerzerrungEffektstaerke"
        Me.lblVerzerrungEffektstaerke.Size = New System.Drawing.Size(70, 41)
        Me.lblVerzerrungEffektstaerke.TabIndex = 89
        Me.lblVerzerrungEffektstaerke.Tag = "langKey=lblWertBrandkante"
        Me.lblVerzerrungEffektstaerke.Text = "50"
        Me.lblVerzerrungEffektstaerke.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkVerzerrungsstaerke
        '
        Me.trkVerzerrungsstaerke.AutoSize = False
        Me.trkVerzerrungsstaerke.LargeChange = 10
        Me.trkVerzerrungsstaerke.Location = New System.Drawing.Point(271, 103)
        Me.trkVerzerrungsstaerke.Maximum = 100
        Me.trkVerzerrungsstaerke.Minimum = 10
        Me.trkVerzerrungsstaerke.Name = "trkVerzerrungsstaerke"
        Me.trkVerzerrungsstaerke.Size = New System.Drawing.Size(465, 57)
        Me.trkVerzerrungsstaerke.TabIndex = 88
        Me.trkVerzerrungsstaerke.TickFrequency = 10
        Me.trkVerzerrungsstaerke.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkVerzerrungsstaerke.Value = 50
        '
        'lblNtrkVerzerrungEffektstärke
        '
        Me.lblNtrkVerzerrungEffektstärke.AutoSize = True
        Me.lblNtrkVerzerrungEffektstärke.Location = New System.Drawing.Point(3, 103)
        Me.lblNtrkVerzerrungEffektstärke.Name = "lblNtrkVerzerrungEffektstärke"
        Me.lblNtrkVerzerrungEffektstärke.Size = New System.Drawing.Size(171, 41)
        Me.lblNtrkVerzerrungEffektstärke.TabIndex = 87
        Me.lblNtrkVerzerrungEffektstärke.Tag = "langKey=lblNtrkBrandkantenbreite"
        Me.lblNtrkVerzerrungEffektstärke.Text = "Effektstärke"
        '
        'lblVerzerrungsbreite
        '
        Me.lblVerzerrungsbreite.AutoSize = True
        Me.lblVerzerrungsbreite.Location = New System.Drawing.Point(755, 22)
        Me.lblVerzerrungsbreite.MaximumSize = New System.Drawing.Size(70, 0)
        Me.lblVerzerrungsbreite.MinimumSize = New System.Drawing.Size(70, 0)
        Me.lblVerzerrungsbreite.Name = "lblVerzerrungsbreite"
        Me.lblVerzerrungsbreite.Size = New System.Drawing.Size(70, 41)
        Me.lblVerzerrungsbreite.TabIndex = 86
        Me.lblVerzerrungsbreite.Tag = "langKey=lblWertBrandkante"
        Me.lblVerzerrungsbreite.Text = "18"
        Me.lblVerzerrungsbreite.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkVerzerrungsbreite
        '
        Me.trkVerzerrungsbreite.AutoSize = False
        Me.trkVerzerrungsbreite.LargeChange = 10
        Me.trkVerzerrungsbreite.Location = New System.Drawing.Point(271, 22)
        Me.trkVerzerrungsbreite.Maximum = 100
        Me.trkVerzerrungsbreite.Minimum = 10
        Me.trkVerzerrungsbreite.Name = "trkVerzerrungsbreite"
        Me.trkVerzerrungsbreite.Size = New System.Drawing.Size(465, 57)
        Me.trkVerzerrungsbreite.TabIndex = 85
        Me.trkVerzerrungsbreite.TickFrequency = 10
        Me.trkVerzerrungsbreite.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkVerzerrungsbreite.Value = 18
        '
        'lblNtrkVerzerrungsbreite
        '
        Me.lblNtrkVerzerrungsbreite.AutoSize = True
        Me.lblNtrkVerzerrungsbreite.Location = New System.Drawing.Point(3, 22)
        Me.lblNtrkVerzerrungsbreite.Name = "lblNtrkVerzerrungsbreite"
        Me.lblNtrkVerzerrungsbreite.Size = New System.Drawing.Size(253, 41)
        Me.lblNtrkVerzerrungsbreite.TabIndex = 84
        Me.lblNtrkVerzerrungsbreite.Tag = "langKey=lblNtrkBrandkantenbreite"
        Me.lblNtrkVerzerrungsbreite.Text = "Verzerrungsbreite"
        '
        'tpTextur
        '
        Me.tpTextur.Location = New System.Drawing.Point(10, 59)
        Me.tpTextur.Name = "tpTextur"
        Me.tpTextur.Padding = New System.Windows.Forms.Padding(3)
        Me.tpTextur.Size = New System.Drawing.Size(836, 280)
        Me.tpTextur.TabIndex = 3
        Me.tpTextur.Text = "Textur"
        Me.tpTextur.UseVisualStyleBackColor = True
        '
        'tpKonfig
        '
        Me.tpKonfig.Controls.Add(Me.lblFBMStaerke)
        Me.tpKonfig.Controls.Add(Me.lblFBMPersistenz)
        Me.tpKonfig.Controls.Add(Me.lblFBMOktaven)
        Me.tpKonfig.Controls.Add(Me.lblFBMGrundfrequenz)
        Me.tpKonfig.Controls.Add(Me.trkFBMStaerke)
        Me.tpKonfig.Controls.Add(Me.trkFBMPersistenz)
        Me.tpKonfig.Controls.Add(Me.trkFBMOktaven)
        Me.tpKonfig.Controls.Add(Me.lblNtrkFBMStaerke)
        Me.tpKonfig.Controls.Add(Me.lblNtrkFBMPersistenz)
        Me.tpKonfig.Controls.Add(Me.lblNtrkGrundfrequenz)
        Me.tpKonfig.Controls.Add(Me.trkFBMGrundfrequenz)
        Me.tpKonfig.Controls.Add(Me.lblNtrkFBMOktaven)
        Me.tpKonfig.Location = New System.Drawing.Point(10, 59)
        Me.tpKonfig.Name = "tpKonfig"
        Me.tpKonfig.Padding = New System.Windows.Forms.Padding(3)
        Me.tpKonfig.Size = New System.Drawing.Size(836, 280)
        Me.tpKonfig.TabIndex = 4
        Me.tpKonfig.Text = "Konfig"
        Me.tpKonfig.UseVisualStyleBackColor = True
        '
        'lblFBMStaerke
        '
        Me.lblFBMStaerke.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFBMStaerke.AutoSize = True
        Me.lblFBMStaerke.Enabled = False
        Me.lblFBMStaerke.Location = New System.Drawing.Point(759, 180)
        Me.lblFBMStaerke.Name = "lblFBMStaerke"
        Me.lblFBMStaerke.Size = New System.Drawing.Size(57, 41)
        Me.lblFBMStaerke.TabIndex = 90
        Me.lblFBMStaerke.Text = "0.4"
        Me.lblFBMStaerke.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.lblFBMStaerke.Visible = False
        '
        'lblFBMPersistenz
        '
        Me.lblFBMPersistenz.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFBMPersistenz.AutoSize = True
        Me.lblFBMPersistenz.Enabled = False
        Me.lblFBMPersistenz.Location = New System.Drawing.Point(759, 123)
        Me.lblFBMPersistenz.Name = "lblFBMPersistenz"
        Me.lblFBMPersistenz.Size = New System.Drawing.Size(57, 41)
        Me.lblFBMPersistenz.TabIndex = 89
        Me.lblFBMPersistenz.Text = "0.6"
        Me.lblFBMPersistenz.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.lblFBMPersistenz.Visible = False
        '
        'lblFBMOktaven
        '
        Me.lblFBMOktaven.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFBMOktaven.AutoSize = True
        Me.lblFBMOktaven.Enabled = False
        Me.lblFBMOktaven.Location = New System.Drawing.Point(785, 72)
        Me.lblFBMOktaven.Name = "lblFBMOktaven"
        Me.lblFBMOktaven.Size = New System.Drawing.Size(34, 41)
        Me.lblFBMOktaven.TabIndex = 88
        Me.lblFBMOktaven.Text = "5"
        Me.lblFBMOktaven.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.lblFBMOktaven.Visible = False
        '
        'lblFBMGrundfrequenz
        '
        Me.lblFBMGrundfrequenz.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFBMGrundfrequenz.AutoSize = True
        Me.lblFBMGrundfrequenz.Enabled = False
        Me.lblFBMGrundfrequenz.Location = New System.Drawing.Point(764, 16)
        Me.lblFBMGrundfrequenz.Name = "lblFBMGrundfrequenz"
        Me.lblFBMGrundfrequenz.Size = New System.Drawing.Size(57, 41)
        Me.lblFBMGrundfrequenz.TabIndex = 87
        Me.lblFBMGrundfrequenz.Text = "8.0"
        Me.lblFBMGrundfrequenz.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.lblFBMGrundfrequenz.Visible = False
        '
        'trkFBMStaerke
        '
        Me.trkFBMStaerke.AutoSize = False
        Me.trkFBMStaerke.Enabled = False
        Me.trkFBMStaerke.Location = New System.Drawing.Point(227, 180)
        Me.trkFBMStaerke.Maximum = 100
        Me.trkFBMStaerke.Name = "trkFBMStaerke"
        Me.trkFBMStaerke.Size = New System.Drawing.Size(523, 75)
        Me.trkFBMStaerke.TabIndex = 86
        Me.trkFBMStaerke.Value = 40
        Me.trkFBMStaerke.Visible = False
        '
        'trkFBMPersistenz
        '
        Me.trkFBMPersistenz.AutoSize = False
        Me.trkFBMPersistenz.Enabled = False
        Me.trkFBMPersistenz.Location = New System.Drawing.Point(229, 123)
        Me.trkFBMPersistenz.Maximum = 90
        Me.trkFBMPersistenz.Minimum = 10
        Me.trkFBMPersistenz.Name = "trkFBMPersistenz"
        Me.trkFBMPersistenz.Size = New System.Drawing.Size(521, 65)
        Me.trkFBMPersistenz.TabIndex = 85
        Me.trkFBMPersistenz.Value = 60
        Me.trkFBMPersistenz.Visible = False
        '
        'trkFBMOktaven
        '
        Me.trkFBMOktaven.AutoSize = False
        Me.trkFBMOktaven.Enabled = False
        Me.trkFBMOktaven.Location = New System.Drawing.Point(230, 72)
        Me.trkFBMOktaven.Maximum = 8
        Me.trkFBMOktaven.Minimum = 1
        Me.trkFBMOktaven.Name = "trkFBMOktaven"
        Me.trkFBMOktaven.Size = New System.Drawing.Size(523, 60)
        Me.trkFBMOktaven.TabIndex = 84
        Me.trkFBMOktaven.Value = 5
        Me.trkFBMOktaven.Visible = False
        '
        'lblNtrkFBMStaerke
        '
        Me.lblNtrkFBMStaerke.AutoSize = True
        Me.lblNtrkFBMStaerke.Enabled = False
        Me.lblNtrkFBMStaerke.Location = New System.Drawing.Point(-2, 180)
        Me.lblNtrkFBMStaerke.Name = "lblNtrkFBMStaerke"
        Me.lblNtrkFBMStaerke.Size = New System.Drawing.Size(99, 41)
        Me.lblNtrkFBMStaerke.TabIndex = 83
        Me.lblNtrkFBMStaerke.Text = "Stärke"
        Me.lblNtrkFBMStaerke.Visible = False
        '
        'lblNtrkFBMPersistenz
        '
        Me.lblNtrkFBMPersistenz.AutoSize = True
        Me.lblNtrkFBMPersistenz.Enabled = False
        Me.lblNtrkFBMPersistenz.Location = New System.Drawing.Point(-2, 123)
        Me.lblNtrkFBMPersistenz.Name = "lblNtrkFBMPersistenz"
        Me.lblNtrkFBMPersistenz.Size = New System.Drawing.Size(150, 41)
        Me.lblNtrkFBMPersistenz.TabIndex = 82
        Me.lblNtrkFBMPersistenz.Text = "Persistenz"
        Me.lblNtrkFBMPersistenz.Visible = False
        '
        'lblNtrkGrundfrequenz
        '
        Me.lblNtrkGrundfrequenz.AutoSize = True
        Me.lblNtrkGrundfrequenz.Enabled = False
        Me.lblNtrkGrundfrequenz.Location = New System.Drawing.Point(3, 16)
        Me.lblNtrkGrundfrequenz.Name = "lblNtrkGrundfrequenz"
        Me.lblNtrkGrundfrequenz.Size = New System.Drawing.Size(218, 41)
        Me.lblNtrkGrundfrequenz.TabIndex = 77
        Me.lblNtrkGrundfrequenz.Text = "Grundfrequenz"
        Me.lblNtrkGrundfrequenz.Visible = False
        '
        'trkFBMGrundfrequenz
        '
        Me.trkFBMGrundfrequenz.AutoSize = False
        Me.trkFBMGrundfrequenz.Enabled = False
        Me.trkFBMGrundfrequenz.Location = New System.Drawing.Point(244, 16)
        Me.trkFBMGrundfrequenz.Maximum = 40
        Me.trkFBMGrundfrequenz.Minimum = 2
        Me.trkFBMGrundfrequenz.Name = "trkFBMGrundfrequenz"
        Me.trkFBMGrundfrequenz.Size = New System.Drawing.Size(511, 60)
        Me.trkFBMGrundfrequenz.TabIndex = 81
        Me.trkFBMGrundfrequenz.Value = 16
        Me.trkFBMGrundfrequenz.Visible = False
        '
        'lblNtrkFBMOktaven
        '
        Me.lblNtrkFBMOktaven.AutoSize = True
        Me.lblNtrkFBMOktaven.Enabled = False
        Me.lblNtrkFBMOktaven.Location = New System.Drawing.Point(1, 72)
        Me.lblNtrkFBMOktaven.Name = "lblNtrkFBMOktaven"
        Me.lblNtrkFBMOktaven.Size = New System.Drawing.Size(128, 41)
        Me.lblNtrkFBMOktaven.TabIndex = 76
        Me.lblNtrkFBMOktaven.Text = "Oktaven"
        Me.lblNtrkFBMOktaven.Visible = False
        '
        'grbZuendmodus
        '
        Me.grbZuendmodus.Controls.Add(Me.rbZuendmodusZufall)
        Me.grbZuendmodus.Controls.Add(Me.rbZuendModusBrandherde)
        Me.grbZuendmodus.Controls.Add(Me.rbZuendmodusBrandRand)
        Me.grbZuendmodus.Location = New System.Drawing.Point(25, 291)
        Me.grbZuendmodus.Name = "grbZuendmodus"
        Me.grbZuendmodus.Size = New System.Drawing.Size(862, 112)
        Me.grbZuendmodus.TabIndex = 72
        Me.grbZuendmodus.TabStop = False
        Me.grbZuendmodus.Text = "Zündmodus"
        '
        'rbZuendmodusZufall
        '
        Me.rbZuendmodusZufall.AutoSize = True
        Me.rbZuendmodusZufall.Location = New System.Drawing.Point(700, 46)
        Me.rbZuendmodusZufall.Name = "rbZuendmodusZufall"
        Me.rbZuendmodusZufall.Size = New System.Drawing.Size(146, 45)
        Me.rbZuendmodusZufall.TabIndex = 2
        Me.rbZuendmodusZufall.TabStop = True
        Me.rbZuendmodusZufall.Text = "Zufällig"
        Me.rbZuendmodusZufall.UseVisualStyleBackColor = True
        '
        'rbZuendModusBrandherde
        '
        Me.rbZuendModusBrandherde.AutoSize = True
        Me.rbZuendModusBrandherde.Location = New System.Drawing.Point(6, 46)
        Me.rbZuendModusBrandherde.Name = "rbZuendModusBrandherde"
        Me.rbZuendModusBrandherde.Size = New System.Drawing.Size(203, 45)
        Me.rbZuendModusBrandherde.TabIndex = 1
        Me.rbZuendModusBrandherde.TabStop = True
        Me.rbZuendModusBrandherde.Text = "Brandherde"
        Me.rbZuendModusBrandherde.UseVisualStyleBackColor = True
        '
        'rbZuendmodusBrandRand
        '
        Me.rbZuendmodusBrandRand.AutoSize = True
        Me.rbZuendmodusBrandRand.Location = New System.Drawing.Point(321, 46)
        Me.rbZuendmodusBrandRand.Name = "rbZuendmodusBrandRand"
        Me.rbZuendmodusBrandRand.Size = New System.Drawing.Size(268, 45)
        Me.rbZuendmodusBrandRand.TabIndex = 0
        Me.rbZuendmodusBrandRand.TabStop = True
        Me.rbZuendmodusBrandRand.Text = "Brand vom Rand"
        Me.rbZuendmodusBrandRand.UseVisualStyleBackColor = True
        '
        'grbEffekte
        '
        Me.grbEffekte.Controls.Add(Me.chkTextur)
        Me.grbEffekte.Controls.Add(Me.chkVerzerrung)
        Me.grbEffekte.Controls.Add(Me.chkGradient)
        Me.grbEffekte.Controls.Add(Me.chkPartikel)
        Me.grbEffekte.Location = New System.Drawing.Point(9, 388)
        Me.grbEffekte.Name = "grbEffekte"
        Me.grbEffekte.Size = New System.Drawing.Size(872, 100)
        Me.grbEffekte.TabIndex = 73
        Me.grbEffekte.TabStop = False
        Me.grbEffekte.Text = "Effekte"
        '
        'chkTextur
        '
        Me.chkTextur.AutoSize = True
        Me.chkTextur.Location = New System.Drawing.Point(726, 40)
        Me.chkTextur.Name = "chkTextur"
        Me.chkTextur.Size = New System.Drawing.Size(130, 45)
        Me.chkTextur.TabIndex = 75
        Me.chkTextur.Text = "Textur"
        Me.chkTextur.UseVisualStyleBackColor = True
        '
        'chkVerzerrung
        '
        Me.chkVerzerrung.AutoSize = True
        Me.chkVerzerrung.Location = New System.Drawing.Point(468, 40)
        Me.chkVerzerrung.Name = "chkVerzerrung"
        Me.chkVerzerrung.Size = New System.Drawing.Size(195, 45)
        Me.chkVerzerrung.TabIndex = 74
        Me.chkVerzerrung.Text = "Verzerrung"
        Me.chkVerzerrung.UseVisualStyleBackColor = True
        '
        'chkGradient
        '
        Me.chkGradient.AutoSize = True
        Me.chkGradient.Location = New System.Drawing.Point(243, 40)
        Me.chkGradient.Name = "chkGradient"
        Me.chkGradient.Size = New System.Drawing.Size(164, 45)
        Me.chkGradient.TabIndex = 73
        Me.chkGradient.Text = "Gradient"
        Me.chkGradient.UseVisualStyleBackColor = True
        '
        'chkPartikel
        '
        Me.chkPartikel.AutoSize = True
        Me.chkPartikel.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.chkPartikel.Location = New System.Drawing.Point(20, 40)
        Me.chkPartikel.Name = "chkPartikel"
        Me.chkPartikel.Size = New System.Drawing.Size(146, 45)
        Me.chkPartikel.TabIndex = 72
        Me.chkPartikel.Tag = "langKey=chkPartikel"
        Me.chkPartikel.Text = "Partikel"
        Me.chkPartikel.UseVisualStyleBackColor = True
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.grbEffekte)
        Me.Controls.Add(Me.grbZuendmodus)
        Me.Controls.Add(Me.tcBP)
        Me.Controls.Add(Me.chkZufallsdauer)
        Me.Controls.Add(Me.lblKeineRichtungInfo)
        Me.Controls.Add(Me.lblNModus)
        Me.Controls.Add(Me.clbModus)
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
        Me.tcBP.ResumeLayout(False)
        Me.tpPartikel.ResumeLayout(False)
        Me.tpPartikel.PerformLayout()
        CType(Me.trkPartikelLebensdauer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpSchwerkraft.ResumeLayout(False)
        Me.grpSchwerkraft.PerformLayout()
        Me.tpGradient.ResumeLayout(False)
        Me.tpGradient.PerformLayout()
        CType(Me.trkBrandkantenbreite, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpVerzerrung.ResumeLayout(False)
        Me.tpVerzerrung.PerformLayout()
        CType(Me.trkScherbengroesse, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkVerzerrungsstaerke, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkVerzerrungsbreite, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpKonfig.ResumeLayout(False)
        Me.tpKonfig.PerformLayout()
        CType(Me.trkFBMStaerke, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkFBMPersistenz, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkFBMOktaven, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkFBMGrundfrequenz, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grbZuendmodus.ResumeLayout(False)
        Me.grbZuendmodus.PerformLayout()
        Me.grbEffekte.ResumeLayout(False)
        Me.grbEffekte.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnDefaults As Windows.Forms.Button
    Friend WithEvents lblTransitionsname As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionname As Windows.Forms.Label
    Friend WithEvents lblGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents trkDauer As Windows.Forms.TrackBar
    Friend WithEvents lblNtrbDauer As Windows.Forms.Label
    Friend WithEvents lblNModus As Windows.Forms.Label
    Friend WithEvents clbModus As Windows.Forms.CheckedListBox
    Friend WithEvents lblKeineRichtungInfo As Windows.Forms.Label
    Friend WithEvents chkZufallsdauer As Windows.Forms.CheckBox
    Friend WithEvents tcBP As Windows.Forms.TabControl
    Friend WithEvents tpPartikel As Windows.Forms.TabPage
    Friend WithEvents tpGradient As Windows.Forms.TabPage
    Friend WithEvents tpVerzerrung As Windows.Forms.TabPage
    Friend WithEvents tpTextur As Windows.Forms.TabPage
    Friend WithEvents tpKonfig As Windows.Forms.TabPage
    Friend WithEvents lblFBMStaerke As Windows.Forms.Label
    Friend WithEvents lblFBMPersistenz As Windows.Forms.Label
    Friend WithEvents lblFBMOktaven As Windows.Forms.Label
    Friend WithEvents lblFBMGrundfrequenz As Windows.Forms.Label
    Friend WithEvents trkFBMStaerke As Windows.Forms.TrackBar
    Friend WithEvents trkFBMPersistenz As Windows.Forms.TrackBar
    Friend WithEvents trkFBMOktaven As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkFBMStaerke As Windows.Forms.Label
    Friend WithEvents lblNtrkFBMPersistenz As Windows.Forms.Label
    Friend WithEvents lblNtrkGrundfrequenz As Windows.Forms.Label
    Friend WithEvents trkFBMGrundfrequenz As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkFBMOktaven As Windows.Forms.Label
    Friend WithEvents grpSchwerkraft As Windows.Forms.GroupBox
    Friend WithEvents rbSchwerkraftZufällig As Windows.Forms.RadioButton
    Friend WithEvents rbSchwerkraftAn As Windows.Forms.RadioButton
    Friend WithEvents rbSchwerkraftAus As Windows.Forms.RadioButton
    Friend WithEvents lblWertBrandkante As Windows.Forms.Label
    Friend WithEvents trkBrandkantenbreite As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkBrandkantenbreite As Windows.Forms.Label
    Friend WithEvents lblPartikelLebensdauer As Windows.Forms.Label
    Friend WithEvents trkPartikelLebensdauer As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkPartikelLebensdauer As Windows.Forms.Label
    Friend WithEvents lblScherbengroesse As Windows.Forms.Label
    Friend WithEvents trkScherbengroesse As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkScherbengroesse As Windows.Forms.Label
    Friend WithEvents lblVerzerrungEffektstaerke As Windows.Forms.Label
    Friend WithEvents trkVerzerrungsstaerke As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkVerzerrungEffektstärke As Windows.Forms.Label
    Friend WithEvents lblVerzerrungsbreite As Windows.Forms.Label
    Friend WithEvents trkVerzerrungsbreite As Windows.Forms.TrackBar
    Friend WithEvents lblNtrkVerzerrungsbreite As Windows.Forms.Label
    Friend WithEvents grbZuendmodus As Windows.Forms.GroupBox
    Friend WithEvents rbZuendmodusZufall As Windows.Forms.RadioButton
    Friend WithEvents rbZuendModusBrandherde As Windows.Forms.RadioButton
    Friend WithEvents rbZuendmodusBrandRand As Windows.Forms.RadioButton
    Friend WithEvents grbEffekte As Windows.Forms.GroupBox
    Friend WithEvents chkTextur As Windows.Forms.CheckBox
    Friend WithEvents chkVerzerrung As Windows.Forms.CheckBox
    Friend WithEvents chkGradient As Windows.Forms.CheckBox
    Friend WithEvents chkPartikel As Windows.Forms.CheckBox
End Class
