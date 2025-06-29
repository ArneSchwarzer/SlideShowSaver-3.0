Imports System.IO
Imports System.Reflection
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowLogging

Public Class ShaderListLoader
    Public Shared Function LadeShaderInfoListe() As List(Of SlideShowShaderInfo)

        Dim shaderInfos As New List(Of SlideShowShaderInfo)
        Dim verzeichnis As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shader")
        Dim dateien() As String

        If Not Directory.Exists(verzeichnis) Then Return shaderInfos

        dateien = Directory.GetFiles(verzeichnis, "*.ssss")

        For Each dateipfad In dateien
            Try
                Dim asm As Assembly = Assembly.LoadFrom(dateipfad)
                For Each typ In asm.GetTypes()
                    If GetType(ISlideShowShader).IsAssignableFrom(typ) AndAlso Not typ.IsInterface AndAlso Not typ.IsAbstract Then
                        Dim dummy As ISlideShowShader = CType(Activator.CreateInstance(typ), ISlideShowShader)
                        Dim info As SlideShowShaderInfo = New SlideShowShaderInfo With {
                            .ShaderName = dummy.ShaderName,
                            .ShaderBeschreibung = dummy.ShaderKurzBeschreibung,
                            .ShaderVersion = dummy.ShaderVersion
                        }
                        shaderInfos.Add(info)
                        dummy = Nothing ' Dummy-Instanz explizit verwerfen
                        Exit For
                    End If
                Next
            Catch ex As Exception
                ' Logging
                LogHandling.LogError("Fehler beim Laden der Liste der Shader: " & ex.ToString)
            End Try
        Next

        Return shaderInfos
    End Function
End Class
