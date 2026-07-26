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
        Me.lblModul = New System.Windows.Forms.Label()
        Me.lblNModul = New System.Windows.Forms.Label()
        Me.lblNcmbGradient = New System.Windows.Forms.Label()
        Me.chkGradientAnimieren = New System.Windows.Forms.CheckBox()
        Me.chkKoordinatenAnzeigen = New System.Windows.Forms.CheckBox()
        Me.clbGradienten = New System.Windows.Forms.CheckedListBox()
        Me.trkZoomgeschwindigkeit = New System.Windows.Forms.TrackBar()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.chkRotation = New System.Windows.Forms.CheckBox()
        CType(Me.trkZoomgeschwindigkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblModul
        '
        Me.lblModul.AutoSize = True
        Me.lblModul.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblModul.Location = New System.Drawing.Point(257, 30)
        Me.lblModul.Name = "lblModul"
        Me.lblModul.Size = New System.Drawing.Size(186, 41)
        Me.lblModul.TabIndex = 17
        Me.lblModul.Tag = "langKey=lblModulname"
        Me.lblModul.Text = "Mandelbrot"
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
        'lblNcmbGradient
        '
        Me.lblNcmbGradient.AutoSize = True
        Me.lblNcmbGradient.Location = New System.Drawing.Point(29, 113)
        Me.lblNcmbGradient.Name = "lblNcmbGradient"
        Me.lblNcmbGradient.Size = New System.Drawing.Size(165, 41)
        Me.lblNcmbGradient.TabIndex = 18
        Me.lblNcmbGradient.Tag = "langKey=lblNcmbGradient"
        Me.lblNcmbGradient.Text = "Gradienten"
        '
        'chkGradientAnimieren
        '
        Me.chkGradientAnimieren.AutoSize = True
        Me.chkGradientAnimieren.Location = New System.Drawing.Point(36, 357)
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
        Me.chkKoordinatenAnzeigen.Location = New System.Drawing.Point(36, 642)
        Me.chkKoordinatenAnzeigen.Name = "chkKoordinatenAnzeigen"
        Me.chkKoordinatenAnzeigen.Size = New System.Drawing.Size(340, 45)
        Me.chkKoordinatenAnzeigen.TabIndex = 21
        Me.chkKoordinatenAnzeigen.Tag = "langKey=chkKoordinatenAnzeigen"
        Me.chkKoordinatenAnzeigen.Text = "Koordinaten anzeigen"
        Me.chkKoordinatenAnzeigen.UseVisualStyleBackColor = True
        '
        'clbGradienten
        '
        Me.clbGradienten.FormattingEnabled = True
        Me.clbGradienten.Items.AddRange(New Object() {"Joker", "Regenbogen", "Wakanda", "Weihnachten", "Zebra"})
        Me.clbGradienten.Location = New System.Drawing.Point(264, 113)
        Me.clbGradienten.Name = "clbGradienten"
        Me.clbGradienten.Size = New System.Drawing.Size(575, 224)
        Me.clbGradienten.TabIndex = 22
        '
        'trkZoomgeschwindigkeit
        '
        Me.trkZoomgeschwindigkeit.Location = New System.Drawing.Point(257, 446)
        Me.trkZoomgeschwindigkeit.Maximum = 100
        Me.trkZoomgeschwindigkeit.Minimum = 1
        Me.trkZoomgeschwindigkeit.Name = "trkZoomgeschwindigkeit"
        Me.trkZoomgeschwindigkeit.Size = New System.Drawing.Size(575, 101)
        Me.trkZoomgeschwindigkeit.TabIndex = 24
        Me.trkZoomgeschwindigkeit.Value = 1
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(29, 446)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(236, 41)
        Me.Label2.TabIndex = 26
        Me.Label2.Text = "Geschwindigkeit"
        '
        'chkRotation
        '
        Me.chkRotation.AutoSize = True
        Me.chkRotation.Location = New System.Drawing.Point(36, 578)
        Me.chkRotation.Name = "chkRotation"
        Me.chkRotation.Size = New System.Drawing.Size(162, 45)
        Me.chkRotation.TabIndex = 27
        Me.chkRotation.Text = "Rotation"
        Me.chkRotation.UseVisualStyleBackColor = True
        '
        'ucOptionsModul
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.chkRotation)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.trkZoomgeschwindigkeit)
        Me.Controls.Add(Me.clbGradienten)
        Me.Controls.Add(Me.chkKoordinatenAnzeigen)
        Me.Controls.Add(Me.chkGradientAnimieren)
        Me.Controls.Add(Me.lblNcmbGradient)
        Me.Controls.Add(Me.lblModul)
        Me.Controls.Add(Me.lblNModul)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsModul"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkZoomgeschwindigkeit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblModul As Windows.Forms.Label
    Friend WithEvents lblNModul As Windows.Forms.Label
    Friend WithEvents lblNcmbGradient As Windows.Forms.Label
    Friend WithEvents chkGradientAnimieren As Windows.Forms.CheckBox
    Friend WithEvents chkKoordinatenAnzeigen As Windows.Forms.CheckBox
    Friend WithEvents clbGradienten As Forms.CheckedListBox
    Friend WithEvents trkZoomgeschwindigkeit As Forms.TrackBar
    Friend WithEvents Label2 As Forms.Label
    Friend WithEvents chkRotation As Forms.CheckBox
End Class