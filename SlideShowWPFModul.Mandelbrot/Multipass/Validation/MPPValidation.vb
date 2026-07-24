Imports System
Imports System.Globalization
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports SlideShowLogging

Public NotInheritable Class MPPValidation
    Implements IDisposable

#Region "Konstanten"

    Private Const StandardMaxPasses As Integer = 8
    Private Const StandardWarteFrames As Integer = 30

#End Region

#Region "Klassenvariablen"

    Private ReadOnly seedContainerIntern As Grid
    Private ReadOnly seedRectangleIntern As Rectangle
    Private ReadOnly nodeAContainerIntern As Grid
    Private ReadOnly nodeARectangleIntern As Rectangle
    Private ReadOnly nodeBContainerIntern As Grid
    Private ReadOnly nodeBRectangleIntern As Rectangle
    Private ReadOnly outputContainerIntern As Grid
    Private ReadOnly outputRectangleIntern As Rectangle

    Private seedNodeIntern As MPPValidationSeedNode
    Private nodeAIntern As MPPValidationContinueNode
    Private nodeBIntern As MPPValidationContinueNode
    Private outputNodeIntern As MPPValidationOutputNode

    Private aktuellerQuellNodeIntern As MultipassRenderNodeBase
    Private aktuellerZielNodeIntern As MPPValidationContinueNode

    Private aktuellerPassIntern As Integer
    Private maxPassesIntern As Integer
    Private warteFramesIntern As Integer
    Private warteFramesProPassIntern As Integer

    Private renderingHandlerAktivIntern As Boolean
    Private istGestartetIntern As Boolean
    Private istDisposedIntern As Boolean

#End Region

#Region "Konstruktor"

    Public Sub New(
        seedContainer As Grid,
        seedRectangle As Rectangle,
        nodeAContainer As Grid,
        nodeARectangle As Rectangle,
        nodeBContainer As Grid,
        nodeBRectangle As Rectangle,
        outputContainer As Grid,
        outputRectangle As Rectangle)

        PruefeVisualArgument(seedContainer, NameOf(seedContainer))
        PruefeVisualArgument(seedRectangle, NameOf(seedRectangle))
        PruefeVisualArgument(nodeAContainer, NameOf(nodeAContainer))
        PruefeVisualArgument(nodeARectangle, NameOf(nodeARectangle))
        PruefeVisualArgument(nodeBContainer, NameOf(nodeBContainer))
        PruefeVisualArgument(nodeBRectangle, NameOf(nodeBRectangle))
        PruefeVisualArgument(outputContainer, NameOf(outputContainer))
        PruefeVisualArgument(outputRectangle, NameOf(outputRectangle))

        seedContainerIntern = seedContainer
        seedRectangleIntern = seedRectangle
        nodeAContainerIntern = nodeAContainer
        nodeARectangleIntern = nodeARectangle
        nodeBContainerIntern = nodeBContainer
        nodeBRectangleIntern = nodeBRectangle
        outputContainerIntern = outputContainer
        outputRectangleIntern = outputRectangle

        maxPassesIntern = StandardMaxPasses
        warteFramesProPassIntern = StandardWarteFrames

    End Sub

#End Region

#Region "Öffentliche Methoden"

    Public Sub Start(
        Optional maxPasses As Integer = StandardMaxPasses,
        Optional warteFramesProPass As Integer = StandardWarteFrames)

        PruefeNichtDisposed()

        If istGestartetIntern Then
            Return
        End If

        If maxPasses <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(maxPasses))
        End If

        If warteFramesProPass <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(warteFramesProPass))
        End If

        maxPassesIntern = maxPasses
        warteFramesProPassIntern = warteFramesProPass

        ErzeugeNodes()

        seedNodeIntern.Aktiviere()
        nodeAIntern.Aktiviere()
        nodeBIntern.Aktiviere()
        outputNodeIntern.Aktiviere()

        nodeAIntern.SetzePassInformation(1, maxPassesIntern)
        nodeAIntern.SetzeEingang(seedNodeIntern.OutputBrush)

        aktuellerQuellNodeIntern = nodeAIntern
        aktuellerZielNodeIntern = nodeBIntern

        outputNodeIntern.SetzeEingang(nodeAIntern.OutputBrush)

        aktuellerPassIntern = 1
        warteFramesIntern = 0
        istGestartetIntern = True

        AddHandler CompositionTarget.Rendering,
            AddressOf CompositionTargetRendering

        renderingHandlerAktivIntern = True

        LogHandling.LogInfo(
            "Modul Mandelbrot: MPPValidation gestartet. " &
            "Pass 1 wurde auf Node A eingerichtet.")

    End Sub

    Public Sub [Stop]()

        If renderingHandlerAktivIntern Then

            RemoveHandler CompositionTarget.Rendering,
                AddressOf CompositionTargetRendering

            renderingHandlerAktivIntern = False

        End If

        If aktuellerQuellNodeIntern IsNot Nothing AndAlso
           outputNodeIntern IsNot Nothing Then

            outputNodeIntern.SetzeEingang(
                aktuellerQuellNodeIntern.OutputBrush)

        End If

        istGestartetIntern = False

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        If istDisposedIntern Then
            Return
        End If

        [Stop]()
        RaeumeNodesAuf()

        istDisposedIntern = True

        GC.SuppressFinalize(Me)

    End Sub

#End Region

#Region "Rendering"

    Private Sub CompositionTargetRendering(
        sender As Object,
        e As EventArgs)

        If Not renderingHandlerAktivIntern Then
            Return
        End If

        warteFramesIntern += 1

        If warteFramesIntern < warteFramesProPassIntern Then
            Return
        End If

        warteFramesIntern = 0

        If aktuellerPassIntern >= maxPassesIntern Then

            BeendeValidierungErfolgreich()

            Return

        End If

        FuehreNaechstenPassAus()

    End Sub

    Private Sub FuehreNaechstenPassAus()

        Dim quellNode As MultipassRenderNodeBase
        Dim zielNode As MPPValidationContinueNode
        Dim neuerZielNode As MPPValidationContinueNode
        Dim naechsterPass As Integer

        quellNode = aktuellerQuellNodeIntern
        zielNode = aktuellerZielNodeIntern
        naechsterPass = aktuellerPassIntern + 1

        If quellNode Is Nothing Then

            LogHandling.LogWarn(
                "MPPValidation: QuellNode ist Nothing.")

            [Stop]()

            Return

        End If

        If zielNode Is Nothing Then

            LogHandling.LogWarn(
                "MPPValidation: ZielNode ist Nothing.")

            [Stop]()

            Return

        End If

        zielNode.SetzePassInformation(
            naechsterPass,
            maxPassesIntern)

        zielNode.SetzeEingang(
            quellNode.OutputBrush)

        outputNodeIntern.SetzeEingang(
            zielNode.OutputBrush)

        aktuellerPassIntern = naechsterPass

        LogHandling.LogInfo(
            "MPPValidation: Pass " &
            aktuellerPassIntern.ToString() &
            ", Quelle=" &
            quellNode.NodeName &
            ", Ziel=" &
            zielNode.NodeName &
            ", Rotwert=" &
            (CDbl(aktuellerPassIntern) /
             CDbl(maxPassesIntern)).ToString(
                "F3",
                CultureInfo.InvariantCulture) &
            ".")

        aktuellerQuellNodeIntern = zielNode

        If zielNode Is nodeAIntern Then
            neuerZielNode = nodeBIntern
        Else
            neuerZielNode = nodeAIntern
        End If

        aktuellerZielNodeIntern = neuerZielNode

    End Sub

    Private Sub BeendeValidierungErfolgreich()

        [Stop]()

        LogHandling.LogInfo(
            "MPPValidation: Test nach " &
            aktuellerPassIntern.ToString() &
            " Pässen beendet. Letzter Node=" &
            If(
                aktuellerQuellNodeIntern Is Nothing,
                "Nothing",
                aktuellerQuellNodeIntern.NodeName) &
            ".")

    End Sub

#End Region

#Region "Node-Verwaltung"

    Private Sub ErzeugeNodes()

        RaeumeNodesAuf()

        seedNodeIntern =
            New MPPValidationSeedNode(
                "Seed",
                seedContainerIntern,
                seedRectangleIntern)

        nodeAIntern =
            New MPPValidationContinueNode(
                "Node A",
                nodeAContainerIntern,
                nodeARectangleIntern)

        nodeBIntern =
            New MPPValidationContinueNode(
                "Node B",
                nodeBContainerIntern,
                nodeBRectangleIntern)

        outputNodeIntern =
            New MPPValidationOutputNode(
                "Output",
                outputContainerIntern,
                outputRectangleIntern)

    End Sub

    Private Sub RaeumeNodesAuf()

        If outputNodeIntern IsNot Nothing Then
            outputNodeIntern.Aufraeumen()
        End If

        If nodeBIntern IsNot Nothing Then
            nodeBIntern.Aufraeumen()
        End If

        If nodeAIntern IsNot Nothing Then
            nodeAIntern.Aufraeumen()
        End If

        If seedNodeIntern IsNot Nothing Then
            seedNodeIntern.Aufraeumen()
        End If

        seedNodeIntern = Nothing
        nodeAIntern = Nothing
        nodeBIntern = Nothing
        outputNodeIntern = Nothing

        aktuellerQuellNodeIntern = Nothing
        aktuellerZielNodeIntern = Nothing

        aktuellerPassIntern = 0
        warteFramesIntern = 0

    End Sub

#End Region

#Region "Hilfsmethoden"

    Private Shared Sub PruefeVisualArgument(
        value As Object,
        parameterName As String)

        If value Is Nothing Then
            Throw New ArgumentNullException(parameterName)
        End If

    End Sub

    Private Sub PruefeNichtDisposed()

        If istDisposedIntern Then
            Throw New ObjectDisposedException(NameOf(MPPValidation))
        End If

    End Sub

#End Region

End Class
