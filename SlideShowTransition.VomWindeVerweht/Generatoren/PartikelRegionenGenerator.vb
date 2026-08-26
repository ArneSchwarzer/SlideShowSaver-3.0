Public Class PartikelRegionGenerator

#Region "Konstanten"

    Private Const REGIONEN_MIN As Integer = 8
    Private Const REGIONEN_MAX As Integer = 14

    Private Shared ReadOnly PARTIKEL_GROESSEN() As Integer = {1, 2, 4, 8, 16, 32, 64, 128}

#End Region

#Region "Regionendaten"

    Public Structure PartikelRegion

        Public x As Single
        Public y As Single

        Public lod As PartikelLOD
        Public partikelGroesse As Integer

    End Structure

    Public Class PartikelRegionFeld

        Private ReadOnly regionen() As PartikelRegion

        Friend Sub New(neueRegionen() As PartikelRegion)

            regionen = neueRegionen

        End Sub

        Public Function ErmittleRegionIndex(x As Single, y As Single) As Integer

            Dim index As Integer

            Dim abstandX As Single
            Dim abstandY As Single
            Dim abstandQuadrat As Single

            Dim kleinsterAbstand As Single
            Dim gefundenerIndex As Integer

            kleinsterAbstand = Single.MaxValue

            gefundenerIndex = 0

            For index = 0 To regionen.Length - 1

                abstandX = x - regionen(index).x
                abstandY = y - regionen(index).y

                abstandQuadrat = abstandX * abstandX + abstandY * abstandY

                If abstandQuadrat < kleinsterAbstand Then

                    kleinsterAbstand = abstandQuadrat

                    gefundenerIndex = index

                End If

            Next

            Return gefundenerIndex

        End Function

        Public Function ErmittlePartikelGroesse(x As Single, y As Single) As Integer

            Dim regionIndex As Integer

            regionIndex = ErmittleRegionIndex(x, y)

            Return regionen(regionIndex).partikelGroesse

        End Function

        Public Function ErmittlePartikelGroesseFuerRegion(regionIndex As Integer) As Integer

            If regionIndex < 0 OrElse regionIndex >= regionen.Length Then

                Throw New ArgumentOutOfRangeException(NameOf(regionIndex))

            End If

            Return regionen(regionIndex).partikelGroesse

        End Function

    End Class

#End Region

#Region "Erzeugung"

    Public Function ErzeugeRegionFeld(bildBreite As Integer, bildHoehe As Integer, Optional seed As Integer = 0) _
        As PartikelRegionFeld

        Dim zufall As Random
        Dim regionen() As PartikelRegion

        Dim anzahlRegionen As Integer
        Dim index As Integer

        If bildBreite <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(bildBreite))
        End If

        If bildHoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(bildHoehe))
        End If

        If seed = 0 Then
            seed = Environment.TickCount
        End If

        zufall = New Random(seed)

        anzahlRegionen = zufall.Next(REGIONEN_MIN, REGIONEN_MAX + 1)

        regionen = New PartikelRegion(anzahlRegionen - 1) {}

        For index = 0 To regionen.Length - 1

            regionen(index).x = CSng(zufall.NextDouble() * bildBreite)
            regionen(index).y = CSng(zufall.NextDouble() * bildHoehe)

            WaehlePartikelGroesse(regionen(index), zufall)

        Next

        Return New PartikelRegionFeld(regionen)

    End Function

    Private Sub WaehlePartikelGroesse(ByRef region As PartikelRegion, zufall As Random)

        Dim auswahl As Integer

        auswahl = zufall.Next(0, PARTIKEL_GROESSEN.Length)

        region.partikelGroesse = PARTIKEL_GROESSEN(auswahl)

        If region.partikelGroesse <= 8 Then

            region.lod = PartikelLOD.Fein

        ElseIf region.partikelGroesse <= 32 Then

            region.lod = PartikelLOD.Mittel

        Else

            region.lod = PartikelLOD.Grob

        End If

    End Sub
#End Region

End Class