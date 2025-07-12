Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.ListHandling
Imports SlideShowLogging.LogHandling
Imports SlideShowTools.GraphicsSizeModeHandling
Imports System.Drawing.Drawing2D
Imports SlideShowTools

Public Class TransitionMain
    Implements ISlideShowTransition

    '--- Variablen, Enums und Structures ---

    Public Const SLIDESHOWTRANSITION_SuW_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Schieben & Wischen\"

    Private WithEvents tmrDuration As New Timer
    Private WithEvents tmrAnimation As New Timer
    Private aktuelleTransitionSettings As New SlideShowTransition_SuW_Settings

    Private oldImg As Image
    Private newImg As Image
    Private oldBmpGerahmt As Bitmap
    Private newBmpGerahmt As Bitmap
    Private oldPicBoxSizeMode As PictureBoxSizeMode
    Private newPicBoxSizeMode As PictureBoxSizeMode
    Private renderTarget As Graphics
    Private bewegungOldImage As BewegungsInfos
    Private bewegungNewImage As BewegungsInfos

    Private cltSize As Size
    Private targetRect As Rectangle
    Private bmp As Bitmap
    Private zeichenFlaeche As Graphics

    Private offsetX As Double
    Private offsetY As Double
    Private fps As Integer = 50
    Private endPunkt As Point = New Point(0, 0)


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
        oldImg = oldImage
        newImg = newImage
        oldPicBoxSizeMode = picBoxModeOld
        newPicBoxSizeMode = picBoxModeNew
        renderTarget = targetGraphics
        cltSize = clientSize

        'Grafikobjekte initialisieren (das muss ja nun nicht bei jedem Timer-Tick passieren)
        If cltSize.IsEmpty Then
            cltSize = renderTarget.VisibleClipBounds.Size.ToSize()
        End If

        bmp = New Bitmap(cltSize.Width, cltSize.Height)
        targetRect = New Rectangle(0, 0, cltSize.Width, cltSize.Height)

        oldBmpGerahmt = ErzeugeGerahmtesBild(oldImg, oldPicBoxSizeMode, cltSize)
        newBmpGerahmt = ErzeugeGerahmtesBild(newImg, newPicBoxSizeMode, cltSize)

        'Aktuelle Settings abholen
        GetCurrentTransitionSettings()

#Region "Richtungen festlegen"
        'Richtung für die Transition aussuchen und Bewegungsinfos setzen (und dabei ein paar if-then sparen... ;-)
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
                bewegungNewImage.aktuellePositionX = -newBmpGerahmt.Width
                bewegungNewImage.aktuellePositionY = -newBmpGerahmt.Height
                bewegungNewImage.directionX = 1
                bewegungNewImage.directionY = 1

                bewegungOldImage.directionX = 1
                bewegungOldImage.directionY = 1
            Case "N"
                bewegungNewImage.aktuellePositionX = 0
                bewegungNewImage.aktuellePositionY = -newBmpGerahmt.Height
                bewegungNewImage.directionX = 0
                bewegungNewImage.directionY = 1

                bewegungOldImage.directionX = 0
                bewegungOldImage.directionY = 1
            Case "NO"
                bewegungNewImage.aktuellePositionX = oldBmpGerahmt.Width
                bewegungNewImage.aktuellePositionY = -newBmpGerahmt.Height
                bewegungNewImage.directionX = -1
                bewegungNewImage.directionY = 1

                bewegungOldImage.directionX = -1
                bewegungOldImage.directionY = 1
            Case "O"
                bewegungNewImage.aktuellePositionX = oldBmpGerahmt.Width
                bewegungNewImage.aktuellePositionY = 0
                bewegungNewImage.directionX = -1
                bewegungNewImage.directionY = 0

                bewegungOldImage.directionX = -1
                bewegungOldImage.directionY = 0
            Case "SO"
                bewegungNewImage.aktuellePositionX = oldBmpGerahmt.Width
                bewegungNewImage.aktuellePositionY = oldBmpGerahmt.Height
                bewegungNewImage.directionX = -1
                bewegungNewImage.directionY = -1

                bewegungOldImage.directionX = -1
                bewegungOldImage.directionY = -1
            Case "S"
                bewegungNewImage.aktuellePositionX = 0
                bewegungNewImage.aktuellePositionY = oldBmpGerahmt.Height
                bewegungNewImage.directionX = 0
                bewegungNewImage.directionY = -1

                bewegungOldImage.directionX = 0
                bewegungOldImage.directionY = -1
            Case "SW"
                bewegungNewImage.aktuellePositionX = -newBmpGerahmt.Width
                bewegungNewImage.aktuellePositionY = oldBmpGerahmt.Height
                bewegungNewImage.directionX = 1
                bewegungNewImage.directionY = -1

                bewegungOldImage.directionX = 1
                bewegungOldImage.directionY = -1
            Case "W"
                bewegungNewImage.aktuellePositionX = -newBmpGerahmt.Width
                bewegungNewImage.aktuellePositionY = 0
                bewegungNewImage.directionX = 1
                bewegungNewImage.directionY = 0

                bewegungOldImage.directionX = 1
                bewegungOldImage.directionY = 0
        End Select

        'Startposition des alten Bildes ist natürlich (0,0)
        bewegungOldImage.aktuellePositionX = 0
        bewegungOldImage.aktuellePositionY = 0

        'Falls Modus = Zufällig, Modus erwürfeln
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
#End Region

        ' Berechne notwendige Distanz zwischen Startpunkt und Zielpunkt:
        Dim distX As Integer = Math.Abs(bewegungNewImage.aktuellePositionX - endPunkt.X)
        Dim distY As Integer = Math.Abs(bewegungNewImage.aktuellePositionY - endPunkt.Y)

        ' Offset berechnen
        offsetX = distX / (aktuelleTransitionSettings.geschwindigkeit * fps)
        offsetY = distY / (aktuelleTransitionSettings.geschwindigkeit * fps)

        'Timer initialisieren
        If tmrAnimation Is Nothing Then tmrAnimation = New Timer()
        If tmrDuration Is Nothing Then tmrDuration = New Timer()

        tmrAnimation.Interval = 1000 \ fps
        tmrAnimation.Start()

        If durationMs > 0 Then
            tmrDuration.Interval = durationMs
            tmrDuration.Start()
        End If


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

        Dim drawRect As Rectangle
        Dim targetRect As Rectangle

        'Grafik vorbereiten
        bmp = New Bitmap(cltSize.Width, cltSize.Height)
        targetRect = New Rectangle(0, 0, cltSize.Width, cltSize.Height)
        drawRect = GetDrawRectangle(newBmpGerahmt.Size, targetRect, newPicBoxSizeMode)

        Try
            renderTarget = Graphics.FromImage(bmp)
            renderTarget.InterpolationMode = InterpolationMode.HighQualityBicubic
            renderTarget.Clear(Color.Black)

            ' Endbild gnadenlos auf den Zeichenbereich malen...
            renderTarget.DrawImage(newBmpGerahmt, drawRect)
        Catch ex As Exception
            LogError("Transition Schieben & Wischen: TransitionMain.tmrDurationTick() - Fehler beim Erstellen von renderTarget: " & ex.Message)
        End Try

        '...und dann raus aus der Transition.
        StopTransition()

    End Sub

    Private Sub tmrAnimation_Tick(sender As Object, e As EventArgs) Handles tmrAnimation.Tick
        ' --- Positionen aktualisieren ---
        bewegungOldImage.aktuellePositionX += CInt(offsetX) * bewegungOldImage.directionX
        bewegungOldImage.aktuellePositionY += CInt(offsetY) * bewegungOldImage.directionY
        bewegungNewImage.aktuellePositionX += CInt(offsetX) * bewegungNewImage.directionX
        bewegungNewImage.aktuellePositionY += CInt(offsetY) * bewegungNewImage.directionY

        ' --- Ziel erreicht? ---
        Dim aktuellePosition As New Point(bewegungNewImage.aktuellePositionX, bewegungNewImage.aktuellePositionY)
        If IstZielErreicht(endPunkt, aktuellePosition, Math.Max(offsetX, offsetY)) Then
            StopTransition()
            Return
        End If

        ' --- Neues Bild generieren ---
        bmp = New Bitmap(cltSize.Width, cltSize.Height)
        Using zeichenFlaeche As Graphics = Graphics.FromImage(bmp)
            zeichenFlaeche.Clear(Color.Black)

            ' Altes Bild zeichnen
            Dim zielRectOld As New Rectangle(
            New Point(bewegungOldImage.aktuellePositionX, bewegungOldImage.aktuellePositionY),
            cltSize)
            zeichenFlaeche.DrawImage(oldBmpGerahmt, zielRectOld)

            ' Neues Bild zeichnen
            Dim zielRectNew As New Rectangle(
            New Point(bewegungNewImage.aktuellePositionX, bewegungNewImage.aktuellePositionY),
            cltSize)
            zeichenFlaeche.DrawImage(newBmpGerahmt, zielRectNew)
        End Using

        ' --- Ausgabe aufs Ziel ---
        renderTarget.DrawImage(bmp, 0, 0)
        bmp.Dispose()
    End Sub


    Private Function IstZielErreicht(p1 As Point, p2 As Point, tolerance As Integer) As Boolean
        Return Math.Abs(p1.X - p2.X) <= tolerance AndAlso Math.Abs(p1.Y - p2.Y) <= tolerance
    End Function

    Private Function ErzeugeGerahmtesBild(bild As Image, sizeMode As PictureBoxSizeMode, zielgroesse As Size) As Bitmap
        'Erstellt ein Bitmap mit dem Bild gemäß SizeMode mit schwarzem Rahmen in der Zielgröße

        bmp = New Bitmap(zielgroesse.Width, zielgroesse.Height)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.Black)
            Dim drawRect As Rectangle = GraphicsSizeModeHandling.GetDrawRectangle(bild.Size, targetRect, sizeMode)
            g.DrawImage(bild, drawRect)
        End Using

        Return bmp
        bmp.Dispose()

    End Function

End Class

