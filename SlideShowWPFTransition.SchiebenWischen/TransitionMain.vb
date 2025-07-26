Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging.LogHandling
Imports SlideShowTools
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling


Namespace TransitionMain_SuW

    Public Class TransitionMain
        Implements ISlideShowTransition

#Region "Variablendeklaration, Structures & Enums ect."
        'Variablen, Enums und Structures

        'Settings und Konstanten
        Public Const SLIDESHOWTRANSITION_SuW_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Schieben & Wischen\"
        Public Const nameTransition As String = "Schieben und Wischen"
        Private aktuelleSettings As New SlideShowTransitionSettings_SuW

        'Fallback auf WPFTransitionWindow
        'Public Shared win As WpfTransitionWindow

        'Timer und Zeitmanagement
        Private WithEvents tmrDuration As New Timer
        Private startTime As DateTime
        Private dauerInMS As Integer
        Private Const FPS As Integer = 120

        'Transitions-Bilder
        Private oldBmpGerahmt As RenderTargetBitmap
        Private newBmpGerahmt As RenderTargetBitmap
        Private oldPicBoxSizeMode As PictureBoxSizeMode
        Private newPicBoxSizeMode As PictureBoxSizeMode
        Private oldBmpSource As BitmapSource
        Private newBmpSource As BitmapSource

        'Animation, Positionen, Offsets etc. - Systems.Windows-Welt
        Private startPosOldWPF As System.Windows.Point
        Private zielPosOldWPF As System.Windows.Point
        Private startPosNewWPF As System.Windows.Point
        Private zielPosNewWPF As System.Windows.Point
        Private sizeWPF As System.Windows.Size

        'Zielausgabe
        Private targetRect As Rectangle
        Private sizeWinForms As System.Drawing.Size

        'Rendering
        Private Shared drawAction As Action(Of DrawingContext, Windows.Size)
        Private Shared frameTimer As DispatcherTimer
        Private Shared renderSize As Windows.Size

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
        Event TransitionFrameIstFertig(bitmap As RenderTargetBitmap) Implements ISlideShowTransition.TransitionFrameIstFertig

        'Transition Ausführung
        Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode,
                             newImage As BitmapImage, picBoxModeNew As PictureBoxSizeMode,
                             clientSize As System.Drawing.Size,
                             Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition

            Dim dx As Integer = 0, dy As Integer = 0
            Dim richtungArray() As String = {"N", "NO", "O", "SO", "S", "SW", "W", "NW"}
            Dim richtung As String

            RaiseEvent TransitionIsRunning(True)

            If oldImage Is Nothing OrElse newImage Is Nothing Then
                StopTransition()
            End If

            'Überführen der Parameter in Klassenvariablen
            oldPicBoxSizeMode = picBoxModeOld
            newPicBoxSizeMode = picBoxModeNew
            sizeWinForms = clientSize
            sizeWPF = New System.Windows.Size(sizeWinForms.Width, sizeWinForms.Height)
            targetRect = New Rectangle(0, 0, sizeWinForms.Width, sizeWinForms.Height)

            ' Bilder vorbereiten
            oldBmpGerahmt = ErzeugeGerahmtesBild(oldImage, oldPicBoxSizeMode, sizeWinForms)
            newBmpGerahmt = ErzeugeGerahmtesBild(newImage, newPicBoxSizeMode, sizeWinForms)
            oldBmpSource = CType(oldBmpGerahmt, ImageSource)
            newBmpSource = CType(newBmpGerahmt, ImageSource)

            ReadTransitionSettingsFromRegistryOrDefaults()
            StoreSettings(TransitionName, aktuelleSettings)

            'Fallback auf WPFTransitionWindow
            'win = New WpfTransitionWindow()
            'AddHandler win.TransitionIstFertig, Sub()
            '                                        StopTransition()
            '                                    End Sub

            'win.WindowState = WindowState.Maximized
            'win.Show()
            'win.StartTransition(oldBmpGerahmt, newBmpGerahmt)

            ' Richtungsauswahl
            If aktuelleSettings.richtungen?.Count > 0 Then
                richtung = aktuelleSettings.richtungen(New Random().Next(aktuelleSettings.richtungen.Count))
            Else
                richtung = richtungArray(New Random().Next(8))
            End If

            ' Koordinaten berechnen
            Select Case richtung
                Case "N" : dx = 0 : dy = -sizeWinForms.Height
                Case "S" : dx = 0 : dy = sizeWinForms.Height
                Case "W" : dx = -sizeWinForms.Width : dy = 0
                Case "O" : dx = sizeWinForms.Width : dy = 0
                Case "NW" : dx = -sizeWinForms.Width : dy = -sizeWinForms.Height
                Case "NO" : dx = sizeWinForms.Width : dy = -sizeWinForms.Height
                Case "SW" : dx = -sizeWinForms.Width : dy = sizeWinForms.Height
                Case "SO" : dx = sizeWinForms.Width : dy = sizeWinForms.Height
            End Select

            ' Modus ggf. zufällig wählen
            If aktuelleSettings.modus = "Zufällig" Then
                aktuelleSettings.modus = If(New Random().Next(2) = 0, "Schieben", "Wischen")
            End If

            startPosOldWPF = New Windows.Point(0, 0)
            zielPosOldWPF = New Windows.Point(-dx, -dy)
            startPosNewWPF = New Windows.Point(dx, dy)
            zielPosNewWPF = New Windows.Point(0, 0)

            If aktuelleSettings.modus = "Wischen" Then
                zielPosOldWPF = New System.Windows.Point(0, 0) ' statisch
            End If

            'Falls vom Modul gewünscht, Notbremse setzen.
            If durationMs > 0 Then
                If tmrDuration Is Nothing Then tmrDuration = New Timer()
                tmrDuration.Interval = durationMs
                AddHandler tmrDuration.Tick, Sub()
                                                 StopTransition()
                                             End Sub
                tmrDuration.Start()
            End If

            ' Animation starten 
            dauerInMS = 1000 * aktuelleSettings.geschwindigkeit
            startTime = DateTime.Now

            StartRenderLoop(AddressOf DrawTransitionFrame, New Windows.Size(sizeWPF.Width, sizeWPF.Height))

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
        Public Sub EndBildZeichnen()
            'Gibt das Endbild aus

            RaiseEvent TransitionFrameIstFertig(newBmpGerahmt)

        End Sub

        Sub tmrDuration_Tick() Handles tmrDuration.Tick
            'Bricht die Transition nach Ende von DurationMS ab.
            LogDebug("Transition SuW - TransitionMain.tmrDuration_Tick() wurde aufgerufen.")

            'Zum Schluss noch einmal die aufrufende targetGraphics aktualisieren
            EndBildZeichnen()

            StopTransition()

        End Sub

        'Animation und Zeichnen
        Private Sub DrawTransitionFrame(dc As DrawingContext, size As System.Windows.Size)
            Dim elapsed As Double = (DateTime.Now - startTime).TotalMilliseconds
            Dim progress As Double = Math.Min(1.0, elapsed / dauerInMS)

            ' Position berechnen (linear interpoliert)
            Dim oldX As Double = startPosOldWPF.X + (zielPosOldWPF.X - startPosOldWPF.X) * progress
            Dim oldY As Double = startPosOldWPF.Y + (zielPosOldWPF.Y - startPosOldWPF.Y) * progress
            Dim newX As Double = startPosNewWPF.X + (zielPosNewWPF.X - startPosNewWPF.X) * progress
            Dim newY As Double = startPosNewWPF.Y + (zielPosNewWPF.Y - startPosNewWPF.Y) * progress

            If aktuelleSettings.modus = "Wischen" Then
                oldX = 0
                oldY = 0
            End If

            ' Bilder zeichnen
            dc.DrawImage(oldBmpSource, New Rect(oldX, oldY, sizeWPF.Width, sizeWPF.Height))
            dc.DrawImage(newBmpSource, New Rect(newX, newY, sizeWPF.Width, sizeWPF.Height))

            ' Fertig?
            If progress >= 1.0 Then
                StopRenderLoop()
                StopTransition()
            End If

        End Sub

        Public Sub StartRenderLoop(drawActionInput As Action(Of DrawingContext, Windows.Size),
                                          zielGroesse As Windows.Size)

            drawAction = drawActionInput
            renderSize = zielGroesse

            StopRenderLoop()

            frameTimer = New DispatcherTimer()
            AddHandler frameTimer.Tick, AddressOf OnFrameTick
            frameTimer.Interval = TimeSpan.FromMilliseconds(1000 \ FPS)
            frameTimer.Start()

        End Sub

        Public Sub StopRenderLoop()
            If frameTimer IsNot Nothing Then
                frameTimer.Stop()
                RemoveHandler frameTimer.Tick, AddressOf OnFrameTick
                frameTimer = Nothing
            End If
        End Sub

        Private Sub OnFrameTick(sender As Object, e As EventArgs)
            If drawAction Is Nothing Then Exit Sub

            ' Neuen Frame zeichnen
            Dim drawingVisual As New DrawingVisual()
            Using dc As DrawingContext = drawingVisual.RenderOpen()
                dc.DrawRectangle(Media.Brushes.Black, Nothing, New Rect(0, 0, renderSize.Width, renderSize.Height))
                drawAction.Invoke(dc, renderSize)
            End Using

            ' Rendern in Bitmap
            Dim rtb As New RenderTargetBitmap(CInt(renderSize.Width),
                                              CInt(renderSize.Height),
                                              96, 96, PixelFormats.Pbgra32)
            rtb.Render(drawingVisual)

            ' Umwandeln in System.Drawing.Bitmap
            'Dim bitmap As Bitmap = ConvertRenderTargetBitmapToBitmap(rtb)

            RaiseEvent TransitionFrameIstFertig(rtb)

        End Sub

        Private Shared Function ConvertRenderTargetBitmapToBitmap(rtb As RenderTargetBitmap) As Bitmap
            Dim width As Integer = rtb.PixelWidth
            Dim height As Integer = rtb.PixelHeight
            Dim stride As Integer = width * 4

            Dim pixelData(stride * height - 1) As Byte
            rtb.CopyPixels(pixelData, stride, 0)

            Dim bmp As New Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
            Dim bmpData As BitmapData = bmp.LockBits(New Rectangle(0, 0, width, height),
                                                     ImageLockMode.WriteOnly,
                                                     System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
            Marshal.Copy(pixelData, 0, bmpData.Scan0, pixelData.Length)
            bmp.UnlockBits(bmpData)

            Return bmp
        End Function

        Public Shared Function ConvertImageToBitmapImage(img As System.Drawing.Image) As BitmapImage
            Using ms As New MemoryStream()
                img.Save(ms, ImageFormat.Png)
                ms.Seek(0, SeekOrigin.Begin)

                Dim bmpImage As New BitmapImage()
                bmpImage.BeginInit()
                bmpImage.CacheOption = BitmapCacheOption.OnLoad
                bmpImage.StreamSource = ms
                bmpImage.EndInit()
                bmpImage.Freeze() ' wichtig für Cross-Thread-Access

                Return bmpImage
            End Using
        End Function

    End Class

End Namespace
