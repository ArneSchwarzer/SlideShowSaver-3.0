<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmModulMain
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
        Me.tmrModul = New System.Windows.Forms.Timer(Me.components)
        Me.picBildAnzeige = New System.Windows.Forms.PictureBox()
        CType(Me.picBildAnzeige, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tmrModul
        '
        Me.tmrModul.Interval = 20000
        '
        'picBildAnzeige
        '
        Me.picBildAnzeige.Location = New System.Drawing.Point(578, 147)
        Me.picBildAnzeige.Name = "picBildAnzeige"
        Me.picBildAnzeige.Size = New System.Drawing.Size(1059, 671)
        Me.picBildAnzeige.TabIndex = 0
        Me.picBildAnzeige.TabStop = False
        '
        'frmModulMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(1967, 1210)
        Me.Controls.Add(Me.picBildAnzeige)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "frmModulMain"
        Me.Text = "ModulMain"
        CType(Me.picBildAnzeige, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tmrModul As Windows.Forms.Timer
    Friend WithEvents picBildAnzeige As Windows.Forms.PictureBox
End Class
