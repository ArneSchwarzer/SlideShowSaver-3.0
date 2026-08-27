Imports System.Drawing
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations
Imports SlideShowTools.ColorHandling
Imports SlideShowTools.RegistryHandling
Imports SlideShowTools.SettingsHandling

Public Class ShaderMain
    Implements ISlideShowShader

#Region "Variablendeklaration und Strukturen"

    'Allgemeines
    Public Const SLIDESHOWSHADER_AQUARELL_FULLPATH As String =
        SLIDESHOWSHADER_PATH &
        "Aquarell\"

    Public Const nameShader As String = "Aquarell"

    Private aktuelleSettings As ShaderSettings_Aquarell

    Private ReadOnly rnd As New Random()

    Private wurdeBereinigt As Boolean

    Public Structure ShaderSettings_Aquarell

        Public Property glaettungsAlgorithmus As String 'nur für interne Testzwecke während V 0.1

    End Structure

    Public Enum ShaderModus

        Bilateral = 0
        Kuwahara = 1
        AnisotropicKuwahara = 2

    End Enum

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property ShaderName As String _
        Implements ISlideShowShader.ShaderName

        Get

            Return nameShader

        End Get

    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String _
        Implements ISlideShowShader.ShaderKurzBeschreibung

        Get

            Return "Wir kippen einen Becher Wasser über das Papier und schauen, was passiert."

        End Get

    End Property

    Public ReadOnly Property ShaderVersion As Version _
        Implements ISlideShowShader.ShaderVersion

        Get

            Return New Version(1, 0, 0, 0)

        End Get

    End Property

#End Region

#Region "Shader-Ausführung"

    Public Function RunShader(baseImage As Image, Optional imagePath As String = "",
                              Optional clientSize As Size = Nothing) As Image Implements ISlideShowShader.RunShader
        'Versieht das Quellbild mit einem Aquarell-Effekt

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(ShaderMain))
        End If

        If baseImage Is Nothing Then
            Throw New ArgumentNullException(NameOf(baseImage))
        End If

        ReadShaderSettingsFromRegistryOrDefaults()

        StoreSettings(nameShader, aktuelleSettings)

        'TODO: Code für Aquarell-Simulation erstellen

        Return baseImage

    End Function

#End Region

#Region "Dialog und Optionen"

    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        'Liest die aktuellen Settings ein und stellt sie dem
        'Options-Control über die SettingsInbox bereit.

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(ShaderMain))
        End If

        ReadShaderSettingsFromRegistryOrDefaults()

        StoreSettings(nameShader, aktuelleSettings)

        Return New ucOptionsShader()

    End Function

#End Region

#Region "Settings und Defaultwerte"

    Friend Shared Function GetShaderDefaultSettings() _
        As Dictionary(Of String, String)
        'Liefert die Default-Werte des Shaders.

        Dim defaults As New Dictionary(Of String, String)

        defaults("Glättungsalgorithmus") = "Kuwahara"

        Return defaults

    End Function

    Private Sub ReadShaderSettingsFromRegistryOrDefaults()
        'Liest die aktuellen Shadereinstellungen aus der Registry
        'oder verwendet die definierten Defaultwerte.

        Dim defaults As Dictionary(Of String, String)

        defaults = GetShaderDefaultSettings()

        aktuelleSettings.glaettungsAlgorithmus = ReadFromRegOrDefaults(SLIDESHOWSHADER_AQUARELL_FULLPATH &
                                                                       "Glättungsalgorithmus", defaults)

    End Sub

#End Region


#Region "IDisposable"

    Public Sub Dispose() Implements IDisposable.Dispose
        'Markiert die Shaderinstanz als endgültig freigegeben.

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class