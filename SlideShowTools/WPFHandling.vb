Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Threading



Public Class WPFHandling

        Private Shared overlayWindow As DrawingVisualWindow
        Private Shared drawTimer As DispatcherTimer
        Private Shared drawDelegate As Action(Of DrawingContext, Size)

        ''' <summary>
        ''' Startet eine WPF-Visual-Transition mit der angegebenen Zeichenaktion.
        ''' </summary>
        ''' <param name="drawAction">Methode, die pro Frame aufgerufen wird und in den DrawingContext zeichnet.</param>
        ''' <param name="frameIntervalMs">Optionaler Frame-Intervall in Millisekunden (Standard: 33 ms = ~30 FPS).</param>
        Public Shared Sub StartVisualTransition(drawAction As Action(Of DrawingContext, Size),
                                                Optional frameIntervalMs As Integer = 33)

            ' Falls bereits ein Fenster aktiv ist, beenden
            StopVisualTransition()

            drawDelegate = drawAction
            overlayWindow = New DrawingVisualWindow()
            AddHandler overlayWindow.ContentRendered, Sub()
                                                          ' Timer starten, sobald das Fenster bereit ist
                                                          drawTimer = New DispatcherTimer()
                                                          AddHandler drawTimer.Tick, AddressOf OnDrawFrame
                                                          drawTimer.Interval = TimeSpan.FromMilliseconds(frameIntervalMs)
                                                          drawTimer.Start()
                                                      End Sub

            overlayWindow.Show()
        End Sub

        ''' <summary>
        ''' Beendet die laufende Visual-Transition und schließt das WPF-Fenster.
        ''' </summary>
        Public Shared Sub StopVisualTransition()
            Try
                If drawTimer IsNot Nothing Then
                    drawTimer.Stop()
                    RemoveHandler drawTimer.Tick, AddressOf OnDrawFrame
                    drawTimer = Nothing
                End If

                If overlayWindow IsNot Nothing Then
                    overlayWindow.Close()
                    overlayWindow = Nothing
                End If
            Catch ex As Exception
                ' Log-Eintrag möglich
            End Try
        End Sub

        Private Shared Sub OnDrawFrame(sender As Object, e As EventArgs)
            If overlayWindow Is Nothing OrElse drawDelegate Is Nothing Then Exit Sub

            Dim dc As DrawingContext = overlayWindow.RenderVisual.RenderOpen()
            Try
                drawDelegate.Invoke(dc, New Size(overlayWindow.ActualWidth, overlayWindow.ActualHeight))
            Finally
                dc.Close()
            End Try
        End Sub

        ''' <summary>
        ''' Internes Vollbild-Fenster mit DrawingVisual-Zeichenfläche.
        ''' </summary>
        Private Class DrawingVisualWindow
            Inherits Window

            Public ReadOnly RenderVisual As DrawingVisual

            Public Sub New()
                Me.WindowStyle = WindowStyle.None
                Me.ResizeMode = ResizeMode.NoResize
                Me.WindowState = WindowState.Maximized
                Me.Topmost = True
                Me.AllowsTransparency = True
                Me.Background = Brushes.Transparent
                Me.ShowInTaskbar = False

                RenderVisual = New DrawingVisual()
                Dim host = New VisualHost(RenderVisual)
                Me.Content = host
            End Sub
        End Class

        ''' <summary>
        ''' Host für das Visual (stellt sicher, dass es gerendert wird).
        ''' </summary>
        Private Class VisualHost
            Inherits FrameworkElement

            Private ReadOnly _visual As Visual

            Public Sub New(visual As Visual)
                _visual = visual
            End Sub

            Protected Overrides Function GetVisualChild(index As Integer) As Visual
                Return _visual
            End Function

            Protected Overrides ReadOnly Property VisualChildrenCount As Integer
                Get
                    Return 1
                End Get
            End Property
        End Class

    End Class


