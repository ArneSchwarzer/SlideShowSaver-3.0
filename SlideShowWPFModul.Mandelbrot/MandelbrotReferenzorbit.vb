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
    Private texturBreiteIntern As Integer
    Private texturHoeheIntern As Integer

    Public ReadOnly Property TexturBreite As Integer
        Get
            Return texturBreiteIntern
        End Get
    End Property

    Public ReadOnly Property TexturHoehe As Integer
        Get
            Return texturHoeheIntern
        End Get
    End Property

    Private Const OrbitHighMinimum As Double = -2.0
    Private Const OrbitHighMaximum As Double = 2.0

    'Die Low-Komponente eines in [-2, +2] liegenden Double-Wertes
    'ist erheblich kleiner. Der gewählte Bereich enthält ausreichend
    'Sicherheitsreserve und bleibt dennoch sehr fein quantisierbar.
    Private Const OrbitLowMinimum As Double = -0.00000025
    Private Const OrbitLowMaximum As Double = 0.00000025

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

    Public Sub New(centerX As Double, centerY As Double, maxIterationen As Integer)

        If maxIterationen <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(maxIterationen), "Die maximale Iterationszahl muss größer als 0 sein.")
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
            Throw New ArgumentOutOfRangeException(NameOf(index), "Der angegebene Orbitindex liegt außerhalb des gültigen Bereichs.")
        End If

        Return orbitIntern(index)

    End Function

    Public Function ErzeugeRealHighOrbitBrush() As ImageBrush

        Return ErzeugeOrbitKomponentenBrush(True, True)

    End Function

    Public Function ErzeugeImaginaryHighOrbitBrush() As ImageBrush

        Return ErzeugeOrbitKomponentenBrush(False, True)

    End Function

    Public Function ErzeugeRealLowOrbitBrush() As ImageBrush

        Return ErzeugeOrbitKomponentenBrush(True, False)

    End Function

    Public Function ErzeugeImaginaryLowOrbitBrush() As ImageBrush

        Return ErzeugeOrbitKomponentenBrush(False, False)

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
            ergebnis.Add(New MandelbrotComplex(zReal, zImaginary))

            zRealQuadrat = zReal * zReal
            zImaginaryQuadrat = zImaginary * zImaginary

            neuerRealteil = zRealQuadrat - zImaginaryQuadrat + CenterX

            neuerImaginaerteil = 2.0 * zReal * zImaginary + CenterY

            zReal = neuerRealteil
            zImaginary = neuerImaginaerteil

            betragQuadrat = zReal * zReal + zImaginary * zImaginary

            If betragQuadrat > 4.0 Then
                Exit For
            End If

        Next

        Return ergebnis

    End Function

#End Region

    Public Sub InitialisiereTexturlayout(maximaleTexturBreite As Integer)

        Dim orbitAnzahl As Integer

        orbitAnzahl = orbitIntern.Count

        maximaleTexturBreite = Math.Max(1, maximaleTexturBreite)
        texturBreiteIntern = Math.Min(orbitAnzahl, maximaleTexturBreite)
        texturHoeheIntern = CInt(Math.Ceiling(CDbl(orbitAnzahl) / CDbl(texturBreiteIntern)))

    End Sub

    Private Function ErzeugeOrbitKomponentenBrush(
    realteil As Boolean,
    highKomponente As Boolean) As ImageBrush

        Dim pixelDaten() As Byte
        Dim orbitWert As MandelbrotComplex
        Dim originalWert As Double
        Dim high As Single
        Dim low As Single
        Dim komponentenWert As Double
        Dim minimum As Double
        Dim maximum As Double
        Dim normalisiert As Double
        Dim codierterWert As Integer
        Dim rot As Byte
        Dim gruen As Byte
        Dim blau As Byte
        Dim pixelOffset As Integer
        Dim stride As Integer
        Dim bitmap As BitmapSource
        Dim brush As ImageBrush
        Dim i As Integer

        If orbitIntern Is Nothing OrElse orbitIntern.Count = 0 Then

            Throw New InvalidOperationException("Der Referenzorbit enthält keine Werte.")

        End If

        If texturBreiteIntern <= 0 OrElse texturHoeheIntern <= 0 Then

            Throw New InvalidOperationException("Das Orbit-Texturlayout wurde noch nicht initialisiert.")

        End If

        If highKomponente Then

            minimum = OrbitHighMinimum
            maximum = OrbitHighMaximum

        Else

            minimum = OrbitLowMinimum
            maximum = OrbitLowMaximum

        End If

        stride = texturBreiteIntern * 4

        ReDim pixelDaten(stride * texturHoeheIntern - 1)

        For i = 0 To orbitIntern.Count - 1

            orbitWert = orbitIntern(i)

            If realteil Then
                originalWert = orbitWert.Real
            Else
                originalWert = orbitWert.Imaginary
            End If

            SplitDouble(originalWert, high, low)

            If highKomponente Then
                komponentenWert = CDbl(high)
            Else
                komponentenWert = CDbl(low)
            End If

            If komponentenWert < minimum OrElse komponentenWert > maximum Then

                Throw New InvalidOperationException(
                "Orbitkomponente außerhalb des odierbaren Bereichs. Index=" & i.ToString() &
                ", Wert=" & komponentenWert.ToString("R", Globalization.CultureInfo.InvariantCulture))

            End If

            normalisiert = (komponentenWert - minimum) / (maximum - minimum)

            codierterWert = CInt(Math.Round(normalisiert * 16777215.0))
            codierterWert = Math.Max(0, Math.Min(16777215, codierterWert))

            rot = CByte((codierterWert >> 16) And &HFF)
            gruen = CByte((codierterWert >> 8) And &HFF)
            blau = CByte(codierterWert And &HFF)

            pixelOffset = i * 4

            pixelDaten(pixelOffset + 0) = blau
            pixelDaten(pixelOffset + 1) = gruen
            pixelDaten(pixelOffset + 2) = rot
            pixelDaten(pixelOffset + 3) = 255

        Next

        bitmap = BitmapSource.Create(texturBreiteIntern, texturHoeheIntern, 96.0, 96.0, PixelFormats.Bgra32, Nothing,
            pixelDaten, stride)

        bitmap.Freeze()

        brush = New ImageBrush(bitmap)

        With brush

            .Stretch = Stretch.None
            .TileMode = TileMode.None
            .AlignmentX = AlignmentX.Left
            .AlignmentY = AlignmentY.Top

        End With

        RenderOptions.SetBitmapScalingMode(brush, BitmapScalingMode.NearestNeighbor)

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

    Private Sub SplitDouble(value As Double, ByRef high As Single, ByRef low As Single)

        high = CSng(value)
        low = CSng(value - CDbl(high))

    End Sub

End Class