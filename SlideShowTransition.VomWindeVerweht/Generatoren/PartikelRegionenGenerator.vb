Public Class PartikelRegionGenerator

#Region "Konstanten"

    Private Const REGIONEN_MIN As Integer = 8
    Private Const REGIONEN_MAX As Integer = 14

    Private Const GROESSE_FEIN As Integer = 4
    Private Const GROESSE_MITTEL As Integer = 16
    Private Const GROESSE_GROB As Integer = 64

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

        Public Function ErmittlePartikelGroesse(x As Single, y As Single) As Integer

            Dim index As Integer
            Dim abstandX As Single
            Dim abstandY As Single
            Dim abstandQuadrat As Single

            Dim kleinsterAbstand As Single
            Dim gefundeneGroesse As Integer

            kleinsterAbstand = Single.MaxValue
            gefundeneGroesse = GROESSE_FEIN

            For index = 0 To regionen.Length - 1

                abstandX = x - regionen(index).x
                abstandY = y - regionen(index).y

                abstandQuadrat = abstandX * abstandX + abstandY * abstandY

                If abstandQuadrat < kleinsterAbstand Then

                    kleinsterAbstand = abstandQuadrat

                    gefundeneGroesse = regionen(index).partikelGroesse

                End If

            Next

            Return gefundeneGroesse

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

            WaehleLOD(regionen(index), zufall)

        Next

        Return New PartikelRegionFeld(regionen)

    End Function

    Private Sub WaehleLOD(ByRef region As PartikelRegion, zufall As Random)

        Dim auswahl As Integer

        auswahl = zufall.Next(0, 3)

        Select Case auswahl

            Case 0

                region.lod = PartikelLOD.Fein
                region.partikelGroesse = GROESSE_FEIN

            Case 1

                region.lod = PartikelLOD.Mittel
                region.partikelGroesse = GROESSE_MITTEL

            Case Else

                region.lod = PartikelLOD.Grob
                region.partikelGroesse = GROESSE_GROB

        End Select

    End Sub

#End Region

End Class