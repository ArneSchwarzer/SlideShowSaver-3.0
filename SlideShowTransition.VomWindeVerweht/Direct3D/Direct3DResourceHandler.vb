Imports System.Runtime.InteropServices
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports SlideShowDirect3DInterop
Imports Vortice.Direct3D11
Imports Vortice.DXGI

Public NotInheritable Class Direct3DRessourceHandler

    Private Sub New()

    End Sub

#Region "Native Funktionen"

    <DllImport("user32.dll", SetLastError:=False)>
    Private Shared Function GetDesktopWindow() As IntPtr

    End Function

#End Region

#Region "Fensterhandling"

    Public Shared Function ErmittleInteropFensterHandle() As IntPtr

        Dim handle As IntPtr

        handle = GetDesktopWindow()

        If handle = IntPtr.Zero Then

            Throw New InvalidOperationException("Für die Direct3D-WPF-Interop konnte kein gültiges Fensterhandle " &
                                                "bestimmt werden.")

        End If

        Return handle

    End Function

#End Region

#Region "Texture-Erzeugung"

    Public Shared Function ErstelleTextureAusBitmapSource(renderDevice As ID3D11Device, quelle As BitmapSource) _
        As ID3D11Texture2D

        Dim bitmap As BitmapSource
        Dim konvertiertesBitmap As FormatConvertedBitmap
        Dim pixel() As Byte
        Dim stride As Integer
        Dim textureDescription As Texture2DDescription
        Dim initialData() As SubresourceData
        Dim pixelHandle As GCHandle
        Dim texture As ID3D11Texture2D

        bitmap = Nothing
        konvertiertesBitmap = Nothing
        pixel = Nothing
        texture = Nothing

        If renderDevice Is Nothing Then
            Throw New ArgumentNullException(NameOf(renderDevice))
        End If

        If quelle Is Nothing Then
            Throw New ArgumentNullException(NameOf(quelle))
        End If

        If quelle.Format = PixelFormats.Bgra32 Then

            bitmap = quelle

        Else

            konvertiertesBitmap = New FormatConvertedBitmap(quelle, PixelFormats.Bgra32, Nothing, 0.0)
            konvertiertesBitmap.Freeze()

            bitmap = konvertiertesBitmap

        End If

        stride = bitmap.PixelWidth * 4

        pixel = New Byte(stride * bitmap.PixelHeight - 1) {}

        bitmap.CopyPixels(pixel, stride, 0)

        Try

            pixelHandle = GCHandle.Alloc(pixel, GCHandleType.Pinned)

            textureDescription =
                New Texture2DDescription(
                    Format.B8G8R8A8_UNorm,
                    CUInt(bitmap.PixelWidth),
                    CUInt(bitmap.PixelHeight),
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
                        pixelHandle.AddrOfPinnedObject(),
                        CUInt(stride),
                        CUInt(stride * bitmap.PixelHeight))
                }

            texture = renderDevice.CreateTexture2D(textureDescription, initialData)

        Finally

            If pixelHandle.IsAllocated Then
                pixelHandle.Free()
            End If

        End Try

        Return texture

    End Function

#End Region

#Region "Ressourcenfreigabe"

    Public Shared Sub GebeFrei(ByRef resource As IDisposable)

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