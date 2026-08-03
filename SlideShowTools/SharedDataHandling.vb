Imports System.Drawing

''' <summary>
''' Globale Daten, die modulübergreifend von SaverMain aus bereitgestellt werden.
''' </summary>
Public NotInheritable Class SharedDataHandling

    Private Sub New()
        ' Verhindert Instanziierung
    End Sub

    Private Shared _hintergrundFarbeSaver As Color = Color.Black

    ''' <summary>
    ''' Liefert die im frmOptionsMain eingestellte Hintergrundfarbe.
    ''' </summary>
    Public Shared Property HintergrundFarbeSaver As Color
        Get
            Return _hintergrundFarbeSaver
        End Get
        Set(value As Color)
            _hintergrundFarbeSaver = value
        End Set
    End Property
End Class


