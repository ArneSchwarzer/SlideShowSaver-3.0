Imports System.Drawing
Imports System.Windows.Forms
Imports System.Windows.Media.Imaging
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.ImageConversionHandling
Imports SlideShowTools.RegistryHandling

Public Class TransitionMain
    Implements ISlideShowTransition

#Region "Variablendeklaration"
    'Settings und Konstanten
    Public Const SLIDESHOWTRANSITION_VOMWINDEVERWEHT_FULLPATH As String = SLIDESHOWTRANSITION_PATH & "Vom Winde verweht\"
    Public Const nameTransition As String = "Vom Winde verweht"
    Private aktuelleSettings As TransitionSettings_VomWindeVerweht

    'Lifecycle
    Private transitionLaeuft As Boolean
    Private wurdeBereinigt As Boolean

    Structure TransitionSettings_VomWindeVerweht
        Public PartikelGroesse As Integer
        Public PartikelGroesseZufall As Boolean
        Public WindStaerke As Integer
        Public WindStaerkeZufall As Boolean
    End Structure

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property TransitionName As String Implements ISlideShowTransition.TransitionName

        Get
            Return "Direkter Übergang"
        End Get

    End Property

    Public ReadOnly Property TransitionKurzBeschreibung As String Implements ISlideShowTransition.TransitionKurzBeschreibung

        Get
            Return "Bläst das alte Bild weg, bis das neue zum Vorschein kommt"
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

    Public Sub RunTransition(
        oldImage As BitmapImage,
        picBoxModeOld As PictureBoxSizeMode,
        newImage As BitmapImage,
        picBoxModeNew As PictureBoxSizeMode,
        clientSize As Size,
        Optional durationMs As Integer = 0) _
        Implements ISlideShowTransition.RunTransition
        'Erzeugt direkt den finalen Ziel-Frame und meldet ihn
        'als fertiges Transitionsergebnis.

        Dim sizeWPF As Windows.Size
        Dim zielFrame As RenderTargetBitmap

        If wurdeBereinigt Then

            Throw New ObjectDisposedException(NameOf(TransitionMain))

        End If

        BeendeUndBereinigeTransition()

        sizeWPF = New Windows.Size(clientSize.Width, clientSize.Height)

        zielFrame = Nothing

        Try

            transitionLaeuft = True

            RaiseEvent TransitionIsRunning(True)

            zielFrame = ConvertBitmapImageToRenderTargetBitmap(newImage, sizeWPF)

            'Dem aufrufenden Modul kurz Zeit zum Umschalten geben.
            System.Threading.Thread.Sleep(500)

            If Not transitionLaeuft Then
                Exit Sub
            End If

            RaiseEvent TransitionFrameIstFertig(zielFrame)

        Finally

            BeendeUndBereinigeTransition()

        End Try

    End Sub

    Public Sub StopTransition() Implements ISlideShowTransition.StopTransition
        'Beendet eine gegebenenfalls noch laufende Cut-Transition.

        BeendeUndBereinigeTransition()

    End Sub

#End Region

#Region "Optionsdialog"

    Public Function GetTransitionOptionsDialog() As UserControl Implements ISlideShowTransition.GetTransitionOptionsDialog
        'Stellt frmOptionsMain das - leere - ucOptionsTransition zur Verfügung

        Return New ucOptionsTransition

    End Function

#End Region

#Region "Bereinigung & Dispose"

    Private Sub BeendeUndBereinigeTransition()
        'Setzt den Laufzustand kontrolliert zurück.

        If Not transitionLaeuft Then
            Exit Sub
        End If

        transitionLaeuft = False

        RaiseEvent TransitionIsRunning(False)

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