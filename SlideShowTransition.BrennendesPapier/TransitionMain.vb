Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Windows
Imports System.Windows.Forms
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Threading
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging
Imports SlideShowLogging.LogHandling
Imports SlideShowTools
Imports SlideShowTools.GraphicsSizeModeHandling
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.ListHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling
Public Class TransitionMain
    Implements ISlideShowTransition

#Region "Variablendeklaration"

    'Settings
    Public Const SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH As String =
        SLIDESHOWTRANSITION_PATH & "Brennendes Papier\"

    Public Const nameTransition As String = "Brennendes Papier"
    Private Const FPS As Integer = 30

    Private aktuelleSettings As New SlideShowTransitionSettings_BrennendesPapier()

    Private Const DAUER_MIN As Integer = 4
    Private Const DAUER_MAX As Integer = 30

    Private Const MAGIE_MAX_NACHLAUF_SEKUNDEN As Double = 4.0
    Private Const MAGIE_NACHLAUF_DAUER_MS As Double = MAGIE_MAX_NACHLAUF_SEKUNDEN * 1000.0
    Private Const REVEAL_MAX_NACHLAUF_SEKUNDEN As Double = 0.5
    Private Const REVEAL_NACHLAUF_DAUER_MS As Double = REVEAL_MAX_NACHLAUF_SEKUNDEN * 1000.0
    Private Const SAEURE_NACHLAUF_SEKUNDEN As Single = 3.0F
    Private Const SAEURE_NACHLAUF_DAUER_MS As Double = SAEURE_NACHLAUF_SEKUNDEN * 1000.0
    Private Const FEUER_NACHLAUF_SEKUNDEN As Double = 2.0
    Private Const FEUER_NACHLAUF_DAUER_MS As Double = FEUER_NACHLAUF_SEKUNDEN * 1000.0
    Private Const BLITZ_NACHLAUF_SEKUNDEN As Double = 1.0
    Private Const BLITZ_NACHLAUF_DAUER_MS As Double = BLITZ_NACHLAUF_SEKUNDEN * 1000.0

    'Lebenszyklus
    Private aktuellerModus As String
    Private aktuellerZuendmodus As String
    Private partikelNachlaufGestartet As Boolean
    Private schwerkraftAktiv As Boolean

    'Zeitmanagement
    Private ReadOnly laufzeit As New Stopwatch()
    Private frameTimer As DispatcherTimer
    Private dauerInMS As Integer
    Private letzteFrameZeitMS As Double

    'Transitionsbilder
    Private oldBitmapSource As BitmapSource
    Private newBitmapSource As BitmapSource
    Private brandMaske As BitmapSource
    Private gradientBitmap As BitmapSource
    Private particleGradientBitmap As BitmapSource

    'Debugging
    Private Const SAVE_BRANDMASKE_AS_IMAGE As Boolean = True

    'Rendering
    Private direct3DRenderer As D3DRenderer
    Private renderSize As Windows.Size

    'Status
    Private transitionLaeuft As Boolean
    Private wurdeBereinigt As Boolean

    'Speicher-Logging
    Private frameZaehler As Integer

    'Sonstiges
    Private ReadOnly rnd As New Random()

#Region "Structures and Enums"

    'Structures und Enums
    Public Structure SlideShowTransitionSettings_BrennendesPapier

        Public modi As List(Of String)

        Public zuendmodus As String

        Public partikel As Boolean
        Public gradient As Boolean
        Public verzerrung As Boolean
        Public textur As Boolean

        Public schwerkraft As String

        Public dauer As Integer
        Public zufallsdauer As Boolean

        Public brandkantenbreite As Integer
        Public partikelLebensdauer As Double

        Public verzerrungsbreite As Integer
        Public verzerrungsstaerke As Integer
        Public magieScherbengroesse As Integer

        Public fbmGrundfrequenz As Double
        Public fbmOktaven As Integer
        Public fbmPersistenz As Double
        Public fbmStaerke As Double

    End Structure


#End Region


#End Region

#Region "Eigenschaften"

    Public ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName

        Get
            Return nameTransition
        End Get

    End Property

    Public ReadOnly Property TransitionKurzBeschreibung As String Implements ISlideShowTransition.TransitionKurzBeschreibung

        Get
            Return "Sehr sehr heiße, sehr sehr flach zusammengepresste Holzfasern"
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

#Region "Transition"

    Public Sub RunTransition(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode, newImage As BitmapImage,
                             picBoxModeNew As PictureBoxSizeMode, clientSize As System.Drawing.Size,
                             Optional durationMs As Integer = 0) Implements ISlideShowTransition.RunTransition

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(TransitionMain))
        End If

        If oldImage Is Nothing Then
            Throw New ArgumentNullException(NameOf(oldImage))
        End If

        If newImage Is Nothing Then
            Throw New ArgumentNullException(NameOf(newImage))
        End If

        If clientSize.Width <= 0 OrElse clientSize.Height <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(clientSize))
        End If

        BeendeUndBereinigeTransition()

        ReadTransitionSettingsFromRegistryOrDefaults()

        If durationMs > 0 Then

            dauerInMS = durationMs

        Else

            dauerInMS = ErmittleTransitionsdauerInMS()

        End If

        renderSize = New Windows.Size(clientSize.Width, clientSize.Height)

        frameZaehler = 0
        partikelNachlaufGestartet = False

        Try

            transitionLaeuft = True

            RaiseEvent TransitionIsRunning(True)

            StarteDirect3DRenderPipeline(oldImage, picBoxModeOld, newImage, picBoxModeNew)

        Catch ex As Exception

            LogError(nameTransition & ": Fehler beim Starten der Transition: " & ex.ToString())

            BeendeUndBereinigeTransition()

            Throw

        End Try

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Beendet eine gegebenenfalls noch laufende Transition.

        BeendeUndBereinigeTransition()

    End Sub

#End Region

#Region "Optionsdialog"

    Public Function GetTransitionOptionsDialog() As System.Windows.Forms.UserControl _
    Implements ISlideShowTransition.GetTransitionOptionsDialog

        ReadTransitionSettingsFromRegistryOrDefaults()

        StoreSettings(
        nameTransition,
        aktuelleSettings)

        Return New ucOptionsTransition()

    End Function

#End Region

#Region "Settings und Defaultwerte"

    Private Sub ReadTransitionSettingsFromRegistryOrDefaults()
        'Liest die aktuellen Transitionseinstellungen aus der Registry
        'oder verwendet die definierten Defaultwerte.

        Dim defaults As Dictionary(Of String, String)

        defaults = GetTransitionDefaultSettings()

        aktuelleSettings.dauer = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                            "Dauer", defaults))
        aktuelleSettings.zufallsdauer = CBool(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                    "Zufallsdauer", defaults))
        aktuelleSettings.modi = SplitSemicolonList(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                         "Modi", defaults))
        aktuelleSettings.partikel = CBool(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                "Partikel", defaults))
        aktuelleSettings.schwerkraft = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                             "Schwerkraft", defaults)
        aktuelleSettings.brandkantenbreite = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                        "Brandkantenbreite", defaults))
        aktuelleSettings.zuendmodus = ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                            "Zuendmodus", defaults)
        aktuelleSettings.gradient = CBool(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                "Gradient", defaults))
        aktuelleSettings.verzerrung = CBool(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                  "Verzerrung", defaults))
        aktuelleSettings.textur = CBool(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                              "Textur", defaults))
        aktuelleSettings.partikelLebensdauer = Double.Parse(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                            "PartikelLebensdauer", defaults), CultureInfo.InvariantCulture)
        aktuelleSettings.verzerrungsbreite = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                        "Verzerrungsbreite", defaults))
        aktuelleSettings.verzerrungsstaerke = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                         "Verzerrungsstaerke", defaults))
        aktuelleSettings.magieScherbengroesse = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                           "MagieScherbengroesse", defaults))
        aktuelleSettings.fbmGrundfrequenz = Double.Parse(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                         "FBMGrundfrequenz", defaults), CultureInfo.InvariantCulture)
        aktuelleSettings.fbmOktaven = CInt(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                                 "FBMOktaven", defaults))
        aktuelleSettings.fbmPersistenz = Double.Parse(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                      "FBMPersistenz", defaults), CultureInfo.InvariantCulture)

        aktuelleSettings.fbmStaerke = Double.Parse(ReadFromRegOrDefaults(SLIDESHOWTRANSITION_BRENNENDESPAPIER_FULLPATH &
                                                  "FBMStaerke", defaults), CultureInfo.InvariantCulture)

    End Sub

    Friend Shared Function GetTransitionDefaultSettings() As Dictionary(Of String, String)
        'Liefert die Defaultwerte der Transition.

        Dim defaults As New Dictionary(Of String, String)

        defaults.Add("Dauer", "15")
        defaults.Add("Zufallsdauer", "True")

        defaults.Add("Modi", "Feuer")
        defaults.Add("Partikel", "True")
        defaults.Add("Schwerkraft", "Zufällig")

        defaults.Add("Brandkantenbreite", "35")

        defaults.Add("Zuendmodus", "Zufällig")

        defaults.Add("Gradient", "True")
        defaults.Add("Verzerrung", "True")
        defaults.Add("Textur", "True")

        defaults.Add("PartikelLebensdauer", "3.2")

        defaults.Add("Verzerrungsbreite", "18")
        defaults.Add("Verzerrungsstaerke", "50")
        defaults.Add("MagieScherbengroesse", "14")

        defaults.Add("FBMGrundfrequenz", "8.0")
        defaults.Add("FBMOktaven", "5")
        defaults.Add("FBMPersistenz", "0.6")
        defaults.Add("FBMStaerke", "0.4")

        Return defaults

    End Function

#End Region

#Region "Hilfsfunktionen"

    Private Function ErmittleTransitionsdauerInMS() As Integer
        'Ermittelt die Dauer des aktuellen Transitionsdurchlaufs.
        'Bei aktivierter Zufallsdauer wird für jeden Durchlauf
        'eine neue Dauer zwischen 1 und 30 Sekunden gewählt.

        Dim dauerInSekunden As Integer

        If aktuelleSettings.zufallsdauer Then

            dauerInSekunden = rnd.Next(DAUER_MIN, DAUER_MAX + 1)

        Else

            dauerInSekunden = Math.Max(DAUER_MIN, Math.Min(DAUER_MAX, aktuelleSettings.dauer))

        End If

        Return dauerInSekunden * 1000

    End Function

    Private Function ErmittleModusFuerAktuellenDurchlauf() As String

        Dim modus As String

        modus = "Feuer"

        If aktuelleSettings.modi IsNot Nothing AndAlso aktuelleSettings.modi.Count > 0 Then

            modus = aktuelleSettings.modi(rnd.Next(0, aktuelleSettings.modi.Count))

        End If

        Return modus

    End Function

    Private Function ErmittleZuendmodusFuerAktuellenDurchlauf() As String

        Select Case aktuelleSettings.zuendmodus
            Case "Brandherde"

                Return "Brandherde"

            Case "Brand vom Rand"

                Return "Brand vom Rand"

            Case "Zufällig"

                If rnd.Next(0, 2) = 0 Then
                    Return "Brandherde"
                Else
                    Return "Brand vom Rand"
                End If

            Case Else

                Return "Brandherde"

        End Select

    End Function

    Private Function ErmittleSchwerkraftFuerAktuellenDurchlauf() As Boolean
        'Löst die gespeicherte Schwerkrafteinstellung für den aktuellen
        'Transitionsdurchlauf in einen eindeutigen Laufzeitwert auf.

        Select Case aktuelleSettings.schwerkraft

            Case "An"

                Return True

            Case "Aus"

                Return False

            Case "Zufällig"

                Return rnd.Next(0, 2) = 1

            Case Else

                'Robuster Fallback bei unbekannten oder alten Registry-Werten.
                Return False

        End Select

    End Function

    Private Function ErmittleGradientNameFuerModus(modus As String) As String

        Select Case modus

            Case "Feuer"

                Return "BP - Feuer"

            Case "Blitze"

                Return "BP - Blitze"

            Case "Säure"

                Return "BP - Säure"

            Case "Magie"

                Return "BP - Magie"

            Case Else

                Return "BP - Feuer"

        End Select

    End Function

    Private Function ErmittlePartikelGradientFuerAktuellenDurchlauf(modus As String) As BitmapSource

        Dim gradientName As String
        Dim gradienten As List(Of SlideShowGradient)
        Dim gradient As SlideShowGradient

        gradientName = ErmittlePartikelGradientNameFuerModus(modus)

        gradienten = GradientenHandling.LadeGradienten()

        gradient =
        gradienten.FirstOrDefault(
            Function(item)

                Return String.Equals(
                    item.Name,
                    gradientName,
                    StringComparison.OrdinalIgnoreCase)

            End Function)

        If gradient Is Nothing Then

            Throw New InvalidOperationException("Der Partikelgradient """ & gradientName & """ für den Modus """ &
                                                modus & """ wurde nicht gefunden.")

        End If

        LogHandling.LogDebug(nameTransition & ": Partikelgradient: " & gradientName)

        Return GradientenHandling.ErzeugeGradientBitmap(gradient, 1024)

    End Function

    Private Function ErmittleGradientFuerAktuellenDurchlauf(modus As String) As BitmapSource

        Dim gradientName As String
        Dim gradienten As List(Of SlideShowGradient)
        Dim gradient As SlideShowGradient

        gradientName = ErmittleGradientNameFuerModus(modus)

        gradienten = GradientenHandling.LadeGradienten()

        gradient =
        gradienten.FirstOrDefault(
            Function(item)

                Return String.Equals(
                    item.Name,
                    gradientName,
                    StringComparison.OrdinalIgnoreCase)

            End Function)

        If gradient Is Nothing Then

            Throw New InvalidOperationException("Der Gradient """ & gradientName & """ für den Modus """ &
                                                modus & """ wurde nicht gefunden.")

        End If

        LogHandling.LogDebug(nameTransition & ": Modus: " & modus & "; Gradient: " & gradientName)

        Return GradientenHandling.ErzeugeGradientBitmap(gradient, 1024)

    End Function

    Private Function ErmittlePartikelGradientNameFuerModus(modus As String) As String

        Select Case modus

            Case "Feuer"

                Return "BP - Feuer - Partikel"

            Case "Blitze"

                Return "BP - Blitze - Partikel"

            Case "Säure"

                Return "BP - Säure - Partikel"

            Case "Magie"

                Return "BP - Magie - Partikel"

            Case Else

                Return "BP - Feuer - Partikel"

        End Select

    End Function

    Private Function ErmittleNormierteBrandkantenbreite() As Single

        Dim breite As Single

        breite = CSng(aktuelleSettings.brandkantenbreite) / 1000.0F

        Return Math.Max(0.001F, Math.Min(1.0F, breite))

    End Function

    Private Sub LogRenderSpeicherstatus(frameNummer As Integer, progress As Double, Optional phase As String = "")

        Dim prozess As Process
        Dim phaseText As String

        phaseText = String.Empty

        If Not String.IsNullOrWhiteSpace(phase) Then
            phaseText = "; Phase=" & phase
        End If

        prozess = Process.GetCurrentProcess()

        LogHandling.LogDebug(nameTransition & ": Render-Speicher" & "; Frame=" & frameNummer.ToString() & "; Progress=" &
                 Math.Round(progress, 3).ToString() & phaseText & "; Managed=" & Math.Round(GC.GetTotalMemory(False) /
                 1024.0 / 1024.0, 1).ToString() & " MB" & "; Private=" & Math.Round(prozess.PrivateMemorySize64 /
                 1024.0 / 1024.0, 1).ToString() & " MB" & "; WorkingSet=" & Math.Round(prozess.WorkingSet64 /
                 1024.0 / 1024.0, 1).ToString() & " MB")

    End Sub

#End Region

#Region "Renderzyklus"

    Private Sub StarteRenderTimer()

        frameTimer = New DispatcherTimer(DispatcherPriority.Render)
        frameTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / FPS)

        AddHandler frameTimer.Tick, AddressOf FrameTimer_Tick

        frameTimer.Start()

    End Sub

    Private Sub FrameTimer_Tick(sender As Object, e As EventArgs)

        Dim progress As Double
        Dim aktuelleFrameZeitMS As Double
        Dim deltaTime As Double
        Dim gesamtDauerInMS As Double

        Dim hauptphaseAbgeschlossen As Boolean
        Dim nachlaufAbgeschlossen As Boolean

        Dim nachlaufDauerInMS As Double
        Dim partikelNachlaufDauerInMS As Double
        Dim verzerrungsNachlaufDauerInMS As Double
        Dim revealNachlaufDauerInMS As Double
        Dim saureNachlaufDauerInMS As Double


        If Not transitionLaeuft Then
            Exit Sub
        End If


        aktuelleFrameZeitMS = laufzeit.Elapsed.TotalMilliseconds

        deltaTime = (aktuelleFrameZeitMS - letzteFrameZeitMS) / 1000.0

        letzteFrameZeitMS = aktuelleFrameZeitMS


        '---------------------------------
        ' Effekt-Nachlauf bestimmen
        '---------------------------------

        nachlaufDauerInMS = 0.0
        partikelNachlaufDauerInMS = 0.0
        verzerrungsNachlaufDauerInMS = 0.0
        revealNachlaufDauerInMS = REVEAL_NACHLAUF_DAUER_MS
        saureNachlaufDauerInMS = 0.0

        'Reveal läuft IMMER.
        nachlaufDauerInMS = Math.Max(nachlaufDauerInMS, revealNachlaufDauerInMS)

        If aktuelleSettings.partikel Then

            partikelNachlaufDauerInMS = aktuelleSettings.partikelLebensdauer * 1000.0
            nachlaufDauerInMS = Math.Max(nachlaufDauerInMS, partikelNachlaufDauerInMS)

        End If


        If aktuelleSettings.verzerrung AndAlso String.Equals(aktuellerModus, "Magie",
                                                 StringComparison.OrdinalIgnoreCase) Then

            verzerrungsNachlaufDauerInMS = MAGIE_NACHLAUF_DAUER_MS
            nachlaufDauerInMS = Math.Max(nachlaufDauerInMS, verzerrungsNachlaufDauerInMS)

        End If

        If aktuelleSettings.verzerrung AndAlso String.Equals(aktuellerModus, "Säure",
                                                 StringComparison.OrdinalIgnoreCase) Then

            nachlaufDauerInMS = Math.Max(nachlaufDauerInMS, SAEURE_NACHLAUF_DAUER_MS)

        End If

        If aktuelleSettings.verzerrung AndAlso String.Equals(aktuellerModus, "Feuer",
                                                 StringComparison.OrdinalIgnoreCase) Then

            nachlaufDauerInMS = Math.Max(nachlaufDauerInMS, FEUER_NACHLAUF_DAUER_MS)

        End If

        If aktuelleSettings.verzerrung AndAlso String.Equals(aktuellerModus, "Blitze",
                                                 StringComparison.OrdinalIgnoreCase) Then

            nachlaufDauerInMS = Math.Max(nachlaufDauerInMS, BLITZ_NACHLAUF_DAUER_MS)

        End If

        gesamtDauerInMS = dauerInMS + nachlaufDauerInMS

        hauptphaseAbgeschlossen = aktuelleFrameZeitMS >= dauerInMS
        nachlaufAbgeschlossen = aktuelleFrameZeitMS >= gesamtDauerInMS


        If aktuelleSettings.partikel AndAlso hauptphaseAbgeschlossen AndAlso Not partikelNachlaufGestartet Then

            partikelNachlaufGestartet = True

            LogHandling.LogDebug(nameTransition & ": Hauptphase abgeschlossen. Partikel-Nachlauf beginnt für maximal " &
                                 aktuelleSettings.partikelLebensdauer.ToString("0.0") & " Sekunden.")

        End If

        If hauptphaseAbgeschlossen Then

            progress = 1.0

        Else

            progress = aktuelleFrameZeitMS / dauerInMS

        End If


        DrawDirect3DFrame(progress, deltaTime)


        If nachlaufAbgeschlossen Then

            LogHandling.LogDebug(nameTransition & ": Effekt-Nachlauf abgeschlossen.")

            BeendeUndBereinigeTransition()

        End If

    End Sub

    Private Sub EndBildZeichnen()

        If newBitmapSource Is Nothing Then
            Exit Sub
        End If

        RaiseEvent TransitionFrameIstFertig(newBitmapSource)

    End Sub

#End Region

#Region "Render-Pipeline Direct3D"

    Private Sub StarteDirect3DRenderPipeline(oldImage As BitmapImage, picBoxModeOld As PictureBoxSizeMode,
                                             newImage As BitmapImage, picBoxModeNew As PictureBoxSizeMode)

        Dim clientSize As System.Drawing.Size
        Dim brandkantenBreite As Single
        Dim verzerrungsBreite As Single
        Dim verzerrungsStaerke As Single
        Dim magieScherbenGroesse As Single
        Dim partikelLebensdauer As Single

        Dim brandmaskenGenerator As BrandmaskenGenerator
        Dim renderParameter As RenderParameter

        clientSize = New System.Drawing.Size(CInt(renderSize.Width), CInt(renderSize.Height))


        ' Die etablierte Bildgrößen-/PictureBoxSizeMode-Logik bleibt
        ' außerhalb des D3D-Renderers.

        oldBitmapSource = ErzeugeGerahmtesBild(oldImage, picBoxModeOld, clientSize)
        newBitmapSource = ErzeugeGerahmtesBild(newImage, picBoxModeNew, clientSize)

        If oldBitmapSource Is Nothing Then
            Throw New InvalidOperationException("Das gerahmte alte Transitionsbild konnte nicht erzeugt werden.")
        End If

        If newBitmapSource Is Nothing Then
            Throw New InvalidOperationException("Das gerahmte neue Transitionsbild konnte nicht erzeugt werden.")
        End If

        ' Prozedurale Brandmaske erzeugen.
        aktuellerZuendmodus = ErmittleZuendmodusFuerAktuellenDurchlauf()

        LogHandling.LogDebug(nameTransition & ": Zündmodus für aktuellen Durchlauf: " & aktuellerZuendmodus)

        brandmaskenGenerator = New BrandmaskenGenerator()

        brandMaske = brandmaskenGenerator.Erzeuge(CInt(renderSize.Width), CInt(renderSize.Height),
                                                  aktuellerZuendmodus, aktuelleSettings.fbmGrundfrequenz,
                                                  aktuelleSettings.fbmOktaven, aktuelleSettings.fbmPersistenz,
                                                  aktuelleSettings.fbmStaerke)

        If brandMaske Is Nothing Then
            Throw New InvalidOperationException("Die prozedurale Brandmaske konnte nicht erzeugt werden.")
        End If

        If SAVE_BRANDMASKE_AS_IMAGE Then

            SaveBrandMaskeAsImage()

        End If


        aktuellerModus = ErmittleModusFuerAktuellenDurchlauf()

        schwerkraftAktiv = ErmittleSchwerkraftFuerAktuellenDurchlauf()

        LogHandling.LogDebug(nameTransition & ": Schwerkraft für aktuellen Durchlauf: " & If(schwerkraftAktiv,
                             "An", "Aus"))

        gradientBitmap = Nothing

        If aktuelleSettings.gradient Then

            gradientBitmap = ErmittleGradientFuerAktuellenDurchlauf(aktuellerModus)

        End If

        particleGradientBitmap = Nothing

        If aktuelleSettings.partikel Then

            particleGradientBitmap = ErmittlePartikelGradientFuerAktuellenDurchlauf(aktuellerModus)

        End If

        If aktuelleSettings.gradient AndAlso gradientBitmap Is Nothing Then
            Throw New InvalidOperationException("Die Gradiententextur konnte nicht erzeugt werden.")
        End If

        If aktuelleSettings.partikel AndAlso particleGradientBitmap Is Nothing Then
            Throw New InvalidOperationException("Der Partikelgradient konnte nicht erzeugt werden.")
        End If

        brandkantenBreite = ErmittleNormierteBrandkantenbreite()
        verzerrungsBreite = CSng(aktuelleSettings.verzerrungsbreite) / 100.0F
        verzerrungsStaerke = CSng(aktuelleSettings.verzerrungsstaerke) / 50.0F
        magieScherbenGroesse = CSng(aktuelleSettings.magieScherbengroesse)
        partikelLebensdauer = CSng(aktuelleSettings.partikelLebensdauer)

        renderParameter = New RenderParameter()

        renderParameter.breite = CInt(renderSize.Width)
        renderParameter.hoehe = CInt(renderSize.Height)

        renderParameter.oldImage = oldBitmapSource
        renderParameter.newImage = newBitmapSource
        renderParameter.brandMaske = brandMaske

        renderParameter.gradientBitmap = gradientBitmap
        renderParameter.particleGradientBitmap = particleGradientBitmap

        renderParameter.brandkantenBreite = brandkantenBreite

        renderParameter.partikelAktiv = aktuelleSettings.partikel
        renderParameter.schwerkraftAktiv = schwerkraftAktiv
        renderParameter.gradientAktiv = aktuelleSettings.gradient
        renderParameter.verzerrungAktiv = aktuelleSettings.verzerrung

        renderParameter.modus = aktuellerModus

        renderParameter.partikelLebensdauer = partikelLebensdauer

        renderParameter.verzerrungsBreite = verzerrungsBreite
        renderParameter.verzerrungsStaerke = verzerrungsStaerke

        renderParameter.magieScherbenGroesse = magieScherbenGroesse


        direct3DRenderer = New D3DRenderer()

        direct3DRenderer.Initialisiere(renderParameter)

        ' Erster Frame = progress 0.
        direct3DRenderer.RenderFrame(0.0F, 0.0F)

        RaiseEvent TransitionFrameIstFertig(direct3DRenderer.FrameImage)

        letzteFrameZeitMS = 0.0

        laufzeit.Restart()

        StarteRenderTimer()

    End Sub

    Private Sub DrawDirect3DFrame(progress As Double, deltaTime As Double)

        Dim begrenzterProgress As Single
        Dim begrenztesDeltaTime As Single

        If direct3DRenderer Is Nothing Then
            Exit Sub
        End If

        frameZaehler += 1

        If frameZaehler Mod 30 = 0 Then

            LogRenderSpeicherstatus(frameZaehler, progress, "Frame: ")

        End If

        begrenzterProgress = CSng(Math.Max(0.0, Math.Min(1.0, progress)))

        begrenztesDeltaTime = CSng(Math.Max(0.0, Math.Min(0.1, deltaTime)))

        direct3DRenderer.RenderFrame(begrenzterProgress, begrenztesDeltaTime)

    End Sub

#End Region

#Region "Debugging"

    Private Sub SaveBrandMaskeAsImage()

        Dim encoder As PngBitmapEncoder
        Dim frame As BitmapFrame

        Dim verzeichnis As String
        Dim dateiname As String
        Dim kompletterPfad As String

        Dim fStream As FileStream


        If brandMaske Is Nothing Then
            Exit Sub
        End If


        verzeichnis = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                                   "SlideShowSaver 3.0", "Debug", "Brennendes Papier")

        Directory.CreateDirectory(verzeichnis)


        dateiname = "BrandMaske_" & DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") & ".png"

        kompletterPfad = Path.Combine(verzeichnis, dateiname)


        encoder = New PngBitmapEncoder()

        frame = BitmapFrame.Create(brandMaske)

        encoder.Frames.Add(frame)

        fStream = New FileStream(kompletterPfad, FileMode.Create, FileAccess.Write, FileShare.None)

        Using fStream

            encoder.Save(fStream)

        End Using

        LogHandling.LogDebug("Brennendes Papier: Brandmaske gespeichert: " & kompletterPfad)

    End Sub

#End Region

#Region "Bereinigung & Dispose"

    Private Sub BeendeUndBereinigeTransition()

        Dim warAktiv As Boolean

        warAktiv = transitionLaeuft

        'Laufenden Betrieb stoppen
        transitionLaeuft = False

        If frameTimer IsNot Nothing Then

            frameTimer.Stop()

            RemoveHandler frameTimer.Tick, AddressOf FrameTimer_Tick

            frameTimer = Nothing

        End If

        laufzeit.Stop()
        laufzeit.Reset()

        ' Der D3D-Renderer besitzt sämtliche GPU- und D3D/WPF-Interop-Ressourcen.
        If direct3DRenderer IsNot Nothing Then

            LogRenderSpeicherstatus(frameZaehler, 1.0, "Vor D3D Dispose")

            direct3DRenderer.Dispose()
            direct3DRenderer = Nothing

            LogRenderSpeicherstatus(frameZaehler, 1.0, "Nach D3D Dispose")

        End If

        ' Die CPU-seitigen Quellen dürfen erst freigegeben werden, nachdem der Renderer seine Texturen
        ' und SRVs zerstört hat.
        oldBitmapSource = Nothing
        newBitmapSource = Nothing
        brandMaske = Nothing

        ' Laufzeitzustand zurücksetzen
        dauerInMS = 0
        renderSize = Windows.Size.Empty
        frameZaehler = 0

        ' Erst nach vollständiger Bereinigung das Ende melden
        If warAktiv Then

            RaiseEvent TransitionIsRunning(False)

        End If

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        'Beendet die Transition endgültig.

        If wurdeBereinigt Then
            Exit Sub
        End If

        BeendeUndBereinigeTransition()

        wurdeBereinigt = True

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class