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
        Me.lblNcmbBildauswahl = New System.Windows.Forms.Label()
        Me.cmbBildauswahl = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.trbAnzeigedauer = New System.Windows.Forms.TrackBar()
        Me.lblAnzeigedauer = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.clbTransitions = New System.Windows.Forms.CheckedListBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cmbEffektauswahl = New System.Windows.Forms.ComboBox()
        Me.chkBildinformationen = New System.Windows.Forms.CheckBox()
        Me.clbShader = New System.Windows.Forms.CheckedListBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cmbShaderauswahl = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblModulname = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        CType(Me.trbAnzeigedauer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblNcmbBildauswahl
        '
        Me.lblNcmbBildauswahl.AutoSize = True
        Me.lblNcmbBildauswahl.Location = New System.Drawing.Point(22, 100)
        Me.lblNcmbBildauswahl.Name = "lblNcmbBildauswahl"
        Me.lblNcmbBildauswahl.Size = New System.Drawing.Size(173, 41)
        Me.lblNcmbBildauswahl.TabIndex = 0
        Me.lblNcmbBildauswahl.Text = "Bildauswahl"
        '
        'cmbBildauswahl
        '
        Me.cmbBildauswahl.FormattingEnabled = True
        Me.cmbBildauswahl.Items.AddRange(New Object() {"Zufallsbild", "Zufallsverzeichnis"})
        Me.cmbBildauswahl.Location = New System.Drawing.Point(500, 97)
        Me.cmbBildauswahl.Name = "cmbBildauswahl"
        Me.cmbBildauswahl.Size = New System.Drawing.Size(361, 49)
        Me.cmbBildauswahl.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(22, 161)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(201, 41)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Anzeigedauer"
        '
        'trbAnzeigedauer
        '
        Me.trbAnzeigedauer.Location = New System.Drawing.Point(39, 205)
        Me.trbAnzeigedauer.Maximum = 120
        Me.trbAnzeigedauer.Minimum = 5
        Me.trbAnzeigedauer.Name = "trbAnzeigedauer"
        Me.trbAnzeigedauer.Size = New System.Drawing.Size(687, 101)
        Me.trbAnzeigedauer.TabIndex = 3
        Me.trbAnzeigedauer.Value = 20
        '
        'lblAnzeigedauer
        '
        Me.lblAnzeigedauer.AutoSize = True
        Me.lblAnzeigedauer.Location = New System.Drawing.Point(732, 205)
        Me.lblAnzeigedauer.MaximumSize = New System.Drawing.Size(129, 41)
        Me.lblAnzeigedauer.MinimumSize = New System.Drawing.Size(129, 41)
        Me.lblAnzeigedauer.Name = "lblAnzeigedauer"
        Me.lblAnzeigedauer.Size = New System.Drawing.Size(129, 41)
        Me.lblAnzeigedauer.TabIndex = 4
        Me.lblAnzeigedauer.Text = "20 s"
        Me.lblAnzeigedauer.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(22, 299)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(382, 41)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Übergangseffekte für Bilder"
        '
        'clbTransitions
        '
        Me.clbTransitions.FormattingEnabled = True
        Me.clbTransitions.Items.AddRange(New Object() {"Direkter Übergang (Cut)"})
        Me.clbTransitions.Location = New System.Drawing.Point(29, 343)
        Me.clbTransitions.Name = "clbTransitions"
        Me.clbTransitions.Size = New System.Drawing.Size(832, 180)
        Me.clbTransitions.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(29, 529)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(243, 41)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Effektreihenfolge"
        '
        'cmbEffektauswahl
        '
        Me.cmbEffektauswahl.FormattingEnabled = True
        Me.cmbEffektauswahl.Items.AddRange(New Object() {"Zufällig bei Start", "Zufällig bei Bildwechsel", "In Reihenfolge bei Start", "In Reihenfolge bei Bildwechsel"})
        Me.cmbEffektauswahl.Location = New System.Drawing.Point(500, 529)
        Me.cmbEffektauswahl.Name = "cmbEffektauswahl"
        Me.cmbEffektauswahl.Size = New System.Drawing.Size(361, 49)
        Me.cmbEffektauswahl.TabIndex = 8
        '
        'chkBildinformationen
        '
        Me.chkBildinformationen.AutoSize = True
        Me.chkBildinformationen.Location = New System.Drawing.Point(29, 948)
        Me.chkBildinformationen.Name = "chkBildinformationen"
        Me.chkBildinformationen.Size = New System.Drawing.Size(414, 45)
        Me.chkBildinformationen.TabIndex = 9
        Me.chkBildinformationen.Text = "Bildinformationen anzeigen"
        Me.chkBildinformationen.UseVisualStyleBackColor = True
        '
        'clbShader
        '
        Me.clbShader.FormattingEnabled = True
        Me.clbShader.Location = New System.Drawing.Point(29, 648)
        Me.clbShader.Name = "clbShader"
        Me.clbShader.Size = New System.Drawing.Size(832, 224)
        Me.clbShader.TabIndex = 11
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(22, 604)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(110, 41)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Shader"
        '
        'cmbShaderauswahl
        '
        Me.cmbShaderauswahl.FormattingEnabled = True
        Me.cmbShaderauswahl.Items.AddRange(New Object() {"Zufällig bei Start", "Zufällig bei Bildwechsel", "In Reihenfolge bei Start", "In Reihenfolge bei Bildwechsel"})
        Me.cmbShaderauswahl.Location = New System.Drawing.Point(500, 878)
        Me.cmbShaderauswahl.Name = "cmbShaderauswahl"
        Me.cmbShaderauswahl.Size = New System.Drawing.Size(361, 49)
        Me.cmbShaderauswahl.TabIndex = 13
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(29, 878)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(261, 41)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "Shaderreihenfolge"
        '
        'lblModulname
        '
        Me.lblModulname.AutoSize = True
        Me.lblModulname.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblModulname.Location = New System.Drawing.Point(252, 29)
        Me.lblModulname.Name = "lblModulname"
        Me.lblModulname.Size = New System.Drawing.Size(292, 41)
        Me.lblModulname.TabIndex = 15
        Me.lblModulname.Text = "SlideShowSaver 3.0"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(22, 29)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(105, 41)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Modul"
        '
        'ucOptionsModul
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblModulname)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.cmbShaderauswahl)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.clbShader)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.chkBildinformationen)
        Me.Controls.Add(Me.cmbEffektauswahl)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.clbTransitions)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblAnzeigedauer)
        Me.Controls.Add(Me.trbAnzeigedauer)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmbBildauswahl)
        Me.Controls.Add(Me.lblNcmbBildauswahl)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsModul"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trbAnzeigedauer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblNcmbBildauswahl As Windows.Forms.Label
    Friend WithEvents cmbBildauswahl As Windows.Forms.ComboBox
    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents trbAnzeigedauer As Windows.Forms.TrackBar
    Friend WithEvents lblAnzeigedauer As Windows.Forms.Label
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents clbTransitions As Windows.Forms.CheckedListBox
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents cmbEffektauswahl As Windows.Forms.ComboBox
    Friend WithEvents chkBildinformationen As Windows.Forms.CheckBox
    Friend WithEvents clbShader As Windows.Forms.CheckedListBox
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents cmbShaderauswahl As Windows.Forms.ComboBox
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents lblModulname As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
End Class
