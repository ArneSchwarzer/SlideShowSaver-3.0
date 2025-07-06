<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ucOptionsBildauswahl
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
        Me.lblNlstVerzeichnisse = New System.Windows.Forms.Label()
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
        Me.lblNlstWhiteList = New System.Windows.Forms.Label()
        Me.lstWhiteList = New System.Windows.Forms.ListBox()
        Me.lstBlackList = New System.Windows.Forms.ListBox()
        Me.lblNlstBlakcList = New System.Windows.Forms.Label()
        Me.lblNAltersfreigabe = New System.Windows.Forms.Label()
        Me.lblNBewertung = New System.Windows.Forms.Label()
        Me.rdo18 = New System.Windows.Forms.RadioButton()
        Me.rdoAkt = New System.Windows.Forms.RadioButton()
        Me.rdoLingerie = New System.Windows.Forms.RadioButton()
        Me.rdoJugendfrei = New System.Windows.Forms.RadioButton()
        Me.sbcBewertung = New StarControlLibrary.SterneBewertungControl()
        Me.SuspendLayout()
        '
        'lblNlstVerzeichnisse
        '
        Me.lblNlstVerzeichnisse.AutoSize = True
        Me.lblNlstVerzeichnisse.Location = New System.Drawing.Point(19, 13)
        Me.lblNlstVerzeichnisse.Name = "lblNlstVerzeichnisse"
        Me.lblNlstVerzeichnisse.Size = New System.Drawing.Size(195, 41)
        Me.lblNlstVerzeichnisse.TabIndex = 0
        Me.lblNlstVerzeichnisse.Tag = "lblNlstVerzeichnisse"
        Me.lblNlstVerzeichnisse.Text = "Verzeichnisse"
        '
        'lstVerzeichnisse
        '
        Me.lstVerzeichnisse.FormattingEnabled = True
        Me.lstVerzeichnisse.ItemHeight = 41
        Me.lstVerzeichnisse.Location = New System.Drawing.Point(26, 57)
        Me.lstVerzeichnisse.Name = "lstVerzeichnisse"
        Me.lstVerzeichnisse.Size = New System.Drawing.Size(765, 291)
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
        Me.btnWhiteListListeLöschen.Location = New System.Drawing.Point(380, 543)
        Me.btnWhiteListListeLöschen.Name = "btnWhiteListListeLöschen"
        Me.btnWhiteListListeLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnWhiteListListeLöschen.TabIndex = 7
        Me.btnWhiteListListeLöschen.UseVisualStyleBackColor = True
        '
        'btnWhiteListLöschen
        '
        Me.btnWhiteListLöschen.Location = New System.Drawing.Point(380, 483)
        Me.btnWhiteListLöschen.Name = "btnWhiteListLöschen"
        Me.btnWhiteListLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnWhiteListLöschen.TabIndex = 6
        Me.btnWhiteListLöschen.Text = "-"
        Me.btnWhiteListLöschen.UseVisualStyleBackColor = True
        '
        'btnWhiteListHinzufügen
        '
        Me.btnWhiteListHinzufügen.Location = New System.Drawing.Point(380, 423)
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
        Me.btnBlackListListeLöschen.Location = New System.Drawing.Point(812, 543)
        Me.btnBlackListListeLöschen.Name = "btnBlackListListeLöschen"
        Me.btnBlackListListeLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnBlackListListeLöschen.TabIndex = 10
        Me.btnBlackListListeLöschen.UseVisualStyleBackColor = True
        '
        'btnBlackListLöschen
        '
        Me.btnBlackListLöschen.Location = New System.Drawing.Point(812, 483)
        Me.btnBlackListLöschen.Name = "btnBlackListLöschen"
        Me.btnBlackListLöschen.Size = New System.Drawing.Size(50, 50)
        Me.btnBlackListLöschen.TabIndex = 9
        Me.btnBlackListLöschen.Text = "-"
        Me.btnBlackListLöschen.UseVisualStyleBackColor = True
        '
        'btnBlackListHinzufügen
        '
        Me.btnBlackListHinzufügen.Location = New System.Drawing.Point(812, 423)
        Me.btnBlackListHinzufügen.Name = "btnBlackListHinzufügen"
        Me.btnBlackListHinzufügen.Size = New System.Drawing.Size(50, 50)
        Me.btnBlackListHinzufügen.TabIndex = 8
        Me.btnBlackListHinzufügen.Text = "+"
        Me.btnBlackListHinzufügen.UseVisualStyleBackColor = True
        '
        'lblNlstWhiteList
        '
        Me.lblNlstWhiteList.AutoSize = True
        Me.lblNlstWhiteList.Location = New System.Drawing.Point(26, 380)
        Me.lblNlstWhiteList.Name = "lblNlstWhiteList"
        Me.lblNlstWhiteList.Size = New System.Drawing.Size(152, 41)
        Me.lblNlstWhiteList.TabIndex = 11
        Me.lblNlstWhiteList.Tag = "lblNlstWhiteList"
        Me.lblNlstWhiteList.Text = "White-List"
        '
        'lstWhiteList
        '
        Me.lstWhiteList.FormattingEnabled = True
        Me.lstWhiteList.ItemHeight = 41
        Me.lstWhiteList.Location = New System.Drawing.Point(26, 424)
        Me.lstWhiteList.Name = "lstWhiteList"
        Me.lstWhiteList.Size = New System.Drawing.Size(337, 291)
        Me.lstWhiteList.TabIndex = 12
        '
        'lstBlackList
        '
        Me.lstBlackList.FormattingEnabled = True
        Me.lstBlackList.ItemHeight = 41
        Me.lstBlackList.Location = New System.Drawing.Point(454, 423)
        Me.lstBlackList.Name = "lstBlackList"
        Me.lstBlackList.Size = New System.Drawing.Size(337, 291)
        Me.lstBlackList.TabIndex = 13
        '
        'lblNlstBlakcList
        '
        Me.lblNlstBlakcList.AutoSize = True
        Me.lblNlstBlakcList.Location = New System.Drawing.Point(454, 380)
        Me.lblNlstBlakcList.Name = "lblNlstBlakcList"
        Me.lblNlstBlakcList.Size = New System.Drawing.Size(142, 41)
        Me.lblNlstBlakcList.TabIndex = 14
        Me.lblNlstBlakcList.Tag = "lblNlstBlakcList"
        Me.lblNlstBlakcList.Text = "Black-List"
        '
        'lblNAltersfreigabe
        '
        Me.lblNAltersfreigabe.AutoSize = True
        Me.lblNAltersfreigabe.Location = New System.Drawing.Point(19, 765)
        Me.lblNAltersfreigabe.Name = "lblNAltersfreigabe"
        Me.lblNAltersfreigabe.Size = New System.Drawing.Size(202, 41)
        Me.lblNAltersfreigabe.TabIndex = 15
        Me.lblNAltersfreigabe.Tag = "lblNAltersfreigabe"
        Me.lblNAltersfreigabe.Text = "Altersfreigabe"
        '
        'lblNBewertung
        '
        Me.lblNBewertung.AutoSize = True
        Me.lblNBewertung.Location = New System.Drawing.Point(454, 765)
        Me.lblNBewertung.Name = "lblNBewertung"
        Me.lblNBewertung.Size = New System.Drawing.Size(161, 41)
        Me.lblNBewertung.TabIndex = 16
        Me.lblNBewertung.Tag = "lblNBewertung"
        Me.lblNBewertung.Text = "Bewertung"
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
        Me.rdoAkt.Tag = "rdoAkt"
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
        Me.rdoLingerie.Tag = "rdoLingerie"
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
        Me.rdoJugendfrei.Tag = "rdoJugendfrei"
        Me.rdoJugendfrei.Text = "Jugendfrei"
        Me.rdoJugendfrei.UseVisualStyleBackColor = True
        '
        'sbcBewertung
        '
        Me.sbcBewertung.Bewertung = 0
        Me.sbcBewertung.Location = New System.Drawing.Point(461, 809)
        Me.sbcBewertung.Name = "sbcBewertung"
        Me.sbcBewertung.Size = New System.Drawing.Size(241, 43)
        Me.sbcBewertung.TabIndex = 21
        Me.sbcBewertung.Text = "SterneBewertungControl1"
        '
        'ucOptionsBildauswahl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.sbcBewertung)
        Me.Controls.Add(Me.rdoJugendfrei)
        Me.Controls.Add(Me.rdoLingerie)
        Me.Controls.Add(Me.rdoAkt)
        Me.Controls.Add(Me.rdo18)
        Me.Controls.Add(Me.lblNBewertung)
        Me.Controls.Add(Me.lblNAltersfreigabe)
        Me.Controls.Add(Me.lblNlstBlakcList)
        Me.Controls.Add(Me.lstBlackList)
        Me.Controls.Add(Me.lstWhiteList)
        Me.Controls.Add(Me.lblNlstWhiteList)
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
        Me.Controls.Add(Me.lblNlstVerzeichnisse)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "ucOptionsBildauswahl"
        Me.Size = New System.Drawing.Size(890, 1020)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblNlstVerzeichnisse As Windows.Forms.Label
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
    Friend WithEvents lblNlstWhiteList As Windows.Forms.Label
    Friend WithEvents lstWhiteList As Windows.Forms.ListBox
    Friend WithEvents lstBlackList As Windows.Forms.ListBox
    Friend WithEvents lblNlstBlakcList As Windows.Forms.Label
    Friend WithEvents lblNAltersfreigabe As Windows.Forms.Label
    Friend WithEvents lblNBewertung As Windows.Forms.Label
    Friend WithEvents rdo18 As Windows.Forms.RadioButton
    Friend WithEvents rdoAkt As Windows.Forms.RadioButton
    Friend WithEvents rdoLingerie As Windows.Forms.RadioButton
    Friend WithEvents rdoJugendfrei As Windows.Forms.RadioButton
    Friend WithEvents sbcBewertung As StarControlLibrary.SterneBewertungControl
End Class
