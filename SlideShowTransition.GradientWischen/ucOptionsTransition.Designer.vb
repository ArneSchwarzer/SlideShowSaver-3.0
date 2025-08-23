<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucOptionsTransition
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
        Me.lblShadername = New System.Windows.Forms.Label()
Me.lblShadername.Tag = "langKey=lblShadername"
        Me.lblNlblTransitionname = New System.Windows.Forms.Label()
Me.lblNlblTransitionname.Tag = "langKey=lblNlblTransitionname"
        Me.lblNRichtungen = New System.Windows.Forms.Label()
Me.lblNRichtungen.Tag = "langKey=lblNRichtungen"
        Me.lblKeineRichtung = New System.Windows.Forms.Label()
Me.lblKeineRichtung.Tag = "langKey=lblKeineRichtung"
        Me.lblNtrbGeschwindigkeit = New System.Windows.Forms.Label()
Me.lblNtrbGeschwindigkeit.Tag = "langKey=lblNtrbGeschwindigkeit"
        Me.trkGeschwindigkeit = New System.Windows.Forms.TrackBar()
        Me.lblGeschwindigkeit = New System.Windows.Forms.Label()
Me.lblGeschwindigkeit.Tag = "langKey=lblGeschwindigkeit"
        Me.tbtNW = New MyControlsLibrary.ToggleButton()
        Me.tbtN = New MyControlsLibrary.ToggleButton()
        Me.tbtNO = New MyControlsLibrary.ToggleButton()
        Me.tbtW = New MyControlsLibrary.ToggleButton()
        Me.tbtO = New MyControlsLibrary.ToggleButton()
        Me.tbtSW = New MyControlsLibrary.ToggleButton()
        Me.tbtS = New MyControlsLibrary.ToggleButton()
        Me.tbtSO = New MyControlsLibrary.ToggleButton()
        Me.lblTransitionsname = New System.Windows.Forms.Label()
Me.lblTransitionsname.Tag = "langKey=lblTransitionsname"
        Me.tbtZIn = New MyControlsLibrary.ToggleButton()
        Me.tbtZOut = New MyControlsLibrary.ToggleButton()
        Me.trkBreite = New System.Windows.Forms.TrackBar()
        Me.lblNBreite = New System.Windows.Forms.Label()
Me.lblNBreite.Tag = "langKey=lblNBreite"
        Me.lblBreiteProzent = New System.Windows.Forms.Label()
Me.lblBreiteProzent.Tag = "langKey=lblBreiteProzent"
        Me.btnDefaults = New System.Windows.Forms.Button()
Me.btnDefaults.Tag = "langKey=btnDefaults"
        CType(Me.trkGeschwindigkeit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trkBreite, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblShadername
        '
        Me.lblShadername.Location = New System.Drawing.Point(0, 0)
        Me.lblShadername.Name = "lblShadername"
        Me.lblShadername.Size = New System.Drawing.Size(100, 23)
        Me.lblShadername.TabIndex = 44
        '
        'lblNlblTransitionname
        '
        Me.lblNlblTransitionname.AutoSize = True
        Me.lblNlblTransitionname.Location = New System.Drawing.Point(34, 26)
        Me.lblNlblTransitionname.Name = "lblNlblTransitionname"
        Me.lblNlblTransitionname.Size = New System.Drawing.Size(151, 41)
        Me.lblNlblTransitionname.TabIndex = 16
        Me.lblNlblTransitionname.Text = "Übergang"
        '
        'lblNRichtungen
        '
        Me.lblNRichtungen.AutoSize = True
        Me.lblNRichtungen.Location = New System.Drawing.Point(34, 144)
        Me.lblNRichtungen.Name = "lblNRichtungen"
        Me.lblNRichtungen.Size = New System.Drawing.Size(169, 41)
        Me.lblNRichtungen.TabIndex = 18
        Me.lblNRichtungen.Text = "Richtungen"
        '
        'lblKeineRichtung
        '
        Me.lblKeineRichtung.AutoSize = True
        Me.lblKeineRichtung.Location = New System.Drawing.Point(325, 485)
        Me.lblKeineRichtung.MaximumSize = New System.Drawing.Size(600, 0)
        Me.lblKeineRichtung.Name = "lblKeineRichtung"
        Me.lblKeineRichtung.Size = New System.Drawing.Size(527, 82)
        Me.lblKeineRichtung.TabIndex = 27
        Me.lblKeineRichtung.Text = "Keine Richtung ausgewählt, verwende zufällige Richtung"
        '
        'lblNtrbGeschwindigkeit
        '
        Me.lblNtrbGeschwindigkeit.AutoSize = True
        Me.lblNtrbGeschwindigkeit.Location = New System.Drawing.Point(41, 660)
        Me.lblNtrbGeschwindigkeit.Name = "lblNtrbGeschwindigkeit"
        Me.lblNtrbGeschwindigkeit.Size = New System.Drawing.Size(236, 41)
        Me.lblNtrbGeschwindigkeit.TabIndex = 28
        Me.lblNtrbGeschwindigkeit.Text = "Geschwindigkeit"
        '
        'trkGeschwindigkeit
        '
        Me.trkGeschwindigkeit.AutoSize = False
        Me.trkGeschwindigkeit.Location = New System.Drawing.Point(332, 660)
        Me.trkGeschwindigkeit.Maximum = 30
        Me.trkGeschwindigkeit.Minimum = 1
        Me.trkGeschwindigkeit.Name = "trkGeschwindigkeit"
        Me.trkGeschwindigkeit.Size = New System.Drawing.Size(450, 57)
        Me.trkGeschwindigkeit.TabIndex = 29
        Me.trkGeschwindigkeit.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkGeschwindigkeit.Value = 15
        '
        'lblGeschwindigkeit
        '
        Me.lblGeschwindigkeit.AutoSize = True
        Me.lblGeschwindigkeit.Location = New System.Drawing.Point(779, 660)
        Me.lblGeschwindigkeit.MinimumSize = New System.Drawing.Size(99, 0)
        Me.lblGeschwindigkeit.Name = "lblGeschwindigkeit"
        Me.lblGeschwindigkeit.Size = New System.Drawing.Size(99, 41)
        Me.lblGeschwindigkeit.TabIndex = 34
        Me.lblGeschwindigkeit.Text = "15 s"
        Me.lblGeschwindigkeit.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'tbtNW
        '
        Me.tbtNW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtNW.Checked = False
        Me.tbtNW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtNW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtNW.Location = New System.Drawing.Point(332, 144)
        Me.tbtNW.Name = "tbtNW"
        Me.tbtNW.Size = New System.Drawing.Size(75, 75)
        Me.tbtNW.TabIndex = 35
        Me.tbtNW.Text = "î"
        Me.tbtNW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtNW.UseVisualStyleBackColor = False
        '
        'tbtN
        '
        Me.tbtN.BackColor = System.Drawing.SystemColors.Control
        Me.tbtN.Checked = False
        Me.tbtN.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtN.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtN.Location = New System.Drawing.Point(567, 144)
        Me.tbtN.Name = "tbtN"
        Me.tbtN.Size = New System.Drawing.Size(75, 75)
        Me.tbtN.TabIndex = 36
        Me.tbtN.Text = "ê"
        Me.tbtN.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtN.UseVisualStyleBackColor = False
        '
        'tbtNO
        '
        Me.tbtNO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtNO.Checked = False
        Me.tbtNO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtNO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtNO.Location = New System.Drawing.Point(802, 144)
        Me.tbtNO.Name = "tbtNO"
        Me.tbtNO.Size = New System.Drawing.Size(75, 75)
        Me.tbtNO.TabIndex = 37
        Me.tbtNO.Text = "í"
        Me.tbtNO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtNO.UseVisualStyleBackColor = False
        '
        'tbtW
        '
        Me.tbtW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtW.Checked = False
        Me.tbtW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtW.Location = New System.Drawing.Point(332, 270)
        Me.tbtW.Name = "tbtW"
        Me.tbtW.Size = New System.Drawing.Size(75, 75)
        Me.tbtW.TabIndex = 38
        Me.tbtW.Text = "è"
        Me.tbtW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtW.UseVisualStyleBackColor = False
        '
        'tbtO
        '
        Me.tbtO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtO.Checked = False
        Me.tbtO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtO.Location = New System.Drawing.Point(802, 270)
        Me.tbtO.Name = "tbtO"
        Me.tbtO.Size = New System.Drawing.Size(75, 75)
        Me.tbtO.TabIndex = 39
        Me.tbtO.Text = "ç"
        Me.tbtO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtO.UseVisualStyleBackColor = False
        '
        'tbtSW
        '
        Me.tbtSW.BackColor = System.Drawing.SystemColors.Control
        Me.tbtSW.Checked = False
        Me.tbtSW.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtSW.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtSW.Location = New System.Drawing.Point(332, 396)
        Me.tbtSW.Name = "tbtSW"
        Me.tbtSW.Size = New System.Drawing.Size(75, 75)
        Me.tbtSW.TabIndex = 40
        Me.tbtSW.Text = "ì"
        Me.tbtSW.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtSW.UseVisualStyleBackColor = False
        '
        'tbtS
        '
        Me.tbtS.BackColor = System.Drawing.SystemColors.Control
        Me.tbtS.Checked = False
        Me.tbtS.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtS.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtS.Location = New System.Drawing.Point(567, 396)
        Me.tbtS.Name = "tbtS"
        Me.tbtS.Size = New System.Drawing.Size(75, 75)
        Me.tbtS.TabIndex = 41
        Me.tbtS.Text = "é"
        Me.tbtS.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtS.UseVisualStyleBackColor = False
        '
        'tbtSO
        '
        Me.tbtSO.BackColor = System.Drawing.SystemColors.Control
        Me.tbtSO.Checked = False
        Me.tbtSO.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtSO.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtSO.Location = New System.Drawing.Point(803, 396)
        Me.tbtSO.Name = "tbtSO"
        Me.tbtSO.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.tbtSO.Size = New System.Drawing.Size(75, 75)
        Me.tbtSO.TabIndex = 42
        Me.tbtSO.Text = "ë"
        Me.tbtSO.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtSO.UseVisualStyleBackColor = False
        '
        'lblTransitionsname
        '
        Me.lblTransitionsname.AutoSize = True
        Me.lblTransitionsname.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblTransitionsname.Location = New System.Drawing.Point(325, 26)
        Me.lblTransitionsname.Name = "lblTransitionsname"
        Me.lblTransitionsname.Size = New System.Drawing.Size(270, 41)
        Me.lblTransitionsname.TabIndex = 45
        Me.lblTransitionsname.Text = "Gradient-Wischen"
        '
        'tbtZIn
        '
        Me.tbtZIn.BackColor = System.Drawing.SystemColors.Control
        Me.tbtZIn.BackgroundImage = Global.SlideShowTransition.GradientWischen.My.Resources.Resources.Sammelpunkt
        Me.tbtZIn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.tbtZIn.Checked = False
        Me.tbtZIn.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtZIn.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtZIn.Location = New System.Drawing.Point(637, 257)
        Me.tbtZIn.Name = "tbtZIn"
        Me.tbtZIn.Size = New System.Drawing.Size(100, 100)
        Me.tbtZIn.TabIndex = 47
        Me.tbtZIn.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtZIn.UseVisualStyleBackColor = False
        '
        'tbtZOut
        '
        Me.tbtZOut.BackColor = System.Drawing.SystemColors.Control
        Me.tbtZOut.BackgroundImage = Global.SlideShowTransition.GradientWischen.My.Resources.Resources.Fluchtpunkt
        Me.tbtZOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.tbtZOut.Checked = False
        Me.tbtZOut.CheckedBackColor = System.Drawing.SystemColors.Highlight
        Me.tbtZOut.Font = New System.Drawing.Font("Wingdings", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.tbtZOut.Location = New System.Drawing.Point(472, 257)
        Me.tbtZOut.Name = "tbtZOut"
        Me.tbtZOut.Size = New System.Drawing.Size(100, 100)
        Me.tbtZOut.TabIndex = 43
        Me.tbtZOut.UncheckedBackColor = System.Drawing.SystemColors.Control
        Me.tbtZOut.UseVisualStyleBackColor = False
        '
        'trkBreite
        '
        Me.trkBreite.AutoSize = False
        Me.trkBreite.Location = New System.Drawing.Point(332, 598)
        Me.trkBreite.Maximum = 100
        Me.trkBreite.Minimum = 1
        Me.trkBreite.Name = "trkBreite"
        Me.trkBreite.Size = New System.Drawing.Size(450, 57)
        Me.trkBreite.TabIndex = 49
        Me.trkBreite.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trkBreite.Value = 33
        '
        'lblNBreite
        '
        Me.lblNBreite.AutoSize = True
        Me.lblNBreite.Location = New System.Drawing.Point(41, 598)
        Me.lblNBreite.Name = "lblNBreite"
        Me.lblNBreite.Size = New System.Drawing.Size(216, 41)
        Me.lblNBreite.TabIndex = 48
        Me.lblNBreite.Text = "Breite Gradient"
        '
        'lblBreiteProzent
        '
        Me.lblBreiteProzent.AutoSize = True
        Me.lblBreiteProzent.Location = New System.Drawing.Point(779, 598)
        Me.lblBreiteProzent.MinimumSize = New System.Drawing.Size(99, 0)
        Me.lblBreiteProzent.Name = "lblBreiteProzent"
        Me.lblBreiteProzent.Size = New System.Drawing.Size(99, 41)
        Me.lblBreiteProzent.TabIndex = 50
        Me.lblBreiteProzent.Text = "33 %"
        Me.lblBreiteProzent.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'btnDefaults
        '
        Me.btnDefaults.Location = New System.Drawing.Point(704, 20)
        Me.btnDefaults.Name = "btnDefaults"
        Me.btnDefaults.Size = New System.Drawing.Size(174, 52)
        Me.btnDefaults.TabIndex = 51
        Me.btnDefaults.Text = "Standards"
        Me.btnDefaults.UseVisualStyleBackColor = True
        '
        'ucOptionsTransition
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnDefaults)
        Me.Controls.Add(Me.lblBreiteProzent)
        Me.Controls.Add(Me.trkBreite)
        Me.Controls.Add(Me.lblNBreite)
        Me.Controls.Add(Me.tbtZIn)
        Me.Controls.Add(Me.lblTransitionsname)
        Me.Controls.Add(Me.tbtZOut)
        Me.Controls.Add(Me.tbtSO)
        Me.Controls.Add(Me.tbtS)
        Me.Controls.Add(Me.tbtSW)
        Me.Controls.Add(Me.tbtO)
        Me.Controls.Add(Me.tbtW)
        Me.Controls.Add(Me.tbtNO)
        Me.Controls.Add(Me.tbtN)
        Me.Controls.Add(Me.tbtNW)
        Me.Controls.Add(Me.lblGeschwindigkeit)
        Me.Controls.Add(Me.trkGeschwindigkeit)
        Me.Controls.Add(Me.lblNtrbGeschwindigkeit)
        Me.Controls.Add(Me.lblKeineRichtung)
        Me.Controls.Add(Me.lblNRichtungen)
        Me.Controls.Add(Me.lblShadername)
        Me.Controls.Add(Me.lblNlblTransitionname)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsTransition"
        Me.Size = New System.Drawing.Size(890, 1020)
        CType(Me.trkGeschwindigkeit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trkBreite, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblShadername As Windows.Forms.Label
    Friend WithEvents lblNlblTransitionname As Windows.Forms.Label
    Friend WithEvents lblNRichtungen As Windows.Forms.Label
    Friend WithEvents lblKeineRichtung As Windows.Forms.Label
    Friend WithEvents lblNtrbGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents trkGeschwindigkeit As Windows.Forms.TrackBar
    Friend WithEvents lblGeschwindigkeit As Windows.Forms.Label
    Friend WithEvents tbtNW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtN As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtNO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtSW As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtS As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtSO As MyControlsLibrary.ToggleButton
    Friend WithEvents tbtZOut As MyControlsLibrary.ToggleButton
    Friend WithEvents lblTransitionsname As Forms.Label
    Friend WithEvents tbtZIn As MyControlsLibrary.ToggleButton
    Friend WithEvents trkBreite As Forms.TrackBar
    Friend WithEvents lblNBreite As Forms.Label
    Friend WithEvents lblBreiteProzent As Forms.Label
    Friend WithEvents btnDefaults As Forms.Button
End Class
