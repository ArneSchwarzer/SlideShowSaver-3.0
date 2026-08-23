Imports System.Runtime.InteropServices

Public Enum PartikelLOD As Integer

    Fein = 0
    Mittel = 1
    Grob = 2

End Enum

Public Enum PartikelStatus As Integer

    Tot = 0
    Ruhend = 1
    Aktiv = 2

End Enum

<StructLayout(LayoutKind.Sequential)>
Public Structure PartikelDaten

    'Position in Pixelkoordinaten.
    Public positionX As Single
    Public positionY As Single
    Public positionZ As Single

    'Aktuelle Geschwindigkeit.
    Public geschwindigkeitX As Single
    Public geschwindigkeitY As Single
    Public geschwindigkeitZ As Single

    'Größe des ursprünglichen Bildausschnitts in Pixel.
    Public breite As Single
    Public hoehe As Single

    'UV-Rechteck im alten Bild.
    Public uvLinks As Single
    Public uvOben As Single
    Public uvRechts As Single
    Public uvUnten As Single

    'Rotation.
    Public rotationX As Single
    Public rotationY As Single
    Public rotationZ As Single

    'Rotationsgeschwindigkeit.
    Public rotationsGeschwindigkeitX As Single
    Public rotationsGeschwindigkeitY As Single
    Public rotationsGeschwindigkeitZ As Single

    'LOD und Lebenszustand.
    Public lod As Integer
    Public status As Integer

End Structure