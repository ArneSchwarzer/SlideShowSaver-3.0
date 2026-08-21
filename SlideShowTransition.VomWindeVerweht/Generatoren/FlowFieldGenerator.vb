Imports System.Numerics

Public Class FlowFieldGenerator

#Region "Konstanten"

    Private Const FLOWFIELD_BREITE As Integer = 256

    Private Const FBM_OKTAVEN As Integer = 4

    Private Const FBM_PERSISTENZ As Single = 0.5F
    Private Const FBM_LAKUNARITAET As Single = 2.0F

    Private Const MIN_VORWAERTSFAKTOR As Single = 0.35F

#End Region

#Region "Variablendeklaration"

    Private permutation() As Integer

#End Region

#Region "Erzeugung"

    Public Function ErzeugeFlowField(renderBreite As Integer, renderHoehe As Integer, windStaerke As Integer,
                                     windRichtung As Single, Optional seed As Integer = 0) As FlowFieldDaten

        Dim daten As FlowFieldDaten
        Dim zufall As Random

        Dim flowBreite As Integer
        Dim flowHoehe As Integer

        Dim x As Integer
        Dim y As Integer
        Dim index As Integer

        Dim normX As Single
        Dim normY As Single

        Dim noiseX As Single
        Dim noiseY As Single

        Dim basisGeschwindigkeit As Single
        Dim turbulenzStaerke As Single
        Dim minimaleVorwaertsGeschwindigkeit As Single

        Dim vx As Single
        Dim vy As Single

        If renderBreite <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(renderBreite))
        End If

        If renderHoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(renderHoehe))
        End If

        If windStaerke < 0 OrElse windStaerke > 12 Then
            Throw New ArgumentOutOfRangeException(NameOf(windStaerke))
        End If

        If windRichtung = 0.0F AndAlso windStaerke > 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(windRichtung))
        End If

        If seed = 0 Then
            seed = Environment.TickCount
        End If

        zufall = New Random(seed)

        InitialisierePermutation(zufall)

        flowBreite = FLOWFIELD_BREITE

        flowHoehe = Math.Max(1, CInt(Math.Round(flowBreite * renderHoehe / CDbl(renderBreite))))

        daten = New FlowFieldDaten()

        daten.breite = flowBreite
        daten.hoehe = flowHoehe
        daten.vektoren = New Vector2(flowBreite * flowHoehe - 1) {}

        basisGeschwindigkeit = BerechneWindGeschwindigkeit(windStaerke)

        turbulenzStaerke = BerechneTurbulenzStaerke(windStaerke)

        minimaleVorwaertsGeschwindigkeit = basisGeschwindigkeit * MIN_VORWAERTSFAKTOR

        For y = 0 To flowHoehe - 1

            For x = 0 To flowBreite - 1

                index =
                    y * flowBreite +
                    x

                If windStaerke = 0 Then

                    daten.vektoren(index) = Vector2.Zero

                    Continue For

                End If

                normX = CSng(x) / CSng(Math.Max(1, flowBreite - 1))
                normY = CSng(y) / CSng(Math.Max(1, flowHoehe - 1))

                ' Zwei voneinander versetzte FBM-Felder.

                noiseX = BerechneFBM(normX * 3.0F, normY * 3.0F)
                noiseY = BerechneFBM(normX * 3.0F + 17.37F, normY * 3.0F + 41.91F)


                ' Hauptwind + Turbulenz.

                vx = basisGeschwindigkeit + noiseX * turbulenzStaerke
                vy = noiseY * turbulenzStaerke

                ' Keine geschlossenen Strudel:

                ' Selbst in der stärksten lokalen Gegenströmung
                ' bleibt immer eine positive Vorwärtskomponente.

                vx = Math.Max(minimaleVorwaertsGeschwindigkeit, vx)


                ' Richtung erst ganz am Ende anwenden.

                If windRichtung < 0.0F Then
                    vx = -vx
                End If

                daten.vektoren(index) = New Vector2(vx, vy)

            Next

        Next

        Return daten

    End Function

#End Region

#Region "Wind"

    Private Function BerechneWindGeschwindigkeit(windStaerke As Integer) As Single

        Dim normiert As Single
        Dim geschwindigkeit As Single

        If windStaerke <= 0 Then
            Return 0.0F
        End If

        normiert = CSng(windStaerke) / 12.0F


        ' Vorerst bewusst keine physikalische Umrechnung
        ' von Beaufort in m/s.
        '
        ' Das Ergebnis sind Pixel pro Sekunde.

        geschwindigkeit = 90.0F + 1110.0F * CSng(Math.Pow(normiert, 1.5))

        Return geschwindigkeit

    End Function

    Private Function BerechneTurbulenzStaerke(windStaerke As Integer) As Single

        Dim normiert As Single

        If windStaerke <= 0 Then
            Return 0.0F
        End If

        normiert = CSng(windStaerke) / 12.0F

        Return 45.0F + 300.0F * normiert

    End Function

#End Region

#Region "FBM"

    Private Function BerechneFBM(x As Single, y As Single) As Single

        Dim summe As Single
        Dim amplitude As Single
        Dim frequenz As Single
        Dim amplitudeSumme As Single

        Dim oktave As Integer

        summe = 0.0F
        amplitude = 1.0F
        frequenz = 1.0F
        amplitudeSumme = 0.0F

        For oktave = 0 To FBM_OKTAVEN - 1

            summe += BerechneValueNoise(x * frequenz, y * frequenz) * amplitude

            amplitudeSumme += amplitude

            frequenz *= FBM_LAKUNARITAET

            amplitude *= FBM_PERSISTENZ

        Next

        If amplitudeSumme <= 0.0F Then
            Return 0.0F
        End If

        Return summe / amplitudeSumme

    End Function

    Private Function BerechneValueNoise(x As Single, y As Single) As Single

        Dim x0 As Integer
        Dim y0 As Integer

        Dim x1 As Integer
        Dim y1 As Integer

        Dim tx As Single
        Dim ty As Single

        Dim sx As Single
        Dim sy As Single

        Dim n00 As Single
        Dim n10 As Single
        Dim n01 As Single
        Dim n11 As Single

        Dim oben As Single
        Dim unten As Single

        x0 = CInt(Math.Floor(x))
        y0 = CInt(Math.Floor(y))

        x1 = x0 + 1
        y1 = y0 + 1

        tx = x - x0
        ty = y - y0

        sx = Fade(tx)
        sy = Fade(ty)

        n00 = HashNoise(x0, y0)
        n10 = HashNoise(x1, y0)
        n01 = HashNoise(x0, y1)
        n11 = HashNoise(x1, y1)

        oben = Lerp(n00, n10, sx)

        unten = Lerp(n01, n11, sx)

        Return Lerp(oben, unten, sy)

    End Function

    Private Function HashNoise(x As Integer, y As Integer) As Single

        Dim index As Integer
        Dim wert As Integer

        index = permutation(x And 255)
        index = permutation((index + y) And 255)

        wert = permutation(index)

        Return CSng(wert) / 127.5F - 1.0F

    End Function

    Private Function Fade(wert As Single) As Single

        Return wert * wert * wert * (wert * (wert * 6.0F - 15.0F) + 10.0F)

    End Function

    Private Function Lerp(
        a As Single,
        b As Single,
        faktor As Single) As Single

        Return a + (b - a) * faktor

    End Function

#End Region

#Region "Permutation"

    Private Sub InitialisierePermutation(zufall As Random)

        Dim werte(255) As Integer

        Dim i As Integer
        Dim j As Integer
        Dim temp As Integer

        For i = 0 To 255
            werte(i) = i
        Next

        For i = 255 To 1 Step -1

            j = zufall.Next(0, i + 1)

            temp = werte(i)
            werte(i) = werte(j)
            werte(j) = temp

        Next

        permutation = New Integer(511) {}

        For i = 0 To 511
            permutation(i) = werte(i And 255)
        Next

    End Sub

#End Region

End Class