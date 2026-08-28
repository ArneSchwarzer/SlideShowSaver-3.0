Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Imports Vortice.Direct3D11
Imports Vortice.DXGI

Friend NotInheritable Class Direct3DRessourceHandler

    Private Sub New()

    End Sub

#Region "Texture-Erzeugung"

    Friend Shared Function ErstelleTextureAusImage(renderDevice As ID3D11Device,
                                                   quelle As Image) As ID3D11Texture2D

        Dim bitmap As Bitmap
        Dim bitmapData As BitmapData

        Dim rectangle As Rectangle

        Dim textureDescription As Texture2DDescription
        Dim initialData() As SubresourceData

        Dim texture As ID3D11Texture2D

        bitmap = Nothing
        bitmapData = Nothing

        texture = Nothing

        If renderDevice Is Nothing Then
            Throw New ArgumentNullException(NameOf(renderDevice))
        End If

        If quelle Is Nothing Then
            Throw New ArgumentNullException(NameOf(quelle))
        End If

        Try

            ' D3D11 arbeitet für unsere komplette Aquarell-Pipeline
            ' einheitlich mit BGRA8.
            '
            ' System.Drawing.Format32bppArgb liegt im Speicher auf
            ' Little-Endian-Windows ebenfalls als B, G, R, A vor.
            '
            ' Dadurch können wir die Pixeldaten ohne zusätzliche
            ' Kanalumsortierung direkt an D3D11 übergeben.

            bitmap = New Bitmap(quelle.Width, quelle.Height, PixelFormat.Format32bppArgb)

            Using graphics As Graphics = Graphics.FromImage(bitmap)

                graphics.DrawImage(quelle, New Rectangle(0, 0, bitmap.Width, bitmap.Height))

            End Using

            rectangle = New Rectangle(0, 0, bitmap.Width, bitmap.Height)

            bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

            textureDescription =
                New Texture2DDescription(
                    Format.B8G8R8A8_UNorm,
                    CUInt(bitmap.Width),
                    CUInt(bitmap.Height),
                    1UI,
                    1UI,
                    BindFlags.ShaderResource,
                    ResourceUsage.Default,
                    CpuAccessFlags.None,
                    1UI,
                    0UI,
                    ResourceOptionFlags.None)

            initialData =
                New SubresourceData() {
                    New SubresourceData(
                        bitmapData.Scan0,
                        CUInt(Math.Abs(bitmapData.Stride)),
                        CUInt(Math.Abs(bitmapData.Stride) * bitmap.Height))
                }

            texture = renderDevice.CreateTexture2D(textureDescription, initialData)

            If texture Is Nothing Then

                Throw New InvalidOperationException("Die D3D11-Quelltextur konnte nicht erzeugt werden.")

            End If

        Finally

            If bitmapData IsNot Nothing AndAlso bitmap IsNot Nothing Then

                bitmap.UnlockBits(bitmapData)

            End If

            If bitmap IsNot Nothing Then

                bitmap.Dispose()

            End If

        End Try

        Return texture

    End Function

    Friend Shared Function ErstelleRenderTargetTexture(renderDevice As ID3D11Device,
                                                       breite As Integer,
                                                       hoehe As Integer) As ID3D11Texture2D

        Dim textureDescription As Texture2DDescription
        Dim texture As ID3D11Texture2D

        texture = Nothing

        If renderDevice Is Nothing Then
            Throw New ArgumentNullException(NameOf(renderDevice))
        End If

        If breite <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(breite))
        End If

        If hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(hoehe))
        End If

        textureDescription =
            New Texture2DDescription(
                Format.B8G8R8A8_UNorm,
                CUInt(breite),
                CUInt(hoehe),
                1UI,
                1UI,
                BindFlags.RenderTarget,
                ResourceUsage.Default,
                CpuAccessFlags.None,
                1UI,
                0UI,
                ResourceOptionFlags.None)

        texture = renderDevice.CreateTexture2D(textureDescription)

        If texture Is Nothing Then

            Throw New InvalidOperationException("Die D3D11-RenderTarget-Texture konnte nicht erzeugt werden.")

        End If

        Return texture

    End Function

    Friend Shared Function ErstelleSimulationsTexture(renderDevice As ID3D11Device, breite As Integer,
                                                      hoehe As Integer, format As Format) As ID3D11Texture2D

        Dim textureDescription As Texture2DDescription
        Dim texture As ID3D11Texture2D

        texture = Nothing

        If renderDevice Is Nothing Then
            Throw New ArgumentNullException(NameOf(renderDevice))
        End If

        textureDescription =
        New Texture2DDescription(
            format,
            CUInt(breite),
            CUInt(hoehe),
            1UI,
            1UI,
            BindFlags.ShaderResource Or BindFlags.RenderTarget,
            ResourceUsage.Default,
            CpuAccessFlags.None,
            1UI,
            0UI,
            ResourceOptionFlags.None)

        texture = renderDevice.CreateTexture2D(textureDescription)

        If texture Is Nothing Then
            Throw New InvalidOperationException("Die D3D11-Simulations-Texture konnte nicht erzeugt werden.")
        End If

        Return texture

    End Function

    Friend Shared Function ErstelleStagingTexture(renderDevice As ID3D11Device,
                                                  breite As Integer,
                                                  hoehe As Integer) As ID3D11Texture2D

        Dim textureDescription As Texture2DDescription
        Dim texture As ID3D11Texture2D

        texture = Nothing

        If renderDevice Is Nothing Then
            Throw New ArgumentNullException(NameOf(renderDevice))
        End If

        If breite <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(breite))
        End If

        If hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(hoehe))
        End If

        ' Staging-Texturen dürfen nicht gleichzeitig RenderTarget oder
        ' ShaderResource sein.
        '
        ' Ihr einziger Zweck besteht hier darin, den vollständig von der
        ' GPU erzeugten Frame anschließend kontrolliert zur CPU zu holen.

        textureDescription =
            New Texture2DDescription(
                Format.B8G8R8A8_UNorm,
                CUInt(breite),
                CUInt(hoehe),
                1UI,
                1UI,
                BindFlags.None,
                ResourceUsage.Staging,
                CpuAccessFlags.Read,
                1UI,
                0UI,
                ResourceOptionFlags.None)

        texture = renderDevice.CreateTexture2D(textureDescription)

        If texture Is Nothing Then

            Throw New InvalidOperationException("Die D3D11-Staging-Texture konnte nicht erzeugt werden.")

        End If

        Return texture

    End Function

#End Region

#Region "Ressourcenfreigabe"

    Friend Shared Sub GebeFrei(ByRef resource As IDisposable)

        If resource Is Nothing Then
            Exit Sub
        End If

        Try

            resource.Dispose()

        Finally

            resource = Nothing

        End Try

    End Sub

#End Region

End Class