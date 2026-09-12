<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        FolderBrowserDialog1 = New FolderBrowserDialog()
        cbUnterordner = New CheckBox()
        Label1 = New Label()
        tbOrdner = New TextBox()
        btnFolderBrowserDialog = New Button()
        Label2 = New Label()
        lblModus = New Label()
        btnAnalyse = New Button()
        btnSynchronisation = New Button()
        btnBeenden = New Button()
        cbUnterschiede = New CheckBox()
        DataGridView1 = New DataGridView()
        colDatei = New DataGridViewTextBoxColumn()
        colBridgeBewertung = New DataGridViewTextBoxColumn()
        colWindowsBewertung = New DataGridViewTextBoxColumn()
        colStatus = New DataGridViewTextBoxColumn()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' cbUnterordner
        ' 
        cbUnterordner.AutoSize = True
        cbUnterordner.Location = New Point(29, 82)
        cbUnterordner.Name = "cbUnterordner"
        cbUnterordner.Size = New Size(344, 41)
        cbUnterordner.TabIndex = 0
        cbUnterordner.Text = "Unterordner einbeziehen"
        cbUnterordner.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(29, 30)
        Label1.Name = "Label1"
        Label1.Size = New Size(100, 37)
        Label1.TabIndex = 1
        Label1.Text = "Ordner"
        ' 
        ' tbOrdner
        ' 
        tbOrdner.Location = New Point(135, 30)
        tbOrdner.Name = "tbOrdner"
        tbOrdner.Size = New Size(743, 43)
        tbOrdner.TabIndex = 2
        ' 
        ' btnFolderBrowserDialog
        ' 
        btnFolderBrowserDialog.Location = New Point(903, 22)
        btnFolderBrowserDialog.Name = "btnFolderBrowserDialog"
        btnFolderBrowserDialog.Size = New Size(58, 52)
        btnFolderBrowserDialog.TabIndex = 3
        btnFolderBrowserDialog.Text = "..."
        btnFolderBrowserDialog.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(28, 154)
        Label2.Name = "Label2"
        Label2.Size = New Size(99, 37)
        Label2.TabIndex = 4
        Label2.Text = "Modus"
        ' 
        ' lblModus
        ' 
        lblModus.AutoSize = True
        lblModus.Font = New Font("Segoe UI", 11F, FontStyle.Bold)
        lblModus.Location = New Point(152, 148)
        lblModus.Name = "lblModus"
        lblModus.Size = New Size(141, 45)
        lblModus.TabIndex = 5
        lblModus.Text = "Analyse"
        ' 
        ' btnAnalyse
        ' 
        btnAnalyse.Location = New Point(493, 697)
        btnAnalyse.Name = "btnAnalyse"
        btnAnalyse.Size = New Size(225, 52)
        btnAnalyse.TabIndex = 7
        btnAnalyse.Text = "Analyse"
        btnAnalyse.UseVisualStyleBackColor = True
        ' 
        ' btnSynchronisation
        ' 
        btnSynchronisation.Enabled = False
        btnSynchronisation.Location = New Point(736, 697)
        btnSynchronisation.Name = "btnSynchronisation"
        btnSynchronisation.Size = New Size(225, 52)
        btnSynchronisation.TabIndex = 8
        btnSynchronisation.Text = "Synchronisation"
        btnSynchronisation.UseVisualStyleBackColor = True
        ' 
        ' btnBeenden
        ' 
        btnBeenden.Location = New Point(736, 774)
        btnBeenden.Name = "btnBeenden"
        btnBeenden.Size = New Size(225, 52)
        btnBeenden.TabIndex = 9
        btnBeenden.Text = "Beenden"
        btnBeenden.UseVisualStyleBackColor = True
        ' 
        ' cbUnterschiede
        ' 
        cbUnterschiede.AutoSize = True
        cbUnterschiede.Location = New Point(32, 205)
        cbUnterschiede.Name = "cbUnterschiede"
        cbUnterschiede.Size = New Size(369, 41)
        cbUnterschiede.TabIndex = 10
        cbUnterschiede.Text = "Nur Unterschiede anzeigen"
        cbUnterschiede.UseVisualStyleBackColor = True
        ' 
        ' DataGridView1
        ' 
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Columns.AddRange(New DataGridViewColumn() {colDatei, colBridgeBewertung, colWindowsBewertung, colStatus})
        DataGridView1.Location = New Point(32, 266)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.RowHeadersWidth = 92
        DataGridView1.Size = New Size(929, 407)
        DataGridView1.TabIndex = 11
        ' 
        ' colDatei
        ' 
        colDatei.Frozen = True
        colDatei.HeaderText = "Datei"
        colDatei.MinimumWidth = 11
        colDatei.Name = "colDatei"
        colDatei.Width = 225
        ' 
        ' colBridgeBewertung
        ' 
        colBridgeBewertung.Frozen = True
        colBridgeBewertung.HeaderText = "Bridge"
        colBridgeBewertung.MinimumWidth = 11
        colBridgeBewertung.Name = "colBridgeBewertung"
        colBridgeBewertung.Width = 225
        ' 
        ' colWindowsBewertung
        ' 
        colWindowsBewertung.Frozen = True
        colWindowsBewertung.HeaderText = "Windows"
        colWindowsBewertung.MinimumWidth = 11
        colWindowsBewertung.Name = "colWindowsBewertung"
        colWindowsBewertung.Width = 225
        ' 
        ' colStatus
        ' 
        colStatus.Frozen = True
        colStatus.HeaderText = "Status"
        colStatus.MinimumWidth = 11
        colStatus.Name = "colStatus"
        colStatus.Width = 225
        ' 
        ' frmMain
        ' 
        AutoScaleDimensions = New SizeF(15F, 37F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(990, 848)
        Controls.Add(DataGridView1)
        Controls.Add(cbUnterschiede)
        Controls.Add(btnBeenden)
        Controls.Add(btnSynchronisation)
        Controls.Add(btnAnalyse)
        Controls.Add(lblModus)
        Controls.Add(Label2)
        Controls.Add(btnFolderBrowserDialog)
        Controls.Add(tbOrdner)
        Controls.Add(Label1)
        Controls.Add(cbUnterordner)
        Name = "frmMain"
        Text = "BridgeRatingSynchronizer"
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents cbUnterordner As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents tbOrdner As TextBox
    Friend WithEvents btnFolderBrowserDialog As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents lblModus As Label
    Friend WithEvents btnAnalyse As Button
    Friend WithEvents btnSynchronisation As Button
    Friend WithEvents btnBeenden As Button
    Friend WithEvents cbUnterschiede As CheckBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents colDatei As DataGridViewTextBoxColumn
    Friend WithEvents colBridgeBewertung As DataGridViewTextBoxColumn
    Friend WithEvents colWindowsBewertung As DataGridViewTextBoxColumn
    Friend WithEvents colStatus As DataGridViewTextBoxColumn

End Class
