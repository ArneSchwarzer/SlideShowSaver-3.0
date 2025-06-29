
Imports System.IO
Imports System.Reflection
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging

Public Class TransitionByNameLoader
    Public Shared Function LadeTransitionNachName(transitionName As String) As ISlideShowTransition

        Dim verzeichnis As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Transitions")
        Dim dateien() As String = Directory.GetFiles(verzeichnis, "*.ssst")

        For Each dateipfad In dateien
            Try
                Dim asm As Assembly = Assembly.LoadFrom(dateipfad)
                For Each typ In asm.GetTypes()
                    If GetType(ISlideShowTransition).IsAssignableFrom(typ) AndAlso Not typ.IsInterface AndAlso Not typ.IsAbstract Then
                        Dim instanz As ISlideShowTransition = CType(Activator.CreateInstance(typ), ISlideShowTransition)
                        If instanz.TransitionName = transitionName Then
                            Return instanz
                        End If
                    End If
                Next
            Catch ex As Exception
                ' Fehlerbehandlung
                LogHandling.LogError("Fehler beim Laden von Transition " & transitionName & ": " & ex.ToString)
            End Try
        Next

        Return Nothing
    End Function
End Class
