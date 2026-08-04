Imports System.Drawing
Imports System.Windows.Forms

Public Class frmAltersfreigabeWarnung

#Region "Strukturen und Enumerationen"

    Public Enum AltersfreigabeWarnungErgebnis
        Abbrechen
        TagTrotzdemHinzufuegen
        AltersfreigabeAnpassen
    End Enum

#End Region

#Region "Variablendeklarationen"

    Private whitelistTag As String
    Private aktuelleAltersfreigabe As String
    Private erforderlicheAltersfreigabe As String

    Public Property Ergebnis As AltersfreigabeWarnungErgebnis

#End Region

#Region "Konstruktor"

    Public Sub New(tag As String, aktuelleFreigabe As String, erforderlicheFreigabe As String)

        InitializeComponent()

        whitelistTag = tag
        aktuelleAltersfreigabe = aktuelleFreigabe
        erforderlicheAltersfreigabe = erforderlicheFreigabe
        Ergebnis = AltersfreigabeWarnungErgebnis.Abbrechen

    End Sub

#End Region

#Region "Eventhandling"

    Private Sub frmAltersfreigabeWarnung_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.TopMost = True

        ErstelleMeldung()

        btnAltersfreigabeAnpassen.Text = "Altersfreigabe auf """ & erforderlicheAltersfreigabe & """ ändern"

    End Sub

    Private Sub btnAltersfreigabeAnpassen_Click(sender As Object, e As EventArgs) Handles btnAltersfreigabeAnpassen.Click

        Ergebnis = AltersfreigabeWarnungErgebnis.AltersfreigabeAnpassen

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub btnTrotzdemHinzufuegen_Click(sender As Object, e As EventArgs) Handles btnTrotzdemHinzufuegen.Click

        Ergebnis = AltersfreigabeWarnungErgebnis.TagTrotzdemHinzufuegen

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub btnAbbrechen_Click(sender As Object, e As EventArgs) Handles btnAbbrechen.Click

        Ergebnis = AltersfreigabeWarnungErgebnis.Abbrechen

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region

#Region "Private Methoden"

    Private Sub ErstelleMeldung()
        'Erstellt den formatierten Warnungstext.

        rtxMessage.Clear()

        rtxMessage.SelectionFont =
            New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Regular)

        rtxMessage.AppendText(
            "Das Whitelist-Tag ")

        rtxMessage.SelectionFont =
            New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold)

        rtxMessage.AppendText(
            whitelistTag)

        rtxMessage.SelectionFont =
            New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Regular)

        rtxMessage.AppendText(
            " benötigt mindestens die Altersfreigabe ")

        rtxMessage.SelectionFont =
            New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold)

        rtxMessage.AppendText(
            erforderlicheAltersfreigabe)

        rtxMessage.SelectionFont =
            New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Regular)

        rtxMessage.AppendText(
            "." &
            Environment.NewLine &
            Environment.NewLine &
            "Aktuell ist die Altersfreigabe ")

        rtxMessage.SelectionFont =
            New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Bold)

        rtxMessage.AppendText(
            aktuelleAltersfreigabe)

        rtxMessage.SelectionFont =
            New Font(
                "Segoe UI",
                10.0F,
                FontStyle.Regular)

        rtxMessage.AppendText(
            " eingestellt." &
            Environment.NewLine &
            Environment.NewLine &
            "Die Altersfreigabe hat immer Vorrang vor der Whitelist. " &
            "Mit der aktuellen Einstellung kann das neue Whitelist-Tag " &
            "daher keine entsprechenden Bilder zulassen.")

    End Sub

#End Region

End Class