Public NotInheritable Class MandelbrotMathematik

#Region "Konstanten"

    Private Const minZoomFaktor As Double = 0.985
    Private Const maxZoomFaktor As Double = 0.7
    Private Const minZoomgeschwindigkeit As Double = 1.0
    Private Const maxZoomgeschwindigkeit As Double = 100.0

    Private Const iterationsFaktor As Double = 35.0
    Private Const maxIterationen As Integer = 2000

#End Region

#Region "Konstruktor"

    Private Sub New()
        'Statische Mathematikklasse
    End Sub

#End Region

#Region "Interpolation"

    Public Shared Function Lerp(startwert As Double, endwert As Double, position As Double) As Double
        'Interpoliert linear zwischen zwei Double-Werten.

        Return startwert + (endwert - startwert) * position

    End Function

    Public Shared Function EaseInOut(position As Double) As Double
        'Interpoliert weich zwischen Anfang und Ende.

        Return position * position * (3.0 - 2.0 * position)

    End Function

    Public Shared Function EaseOutCubic(position As Double) As Double
        'Berechnet eine kubische Ease-Out-Kurve.

        position = Math.Max(0.0, Math.Min(1.0, position))

        Return 1.0 - Math.Pow(1.0 - position, 3.0)

    End Function

#End Region

#Region "Zoom"

    Public Shared Function BerechneZoomFaktorProSekunde(zoomgeschwindigkeit As Integer) As Double
        'Berechnet den exponentiellen Zoomfaktor pro Sekunde.

        Dim slider As Double
        Dim position As Double

        slider = Math.Max(minZoomgeschwindigkeit, Math.Min(maxZoomgeschwindigkeit, CDbl(zoomgeschwindigkeit)))
        position = (slider - minZoomgeschwindigkeit) / (maxZoomgeschwindigkeit - minZoomgeschwindigkeit)

        Return Lerp(minZoomFaktor, maxZoomFaktor, position)

    End Function

    Public Shared Function BerechneGesamtZoomdauerSekunden(startSkala As Double, zielSkala As Double,
        zoomgeschwindigkeit As Integer) As Double
        'Berechnet die benötigte Dauer vom Start- bis zum Zielmaßstab.

        Dim zoomFaktorProSekunde As Double
        Dim skalenVerhaeltnis As Double

        If startSkala <= 0.0 Then
            Return 0.0
        End If

        If zielSkala <= 0.0 OrElse zielSkala >= startSkala Then

            Return 0.0

        End If

        zoomFaktorProSekunde = BerechneZoomFaktorProSekunde(zoomgeschwindigkeit)
        skalenVerhaeltnis = zielSkala / startSkala

        Return Math.Log(skalenVerhaeltnis) / Math.Log(zoomFaktorProSekunde)

    End Function

    Public Shared Function BerechneAktuelleSkala(startSkala As Double, vergangeneSekunden As Double,
        zoomgeschwindigkeit As Integer) As Double
        'Berechnet den Maßstab nach einer bestimmten Zoomdauer.

        Dim zoomFaktorProSekunde As Double

        zoomFaktorProSekunde = BerechneZoomFaktorProSekunde(zoomgeschwindigkeit)

        Return startSkala * Math.Pow(zoomFaktorProSekunde, vergangeneSekunden)

    End Function

#End Region

#Region "Iterationen"

    Public Shared Function BerechneMaxIterationen(startSkala As Double, aktuelleSkala As Double,
                                                  startIterationen As Integer) As Integer
        'Berechnet die Iterationszahl anhand der aktuellen Zoomtiefe.

        Dim zoomTiefe As Double
        Dim berechneteIterationen As Integer

        If startSkala <= 0.0 OrElse aktuelleSkala <= 0.0 Then

            Return startIterationen

        End If

        zoomTiefe = Math.Log(startSkala / aktuelleSkala, 2.0)
        berechneteIterationen = CInt(Math.Round(startIterationen + zoomTiefe * iterationsFaktor))

        Return Math.Max(startIterationen, Math.Min(berechneteIterationen, maxIterationen))

    End Function

#End Region

#Region "Rotation"

    Public Shared Function BerechneZielRotation(zufall As Random, minWinkelGrad As Double,
                                                maxWinkelGrad As Double) As Double
        'Berechnet einen zufälligen Zielwinkel in Radiant.

        Dim richtung As Double
        Dim winkelGrad As Double

        If zufall Is Nothing Then

            Throw New ArgumentNullException(NameOf(zufall))

        End If

        If minWinkelGrad < 0.0 Then

            Throw New ArgumentOutOfRangeException(NameOf(minWinkelGrad))

        End If

        If maxWinkelGrad < minWinkelGrad Then

            Throw New ArgumentOutOfRangeException(NameOf(maxWinkelGrad))

        End If

        If zufall.Next(2) = 0 Then

            richtung = -1.0

        Else

            richtung = 1.0

        End If

        winkelGrad = zufall.NextDouble() * (maxWinkelGrad - minWinkelGrad) + minWinkelGrad

        Return richtung * winkelGrad * Math.PI / 180.0

    End Function

#End Region

#Region "Double-Single"

    Public Shared Sub SplitDouble(wert As Double, ByRef high As Single, ByRef low As Single)
        'Zerlegt einen Double-Wert in High- und Low-Single-Anteile.

        high = CSng(wert)
        low = CSng(wert - CDbl(high))

    End Sub

#End Region

End Class