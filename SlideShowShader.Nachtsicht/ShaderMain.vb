Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Windows.Forms
Imports SlideShowInterfaces.InterfaceDeclarations

Public Class ShaderMain
    Implements ISlideShowShader

    ' === Eigenschaften ===
    Public ReadOnly Property ShaderName As String Implements ISlideShowShader.ShaderName
        Get
            Return "Nachtsicht"
        End Get
    End Property

    Public ReadOnly Property ShaderKurzBeschreibung As String Implements ISlideShowShader.ShaderKurzBeschreibung
        Get
            Return "Simuliert eine Nachtsichtkamera mit grünem Farbton, Rauschen und Scanlines."
        End Get
    End Property

    Public ReadOnly Property ShaderVersion As Version Implements ISlideShowShader.ShaderVersion
        Get
            Return New Version(1, 0, 0, 0)
        End Get
    End Property

    ' === Shader-Ausführung ===
    Public Function RunShader(baseImage As Image) As Image Implements ISlideShowShader.RunShader
        Dim bmp As New Bitmap(baseImage.Width, baseImage.Height)

        ' === 1. Grüner Nachtsicht-Farbfilter (ColorMatrix) ===
        Dim cm As New ColorMatrix(New Single()() {
            New Single() {0.1F, 0.1F, 0.1F, 0, 0},
            New Single() {0.9F, 0.9F, 0.9F, 0, 0},
            New Single() {0.1F, 0.1F, 0.1F, 0, 0},
            New Single() {0, 0, 0, 1, 0},
            New Single() {0, 0.1F, 0, 0, 1}
        })

        Dim ia As New ImageAttributes()
        ia.SetColorMatrix(cm)

        Using g As Graphics = Graphics.FromImage(bmp)
            g.DrawImage(baseImage, New Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, baseImage.Width, baseImage.Height, GraphicsUnit.Pixel, ia)

            ' === 2. Rauschen (Noise) ===
            Dim rnd As New Random()
            Using noiseBmp As New Bitmap(bmp.Width, bmp.Height)
                For y As Integer = 0 To noiseBmp.Height - 1
                    For x As Integer = 0 To noiseBmp.Width - 1
                        Dim noise As Integer = rnd.Next(0, 50)
                        noiseBmp.SetPixel(x, y, Color.FromArgb(noise, noise, noise))
                    Next
                Next
                g.DrawImage(noiseBmp, 0, 0, bmp.Width, bmp.Height)
            End Using

            ' === 3. Scanlines (dunkle horizontale Streifen) ===
            Dim scanPen As New Pen(Color.FromArgb(40, 0, 0, 0), 1)
            For y As Integer = 0 To bmp.Height - 1 Step 3
                g.DrawLine(scanPen, 0, y, bmp.Width, y)
            Next
        End Using

        Return bmp
    End Function

    ' === Optionen/Dialog (nicht verwendet) ===
    Public Function GetShaderOptionsDialog() As UserControl Implements ISlideShowShader.GetShaderOptionsDialog
        Return New ucOptionsShader()
    End Function

    Public Function MemorizeShaderSettings(uc As UserControl) As Object Implements ISlideShowShader.MemorizeShaderSettings
        Return Nothing
    End Function

    Public Sub ApplyShaderSettings(settings As Object) Implements ISlideShowShader.ApplyShaderSettings
        ' Keine Einstellungen
    End Sub

    Public Sub GetShaderSettings(uc As UserControl, restoreSettings As Object) Implements ISlideShowShader.GetShaderSettings
        ' Keine Einstellungen
    End Sub

    Public Sub GetShaderRegistryOrDefaultSettings(uc As UserControl) Implements ISlideShowShader.GetShaderRegistryOrDefaultSettings
        ' Keine Einstellungen
    End Sub

    Public Sub CheckYourSettings() Implements ISlideShowShader.CheckYourSettings
        ' Keine Einstellungen
    End Sub

End Class
