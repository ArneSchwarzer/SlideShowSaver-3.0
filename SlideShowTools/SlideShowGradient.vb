Imports System.Windows.Media

Public Class SlideShowGradient

    Public Property Name As String

    Public ReadOnly Property Stops As List(Of SlideShowGradientStop)

    Public Sub New()

        Name = String.Empty
        Stops = New List(Of SlideShowGradientStop)()

    End Sub

End Class

Public Class SlideShowGradientStop

    Public Property Position As Double

    Public Property Farbe As Color

End Class