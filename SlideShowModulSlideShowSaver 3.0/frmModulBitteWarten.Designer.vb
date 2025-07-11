<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmModulBitteWarten
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
        Me.lblInitializing = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblInitializing
        '
        Me.lblInitializing.AutoSize = True
        Me.lblInitializing.BackColor = System.Drawing.Color.Transparent
        Me.lblInitializing.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Bold)
        Me.lblInitializing.ForeColor = System.Drawing.Color.Snow
        Me.lblInitializing.Location = New System.Drawing.Point(-68, 189)
        Me.lblInitializing.Name = "lblInitializing"
        Me.lblInitializing.Size = New System.Drawing.Size(937, 72)
        Me.lblInitializing.TabIndex = 2
        Me.lblInitializing.Text = "SlideShowSaver 3.0 wird gestartet..."
        '
        'frmModulBitteWarten
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(14.0!, 29.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.lblInitializing)
        Me.Name = "frmModulBitteWarten"
        Me.Text = "frmModulBitteWarten"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblInitializing As Windows.Forms.Label
End Class
