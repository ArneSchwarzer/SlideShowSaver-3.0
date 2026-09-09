Imports System.Windows.Media.Imaging

Friend Class RenderParameter

    Public breite As Integer
    Public hoehe As Integer

    Public oldImage As BitmapSource
    Public newImage As BitmapSource
    Public brandMaske As BitmapSource

    Public gradientBitmap As BitmapSource
    Public particleGradientBitmap As BitmapSource

    Public brandkantenBreite As Single

    Public partikelAktiv As Boolean
    Public schwerkraftAktiv As Boolean
    Public gradientAktiv As Boolean
    Public verzerrungAktiv As Boolean

    Public modus As String

    Public partikelLebensdauer As Single

    Public verzerrungsBreite As Single
    Public verzerrungsStaerke As Single

    Public magieScherbenGroesse As Single

End Class