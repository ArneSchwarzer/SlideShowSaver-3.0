Imports System.Drawing
Imports System.Windows.Media


Public Class ColorHandling

    Public Shared Function ColorToString(color As System.Drawing.Color) As String
        'Wandelt eine System.Drawing.Color in einen RGBA-String

        Return $"{color.R},{color.G},{color.B},{color.A}"

    End Function

    Public Shared Function StringToColor(value As String) As System.Drawing.Color
        'Wandelt einen RGBA- oder einen RGB-String in eine System.Drawing.Color

        Dim teile() As String = value.Split(","c)
        If teile.Length = 3 Then
            'Wandlung eines RGB-Strings
            Return System.Drawing.Color.FromArgb(255, CInt(teile(0)), CInt(teile(1)), CInt(teile(2)))
        ElseIf teile.Length = 4 Then
            'Wandlung eines RGBA-Strings
            Return System.Drawing.Color.FromArgb(CInt(teile(3)), CInt(teile(0)), CInt(teile(1)), CInt(teile(2)))
        Else
            'Fallback: Schwarz ausgeben
            Return System.Drawing.Color.Black ' oder Throw New FormatException
        End If

    End Function

    Public Shared Function InvertSDColor(c As System.Drawing.Color) As System.Drawing.Color
        'Invertiert eine System.Drawing.Color. Der Alpha-Wert bleibt erhalten.

        Return System.Drawing.Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B)

    End Function

    Public Shared Function InvertWMColor(c As Windows.Media.Color) As Windows.Media.Color
        'Invertiert eine Windows.Media.Color. Der Alpha-Wert bleibt erhalten

        Return Windows.Media.Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B)

    End Function

    Public Shared Function SDColorToWMColor(c As System.Drawing.Color) As System.Windows.Media.Color
        'System.Drawing.Color → System.Windows.Media.Color

        Return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B)

    End Function

    Public Shared Function WMColorToSDColor(c As System.Windows.Media.Color) As System.Drawing.Color
        'System.Windows.Media.Color → System.Drawing.Color

        Return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B)

    End Function

End Class
