Imports System.Drawing
Imports System.Drawing.Drawing2D

'##############################################
'#                                            #
'# Für die bereits geplante V 2.0 von der     #
'# Transition Brennendes Papier               #
'#                                            #
'##############################################



Public Class BlitzTexturGenerator

    Private Class BlitzSegment

        Public Property StartPunkt As PointF
        Public Property EndPunkt As PointF
        Public Property Intensitaet As Single
        Public Property AstTiefe As Integer

    End Class

    Private Const STANDARD_BREITE As Integer = 512
    Private Const STANDARD_HOEHE As Integer = 512

    Private Const HAUPTAST_SEGMENTE_MIN As Integer = 18
    Private Const HAUPTAST_SEGMENTE_MAX As Integer = 28

    Private Const SEITENABWEICHUNG_MAX As Single = 28.0F

    Private Const NEBENAST_WAHRSCHEINLICHKEIT As Double = 0.18
    Private Const MAX_AST_TIEFE As Integer = 2

    Private Const NEBENAST_SEGMENTE_MIN As Integer = 3
    Private Const NEBENAST_SEGMENTE_MAX As Integer = 8

    Private Const KERN_BREITE As Single = 2.0F
    Private Const INNERER_GLOW_BREITE As Single = 7.0F
    Private Const AEUSSERER_GLOW_BREITE As Single = 18.0F

    Public Function ErzeugeBlitzTextur(Optional breite As Integer = STANDARD_BREITE,
                                      Optional hoehe As Integer = STANDARD_HOEHE,
                                      Optional seed As Integer = 0) As BlitzTexturDaten

        Dim zufall As Random
        Dim segmente As List(Of BlitzSegment)
        Dim bitmap As Bitmap
        Dim daten As BlitzTexturDaten

        If breite <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(breite))
        End If

        If hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(hoehe))
        End If

        If seed = 0 Then
            seed = Environment.TickCount
        End If

        zufall = New Random(seed)

        segmente = ErzeugeBlitzGeometrie(breite, hoehe, zufall)

        bitmap = RendereBlitz(breite, hoehe, segmente)

        daten = New BlitzTexturDaten()

        daten.Breite = breite
        daten.Hoehe = hoehe
        daten.Seed = seed
        daten.Bitmap = bitmap

        Return daten

    End Function

    Private Function ErzeugeBlitzGeometrie(breite As Integer, hoehe As Integer, zufall As Random) _
        As List(Of BlitzSegment)

        Dim segmente As List(Of BlitzSegment)
        Dim startPunkt As PointF
        Dim endPunkt As PointF
        Dim segmentAnzahl As Integer
        Dim i As Integer
        Dim aktuellePosition As PointF
        Dim naechstePosition As PointF
        Dim schrittY As Single
        Dim abweichungX As Single
        Dim intensitaet As Single

        segmente = New List(Of BlitzSegment)()

        startPunkt = New PointF(CSng(breite * 0.5), CSng(hoehe * 0.05))
        endPunkt = New PointF(CSng(breite * 0.5), CSng(hoehe * 0.95))

        segmentAnzahl = zufall.Next(HAUPTAST_SEGMENTE_MIN, HAUPTAST_SEGMENTE_MAX + 1)

        aktuellePosition = startPunkt

        schrittY = (endPunkt.Y - startPunkt.Y) / CSng(segmentAnzahl)

        For i = 0 To segmentAnzahl - 1

            abweichungX = CSng((zufall.NextDouble() * 2.0 - 1.0) * SEITENABWEICHUNG_MAX)

            naechstePosition = New PointF(
                Begrenze(
                    aktuellePosition.X + abweichungX,
                    CSng(breite * 0.1),
                    CSng(breite * 0.9)),
                aktuellePosition.Y + schrittY)

            intensitaet = CSng(1.0 - (i / CDbl(segmentAnzahl)) * 0.25)

            segmente.Add(
                New BlitzSegment With
                {
                    .StartPunkt = aktuellePosition,
                    .EndPunkt = naechstePosition,
                    .Intensitaet = intensitaet,
                    .AstTiefe = 0
                })

            If zufall.NextDouble() < NEBENAST_WAHRSCHEINLICHKEIT Then

                ErzeugeNebenast(segmente, aktuellePosition, naechstePosition, breite, hoehe, zufall, 1)

            End If

            aktuellePosition = naechstePosition

        Next

        Return segmente

    End Function

    Private Sub ErzeugeNebenast(segmente As List(Of BlitzSegment), ursprung As PointF, hauptRichtung As PointF,
                                breite As Integer, hoehe As Integer, zufall As Random, astTiefe As Integer)

        Dim segmentAnzahl As Integer
        Dim aktuellePosition As PointF
        Dim naechstePosition As PointF
        Dim richtungsFaktor As Single
        Dim schrittX As Single
        Dim schrittY As Single
        Dim seitlicheAbweichung As Single
        Dim i As Integer
        Dim intensitaet As Single

        If astTiefe > MAX_AST_TIEFE Then
            Return
        End If

        segmentAnzahl = zufall.Next(NEBENAST_SEGMENTE_MIN, NEBENAST_SEGMENTE_MAX + 1)

        aktuellePosition = ursprung

        If zufall.Next(0, 2) = 0 Then
            richtungsFaktor = -1.0F
        Else
            richtungsFaktor = 1.0F
        End If

        schrittX = CSng((12.0 + zufall.NextDouble() * 18.0) * richtungsFaktor)
        schrittY = CSng(8.0 + zufall.NextDouble() * 18.0)

        For i = 0 To segmentAnzahl - 1

            seitlicheAbweichung = CSng((zufall.NextDouble() * 2.0 - 1.0) * 8.0)

            naechstePosition = New PointF(
                Begrenze(
                    aktuellePosition.X +
                    schrittX +
                    seitlicheAbweichung,
                    0.0F,
                    breite - 1.0F),
                Begrenze(
                    aktuellePosition.Y +
                    schrittY,
                    0.0F,
                    hoehe - 1.0F))

            intensitaet = CSng(0.75 - astTiefe * 0.15 - (i / CDbl(segmentAnzahl)) * 0.25)

            segmente.Add(
                New BlitzSegment With
                {
                    .StartPunkt = aktuellePosition,
                    .EndPunkt = naechstePosition,
                    .Intensitaet = Math.Max(0.2F, intensitaet),
                    .AstTiefe = astTiefe
                })

            If astTiefe < MAX_AST_TIEFE AndAlso zufall.NextDouble() < NEBENAST_WAHRSCHEINLICHKEIT * 0.35 Then

                ErzeugeNebenast(segmente, aktuellePosition, naechstePosition, breite, hoehe, zufall, astTiefe + 1)

            End If

            aktuellePosition = naechstePosition

        Next

    End Sub

    Private Function RendereBlitz(breite As Integer, hoehe As Integer, segmente As List(Of BlitzSegment)) As Bitmap

        Dim bitmap As Bitmap
        Dim graphics As Graphics
        Dim segment As BlitzSegment
        Dim alpha As Integer
        Dim kernBreite As Single
        Dim innerGlowBreite As Single
        Dim aeussererGlowBreite As Single

        bitmap = New Bitmap(breite, hoehe, Imaging.PixelFormat.Format32bppArgb)

        graphics = Graphics.FromImage(bitmap)

        Try

            graphics.Clear(Color.Transparent)
            graphics.SmoothingMode = SmoothingMode.AntiAlias
            graphics.CompositingMode = CompositingMode.SourceOver
            graphics.CompositingQuality = CompositingQuality.HighQuality
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality

            ' Äußerer Glow
            For Each segment In segmente

                alpha = CInt(38.0F * segment.Intensitaet)

                aeussererGlowBreite = AEUSSERER_GLOW_BREITE * BerechneAstBreitenFaktor(segment.AstTiefe)

                ZeichneSegment(graphics, segment, aeussererGlowBreite, Color.FromArgb(alpha, 255, 120, 0))

            Next

            ' Innerer Glow
            For Each segment In segmente

                alpha = CInt(110.0F * segment.Intensitaet)

                innerGlowBreite = INNERER_GLOW_BREITE * BerechneAstBreitenFaktor(segment.AstTiefe)

                ZeichneSegment(graphics, segment, innerGlowBreite, Color.FromArgb(alpha, 255, 180, 40))

            Next

            ' Heller Kern
            For Each segment In segmente

                alpha = CInt(255.0F * segment.Intensitaet)

                kernBreite = KERN_BREITE * BerechneAstBreitenFaktor(segment.AstTiefe)

                ZeichneSegment(graphics, segment, kernBreite, Color.FromArgb(alpha, 255, 245, 210))

            Next

        Finally

            graphics.Dispose()

        End Try

        Return bitmap

    End Function

    Private Sub ZeichneSegment(graphics As Graphics, segment As BlitzSegment, breite As Single, farbe As Color)

        Dim pen As Pen

        pen = New Pen(farbe, Math.Max(1.0F, breite))
        Try

            pen.StartCap = LineCap.Round
            pen.EndCap = LineCap.Round

            graphics.DrawLine(pen, segment.StartPunkt, segment.EndPunkt)

        Finally

            pen.Dispose()

        End Try

    End Sub

    Private Function BerechneAstBreitenFaktor(astTiefe As Integer) As Single

        Select Case astTiefe

            Case 0
                Return 1.0F

            Case 1
                Return 0.65F

            Case Else
                Return 0.4F

        End Select

    End Function

    Private Function Begrenze(wert As Single, minimum As Single, maximum As Single) As Single

        If wert < minimum Then
            Return minimum
        End If

        If wert > maximum Then
            Return maximum
        End If

        Return wert

    End Function

End Class