<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucOptionsModul
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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
        Me.lblModulname = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btnHighlighttextListeLöschen = New System.Windows.Forms.Button()
        Me.btnHighlighttextLöschen = New System.Windows.Forms.Button()
        Me.btnHighlighttextHinzufügen = New System.Windows.Forms.Button()
        Me.lstHiglightTexte = New System.Windows.Forms.ListBox()
        Me.lblSzenendauer = New System.Windows.Forms.Label()
        Me.trbSzenendauer = New System.Windows.Forms.TrackBar()
        Me.lblNtrbSzenendauer = New System.Windows.Forms.Label()
        Me.lblNclbHighlighttexte = New System.Windows.Forms.Label()
        CType(Me.trbSzenendauer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblModulname
        '
        Me.lblModulname.AutoSize = True
        Me.lblModulname.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblModulname.Location = New System.Drawing.Point(223, 65)
        Me.lblModulname.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblModulname.Name = "lblModulname"
        Me.lblModulname.Size = New System.Drawing.Size(113, 41)
        Me.lblModulname.TabIndex = 17
        Me.lblModulname.Text = "Matrix"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(33, 65)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(105, 41)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Modul"
        '
        'btnHighlighttextListeLöschen
        '
        Me.btnHighlighttextListeLöschen.BackgroundImage = Global.Modul_Matrix.My.Resources.Resources.Trashbin
        Me.btnHighlighttextListeLöschen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnHighlighttextListeLöschen.Location = New System.Drawing.Point(805, 365)
        Me.btnHighlighttextListeLöschen.Margin = New System.Windows.Forms.Padding(4)
        Me.btnHighlighttextListeLöschen.Name = "btnHighlighttextListeLöschen"
        Me.btnHighlighttextListeLöschen.Size = New System.Drawing.Size(61, 71)
        Me.btnHighlighttextListeLöschen.TabIndex = 21
        Me.btnHighlighttextListeLöschen.UseVisualStyleBackColor = True
        '
        'btnHighlighttextLöschen
        '
        Me.btnHighlighttextLöschen.Location = New System.Drawing.Point(805, 272)
        Me.btnHighlighttextLöschen.Margin = New System.Windows.Forms.Padding(4)
        Me.btnHighlighttextLöschen.Name = "btnHighlighttextLöschen"
        Me.btnHighlighttextLöschen.Size = New System.Drawing.Size(61, 71)
        Me.btnHighlighttextLöschen.TabIndex = 20
        Me.btnHighlighttextLöschen.Text = "-"
        Me.btnHighlighttextLöschen.UseVisualStyleBackColor = True
        '
        'btnHighlighttextHinzufügen
        '
        Me.btnHighlighttextHinzufügen.Location = New System.Drawing.Point(805, 179)
        Me.btnHighlighttextHinzufügen.Margin = New System.Windows.Forms.Padding(4)
        Me.btnHighlighttextHinzufügen.Name = "btnHighlighttextHinzufügen"
        Me.btnHighlighttextHinzufügen.Size = New System.Drawing.Size(61, 71)
        Me.btnHighlighttextHinzufügen.TabIndex = 19
        Me.btnHighlighttextHinzufügen.Text = "+"
        Me.btnHighlighttextHinzufügen.UseVisualStyleBackColor = True
        '
        'lstHiglightTexte
        '
        Me.lstHiglightTexte.FormattingEnabled = True
        Me.lstHiglightTexte.ItemHeight = 41
        Me.lstHiglightTexte.Location = New System.Drawing.Point(40, 179)
        Me.lstHiglightTexte.Margin = New System.Windows.Forms.Padding(4)
        Me.lstHiglightTexte.Name = "lstHiglightTexte"
        Me.lstHiglightTexte.Size = New System.Drawing.Size(744, 373)
        Me.lstHiglightTexte.TabIndex = 18
        '
        'lblSzenendauer
        '
        Me.lblSzenendauer.AutoSize = True
        Me.lblSzenendauer.Location = New System.Drawing.Point(709, 651)
        Me.lblSzenendauer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblSzenendauer.MaximumSize = New System.Drawing.Size(157, 58)
        Me.lblSzenendauer.MinimumSize = New System.Drawing.Size(157, 58)
        Me.lblSzenendauer.Name = "lblSzenendauer"
        Me.lblSzenendauer.Size = New System.Drawing.Size(157, 58)
        Me.lblSzenendauer.TabIndex = 24
        Me.lblSzenendauer.Text = "20 s"
        Me.lblSzenendauer.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'trbSzenendauer
        '
        Me.trbSzenendauer.Location = New System.Drawing.Point(40, 651)
        Me.trbSzenendauer.Margin = New System.Windows.Forms.Padding(4)
        Me.trbSzenendauer.Maximum = 120
        Me.trbSzenendauer.Minimum = 5
        Me.trbSzenendauer.Name = "trbSzenendauer"
        Me.trbSzenendauer.Size = New System.Drawing.Size(669, 101)
        Me.trbSzenendauer.TabIndex = 23
        Me.trbSzenendauer.Value = 20
        '
        'lblNtrbSzenendauer
        '
        Me.lblNtrbSzenendauer.AutoSize = True
        Me.lblNtrbSzenendauer.Location = New System.Drawing.Point(33, 591)
        Me.lblNtrbSzenendauer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblNtrbSzenendauer.Name = "lblNtrbSzenendauer"
        Me.lblNtrbSzenendauer.Size = New System.Drawing.Size(189, 41)
        Me.lblNtrbSzenendauer.TabIndex = 22
        Me.lblNtrbSzenendauer.Text = "Szenendauer"
        '
        'lblNclbHighlighttexte
        '
        Me.lblNclbHighlighttexte.AutoSize = True
        Me.lblNclbHighlighttexte.Location = New System.Drawing.Point(33, 134)
        Me.lblNclbHighlighttexte.Name = "lblNclbHighlighttexte"
        Me.lblNclbHighlighttexte.Size = New System.Drawing.Size(289, 41)
        Me.lblNclbHighlighttexte.TabIndex = 25
        Me.lblNclbHighlighttexte.Text = "Hervorhebungstexte"
        '
        'ucOptionsModul
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblNclbHighlighttexte)
        Me.Controls.Add(Me.lblSzenendauer)
        Me.Controls.Add(Me.trbSzenendauer)
        Me.Controls.Add(Me.lblNtrbSzenendauer)
        Me.Controls.Add(Me.btnHighlighttextListeLöschen)
        Me.Controls.Add(Me.btnHighlighttextLöschen)
        Me.Controls.Add(Me.btnHighlighttextHinzufügen)
        Me.Controls.Add(Me.lstHiglightTexte)
        Me.Controls.Add(Me.lblModulname)
        Me.Controls.Add(Me.Label6)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsModul"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trbSzenendauer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblModulname As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents btnHighlighttextListeLöschen As Windows.Forms.Button
    Friend WithEvents btnHighlighttextLöschen As Windows.Forms.Button
    Friend WithEvents btnHighlighttextHinzufügen As Windows.Forms.Button
    Friend WithEvents lstHiglightTexte As Windows.Forms.ListBox
    Friend WithEvents lblSzenendauer As Windows.Forms.Label
    Friend WithEvents trbSzenendauer As Windows.Forms.TrackBar
    Friend WithEvents lblNtrbSzenendauer As Windows.Forms.Label
    Friend WithEvents lblNclbHighlighttexte As Windows.Forms.Label
End Class
