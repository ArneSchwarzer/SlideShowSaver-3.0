Imports System.Globalization
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media

Public Class ucMandelbrotZielOverlay

    Private Const grosseSchriftgroesse As Double = 40.0
    Private Const kleineSchriftgroesse As Double = 16.0

    Private Const minimaleKartenbreite As Double = 360.0
    Private Const maximaleKartenbreite As Double = 720.0

    Private Const minimaleKartenhoehe As Double = 72.0
    Private Const maximaleKartenhoehe As Double = 110.0

    Private Const informationszeilenHoehe As Double = 42.0

    Private Const grosseEckenrundung As Double = 18.0
    Private Const kleineEckenrundung As Double = 0.0

    Private Const grosseHorizontalePolsterung As Double = 28.0
    Private Const grosseVertikalePolsterung As Double = 16.0
    Private Const kleineHorizontalePolsterung As Double = 12.0
    Private Const kleineVertikalePolsterung As Double = 4.0

    Private zielName As String

    Private zielCenterX As Double
    Private zielCenterY As Double
    Private zielSkala As Double
    Private zielIterationen As Integer

    Private aktuellesCenterX As Double
    Private aktuellesCenterY As Double
    Private aktuelleSkala As Double
    Private aktuelleIterationen As Integer

    Private aktuellerTransitionsFortschritt As Double

    Private rotationAktiv As Boolean
    Private zielRotation As Double

    Public Sub New()

        InitializeComponent()

        zielName = String.Empty
        aktuellerTransitionsFortschritt = 0.0

    End Sub

    Public Sub InitialisiereZiel(
    name As String,
    centerX As Double,
    centerY As Double,
    targetScale As Double,
    maxIterationen As Integer,
    rotationAnzeigen As Boolean,
    rotationsWinkel As Double,
    hintergrundfarbe As Color,
    textfarbe As Color)

        Dim hintergrundMitTransparenz As Color

        zielName = name

        zielCenterX = centerX
        zielCenterY = centerY
        zielSkala = targetScale
        zielIterationen = maxIterationen

        rotationAktiv = rotationAnzeigen
        zielRotation = rotationsWinkel

        aktuellesCenterX = centerX
        aktuellesCenterY = centerY
        aktuelleSkala = targetScale
        aktuelleIterationen = maxIterationen

        hintergrundMitTransparenz = Color.FromArgb(
        128,
        hintergrundfarbe.R,
        hintergrundfarbe.G,
        hintergrundfarbe.B)

        brdInformation.Background =
        New SolidColorBrush(hintergrundMitTransparenz)

        txbKoordinaten.Foreground =
        New SolidColorBrush(textfarbe)

        txbZielname.Foreground =
        New SolidColorBrush(textfarbe)

        txbRenderInformationen.Foreground =
        New SolidColorBrush(textfarbe)

        txbZielname.Text = zielName

        AktualisiereTexte()

    End Sub

    Public Sub AktualisiereWerte(
        centerX As Double,
        centerY As Double,
        skala As Double,
        maxIterationen As Integer)

        aktuellesCenterX = centerX
        aktuellesCenterY = centerY
        aktuelleSkala = skala
        aktuelleIterationen = maxIterationen

        AktualisiereTexte()

    End Sub

    Public Sub ZeigeFreezeIn()

        Visibility = Visibility.Visible
        brdInformation.Visibility = Visibility.Visible

        aktuellerTransitionsFortschritt = 0.0

        AktualisiereDarstellung(aktuellerTransitionsFortschritt)

    End Sub

    Public Sub AktualisiereTranslation(fortschritt As Double)

        fortschritt = Begrenze(fortschritt, 0.0, 1.0)

        aktuellerTransitionsFortschritt = fortschritt

        Visibility = Visibility.Visible
        brdInformation.Visibility = Visibility.Visible

        AktualisiereDarstellung(aktuellerTransitionsFortschritt)

    End Sub

    Public Sub ZeigeInformationszeile()

        aktuellerTransitionsFortschritt = 1.0

        Visibility = Visibility.Visible
        brdInformation.Visibility = Visibility.Visible

        AktualisiereDarstellung(aktuellerTransitionsFortschritt)

    End Sub

    Public Sub AktualisiereFreezeOut(fortschritt As Double)

        fortschritt = Begrenze(fortschritt, 0.0, 1.0)

        Visibility = Visibility.Visible
        brdInformation.Visibility = Visibility.Visible

        brdInformation.Opacity = 1.0 - fortschritt

    End Sub

    Public Sub Verberge()

        brdInformation.Visibility = Visibility.Collapsed
        brdInformation.Opacity = 1.0

        Visibility = Visibility.Collapsed

    End Sub

    Private Sub AktualisiereTexte()

        Dim koordinatenText As String
        Dim renderInformationenText As String

        koordinatenText =
        "X: " &
        aktuellesCenterX.ToString(
            "G17",
            CultureInfo.InvariantCulture) &
        " | Y: " &
        aktuellesCenterY.ToString(
            "G17",
            CultureInfo.InvariantCulture)

        If rotationAktiv Then

            koordinatenText &=
            " | Rotwinkel: " &
            FormatiereRotationsWinkel(zielRotation)

        End If

        renderInformationenText =
        "Skala: " &
        FormatiereSkala(aktuelleSkala) &
        " → " &
        FormatiereSkala(zielSkala) &
        " | Iterationen: " &
        aktuelleIterationen.ToString(
            "N0",
            CultureInfo.CurrentCulture) &
        " von " &
        zielIterationen.ToString(
            "N0",
            CultureInfo.CurrentCulture)

        txbKoordinaten.Text = koordinatenText
        txbZielname.Text = zielName
        txbRenderInformationen.Text = renderInformationenText

    End Sub

    Private Sub AktualisiereDarstellung(fortschritt As Double)

        Dim geglaetteterFortschritt As Double

        Dim verfuegbareBreite As Double
        Dim verfuegbareHoehe As Double

        Dim kartenBreite As Double
        Dim kartenHoehe As Double
        Dim kartenPositionOben As Double

        Dim aktuelleBreite As Double
        Dim aktuelleHoehe As Double
        Dim aktuellePositionOben As Double
        Dim aktuelleEckenrundung As Double
        Dim aktuelleSchriftgroesse As Double

        Dim horizontalePolsterung As Double
        Dim vertikalePolsterung As Double
        Dim informationsDeckkraft As Double

        fortschritt = Begrenze(fortschritt, 0.0, 1.0)

        geglaetteterFortschritt =
            SmoothStep(fortschritt)

        verfuegbareBreite = Math.Max(1.0, ActualWidth)
        verfuegbareHoehe = Math.Max(1.0, ActualHeight)

        kartenBreite = Begrenze(
            verfuegbareBreite * 0.45,
            minimaleKartenbreite,
            maximaleKartenbreite)

        kartenHoehe = Begrenze(
            verfuegbareHoehe * 0.1,
            minimaleKartenhoehe,
            maximaleKartenhoehe)

        kartenPositionOben =
            Math.Max(
                0.0,
                (verfuegbareHoehe - kartenHoehe) / 2.0)

        aktuelleBreite = Lerp(
            kartenBreite,
            verfuegbareBreite,
            geglaetteterFortschritt)

        aktuelleHoehe = Lerp(
            kartenHoehe,
            informationszeilenHoehe,
            geglaetteterFortschritt)

        aktuellePositionOben = Lerp(
            kartenPositionOben,
            0.0,
            geglaetteterFortschritt)

        aktuelleEckenrundung = Lerp(
            grosseEckenrundung,
            kleineEckenrundung,
            geglaetteterFortschritt)

        aktuelleSchriftgroesse = Lerp(
            grosseSchriftgroesse,
            kleineSchriftgroesse,
            geglaetteterFortschritt)

        horizontalePolsterung = Lerp(
            grosseHorizontalePolsterung,
            kleineHorizontalePolsterung,
            geglaetteterFortschritt)

        vertikalePolsterung = Lerp(
            grosseVertikalePolsterung,
            kleineVertikalePolsterung,
            geglaetteterFortschritt)

        informationsDeckkraft = Begrenze(
            (fortschritt - 0.5) / 0.5,
            0.0,
            1.0)

        brdInformation.Width = aktuelleBreite
        brdInformation.Height = aktuelleHoehe

        brdInformation.Margin =
            New Thickness(
                0.0,
                aktuellePositionOben,
                0.0,
                0.0)

        brdInformation.CornerRadius =
            New CornerRadius(aktuelleEckenrundung)

        brdInformation.Padding =
            New Thickness(
                horizontalePolsterung,
                vertikalePolsterung,
                horizontalePolsterung,
                vertikalePolsterung)

        brdInformation.Opacity = 1.0

        txbZielname.FontSize = aktuelleSchriftgroesse

        txbKoordinaten.FontSize = kleineSchriftgroesse
        txbRenderInformationen.FontSize = kleineSchriftgroesse

        txbKoordinaten.Opacity = informationsDeckkraft
        txbRenderInformationen.Opacity = informationsDeckkraft

    End Sub

    Private Function FormatiereSkala(skala As Double) As String

        Return skala.ToString(
            "0.0000E+0",
            CultureInfo.InvariantCulture)

    End Function

    Private Function SmoothStep(wert As Double) As Double

        wert = Begrenze(wert, 0.0, 1.0)

        Return wert * wert * (3.0 - 2.0 * wert)

    End Function

    Private Function Lerp(
        startwert As Double,
        endwert As Double,
        fortschritt As Double) As Double

        Return startwert +
               (endwert - startwert) *
               fortschritt

    End Function

    Private Function Begrenze(
        wert As Double,
        minimum As Double,
        maximum As Double) As Double

        Return Math.Max(
            minimum,
            Math.Min(maximum, wert))

    End Function

    Private Sub ucMandelbrotZielOverlay_SizeChanged(
        sender As Object,
        e As SizeChangedEventArgs) Handles Me.SizeChanged

        If brdInformation.Visibility <> Visibility.Visible Then
            Return
        End If

        AktualisiereDarstellung(
            aktuellerTransitionsFortschritt)

    End Sub

    Private Function FormatiereRotationsWinkel(rotation As Double) As String

        Dim winkelGrad As Double

        winkelGrad =
            rotation * 180.0 / Math.PI

        Return Math.Round(winkelGrad).ToString(
            "0",
            CultureInfo.CurrentCulture) & "°"

    End Function

End Class