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

    'Timer und Zeitmanagement
    Private WithEvents tmrDuration As New Timer
    Private startTime As DateTime
    Private dauerInMS As Integer
    Private Const FPS As Integer = 120

    'Transitions-Bilder
    Private oldImg As Image
    Private newImg As Image
    Private oldBmpSource As BitmapSource
    Private newBmpSource As BitmapSource
    Private oldRTB As RenderTargetBitmap
    Private newRTB As RenderTargetBitmap
    Private oldSize As System.Drawing.Size
    Private newSize As System.Drawing.Size

    'Animation
    Private drawRectOld As Rectangle
    Private drawRectNew As Rectangle
    Private containerRect As Rectangle

    'Rendering
    Private Shared sizeWPF As Windows.Size
    Private Shared sizeWF As System.Drawing.Size
    Private gdiFrameTimer As DispatcherTimer
    Private gdiStartTime As DateTime
    Private gdiRenderSize As Windows.Size
    Private gdiDrawAction As Action(Of Windows.Size)
    Private requestEndOfTransition As Boolean = False

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

    'Eigenschaften
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

    Public Event TransitionIsRunning As ISlideShowTransition.TransitionIsRunningEventHandler Implements ISlideShowTransition.TransitionIsRunning
    Public Event TransitionFrameIstFertig As ISlideShowTransition.TransitionFrameIstFertigEventHandler Implements ISlideShowTransition.TransitionFrameIstFertig

    Public Sub RunTransition(
    oldImage As BitmapImage,
    picBoxModeOld As PictureBoxSizeMode,
    newImage As BitmapImage,
    picBoxModeNew As PictureBoxSizeMode,
    clientSize As System.Drawing.Size,
    Optional durationMs As Integer = 0
) Implements ISlideShowTransition.RunTransition
        'Bereitet die Animation vor und startet den Render-Loop.

        'Mögliche Ressourcen eines vorherigen Durchlaufs freigeben.
        StopGDIRenderLoop()
        GebeBildressourcenFrei()

        requestEndOfTransition = False

        ReadTransitionSettingsFromRegistryOrDefaults()
        StoreSettings(TransitionName, aktuelleSettings)

        RaiseEvent TransitionIsRunning(True)

        sizeWPF = New Windows.Size(clientSize.Width, clientSize.Height)
        sizeWF = clientSize

        'Gerahmte WPF-Bilder erzeugen.
        oldRTB = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
        newRTB = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

        oldBmpSource = oldRTB
        newBmpSource = newRTB

        'Nur diese beiden GDI-Bilder werden tatsächlich zum Zeichnen benötigt.
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

        If durationMs > 0 Then

            If tmrDuration Is Nothing Then
                tmrDuration = New Timer()
            End If

            tmrDuration.Interval = durationMs
            tmrDuration.Start()

        End If

        dauerInMS = 1000 * aktuelleSettings.Geschwindigkeit
        startTime = DateTime.Now

        StartGDIRenderLoop(AddressOf DrawGDITransitionFrame, dauerInMS, sizeWPF)

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Beendet Renderloop und Timer und gibt sämtliche Bildressourcen frei.

        requestEndOfTransition = True

        StopGDIRenderLoop()

        If tmrDuration IsNot Nothing Then

            tmrDuration.Stop()
            tmrDuration.Dispose()
            tmrDuration = Nothing

        End If

        gdiDrawAction = Nothing

        GebeBildressourcenFrei()

        RaiseEvent TransitionIsRunning(False)

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
    Sub ReadTransitionSettingsFromRegistryOrDefaults()
        'Setzt aktuelleTransitonSettings mit den Werten aus der Registry oder mit Defaultwerten.

        Dim defaults As Dictionary(Of String, String) = GetTransitionDefaultSettings()
        Dim colorString As String

        colorString = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Farbton", defaults)
        If String.IsNullOrWhiteSpace(colorString) Then
            colorString = defaults("Farbton")
        End If

        aktuelleSettings.Farbton = ColorHandling.StringToColor(colorString)

        If ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Zufallsfarbe", defaults) = "True" Then
            aktuelleSettings.Zufallsfarbe = True
        Else
            aktuelleSettings.Zufallsfarbe = False
        End If

        If ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Morphing", defaults) = "True" Then
            aktuelleSettings.Morphing = True
        Else
            aktuelleSettings.Morphing = False
        End If

        aktuelleSettings.Modus = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Modus", defaults)
        aktuelleSettings.Geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_FADECROSSFADE_FULLPATH & "Geschwindigkeit", defaults))

    End Sub

    Public Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        Dim defaults As New Dictionary(Of String, String)

        defaults.Add("Farbton", "0, 0, 0, 255")
        defaults.Add("Zufallsfarbe", "False")
        defaults.Add("Morphing", "True")
        defaults.Add("Modus", "Fade")
        defaults.Add("Geschwindigkeit", "20")

        Return defaults

    End Function

    'Abbruch- und Endverwaltung
    Public Sub EndBildZeichnen()
        'Gibt das Endbild aus

        RaiseEvent TransitionFrameIstFertig(newRTB)

    End Sub

    Sub tmrDuration_Tick() Handles tmrDuration.Tick
        'Bricht die Transition nach Ende von DurationMS ab.

        'Zum Schluss noch einmal die aufrufende targetGraphics aktualisieren
        EndBildZeichnen()

        StopTransition()

    End Sub

    'Animation und rendern 
    Private Sub DrawGDITransitionFrame(size As Windows.Size)
        'Zeichnet einen einzelnen GDI-Frame und gibt sämtliche temporären
        'GDI-Ressourcen unmittelbar nach der Konvertierung wieder frei.

        Dim phasenDauer1 As Single
        Dim phasenDauer2 As Single
        Dim phasenDauer3 As Single
        Dim morphColor As System.Drawing.Color
        Dim progress As Double
        Dim frameBitmap As Bitmap
        Dim frameBitmapSource As RenderTargetBitmap

        morphColor = System.Drawing.Color.FromArgb(255, aktuelleSettings.Farbton.R, aktuelleSettings.Farbton.G,
                                                   aktuelleSettings.Farbton.B)

        progress = (DateTime.Now - startTime).TotalMilliseconds / dauerInMS
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

        frameBitmap = Nothing
        frameBitmapSource = Nothing

        Try

            frameBitmap = New Bitmap(CInt(size.Width), CInt(size.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            Using gfx As Graphics = Graphics.FromImage(frameBitmap)

                Using morphBrush As New SolidBrush(morphColor)

                    Using rahmenPen As New System.Drawing.Pen(System.Drawing.Color.DarkGray, 1.0F)

                        gfx.Clear(HintergrundFarbeSaver)

                        rahmenPen.Alignment = Drawing2D.PenAlignment.Inset

                        If aktuelleSettings.Modus = "Fade" Then

                            ZeichneFadeFrame(gfx, morphBrush, rahmenPen, progress, phasenDauer1, phasenDauer2, phasenDauer3)

                        Else

                            ZeichneCrossfadeFrame(gfx, progress)

                        End If

                    End Using

                End Using

            End Using

            frameBitmapSource = ConvertImageToRenderTargetBitmap(frameBitmap)

            RaiseEvent TransitionFrameIstFertig(frameBitmapSource)

        Finally

            If frameBitmap IsNot Nothing Then

                frameBitmap.Dispose()
                frameBitmap = Nothing

            End If

        End Try

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

        oldBmpSource = Nothing
        newBmpSource = Nothing
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
        'Sucht eine zufällige Farbe aus der Liste der benannten Farben (Systemfarben werden ignoriert)

        Dim zufallsFarbe As System.Drawing.Color
        Dim rnd As New Random()
        Dim knownColors = [Enum].GetValues(GetType(KnownColor))
        Dim echteFarben = knownColors.Cast(Of KnownColor)().
                Where(Function(kc) Not System.Drawing.Color.FromKnownColor(kc).IsSystemColor).ToList()
        Dim colorName = echteFarben(rnd.Next(echteFarben.Count))

        zufallsFarbe = System.Drawing.Color.FromKnownColor(colorName)

        Return zufallsFarbe

    End Function

    Public Sub StartGDIRenderLoop(drawAction As Action(Of Windows.Size), dauerInMS As Integer, size As Windows.Size)
        'Startet den GDI-Renderloop.

        StopGDIRenderLoop()

        requestEndOfTransition = False
        gdiDrawAction = drawAction
        gdiRenderSize = size
        gdiStartTime = DateTime.Now

        gdiFrameTimer = New DispatcherTimer()

        AddHandler gdiFrameTimer.Tick, AddressOf OnGDIFrameTick

        gdiFrameTimer.Interval = TimeSpan.FromMilliseconds(33)
        gdiFrameTimer.Start()

    End Sub

    Public Sub StopGDIRenderLoop()
        If gdiFrameTimer IsNot Nothing Then
            gdiFrameTimer.Stop()
            RemoveHandler gdiFrameTimer.Tick, AddressOf OnGDIFrameTick
            gdiFrameTimer = Nothing
        End If
    End Sub

    Private Sub OnGDIFrameTick(sender As Object, e As EventArgs)
        If gdiDrawAction IsNot Nothing Then
            gdiDrawAction.Invoke(gdiRenderSize)
        End If

        If requestEndOfTransition Then
            StopGDIRenderLoop()
        End If

    End Sub

End Class
