Public Class MandelbrotKameraZustand

    Public Enum KameraPhaseTyp

        FreezeIn
        Translation
        Cruise
        EaseOut
        FreezeOut

    End Enum

    Public Property CenterX As Double
    Public Property CenterY As Double
    Public Property Skala As Double
    Public Property MaxIterationen As Integer

    Public Property Rotation As Double
    Public Property ZielRotation As Double

    Public Property Phase As KameraPhaseTyp
    Public Property PhasenFortschritt As Double
    Public Property KamerafahrtSekunden As Double

    Public Property KamerafahrtBeendet As Boolean

End Class