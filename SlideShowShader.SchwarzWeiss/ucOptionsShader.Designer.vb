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
        Me.lblNlblShaderName = New System.Windows.Forms.Label()
        Me.lblNoOptions = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblShadername.Location = New System.Drawing.Point(264, 26)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(219, 41)
        Me.lblShadername.TabIndex = 17
        Me.lblShadername.Text = "Schwarz-Weiß"
        '
        'lblNlblShaderName
        '
        Me.lblNlblShaderName.AutoSize = True
        Me.lblNlblShaderName.Location = New System.Drawing.Point(34, 26)
        Me.lblNlblShaderName.Name = "lblNlblShaderName"
        Me.lblNlblShaderName.Size = New System.Drawing.Size(110, 41)
        Me.lblNlblShaderName.TabIndex = 16
        Me.lblNlblShaderName.Text = "Shader"
        '
        'lblNoOptions
        '
        Me.lblNoOptions.AutoSize = True
        Me.lblNoOptions.Location = New System.Drawing.Point(34, 144)
        Me.lblNoOptions.Name = "lblNoOptions"
        Me.lblNoOptions.Size = New System.Drawing.Size(676, 41)
        Me.lblNoOptions.TabIndex = 18
        Me.lblNoOptions.Text = "Dieser Shader bietet keine einstellbaren Optionen"
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblNoOptions)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNlblShaderName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsShader"
        Me.Size = New System.Drawing.Size(890, 1020)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNlblShaderName As Windows.Forms.Label
    Friend WithEvents lblNoOptions As Windows.Forms.Label
End Class