Imports TagLib
Imports System.Windows.Forms
Imports System.Drawing
Imports SlideShowTools
Imports SlideShowMain.SaverMain
Imports SlideShowLogging
Imports SlideShowMain.My.Resources

Public Class frmFallbackSaver
    Inherits Form

    '-- Variablen Deklarationen --
    Private bewegungsWinkel As Double = 45
    Private bewegungsgeschwindigkeit As Double = 4.0
    Private ReadOnly randPuffer As Integer = 5
    Private rnd As New Random()
    Private picSplashscreen As PictureBox

    Private Sub frmFallbacksaver_Load(sender As Object, e As EventArgs) Handles Me.Load
        FormsHandling.InitialFormPreparation(Me, Color.Black)
    End Sub

    Private Sub frmFallbacksaver_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        picSplashscreen = New PictureBox With {
            .Image = DirectCast(Splashscreen, Image),
            .SizeMode = PictureBoxSizeMode.AutoSize,
            .Location = New Point(
                                (Me.ClientSize.Width - Splashscreen.Width) \ 2,
                                (Me.ClientSize.Height - Splashscreen.Height) \ 2)
        }
        Me.Controls.Add(picSplashscreen)
        picSplashscreen.Visible = True

        tmrFallback.Interval = 30
        tmrFallback.Start()
    End Sub

    Private Sub tmrFallback_Tick(sender As Object, e As EventArgs) Handles tmrFallback.Tick
        Dim pos As Point = picSplashscreen.Location

        Dim dx As Double = Math.Cos(bewegungsWinkel * Math.PI / 180) * bewegungsgeschwindigkeit
        Dim dy As Double = Math.Sin(bewegungsWinkel * Math.PI / 180) * bewegungsgeschwindigkeit

        Dim neueX As Integer = CInt(pos.X + dx)
        Dim neueY As Integer = CInt(pos.Y + dy)

        Dim pralltAb As Boolean = False
        Dim clientWidth As Integer = Me.ClientSize.Width
        Dim clientHeight As Integer = Me.ClientSize.Height

        If neueX < 0 OrElse neueX + picSplashscreen.Width > clientWidth Then
            bewegungsWinkel = 180 - bewegungsWinkel
            pralltAb = True
        End If

        If neueY < 0 OrElse neueY + picSplashscreen.Height > clientHeight Then
            bewegungsWinkel = -bewegungsWinkel
            pralltAb = True
        End If

        If pralltAb Then
            bewegungsWinkel = KorrigiereWinkel(bewegungsWinkel)
        End If

        picSplashscreen.Location = New Point(
            Math.Max(0, Math.Min(clientWidth - picSplashscreen.Width, CInt(picSplashscreen.Left + dx))),
            Math.Max(0, Math.Min(clientHeight - picSplashscreen.Height, CInt(picSplashscreen.Top + dy)))
        )
    End Sub

    Private Function KorrigiereWinkel(winkel As Double) As Double
        Dim erlaubteBasiswinkel As Double() = {45, 135, 225, 315}
        Dim minAbweichung As Double = 0
        Dim maxAbweichung As Double = 20

        Dim nächsterBasiswinkel = erlaubteBasiswinkel.
            OrderBy(Function(bw) Math.Abs(NormalisiereWinkel(winkel - bw))).
            First()

        Dim abweichung As Double = rnd.NextDouble() * (maxAbweichung - minAbweichung) + minAbweichung
        If rnd.Next(2) = 0 Then abweichung *= -1

        Return NormalisiereWinkel(nächsterBasiswinkel + abweichung)
    End Function

    Private Function NormalisiereWinkel(w As Double) As Double
        While w < 0
            w += 360
        End While
        While w >= 360
            w -= 360
        End While
        Return w
    End Function

    Private Sub frmFallbackSaver_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        KeyAndMouseHandling.ForwardMouseDown(Me, e)
    End Sub

    Private Sub frmFallbackSaver_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        KeyAndMouseHandling.ForwardKeyDown(Me, e)
    End Sub

End Class
