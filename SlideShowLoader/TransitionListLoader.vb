Imports System.IO
Imports System.Reflection
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowInterfaces.InfoHandling
Imports SlideShowLogging

Public Class TransitionListLoader
    Public Shared Function LadeTransitionInfoListe() As List(Of SlideShowTransitionInfo)

        Dim transitionInfos As New List(Of SlideShowTransitionInfo)
        Dim verzeichnis As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Transitions")
        Dim dateien() As String

        If Not Directory.Exists(verzeichnis) Then Return transitionInfos

        dateien = Directory.GetFiles(verzeichnis, "*.ssst")

        For Each dateipfad In dateien
            Try
                Dim asm As Assembly = Assembly.LoadFrom(dateipfad)
                For Each typ In asm.GetTypes()
                    If GetType(ISlideShowTransition).IsAssignableFrom(typ) AndAlso Not typ.IsInterface AndAlso Not typ.IsAbstract Then
                        Dim dummy As ISlideShowTransition = CType(Activator.CreateInstance(typ), ISlideShowTransition)
                        Dim info As SlideShowTransitionInfo = New SlideShowTransitionInfo With {
                            .TransitionName = dummy.TransitionName,
                            .TransitionBeschreibung = dummy.TransitionKurzBeschreibung,
                            .TransitionVersion = dummy.TransitionVersion
                        }
                        transitionInfos.Add(info)
                        'LogHandling.LogDebug("SlideShowLoader - TransitionlistLoader.LadeTransitionInfoListe: Transition " & info.TransitionName & " erfolgreich geladen")
                        dummy = Nothing ' Dummy-Instanz verwerfen
                        Exit For
                    End If
                Next
            Catch ex As ReflectionTypeLoadException
                ' Fehlerbehandlung
                For Each lex In ex.LoaderExceptions
                    LogHandling.LogError("SlideShowLoader - TransitionListLoader.LadeTranstionInfoListe: Fehler beim Laden der Liste der Transitions: " & lex.Message)
                Next

            End Try

        Next

        Return transitionInfos
    End Function
End Class
