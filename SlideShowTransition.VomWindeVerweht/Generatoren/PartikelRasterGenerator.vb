Imports System.Drawing

Public Class PartikelRasterGenerator

#Region "Konstanten"

    Private Const GROESSENFAKTOR_MIN As Double = 0.85
    Private Const GROESSENFAKTOR_MAX As Double = 1.15

    Private Const LOD_FEIN_MAX As Integer = 8
    Private Const LOD_MITTEL_MAX As Integer = 32

#End Region

    Public Function ErzeugePartikelRaster(bildBreite As Integer, bildHoehe As Integer, zielPartikelGroesse _
                                          As Integer, Optional seed As Integer = 0) As PartikelDaten()

        Dim zufall As Random
        Dim partikel As List(Of PartikelDaten)
        Dim aktuelleY As Integer
        Dim zeilenHoehe As Integer

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

        zufall = New Random(seed)
        partikel = New List(Of PartikelDaten)()

        aktuelleY = 0

        While aktuelleY < bildHoehe

            zeilenHoehe = BerechneNaechsteGroesse(zielPartikelGroesse, zufall)

            If aktuelleY + zeilenHoehe > bildHoehe Then
                zeilenHoehe = bildHoehe - aktuelleY
            End If

            ErzeugeZeile(partikel, bildBreite, bildHoehe, aktuelleY, zeilenHoehe, zielPartikelGroesse, zufall)

            aktuelleY += zeilenHoehe

        End While

        Return partikel.ToArray()

    End Function

    Private Sub ErzeugeZeile(partikel As List(Of PartikelDaten), bildBreite As Integer, bildHoehe As Integer,
                             y As Integer, hoehe As Integer, zielPartikelGroesse As Integer, zufall As Random)

        Dim aktuelleX As Integer
        Dim breite As Integer
        Dim daten As PartikelDaten

        aktuelleX = 0

        While aktuelleX < bildBreite

            breite = BerechneNaechsteGroesse(zielPartikelGroesse, zufall)

            If aktuelleX + breite > bildBreite Then
                breite = bildBreite - aktuelleX
            End If

            daten = ErzeugePartikel(aktuelleX, y, breite, hoehe, bildBreite, bildHoehe)

            partikel.Add(daten)

            aktuelleX += breite

        End While

    End Sub

    Private Function ErzeugePartikel(x As Integer, y As Integer, breite As Integer, hoehe As Integer,
                                     bildBreite As Integer, bildHoehe As Integer) As PartikelDaten

        Dim daten As PartikelDaten
        Dim mittlereGroesse As Single

        daten = New PartikelDaten()

        'Position bezeichnet den Partikelmittelpunkt.
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

        daten.lebt = 1

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

End Class