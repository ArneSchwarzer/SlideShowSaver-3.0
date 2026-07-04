' ===========================
' LanguageSpecialHandling.vb
' Vereinheitlichte Lokalisierung (ersetzt alte LanguageManager/LanguageSpecialHelper)
' ===========================
Option Strict On
Option Explicit On
Imports System.Drawing
Imports System.Reflection
Imports System.Windows.Forms
' CSV-Lader:
'  -> LanguageCSVManager.HoleText(ownerName, iso, key, asm)
' Sprachinfos (Fonts):
'  -> LanguageHelper.SprachenListe (Structure mit ISOCode, Font, ToolTipFont, ...)
' Passe ggf. die Imports/Namespaces an dein Projekt an:

' Reentrancy-Schutz: Während Apply ignorieren Handlers ggf. ihre Arbeit
Friend Module LocalizationState
    <ThreadStatic> Public IsApplying As Boolean
End Module

Public NotInheritable Class LanguageSpecialHandling

    Private Sub New()
    End Sub

    ' -------------------------------------------
    ' Öffentliche API
    ' -------------------------------------------

    ''' <summary>
    ''' Lokalisierung auf das gesamte Control (Form oder UserControl) anwenden.
    ''' Tipp: Bei Forms im Shown-Event aufrufen; bei UCs mit ApplyDeferred (siehe unten).
    ''' </summary>
    Public Shared Sub ApplyTo(root As Control, iso As String)
        If root Is Nothing OrElse String.IsNullOrWhiteSpace(iso) Then Exit Sub

        LocalizationState.IsApplying = True
        root.SuspendLayout()
        Try
            ' 1) Schriftwechsel je Sprache
            ApplyFontForLanguage(root, iso)

            ' 2) Rekursiv alle Controls lokalisieren
            LocalizeTree(root, iso)

            ' 3) Klingon-Style (nach Texten)
            If iso.Equals("TLH", StringComparison.OrdinalIgnoreCase) Then
                ApplyKlingonStyle(root)
            End If
        Finally
            root.ResumeLayout(True)
            LocalizationState.IsApplying = False
        End Try
    End Sub

    ''' <summary>
    ''' Für UserControls, die kein Shown-Event haben: verzögert anwenden,
    ''' sobald Handle & Layout stehen.
    ''' </summary>
    Public Shared Sub ApplyDeferred(root As Control, iso As String)
        If root Is Nothing Then Exit Sub

        ' Einmaliger Handler: nach Handle-Erzeugung lokalisieren
        If Not root.IsHandleCreated Then
            Dim h As EventHandler = Nothing
            h = Sub(sender As Object, e As EventArgs)
                    ' nur einmal ausführen
                    RemoveHandler root.HandleCreated, h
                    ' erst nach dem Aufbau in die Nachrichten­schlange posten
                    root.BeginInvoke(CType(Sub() ApplyTo(root, iso), MethodInvoker))
                End Sub
            AddHandler root.HandleCreated, h
            Return
        End If

        ' Handle existiert schon → direkt (asynchron) anwenden
        root.BeginInvoke(CType(Sub() ApplyTo(root, iso), MethodInvoker))
    End Sub


    ''' <summary>
    ''' Backwards-Compat-Alias (falls alter Name im Code steckt).
    ''' </summary>
    Public Shared Sub WendeSpracheAufAlleControls(root As Control, iso As String)
        ApplyTo(root, iso)
    End Sub

    ''' <summary>
    ''' ToolTips für Sprache konfigurieren (OwnerDraw & passende Font).
    ''' Einmalig pro ToolTip-Instanz.
    ''' </summary>
    Public Shared Sub SetzeTooltipsSprache(owner As Control, tt As ToolTip, Optional iso As String = Nothing)
        If owner Is Nothing OrElse tt Is Nothing Then Exit Sub
        If String.IsNullOrWhiteSpace(iso) Then iso = "DE"

        RemoveHandler tt.Popup, AddressOf ToolTip_Popup
        RemoveHandler tt.Draw, AddressOf ToolTip_Draw
        tt.OwnerDraw = True
        AddHandler tt.Popup, AddressOf ToolTip_Popup
        AddHandler tt.Draw, AddressOf ToolTip_Draw

        tt.Tag = ChooseToolTipFont(owner, iso)
    End Sub

    ' -------------------------------------------
    ' Kernlokalisierung
    ' -------------------------------------------

    Private Shared Sub LocalizeTree(c As Control, iso As String)
        ' 1) Schlüssel bestimmen (Tag: langKey=..., sonst Name)
        Dim key As String = ExtractLangKey(c)

        ' 2) Owner-Kandidaten: Form → Top-Level-UserControls → Control-Typname
        Dim owners As List(Of String) = BuildOwnerCandidates(c)

        ' 3) Text nachschlagen & anwenden
        If Not String.IsNullOrWhiteSpace(key) Then
            Dim txt As String = LookupText(owners, iso, key, c.[GetType]().Assembly)
            If txt IsNot Nothing Then
                ApplyText(c, txt)
            End If
        End If

        ' 4) Spezialfälle
        If TypeOf c Is ToolStrip Then
            LocalizeToolStripItems(DirectCast(c, ToolStrip), owners, iso)
        ElseIf TypeOf c Is TabControl Then
            LocalizeTabPages(DirectCast(c, TabControl), owners, iso)
        End If

        ' 5) Kinder
        For Each child As Control In c.Controls
            LocalizeTree(child, iso)
        Next
    End Sub

    Private Shared Sub ApplyText(ctrl As Control, text As String)
        If TypeOf ctrl Is ComboBox Then
            ApplyComboBoxItems(DirectCast(ctrl, ComboBox), text)
        Else
            ctrl.Text = text
        End If
    End Sub

    Private Shared Sub LocalizeToolStripItems(ts As ToolStrip, owners As List(Of String), iso As String)
        For Each it As ToolStripItem In ts.Items
            Dim key As String = GetKey(it)
            If key Is Nothing Then Continue For

            Dim txt As String = LookupText(owners, iso, key, ts.FindForm()?.[GetType]().Assembly)
            If txt IsNot Nothing Then it.Text = txt

            If TypeOf it Is ToolStripDropDownItem Then
                LocalizeDropDownItems(DirectCast(it, ToolStripDropDownItem), owners, iso)
            End If
        Next
    End Sub

    Private Shared Sub LocalizeDropDownItems(dd As ToolStripDropDownItem, owners As List(Of String), iso As String)
        For Each it As ToolStripItem In dd.DropDownItems
            Dim key As String = GetKey(it)
            If key Is Nothing Then Continue For

            Dim txt As String = LookupText(owners, iso, key, dd.Owner?.FindForm()?.[GetType]().Assembly)
            If txt IsNot Nothing Then it.Text = txt

            If TypeOf it Is ToolStripDropDownItem Then
                LocalizeDropDownItems(DirectCast(it, ToolStripDropDownItem), owners, iso)
            End If
        Next
    End Sub

    Private Shared Sub LocalizeTabPages(tc As TabControl, owners As List(Of String), iso As String)
        For Each page As TabPage In tc.TabPages
            Dim key As String = GetKey(page)
            If key Is Nothing Then Continue For

            Dim txt As String = LookupText(owners, iso, key, tc.FindForm()?.[GetType]().Assembly)
            If txt IsNot Nothing Then page.Text = txt
        Next
    End Sub

    ' -------------------------------------------
    ' Schlüssel/Owner/Lookup-Helpers
    ' -------------------------------------------

    Private Shared Function ExtractLangKey(c As Control) As String
        Dim tagText As String = TryCast(c.Tag, String)
        If Not String.IsNullOrWhiteSpace(tagText) AndAlso tagText.IndexOf("langKey=", StringComparison.OrdinalIgnoreCase) >= 0 Then
            For Each part In tagText.Split(";"c)
                If part.TrimStart().StartsWith("langKey=", StringComparison.OrdinalIgnoreCase) Then
                    Return part.Substring("langKey=".Length).Trim()
                End If
            Next
        End If
        Return c.Name
    End Function

    Private Shared Function GetKey(it As ToolStripItem) As String
        Dim t As String = TryCast(it.Tag, String)
        If Not String.IsNullOrWhiteSpace(t) Then Return t
        Return it.Name
    End Function

    Private Shared Function GetKey(tp As TabPage) As String
        Dim t As String = TryCast(tp.Tag, String)
        If Not String.IsNullOrWhiteSpace(t) Then Return t
        Return tp.Name
    End Function

    Private Shared Function BuildOwnerCandidates(c As Control) As List(Of String)
        Dim owners As New List(Of String)(8)

        ' (1) Form
        Dim frm As Form = c.FindForm()
        If frm IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(frm.Name) Then
            owners.Add(frm.Name)
        End If

        ' (2) Top-Level-UserControls
        Dim ucNames As New List(Of String)(8)
        Dim p As Control = c
        While p IsNot Nothing
            If TypeOf p Is UserControl AndAlso Not String.IsNullOrWhiteSpace(p.Name) Then
                ucNames.Add(p.Name)
            End If
            p = p.Parent
        End While
        ucNames.Reverse()
        owners.AddRange(ucNames)

        ' (3) Control-Typ
        Dim tName As String = c.GetType().Name
        If Not String.IsNullOrWhiteSpace(tName) Then owners.Add(tName)

        ' Distinct
        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim result As New List(Of String)(owners.Count)
        For Each s In owners
            If seen.Add(s) Then result.Add(s)
        Next
        Return result
    End Function

    Private Shared Function LookupText(owners As List(Of String), iso As String, key As String, asm As Assembly) As String
        For Each owner In owners
            Dim txt As String = LanguageCSVManager.HoleText(owner, iso, key, asm)
            If txt IsNot Nothing Then Return txt
        Next
        Return Nothing
    End Function

    ' -------------------------------------------
    ' ComboBox-Mehrzeilen
    ' -------------------------------------------

    Private Shared Sub ApplyComboBoxItems(cmb As ComboBox, multiline As String)
        Dim oldText As String = TryCast(cmb.SelectedItem, String)

        cmb.BeginUpdate()
        Try
            cmb.Items.Clear()
            Dim t As String = NormalizeLineBreaks(multiline)
            Dim lines() As String = t.Split(New String() {vbLf}, StringSplitOptions.None)
            For Each line As String In lines
                cmb.Items.Add(line)
            Next

            ' Auswahl nach Text wiederherstellen (robust bei anderer Reihenfolge)
            If oldText IsNot Nothing Then
                Dim idx As Integer = cmb.Items.IndexOf(oldText)
                If idx >= 0 Then cmb.SelectedIndex = idx
            ElseIf cmb.Items.Count > 0 AndAlso cmb.SelectedIndex < 0 Then
                cmb.SelectedIndex = 0
            End If
        Finally
            cmb.EndUpdate()
        End Try
    End Sub

    Private Shared Function NormalizeLineBreaks(s As String) As String
        If s Is Nothing Then Return String.Empty
        ' echte CRLF/CR in LF umwandeln; Literal "\n" ebenfalls
        Return s.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Replace("\r\n", vbLf).Replace("\n", vbLf)
    End Function

    ' -------------------------------------------
    ' Fonts / ToolTips
    ' -------------------------------------------

    Private Shared Sub ApplyFontForLanguage(root As Control, iso As String)
        ' 1) Aus LanguageHelper.SprachenListe (Structure → nie Nothing!)
        Try
            Dim found As SlideShowSprachen.LanguageHelper.SprachInformation
            Dim got As Boolean = False
            For Each si In LanguageHelper.SprachenListe
                If si.ISOCode IsNot Nothing AndAlso si.ISOCode.Equals(iso, StringComparison.OrdinalIgnoreCase) Then
                    found = si : got = True : Exit For
                End If
            Next
            If got AndAlso found.Font IsNot Nothing Then
                ApplyFontRecursive(root, found.Font)
                Return
            End If
        Catch
            ' ignoriere und nutze Heuristik
        End Try

        ' 2) Heuristik
        Dim family As String = "Segoe UI"
        If iso.StartsWith("ZH", StringComparison.OrdinalIgnoreCase) Then
            family = "Microsoft YaHei UI"
        ElseIf iso.StartsWith("HI", StringComparison.OrdinalIgnoreCase) Then
            family = "Nirmala UI"
        End If
        Dim f As New Font(family, root.Font.Size, root.Font.Style)
        ApplyFontRecursive(root, f)
    End Sub

    Private Shared Sub ApplyFontRecursive(c As Control, f As Font)
        Try : c.Font = f : Catch : End Try
        For Each ch As Control In c.Controls
            ApplyFontRecursive(ch, f)
        Next
    End Sub

    Private Shared Function ChooseToolTipFont(owner As Control, iso As String) As Font
        ' Versuche gezielte Font aus SprachenListe
        Try
            Dim found As SlideShowSprachen.LanguageHelper.SprachInformation
            Dim got As Boolean = False
            For Each si In LanguageHelper.SprachenListe
                If si.ISOCode IsNot Nothing AndAlso si.ISOCode.Equals(iso, StringComparison.OrdinalIgnoreCase) Then
                    found = si : got = True : Exit For
                End If
            Next
            If got Then
                'If found.ToolTipFont IsNot Nothing Then Return found.ToolTipFont
                If found.Font IsNot Nothing Then Return New Font(found.Font, FontStyle.Regular)
            End If
        Catch
        End Try

        ' Heuristik
        Dim baseSize As Single = Math.Max(8.0F, owner.Font.Size - 0.5F)
        Dim family As String = "Segoe UI"
        If iso.StartsWith("ZH", StringComparison.OrdinalIgnoreCase) Then
            family = "Microsoft YaHei UI"
        ElseIf iso.StartsWith("HI", StringComparison.OrdinalIgnoreCase) Then
            family = "Nirmala UI"
        End If
        Return New Font(family, baseSize, FontStyle.Regular)
    End Function

    Private Shared Sub ToolTip_Popup(sender As Object, e As PopupEventArgs)
        ' optional: Größe an Schrift anpassen
    End Sub

    Private Shared Sub ToolTip_Draw(sender As Object, e As DrawToolTipEventArgs)
        Dim tt As ToolTip = TryCast(sender, ToolTip)
        Dim f As Font = TryCast(tt?.Tag, Font)
        If f Is Nothing Then f = SystemFonts.DialogFont

        Dim g As Graphics = e.Graphics
        Dim r As Rectangle = e.Bounds

        g.Clear(SystemColors.Info)
        Using pen As New Pen(SystemColors.InfoText)
            g.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1)
        End Using
        Using br As New SolidBrush(SystemColors.InfoText)
            Dim inner As New RectangleF(r.X + 6, r.Y + 4, r.Width - 12, r.Height - 8)
            g.DrawString(e.ToolTipText, f, br, inner)
        End Using
    End Sub

    ' -------------------------------------------
    ' Klingon-Style Integration
    ' -------------------------------------------

    Private Shared Sub ApplyKlingonStyle(root As Control)
        ' 1) Versuche bevorzugt Properties per Reflection zu setzen
        '    (falls deine Controls sowas anbieten, z. B. UseKlingonLogo / SymbolMode="Klingon")
        ApplyKlingonStyleByReflection(root)

        ' 2) Fallback: Controls mit Tag "STYLE_KLINGON_LOGO" selbst zeichnen
        AttachKlingonLogoPainters(root)
    End Sub

    Private Shared Sub ApplyKlingonStyleByReflection(root As Control)
        Dim stack As New Stack(Of Control)()
        stack.Push(root)
        While stack.Count > 0
            Dim c As Control = stack.Pop()

            ' Nur wenn explizit markiert (z. B. per Tag "STYLE_KLINGON"), sonst weiter
            Dim hasStyleTag As Boolean = False
            Dim t As String = TryCast(c.Tag, String)
            If Not String.IsNullOrWhiteSpace(t) AndAlso t.IndexOf("STYLE_KLINGON", StringComparison.OrdinalIgnoreCase) >= 0 Then
                hasStyleTag = True
            End If

            If hasStyleTag Then
                Dim typ As Type = c.GetType()
                ' Kandidaten-Properties: UseKlingonLogo, UseKlingonSymbols, KlingonMode,
                '                        SymbolMode (Enum), Style/Theme (Enum/String)
                Dim propNames() As String = {
                    "UseKlingonLogo", "UseKlingonSymbols", "KlingonMode",
                    "SymbolMode", "Style", "Theme"
                }
                For Each pn In propNames
                    Dim p As PropertyInfo = typ.GetProperty(pn, BindingFlags.Public Or BindingFlags.Instance)
                    If p Is Nothing OrElse Not p.CanWrite Then Continue For

                    Try
                        If p.PropertyType Is GetType(Boolean) Then
                            p.SetValue(c, True, Nothing)
                            Exit For
                        ElseIf p.PropertyType.IsEnum Then
                            ' Versuche Enum "Klingon"
                            Dim val As Object = [Enum].Parse(p.PropertyType, "Klingon", ignoreCase:=True)
                            p.SetValue(c, val, Nothing)
                            Exit For
                        ElseIf p.PropertyType Is GetType(String) Then
                            p.SetValue(c, "Klingon", Nothing)
                            Exit For
                        End If
                    Catch
                        ' Ignorieren, wenn ein Versuch fehlschlägt
                    End Try
                Next
            End If

            For Each ch As Control In c.Controls
                stack.Push(ch)
            Next
        End While
    End Sub

    Private Shared Sub AttachKlingonLogoPainters(root As Control)
        Dim stack As New Stack(Of Control)()
        stack.Push(root)
        While stack.Count > 0
            Dim c As Control = stack.Pop()
            Dim t As String = TryCast(c.Tag, String)
            If Not String.IsNullOrWhiteSpace(t) AndAlso t.IndexOf("STYLE_KLINGON_LOGO", StringComparison.OrdinalIgnoreCase) >= 0 Then
                ' Paint-Handler nur EINMAL anhängen
                RemoveHandler c.Paint, AddressOf KlingonLogo_Paint
                AddHandler c.Paint, AddressOf KlingonLogo_Paint
                c.Invalidate()
            End If
            For Each ch As Control In c.Controls
                stack.Push(ch)
            Next
        End While
    End Sub

    Private Shared Sub KlingonLogo_Paint(sender As Object, e As PaintEventArgs)
        Dim c As Control = DirectCast(sender, Control)
        DrawKlingonLogo(e.Graphics, c.ClientRectangle)
    End Sub

    ''' <summary>
    ''' Vektor-Rendering eines stilisierten klingonischen Wappens (drei Klingen + Kreis).
    ''' Farben kannst du gern anpassen.
    ''' </summary>
    Public Shared Sub DrawKlingonLogo(g As Graphics, bounds As Rectangle)
        If g Is Nothing Then Return
        If bounds.Width <= 2 OrElse bounds.Height <= 2 Then Return

        Dim cx As Single = bounds.X + bounds.Width / 2.0F
        Dim cy As Single = bounds.Y + bounds.Height / 2.0F
        Dim radius As Single = Math.Min(bounds.Width, bounds.Height) / 2.0F

        ' Farben
        Dim fillRed As Color = Color.FromArgb(220, 20, 20)
        Dim dark As Color = Color.FromArgb(50, 0, 0)
        Dim outline As Color = Color.FromArgb(80, 0, 0)

        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        ' Äußerer Ring
        Using pen As New Pen(outline, Math.Max(1.5F, radius * 0.06F))
            g.DrawEllipse(pen, cx - radius * 0.9F, cy - radius * 0.9F, radius * 1.8F, radius * 1.8F)
        End Using

        ' Drei Klingen: eine nach oben, zwei bei ±120°
        Dim bladeLen As Single = radius * 0.95F
        Dim bladeW As Single = radius * 0.28F
        Dim inner As Single = radius * 0.28F

        For i As Integer = 0 To 2
            Dim ang As Single = -90.0F + i * 120.0F
            Dim rad As Single = CSng(ang * Math.PI / 180.0)
            Dim tip As New PointF(cx + bladeLen * CSng(Math.Cos(rad)),
                                  cy + bladeLen * CSng(Math.Sin(rad)))
            ' Basislinks/rechts
            Dim left As New PointF(cx + inner * CSng(Math.Cos(rad + Math.PI / 2.2)),
                                   cy + inner * CSng(Math.Sin(rad + Math.PI / 2.2)))
            Dim right As New PointF(cx + inner * CSng(Math.Cos(rad - Math.PI / 2.2)),
                                    cy + inner * CSng(Math.Sin(rad - Math.PI / 2.2)))

            Using path As New Drawing2D.GraphicsPath()
                path.AddPolygon(New PointF() {left, tip, right})
                Using br As New SolidBrush(fillRed)
                    g.FillPath(br, path)
                End Using
                Using pen As New Pen(outline, Math.Max(1.0F, radius * 0.03F))
                    g.DrawPath(pen, path)
                End Using
            End Using
        Next

        ' Zentrales Dreieck (klein)
        Dim centerR As Single = radius * 0.22F
        Using br As New SolidBrush(dark)
            g.FillEllipse(br, cx - centerR, cy - centerR, centerR * 2, centerR * 2)
        End Using
        Using pen As New Pen(outline, Math.Max(1.0F, radius * 0.025F))
            g.DrawEllipse(pen, cx - centerR, cy - centerR, centerR * 2, centerR * 2)
        End Using
    End Sub

End Class
