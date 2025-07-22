Imports System.Windows.Forms
Imports System.Windows.Input

Public Class KeyAndMouseHandling

    ' ================
    ' Events
    ' ================
    Public Shared Event GlobalKeyDown(sender As Object, e As Windows.Forms.KeyEventArgs)
    Public Shared Event GlobalMouseDown(sender As Object, e As Windows.Forms.MouseEventArgs)

    ' ================
    ' Öffentliche Methoden zum Weiterleiten von Events
    ' ================

    ''' <summary>
    ''' Leitet ein KeyDown-Ereignis an den globalen Event-Handler weiter.
    ''' </summary>
    ''' <param name="sender">Das Objekt, das das Ereignis ausgelöst hat.</param>
    ''' <param name="e">Das ursprüngliche KeyEventArgs-Objekt.</param>
    Public Shared Sub ForwardKeyDown(sender As Object, e As Windows.Forms.KeyEventArgs)
        RaiseEvent GlobalKeyDown(sender, e)
    End Sub

    ''' <summary>
    ''' Leitet ein MouseDown-Ereignis an den globalen Event-Handler weiter.
    ''' </summary>
    ''' <param name="sender">Das Objekt, das das Ereignis ausgelöst hat.</param>
    ''' <param name="e">Das ursprüngliche MouseEventArgs-Objekt.</param>
    Public Shared Sub ForwardMouseDown(sender As Object, e As Windows.Forms.MouseEventArgs)
        RaiseEvent GlobalMouseDown(sender, e)
    End Sub

    ' ================
    ' Komfortmethoden (typed)
    ' ================

    ''' <summary>
    ''' Wird aufgerufen, wenn eine bestimmte Taste erkannt wurde.
    ''' </summary>
    Public Shared Sub ForwardKeyDown(sender As Object, keyCode As Keys)
        Dim args As New Windows.Forms.KeyEventArgs(keyCode)
        RaiseEvent GlobalKeyDown(sender, args)
    End Sub

    ''' <summary>
    ''' Wird aufgerufen, wenn ein Mausklick erkannt wurde.
    ''' </summary>
    Public Shared Sub ForwardMouseDown(sender As Object, button As MouseButtons, x As Integer, y As Integer)
        Dim args As New Windows.Forms.MouseEventArgs(button, 1, x, y, 0)
        RaiseEvent GlobalMouseDown(sender, args)
    End Sub

    ' ================
    ' WPF Versionen der Methoden
    ' ================

    Public Shared Sub ForwardKeyDownWPF(sender As Object, e As Windows.Input.KeyEventArgs)
        ' Wandle WPF-Key in WinForms-Key
        Dim keyCode As Windows.Forms.Keys = CType(KeyInterop.VirtualKeyFromKey(e.Key), System.Windows.Forms.Keys)
        ForwardKeyDown(sender, keyCode)
    End Sub

    Public Shared Sub ForwardMouseDownWPF(sender As Object, e As MouseButtonEventArgs)
        ' Ermittele die gedrückte Maustaste
        Dim winFormsButton As Windows.Forms.MouseButtons

        Select Case e.ChangedButton
            Case MouseButton.Left
                winFormsButton = Windows.Forms.MouseButtons.Left
            Case MouseButton.Right
                winFormsButton = Windows.Forms.MouseButtons.Right
            Case MouseButton.Middle
                winFormsButton = Windows.Forms.MouseButtons.Middle
            Case MouseButton.XButton1
                winFormsButton = Windows.Forms.MouseButtons.XButton1
            Case MouseButton.XButton2
                winFormsButton = Windows.Forms.MouseButtons.XButton2
            Case Else
                winFormsButton = Windows.Forms.MouseButtons.None
        End Select

        ' Rufe WinForms-Version auf – X/Y sind hier 0,0 da irrelevant
        Dim args As New Windows.Forms.MouseEventArgs(winFormsButton, 1, 0, 0, 0)
        ForwardMouseDown(sender, args)
    End Sub

End Class

