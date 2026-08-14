Imports System.Diagnostics
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
        'Settings und Konstanten
        Public Const SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH As String = SLIDESHOWTRANSITION_PATH &
            "Gradient-Wischen\"

        Public Const nameTransition As String = "Gradient-Wischen"

        Private aktuelleSettings As New SlideShowTransitionSettings_GradientWischen()

        'Zeitmanagement
        Private ReadOnly laufzeit As New Stopwatch()
        Private dauerInMS As Integer
        Private externeDauerInMS As Integer

        'Transitions-Bilder
        Private oldBmpGerahmt As RenderTargetBitmap
        Private newBmpGerahmt As RenderTargetBitmap
        Private oldBmp As Bitmap
        Private newBmp As Bitmap

        'Blending-Maske
        Private mask As Bitmap

        Private startX As Integer
        Private startY As Integer
        Private endX As Integer
        Private endY As Integer

        'Animation
        Private richtung As String

        'Zielausgabe
        Private targetRect As Rectangle
        Private sizeWinForms As System.Drawing.Size

        'Rendering
        Private frameTimer As DispatcherTimer

        Private gdiFrameBitmap As Bitmap
        Private gdiFrameSource As WriteableBitmap

        Private oldPixelBuffer() As Byte
        Private newPixelBuffer() As Byte
        Private maskPixelBuffer() As Byte
        Private framePixelBuffer() As Byte
        Private radialMaskPixelBuffer() As Byte

        Private oldPixelStride As Integer
        Private newPixelStride As Integer
        Private framePixelStride As Integer
        Private maskRowBytes As Integer

        Private transitionLaeuft As Boolean
        Private wurdeBereinigt As Boolean

        'Sonstiges
        Private ReadOnly rnd As New Random()

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
        Event TransitionFrameIstFertig(bitmap As ImageSource) Implements ISlideShowTransition.TransitionFrameIstFertig

        'Transition Ausführung
        Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As BitmapImage,
                                 picBoxModeNew As PictureBoxSizeMode, clientSize As System.Drawing.Size,
                                 Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition
            'Initialisiert und startet eine neue Gradient-Wischen-Transition.

            Dim richtungArray() As String

            If wurdeBereinigt Then

                Throw New ObjectDisposedException(NameOf(TransitionMain))

            End If

            BeendeUndBereinigeTransition()

            If oldImage Is Nothing Then

                Throw New ArgumentNullException(NameOf(oldImage))

            End If

            If newImage Is Nothing Then

                Throw New ArgumentNullException(NameOf(newImage))

            End If

            richtungArray = New String() {"N", "NO", "O", "SO", "S", "SW", "W", "NW", "ZOut", "ZIn"}

            Try

                ReadTransitionSettingsFromRegistryOrDefaults()

                StoreSettings(TransitionName, aktuelleSettings)

                sizeWinForms = clientSize

                oldBmpGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
                newBmpGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

                oldBmp = ConvertRenderTargetBitmapToBitmap(oldBmpGerahmt)
                newBmp = ConvertRenderTargetBitmapToBitmap(newBmpGerahmt)

                If aktuelleSettings.richtungen IsNot Nothing AndAlso aktuelleSettings.richtungen.Count > 0 Then

                    richtung = aktuelleSettings.richtungen(rnd.Next(aktuelleSettings.richtungen.Count))

                Else

                    richtung = richtungArray(rnd.Next(richtungArray.Length))

                End If

                Select Case richtung

                    Case "O", "W"

                        mask = BuildHorizontalMask(clientSize.Width, clientSize.Height,
                                                   BreiteInPixel(clientSize.Width, aktuelleSettings.breite))

                    Case "N", "S"

                        mask = BuildVerticalMask(clientSize.Width, clientSize.Height,
                                                 BreiteInPixel(clientSize.Height, aktuelleSettings.breite))

                    Case "NW", "NO", "SO", "SW"

                        Dim diagLen As Double

                        diagLen = Math.Sqrt((Math.Min(clientSize.Width, clientSize.Height) ^ 2) * 2)

                        mask = BuildDiagonalMask(clientSize.Width, clientSize.Height,
                                                 BreiteInPixel(diagLen, aktuelleSettings.breite))

                    Case "ZIn", "ZOut"

                        mask = New Bitmap(clientSize.Width, clientSize.Height, Imaging.PixelFormat.Format32bppArgb)

                End Select

                dauerInMS = Math.Max(1, 1000 * aktuelleSettings.geschwindigkeit)
                externeDauerInMS = Math.Max(0, durationMs)

                laufzeit.Restart()

                transitionLaeuft = True

                StartRenderLoop()

                RaiseEvent TransitionIsRunning(True)

            Catch ex As Exception

                LogError("Transition Gradient-Wischen - TransitionMain.RunTransition(): " &
                         "Fehler beim Starten der Transition: " & ex.ToString())

                BeendeUndBereinigeTransition()

                Throw

            End Try

        End Sub

        Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
            'Beendet eine laufende Transition und gibt sämtliche gehaltenen Ressourcen frei.

            Dim warAktiv As Boolean

            warAktiv = transitionLaeuft

            BeendeUndBereinigeTransition()

            If warAktiv Then

                RaiseEvent TransitionIsRunning(False)

            End If

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
        Private Sub ReadTransitionSettingsFromRegistryOrDefaults()
            'Setzt aktuelleTransitonSettings mit den Werten aus der Registry oder mit Defaultwerten.

            Dim defaults As Dictionary(Of String, String) = GetTransitionDefaultSettings()

            aktuelleSettings.geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Geschwindigkeit", defaults))
            aktuelleSettings.richtungen = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Richtungen", defaults))
            aktuelleSettings.breite = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_GRADIENTWISCHEN_FULLPATH & "Breite", defaults))

        End Sub

        Friend Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
            Dim defaults As New Dictionary(Of String, String)

            defaults.Add("Geschwindigkeit", "20")
            defaults.Add("Richtungen", "W; O; ZIn; ZOut")
            defaults.Add("Breite", "35")

            Return defaults

        End Function

        'Abbruch- und Endverwaltung
        Private Sub EndBildZeichnen()
            'Gibt das endgültige Zielbild aus.

            If newBmpGerahmt Is Nothing Then
                Exit Sub
            End If

            RaiseEvent TransitionFrameIstFertig(newBmpGerahmt)

        End Sub

        'Animation
        Private Sub DrawTransitionFrame()
            'Erzeugt einen Transitionframe direkt im wiederverwendeten
            'GDI-Zielbitmap und überträgt ihn anschließend in das
            'wiederverwendete WriteableBitmap.

            Dim progress As Double
            Dim maskRectangle As Rectangle
            Dim fullRect As Rectangle

            If gdiFrameBitmap Is Nothing OrElse gdiFrameSource Is Nothing Then

                Exit Sub

            End If

            progress = Math.Min(1.0, laufzeit.Elapsed.TotalMilliseconds / dauerInMS)

            fullRect = New Rectangle(0, 0, sizeWinForms.Width, sizeWinForms.Height)

            Select Case richtung

                Case "ZIn"

                    AktualisiereRadialMask(mask, progress, aktuelleSettings.breite, True)

                    maskRectangle = fullRect

                Case "ZOut"

                    AktualisiereRadialMask(mask, progress, aktuelleSettings.breite, False)

                    maskRectangle = fullRect

                Case Else

                    maskRectangle = ShiftMaskRectangle(progress, New Windows.Size(sizeWinForms.Width,
                                                                                  sizeWinForms.Height))

            End Select

            If mask Is Nothing Then

                EndBildZeichnen()
                StopTransition()

                Exit Sub

            End If

            BlendWithMask(mask, maskRectangle, gdiFrameBitmap)

            AktualisiereWriteableBitmap(gdiFrameBitmap, gdiFrameSource)

            RaiseEvent TransitionFrameIstFertig(gdiFrameSource)

            If progress >= 1.0 Then

                EndBildZeichnen()
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
        Private Sub AktualisiereRadialMask(zielMaske As Bitmap, progress As Double, breite As Double,
                                           inward As Boolean)
            'Aktualisiert eine bestehende Radialmaske in-place
            'unter Verwendung eines wiederverwendeten Pixelpuffers.

            Dim width As Integer
            Dim height As Integer

            Dim rect As Rectangle
            Dim data As Imaging.BitmapData

            Dim stride As Integer
            Dim bpp As Integer

            Dim weich As Integer

            Dim cx As Double
            Dim cy As Double
            Dim maxR As Double
            Dim startR As Double
            Dim faktor As Double

            data = Nothing

            If zielMaske Is Nothing Then
                Exit Sub
            End If

            width = zielMaske.Width
            height = zielMaske.Height

            rect = New Rectangle(0, 0, width, height)

            Try

                data = zielMaske.LockBits(rect, Imaging.ImageLockMode.WriteOnly, Imaging.PixelFormat.Format32bppArgb)

                stride = data.Stride

                bpp = 4

                If radialMaskPixelBuffer Is Nothing OrElse radialMaskPixelBuffer.Length <> Math.Abs(stride) *
                                                                                            height Then

                    ReDim radialMaskPixelBuffer(Math.Abs(stride) * height - 1)

                End If

                cx = (width - 1) / 2.0
                cy = (height - 1) / 2.0

                maxR = Math.Sqrt(cx * cx + cy * cy)

                faktor = Math.Max(0.0, Math.Min(1.0, breite / 100.0))

                weich = Math.Max(1, CInt(faktor * maxR))

                If inward Then

                    startR = (maxR + weich) - progress * (maxR + weich)

                Else

                    startR = -weich + progress * (maxR + weich)

                End If

                For y As Integer = 0 To height - 1

                    For x As Integer = 0 To width - 1

                        Dim dx As Double
                        Dim dy As Double
                        Dim radius As Double

                        Dim alpha As Byte
                        Dim offset As Integer

                        dx = x - cx
                        dy = y - cy

                        radius = Math.Sqrt(dx * dx + dy * dy)

                        alpha = ComputeRadial(radius, startR, weich, inward)

                        offset = y * stride + x * bpp

                        radialMaskPixelBuffer(offset + 0) = alpha
                        radialMaskPixelBuffer(offset + 1) = alpha
                        radialMaskPixelBuffer(offset + 2) = alpha
                        radialMaskPixelBuffer(offset + 3) = alpha

                    Next

                Next

                Marshal.Copy(radialMaskPixelBuffer, 0, data.Scan0, radialMaskPixelBuffer.Length)

            Finally

                If data IsNot Nothing Then

                    zielMaske.UnlockBits(data)

                End If

            End Try

        End Sub

        ' Bilder mit Maske blenden.
        ' Semantik: Weiß (Alpha 255) der Maske = ALT voll sichtbar, Schwarz (Alpha 0) = NEU voll sichtbar.
        Private Sub BlendWithMask(bigMask As Bitmap, maskSrcRect As Rectangle, zielBitmap As Bitmap)
            'Mischt die einmalig gecachten Pixel von Alt- und Neubild
            'anhand der aktuellen Maske direkt in den wiederverwendeten
            'Framepuffer.

            Dim dMsk As Imaging.BitmapData
            Dim dOut As Imaging.BitmapData

            Dim maskStride As Integer
            Dim breite As Integer
            Dim hoehe As Integer

            dMsk = Nothing
            dOut = Nothing

            If bigMask Is Nothing OrElse zielBitmap Is Nothing Then

                Exit Sub

            End If

            breite = zielBitmap.Width
            hoehe = zielBitmap.Height

            If maskSrcRect.Width <> breite OrElse
                   maskSrcRect.Height <> hoehe OrElse
                   maskSrcRect.X < 0 OrElse
                   maskSrcRect.Y < 0 OrElse
                   maskSrcRect.Right > bigMask.Width OrElse
                   maskSrcRect.Bottom > bigMask.Height Then

                Throw New ArgumentOutOfRangeException(NameOf(maskSrcRect),
                                                      "Das Maskenrechteck liegt außerhalb der verfügbaren " &
                                                      "Maskenfläche.")

            End If

            Try

                dMsk = bigMask.LockBits(maskSrcRect, Imaging.ImageLockMode.ReadOnly,
                                        Imaging.PixelFormat.Format32bppArgb)
                dOut = zielBitmap.LockBits(New Rectangle(0, 0, breite, hoehe), Imaging.ImageLockMode.WriteOnly,
                                           Imaging.PixelFormat.Format32bppArgb)

                maskStride = dMsk.Stride

                'Das benötigte Maskenrechteck zeilenweise in den
                'bereits vorhandenen kompakten Maskenpuffer kopieren.
                For y As Integer = 0 To hoehe - 1

                    Dim srcPtr As IntPtr

                    srcPtr = IntPtr.Add(dMsk.Scan0, y * maskStride)

                    Marshal.Copy(srcPtr, maskPixelBuffer, y * maskRowBytes, maskRowBytes)

                Next

                For y As Integer = 0 To hoehe - 1

                    Dim oldRow As Integer
                    Dim newRow As Integer
                    Dim maskRow As Integer
                    Dim frameRow As Integer

                    oldRow = y * oldPixelStride
                    newRow = y * newPixelStride
                    maskRow = y * maskRowBytes
                    frameRow = y * framePixelStride

                    For x As Integer = 0 To breite - 1

                        Dim oldOffset As Integer
                        Dim newOffset As Integer
                        Dim maskOffset As Integer
                        Dim frameOffset As Integer

                        Dim alphaAlt As Integer
                        Dim alphaNeu As Integer

                        oldOffset = oldRow + x * 4
                        newOffset = newRow + x * 4
                        maskOffset = maskRow + x * 4
                        frameOffset = frameRow + x * 4

                        alphaAlt = maskPixelBuffer(maskOffset + 3)
                        alphaNeu = 255 - alphaAlt

                        framePixelBuffer(frameOffset + 0) = CByte((CInt(oldPixelBuffer(oldOffset + 0)) * alphaAlt +
                            CInt(newPixelBuffer(newOffset + 0)) * alphaNeu) \ 255)

                        framePixelBuffer(frameOffset + 1) = CByte((CInt(oldPixelBuffer(oldOffset + 1)) * alphaAlt +
                            CInt(newPixelBuffer(newOffset + 1)) * alphaNeu) \ 255)

                        framePixelBuffer(frameOffset + 2) = CByte((CInt(oldPixelBuffer(oldOffset + 2)) * alphaAlt +
                            CInt(newPixelBuffer(newOffset + 2)) * alphaNeu) \ 255)

                        framePixelBuffer(frameOffset + 3) = 255

                    Next

                Next

                Marshal.Copy(framePixelBuffer, 0, dOut.Scan0, framePixelBuffer.Length)

            Finally

                If dMsk IsNot Nothing Then

                    bigMask.UnlockBits(dMsk)

                End If

                If dOut IsNot Nothing Then

                    zielBitmap.UnlockBits(dOut)

                End If

            End Try

        End Sub

        'Render-Loop
        Private Sub StartRenderLoop()
            'Startet den zentralen Frame-Timer und legt die
            'wiederverwendeten Render- und Pixelpuffer an.

            Dim pixelBreite As Integer
            Dim pixelHoehe As Integer

            StopRenderLoop()

            pixelBreite = Math.Max(sizeWinForms.Width, 1)
            pixelHoehe = Math.Max(sizeWinForms.Height, 1)

            gdiFrameBitmap = New Bitmap(pixelBreite, pixelHoehe, Imaging.PixelFormat.Format32bppArgb)
            gdiFrameSource = New WriteableBitmap(pixelBreite, pixelHoehe, 96.0, 96.0, PixelFormats.Bgra32, Nothing)

            InitialisierePixelCaches()

            frameTimer = New DispatcherTimer(DispatcherPriority.Render)

            AddHandler frameTimer.Tick, AddressOf OnFrameTick

            frameTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0)

            frameTimer.Start()

        End Sub

        Private Sub InitialisierePixelCaches()
            'Legt die wiederverwendeten Pixelpuffer für Altbild, Neubild, Maske und Zielbild an.

            Dim fullRect As Rectangle

            Dim oldData As Imaging.BitmapData
            Dim newData As Imaging.BitmapData
            Dim frameData As Imaging.BitmapData

            oldData = Nothing
            newData = Nothing
            frameData = Nothing

            If oldBmp Is Nothing OrElse newBmp Is Nothing OrElse gdiFrameBitmap Is Nothing Then

                Throw New InvalidOperationException("Die Pixelcaches können ohne initialisierte Bildpuffer nicht" &
                                                    " erzeugt werden.")

            End If

            fullRect = New Rectangle(0, 0, oldBmp.Width, oldBmp.Height)

            Try

                oldData = oldBmp.LockBits(fullRect, Imaging.ImageLockMode.ReadOnly, oldBmp.PixelFormat)
                newData = newBmp.LockBits(fullRect, Imaging.ImageLockMode.ReadOnly, newBmp.PixelFormat)

                frameData = gdiFrameBitmap.LockBits(fullRect, Imaging.ImageLockMode.WriteOnly,
                                                    Imaging.PixelFormat.Format32bppArgb)

                oldPixelStride = oldData.Stride
                newPixelStride = newData.Stride
                framePixelStride = frameData.Stride

                maskRowBytes = oldBmp.Width * 4

                ReDim oldPixelBuffer(Math.Abs(oldPixelStride) * oldBmp.Height - 1)
                ReDim newPixelBuffer(Math.Abs(newPixelStride) * newBmp.Height - 1)
                ReDim framePixelBuffer(Math.Abs(framePixelStride) * gdiFrameBitmap.Height - 1)
                ReDim maskPixelBuffer(maskRowBytes * oldBmp.Height - 1)
                ReDim radialMaskPixelBuffer(maskRowBytes * oldBmp.Height - 1)

                Marshal.Copy(oldData.Scan0, oldPixelBuffer, 0, oldPixelBuffer.Length)
                Marshal.Copy(newData.Scan0, newPixelBuffer, 0, newPixelBuffer.Length)

            Finally

                If oldData IsNot Nothing Then

                    oldBmp.UnlockBits(oldData)

                End If

                If newData IsNot Nothing Then

                    newBmp.UnlockBits(newData)

                End If

                If frameData IsNot Nothing Then

                    gdiFrameBitmap.UnlockBits(frameData)

                End If

            End Try

        End Sub

        Private Sub OnFrameTick(sender As Object, e As EventArgs)
            'Steuert den zeitlichen Ablauf der Transition.

            If Not transitionLaeuft Then

                StopRenderLoop()

                Exit Sub

            End If

            If externeDauerInMS > 0 AndAlso laufzeit.Elapsed.TotalMilliseconds >= externeDauerInMS Then

                EndBildZeichnen()
                StopTransition()

                Exit Sub

            End If

            DrawTransitionFrame()

        End Sub

        Private Sub StopRenderLoop()
            'Stoppt den zentralen Frame-Timer und entfernt
            'den registrierten Tick-Handler.

            If frameTimer Is Nothing Then
                Exit Sub
            End If

            frameTimer.Stop()

            RemoveHandler frameTimer.Tick, AddressOf OnFrameTick

            frameTimer = Nothing

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

            Dim faktor As Double
            Dim breitePixel As Integer

            faktor = Clamp01(breiteProzent / 100.0)
            breitePixel = Math.Max(1, CInt(imgSize * faktor))

            Return breitePixel

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

        'Bereinigen und Dispose
        Private Sub BeendeUndBereinigeTransition()
            'Beendet sämtliche laufenden Arbeiten und gibt alle von der Transition gehaltenen Ressourcen frei.

            transitionLaeuft = False

            laufzeit.Stop()

            StopRenderLoop()

            If gdiFrameBitmap IsNot Nothing Then

                gdiFrameBitmap.Dispose()
                gdiFrameBitmap = Nothing

            End If

            gdiFrameSource = Nothing

            If mask IsNot Nothing Then

                mask.Dispose()
                mask = Nothing

            End If

            If oldBmp IsNot Nothing Then

                oldBmp.Dispose()
                oldBmp = Nothing

            End If

            If newBmp IsNot Nothing Then

                newBmp.Dispose()
                newBmp = Nothing

            End If

            oldBmpGerahmt = Nothing
            newBmpGerahmt = Nothing

            oldPixelBuffer = Nothing
            newPixelBuffer = Nothing
            maskPixelBuffer = Nothing
            framePixelBuffer = Nothing
            radialMaskPixelBuffer = Nothing

            oldPixelStride = 0
            newPixelStride = 0
            framePixelStride = 0

            maskRowBytes = 0

            richtung = Nothing

            externeDauerInMS = 0

        End Sub

#Region "IDisposable"

        Public Sub Dispose() Implements IDisposable.Dispose
            'Beendet die Transition endgültig und gibt
            'sämtliche gehaltenen Ressourcen frei.

            If wurdeBereinigt Then
                Exit Sub
            End If

            wurdeBereinigt = True

            BeendeUndBereinigeTransition()

            GC.SuppressFinalize(Me)

        End Sub

#End Region

    End Class

End Namespace
