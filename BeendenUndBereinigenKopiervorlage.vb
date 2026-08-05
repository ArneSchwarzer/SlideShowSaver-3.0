Imports Microsoft.VisualBasic

Public Class BeendenUndBEreinigen

    Private Sub BeendeUndBereinigeModul(ByRef modul As ISlideShowModul)

        If modul Is Nothing Then Exit Sub

        Try

            modul.StopModul()
            modul.Dispose()

        Finally

            modul = Nothing

        End Try

    End Sub

    Private Sub BeendeUndBereinigeTransition(ByRef transition As ISlideShowTransition)

        If transition Is Nothing Then Exit Sub

        Try

            transition.StopTransition()
            transition.Dispose()

        Finally

            transition = Nothing

        End Try

    End Sub

    Private Sub BeendeUndBereinigeShader(ByRef shader As ISlideShowShader)

        If shader Is Nothing Then Exit Sub

        Try

            shader.Dispose()

        Finally

            shader = Nothing

        End Try

    End Sub


End Class
