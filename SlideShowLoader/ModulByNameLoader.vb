Imports System.IO
Imports System.Reflection
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging

Public Class ModulByNameLoader
    Public Shared Function LadeModulNachName(modulName As String) As ISlideShowModul

        Dim modulVerzeichnis As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Module")
        Dim modulDateien() As String = Directory.GetFiles(modulVerzeichnis, "*.sssm")

        For Each modulPfad In modulDateien
            Try
                Dim asm As Assembly = Assembly.LoadFrom(modulPfad)
                For Each typ In asm.GetTypes()
                    If GetType(ISlideShowModul).IsAssignableFrom(typ) AndAlso Not typ.IsInterface AndAlso Not typ.IsAbstract Then
                        Dim instanz As ISlideShowModul = CType(Activator.CreateInstance(typ), ISlideShowModul)
                        If instanz.ModulName = modulName Then
                            Return instanz
                        End If
                    End If
                Next
            Catch ex As Exception
                ' Fehlerbehandlung
                LogHandling.LogError("Fehler beim Laden des Moduls " & modulName & ": " & ex.ToString)
            End Try
        Next

        Return Nothing ' Falls kein Modul mit passendem Namen gefunden wurde
    End Function
End Class
