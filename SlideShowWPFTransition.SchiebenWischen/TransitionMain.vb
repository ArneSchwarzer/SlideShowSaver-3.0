Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Imaging
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging.LogHandling
Imports SlideShowTools
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.WPFHandling

Namespace TransitionMain_SuW

    Public Class TransitionMain
        Implements ISlideShowTransition

#Region "Variablendeklaration, Structures & Enums ect."
        'Variablen, Enums und Structures

        'Settings und Konstanten
        Public Const SLIDESHOWTRANSITION_SuW_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Schieben & Wischen\"
        Public Const nameTransition As String = "Schieben und Wischen"
        Private aktuelleSettings As New SlideShowTransitionSettings_SuW

        'Timer und Zeitmanagement
        Private WithEvents tmrDuration As New Timer
        Private startTime As DateTime
        Private dauerInMS As Integer
        Private Const FPS As Integer = 60

        'Transitions-Bilder
        Private oldBmpGerahmt As Bitmap
        Private newBmpGerahmt As Bitmap
        Private oldPicBoxSizeMode As PictureBoxSizeMode
        Private newPicBoxSizeMode As PictureBoxSizeMode
        Private oldBmpSource As BitmapSource
        Private newBmpSource As BitmapSource

        'Animation, Positionen, Offsets etc. - Systems.Windows-Welt
        Private posOldWPF As System.Windows.Point
        Private posNewWPF As System.Windows.Point
        Private sizeWPF As System.Windows.Size

        'Zielausgabe
        Private renderTarget As Graphics
        Private targetRect As Rectangle
        Private cltSize As System.Drawing.Size

        'Sonstiges
        Private bmp As Bitmap

        Public Structure SlideShowTransitionSettings_SuW
            Public geschwindigkeit As Integer
            Public richtungen As List(Of String)
            Public modus As String
            Public FPS As Integer
        End Structure

#End Region

        'Eigenschaften
        ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName
            Get
                Return nameTransition
            End Get
        End Property

        ReadOnly Property TransitionKurzBeschreibung As String Implements ISlideShowTransition.TransitionKurzBeschreibung
            Get
                Return "Das neue Bild kommt von der Seite. Schiebt ggf. das alte dabei weg."
            End Get
        End Property

        ReadOnly Property TransitionVersion As Version Implements ISlideShowTransition.TransitionVersion
            Get
                Return New Version(1, 0, 0, 0)
            End Get
        End Property

        'Events
        Event TransitionIsRunning(state As Boolean) Implements ISlideShowTransition.TransitionIsRunning

        'Transition Ausführung
        Public Sub RunTransition(oldImage As System.Drawing.Image, picBoxModeOld As PictureBoxSizeMode,
                             newImage As System.Drawing.Image, picBoxModeNew As PictureBoxSizeMode,
                             targetGraphics As Graphics,
                             Optional clientSize As System.Drawing.Size = Nothing,
                             Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition

            RaiseEvent TransitionIsRunning(True)

            startTime = DateTime.Now
            dauerInMS = 1000 * aktuelleSettings.geschwindigkeit

            oldPicBoxSizeMode = picBoxModeOld
            newPicBoxSizeMode = picBoxModeNew
            renderTarget = targetGraphics
            cltSize = If(clientSize.IsEmpty, renderTarget.VisibleClipBounds.Size.ToSize(), clientSize)
            sizeWPF = New System.Windows.Size(cltSize.Width, cltSize.Height)
            targetRect = New Rectangle(0, 0, cltSize.Width, cltSize.Height)

            ' Bilder vorbereiten
            oldBmpGerahmt = ErzeugeGerahmtesBild(oldImage, oldPicBoxSizeMode, cltSize)
            newBmpGerahmt = ErzeugeGerahmtesBild(newImage, newPicBoxSizeMode, cltSize)
            oldBmpSource = ConvertBitmapToImageSource(oldBmpGerahmt)
            newBmpSource = ConvertBitmapToImageSource(newBmpGerahmt)

            ReadTransitionSettingsFromRegistryOrDefaults()
            StoreSettings(TransitionName, aktuelleSettings)

            ' Richtungsauswahl
            Dim richtung As String = If(aktuelleSettings.richtungen?.Count > 0,
                                    aktuelleSettings.richtungen(New Random().Next(aktuelleSettings.richtungen.Count)),
                                    "W")

            ' Koordinaten berechnen
            Dim dx As Integer = 0, dy As Integer = 0
            Select Case richtung
                Case "N" : dx = 0 : dy = -cltSize.Height
                Case "S" : dx = 0 : dy = cltSize.Height
                Case "W" : dx = -cltSize.Width : dy = 0
                Case "O" : dx = cltSize.Width : dy = 0
                Case "NW" : dx = -cltSize.Width : dy = -cltSize.Height
                Case "NO" : dx = cltSize.Width : dy = -cltSize.Height
                Case "SW" : dx = -cltSize.Width : dy = cltSize.Height
                Case "SO" : dx = cltSize.Width : dy = cltSize.Height
            End Select

            ' Modus ggf. zufällig wählen
            If aktuelleSettings.modus = "Zufällig" Then
                aktuelleSettings.modus = If(New Random().Next(2) = 0, "Schieben", "Wischen")
            End If

            posOldWPF = New System.Windows.Point(0, 0)
            posNewWPF = New System.Windows.Point(dx, dy)

            If aktuelleSettings.modus = "Wischen" Then
                posOldWPF = New System.Windows.Point(0, 0) ' statisch
            End If

            ' Notbremse?
            If durationMs > 0 Then
                If tmrDuration Is Nothing Then tmrDuration = New Timer()
                tmrDuration.Interval = durationMs
                AddHandler tmrDuration.Tick, Sub()
                                                 StopTransition()
                                             End Sub
                tmrDuration.Start()
            End If

            ' Animation starten (Delegat an WPFHandling)
            WPFHandling.StartVisualTransition(AddressOf DrawTransitionFrame, 1000 \ FPS)
        End Sub

        Sub StopTransition() Implements ISlideShowTransition.StopTransition
            'Aufräumen und Transition beenden.

            If tmrDuration IsNot Nothing Then
                tmrDuration.Stop()
                tmrDuration.Dispose()
                tmrDuration = Nothing
            End If

            RaiseEvent TransitionIsRunning(False)

        End Sub

        'Optionen/Dialoghandling
        Function GetTransitionOptionsDialog() As Windows.Forms.UserControl Implements ISlideShowTransition.GetTransitionOptionsDialog
            'Liefert den Options-Dialog der Transition

            'Liest die aktuelleSettings ein und speichert sie in SettingsInbox
            ReadTransitionSettingsFromRegistryOrDefaults()
            StoreSettings(nameTransition, aktuelleSettings)

            'Und liefert dann die ucOptionsTransition
            Return New ucOptionsTransition()

        End Function

        'Private Funktionen

        ' Settings und Defaultwerte
        Sub ReadTransitionSettingsFromRegistryOrDefaults()
            'Setzt aktuelleTransitonSettings mit den Werten aus der Registry oder mit Defaultwerten.

            Dim defaults As Dictionary(Of String, String) = GetTransitionDefaultSettings()

            aktuelleSettings.geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", defaults))
            aktuelleSettings.richtungen = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", defaults))
            aktuelleSettings.modus = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", defaults)
            aktuelleSettings.FPS = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "FPS", defaults))

        End Sub

        Public Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
            Dim defaults As New Dictionary(Of String, String)

            defaults.Add("Geschwindigkeit", "20")
            defaults.Add("Richtungen", "W; O")
            defaults.Add("Modus", "Wischen")
            defaults.Add("FPS", "60")

            Return defaults

        End Function

        'Bildwandlung und -manipulation
        Private Function ErzeugeGerahmtesBild(bild As System.Drawing.Image, sizeMode As PictureBoxSizeMode, zielgroesse As System.Drawing.Size) As Bitmap
            'Erstellt ein Bitmap mit dem Bild gemäß SizeMode mit schwarzem Rahmen in der Zielgröße

            bmp = New Bitmap(zielgroesse.Width, zielgroesse.Height)

            Using g As Graphics = Graphics.FromImage(bmp)
                g.Clear(System.Drawing.Color.Black)
                Dim drawRect As Rectangle = GraphicsSizeModeHandling.GetDrawRectangle(bild.Size, targetRect, sizeMode)
                g.DrawImage(bild, drawRect)
            End Using

            Return bmp
            bmp.Dispose()

        End Function

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

        'Abbruch- und Endverwaltung
        Sub endBildZeichnen()
            'Zeichnet "neuBmpGerahmt" auf das renderTarget

            Dim drawRect As Rectangle
            Dim targetRect As Rectangle

            'Grafik vorbereiten
            bmp = New Bitmap(cltSize.Width, cltSize.Height)
            targetRect = New Rectangle(0, 0, cltSize.Width, cltSize.Height)
            drawRect = GetDrawRectangle(newBmpGerahmt.Size, targetRect, newPicBoxSizeMode)

            Try
                renderTarget = Graphics.FromImage(bmp)
                renderTarget.InterpolationMode = InterpolationMode.HighQualityBicubic
                renderTarget.Clear(System.Drawing.Color.Black)

                ' Endbild auf den Zeichenbereich malen...
                renderTarget.DrawImage(newBmpGerahmt, drawRect)
            Catch ex As Exception
                LogError("Transition Schieben & Wischen - TransitionMain.endBildZeichnen(): Fehler beim Erstellen von renderTarget: " & ex.Message)
            End Try

        End Sub

        Sub tmrDuration_Tick() Handles tmrDuration.Tick
            'Bricht die Transition nach Ende von DurationMS ab.

            'Zum Schluss noch einmal die aufrufende targetGraphics aktualisieren
            endBildZeichnen()

            StopTransition()

        End Sub

        'Animation und Zeichnen
        Private Sub DrawTransitionFrame(dc As DrawingContext, size As System.Windows.Size)
            Dim elapsed As Double = (DateTime.Now - startTime).TotalMilliseconds
            Dim progress As Double = Math.Min(1.0, elapsed / dauerInMS)

            ' Position berechnen (linear interpoliert)
            Dim newX As Double = posNewWPF.X * (1.0 - progress)
            Dim newY As Double = posNewWPF.Y * (1.0 - progress)
            Dim oldX As Double = posOldWPF.X + (posNewWPF.X * progress)
            Dim oldY As Double = posOldWPF.Y + (posNewWPF.Y * progress)

            If aktuelleSettings.modus = "Wischen" Then
                oldX = 0
                oldY = 0
            End If

            ' Bilder zeichnen
            dc.DrawImage(oldBmpSource, New Rect(oldX, oldY, sizeWPF.Width, sizeWPF.Height))
            dc.DrawImage(newBmpSource, New Rect(newX, newY, sizeWPF.Width, sizeWPF.Height))

            ' Fertig?
            If progress >= 1.0 Then
                StopTransition()
            End If
        End Sub

    End Class

End Namespace
