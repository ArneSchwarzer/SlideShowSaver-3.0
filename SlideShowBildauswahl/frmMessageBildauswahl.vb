Imports System.Drawing

Public Class frmMessageBildauswahl

    'Variablendeklarationen
    Private _tagText As String
    Private _isBlacklist As Boolean
    Private xList As String
    Private yList As String
    Private messageText As String

    Public Sub New(tagText As String, isBlacklist As Boolean)
        InitializeComponent() ' MUSS als erster Aufruf stehen
        _tagText = tagText
        _isBlacklist = isBlacklist
    End Sub

    Private Sub frmFolderBildauswahl_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TopMost = True
        If _isBlacklist Then
            xList = "Black-List"
            yList = "White-List"
        Else
            xList = "White-List"
            yList = "Black-List"
        End If

        rtxMessage.Clear()
        rtxMessage.SelectionFont = New Font("Segoe UI", 10, FontStyle.Regular)
        rtxMessage.AppendText("Du versuchst gerade, das Tag ")
        rtxMessage.SelectionFont = New Font("Segoe UI", 10, FontStyle.Bold)
        rtxMessage.AppendText(_tagText)
        rtxMessage.SelectionFont = New Font("Segoe UI", 10, FontStyle.Regular)
        rtxMessage.AppendText(" in die " & xList & " einzutragen, obwohl es bereits in der " & yList & " vorhanden ist. Das kann nicht funktionieren, da sonst gar kein Bild mehr angezeigt werden kann. Wenn Du ")
        rtxMessage.SelectionFont = New Font("Segoe UI", 10, FontStyle.Bold)
        rtxMessage.AppendText(_tagText)
        rtxMessage.SelectionFont = New Font("Segoe UI", 10, FontStyle.Regular)
        rtxMessage.AppendText(" in die " & xList & " eintragen möchtest, lösche das Tag bitte erst aus der " & yList & " .")

    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub
End Class