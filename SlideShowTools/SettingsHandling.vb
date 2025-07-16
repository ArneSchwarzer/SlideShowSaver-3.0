

''' <summary>
''' Temporärer Speicher für Settings-Objekte während der Laufzeit.
''' Erlaubt die Übergabe von Konfigurationsdaten z. B. vom ShaderMain an ein UserControl,
''' ohne Registry oder direkte Objektbindung zu verwenden.
''' </summary>
Public NotInheritable Class SettingsHandling

        Private Shared inbox As New Dictionary(Of String, Object)

        ''' <summary>
        ''' Speichert ein beliebiges Settings-Objekt unter einem eindeutigen Namen.
        ''' </summary>
        ''' <param name="key">Eindeutiger Bezeichner (z. B. Modulname)</param>
        ''' <param name="settings">Das zu speichernde Objekt</param>
        Public Shared Sub StoreSettings(key As String, settings As Object)
            If String.IsNullOrWhiteSpace(key) OrElse settings Is Nothing Then Exit Sub
            inbox(key) = settings
        End Sub

        ''' <summary>
        ''' Holt das gespeicherte Objekt für den angegebenen Key.
        ''' Gibt Nothing zurück, wenn nicht gefunden oder Typ nicht kompatibel.
        ''' </summary>
        ''' <typeparam name="T">Erwarteter Rückgabetyp</typeparam>
        ''' <param name="key">Bezeichner wie zuvor gespeichert</param>
        Public Shared Function GetSettings(Of T)(key As String) As T
            If inbox.ContainsKey(key) AndAlso TypeOf inbox(key) Is T Then
                Return CType(inbox(key), T)
            End If
            Return Nothing
        End Function

    ''' <summary>
    ''' Entfernt alle Einträge.
    ''' </summary>
    Public Shared Sub ClearAll()
            inbox.Clear()
        End Sub

    End Class


