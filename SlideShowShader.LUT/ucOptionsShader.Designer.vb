<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsShader
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
        Me.lblShadername = New System.Windows.Forms.Label()
        Me.lblNShaderName = New System.Windows.Forms.Label()
        Me.clbLUTs = New System.Windows.Forms.CheckedListBox()
        Me.lblNLUTs = New System.Windows.Forms.Label()
        Me.lblNIntensitaet = New System.Windows.Forms.Label()
        Me.lblIntensitaet = New System.Windows.Forms.Label()
        Me.trkIntensitaet = New System.Windows.Forms.TrackBar()
        Me.btnDefaults = New System.Windows.Forms.Button()
        CType(Me.trkIntensitaet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblShadername.Location = New System.Drawing.Point(264, 26)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(178, 41)
        Me.lblShadername.TabIndex = 17
        Me.lblShadername.Text = "LUT Shader"
        '
        'lblNShaderName
        '
        Me.lblNShaderName.AutoSize = True
        Me.lblNShaderName.Location = New System.Drawing.Point(34, 26)
        Me.lblNShaderName.Name = "lblNShaderName"
        Me.lblNShaderName.Size = New System.Drawing.Size(110, 41)
        Me.lblNShaderName.TabIndex = 16
        Me.lblNShaderName.Text = "Shader"
        '
        'clbLUTs
        '
        Me.clbLUTs.FormattingEnabled = True
        Me.clbLUTs.Location = New System.Drawing.Point(41, 141)
        Me.clbLUTs.Name = "clbLUTs"
        Me.clbLUTs.Size = New System.Drawing.Size(810, 576)
        Me.clbLUTs.TabIndex = 18
        '
        'lblNLUTs
        '
        Me.lblNLUTs.AutoSize = True
        Me.lblNLUTs.Location = New System.Drawing.Point(34, 97)
        Me.lblNLUTs.Name = "lblNLUTs"
        Me.lblNLUTs.Size = New System.Drawing.Size(79, 41)
        Me.lblNLUTs.TabIndex = 19
        Me.lblNLUTs.Text = "LUTs"
        '
        'lblNIntensitaet
        '
        Me.lblNIntensitaet.AutoSize = True
        Me.lblNIntensitaet.Location = New System.Drawing.Point(41, 756)
        Me.lblNIntensitaet.Name = "lblNIntensitaet"
        Me.lblNIntensitaet.Size = New System.Drawing.Size(141, 41)
        Me.lblNIntensitaet.TabIndex = 20
        Me.lblNIntensitaet.Text = "Intensität"
        '
        'lblIntensitaet
        '
        Me.lblIntensitaet.AutoSize = True
        Me.lblIntensitaet.Location = New System.Drawing.Point(747, 756)
        Me.lblIntensitaet.MaximumSize = New System.Drawing.Size(99, 41)
        Me.lblIntensitaet.MinimumSize = New System.Drawing.Size(99, 41)
        Me.lblIntensitaet.Name = "lblIntensitaet"
        Me.lblIntensitaet.Size = New System.Drawing.Size(99, 41)
        Me.lblIntensitaet.TabIndex = 21
        Me.lblIntensitaet.Text = "30 %"
        Me.lblIntensitaet.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trkIntensitaet
        '
        Me.trkIntensitaet.AutoSize = False
        Me.trkIntensitaet.Location = New System.Drawing.Point(200, 756)
        Me.trkIntensitaet.Maximum = 100
        Me.trkIntensitaet.Name = "trkIntensitaet"
        Me.trkIntensitaet.Size = New System.Drawing.Size(541, 54)
        Me.trkIntensitaet.TabIndex = 22
        Me.trkIntensitaet.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkIntensitaet.Value = 30
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(671, 19)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(175, 54)
        Me.btnDefaults.TabIndex = 23
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.trkIntensitaet)
        Me.Controls.Add(Me.lblIntensitaet)
        Me.Controls.Add(Me.lblNIntensitaet)
        Me.Controls.Add(Me.lblNLUTs)
        Me.Controls.Add(Me.clbLUTs)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNShaderName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsShader"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkIntensitaet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNShaderName As Windows.Forms.Label
    Friend WithEvents clbLUTs As Forms.CheckedListBox
    Friend WithEvents lblNLUTs As Forms.Label
    Friend WithEvents lblNIntensitaet As Forms.Label
    Friend WithEvents lblIntensitaet As Forms.Label
    Friend WithEvents trkIntensitaet As Forms.TrackBar
    Friend WithEvents btnDefaults As Forms.Button
End Class
