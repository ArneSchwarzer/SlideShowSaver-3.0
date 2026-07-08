Imports System.Windows.Media

Public Class MandelbrotGradient
    Public Property Name As String
    Public Property Stops As New List(Of MandelbrotGradientStop)
End Class

Public Class MandelbrotGradientStop
    Public Property Position As Double
    Public Property Farbe As Color
End Class