Public Class DuenenGenerator

#Region "Konstanten"

    Private Const DUENENFELD_BREITE As Integer = 256

    Private Const FBM_OKTAVEN As Integer = 3

    Private Const FBM_PERSISTENZ As Single = 0.45F
    Private Const FBM_LAKUNARITAET As Single = 2.0F

    Private Const DUENEN_FREQUENZ_MIN As Single = 3.5F
    Private Const DUENEN_FREQUENZ_MAX As Single = 5.5F

    Private Const DOMAIN_WARP_ENTLANG As Single = 0.1F
    Private Const DOMAIN_WARP_QUER As Single = 0.07F

    Private Const KANTEN_WELLIGKEIT As Single = 0.16F

#End Region

#Region "Variablendeklaration"

    Private permutation() As Integer

#End Region

#Region "Erzeugung"

    Public Function ErzeugeDuenenFeld(renderBreite As Integer, renderHoehe As Integer, Optional seed As _
                                      Integer = 0) As DuenenFeldDaten

        Dim daten As DuenenFeldDaten
        Dim zufall As Random

        Dim feldBreite As Integer
        Dim feldHoehe As Integer

        Dim historischeWindRichtung As Single

        Dim cosRichtung As Single
        Dim sinRichtung As Single

        Dim duenenFrequenz As Single
        Dim phaseOffset As Single

        Dim x As Integer
        Dim y As Integer
        Dim index As Integer

        Dim normX As Single
        Dim normY As Single

        Dim entlang As Single
        Dim quer As Single

        Dim warpEntlang As Single
        Dim warpQuer As Single

        Dim entlangVerformt As Single
        Dim querVerformt As Single

        Dim kantenWelle As Single

        Dim phase As Single
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

        InitialisierePermutation(zufall)

        feldBreite = DUENENFELD_BREITE

        feldHoehe = Math.Max(1, CInt(Math.Round(feldBreite * renderHoehe / CDbl(renderBreite))))

        daten = New DuenenFeldDaten()

        daten.breite = feldBreite
        daten.hoehe = feldHoehe
        daten.werte = New Single(feldBreite * feldHoehe - 1) {}


        ' Diese Richtung beschreibt ausdrücklich NICHT
        ' den aktuellen Wind.
        '
        ' Sie repräsentiert die historische Windrichtung,
        ' unter der die Dünen entstanden sind.

        historischeWindRichtung = CSng(zufall.NextDouble() * Math.PI * 2.0)

        cosRichtung = CSng(Math.Cos(historischeWindRichtung))

        sinRichtung = CSng(Math.Sin(historischeWindRichtung))

        duenenFrequenz = DUENEN_FREQUENZ_MIN + CSng(zufall.NextDouble()) * (DUENEN_FREQUENZ_MAX -
                                                    DUENEN_FREQUENZ_MIN)

        phaseOffset = CSng(zufall.NextDouble())

        For y = 0 To feldHoehe - 1

            For x = 0 To feldBreite - 1

                index = y * feldBreite + x


                ' Normierter Bildschirmraum mit Ursprung
                ' in der Mitte.

                normX = CSng(x) / CSng(Math.Max(1, feldBreite - 1)) - 0.5F
                normY = CSng(y) / CSng(Math.Max(1, feldHoehe - 1)) - 0.5F


                ' In den historischen Dünenraum drehen.

                entlang = normX * cosRichtung + normY * sinRichtung
                quer = -normX * sinRichtung + normY * cosRichtung


                ' Sehr weicher Domain-Warp.
                '
                ' Dadurch bleiben die Dünenkanten lang und
                ' glatt, statt zu Wasser-/Noise-Gekräusel
                ' zu zerfallen.

                warpEntlang = BerechneFBM(quer * 1.1F + 13.71F, entlang * 0.4F + 37.19F) * DOMAIN_WARP_ENTLANG
                warpQuer = BerechneFBM(quer * 0.85F + 71.31F, entlang * 0.55F + 19.43F) * DOMAIN_WARP_QUER

                entlangVerformt = entlang + warpEntlang
                querVerformt = quer + warpQuer


                ' Langgezogene, weiche Welligkeit
                ' der eigentlichen Dünenkante.

                kantenWelle = BerechneFBM(querVerformt * 1.45F + 91.17F, entlangVerformt * 0.18F + 7.53F) * KANTEN_WELLIGKEIT


                ' Periodisches Feld:
                '
                ' Jede Ganzzahlgrenze ist eine neue
                ' scharfe Dünenkante.

                phase = entlangVerformt * duenenFrequenz + kantenWelle + phaseOffset


                ' Nachkommaanteil 0..1:
                '
                ' 0 = unmittelbar an der Dünenkante
                ' 1 = Ende des jeweiligen Dünenhanges

                abloeseWert = phase - CSng(Math.Floor(phase))

                daten.werte(index) = Begrenze01(abloeseWert)

            Next

        Next

        Return daten

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