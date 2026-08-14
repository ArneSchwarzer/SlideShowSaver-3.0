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
        Me.btnDefaults = New System.Windows.Forms.Button()
        Me.chkBNDOverlay = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.AutoSize = True
        Me.lblShadername.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblShadername.Location = New System.Drawing.Point(264, 26)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(168, 41)
        Me.lblShadername.TabIndex = 17
        Me.lblShadername.Tag = "langKey=lblShadername"
        Me.lblShadername.Text = "Nachtsicht"
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
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(667, 20)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(181, 52)
        Me.btnDefaults.TabIndex = 23
        Me.btnDefaults.Tag = "langKey=btnDefaults"
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'chkBNDOverlay
        '
        Me.chkBNDOverlay.AutoSize = True
        Me.chkBNDOverlay.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.chkBNDOverlay.Location = New System.Drawing.Point(41, 130)
        Me.chkBNDOverlay.Name = "chkBNDOverlay"
        Me.chkBNDOverlay.Size = New System.Drawing.Size(353, 45)
        Me.chkBNDOverlay.TabIndex = 29
        Me.chkBNDOverlay.Tag = "langKey=chkBNDOverlay"
        Me.chkBNDOverlay.Text = "BND Overlay aktivieren"
        Me.chkBNDOverlay.UseVisualStyleBackColor = True
        '
        'ucOptionsShader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.chkBNDOverlay)
        Me.Controls.Add(Me.btnDefaults)
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
    Friend WithEvents btnDefaults As Windows.Forms.Button
    Friend WithEvents chkBNDOverlay As Windows.Forms.CheckBox
End Class
