Imports Vortice.Direct3D
Imports Vortice.Direct3D11
Imports System.Runtime.InteropServices
Imports System.Windows.Media.Imaging
Imports SlideShowDirect3DInterop
Imports SlideShowLogging

Friend Class PartikelRendererAPC
    Implements IDisposable

#Region "Variablendeklaration"

#Region "Konstanten"

    Private Const ANZAHL_PARTIKEL As UInteger = 8192UI
    Private Const PARTIKEL_STRIDE As UInteger = 32UI

#End Region

#Region "Variablen"

    '---------------------------------
    ' Direct3D / Rendering
    '---------------------------------
    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext
    Private renderFeatureLevel As FeatureLevel

    Private renderBreite As Integer
    Private renderHoehe As Integer

    '----------------------------------
    ' Partikel - Simulation
    '----------------------------------
    Private particleComputeShader As ID3D11ComputeShader
    Private particleSimulationParameterBuffer As ID3D11Buffer

    Private schwerkraftIstAktiv As Boolean
    Private partikelMaxLebensdauer As Single
    Private partikelSimulationsZeit As Single

    '----------------------------------
    ' Partikel - Rendering
    '----------------------------------
    Private particleBuffer As ID3D11Buffer
    Private particleUnorderedAccessView As ID3D11UnorderedAccessView
    Private particleShaderResourceView As ID3D11ShaderResourceView

    Private particleRenderVertexShader As ID3D11VertexShader
    Private particleRenderPixelShader As ID3D11PixelShader
    Private particleRenderParameterBuffer As ID3D11Buffer

    '---------------------------------
    ' APC - Render-Partikelindizes
    '---------------------------------
    Private renderPartikelIndexBuffer As ID3D11Buffer
    Private renderPartikelIndexView As ID3D11ShaderResourceView
    Private renderPartikelIndexUnorderedAccessView As ID3D11UnorderedAccessView

    '---------------------------------
    ' APC - Indirekte Renderargumente
    '---------------------------------
    Private indirectArgumentBuffer As ID3D11Buffer

    '---------------------------------
    ' Partikel-Gradienten
    '---------------------------------
    Private particleGradientTexture As ID3D11Texture2D
    Private particleGradientView As ID3D11ShaderResourceView

    Private particleBlendState As ID3D11BlendState

    '---------------------------------
    ' Haushalt sauber halten
    '---------------------------------
    Private wurdeBereinigt As Boolean

    'Debugging
    '
    '---------------------------------
    ' APC - temporäre Diagnose
    '---------------------------------
    '
    ' CPU-lesbare Kopie des IndirectArgumentBuffers.
    ' Wird ausschließlich verwendet, um während der
    ' APC-Fehlersuche InstanceCount auszulesen.

    Private indirectArgumentStagingBuffer As ID3D11Buffer

#End Region

#Region "Structures & Enums"

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

        Public schwerkraftAktiv As Single
        Public zeit As Single
        Public flowFieldStaerke As Single
        Public partikelMaxLebensdauer As Single

        Public padding1 As Single
        Public padding2 As Single
        Public padding3 As Single
        Public padding4 As Single

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

#Region "Initialisierung"

    Friend Sub Initialisiere(device As ID3D11Device, context As ID3D11DeviceContext, featureLevel As FeatureLevel,
                             breite As Integer, hoehe As Integer, gradientBitmap As BitmapSource,
                             schwerkraftAktiv As Boolean, partikelLebensdauer As Single)

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(PartikelRendererAPC))
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

        If breite <= 0 OrElse hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(breite))
        End If

        renderDevice = device
        renderContext = context
        renderFeatureLevel = featureLevel

        renderBreite = breite
        renderHoehe = hoehe

        schwerkraftIstAktiv = schwerkraftAktiv

        partikelMaxLebensdauer = Math.Max(0.1F, partikelLebensdauer)
        partikelSimulationsZeit = 0.0F

        InitialisierePartikelBuffer()
        InitialisiereRenderPartikelIndexBuffer()
        InitialisiereIndirectArgumentBuffer()

        'Debugging
        InitialisiereIndirectArgumentStagingBuffer()

        InitialisierePartikelComputeShader()
        InitialisierePartikelSimulationParameterBuffer()

        InitialisierePartikelRenderShader()
        InitialisierePartikelRenderParameterBuffer()

        InitialisierePartikelGradientTextur(gradientBitmap)

        InitialisierePartikelBlendState()

        AktualisierePartikelRenderParameter()

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

        shaderBytes = D3DRenderer.LadeShaderBytecode("PartikelComputeShaderAPCCS.cso")

        particleComputeShader = renderDevice.CreateComputeShader(shaderBytes)

        If particleComputeShader Is Nothing Then
            Throw New InvalidOperationException("Der Partikel-Compute-Shader konnte nicht erzeugt werden.")
        End If

        LogHandling.LogDebug("D3D: Partikel-Compute-Shader wurde geladen.")

    End Sub

    Private Sub InitialisierePartikelRenderShader()

        Dim vertexShaderCode() As Byte
        Dim pixelShaderCode() As Byte

        vertexShaderCode = D3DRenderer.LadeShaderBytecode("PartikelRenderShaderAPCVS.cso")
        pixelShaderCode = D3DRenderer.LadeShaderBytecode("PartikelRenderShaderAPCPS.cso")

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

    Private Sub InitialisiereRenderPartikelIndexBuffer()

        Dim description As BufferDescription
        Dim viewDescription As UnorderedAccessViewDescription

        description = New BufferDescription()

        description.ByteWidth = ANZAHL_PARTIKEL * 4UI
        description.Usage = ResourceUsage.Default
        description.BindFlags = BindFlags.ShaderResource Or BindFlags.UnorderedAccess
        description.CPUAccessFlags = CpuAccessFlags.None
        description.MiscFlags = ResourceOptionFlags.BufferStructured
        description.StructureByteStride = 4UI

        renderPartikelIndexBuffer = renderDevice.CreateBuffer(description)

        If renderPartikelIndexBuffer Is Nothing Then

            Throw New InvalidOperationException(
            "Der APC-RenderPartikelIndexBuffer konnte nicht erzeugt werden.")

        End If

        renderPartikelIndexView = renderDevice.CreateShaderResourceView(renderPartikelIndexBuffer)

        If renderPartikelIndexView Is Nothing Then

            Throw New InvalidOperationException(
            "Die ShaderResourceView des APC-RenderPartikelIndexBuffers konnte nicht erzeugt werden.")

        End If

        viewDescription =
        New UnorderedAccessViewDescription(
            renderPartikelIndexBuffer,
            Vortice.DXGI.Format.Unknown,
            0UI,
            ANZAHL_PARTIKEL,
            BufferUnorderedAccessViewFlags.Append)

        renderPartikelIndexUnorderedAccessView = renderDevice.CreateUnorderedAccessView(renderPartikelIndexBuffer,
                                                                                        viewDescription)

        If renderPartikelIndexUnorderedAccessView Is Nothing Then

            Throw New InvalidOperationException("Die APC-Append-UAV konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisiereIndirectArgumentBuffer()

        Dim argumente() As UInteger

        argumente = New UInteger() {6UI, 0UI, 0UI, 0UI}

        indirectArgumentBuffer =
        renderDevice.CreateBuffer(
            argumente,
            BindFlags.None,
            ResourceUsage.Default,
            CpuAccessFlags.None,
            ResourceOptionFlags.DrawIndirectArguments,
            16,
            0)

        If indirectArgumentBuffer Is Nothing Then

            Throw New InvalidOperationException(
            "Der APC-IndirectArgumentBuffer konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisiereIndirectArgumentStagingBuffer()

        Dim description As BufferDescription

        description = New BufferDescription()

        '
        ' DrawInstancedIndirect verwendet vier UInt32:
        '
        ' Offset  0: VertexCountPerInstance
        ' Offset  4: InstanceCount
        ' Offset  8: StartVertexLocation
        ' Offset 12: StartInstanceLocation
        '

        description.ByteWidth = 16UI
        description.Usage = ResourceUsage.Staging
        description.BindFlags = BindFlags.None
        description.CPUAccessFlags = CpuAccessFlags.Read
        description.MiscFlags = ResourceOptionFlags.None
        description.StructureByteStride = 0UI

        indirectArgumentStagingBuffer =
        renderDevice.CreateBuffer(
            description)

        If indirectArgumentStagingBuffer Is Nothing Then

            Throw New InvalidOperationException(
            "Der APC-Diagnose-Stagingbuffer konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisierePartikelSimulationParameterBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer

        bufferGroesse = Marshal.SizeOf(GetType(ParticleSimulationParameter))

        If bufferGroesse <> 48 Then

            Throw New InvalidOperationException("Die Partikel-Simulationsparameter besitzen eine " &
                                                "unerwartete Größe. Erwartet: 48 Byte, tatsächlich: " &
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

        LogHandling.LogDebug("D3D: Partikel-Simulationsparameterbuffer erzeugt. Größe: " &
                             bufferGroesse.ToString() & " Byte.")
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

    Private Function ErzeugePartikelInitialdaten() As Particle()

        Dim particles(CInt(ANZAHL_PARTIKEL) - 1) As Particle

        Return particles

    End Function

    Private Sub InitialisierePartikelBlendState()

        Dim blendDescription As BlendDescription

        blendDescription = BlendDescription.NonPremultiplied

        particleBlendState = renderDevice.CreateBlendState(blendDescription)

    End Sub

#End Region

#Region "Rendering"

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

    Friend Sub Simuliere(deltaTime As Single, progress As Single, maskView As ID3D11ShaderResourceView)

        Const THREADS_PRO_GRUPPE As UInteger = 64UI

        Dim parameter As ParticleSimulationParameter
        Dim anzahlThreadGruppen As UInteger

        If deltaTime <= 0.0F Then
            Exit Sub
        End If

        If maskView Is Nothing Then
            Throw New ArgumentNullException(NameOf(maskView))
        End If

        partikelSimulationsZeit += deltaTime

        parameter.deltaTime = deltaTime
        parameter.progress = progress
        parameter.emitterBreite = 0.02F
        parameter.spawnRate = 2400.0F
        parameter.schwerkraftAktiv = If(schwerkraftIstAktiv, 1.0F, 0.0F)
        parameter.zeit = partikelSimulationsZeit
        parameter.flowFieldStaerke = 0.035F
        parameter.partikelMaxLebensdauer = partikelMaxLebensdauer

        parameter.padding1 = 0.0F
        parameter.padding2 = 0.0F
        parameter.padding3 = 0.0F
        parameter.padding4 = 0.0F

        renderContext.UpdateSubresource(parameter, particleSimulationParameterBuffer)

        anzahlThreadGruppen = (ANZAHL_PARTIKEL + THREADS_PRO_GRUPPE - 1UI) \ THREADS_PRO_GRUPPE

        renderContext.CSSetShader(particleComputeShader)
        renderContext.CSSetConstantBuffer(0UI, particleSimulationParameterBuffer)
        renderContext.CSSetUnorderedAccessView(0UI, particleUnorderedAccessView)

        ' APC:
        ' Die Renderliste wird für jeden Frame neu aufgebaut.
        ' Der versteckte Append-Counter beginnt deshalb bei 0.

        renderContext.CSSetUnorderedAccessView(1UI, renderPartikelIndexUnorderedAccessView, 0UI)

        renderContext.CSSetShaderResource(0UI, maskView)

        renderContext.Dispatch(anzahlThreadGruppen, 1UI, 1UI)

        '---------------------------------
        ' APC-Renderliste abschließen
        '---------------------------------

        renderContext.CSSetUnorderedAccessView(1UI, Nothing)

        ' InstanceCount des indirekten DrawCalls übernimmt
        ' direkt den GPU-seitigen Append-Counter.

        renderContext.CopyStructureCount(indirectArgumentBuffer, 4UI, renderPartikelIndexUnorderedAccessView)

        ' Restliche Ressourcen lösen
        renderContext.CSSetShaderResource(0UI, Nothing)
        renderContext.CSSetUnorderedAccessView(0UI, Nothing)
        renderContext.CSSetConstantBuffer(0UI, Nothing)
        renderContext.CSSetShader(Nothing)

    End Sub

    Friend Function GibAPCAnzahlZurueck() As Integer

        Dim mappedSubresource As MappedSubresource
        Dim result As SharpGen.Runtime.Result

        Dim wurdeGemappt As Boolean
        Dim anzahlRenderPartikel As Integer

        wurdeGemappt = False
        anzahlRenderPartikel = -1

        If indirectArgumentBuffer Is Nothing OrElse
       indirectArgumentStagingBuffer Is Nothing Then

            Return -1

        End If

        '
        ' Den vollständigen 16-Byte-IndirectArgumentBuffer
        ' GPU -> CPU kopieren.
        '

        renderContext.CopyResource(
        indirectArgumentStagingBuffer,
        indirectArgumentBuffer)

        Try

            result =
            renderContext.Map(
                indirectArgumentStagingBuffer,
                0UI,
                MapMode.Read,
                Vortice.Direct3D11.MapFlags.None,
                mappedSubresource)

            If result.Failure Then

                Throw New InvalidOperationException(
                "Der APC-IndirectArgumentBuffer konnte nicht gelesen werden. HRESULT: " &
                result.Code.ToString())

            End If

            wurdeGemappt = True

            '
            ' InstanceCount befindet sich bei Byteoffset 4.
            '

            anzahlRenderPartikel =
            Marshal.ReadInt32(
                mappedSubresource.DataPointer,
                4)

        Finally

            If wurdeGemappt Then

                renderContext.Unmap(
                indirectArgumentStagingBuffer,
                0UI)

            End If

        End Try

        Return anzahlRenderPartikel

    End Function

    Friend Sub Render(sampler As ID3D11SamplerState)

        If sampler Is Nothing Then
            Throw New ArgumentNullException(NameOf(sampler))
        End If

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.VSSetShader(particleRenderVertexShader)
        renderContext.PSSetShader(particleRenderPixelShader)
        renderContext.OMSetBlendState(particleBlendState)
        renderContext.VSSetShaderResource(0UI, particleShaderResourceView)
        renderContext.VSSetShaderResource(1UI, renderPartikelIndexView)
        renderContext.VSSetConstantBuffer(0UI, particleRenderParameterBuffer)
        renderContext.PSSetShaderResource(0UI, particleGradientView)
        renderContext.PSSetSampler(0UI, sampler)

        renderContext.DrawInstancedIndirect(indirectArgumentBuffer, 0UI)

        renderContext.VSSetShaderResource(1UI, Nothing)
        renderContext.VSSetShaderResource(0UI, Nothing)
        renderContext.VSSetConstantBuffer(0UI, Nothing)
        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetSampler(0UI, Nothing)
        renderContext.OMSetBlendState(Nothing)

    End Sub

#End Region

#Region "Bereinigen & Dispose"

    Private Sub BeendeUndBereinigePartikelRenderer()

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        Direct3DRessourceHandler.GebeFrei(particleRenderParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(particleRenderPixelShader)
        Direct3DRessourceHandler.GebeFrei(particleRenderVertexShader)
        Direct3DRessourceHandler.GebeFrei(particleBlendState)
        Direct3DRessourceHandler.GebeFrei(particleSimulationParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(particleComputeShader)
        Direct3DRessourceHandler.GebeFrei(particleShaderResourceView)
        Direct3DRessourceHandler.GebeFrei(particleUnorderedAccessView)
        Direct3DRessourceHandler.GebeFrei(particleBuffer)
        Direct3DRessourceHandler.GebeFrei(particleGradientView)
        Direct3DRessourceHandler.GebeFrei(particleGradientTexture)

        '---------------------------------
        ' APC
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(indirectArgumentBuffer)
        Direct3DRessourceHandler.GebeFrei(renderPartikelIndexView)
        Direct3DRessourceHandler.GebeFrei(renderPartikelIndexUnorderedAccessView)
        Direct3DRessourceHandler.GebeFrei(renderPartikelIndexBuffer)

        Direct3DRessourceHandler.GebeFrei(indirectArgumentStagingBuffer)
        indirectArgumentStagingBuffer = Nothing

    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose

        If wurdeBereinigt Then
            Exit Sub
        End If

        BeendeUndBereinigePartikelRenderer()

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class
