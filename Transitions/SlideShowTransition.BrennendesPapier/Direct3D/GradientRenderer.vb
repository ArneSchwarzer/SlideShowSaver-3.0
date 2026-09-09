Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Media.Imaging
Imports SharpGen.Runtime
Imports SlideShowDirect3DInterop
Imports SlideShowLogging
Imports Vortice.Direct3D11
Imports TqkLibrary.Wpf.Interop.DirectX
Imports Vortice.Direct3D
Imports Vortice.DXGI
Imports Vortice.Mathematics

Friend Class GradientRenderer
    Implements IDisposable

#Region "Variablendeklaration"

    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext

    Private gradientTexture As ID3D11Texture2D
    Private gradientView As ID3D11ShaderResourceView

    Private gradientVertexShader As ID3D11VertexShader
    Private gradientPixelShader As ID3D11PixelShader

    Private gradientParameterBuffer As ID3D11Buffer

    Private gradientBlendState As ID3D11BlendState

    Private brandkantenBreite As Single

    Private wurdeBereinigt As Boolean

    <StructLayout(LayoutKind.Sequential)>
    Private Structure GradientenShaderParameter

        Public progress As Single
        Public brandkantenBreite As Single

        Public padding1 As Single
        Public padding2 As Single

    End Structure

#End Region

#Region "Initialisierung"

    Friend Sub Initialisiere(device As ID3D11Device, context As ID3D11DeviceContext, gradientBitmap As BitmapSource,
                             brandkantenBreite As Single)

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(GradientRenderer))
        End If

        If device Is Nothing Then
            Throw New ArgumentNullException(NameOf(device))
        End If

        If context Is Nothing Then
            Throw New ArgumentNullException(NameOf(context))
        End If

        If gradientBitmap Is Nothing Then
            Throw New ArgumentNullException(NameOf(gradientBitmap))
        End If

        renderDevice = device
        renderContext = context

        Me.brandkantenBreite = Math.Max(0.001F, Math.Min(1.0F, brandkantenBreite))

        InitialisiereGradientTextur(gradientBitmap)
        InitialisiereGradientShader()
        InitialisiereGradientParameterBuffer()
        InitialisiereGradientBlendState()

        AktualisiereParameter(0.0F)

    End Sub

    Private Sub InitialisiereGradientTextur(gradientBitmap As BitmapSource)

        gradientTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, gradientBitmap)

        gradientView = renderDevice.CreateShaderResourceView(gradientTexture)

        LogHandling.LogDebug("D3D: Gradiententextur und ShaderResourceView erzeugt.")

    End Sub

    Private Sub InitialisiereGradientShader()

        Dim vertexShaderCode() As Byte
        Dim pixelShaderCode() As Byte

        vertexShaderCode = D3DRenderer.LadeShaderBytecode("GradientenShaderVS.cso")
        pixelShaderCode = D3DRenderer.LadeShaderBytecode("GradientenShaderPS.cso")

        gradientVertexShader = renderDevice.CreateVertexShader(vertexShaderCode)
        gradientPixelShader = renderDevice.CreatePixelShader(pixelShaderCode)

        LogHandling.LogDebug("D3D: GradientenShader wurde geladen.")

    End Sub

    Private Sub InitialisiereGradientParameterBuffer()

        Dim bufferDescription As BufferDescription

        bufferDescription =
        New BufferDescription(
            16UI,
            BindFlags.ConstantBuffer,
            ResourceUsage.Dynamic,
            CpuAccessFlags.Write)

        gradientParameterBuffer = renderDevice.CreateBuffer(bufferDescription)

    End Sub

    Private Sub InitialisiereGradientBlendState()

        Dim blendDescription As BlendDescription

        blendDescription = BlendDescription.NonPremultiplied

        gradientBlendState = renderDevice.CreateBlendState(blendDescription)

    End Sub

#End Region

#Region "Rendering"

    Friend Sub AktualisiereParameter(progress As Single)

        Dim mappedResource As MappedSubresource
        Dim parameter As GradientenShaderParameter

        parameter.progress = Math.Max(0.0F, Math.Min(1.0F, progress))
        parameter.brandkantenBreite = brandkantenBreite
        parameter.padding1 = 0.0F
        parameter.padding2 = 0.0F

        mappedResource = renderContext.Map(gradientParameterBuffer, 0UI, MapMode.WriteDiscard,
                                           Vortice.Direct3D11.MapFlags.None)

        Marshal.StructureToPtr(parameter, mappedResource.DataPointer, False)

        renderContext.Unmap(gradientParameterBuffer, 0UI)

    End Sub

    Friend Sub Render(maskView As ID3D11ShaderResourceView, sampler As ID3D11SamplerState)

        If maskView Is Nothing Then
            Throw New ArgumentNullException(NameOf(maskView))
        End If

        If sampler Is Nothing Then
            Throw New ArgumentNullException(NameOf(sampler))
        End If

        renderContext.OMSetBlendState(gradientBlendState)
        renderContext.VSSetShader(gradientVertexShader)
        renderContext.PSSetShader(gradientPixelShader)
        renderContext.PSSetShaderResource(0UI, maskView)
        renderContext.PSSetShaderResource(1UI, gradientView)
        renderContext.PSSetSampler(0UI, sampler)
        renderContext.PSSetConstantBuffer(0UI, gradientParameterBuffer)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetShaderResource(1UI, Nothing)
        renderContext.OMSetBlendState(Nothing)

    End Sub

#End Region

#Region "Bereinigung & Dispose"

    Private Sub BeendeUndBereinigeGradientRenderer()

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        Direct3DRessourceHandler.GebeFrei(gradientParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(gradientBlendState)
        Direct3DRessourceHandler.GebeFrei(gradientView)
        Direct3DRessourceHandler.GebeFrei(gradientTexture)
        Direct3DRessourceHandler.GebeFrei(gradientPixelShader)
        Direct3DRessourceHandler.GebeFrei(gradientVertexShader)

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        If wurdeBereinigt Then
            Exit Sub
        End If

        BeendeUndBereinigeGradientRenderer()

        GC.SuppressFinalize(Me)

    End Sub


#End Region

End Class
