Public Class DuenenGenerator

#Region "Konstanten"

    Private Const DUENENFELD_BREITE As Integer = 256

    Private Const FBM_OKTAVEN As Integer = 3

    Private Const FBM_PERSISTENZ As Single = 0.45F
    Private Const FBM_LAKUNARITAET As Single = 2.0F

    Private Const DUENEN_ANZAHL_MIN As Integer = 12
    Private Const DUENEN_ANZAHL_MAX As Integer = 22

    Private Const DUENEN_LAENGE_MIN As Single = 0.22F
    Private Const DUENEN_LAENGE_MAX As Single = 0.55F

    Private Const DUENEN_TIEFE_MIN As Single = 0.12F
    Private Const DUENEN_TIEFE_MAX As Single = 0.28F

    Private Const RICHTUNGS_ABWEICHUNG As Single = 0.22F

    Private Const WELLEN_AMPLITUDE_MIN As Single = 0.008F
    Private Const WELLEN_AMPLITUDE_MAX As Single = 0.025F

    Private Const WELLEN_FREQUENZ_MIN As Single = 1.0F
    Private Const WELLEN_FREQUENZ_MAX As Single = 2.5F

    Private Const STARTWERT_MIN As Single = 0.02F
    Private Const STARTWERT_MAX As Single = 0.32F

#End Region

#Region "Variablendeklaration"

    Private permutation() As Integer

#End Region

#Region "Struct und Enums"

    Private Structure Duene

        Public mittelpunktX As Single
        Public mittelpunktY As Single

        Public laenge As Single
        Public tiefe As Single

        Public winkel As Single

        Public wellenAmplitude As Single
        Public wellenFrequenz As Single
        Public wellenPhase As Single

        Public startWert As Single

    End Structure

#End Region

#Region "Erzeugung"

    Public Function ErzeugeDuenenFeld(renderBreite As Integer, renderHoehe As Integer, Optional seed As Integer = 0) As DuenenFeldDaten

        Dim daten As DuenenFeldDaten
        Dim zufall As Random

        Dim duenen() As Duene

        Dim feldBreite As Integer
        Dim feldHoehe As Integer

        Dim duenenAnzahl As Integer
        Dim historischeWindRichtung As Single

        Dim x As Integer
        Dim y As Integer
        Dim index As Integer

        Dim normX As Single
        Dim normY As Single

        Dim abloeseWert As Single

        If renderBreite <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(renderBreite))
        End If

        If renderHoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(renderHoehe))
        End If

        If seed = 0 Then
            seed = Environment.TickCount
        End If

        zufall = New Random(seed)

        feldBreite = DUENENFELD_BREITE

        feldHoehe = Math.Max(1, CInt(Math.Round(feldBreite * renderHoehe / CDbl(renderBreite))))

        daten = New DuenenFeldDaten()

        daten.breite = feldBreite
        daten.hoehe = feldHoehe
        daten.werte = New Single(feldBreite * feldHoehe - 1) {}

        historischeWindRichtung = CSng(zufall.NextDouble() * Math.PI * 2.0)

        duenenAnzahl = zufall.Next(DUENEN_ANZAHL_MIN, DUENEN_ANZAHL_MAX + 1)

        duenen = ErzeugeDuenen(duenenAnzahl, historischeWindRichtung, zufall)

        For y = 0 To feldHoehe - 1

            For x = 0 To feldBreite - 1

                index = y * feldBreite + x

                normX = CSng(x) / CSng(Math.Max(1, feldBreite - 1))
                normY = CSng(y) / CSng(Math.Max(1, feldHoehe - 1))

                abloeseWert = BerechneAbloeseWert(normX, normY, duenen)

                daten.werte(index) = Begrenze01(abloeseWert)

            Next

        Next

        Return daten

    End Function

    Private Function ErzeugeDuenen(
    anzahl As Integer,
    historischeWindRichtung As Single,
    zufall As Random) As Duene()

        Dim duenen() As Duene

        Dim i As Integer

        duenen = New Duene(anzahl - 1) {}

        For i = 0 To anzahl - 1

            duenen(i).mittelpunktX = CSng(zufall.NextDouble())
            duenen(i).mittelpunktY = CSng(zufall.NextDouble())

            duenen(i).laenge = DUENEN_LAENGE_MIN + CSng(zufall.NextDouble()) * (DUENEN_LAENGE_MAX -
                                                                                DUENEN_LAENGE_MIN)

            duenen(i).tiefe = DUENEN_TIEFE_MIN + CSng(zufall.NextDouble()) * (DUENEN_TIEFE_MAX - DUENEN_TIEFE_MIN)

            duenen(i).winkel = historischeWindRichtung + (CSng(zufall.NextDouble()) * 2.0F - 1.0F) *
                               RICHTUNGS_ABWEICHUNG

            duenen(i).wellenAmplitude = WELLEN_AMPLITUDE_MIN + CSng(zufall.NextDouble()) * (WELLEN_AMPLITUDE_MAX -
                                        WELLEN_AMPLITUDE_MIN)

            duenen(i).wellenFrequenz = WELLEN_FREQUENZ_MIN + CSng(zufall.NextDouble()) * (WELLEN_FREQUENZ_MAX -
                                       WELLEN_FREQUENZ_MIN)

            duenen(i).wellenPhase = CSng(zufall.NextDouble() * Math.PI * 2.0)

            duenen(i).startWert = STARTWERT_MIN + CSng(zufall.NextDouble()) * (STARTWERT_MAX - STARTWERT_MIN)

        Next

        Return duenen

    End Function

    Private Function BerechneAbloeseWert(x As Single, y As Single, duenen() As Duene) As Single

        Dim besterWert As Single
        Dim duenenWert As Single

        Dim i As Integer

        besterWert = 1.0F

        For i = 0 To duenen.Length - 1

            duenenWert = BerechneDuenenWert(x, y, duenen(i))

            If duenenWert < besterWert Then

                besterWert = duenenWert

            End If

        Next

        Return besterWert

    End Function

    Private Function BerechneDuenenWert(x As Single, y As Single, duene As Duene) As Single

        Dim deltaX As Single
        Dim deltaY As Single

        Dim cosWinkel As Single
        Dim sinWinkel As Single

        Dim entlang As Single
        Dim hang As Single

        Dim normEntlang As Single
        Dim wellenOffset As Single

        Dim normHang As Single
        Dim randAbschwaechung As Single

        Dim wert As Single

        deltaX = x - duene.mittelpunktX
        deltaY = y - duene.mittelpunktY

        cosWinkel = CSng(Math.Cos(duene.winkel))
        sinWinkel = CSng(Math.Sin(duene.winkel))

        ' entlang:
        ' Position entlang der Dünenkante.

        entlang = deltaX * cosWinkel + deltaY * sinWinkel

        ' hang:
        ' Position senkrecht zur Dünenkante.

        hang = -deltaX * sinWinkel + deltaY * cosWinkel

        normEntlang = entlang / Math.Max(duene.laenge * 0.5F, 0.0001F)

        If Math.Abs(normEntlang) > 1.0F Then

            Return 1.0F

        End If


        ' Nur eine sanfte, lange Welle pro Dünenzug.

        wellenOffset = CSng(Math.Sin(normEntlang * Math.PI * duene.wellenFrequenz + duene.wellenPhase)) *
                       duene.wellenAmplitude

        hang -= wellenOffset


        ' Nur die windabgewandte Hangseite gehört
        ' zu dieser Düne.

        If hang < 0.0F OrElse hang > duene.tiefe Then
            Return 1.0F
        End If

        normHang = hang / duene.tiefe


        ' An den Enden läuft die Düne weich aus.
        ' Dadurch entstehen keine abgeschnittenen Rechtecke.

        randAbschwaechung = normEntlang * normEntlang

        normHang += randAbschwaechung * 0.35F

        normHang = Begrenze01(normHang)


        ' Jede Düne beginnt bei ihrem eigenen Startwert.

        wert = duene.startWert + normHang * (1.0F - duene.startWert)

        Return Begrenze01(wert)

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

    Private Function Lerp(a As Single, b As Single, faktor As Single) As Single

        Return a + (b - a) * faktor

    End Function

    Private Function Begrenze01(wert As Single) As Single

        Return Math.Max(0.0F, Math.Min(1.0F, wert))

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