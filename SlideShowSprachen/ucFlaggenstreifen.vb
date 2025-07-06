Imports System.Windows.Forms
Imports SlideShowSprachen.LanguageHelper
Imports SlideShowTools.RegistryHandling

Public Class ucFlaggenstreifen
    Private Sub ucFlaggenstreifen_Load(sender As Object, e As EventArgs) Handles Me.Load

        FlaggenSetzen()

    End Sub



    Private Sub FlaggenSetzen()
        Dim regValSprache As String

        regValSprache = ReadFromRegistry(SLIDESHOWMAIN_PATH & "Sprache")

        For Each ctrl As Control In Me.Controls
            ' Sicherstellen, dass es ein Button ist UND ein Tag gesetzt ist
            If TypeOf ctrl Is Button AndAlso ctrl.Tag IsNot Nothing Then
                Dim btn As Button = DirectCast(ctrl, Button)
                Dim isoCode As String = btn.Tag.ToString()

                ' Die passende SprachInformation aus der Liste finden
                Dim sprachInfo As SprachInformation = SprachenListe.FirstOrDefault(Function(x) x.ISOCode = isoCode)

                ' Wenn gefunden, dann Flagge setzen
                If sprachInfo.ISOCode IsNot Nothing Then
                    If btn.Tag.ToString = regValSprache Then
                        btn.BackgroundImage = sprachInfo.FlaggeAktiv
                        btn.BackgroundImageLayout = ImageLayout.Zoom ' Optional, damit das Bild skaliert wird
                    Else
                        btn.BackgroundImage = sprachInfo.FlaggeInaktiv
                        btn.BackgroundImageLayout = ImageLayout.Zoom
                    End If
                End If

            End If
        Next
    End Sub

    Private Sub btnLangDE_Click(sender As Object, e As EventArgs) Handles btnLangDE.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "DE")
        FlaggenSetzen()

    End Sub

    Private Sub btnLangEN_Click(sender As Object, e As EventArgs) Handles btnLangEN.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "EN")
        FlaggenSetzen()

    End Sub

    Private Sub btnLangES_Click(sender As Object, e As EventArgs) Handles btnLangES.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "ES")
        FlaggenSetzen()

    End Sub

    Private Sub btnLangFR_Click(sender As Object, e As EventArgs) Handles btnLangFR.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "FR")
        FlaggenSetzen()

    End Sub

    Private Sub btnLangHI_Click(sender As Object, e As EventArgs) Handles btnLangHI.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "HI")
        FlaggenSetzen()

    End Sub

    Private Sub btnLangPL_Click(sender As Object, e As EventArgs) Handles btnLangPL.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "PL")
        FlaggenSetzen()

    End Sub

    Private Sub btnLangRU_Click(sender As Object, e As EventArgs) Handles btnLangRU.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "RU")
        FlaggenSetzen()

    End Sub

    Private Sub btnLangTLH_Click(sender As Object, e As EventArgs) Handles btnLangTLH.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "TLH")
        FlaggenSetzen()

    End Sub

    Private Sub btnLangZH_Click(sender As Object, e As EventArgs) Handles btnLangZH.Click

        ' Hier den LanguageManager Aufrufen

        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", "ZH")
        FlaggenSetzen()

    End Sub
End Class
