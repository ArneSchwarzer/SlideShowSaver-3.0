Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows.Forms


Public Module CursorHandling

        <DllImport("user32.dll")>
        Private Function GetCursorInfo(ByRef pci As CURSORINFO) As Boolean
        End Function

        <StructLayout(LayoutKind.Sequential)>
        Private Structure CURSORINFO
            Public cbSize As Integer
            Public flags As Integer
            Public hCursor As IntPtr
            Public ptScreenPos As Point
        End Structure

        Private Const CURSOR_SHOWING As Integer = &H1

        ''' <summary>
        ''' Prüft, ob der Cursor aktuell sichtbar ist.
        ''' </summary>
        Public Function IsCursorVisible() As Boolean
            Dim ci As CURSORINFO
            ci.cbSize = Marshal.SizeOf(ci)
            GetCursorInfo(ci)
            Return (ci.flags And CURSOR_SHOWING) = CURSOR_SHOWING
        End Function

        ''' <summary>
        ''' Macht den Cursor sichtbar – egal wie oft Cursor.Hide() vorher aufgerufen wurde.
        ''' </summary>
        Public Sub CursorPowerShow()
            Dim maxVersuche As Integer = 20 ' Absicherung gegen Endlosschleifen
            Dim versuch As Integer = 0

            Do While Not IsCursorVisible() AndAlso versuch < maxVersuche
                Cursor.Show()
                versuch += 1
            Loop
        End Sub

    End Module


