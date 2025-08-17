Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging
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

        'Blending-Maske
        Private mask As Bitmap
        Private breiteGradientInPx As Integer
        Private startX As Integer : Private startY As Integer
        Private endX As Integer : Private endY As Integer

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

            'Lineare Maske vorgenerieren
            Select Case richtung
                Case "O", "W" 'Horizontale Maske bauen
                    mask = BuildHorizontalMask(clientSize.Width, clientSize.Height, BreiteInPixel(clientSize.Width, aktuelleSettings.breite))
                Case "N", "S" 'Vertikale Maske bauen
                    mask = BuildVerticalMask(clientSize.Width, clientSize.Height, BreiteInPixel(clientSize.Height, aktuelleSettings.breite))
                Case "NW", "NO", "SO", "SW" 'Diagonale Maske bauen
                    Dim diagLen As Double = Math.Sqrt((Math.Min(clientSize.Width, clientSize.Height) ^ 2) * 2)
                    mask = BuildDiagonalMask(clientSize.Width, clientSize.Height, BreiteInPixel(diagLen, aktuelleSettings.breite))
                Case Else 'Radiale Masken werden während der Laufzeit generiert
            End Select

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
            Dim frame As Bitmap
            Dim imgSource As ImageSource
            Dim maskRectangle As Rectangle
            ' DrawTransitionFrame – Maske/Rahmen setzen
            Dim fullRect As New Rectangle(0, 0, CInt(size.Width), CInt(size.Height))

            Select Case richtung
                Case "ZIn"
                    mask = GenerateRadialMask(size.Width, size.Height, progress, aktuelleSettings.breite, True)
                    maskRectangle = fullRect
                Case "ZOut"
                    mask = GenerateRadialMask(size.Width, size.Height, progress, aktuelleSettings.breite, False)
                    maskRectangle = fullRect
                Case Else
                    maskRectangle = ShiftMaskRectangle(progress, size)
            End Select

            If mask Is Nothing Then
                ConvertRenderTargetBitmapToBitmap(newBmpGerahmt) ' Fallback
                StopTransition()
            End If

            'Bilder per Maske mischen
            frame = BlendWithMask(oldBmp, newBmp, mask, maskRectangle)

            'In ImageSource umwandeln
            imgSource = ConvertBitmapToImageSource(frame)

            ' Bild zeichnen
            dc.DrawImage(imgSource, New Rect(0, 0, size.Width, size.Height))

            ' Fertig?
            If progress >= 1.0 Then
                StopTransition()
            End If

        End Sub

        'Linearen Masken generieren
        'Horizontal
        Private Function BuildHorizontalMask(imgW As Integer, imgH As Integer, breitePx As Integer) As Bitmap
            ' Größe:  [Schwarz-Block (imgW)] + [Gradient (breitePx)] + [Weiß-Block (imgW)] x imgH
            Dim mw As Integer = imgW * 2 + breitePx
            Dim mh As Integer = imgH
            Dim bmp As New Bitmap(mw, mh, Imaging.PixelFormat.Format32bppArgb)

            Dim rect As New Rectangle(0, 0, mw, mh)
            Dim data = bmp.LockBits(rect, Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat)
            Dim stride = data.Stride
            Dim bpp = 4
            Dim buf(stride * mh - 1) As Byte

            For y = 0 To mh - 1
                Dim ofs As Integer = y * stride
                'linker Schwarzblock
                For x = 0 To imgW - 1
                    buf(ofs + x * bpp + 3) = 0
                    buf(ofs + x * bpp + 2) = 0 : buf(ofs + x * bpp + 1) = 0 : buf(ofs + x * bpp + 0) = 0
                Next
                'Gradient
                For x = 0 To breitePx - 1
                    Dim a As Byte = CByte(x * 255 / Math.Max(1, breitePx - 1))
                    Dim idx = ofs + (imgW + x) * bpp
                    buf(idx + 3) = a
                    buf(idx + 2) = a : buf(idx + 1) = a : buf(idx + 0) = a
                Next
                'rechter Weißblock
                For x = 0 To imgW - 1
                    Dim idx = ofs + (imgW + breitePx + x) * bpp
                    buf(idx + 3) = 255
                    buf(idx + 2) = 255 : buf(idx + 1) = 255 : buf(idx + 0) = 255
                Next
            Next

            Runtime.InteropServices.Marshal.Copy(buf, 0, data.Scan0, buf.Length)
            bmp.UnlockBits(data)

            If richtung = "W" Then
                startX = imgW + breitePx : endX = 0
                startY = 0 : endY = 0
            Else 'Richtung "O" 
                InvertMaskBitmap(bmp)
                startX = 0 : endX = imgW + breitePx
                startY = 0 : endY = 0
            End If

            Return bmp

        End Function

        'Vertikal
        Private Function BuildVerticalMask(imgW As Integer, imgH As Integer, breitePx As Integer) As Bitmap
            'Größe: imgW x [Schwarz-Block (imgH)] + [Gradient (breitePx)] + [Weiß-Block (imgH)] 

            Dim mw As Integer = imgW
            Dim mh As Integer = imgH * 2 + Math.Max(1, breitePx)
            breitePx = Math.Max(1, breitePx)

            Dim bmp As New Bitmap(mw, mh, Imaging.PixelFormat.Format32bppArgb)
            Dim rect As New Rectangle(0, 0, mw, mh)
            Dim data = bmp.LockBits(rect, Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat)
            Dim stride As Integer = data.Stride
            Dim bpp As Integer = 4
            Dim buf(stride * mh - 1) As Byte

            LogDebug("Maskengröße bei Richtung: " & richtung & " und Breite: " & aktuelleSettings.breite.ToString & " ist gleich: " & mw.ToString & " x " & mh.ToString)

            'oberer Schwarz-Block
            For y As Integer = 0 To imgH - 1
                Dim row As Integer = y * stride
                For x As Integer = 0 To mw - 1
                    Dim idx = row + x * bpp
                    buf(idx + 3) = 0        ' A
                    buf(idx + 2) = 0        ' R
                    buf(idx + 1) = 0        ' G
                    buf(idx + 0) = 0        ' B  (RGB nur zur Visualisierung)
                Next
            Next

            'vertikaler Gradient (0 -> 255)
            For g As Integer = 0 To breitePx - 1
                Dim a As Byte = CByte(g * 255 / Math.Max(1, breitePx - 1))
                Dim y As Integer = imgH + g
                Dim row As Integer = y * stride
                For x As Integer = 0 To mw - 1
                    Dim idx = row + x * bpp
                    buf(idx + 3) = a
                    buf(idx + 2) = a
                    buf(idx + 1) = a
                    buf(idx + 0) = a
                Next
            Next

            'unterer Weiß-Block
            For yOff As Integer = 0 To imgH - 1
                Dim y As Integer = imgH + breitePx + yOff
                Dim row As Integer = y * stride
                For x As Integer = 0 To mw - 1
                    Dim idx = row + x * bpp
                    buf(idx + 3) = 255
                    buf(idx + 2) = 255
                    buf(idx + 1) = 255
                    buf(idx + 0) = 255
                Next
            Next

            Runtime.InteropServices.Marshal.Copy(buf, 0, data.Scan0, buf.Length)
            bmp.UnlockBits(data)

            If richtung = "N" Then
                startX = 0 : endX = 0
                startY = imgH + breitePx : endY = 0
            Else 'Richtung "S"
                InvertMaskBitmap(bmp)
                startX = 0 : endX = 0
                startY = 0 : endY = imgH + breitePx
            End If

            Return bmp
        End Function

        'Diagonal
        Private Function BuildDiagonalMask(imgW As Integer, imgH As Integer, breitePxAxis As Integer) As Bitmap
            Dim weich45 As Integer = Math.Max(1, CInt(breitePxAxis / Math.Sqrt(2.0)))

            ' Mittelteil MUSS um die Verlaufsbreite erweitert werden:
            Dim midW As Integer = imgH + weich45        ' <— NEU: statt nur imgH
            Dim mw As Integer = imgW * 2 + midW
            Dim mh As Integer = imgH

            Dim bmp As New Bitmap(mw, mh, Imaging.PixelFormat.Format32bppArgb)
            Dim rect As New Rectangle(0, 0, mw, mh)
            Dim data = bmp.LockBits(rect, Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat)
            Dim stride As Integer = data.Stride
            Dim bpp As Integer = 4
            Dim buf(stride * mh - 1) As Byte
            Dim farbe As Integer

            If richtung = "NW" OrElse richtung = "SW" Then
                farbe = 0
            Else
                farbe = 255
            End If

            ' linker Block
            For y As Integer = 0 To mh - 1
                Dim row = y * stride
                For x As Integer = 0 To imgW - 1
                    Dim ofs = row + x * bpp
                    buf(ofs + 3) = CByte(farbe)  ' A
                    buf(ofs + 2) = CByte(farbe) : buf(ofs + 1) = CByte(farbe) : buf(ofs + 0) = CByte(farbe)
                Next
            Next

            ' mittleres Diagonal-„Quadrat“ — jetzt midW breit (imgH + weich45)
            ' NW: Diagonale von unten links nach oben rechts, weiß unterhalb/rechts
            ' NO: Diagonale von oben links nach unten rechts, weiß unterhalb/links
            ' SW: Diagonale von oben links nach unten rechts, weiß oberhalb/rechts
            ' SO: Diagonale von unten links nach oben rechts, weiß oberhalb/links
            For y As Integer = 0 To mh - 1
                Dim row = y * stride
                For u As Integer = 0 To midW - 1
                    Dim x As Integer = imgW + u
                    Dim ofs = row + x * bpp
                    Dim a As Integer

                    If richtung = "SW" OrElse richtung = "NO" Then
                        ' Abstand zu v = u
                        Dim d As Integer = u - y
                        If d <= 0 Then
                            a = farbe
                        ElseIf d >= weich45 Then
                            a = 255 - farbe
                        Else
                            a = Math.Abs(farbe - CInt((d / Math.Max(1, weich45)) * 255))
                        End If
                    Else
                        ' NW / SO: Abstand zu v = (H-1 - u)
                        Dim d As Integer = (u + y) - (imgH - 1)
                        If d <= 0 Then
                            a = farbe
                        ElseIf d >= weich45 Then
                            a = 255 - farbe
                        Else
                            a = Math.Abs(farbe - CInt((d / Math.Max(1, weich45)) * 255))
                        End If
                    End If

                    Dim aa As Byte = CByte(Math.Max(0, Math.Min(255, a)))
                    buf(ofs + 3) = aa
                    buf(ofs + 2) = aa : buf(ofs + 1) = aa : buf(ofs + 0) = aa
                Next
            Next

            ' rechter Block — beginnt jetzt bei imgW + midW
            For y As Integer = 0 To mh - 1
                Dim row = y * stride
                For x As Integer = imgW + midW To mw - 1
                    Dim ofs = row + x * bpp
                    Dim val As Byte = CByte(255 - farbe)
                    buf(ofs + 3) = val
                    buf(ofs + 2) = val : buf(ofs + 1) = val : buf(ofs + 0) = val
                Next
            Next

            Runtime.InteropServices.Marshal.Copy(buf, 0, data.Scan0, buf.Length)
            bmp.UnlockBits(data)

            ' Bewegungsspanne anpassen: travel = imgW + midW
            Dim travel As Integer = imgW + midW
            If richtung = "NW" OrElse richtung = "SW" Then
                startX = travel : endX = 0
            Else
                startX = 0 : endX = travel
            End If
            startY = 0 : endY = 0

            Return bmp
        End Function


        'Lineare Masken verschieben
        Private Function ShiftMaskRectangle(progress As Double, sizeDrawingFrame As System.Windows.Size) As Rectangle
            Dim p As Double = Math.Max(0.0, Math.Min(1.0, progress))
            Dim frameW As Integer = CInt(sizeDrawingFrame.Width)
            Dim frameH As Integer = CInt(sizeDrawingFrame.Height)

            If mask Is Nothing Then
                Return New Rectangle(0, 0, frameW, frameH)
            End If

            ' Linearer Interpolator (runde, damit der letzte Frame sicher erreicht wird)
            Dim srcX As Integer = CInt(Math.Round(startX + (endX - startX) * p))
            Dim srcY As Integer = CInt(Math.Round(startY + (endY - startY) * p))

            Return New Rectangle(srcX, srcY, frameW, frameH)

        End Function

        'Radiale Maske generieren
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
            Dim factor As Double = Math.Max(0.0, Math.Min(1.0, breite / 100.0))
            weich = Math.Max(1, CInt(factor * maxR))

            ' progress bestimmt den Radius der „Kante“
            If inward Then
                startR = (maxR + weich) - progress * (maxR + weich)
            Else
                startR = -weich + progress * (maxR + weich)
            End If

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
                    buffer(ofs + 0) = a
                    buffer(ofs + 1) = a
                    buffer(ofs + 2) = a
                    buffer(ofs + 3) = a
                Next
            Next

            Runtime.InteropServices.Marshal.Copy(buffer, 0, data.Scan0, buffer.Length)
            bmp.UnlockBits(data)
            Return bmp
        End Function

        ' Bilder mit Maske blenden.
        ' Semantik: Weiß (Alpha 255) der Maske = ALT voll sichtbar, Schwarz (Alpha 0) = NEU voll sichtbar.
        Private Function BlendWithMask(oldBmp As Bitmap, newBmp As Bitmap, bigMask As Bitmap, maskSrcRect As Rectangle) As Bitmap
            Dim w As Integer = oldBmp.Width
            Dim h As Integer = oldBmp.Height
            Dim outBmp As New Bitmap(w, h, Imaging.PixelFormat.Format32bppArgb)
            Dim fullRect As New Rectangle(0, 0, w, h)

            ' WICHTIG: ggf. in 32bppARGB konvertieren, bevor gelockt wird
            If bigMask.PixelFormat <> Imaging.PixelFormat.Format32bppArgb Then
                bigMask = bigMask.Clone(New Rectangle(0, 0, bigMask.Width, bigMask.Height), Imaging.PixelFormat.Format32bppArgb)
            End If

            Dim dOld As Imaging.BitmapData = Nothing
            Dim dNew As Imaging.BitmapData = Nothing
            Dim dMsk As Imaging.BitmapData = Nothing
            Dim dOut As Imaging.BitmapData = Nothing

            Try
                dOld = oldBmp.LockBits(fullRect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                dNew = newBmp.LockBits(fullRect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                dMsk = bigMask.LockBits(maskSrcRect, Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
                dOut = outBmp.LockBits(fullRect, Imaging.ImageLockMode.WriteOnly, Imaging.PixelFormat.Format32bppArgb)

                Dim sOld As Integer = dOld.Stride
                Dim sNew As Integer = dNew.Stride
                Dim sMsk As Integer = dMsk.Stride           ' Stride der GESAMT-Maske (nicht Rect-Breite!)
                Dim sOut As Integer = dOut.Stride

                ' Vollbild-Puffer (Stride-basiert) für alt/neu/out
                Dim bufOld(sOld * h - 1) As Byte
                Dim bufNew(sNew * h - 1) As Byte
                Dim bufOut(sOut * h - 1) As Byte

                ' Maske: COMPACT-Puffer in Rechteck-BREITE (w * 4) – zeilenweise kopieren!
                Dim rowBytesMsk As Integer = w * 4
                Dim bufMsk(rowBytesMsk * h - 1) As Byte

                ' Kopieren
                Runtime.InteropServices.Marshal.Copy(dOld.Scan0, bufOld, 0, bufOld.Length)
                Runtime.InteropServices.Marshal.Copy(dNew.Scan0, bufNew, 0, bufNew.Length)

                ' Maske: pro Zeile nur die eigentliche Rect-Breite kopieren
                For y As Integer = 0 To h - 1
                    Dim srcPtr As IntPtr = IntPtr.Add(dMsk.Scan0, y * sMsk)
                    Runtime.InteropServices.Marshal.Copy(srcPtr, bufMsk, y * rowBytesMsk, rowBytesMsk)
                Next

                ' Mischen: weiß = ALT, schwarz = NEU
                For y As Integer = 0 To h - 1
                    Dim oRow As Integer = y * sOld
                    Dim nRow As Integer = y * sNew
                    Dim mRow As Integer = y * rowBytesMsk
                    Dim outRow As Integer = y * sOut

                    For x As Integer = 0 To w - 1
                        Dim oOfs As Integer = oRow + x * 4
                        Dim nOfs As Integer = nRow + x * 4
                        Dim mOfs As Integer = mRow + x * 4
                        Dim outOfs As Integer = outRow + x * 4

                        ' aAlt = Masken-Alpha normiert: 1.0 => ALT voll sichtbar (weiß), 0.0 => ALT unsichtbar (schwarz)
                        Dim aAlt As Double = bufMsk(mOfs + 3) / 255.0
                        Dim aNeu As Double = 1.0 - aAlt

                        ' B, G, R (Reihenfolge: BGRA)
                        Dim b As Double = bufOld(oOfs + 0) * aAlt + bufNew(nOfs + 0) * aNeu
                        Dim g As Double = bufOld(oOfs + 1) * aAlt + bufNew(nOfs + 1) * aNeu
                        Dim r As Double = bufOld(oOfs + 2) * aAlt + bufNew(nOfs + 2) * aNeu

                        bufOut(outOfs + 0) = CByte(If(b < 0, 0, If(b > 255, 255, b)))
                        bufOut(outOfs + 1) = CByte(If(g < 0, 0, If(g > 255, 255, g)))
                        bufOut(outOfs + 2) = CByte(If(r < 0, 0, If(r > 255, 255, r)))
                        bufOut(outOfs + 3) = 255
                    Next
                Next

                Runtime.InteropServices.Marshal.Copy(bufOut, 0, dOut.Scan0, bufOut.Length)

            Finally
                If dOld IsNot Nothing Then oldBmp.UnlockBits(dOld)
                If dNew IsNot Nothing Then newBmp.UnlockBits(dNew)
                If dMsk IsNot Nothing Then bigMask.UnlockBits(dMsk)
                If dOut IsNot Nothing Then outBmp.UnlockBits(dOut)
            End Try

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

        'Helper-Funktionen
        Private Function Clamp01(x As Double) As Double
            If x < 0 Then Return 0
            If x > 1 Then Return 1
            Return x
        End Function

        Private Function BreiteInPixel(imgSize As Double, breiteProzent As Double) As Integer

            Dim f As Double = Clamp01(breiteProzent / 100.0)
            breiteGradientInPx = Math.Max(1, CInt(imgSize * f))
            Return breiteGradientInPx

        End Function

        Public Sub InvertMaskBitmap(bmp As Bitmap, Optional alphaOnly As Boolean = True)
            If bmp Is Nothing Then Exit Sub
            Dim rect As New Rectangle(0, 0, bmp.Width, bmp.Height)
            Dim data = bmp.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            Dim stride = data.Stride
            Dim bytes(stride * bmp.Height - 1) As Byte
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length)

            For i As Integer = 0 To bytes.Length - 4 Step 4
                ' BGRA Reihenfolge
                If alphaOnly Then
                    bytes(i + 3) = CByte(255 - bytes(i + 3))     ' Alpha invertieren
                Else
                    bytes(i + 0) = CByte(255 - bytes(i + 0))     ' B
                    bytes(i + 1) = CByte(255 - bytes(i + 1))     ' G
                    bytes(i + 2) = CByte(255 - bytes(i + 2))     ' R
                    bytes(i + 3) = CByte(255 - bytes(i + 3))     ' A
                End If
            Next

            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length)
            bmp.UnlockBits(data)
        End Sub

        Private Function ComputeRadial(r As Double, startR As Double, weich As Integer, inward As Boolean) As Byte
            'Radialen Gradienten berechnen: outward=False (Ring wächst), inward=True (Ring schrumpft nach innen)
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

    End Class

End Namespace
