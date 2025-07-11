Imports System.Drawing


''' <summary>
''' Globale Daten, die modulübergreifend von SaverMain aus bereitgestellt werden.
''' </summary>
Public NotInheritable Class SharedDataHandling

        Private Sub New()
            ' Verhindert Instanziierung
        End Sub

        Private Shared _startBild As Image = Nothing
        Private Shared _startBildWurdeVerwendet As Boolean = False

        ''' <summary>
        ''' Enthält den Screenshot vom Desktop zum Zeitpunkt des Starts von SaverMain.
        ''' </summary>
        Public Shared Property StartBild As Image
            Get
                Return _startBild
            End Get
            Set(value As Image)
                _startBild = value
            End Set
        End Property

        ''' <summary>
        ''' Gibt an, ob ein Modul das Startbild bereits verwendet hat.
        ''' </summary>
        Public Shared Property StartBildWurdeVerwendet As Boolean
            Get
                Return _startBildWurdeVerwendet
            End Get
            Set(value As Boolean)
                _startBildWurdeVerwendet = value
            End Set
        End Property

    End Class


