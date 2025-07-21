Imports System.Windows.Media.Animation
Imports System.Windows.Media.Imaging
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports System.Windows.Interop
Imports SlideShowTools.SettingsHandling
Imports SlideShowWPFTransition.SchiebenWischen.TransitionMain_SuW.TransitionMain
Imports System.Windows


Namespace SlideShowWPFTransition.SchiebenWischen

    Partial Public Class WpfTransitionWindow
        Inherits System.Windows.Window

        'Variablendeklaration
        Private aktuelleSettings As SlideShowTransitionSettings_SuW

        'Events
        Public Event TransitionIstFertig()

        Public Sub New()

            InitializeComponent()

        End Sub

        Public Sub StartTransition(oldBmp As Bitmap, newBmp As Bitmap)
            'Animiert die beiden Bilder 

            Dim zielOld As Drawing.Point
            Dim startNew As Drawing.Point
            Dim richtung As String
            Dim rnd As New Random
            Dim würfel1D2 As Integer
            Dim animNeuX As New DoubleAnimation()
            Dim animNeuY As New DoubleAnimation()
            Dim animOldX As New DoubleAnimation()
            Dim animOldY As New DoubleAnimation()
            Dim alreadyClosed As Boolean = False

            ' Bilder einsetzen
            imgOld.Source = ConvertBitmapToImageSource(oldBmp)
            imgNew.Source = ConvertBitmapToImageSource(newBmp)

#Region "Richtungen festlegen"
            'Richtung für die Transition aussuchen und Bewegungsinfos setzen (und dabei ein paar if-then sparen... ;-)
            If aktuelleSettings.richtungen.Count = 0 Then
                aktuelleSettings.richtungen.Add("NW")
                aktuelleSettings.richtungen.Add("N")
                aktuelleSettings.richtungen.Add("NO")
                aktuelleSettings.richtungen.Add("O")
                aktuelleSettings.richtungen.Add("SO")
                aktuelleSettings.richtungen.Add("S")
                aktuelleSettings.richtungen.Add("SW")
                aktuelleSettings.richtungen.Add("W")
            End If

            richtung = aktuelleSettings.richtungen(Rnd.Next(aktuelleSettings.richtungen.Count))

            Select Case richtung
                Case "NW"
                    startNew.X = -Me.Width
                    startNew.Y = -Me.Height
                    zielOld.X = Me.Width
                    zielOld.Y = Me.Height
                Case "N"
                    startNew.X = 0
                    startNew.Y = -Me.Height
                    zielOld.X = 0
                    zielOld.Y = Me.Height
                Case "NO"
                    startNew.X = Me.Width
                    startNew.Y = -Me.Height
                    zielOld.X = -Me.Width
                    zielOld.Y = Me.Height
                Case "O"
                    startNew.X = Me.Width
                    startNew.Y = 0
                    zielOld.X = -Me.Width
                    zielOld.Y = 0
                Case "SO"
                    startNew.X = Me.Width
                    startNew.Y = Me.Height
                    zielOld.X = -Me.Width
                    zielOld.Y = -Me.Height
                Case "S"
                    startNew.X = 0
                    startNew.Y = Me.Height
                    zielOld.X = 0
                    zielOld.Y = -Me.Height
                Case "SW"
                    startNew.X = -Me.Width
                    startNew.Y = Me.Height
                    zielOld.X = Me.Width
                    zielOld.Y = -Me.Height
                Case "W"
                    startNew.X = -Me.Width
                    startNew.Y = 0
                    zielOld.X = Me.Width
                    zielOld.Y = 0
            End Select

            'Falls Modus = Zufällig, Modus erwürfeln
            If aktuelleSettings.modus = "Zufällig" Then
                würfel1D2 = Rnd.Next(2)
                If würfel1D2 = 0 Then
                    aktuelleSettings.modus = "Schieben"
                Else
                    aktuelleSettings.modus = "Wischen"
                End If
            End If

            'Falls Modus = Wischen, die Zielposition für das alte Bild wieder auf (0,0) setzen
            If aktuelleSettings.modus = "Wischen" Then
                zielOld.X = 0
                zielOld.Y = 0
            End If
#End Region

#Region "Animationen festlegen"
            ' Animationen festlegen
            animNeuX.From = startNew.X
            animNeuX.To = 0
            animNeuX.Duration = TimeSpan.FromSeconds(aktuelleSettings.geschwindigkeit)
            AddHandler animNeuX.Completed, Sub()
                                               If Not alreadyClosed Then
                                                   alreadyClosed = True
                                                   RaiseEvent TransitionIstFertig()
                                                   Me.Close()
                                               End If
                                           End Sub

            animNeuY.From = startNew.Y
            animNeuY.To = 0
            animNeuY.Duration = TimeSpan.FromSeconds(aktuelleSettings.geschwindigkeit)
            AddHandler animNeuY.Completed, Sub()
                                               If Not alreadyClosed Then
                                                   alreadyClosed = True
                                                   RaiseEvent TransitionIstFertig()
                                                   Me.Close()
                                               End If
                                           End Sub

            animOldX.From = 0
            animOldX.To = zielOld.X
            animOldX.Duration = TimeSpan.FromSeconds(aktuelleSettings.geschwindigkeit)
            AddHandler animOldX.Completed, Sub()
                                               If Not alreadyClosed Then
                                                   alreadyClosed = True
                                                   RaiseEvent TransitionIstFertig()
                                                   Me.Close()
                                               End If
                                           End Sub

            animOldY.From = 0
            animOldY.To = zielOld.Y
            animOldY.Duration = TimeSpan.FromSeconds(aktuelleSettings.geschwindigkeit)
            AddHandler animOldY.Completed, Sub()
                                               If Not alreadyClosed Then
                                                   alreadyClosed = True
                                                   RaiseEvent TransitionIstFertig()
                                                   Me.Close()
                                               End If
                                           End Sub
#End Region


            'Animationen beginnen
            imgNew.BeginAnimation(Canvas.LeftProperty, animNeuX)
            imgNew.BeginAnimation(Canvas.TopProperty, animNeuY)
            imgOld.BeginAnimation(Canvas.LeftProperty, animOldX)
            imgOld.BeginAnimation(Canvas.TopProperty, animOldY)

        End Sub

        Private Function ConvertBitmapToImageSource(bmp As Bitmap) As BitmapSource
            Using memory = New MemoryStream()
                bmp.Save(memory, ImageFormat.Png)
                memory.Position = 0
                Dim bitmapImage As New BitmapImage()
                bitmapImage.BeginInit()
                bitmapImage.StreamSource = memory
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad
                bitmapImage.EndInit()
                Return bitmapImage
            End Using
        End Function

        Private Sub CheckYourMail()
            'Liest die aktuellenSettings aus der SettingsInbox

            aktuelleSettings = GetSettings(Of SlideShowTransitionSettings_SuW)(nameTransition)

        End Sub

        Private Sub WpfTransitionWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            'Form und ihre Elemente initialisieren

            ' Debug-/Overlay-Effekte deaktivieren (falls aktiv)
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = TraceLevel.Off
            System.Diagnostics.PresentationTraceSources.DependencyPropertySource.Switch.Level = TraceLevel.Off

            Me.Topmost = False
            Me.Activate()

            'Aktuelle Settings abholen
            CheckYourMail()

        End Sub
    End Class

End Namespace
