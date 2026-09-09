Public Class MandelbrotZiel

    Public Property Name As String
    Public Property CenterX As Double
    Public Property CenterY As Double
    Public Property TargetScale As Double

    Public Sub New()

        Name = String.Empty

    End Sub

    Public Function ErzeugeKopie() As MandelbrotZiel
        'Erzeugt eine unabhängige Kopie des Mandelbrot-Ziels.

        Return New MandelbrotZiel() With {
            .Name = Name,
            .CenterX = CenterX,
            .CenterY = CenterY,
            .TargetScale = TargetScale
        }

    End Function

End Class