Imports System.Drawing
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging.LogHandling
Imports SlideShowTools
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling


Namespace TransitionMain_GradientWischen

    Public Class TransitionMain
        Implements ISlideShowTransition

#Region "Variablendeklaration, Structures & Enums ect."
        'Variablen, Enums und Structures

        'Settings und Konstanten
        Public Const SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Gradient-Wischen\"
        Public Const nameTransition As String = "Gradient-Wischen"
        Private aktuelleSettings As New SlideShowTransitionSettings_GradientWischen

        'Timer und Zeitmanagement
        Private WithEvents tmrDuration As New Timer
        Private startTime As DateTime
        Private dauerInMS As Integer
        Private Const FPS As Integer = 120

        'Transitions-Bilder
        Private oldBmpGerahmt As RenderTargetBitmap
        Private newBmpGerahmt As RenderTargetBitmap
        Private oldBmp As Bitmap
        Private newBmp As Bitmap

        'Animation, Positionen, Offsets etc. - Systems.Windows-Welt
        Private richtung As String

        'Zielausgabe
        Private targetRect As Rectangle
        Private sizeWinForms As System.Drawing.Size

        'Rendering
        Private drawAction As Action(Of DrawingContext, Windows.Size)
        Private frameTimer As DispatcherTimer
        Private renderSize As Windows.Size
        Private rtbCache As RenderTargetBitmap
        Private stopRequested As Boolean = False

        'Sonstiges
        Private bmp As Bitmap

        Public Structure SlideShowTransitionSettings_GradientWischen
            Public geschwindigkeit As Integer
            Public richtungen As List(Of String)
            Public breite As Integer
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
                Return "Das neue Bild kommt mit einem weichen Übergang von der Seite oder in das/aus dem Zentrum."
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
            Dim richtungArray() As String = {"N", "NO", "O", "SO", "S", "SW", "W", "NW", "ZOut", "ZIn"}

            RaiseEvent TransitionIsRunning(True)

            If oldImage Is Nothing OrElse newImage Is Nothing Then
                StopTransition()
            End If

            ReadTransitionSettingsFromRegistryOrDefaults()
            StoreSettings(TransitionName, aktuelleSettings)

            'Überführen der Parameter in Klassenvariablen
            oldBmpGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
            newBmpGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)
            oldBmp = ConvertRenderTargetBitmapToBitmap(oldBmpGerahmt)
            newBmp = ConvertRenderTargetBitmapToBitmap(newBmpGerahmt)

            'Richtungsauswahl
            If aktuelleSettings.richtungen?.Count > 0 Then
                richtung = aktuelleSettings.richtungen(New Random().Next(aktuelleSettings.richtungen.Count))
            Else
                richtung = richtungArray(New Random().Next(10))
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

            StartRenderLoop(AddressOf DrawTransitionFrame, New Windows.Size(clientSize.Width, clientSize.Height))

        End Sub

        Sub StopTransition() Implements ISlideShowTransition.StopTransition
            'Aufräumen und Transition beenden.

            If tmrDuration IsNot Nothing Then
                tmrDuration.Stop()
                tmrDuration.Dispose()
                tmrDuration = Nothing
            End If

            stopRequested = True

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

            aktuelleSettings.geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Geschwindigkeit", defaults))
            aktuelleSettings.richtungen = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Richtungen", defaults))
            aktuelleSettings.breite = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Breite", defaults))

        End Sub

        Public Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
            Dim defaults As New Dictionary(Of String, String)

            defaults.Add("Geschwindigkeit", "20")
            defaults.Add("Richtungen", "W; O; ZIn; ZOut")
            defaults.Add("Breite", "35")

            Return defaults

        End Function

        'Abbruch- und Endverwaltung
        Public Sub EndBildZeichnen()
            'Gibt das Endbild aus

            RaiseEvent TransitionFrameIstFertig(newBmpGerahmt)

        End Sub

        Sub tmrDuration_Tick() Handles tmrDuration.Tick
            'Bricht die Transition nach Ende von DurationMS ab.
            LogDebug("Transition Gradient-Wischen - TransitionMain.tmrDuration_Tick() wurde aufgerufen.")

            'Zum Schluss noch einmal die aufrufende targetGraphics aktualisieren
            EndBildZeichnen()

            StopTransition()

        End Sub

        'Animation
        Private Sub DrawTransitionFrame(dc As DrawingContext, size As System.Windows.Size)
            Dim elapsed As Double = (DateTime.Now - startTime).TotalMilliseconds
            Dim progress As Double = Math.Min(1.0, elapsed / dauerInMS)
            Dim mask As Bitmap
            Dim frame As Bitmap
            Dim imgSource As ImageSource

            'Maske generieren
            Select Case richtung
                Case "ZIn"
                    mask = GenerateRadialMask(size.Width, size.Height, progress, aktuelleSettings.breite, True)
                Case "ZOut"
                    mask = GenerateRadialMask(size.Width, size.Height, progress, aktuelleSettings.breite, False)
                Case Else
                    mask = GenerateLinearMask(size.Width, size.Height, progress, aktuelleSettings.breite)
            End Select

            'Bilder per Maske mischen
            frame = BlendWithMask(oldBmp, newBmp, mask)

            'In ImageSource umwandeln
            imgSource = ConvertBitmapToImageSource(frame)

            ' Bild zeichnen
            dc.DrawImage(imgSource, New Rect(0, 0, size.Width, size.Height))

            ' Fertig?
            If progress >= 1.0 Then
                StopTransition()
            End If

        End Sub

        'Generierung der Masken

        'Lineare Maske
        Private Function GenerateLinearMask(width As Integer, height As Integer, progress As Double, breite As Integer) As Bitmap

            Dim bmp As New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)
            Dim rect As New Rectangle(0, 0, width, height)
            Dim data As Imaging.BitmapData
            Dim stride As Integer
            Dim bpp As Integer
            Dim buffer() As Byte
            Dim weich As Integer
            Dim startX As Integer
            Dim startY As Integer
            Dim useDirs As List(Of String)

            ' Variablen am Anfang (deine Regel)
            data = bmp.LockBits(rect, Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat)
            stride = data.Stride
            bpp = 4
            ReDim buffer(stride * height - 1)

            Dim breiteFaktor As Double = Math.Max(0.0, Math.Min(1.0, breite / 100.0))
            Dim basis As Integer
            Select Case richtung
                Case "W", "O"
                    basis = width
                Case "N", "S"
                    basis = height
                Case "NW", "NO", "SW", "SO"
                    basis = (width + height) ' zur T-Projektion passend
                Case Else
                    basis = width
            End Select

            weich = Math.Max(1, CInt(basis * breiteFaktor))

            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1
                    Dim alphaMax As Byte = 0
                    Dim a As Byte = 0

                    Select Case richtung
                        Case "W"
                            startX = CInt(progress * width)
                            a = ComputeRamp(x, startX, weich)
                        Case "O"
                            startX = CInt((1.0 - progress) * width)
                            a = ComputeRampInv(x, startX, weich)
                        Case "N"
                            startY = CInt(progress * height)
                            a = ComputeRamp(y, startY, weich)
                        Case "S"
                            startY = CInt((1.0 - progress) * height)
                            a = ComputeRampInv(y, startY, weich)
                        Case "NW" ' oben-links → unten-rechts
                            ' projiziere (x,y) auf Diagonale
                            Dim t As Integer = x + y
                            Dim startT As Integer = CInt(progress * (width + height))
                            a = ComputeRamp(t, startT, weich)
                        Case "NO" ' oben-rechts → unten-links
                            Dim t As Integer = (width - 1 - x) + y
                            Dim startT As Integer = CInt(progress * (width + height))
                            a = ComputeRamp(t, startT, weich)
                        Case "SW" ' unten-links → oben-rechts
                            ' projiziere (x,y) auf Diagonale, aber y invertieren
                            Dim t As Integer = x + (height - 1 - y)
                            Dim startT As Integer = CInt(progress * (width + height))
                            a = ComputeRamp(t, startT, weich)
                        Case "SO" ' unten-rechts → oben-links
                            ' projiziere (x,y) auf Diagonale, aber beide Achsen invertieren
                            Dim t As Integer = (width - 1 - x) + (height - 1 - y)
                            Dim startT As Integer = CInt(progress * (width + height))
                            a = ComputeRamp(t, startT, weich)
                        Case Else
                            ' Fallback auf "von links nach rechts (W)"
                            startX = CInt(progress * width)
                            a = ComputeRamp(x, startX, weich)
                    End Select

                    If a > alphaMax Then alphaMax = a

                    Dim ofs As Integer = y * stride + x * bpp

                    'Zum Maske visualisieren:
                    'buffer(ofs + 0) = alphaMax ' B
                    'buffer(ofs + 1) = alphaMax ' G
                    'buffer(ofs + 2) = alphaMax ' R
                    'buffer(ofs + 3) = 255      ' A (voll deckend für die Vorschau)

                    'Echte Darstellung fürs Blenden
                    buffer(ofs + 0) = 255  ' B
                    buffer(ofs + 1) = 255  ' G
                    buffer(ofs + 2) = 255  ' R
                    buffer(ofs + 3) = alphaMax ' A = Sichtbarkeit des NEUEN Bildes
                Next
            Next

            Runtime.InteropServices.Marshal.Copy(buffer, 0, data.Scan0, buffer.Length)
            bmp.UnlockBits(data)
            Return bmp
        End Function

        'Berechnung des Gradienten: 0..255 Rampen (weichBreite = Anzahl Pixel für Übergang)
        Private Function ComputeRamp(pos As Integer, startPos As Integer, weichBreite As Integer) As Byte
            If pos < startPos Then Return 0
            If pos >= startPos + weichBreite Then Return 255
            Dim t As Double = (pos - startPos) / Math.Max(1, weichBreite)
            Return CByte(t * 255)
        End Function

        'Berechnung des inversen Gradienten (weiß vor der Kante, schwarz dahinter)
        Private Function ComputeRampInv(pos As Integer, startPos As Integer, weichBreite As Integer) As Byte
            If pos > startPos Then Return 0
            If pos <= startPos - weichBreite Then Return 255
            Dim t As Double = (startPos - pos) / Math.Max(1, weichBreite)
            Return CByte(t * 255)
        End Function

        'Radiale Maske
        Private Function GenerateRadialMask(width As Integer, height As Integer, progress As Double, breite As Double, inward As Boolean) As Bitmap
            Dim bmp As New Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)
            Dim rect As New Rectangle(0, 0, width, height)
            Dim data As Imaging.BitmapData
            Dim stride As Integer
            Dim bpp As Integer
            Dim buffer() As Byte
            Dim weich As Integer
            Dim cx As Double
            Dim cy As Double
            Dim maxR As Double
            Dim startR As Double

            data = bmp.LockBits(rect, Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat)
            stride = data.Stride
            bpp = 4
            ReDim buffer(stride * height - 1)

            cx = (width - 1) / 2.0
            cy = (height - 1) / 2.0
            maxR = Math.Sqrt(cx * cx + cy * cy)
            weich = Math.Max(1, CInt((If(breite < 0, 0, If(breite > 1, 1, breite))) * maxR))

            ' progress bestimmt den Radius der „Kante“
            startR = progress * maxR

            For y As Integer = 0 To height - 1
                For x As Integer = 0 To width - 1
                    Dim dx As Double = x - cx
                    Dim dy As Double = y - cy
                    Dim r As Double = Math.Sqrt(dx * dx + dy * dy)

                    Dim a As Byte
                    If inward Then
                        ' nach innen: weißer Bereich kollabiert zum Zentrum
                        ' innen (klein r) zuerst weiß → nach und nach verschwindet außen
                        a = ComputeRadial(r, startR, weich, True)
                    Else
                        ' nach außen: Weiß-Ring wächst
                        a = ComputeRadial(r, startR, weich, False)
                    End If

                    Dim ofs As Integer = y * stride + x * bpp
                    buffer(ofs + 0) = 255
                    buffer(ofs + 1) = 255
                    buffer(ofs + 2) = 255
                    buffer(ofs + 3) = a
                Next
            Next

            Runtime.InteropServices.Marshal.Copy(buffer, 0, data.Scan0, buffer.Length)
            bmp.UnlockBits(data)
            Return bmp
        End Function

        'Radialen Gradenten berechnen: outward=False (Ring wächst), inward=True (Ring schrumpft nach innen)
        Private Function ComputeRadial(r As Double, startR As Double, weich As Integer, inward As Boolean) As Byte
            If Not inward Then
                If r < startR Then Return 0
                If r >= startR + weich Then Return 255
                Dim t As Double = (r - startR) / Math.Max(1, weich)
                Return CByte(t * 255)
            Else
                If r > startR Then Return 0
                If r <= startR - weich Then Return 255
                Dim t As Double = (startR - r) / Math.Max(1, weich)
                Return CByte(t * 255)
            End If
        End Function

        'Bilder über die Maske ineinander blenden.
        Private Function BlendWithMask(oldBmp As Bitmap, newBmp As Bitmap, mask As Bitmap) As Bitmap
            Dim w As Integer = oldBmp.Width
            Dim h As Integer = oldBmp.Height
            Dim rect As New Rectangle(0, 0, w, h)

            Dim outBmp As New Bitmap(w, h, Imaging.PixelFormat.Format32bppArgb)

            Dim dOld = oldBmp.LockBits(rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
            Dim dNew = newBmp.LockBits(rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
            Dim dMsk = mask.LockBits(rect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
            Dim dOut = outBmp.LockBits(rect, Imaging.ImageLockMode.WriteOnly, Imaging.PixelFormat.Format32bppArgb)

            Dim stride As Integer = dOut.Stride
            Dim bufOld(stride * h - 1) As Byte
            Dim bufNew(stride * h - 1) As Byte
            Dim bufMsk(stride * h - 1) As Byte
            Dim bufOut(stride * h - 1) As Byte

            Runtime.InteropServices.Marshal.Copy(dOld.Scan0, bufOld, 0, bufOld.Length)
            Runtime.InteropServices.Marshal.Copy(dNew.Scan0, bufNew, 0, bufNew.Length)
            Runtime.InteropServices.Marshal.Copy(dMsk.Scan0, bufMsk, 0, bufMsk.Length)

            For y As Integer = 0 To h - 1
                For x As Integer = 0 To w - 1
                    Dim ofs As Integer = y * stride + x * 4
                    Dim a As Double = bufMsk(ofs + 3) / 255.0

                    ' B,G,R (A = 255)
                    For c As Integer = 0 To 2
                        Dim v As Double = bufOld(ofs + c) * a + bufNew(ofs + c) * (1 - a)
                        bufOut(ofs + c) = CByte(Math.Min(255, Math.Max(0, v)))
                    Next
                    bufOut(ofs + 3) = 255
                Next
            Next

            Runtime.InteropServices.Marshal.Copy(bufOut, 0, dOut.Scan0, bufOut.Length)

            oldBmp.UnlockBits(dOld)
            newBmp.UnlockBits(dNew)
            mask.UnlockBits(dMsk)
            outBmp.UnlockBits(dOut)

            Return outBmp
        End Function

        'Render-Loop
        Public Sub StartRenderLoop(drawActionInput As Action(Of DrawingContext, Windows.Size),
                           zielGroesse As Windows.Size)
            'RenderLoop starten und einmaliges renderTargetBitmap anlegen

            drawAction = drawActionInput
            renderSize = zielGroesse

            'Alten Timer stoppen
            StopRenderLoop()

            ' Falls Größe geändert → neues RTB erzeugen
            If rtbCache Is Nothing OrElse
               rtbCache.PixelWidth <> CInt(renderSize.Width) OrElse
               rtbCache.PixelHeight <> CInt(renderSize.Height) Then

                rtbCache = New RenderTargetBitmap(CInt(renderSize.Width),
                                          CInt(renderSize.Height),
                                          96, 96, PixelFormats.Pbgra32)
            End If

            'Neuen Timer starten
            frameTimer = New DispatcherTimer()
            AddHandler frameTimer.Tick, AddressOf OnFrameTick
            frameTimer.Interval = TimeSpan.FromMilliseconds(1000 \ FPS)
            frameTimer.Start()
        End Sub

        Private Sub OnFrameTick(sender As Object, e As EventArgs)
            'Zeichnet einen einzelnen Frame
            Dim hintergrundBrush As New Media.SolidColorBrush(SDColorToWMColor(HintergrundFarbeSaver))

            If rtbCache Is Nothing OrElse drawAction Is Nothing Then Exit Sub

            Dim drawingVisual As New DrawingVisual()
            Using dc As DrawingContext = drawingVisual.RenderOpen()
                ' Hintergrund leeren
                dc.DrawRectangle(hintergrundBrush, Nothing,
                         New Rect(0, 0, renderSize.Width, renderSize.Height))
                ' Benutzerdefinierte Zeichenlogik
                drawAction.Invoke(dc, renderSize)
            End Using

            ' RTB mit neuem Inhalt füllen
            ClearRTB(rtbCache)
            rtbCache.Render(drawingVisual)

            RaiseEvent TransitionFrameIstFertig(rtbCache)

            'Aufräumen nach dem letzten Frame
            If stopRequested Then
                StopRenderLoop()
                rtbCache = Nothing
            End If
        End Sub

        Public Sub StopRenderLoop()
            'RenderLoop beenden und aufräumen

            If frameTimer IsNot Nothing Then
                frameTimer.Stop()
                RemoveHandler frameTimer.Tick, AddressOf OnFrameTick
                frameTimer = Nothing
            End If

        End Sub

        Public Sub ClearRTB(rtb As RenderTargetBitmap)
            'rtbCache leeren

            If rtb Is Nothing Then Exit Sub
            Dim dv As New DrawingVisual()
            Using dc As DrawingContext = dv.RenderOpen()
                dc.DrawRectangle(Media.Brushes.Transparent, Nothing,
                             New Rect(0, 0, rtb.PixelWidth, rtb.PixelHeight))
            End Using
            rtb.Render(dv)

        End Sub

    End Class

End Namespace
