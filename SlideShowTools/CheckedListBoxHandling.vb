Imports System.Windows.Forms
Imports SlideShowLogging

Public Class CheckedListBoxHandling

    ''' <summary>
    ''' Gibt alle als "Checked" markierten Items in einer CheckedListBox als Semikolon-getrennte Zeichenkette zurück.
    ''' </summary>
    Public Shared Function GetCheckedItemsAsString(clb As CheckedListBox) As String
        Dim result As New List(Of String)
        Dim item As Object

        For Each item In clb.CheckedItems
            result.Add(item.ToString())
        Next

        Return String.Join(";", result)
    End Function

    Public Shared Function GetCheckedItemsAsString(Of T)(clb As CheckedListBox, selector As Func(Of T, String)) _
        As String
        'Gibt die Namen aller markierten Elemente anhand
        'einer typisierten Auswahlfunktion zurück.

        Dim result As New List(Of String)
        Dim item As Object
        Dim typedItem As T

        For Each item In clb.CheckedItems

            If TypeOf item Is T Then

                typedItem = CType(item, T)

                result.Add(selector(typedItem))

            End If

        Next

        Return String.Join(";", result)

    End Function

    ''' <summary>
    ''' Markiert Elemente in einer CheckedListBox anhand eines Semikolon-getrennten Strings
    ''' </summary>
    ''' <typeparam name="T">Datentyp der Listeneinträge (z. B. SlideShowModulInfo)</typeparam>
    ''' <param name="clb">Die CheckedListBox</param>
    ''' <param name="csvList">Semikolon-getrennter String (z. B. "Matrix;Mandelbrot")</param>
    ''' <param name="selector">Funktion, die aus dem Item den zu vergleichenden Namen extrahiert</param>
    Public Shared Sub SetCheckedItemsByName(Of T)(
    clb As CheckedListBox,
    csvList As String,
    selector As Func(Of T, String)
)

        Dim checkedNames As HashSet(Of String)
        checkedNames = New HashSet(Of String)(
        csvList.Split(New String() {";"}, StringSplitOptions.RemoveEmptyEntries).
        Select(Function(s) s.Trim())
    )

        For i = 0 To clb.Items.Count - 1
            Dim item As Object = clb.Items(i)

            If TypeOf item Is T Then
                Dim typedItem As T = CType(item, T)
                If checkedNames.Contains(selector(typedItem)) Then
                    clb.SetItemChecked(i, True)
                End If
            End If
        Next

    End Sub

    Public Shared Sub SaveListBoxToRegistry(lb As ListBox, regPath As String, Optional saveMarkedOnly As Boolean = True)
        Dim einträge As New List(Of String)
        Dim i As Integer
        Dim clb As CheckedListBox = TryCast(lb, CheckedListBox)

        If clb IsNot Nothing AndAlso saveMarkedOnly Then
            ' Nur markierte Einträge in CheckedListBox
            For Each item In clb.CheckedItems
                einträge.Add(item.ToString())
            Next
        Else
            ' Alle Einträge (egal ob ListBox oder CheckedListBox)
            For i = 0 To lb.Items.Count - 1
                einträge.Add(lb.Items(i).ToString())
            Next
        End If

        Dim joined = String.Join(";", einträge)
        RegistryHandling.WriteToRegistry(regPath, joined)
    End Sub


End Class

