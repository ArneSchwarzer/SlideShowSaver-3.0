Imports SlideShowLogging

Public Class MandelbrotZielAuswahl

#Region "Variablendeklaration"
    'Variablendeklaration

    Private ReadOnly zufall As Random

#End Region

#Region "Konstruktor"

    Public Sub New(randomGenerator As Random)

        If randomGenerator Is Nothing Then

            Throw New ArgumentNullException(NameOf(randomGenerator))

        End If

        zufall = randomGenerator

    End Sub

#End Region

#Region "Zielauswahl"

    Public Function WaehleZiel(ziele As IEnumerable(Of MandelbrotZiel), startSkala As Double) As MandelbrotZiel
        'Wählt ein gültiges Ziel und erstellt daraus eine
        'für die aktuelle Kamerafahrt unabhängige Kopie.

        Dim gueltigeZiele As List(Of MandelbrotZiel)
        Dim ausgewaehltesZiel As MandelbrotZiel

        If ziele Is Nothing Then
            Return Nothing
        End If

        gueltigeZiele =
            ziele.Where(
                Function(ziel)

                    Return ziel IsNot Nothing AndAlso
                           ziel.TargetScale > 0.0 AndAlso
                           ziel.TargetScale < startSkala

                End Function).
            ToList()

        If gueltigeZiele.Count = 0 Then

            LogHandling.LogWarn("Modul Mandelbrot - MandelbrotZielAuswahl.WaehleZiel(): " &
                                "Es steht kein für die aktuelle Startskala geeignetes Ziel zur Verfügung.")

            Return Nothing

        End If

        ausgewaehltesZiel = gueltigeZiele(zufall.Next(gueltigeZiele.Count)).ErzeugeKopie()

        SpiegeleZielGelegentlich(ausgewaehltesZiel)
        ErgaenzeHimmelsrichtung(ausgewaehltesZiel)

        Return ausgewaehltesZiel

    End Function

    Private Sub SpiegeleZielGelegentlich(ziel As MandelbrotZiel)
        'Spiegelt das Ziel mit einer Wahrscheinlichkeit von 50 Prozent
        'an der horizontalen Symmetrieachse des Mandelbrot-Sets.

        If ziel Is Nothing Then
            Exit Sub
        End If

        If zufall.Next(2) = 0 Then

            ziel.CenterY *= -1.0

        End If

    End Sub

    Private Sub ErgaenzeHimmelsrichtung(ziel As MandelbrotZiel)
        'Ergänzt die aus der Y-Position abgeleitete Himmelsrichtung.

        If ziel Is Nothing Then
            Exit Sub
        End If

        If ziel.CenterY < 0.0 Then

            ziel.Name &= " (Nord)"

        ElseIf ziel.CenterY > 0.0 Then

            ziel.Name &= " (Süd)"

        End If

    End Sub

#End Region

End Class