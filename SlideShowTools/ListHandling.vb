Public Class ListHandling

    ''' <summary>
    ''' Gibt den alphabetisch nächsten Eintrag aus einer sortierten Liste zurück. Ist der aktuelle Eintrag der letzte, wird zyklisch das erste Element zurückgegeben.
    ''' </summary>
    ''' <param name="sortedList">Eine alphabetisch sortierte Liste von Strings</param>
    ''' <param name="currentItem">Der aktuell ausgewählte Eintrag</param>
    ''' <returns>Den nächsten Eintrag in alphabetischer Reihenfolge</returns>
    Public Shared Function GetNextAlphabeticItemName(sortedList As List(Of String), currentItem As String) As String
        If sortedList Is Nothing OrElse sortedList.Count = 0 Then
            Return Nothing
        End If

        Dim currentIndex As Integer = sortedList.IndexOf(currentItem)

        If currentIndex = -1 Then
            Return sortedList(0)
        End If

        Dim nextIndex As Integer = (currentIndex + 1) Mod sortedList.Count
        Return sortedList(nextIndex)
    End Function


    ''' <summary>
    ''' Gibt ein zufälliges Element aus einer Liste zurück.
    ''' </summary>
    ''' <typeparam name="T">Der Typ der Elemente in der Liste.</typeparam>
    ''' <param name="list">Die Liste, aus der ein Element gewählt werden soll.</param>
    ''' <returns>Ein zufälliges Element der Liste, oder Nothing, wenn die Liste leer ist.</returns>
    Public Shared Function GetRandomItemFromList(Of T)(list As List(Of T)) As T
        If list Is Nothing OrElse list.Count = 0 Then
            Return Nothing
        End If

        Dim rnd As New Random()
        Dim index As Integer = rnd.Next(list.Count)
        Return list(index)
    End Function

    ''' <summary>
    ''' Wandelt einen Semikolon-separierten String in eine Liste von Strings um.
    ''' Leere Einträge werden entfernt, und alle Elemente werden getrimmt.
    ''' </summary>
    ''' <param name="input">Der Quellstring (z. B. "ModulA; ModulB;ModulC")</param>
    ''' <returns>Liste von Strings</returns>
    Public Shared Function SplitSemicolonList(input As String) As List(Of String)
        If String.IsNullOrWhiteSpace(input) Then
            Return New List(Of String)()
        End If

        Return input.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries).
                     Select(Function(s) s.Trim()).
                     ToList()
    End Function

    ''' <summary>
    ''' Wandelt eine Liste von Strings in einen einzigen String mit Semikolon als Trennzeichen.
    ''' Leere oder Null-Einträge werden ignoriert.
    ''' </summary>
    ''' <param name="list">Die Quellliste</param>
    ''' <returns>Ein String mit Semikolon-separierten Einträgen</returns>
    Public Shared Function JoinSemicolonList(list As List(Of String)) As String
        If list Is Nothing OrElse list.Count = 0 Then
            Return String.Empty
        End If

        Return String.Join(";", list.Where(Function(s) Not String.IsNullOrWhiteSpace(s)).Select(Function(s) s.Trim()))
    End Function

End Class


