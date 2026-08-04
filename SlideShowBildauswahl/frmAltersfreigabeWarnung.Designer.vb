<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAltersfreigabeWarnung
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
        Me.btnAltersfreigabeAnpassen = New System.Windows.Forms.Button()
        Me.btnTrotzdemHinzufuegen = New System.Windows.Forms.Button()
        Me.btnAbbrechen = New System.Windows.Forms.Button()
        Me.rtxMessage = New System.Windows.Forms.RichTextBox()
        Me.lblTitel = New System.Windows.Forms.Label()
        Me.picLogo = New System.Windows.Forms.PictureBox()
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnAltersfreigabeAnpassen
        '
        Me.btnAltersfreigabeAnpassen.Location = New System.Drawing.Point(27, 451)
        Me.btnAltersfreigabeAnpassen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnAltersfreigabeAnpassen.Name = "btnAltersfreigabeAnpassen"
        Me.btnAltersfreigabeAnpassen.Size = New System.Drawing.Size(290, 103)
        Me.btnAltersfreigabeAnpassen.TabIndex = 0
        Me.btnAltersfreigabeAnpassen.Text = "Altersfreigabe auf ""Lingerie"" anpassen"
        Me.btnAltersfreigabeAnpassen.UseVisualStyleBackColor = True
        '
        'btnTrotzdemHinzufuegen
        '
        Me.btnTrotzdemHinzufuegen.Location = New System.Drawing.Point(383, 451)
        Me.btnTrotzdemHinzufuegen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnTrotzdemHinzufuegen.Name = "btnTrotzdemHinzufuegen"
        Me.btnTrotzdemHinzufuegen.Size = New System.Drawing.Size(290, 103)
        Me.btnTrotzdemHinzufuegen.TabIndex = 1
        Me.btnTrotzdemHinzufuegen.Text = "Tag trotzdem einfügen"
        Me.btnTrotzdemHinzufuegen.UseVisualStyleBackColor = True
        '
        'btnAbbrechen
        '
        Me.btnAbbrechen.Location = New System.Drawing.Point(739, 451)
        Me.btnAbbrechen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnAbbrechen.Name = "btnAbbrechen"
        Me.btnAbbrechen.Size = New System.Drawing.Size(290, 103)
        Me.btnAbbrechen.TabIndex = 2
        Me.btnAbbrechen.Text = "Abbrechen"
        Me.btnAbbrechen.UseVisualStyleBackColor = True
        '
        'rtxMessage
        '
        Me.rtxMessage.BackColor = System.Drawing.SystemColors.Control
        Me.rtxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtxMessage.Location = New System.Drawing.Point(27, 94)
        Me.rtxMessage.Name = "rtxMessage"
        Me.rtxMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
        Me.rtxMessage.Size = New System.Drawing.Size(752, 349)
        Me.rtxMessage.TabIndex = 3
        Me.rtxMessage.Text = "Die Altersfreigabe hat immer Vorrang vor der Whitelist. Mit der aktuellen Einstel" &
    "lung kann das neue Whitelist-Tag daher keine entsprechenden Bilder zulassen."
        '
        'lblTitel
        '
        Me.lblTitel.AutoSize = True
        Me.lblTitel.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitel.Location = New System.Drawing.Point(19, 13)
        Me.lblTitel.Name = "lblTitel"
        Me.lblTitel.Size = New System.Drawing.Size(638, 48)
        Me.lblTitel.TabIndex = 4
        Me.lblTitel.Text = "Uuuups... Ein Altersfreigabe-Konflikt"
        '
        'picLogo
        '
        Me.picLogo.Image = Global.SlideShowBildauswahl.My.Resources.Resources.Flying_Kitchen_Aid_Logo_Transparent
        Me.picLogo.Location = New System.Drawing.Point(785, 12)
        Me.picLogo.Name = "picLogo"
        Me.picLogo.Size = New System.Drawing.Size(235, 223)
        Me.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLogo.TabIndex = 5
        Me.picLogo.TabStop = False
        Me.picLogo.Tag = "langKey=picLogo"
        '
        'frmAltersfreigabeWarnung
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(17.0!, 41.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1061, 568)
        Me.Controls.Add(Me.picLogo)
        Me.Controls.Add(Me.lblTitel)
        Me.Controls.Add(Me.rtxMessage)
        Me.Controls.Add(Me.btnAbbrechen)
        Me.Controls.Add(Me.btnTrotzdemHinzufuegen)
        Me.Controls.Add(Me.btnAltersfreigabeAnpassen)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "frmAltersfreigabeWarnung"
        Me.Text = "Altersfreigabe - Warnung"
        CType(Me.picLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnAltersfreigabeAnpassen As Windows.Forms.Button
    Friend WithEvents btnTrotzdemHinzufuegen As Windows.Forms.Button
    Friend WithEvents btnAbbrechen As Windows.Forms.Button
    Friend WithEvents rtxMessage As Windows.Forms.RichTextBox
    Friend WithEvents lblTitel As Windows.Forms.Label
    Friend WithEvents picLogo As Windows.Forms.PictureBox
End Class
