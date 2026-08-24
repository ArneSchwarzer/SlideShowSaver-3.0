Public Class PartikelRasterGenerator

#Region "Konstanten"

    Private Const GROESSENFAKTOR_MIN As Double = 0.85
    Private Const GROESSENFAKTOR_MAX As Double = 1.15

    Private Const LOD_FEIN_MAX As Integer = 8
    Private Const LOD_MITTEL_MAX As Integer = 32

#End Region

#Region "Rastererzeugung"

    Public Function ErzeugePartikelRaster(bildBreite As Integer, bildHoehe As Integer,
                                          zielPartikelGroesse As Integer, Optional seed As Integer = 0) _
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

#Region "Pass 1 - Partikel zählen"

    Private Function ErmittlePartikelAnzahl(bildBreite As Integer, bildHoehe As Integer,
                                            zielPartikelGroesse As Integer, seed As Integer) As Integer

        Dim zufall As Random

        Dim aktuelleX As Integer
        Dim aktuelleY As Integer

        Dim breite As Integer
        Dim zeilenHoehe As Integer

        Dim anzahl As Integer

        zufall = New Random(seed)

        aktuelleY = 0
        anzahl = 0

        While aktuelleY < bildHoehe

            zeilenHoehe = BerechneNaechsteGroesse(zielPartikelGroesse, zufall)

            If aktuelleY + zeilenHoehe > bildHoehe Then
                zeilenHoehe = bildHoehe - aktuelleY
            End If

            aktuelleX = 0

            While aktuelleX < bildBreite

                breite = BerechneNaechsteGroesse(zielPartikelGroesse, zufall)

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

            zeilenHoehe = BerechneNaechsteGroesse(zielPartikelGroesse, rasterZufall)

            If aktuelleY + zeilenHoehe > bildHoehe Then

                zeilenHoehe = bildHoehe - aktuelleY

            End If

            aktuelleX = 0

            While aktuelleX < bildBreite

                breite = BerechneNaechsteGroesse(zielPartikelGroesse, rasterZufall)

                If aktuelleX + breite > bildBreite Then

                    breite = bildBreite - aktuelleX

                End If

                If partikelIndex >= partikel.Length Then

                    Throw New InvalidOperationException(
                    "Die berechnete Partikelanzahl stimmt nicht mit dem erzeugten Raster überein.")

                End If

                partikel(partikelIndex) = ErzeugePartikel(aktuelleX, aktuelleY, breite, zeilenHoehe, bildBreite,
                                                          bildHoehe, zielPartikelGroesse, partikelZufall)

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