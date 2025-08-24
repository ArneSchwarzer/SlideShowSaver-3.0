Imports System.Windows.Forms
Imports SlideShowSprachen.LanguageHelper
Imports SlideShowTools.RegistryHandling
Imports SlideShowSprachen.LanguageSpecialHandling
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MyControlsLibrary

Public Class ucFlaggenstreifen
    Private Sub ucFlaggenstreifen_Load(sender As Object, e As EventArgs) Handles Me.Load

        FlaggenSetzen()

    End Sub



    Private Sub FlaggenSetzen()
        Dim regValSprache As String

        regValSprache = ReadFromRegistry(SLIDESHOWMAIN_PATH & "Sprache")

        For Each ctrl As Control In Me.Controls
            ' Sicherstellen, dass es ein Button ist UND ein Tag gesetzt ist
            If TypeOf ctrl Is Windows.Forms.Button AndAlso ctrl.Tag IsNot Nothing Then
                Dim btn As Windows.Forms.Button = DirectCast(ctrl, Windows.Forms.Button)
                Dim isoCode As String = btn.Tag.ToString()

                ' Die passende SprachInformation aus der Liste finden
                Dim sprachInfo As SprachInformation = SprachenListe.FirstOrDefault(Function(x) x.ISOCode = isoCode)

                ' Wenn gefunden, dann Flagge setzen
                If sprachInfo.ISOCode IsNot Nothing Then

                    'ToolTip Setzen
                    SprachSpezialHandling.SetzeTooltipsSprache(Me, ttSprachen)

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

    Private Sub FlaggeGeklickt(langIso As String)
        WriteToRegistry(SLIDESHOWMAIN_PATH & "Sprache", langIso)
        LanguageSpecialHandling.WendeSpracheAufAlleControls(Me.FindForm(), langIso)
        FlaggenSetzen()
    End Sub

    Private Sub btnLangDE_Click(sender As Object, e As EventArgs) Handles btnLangDE.Click
        FlaggeGeklickt("DE")
    End Sub

    Private Sub btnLangEN_Click(sender As Object, e As EventArgs) Handles btnLangEN.Click
        FlaggeGeklickt("EN")
    End Sub

    Private Sub btnLangFR_Click(sender As Object, e As EventArgs) Handles btnLangFR.Click
        FlaggeGeklickt("FR")
    End Sub

    Private Sub btnLangES_Click(sender As Object, e As EventArgs) Handles btnLangES.Click
        FlaggeGeklickt("ES")
    End Sub

    Private Sub btnLangPL_Click(sender As Object, e As EventArgs) Handles btnLangPL.Click
        FlaggeGeklickt("PL")
    End Sub

    Private Sub btnLangRU_Click(sender As Object, e As EventArgs) Handles btnLangRU.Click
        FlaggeGeklickt("RU")
    End Sub

    Private Sub btnLangHI_Click(sender As Object, e As EventArgs) Handles btnLangHI.Click
        FlaggeGeklickt("HI")
    End Sub

    Private Sub btnLangZH_Click(sender As Object, e As EventArgs) Handles btnLangZH.Click
        FlaggeGeklickt("ZH")
    End Sub

    Private Sub btnLangTLH_Click(sender As Object, e As EventArgs) Handles btnLangTLH.Click
        FlaggeGeklickt("TLH")
    End Sub

End Class
