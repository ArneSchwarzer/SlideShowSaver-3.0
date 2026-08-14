Imports System.Diagnostics
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
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Imports SlideShowTools.SharedDataHandling


Namespace TransitionMain_SuW

    Public Class TransitionMain
        Implements ISlideShowTransition

#Region "Variablendeklaration, Structures & Enums ect."

        'Settings und Konstanten
        Public Const SLIDESHOWTRANSITION_SuW_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Schieben & Wischen\"
        Public Const nameTransition As String = "Schieben und Wischen"

        Private aktuelleSettings As New SlideShowTransitionSettings_SuW()

        'Zeitmanagement
        Private ReadOnly laufzeit As New Stopwatch()
        Private dauerInMS As Integer
        Private externeDauerInMS As Integer

        'Transitions-Bilder
        Private oldBmpGerahmt As RenderTargetBitmap
        Private newBmpGerahmt As RenderTargetBitmap

        'Animation und Positionen
        Private startPosOldWPF As System.Windows.Point
        Private zielPosOldWPF As System.Windows.Point
        Private startPosNewWPF As System.Windows.Point
        Private zielPosNewWPF As System.Windows.Point

        Private effektiverModus As String

        'Zielgröße
        Private sizeWinForms As System.Drawing.Size

        'Rendering
        Private frameTimer As DispatcherTimer

        Private frameSource As WriteableBitmap

        Private oldPixelBuffer() As Byte
        Private newPixelBuffer() As Byte
        Private hintergrundPixelBuffer() As Byte

        Private pixelStride As Integer

        'Lifecycle
        Private transitionLaeuft As Boolean
        Private wurdeBereinigt As Boolean

        'Sonstiges
        Private ReadOnly rnd As New Random()

        Public Structure SlideShowTransitionSettings_SuW

            Public geschwindigkeit As Integer
            Public richtungen As List(Of String)
            Public modus As String

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
        Public Event TransitionIsRunning(state As Boolean) Implements ISlideShowTransition.TransitionIsRunning
        Public Event TransitionFrameIstFertig(bitmap As ImageSource) Implements ISlideShowTransition.TransitionFrameIstFertig

        'Transition Ausführung
        Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As BitmapImage,
                                 picBoxModeNew As PictureBoxSizeMode, clientSize As System.Drawing.Size,
                                 Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition

            'Initialisiert und startet eine neue Schieben-&-Wischen-Transition.

            Dim dx As Integer
            Dim dy As Integer
            Dim richtungArray() As String
            Dim richtung As String

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

            If clientSize.Width <= 0 OrElse clientSize.Height <= 0 Then

                Throw New ArgumentOutOfRangeException(NameOf(clientSize))

            End If

            dx = 0
            dy = 0

            richtungArray = New String() {"N", "NO", "O", "SO", "S", "SW", "W", "NW"}

            Try

                sizeWinForms = clientSize

                ReadTransitionSettingsFromRegistryOrDefaults()

                StoreSettings(TransitionName, aktuelleSettings)

                oldBmpGerahmt = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
                newBmpGerahmt = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

                If oldBmpGerahmt Is Nothing OrElse newBmpGerahmt Is Nothing Then

                    Throw New InvalidOperationException("Die gerahmten Transitionsbilder konnten nicht " &
                                                        "erzeugt werden.")

                End If

                If aktuelleSettings.richtungen IsNot Nothing AndAlso
                        aktuelleSettings.richtungen.Count > 0 Then

                    richtung = aktuelleSettings.richtungen(rnd.Next(aktuelleSettings.richtungen.Count))

                Else

                    richtung = richtungArray(rnd.Next(richtungArray.Length))

                End If

                Select Case richtung

                    Case "N"

                        dy = -sizeWinForms.Height

                    Case "S"

                        dy = sizeWinForms.Height

                    Case "W"

                        dx = -sizeWinForms.Width

                    Case "O"

                        dx = sizeWinForms.Width

                    Case "NW"

                        dx = -sizeWinForms.Width
                        dy = -sizeWinForms.Height

                    Case "NO"

                        dx = sizeWinForms.Width
                        dy = -sizeWinForms.Height

                    Case "SW"

                        dx = -sizeWinForms.Width
                        dy = sizeWinForms.Height

                    Case "SO"

                        dx = sizeWinForms.Width
                        dy = sizeWinForms.Height

                End Select

                effektiverModus = aktuelleSettings.modus

                If effektiverModus = "Zufällig" Then

                    effektiverModus = If(rnd.Next(2) = 0, "Schieben", "Wischen")

                End If

                startPosOldWPF = New Windows.Point(0, 0)
                zielPosOldWPF = New Windows.Point(-dx, -dy)

                startPosNewWPF = New Windows.Point(dx, dy)
                zielPosNewWPF = New Windows.Point(0, 0)

                If effektiverModus = "Wischen" Then

                    zielPosOldWPF = New Windows.Point(0, 0)

                End If

                dauerInMS = Math.Max(1, aktuelleSettings.geschwindigkeit * 1000)
                externeDauerInMS = Math.Max(0, durationMs)

                InitialisiereRenderpuffer()

                laufzeit.Restart()

                transitionLaeuft = True

                StartRenderLoop()

                RaiseEvent TransitionIsRunning(True)

            Catch ex As Exception

                LogError("Transition Schieben & Wischen - RunTransition(): " & ex.ToString())

                BeendeUndBereinigeTransition()

                Throw

            End Try

        End Sub

        Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
            'Beendet eine aktive Transition kontrolliert.

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

            aktuelleSettings.geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", defaults))
            aktuelleSettings.richtungen = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", defaults))
            aktuelleSettings.modus = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", defaults)


        End Sub

        Friend Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
            Dim defaults As New Dictionary(Of String, String)

            defaults.Add("Geschwindigkeit", "20")
            defaults.Add("Richtungen", "W; O")
            defaults.Add("Modus", "Wischen")

            Return defaults

        End Function

        'Abbruch- und Endverwaltung
        Private Sub EndBildZeichnen()
            'Gibt garantiert das vollständige endgültige Zielbild aus.

            If newBmpGerahmt Is Nothing Then
                Exit Sub
            End If

            RaiseEvent TransitionFrameIstFertig(newBmpGerahmt)

        End Sub

        'Animation und rendern
        Private Sub DrawTransitionFrame()
            'Berechnet die aktuelle Position beider Bilder und schreibt
            'nur deren sichtbare Ausschnitte in den wiederverwendeten Frame.

            Dim progress As Double

            Dim oldX As Integer
            Dim oldY As Integer

            Dim newX As Integer
            Dim newY As Integer

            Dim fullRect As Int32Rect

            If frameSource Is Nothing Then
                Exit Sub
            End If

            progress = laufzeit.Elapsed.TotalMilliseconds / dauerInMS
            progress = Math.Min(progress, 1.0)

            oldX = CInt(Math.Round(startPosOldWPF.X + (zielPosOldWPF.X - startPosOldWPF.X) * progress))
            oldY = CInt(Math.Round(startPosOldWPF.Y + (zielPosOldWPF.Y - startPosOldWPF.Y) * progress))

            newX = CInt(Math.Round(startPosNewWPF.X + (zielPosNewWPF.X - startPosNewWPF.X) * progress))
            newY = CInt(Math.Round(startPosNewWPF.Y + (zielPosNewWPF.Y - startPosNewWPF.Y) * progress))

            If effektiverModus = "Wischen" Then

                oldX = 0
                oldY = 0

            End If

            fullRect = New Int32Rect(0, 0, sizeWinForms.Width, sizeWinForms.Height)

            'Frame auf Hintergrundfarbe zurücksetzen.
            frameSource.WritePixels(fullRect, hintergrundPixelBuffer, pixelStride, 0)

            'Altbild zuerst zeichnen.
            SchreibeBildMitOffset(oldPixelBuffer, oldX, oldY)

            'Neubild darüber zeichnen.
            SchreibeBildMitOffset(newPixelBuffer, newX, newY)

            RaiseEvent TransitionFrameIstFertig(frameSource)

            If progress >= 1.0 Then

                EndBildZeichnen()
                StopTransition()

            End If

        End Sub

        Private Sub SchreibeBildMitOffset(pixelBuffer() As Byte, offsetX As Integer, offsetY As Integer)
            'Schreibt den sichtbaren Ausschnitt eines Vollbildpuffers
            'an die gewünschte Zielposition.

            Dim sourceX As Integer
            Dim sourceY As Integer

            Dim zielX As Integer
            Dim zielY As Integer

            Dim breite As Integer
            Dim hoehe As Integer

            Dim sourceOffset As Integer
            Dim zielRect As Int32Rect

            If pixelBuffer Is Nothing OrElse frameSource Is Nothing Then

                Exit Sub

            End If

            sourceX = Math.Max(0, -offsetX)
            sourceY = Math.Max(0, -offsetY)

            zielX = Math.Max(0, offsetX)
            zielY = Math.Max(0, offsetY)

            breite = Math.Min(sizeWinForms.Width - sourceX, sizeWinForms.Width - zielX)
            hoehe = Math.Min(sizeWinForms.Height - sourceY, sizeWinForms.Height - zielY)

            If breite <= 0 OrElse hoehe <= 0 Then

                Exit Sub

            End If

            sourceOffset = sourceY * pixelStride + sourceX * 4
            zielRect = New Int32Rect(zielX, zielY, breite, hoehe)

            frameSource.WritePixels(zielRect, pixelBuffer, pixelStride, sourceOffset)

        End Sub

        Private Sub InitialisiereRenderpuffer()
            'Erzeugt alle während der Transition wiederverwendeten
            'Pixel- und Ausgabepuffer genau einmal.

            Dim pixelAnzahl As Integer
            Dim hintergrundFarbe As System.Drawing.Color

            pixelStride = sizeWinForms.Width * 4
            pixelAnzahl = pixelStride * sizeWinForms.Height

            ReDim oldPixelBuffer(pixelAnzahl - 1)
            ReDim newPixelBuffer(pixelAnzahl - 1)

            ReDim hintergrundPixelBuffer(pixelAnzahl - 1)

            oldBmpGerahmt.CopyPixels(oldPixelBuffer, pixelStride, 0)
            newBmpGerahmt.CopyPixels(newPixelBuffer, pixelStride, 0)

            hintergrundFarbe = HintergrundFarbeSaver

            For pixelOffset As Integer = 0 To pixelAnzahl - 1 Step 4

                hintergrundPixelBuffer(pixelOffset + 0) = hintergrundFarbe.B
                hintergrundPixelBuffer(pixelOffset + 1) = hintergrundFarbe.G
                hintergrundPixelBuffer(pixelOffset + 2) = hintergrundFarbe.R
                hintergrundPixelBuffer(pixelOffset + 3) = 255

            Next

            frameSource = New WriteableBitmap(sizeWinForms.Width, sizeWinForms.Height, 96.0, 96.0,
                                              PixelFormats.Pbgra32, Nothing)

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
            'Steuert Animation und optionale externe Notbremse.

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
            'Stoppt und trennt den Frame-Timer vollständig.

            If frameTimer Is Nothing Then
                Exit Sub
            End If

            frameTimer.Stop()

            RemoveHandler frameTimer.Tick, AddressOf OnFrameTick

            frameTimer = Nothing

        End Sub

        'Bereinigen und Dispose
        Private Sub BeendeUndBereinigeTransition()
            'Stoppt die Transition und löst sämtliche gehaltenen
            'Render- und Bildreferenzen.

            transitionLaeuft = False

            laufzeit.Stop()

            StopRenderLoop()

            oldBmpGerahmt = Nothing
            newBmpGerahmt = Nothing

            frameSource = Nothing

            oldPixelBuffer = Nothing
            newPixelBuffer = Nothing
            hintergrundPixelBuffer = Nothing

            pixelStride = 0

            effektiverModus = Nothing

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

End Namespace
