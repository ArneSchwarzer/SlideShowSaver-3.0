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
        Me.lblNModus = New System.Windows.Forms.Label()
        Me.rdoFarbe = New System.Windows.Forms.RadioButton()
        Me.rdoWS = New System.Windows.Forms.RadioButton()
        Me.rdoZufall = New System.Windows.Forms.RadioButton()
        Me.btnDefaults = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblShadername.Location = New System.Drawing.Point(264, 26)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(174, 41)
        Me.lblShadername.TabIndex = 17
        Me.lblShadername.Text = "Invertieren"
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
        'lblNModus
        '
        Me.lblNModus.AutoSize = True
        Me.lblNModus.Location = New System.Drawing.Point(34, 144)
        Me.lblNModus.Name = "lblNModus"
        Me.lblNModus.Size = New System.Drawing.Size(111, 41)
        Me.lblNModus.TabIndex = 18
        Me.lblNModus.Text = "Modus"
        '
        'rdoFarbe
        '
        Me.rdoFarbe.AutoSize = True
        Me.rdoFarbe.Location = New System.Drawing.Point(271, 144)
        Me.rdoFarbe.Name = "rdoFarbe"
        Me.rdoFarbe.Size = New System.Drawing.Size(122, 45)
        Me.rdoFarbe.TabIndex = 19
        Me.rdoFarbe.TabStop = True
        Me.rdoFarbe.Text = "Farbe"
        Me.rdoFarbe.UseVisualStyleBackColor = True
        '
        'rdoWS
        '
        Me.rdoWS.AutoSize = True
        Me.rdoWS.Location = New System.Drawing.Point(271, 196)
        Me.rdoWS.Name = "rdoWS"
        Me.rdoWS.Size = New System.Drawing.Size(245, 45)
        Me.rdoWS.TabIndex = 20
        Me.rdoWS.TabStop = True
        Me.rdoWS.Text = "Weiss-Schwarz"
        Me.rdoWS.UseVisualStyleBackColor = True
        '
        'rdoZufall
        '
        Me.rdoZufall.AutoSize = True
        Me.rdoZufall.Location = New System.Drawing.Point(271, 248)
        Me.rdoZufall.Name = "rdoZufall"
        Me.rdoZufall.Size = New System.Drawing.Size(146, 45)
        Me.rdoZufall.TabIndex = 21
        Me.rdoZufall.TabStop = True
        Me.rdoZufall.Text = "Zufällig"
        Me.rdoZufall.UseVisualStyleBackColor = True
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(652, 26)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(197, 52)
        Me.btnDefaults.TabIndex = 22
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.rdoZufall)
        Me.Controls.Add(Me.rdoWS)
        Me.Controls.Add(Me.rdoFarbe)
        Me.Controls.Add(Me.lblNModus)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNShaderName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsShader"
        Me.Size = New System.Drawing.Size(890, 1020)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNShaderName As Windows.Forms.Label
    Friend WithEvents lblNModus As Windows.Forms.Label
    Friend WithEvents rdoFarbe As RadioButton
    Friend WithEvents rdoWS As RadioButton
    Friend WithEvents rdoZufall As RadioButton
    Friend WithEvents btnDefaults As Button
End Class
