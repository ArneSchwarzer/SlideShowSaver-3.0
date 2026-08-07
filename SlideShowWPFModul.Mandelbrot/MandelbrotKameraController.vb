Imports System.Diagnostics

Public Class MandelbrotKameraController

#Region "Interne Datentypen"

    Private Structure KameraPhase

        Public Property Typ As MandelbrotKameraZustand.KameraPhaseTyp

        Public Property Dauer As TimeSpan

    End Structure

#End Region

#Region "Konstanten"

    Private Const freezeInSekunden As Double = 2.0
    Private Const translationSekunden As Double = 4.0
    Private Const maxEaseOutSekunden As Double = 5.0
    Private Const freezeOutSekunden As Double = 15.0

#End Region

#Region "Variablendeklaration"

    Private ReadOnly zufall As Random
    Private ReadOnly minRotationsWinkel As Double
    Private ReadOnly maxRotationsWinkel As Double
    Private ReadOnly startIterationen As Integer

    Private ReadOnly laufzeit As Stopwatch
    Private ReadOnly zustand As MandelbrotKameraZustand

    Private startpunkt As MandelbrotZiel
    Private zielpunkt As MandelbrotZiel

    Private kameraPhasen As List(Of KameraPhase)
    Private aktuellePhaseIndex As Integer
    Private phasenStartZeit As TimeSpan

    Private zoomgeschwindigkeit As Integer

    Private startRotation As Double
    Private zielRotation As Double
    Private easeOutStartSkala As Double

    Private kamerafahrtAktiv As Boolean

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property IstAktiv As Boolean
        Get
            Return kamerafahrtAktiv
        End Get
    End Property

    Public ReadOnly Property AktuellerZustand As MandelbrotKameraZustand
        Get
            Return zustand
        End Get
    End Property

#End Region

#Region "Konstruktor"

    Public Sub New(randomGenerator As Random, minimaleRotationGrad As Double, maximaleRotationGrad As Double,
                   basisIterationen As Integer)

        If randomGenerator Is Nothing Then

            Throw New ArgumentNullException(NameOf(randomGenerator))

        End If

        If minimaleRotationGrad < 0.0 Then

            Throw New ArgumentOutOfRangeException(NameOf(minimaleRotationGrad))

        End If

        If maximaleRotationGrad < minimaleRotationGrad Then

            Throw New ArgumentOutOfRangeException(NameOf(maximaleRotationGrad))

        End If

        If basisIterationen < 1 Then

            Throw New ArgumentOutOfRangeException(NameOf(basisIterationen))

        End If

        zufall = randomGenerator
        minRotationsWinkel = minimaleRotationGrad
        maxRotationsWinkel = maximaleRotationGrad
        startIterationen = basisIterationen

        laufzeit = New Stopwatch()
        zustand = New MandelbrotKameraZustand()

        kameraPhasen = New List(Of KameraPhase)()

    End Sub

#End Region

#Region "Kamerafahrt"

    Public Function StarteKamerafahrt(
        neuerStartpunkt As MandelbrotZiel,
        neuerZielpunkt As MandelbrotZiel,
        neueZoomgeschwindigkeit As Integer,
        rotationAktiv As Boolean) As Boolean
        'Initialisiert und startet eine vollständige Kamerafahrt.

        If neuerStartpunkt Is Nothing Then

            Throw New ArgumentNullException(NameOf(neuerStartpunkt))

        End If

        If neuerZielpunkt Is Nothing Then

            Throw New ArgumentNullException(NameOf(neuerZielpunkt))

        End If

        If neuerStartpunkt.TargetScale <= 0.0 Then
            Return False
        End If

        If neuerZielpunkt.TargetScale <= 0.0 OrElse neuerZielpunkt.TargetScale >= neuerStartpunkt.TargetScale Then

            Return False

        End If

        startpunkt = neuerStartpunkt.ErzeugeKopie()
        zielpunkt = neuerZielpunkt.ErzeugeKopie()

        zoomgeschwindigkeit = neueZoomgeschwindigkeit
        InitialisiereRotation(rotationAktiv)

        InitialisierePhasen()

        aktuellePhaseIndex = 0
        phasenStartZeit = TimeSpan.Zero
        easeOutStartSkala = startpunkt.TargetScale

        InitialisiereZustand()

        kamerafahrtAktiv = True

        laufzeit.Restart()

        Return True

    End Function

    Public Function Aktualisiere() As MandelbrotKameraZustand
        'Aktualisiert die Kamera anhand der monotonen Laufzeit.

        Dim aktuellePhase As KameraPhase
        Dim vergangenePhasenZeit As TimeSpan
        Dim phasenFortschritt As Double

        If Not kamerafahrtAktiv Then
            Return zustand
        End If

        If zustand.KamerafahrtBeendet Then
            Return zustand
        End If

        If kameraPhasen Is Nothing OrElse kameraPhasen.Count = 0 Then

            BeendeKamerafahrt()

            Return zustand

        End If

        ÜberspringePhasenOhneDauer()

        If zustand.KamerafahrtBeendet Then
            Return zustand
        End If

        zustand.KamerafahrtSekunden = laufzeit.Elapsed.TotalSeconds

        aktuellePhase = kameraPhasen(aktuellePhaseIndex)

        vergangenePhasenZeit = laufzeit.Elapsed - phasenStartZeit

        phasenFortschritt = Math.Min(1.0, vergangenePhasenZeit.TotalMilliseconds /
                                     aktuellePhase.Dauer.TotalMilliseconds)

        zustand.Phase = aktuellePhase.Typ
        zustand.PhasenFortschritt = phasenFortschritt

        Select Case aktuellePhase.Typ

            Case MandelbrotKameraZustand.KameraPhaseTyp.FreezeIn

                AktualisiereFreezeIn()

            Case MandelbrotKameraZustand.KameraPhaseTyp.Translation

                AktualisiereTranslation(phasenFortschritt)

            Case MandelbrotKameraZustand.KameraPhaseTyp.Cruise

                AktualisiereCruise(vergangenePhasenZeit.TotalSeconds, phasenFortschritt)

            Case MandelbrotKameraZustand.KameraPhaseTyp.EaseOut

                AktualisiereEaseOut(vergangenePhasenZeit.TotalSeconds, aktuellePhase.Dauer.TotalSeconds)

            Case MandelbrotKameraZustand.KameraPhaseTyp.FreezeOut

                AktualisiereFreezeOut()

        End Select

        If phasenFortschritt >= 1.0 Then

            WechsleZurNaechstenPhase()

        End If

        Return zustand

    End Function

    Public Sub Beende()
        'Beendet die aktuelle Kamerafahrt und hält die Zeitmessung an.

        laufzeit.Stop()

        kamerafahrtAktiv = False
        zustand.KamerafahrtBeendet = True

    End Sub

#End Region

#Region "Initialisierung"

    Private Sub InitialisiereRotation(rotationAktiv As Boolean)
        'Initialisiert Start- und Zielrotation.

        startRotation = 0.0

        If rotationAktiv Then

            zielRotation =
                MandelbrotMathematik.BerechneZielRotation(
                    zufall,
                    minRotationsWinkel,
                    maxRotationsWinkel)

        Else

            zielRotation = 0.0

        End If

    End Sub

    Private Sub InitialisierePhasen()
        'Erstellt die fünf Phasen der Kamerafahrt.

        Dim gesamtZoomSekunden As Double
        Dim easeOutSekunden As Double
        Dim cruiseSekunden As Double

        gesamtZoomSekunden =
            MandelbrotMathematik.BerechneGesamtZoomdauerSekunden(
                startpunkt.TargetScale,
                zielpunkt.TargetScale,
                zoomgeschwindigkeit)

        easeOutSekunden =
            Math.Min(
                maxEaseOutSekunden,
                gesamtZoomSekunden * 2.0)

        cruiseSekunden =
            Math.Max(
                0.0,
                gesamtZoomSekunden -
                easeOutSekunden / 2.0)

        kameraPhasen.Clear()

        kameraPhasen.Add(
            New KameraPhase() With {
                .Typ = MandelbrotKameraZustand.KameraPhaseTyp.FreezeIn,
                .Dauer = TimeSpan.FromSeconds(freezeInSekunden)
            })

        kameraPhasen.Add(
            New KameraPhase() With {
                .Typ = MandelbrotKameraZustand.KameraPhaseTyp.Translation,
                .Dauer = TimeSpan.FromSeconds(translationSekunden)
            })

        kameraPhasen.Add(
            New KameraPhase() With {
                .Typ = MandelbrotKameraZustand.KameraPhaseTyp.Cruise,
                .Dauer = TimeSpan.FromSeconds(cruiseSekunden)
            })

        kameraPhasen.Add(
            New KameraPhase() With {
                .Typ = MandelbrotKameraZustand.KameraPhaseTyp.EaseOut,
                .Dauer = TimeSpan.FromSeconds(easeOutSekunden)
            })

        kameraPhasen.Add(
            New KameraPhase() With {
                .Typ = MandelbrotKameraZustand.KameraPhaseTyp.FreezeOut,
                .Dauer = TimeSpan.FromSeconds(freezeOutSekunden)
            })

    End Sub

    Private Sub InitialisiereZustand()
        'Setzt den Zustand auf den Beginn der Kamerafahrt.

        zustand.CenterX = startpunkt.CenterX
        zustand.CenterY = startpunkt.CenterY
        zustand.Skala = startpunkt.TargetScale
        zustand.MaxIterationen = startIterationen
        zustand.Rotation = startRotation
        zustand.ZielRotation = zielRotation
        zustand.Phase = MandelbrotKameraZustand.KameraPhaseTyp.FreezeIn
        zustand.PhasenFortschritt = 0.0
        zustand.KamerafahrtSekunden = 0.0
        zustand.KamerafahrtBeendet = False

    End Sub

#End Region

#Region "Phasenaktualisierung"

    Private Sub AktualisiereFreezeIn()
        'Hält die Kamera auf der vollständigen Mandelbrot-Ansicht.

        zustand.CenterX = startpunkt.CenterX
        zustand.CenterY = startpunkt.CenterY
        zustand.Skala = startpunkt.TargetScale
        zustand.MaxIterationen = startIterationen
        zustand.Rotation = startRotation

    End Sub

    Private Sub AktualisiereTranslation(phasenFortschritt As Double)
        'Bewegt die Kamera zum Zielzentrum, ohne bereits zu zoomen.

        Dim translationPosition As Double

        translationPosition = MandelbrotMathematik.EaseInOut(phasenFortschritt)

        zustand.CenterX =
            MandelbrotMathematik.Lerp(
                startpunkt.CenterX,
                zielpunkt.CenterX,
                translationPosition)

        zustand.CenterY =
            MandelbrotMathematik.Lerp(
                startpunkt.CenterY,
                zielpunkt.CenterY,
                translationPosition)

        zustand.Skala = startpunkt.TargetScale
        zustand.MaxIterationen = startIterationen
        zustand.Rotation = startRotation

    End Sub

    Private Sub AktualisiereCruise(vergangeneSekunden As Double, phasenFortschritt As Double)
        'Führt den Hauptzoom und die Rotation aus.

        Dim rotationsPosition As Double

        zustand.CenterX = zielpunkt.CenterX
        zustand.CenterY = zielpunkt.CenterY

        zustand.Skala =
            Math.Max(
                zielpunkt.TargetScale,
                MandelbrotMathematik.BerechneAktuelleSkala(
                    startpunkt.TargetScale,
                    vergangeneSekunden,
                    zoomgeschwindigkeit))

        zustand.MaxIterationen =
            MandelbrotMathematik.BerechneMaxIterationen(
                startpunkt.TargetScale,
                zustand.Skala,
                startIterationen)

        rotationsPosition = MandelbrotMathematik.EaseInOut(phasenFortschritt)

        zustand.Rotation =
            MandelbrotMathematik.Lerp(
                startRotation,
                zielRotation,
                rotationsPosition)

    End Sub

    Private Sub AktualisiereEaseOut(vergangeneSekunden As Double, dauerSekunden As Double)
        'Bremst den Zoom bis zur Zielskala linear ab.

        Dim effektiveZoomSekunden As Double

        zustand.CenterX = zielpunkt.CenterX
        zustand.CenterY = zielpunkt.CenterY

        If dauerSekunden <= 0.0 Then

            zustand.Skala = zielpunkt.TargetScale

        Else

            effektiveZoomSekunden = vergangeneSekunden - ((vergangeneSekunden * vergangeneSekunden) /
                 (2.0 * dauerSekunden))

            zustand.Skala = Math.Max(zielpunkt.TargetScale,
                    MandelbrotMathematik.BerechneAktuelleSkala(
                        easeOutStartSkala,
                        effektiveZoomSekunden,
                        zoomgeschwindigkeit))

        End If

        zustand.MaxIterationen =
            MandelbrotMathematik.BerechneMaxIterationen(
                startpunkt.TargetScale,
                zustand.Skala,
                startIterationen)

        zustand.Rotation = zielRotation

    End Sub

    Private Sub AktualisiereFreezeOut()
        'Hält die Kamera auf der endgültigen Zielansicht.

        zustand.CenterX = zielpunkt.CenterX
        zustand.CenterY = zielpunkt.CenterY
        zustand.Skala = zielpunkt.TargetScale

        zustand.MaxIterationen =
            MandelbrotMathematik.BerechneMaxIterationen(
                startpunkt.TargetScale,
                zustand.Skala,
                startIterationen)

        zustand.Rotation = zielRotation

    End Sub

#End Region

#Region "Phasensteuerung"

    Private Sub ÜberspringePhasenOhneDauer()
        'Überspringt rechnerisch leere Phasen kontrolliert.

        Dim aktuellePhase As KameraPhase

        Do While aktuellePhaseIndex >= 0 AndAlso aktuellePhaseIndex < kameraPhasen.Count

            aktuellePhase = kameraPhasen(aktuellePhaseIndex)

            If aktuellePhase.Dauer.TotalMilliseconds > 0.0 Then
                Exit Do
            End If

            WechsleZurNaechstenPhase()

            If zustand.KamerafahrtBeendet Then
                Exit Do
            End If

        Loop

    End Sub

    Private Sub WechsleZurNaechstenPhase()
        'Wechselt in die nächste Phase oder beendet die Kamerafahrt.

        Dim neuePhase As MandelbrotKameraZustand.KameraPhaseTyp

        aktuellePhaseIndex += 1

        If aktuellePhaseIndex >= kameraPhasen.Count Then

            BeendeKamerafahrt()

            Exit Sub

        End If

        neuePhase = kameraPhasen(aktuellePhaseIndex).Typ
        phasenStartZeit = laufzeit.Elapsed
        zustand.Phase = neuePhase
        zustand.PhasenFortschritt = 0.0

        Select Case neuePhase

            Case MandelbrotKameraZustand.KameraPhaseTyp.EaseOut

                easeOutStartSkala = zustand.Skala
                zustand.Rotation = zielRotation

        End Select

    End Sub

    Private Sub BeendeKamerafahrt()
        'Markiert die aktuelle Fahrt als vollständig beendet.

        laufzeit.Stop()

        kamerafahrtAktiv = False

        zustand.CenterX = zielpunkt.CenterX
        zustand.CenterY = zielpunkt.CenterY
        zustand.Skala = zielpunkt.TargetScale

        zustand.MaxIterationen =
            MandelbrotMathematik.BerechneMaxIterationen(
                startpunkt.TargetScale,
                zielpunkt.TargetScale,
                startIterationen)

        zustand.Rotation = zielRotation
        zustand.ZielRotation = zielRotation

        zustand.Phase = MandelbrotKameraZustand.KameraPhaseTyp.FreezeOut

        zustand.PhasenFortschritt = 1.0
        zustand.KamerafahrtBeendet = True

    End Sub

#End Region

End Class