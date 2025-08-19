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

    Public Sub RunTransition(oldImage As Windows.Media.Imaging.BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As Windows.Media.Imaging.BitmapImage, picBoxModeNew As PictureBoxSizeMode, clientSize As System.Drawing.Size, Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition
        'Bereitet die Animation vor und startet den Render-Loop

        'Interne Initialisierungen & Formalitäten
        ReadTransitionSettingsFromRegistryOrDefaults()
        StoreSettings(TransitionName, aktuelleSettings)

        RaiseEvent TransitionIsRunning(True)

        'Konvertieren und in lokalen Variablen speichern
        sizeWPF = New Windows.Size(clientSize.Width, clientSize.Height)
        sizeWF = clientSize

        'Bilder vorbereiten (ein ewiges hin- und herkonvertieren...)
        oldImg = ConvertBitmapImageToImage(oldImage)
        newImg = ConvertBitmapImageToImage(newImage)
        oldRTB = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
        newRTB = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)
        oldBmpSource = CType(oldRTB, ImageSource)
        newBmpSource = CType(newRTB, ImageSource)
        oldImg = ConvertRenderTargetBitmapToBitmap(oldRTB)
        newImg = ConvertRenderTargetBitmapToBitmap(newRTB)

        'Bilder Sizes festlegen
        oldSize = New System.Drawing.Size(oldImage.PixelWidth, oldImage.PixelHeight)
        newSize = New System.Drawing.Size(newImage.PixelWidth, newImage.PixelHeight)

        'Rechtecke vorbereiten (sichtbare Bildbereiche im Client)
        containerRect = New Rectangle(0, 0, sizeWF.Width, sizeWF.Height)
        drawRectOld = GetDrawRectangle(oldSize, containerRect, picBoxModeOld)
        drawRectNew = GetDrawRectangle(newSize, containerRect, picBoxModeNew)

        'Moduls "Zufall" auflösen
        If aktuelleSettings.Modus = "Zufall" Then
            If rnd.Next(2) > 0 Then
                aktuelleSettings.Modus = "Fade"
            Else
                aktuelleSettings.Modus = "Crossfade"
            End If
        End If

        'Ggf. Zufalls-Farbe setzen
        If aktuelleSettings.Zufallsfarbe Then
            aktuelleSettings.Farbton = SetzeZufallsFarbe()
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
        dauerInMS = 1000 * aktuelleSettings.Geschwindigkeit
        startTime = DateTime.Now
        ' Basisdaten sichern

        ' GDI-Renderloop starten
        StartGDIRenderLoop(AddressOf DrawGDITransitionFrame, dauerInMS, sizeWPF)

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Aufräumen und Transition beenden.

        If tmrDuration IsNot Nothing Then
            tmrDuration.Stop()
            tmrDuration.Dispose()
            tmrDuration = Nothing
        End If

        requestEndOfTransition = True

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
        Dim phasenDauer1 As Single
        Dim phasenDauer2 As Single
        Dim phasenDauer3 As Single

        Dim morphColor As System.Drawing.Color = System.Drawing.Color.FromArgb(255, aktuelleSettings.Farbton.R, aktuelleSettings.Farbton.G, aktuelleSettings.Farbton.B)
        Dim b As New SolidBrush(morphColor)
        Dim p As New System.Drawing.Pen(System.Drawing.Color.DarkGray, 1)

        Dim bmp As New Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim gfx As Graphics = Graphics.FromImage(bmp)
        gfx.Clear(HintergrundFarbeSaver)

        Dim progress As Double = (DateTime.Now - startTime).TotalMilliseconds / dauerInMS
        progress = Math.Min(progress, 1.0)

        'Phasendauern definieren
        If aktuelleSettings.Morphing Then
            phasenDauer1 = 0.33
            phasenDauer2 = 0.33
            phasenDauer3 = 0.34
        Else
            phasenDauer1 = 0.495
            phasenDauer2 = 0.1
            phasenDauer3 = 0.495
        End If

        If aktuelleSettings.Modus = "Fade" Then
            If progress < phasenDauer1 Then
                ' Phase 1: Altes Bild ausblenden
                Dim alpha As Double = 1.0 - (progress / phasenDauer1)
                Dim cm As System.Drawing.Imaging.ColorMatrix = CreateAlphaColorMatrix(alpha)

                Using ia As New System.Drawing.Imaging.ImageAttributes()
                    ia.SetColorMatrix(cm)

                    If aktuelleSettings.Morphing Then
                        gfx.FillRectangle(New SolidBrush(aktuelleSettings.Farbton), drawRectOld)
                        p.Alignment = Drawing2D.PenAlignment.Inset
                        gfx.DrawRectangle(p, drawRectOld)
                    End If

                    gfx.DrawImage(oldImg, containerRect, 0, 0, oldImg.Width, oldImg.Height, GraphicsUnit.Pixel, ia)
                End Using

            ElseIf progress < (phasenDauer1 + phasenDauer2) Then
                ' Phase 2: Farbfläche morphend (wenn aktiviert)
                If aktuelleSettings.Morphing Then
                    Dim morphT As Double = (progress - phasenDauer1) / phasenDauer2
                    Dim rect As Rectangle = InterpolateRect(drawRectOld, drawRectNew, morphT)

                    gfx.FillRectangle(b, rect)
                    p.Alignment = Drawing2D.PenAlignment.Inset
                    gfx.DrawRectangle(p, rect)

                Else
                    gfx.Clear(HintergrundFarbeSaver)
                End If

            Else
                ' Phase 3: Neues Bild einblenden
                Dim alpha As Double = ((progress - (phasenDauer1 + phasenDauer2)) / phasenDauer3)
                Dim cm As System.Drawing.Imaging.ColorMatrix = CreateAlphaColorMatrix(alpha)
                Using ia As New System.Drawing.Imaging.ImageAttributes()
                    ia.SetColorMatrix(cm)

                    If aktuelleSettings.Morphing Then
                        gfx.FillRectangle(New SolidBrush(aktuelleSettings.Farbton), drawRectNew)
                        p.Alignment = Drawing2D.PenAlignment.Inset
                        gfx.DrawRectangle(p, drawRectNew)

                    End If

                    gfx.DrawImage(newImg, containerRect, 0, 0, newImg.Width, newImg.Height, GraphicsUnit.Pixel, ia)
                End Using
            End If

        Else
            ' Crossfade-Modus
            Dim alphaOld As Double = 1.0 - progress
            Dim alphaNew As Double = progress

            Using iaOld As New System.Drawing.Imaging.ImageAttributes()
                iaOld.SetColorMatrix(CreateAlphaColorMatrix(alphaOld))
                gfx.DrawImage(oldImg, containerRect, 0, 0, oldImg.Width, oldImg.Height, GraphicsUnit.Pixel, iaOld)
            End Using

            Using iaNew As New System.Drawing.Imaging.ImageAttributes()
                iaNew.SetColorMatrix(CreateAlphaColorMatrix(alphaNew))
                gfx.DrawImage(newImg, containerRect, 0, 0, newImg.Width, newImg.Height, GraphicsUnit.Pixel, iaNew)
            End Using
        End If

        gfx.Dispose()

        Dim rtb As RenderTargetBitmap = ConvertImageToRenderTargetBitmap(bmp)
        RaiseEvent TransitionFrameIstFertig(rtb)

        'Abbruchkriterium
        If progress >= 1.0 Then
            EndBildZeichnen()
            StopTransition()
        End If

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
        StopGDIRenderLoop() ' Falls noch läuft

        gdiDrawAction = drawAction
        gdiRenderSize = size
        gdiStartTime = DateTime.Now

        gdiFrameTimer = New DispatcherTimer()
        AddHandler gdiFrameTimer.Tick, AddressOf OnGDIFrameTick
        gdiFrameTimer.Interval = TimeSpan.FromMilliseconds(33) ' ca. 30 FPS
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
