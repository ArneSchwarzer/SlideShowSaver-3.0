<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSaverMain
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
        Me.components = New System.ComponentModel.Container()
        Me.tmrMain = New System.Windows.Forms.Timer(Me.components)
        Me.lblMCP = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'tmrMain
        '
        Me.tmrMain.Interval = 900000
        '
        'lblMCP
        '
        Me.lblMCP.AutoSize = True
        Me.lblMCP.Font = New System.Drawing.Font("Segoe UI", 20.0!, System.Drawing.FontStyle.Bold)
        Me.lblMCP.ForeColor = System.Drawing.Color.Red
        Me.lblMCP.Location = New System.Drawing.Point(16, -11)
        Me.lblMCP.Name = "lblMCP"
        Me.lblMCP.Size = New System.Drawing.Size(1007, 81)
        Me.lblMCP.TabIndex = 0
        Me.lblMCP.Text = "Master Control Program running..."
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.Red
        Me.Label1.Location = New System.Drawing.Point(31, 70)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(547, 48)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "SlideShowSaver 3.0 Framework"
        '
        'frmSaverMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(971, 636)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblMCP)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmSaverMain"
        Me.Text = "SlideShowSaverMain"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tmrMain As Timer
    Friend WithEvents lblMCP As Label
    Friend WithEvents Label1 As Label
End Class
