Imports System.Numerics

Public Class RandAbloeseGenerator

#Region "Konstanten"

    Private Const RANDABLOESE_MAX_BREITE As Integer = 960

    Private Const URSPRUNG_ABSTAND As Single = 0.12F
    Private Const MAX_AUFWIND_WINKEL As Single = CSng(Math.PI / 6.0)

    Private Const FBM_OKTAVEN As Integer = 4
    Private Const FBM_PERSISTENZ As Single = 0.5F
    Private Const FBM_LAKUNARITAET As Single = 2.0F

    Private Const FBM_GRUNDFREQUENZ As Single = 2.2F
    Private Const FBM_STAERKE As Single = 0.14F

#End Region

#Region "Variablendeklaration"

    Private permutation() As Integer

#End Region

#Region "Erzeugung"

    Public Function ErzeugeRandAbloeseFeld(renderBreite As Integer, renderHoehe As Integer,
                                           schwerkraftAktiv As Boolean, Optional seed As Integer = 0) As RandAbloeseFeldDaten

        Dim daten As RandAbloeseFeldDaten
        Dim zufall As Random

        Dim feldBreite As Integer
        Dim feldHoehe As Integer

        Dim minimaleDistanz As Single
        Dim maximaleDistanz As Single

        Dim distanz As Single
        Dim normierteDistanz As Single

        Dim noiseOffsetX As Single
        Dim noiseOffsetY As Single

        Dim noiseWert As Single
        Dim noiseEinfluss As Single

        Dim abloeseWert As Single

        Dim x As Integer
        Dim y As Integer
        Dim index As Integer

        Dim startPunkt As Vector2
        Dim mittelPunkt As Vector2
        Dim windRichtung As Vector2

        Dim ursprungX As Single
        Dim ursprungY As Single

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

        feldBreite = Math.Min(RANDABLOESE_MAX_BREITE, renderBreite)
        feldHoehe = Math.Max(1, CInt(Math.Round(feldBreite * renderHoehe / CDbl(renderBreite))))

        daten = New RandAbloeseFeldDaten()

        daten.breite = feldBreite
        daten.hoehe = feldHoehe

        daten.werte = New Single(feldBreite * feldHoehe - 1) {}

        ' -------------------------------------------------------
        ' Gemeinsame Geometrie für Abrisskante und Hauptwind.
        ' -------------------------------------------------------
        '
        ' Der RandAbloeseGenerator bestimmt EINMAL den Ursprung.
        '
        ' Aus diesem Ursprung entstehen anschließend:
        '
        ' 1. die radiale Abrisskante
        ' 2. die globale Hauptwindrichtung
        '
        ' Damit können beide niemals gegeneinander laufen.


        mittelPunkt = New Vector2(CSng(renderBreite) * 0.5F, CSng(renderHoehe) * 0.5F)

        If schwerkraftAktiv Then

            startPunkt = ErzeugeStartPunktMitSchwerkraft(renderBreite, renderHoehe, zufall)

        Else

            startPunkt = ErzeugeStartPunktTopView(renderBreite, renderHoehe, zufall)

        End If

        windRichtung = Vector2.Normalize(mittelPunkt - startPunkt)


        ' Der Ablöse-Texture arbeitet mit ihren eigenen Dimensionen.
        ' Deshalb den in Renderkoordinaten erzeugten Ursprung
        ' auf das Ablösefeld übertragen.

        ursprungX = startPunkt.X / CSng(renderBreite) * CSng(feldBreite)
        ursprungY = startPunkt.Y / CSng(renderHoehe) * CSng(feldHoehe)

        daten.startPunkt = startPunkt
        daten.windRichtung = windRichtung

        ' Der Ursprung liegt außerhalb des sichtbaren Bereichs.
        '
        ' Ohne Korrektur würde deshalb selbst der erste sichtbare
        ' Pixel bereits einen Ablösewert > 0 besitzen.
        '
        ' Wir ziehen die Entfernung zur nächstgelegenen sichtbaren
        ' Randposition ab. Die Ablösefront beginnt dadurch bei 0.
        '

        minimaleDistanz = BerechneMinimaleDistanzZumBild(ursprungX, ursprungY, feldBreite, feldHoehe)
        maximaleDistanz = BerechneMaximaleDistanzZuBildecke(ursprungX, ursprungY, feldBreite, feldHoehe)

        noiseOffsetX = CSng(zufall.NextDouble() * 1000.0)
        noiseOffsetY = CSng(zufall.NextDouble() * 1000.0)

        index = 0

        For y = 0 To feldHoehe - 1

            For x = 0 To feldBreite - 1

                distanz = BerechneDistanz(CSng(x), CSng(y), ursprungX, ursprungY)

                normierteDistanz = (distanz - minimaleDistanz) / Math.Max(maximaleDistanz - minimaleDistanz, 0.000001F)
                normierteDistanz = Begrenze01(normierteDistanz)


                ' Kohärente Verzerrung der Ablösefront.
                '
                ' Am Anfang bleibt die Front relativ klar lesbar.
                ' Mit wachsender Entfernung wird sie zunehmend
                ' organisch aufgebrochen.


                noiseWert = BerechneFBM(CSng(x) / CSng(feldBreite) + noiseOffsetX, CSng(y) / CSng(feldHoehe) + noiseOffsetY)

                noiseEinfluss = CSng(Math.Sqrt(normierteDistanz))

                abloeseWert = normierteDistanz + noiseWert * FBM_STAERKE * noiseEinfluss

                daten.werte(index) = Begrenze01(abloeseWert)

                index += 1

            Next

        Next

        Return daten

    End Function

    Private Function ErzeugeStartPunktMitSchwerkraft(renderBreite As Integer, renderHoehe As Integer,
                                                     zufall As Random) As Vector2

        Dim startX As Single
        Dim startY As Single

        Dim mittelY As Single
        Dim horizontalerAbstand As Single
        Dim maximalerYOffset As Single
        Dim yOffset As Single

        Dim kommtVonLinks As Boolean

        mittelY = CSng(renderHoehe) * 0.5F

        kommtVonLinks = zufall.Next(0, 2) = 0

        If kommtVonLinks Then

            startX = -CSng(renderBreite) * URSPRUNG_ABSTAND

        Else

            startX = CSng(renderBreite) * (1.0F + URSPRUNG_ABSTAND)

        End If


        ' Der Ursprung liegt bewusst unterhalb der Bildmitte.
        '
        ' Dadurch zeigt der Vektor vom Ursprung zum Mittelpunkt
        ' ausschließlich horizontal bis maximal 30° nach oben.


        horizontalerAbstand = Math.Abs(CSng(renderBreite) * 0.5F - startX)

        maximalerYOffset = horizontalerAbstand * CSng(Math.Tan(MAX_AUFWIND_WINKEL))


        ' Natürlich nicht unter den Bildschirm hinausschießen.

        maximalerYOffset = Math.Min(maximalerYOffset, CSng(renderHoehe) * 0.5F)

        yOffset = CSng(zufall.NextDouble()) * maximalerYOffset

        startY = mittelY + yOffset

        Return New Vector2(startX, startY)

    End Function

    Private Function ErzeugeStartPunktTopView(renderBreite As Integer, renderHoehe As Integer, zufall As Random) _
        As Vector2

        Dim winkel As Double

        Dim richtungX As Single
        Dim richtungY As Single

        Dim mittelX As Single
        Dim mittelY As Single

        Dim halbBreite As Single
        Dim halbHoehe As Single

        Dim faktorX As Single
        Dim faktorY As Single
        Dim faktorRand As Single

        Dim offset As Single

        Dim startX As Single
        Dim startY As Single


        ' D360:
        ' ein beliebiger Winkel auf dem vollständigen Einheitskreis.

        winkel = zufall.NextDouble() * Math.PI * 2.0

        richtungX = CSng(Math.Cos(winkel))
        richtungY = CSng(Math.Sin(winkel))

        mittelX = CSng(renderBreite) * 0.5F
        mittelY = CSng(renderHoehe) * 0.5F

        halbBreite = CSng(renderBreite) * 0.5F
        halbHoehe = CSng(renderHoehe) * 0.5F

        ' Schnittpunkt des Strahls mit dem Bildschirmrechteck.
        '
        ' Für jede Achse bestimmen wir, wie weit wir in der
        ' gewählten Richtung laufen könnten.
        '
        ' Der kleinere Faktor trifft zuerst auf einen Rand.

        If Math.Abs(richtungX) > 0.000001F Then

            faktorX = halbBreite / Math.Abs(richtungX)

        Else

            faktorX = Single.MaxValue

        End If

        If Math.Abs(richtungY) > 0.000001F Then

            faktorY = halbHoehe / Math.Abs(richtungY)

        Else

            faktorY = Single.MaxValue

        End If

        faktorRand = Math.Min(faktorX, faktorY)

        ' Den Ursprung wie bisher ein Stück außerhalb des
        ' sichtbaren Bereichs platzieren.

        offset = Math.Min(CSng(renderBreite), CSng(renderHoehe)) * URSPRUNG_ABSTAND

        faktorRand += offset

        startX = mittelX + richtungX * faktorRand
        startY = mittelY + richtungY * faktorRand

        Return New Vector2(startX, startY)

    End Function

#End Region

#Region "Distanz"

    Private Function BerechneDistanz(x1 As Single, y1 As Single, x2 As Single, y2 As Single) As Single

        Dim deltaX As Single
        Dim deltaY As Single

        deltaX = x1 - x2
        deltaY = y1 - y2

        Return CSng(Math.Sqrt(deltaX * deltaX + deltaY * deltaY))

    End Function

    Private Function BerechneMinimaleDistanzZumBild(ursprungX As Single, ursprungY As Single, breite As Integer,
                                                    hoehe As Integer) As Single

        Dim naechstesX As Single
        Dim naechstesY As Single

        naechstesX = Math.Max(0.0F, Math.Min(CSng(breite), ursprungX))
        naechstesY = Math.Max(0.0F, Math.Min(CSng(hoehe), ursprungY))

        Return BerechneDistanz(ursprungX, ursprungY, naechstesX, naechstesY)

    End Function

    Private Function BerechneMaximaleDistanzZuBildecke(ursprungX As Single, ursprungY As Single, breite As Integer,
                                                       hoehe As Integer) As Single

        Dim distanzLinksOben As Single
        Dim distanzRechtsOben As Single
        Dim distanzLinksUnten As Single
        Dim distanzRechtsUnten As Single

        distanzLinksOben = BerechneDistanz(0.0F, 0.0F, ursprungX, ursprungY)
        distanzRechtsOben = BerechneDistanz(CSng(breite), 0.0F, ursprungX, ursprungY)
        distanzLinksUnten = BerechneDistanz(0.0F, CSng(hoehe), ursprungX, ursprungY)
        distanzRechtsUnten = BerechneDistanz(CSng(breite), CSng(hoehe), ursprungX, ursprungY)

        Return Math.Max(Math.Max(distanzLinksOben, distanzRechtsOben), Math.Max(distanzLinksUnten,
               distanzRechtsUnten))

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
        frequenz = FBM_GRUNDFREQUENZ
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