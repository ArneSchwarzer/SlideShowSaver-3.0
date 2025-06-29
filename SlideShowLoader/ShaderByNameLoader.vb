Imports System.IO
Imports System.Reflection
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowLogging

Public Class ShaderByNameLoader
    Public Shared Function LadeShaderNachName(shaderName As String) As ISlideShowShader

        Dim verzeichnis As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shader")
        Dim dateien() As String = Directory.GetFiles(verzeichnis, "*.ssss")

        For Each dateipfad In dateien
            Try
                Dim asm As Assembly = Assembly.LoadFrom(dateipfad)
                For Each typ In asm.GetTypes()
                    If GetType(ISlideShowShader).IsAssignableFrom(typ) AndAlso Not typ.IsInterface AndAlso Not typ.IsAbstract Then
                        Dim instanz As ISlideShowShader = CType(Activator.CreateInstance(typ), ISlideShowShader)
                        If instanz.ShaderName = shaderName Then
                            Return instanz
                        End If
                    End If
                Next
            Catch ex As Exception
                ' Fehlerbehandlung oder Logging
                LogHandling.LogError("Fehler beim Laden des Shaders " & shaderName & ": " & ex.ToString)
            End Try
        Next

        Return Nothing
    End Function
End Class
