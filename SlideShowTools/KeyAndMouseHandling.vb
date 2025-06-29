Imports System.Windows.Forms

Public Class KeyAndMouseHandling

    ' ================
    ' Events
    ' ================
    Public Shared Event GlobalKeyDown(sender As Object, e As KeyEventArgs)
    Public Shared Event GlobalMouseDown(sender As Object, e As MouseEventArgs)

    ' ================
    ' Öffentliche Methoden zum Weiterleiten von Events
    ' ================

    ''' <summary>
    ''' Leitet ein KeyDown-Ereignis an den globalen Event-Handler weiter.
    ''' </summary>
    ''' <param name="sender">Das Objekt, das das Ereignis ausgelöst hat.</param>
    ''' <param name="e">Das ursprüngliche KeyEventArgs-Objekt.</param>
    Public Shared Sub ForwardKeyDown(sender As Object, e As KeyEventArgs)
        RaiseEvent GlobalKeyDown(sender, e)
    End Sub

    ''' <summary>
    ''' Leitet ein MouseDown-Ereignis an den globalen Event-Handler weiter.
    ''' </summary>
    ''' <param name="sender">Das Objekt, das das Ereignis ausgelöst hat.</param>
    ''' <param name="e">Das ursprüngliche MouseEventArgs-Objekt.</param>
    Public Shared Sub ForwardMouseDown(sender As Object, e As MouseEventArgs)
        RaiseEvent GlobalMouseDown(sender, e)
    End Sub

    ' ================
    ' Komfortmethoden (typed)
    ' ================

    ''' <summary>
    ''' Wird aufgerufen, wenn eine bestimmte Taste erkannt wurde.
    ''' </summary>
    Public Shared Sub ForwardKeyDown(sender As Object, keyCode As Keys)
        Dim args As New KeyEventArgs(keyCode)
        RaiseEvent GlobalKeyDown(sender, args)
    End Sub

    ''' <summary>
    ''' Wird aufgerufen, wenn ein Mausklick erkannt wurde.
    ''' </summary>
    Public Shared Sub ForwardMouseDown(sender As Object, button As MouseButtons, x As Integer, y As Integer)
        Dim args As New MouseEventArgs(button, 1, x, y, 0)
        RaiseEvent GlobalMouseDown(sender, args)
    End Sub

End Class

