Public Class PartikelRasterGenerator

#Region "Konstanten"

    Private Const GROESSENFAKTOR_MIN As Double = 0.85
    Private Const GROESSENFAKTOR_MAX As Double = 1.15

    Private Const LOD_FEIN_MAX As Integer = 8
    Private Const LOD_MITTEL_MAX As Integer = 32
    Private Const MIXED_MAX_ZELLENGROESSE As Integer = 128

#End Region

#Region "Rastererzeugung"

    Public Function ErzeugePartikelRaster(bildBreite As Integer, bildHoehe As Integer, zielPartikelGroesse As Integer,
                                          Optional seed As Integer = 0,
                                          Optional regionFeld As PartikelRegionGenerator.PartikelRegionFeld = Nothing) _
                                            As PartikelDaten()

        Dim partikelAnzahl As Integer
        Dim partikel() As PartikelDaten

        If bildBreite <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(bildBreite))
        End If

        If bildHoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(bildHoehe))
        End If

        If zielPartikelGroesse <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(zielPartikelGroesse))
        End If

        If seed = 0 Then
            seed = Environment.TickCount
        End If

        ' Mixed Particles verwenden ein eigenes quadratisches Subraster.
        '
        ' Dadurch bestimmt jede Region sowohl Breite als auch Höhe
        ' ihrer Partikel. Der normale zeilenbasierte Rasterpfad bleibt
        ' vollständig unverändert.

        If regionFeld IsNot Nothing Then

            Return ErzeugeMixedPartikelRaster(bildBreite, bildHoehe, seed, regionFeld)

        End If

        ' Pass 1:
        ' Exakte Anzahl bestimmen, ohne Partikeldaten anzulegen.

        partikelAnzahl = ErmittlePartikelAnzahl(bildBreite, bildHoehe, zielPartikelGroesse, seed)

        If partikelAnzahl <= 0 Then

            Throw New InvalidOperationException("Das Partikelraster enthält keine Partikel.")

        End If


        ' Nur EIN endgültiges Array anlegen.

        partikel = New PartikelDaten(partikelAnzahl - 1) {}


        ' Pass 2:
        ' Mit identischem Seed exakt dasselbe Raster noch einmal
        ' erzeugen und unmittelbar in das Array schreiben.

        BefuellePartikelRaster(partikel, bildBreite, bildHoehe, zielPartikelGroesse, seed)

        Return partikel

    End Function

#End Region

#Region "Mixed-Partikel-Subraster"

    Private Function ErzeugeMixedPartikelRaster(bildBreite As Integer, bildHoehe As Integer, seed As Integer,
                                                regionFeld As PartikelRegionGenerator.PartikelRegionFeld) _
                                                As PartikelDaten()

        Dim partikelListe As List(Of PartikelDaten)
        Dim partikelZufall As Random

        Dim aktuelleX As Integer
        Dim aktuelleY As Integer

        Dim zellenBreite As Integer
        Dim zellenHoehe As Integer

        partikelListe = New List(Of PartikelDaten)()

        partikelZufall = New Random(seed Xor &H5F3759DF)

        '---------------------------------
        ' 128er-Makroraster
        '---------------------------------
        '
        ' Jede Makrozelle wird anschließend unabhängig
        ' hierarchisch unterteilt.
        '
        ' Dadurch kann eine kleine Region nicht mehr die
        ' Partikelgröße des gesamten Bildes bestimmen.
        '

        aktuelleY = 0

        While aktuelleY < bildHoehe

            zellenHoehe = Math.Min(MIXED_MAX_ZELLENGROESSE, bildHoehe - aktuelleY)

            aktuelleX = 0

            While aktuelleX < bildBreite

                zellenBreite = Math.Min(MIXED_MAX_ZELLENGROESSE, bildBreite - aktuelleX)

                ErzeugeMixedSubraster(partikelListe, aktuelleX, aktuelleY, zellenBreite, zellenHoehe, bildBreite,
                                      bildHoehe, regionFeld, partikelZufall, -1)

                aktuelleX += zellenBreite

            End While

            aktuelleY += zellenHoehe

        End While

        If partikelListe.Count <= 0 Then

            Throw New InvalidOperationException("Das Mixed-Partikelraster enthält keine Partikel.")

        End If

        Return partikelListe.ToArray()

    End Function


    Private Sub ErzeugeMixedSubraster(partikelListe As List(Of PartikelDaten), x As Integer, y As Integer,
                                      breite As Integer, hoehe As Integer, bildBreite As Integer, bildHoehe As Integer,
                                      regionFeld As PartikelRegionGenerator.PartikelRegionFeld,
                                      partikelZufall As Random, bekannterRegionIndex As Integer)

        Dim regionIndex As Integer
        Dim zielGroesse As Integer

        Dim istEinheitlicheRegion As Boolean

        regionIndex = bekannterRegionIndex

        '---------------------------------
        ' Region der aktuellen Zelle prüfen
        '---------------------------------
        '
        ' Wenn der Elternknoten bereits eindeutig derselben
        ' Region zugeordnet wurde, muss diese relativ teure
        ' Prüfung nicht erneut ausgeführt werden.
        '
        ' Das ist insbesondere für 1-px-Regionen wichtig:
        ' Nach einmal bestätigter Region kann der Baum bis
        ' auf 1 px heruntergeteilt werden, ohne für jeden
        ' Knoten erneut Voronoi-Abstände zu berechnen.
        '

        If regionIndex >= 0 Then

            istEinheitlicheRegion = True

        Else

            istEinheitlicheRegion = ErmittleEinheitlicheRegion(x, y, breite, hoehe, regionFeld, regionIndex)

        End If

        If istEinheitlicheRegion Then

            zielGroesse = regionFeld.ErmittlePartikelGroesseFuerRegion(regionIndex)

            '---------------------------------
            ' Zielgröße erreicht
            '---------------------------------

            If breite <= zielGroesse AndAlso
               hoehe <= zielGroesse Then

                partikelListe.Add(ErzeugePartikel(x, y, breite, hoehe, bildBreite, bildHoehe, zielGroesse,
                                                  partikelZufall))

                Return

            End If

        End If

        '---------------------------------
        ' Absolute Untergrenze
        '---------------------------------
        '
        ' Eine 1x1-Zelle kann nicht weiter geteilt werden.
        '
        ' Falls die Region wegen einer Grenzsituation noch
        ' nicht eindeutig war, bestimmt nun dieser Pixel selbst
        ' seine Region.
        '

        If breite <= 1 AndAlso hoehe <= 1 Then

            If regionIndex < 0 Then

                regionIndex = regionFeld.ErmittleRegionIndex(CSng(x), CSng(y))

            End If

            zielGroesse = regionFeld.ErmittlePartikelGroesseFuerRegion(regionIndex)

            partikelListe.Add(ErzeugePartikel(x, y, breite, hoehe, bildBreite, bildHoehe, zielGroesse,
                                              partikelZufall))

            Return

        End If

        '---------------------------------
        ' Zelle weiter unterteilen
        '---------------------------------
        '
        ' Zwei Gründe können hierher führen:
        '
        ' 1. Die Zelle überschreitet die Zielgröße ihrer Region.
        '
        ' 2. Die Zelle überdeckt mehrere Regionen.
        '
        ' Nur im ersten Fall darf der bekannte RegionIndex an
        ' die Kinder weitergereicht werden.
        '

        If istEinheitlicheRegion Then

            TeileMixedZelle(partikelListe, x, y, breite, hoehe, bildBreite, bildHoehe, regionFeld, partikelZufall,
                            regionIndex)

        Else

            TeileMixedZelle(partikelListe, x, y, breite, hoehe, bildBreite, bildHoehe, regionFeld, partikelZufall, -1)

        End If

    End Sub


    Private Sub TeileMixedZelle(partikelListe As List(Of PartikelDaten), x As Integer, y As Integer, breite As Integer,
                                hoehe As Integer, bildBreite As Integer, bildHoehe As Integer,
                                regionFeld As PartikelRegionGenerator.PartikelRegionFeld, partikelZufall As Random,
                                bekannterRegionIndex As Integer)

        Dim breiteLinks As Integer
        Dim breiteRechts As Integer

        Dim hoeheOben As Integer
        Dim hoeheUnten As Integer

        '---------------------------------
        ' Beide Dimensionen teilbar
        '---------------------------------

        If breite > 1 AndAlso hoehe > 1 Then

            breiteLinks = breite \ 2
            breiteRechts = breite - breiteLinks
            hoeheOben = hoehe \ 2
            hoeheUnten = hoehe - hoeheOben

            ErzeugeMixedSubraster(partikelListe, x, y, breiteLinks, hoeheOben, bildBreite, bildHoehe, regionFeld,
                                  partikelZufall, bekannterRegionIndex)

            ErzeugeMixedSubraster(partikelListe, x + breiteLinks, y, breiteRechts, hoeheOben, bildBreite, bildHoehe,
                                  regionFeld, partikelZufall, bekannterRegionIndex)

            ErzeugeMixedSubraster(partikelListe, x, y + hoeheOben, breiteLinks, hoeheUnten, bildBreite, bildHoehe,
                                  regionFeld, partikelZufall, bekannterRegionIndex)

            ErzeugeMixedSubraster(partikelListe, x + breiteLinks, y + hoeheOben, breiteRechts, hoeheUnten, bildBreite,
                                  bildHoehe, regionFeld, partikelZufall, bekannterRegionIndex)

            Return

        End If

        '---------------------------------
        ' Nur horizontal teilbar
        '---------------------------------

        If breite > 1 Then

            breiteLinks = breite \ 2

            breiteRechts = breite - breiteLinks

            ErzeugeMixedSubraster(partikelListe, x, y, breiteLinks, hoehe, bildBreite, bildHoehe, regionFeld,
                                  partikelZufall, bekannterRegionIndex)

            ErzeugeMixedSubraster(partikelListe, x + breiteLinks, y, breiteRechts, hoehe, bildBreite, bildHoehe,
                                  regionFeld, partikelZufall, bekannterRegionIndex)

            Return

        End If

        '---------------------------------
        ' Nur vertikal teilbar
        '---------------------------------

        hoeheOben = hoehe \ 2

        hoeheUnten = hoehe - hoeheOben

        ErzeugeMixedSubraster(partikelListe, x, y, breite, hoeheOben, bildBreite, bildHoehe, regionFeld,
                              partikelZufall, bekannterRegionIndex)

        ErzeugeMixedSubraster(partikelListe, x, y + hoeheOben, breite, hoeheUnten, bildBreite, bildHoehe,
                              regionFeld, partikelZufall, bekannterRegionIndex)

    End Sub


    Private Function ErmittleEinheitlicheRegion(x As Integer, y As Integer, breite As Integer, hoehe As Integer,
                                                regionFeld As PartikelRegionGenerator.PartikelRegionFeld,
                                                ByRef regionIndex As Integer) As Boolean

        Dim links As Single
        Dim rechts As Single
        Dim oben As Single
        Dim unten As Single

        Dim mitteX As Single
        Dim mitteY As Single

        Dim pruefRegionIndex As Integer

        links = CSng(x)
        rechts = CSng(x + breite - 1)
        oben = CSng(y)
        unten = CSng(y + hoehe - 1)

        mitteX = links + (rechts - links) * 0.5F
        mitteY = oben + (unten - oben) * 0.5F

        '---------------------------------
        ' Mittelpunkt bestimmt Referenzregion
        '---------------------------------

        regionIndex = regionFeld.ErmittleRegionIndex(mitteX, mitteY)

        '---------------------------------
        ' Vier Ecken prüfen
        '---------------------------------

        pruefRegionIndex = regionFeld.ErmittleRegionIndex(links, oben)

        If pruefRegionIndex <> regionIndex Then
            Return False
        End If

        pruefRegionIndex = regionFeld.ErmittleRegionIndex(rechts, oben)

        If pruefRegionIndex <> regionIndex Then
            Return False
        End If

        pruefRegionIndex = regionFeld.ErmittleRegionIndex(links, unten)

        If pruefRegionIndex <> regionIndex Then
            Return False
        End If

        pruefRegionIndex = regionFeld.ErmittleRegionIndex(rechts, unten)

        If pruefRegionIndex <> regionIndex Then
            Return False
        End If


        '---------------------------------
        ' Vier Kantenmittelpunkte prüfen
        '---------------------------------

        pruefRegionIndex = regionFeld.ErmittleRegionIndex(mitteX, oben)

        If pruefRegionIndex <> regionIndex Then
            Return False
        End If

        pruefRegionIndex = regionFeld.ErmittleRegionIndex(mitteX, unten)

        If pruefRegionIndex <> regionIndex Then
            Return False
        End If

        pruefRegionIndex = regionFeld.ErmittleRegionIndex(links, mitteY)

        If pruefRegionIndex <> regionIndex Then
            Return False
        End If

        pruefRegionIndex = regionFeld.ErmittleRegionIndex(rechts, mitteY)

        If pruefRegionIndex <> regionIndex Then
            Return False
        End If

        Return True

    End Function

#End Region

#Region "Pass 1 - Partikel zählen"

    Private Function ErmittlePartikelAnzahl(bildBreite As Integer, bildHoehe As Integer, zielPartikelGroesse As Integer,
                                            seed As Integer) As Integer
        Dim zufall As Random

        Dim aktuelleX As Integer
        Dim aktuelleY As Integer

        Dim breite As Integer
        Dim zeilenHoehe As Integer

        Dim anzahl As Integer

        Dim aktuelleZielGroesse As Integer

        zufall = New Random(seed)

        aktuelleY = 0
        anzahl = 0

        While aktuelleY < bildHoehe

            aktuelleZielGroesse = zielPartikelGroesse

            zeilenHoehe = BerechneNaechsteGroesse(aktuelleZielGroesse, zufall)

            If aktuelleY + zeilenHoehe > bildHoehe Then
                zeilenHoehe = bildHoehe - aktuelleY
            End If

            aktuelleX = 0

            While aktuelleX < bildBreite

                aktuelleZielGroesse = zielPartikelGroesse

                breite = BerechneNaechsteGroesse(aktuelleZielGroesse, zufall)

                If aktuelleX + breite > bildBreite Then
                    breite = bildBreite - aktuelleX
                End If

                anzahl += 1

                aktuelleX += breite

            End While

            aktuelleY += zeilenHoehe

        End While

        Return anzahl

    End Function

#End Region

#Region "Pass 2 - Array befüllen"

    Private Sub BefuellePartikelRaster(partikel() As PartikelDaten, bildBreite As Integer, bildHoehe As Integer,
                                       zielPartikelGroesse As Integer, seed As Integer)

        Dim rasterZufall As Random
        Dim partikelZufall As Random

        Dim aktuelleX As Integer
        Dim aktuelleY As Integer

        Dim breite As Integer
        Dim zeilenHoehe As Integer

        Dim partikelIndex As Integer

        Dim aktuelleZielGroesse As Integer

        ' WICHTIG:
        '
        ' Die Rastergeometrie muss exakt denselben Zufallsstrom
        ' verwenden wie Pass 1.

        rasterZufall = New Random(seed)


        ' Alle zufälligen Partikeleigenschaften bekommen einen
        ' vollständig unabhängigen Zufallsstrom.
        '
        ' Dadurch können Gewicht, Rotation usw. später beliebig
        ' erweitert werden, ohne jemals wieder die Rastergeometrie
        ' von Pass 1 und Pass 2 auseinanderlaufen zu lassen.

        partikelZufall = New Random(seed Xor &H5F3759DF)

        aktuelleY = 0
        partikelIndex = 0

        While aktuelleY < bildHoehe

            aktuelleZielGroesse = zielPartikelGroesse

            zeilenHoehe = BerechneNaechsteGroesse(aktuelleZielGroesse, rasterZufall)

            If aktuelleY + zeilenHoehe > bildHoehe Then

                zeilenHoehe = bildHoehe - aktuelleY

            End If

            aktuelleX = 0

            While aktuelleX < bildBreite

                aktuelleZielGroesse = zielPartikelGroesse

                breite = BerechneNaechsteGroesse(aktuelleZielGroesse, rasterZufall)

                If aktuelleX + breite > bildBreite Then

                    breite = bildBreite - aktuelleX

                End If

                If partikelIndex >= partikel.Length Then

                    Throw New InvalidOperationException(
                    "Die berechnete Partikelanzahl stimmt nicht mit dem erzeugten Raster überein.")

                End If

                partikel(partikelIndex) = ErzeugePartikel(aktuelleX, aktuelleY, breite, zeilenHoehe, bildBreite,
                                                          bildHoehe, aktuelleZielGroesse, partikelZufall)

                partikelIndex += 1

                aktuelleX += breite

            End While

            aktuelleY += zeilenHoehe

        End While

        If partikelIndex <> partikel.Length Then

            Throw New InvalidOperationException(
            "Das erzeugte Raster enthält eine unerwartete Anzahl von Partikeln.")

        End If

    End Sub

#End Region

#Region "Partikeldaten"

    Private Function ErzeugePartikel(x As Integer, y As Integer, breite As Integer, hoehe As Integer,
                                     bildBreite As Integer, bildHoehe As Integer, zielPartikelGroesse As Integer, zufall As Random) As PartikelDaten

        Dim daten As PartikelDaten
        Dim mittlereGroesse As Single
        Dim gewichtsZufall As Double
        Dim groessenFaktor As Single

        daten = New PartikelDaten()

        daten.positionX = x + breite * 0.5F
        daten.positionY = y + hoehe * 0.5F
        daten.positionZ = 0.0F

        daten.geschwindigkeitX = 0.0F
        daten.geschwindigkeitY = 0.0F
        daten.geschwindigkeitZ = 0.0F

        daten.breite = breite
        daten.hoehe = hoehe

        daten.uvLinks = CSng(x / CDbl(bildBreite))
        daten.uvOben = CSng(y / CDbl(bildHoehe))
        daten.uvRechts = CSng((x + breite) / CDbl(bildBreite))
        daten.uvUnten = CSng((y + hoehe) / CDbl(bildHoehe))

        daten.rotationX = 0.0F
        daten.rotationY = 0.0F
        daten.rotationZ = 0.0F

        daten.rotationsGeschwindigkeitX = 0.0F
        daten.rotationsGeschwindigkeitY = 0.0F
        daten.rotationsGeschwindigkeitZ = 0.0F

        mittlereGroesse = (breite + hoehe) * 0.5F

        daten.lod = CInt(BestimmeLOD(mittlereGroesse))

        daten.status = CInt(PartikelStatus.Ruhend)

        ' Individuelles Partikelgewicht.
        '
        ' Zwei gemittelte Zufallswerte erzeugen eine Verteilung,
        ' bei der mittlere Gewichte häufiger und Extremwerte
        ' seltener auftreten.

        gewichtsZufall = (zufall.NextDouble() + zufall.NextDouble()) / 2.0

        ' Für V1 dominiert bewusst die individuelle Masse.

        daten.gewicht = 0.3F + CSng(gewichtsZufall) * 2.2F


        ' Die tatsächliche Partikelgröße beeinflusst das Gewicht
        ' bereits leicht. Dadurch ist die Architektur für spätere
        ' LOD-Mischungen vorbereitet, ohne bei feinem Sand wieder
        ' die Individualität zu verlieren.

        groessenFaktor = CSng(Math.Sqrt(mittlereGroesse / Math.Max(1.0, CDbl(zielPartikelGroesse))))

        groessenFaktor = Math.Max(0.75F, Math.Min(1.35F, groessenFaktor))

        daten.gewicht *= groessenFaktor

        Return daten

    End Function

    Private Function BerechneNaechsteGroesse(zielGroesse As Integer, zufall As Random) As Integer

        Dim faktor As Double
        Dim groesse As Integer

        faktor = GROESSENFAKTOR_MIN + zufall.NextDouble() * (GROESSENFAKTOR_MAX - GROESSENFAKTOR_MIN)

        groesse = CInt(Math.Round(zielGroesse * faktor))

        Return Math.Max(1, groesse)

    End Function

    Private Function BestimmeLOD(partikelGroesse As Single) As PartikelLOD

        If partikelGroesse <= LOD_FEIN_MAX Then
            Return PartikelLOD.Fein
        End If

        If partikelGroesse <= LOD_MITTEL_MAX Then
            Return PartikelLOD.Mittel
        End If

        Return PartikelLOD.Grob

    End Function

#End Region

End Class