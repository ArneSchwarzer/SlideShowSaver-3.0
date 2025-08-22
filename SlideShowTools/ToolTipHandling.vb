Imports System.Windows.Forms
Imports SlideShowInterfaces

Public Class ToolTipHandling

    ' --- Interne Verwaltung ---
    Private Shared WithEvents clbToolTip As New ToolTip()
    Private Shared lastHoveredIndex As Integer = -1

    ''' <summary>
    ''' Initialisiert ToolTips für eine CheckedListBox mit SlideShowModulInfo, SlideShowTransitionInfo oder SlideShowShaderInfo.
    ''' </summary>
    ''' <param name="clb">Die CheckedListBox, auf der ToolTips angezeigt werden sollen</param>
    Public Shared Sub EnableToolTipsForCLB(clb As CheckedListBox)
        ' MouseMove- und MouseLeave-Handler binden
        AddHandler clb.MouseMove, AddressOf CLB_MouseMove
        AddHandler clb.MouseLeave, AddressOf CLB_MouseLeave
    End Sub

    ' --- Interne Ereignisse ---
    Private Shared Sub CLB_MouseMove(sender As Object, e As MouseEventArgs)
        Dim clb As CheckedListBox = DirectCast(sender, CheckedListBox)
        Dim index As Integer = clb.IndexFromPoint(e.Location)

        If index <> lastHoveredIndex AndAlso index >= 0 AndAlso index < clb.Items.Count Then
            lastHoveredIndex = index
            Dim item = clb.Items(index)

            Dim tooltipText As String = GetBeschreibungFromItem(item)
            clbToolTip.SetToolTip(clb, tooltipText)
        End If
    End Sub

    Private Shared Sub CLB_MouseLeave(sender As Object, e As EventArgs)
        lastHoveredIndex = -1
        clbToolTip.SetToolTip(DirectCast(sender, Control), "")
    End Sub

    ' --- Hilfsmethode zur Extraktion der Beschreibung ---
    Private Shared Function GetBeschreibungFromItem(item As Object) As String
        If TypeOf item Is InfoHandling.SlideShowModulInfo Then
            Return DirectCast(item, InfoHandling.SlideShowModulInfo).ModulBeschreibung
        ElseIf TypeOf item Is InfoHandling.SlideShowTransitionInfo Then
            Return DirectCast(item, InfoHandling.SlideShowTransitionInfo).TransitionBeschreibung
        ElseIf TypeOf item Is InfoHandling.SlideShowShaderInfo Then
            Return DirectCast(item, InfoHandling.SlideShowShaderInfo).ShaderBeschreibung
        ElseIf TypeOf item Is InfoHandling.LutInfo Then
            Return DirectCast(item, InfoHandling.LutInfo).LUTBeschreibung
        Else
            Return item.ToString()
        End If
    End Function

End Class
