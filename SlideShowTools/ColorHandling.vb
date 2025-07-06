Imports System.Drawing

Public Class ColorHandling

    Public Shared Function ColorToString(color As Color) As String
        Return $"{color.R},{color.G},{color.B},{color.A}"
    End Function

    Public Shared Function StringToColor(value As String) As Color
        Dim teile() As String = value.Split(","c)
        If teile.Length = 3 Then
            Return Color.FromArgb(255, CInt(teile(0)), CInt(teile(1)), CInt(teile(2)))
        ElseIf teile.Length = 4 Then
            Return Color.FromArgb(CInt(teile(3)), CInt(teile(0)), CInt(teile(1)), CInt(teile(2)))
        Else
            Return Color.Black ' oder Throw New FormatException
        End If
    End Function

End Class
