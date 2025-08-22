Public Class InfoHandling
    ''' <summary>
    ''' Struktur zur Beschreibung eines Bildschirmschoner-Moduls.
    ''' </summary>
    Public Structure SlideShowModulInfo
        Public ModulName As String
        Public ModulBeschreibung As String
        Public ModulVersion As Version
        Public ModulNutztSlideShowBildauswahl As Boolean
        Public ModulNutztTransitions As Boolean
        Public ModulNutztShader As Boolean

        Public Overrides Function ToString() As String
            Return ModulName
        End Function
    End Structure


    ''' <summary>
    ''' Struktur zur Beschreibung eines Transitionseffektes
    ''' </summary>
    Public Structure SlideShowTransitionInfo
        Public TransitionName As String
        Public TransitionBeschreibung As String
        Public TransitionVersion As Version

        Public Overrides Function ToString() As String
            Return TransitionName
        End Function
    End Structure

    '''<summary>
    '''Struktur zur Beschreibung eines Shaders
    ''' </summary>
    Public Structure SlideShowShaderInfo
        Public ShaderName As String
        Public ShaderBeschreibung As String
        Public ShaderVersion As Version

        Public Overrides Function ToString() As String
            Return ShaderName
        End Function
    End Structure

    '''<summary>
    '''Struktur zur Beschreibung eines Color-Lookup-Tabels (LUT)
    ''' </summary>
    Public Structure LutInfo
        Public FullPath As String
        Public LUTName As String
        Public LUTBeschreibung As String
        Public Type As LutType
        Public SizeN As Integer          ' 3D-Würfel-Kantenlänge (z.B. 17, 32, 33)
        Public ErrorMessage As String
        Public Overrides Function ToString() As String
            Return LUTName
        End Function
    End Structure

    Public Enum LutType
        Unknown = 0
        Cube3D = 1      ' .cube Textformat (IRIDAS)
        HaldPng = 2     ' Hald CLUT als PNG-Bild
    End Enum

End Class
