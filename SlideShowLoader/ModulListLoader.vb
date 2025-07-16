Imports System.IO
Imports System.Reflection
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowLogging

Public Class ModulListLoader
    Public Shared Function LadeModulInfoListe() As List(Of SlideShowModulInfo)

        Dim modulInfos As New List(Of SlideShowModulInfo)
        Dim modulVerzeichnis As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Module")
        Dim modulDateien() As String

        If Not Directory.Exists(modulVerzeichnis) Then Return modulInfos

        modulDateien = Directory.GetFiles(modulVerzeichnis, "*.sssm")

        For Each modulPfad In modulDateien
            Try
                Dim asm As Assembly = Assembly.LoadFrom(modulPfad)
                For Each typ In asm.GetTypes()
                    If GetType(ISlideShowModul).IsAssignableFrom(typ) AndAlso Not typ.IsInterface AndAlso Not typ.IsAbstract Then
                        Dim dummy As ISlideShowModul = CType(Activator.CreateInstance(typ), ISlideShowModul)
                        Dim info As SlideShowModulInfo = New SlideShowModulInfo With {
                            .ModulName = dummy.ModulName,
                            .ModulBeschreibung = dummy.ModulBeschreibung,
                            .ModulNutztSlideShowBildauswahl = dummy.ModulNutztSlideShowBildauswahl,
                            .ModulNutztTransitions = dummy.ModulNutztTransitions,
                            .ModulNutztShader = dummy.ModulNutztShader,
                            .ModulVersion = dummy.ModulVersion
                            }
                        modulInfos.Add(info)
                        LogHandling.LogDebug("SlideShowLoader - ModullistLoader.LadeModulInfoListe: Modul " & info.ModulName & " erfolgreich geladen")
                        dummy = Nothing ' Dummy-Instanz sofort wieder freigeben
                        Exit For
                    End If
                Next
            Catch ex As Exception
                ' Fehlerbehandlung
                LogHandling.LogError("SlideShowLoader - ModullistLoader.LadeModulInfoListe: Fehler beim Laden der Liste der Module: " & ex.Message)
            End Try

        Next

        Return modulInfos
    End Function
End Class
