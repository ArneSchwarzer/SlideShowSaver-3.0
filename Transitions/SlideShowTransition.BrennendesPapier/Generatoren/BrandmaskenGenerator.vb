Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Friend Class BrandmaskenGenerator

#Region "Variablendeklaration"

    Private Const BRANDMASKE_MAX_BREITE As Integer = 960
    Private Const BRANDMASKE_OVERSCAN As Double = 0.2

    Private Const BRANDHERDE_MIN As Integer = 30
    Private Const BRANDHERDE_MAX As Integer = 60

    Private Const BRANDREGIONEN_MIN As Integer = 2
    Private Const BRANDREGIONEN_MAX As Integer = 5

    Private Const BRANDHERDE_CLUSTER_ANTEIL As Double = 0.7

    Private Const BRANDREGION_RADIUS_MIN As Double = 0.12
    Private Const BRANDREGION_RADIUS_MAX As Double = 0.35

    Private Const BRANDSTART_MAX As Double = 0.7
    Private Const BRANDENDE_MIN As Double = 0.82
    Private Const BRANDENDE_MAX As Double = 0.97

    Private Const BEGRENZUNGSRING_ABSTAND As Double = 0.45
    Private Const BEGRENZUNGSRING_ABSTAND_ZWISCHEN_SEEDS As Double = 0.12

    Private Const MAKRO_GRUNDFREQUENZ As Double = 1.5
    Private Const MAKRO_STAERKE As Double = 0.3

    Private renderBreite As Integer
    Private renderHoehe As Integer

    Private fbmGrundfrequenz As Double
    Private fbmOktaven As Integer
    Private fbmPersistenz As Double
    Private fbmStaerke As Double

    Private ReadOnly rnd As New Random()

#End Region
#Region "Structures & Enums"
    Private Structure Brandherd

        Public x As Double
        Public y As Double

        Public startwert As Double
        Public endwert As Double

        Public istBegrenzungsHerd As Boolean

        Public makroOffsetX As Double
        Public makroOffsetY As Double

    End Structure

    Private Structure BrandherdKandidaten

        Public index0 As Integer
        Public index1 As Integer
        Public index2 As Integer
        Public index3 As Integer

    End Structure

    Private Structure BrandherdRegion

        Public x As Double
        Public y As Double

        Public radius As Double

    End Structure

#End Region

#Region "Brandmaske"

    Friend Function Erzeuge(renderBreite As Integer, renderHoehe As Integer, zuendmodus As String,
                            fbmGrundfrequenz As Double, fbmOktaven As Integer, fbmPersistenz As Double,
                            fbmStaerke As Double) As BitmapSource

        If renderBreite <= 0 OrElse renderHoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(renderBreite))
        End If

        Me.renderBreite = renderBreite
        Me.renderHoehe = renderHoehe

        Me.fbmGrundfrequenz = fbmGrundfrequenz
        Me.fbmOktaven = fbmOktaven
        Me.fbmPersistenz = fbmPersistenz
        Me.fbmStaerke = fbmStaerke

        Select Case zuendmodus

            Case "Brand vom Rand"

                Return ErzeugeBrandMaskeVomRand()

            Case Else

                Return ErzeugeBrandMaskeAusBrandherden()

        End Select

    End Function

    Private Function ErzeugeBrandMaskeVomRand() As BitmapSource

        Dim maskenBreite As Integer
        Dim maskenHoehe As Integer

        Dim seite As Integer

        Dim ursprungX As Double
        Dim ursprungY As Double

        Dim maximaleDistanz As Double
        Dim distanz As Double
        Dim normierteDistanz As Double

        Dim noiseOffsetX As Double
        Dim noiseOffsetY As Double
        Dim noiseWert As Double
        Dim noiseEinfluss As Double

        Dim brandWert As Double

        Dim pixel As Byte()
        Dim stride As Integer
        Dim index As Integer
        Dim grauwert As Byte

        Dim x As Integer
        Dim y As Integer

        Dim bitmap As WriteableBitmap


        maskenBreite = Math.Min(BRANDMASKE_MAX_BREITE, renderBreite)
        maskenHoehe = Math.Max(1, CInt(maskenBreite * renderHoehe / CDbl(renderBreite)))

        '---------------------------------
        ' Zufälligen Rand wählen
        '
        ' 0 = oben
        ' 1 = rechts
        ' 2 = unten
        ' 3 = links
        '---------------------------------

        seite = rnd.Next(0, 4)

        Select Case seite

            Case 0

                ursprungX = rnd.NextDouble() * maskenBreite
                ursprungY = -maskenHoehe * 0.12

            Case 1

                ursprungX = maskenBreite * 1.12
                ursprungY = rnd.NextDouble() * maskenHoehe

            Case 2

                ursprungX = rnd.NextDouble() * maskenBreite
                ursprungY = maskenHoehe * 1.12

            Case Else

                ursprungX = -maskenBreite * 0.12
                ursprungY = rnd.NextDouble() * maskenHoehe

        End Select


        'Größte Entfernung zu einer der vier Bildecken.

        maximaleDistanz =
        Math.Max(
            Math.Max(
                BerechneDistanz(
                    0,
                    0,
                    ursprungX,
                    ursprungY),
                BerechneDistanz(
                    maskenBreite,
                    0,
                    ursprungX,
                    ursprungY)),
            Math.Max(
                BerechneDistanz(
                    0,
                    maskenHoehe,
                    ursprungX,
                    ursprungY),
                BerechneDistanz(
                    maskenBreite,
                    maskenHoehe,
                    ursprungX,
                    ursprungY)))


        noiseOffsetX = rnd.NextDouble() * 1000.0
        noiseOffsetY = rnd.NextDouble() * 1000.0

        stride = maskenBreite * 4

        pixel = New Byte(stride * maskenHoehe - 1) {}

        For y = 0 To maskenHoehe - 1

            For x = 0 To maskenBreite - 1

                distanz = BerechneDistanz(x, y, ursprungX, ursprungY)

                normierteDistanz = distanz / Math.Max(maximaleDistanz, 0.000001)


                noiseWert = BerechneFBM(x / CDbl(maskenBreite) + noiseOffsetX, y / CDbl(maskenHoehe) + noiseOffsetY,
                                        fbmGrundfrequenz, fbmOktaven, fbmPersistenz)

                'Am Ursprung wenig Noise, mit wachsender Entfernung
                'immer stärkeres Aufbrechen der Front.

                noiseEinfluss = Math.Sqrt(Math.Max(0.0, Math.Min(1.0, normierteDistanz)))

                brandWert = normierteDistanz + noiseWert * fbmStaerke * noiseEinfluss
                brandWert = Math.Max(0.0, Math.Min(1.0, brandWert))

                grauwert = CByte(Math.Round(brandWert * 255.0))

                index = y * stride + x * 4

                pixel(index) = grauwert
                pixel(index + 1) = grauwert
                pixel(index + 2) = grauwert
                pixel(index + 3) = 255

            Next

        Next


        bitmap = New WriteableBitmap(maskenBreite, maskenHoehe, 96.0, 96.0, PixelFormats.Bgra32, Nothing)

        bitmap.WritePixels(New Int32Rect(0, 0, maskenBreite, maskenHoehe), pixel, stride, 0)

        bitmap.Freeze()

        Return bitmap

    End Function

    Private Function ErzeugeBrandMaskeAusBrandherden() As BitmapSource

        'Erzeugt die zeitcodierte Reveal-/Brandmaske für einen vollständigen
        'Transitionsdurchlauf.
        '
        'Die Maske ist KEIN bereits gerendertes Feuerbild. Jeder Maskenpixel speichert
        'stattdessen als Grauwert einen normierten Zeitpunkt zwischen 0.0 und 1.0:
        '
        '   0.0  -> dieser Pixel wird unmittelbar zu Beginn freigelegt
        '   0.5  -> dieser Pixel wird etwa in der Mitte der Transition freigelegt
        '   1.0  -> dieser Pixel wird erst ganz am Ende freigelegt
        '
        'Der RevealShader vergleicht diesen gespeicherten Brandzeitpunkt später mit
        'dem aktuellen Progress der Transition. Dadurch muss die geometrisch aufwendige
        'Brandentwicklung nur EINMAL pro Transition auf der CPU berechnet werden.
        'Während der eigentlichen Animation liest die GPU lediglich die fertige Maske.
        '
        'Die Brandgeometrie basiert auf einem Voronoi-Diagramm:
        '
        '   - Jeder echte Brandherd bildet den Kern einer Voronoi-Zelle.
        '   - Jeder Maskenpixel wird dem räumlich nächsten Brandherd zugeordnet.
        '   - Innerhalb jeder Zelle breitet sich der Brand vom Herd zur Zellmembran aus.
        '   - Die Entfernung wird nicht absolut, sondern relativ zur lokalen Zellgröße
        '     bewertet. Große und kleine Zellen brennen dadurch beide vollständig durch.
        '
        'Die Brandfront würde bei reinem Voronoi jedoch geometrisch zu sauber wirken.
        'Deshalb wird die relative Distanz zusätzlich mit kohärentem FBM-Noise
        '(Fractal Brownian Motion) verzerrt. Die Verzerrung nimmt zum Zellrand hin zu:
        'Der Brandherd selbst bleibt stabil, während die Membran unregelmäßig,
        'organisch und papierähnlich ausfranst.
        '
        'WICHTIG ZUM OVERSCAN:
        'Die fertige Bitmap selbst besitzt nur die sichtbare Maskengröße.
        'Der Overscan erweitert stattdessen den Raum, in dem Brandherde liegen dürfen.
        'Dadurch können Zellen außerhalb des sichtbaren Bildes entstehen und von außen
        'in das Papier hineinbrennen. Zusätzlich sorgt ein separater Begrenzungsring aus
        'Sperr-/Guard-Seeds dafür, dass auch Randzellen mathematisch geschlossen bleiben.
        '
        'Ablauf:
        '
        'PASS 1:
        'Für jeden Pixel wird der nächstgelegene ECHTE Brandherd und damit seine
        'Voronoi-Zelle bestimmt.
        '
        'PASS 2:
        'Für jeden Pixel wird innerhalb dieser Zelle berechnet:
        '
        '   Distanz Brandherd -> Pixel
        '   -------------------------------- = relativeDistanz
        '   Distanz Brandherd -> Zellmembran
        '
        'Diese relative Distanz wird mit FBM verzerrt und anschließend zwischen dem
        'individuellen Start- und Endzeitpunkt des Brandherdes interpoliert.
        '
        'Das Ergebnis ist eine fertige zeitcodierte Grauwertmaske, die anschließend
        'unverändert als Texture an Direct3D übergeben werden kann.

        Dim maskenBreite As Integer
        Dim maskenHoehe As Integer

        Dim brandherde As List(Of Brandherd)

        Dim kandidaten As BrandherdKandidaten()

        Dim pixelKandidaten As BrandherdKandidaten

        Dim kandidatenIndizes(3) As Integer
        Dim kandidatNummer As Integer

        Dim kandidatBrandWert As Double
        Dim kleinsterBrandWert As Double

        Dim pixel As Byte()
        Dim stride As Integer

        Dim bitmap As WriteableBitmap

        Dim x As Integer
        Dim y As Integer
        Dim index As Integer

        Dim brandherdIndex As Integer

        Dim noiseOffsetX As Double
        Dim noiseOffsetY As Double

        'Die Brandmaske muss nicht in voller Bildschirmauflösung berechnet werden.
        'Die GPU sampelt sie später auf die Rendergröße hoch.
        '
        'Die maximale Breite begrenzt daher bewusst den CPU-Aufwand der Voronoi- und
        'FBM-Berechnung. Das Seitenverhältnis entspricht weiterhin exakt dem sichtbaren
        'Renderbereich, damit Bild-UV und Masken-UV direkt 1:1 verwendet werden können.

        maskenBreite = Math.Min(BRANDMASKE_MAX_BREITE, renderBreite)
        maskenHoehe = Math.Max(1, CInt(maskenBreite * renderHoehe / CDbl(renderBreite)))

        brandherde = ErzeugeBrandherde(maskenBreite, maskenHoehe)

        kandidaten = New BrandherdKandidaten(maskenBreite * maskenHoehe - 1) {}

        'Noise initialisieren
        noiseOffsetX = rnd.NextDouble() * 1000.0
        noiseOffsetY = rnd.NextDouble() * 1000.0

        '---------------------------------
        ' PASS 1:
        ' konkurrierende Brandherde bestimmen
        '---------------------------------
        '
        'Ein Pixel besitzt ab jetzt keinen dauerhaft zuständigen Voronoi-Herd mehr.
        '
        'Stattdessen werden die vier räumlich nächsten echten Brandherde gespeichert.
        'Die klassische Voronoi-Struktur bleibt dadurch als räumliche Grundordnung
        'erhalten, ist aber keine undurchdringliche Zellgrenze mehr.

        index = 0

        For y = 0 To maskenHoehe - 1

            For x = 0 To maskenBreite - 1

                kandidaten(index) = ErmittleNaechsteBrandherde(x, y, brandherde)

                index += 1

            Next

        Next

        '---------------------------------
        ' PASS 2:
        ' Brandzeit innerhalb der Zellen
        '---------------------------------

        stride = maskenBreite * 4

        pixel = New Byte(stride * maskenHoehe - 1) {}

        index = 0

        For y = 0 To maskenHoehe - 1

            For x = 0 To maskenBreite - 1

                pixelKandidaten = kandidaten(index)

                kandidatenIndizes(0) = pixelKandidaten.index0
                kandidatenIndizes(1) = pixelKandidaten.index1
                kandidatenIndizes(2) = pixelKandidaten.index2
                kandidatenIndizes(3) = pixelKandidaten.index3

                kleinsterBrandWert = 1.0

                For kandidatNummer = 0 To 3

                    brandherdIndex = kandidatenIndizes(kandidatNummer)

                    If brandherdIndex >= 0 Then

                        kandidatBrandWert = BerechneBrandWertFuerHerd(x, y, brandherdIndex, brandherde,
                                                                      maskenBreite, maskenHoehe, noiseOffsetX,
                                                                      noiseOffsetY)

                        If kandidatBrandWert < kleinsterBrandWert Then

                            kleinsterBrandWert = kandidatBrandWert

                        End If

                    End If

                Next

                SchreibeGrauwertPixel(pixel, index, kleinsterBrandWert)

                index += 1

            Next

        Next

        bitmap = New WriteableBitmap(maskenBreite, maskenHoehe, 96.0, 96.0, PixelFormats.Bgra32, Nothing)
        bitmap.WritePixels(New Int32Rect(0, 0, maskenBreite, maskenHoehe), pixel, stride, 0)
        bitmap.Freeze()

        bitmap.Freeze()

        Return bitmap

    End Function

    Private Function BerechneBrandWertFuerHerd(x As Integer, y As Integer, brandherdIndex As Integer,
                                               brandherde As List(Of Brandherd), maskenBreite As Integer,
                                               maskenHoehe As Integer, noiseOffsetX As Double,
                                               noiseOffsetY As Double) As Double

        Dim eigenerHerd As Brandherd

        Dim distanzZumKern As Double
        Dim distanzZurMembran As Double

        Dim relativeDistanz As Double
        Dim relativeDistanzMakro As Double
        Dim relativeDistanzVerzerrt As Double

        Dim makroX As Double
        Dim makroY As Double
        Dim makroNoiseWert As Double
        Dim makroEinfluss As Double

        Dim noiseX As Double
        Dim noiseY As Double

        Dim noiseWert As Double
        Dim noiseEinfluss As Double

        Dim brandWert As Double

        eigenerHerd =
        brandherde(
            brandherdIndex)

        distanzZumKern = BerechneDistanz(x, y, eigenerHerd.x, eigenerHerd.y)
        distanzZurMembran = BerechneDistanzBisZellmembran(x, y, brandherdIndex, brandherde)

        If distanzZurMembran <= 0.000001 Then

            Return 1.0

        End If

        '
        'WICHTIG:
        'relativeDistanz wird jetzt NICHT mehr nach oben auf 1.0 begrenzt.
        '
        '0..1  = Pixel liegt innerhalb der ursprünglichen Voronoi-Zelle.
        '> 1   = Pixel liegt bereits auf dem ehemaligen Grundstück eines
        '        Nachbarherdes.
        '
        'Genau dieser Bereich > 1 ermöglicht das Überbrennen der alten Zellgrenze.
        '
        relativeDistanz = distanzZumKern / distanzZurMembran
        relativeDistanz = Math.Max(0.0, relativeDistanz)

        '
        '---------------------------------
        ' Individuelle Makroverzerrung
        '---------------------------------
        '

        makroX = ((x - eigenerHerd.x) / distanzZurMembran) * MAKRO_GRUNDFREQUENZ + eigenerHerd.makroOffsetX
        makroY = ((y - eigenerHerd.y) / distanzZurMembran) * MAKRO_GRUNDFREQUENZ + eigenerHerd.makroOffsetY

        makroNoiseWert = BerechneGradientNoise(makroX, makroY)

        'Die Verzerrungsstärke erreicht spätestens an der ehemaligen Zellmembran
        '100 %, wächst außerhalb der Zelle aber nicht unbegrenzt weiter.

        makroEinfluss = Math.Sqrt(Math.Min(1.0, relativeDistanz))

        relativeDistanzMakro = relativeDistanz + makroNoiseWert * MAKRO_STAERKE * makroEinfluss
        relativeDistanzMakro = Math.Max(0.0, relativeDistanzMakro)

        'ACHTUNG:
        'Auch hier KEIN Clamp auf maximal 1.0.
        '
        '---------------------------------
        ' FBM-Mikroverzerrung
        '---------------------------------

        noiseX = x / CDbl(maskenBreite) + noiseOffsetX
        noiseY = y / CDbl(maskenHoehe) + noiseOffsetY

        noiseWert = BerechneFBM(noiseX, noiseY, fbmGrundfrequenz, fbmOktaven, fbmPersistenz)

        noiseEinfluss = Math.Sqrt(Math.Min(1.0, relativeDistanzMakro))

        relativeDistanzVerzerrt = relativeDistanzMakro + noiseWert * fbmStaerke * noiseEinfluss
        relativeDistanzVerzerrt = Math.Max(0.0, relativeDistanzVerzerrt)

        'Die Ausbreitung läuft auch jenseits der ehemaligen Zellmembran
        'mit derselben lokalen Geschwindigkeit weiter.

        brandWert = eigenerHerd.startwert + relativeDistanzVerzerrt * (eigenerHerd.endwert - eigenerHerd.startwert)
        brandWert = Math.Max(0.0, Math.Min(1.0, brandWert))

        Return brandWert

    End Function

    Private Function ErzeugeBrandherde(maskenBreite As Integer, maskenHoehe As Integer) As List(Of Brandherd)

        'Erzeugt sämtliche Punkte, die zur Konstruktion des Voronoi-Feldes benötigt
        'werden.
        '
        'Es existieren dabei zwei grundsätzlich verschiedene Herdtypen:
        '
        '1. ECHTE Brandherde
        '   Sie besitzen startwert/endwert und können sichtbare Maskenpixel übernehmen.
        '   Von ihnen breitet sich die Verbrennung tatsächlich aus.
        '
        '2. BEGRENZUNGSHERDE / GUARD-SEEDS
        '   Sie liegen außerhalb des sichtbaren Bereiches, besitzen keine sichtbare
        '   Brandzelle und dienen ausschließlich dazu, offene Voronoi-Zellen am Rand
        '   mathematisch zu schließen.
        '
        'Die echten Brandherde werden absichtlich nicht vollkommen gleichmäßig verteilt:
        '
        '   - ein Teil liegt frei im gesamten Overscan-Bereich,
        '   - ein Teil wird in zufällig erzeugten Regionen geclustert.
        '
        'Dadurch entstehen sowohl größere ruhige Brandflächen als auch Zonen mit vielen
        'kleinen, schnell ineinandergreifenden Brandherden.

        Dim brandherde As New List(Of Brandherd)
        Dim regionen As List(Of BrandherdRegion)

        Dim anzahlGesamt As Integer
        Dim anzahlCluster As Integer
        Dim anzahlFrei As Integer

        Dim index As Integer
        Dim regionIndex As Integer

        Dim overscanX As Double
        Dim overscanY As Double

        Dim herd As Brandherd
        Dim region As BrandherdRegion

        Dim winkel As Double
        Dim abstand As Double

        anzahlGesamt = rnd.Next(BRANDHERDE_MIN, BRANDHERDE_MAX + 1)
        anzahlCluster = CInt(Math.Round(anzahlGesamt * BRANDHERDE_CLUSTER_ANTEIL))
        anzahlFrei = anzahlGesamt - anzahlCluster

        'Der Overscan erweitert ausschließlich den möglichen POSITIONIERUNGSRAUM der
        'Brandherde. Die erzeugte Bitmap selbst wird dadurch nicht größer.
        '
        'Brandherde dürfen damit außerhalb des sichtbaren Papiers liegen. Ihre Voronoi-
        'Zellen schneiden den sichtbaren Bereich und lassen die Verbrennung glaubwürdig
        'von außen über den Papierrand hineinlaufen.

        overscanX = maskenBreite * BRANDMASKE_OVERSCAN
        overscanY = maskenHoehe * BRANDMASKE_OVERSCAN

        regionen = ErzeugeBrandherdRegionen(maskenBreite, maskenHoehe)

        'Mindestens ein Herd beginnt garantiert bei Progress = 0 innerhalb des sichtbaren
        'Bereiches. Damit besitzt jede Transition sofort einen sichtbaren Zündpunkt und
        'kann nicht zufällig mehrere Sekunden lang scheinbar "nichts tun".

        herd.x = rnd.NextDouble() * maskenBreite
        herd.y = rnd.NextDouble() * maskenHoehe

        herd.startwert = 0.0
        herd.endwert = BRANDENDE_MAX

        herd.makroOffsetX = rnd.NextDouble() * 1000.0
        herd.makroOffsetY = rnd.NextDouble() * 1000.0

        herd.istBegrenzungsHerd = False

        brandherde.Add(herd)

        '---------------------------------
        ' Frei verteilte Brandherde
        '---------------------------------

        For index = 1 To anzahlFrei - 1

            herd.x = -overscanX + rnd.NextDouble() * (maskenBreite + 2.0 * overscanX)
            herd.y = -overscanY + rnd.NextDouble() * (maskenHoehe + 2.0 * overscanY)

            herd.startwert = rnd.NextDouble() * BRANDSTART_MAX

            herd.endwert = BRANDENDE_MIN + rnd.NextDouble() * (BRANDENDE_MAX - BRANDENDE_MIN)
            herd.endwert = Math.Max(herd.startwert + 0.1, herd.endwert)

            'Ein kleiner zeitlicher Sicherheitsabstand zu Progress = 1.0 stellt sicher,
            'dass diese Zellen vor dem endgültigen Endframe vollständig aufbrechen können.
            herd.endwert = Math.Min(0.97, herd.endwert)

            herd.makroOffsetX = rnd.NextDouble() * 1000.0
            herd.makroOffsetY = rnd.NextDouble() * 1000.0

            herd.istBegrenzungsHerd = False

            brandherde.Add(herd)

        Next

        '---------------------------------
        ' Brandherde innerhalb dichter Regionen
        '---------------------------------

        For index = 0 To anzahlCluster - 1

            regionIndex = rnd.Next(0, regionen.Count)

            region = regionen(regionIndex)

            winkel = rnd.NextDouble() * Math.PI * 2.0

            'Für eine flächengleichmäßige Zufallsverteilung in einem Kreis darf der Radius
            'nicht linear aus rnd.NextDouble() gewählt werden.
            '
            'Bei linearer Radiuswahl hätten alle radialen Intervalle dieselbe Wahrscheinlichkeit,
            'obwohl äußere Kreisringe eine deutlich größere Fläche besitzen. Dadurch würden
            'Punkte künstlich zum Zentrum hin konzentriert.
            '
            'Sqrt(U) korrigiert genau diesen Flächeneffekt.
            abstand = Math.Sqrt(rnd.NextDouble()) * region.radius

            herd.x = region.x + Math.Cos(winkel) * abstand
            herd.y = region.y + Math.Sin(winkel) * abstand

            herd.startwert = rnd.NextDouble() * BRANDSTART_MAX

            herd.endwert = BRANDENDE_MIN + rnd.NextDouble() * (BRANDENDE_MAX - BRANDENDE_MIN)
            herd.endwert = Math.Max(herd.startwert + 0.1, herd.endwert)
            herd.endwert = Math.Min(BRANDENDE_MAX, herd.endwert)

            herd.makroOffsetX = rnd.NextDouble() * 1000.0
            herd.makroOffsetY = rnd.NextDouble() * 1000.0

            herd.istBegrenzungsHerd = False

            brandherde.Add(herd)

        Next

        'Äußerer Guard-Ring.
        FuegeBegrenzungsHerdeHinzu(brandherde, maskenBreite, maskenHoehe)

        Return brandherde

    End Function

    Private Function ErmittleNaechsteBrandherde(x As Double, y As Double, brandherde As List(Of Brandherd)) _
        As BrandherdKandidaten

        Dim ergebnis As BrandherdKandidaten

        Dim index As Integer

        Dim deltaX As Double
        Dim deltaY As Double
        Dim distanzQuadrat As Double

        Dim distanz0 As Double
        Dim distanz1 As Double
        Dim distanz2 As Double
        Dim distanz3 As Double

        ergebnis.index0 = -1
        ergebnis.index1 = -1
        ergebnis.index2 = -1
        ergebnis.index3 = -1

        distanz0 = Double.MaxValue
        distanz1 = Double.MaxValue
        distanz2 = Double.MaxValue
        distanz3 = Double.MaxValue

        For index = 0 To brandherde.Count - 1

            If Not brandherde(index).istBegrenzungsHerd Then

                deltaX = x - brandherde(index).x
                deltaY = y - brandherde(index).y

                distanzQuadrat = deltaX * deltaX + deltaY * deltaY

                If distanzQuadrat < distanz0 Then

                    distanz3 = distanz2
                    ergebnis.index3 = ergebnis.index2

                    distanz2 = distanz1
                    ergebnis.index2 = ergebnis.index1

                    distanz1 = distanz0
                    ergebnis.index1 = ergebnis.index0

                    distanz0 = distanzQuadrat
                    ergebnis.index0 = index

                ElseIf distanzQuadrat < distanz1 Then

                    distanz3 = distanz2
                    ergebnis.index3 = ergebnis.index2

                    distanz2 = distanz1
                    ergebnis.index2 = ergebnis.index1

                    distanz1 = distanzQuadrat
                    ergebnis.index1 = index

                ElseIf distanzQuadrat < distanz2 Then

                    distanz3 = distanz2
                    ergebnis.index3 = ergebnis.index2

                    distanz2 = distanzQuadrat
                    ergebnis.index2 = index

                ElseIf distanzQuadrat < distanz3 Then

                    distanz3 = distanzQuadrat
                    ergebnis.index3 = index

                End If

            End If

        Next

        If ergebnis.index0 < 0 Then

            Throw New InvalidOperationException("Für den Maskenpixel konnte kein Brandherd bestimmt werden.")

        End If

        Return ergebnis

    End Function

    Private Function ErzeugeBrandherdRegionen(maskenBreite As Integer, maskenHoehe As Integer) _
        As List(Of BrandherdRegion)

        'Erzeugt virtuelle Clusterbereiche für die räumliche Verteilung von Brandherden.
        '
        'Eine Region ist selbst KEIN Brandherd und erscheint auch nicht in der Maske.
        'Sie beschreibt lediglich einen Kreis, innerhalb dessen mehrere echte Brandherde
        'wahrscheinlicher platziert werden.
        '
        'Dadurch wird die Punktverteilung bewusst inhomogen:
        'Reale Verbrennung bildet typischerweise keine mathematisch gleichmäßig
        'verteilten Zündpunkte, sondern lokale Häufungen und größere Zwischenräume.

        Dim regionen As New List(Of BrandherdRegion)

        Dim anzahl As Integer
        Dim index As Integer

        Dim minDimension As Double
        Dim radiusFaktor As Double

        Dim region As BrandherdRegion

        anzahl = rnd.Next(BRANDREGIONEN_MIN, BRANDREGIONEN_MAX + 1)

        minDimension = Math.Min(maskenBreite, maskenHoehe)

        For index = 0 To anzahl - 1

            region.x = rnd.NextDouble() * maskenBreite
            region.y = rnd.NextDouble() * maskenHoehe

            radiusFaktor = BRANDREGION_RADIUS_MIN + rnd.NextDouble() * (BRANDREGION_RADIUS_MAX -
                           BRANDREGION_RADIUS_MIN)

            region.radius = minDimension * radiusFaktor

            regionen.Add(region)

        Next

        Return regionen

    End Function

    Private Sub FuegeBegrenzungsHerdeHinzu(brandherde As List(Of Brandherd), maskenBreite As Integer,
                                           maskenHoehe As Integer)

        'Erzeugt einen rechteckigen Ring aus künstlichen Voronoi-Seeds außerhalb der
        'sichtbaren Maske.
        '
        'Warum wird dieser Ring benötigt?
        '
        'Eine gewöhnliche Voronoi-Zelle am äußeren Rand einer endlichen Punktmenge kann
        'nach außen unbeschränkt sein. Für unsere Brandberechnung wäre dann entlang
        'bestimmter Richtungen keine Zellmembran vorhanden.
        '
        'Wir benötigen jedoch für JEDEN Pixel eine endliche Entfernung
        '
        '   Brandherd -> Zellmembran
        '
        'damit die relative Branddistanz zuverlässig auf 0..1 normiert werden kann.
        '
        'Die Guard-Seeds schließen deshalb die äußeren Zellen künstlich.
        '
        'WICHTIG:
        'Diese Seeds sind KEINE echten Brandherde.
        'Sie werden bei ErmittleNaechstenBrandherd() ignoriert und können daher niemals
        'sichtbare Maskenpixel besitzen oder selbst eine Verbrennung starten.
        '
        'Sie existieren ausschließlich als geometrische Gegenpunkte bei der Berechnung
        'der Zellmembran.

        Dim minDimension As Double

        Dim ringAbstand As Double
        Dim seedAbstand As Double

        Dim links As Double
        Dim rechts As Double
        Dim oben As Double
        Dim unten As Double

        Dim x As Double
        Dim y As Double

        minDimension = Math.Min(maskenBreite, maskenHoehe)

        ringAbstand = minDimension * BEGRENZUNGSRING_ABSTAND

        seedAbstand = minDimension * BEGRENZUNGSRING_ABSTAND_ZWISCHEN_SEEDS

        links = -ringAbstand
        rechts = maskenBreite + ringAbstand
        oben = -ringAbstand
        unten = maskenHoehe + ringAbstand

        'Obere und untere Begrenzung
        x = links

        Do While x <= rechts

            FuegeBegrenzungsHerdHinzu(brandherde, x, oben)
            FuegeBegrenzungsHerdHinzu(brandherde, x, unten)

            x += seedAbstand

        Loop

        'Linke und rechte Begrenzung
        y = oben + seedAbstand

        Do While y < unten

            FuegeBegrenzungsHerdHinzu(brandherde, links, y)
            FuegeBegrenzungsHerdHinzu(brandherde, rechts, y)

            y += seedAbstand

        Loop

    End Sub

    Private Sub FuegeBegrenzungsHerdHinzu(brandherde As List(Of Brandherd), x As Double, y As Double)

        Dim herd As Brandherd

        herd.x = x
        herd.y = y

        herd.startwert = 1.0

        herd.makroOffsetX = 0.0
        herd.makroOffsetY = 0.0

        herd.istBegrenzungsHerd = True

        brandherde.Add(herd)

    End Sub

    Private Function BerechneDistanzBisZellmembran(x As Double, y As Double, eigenerIndex As Integer,
                                                   brandherde As List(Of Brandherd)) As Double
        'Berechnet für einen Pixel die Entfernung vom Kern seiner Voronoi-Zelle bis
        'zur Zellmembran exakt entlang der Richtung Herd -> Pixel.
        '
        'Gesucht wird also NICHT die allgemein kürzeste Entfernung zur Zellgrenze,
        'sondern der Schnittpunkt der radialen Halbgeraden vom eigenen Herd durch den
        'betrachteten Pixel mit der ersten Voronoi-Grenze.
        '
        'Für jeden anderen Seed gilt auf der gemeinsamen Voronoi-Grenze:
        '
        '   Abstand(Punkt, eigenerHerd) = Abstand(Punkt, andererHerd)
        '
        'Setzt man den Punkt als
        '
        '   eigenerHerd + t * richtungsVektor
        '
        'ein und löst nach t auf, ergibt sich:
        '
        '               |andererHerd - eigenerHerd|²
        '   t = ------------------------------------------------
        '       2 * dot(richtung, andererHerd - eigenerHerd)
        '
        'Nur Seeds mit positivem Skalarprodukt liegen vor dem Herd in der betrachteten
        'Strahlrichtung und können dort eine Grenze bilden.
        '
        'Von allen gültigen Kandidaten wird der kleinste positive t-Wert gewählt:
        'Das ist die erste Zellmembran, auf die der Strahl trifft.

        Dim eigenerHerd As Brandherd
        Dim andererHerd As Brandherd

        Dim richtungX As Double
        Dim richtungY As Double

        Dim pixelDeltaX As Double
        Dim pixelDeltaY As Double

        Dim pixelDistanz As Double

        Dim andererDeltaX As Double
        Dim andererDeltaY As Double

        Dim skalarprodukt As Double
        Dim abstandQuadrat As Double

        Dim grenzDistanz As Double
        Dim kleinsteGrenzDistanz As Double

        Dim index As Integer

        eigenerHerd = brandherde(eigenerIndex)

        pixelDeltaX = x - eigenerHerd.x
        pixelDeltaY = y - eigenerHerd.y

        pixelDistanz = Math.Sqrt(pixelDeltaX * pixelDeltaX + pixelDeltaY * pixelDeltaY)

        If pixelDistanz <= 0.000001 Then

            Return 1.0

        End If

        richtungX = pixelDeltaX / pixelDistanz
        richtungY = pixelDeltaY / pixelDistanz

        kleinsteGrenzDistanz = Double.MaxValue

        For index = 0 To brandherde.Count - 1

            If index <> eigenerIndex Then

                andererHerd = brandherde(index)

                andererDeltaX = andererHerd.x - eigenerHerd.x
                andererDeltaY = andererHerd.y - eigenerHerd.y

                skalarprodukt = richtungX * andererDeltaX + richtungY * andererDeltaY

                If skalarprodukt > 0.000001 Then

                    abstandQuadrat = andererDeltaX * andererDeltaX + andererDeltaY * andererDeltaY

                    grenzDistanz = abstandQuadrat / (2.0 * skalarprodukt)

                    If grenzDistanz < kleinsteGrenzDistanz Then

                        kleinsteGrenzDistanz = grenzDistanz

                    End If

                End If

            End If

        Next

        If kleinsteGrenzDistanz = Double.MaxValue Then

            Throw New InvalidOperationException("Die Voronoi-Zelle besitzt trotz Begrenzungsring keine " &
                                                "geschlossene Zellmembran.")

        End If

        '
        'Der betrachtete Pixel darf außerhalb der ursprünglichen Voronoi-Zelle
        'des Brandherdes liegen.
        '
        'Das ist ausdrücklich erwünscht:
        'Mehrere benachbarte Brandherde konkurrieren später darum, welcher von
        'ihnen diesen Pixel zeitlich zuerst erreicht.
        '
        'Liegt die Zellmembran vor dem Pixel, wird deshalb bewusst die tatsächliche
        'Grenzdistanz zurückgegeben. Dadurch kann relativeDistanz > 1.0 werden.
        '
        Return kleinsteGrenzDistanz

    End Function

    Private Function BerechneDistanz(x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double

        Dim deltaX As Double
        Dim deltaY As Double

        deltaX = x2 - x1
        deltaY = y2 - y1

        Return Math.Sqrt(deltaX * deltaX + deltaY * deltaY)

    End Function

    Private Sub SchreibeGrauwertPixel(pixel As Byte(), pixelIndex As Integer, wert As Double)

        Dim byteWert As Byte
        Dim basisIndex As Integer

        byteWert = CByte(Math.Round(Math.Max(0.0, Math.Min(1.0, wert)) * 255.0))

        basisIndex = pixelIndex * 4

        pixel(basisIndex) = byteWert
        pixel(basisIndex + 1) = byteWert
        pixel(basisIndex + 2) = byteWert
        pixel(basisIndex + 3) = 255

    End Sub

    Private Function BerechneFBM(x As Double, y As Double, grundFrequenz As Double, oktaven As Integer,
                                 persistenz As Double) As Double
        'Berechnet zweidimensionales Fractal Brownian Motion (FBM).
        '
        'FBM kombiniert mehrere unterschiedlich skalierte Ebenen desselben kohärenten
        'Gradient-Noise:
        '
        '   Oktave 0: niedrige Frequenz, hohe Amplitude
        '             -> große, weiche Verformungen
        '
        '   Oktave 1: doppelte Frequenz, kleinere Amplitude
        '             -> mittelgroße Strukturen
        '
        '   ...
        '
        '   höhere Oktaven
        '             -> zunehmend feinere Details
        '
        'Die Frequenz wird pro Oktave verdoppelt.
        'Die Amplitude wird mit "persistenz" multipliziert.
        '
        'Die Summe wird anschließend durch die Summe aller Amplituden normalisiert,
        'damit sich die ungefähre Ausgangsamplitude nicht mit der Anzahl der Oktaven
        'verändert.
        '
        'Das Ergebnis liegt näherungsweise im Bereich -1.0 bis +1.0 und wird nicht als
        'eigenständige Textur benutzt, sondern zur organischen Verzerrung der radialen
        'Voronoi-Brandfront.

        Dim oktave As Integer

        Dim frequenz As Double
        Dim amplitude As Double

        Dim noiseSumme As Double
        Dim amplitudenSumme As Double

        frequenz = grundFrequenz
        amplitude = 1.0

        noiseSumme = 0.0
        amplitudenSumme = 0.0

        For oktave = 0 To oktaven - 1

            noiseSumme += BerechneGradientNoise(x * frequenz, y * frequenz) * amplitude
            amplitudenSumme += amplitude

            frequenz *= 2.0

            amplitude *= persistenz

        Next

        If amplitudenSumme <= 0.0 Then

            Return 0.0

        End If

        Return noiseSumme / amplitudenSumme

    End Function

    Private Function BerechneGradientNoise(x As Double, y As Double) As Double
        'Berechnet kohärentes zweidimensionales Gradient Noise nach dem Grundprinzip
        'von Perlin Noise.
        '
        'Der Raum wird in ein ganzzahliges Gitter zerlegt. Für die vier Gitterecken der
        'Zelle, in der (x,y) liegt, wird jeweils ein deterministischer Pseudozufalls-
        'Gradient bestimmt.
        '
        'An jeder Ecke wird das Skalarprodukt berechnet aus:
        '
        '   Gradient der Ecke
        '       DOT
        '   Vektor von der Ecke zum Abfragepunkt
        '
        'Die vier Ergebnisse werden anschließend weich interpoliert.
        '
        'Im Gegensatz zu unabhängigem Random-Noise ändern sich benachbarte Werte dadurch
        'kontinuierlich. Genau diese räumliche Kohärenz verhindert pixeliges Flimmern
        'und erzeugt zusammenhängende organische Formen.

        Dim x0 As Integer
        Dim y0 As Integer

        Dim x1 As Integer
        Dim y1 As Integer

        Dim lokalX As Double
        Dim lokalY As Double

        Dim wert00 As Double
        Dim wert10 As Double
        Dim wert01 As Double
        Dim wert11 As Double

        Dim interpolationX As Double
        Dim interpolationY As Double

        Dim oben As Double
        Dim unten As Double

        x0 = CInt(Math.Floor(x))
        y0 = CInt(Math.Floor(y))

        x1 = x0 + 1
        y1 = y0 + 1

        lokalX = x - x0
        lokalY = y - y0

        wert00 = BerechneGradientProdukt(x0, y0, lokalX, lokalY)
        wert10 = BerechneGradientProdukt(x1, y0, lokalX - 1.0, lokalY)
        wert01 = BerechneGradientProdukt(x0, y1, lokalX, lokalY - 1.0)
        wert11 = BerechneGradientProdukt(x1, y1, lokalX - 1.0, lokalY - 1.0)

        interpolationX = Fade(lokalX)
        interpolationY = Fade(lokalY)

        oben = Interpoliere(wert00, wert10, interpolationX)
        unten = Interpoliere(wert01, wert11, interpolationX)

        Return Interpoliere(oben, unten, interpolationY)

    End Function

    Private Function BerechneGradientProdukt(gitterX As Integer, gitterY As Integer, deltaX As Double,
                                             deltaY As Double) As Double

        Dim hash As Integer

        Dim gradientX As Double
        Dim gradientY As Double

        hash = BerechneNoiseHash(gitterX, gitterY)


        'Der Hash wählt deterministisch einen von acht gleichmäßig über den Kreis
        'verteilten 2D-Richtungsvektoren aus:
        '
        '   horizontal, vertikal und vier Diagonalen.
        '
        'Die diagonalen Komponenten besitzen 1/sqrt(2), damit alle Gradientenvektoren
        'dieselbe Länge 1 besitzen.

        Select Case hash

            Case 0

                gradientX = 1.0
                gradientY = 0.0

            Case 1

                gradientX = -1.0
                gradientY = 0.0

            Case 2

                gradientX = 0.0
                gradientY = 1.0

            Case 3

                gradientX = 0.0
                gradientY = -1.0

            Case 4

                gradientX = 0.70710678118654757
                gradientY = 0.70710678118654757

            Case 5

                gradientX = -0.70710678118654757
                gradientY = 0.70710678118654757

            Case 6

                gradientX = 0.70710678118654757
                gradientY = -0.70710678118654757

            Case Else

                gradientX = -0.70710678118654757
                gradientY = -0.70710678118654757

        End Select

        Return gradientX * deltaX + gradientY * deltaY

    End Function

    Private Function BerechneNoiseHash(x As Integer, y As Integer) As Integer
        'Erzeugt aus ganzzahligen Gitterkoordinaten deterministisch einen Wert 0..7.
        '
        'Wichtig ist hier nicht kryptographische Qualität, sondern eine schnelle und
        'hinreichend gut durchmischte räumliche Verteilung:
        '
        '   gleiche Koordinate -> immer gleicher Gradient
        '   benachbarte Koordinaten -> möglichst unkorrelierte Gradientenauswahl
        '
        'Dadurch benötigt das Gradient-Noise weder eine vorberechnete Permutationstabelle
        'noch zusätzlichen persistenten Zustand.

        Dim hash As Long

        hash = CLng(x) * 374761393L + CLng(y) * 668265263L

        hash = hash Xor (hash >> 13)

        'Vor der nächsten Multiplikation bewusst auf
        '31 Bit begrenzen, damit Long nicht überlaufen kann.
        hash = hash And &H7FFFFFFFL
        hash *= 1274126177L
        hash = hash And &H7FFFFFFFL
        hash = hash Xor (hash >> 16)

        Return CInt(hash And 7L)

    End Function

    Private Function Fade(wert As Double) As Double
        'Perlin-Fade-Kurve:
        '
        '   f(t) = 6t^5 - 15t^4 + 10t^3
        '
        'Sie besitzt an t=0 und t=1 sowohl eine Steigung als auch eine zweite Ableitung
        'von 0. Dadurch gehen benachbarte Noise-Gitterzellen besonders weich ineinander
        'über und sichtbare Knicke an den Zellgrenzen werden vermieden.

        Return wert * wert * wert * (wert * (wert * 6.0 - 15.0) + 10.0)

    End Function

    Private Function Interpoliere(startwert As Double, endwert As Double, faktor As Double) As Double

        Return startwert + (endwert - startwert) * faktor

    End Function

#End Region

End Class