Imports System.Diagnostics
Imports System.Windows.Media.Imaging
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
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

Public Class TransitionMain
    Implements ISlideShowTransition

    'Variablendeklaration

    'Settings und Konstanten
    Public Const SLIDESHOWTRANSITION_ZOOM_FULLPATH As String =
    SLIDESHOWTRANSITION_PATH & "Zoom\"

    Public Const nameTransition As String =
    "Zoom"

    Private aktuelleSettings As SlideShowTransitionSettings_Zoom

    'Zeitmanagement
    Private ReadOnly laufzeit As New Stopwatch()
    Private dauerInMS As Integer
    Private externeDauerInMS As Integer

    'Transitions-Bilder
    Private oldBmp As Bitmap
    Private newBmp As Bitmap

    Private oldBmpGerahmt As RenderTargetBitmap
    Private newBmpGerahmt As RenderTargetBitmap

    'Animation
    Private aktuellerAnkerpunktOld As String
    Private aktuellerAnkerpunktNew As String

    Private clientSize As System.Drawing.Size

    'Rendering
    Private frameTimer As DispatcherTimer
    Private frameSource As WriteableBitmap

    'Lifecycle
    Private transitionLaeuft As Boolean
    Private wurdeBereinigt As Boolean

    'Sonstiges
    Private ReadOnly rnd As New Random()

    Public Structure SlideShowTransitionSettings_Zoom
        Public ankerpunkte As List(Of String)
        Public geschwindigkeit As Integer
        Public gleicherAnkerpunkt As Boolean
    End Structure

    'Eigenschaften
    Public ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName
        Get
            Return nameTransition
        End Get
    End Property

    Public ReadOnly Property TransitionKurzBeschreibung As String Implements ISlideShowTransition.TransitionKurzBeschreibung
        Get
            Return "Schrumpft das alte Bild und lässt das neue wachsen."
        End Get
    End Property

    Public ReadOnly Property TransitionVersion As Version Implements ISlideShowTransition.TransitionVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    'Events
    Public Event TransitionIsRunning As ISlideShowTransition.TransitionIsRunningEventHandler Implements ISlideShowTransition.TransitionIsRunning
    Public Event TransitionFrameIstFertig As ISlideShowTransition.TransitionFrameIstFertigEventHandler Implements ISlideShowTransition.TransitionFrameIstFertig

    'Start, Stop & OptionsDialog
    Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As BitmapImage,
                             picBoxModeNew As PictureBoxSizeMode, clientSizeInput As System.Drawing.Size,
                             Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition
        'Initialisiert und startet eine neue Zoom-Transition.

        Dim ankerArray() As String

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

        If clientSizeInput.Width <= 0 OrElse clientSizeInput.Height <= 0 Then

            Throw New ArgumentOutOfRangeException(NameOf(clientSizeInput))

        End If

        ankerArray = New String() {"N", "NO", "O", "SO", "S", "SW", "W", "NW", "Z"}

        Try

            clientSize = clientSizeInput

            ReadTransitionSettingsFromRegistryOrDefaults()

            StoreSettings(nameTransition, aktuelleSettings)

            oldBmpGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
            newBmpGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

            oldBmp = ConvertRenderTargetBitmapToBitmap(oldBmpGerahmt)
            newBmp = ConvertRenderTargetBitmapToBitmap(newBmpGerahmt)

            If oldBmp Is Nothing OrElse newBmp Is Nothing Then

                Throw New InvalidOperationException("Die GDI-Quellbilder der Zoom-Transition konnten nicht " &
                                                    "erzeugt werden.")

            End If

            If oldBmpGerahmt Is Nothing OrElse newBmpGerahmt Is Nothing Then

                Throw New InvalidOperationException("Die gerahmten Transitionsbilder konnten nicht erzeugt " &
                                                    "werden.")

            End If

            If aktuelleSettings.ankerpunkte IsNot Nothing AndAlso aktuelleSettings.ankerpunkte.Count > 0 Then

                aktuellerAnkerpunktOld = aktuelleSettings.ankerpunkte(rnd.Next(aktuelleSettings.ankerpunkte.Count))

            Else

                aktuellerAnkerpunktOld = ankerArray(rnd.Next(ankerArray.Length))

            End If

            If aktuelleSettings.gleicherAnkerpunkt Then

                aktuellerAnkerpunktNew = aktuellerAnkerpunktOld

            ElseIf aktuelleSettings.ankerpunkte IsNot Nothing AndAlso aktuelleSettings.ankerpunkte.Count > 0 Then

                aktuellerAnkerpunktNew = aktuelleSettings.ankerpunkte(rnd.Next(aktuelleSettings.ankerpunkte.Count))

            Else

                aktuellerAnkerpunktNew = ankerArray(rnd.Next(ankerArray.Length))

            End If

            dauerInMS = Math.Max(1, aktuelleSettings.geschwindigkeit * 1000)
            externeDauerInMS = Math.Max(0, durationMs)

            InitialisiereRenderpuffer()

            laufzeit.Restart()

            transitionLaeuft = True

            StartRenderLoop()

            RaiseEvent TransitionIsRunning(True)

        Catch ex As Exception

            LogError("Transition Zoom - RunTransition(): " & ex.ToString())

            BeendeUndBereinigeTransition()

            Throw

        End Try

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Beendet eine aktive Zoom-Transition kontrolliert.

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
        'Setzt aktuelleTransitonSettings mit den Werten aus der Registry oder mit Defaultwerten.

        Dim defaults As Dictionary(Of String, String) = GetTransitionDefaultSettings()

        aktuelleSettings.geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Geschwindigkeit", defaults))
        aktuelleSettings.ankerpunkte = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "Ankerpunkte", defaults))
        If ReadFromRegOrDefaults(SLIDESHOWTRANSITION_ZOOM_FULLPATH & "GleicherAnkerpunkt", defaults) = "True" Then
            aktuelleSettings.gleicherAnkerpunkt = True
        Else
            aktuelleSettings.gleicherAnkerpunkt = False
        End If

    End Sub

    Friend Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        Dim defaults As New Dictionary(Of String, String)

        defaults.Add("Geschwindigkeit", "10")
        defaults.Add("Ankerpunkte", "Z")
        defaults.Add("GleicherAnkerpunkt", "True")

        Return defaults

    End Function

    'Abbruch- und Endverwaltung
    Private Sub EndBildZeichnen()
        'Gibt garantiert das vollständige Zielbild aus.

        If newBmpGerahmt Is Nothing Then
            Exit Sub
        End If

        RaiseEvent TransitionFrameIstFertig(newBmpGerahmt)

    End Sub

    'Animation und rendern
    Private Sub DrawTransitionFrame()
        'Zeichnet einen einzelnen Zoom-Frame direkt
        'in den wiederverwendeten WPF-Renderpuffer.

        Dim progress As Double
        Dim faktor As Double

        Dim bildBreite As Double
        Dim bildHoehe As Double

        Dim bildPosition As Windows.Point
        Dim zielRect As RectangleF

        Dim quellBild As Bitmap
        Dim ankerpunkt As String

        If Not transitionLaeuft OrElse frameSource Is Nothing OrElse oldBmp Is Nothing OrElse newBmp Is Nothing Then
            Exit Sub
        End If

        progress = Math.Min(1.0, laufzeit.Elapsed.TotalMilliseconds / dauerInMS)

        If progress >= 1.0 Then

            EndBildZeichnen()
            StopTransition()

            Exit Sub

        End If

        If progress <= 0.5 Then

            faktor = 1.0 - progress / 0.5
            quellBild = oldBmp
            ankerpunkt = aktuellerAnkerpunktOld

        Else

            faktor = (progress - 0.5) / 0.5
            quellBild = newBmp
            ankerpunkt = aktuellerAnkerpunktNew

        End If

        bildBreite = Math.Max(1.0, clientSize.Width * faktor)
        bildHoehe = Math.Max(1.0, clientSize.Height * faktor)

        bildPosition = BerechneAnkerpunkt(ankerpunkt, bildBreite, bildHoehe)

        zielRect = New RectangleF(CSng(bildPosition.X), CSng(bildPosition.Y), CSng(bildBreite), CSng(bildHoehe))

        ZeichneDirektInFrameSource(quellBild, zielRect)

        RaiseEvent TransitionFrameIstFertig(frameSource)

    End Sub

    Private Sub InitialisiereRenderpuffer()
        'Erzeugt den einzigen wiederverwendeten Ausgabepuffer.
        'Die Transition zeichnet direkt in dessen BackBuffer.

        frameSource = New WriteableBitmap(clientSize.Width, clientSize.Height, 96.0, 96.0, PixelFormats.Pbgra32,
                                          Nothing)

    End Sub

    Private Sub StartRenderLoop()
        'Startet den einzigen Frame-Timer der Transition.

        StopRenderLoop()

        frameTimer = New DispatcherTimer(DispatcherPriority.Render)

        AddHandler frameTimer.Tick, AddressOf OnFrameTick

        frameTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0)

        frameTimer.Start()

    End Sub

    Private Sub OnFrameTick(sender As Object, e As EventArgs)
        'Steuert die zeitliche Ausführung der Transition.

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

    Private Sub ZeichneDirektInFrameSource(quellBild As Bitmap, zielRect As RectangleF)
        'Skaliert das aktuelle Bild unmittelbar in den
        'WPF-BackBuffer, ohne einen Zwischenframe zu erzeugen.

        Dim backBufferBitmap As Bitmap
        Dim graphics As Graphics

        backBufferBitmap = Nothing
        graphics = Nothing

        If frameSource Is Nothing OrElse quellBild Is Nothing Then
            Exit Sub
        End If

        frameSource.Lock()

        Try

            backBufferBitmap =
            New Bitmap(
                frameSource.PixelWidth,
                frameSource.PixelHeight,
                frameSource.BackBufferStride,
                Imaging.PixelFormat.Format32bppPArgb,
                frameSource.BackBuffer)

            graphics = Graphics.FromImage(backBufferBitmap)
            graphics.CompositingMode = CompositingMode.SourceCopy
            graphics.CompositingQuality = CompositingQuality.HighSpeed
            graphics.InterpolationMode = InterpolationMode.Low
            graphics.SmoothingMode = SmoothingMode.None
            graphics.Clear(HintergrundFarbeSaver)
            graphics.DrawImage(quellBild, zielRect)

            frameSource.AddDirtyRect(New Int32Rect(0, 0, frameSource.PixelWidth, frameSource.PixelHeight))

        Finally

            If graphics IsNot Nothing Then

                graphics.Dispose()
                graphics = Nothing

            End If

            If backBufferBitmap IsNot Nothing Then

                backBufferBitmap.Dispose()
                backBufferBitmap = Nothing

            End If

            frameSource.Unlock()

        End Try

    End Sub

    Private Sub StopRenderLoop()
        'Stoppt den Frame-Timer und trennt dessen Eventhandler.

        If frameTimer Is Nothing Then
            Exit Sub
        End If

        frameTimer.Stop()

        RemoveHandler frameTimer.Tick, AddressOf OnFrameTick

        frameTimer = Nothing

    End Sub

    'Hilfsfunktionen
    Private Function BerechneAnkerpunkt(ankerpunkt As String, bildBreite As Double, bildHoehe As Double) _
        As Windows.Point

        'Berechnet die linke obere Position eines skalierten Bildes anhand des gewählten Ankerpunktes.

        Dim x As Double
        Dim y As Double

        x = 0.0F
        y = 0.0F

        Select Case ankerpunkt

            Case "NW"

                x = 0.0F
                y = 0.0F

            Case "N"

                x = (clientSize.Width - bildBreite) / 2.0F
                y = 0.0F

            Case "NO"

                x = clientSize.Width - bildBreite
                y = 0.0F

            Case "O"

                x = clientSize.Width - bildBreite
                y = (clientSize.Height - bildHoehe) / 2.0F

            Case "SO"

                x = clientSize.Width - bildBreite
                y = clientSize.Height - bildHoehe

            Case "S"

                x = (clientSize.Width - bildBreite) / 2.0F
                y = clientSize.Height - bildHoehe

            Case "SW"

                x = 0.0F
                y = clientSize.Height - bildHoehe

            Case "W"

                x = 0.0F
                y = (clientSize.Height - bildHoehe) / 2.0F

            Case "Z"

                x = (clientSize.Width - bildBreite) / 2.0F
                y = (clientSize.Height - bildHoehe) / 2.0F

        End Select

        Return New Windows.Point(x, y)

    End Function

    'Bereinigen und Dispose
    Private Sub BeendeUndBereinigeTransition()
        'Stoppt die Transition und gibt sämtliche gehaltenen
        'Render- und Bildressourcen frei.

        transitionLaeuft = False

        laufzeit.Stop()

        StopRenderLoop()

        frameSource = Nothing

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

        aktuellerAnkerpunktOld = Nothing
        aktuellerAnkerpunktNew = Nothing

        externeDauerInMS = 0

    End Sub

#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
        'Gibt sämtliche Ressourcen dieser Transition endgültig frei.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        BeendeUndBereinigeTransition()

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class
