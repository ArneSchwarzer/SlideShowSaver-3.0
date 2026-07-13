Imports System.Collections.Generic
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

''' <summary>
''' Repräsentiert eine komplexe Zahl mit Double-Genauigkeit.
''' </summary>
Public Structure MandelbrotComplex

    Public Property Real As Double
    Public Property Imaginary As Double

    Public Sub New(real As Double, imaginary As Double)

        Me.Real = real
        Me.Imaginary = imaginary

    End Sub

End Structure


''' <summary>
''' Berechnet und verwaltet den Referenzorbit eines Punktes
''' innerhalb der komplexen Mandelbrot-Ebene.
''' </summary>
Public Class MandelbrotReferenzOrbit

#Region "Variablendeklaration"

    Private ReadOnly orbitIntern As List(Of MandelbrotComplex)
    Public ReadOnly Property TexturBreite As Integer
        Get
            Return BerechneNaechsteZweierpotenz(orbitIntern.Count)
        End Get
    End Property

#End Region

#Region "Eigenschaften"

    ''' <summary>
    ''' Realteil des Referenzpunktes.
    ''' </summary>
    Public ReadOnly Property CenterX As Double

    ''' <summary>
    ''' Imaginärteil des Referenzpunktes.
    ''' </summary>
    Public ReadOnly Property CenterY As Double

    ''' <summary>
    ''' Maximale Anzahl der zu berechnenden Iterationen.
    ''' </summary>
    Public ReadOnly Property MaxIterationen As Integer

    ''' <summary>
    ''' Nur lesbarer Zugriff auf die berechneten Orbitwerte.
    ''' </summary>
    Public ReadOnly Property Orbit As IReadOnlyList(Of MandelbrotComplex)
        Get
            Return orbitIntern
        End Get
    End Property

    ''' <summary>
    ''' Anzahl der tatsächlich berechneten Orbitwerte.
    ''' </summary>
    Public ReadOnly Property Count As Integer
        Get
            Return orbitIntern.Count
        End Get
    End Property

#End Region

#Region "Konstruktor"

    Public Sub New(centerX As Double,
                   centerY As Double,
                   maxIterationen As Integer)

        If maxIterationen <= 0 Then
            Throw New ArgumentOutOfRangeException(
                NameOf(maxIterationen),
                "Die maximale Iterationszahl muss größer als 0 sein.")
        End If

        Me.CenterX = centerX
        Me.CenterY = centerY
        Me.MaxIterationen = maxIterationen

        orbitIntern = BerechneOrbit()

    End Sub

#End Region

#Region "Öffentliche Funktionen"

    ''' <summary>
    ''' Liefert den Orbitwert an der angegebenen Position.
    ''' </summary>
    Public Function Item(index As Integer) As MandelbrotComplex

        If index < 0 OrElse index >= orbitIntern.Count Then
            Throw New ArgumentOutOfRangeException(
                NameOf(index),
                "Der angegebene Orbitindex liegt außerhalb des gültigen Bereichs.")
        End If

        Return orbitIntern(index)

    End Function

    Public Function ErzeugeRealOrbitBrush() As ImageBrush

        Return ErzeugeOrbitBrush(True)

    End Function

    Public Function ErzeugeImaginaryOrbitBrush() As ImageBrush

        Return ErzeugeOrbitBrush(False)

    End Function

#End Region

#Region "Orbit-Berechnung"

    ''' <summary>
    ''' Berechnet die Referenzbahn Z(n+1) = Z(n)^2 + C.
    ''' Z0 wird als erster Eintrag gespeichert.
    ''' </summary>
    Private Function BerechneOrbit() As List(Of MandelbrotComplex)

        Dim ergebnis As List(Of MandelbrotComplex)
        Dim zReal As Double
        Dim zImaginary As Double
        Dim zRealQuadrat As Double
        Dim zImaginaryQuadrat As Double
        Dim neuerRealteil As Double
        Dim neuerImaginaerteil As Double
        Dim betragQuadrat As Double
        Dim i As Integer

        ergebnis = New List(Of MandelbrotComplex)(MaxIterationen)

        zReal = 0.0
        zImaginary = 0.0

        For i = 0 To MaxIterationen - 1

            ' Z(n) speichern, bevor Z(n+1) berechnet wird.
            ergebnis.Add(
                New MandelbrotComplex(
                    zReal,
                    zImaginary))

            zRealQuadrat = zReal * zReal
            zImaginaryQuadrat = zImaginary * zImaginary

            neuerRealteil =
                zRealQuadrat -
                zImaginaryQuadrat +
                CenterX

            neuerImaginaerteil =
                2.0 *
                zReal *
                zImaginary +
                CenterY

            zReal = neuerRealteil
            zImaginary = neuerImaginaerteil

            betragQuadrat =
                zReal * zReal +
                zImaginary * zImaginary

            If betragQuadrat > 4.0 Then
                Exit For
            End If

        Next

        Return ergebnis

    End Function

#End Region

    Private Function ErzeugeOrbitBrush(realteil As Boolean) As ImageBrush

        Dim texturBreite As Integer
        Dim pixelDaten() As Byte
        Dim orbitWert As MandelbrotComplex
        Dim wert As Double
        Dim normalisiert As Double
        Dim codiert As Integer
        Dim rot As Byte
        Dim gruen As Byte
        Dim blau As Byte
        Dim pixelOffset As Integer
        Dim letzterIndex As Integer
        Dim bitmap As BitmapSource
        Dim brush As ImageBrush
        Dim i As Integer

        If orbitIntern Is Nothing OrElse orbitIntern.Count = 0 Then
            Throw New InvalidOperationException(
            "Der Referenzorbit enthält keine Werte.")
        End If

        texturBreite =
        BerechneNaechsteZweierpotenz(
            orbitIntern.Count)

        'BGRA32 = vier Bytes pro Pixel.
        ReDim pixelDaten(texturBreite * 4 - 1)

        letzterIndex = orbitIntern.Count - 1

        For i = 0 To texturBreite - 1

            orbitWert =
            orbitIntern(Math.Min(i, letzterIndex))

            If realteil Then
                wert = orbitWert.Real
            Else
                wert = orbitWert.Imaginary
            End If

            'Sicherheitsbegrenzung auf den darstellbaren Bereich.
            wert = Math.Max(-2.0, Math.Min(2.0, wert))

            '[-2, +2] nach [0, 1] transformieren.
            normalisiert =
            (wert + 2.0) / 4.0

            'In 24 Bit quantisieren.
            codiert =
            CInt(Math.Round(
                normalisiert * 16777215.0))

            codiert =
            Math.Max(
                0,
                Math.Min(16777215, codiert))

            rot =
            CByte((codiert >> 16) And &HFF)

            gruen =
            CByte((codiert >> 8) And &HFF)

            blau =
            CByte(codiert And &HFF)

            pixelOffset = i * 4

            'PixelFormats.Bgra32 erwartet physisch:
            'B, G, R, A
            pixelDaten(pixelOffset + 0) = blau
            pixelDaten(pixelOffset + 1) = gruen
            pixelDaten(pixelOffset + 2) = rot
            pixelDaten(pixelOffset + 3) = 255

        Next

        bitmap = BitmapSource.Create(
        texturBreite,
        1,
        96.0,
        96.0,
        PixelFormats.Bgra32,
        Nothing,
        pixelDaten,
        texturBreite * 4)

        bitmap.Freeze()

        brush = New ImageBrush(bitmap)

        With brush
            .Stretch = Stretch.Fill
            .TileMode = TileMode.None
            .AlignmentX = AlignmentX.Left
            .AlignmentY = AlignmentY.Top
        End With

        RenderOptions.SetBitmapScalingMode(
        brush,
        BitmapScalingMode.NearestNeighbor)

        brush.Freeze()

        Return brush

    End Function

    Private Function BerechneNaechsteZweierpotenz(value As Integer) As Integer

        Dim result As Integer

        result = 1

        While result < value
            result *= 2
        End While

        Return Math.Max(2, result)

    End Function

End Class