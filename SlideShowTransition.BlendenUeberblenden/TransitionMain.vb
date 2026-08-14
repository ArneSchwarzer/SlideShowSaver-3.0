Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Interop
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging.LogHandling
Imports SlideShowTools
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.SharedDataHandling

Public Class TransitionMain
    Implements ISlideShowTransition

    'Variablendeklaration
#Region "Variablendeklaration"
    'Settings und Konstanten
    Public Const SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Blenden & Überblenden\"
    Public Const nameTransition As String = "Blenden & Überblenden"
    Private aktuelleSettings As TransitionSettings_FadeCrossfade

    'Zeitmanagement
    Private ReadOnly laufzeit As New Stopwatch()
    Private dauerInMS As Integer
    Private externeDauerInMS As Integer

    'Transitions-Bilder
    Private oldImg As Image
    Private newImg As Image
    Private oldRTB As RenderTargetBitmap
    Private newRTB As RenderTargetBitmap
    Private oldSize As System.Drawing.Size
    Private newSize As System.Drawing.Size

    'Animation
    Private drawRectOld As Rectangle
    Private drawRectNew As Rectangle
    Private containerRect As Rectangle

    'Rendering
    Private sizeWPF As Windows.Size
    Private sizeWF As System.Drawing.Size

    Private gdiFrameTimer As DispatcherTimer
    Private gdiRenderSize As Windows.Size
    Private gdiDrawAction As Action(Of Windows.Size)

    Private gdiFrameBitmap As Bitmap
    Private gdiFrameSource As WriteableBitmap

    Private transitionLaeuft As Boolean
    Private wurdeBereinigt As Boolean

    'Sonstiges
    Private rnd As New Random

    Structure TransitionSettings_FadeCrossfade
        Public Farbton As System.Drawing.Color
        Public Zufallsfarbe As Boolean
        Public Morphing As Boolean
        Public Modus As String
        Public Geschwindigkeit As Integer
    End Structure
#End Region

#Region "Eigenschaften"

    Public ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName
        Get
            Return nameTransition
        End Get
    End Property

    Public ReadOnly Property TransitionKurzBeschreibung As String Implements ISlideShowTransition.TransitionKurzBeschreibung
        Get
            Return "Blendet das alte Bild langsam aus und das neue ein."
        End Get
    End Property

    Public ReadOnly Property TransitionVersion As Version Implements ISlideShowTransition.TransitionVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

#End Region

#Region "Events"

    Public Event TransitionIsRunning As ISlideShowTransition.TransitionIsRunningEventHandler Implements ISlideShowTransition.TransitionIsRunning
    Public Event TransitionFrameIstFertig As ISlideShowTransition.TransitionFrameIstFertigEventHandler Implements ISlideShowTransition.TransitionFrameIstFertig


#End Region

    Public Sub RunTransition(
        oldImage As BitmapImage,
        picBoxModeOld As PictureBoxSizeMode,
        newImage As BitmapImage,
        picBoxModeNew As PictureBoxSizeMode,
        clientSize As System.Drawing.Size,
        Optional durationMs As Integer = 0) _
             Implements ISlideShowTransition.RunTransition

        'Bereitet die Animation vor und startet den Renderloop.

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(TransitionMain))

        End If

        If transitionLaeuft Then

            StopTransition()

        Else

            BeendeUndBereinigeTransition()

        End If

        Try

            ReadTransitionSettingsFromRegistryOrDefaults()

            StoreSettings(TransitionName, aktuelleSettings)

            sizeWPF = New Windows.Size(clientSize.Width, clientSize.Height)
            sizeWF = clientSize

            oldRTB = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
            newRTB = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

            oldImg = ConvertRenderTargetBitmapToBitmap(oldRTB)
            newImg = ConvertRenderTargetBitmapToBitmap(newRTB)

            oldSize = New System.Drawing.Size(oldImage.PixelWidth, oldImage.PixelHeight)
            newSize = New System.Drawing.Size(newImage.PixelWidth, newImage.PixelHeight)

            containerRect = New Rectangle(0, 0, sizeWF.Width, sizeWF.Height)
            drawRectOld = GetDrawRectangle(oldSize, containerRect, picBoxModeOld)
            drawRectNew = GetDrawRectangle(newSize, containerRect, picBoxModeNew)

            If aktuelleSettings.Modus = "Zufall" Then

                If rnd.Next(2) > 0 Then

                    aktuelleSettings.Modus = "Fade"

                Else

                    aktuelleSettings.Modus = "Crossfade"

                End If

            End If

            If aktuelleSettings.Zufallsfarbe Then

                aktuelleSettings.Farbton = SetzeZufallsFarbe()

            End If

            externeDauerInMS = Math.Max(0, durationMs)
            dauerInMS = Math.Max(1, 1000 * aktuelleSettings.Geschwindigkeit)

            laufzeit.Restart()

            transitionLaeuft = True

            StartGDIRenderLoop(AddressOf DrawGDITransitionFrame, sizeWPF)

            RaiseEvent TransitionIsRunning(True)

        Catch ex As Exception

            LogError("Transition Blenden & Überblenden - TransitionMain.RunTransition(): " &
                     "Fehler beim Starten der Transition: " & ex.ToString())

            BeendeUndBereinigeTransition()

            Throw

        End Try

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Beendet die Transition und gibt sämtliche gehaltenen
        'Ressourcen frei.

        Dim warAktiv As Boolean

        warAktiv = transitionLaeuft

        BeendeUndBereinigeTransition()

        If warAktiv Then

            RaiseEvent TransitionIsRunning(False)

        End If

    End Sub

    Public Function GetTransitionOptionsDialog() As UserControl Implements ISlideShowTransition.GetTransitionOptionsDialog
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
        'Liest die Transitionseinstellungen aus der Registry
        'oder verwendet die definierten Defaultwerte.

        Dim defaults As Dictionary(Of String, String)
        Dim colorString As String

        defaults = GetTransitionDefaultSettings()

        colorString = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Farbton", defaults)

        If String.IsNullOrWhiteSpace(colorString) Then

            colorString = defaults("Farbton")

        End If

        aktuelleSettings.Farbton = ColorHandling.StringToColor(colorString)

        aktuelleSettings.Zufallsfarbe = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH &
                                                              "Zufallsfarbe", defaults) = "True"

        aktuelleSettings.Morphing = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH &
                                                          "Morphing", defaults) = "True"

        aktuelleSettings.Modus = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", defaults)

        aktuelleSettings.Geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH &
                                                                      "Geschwindigkeit", defaults))

    End Sub

    Friend Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        Dim defaults As New Dictionary(Of String, String)

        defaults.Add("Farbton", "0, 0, 0, 255")
        defaults.Add("Zufallsfarbe", "False")
        defaults.Add("Morphing", "True")
        defaults.Add("Modus", "Fade")
        defaults.Add("Geschwindigkeit", "20")

        Return defaults

    End Function

    'Abbruch- und Endverwaltung
    Private Sub EndBildZeichnen()
        'Gibt den finalen Transitionframe aus.

        If newRTB Is Nothing Then
            Exit Sub
        End If

        RaiseEvent TransitionFrameIstFertig(newRTB)

    End Sub

    'Animation und rendern 
    Private Sub DrawGDITransitionFrame(size As Windows.Size)
        'Zeichnet einen GDI-Frame, überträgt dessen Pixel
        'direkt in das wiederverwendete WriteableBitmap
        'und meldet dieses als fertigen Frame.

        Dim phasenDauer1 As Single
        Dim phasenDauer2 As Single
        Dim phasenDauer3 As Single
        Dim morphColor As System.Drawing.Color
        Dim progress As Double

        If gdiFrameBitmap Is Nothing OrElse gdiFrameSource Is Nothing Then

            Exit Sub

        End If

        morphColor =
        System.Drawing.Color.FromArgb(
            255,
            aktuelleSettings.Farbton.R,
            aktuelleSettings.Farbton.G,
            aktuelleSettings.Farbton.B)

        progress = laufzeit.Elapsed.TotalMilliseconds / dauerInMS
        progress = Math.Min(progress, 1.0)

        If aktuelleSettings.Morphing Then

            phasenDauer1 = 0.33F
            phasenDauer2 = 0.33F
            phasenDauer3 = 0.34F

        Else

            phasenDauer1 = 0.495F
            phasenDauer2 = 0.01F
            phasenDauer3 = 0.495F

        End If

        Using gfx As Graphics = Graphics.FromImage(gdiFrameBitmap)

            Using morphBrush As New SolidBrush(morphColor)

                Using rahmenPen As New System.Drawing.Pen(System.Drawing.Color.DarkGray, 1.0F)

                    gfx.Clear(HintergrundFarbeSaver)

                    rahmenPen.Alignment = Drawing2D.PenAlignment.Inset

                    If aktuelleSettings.Modus = "Fade" Then

                        ZeichneFadeFrame(gfx, morphBrush, rahmenPen, progress,
                                         phasenDauer1, phasenDauer2, phasenDauer3)

                    Else

                        ZeichneCrossfadeFrame(gfx, progress)

                    End If

                End Using

            End Using

        End Using

        AktualisiereWriteableBitmap(gdiFrameBitmap, gdiFrameSource)

        RaiseEvent TransitionFrameIstFertig(gdiFrameSource)

        If progress >= 1.0 Then

            EndBildZeichnen()
            StopTransition()

        End If

    End Sub

    Private Sub ZeichneFadeFrame(
            gfx As Graphics,
            morphBrush As SolidBrush,
            rahmenPen As System.Drawing.Pen,
            progress As Double,
            phasenDauer1 As Single,
            phasenDauer2 As Single,
            phasenDauer3 As Single
        )
        'Zeichnet einen Frame des dreiphasigen Fade-Modus.

        Dim alpha As Double
        Dim morphT As Double
        Dim morphRect As Rectangle
        Dim colorMatrix As ColorMatrix

        If progress < phasenDauer1 Then

            alpha = 1.0 - progress / phasenDauer1

            colorMatrix = CreateAlphaColorMatrix(CSng(alpha))

            Using imageAttributes As New ImageAttributes()

                imageAttributes.SetColorMatrix(colorMatrix)

                If aktuelleSettings.Morphing Then

                    gfx.FillRectangle(morphBrush, drawRectOld)
                    gfx.DrawRectangle(rahmenPen, drawRectOld)

                End If

                gfx.DrawImage(oldImg, containerRect, 0, 0, oldImg.Width, oldImg.Height, GraphicsUnit.Pixel,
                              imageAttributes)

            End Using

        ElseIf progress < phasenDauer1 + phasenDauer2 Then

            If aktuelleSettings.Morphing Then

                morphT = (progress - phasenDauer1) / phasenDauer2
                morphRect = InterpolateRect(drawRectOld, drawRectNew, morphT)

                gfx.FillRectangle(morphBrush, morphRect)
                gfx.DrawRectangle(rahmenPen, morphRect)

            Else

                gfx.Clear(HintergrundFarbeSaver)

            End If

        Else

            alpha = (progress - (phasenDauer1 + phasenDauer2)) / phasenDauer3
            colorMatrix = CreateAlphaColorMatrix(CSng(alpha))

            Using imageAttributes As New ImageAttributes()

                imageAttributes.SetColorMatrix(colorMatrix)

                If aktuelleSettings.Morphing Then

                    gfx.FillRectangle(morphBrush, drawRectNew)
                    gfx.DrawRectangle(rahmenPen, drawRectNew)

                End If

                gfx.DrawImage(newImg, containerRect, 0, 0, newImg.Width, newImg.Height, GraphicsUnit.Pixel, imageAttributes)

            End Using

        End If

    End Sub

    Private Sub ZeichneCrossfadeFrame(gfx As Graphics, progress As Double)
        'Zeichnet einen Frame des Crossfade-Modus.

        Dim alphaOld As Double
        Dim alphaNew As Double

        alphaOld = 1.0 - progress
        alphaNew = progress

        Using imageAttributesOld As New ImageAttributes()

            imageAttributesOld.SetColorMatrix(CreateAlphaColorMatrix(CSng(alphaOld)))

            gfx.DrawImage(oldImg, containerRect, 0, 0, oldImg.Width, oldImg.Height, GraphicsUnit.Pixel,
                          imageAttributesOld)

        End Using

        Using imageAttributesNew As New ImageAttributes()

            imageAttributesNew.SetColorMatrix(CreateAlphaColorMatrix(CSng(alphaNew)))

            gfx.DrawImage(newImg, containerRect, 0, 0, newImg.Width, newImg.Height, GraphicsUnit.Pixel,
                          imageAttributesNew)

        End Using

    End Sub

    Private Sub GebeBildressourcenFrei()
        'Gibt alle für die Transition gehaltenen Bildressourcen frei.

        If oldImg IsNot Nothing Then

            oldImg.Dispose()
            oldImg = Nothing

        End If

        If newImg IsNot Nothing Then

            newImg.Dispose()
            newImg = Nothing

        End If

        oldRTB = Nothing
        newRTB = Nothing

    End Sub

    Private Function InterpolateRect(r1 As Rectangle, r2 As Rectangle, t As Double) As Rectangle
        Dim x As Integer = CInt(r1.X + (r2.X - r1.X) * t)
        Dim y As Integer = CInt(r1.Y + (r2.Y - r1.Y) * t)
        Dim w As Integer = CInt(r1.Width + (r2.Width - r1.Width) * t)
        Dim h As Integer = CInt(r1.Height + (r2.Height - r1.Height) * t)

        Return New Rectangle(x, y, w, h)

    End Function

    Private Function CreateAlphaColorMatrix(alpha As Single) As System.Drawing.Imaging.ColorMatrix
        Return New System.Drawing.Imaging.ColorMatrix(New Single()() {
        New Single() {1, 0, 0, 0, 0},
        New Single() {0, 1, 0, 0, 0},
        New Single() {0, 0, 1, 0, 0},
        New Single() {0, 0, 0, alpha, 0},
        New Single() {0, 0, 0, 0, 1}
    })
    End Function

    Private Function SetzeZufallsFarbe() As System.Drawing.Color
        'Sucht eine zufällige benannte Nicht-Systemfarbe aus.

        Dim bekannteFarben As Array
        Dim echteFarben As List(Of KnownColor)
        Dim farbName As KnownColor
        Dim zufallsFarbe As System.Drawing.Color

        bekannteFarben = [Enum].GetValues(GetType(KnownColor))

        echteFarben = bekannteFarben.
        Cast(Of KnownColor)().
        Where(
            Function(kc)

                Return Not System.Drawing.Color.FromKnownColor(kc).IsSystemColor

            End Function).
        ToList()

        farbName = echteFarben(rnd.Next(echteFarben.Count))

        zufallsFarbe = System.Drawing.Color.FromKnownColor(farbName)

        Return zufallsFarbe

    End Function

    Private Sub StartGDIRenderLoop(drawAction As Action(Of Windows.Size), size As Windows.Size)
        'Startet den GDI-Renderloop und legt die beiden
        'wiederverwendeten Frame-Puffer an.

        Dim pixelBreite As Integer
        Dim pixelHoehe As Integer

        StopGDIRenderLoop()
        GebeRenderressourcenFrei()

        pixelBreite = Math.Max(CInt(size.Width), 1)
        pixelHoehe = Math.Max(CInt(size.Height), 1)

        gdiFrameBitmap = New Bitmap(pixelBreite, pixelHoehe, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

        gdiFrameSource =
        New WriteableBitmap(
            pixelBreite,
            pixelHoehe,
            96.0,
            96.0,
            PixelFormats.Bgra32,
            Nothing)

        gdiDrawAction = drawAction
        gdiRenderSize = size
        gdiFrameTimer = New DispatcherTimer(DispatcherPriority.Render)

        AddHandler gdiFrameTimer.Tick, AddressOf OnGDIFrameTick

        gdiFrameTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0)

        gdiFrameTimer.Start()

    End Sub

    Private Sub StopGDIRenderLoop()
        'Beendet ausschließlich den laufenden DispatcherTimer.

        If gdiFrameTimer IsNot Nothing Then

            gdiFrameTimer.Stop()

            RemoveHandler gdiFrameTimer.Tick,
            AddressOf OnGDIFrameTick

            gdiFrameTimer = Nothing

        End If

    End Sub

    Private Sub OnGDIFrameTick(sender As Object, e As EventArgs)
        'Erzeugt den nächsten Frame des laufenden Renderloops
        'und überwacht eine optionale externe Maximaldauer.

        If Not transitionLaeuft Then

            StopGDIRenderLoop()

            Exit Sub

        End If

        If externeDauerInMS > 0 AndAlso laufzeit.Elapsed.TotalMilliseconds >= externeDauerInMS Then

            EndBildZeichnen()
            StopTransition()

            Exit Sub

        End If

        If gdiDrawAction Is Nothing Then
            Exit Sub
        End If

        gdiDrawAction.Invoke(gdiRenderSize)

    End Sub

    'Bereinigung & Dispose

    Private Sub BeendeUndBereinigeTransition()
        'Beendet sämtliche laufenden Arbeiten und gibt
        'alle von der Transition gehaltenen Ressourcen frei.

        transitionLaeuft = False

        laufzeit.Stop()

        StopGDIRenderLoop()

        GebeBildressourcenFrei()
        GebeRenderressourcenFrei()

        externeDauerInMS = 0

    End Sub

    Private Sub GebeRenderressourcenFrei()
        'Gibt sämtliche wiederverwendeten Renderressourcen frei.

        gdiDrawAction = Nothing

        If gdiFrameBitmap IsNot Nothing Then

            gdiFrameBitmap.Dispose()
            gdiFrameBitmap = Nothing

        End If

        gdiFrameSource = Nothing

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
