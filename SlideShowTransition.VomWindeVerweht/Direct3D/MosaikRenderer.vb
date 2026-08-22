Imports System.Runtime.InteropServices
Imports System.Windows.Media.Imaging
Imports Vortice.Direct3D
Imports Vortice.Direct3D11
Imports Vortice.DXGI

Friend Class MosaikRenderer
    Implements IDisposable

#Region "Variablendeklaration"

    Private Const PARTIKEL_STRIDE As Integer = 80

    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext

    '---------------------------------
    ' Partikeldaten
    '---------------------------------

    Private partikelBuffer As ID3D11Buffer
    Private partikelView As ID3D11ShaderResourceView
    Private partikelUnorderedAccessView As ID3D11UnorderedAccessView

    '---------------------------------
    ' GPU-Lebendzähler
    '---------------------------------

    Private lebendZaehlerBuffer As ID3D11Buffer
    Private lebendZaehlerView As ID3D11UnorderedAccessView

    Private lebendZaehlerStagingBuffer As ID3D11Buffer

    '---------------------------------
    ' Bildtextur
    '---------------------------------

    Private bildTexture As ID3D11Texture2D
    Private bildView As ID3D11ShaderResourceView

    Private zielBildTexture As ID3D11Texture2D
    Private zielBildView As ID3D11ShaderResourceView

    '---------------------------------
    ' Hintergrundbild
    '---------------------------------

    Private hintergrundVertexShader As ID3D11VertexShader
    Private hintergrundPixelShader As ID3D11PixelShader

    '---------------------------------
    ' Render-Shader
    '---------------------------------

    Private vertexShader As ID3D11VertexShader
    Private pixelShader As ID3D11PixelShader
    Private renderParameterBuffer As ID3D11Buffer

    '---------------------------------
    ' Partikelbewegung
    '---------------------------------

    Private partikelBewegungsShader As ID3D11ComputeShader
    Private partikelBewegungsParameterBuffer As ID3D11Buffer

    Private flowFieldTexture As ID3D11Texture2D
    Private flowFieldView As ID3D11ShaderResourceView
    Private flowFieldSampler As ID3D11SamplerState

    '---------------------------------
    ' Dimensionen / Status
    '---------------------------------

    Private partikelAnzahl As Integer
    Private renderBreite As Integer
    Private renderHoehe As Integer

    Private wurdeBereinigt As Boolean

#Region "Structures"

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RenderParameter

        Public renderBreite As Single
        Public renderHoehe As Single

        Public padding1 As Single
        Public padding2 As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PartikelBewegungsParameter

        Public deltaTime As Single
        Public renderBreite As Single
        Public renderHoehe As Single

        Public partikelAnzahl As UInteger

        Public padding1 As Single
        Public padding2 As Single
        Public padding3 As Single
        Public padding4 As Single

    End Structure

#End Region

#End Region

#Region "Initialisierung"

    Friend Sub Initialisiere(device As ID3D11Device, context As ID3D11DeviceContext, breite As Integer,
                             hoehe As Integer, partikel() As PartikelDaten, altesBild As BitmapSource,
                             neuesBild As BitmapSource, flowField As FlowFieldDaten)

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(MosaikRenderer))
        End If

        If device Is Nothing Then
            Throw New ArgumentNullException(NameOf(device))
        End If

        If context Is Nothing Then
            Throw New ArgumentNullException(NameOf(context))
        End If

        If partikel Is Nothing OrElse partikel.Length = 0 Then
            Throw New ArgumentException("Es wurden keine Partikeldaten übergeben.", NameOf(partikel))
        End If

        If altesBild Is Nothing Then
            Throw New ArgumentNullException(NameOf(altesBild))
        End If

        If neuesBild Is Nothing Then
            Throw New ArgumentNullException(NameOf(neuesBild))
        End If

        If breite <= 0 OrElse hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(breite))
        End If

        If flowField Is Nothing Then
            Throw New ArgumentNullException(NameOf(flowField))
        End If

        renderDevice = device
        renderContext = context

        renderBreite = breite
        renderHoehe = hoehe

        partikelAnzahl = partikel.Length

        InitialisierePartikelBuffer(partikel)
        InitialisiereLebendZaehler()
        InitialisiereFlowField(flowField)

        InitialisiereBildTextur(altesBild)
        InitialisiereZielBildTextur(neuesBild)

        InitialisiereShader()
        InitialisiereHintergrundShader()
        InitialisierePartikelBewegungsShader()

        InitialisiereRenderParameterBuffer()
        InitialisierePartikelBewegungsParameterBuffer()

        AktualisiereRenderParameter()

    End Sub

    Private Sub InitialisierePartikelBuffer(partikel() As PartikelDaten)

        Dim stride As Integer
        Dim bufferGroesse As Integer

        stride =
            Marshal.SizeOf(GetType(PartikelDaten))

        If stride <> PARTIKEL_STRIDE Then

            Throw New InvalidOperationException("PartikelDaten besitzt eine unerwartete Größe. Erwartet: " &
                                                PARTIKEL_STRIDE.ToString() & " Byte, tatsächlich: " &
                                                stride.ToString() & " Byte.")

        End If

        bufferGroesse = partikel.Length * stride

        partikelBuffer =
             renderDevice.CreateBuffer(Of PartikelDaten)(
                partikel,
                BindFlags.ShaderResource Or BindFlags.UnorderedAccess,
                ResourceUsage.Default,
                CpuAccessFlags.None,
                ResourceOptionFlags.BufferStructured,
                bufferGroesse,
                stride)

        If partikelBuffer Is Nothing Then

            Throw New InvalidOperationException("Der Partikel-StructuredBuffer konnte nicht erzeugt werden.")

        End If

        partikelView = renderDevice.CreateShaderResourceView(partikelBuffer)

        If partikelView Is Nothing Then

            Throw New InvalidOperationException("Die ShaderResourceView des Partikelbuffers konnte nicht " &
                                                "erzeugt werden.")

        End If

        partikelUnorderedAccessView = renderDevice.CreateUnorderedAccessView(partikelBuffer)

        If partikelUnorderedAccessView Is Nothing Then

            Throw New InvalidOperationException(
                "Die UnorderedAccessView des Partikelbuffers konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisiereFlowField(flowField As FlowFieldDaten)

        Dim textureDescription As Texture2DDescription
        Dim initialData() As SubresourceData
        Dim datenHandle As GCHandle

        If flowField.breite <= 0 OrElse flowField.hoehe <= 0 Then

            Throw New InvalidOperationException("Das FlowField besitzt ungültige Dimensionen.")

        End If

        If flowField.vektoren Is Nothing Then

            Throw New InvalidOperationException("Das FlowField enthält keine Vektordaten.")

        End If

        If flowField.vektoren.Length <>
       flowField.breite * flowField.hoehe Then

            Throw New InvalidOperationException(
            "Die Anzahl der FlowField-Vektoren entspricht nicht den FlowField-Dimensionen.")

        End If

        Try

            datenHandle = GCHandle.Alloc(flowField.vektoren, GCHandleType.Pinned)

            textureDescription =
            New Texture2DDescription(
                Format.R32G32_Float,
                CUInt(flowField.breite),
                CUInt(flowField.hoehe),
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
                    datenHandle.AddrOfPinnedObject(),
                    CUInt(flowField.breite * 8),
                    CUInt(flowField.breite *
                          flowField.hoehe * 8))
            }

            flowFieldTexture = renderDevice.CreateTexture2D(textureDescription, initialData)

        Finally

            If datenHandle.IsAllocated Then
                datenHandle.Free()
            End If

        End Try

        If flowFieldTexture Is Nothing Then

            Throw New InvalidOperationException(
            "Die FlowField-Textur konnte nicht erzeugt werden.")

        End If

        flowFieldView = renderDevice.CreateShaderResourceView(flowFieldTexture)

        If flowFieldView Is Nothing Then

            Throw New InvalidOperationException("Die ShaderResourceView des FlowFields konnte nicht erzeugt werden.")

        End If

        InitialisiereFlowFieldSampler()

    End Sub

    Private Sub InitialisiereFlowFieldSampler()

        Dim description As SamplerDescription

        description = New SamplerDescription()

        description.Filter = Filter.MinMagMipLinear
        description.AddressU = TextureAddressMode.Clamp
        description.AddressV = TextureAddressMode.Clamp
        description.AddressW = TextureAddressMode.Clamp
        description.MinLOD = 0.0F
        description.MaxLOD = 0.0F

        flowFieldSampler = renderDevice.CreateSamplerState(description)

        If flowFieldSampler Is Nothing Then

            Throw New InvalidOperationException("Der FlowField-Sampler konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisiereBildTextur(bild As BitmapSource)

        bildTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, bild)

        bildView = renderDevice.CreateShaderResourceView(bildTexture)

        If bildView Is Nothing Then

            Throw New InvalidOperationException("Die ShaderResourceView des alten Bildes konnte nicht erzeugt " &
                                                "werden.")

        End If

    End Sub

    Private Sub InitialisiereZielBildTextur(bild As BitmapSource)

        zielBildTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, bild)

        zielBildView = renderDevice.CreateShaderResourceView(zielBildTexture)

        If zielBildView Is Nothing Then

            Throw New InvalidOperationException(
            "Die ShaderResourceView des Zielbildes konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisiereLebendZaehler()

        Dim startWert() As Integer
        Dim stagingDescription As BufferDescription

        startWert = New Integer() {partikelAnzahl}

        ' GPU-Buffer:
        ' Ein einzelner Integer, den der Compute Shader
        ' atomar vermindern kann.
        '
        lebendZaehlerBuffer =
        renderDevice.CreateBuffer(Of Integer)(
            startWert,
            BindFlags.UnorderedAccess,
            ResourceUsage.Default,
            CpuAccessFlags.None,
            ResourceOptionFlags.BufferStructured,
            4,
            4)

        If lebendZaehlerBuffer Is Nothing Then

            Throw New InvalidOperationException("Der GPU-Lebendzähler konnte nicht erzeugt werden.")

        End If

        lebendZaehlerView = renderDevice.CreateUnorderedAccessView(lebendZaehlerBuffer)

        If lebendZaehlerView Is Nothing Then

            Throw New InvalidOperationException(
            "Die UnorderedAccessView des GPU-Lebendzählers konnte nicht erzeugt werden.")

        End If

        ' Kleiner CPU-lesbarer Stagingbuffer.
        ' Nur vier Byte werden bei Bedarf zurückgelesen.

        stagingDescription = New BufferDescription()

        stagingDescription.ByteWidth = 4UI
        stagingDescription.Usage = ResourceUsage.Staging
        stagingDescription.BindFlags = BindFlags.None
        stagingDescription.CPUAccessFlags = CpuAccessFlags.Read
        stagingDescription.MiscFlags = ResourceOptionFlags.BufferStructured
        stagingDescription.StructureByteStride = 4UI

        lebendZaehlerStagingBuffer = renderDevice.CreateBuffer(stagingDescription)

        If lebendZaehlerStagingBuffer Is Nothing Then

            Throw New InvalidOperationException(
            "Der Stagingbuffer des GPU-Lebendzählers konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisiereShader()

        Dim vertexShaderCode() As Byte
        Dim pixelShaderCode() As Byte

        vertexShaderCode = D3DRenderer.LadeShaderBytecode("MosaikShaderVS.cso")
        pixelShaderCode = D3DRenderer.LadeShaderBytecode("MosaikShaderPS.cso")

        vertexShader = renderDevice.CreateVertexShader(vertexShaderCode)

        pixelShader = renderDevice.CreatePixelShader(pixelShaderCode)

        If vertexShader Is Nothing Then

            Throw New InvalidOperationException("Der Mosaik-Vertexshader konnte nicht erzeugt werden.")

        End If

        If pixelShader Is Nothing Then

            Throw New InvalidOperationException("Der Mosaik-Pixelshader konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisiereHintergrundShader()

        Dim vertexShaderCode() As Byte
        Dim pixelShaderCode() As Byte

        vertexShaderCode = D3DRenderer.LadeShaderBytecode("HintergrundShaderVS.cso")
        pixelShaderCode = D3DRenderer.LadeShaderBytecode("HintergrundShaderPS.cso")

        hintergrundVertexShader = renderDevice.CreateVertexShader(vertexShaderCode)
        hintergrundPixelShader = renderDevice.CreatePixelShader(pixelShaderCode)

        If hintergrundVertexShader Is Nothing Then

            Throw New InvalidOperationException("Der Hintergrund-Vertexshader konnte nicht erzeugt werden.")

        End If

        If hintergrundPixelShader Is Nothing Then

            Throw New InvalidOperationException("Der Hintergrund-Pixelshader konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisierePartikelBewegungsShader()

        Dim shaderBytes() As Byte

        shaderBytes = D3DRenderer.LadeShaderBytecode("PartikelBewegungsShaderCS.cso")

        partikelBewegungsShader = renderDevice.CreateComputeShader(shaderBytes)

        If partikelBewegungsShader Is Nothing Then

            Throw New InvalidOperationException(
            "Der Partikel-Bewegungs-ComputeShader konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisiereRenderParameterBuffer()

        Dim description As BufferDescription

        description = New BufferDescription()

        description.ByteWidth = 16UI
        description.Usage = ResourceUsage.Default
        description.BindFlags = BindFlags.ConstantBuffer
        description.CPUAccessFlags = CpuAccessFlags.None
        description.MiscFlags = ResourceOptionFlags.None
        description.StructureByteStride = 0UI

        renderParameterBuffer = renderDevice.CreateBuffer(description)

        If renderParameterBuffer Is Nothing Then

            Throw New InvalidOperationException("Der Mosaik-Renderparameterbuffer konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub InitialisierePartikelBewegungsParameterBuffer()

        Dim description As BufferDescription
        Dim bufferGroesse As Integer

        bufferGroesse = Marshal.SizeOf(GetType(PartikelBewegungsParameter))

        If bufferGroesse <> 32 Then

            Throw New InvalidOperationException(
            "Die Partikel-Bewegungsparameter besitzen eine unerwartete Größe. Erwartet: 32 Byte, tatsächlich: " &
            bufferGroesse.ToString() &
            " Byte.")

        End If

        description = New BufferDescription()
        description.ByteWidth = CUInt(bufferGroesse)
        description.Usage = ResourceUsage.Default
        description.BindFlags = BindFlags.ConstantBuffer
        description.CPUAccessFlags = CpuAccessFlags.None
        description.MiscFlags = ResourceOptionFlags.None
        description.StructureByteStride = 0UI

        partikelBewegungsParameterBuffer = renderDevice.CreateBuffer(description)

        If partikelBewegungsParameterBuffer Is Nothing Then

            Throw New InvalidOperationException("Der Partikel-Bewegungsparameterbuffer konnte nicht erzeugt " &
                                                "werden.")

        End If

    End Sub

#End Region

#Region "Simulation"

    Friend Sub Simuliere(deltaTime As Single)

        Const THREADS_PRO_GRUPPE As UInteger = 64UI

        Dim parameter As PartikelBewegungsParameter
        Dim anzahlThreadGruppen As UInteger

        If deltaTime <= 0.0F Then
            Exit Sub
        End If

        If partikelAnzahl <= 0 Then
            Exit Sub
        End If

        parameter.deltaTime = deltaTime
        parameter.renderBreite = CSng(renderBreite)
        parameter.renderHoehe = CSng(renderHoehe)
        parameter.partikelAnzahl = CUInt(partikelAnzahl)

        parameter.padding1 = 0.0F
        parameter.padding2 = 0.0F
        parameter.padding3 = 0.0F
        parameter.padding4 = 0.0F

        renderContext.UpdateSubresource(parameter, partikelBewegungsParameterBuffer)

        anzahlThreadGruppen = (CUInt(partikelAnzahl) + THREADS_PRO_GRUPPE - 1UI) \ THREADS_PRO_GRUPPE

        renderContext.CSSetShader(partikelBewegungsShader)
        renderContext.CSSetConstantBuffer(0UI, partikelBewegungsParameterBuffer)
        renderContext.CSSetUnorderedAccessView(0UI, partikelUnorderedAccessView)
        renderContext.CSSetUnorderedAccessView(1UI, lebendZaehlerView)
        renderContext.CSSetShaderResource(0UI, flowFieldView)

        renderContext.CSSetSampler(0UI, flowFieldSampler)

        renderContext.Dispatch(anzahlThreadGruppen, 1UI, 1UI)

        'Alle Compute-Ressourcen wieder lösen,
        'bevor der Partikelbuffer anschließend
        'vom Vertexshader als SRV gelesen wird.

        renderContext.CSSetShaderResource(0UI, Nothing)
        renderContext.CSSetSampler(0UI, Nothing)
        renderContext.CSSetUnorderedAccessView(0UI, Nothing)
        renderContext.CSSetUnorderedAccessView(1UI, Nothing)
        renderContext.CSSetConstantBuffer(0UI, Nothing)
        renderContext.CSSetShader(Nothing)

    End Sub

    Friend Function GibAnzahlLebendePartikelZurueck() As Integer

        Dim mappedSubresource As MappedSubresource
        Dim result As SharpGen.Runtime.Result

        Dim wurdeGemappt As Boolean
        Dim anzahlLebendePartikel As Integer

        wurdeGemappt = False
        anzahlLebendePartikel = 0


        ' Nur vier Byte werden von der GPU in einen
        ' CPU-lesbaren Stagingbuffer kopiert.

        renderContext.CopyResource(lebendZaehlerStagingBuffer, lebendZaehlerBuffer)

        Try

            result = renderContext.Map(lebendZaehlerStagingBuffer, 0UI, MapMode.Read, MapFlags.None,
                                       mappedSubresource)

            If result.Failure Then

                Throw New InvalidOperationException(
                "Der GPU-Lebendzähler konnte nicht gelesen werden. HRESULT: " &
                result.Code.ToString())

            End If

            wurdeGemappt = True

            anzahlLebendePartikel = Marshal.ReadInt32(mappedSubresource.DataPointer)

        Finally

            If wurdeGemappt Then

                renderContext.Unmap(lebendZaehlerStagingBuffer, 0UI)

            End If

        End Try

        Return anzahlLebendePartikel

    End Function

    Private Sub AktualisiereRenderParameter()

        Dim parameter As RenderParameter

        parameter.renderBreite = CSng(renderBreite)
        parameter.renderHoehe = CSng(renderHoehe)
        parameter.padding1 = 0.0F
        parameter.padding2 = 0.0F

        renderContext.UpdateSubresource(parameter, renderParameterBuffer)

    End Sub

#End Region

#Region "Rendering"

    Friend Sub Render(sampler As ID3D11SamplerState)

        Dim vertexAnzahl As UInteger

        If sampler Is Nothing Then
            Throw New ArgumentNullException(NameOf(sampler))
        End If

        '---------------------------------
        ' Zielbild
        '---------------------------------

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.VSSetShader(hintergrundVertexShader)
        renderContext.PSSetShader(hintergrundPixelShader)
        renderContext.PSSetShaderResource(0UI, zielBildView)
        renderContext.PSSetSampler(0UI, sampler)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetSampler(0UI, Nothing)
        renderContext.VSSetShader(Nothing)
        renderContext.PSSetShader(Nothing)

        '---------------------------------
        ' Alte Bildpartikel darüber
        '---------------------------------

        vertexAnzahl = CUInt(partikelAnzahl * 6)

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.VSSetShader(vertexShader)
        renderContext.VSSetShaderResource(0UI, partikelView)
        renderContext.VSSetConstantBuffer(0UI, renderParameterBuffer)
        renderContext.PSSetShader(pixelShader)
        renderContext.PSSetShaderResource(0UI, bildView)
        renderContext.PSSetSampler(0UI, sampler)

        renderContext.Draw(vertexAnzahl, 0UI)

        renderContext.VSSetShaderResource(0UI, Nothing)
        renderContext.VSSetConstantBuffer(0UI, Nothing)
        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetSampler(0UI, Nothing)
        renderContext.VSSetShader(Nothing)
        renderContext.PSSetShader(Nothing)

    End Sub

#End Region

#Region "Bereinigung"

    Private Sub BeendeUndBereinigeMosaikRenderer()

        '---------------------------------
        ' Compute Shader
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(partikelBewegungsParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(partikelBewegungsShader)

        '---------------------------------
        ' GPU-Lebendzähler
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(lebendZaehlerStagingBuffer)
        Direct3DRessourceHandler.GebeFrei(lebendZaehlerView)
        Direct3DRessourceHandler.GebeFrei(lebendZaehlerBuffer)

        '---------------------------------
        ' FlowField
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(flowFieldSampler)
        Direct3DRessourceHandler.GebeFrei(flowFieldView)
        Direct3DRessourceHandler.GebeFrei(flowFieldTexture)

        '---------------------------------
        ' Render Shader
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(renderParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(pixelShader)
        Direct3DRessourceHandler.GebeFrei(vertexShader)

        '---------------------------------
        ' Bild
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(bildView)
        Direct3DRessourceHandler.GebeFrei(bildTexture)

        '---------------------------------
        ' Partikel
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(partikelView)
        Direct3DRessourceHandler.GebeFrei(partikelUnorderedAccessView)
        Direct3DRessourceHandler.GebeFrei(partikelBuffer)

        renderDevice = Nothing
        renderContext = Nothing

        partikelAnzahl = 0
        renderBreite = 0
        renderHoehe = 0

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        BeendeUndBereinigeMosaikRenderer()

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class