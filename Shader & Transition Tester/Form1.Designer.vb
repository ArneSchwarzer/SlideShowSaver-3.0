<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmShaderTranstionTester
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmShaderTranstionTester))
        Me.picShaderOriginal = New System.Windows.Forms.PictureBox()
        Me.picTest1 = New System.Windows.Forms.PictureBox()
        Me.lblTitel = New System.Windows.Forms.Label()
        Me.picTest2 = New System.Windows.Forms.PictureBox()
        Me.lblTestbilder = New System.Windows.Forms.Label()
        Me.cmbPBSizeMod1 = New System.Windows.Forms.ComboBox()
        Me.cmbPBSizeMode2 = New System.Windows.Forms.ComboBox()
        Me.picShaderAngewandt = New System.Windows.Forms.PictureBox()
        Me.picTransition = New System.Windows.Forms.PictureBox()
        Me.ofdBildauswahl = New System.Windows.Forms.OpenFileDialog()
        Me.tabOptionDialoge = New System.Windows.Forms.TabControl()
        Me.tpShader = New System.Windows.Forms.TabPage()
        Me.tpTransition = New System.Windows.Forms.TabPage()
        Me.lblOptionsdialoge = New System.Windows.Forms.Label()
        Me.lblTitelShader = New System.Windows.Forms.Label()
        Me.lvRegValsShader = New System.Windows.Forms.ListView()
        Me.lvRegValsTransition = New System.Windows.Forms.ListView()
        Me.lblShaderName = New System.Windows.Forms.Label()
        Me.lblShaderKurzbeschreibung = New System.Windows.Forms.Label()
        Me.lblShaderVersion = New System.Windows.Forms.Label()
        Me.lblTitelTransition = New System.Windows.Forms.Label()
        Me.cmbShaderAuswahl = New System.Windows.Forms.ComboBox()
        Me.cmbTransitionAuswahl = New System.Windows.Forms.ComboBox()
        Me.lblTransitionVersion = New System.Windows.Forms.Label()
        Me.lblTransitionKurzbeschreibung = New System.Windows.Forms.Label()
        Me.lblTransitionName = New System.Windows.Forms.Label()
        Me.trbLoop = New System.Windows.Forms.TrackBar()
        Me.rbTransitionModeManual = New System.Windows.Forms.RadioButton()
        Me.rbTransitionModeLoop = New System.Windows.Forms.RadioButton()
        Me.btnRunTransition = New System.Windows.Forms.Button()
        Me.chbTransitionDuration = New System.Windows.Forms.CheckBox()
        Me.nudDurationMS = New System.Windows.Forms.NumericUpDown()
        CType(Me.picShaderOriginal, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picTest1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picTest2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picShaderAngewandt, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picTransition, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabOptionDialoge.SuspendLayout()
        CType(Me.trbLoop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudDurationMS, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picShaderOriginal
        '
        Me.picShaderOriginal.Image = CType(resources.GetObject("picShaderOriginal.Image"), System.Drawing.Image)
        Me.picShaderOriginal.Location = New System.Drawing.Point(424, 531)
        Me.picShaderOriginal.Name = "picShaderOriginal"
        Me.picShaderOriginal.Size = New System.Drawing.Size(640, 360)
        Me.picShaderOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picShaderOriginal.TabIndex = 0
        Me.picShaderOriginal.TabStop = False
        '
        'picTest1
        '
        Me.picTest1.Image = CType(resources.GetObject("picTest1.Image"), System.Drawing.Image)
        Me.picTest1.Location = New System.Drawing.Point(53, 176)
        Me.picTest1.Name = "picTest1"
        Me.picTest1.Size = New System.Drawing.Size(320, 180)
        Me.picTest1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picTest1.TabIndex = 1
        Me.picTest1.TabStop = False
        '
        'lblTitel
        '
        Me.lblTitel.AutoSize = True
        Me.lblTitel.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitel.Location = New System.Drawing.Point(41, 25)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(684, 72)
        Me.lblTitel.TabIndex = 2
        Me.lblTitel.Text = "Shader && Transtion Tester"
        '
        'picTest2
        '
        Me.picTest2.Image = CType(resources.GetObject("picTest2.Image"), System.Drawing.Image)
        Me.picTest2.Location = New System.Drawing.Point(426, 176)
        Me.picTest2.Name = "picTest2"
        Me.picTest2.Size = New System.Drawing.Size(320, 180)
        Me.picTest2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picTest2.TabIndex = 3
        Me.picTest2.TabStop = False
        '
        'lblTestbilder
        '
        Me.lblTestbilder.AutoSize = True
        Me.lblTestbilder.Location = New System.Drawing.Point(53, 119)
        Me.lblTestbilder.Name = "lblTestbilder"
        Me.lblTestbilder.Size = New System.Drawing.Size(146, 41)
        Me.lblTestbilder.TabIndex = 4
        Me.lblTestbilder.Text = "Testbilder"
        '
        'cmbPBSizeMod1
        '
        Me.cmbPBSizeMod1.FormattingEnabled = True
        Me.cmbPBSizeMod1.Items.AddRange(New Object() {"Normal", "Center Image", "Stretched", "Zoom", "AutoSize"})
        Me.cmbPBSizeMod1.Location = New System.Drawing.Point(53, 382)
        Me.cmbPBSizeMod1.Name = "cmbPBSizeMod1"
        Me.cmbPBSizeMod1.Size = New System.Drawing.Size(320, 49)
        Me.cmbPBSizeMod1.TabIndex = 5
        '
        'cmbPBSizeMode2
        '
        Me.cmbPBSizeMode2.FormattingEnabled = True
        Me.cmbPBSizeMode2.Items.AddRange(New Object() {"Normal", "Center Image", "Stretched", "Zoom", "AutoSize"})
        Me.cmbPBSizeMode2.Location = New System.Drawing.Point(426, 382)
        Me.cmbPBSizeMode2.Name = "cmbPBSizeMode2"
        Me.cmbPBSizeMode2.Size = New System.Drawing.Size(320, 49)
        Me.cmbPBSizeMode2.TabIndex = 6
        '
        'picShaderAngewandt
        '
        Me.picShaderAngewandt.Image = CType(resources.GetObject("picShaderAngewandt.Image"), System.Drawing.Image)
        Me.picShaderAngewandt.Location = New System.Drawing.Point(1088, 531)
        Me.picShaderAngewandt.Name = "picShaderAngewandt"
        Me.picShaderAngewandt.Size = New System.Drawing.Size(640, 360)
        Me.picShaderAngewandt.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picShaderAngewandt.TabIndex = 7
        Me.picShaderAngewandt.TabStop = False
        '
        'picTransition
        '
        Me.picTransition.Image = CType(resources.GetObject("picTransition.Image"), System.Drawing.Image)
        Me.picTransition.Location = New System.Drawing.Point(1088, 988)
        Me.picTransition.Name = "picTransition"
        Me.picTransition.Size = New System.Drawing.Size(640, 360)
        Me.picTransition.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picTransition.TabIndex = 8
        Me.picTransition.TabStop = False
        '
        'ofdBildauswahl
        '
        Me.ofdBildauswahl.FileName = "OpenFileDialog1"
        '
        'tabOptionDialoge
        '
        Me.tabOptionDialoge.Controls.Add(Me.tpShader)
        Me.tabOptionDialoge.Controls.Add(Me.tpTransition)
        Me.tabOptionDialoge.Location = New System.Drawing.Point(2434, 176)
        Me.tabOptionDialoge.Name = "tabOptionDialoge"
        Me.tabOptionDialoge.SelectedIndex = 0
        Me.tabOptionDialoge.Size = New System.Drawing.Size(900, 1040)
        Me.tabOptionDialoge.TabIndex = 9
        '
        'tpShader
        '
        Me.tpShader.Location = New System.Drawing.Point(10, 59)
        Me.tpShader.Name = "tpShader"
        Me.tpShader.Padding = New System.Windows.Forms.Padding(3)
        Me.tpShader.Size = New System.Drawing.Size(880, 971)
        Me.tpShader.TabIndex = 0
        Me.tpShader.Text = "Shader"
        Me.tpShader.UseVisualStyleBackColor = True
        '
        'tpTransition
        '
        Me.tpTransition.Location = New System.Drawing.Point(10, 59)
        Me.tpTransition.Name = "tpTransition"
        Me.tpTransition.Padding = New System.Windows.Forms.Padding(3)
        Me.tpTransition.Size = New System.Drawing.Size(880, 971)
        Me.tpTransition.TabIndex = 1
        Me.tpTransition.Text = "Transition"
        Me.tpTransition.UseVisualStyleBackColor = True
        '
        'lblOptionsdialoge
        '
        Me.lblOptionsdialoge.AutoSize = True
        Me.lblOptionsdialoge.Location = New System.Drawing.Point(2434, 119)
        Me.lblOptionsdialoge.Name = "lblOptionsdialoge"
        Me.lblOptionsdialoge.Size = New System.Drawing.Size(223, 41)
        Me.lblOptionsdialoge.TabIndex = 10
        Me.lblOptionsdialoge.Text = "Optionsdialoge"
        '
        'lblTitelShader
        '
        Me.lblTitelShader.AutoSize = True
        Me.lblTitelShader.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitelShader.Location = New System.Drawing.Point(46, 470)
        Me.lblTitelShader.Name = "lblTitelShader"
        Me.lblTitelShader.Size = New System.Drawing.Size(116, 41)
        Me.lblTitelShader.TabIndex = 11
        Me.lblTitelShader.Text = "Shader"
        '
        'lvRegValsShader
        '
        Me.lvRegValsShader.HideSelection = False
        Me.lvRegValsShader.Location = New System.Drawing.Point(1759, 531)
        Me.lvRegValsShader.Name = "lvRegValsShader"
        Me.lvRegValsShader.Size = New System.Drawing.Size(633, 360)
        Me.lvRegValsShader.TabIndex = 12
        Me.lvRegValsShader.UseCompatibleStateImageBehavior = False
        '
        'lvRegValsTransition
        '
        Me.lvRegValsTransition.HideSelection = False
        Me.lvRegValsTransition.Location = New System.Drawing.Point(1759, 988)
        Me.lvRegValsTransition.Name = "lvRegValsTransition"
        Me.lvRegValsTransition.Size = New System.Drawing.Size(633, 360)
        Me.lvRegValsTransition.TabIndex = 13
        Me.lvRegValsTransition.UseCompatibleStateImageBehavior = False
        '
        'lblShaderName
        '
        Me.lblShaderName.AutoSize = True
        Me.lblShaderName.Location = New System.Drawing.Point(46, 605)
        Me.lblShaderName.MaximumSize = New System.Drawing.Size(320, 0)
        Me.lblShaderName.Name = "lblShaderName"
        Me.lblShaderName.Size = New System.Drawing.Size(197, 41)
        Me.lblShaderName.TabIndex = 14
        Me.lblShaderName.Text = "Shader Name"
        '
        'lblShaderKurzbeschreibung
        '
        Me.lblShaderKurzbeschreibung.AutoSize = True
        Me.lblShaderKurzbeschreibung.Location = New System.Drawing.Point(46, 656)
        Me.lblShaderKurzbeschreibung.MaximumSize = New System.Drawing.Size(320, 0)
        Me.lblShaderKurzbeschreibung.Name = "lblShaderKurzbeschreibung"
        Me.lblShaderKurzbeschreibung.Size = New System.Drawing.Size(257, 82)
        Me.lblShaderKurzbeschreibung.TabIndex = 15
        Me.lblShaderKurzbeschreibung.Text = "Shader Kurzbeschreibung"
        '
        'lblShaderVersion
        '
        Me.lblShaderVersion.AutoSize = True
        Me.lblShaderVersion.Location = New System.Drawing.Point(46, 850)
        Me.lblShaderVersion.MaximumSize = New System.Drawing.Size(320, 0)
        Me.lblShaderVersion.Name = "lblShaderVersion"
        Me.lblShaderVersion.Size = New System.Drawing.Size(216, 41)
        Me.lblShaderVersion.TabIndex = 16
        Me.lblShaderVersion.Text = "Shader Version"
        '
        'lblTitelTransition
        '
        Me.lblTitelTransition.AutoSize = True
        Me.lblTitelTransition.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitelTransition.Location = New System.Drawing.Point(46, 929)
        Me.lblTitelTransition.Name = "lblTitelTransition"
        Me.lblTitelTransition.Size = New System.Drawing.Size(159, 41)
        Me.lblTitelTransition.TabIndex = 17
        Me.lblTitelTransition.Text = "Transition"
        '
        'cmbShaderAuswahl
        '
        Me.cmbShaderAuswahl.FormattingEnabled = True
        Me.cmbShaderAuswahl.Location = New System.Drawing.Point(53, 531)
        Me.cmbShaderAuswahl.Name = "cmbShaderAuswahl"
        Me.cmbShaderAuswahl.Size = New System.Drawing.Size(320, 49)
        Me.cmbShaderAuswahl.TabIndex = 18
        '
        'cmbTransitionAuswahl
        '
        Me.cmbTransitionAuswahl.FormattingEnabled = True
        Me.cmbTransitionAuswahl.Location = New System.Drawing.Point(53, 988)
        Me.cmbTransitionAuswahl.Name = "cmbTransitionAuswahl"
        Me.cmbTransitionAuswahl.Size = New System.Drawing.Size(320, 49)
        Me.cmbTransitionAuswahl.TabIndex = 22
        '
        'lblTransitionVersion
        '
        Me.lblTransitionVersion.AutoSize = True
        Me.lblTransitionVersion.Location = New System.Drawing.Point(46, 1307)
        Me.lblTransitionVersion.MaximumSize = New System.Drawing.Size(320, 0)
        Me.lblTransitionVersion.Name = "lblTransitionVersion"
        Me.lblTransitionVersion.Size = New System.Drawing.Size(251, 41)
        Me.lblTransitionVersion.TabIndex = 21
        Me.lblTransitionVersion.Text = "Transition Version"
        '
        'lblTransitionKurzbeschreibung
        '
        Me.lblTransitionKurzbeschreibung.AutoSize = True
        Me.lblTransitionKurzbeschreibung.Location = New System.Drawing.Point(46, 1113)
        Me.lblTransitionKurzbeschreibung.MaximumSize = New System.Drawing.Size(320, 0)
        Me.lblTransitionKurzbeschreibung.Name = "lblTransitionKurzbeschreibung"
        Me.lblTransitionKurzbeschreibung.Size = New System.Drawing.Size(257, 82)
        Me.lblTransitionKurzbeschreibung.TabIndex = 20
        Me.lblTransitionKurzbeschreibung.Text = "Transition Kurzbeschreibung"
        '
        'lblTransitionName
        '
        Me.lblTransitionName.AutoSize = True
        Me.lblTransitionName.Location = New System.Drawing.Point(46, 1062)
        Me.lblTransitionName.MaximumSize = New System.Drawing.Size(320, 0)
        Me.lblTransitionName.Name = "lblTransitionName"
        Me.lblTransitionName.Size = New System.Drawing.Size(232, 41)
        Me.lblTransitionName.TabIndex = 19
        Me.lblTransitionName.Text = "Transition Name"
        '
        'trbLoop
        '
        Me.trbLoop.Location = New System.Drawing.Point(701, 1197)
        Me.trbLoop.Maximum = 120
        Me.trbLoop.Minimum = 1
        Me.trbLoop.Name = "trbLoop"
        Me.trbLoop.Size = New System.Drawing.Size(363, 101)
        Me.trbLoop.TabIndex = 23
        Me.trbLoop.Value = 5
        '
        'rbTransitionModeManual
        '
        Me.rbTransitionModeManual.AutoSize = True
        Me.rbTransitionModeManual.Location = New System.Drawing.Point(424, 988)
        Me.rbTransitionModeManual.Name = "rbTransitionModeManual"
        Me.rbTransitionModeManual.Size = New System.Drawing.Size(249, 45)
        Me.rbTransitionModeManual.TabIndex = 24
        Me.rbTransitionModeManual.TabStop = True
        Me.rbTransitionModeManual.Text = "Manueller Start"
        Me.rbTransitionModeManual.UseVisualStyleBackColor = True
        '
        'rbTransitionModeLoop
        '
        Me.rbTransitionModeLoop.AutoSize = True
        Me.rbTransitionModeLoop.Location = New System.Drawing.Point(426, 1197)
        Me.rbTransitionModeLoop.Name = "rbTransitionModeLoop"
        Me.rbTransitionModeLoop.Size = New System.Drawing.Size(117, 45)
        Me.rbTransitionModeLoop.TabIndex = 25
        Me.rbTransitionModeLoop.TabStop = True
        Me.rbTransitionModeLoop.Text = "Loop"
        Me.rbTransitionModeLoop.UseVisualStyleBackColor = True
        '
        'btnRunTransition
        '
        Me.btnRunTransition.Location = New System.Drawing.Point(701, 988)
        Me.btnRunTransition.Name = "btnRunTransition"
        Me.btnRunTransition.Size = New System.Drawing.Size(363, 49)
        Me.btnRunTransition.TabIndex = 26
        Me.btnRunTransition.Text = "RunTransition()"
        Me.btnRunTransition.UseVisualStyleBackColor = True
        '
        'chbTransitionDuration
        '
        Me.chbTransitionDuration.AutoSize = True
        Me.chbTransitionDuration.Location = New System.Drawing.Point(701, 1062)
        Me.chbTransitionDuration.Name = "chbTransitionDuration"
        Me.chbTransitionDuration.Size = New System.Drawing.Size(295, 45)
        Me.chbTransitionDuration.TabIndex = 27
        Me.chbTransitionDuration.Text = "Duration festlegen"
        Me.chbTransitionDuration.UseVisualStyleBackColor = True
        '
        'nudDurationMS
        '
        Me.nudDurationMS.Location = New System.Drawing.Point(735, 1113)
        Me.nudDurationMS.Name = "nudDurationMS"
        Me.nudDurationMS.Size = New System.Drawing.Size(120, 47)
        Me.nudDurationMS.TabIndex = 28
        '
        'frmShaderTranstionTester
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(3372, 1378)
        Me.Controls.Add(Me.nudDurationMS)
        Me.Controls.Add(Me.chbTransitionDuration)
        Me.Controls.Add(Me.btnRunTransition)
        Me.Controls.Add(Me.rbTransitionModeLoop)
        Me.Controls.Add(Me.rbTransitionModeManual)
        Me.Controls.Add(Me.trbLoop)
        Me.Controls.Add(Me.cmbTransitionAuswahl)
        Me.Controls.Add(Me.lblTransitionVersion)
        Me.Controls.Add(Me.lblTransitionKurzbeschreibung)
        Me.Controls.Add(Me.lblTransitionName)
        Me.Controls.Add(Me.cmbShaderAuswahl)
        Me.Controls.Add(Me.lblTitelTransition)
        Me.Controls.Add(Me.lblShaderVersion)
        Me.Controls.Add(Me.lblShaderKurzbeschreibung)
        Me.Controls.Add(Me.lblShaderName)
        Me.Controls.Add(Me.lvRegValsTransition)
        Me.Controls.Add(Me.lvRegValsShader)
        Me.Controls.Add(Me.lblTitelShader)
        Me.Controls.Add(Me.lblOptionsdialoge)
        Me.Controls.Add(Me.tabOptionDialoge)
        Me.Controls.Add(Me.picTransition)
        Me.Controls.Add(Me.picShaderAngewandt)
        Me.Controls.Add(Me.cmbPBSizeMode2)
        Me.Controls.Add(Me.cmbPBSizeMod1)
        Me.Controls.Add(Me.lblTestbilder)
        Me.Controls.Add(Me.picTest2)
        Me.Controls.Add(Me.lblTitel)
        Me.Controls.Add(Me.picTest1)
        Me.Controls.Add(Me.picShaderOriginal)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmShaderTranstionTester"
        Me.Text = "5"
        CType(Me.picShaderOriginal, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picTest1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picTest2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picShaderAngewandt, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picTransition, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabOptionDialoge.ResumeLayout(False)
        CType(Me.trbLoop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudDurationMS, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents picShaderOriginal As PictureBox
    Friend WithEvents picTest1 As PictureBox
    Friend WithEvents lblTitel As Label
    Friend WithEvents picTest2 As PictureBox
    Friend WithEvents lblTestbilder As Label
    Friend WithEvents cmbPBSizeMod1 As ComboBox
    Friend WithEvents cmbPBSizeMode2 As ComboBox
    Friend WithEvents picShaderAngewandt As PictureBox
    Friend WithEvents picTransition As PictureBox
    Friend WithEvents ofdBildauswahl As OpenFileDialog
    Friend WithEvents tabOptionDialoge As TabControl
    Friend WithEvents tpShader As TabPage
    Friend WithEvents tpTransition As TabPage
    Friend WithEvents lblOptionsdialoge As Label
    Friend WithEvents lblTitelShader As Label
    Friend WithEvents lvRegValsShader As ListView
    Friend WithEvents lvRegValsTransition As ListView
    Friend WithEvents lblShaderName As Label
    Friend WithEvents lblShaderKurzbeschreibung As Label
    Friend WithEvents lblShaderVersion As Label
    Friend WithEvents lblTitelTransition As Label
    Friend WithEvents cmbShaderAuswahl As ComboBox
    Friend WithEvents cmbTransitionAuswahl As ComboBox
    Friend WithEvents lblTransitionVersion As Label
    Friend WithEvents lblTransitionKurzbeschreibung As Label
    Friend WithEvents lblTransitionName As Label
    Friend WithEvents trbLoop As TrackBar
    Friend WithEvents rbTransitionModeManual As RadioButton
    Friend WithEvents rbTransitionModeLoop As RadioButton
    Friend WithEvents btnRunTransition As Button
    Friend WithEvents chbTransitionDuration As CheckBox
    Friend WithEvents nudDurationMS As NumericUpDown
End Class
