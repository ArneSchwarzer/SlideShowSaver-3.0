<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucOptionsBildauswahl
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lstVerzeichnisse = New System.Windows.Forms.ListBox()
        Me.btnVerzeichnisHinzufügen = New System.Windows.Forms.Button()
        Me.btnVerzeichnisseLöschen = New System.Windows.Forms.Button()
        Me.btnVerzeichnisseListeLöschen = New System.Windows.Forms.Button()
        Me.btnWhiteListListeLöschen = New System.Windows.Forms.Button()
        Me.btnWhiteListLöschen = New System.Windows.Forms.Button()
        Me.btnWhiteListHinzufügen = New System.Windows.Forms.Button()
        Me.btnBlackListListeLöschen = New System.Windows.Forms.Button()
        Me.btnBlackListLöschen = New System.Windows.Forms.Button()
        Me.btnBlackListHinzufügen = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lstWhiteList = New System.Windows.Forms.ListBox()
        Me.lstBlackList = New System.Windows.Forms.ListBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.rdo18 = New System.Windows.Forms.RadioButton()
        Me.rdoAkt = New System.Windows.Forms.RadioButton()
        Me.rdoLingerie = New System.Windows.Forms.RadioButton()
        Me.rdoJugendfrei = New System.Windows.Forms.RadioButton()
        Me.cmbBewertung = New System.Windows.Forms.ComboBox()
        Me.lblHinweisLabel = New System.Windows.Forms.Label()
        Me.lblHinweistext = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(19, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(195, 41)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Verzeichnisse"
        '
        'lstVerzeichnisse
        '
        Me.lstVerzeichnisse.FormattingEnabled = True
        Me.lstVerzeichnisse.ItemHeight = 41
        Me.lstVerzeichnisse.Location = New System.Drawing.Point(26, 57)
        Me.lstVerzeichnisse.Name = "lstVerzeichnisse"
        Me.lstVerzeichnisse.Size = New System.Drawing.Size(765, 209)
        Me.lstVerzeichnisse.TabIndex = 1
        '
        'btnVerzeichnisHinzufügen
        '
        Me.btnVerzeichnisHinzufügen.Location = New System.Drawing.Point(812, 57)
        Me.btnVerzeichnisHinzufügen.Name = "btnVerzeichnisHinzufügen"
        Me.btnVerzeichnisHinzufügen.Size = New System.Drawing.Size(50, 50)
        Me.btnVerzeichnisHinzufügen.TabIndex = 2
        Me.btnVerzeichnisHinzufügen.Text = "+"
        Me.btnVerzeichnisHinzufügen.UseVisualStyleBackColor = True
        '
        'btnVerzeichnisseLöschen
        '
        Me.btnVerzeichnisseLöschen.Location = New System.Drawing.Point(812, 117)
        Me.btnVerzeichnisseLöschen.Name = "btnVerzeichnisseLöschen"
        Me.btnVerzeichnisseLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnVerzeichnisseLöschen.TabIndex = 3
        Me.btnVerzeichnisseLöschen.Text = "-"
        Me.btnVerzeichnisseLöschen.UseVisualStyleBackColor = True
        '
        'btnVerzeichnisseListeLöschen
        '
        Me.btnVerzeichnisseListeLöschen.BackgroundImage = Global.SlideShowBildauswahl.My.Resources.Resources.Trashbin
        Me.btnVerzeichnisseListeLöschen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnVerzeichnisseListeLöschen.Location = New System.Drawing.Point(812, 177)
        Me.btnVerzeichnisseListeLöschen.Name = "btnVerzeichnisseListeLöschen"
        Me.btnVerzeichnisseListeLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnVerzeichnisseListeLöschen.TabIndex = 4
        Me.btnVerzeichnisseListeLöschen.UseVisualStyleBackColor = True
        '
        'btnWhiteListListeLöschen
        '
        Me.btnWhiteListListeLöschen.BackgroundImage = Global.SlideShowBildauswahl.My.Resources.Resources.Trashbin
        Me.btnWhiteListListeLöschen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnWhiteListListeLöschen.Location = New System.Drawing.Point(380, 663)
        Me.btnWhiteListListeLöschen.Name = "btnWhiteListListeLöschen"
        Me.btnWhiteListListeLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnWhiteListListeLöschen.TabIndex = 7
        Me.btnWhiteListListeLöschen.UseVisualStyleBackColor = True
        '
        'btnWhiteListLöschen
        '
        Me.btnWhiteListLöschen.Location = New System.Drawing.Point(380, 603)
        Me.btnWhiteListLöschen.Name = "btnWhiteListLöschen"
        Me.btnWhiteListLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnWhiteListLöschen.TabIndex = 6
        Me.btnWhiteListLöschen.Text = "-"
        Me.btnWhiteListLöschen.UseVisualStyleBackColor = True
        '
        'btnWhiteListHinzufügen
        '
        Me.btnWhiteListHinzufügen.Location = New System.Drawing.Point(380, 543)
        Me.btnWhiteListHinzufügen.Name = "btnWhiteListHinzufügen"
        Me.btnWhiteListHinzufügen.Size = New System.Drawing.Size(50, 50)
        Me.btnWhiteListHinzufügen.TabIndex = 5
        Me.btnWhiteListHinzufügen.Text = "+"
        Me.btnWhiteListHinzufügen.UseVisualStyleBackColor = True
        '
        'btnBlackListListeLöschen
        '
        Me.btnBlackListListeLöschen.BackgroundImage = Global.SlideShowBildauswahl.My.Resources.Resources.Trashbin
        Me.btnBlackListListeLöschen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnBlackListListeLöschen.Location = New System.Drawing.Point(812, 663)
        Me.btnBlackListListeLöschen.Name = "btnBlackListListeLöschen"
        Me.btnBlackListListeLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnBlackListListeLöschen.TabIndex = 10
        Me.btnBlackListListeLöschen.UseVisualStyleBackColor = True
        '
        'btnBlackListLöschen
        '
        Me.btnBlackListLöschen.Location = New System.Drawing.Point(812, 603)
        Me.btnBlackListLöschen.Name = "btnBlackListLöschen"
        Me.btnBlackListLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnBlackListLöschen.TabIndex = 9
        Me.btnBlackListLöschen.Text = "-"
        Me.btnBlackListLöschen.UseVisualStyleBackColor = True
        '
        'btnBlackListHinzufügen
        '
        Me.btnBlackListHinzufügen.Location = New System.Drawing.Point(812, 543)
        Me.btnBlackListHinzufügen.Name = "btnBlackListHinzufügen"
        Me.btnBlackListHinzufügen.Size = New System.Drawing.Size(50, 50)
        Me.btnBlackListHinzufügen.TabIndex = 8
        Me.btnBlackListHinzufügen.Text = "+"
        Me.btnBlackListHinzufügen.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(26, 500)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(152, 41)
        Me.Label2.TabIndex = 11
        Me.Label2.Text = "White-List"
        '
        'lstWhiteList
        '
        Me.lstWhiteList.FormattingEnabled = True
        Me.lstWhiteList.ItemHeight = 41
        Me.lstWhiteList.Location = New System.Drawing.Point(26, 544)
        Me.lstWhiteList.Name = "lstWhiteList"
        Me.lstWhiteList.Size = New System.Drawing.Size(337, 209)
        Me.lstWhiteList.TabIndex = 12
        '
        'lstBlackList
        '
        Me.lstBlackList.FormattingEnabled = True
        Me.lstBlackList.ItemHeight = 41
        Me.lstBlackList.Location = New System.Drawing.Point(454, 543)
        Me.lstBlackList.Name = "lstBlackList"
        Me.lstBlackList.Size = New System.Drawing.Size(337, 209)
        Me.lstBlackList.TabIndex = 13
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(454, 500)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(142, 41)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Black-List"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(19, 765)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(202, 41)
        Me.Label4.TabIndex = 15
        Me.Label4.Text = "Altersfreigabe"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(454, 765)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(161, 41)
        Me.Label5.TabIndex = 16
        Me.Label5.Text = "Bewertung"
        '
        'rdo18
        '
        Me.rdo18.AutoSize = True
        Me.rdo18.Location = New System.Drawing.Point(26, 807)
        Me.rdo18.Name = "rdo18"
        Me.rdo18.Size = New System.Drawing.Size(102, 45)
        Me.rdo18.TabIndex = 17
        Me.rdo18.TabStop = True
        Me.rdo18.Text = "18+"
        Me.rdo18.UseVisualStyleBackColor = True
        '
        'rdoAkt
        '
        Me.rdoAkt.AutoSize = True
        Me.rdoAkt.Location = New System.Drawing.Point(26, 853)
        Me.rdoAkt.Name = "rdoAkt"
        Me.rdoAkt.Size = New System.Drawing.Size(93, 45)
        Me.rdoAkt.TabIndex = 18
        Me.rdoAkt.TabStop = True
        Me.rdoAkt.Text = "Akt"
        Me.rdoAkt.UseVisualStyleBackColor = True
        '
        'rdoLingerie
        '
        Me.rdoLingerie.AutoSize = True
        Me.rdoLingerie.Location = New System.Drawing.Point(26, 899)
        Me.rdoLingerie.Name = "rdoLingerie"
        Me.rdoLingerie.Size = New System.Drawing.Size(154, 45)
        Me.rdoLingerie.TabIndex = 19
        Me.rdoLingerie.TabStop = True
        Me.rdoLingerie.Text = "Lingerie"
        Me.rdoLingerie.UseVisualStyleBackColor = True
        '
        'rdoJugendfrei
        '
        Me.rdoJugendfrei.AutoSize = True
        Me.rdoJugendfrei.Location = New System.Drawing.Point(26, 945)
        Me.rdoJugendfrei.Name = "rdoJugendfrei"
        Me.rdoJugendfrei.Size = New System.Drawing.Size(188, 45)
        Me.rdoJugendfrei.TabIndex = 20
        Me.rdoJugendfrei.TabStop = True
        Me.rdoJugendfrei.Text = "Jugendfrei"
        Me.rdoJugendfrei.UseVisualStyleBackColor = True
        '
        'cmbBewertung
        '
        Me.cmbBewertung.FormattingEnabled = True
        Me.cmbBewertung.Items.AddRange(New Object() {"5 Sterne", "4 Sterne", "3 Sterne", "2 Sterne", "1 Stern", "Keine Beschränkung"})
        Me.cmbBewertung.Location = New System.Drawing.Point(461, 807)
        Me.cmbBewertung.Name = "cmbBewertung"
        Me.cmbBewertung.Size = New System.Drawing.Size(330, 49)
        Me.cmbBewertung.TabIndex = 21
        '
        'lblHinweisLabel
        '
        Me.lblHinweisLabel.AutoSize = True
        Me.lblHinweisLabel.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblHinweisLabel.Location = New System.Drawing.Point(35, 316)
        Me.lblHinweisLabel.Name = "lblHinweisLabel"
        Me.lblHinweisLabel.Size = New System.Drawing.Size(130, 41)
        Me.lblHinweisLabel.TabIndex = 22
        Me.lblHinweisLabel.Text = "Hinweis"
        '
        'lblHinweistext
        '
        Me.lblHinweistext.AutoSize = True
        Me.lblHinweistext.Location = New System.Drawing.Point(171, 316)
        Me.lblHinweistext.MaximumSize = New System.Drawing.Size(700, 0)
        Me.lblHinweistext.Name = "lblHinweistext"
        Me.lblHinweistext.Size = New System.Drawing.Size(660, 164)
        Me.lblHinweistext.TabIndex = 23
        Me.lblHinweistext.Text = "Die untenstehenden Filterkriterien funktionieren ausschließlich für JPEG-Dateien." &
    " Die anderen  darstellbaren Bildformate (BMP, PNG) werden ungefiltert angezeigt." &
    ""
        '
        'ucOptionsBildauswahl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblHinweistext)
        Me.Controls.Add(Me.lblHinweisLabel)
        Me.Controls.Add(Me.cmbBewertung)
        Me.Controls.Add(Me.rdoJugendfrei)
        Me.Controls.Add(Me.rdoLingerie)
        Me.Controls.Add(Me.rdoAkt)
        Me.Controls.Add(Me.rdo18)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lstBlackList)
        Me.Controls.Add(Me.lstWhiteList)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnBlackListListeLöschen)
        Me.Controls.Add(Me.btnBlackListLöschen)
        Me.Controls.Add(Me.btnBlackListHinzufügen)
        Me.Controls.Add(Me.btnWhiteListListeLöschen)
        Me.Controls.Add(Me.btnWhiteListLöschen)
        Me.Controls.Add(Me.btnWhiteListHinzufügen)
        Me.Controls.Add(Me.btnVerzeichnisseListeLöschen)
        Me.Controls.Add(Me.btnVerzeichnisseLöschen)
        Me.Controls.Add(Me.btnVerzeichnisHinzufügen)
        Me.Controls.Add(Me.lstVerzeichnisse)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsBildauswahl"
        Me.Size = New System.Drawing.Size(890, 1020)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Windows.Forms.Label
    Friend WithEvents lstVerzeichnisse As Windows.Forms.ListBox
    Friend WithEvents btnVerzeichnisHinzufügen As Windows.Forms.Button
    Friend WithEvents btnVerzeichnisseLöschen As Windows.Forms.Button
    Friend WithEvents btnVerzeichnisseListeLöschen As Windows.Forms.Button
    Friend WithEvents btnWhiteListListeLöschen As Windows.Forms.Button
    Friend WithEvents btnWhiteListLöschen As Windows.Forms.Button
    Friend WithEvents btnWhiteListHinzufügen As Windows.Forms.Button
    Friend WithEvents btnBlackListListeLöschen As Windows.Forms.Button
    Friend WithEvents btnBlackListLöschen As Windows.Forms.Button
    Friend WithEvents btnBlackListHinzufügen As Windows.Forms.Button
    Friend WithEvents Label2 As Windows.Forms.Label
    Friend WithEvents lstWhiteList As Windows.Forms.ListBox
    Friend WithEvents lstBlackList As Windows.Forms.ListBox
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents Label4 As Windows.Forms.Label
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents rdo18 As Windows.Forms.RadioButton
    Friend WithEvents rdoAkt As Windows.Forms.RadioButton
    Friend WithEvents rdoLingerie As Windows.Forms.RadioButton
    Friend WithEvents rdoJugendfrei As Windows.Forms.RadioButton
    Friend WithEvents cmbBewertung As Windows.Forms.ComboBox
    Friend WithEvents lblHinweisLabel As Windows.Forms.Label
    Friend WithEvents lblHinweistext As Windows.Forms.Label
End Class
