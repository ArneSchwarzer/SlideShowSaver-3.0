<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucOptionsShader
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
        Me.lblShadername = New System.Windows.Forms.Label()
        Me.lblNShaderName = New System.Windows.Forms.Label()
        Me.tcAquarell = New System.Windows.Forms.TabControl()
        Me.tpPinselPigmente = New System.Windows.Forms.TabPage()
        Me.tpFluessigkeit = New System.Windows.Forms.TabPage()
        Me.tpPapier = New System.Windows.Forms.TabPage()
        Me.tpSimulation = New System.Windows.Forms.TabPage()
        Me.btnDefaults = New System.Windows.Forms.Button()
        Me.lblNtrkIterationen = New System.Windows.Forms.Label()
        Me.lblIterationen = New System.Windows.Forms.Label()
        Me.TrackBar1 = New System.Windows.Forms.TrackBar()
        Me.trkViskositaet = New System.Windows.Forms.TrackBar()
        Me.lblViskositaet = New System.Windows.Forms.Label()
        Me.lblNtrkViskositaet = New System.Windows.Forms.Label()
        Me.trkFluessigkeitMenge = New System.Windows.Forms.TrackBar()
        Me.lblFluessigkeitMenge = New System.Windows.Forms.Label()
        Me.lblNtrkFluessigkeitMenge = New System.Windows.Forms.Label()
        Me.trkPigmentbeweglichkeit = New System.Windows.Forms.TrackBar()
        Me.lblPigmentbeweglichkeit = New System.Windows.Forms.Label()
        Me.lblNPigmentbeweglichkeit = New System.Windows.Forms.Label()
        Me.trkKuwaharaRadius = New System.Windows.Forms.TrackBar()
        Me.lblKuwaharaRadius = New System.Windows.Forms.Label()
        Me.lblNtrkKuwaharaRadius = New System.Windows.Forms.Label()
        Me.tcAquarell.SuspendLayout()
        Me.tpPinselPigmente.SuspendLayout()
        Me.tpFluessigkeit.SuspendLayout()
        Me.tpSimulation.SuspendLayout()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkViskositaet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkFluessigkeitMenge, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkPigmentbeweglichkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkKuwaharaRadius, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblShadername.Location = New System.Drawing.Point(255, 26)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(138, 41)
        Me.lblShadername.TabIndex = 17
        Me.lblShadername.Tag = "langKey=lblShadername"
        Me.lblShadername.Text = "Aquarell"
        '
        'lblNShaderName
        '
        Me.lblNShaderName.AutoSize = True
        Me.lblNShaderName.Location = New System.Drawing.Point(34, 26)
        Me.lblNShaderName.Name = "lblNShaderName"
        Me.lblNShaderName.Size = New System.Drawing.Size(110, 41)
        Me.lblNShaderName.TabIndex = 16
        Me.lblNShaderName.Tag = "langKey=lblNShaderName"
        Me.lblNShaderName.Text = "Shader"
        '
        'tcAquarell
        '
        Me.tcAquarell.Controls.Add(Me.tpPinselPigmente)
        Me.tcAquarell.Controls.Add(Me.tpFluessigkeit)
        Me.tcAquarell.Controls.Add(Me.tpPapier)
        Me.tcAquarell.Controls.Add(Me.tpSimulation)
        Me.tcAquarell.Location = New System.Drawing.Point(41, 103)
        Me.tcAquarell.Name = "tcAquarell"
        Me.tcAquarell.SelectedIndex = 0
        Me.tcAquarell.Size = New System.Drawing.Size(802, 852)
        Me.tcAquarell.TabIndex = 18
        '
        'tpPinselPigmente
        '
        Me.tpPinselPigmente.Controls.Add(Me.trkKuwaharaRadius)
        Me.tpPinselPigmente.Controls.Add(Me.lblKuwaharaRadius)
        Me.tpPinselPigmente.Controls.Add(Me.lblNtrkKuwaharaRadius)
        Me.tpPinselPigmente.Controls.Add(Me.trkPigmentbeweglichkeit)
        Me.tpPinselPigmente.Controls.Add(Me.lblPigmentbeweglichkeit)
        Me.tpPinselPigmente.Controls.Add(Me.lblNPigmentbeweglichkeit)
        Me.tpPinselPigmente.Location = New System.Drawing.Point(10, 59)
        Me.tpPinselPigmente.Name = "tpPinselPigmente"
        Me.tpPinselPigmente.Padding = New System.Windows.Forms.Padding(3)
        Me.tpPinselPigmente.Size = New System.Drawing.Size(782, 783)
        Me.tpPinselPigmente.TabIndex = 0
        Me.tpPinselPigmente.Text = "Pinsel & Pigmente"
        Me.tpPinselPigmente.UseVisualStyleBackColor = True
        '
        'tpFluessigkeit
        '
        Me.tpFluessigkeit.Controls.Add(Me.trkFluessigkeitMenge)
        Me.tpFluessigkeit.Controls.Add(Me.lblFluessigkeitMenge)
        Me.tpFluessigkeit.Controls.Add(Me.lblNtrkFluessigkeitMenge)
        Me.tpFluessigkeit.Controls.Add(Me.trkViskositaet)
        Me.tpFluessigkeit.Controls.Add(Me.lblViskositaet)
        Me.tpFluessigkeit.Controls.Add(Me.lblNtrkViskositaet)
        Me.tpFluessigkeit.Location = New System.Drawing.Point(10, 59)
        Me.tpFluessigkeit.Name = "tpFluessigkeit"
        Me.tpFluessigkeit.Padding = New System.Windows.Forms.Padding(3)
        Me.tpFluessigkeit.Size = New System.Drawing.Size(782, 783)
        Me.tpFluessigkeit.TabIndex = 1
        Me.tpFluessigkeit.Text = "Flüssigkeit"
        Me.tpFluessigkeit.UseVisualStyleBackColor = True
        '
        'tpPapier
        '
        Me.tpPapier.Location = New System.Drawing.Point(10, 59)
        Me.tpPapier.Name = "tpPapier"
        Me.tpPapier.Padding = New System.Windows.Forms.Padding(3)
        Me.tpPapier.Size = New System.Drawing.Size(782, 783)
        Me.tpPapier.TabIndex = 2
        Me.tpPapier.Text = "Papier"
        Me.tpPapier.UseVisualStyleBackColor = True
        '
        'tpSimulation
        '
        Me.tpSimulation.Controls.Add(Me.TrackBar1)
        Me.tpSimulation.Controls.Add(Me.lblIterationen)
        Me.tpSimulation.Controls.Add(Me.lblNtrkIterationen)
        Me.tpSimulation.Location = New System.Drawing.Point(10, 59)
        Me.tpSimulation.Name = "tpSimulation"
        Me.tpSimulation.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSimulation.Size = New System.Drawing.Size(782, 783)
        Me.tpSimulation.TabIndex = 3
        Me.tpSimulation.Text = "Simulation"
        Me.tpSimulation.UseVisualStyleBackColor = True
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(658, 19)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(185, 55)
        Me.btnDefaults.TabIndex = 33
        Me.btnDefaults.Tag = "langKey=btnDefaults"
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'lblNtrkIterationen
        '
        Me.lblNtrkIterationen.AutoSize = True
        Me.lblNtrkIterationen.Location = New System.Drawing.Point(19, 43)
        Me.lblNtrkIterationen.Name = "lblNtrkIterationen"
        Me.lblNtrkIterationen.Size = New System.Drawing.Size(162, 41)
        Me.lblNtrkIterationen.TabIndex = 0
        Me.lblNtrkIterationen.Text = "Iterationen"
        '
        'lblIterationen
        '
        Me.lblIterationen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblIterationen.AutoSize = True
        Me.lblIterationen.Location = New System.Drawing.Point(706, 43)
        Me.lblIterationen.MaximumSize = New System.Drawing.Size(70, 0)
        Me.lblIterationen.MinimumSize = New System.Drawing.Size(70, 0)
        Me.lblIterationen.Name = "lblIterationen"
        Me.lblIterationen.Size = New System.Drawing.Size(70, 41)
        Me.lblIterationen.TabIndex = 1
        Me.lblIterationen.Text = "20"
        Me.lblIterationen.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TrackBar1
        '
        Me.TrackBar1.Location = New System.Drawing.Point(188, 43)
        Me.TrackBar1.Maximum = 100
        Me.TrackBar1.Minimum = 10
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Size = New System.Drawing.Size(512, 101)
        Me.TrackBar1.TabIndex = 2
        Me.TrackBar1.Value = 20
        '
        'trkViskositaet
        '
        Me.trkViskositaet.AutoSize = False
        Me.trkViskositaet.LargeChange = 50
        Me.trkViskositaet.Location = New System.Drawing.Point(184, 17)
        Me.trkViskositaet.Maximum = 400
        Me.trkViskositaet.Minimum = -100
        Me.trkViskositaet.Name = "trkViskositaet"
        Me.trkViskositaet.Size = New System.Drawing.Size(592, 48)
        Me.trkViskositaet.SmallChange = 10
        Me.trkViskositaet.TabIndex = 5
        '
        'lblViskositaet
        '
        Me.lblViskositaet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblViskositaet.AutoSize = True
        Me.lblViskositaet.Location = New System.Drawing.Point(177, 68)
        Me.lblViskositaet.Name = "lblViskositaet"
        Me.lblViskositaet.Size = New System.Drawing.Size(608, 41)
        Me.lblViskositaet.TabIndex = 4
        Me.lblViskositaet.Text = "1 mPa/s (entspricht ca. Wasser - oder Kaffee)"
        '
        'lblNtrkViskositaet
        '
        Me.lblNtrkViskositaet.AutoSize = True
        Me.lblNtrkViskositaet.Location = New System.Drawing.Point(15, 17)
        Me.lblNtrkViskositaet.Name = "lblNtrkViskositaet"
        Me.lblNtrkViskositaet.Size = New System.Drawing.Size(145, 41)
        Me.lblNtrkViskositaet.TabIndex = 3
        Me.lblNtrkViskositaet.Text = "Viskosität"
        '
        'trkFluessigkeitMenge
        '
        Me.trkFluessigkeitMenge.Location = New System.Drawing.Point(184, 149)
        Me.trkFluessigkeitMenge.Maximum = 100
        Me.trkFluessigkeitMenge.Minimum = 1
        Me.trkFluessigkeitMenge.Name = "trkFluessigkeitMenge"
        Me.trkFluessigkeitMenge.Size = New System.Drawing.Size(428, 101)
        Me.trkFluessigkeitMenge.TabIndex = 8
        Me.trkFluessigkeitMenge.Value = 20
        '
        'lblFluessigkeitMenge
        '
        Me.lblFluessigkeitMenge.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFluessigkeitMenge.AutoSize = True
        Me.lblFluessigkeitMenge.Location = New System.Drawing.Point(604, 149)
        Me.lblFluessigkeitMenge.MaximumSize = New System.Drawing.Size(175, 0)
        Me.lblFluessigkeitMenge.MinimumSize = New System.Drawing.Size(175, 0)
        Me.lblFluessigkeitMenge.Name = "lblFluessigkeitMenge"
        Me.lblFluessigkeitMenge.Size = New System.Drawing.Size(175, 41)
        Me.lblFluessigkeitMenge.TabIndex = 7
        Me.lblFluessigkeitMenge.Text = " 20 µl/cm²"
        Me.lblFluessigkeitMenge.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblNtrkFluessigkeitMenge
        '
        Me.lblNtrkFluessigkeitMenge.AutoSize = True
        Me.lblNtrkFluessigkeitMenge.Location = New System.Drawing.Point(15, 149)
        Me.lblNtrkFluessigkeitMenge.Name = "lblNtrkFluessigkeitMenge"
        Me.lblNtrkFluessigkeitMenge.Size = New System.Drawing.Size(112, 41)
        Me.lblNtrkFluessigkeitMenge.TabIndex = 6
        Me.lblNtrkFluessigkeitMenge.Text = "Menge"
        '
        'trkPigmentbeweglichkeit
        '
        Me.trkPigmentbeweglichkeit.AutoSize = False
        Me.trkPigmentbeweglichkeit.Location = New System.Drawing.Point(211, 110)
        Me.trkPigmentbeweglichkeit.Maximum = 100
        Me.trkPigmentbeweglichkeit.Minimum = 1
        Me.trkPigmentbeweglichkeit.Name = "trkPigmentbeweglichkeit"
        Me.trkPigmentbeweglichkeit.Size = New System.Drawing.Size(483, 59)
        Me.trkPigmentbeweglichkeit.TabIndex = 5
        Me.trkPigmentbeweglichkeit.Value = 20
        '
        'lblPigmentbeweglichkeit
        '
        Me.lblPigmentbeweglichkeit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPigmentbeweglichkeit.AutoSize = True
        Me.lblPigmentbeweglichkeit.Location = New System.Drawing.Point(700, 110)
        Me.lblPigmentbeweglichkeit.MaximumSize = New System.Drawing.Size(70, 0)
        Me.lblPigmentbeweglichkeit.MinimumSize = New System.Drawing.Size(70, 0)
        Me.lblPigmentbeweglichkeit.Name = "lblPigmentbeweglichkeit"
        Me.lblPigmentbeweglichkeit.Size = New System.Drawing.Size(70, 41)
        Me.lblPigmentbeweglichkeit.TabIndex = 4
        Me.lblPigmentbeweglichkeit.Text = "20"
        Me.lblPigmentbeweglichkeit.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblNPigmentbeweglichkeit
        '
        Me.lblNPigmentbeweglichkeit.AutoSize = True
        Me.lblNPigmentbeweglichkeit.Location = New System.Drawing.Point(13, 110)
        Me.lblNPigmentbeweglichkeit.MaximumSize = New System.Drawing.Size(210, 0)
        Me.lblNPigmentbeweglichkeit.Name = "lblNPigmentbeweglichkeit"
        Me.lblNPigmentbeweglichkeit.Size = New System.Drawing.Size(201, 82)
        Me.lblNPigmentbeweglichkeit.TabIndex = 3
        Me.lblNPigmentbeweglichkeit.Text = "Pigment- beweglichkeit"
        '
        'trkKuwaharaRadius
        '
        Me.trkKuwaharaRadius.AutoSize = False
        Me.trkKuwaharaRadius.Location = New System.Drawing.Point(211, 19)
        Me.trkKuwaharaRadius.Maximum = 48
        Me.trkKuwaharaRadius.Minimum = 16
        Me.trkKuwaharaRadius.Name = "trkKuwaharaRadius"
        Me.trkKuwaharaRadius.Size = New System.Drawing.Size(483, 56)
        Me.trkKuwaharaRadius.TabIndex = 8
        Me.trkKuwaharaRadius.Value = 32
        '
        'lblKuwaharaRadius
        '
        Me.lblKuwaharaRadius.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblKuwaharaRadius.AutoSize = True
        Me.lblKuwaharaRadius.Location = New System.Drawing.Point(700, 19)
        Me.lblKuwaharaRadius.MaximumSize = New System.Drawing.Size(70, 0)
        Me.lblKuwaharaRadius.MinimumSize = New System.Drawing.Size(70, 0)
        Me.lblKuwaharaRadius.Name = "lblKuwaharaRadius"
        Me.lblKuwaharaRadius.Size = New System.Drawing.Size(70, 41)
        Me.lblKuwaharaRadius.TabIndex = 7
        Me.lblKuwaharaRadius.Text = "32"
        Me.lblKuwaharaRadius.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'lblNtrkKuwaharaRadius
        '
        Me.lblNtrkKuwaharaRadius.AutoSize = True
        Me.lblNtrkKuwaharaRadius.Location = New System.Drawing.Point(13, 19)
        Me.lblNtrkKuwaharaRadius.Name = "lblNtrkKuwaharaRadius"
        Me.lblNtrkKuwaharaRadius.Size = New System.Drawing.Size(172, 41)
        Me.lblNtrkKuwaharaRadius.TabIndex = 6
        Me.lblNtrkKuwaharaRadius.Text = "Pinselbreite"
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.tcAquarell)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNShaderName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsShader"
        Me.Size = New System.Drawing.Size(890, 1020)
        Me.tcAquarell.ResumeLayout(False)
        Me.tpPinselPigmente.ResumeLayout(False)
        Me.tpPinselPigmente.PerformLayout()
        Me.tpFluessigkeit.ResumeLayout(False)
        Me.tpFluessigkeit.PerformLayout()
        Me.tpSimulation.ResumeLayout(False)
        Me.tpSimulation.PerformLayout()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkViskositaet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkFluessigkeitMenge, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkPigmentbeweglichkeit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkKuwaharaRadius, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNShaderName As Windows.Forms.Label
    Friend WithEvents tcAquarell As Windows.Forms.TabControl
    Friend WithEvents tpPinselPigmente As Windows.Forms.TabPage
    Friend WithEvents tpFluessigkeit As Windows.Forms.TabPage
    Friend WithEvents tpPapier As Windows.Forms.TabPage
    Friend WithEvents tpSimulation As Windows.Forms.TabPage
    Friend WithEvents btnDefaults As Windows.Forms.Button
    Friend WithEvents lblIterationen As Windows.Forms.Label
    Friend WithEvents lblNtrkIterationen As Windows.Forms.Label
    Friend WithEvents TrackBar1 As Windows.Forms.TrackBar
    Friend WithEvents trkViskositaet As Windows.Forms.TrackBar
    Friend WithEvents lblViskositaet As Windows.Forms.Label
    Friend WithEvents lblNtrkViskositaet As Windows.Forms.Label
    Friend WithEvents trkFluessigkeitMenge As Windows.Forms.TrackBar
    Friend WithEvents lblFluessigkeitMenge As Windows.Forms.Label
    Friend WithEvents lblNtrkFluessigkeitMenge As Windows.Forms.Label
    Friend WithEvents trkKuwaharaRadius As Windows.Forms.TrackBar
    Friend WithEvents lblKuwaharaRadius As Windows.Forms.Label
    Friend WithEvents lblNtrkKuwaharaRadius As Windows.Forms.Label
    Friend WithEvents trkPigmentbeweglichkeit As Windows.Forms.TrackBar
    Friend WithEvents lblPigmentbeweglichkeit As Windows.Forms.Label
    Friend WithEvents lblNPigmentbeweglichkeit As Windows.Forms.Label
End Class
