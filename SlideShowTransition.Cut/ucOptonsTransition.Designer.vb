<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptonsTransition
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
        Me.lblNoOptions = New System.Windows.Forms.Label()
Me.lblNoOptions.Tag = "langKey=lblNoOptions"
        Me.lblTransitonName = New System.Windows.Forms.Label()
Me.lblTransitonName.Tag = "langKey=lblTransitonName"
        Me.lblNlblTransitionName = New System.Windows.Forms.Label()
Me.lblNlblTransitionName.Tag = "langKey=lblNlblTransitionName"
        Me.SuspendLayout()
        '
        'lblNoOptions
        '
        Me.lblNoOptions.AutoSize = True
        Me.lblNoOptions.Location = New System.Drawing.Point(34, 147)
        Me.lblNoOptions.Name = "lblNoOptions"
        Me.lblNoOptions.Size = New System.Drawing.Size(717, 41)
        Me.lblNoOptions.TabIndex = 21
        Me.lblNoOptions.Text = "Dieser Übergang bietet keine einstellbaren Optionen"
        '
        'lblTransitonName
        '
        Me.lblTransitonName.AutoSize = True
        Me.lblTransitonName.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTransitonName.Location = New System.Drawing.Point(264, 29)
        Me.lblTransitonName.Name = "lblTransitonName"
        Me.lblTransitonName.Size = New System.Drawing.Size(283, 41)
        Me.lblTransitonName.TabIndex = 20
        Me.lblTransitonName.Text = "Direkter Übergang"
        '
        'lblNlblTransitionName
        '
        Me.lblNlblTransitionName.AutoSize = True
        Me.lblNlblTransitionName.Location = New System.Drawing.Point(34, 29)
        Me.lblNlblTransitionName.Name = "lblNlblTransitionName"
        Me.lblNlblTransitionName.Size = New System.Drawing.Size(151, 41)
        Me.lblNlblTransitionName.TabIndex = 19
        Me.lblNlblTransitionName.Text = "Übergang"
        '
        'ucOptonsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblNoOptions)
        Me.Controls.Add(Me.lblTransitonName)
        Me.Controls.Add(Me.lblNlblTransitionName)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "ucOptonsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblNoOptions As Windows.Forms.Label
    Friend WithEvents lblTransitonName As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionName As Windows.Forms.Label
End Class
