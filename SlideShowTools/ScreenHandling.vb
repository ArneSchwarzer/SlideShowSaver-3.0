Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Drawing.Imaging

Public Class ScreenHandling

    ' DEVMODE-Struktur für EnumDisplaySettings
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)>
    Private Structure DEVMODE
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public dmDeviceName As String
        Public dmSpecVersion As Short
        Public dmDriverVersion As Short
        Public dmSize As Short
        Public dmDriverExtra As Short
        Public dmFields As Integer
        Public dmPositionX As Integer
        Public dmPositionY As Integer
        Public dmDisplayOrientation As Integer
        Public dmDisplayFixedOutput As Integer
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=32)>
        Public dmFormName As String
        Public dmLogPixels As Short
        Public dmBitsPerPel As Integer
        Public dmPelsWidth As Integer
        Public dmPelsHeight As Integer
        Public dmDisplayFlags As Integer
        Public dmDisplayFrequency As Integer
        Public dmICMMethod As Integer
        Public dmICMIntent As Integer
        Public dmMediaType As Integer
        Public dmDitherType As Integer
        Public dmReserved1 As Integer
        Public dmReserved2 As Integer
        Public dmPanningWidth As Integer
        Public dmPanningHeight As Integer
    End Structure

    ' EnumDisplaySettings-API für echte Auflösung
    <DllImport("user32.dll", CharSet:=CharSet.Auto)>
    Private Shared Function EnumDisplaySettings(
        lpszDeviceName As String,
        iModeNum As Integer,
        ByRef lpDevMode As DEVMODE) As Boolean
    End Function

    Private Const ENUM_CURRENT_SETTINGS As Integer = -1

    ''' <summary>
    ''' Gibt die native Auflösung des Bildschirms zurück, auf dem sich das angegebene Control befindet.
    ''' </summary>
    Public Shared Function GetNativeScreenResolution(targetControl As Control) As Size
        Dim targetScreen As Screen = Screen.FromControl(targetControl)
        Return GetNativeScreenResolution(targetScreen)
    End Function

    ''' <summary>
    ''' Gibt die native Auflösung des übergebenen Screen-Objekts zurück (unabhängig von DPI).
    ''' </summary>
    Public Shared Function GetNativeScreenResolution(targetScreen As Screen) As Size
        Dim devMode As New DEVMODE()
        devMode.dmSize = CShort(Marshal.SizeOf(devMode))

        If EnumDisplaySettings(targetScreen.DeviceName, ENUM_CURRENT_SETTINGS, devMode) Then
            Return New Size(devMode.dmPelsWidth, devMode.dmPelsHeight)
        Else
            Return Size.Empty
        End If
    End Function

    ''' <summary>
    ''' Gibt die native Auflösung des Bildschirms zurück, auf dem sich das aktive Formular befindet.
    ''' </summary>
    Public Shared Function GetNativeScreenResolution() As Size
        Dim activeForm As Form = Form.ActiveForm
        If activeForm IsNot Nothing Then
            Return GetNativeScreenResolution(activeForm)
        Else
            Return GetNativeScreenResolution(Screen.PrimaryScreen)
        End If
    End Function

    ''' <summary>
    ''' Erstellt einen Screenshot des angegebenen Bildschirms.
    ''' </summary>
    Public Shared Function ErstelleScreenshot(targetScreen As Screen) As Image

        Dim bounds As Rectangle
        Dim screenshot As Bitmap

        If targetScreen Is Nothing Then
            Return Nothing
        End If

        bounds = targetScreen.Bounds

        screenshot =
            New Bitmap(
                bounds.Width,
                bounds.Height,
                Imaging.PixelFormat.Format32bppArgb)

        Using grafik As Graphics = Graphics.FromImage(screenshot)

            grafik.CopyFromScreen(
                bounds.Location,
                Point.Empty,
                bounds.Size,
                CopyPixelOperation.SourceCopy)

        End Using

        Return screenshot

    End Function

    ''' <summary>
    ''' Erstellt einen Screenshot des primären Bildschirms.
    ''' </summary>
    Public Shared Function ErstelleScreenshot() As Image

        Return ErstelleScreenshot(
            Screen.PrimaryScreen)

    End Function

End Class
