Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Shapes

Public MustInherit Class MultipassRenderNodeBase

#Region "Klassenvariablen"

    Private ReadOnly containerIntern As Grid
    Private ReadOnly renderRectangleIntern As Rectangle
    Private ReadOnly outputBrushIntern As BitmapCacheBrush
    Private ReadOnly nodeNameIntern As String

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property Container As Grid
        Get
            Return containerIntern
        End Get
    End Property

    Public ReadOnly Property RenderRectangle As Rectangle
        Get
            Return renderRectangleIntern
        End Get
    End Property

    Public ReadOnly Property OutputBrush As BitmapCacheBrush
        Get
            Return outputBrushIntern
        End Get
    End Property

    Public ReadOnly Property NodeName As String
        Get
            Return nodeNameIntern
        End Get
    End Property

#End Region

#Region "Konstruktor"

    Protected Sub New(
        nodeName As String,
        container As Grid,
        renderRectangle As Rectangle)

        Dim bitmapCacheIntern As BitmapCache

        If String.IsNullOrWhiteSpace(nodeName) Then

            Throw New ArgumentException("Der Name des RenderNodes darf nicht leer sein.", NameOf(nodeName))

        End If

        If container Is Nothing Then

            Throw New ArgumentNullException(NameOf(container))

        End If

        If renderRectangle Is Nothing Then

            Throw New ArgumentNullException(NameOf(renderRectangle))

        End If

        nodeNameIntern = nodeName
        containerIntern = container
        renderRectangleIntern = renderRectangle
        bitmapCacheIntern = New BitmapCache With {.RenderAtScale = 1.0, .EnableClearType = False, .SnapsToDevicePixels = False}
        containerIntern.CacheMode = bitmapCacheIntern

        outputBrushIntern = New BitmapCacheBrush(containerIntern)

    End Sub

#End Region

#Region "Öffentliche Methoden"

    Public Sub Aktiviere()

        containerIntern.Visibility = Visibility.Visible
        containerIntern.IsHitTestVisible = False

        containerIntern.InvalidateVisual()
        renderRectangleIntern.InvalidateVisual()

    End Sub

    Public Sub Deaktiviere()

        containerIntern.Visibility = Visibility.Hidden

    End Sub

    Public Sub LoeseEffekt()

        renderRectangleIntern.Effect = Nothing

    End Sub

    Public Overridable Sub Aufraeumen()

        renderRectangleIntern.Effect = Nothing
        renderRectangleIntern.Fill = Nothing
        outputBrushIntern.Target = Nothing

    End Sub

#End Region

End Class