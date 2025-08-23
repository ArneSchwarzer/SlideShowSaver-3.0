<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmOptionsMain
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOptionsMain))
        Me.lblNTitel = New System.Windows.Forms.Label()
Me.lblNTitel.Tag = "langKey=lblNTitel"
        Me.tabOptions = New System.Windows.Forms.TabControl()
Me.tabOptions.Tag = "langKey=tabOptions"
        Me.tpAllgemein = New System.Windows.Forms.TabPage()
Me.tpAllgemein.Tag = "langKey=tpAllgemein"
        Me.picHintergrundfarbe = New System.Windows.Forms.PictureBox()
Me.picHintergrundfarbe.Tag = "langKey=picHintergrundfarbe"
        Me.lblNHintergrundfarbe = New System.Windows.Forms.Label()
Me.lblNHintergrundfarbe.Tag = "langKey=lblNHintergrundfarbe"
        Me.lblKeineTransitionsModule = New System.Windows.Forms.Label()
Me.lblKeineTransitionsModule.Tag = "langKey=lblKeineTransitionsModule"
        Me.lblNcmbAbspielmodusTransitionsModule = New System.Windows.Forms.Label()
Me.lblNcmbAbspielmodusTransitionsModule.Tag = "langKey=lblNcmbAbspielmodusTransitionsModule"
        Me.cmbTransitionsReihenfolge = New System.Windows.Forms.ComboBox()
Me.cmbTransitionsReihenfolge.Tag = "langKey=cmbTransitionsReihenfolge"
        Me.clbTransitionsModule = New System.Windows.Forms.CheckedListBox()
Me.clbTransitionsModule.Tag = "langKey=clbTransitionsModule"
        Me.lblNclbTransitionsModule = New System.Windows.Forms.Label()
Me.lblNclbTransitionsModule.Tag = "langKey=lblNclbTransitionsModule"
        Me.lblKeineModule = New System.Windows.Forms.Label()
Me.lblKeineModule.Tag = "langKey=lblKeineModule"
        Me.chkMultiMonitor = New System.Windows.Forms.CheckBox()
Me.chkMultiMonitor.Tag = "langKey=chkMultiMonitor"
        Me.lblDauerModuswechsel = New System.Windows.Forms.Label()
Me.lblDauerModuswechsel.Tag = "langKey=lblDauerModuswechsel"
        Me.trkDauerModulwechsel = New System.Windows.Forms.TrackBar()
        Me.lblNtrkDauerModulwechsel = New System.Windows.Forms.Label()
Me.lblNtrkDauerModulwechsel.Tag = "langKey=lblNtrkDauerModulwechsel"
        Me.lblNcmbModulWechsel = New System.Windows.Forms.Label()
Me.lblNcmbModulWechsel.Tag = "langKey=lblNcmbModulWechsel"
        Me.cmbModulwechsel = New System.Windows.Forms.ComboBox()
Me.cmbModulwechsel.Tag = "langKey=cmbModulwechsel"
        Me.clbModule = New System.Windows.Forms.CheckedListBox()
Me.clbModule.Tag = "langKey=clbModule"
        Me.lblNcblModule = New System.Windows.Forms.Label()
Me.lblNcblModule.Tag = "langKey=lblNcblModule"
        Me.tpModul = New System.Windows.Forms.TabPage()
Me.tpModul.Tag = "langKey=tpModul"
        Me.tpBildauswahl = New System.Windows.Forms.TabPage()
Me.tpBildauswahl.Tag = "langKey=tpBildauswahl"
        Me.tpTransitions = New System.Windows.Forms.TabPage()
Me.tpTransitions.Tag = "langKey=tpTransitions"
        Me.tpShader = New System.Windows.Forms.TabPage()
Me.tpShader.Tag = "langKey=tpShader"
        Me.btnOK = New System.Windows.Forms.Button()
Me.btnOK.Tag = "langKey=btnOK"
        Me.pnlLanguages = New System.Windows.Forms.Panel()
        Me.btnDefaults = New System.Windows.Forms.Button()
Me.btnDefaults.Tag = "langKey=btnDefaults"
        Me.tabOptions.SuspendLayout()
        Me.tpAllgemein.SuspendLayout()
        CType(Me.picHintergrundfarbe, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkDauerModulwechsel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblNTitel
        '
        Me.lblNTitel.AutoSize = True
        Me.lblNTitel.Font = New System.Drawing.Font("Segoe UI", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNTitel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.lblNTitel.Location = New System.Drawing.Point(15, 13)
        Me.lblNTitel.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblNTitel.Name = "lblNTitel"
        Me.lblNTitel.Size = New System.Drawing.Size(468, 65)
        Me.lblNTitel.TabIndex = 0
        Me.lblNTitel.Text = "SlideShowSaver 3.0"
        '
        'tabOptions
        '
        Me.tabOptions.Controls.Add(Me.tpAllgemein)
        Me.tabOptions.Controls.Add(Me.tpModul)
        Me.tabOptions.Controls.Add(Me.tpBildauswahl)
        Me.tabOptions.Controls.Add(Me.tpTransitions)
        Me.tabOptions.Controls.Add(Me.tpShader)
        Me.tabOptions.Location = New System.Drawing.Point(26, 82)
        Me.tabOptions.Margin = New System.Windows.Forms.Padding(4)
        Me.tabOptions.Name = "tabOptions"
        Me.tabOptions.SelectedIndex = 0
        Me.tabOptions.Size = New System.Drawing.Size(922, 1108)
        Me.tabOptions.TabIndex = 1
        '
        'tpAllgemein
        '
        Me.tpAllgemein.Controls.Add(Me.btnDefaults)
        Me.tpAllgemein.Controls.Add(Me.picHintergrundfarbe)
        Me.tpAllgemein.Controls.Add(Me.lblNHintergrundfarbe)
        Me.tpAllgemein.Controls.Add(Me.lblKeineTransitionsModule)
        Me.tpAllgemein.Controls.Add(Me.lblNcmbAbspielmodusTransitionsModule)
        Me.tpAllgemein.Controls.Add(Me.cmbTransitionsReihenfolge)
        Me.tpAllgemein.Controls.Add(Me.clbTransitionsModule)
        Me.tpAllgemein.Controls.Add(Me.lblNclbTransitionsModule)
        Me.tpAllgemein.Controls.Add(Me.lblKeineModule)
        Me.tpAllgemein.Controls.Add(Me.chkMultiMonitor)
        Me.tpAllgemein.Controls.Add(Me.lblDauerModuswechsel)
        Me.tpAllgemein.Controls.Add(Me.trkDauerModulwechsel)
        Me.tpAllgemein.Controls.Add(Me.lblNtrkDauerModulwechsel)
        Me.tpAllgemein.Controls.Add(Me.lblNcmbModulWechsel)
        Me.tpAllgemein.Controls.Add(Me.cmbModulwechsel)
        Me.tpAllgemein.Controls.Add(Me.clbModule)
        Me.tpAllgemein.Controls.Add(Me.lblNcblModule)
        Me.tpAllgemein.Location = New System.Drawing.Point(10, 59)
        Me.tpAllgemein.Margin = New System.Windows.Forms.Padding(4)
        Me.tpAllgemein.Name = "tpAllgemein"
        Me.tpAllgemein.Padding = New System.Windows.Forms.Padding(4)
        Me.tpAllgemein.Size = New System.Drawing.Size(902, 1039)
        Me.tpAllgemein.TabIndex = 0
        Me.tpAllgemein.Text = "Allgemein"
        Me.tpAllgemein.UseVisualStyleBackColor = True
        '
        'picHintergrundfarbe
        '
        Me.picHintergrundfarbe.BackColor = System.Drawing.Color.Black
        Me.picHintergrundfarbe.Location = New System.Drawing.Point(489, 924)
        Me.picHintergrundfarbe.Name = "picHintergrundfarbe"
        Me.picHintergrundfarbe.Size = New System.Drawing.Size(387, 50)
        Me.picHintergrundfarbe.TabIndex = 15
        Me.picHintergrundfarbe.TabStop = False
        '
        'lblNHintergrundfarbe
        '
        Me.lblNHintergrundfarbe.AutoSize = True
        Me.lblNHintergrundfarbe.Location = New System.Drawing.Point(27, 933)
        Me.lblNHintergrundfarbe.Name = "lblNHintergrundfarbe"
        Me.lblNHintergrundfarbe.Size = New System.Drawing.Size(247, 41)
        Me.lblNHintergrundfarbe.TabIndex = 14
        Me.lblNHintergrundfarbe.Text = "Hintergrundfarbe"
        '
        'lblKeineTransitionsModule
        '
        Me.lblKeineTransitionsModule.AutoSize = True
        Me.lblKeineTransitionsModule.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblKeineTransitionsModule.ForeColor = System.Drawing.Color.Red
        Me.lblKeineTransitionsModule.Location = New System.Drawing.Point(438, 567)
        Me.lblKeineTransitionsModule.Name = "lblKeineTransitionsModule"
        Me.lblKeineTransitionsModule.Size = New System.Drawing.Size(382, 41)
        Me.lblKeineTransitionsModule.TabIndex = 13
        Me.lblKeineTransitionsModule.Text = "Keine Übergänge geladen"
        Me.lblKeineTransitionsModule.Visible = False
        '
        'lblNcmbAbspielmodusTransitionsModule
        '
        Me.lblNcmbAbspielmodusTransitionsModule.AutoSize = True
        Me.lblNcmbAbspielmodusTransitionsModule.Location = New System.Drawing.Point(21, 852)
        Me.lblNcmbAbspielmodusTransitionsModule.Name = "lblNcmbAbspielmodusTransitionsModule"
        Me.lblNcmbAbspielmodusTransitionsModule.Size = New System.Drawing.Size(365, 41)
        Me.lblNcmbAbspielmodusTransitionsModule.TabIndex = 12
        Me.lblNcmbAbspielmodusTransitionsModule.Text = "Abspielmodus Übergänge"
        '
        'cmbTransitionsReihenfolge
        '
        Me.cmbTransitionsReihenfolge.FormattingEnabled = True
        Me.cmbTransitionsReihenfolge.Items.AddRange(New Object() {"Zufällig bei Start", "In Reihenfolge bei Start"})
        Me.cmbTransitionsReihenfolge.Location = New System.Drawing.Point(489, 852)
        Me.cmbTransitionsReihenfolge.Name = "cmbTransitionsReihenfolge"
        Me.cmbTransitionsReihenfolge.Size = New System.Drawing.Size(388, 49)
        Me.cmbTransitionsReihenfolge.TabIndex = 11
        '
        'clbTransitionsModule
        '
        Me.clbTransitionsModule.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.clbTransitionsModule.FormattingEnabled = True
        Me.clbTransitionsModule.Location = New System.Drawing.Point(27, 614)
        Me.clbTransitionsModule.Margin = New System.Windows.Forms.Padding(4)
        Me.clbTransitionsModule.Name = "clbTransitionsModule"
        Me.clbTransitionsModule.Size = New System.Drawing.Size(850, 224)
        Me.clbTransitionsModule.TabIndex = 10
        '
        'lblNclbTransitionsModule
        '
        Me.lblNclbTransitionsModule.AutoSize = True
        Me.lblNclbTransitionsModule.Location = New System.Drawing.Point(21, 567)
        Me.lblNclbTransitionsModule.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblNclbTransitionsModule.Name = "lblNclbTransitionsModule"
        Me.lblNclbTransitionsModule.Size = New System.Drawing.Size(410, 41)
        Me.lblNclbTransitionsModule.TabIndex = 9
        Me.lblNclbTransitionsModule.Text = "Übergangseffekte für Module"
        '
        'lblKeineModule
        '
        Me.lblKeineModule.AutoSize = True
        Me.lblKeineModule.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblKeineModule.ForeColor = System.Drawing.Color.Red
        Me.lblKeineModule.Location = New System.Drawing.Point(148, 65)
        Me.lblKeineModule.Name = "lblKeineModule"
        Me.lblKeineModule.Size = New System.Drawing.Size(651, 41)
        Me.lblKeineModule.TabIndex = 8
        Me.lblKeineModule.Text = "Keine Module geladen, spiele Bouncing Logo"
        Me.lblKeineModule.Visible = False
        '
        'chkMultiMonitor
        '
        Me.chkMultiMonitor.AutoSize = True
        Me.chkMultiMonitor.Location = New System.Drawing.Point(28, 987)
        Me.chkMultiMonitor.Name = "chkMultiMonitor"
        Me.chkMultiMonitor.Size = New System.Drawing.Size(338, 45)
        Me.chkMultiMonitor.TabIndex = 7
        Me.chkMultiMonitor.Text = "Multi-Monitor Modus"
        Me.chkMultiMonitor.UseVisualStyleBackColor = True
        '
        'lblDauerModuswechsel
        '
        Me.lblDauerModuswechsel.AutoSize = True
        Me.lblDauerModuswechsel.Location = New System.Drawing.Point(794, 505)
        Me.lblDauerModuswechsel.Name = "lblDauerModuswechsel"
        Me.lblDauerModuswechsel.Size = New System.Drawing.Size(84, 41)
        Me.lblDauerModuswechsel.TabIndex = 6
        Me.lblDauerModuswechsel.Text = "15 m"
        Me.lblDauerModuswechsel.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkDauerModulwechsel
        '
        Me.trkDauerModulwechsel.AutoSize = False
        Me.trkDauerModulwechsel.Location = New System.Drawing.Point(28, 505)
        Me.trkDauerModulwechsel.Maximum = 60
        Me.trkDauerModulwechsel.Minimum = 1
        Me.trkDauerModulwechsel.Name = "trkDauerModulwechsel"
        Me.trkDauerModulwechsel.Size = New System.Drawing.Size(744, 52)
        Me.trkDauerModulwechsel.TabIndex = 5
        Me.trkDauerModulwechsel.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkDauerModulwechsel.Value = 15
        '
        'lblNtrkDauerModulwechsel
        '
        Me.lblNtrkDauerModulwechsel.AutoSize = True
        Me.lblNtrkDauerModulwechsel.Location = New System.Drawing.Point(21, 461)
        Me.lblNtrkDauerModulwechsel.Name = "lblNtrkDauerModulwechsel"
        Me.lblNtrkDauerModulwechsel.Size = New System.Drawing.Size(343, 41)
        Me.lblNtrkDauerModulwechsel.TabIndex = 4
        Me.lblNtrkDauerModulwechsel.Text = "Dauer bis Modulwechsel"
        '
        'lblNcmbModulWechsel
        '
        Me.lblNcmbModulWechsel.AutoSize = True
        Me.lblNcmbModulWechsel.Location = New System.Drawing.Point(21, 387)
        Me.lblNcmbModulWechsel.Name = "lblNcmbModulWechsel"
        Me.lblNcmbModulWechsel.Size = New System.Drawing.Size(319, 41)
        Me.lblNcmbModulWechsel.TabIndex = 3
        Me.lblNcmbModulWechsel.Text = "Abspielmodus Module"
        '
        'cmbModulwechsel
        '
        Me.cmbModulwechsel.FormattingEnabled = True
        Me.cmbModulwechsel.Items.AddRange(New Object() {"Zufällig bei Start", "Zufällig", "In Reihenfolge bei Start", "In Reihenfolge"})
        Me.cmbModulwechsel.Location = New System.Drawing.Point(489, 387)
        Me.cmbModulwechsel.Name = "cmbModulwechsel"
        Me.cmbModulwechsel.Size = New System.Drawing.Size(388, 49)
        Me.cmbModulwechsel.TabIndex = 2
        '
        'clbModule
        '
        Me.clbModule.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.clbModule.FormattingEnabled = True
        Me.clbModule.Location = New System.Drawing.Point(26, 112)
        Me.clbModule.Margin = New System.Windows.Forms.Padding(4)
        Me.clbModule.Name = "clbModule"
        Me.clbModule.Size = New System.Drawing.Size(850, 268)
        Me.clbModule.TabIndex = 1
        '
        'lblNcblModule
        '
        Me.lblNcblModule.AutoSize = True
        Me.lblNcblModule.Location = New System.Drawing.Point(20, 65)
        Me.lblNcblModule.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblNcblModule.Name = "lblNcblModule"
        Me.lblNcblModule.Size = New System.Drawing.Size(121, 41)
        Me.lblNcblModule.TabIndex = 0
        Me.lblNcblModule.Text = "Module"
        '
        'tpModul
        '
        Me.tpModul.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tpModul.Location = New System.Drawing.Point(10, 59)
        Me.tpModul.Margin = New System.Windows.Forms.Padding(4)
        Me.tpModul.Name = "tpModul"
        Me.tpModul.Padding = New System.Windows.Forms.Padding(4)
        Me.tpModul.Size = New System.Drawing.Size(902, 1039)
        Me.tpModul.TabIndex = 1
        Me.tpModul.Text = "Modul"
        Me.tpModul.UseVisualStyleBackColor = True
        '
        'tpBildauswahl
        '
        Me.tpBildauswahl.Location = New System.Drawing.Point(10, 59)
        Me.tpBildauswahl.Name = "tpBildauswahl"
        Me.tpBildauswahl.Padding = New System.Windows.Forms.Padding(3)
        Me.tpBildauswahl.Size = New System.Drawing.Size(902, 1039)
        Me.tpBildauswahl.TabIndex = 2
        Me.tpBildauswahl.Text = "Bildauswahl"
        Me.tpBildauswahl.UseVisualStyleBackColor = True
        '
        'tpTransitions
        '
        Me.tpTransitions.Location = New System.Drawing.Point(10, 59)
        Me.tpTransitions.Name = "tpTransitions"
        Me.tpTransitions.Padding = New System.Windows.Forms.Padding(3)
        Me.tpTransitions.Size = New System.Drawing.Size(902, 1039)
        Me.tpTransitions.TabIndex = 3
        Me.tpTransitions.Text = "Transitionseffekt"
        Me.tpTransitions.UseVisualStyleBackColor = True
        '
        'tpShader
        '
        Me.tpShader.Location = New System.Drawing.Point(10, 59)
        Me.tpShader.Name = "tpShader"
        Me.tpShader.Padding = New System.Windows.Forms.Padding(3)
        Me.tpShader.Size = New System.Drawing.Size(902, 1039)
        Me.tpShader.TabIndex = 4
        Me.tpShader.Text = "Shader"
        Me.tpShader.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(763, 1300)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(185, 71)
        Me.btnOK.TabIndex = 2
        Me.btnOK.Text = "Fertig"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'pnlLanguages
        '
        Me.pnlLanguages.Location = New System.Drawing.Point(26, 1198)
        Me.pnlLanguages.Name = "pnlLanguages"
        Me.pnlLanguages.Size = New System.Drawing.Size(922, 84)
        Me.pnlLanguages.TabIndex = 4
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(691, 8)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(187, 54)
        Me.btnDefaults.TabIndex = 16
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'frmOptionsMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(979, 1384)
        Me.Controls.Add(Me.pnlLanguages)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.tabOptions)
        Me.Controls.Add(Me.lblNTitel)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmOptionsMain"
        Me.Text = "Einstellungen"
        Me.tabOptions.ResumeLayout(False)
        Me.tpAllgemein.ResumeLayout(False)
        Me.tpAllgemein.PerformLayout()
        CType(Me.picHintergrundfarbe, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkDauerModulwechsel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblNTitel As Label
    Friend WithEvents tabOptions As TabControl
    Friend WithEvents tpAllgemein As TabPage
    Friend WithEvents tpModul As TabPage
    Friend WithEvents clbModule As CheckedListBox
    Friend WithEvents lblNcblModule As Label
    Friend WithEvents tpBildauswahl As TabPage
    Friend WithEvents btnOK As Button
    Friend WithEvents cmbModulwechsel As ComboBox
    Friend WithEvents trkDauerModulwechsel As TrackBar
    Friend WithEvents lblNtrkDauerModulwechsel As Label
    Friend WithEvents lblNcmbModulWechsel As Label
    Friend WithEvents lblDauerModuswechsel As Label
    Friend WithEvents chkMultiMonitor As CheckBox
    Friend WithEvents tpTransitions As TabPage
    Friend WithEvents tpShader As TabPage
    Friend WithEvents lblKeineModule As Label
    Friend WithEvents lblNcmbAbspielmodusTransitionsModule As Label
    Friend WithEvents cmbTransitionsReihenfolge As ComboBox
    Friend WithEvents clbTransitionsModule As CheckedListBox
    Friend WithEvents lblNclbTransitionsModule As Label
    Friend WithEvents lblKeineTransitionsModule As Label
    Friend WithEvents pnlLanguages As Panel
    Friend WithEvents picHintergrundfarbe As PictureBox
    Friend WithEvents lblNHintergrundfarbe As Label
    Friend WithEvents btnDefaults As Button
End Class