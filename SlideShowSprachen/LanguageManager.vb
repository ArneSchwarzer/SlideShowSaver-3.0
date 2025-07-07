' SprachSpezialHandling.vb – Enthält die direkte Sprachumschaltung inkl. ToolTip- und Speziallogik

Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowSprachen.LanguageHelper
Imports MyControlsLibrary

Public Class SprachSpezialHandling

    Public Shared Sub WendeSpracheAufAlleControls(anwenderSteuerbereich As Control, aktuelleSprache As String)
        For Each ctrl As Control In anwenderSteuerbereich.Controls

            ' Tag verarbeiten – z. B. langKey=xyz;STYLE_KLINGON
            Dim tagText As String = TryCast(ctrl.Tag, String)
            If Not String.IsNullOrWhiteSpace(tagText) Then
                Dim flags = tagText.Split(";"c)

                For Each flag In flags
                    If flag.StartsWith("langKey=") Then
                        Dim key = flag.Substring("langKey=".Length)
                        'Muss erst noch gebaut werden!
                        'ctrl.Text = SprachCSVManager.HoleText(key, aktuelleSprache)
                    ElseIf flag = "NO_TRANSLATE" Then
                        ' überspringen
                    ElseIf flag.StartsWith("STYLE_") Then
                        WendeStilAn(ctrl, flag, aktuelleSprache)
                    End If
                Next
            End If

            ' Rekursiv in Unter-Controls, z. B. bei Panels, Tabs etc.
            If ctrl.HasChildren Then
                WendeSpracheAufAlleControls(ctrl, aktuelleSprache)
            End If
        Next
    End Sub

    Private Shared Sub WendeStilAn(ctrl As Control, stilFlag As String, aktuelleSprache As String)
        If stilFlag = "STYLE_KLINGON" AndAlso aktuelleSprache = "TLH" Then
            If TypeOf ctrl Is SterneBewertungControl Then
                DirectCast(ctrl, SterneBewertungControl).SternFarbeAktiv = Color.DarkRed
            End If
        ElseIf stilFlag = "STYLE_MATRIX" AndAlso aktuelleSprache = "ZH" Then
            If TypeOf ctrl Is Label Then
                ctrl.ForeColor = Color.Lime
            End If
        End If
    End Sub

    Private Shared toolTipFontZuordnung As New Dictionary(Of Control, Font)

    Public Shared Sub SetzeTooltipsSprache(ctrl As Control, toolTipCtrl As ToolTip)
        toolTipCtrl.OwnerDraw = True
        AddHandler toolTipCtrl.Draw, AddressOf ToolTip_Draw
        AddHandler toolTipCtrl.Popup, AddressOf ToolTip_Popup

        For Each unterControl As Control In ctrl.Controls
            If unterControl.Tag IsNot Nothing Then
                Dim isoCode As String = unterControl.Tag.ToString()

                Dim info = SprachenListe.FirstOrDefault(Function(x) x.ISOCode = isoCode)
                If Not String.IsNullOrWhiteSpace(info.ToolTipText) Then
                    toolTipCtrl.SetToolTip(unterControl, info.ToolTipText)
                    toolTipFontZuordnung(unterControl) = info.Font
                End If
            End If
        Next
    End Sub



    Private Shared Sub ToolTip_Popup(sender As Object, e As PopupEventArgs)
        Dim font As Font = If(toolTipFontZuordnung.ContainsKey(e.AssociatedControl),
                              toolTipFontZuordnung(e.AssociatedControl),
                              New Font("Segoe UI", 10))

        Using g As Graphics = e.AssociatedControl.CreateGraphics()
            Dim text = DirectCast(sender, ToolTip).GetToolTip(e.AssociatedControl)
            Dim size = g.MeasureString(text, font)
            e.ToolTipSize = size.ToSize()
        End Using
    End Sub

    Private Shared Sub ToolTip_Draw(sender As Object, e As DrawToolTipEventArgs)
        Dim font As Font = If(toolTipFontZuordnung.ContainsKey(e.AssociatedControl),
                              toolTipFontZuordnung(e.AssociatedControl),
                              New Font("Segoe UI", 10))

        e.DrawBackground()
        e.DrawBorder()
        e.Graphics.DrawString(e.ToolTipText, font, Brushes.Black, New PointF(2, 2))
    End Sub


End Class
