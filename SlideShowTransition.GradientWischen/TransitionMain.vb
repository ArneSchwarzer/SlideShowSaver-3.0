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
            Dim richtung As String

            RaiseEvent TransitionIsRunning(True)

            If oldImage Is Nothing OrElse newImage Is Nothing Then
                StopTransition()
            End If

            'Überführen der Parameter in Klassenvariablen

            'Bilder vorbereiten

            ReadTransitionSettingsFromRegistryOrDefaults()
            StoreSettings(TransitionName, aktuelleSettings)

            'Richtungsauswahl
            If aktuelleSettings.richtungen?.Count > 0 Then
                richtung = aktuelleSettings.richtungen(New Random().Next(aktuelleSettings.richtungen.Count))
            Else
                richtung = richtungArray(New Random().Next(10))
            End If

            'Animation initiieren

            ' Koordinaten berechnen
            Select Case richtung
                Case "N"
                Case "S"
                Case "W"
                Case "O"
                Case "NW"
                Case "NO"
                Case "SW"
                Case "SO"
                Case "ZIn"
                Case "ZOut"
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

            StartRenderLoop(AddressOf DrawTransitionFrame, New Windows.Size(sizeWPF.Width, sizeWPF.Height))

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

        'Animation und rendern
        Private Sub DrawTransitionFrame(dc As DrawingContext, size As System.Windows.Size)
            Dim elapsed As Double = (DateTime.Now - startTime).TotalMilliseconds
            Dim progress As Double = Math.Min(1.0, elapsed / dauerInMS)

            'Hier die Animation einfügen

            'TODO:
            'Maske generieren / Verschieben etc.

            ' Bilder zeichnen
            dc.DrawImage(oldBmpSource, New Rect(0, 0, sizeWPF.Width, sizeWPF.Height))
            dc.DrawImage(newBmpSource, New Rect(0, 0, sizeWPF.Width, sizeWPF.Height))

            ' Fertig?
            If progress >= 1.0 Then
                StopTransition()
            End If

        End Sub

        Private Sub LineareMaskeGenerieren()

        End Sub

        Private Sub RadialMaskeGenerieren()

        End Sub
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
