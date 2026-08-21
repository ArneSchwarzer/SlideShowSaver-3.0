Imports System.Runtime.InteropServices
Imports System.Windows.Media.Imaging
Imports Vortice.Direct3D
Imports Vortice.Direct3D11

Friend Class MosaikRenderer
    Implements IDisposable

#Region "Variablendeklaration"

    Private Const PARTIKEL_STRIDE As Integer = 80

    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext

    Private partikelBuffer As ID3D11Buffer
    Private partikelView As ID3D11ShaderResourceView

    Private bildTexture As ID3D11Texture2D
    Private bildView As ID3D11ShaderResourceView

    Private vertexShader As ID3D11VertexShader
    Private pixelShader As ID3D11PixelShader

    Private renderParameterBuffer As ID3D11Buffer

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

#End Region

#End Region

#Region "Initialisierung"

    Friend Sub Initialisiere(device As ID3D11Device, context As ID3D11DeviceContext, breite As Integer,
                             hoehe As Integer, partikel() As PartikelDaten, bild As BitmapSource)

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

        If bild Is Nothing Then
            Throw New ArgumentNullException(NameOf(bild))
        End If

        If breite <= 0 OrElse hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(breite))
        End If

        renderDevice = device
        renderContext = context

        renderBreite = breite
        renderHoehe = hoehe

        partikelAnzahl = partikel.Length

        InitialisierePartikelBuffer(partikel)
        InitialisiereBildTextur(bild)
        InitialisiereShader()
        InitialisiereRenderParameterBuffer()

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
                BindFlags.ShaderResource,
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

    End Sub

    Private Sub InitialisiereBildTextur(bild As BitmapSource)

        bildTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, bild)

        bildView = renderDevice.CreateShaderResourceView(bildTexture)

        If bildView Is Nothing Then

            Throw New InvalidOperationException("Die ShaderResourceView des alten Bildes konnte nicht erzeugt " &
                                                "werden.")

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

        Direct3DRessourceHandler.GebeFrei(renderParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(pixelShader)
        Direct3DRessourceHandler.GebeFrei(vertexShader)
        Direct3DRessourceHandler.GebeFrei(bildView)
        Direct3DRessourceHandler.GebeFrei(bildTexture)
        Direct3DRessourceHandler.GebeFrei(partikelView)
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