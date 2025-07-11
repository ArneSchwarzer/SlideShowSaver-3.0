<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsModul
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
        Me.lblModulname = New System.Windows.Forms.Label()
        Me.lblNModul = New System.Windows.Forms.Label()
        Me.cmbGradient = New System.Windows.Forms.ComboBox()
        Me.lblNcmbGradient = New System.Windows.Forms.Label()
        Me.chkGradientAnimieren = New System.Windows.Forms.CheckBox()
        Me.chkKoordinatenAnzeigen = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'lblModulname
        '
        Me.lblModulname.AutoSize = True
        Me.lblModulname.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblModulname.Location = New System.Drawing.Point(257, 30)
        Me.lblModulname.Name = "lblModulname"
        Me.lblModulname.Size = New System.Drawing.Size(186, 41)
        Me.lblModulname.TabIndex = 17
        Me.lblModulname.Tag = "langKey=lblModulname"
        Me.lblModulname.Text = "Mandelbrot"
        '
        'lblNModul
        '
        Me.lblNModul.AutoSize = True
        Me.lblNModul.Location = New System.Drawing.Point(29, 30)
        Me.lblNModul.Name = "lblNModul"
        Me.lblNModul.Size = New System.Drawing.Size(105, 41)
        Me.lblNModul.TabIndex = 16
        Me.lblNModul.Tag = "langKey=lblNModul"
        Me.lblNModul.Text = "Modul"
        '
        'cmbGradient
        '
        Me.cmbGradient.FormattingEnabled = True
        Me.cmbGradient.Items.AddRange(New Object() {"Regenbogen", "Zebra", "Joker", "Wakanda", "Weihnachten"})
        Me.cmbGradient.Location = New System.Drawing.Point(264, 110)
        Me.cmbGradient.Name = "cmbGradient"
        Me.cmbGradient.Size = New System.Drawing.Size(604, 49)
        Me.cmbGradient.TabIndex = 19
        '
        'lblNcmbGradient
        '
        Me.lblNcmbGradient.AutoSize = True
        Me.lblNcmbGradient.Location = New System.Drawing.Point(29, 113)
        Me.lblNcmbGradient.Name = "lblNcmbGradient"
        Me.lblNcmbGradient.Size = New System.Drawing.Size(132, 41)
        Me.lblNcmbGradient.TabIndex = 18
        Me.lblNcmbGradient.Tag = "langKey=lblNcmbGradient"
        Me.lblNcmbGradient.Text = "Gradient"
        '
        'chkGradientAnimieren
        '
        Me.chkGradientAnimieren.AutoSize = True
        Me.chkGradientAnimieren.Location = New System.Drawing.Point(36, 165)
        Me.chkGradientAnimieren.Name = "chkGradientAnimieren"
        Me.chkGradientAnimieren.Size = New System.Drawing.Size(303, 45)
        Me.chkGradientAnimieren.TabIndex = 20
        Me.chkGradientAnimieren.Tag = "langKey=chkGradientAnimieren"
        Me.chkGradientAnimieren.Text = "Gradient animieren"
        Me.chkGradientAnimieren.UseVisualStyleBackColor = True
        '
        'chkKoordinatenAnzeigen
        '
        Me.chkKoordinatenAnzeigen.AutoSize = True
        Me.chkKoordinatenAnzeigen.Location = New System.Drawing.Point(36, 245)
        Me.chkKoordinatenAnzeigen.Name = "chkKoordinatenAnzeigen"
        Me.chkKoordinatenAnzeigen.Size = New System.Drawing.Size(340, 45)
        Me.chkKoordinatenAnzeigen.TabIndex = 21
        Me.chkKoordinatenAnzeigen.Tag = "langKey=chkKoordinatenAnzeigen"
        Me.chkKoordinatenAnzeigen.Text = "Koordinaten anzeigen"
        Me.chkKoordinatenAnzeigen.UseVisualStyleBackColor = True
        '
        'ucOptionsModul
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.chkKoordinatenAnzeigen)
        Me.Controls.Add(Me.chkGradientAnimieren)
        Me.Controls.Add(Me.cmbGradient)
        Me.Controls.Add(Me.lblNcmbGradient)
        Me.Controls.Add(Me.lblModulname)
        Me.Controls.Add(Me.lblNModul)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsModul"
        Me.Size = New System.Drawing.Size(890, 1020)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblModulname As Windows.Forms.Label
    Friend WithEvents lblNModul As Windows.Forms.Label
    Friend WithEvents cmbGradient As Windows.Forms.ComboBox
    Friend WithEvents lblNcmbGradient As Windows.Forms.Label
    Friend WithEvents chkGradientAnimieren As Windows.Forms.CheckBox
    Friend WithEvents chkKoordinatenAnzeigen As Windows.Forms.CheckBox
End Class