Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.GraphicsSizeModeHandling
Imports System.Drawing.Drawing2D

Public Class TransitionMain
    Implements ISlideShowTransition

    '--- Variablen, Enums und Structures ---

    Public Const SLIDESHOWTRANSITION_SuW_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Schieben & Wischen\"

    Private WithEvents tmrDuration As New Timer
    Private WithEvents tmrAnimation As New Timer
    Private aktuelleTransitionSettings As New SlideShowTransition_SuW_Settings

    Private oldImage As Image
    Private newImage As Image
    Private oldPicBoxSizeMode As PictureBoxSizeMode
    Private newPicBoxSizeMode As PictureBoxSizeMode
    Private targetGrapics As Graphics
    Private bewegungOldImage As BewegungsInfos
    Private bewegungNewImage As BewegungsInfos

    Private clientSize As Size
    Private drawRectOld As Rectangle
    Private drawRectNew As Rectangle
    Private targetRect As Rectangle
    Private bmp As Bitmap
    Private offsetX As Double
    Private offsetY As Double

    Public Structure SlideShowTransition_SuW_Settings
        Public geschwindigkeit As Integer
        Public richtungen As List(Of String)
        Public modus As String
    End Structure

    Private Structure BewegungsInfos
        Public aktuellePositionX As Integer
        Public aktuellePositionY As Integer
        Public directionX As Integer
        Public directionY As Integer
    End Structure

    ' --- Eigenschaften ---
    ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName
        Get
            Return "Schieben & Wischen"
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

    ' --- Events ---
    Event TransitionIsRunning(state As Boolean) Implements ISlideShowTransition.TransitionIsRunning
    Event PleaseChangeToShader(shaderName As String) Implements ISlideShowTransition.PleaseChangeToShader

    ' --- Ausführung ---
    Sub RunTransition(oldImage As Image, picBoxModeOld As PictureBoxSizeMode, newImage As Image, picBoxModeNew As PictureBoxSizeMode, targetGraphics As Graphics, Optional clientSize As Size = Nothing, Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition
        Dim rnd As New Random
        Dim richtung As String
        Dim würfel1D2 As Integer

        RaiseEvent TransitionIsRunning(True)

        'Parameter in interne Variablen überführen
        Me.oldImage = oldImage
        Me.newImage = newImage
        Me.oldPicBoxSizeMode = picBoxModeOld
        Me.newPicBoxSizeMode = picBoxModeNew
        Me.targetGrapics = targetGraphics
        Me.clientSize = clientSize

        'Aktuelle Settings abholen
        GetCurrentTransitionSettings()

        'Rictung für die Transition aussuchen und Bewegungsinfos setzen (und dabei ein paar if-then sparen... ;-)
        If aktuelleTransitionSettings.richtungen.Count = 0 Then
            aktuelleTransitionSettings.richtungen.Add("NW")
            aktuelleTransitionSettings.richtungen.Add("N")
            aktuelleTransitionSettings.richtungen.Add("NO")
            aktuelleTransitionSettings.richtungen.Add("O")
            aktuelleTransitionSettings.richtungen.Add("SO")
            aktuelleTransitionSettings.richtungen.Add("S")
            aktuelleTransitionSettings.richtungen.Add("SW")
            aktuelleTransitionSettings.richtungen.Add("W")
        End If

        richtung = aktuelleTransitionSettings.richtungen(rnd.Next(aktuelleTransitionSettings.richtungen.Count))

        Select Case richtung
            Case "NW"
                bewegungNewImage.aktuellePositionX = -newImage.Width
                bewegungNewImage.aktuellePositionY = -newImage.Height
                bewegungNewImage.directionX = 1
                bewegungNewImage.directionY = 1

                bewegungOldImage.directionX = 1
                bewegungOldImage.directionY = 1
            Case "N"
                bewegungNewImage.aktuellePositionX = 0
                bewegungNewImage.aktuellePositionY = -newImage.Height
                bewegungNewImage.directionX = 0
                bewegungNewImage.directionY = 1

                bewegungOldImage.directionX = 0
                bewegungOldImage.directionY = 1
            Case "NO"
                bewegungNewImage.aktuellePositionX = oldImage.Width
                bewegungNewImage.aktuellePositionY = -newImage.Height
                bewegungNewImage.directionX = -1
                bewegungNewImage.directionY = 1

                bewegungOldImage.directionX = -1
                bewegungOldImage.directionY = 1
            Case "O"
                bewegungNewImage.aktuellePositionX = oldImage.Width
                bewegungNewImage.aktuellePositionY = 0
                bewegungNewImage.directionX = -1
                bewegungNewImage.directionY = 0

                bewegungOldImage.directionX = -1
                bewegungOldImage.directionY = 0
            Case "SO"
                bewegungNewImage.aktuellePositionX = oldImage.Width
                bewegungNewImage.aktuellePositionY = oldImage.Height
                bewegungNewImage.directionX = -1
                bewegungNewImage.directionY = -1

                bewegungOldImage.directionX = -1
                bewegungOldImage.directionY = -1
            Case "S"
                bewegungNewImage.aktuellePositionX = 0
                bewegungNewImage.aktuellePositionY = oldImage.Height
                bewegungNewImage.directionX = 0
                bewegungNewImage.directionY = -1

                bewegungOldImage.directionX = 0
                bewegungOldImage.directionY = -1
            Case "SW"
                bewegungNewImage.aktuellePositionX = -newImage.Width
                bewegungNewImage.aktuellePositionY = oldImage.Height
                bewegungNewImage.directionX = 1
                bewegungNewImage.directionY = -1

                bewegungOldImage.directionX = 1
                bewegungOldImage.directionY = -1
            Case "W"
                bewegungNewImage.aktuellePositionX = -newImage.Width
                bewegungNewImage.aktuellePositionY = 0
                bewegungNewImage.directionX = 1
                bewegungNewImage.directionY = 0

                bewegungOldImage.directionX = 1
                bewegungOldImage.directionY = 0
        End Select

        bewegungNewImage.aktuellePositionX = 0
        bewegungNewImage.aktuellePositionY = 0

        If aktuelleTransitionSettings.modus = "Zufällig" Then
            würfel1D2 = rnd.Next(2)
            If würfel1D2 = 0 Then
                aktuelleTransitionSettings.modus = "Schieben"
            Else
                aktuelleTransitionSettings.modus = "Wischen"
            End If
        End If

        'Im Modus Wischen die Deltas von oldImage wieder verwerfen.
        If aktuelleTransitionSettings.modus = "Wischen" Then
            bewegungOldImage.directionX = 0
            bewegungOldImage.directionY = 0
        End If

        'Grafikobjekte initialisieren (das muss ja nun nicht bei jedem Timer-Tick passieren)
        If clientSize.IsEmpty Then
            clientSize = targetGrapics.VisibleClipBounds.Size.ToSize()
        End If

        bmp = New Bitmap(clientSize.Width, clientSize.Height)
        targetRect = New Rectangle(0, 0, clientSize.Width, clientSize.Height)
        drawRectOld = GetDrawRectangle(oldImage.Size, targetRect, oldPicBoxSizeMode)
        drawRectNew = GetDrawRectangle(newImage.Size, targetRect, newPicBoxSizeMode)
        offsetX = 100 / clientSize.Width
        offsetY = 100 / clientSize.Height

        'Timer initialisieren
        If tmrAnimation Is Nothing Then tmrAnimation = New Timer()
        If tmrDuration Is Nothing Then tmrDuration = New Timer()
        tmrAnimation.Interval = (100 - aktuelleTransitionSettings.geschwindigkeit)
        tmrDuration.Interval = If(durationMs > 0, durationMs, 999999)
        tmrAnimation.Start()
        tmrDuration.Start()

    End Sub
    Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Aufräumen und Transition beenden.

        If tmrAnimation IsNot Nothing Then
            tmrAnimation.Stop()
            tmrAnimation.Dispose()
            tmrAnimation = Nothing
        End If

        If tmrDuration IsNot Nothing Then
            tmrDuration.Stop()
            tmrDuration.Dispose()
            tmrDuration = Nothing
        End If

        RaiseEvent TransitionIsRunning(False)

    End Sub

    ' --- Optionen/Dialoghandling ---
    Function GetTransitionOptionsDialog() As UserControl Implements ISlideShowTransition.GetTransitionOptionsDialog
        'Liefert den Options-Dialog der Transition

        Return New ucOptionsTransition()

    End Function
    Function MemorizeTransitionSettings(uc As UserControl) As Object Implements ISlideShowTransition.MemorizeTransitionSettings
        ' Nicht implementiert - Direct Commit in dem ucOptionsTransition
    End Function
    Sub ApplyTransitionSettings(settings As Object) Implements ISlideShowTransition.ApplyTransitionSettings
        ' Nicht implementiert - Direct Commit in dem ucOptionsTransition
    End Sub
    Sub GetTransitionSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowTransition.GetTransitionSettings
        ' Nicht implementiert - Direct Commit in dem ucOptionsTransition
    End Sub
    Sub GetTransitionRegistryOrDefaultSettings(uc As UserControl) Implements ISlideShowTransition.GetTransitionRegistryOrDefaultSettings
        ' Nicht implementiert - Direct Commit in dem ucOptionsTransition
    End Sub

    Sub GetCurrentTransitionSettings()
        'Setzt aktuelleTransitonSettings mit den Werten aus der Registry oder mit Defaultwerten.

        Dim defaults As Dictionary(Of String, String) = GetTransitionDefaultSettings()

        aktuelleTransitionSettings.geschwindigkeit = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Geschwindigkeit", defaults))
        aktuelleTransitionSettings.richtungen = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Richtungen", defaults))
        aktuelleTransitionSettings.modus = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_SuW_FULLPATH & "Modus", defaults)

    End Sub

    ' --- Info-Kommunikation ---
    Sub AttentionShaderGewechselt(shaderName As String) Implements ISlideShowTransition.AttentionShaderGewechselt
        'Für Transitionen, die mit Shadern arbeiten. Hier nicht genutzt
    End Sub

    Sub CheckYourSettings() Implements ISlideShowTransition.CheckYourSettings
        'Macht bei dieser Transition keinen Sinn
    End Sub

    Public Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        Dim defaults As New Dictionary(Of String, String)

        defaults.Add("Geschwindigkeit", "50")
        defaults.Add("Richtungen", "W; O")
        defaults.Add("Modus", "Wischen")

        Return defaults

    End Function

    ' --- Eigentliche Transition  & Abbruch-Timer ---
    Sub tmrDuration_Tick() Handles tmrDuration.Tick
        'Bricht die Transition nach Ende von DurationMS ab.

        Dim clientSize As Size = targetGrapics.VisibleClipBounds.Size.ToSize()
        Dim drawRect As Rectangle
        Dim targetRect As Rectangle
        Dim bmp As New Bitmap(clientSize.Width, clientSize.Height)

        'Grafik vorbereiten
        targetRect = New Rectangle(0, 0, clientSize.Width, clientSize.Height)
        drawRect = GetDrawRectangle(newImage.Size, targetRect, newPicBoxSizeMode)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.Clear(Color.Black)

            ' Endbild gnadenlos auf den Zeichenbereich malen...
            g.DrawImage(newImage, drawRect)
        End Using

        '...und dann raus aus der Transition.
        StopTransition()

    End Sub

    Sub tmrAnimation_Tick() Handles tmrAnimation.Tick
        ' Hier passierte die eigentliche Animation.

        ' Aktuelle Zeichenposition berechnen
        Dim drawPosOld As New Point(drawRectOld.X + bewegungOldImage.aktuellePositionX,
                                drawRectOld.Y + bewegungOldImage.aktuellePositionY)

        Dim drawPosNew As New Point(drawRectNew.X + bewegungNewImage.aktuellePositionX,
                                drawRectNew.Y + bewegungNewImage.aktuellePositionY)

        ' Abbruchbedingung: Wenn das neue Bild (fast) da ist
        If IstZielErreicht(drawPosNew, drawRectNew.Location, 3) Then
            StopTransition()
            Return
        End If

        ' Zeichenfläche holen
        Using g As Graphics = targetGrapics
            g.Clear(Color.Black) ' oder Hintergrundfarbe
            g.DrawImage(oldImage, New Rectangle(drawPosOld, drawRectOld.Size))
            g.DrawImage(newImage, New Rectangle(drawPosNew, drawRectNew.Size))
        End Using

        ' Bewegung fortsetzen
        bewegungOldImage.aktuellePositionX += offsetX * bewegungOldImage.directionX
        bewegungOldImage.aktuellePositionY += offsetY * bewegungOldImage.directionY
        bewegungNewImage.aktuellePositionX += offsetX * bewegungNewImage.directionX
        bewegungNewImage.aktuellePositionY += offsetY * bewegungNewImage.directionY

    End Sub

    Private Function IstZielErreicht(p1 As Point, p2 As Point, tolerance As Integer) As Boolean
        Return Math.Abs(p1.X - p2.X) <= tolerance AndAlso Math.Abs(p1.Y - p2.Y) <= tolerance
    End Function

End Class
