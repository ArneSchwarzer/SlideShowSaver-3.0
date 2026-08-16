Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports SharpGen.Runtime
Imports SlideShowDirect3DInterop
Imports SlideShowLogging
Imports SlideShowLogging.LogHandling
Imports TqkLibrary.Wpf.Interop.DirectX
Imports Vortice.Direct3D
Imports Vortice.Direct3D11
Imports Vortice.DXGI
Imports Vortice.Mathematics

Public Class BrennendesPapierD3DRenderer
    Implements IDisposable

#Region "Variablendeklaration"

#Region "Variablen"

    ' Persistentes D3D11 Device
    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext
    Private renderFeatureLevel As FeatureLevel

    Private d3dImage As D3D11Image

    'Partikel
    Private Const ANZAHL_PARTIKEL As UInteger = 8192UI
    Private Const PARTIKEL_STRIDE As UInteger = 32UI

    Private particleBuffer As ID3D11Buffer
    Private particleUnorderedAccessView As ID3D11UnorderedAccessView
    Private particleShaderResourceView As ID3D11ShaderResourceView

    Private particleComputeShader As ID3D11ComputeShader
    Private particleSimulationParameterBuffer As ID3D11Buffer

    Private particleRenderVertexShader As ID3D11VertexShader
    Private particleRenderPixelShader As ID3D11PixelShader
    Private particleRenderParameterBuffer As ID3D11Buffer

    Private particleTestStagingBuffer As ID3D11Buffer

    'Reveal-Texturen
    Private oldImageTexture As ID3D11Texture2D
    Private newImageTexture As ID3D11Texture2D
    Private maskTexture As ID3D11Texture2D

    'Reveal-ShaderResourceViews
    Private oldImageView As ID3D11ShaderResourceView
    Private newImageView As ID3D11ShaderResourceView
    Private maskView As ID3D11ShaderResourceView

    'Reveal-Shader
    Private revealVertexShader As ID3D11VertexShader
    Private revealPixelShader As ID3D11PixelShader

    Private revealSampler As ID3D11SamplerState
    Private revealParameterBuffer As ID3D11Buffer

    Private renderBreite As Integer
    Private renderHoehe As Integer

    'Gradienten-Shader
    Private gradientTexture As ID3D11Texture2D
    Private gradientView As ID3D11ShaderResourceView

    Private gradientVertexShader As ID3D11VertexShader
    Private gradientPixelShader As ID3D11PixelShader

    Private gradientParameterBuffer As ID3D11Buffer

    Private gradientBlendState As ID3D11BlendState

    Private brandkantenBreite As Single

    ' Partikel-Lebenszyklusgradient
    Private particleGradientTexture As ID3D11Texture2D
    Private particleGradientView As ID3D11ShaderResourceView

    ' Lifecycle
    Private wurdeBereinigt As Boolean
    Private istInitialisiert As Boolean


#End Region

#Region "Delegaten"

    Private Delegate Sub ClearRenderTargetViewDelegate(renderTargetView As ID3D11RenderTargetView, ByRef color As Color4)

#End Region

#Region "Structures"

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RevealShaderParameter

        Public progress As Single

        Public padding1 As Single
        Public padding2 As Single
        Public padding3 As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure GradientenShaderParameter

        Public progress As Single
        Public brandkantenBreite As Single

        Public padding1 As Single
        Public padding2 As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure Particle

        Public positionX As Single
        Public positionY As Single

        Public velocityX As Single
        Public velocityY As Single

        Public age As Single
        Public lifetime As Single

        Public size As Single

        Public seed As UInteger

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure ParticleSimulationParameter

        Public deltaTime As Single
        Public progress As Single
        Public emitterBreite As Single
        Public spawnRate As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure ParticleRenderParameter

        Public aspectCorrection As Single

        Public padding1 As Single
        Public padding2 As Single
        Public padding3 As Single

    End Structure

#End Region

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property FrameImage As ImageSource

        Get

            Return d3dImage

        End Get

    End Property

#End Region

#Region "Initialisierung"

    Public Sub Initialisiere(
        breite As Integer,
        hoehe As Integer,
        oldImage As BitmapSource,
        newImage As BitmapSource,
        brandMaske As BitmapSource,
        gradientBitmap As BitmapSource,
        particleGradientBitmap As BitmapSource,
        brandkantenBreite As Single)

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(BrennendesPapierD3DRenderer))
        End If

        If breite <= 0 OrElse hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(breite))
        End If

        If gradientBitmap Is Nothing Then
            Throw New ArgumentNullException(NameOf(gradientBitmap))
        End If

        If particleGradientBitmap Is Nothing Then
            Throw New ArgumentNullException(NameOf(particleGradientBitmap))
        End If

        LogHandling.LogDebug("D3D: Initialisiere() beginnt.")

        renderBreite = breite
        renderHoehe = hoehe

        Me.brandkantenBreite = Math.Max(0.001F, Math.Min(1.0F, brandkantenBreite))

        InitialisiereDirect3D()

        InitialisierePartikelBuffer()

        InitialisierePartikelComputeShader()
        InitialisierePartikelSimulationParameterBuffer()

        InitialisierePartikelRenderShader()
        InitialisierePartikelRenderParameterBuffer()

        InitialisierePartikelTestStagingBuffer()

        InitialisiereRevealTexturen(oldImage, newImage, brandMaske)
        InitialisiereGradientTextur(gradientBitmap)
        InitialisierePartikelGradientTextur(particleGradientBitmap)

        InitialisiereRevealShader()
        InitialisiereGradientShader()

        InitialisiereSampler()

        InitialisiereRevealParameterBuffer()
        InitialisiereGradientParameterBuffer()

        AktualisiereRevealParameter(0.0F)
        AktualisiereGradientParameter(0.0F)

        InitialisiereGradientBlendState()

        AktualisierePartikelRenderParameter()

        d3dImage = New D3D11Image()

        d3dImage.WindowOwner = Direct3DRessourceHandler.ErmittleInteropFensterHandle()

        d3dImage.OnRender = AddressOf RenderSurface

        d3dImage.SetPixelSize(breite, hoehe)

        istInitialisiert = True

        LogHandling.LogDebug("D3D: Initialisiere() beendet.")

    End Sub

#Region "Texturen und Gradinten"

    Private Sub InitialisiereRevealTexturen(oldImage As BitmapSource, newImage As BitmapSource,
                                            brandMaske As BitmapSource)

        oldImageTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, oldImage)
        newImageTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, newImage)
        maskTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, brandMaske)

        oldImageView = renderDevice.CreateShaderResourceView(oldImageTexture)
        newImageView = renderDevice.CreateShaderResourceView(newImageTexture)
        maskView = renderDevice.CreateShaderResourceView(maskTexture)

        LogHandling.LogDebug("D3D: Reveal-Texturen und ShaderResourceViews erzeugt.")

    End Sub

    Private Sub InitialisiereGradientTextur(gradientBitmap As BitmapSource)

        gradientTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, gradientBitmap)

        gradientView = renderDevice.CreateShaderResourceView(gradientTexture)

        LogHandling.LogDebug("D3D: Gradiententextur und ShaderResourceView erzeugt.")

    End Sub

#End Region

#Region "Shader"

    Private Sub InitialisiereRevealShader()

        Dim vertexShaderCode() As Byte
        Dim pixelShaderCode() As Byte

        vertexShaderCode = LadeShaderBytecode("RevealShaderVS.cso")
        pixelShaderCode = LadeShaderBytecode("RevealShaderPS.cso")

        revealVertexShader = renderDevice.CreateVertexShader(vertexShaderCode)
        revealPixelShader = renderDevice.CreatePixelShader(pixelShaderCode)

        LogHandling.LogDebug("D3D: RevealShader wurde geladen.")

    End Sub

    Private Sub InitialisiereGradientShader()

        Dim vertexShaderCode() As Byte
        Dim pixelShaderCode() As Byte

        vertexShaderCode = LadeShaderBytecode("GradientenShaderVS.cso")
        pixelShaderCode = LadeShaderBytecode("GradientenShaderPS.cso")

        gradientVertexShader = renderDevice.CreateVertexShader(vertexShaderCode)

        gradientPixelShader = renderDevice.CreatePixelShader(pixelShaderCode)

        LogHandling.LogDebug("D3D: GradientenShader wurde geladen.")

    End Sub

    Private Sub InitialisierePartikelGradientTextur(particleGradientBitmap As BitmapSource)

        particleGradientTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice,
                                                                                          particleGradientBitmap)

        particleGradientView = renderDevice.CreateShaderResourceView(particleGradientTexture)

        If particleGradientView Is Nothing Then

            Throw New InvalidOperationException("Die ShaderResourceView des Partikelgradienten " &
                                                "konnte nicht erzeugt werden.")

        End If

        LogHandling.LogDebug("D3D: Partikelgradiententextur und ShaderResourceView erzeugt.")

    End Sub

    Private Sub InitialisierePartikelComputeShader()

        Dim shaderBytes() As Byte

        shaderBytes = LadeShaderBytecode("PartikelComputeShaderCS.cso")

        particleComputeShader = renderDevice.CreateComputeShader(shaderBytes)

        If particleComputeShader Is Nothing Then
            Throw New InvalidOperationException("Der Partikel-Compute-Shader konnte nicht erzeugt werden.")
        End If

        LogHandling.LogDebug("D3D: Partikel-Compute-Shader wurde geladen.")

    End Sub

    Private Sub InitialisierePartikelRenderShader()

        Dim vertexShaderCode() As Byte
        Dim pixelShaderCode() As Byte

        vertexShaderCode = LadeShaderBytecode("PartikelRenderShaderVS.cso")
        pixelShaderCode = LadeShaderBytecode("PartikelRenderShaderPS.cso")

        particleRenderVertexShader = renderDevice.CreateVertexShader(vertexShaderCode)
        particleRenderPixelShader = renderDevice.CreatePixelShader(pixelShaderCode)

        If particleRenderVertexShader Is Nothing Then

            Throw New InvalidOperationException("Der Partikel-Render-VertexShader konnte nicht erzeugt werden.")

        End If

        If particleRenderPixelShader Is Nothing Then

            Throw New InvalidOperationException("Der Partikel-Render-PixelShader konnte nicht erzeugt werden.")

        End If

        LogHandling.LogDebug("D3D: Partikel-RenderShader wurde geladen.")

    End Sub

#End Region

#Region "Buffer"

    Private Sub InitialisiereRevealParameterBuffer()

        Dim bufferDescription As BufferDescription

        bufferDescription =
            New BufferDescription(
                16UI,
                BindFlags.ConstantBuffer,
                ResourceUsage.Dynamic,
                CpuAccessFlags.Write)

        revealParameterBuffer = renderDevice.CreateBuffer(bufferDescription)

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

    Private Sub InitialisierePartikelBuffer()

        Dim particleGroesse As Integer
        Dim bufferGroesse As UInteger
        Dim particles() As Particle

        particleGroesse = Marshal.SizeOf(GetType(Particle))

        If particleGroesse <> CInt(PARTIKEL_STRIDE) Then

            Throw New InvalidOperationException("Die Partikelstruktur besitzt eine unerwartete Größe. Erwartet: " &
                                                PARTIKEL_STRIDE.ToString() & " Byte, tatsächlich: " &
                                                particleGroesse.ToString() & " Byte.")

        End If

        If renderFeatureLevel <> FeatureLevel.Level_11_0 AndAlso
           renderFeatureLevel <> FeatureLevel.Level_11_1 Then

            LogHandling.LogWarn("D3D: Partikel-Buffer wird nicht initialisiert. " & "Das aktuelle FeatureLevel " &
                                renderFeatureLevel.ToString() & "unterstützt den vorgesehenen D3D11-Compute-Partikelpfad nicht.")

            Exit Sub

        End If

        bufferGroesse = ANZAHL_PARTIKEL * PARTIKEL_STRIDE

        particles = ErzeugePartikelInitialdaten()

        particleBuffer = renderDevice.CreateBuffer(Of Particle)(particles, BindFlags.ShaderResource Or
                                                                BindFlags.UnorderedAccess, ResourceUsage.Default,
                                                                CpuAccessFlags.None,
                                                                ResourceOptionFlags.BufferStructured,
                                                                CInt(bufferGroesse), CInt(PARTIKEL_STRIDE))

        If particleBuffer Is Nothing Then
            Throw New InvalidOperationException("Der D3D11-Partikelbuffer konnte nicht erzeugt werden.")
        End If

        particleUnorderedAccessView = renderDevice.CreateUnorderedAccessView(particleBuffer)

        If particleUnorderedAccessView Is Nothing Then
            Throw New InvalidOperationException("Die UnorderedAccessView des Partikelbuffers konnte nicht erzeugt werden.")
        End If

        particleShaderResourceView = renderDevice.CreateShaderResourceView(particleBuffer)

        If particleShaderResourceView Is Nothing Then
            Throw New InvalidOperationException("Die ShaderResourceView des Partikelbuffers konnte nicht erzeugt werden.")
        End If

        LogHandling.LogDebug("D3D: Partikelbuffer erfolgreich initialisiert. " & "Partikel: " & ANZAHL_PARTIKEL.ToString() &
                             ", Stride: " & PARTIKEL_STRIDE.ToString() & " Byte, Gesamtgröße: " & bufferGroesse.ToString() &
                             " Byte.")

        LogHandling.LogDebug("D3D: Partikel-UAV und Partikel-SRV erfolgreich erzeugt.")
        LogHandling.LogDebug("D3D: Partikelbuffer leer initialisiert. Alle Partikel werden GPU-seitig über den " &
                             "Brandkanten-Emitter erzeugt.")

    End Sub

    Private Sub InitialisierePartikelSimulationParameterBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer

        bufferGroesse = Marshal.SizeOf(GetType(ParticleSimulationParameter))

        If bufferGroesse <> 16 Then

            Throw New InvalidOperationException("Die Partikel-Simulationsparameter besitzen eine " &
                                                "unerwartete Größe. Erwartet: 16 Byte, tatsächlich: " &
                                                bufferGroesse.ToString() & " Byte.")

        End If

        bufferDescription = New BufferDescription()
        bufferDescription.ByteWidth = CUInt(bufferGroesse)
        bufferDescription.Usage = ResourceUsage.Default
        bufferDescription.BindFlags = BindFlags.ConstantBuffer
        bufferDescription.CPUAccessFlags = CpuAccessFlags.None
        bufferDescription.MiscFlags = ResourceOptionFlags.None
        bufferDescription.StructureByteStride = 0

        particleSimulationParameterBuffer = renderDevice.CreateBuffer(bufferDescription)

        If particleSimulationParameterBuffer Is Nothing Then

            Throw New InvalidOperationException("Der Partikel-Simulationsparameterbuffer " &
                                                "konnte nicht erzeugt werden.")

        End If

        LogHandling.LogDebug("D3D: Partikel-Simulationsparameterbuffer wurde erzeugt.")

    End Sub

    Private Sub InitialisierePartikelTestStagingBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As UInteger

        bufferGroesse = ANZAHL_PARTIKEL * PARTIKEL_STRIDE

        bufferDescription = New BufferDescription()
        bufferDescription.ByteWidth = bufferGroesse
        bufferDescription.Usage = ResourceUsage.Staging
        bufferDescription.BindFlags = BindFlags.None
        bufferDescription.CPUAccessFlags = CpuAccessFlags.Read
        bufferDescription.MiscFlags = ResourceOptionFlags.BufferStructured
        bufferDescription.StructureByteStride = PARTIKEL_STRIDE

        particleTestStagingBuffer = renderDevice.CreateBuffer(bufferDescription)

        If particleTestStagingBuffer Is Nothing Then

            Throw New InvalidOperationException("Der Partikel-Test-Staging-Buffer konnte nicht erzeugt werden.")

        End If

        LogHandling.LogDebug("D3D: Partikel-Test-Staging-Buffer wurde erzeugt.")

    End Sub

    Private Sub InitialisierePartikelRenderParameterBuffer()

        Dim bufferDescription As BufferDescription

        bufferDescription = New BufferDescription()
        bufferDescription.ByteWidth = 16UI
        bufferDescription.Usage = ResourceUsage.Default
        bufferDescription.BindFlags = BindFlags.ConstantBuffer
        bufferDescription.CPUAccessFlags = CpuAccessFlags.None
        bufferDescription.MiscFlags = ResourceOptionFlags.None
        bufferDescription.StructureByteStride = 0UI

        particleRenderParameterBuffer = renderDevice.CreateBuffer(bufferDescription)

        If particleRenderParameterBuffer Is Nothing Then

            Throw New InvalidOperationException("Der Partikel-Renderparameterbuffer konnte nicht erzeugt werden.")

        End If

        LogHandling.LogDebug("D3D: Partikel-Renderparameterbuffer wurde erzeugt.")

    End Sub

#End Region

#Region "Sonstiges und Hilfsfunktionen"

    Private Sub InitialisiereGradientBlendState()

        Dim blendDescription As BlendDescription

        blendDescription = BlendDescription.NonPremultiplied

        gradientBlendState = renderDevice.CreateBlendState(blendDescription)

    End Sub

    Private Sub InitialisiereDirect3D()

        Dim featureLevels() As FeatureLevel
        Dim featureLevel As FeatureLevel
        Dim result As Result

        featureLevels =
        New FeatureLevel() {
            FeatureLevel.Level_11_1,
            FeatureLevel.Level_11_0,
            FeatureLevel.Level_10_1,
            FeatureLevel.Level_10_0
        }

        result = D3D11.D3D11CreateDevice(IntPtr.Zero, DriverType.Hardware, DeviceCreationFlags.BgraSupport,
                                  featureLevels, renderDevice, featureLevel, renderContext)

        If result.Failure Then

            Throw New InvalidOperationException("Das Direct3D11-Device konnte nicht erzeugt werden. HRESULT: " &
                                                result.Code.ToString())

        End If

        renderFeatureLevel = featureLevel

        LogHandling.LogDebug("D3D: Eigenes D3D11-Device initialisiert. FeatureLevel: " & featureLevel.ToString())

    End Sub

    Private Sub InitialisiereSampler()

        Dim samplerDescription As SamplerDescription

        samplerDescription = New SamplerDescription()
        samplerDescription.Filter = Filter.MinMagMipLinear
        samplerDescription.AddressU = TextureAddressMode.Clamp
        samplerDescription.AddressV = TextureAddressMode.Clamp
        samplerDescription.AddressW = TextureAddressMode.Clamp
        samplerDescription.MinLOD = 0.0F
        samplerDescription.MaxLOD = Single.MaxValue

        revealSampler = renderDevice.CreateSamplerState(samplerDescription)

    End Sub

    Private Function ErzeugePartikelInitialdaten() As Particle()

        Dim particles(CInt(ANZAHL_PARTIKEL) - 1) As Particle

        Return particles

    End Function

#End Region

#End Region

#Region "Testing"

    Private Sub FuehrePartikelComputeTestAus()

        Const THREADS_PRO_GRUPPE As UInteger = 64UI

        Dim anzahlThreadGruppen As UInteger

        anzahlThreadGruppen = (ANZAHL_PARTIKEL + THREADS_PRO_GRUPPE - 1UI) \ THREADS_PRO_GRUPPE

        ' renderContext.CSSetShader(particleTestComputeShader)
        renderContext.CSSetUnorderedAccessView(0, particleUnorderedAccessView)
        renderContext.Dispatch(anzahlThreadGruppen, 1, 1)

        ' UAV unbedingt wieder lösen.
        renderContext.CSSetUnorderedAccessView(0, Nothing)
        renderContext.CSSetShader(Nothing)

        LogHandling.LogDebug("D3D: Partikel-Test-Compute-Shader ausgeführt. Threadgruppen: " &
                             anzahlThreadGruppen.ToString())

    End Sub

    Private Sub KopierePartikelbufferInStagingBuffer()

        renderContext.CopyResource(particleTestStagingBuffer, particleBuffer)

    End Sub

    Private Sub PruefePartikelComputeTest()
        '
        Dim mappedResource As MappedSubresource
        Dim particlePointer As IntPtr

        Dim particle As Particle
        Dim particleIndex As Integer

        renderContext.Map(particleTestStagingBuffer, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None, mappedResource)

        Try

            particlePointer = mappedResource.DataPointer

            For particleIndex = 0 To 3

                particle = Marshal.PtrToStructure(Of Particle)(IntPtr.Add(particlePointer, particleIndex *
                                                                          CInt(PARTIKEL_STRIDE)))

                LogHandling.LogDebug(
                    "D3D Partikel-Test Slot " &
                    particleIndex.ToString() &
                    ": Position=(" &
                    particle.positionX.ToString("0.000000") &
                    ", " &
                    particle.positionY.ToString("0.000000") &
                    "), Velocity=(" &
                    particle.velocityX.ToString("0.000000") &
                    ", " &
                    particle.velocityY.ToString("0.000000") &
                    "), Age=" &
                    particle.age.ToString("0.000000") &
                    ", Lifetime=" &
                    particle.lifetime.ToString("0.000000") &
                    ", Size=" &
                    particle.size.ToString("0.000000") &
                    ", Seed=0x" &
                    particle.seed.ToString("X8"))

            Next

        Finally

            renderContext.Unmap(particleTestStagingBuffer, 0)

        End Try

    End Sub

    Private Sub FuehrePartikelHelloWorldTestAus()

        LogHandling.LogDebug("D3D: Partikel-Compute-Hello-World-Test beginnt.")

        FuehrePartikelComputeTestAus()

        KopierePartikelbufferInStagingBuffer()

        PruefePartikelComputeTest()

        LogHandling.LogDebug("D3D: Partikel-Compute-Hello-World-Test beendet.")

    End Sub

    Private Sub FuehrePartikelSimulationsschrittAus(deltaTime As Single, progress As Single)

        Const THREADS_PRO_GRUPPE As UInteger = 64UI

        Dim parameter As ParticleSimulationParameter
        Dim anzahlThreadGruppen As UInteger

        parameter.deltaTime = deltaTime
        parameter.progress = progress
        parameter.emitterBreite = 0.02F
        parameter.spawnRate = 2400.0F

        renderContext.UpdateSubresource(parameter, particleSimulationParameterBuffer)

        anzahlThreadGruppen = (ANZAHL_PARTIKEL + THREADS_PRO_GRUPPE - 1UI) \ THREADS_PRO_GRUPPE

        renderContext.CSSetShader(particleComputeShader)
        renderContext.CSSetConstantBuffer(0UI, particleSimulationParameterBuffer)
        renderContext.CSSetUnorderedAccessView(0UI, particleUnorderedAccessView)

        ' CS t0 = zeitcodierte Brandmaske
        renderContext.CSSetShaderResource(0UI, maskView)
        renderContext.Dispatch(anzahlThreadGruppen, 1UI, 1UI)

        ' Unbedingt wieder lösen:
        ' maskView wird anschließend im PixelShader benötigt,
        ' particleBuffer anschließend als SRV im VertexShader.

        renderContext.CSSetShaderResource(0UI, Nothing)
        renderContext.CSSetUnorderedAccessView(0UI, Nothing)
        renderContext.CSSetConstantBuffer(0UI, Nothing)
        renderContext.CSSetShader(Nothing)

    End Sub

#End Region

#Region "Rendering"

    Private Sub AktualisiereRevealParameter(progress As Single)

        Dim mappedResource As MappedSubresource
        Dim parameter As RevealShaderParameter

        parameter.progress = Math.Max(0.0F, Math.Min(1.0F, progress))

        parameter.padding1 = 0.0F
        parameter.padding2 = 0.0F
        parameter.padding3 = 0.0F

        mappedResource = renderContext.Map(revealParameterBuffer, 0UI, MapMode.WriteDiscard,
                                           Vortice.Direct3D11.MapFlags.None)

        Marshal.StructureToPtr(parameter, mappedResource.DataPointer, False)

        renderContext.Unmap(revealParameterBuffer, 0UI)

    End Sub

    Private Sub AktualisiereGradientParameter(progress As Single)

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

    Private Sub AktualisierePartikelRenderParameter()

        Dim parameter As ParticleRenderParameter

        If renderBreite <= 0 OrElse renderHoehe <= 0 Then

            Throw New InvalidOperationException("Die Rendergröße für die Partikeldarstellung ist ungültig.")

        End If

        parameter.aspectCorrection = CSng(renderHoehe) / CSng(renderBreite)

        parameter.padding1 = 0.0F
        parameter.padding2 = 0.0F
        parameter.padding3 = 0.0F

        renderContext.UpdateSubresource(parameter, particleRenderParameterBuffer)

    End Sub

    Public Sub RenderFrame(progress As Single, deltaTime As Single)

        If wurdeBereinigt Then
            Exit Sub
        End If

        If Not istInitialisiert Then
            Exit Sub
        End If

        If d3dImage Is Nothing Then
            Exit Sub
        End If

        AktualisiereRevealParameter(progress)

        AktualisiereGradientParameter(progress)

        If deltaTime > 0.0F Then

            FuehrePartikelSimulationsschrittAus(deltaTime, progress)

        End If

        d3dImage.RequestRender()

    End Sub

    Private Sub RenderSurface(surfacePointer As IntPtr, isNewSurface As Boolean)

        Dim surface As IDXGISurface
        Dim dxgiResource As IDXGIResource
        Dim sharedTexture As ID3D11Texture2D
        Dim sharedHandle As IntPtr
        Dim renderTargetView As ID3D11RenderTargetView

        surface = Nothing
        dxgiResource = Nothing
        sharedTexture = Nothing
        sharedHandle = IntPtr.Zero
        renderTargetView = Nothing

        If surfacePointer = IntPtr.Zero Then
            Exit Sub
        End If

        If oldImageView Is Nothing OrElse
       newImageView Is Nothing OrElse
       maskView Is Nothing Then

            Exit Sub

        End If

        Try

            surface = New IDXGISurface(surfacePointer)

            dxgiResource = surface.QueryInterface(Of IDXGIResource)()

            sharedHandle = dxgiResource.SharedHandle

            If sharedHandle = IntPtr.Zero Then

                Throw New InvalidOperationException("Die von Tqk gelieferte DXGI-Resource besitzt " &
                                                    "keinen gültigen SharedHandle.")

            End If

            sharedTexture = renderDevice.OpenSharedResource(Of ID3D11Texture2D)(sharedHandle)

            renderTargetView = renderDevice.CreateRenderTargetView(sharedTexture)

            renderContext.OMSetRenderTargets(renderTargetView)
            renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(renderBreite), CSng(renderHoehe), 0.0F, 1.0F))
            renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)

            '---------------------------------
            ' Pass 1: Reveal
            '---------------------------------

            renderContext.OMSetBlendState(Nothing)
            renderContext.VSSetShader(revealVertexShader)
            renderContext.PSSetShader(revealPixelShader)
            renderContext.PSSetShaderResource(0UI, oldImageView)
            renderContext.PSSetShaderResource(1UI, newImageView)
            renderContext.PSSetShaderResource(2UI, maskView)
            renderContext.PSSetSampler(0UI, revealSampler)
            renderContext.PSSetConstantBuffer(0UI, revealParameterBuffer)

            renderContext.Draw(3UI, 0UI)

            renderContext.PSSetShaderResource(0UI, Nothing)
            renderContext.PSSetShaderResource(1UI, Nothing)
            renderContext.PSSetShaderResource(2UI, Nothing)

            '---------------------------------
            ' Pass 2: Gradient
            '---------------------------------

            renderContext.OMSetBlendState(gradientBlendState)
            renderContext.VSSetShader(gradientVertexShader)
            renderContext.PSSetShader(gradientPixelShader)
            renderContext.PSSetShaderResource(0UI, maskView)
            renderContext.PSSetShaderResource(1UI, gradientView)
            renderContext.PSSetSampler(0UI, revealSampler)
            renderContext.PSSetConstantBuffer(0UI, gradientParameterBuffer)

            renderContext.Draw(3UI, 0UI)

            renderContext.PSSetShaderResource(0UI, Nothing)
            renderContext.PSSetShaderResource(1UI, Nothing)
            renderContext.OMSetBlendState(Nothing)

            '---------------------------------
            ' Pass 3: Partikel
            '---------------------------------

            renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
            renderContext.VSSetShader(particleRenderVertexShader)
            renderContext.PSSetShader(particleRenderPixelShader)

            renderContext.OMSetBlendState(gradientBlendState)

            ' VS t0 = StructuredBuffer<Particle>
            renderContext.VSSetShaderResource(0UI, particleShaderResourceView)

            ' VS b0 = ParticleRenderParameter
            renderContext.VSSetConstantBuffer(0UI, particleRenderParameterBuffer)

            ' PS t0 = Lebenszyklusgradient
            renderContext.PSSetShaderResource(0UI, particleGradientView)

            ' PS s0 = linearer Sampler
            renderContext.PSSetSampler(0UI, revealSampler)

            renderContext.Draw(ANZAHL_PARTIKEL * 6UI, 0UI)

            ' Ressourcen wieder lösen.

            renderContext.VSSetShaderResource(0UI, Nothing)
            renderContext.VSSetConstantBuffer(0UI, Nothing)
            renderContext.PSSetShaderResource(0UI, Nothing)
            renderContext.PSSetSampler(0UI, Nothing)
            renderContext.OMSetBlendState(Nothing)

            '---------------------------------
            ' Frame abschließen
            '---------------------------------

            D3D11InteropHelper.UnbindRenderTarget(renderContext)

            renderContext.Flush()

        Catch ex As Exception

            LogHandling.LogError("D3D: Fehler beim Rendern: " & ex.ToString())

        Finally

            If renderTargetView IsNot Nothing Then

                renderTargetView.Dispose()
                renderTargetView = Nothing

            End If

            If sharedTexture IsNot Nothing Then

                sharedTexture.Dispose()
                sharedTexture = Nothing

            End If

            If dxgiResource IsNot Nothing Then

                dxgiResource.Dispose()
                dxgiResource = Nothing

            End If

            surface = Nothing

        End Try

    End Sub

#End Region

#Region "Hilfsfunktionen"

    Private Function LadeShaderBytecode(dateiname As String) As Byte()

        Dim assembly As Reflection.Assembly
        Dim ressourcenNamen() As String
        Dim ressourcenName As String
        Dim stream As IO.Stream
        Dim daten() As Byte

        assembly = Reflection.Assembly.GetExecutingAssembly()

        ressourcenNamen = assembly.GetManifestResourceNames()

        ressourcenName =
        ressourcenNamen.FirstOrDefault(
            Function(name)
                Return name.EndsWith(
                    "." & dateiname,
                    StringComparison.OrdinalIgnoreCase)
            End Function)

        If String.IsNullOrEmpty(ressourcenName) Then

            Throw New IO.FileNotFoundException(
            "Die eingebettete Shader-Ressource """ &
            dateiname &
            """ wurde nicht gefunden.")

        End If

        stream = assembly.GetManifestResourceStream(ressourcenName)

        If stream Is Nothing Then

            Throw New IO.FileNotFoundException("Die Shader-Ressource """ & dateiname &
                                               """ konnte nicht geöffnet werden.")

        End If

        Using stream

            ReDim daten(CInt(stream.Length) - 1)

            stream.Read(daten, 0, daten.Length)

        End Using

        Return daten

    End Function

#End Region

#Region "Bereinigung & Dispose"

    Private Sub BeendeUndBereinigeRenderer()

        istInitialisiert = False

        renderBreite = 0
        renderHoehe = 0

        ' Keine neuen Tqk-Callbacks mehr
        If d3dImage IsNot Nothing Then

            d3dImage.OnRender = Nothing

            d3dImage.Dispose()
            d3dImage = Nothing

        End If

        ' Entferne RenderContext und Ressourcen
        If renderContext IsNot Nothing Then

            renderContext.ClearState()
            renderContext.Flush()

        End If

        '---------------------------------
        ' Partikel: Renderpipeline
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(particleRenderParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(particleRenderPixelShader)
        Direct3DRessourceHandler.GebeFrei(particleRenderVertexShader)

        '---------------------------------
        ' Partikel: Simulation
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(particleSimulationParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(particleComputeShader)

        '---------------------------------
        ' Partikel: Test / Diagnose
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(particleTestStagingBuffer)

        '---------------------------------
        ' Partikel: Buffer-Views
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(particleShaderResourceView)
        Direct3DRessourceHandler.GebeFrei(particleUnorderedAccessView)

        '---------------------------------
        ' Partikel: Hauptbuffer
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(particleBuffer)

        '---------------------------------
        ' Constant Buffer
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(gradientParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(revealParameterBuffer)

        '---------------------------------
        ' Blend States
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(gradientBlendState)

        '---------------------------------
        ' Sampler
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(revealSampler)

        '---------------------------------
        ' Shader Resource Views
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(particleGradientView)
        Direct3DRessourceHandler.GebeFrei(gradientView)
        Direct3DRessourceHandler.GebeFrei(maskView)
        Direct3DRessourceHandler.GebeFrei(newImageView)
        Direct3DRessourceHandler.GebeFrei(oldImageView)

        '---------------------------------
        ' Texturen
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(particleGradientTexture)
        Direct3DRessourceHandler.GebeFrei(gradientTexture)
        Direct3DRessourceHandler.GebeFrei(maskTexture)
        Direct3DRessourceHandler.GebeFrei(newImageTexture)
        Direct3DRessourceHandler.GebeFrei(oldImageTexture)

        '---------------------------------
        ' GradientenShader
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(gradientPixelShader)
        Direct3DRessourceHandler.GebeFrei(gradientVertexShader)

        '---------------------------------
        ' RevealShader
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(revealPixelShader)
        Direct3DRessourceHandler.GebeFrei(revealVertexShader)

        ' D3D11-Context
        If renderContext IsNot Nothing Then

            renderContext.ClearState()
            renderContext.Flush()

            renderContext.Dispose()
            renderContext = Nothing

        End If

        ' D3D11-Device
        If renderDevice IsNot Nothing Then

            renderDevice.Dispose()
            renderDevice = Nothing

        End If

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        BeendeUndBereinigeRenderer()

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class