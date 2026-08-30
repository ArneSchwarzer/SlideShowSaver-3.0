Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Imports SharpGen.Runtime

Imports SlideShowDirect3DInterop
Imports SlideShowLogging
Imports SlideShowLogging.LogHandling

Imports Vortice.Direct3D
Imports Vortice.Direct3D11
Imports Vortice.DXGI
Imports Vortice.Mathematics

Friend Class D3DRenderer
    Implements IDisposable

#Region "Variablendeklaration"

#Region "Konstanten"

    Const TEST_VISKOSITAET As Single = 1.0F
    Const TEST_ITERATIONEN As Integer = 16

    Private Const TEST_PIGMENT_TRANSPORT_STRENGTH As Single = 2.0F

#End Region

#Region "Variablen"

    '---------------------------------
    ' D3D11-Grundsystem
    '---------------------------------
    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext
    Private renderFeatureLevel As FeatureLevel

    '---------------------------------
    ' Eingabebild
    '---------------------------------
    Private sourceTexture As ID3D11Texture2D
    Private sourceView As ID3D11ShaderResourceView


    '---------------------------------
    ' Kuwahara
    '---------------------------------
    Private kuwaharaTexture As ID3D11Texture2D
    Private kuwaharaView As ID3D11ShaderResourceView
    Private kuwaharaTargetView As ID3D11RenderTargetView


    '---------------------------------
    ' Region Distance
    '---------------------------------
    Private regionDistanceTexture As ID3D11Texture2D
    Private regionDistanceView As ID3D11ShaderResourceView
    Private regionDistanceTargetView As ID3D11RenderTargetView

    Private sourceRegionSeedTexture As ID3D11Texture2D
    Private sourceRegionSeedView As ID3D11ShaderResourceView
    Private sourceRegionSeedTargetView As ID3D11RenderTargetView

    Private targetRegionSeedTexture As ID3D11Texture2D
    Private targetRegionSeedView As ID3D11ShaderResourceView
    Private targetRegionSeedTargetView As ID3D11RenderTargetView


    '---------------------------------
    ' Papierzustand
    '---------------------------------
    Private paperTexture As ID3D11Texture2D
    Private paperView As ID3D11ShaderResourceView
    Private paperTargetView As ID3D11RenderTargetView


    '---------------------------------
    ' Wasserzustand - Pressure
    '---------------------------------
    Private sourcePressureTexture As ID3D11Texture2D
    Private sourcePressureView As ID3D11ShaderResourceView
    Private sourcePressureTargetView As ID3D11RenderTargetView

    Private targetPressureTexture As ID3D11Texture2D
    Private targetPressureView As ID3D11ShaderResourceView
    Private targetPressureTargetView As ID3D11RenderTargetView


    '---------------------------------
    ' Wasserzustand - Velocity
    '---------------------------------
    Private sourceVelocityTexture As ID3D11Texture2D
    Private sourceVelocityView As ID3D11ShaderResourceView
    Private sourceVelocityTargetView As ID3D11RenderTargetView

    Private targetVelocityTexture As ID3D11Texture2D
    Private targetVelocityView As ID3D11ShaderResourceView
    Private targetVelocityTargetView As ID3D11RenderTargetView


    '---------------------------------
    ' Pigmentzustand - Suspension
    '---------------------------------
    Private sourcePigmentSuspensionTexture As ID3D11Texture2D
    Private sourcePigmentSuspensionView As ID3D11ShaderResourceView
    Private sourcePigmentSuspensionTargetView As ID3D11RenderTargetView

    Private targetPigmentSuspensionTexture As ID3D11Texture2D
    Private targetPigmentSuspensionView As ID3D11ShaderResourceView
    Private targetPigmentSuspensionTargetView As ID3D11RenderTargetView


    '---------------------------------
    ' Pigmentzustand - Deposit
    '---------------------------------
    Private sourcePigmentDepositTexture As ID3D11Texture2D
    Private sourcePigmentDepositView As ID3D11ShaderResourceView
    Private sourcePigmentDepositTargetView As ID3D11RenderTargetView

    Private targetPigmentDepositTexture As ID3D11Texture2D
    Private targetPigmentDepositView As ID3D11ShaderResourceView
    Private targetPigmentDepositTargetView As ID3D11RenderTargetView

    '---------------------------------
    ' LEGACY - nach Umbau von RenderBild entfernen
    '---------------------------------
    Private sourceWaterTexture As ID3D11Texture2D
    Private sourceWaterView As ID3D11ShaderResourceView
    Private sourceWaterTargetView As ID3D11RenderTargetView

    Private targetWaterTexture As ID3D11Texture2D
    Private targetWaterView As ID3D11ShaderResourceView
    Private targetWaterTargetView As ID3D11RenderTargetView

    '---------------------------------
    ' Ausgabe
    '---------------------------------
    Private renderTargetTexture As ID3D11Texture2D
    Private renderTargetView As ID3D11RenderTargetView

    Private stagingTexture As ID3D11Texture2D

    '---------------------------------
    'Shader allgemein
    '---------------------------------
    Private renderSampler As ID3D11SamplerState

    '---------------------------------
    ' Dimensionen
    '---------------------------------
    Private renderBreite As Integer
    Private renderHoehe As Integer

    '---------------------------------
    ' Lifecycle
    '---------------------------------
    Private wurdeBereinigt As Boolean
    Private istInitialisiert As Boolean


#End Region

#Region "Stuctures & Enums"

    <StructLayout(LayoutKind.Sequential)>
    Private Structure WaterFlowConstants

        Public viscosity As Single

        Public reserve1 As Single
        Public reserve2 As Single
        Public reserve3 As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PigmentFlowConstants
        Public pigmentTransportStrength As Single

        Public padding1 As Single
        Public padding2 As Single
        Public padding3 As Single
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PigmentDisplayConstants

        Public backgroundRed As Single
        Public backgroundGreen As Single
        Public backgroundBlue As Single
        Public backgroundAlpha As Single

    End Structure

#End Region

#End Region

#Region "Initialisierung"

    Friend Sub Initialisiere(baseImage As Image)

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(D3DRenderer))
        End If

        If baseImage Is Nothing Then
            Throw New ArgumentNullException(NameOf(baseImage))
        End If

        renderBreite = baseImage.Width
        renderHoehe = baseImage.Height

        If renderBreite <= 0 OrElse renderHoehe <= 0 Then

            Throw New ArgumentException("Das Quellbild besitzt ungültige Dimensionen.", NameOf(baseImage))

        End If

        LogHandling.LogDebug("Aquarell D3D: Initialisiere() beginnt. Größe: " & renderBreite.ToString() &
                             " x " & renderHoehe.ToString())

        InitialisiereDirect3D()
        InitialisiereTexturen(baseImage)
        InitialisiereShader()
        InitialisiereSampler()
        InitialisiereWaterFlowConstantBuffer()
        InitialisierePigmentFlowConstantBuffer()
        InitialisierePigmentDisplayConstantBuffer()

        istInitialisiert = True

        LogHandling.LogDebug("Aquarell D3D: Initialisiere() beendet. FeatureLevel: " & renderFeatureLevel.ToString())

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

        result =
            D3D11.D3D11CreateDevice(
                IntPtr.Zero,
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                featureLevels,
                renderDevice,
                featureLevel,
                renderContext)

        If result.Failure Then

            Throw New InvalidOperationException(
                "Das Direct3D11-Device konnte nicht erzeugt werden. HRESULT: " &
                result.Code.ToString())

        End If

        If renderDevice Is Nothing Then
            Throw New InvalidOperationException("D3D11 lieferte kein Device.")
        End If

        If renderContext Is Nothing Then
            Throw New InvalidOperationException("D3D11 lieferte keinen DeviceContext.")
        End If

        renderFeatureLevel = featureLevel

    End Sub

    Private Sub InitialisiereTexturen(baseImage As Image)

        InitialisiereBildressourcen(baseImage)

        InitialisiereRegionDistanceRessourcen()
        InitialisierePapierressourcen()

        InitialisiereWasserressourcen()
        InitialisierePigmentressourcen()

        InitialisiereAusgaberessourcen()

        ' Nur während des Umbaus erforderlich.
        InitialisiereLegacyWaterRessourcen()

    End Sub

    Private Sub InitialisiereBildressourcen(baseImage As Image)

        sourceTexture = Direct3DRessourceHandler.ErstelleTextureAusImage(renderDevice, baseImage)

        If sourceTexture Is Nothing Then
            Throw New InvalidOperationException(
            "Die Source-Texture konnte nicht erzeugt werden.")
        End If

        sourceView = renderDevice.CreateShaderResourceView(sourceTexture)

        If sourceView Is Nothing Then
            Throw New InvalidOperationException(
            "Die Source-SRV konnte nicht erzeugt werden.")
        End If


        InitialisiereSimulationsRessource(kuwaharaTexture, kuwaharaView, kuwaharaTargetView, Format.B8G8R8A8_UNorm)

    End Sub

    Private Sub InitialisiereRegionDistanceRessourcen()

        ' Ergebnis der Distance Transformation.
        InitialisiereSimulationsRessource(regionDistanceTexture, regionDistanceView, regionDistanceTargetView,
                                          Format.R16_Float)

        ' Ping-Pong-Arbeitsfelder des Jump-Flood-Algorithmus.
        '
        ' R32G32_Float ist hier bewusst gewählt:
        ' Wir speichern Pixelkoordinaten und wollen auch bei 4K keine
        ' Half-Float-Quantisierung der Seed-Positionen.

        InitialisiereSimulationsRessource(sourceRegionSeedTexture, sourceRegionSeedView,
                                          sourceRegionSeedTargetView, Format.R32G32_Float)

        InitialisiereSimulationsRessource(targetRegionSeedTexture, targetRegionSeedView,
                                          targetRegionSeedTargetView, Format.R32G32_Float)

    End Sub

    Private Sub InitialisierePapierressourcen()

        InitialisiereSimulationsRessource(paperTexture, paperView, paperTargetView, Format.R16_Float)

    End Sub

    Private Sub InitialisiereWasserressourcen()

        '---------------------------------
        ' Pressure p
        '---------------------------------

        InitialisiereSimulationsRessource(sourcePressureTexture, sourcePressureView, sourcePressureTargetView,
                                          Format.R16_Float)

        InitialisiereSimulationsRessource(targetPressureTexture, targetPressureView, targetPressureTargetView,
                                          Format.R16_Float)

        '---------------------------------
        ' Velocity (u,v)
        '---------------------------------

        InitialisiereSimulationsRessource(sourceVelocityTexture, sourceVelocityView, sourceVelocityTargetView,
                                          Format.R16G16_Float)

        InitialisiereSimulationsRessource(targetVelocityTexture, targetVelocityView, targetVelocityTargetView,
                                          Format.R16G16_Float)

    End Sub

    Private Sub InitialisierePigmentressourcen()

        '---------------------------------
        ' Suspension g
        '---------------------------------

        InitialisiereSimulationsRessource(sourcePigmentSuspensionTexture, sourcePigmentSuspensionView,
                                          sourcePigmentSuspensionTargetView, Format.R16G16B16A16_Float)

        InitialisiereSimulationsRessource(targetPigmentSuspensionTexture, targetPigmentSuspensionView,
                                          targetPigmentSuspensionTargetView, Format.R16G16B16A16_Float)


        '---------------------------------
        ' Deposit d
        '---------------------------------

        InitialisiereSimulationsRessource(sourcePigmentDepositTexture, sourcePigmentDepositView,
                                          sourcePigmentDepositTargetView, Format.R16G16B16A16_Float)

        InitialisiereSimulationsRessource(targetPigmentDepositTexture, targetPigmentDepositView,
                                          targetPigmentDepositTargetView, Format.R16G16B16A16_Float)

    End Sub

    Private Sub InitialisiereAusgaberessourcen()

        renderTargetTexture =
        Direct3DRessourceHandler.ErstelleRenderTargetTexture(
            renderDevice,
            renderBreite,
            renderHoehe)

        If renderTargetTexture Is Nothing Then
            Throw New InvalidOperationException("Die RenderTarget-Texture konnte nicht erzeugt werden.")
        End If

        renderTargetView = renderDevice.CreateRenderTargetView(renderTargetTexture)

        If renderTargetView Is Nothing Then
            Throw New InvalidOperationException("Die RenderTargetView konnte nicht erzeugt werden.")
        End If


        stagingTexture =
        Direct3DRessourceHandler.ErstelleStagingTexture(
            renderDevice,
            renderBreite,
            renderHoehe)

        If stagingTexture Is Nothing Then
            Throw New InvalidOperationException("Die Staging-Texture konnte nicht erzeugt werden.")
        End If

    End Sub

    Private Sub InitialisiereLegacyWaterRessourcen()

        InitialisiereSimulationsRessource(
        sourceWaterTexture,
        sourceWaterView,
        sourceWaterTargetView,
        Format.R16_Float)

        InitialisiereSimulationsRessource(
        targetWaterTexture,
        targetWaterView,
        targetWaterTargetView,
        Format.R16_Float)

    End Sub

    Private Sub InitialisiereShader()

        Dim copyVertexShaderCode() As Byte
        Dim copyPixelShaderCode() As Byte

        Dim kuwaharaVertexShaderCode() As Byte
        Dim kuwaharaPixelShaderCode() As Byte

        Dim pigmentInitializerVertexShaderCode() As Byte
        Dim pigmentInitializerPixelShaderCode() As Byte

        Dim pigmentFlowVertexShaderCode() As Byte
        Dim pigmentFlowPixelShaderCode() As Byte

        Dim waterInitializerVertexShaderCode() As Byte
        Dim waterInitializerPixelShaderCode() As Byte

        Dim waterFlowVertexShaderCode() As Byte
        Dim waterFlowPixelShaderCode() As Byte

        Dim pigmentDisplayVertexShaderCode() As Byte
        Dim pigmentDisplayPixelShaderCode() As Byte

        copyVertexShaderCode = LadeShaderBytecode("AquarellCopyShaderVS.cso")
        copyPixelShaderCode = LadeShaderBytecode("AquarellCopyShaderPS.cso")

        kuwaharaVertexShaderCode = LadeShaderBytecode("KuwaharaShaderVS.cso")
        kuwaharaPixelShaderCode = LadeShaderBytecode("KuwaharaShaderPS.cso")

        pigmentInitializerVertexShaderCode = LadeShaderBytecode("PigmentInitializerShaderVS.cso")
        pigmentInitializerPixelShaderCode = LadeShaderBytecode("PigmentInitializerShaderPS.cso")

        pigmentFlowVertexShaderCode = LadeShaderBytecode("PigmentFlowShaderVS.cso")
        pigmentFlowPixelShaderCode = LadeShaderBytecode("PigmentFlowShaderPS.cso")

        waterInitializerVertexShaderCode = LadeShaderBytecode("WaterInitializerShaderVS.cso")
        waterInitializerPixelShaderCode = LadeShaderBytecode("WaterInitializerShaderPS.cso")

        waterFlowVertexShaderCode = LadeShaderBytecode("WaterFlowShaderVS.cso")
        waterFlowPixelShaderCode = LadeShaderBytecode("WaterFlowShaderPS.cso")

        pigmentDisplayVertexShaderCode = LadeShaderBytecode("PigmentDisplayShaderVS.cso")
        pigmentDisplayPixelShaderCode = LadeShaderBytecode("PigmentDisplayShaderPS.cso")

        copyVertexShader = renderDevice.CreateVertexShader(copyVertexShaderCode)
        copyPixelShader = renderDevice.CreatePixelShader(copyPixelShaderCode)

        kuwaharaVertexShader = renderDevice.CreateVertexShader(kuwaharaVertexShaderCode)
        kuwaharaPixelShader = renderDevice.CreatePixelShader(kuwaharaPixelShaderCode)

        pigmentInitializerVertexShader = renderDevice.CreateVertexShader(pigmentInitializerVertexShaderCode)
        pigmentInitializerPixelShader = renderDevice.CreatePixelShader(pigmentInitializerPixelShaderCode)

        pigmentFlowVertexShader = renderDevice.CreateVertexShader(pigmentFlowVertexShaderCode)
        pigmentFlowPixelShader = renderDevice.CreatePixelShader(pigmentFlowPixelShaderCode)

        waterInitializerVertexShader = renderDevice.CreateVertexShader(waterInitializerVertexShaderCode)
        waterInitializerPixelShader = renderDevice.CreatePixelShader(waterInitializerPixelShaderCode)

        waterFlowVertexShader = renderDevice.CreateVertexShader(waterFlowVertexShaderCode)
        waterFlowPixelShader = renderDevice.CreatePixelShader(waterFlowPixelShaderCode)

        pigmentDisplayVertexShader = renderDevice.CreateVertexShader(pigmentDisplayVertexShaderCode)
        pigmentDisplayPixelShader = renderDevice.CreatePixelShader(pigmentDisplayPixelShaderCode)

        If copyVertexShader Is Nothing Then
            Throw New InvalidOperationException("Der Copy-VertexShader konnte nicht erzeugt werden.")
        End If

        If copyPixelShader Is Nothing Then
            Throw New InvalidOperationException("Der Copy-PixelShader konnte nicht erzeugt werden.")
        End If

        If kuwaharaVertexShader Is Nothing Then
            Throw New InvalidOperationException("Der Kuwahara-VertexShader konnte nicht erzeugt werden.")
        End If

        If kuwaharaPixelShader Is Nothing Then
            Throw New InvalidOperationException("Der Kuwahara-PixelShader konnte nicht erzeugt werden.")
        End If

        If pigmentInitializerVertexShader Is Nothing Then
            Throw New InvalidOperationException("Der PigmentInitializer-VertexShader konnte nicht erzeugt werden.")
        End If

        If pigmentInitializerPixelShader Is Nothing Then
            Throw New InvalidOperationException("Der PigmentInitializer-PixelShader konnte nicht erzeugt werden.")
        End If

        If pigmentFlowVertexShader Is Nothing Then
            Throw New InvalidOperationException("Der PigmentFlow-VertexShader konnte nicht erzeugt werden.")
        End If

        If pigmentFlowPixelShader Is Nothing Then
            Throw New InvalidOperationException("Der PigmentFlow-PixelShader konnte nicht erzeugt werden.")
        End If

        If pigmentDisplayVertexShader Is Nothing Then
            Throw New InvalidOperationException("Der PigmentDisplay-VertexShader konnte nicht erzeugt werden.")
        End If

        If pigmentDisplayPixelShader Is Nothing Then
            Throw New InvalidOperationException("Der PigmentDisplay-PixelShader konnte nicht erzeugt werden.")
        End If

        If waterInitializerVertexShader Is Nothing Then
            Throw New InvalidOperationException("Der WaterInitializer-VertexShader konnte nicht erzeugt werden.")
        End If

        If waterInitializerPixelShader Is Nothing Then
            Throw New InvalidOperationException("Der WaterInitializer-PixelShader konnte nicht erzeugt werden.")
        End If

        If waterFlowVertexShader Is Nothing Then
            Throw New InvalidOperationException("Der WaterFlow-VertexShader konnte nicht erzeugt werden.")
        End If

        If waterFlowPixelShader Is Nothing Then
            Throw New InvalidOperationException("Der WaterFlow-PixelShader konnte nicht erzeugt werden.")
        End If

    End Sub

    Private Sub InitialisiereSimulationsRessource(ByRef texture As ID3D11Texture2D,
                                              ByRef view As ID3D11ShaderResourceView,
                                              ByRef targetView As ID3D11RenderTargetView,
                                              format As Format)

        texture =
        Direct3DRessourceHandler.ErstelleSimulationsTexture(
            renderDevice,
            renderBreite,
            renderHoehe,
            format)

        If texture Is Nothing Then
            Throw New InvalidOperationException(
            "Eine Simulations-Texture im Format " &
            format.ToString() &
            " konnte nicht erzeugt werden.")
        End If

        view = renderDevice.CreateShaderResourceView(texture)

        If view Is Nothing Then
            Throw New InvalidOperationException(
            "Die ShaderResourceView einer Simulations-Texture im Format " &
            format.ToString() &
            " konnte nicht erzeugt werden.")
        End If

        targetView = renderDevice.CreateRenderTargetView(texture)

        If targetView Is Nothing Then
            Throw New InvalidOperationException(
            "Die RenderTargetView einer Simulations-Texture im Format " &
            format.ToString() &
            " konnte nicht erzeugt werden.")
        End If

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

        renderSampler = renderDevice.CreateSamplerState(samplerDescription)

        If renderSampler Is Nothing Then
            Throw New InvalidOperationException("Der D3D11-Sampler konnte nicht erzeugt werden.")
        End If

    End Sub

    Private Sub InitialisiereWaterFlowConstantBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer

        bufferGroesse = Marshal.SizeOf(GetType(WaterFlowConstants))

        If bufferGroesse <> 16 Then

            Throw New InvalidOperationException("WaterFlowConstants besitzt eine unerwartete Größe. " &
                                                "Erwartet: 16 Byte, tatsächlich: " & bufferGroesse.ToString() &
                                                " Byte.")

        End If

        bufferDescription = New BufferDescription()
        bufferDescription.ByteWidth = CUInt(bufferGroesse)
        bufferDescription.Usage = ResourceUsage.Default
        bufferDescription.BindFlags = BindFlags.ConstantBuffer
        bufferDescription.CPUAccessFlags = CpuAccessFlags.None
        bufferDescription.MiscFlags = ResourceOptionFlags.None
        bufferDescription.StructureByteStride = 0UI

        waterFlowConstantBuffer = renderDevice.CreateBuffer(bufferDescription)

        If waterFlowConstantBuffer Is Nothing Then
            Throw New InvalidOperationException("Der WaterFlow-ConstantBuffer konnte nicht erzeugt werden.")
        End If

    End Sub

    Private Sub InitialisierePigmentFlowConstantBuffer()

        Dim bufferDescription As BufferDescription


        bufferDescription = New BufferDescription() With {
        .ByteWidth = Marshal.SizeOf(GetType(PigmentFlowConstants)),
        .Usage = ResourceUsage.Default,
        .BindFlags = BindFlags.ConstantBuffer,
        .CPUAccessFlags = CpuAccessFlags.None,
        .MiscFlags = ResourceOptionFlags.None,
        .StructureByteStride = 0
    }

        pigmentFlowConstantBuffer = renderDevice.CreateBuffer(bufferDescription)

    End Sub

    Private Sub InitialisierePigmentDisplayConstantBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer

        bufferGroesse = Marshal.SizeOf(GetType(PigmentDisplayConstants))

        If bufferGroesse <> 16 Then

            Throw New InvalidOperationException(
                "PigmentDisplayConstants besitzt eine unerwartete Größe. " &
                "Erwartet: 16 Byte, tatsächlich: " &
                bufferGroesse.ToString() &
                " Byte.")

        End If

        bufferDescription = New BufferDescription()

        bufferDescription.ByteWidth = CUInt(bufferGroesse)
        bufferDescription.Usage = ResourceUsage.Default
        bufferDescription.BindFlags = BindFlags.ConstantBuffer
        bufferDescription.CPUAccessFlags = CpuAccessFlags.None
        bufferDescription.MiscFlags = ResourceOptionFlags.None
        bufferDescription.StructureByteStride = 0UI

        pigmentDisplayConstantBuffer = renderDevice.CreateBuffer(bufferDescription)

        If pigmentDisplayConstantBuffer Is Nothing Then

            Throw New InvalidOperationException("Der PigmentDisplay-ConstantBuffer konnte nicht erzeugt werden.")

        End If

    End Sub

#End Region

#Region "Simulation"

    Private Sub SimuliereWasserUndPigmente(viskositaet As Single,
                                      iterationen As Integer)

        Dim waterFlowParameter As WaterFlowConstants
        Dim pigmentFlowParameter As PigmentFlowConstants

        Dim tempTexture As ID3D11Texture2D
        Dim tempView As ID3D11ShaderResourceView
        Dim tempTargetView As ID3D11RenderTargetView

        Dim i As Integer

        If iterationen <= 0 Then
            Exit Sub
        End If

        waterFlowParameter.viscosity = Math.Max(0.0001F, viskositaet)
        waterFlowParameter.reserve1 = 0.0F
        waterFlowParameter.reserve2 = 0.0F
        waterFlowParameter.reserve3 = 0.0F

        pigmentFlowParameter.pigmentTransportStrength = TEST_PIGMENT_TRANSPORT_STRENGTH
        pigmentFlowParameter.padding1 = 0.0F
        pigmentFlowParameter.padding2 = 0.0F
        pigmentFlowParameter.padding3 = 0.0F

        renderContext.UpdateSubresource(waterFlowParameter, waterFlowConstantBuffer)
        renderContext.UpdateSubresource(pigmentFlowParameter, pigmentFlowConstantBuffer)
        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(renderBreite), CSng(renderHoehe), 0.0F, 1.0F))
        renderContext.OMSetBlendState(Nothing)

        For i = 0 To iterationen - 1

            ' ============================================================
            ' WASSER
            ' ============================================================
            '
            ' sourceWater -> targetWater
            ' ============================================================

            renderContext.OMSetRenderTargets(targetWaterTargetView)
            renderContext.VSSetShader(waterFlowVertexShader)
            renderContext.PSSetShader(waterFlowPixelShader)
            renderContext.PSSetConstantBuffer(0UI, waterFlowConstantBuffer)
            renderContext.PSSetShaderResource(0UI, sourceWaterView)

            renderContext.Draw(3UI, 0UI)

            renderContext.PSSetShaderResource(0UI, Nothing)
            D3D11InteropHelper.UnbindRenderTarget(renderContext)


            ' ============================================================
            ' PIGMENT
            ' ============================================================
            '
            ' sourcePigment + sourceWater -> targetPigment
            '
            ' WICHTIG:
            '
            ' Wir benutzen hier bewusst noch sourceWater.
            '
            ' Damit bewegen sich Wasser und Pigment innerhalb derselben
            ' Simulationszeitscheibe aus demselben Ausgangszustand.
            '
            ' Erst DANACH werden beide Paare gemeinsam vertauscht.
            ' ============================================================

            renderContext.OMSetRenderTargets(targetPigmentSuspensionTargetView)
            renderContext.VSSetShader(pigmentFlowVertexShader)
            renderContext.PSSetShader(pigmentFlowPixelShader)
            renderContext.PSSetConstantBuffer(0UI, pigmentFlowConstantBuffer)
            renderContext.PSSetShaderResource(0UI, sourcePigmentSuspensionView)
            renderContext.PSSetShaderResource(1UI, sourceWaterView)

            renderContext.Draw(3UI, 0UI)

            ' ============================================================
            ' SRVs lösen
            ' ============================================================

            renderContext.PSSetShaderResource(0UI, Nothing)
            renderContext.PSSetShaderResource(1UI, Nothing)
            D3D11InteropHelper.UnbindRenderTarget(renderContext)


            ' ============================================================
            ' WASSER PING-PONG
            ' ============================================================

            tempTexture = sourceWaterTexture
            sourceWaterTexture = targetWaterTexture
            targetWaterTexture = tempTexture

            tempView = sourceWaterView
            sourceWaterView = targetWaterView
            targetWaterView = tempView

            tempTargetView = sourceWaterTargetView
            sourceWaterTargetView = targetWaterTargetView
            targetWaterTargetView = tempTargetView


            ' ============================================================
            ' PIGMENT PING-PONG
            ' ============================================================

            tempTexture = sourcePigmentSuspensionTexture
            sourcePigmentSuspensionTexture = targetPigmentSuspensionTexture
            targetPigmentSuspensionTexture = tempTexture

            tempView = sourcePigmentSuspensionView
            sourcePigmentSuspensionView = targetPigmentSuspensionView
            targetPigmentSuspensionView = tempView

            tempTargetView = sourcePigmentSuspensionTargetView
            sourcePigmentSuspensionTargetView = targetPigmentSuspensionTargetView
            targetPigmentSuspensionTargetView = tempTargetView

        Next


        ' ================================================================
        ' Pipeline sauber verlassen
        ' ================================================================

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetShaderResource(1UI, Nothing)

        renderContext.PSSetConstantBuffer(0UI, Nothing)

        renderContext.PSSetShader(Nothing)
        renderContext.VSSetShader(Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

#End Region

#Region "Rendering"

    Private Sub AktualisierePigmentDisplayConstantBuffer()

        Dim hintergrundFarbe As System.Drawing.Color
        Dim pigmentDisplayParameter As PigmentDisplayConstants

        hintergrundFarbe = SlideShowTools.SharedDataHandling.HintergrundFarbeSaver

        pigmentDisplayParameter.backgroundRed = hintergrundFarbe.R / 255.0F
        pigmentDisplayParameter.backgroundGreen = hintergrundFarbe.G / 255.0F
        pigmentDisplayParameter.backgroundBlue = hintergrundFarbe.B / 255.0F
        pigmentDisplayParameter.backgroundAlpha = 1.0F

        renderContext.UpdateSubresource(pigmentDisplayParameter, pigmentDisplayConstantBuffer)

    End Sub

    Friend Function RenderBild() As Bitmap

        Dim clearColor As Color4

        Dim quadrantBreite As Integer
        Dim quadrantHoehe As Integer

        Dim ergebnis As Bitmap


        ergebnis = Nothing

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(D3DRenderer))
        End If

        If Not istInitialisiert Then
            Throw New InvalidOperationException("Der D3DRenderer wurde noch nicht initialisiert.")
        End If

#Region "Initialisierung der Shader"

        ' ============================================================
        ' PASS II Initialisierung: Classic Kuwahara, volle Auflösung
        ' ============================================================

        renderContext.OMSetRenderTargets(kuwaharaTargetView)
        renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(renderBreite), CSng(renderHoehe), 0.0F, 1.0F))
        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)
        renderContext.VSSetShader(kuwaharaVertexShader)
        renderContext.PSSetShader(kuwaharaPixelShader)
        renderContext.PSSetShaderResource(0UI, sourceView)
        renderContext.PSSetSampler(0UI, renderSampler)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)
        D3D11InteropHelper.UnbindRenderTarget(renderContext)

        ' ============================================================
        ' PASS III Initialisierung: Pigmente initialisieren
        ' ============================================================
        '
        ' Das fertige Kuwahara-Bild wird als Ausgangsfarbverteilung
        ' in die mobile Pigment-Texture geschrieben.
        '
        ' RGB = farbige Pigmentmasse
        ' A   = Pigmentmenge
        '
        ' Für V0.1 startet die Pigmentmenge überall mit 1.0.
        ' ============================================================
        renderContext.OMSetRenderTargets(sourcePigmentSuspensionTargetView)
        renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(renderBreite), CSng(renderHoehe), 0.0F, 1.0F))
        renderContext.VSSetShader(pigmentInitializerVertexShader)
        renderContext.PSSetShader(pigmentInitializerPixelShader)
        renderContext.PSSetShaderResource(0UI, kuwaharaView)
        renderContext.PSSetSampler(0UI, renderSampler)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)
        D3D11InteropHelper.UnbindRenderTarget(renderContext)

        ' ============================================================
        ' PASS IV Initialisierung: Wasser aus Kuwahara erzeugen
        ' ============================================================
        '
        ' Das bereits vereinfachte Kuwahara-Bild dient nun als Grundlage
        ' für die WaterMap.
        '
        ' Große homogene Farbflächen bleiben weitgehend ruhig.
        '
        ' Deutliche Kuwahara-Farbgrenzen erzeugen leichte Senken im
        ' Wasserfeld. Damit ersetzen wir für den aktuellen PoC bewusst
        ' einen Teil der noch nicht vorhandenen Pigmentablagerungs- und
        ' Verdunstungsphysik.
        ' ============================================================

        renderContext.OMSetRenderTargets(sourceWaterTargetView)
        renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(renderBreite), CSng(renderHoehe), 0.0F, 1.0F))
        renderContext.VSSetShader(waterInitializerVertexShader)
        renderContext.PSSetShader(waterInitializerPixelShader)
        renderContext.PSSetShaderResource(0UI, kuwaharaView)

        renderContext.Draw(3UI, 0UI)

        ' Kuwahara-SRV unbedingt wieder lösen.
        '
        ' Für den WaterInitializer selbst wäre das anschließende Ping-Pong
        ' zwar noch kein Konflikt, aber wir verlassen jeden Pass weiterhin
        ' mit sauberer D3D-Pipeline.

        renderContext.PSSetShaderResource(0UI, Nothing)
        D3D11InteropHelper.UnbindRenderTarget(renderContext)

#End Region

        ' ============================================================
        ' TEST V0.2 
        ' ============================================================
        ' Pass I Testing: Copy-Pass Originalbild
        ' ============================================================
        '
        ' Das vollständige RenderTarget besitzt weiterhin exakt die
        ' Dimensionen des Originalbildes.
        '
        ' Wir reduzieren lediglich den Viewport auf das linke obere
        ' Viertel.
        '
        ' Unser Fullscreen-Triangle füllt damit nicht den gesamten
        ' RenderTarget, sondern ausschließlich diesen Viewport.
        '
        ' Das ist bereits exakt die Geometrie, die wir später für die
        ' vier Vergleichsfelder benötigen.
        ' ============================================================

        quadrantBreite = Math.Max(1, renderBreite \ 2)
        quadrantHoehe = Math.Max(1, renderHoehe \ 2)

        ' Erst das gesamte private RenderTarget schwarz löschen.

        clearColor = New Color4(0.0F, 0.0F, 0.0F, 1.0F)

        D3D11InteropHelper.ClearRenderTargetView(renderContext, renderTargetView, 0.0F, 0.0F, 0.0F, 1.0F)

        ' Privates RenderTarget aktivieren.

        renderContext.OMSetRenderTargets(renderTargetView)

        ' Nur links oben rendern.

        renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(quadrantBreite), CSng(quadrantHoehe), 0.0F, 1.0F))
        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)
        renderContext.VSSetShader(copyVertexShader)
        renderContext.PSSetShader(copyPixelShader)
        renderContext.PSSetShaderResource(0UI, sourceView)
        renderContext.PSSetSampler(0UI, renderSampler)

        ' Fullscreen-Triangle.
        '
        ' Es existiert bewusst kein VertexBuffer.
        ' Die drei Positionen entstehen über SV_VertexID direkt im
        ' VertexShader.

        renderContext.Draw(3UI, 0UI)

        ' ============================================================
        ' PASS II Testing: Kuwahara unten links
        '
        ' Erzeugt ist er ja bereits, daher nur per Copy-Shader in die
        ' linke untere Ecke platzieren
        ' ============================================================
        renderContext.RSSetViewport(New Viewport(0.0F, CSng(quadrantHoehe), CSng(quadrantBreite),
                                                 CSng(quadrantHoehe), 0.0F, 1.0F))

        renderContext.VSSetShader(copyVertexShader)
        renderContext.PSSetShader(copyPixelShader)
        renderContext.PSSetShaderResource(0UI, kuwaharaView)

        renderContext.Draw(3UI, 0UI)

        '' ============================================================
        '' Pass III Testing: Pigment Initialisierung unten rechts
        '' ============================================================
        'renderContext.RSSetViewport(New Viewport(CSng(quadrantBreite), CSng(quadrantHoehe), CSng(quadrantBreite),
        '                                         CSng(quadrantHoehe), 0.0F, 1.0F))
        'renderContext.PSSetShaderResource(0UI, sourcePigmentSuspensionView)

        'renderContext.Draw(3UI, 0UI)

        ' ============================================================
        ' Pass IV Testing: Water Initialisierung oben rechts
        ' ============================================================
        renderContext.RSSetViewport(New Viewport(CSng(quadrantBreite), 0.0F, CSng(quadrantBreite),
                                                 CSng(quadrantHoehe), 0.0F, 1.0F))
        renderContext.PSSetShaderResource(0UI, sourceWaterView)

        renderContext.Draw(3UI, 0UI)

        'Vor Beginn der Simulation noch einmal SRV lösen
        renderContext.PSSetShaderResource(0UI, Nothing)
        D3D11InteropHelper.UnbindRenderTarget(renderContext)

        ' ============================================================
        ' PASS V: Wasserfluss-Simulation mit konstanten Iterationen &
        '         Viskosität
        ' ============================================================

        SimuliereWasserUndPigmente(TEST_VISKOSITAET, TEST_ITERATIONEN)

        ' ============================================================
        ' PASS VI Testing:
        ' Pigmente nach Wassertransport unten rechts darstellen
        ' ============================================================
        '
        ' WICHTIG:
        '
        ' sourcePigmentSuspensionView enthält den INTERNEN Simulationszustand:
        '
        ' RGB = premultiplizierte Pigmentfarbe
        ' A   = Pigmentmenge
        '
        ' Der PigmentDisplayShader komponiert daraus ein vollständig
        ' opakes Bild über HintergrundFarbeSaver.
        ' ============================================================

        AktualisierePigmentDisplayConstantBuffer()

        renderContext.OMSetRenderTargets(renderTargetView)
        renderContext.RSSetViewport(New Viewport(CSng(quadrantBreite), CSng(quadrantHoehe), CSng(quadrantBreite),
                                                 CSng(quadrantHoehe), 0.0F, 1.0F))
        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)
        renderContext.VSSetShader(pigmentDisplayVertexShader)
        renderContext.PSSetShader(pigmentDisplayPixelShader)
        renderContext.PSSetConstantBuffer(0UI, pigmentDisplayConstantBuffer)
        renderContext.PSSetShaderResource(0UI, sourcePigmentSuspensionView)
        renderContext.PSSetSampler(0UI, renderSampler)

        renderContext.Draw(3UI, 0UI)

        ' ============================================================
        ' SRV wieder lösen.
        '
        ' Das wird später bei Multipass besonders wichtig, weil dieselbe
        ' Texture niemals gleichzeitig als Input und Output gebunden
        ' bleiben darf.
        ' ============================================================

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

        ' ============================================================
        ' READBACK
        ' ============================================================
        '
        ' Erst nachdem das Bild vollständig im privaten RenderTarget
        ' aufgebaut wurde, kopieren wir es in die CPU-lesbare
        ' Staging-Texture.
        '
        ' Es existiert damit auch hier zu keinem Zeitpunkt ein
        ' "halbfertiges veröffentlichtes Bild".

        renderContext.CopyResource(stagingTexture, renderTargetTexture)

        ' Bei diesem statischen Roundtrip wäre Map(Read) bereits eine
        ' Synchronisationsstelle.
        '
        ' Wir behalten Flush() zunächst trotzdem ausdrücklich bei.
        '
        ' Erstens entspricht dies unserer bewährten D3D11-Diagnostik,
        ' zweitens wollen wir für V0.1 keinerlei Unsicherheit darüber,
        ' wann der Copy-Befehl tatsächlich an den Treiber übergeben
        ' wurde.

        renderContext.Flush()

        ergebnis = LeseStagingTextureAlsBitmap()

        Return ergebnis

    End Function

#End Region

#Region "Readback"

    Private Function LeseStagingTextureAlsBitmap() As Bitmap

        Dim mappedResource As MappedSubresource

        Dim bitmap As Bitmap
        Dim bitmapData As BitmapData

        Dim rectangle As Rectangle

        Dim zeile() As Byte

        Dim quellPointer As IntPtr
        Dim zielPointer As IntPtr

        Dim bytesProZeile As Integer
        Dim y As Integer

        bitmap = Nothing
        bitmapData = Nothing

        zeile = Nothing

        mappedResource = renderContext.Map(stagingTexture, 0UI, MapMode.Read, Vortice.Direct3D11.MapFlags.None)

        Try

            bitmap = New Bitmap(renderBreite, renderHoehe, PixelFormat.Format32bppArgb)

            rectangle = New Rectangle(0, 0, renderBreite, renderHoehe)

            bitmapData = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

            ' RowPitch einer D3D11-Texture darf größer sein als
            '
            '     Breite * 4
            '
            ' weil GPU-Zeilen intern aus Alignment-Gründen gepolstert
            ' werden können.
            '
            ' Deshalb niemals die komplette Texture als einen einzigen
            ' zusammenhängenden Block kopieren.

            bytesProZeile = renderBreite * 4

            ReDim zeile(bytesProZeile - 1)

            For y = 0 To renderHoehe - 1

                quellPointer = IntPtr.Add(mappedResource.DataPointer, CInt(CUInt(y) * mappedResource.RowPitch))

                zielPointer = IntPtr.Add(bitmapData.Scan0, y * bitmapData.Stride)

                Marshal.Copy(quellPointer, zeile, 0, bytesProZeile)
                Marshal.Copy(zeile, 0, zielPointer, bytesProZeile)

            Next

            bitmap.UnlockBits(bitmapData)

            bitmapData = Nothing

        Catch

            If bitmapData IsNot Nothing AndAlso bitmap IsNot Nothing Then

                bitmap.UnlockBits(bitmapData)

                bitmapData = Nothing

            End If

            If bitmap IsNot Nothing Then

                bitmap.Dispose()
                bitmap = Nothing

            End If

            Throw

        Finally

            renderContext.Unmap(stagingTexture, 0UI)

        End Try

        Return bitmap

    End Function

#End Region

#Region "Shader-Ressourcen"

    Private Shared Function LadeShaderBytecode(dateiname As String) As Byte()

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

            Throw New IO.FileNotFoundException("Die eingebettete Shader-Ressource """ & dateiname &
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

#Region "Bereinigung"

    Private Sub BeendeUndBereinigeRenderer()

        istInitialisiert = False

        renderBreite = 0
        renderHoehe = 0

        If renderContext IsNot Nothing Then

            renderContext.ClearState()
            renderContext.Flush()

        End If


        Direct3DRessourceHandler.GebeFrei(renderSampler)

        GebeBildressourcenFrei()
        GebeRegionDistanceRessourcenFrei()
        GebePapierressourcenFrei()
        GebeWasserressourcenFrei()
        GebePigmentressourcenFrei()
        GebeAusgaberessourcenFrei()

        GebeLegacyWaterRessourcenFrei()

        GebeShaderRessourcenFrei()


        If renderContext IsNot Nothing Then

            renderContext.ClearState()
            renderContext.Flush()

            renderContext.Dispose()
            renderContext = Nothing

        End If


        If renderDevice IsNot Nothing Then

            renderDevice.Dispose()
            renderDevice = Nothing

        End If

    End Sub

    Private Sub GebeBildressourcenFrei()

        Direct3DRessourceHandler.GebeFrei(kuwaharaTargetView)
        Direct3DRessourceHandler.GebeFrei(kuwaharaView)

        Direct3DRessourceHandler.GebeFrei(kuwaharaTexture)

        Direct3DRessourceHandler.GebeFrei(sourceView)
        Direct3DRessourceHandler.GebeFrei(sourceTexture)

    End Sub

    Private Sub GebeRegionDistanceRessourcenFrei()

        Direct3DRessourceHandler.GebeFrei(regionDistanceTargetView)
        Direct3DRessourceHandler.GebeFrei(regionDistanceView)
        Direct3DRessourceHandler.GebeFrei(regionDistanceTexture)

        Direct3DRessourceHandler.GebeFrei(sourceRegionSeedTargetView)
        Direct3DRessourceHandler.GebeFrei(sourceRegionSeedView)
        Direct3DRessourceHandler.GebeFrei(sourceRegionSeedTexture)

        Direct3DRessourceHandler.GebeFrei(targetRegionSeedTargetView)
        Direct3DRessourceHandler.GebeFrei(targetRegionSeedView)
        Direct3DRessourceHandler.GebeFrei(targetRegionSeedTexture)

    End Sub

    Private Sub GebePapierressourcenFrei()

        Direct3DRessourceHandler.GebeFrei(paperTargetView)
        Direct3DRessourceHandler.GebeFrei(paperView)
        Direct3DRessourceHandler.GebeFrei(paperTexture)

    End Sub

    Private Sub GebeWasserressourcenFrei()

        Direct3DRessourceHandler.GebeFrei(sourcePressureTargetView)
        Direct3DRessourceHandler.GebeFrei(sourcePressureView)
        Direct3DRessourceHandler.GebeFrei(sourcePressureTexture)

        Direct3DRessourceHandler.GebeFrei(targetPressureTargetView)
        Direct3DRessourceHandler.GebeFrei(targetPressureView)
        Direct3DRessourceHandler.GebeFrei(targetPressureTexture)


        Direct3DRessourceHandler.GebeFrei(sourceVelocityTargetView)
        Direct3DRessourceHandler.GebeFrei(sourceVelocityView)
        Direct3DRessourceHandler.GebeFrei(sourceVelocityTexture)

        Direct3DRessourceHandler.GebeFrei(targetVelocityTargetView)
        Direct3DRessourceHandler.GebeFrei(targetVelocityView)
        Direct3DRessourceHandler.GebeFrei(targetVelocityTexture)

    End Sub

    Private Sub GebePigmentressourcenFrei()

        Direct3DRessourceHandler.GebeFrei(sourcePigmentSuspensionTargetView)
        Direct3DRessourceHandler.GebeFrei(sourcePigmentSuspensionView)
        Direct3DRessourceHandler.GebeFrei(sourcePigmentSuspensionTexture)

        Direct3DRessourceHandler.GebeFrei(targetPigmentSuspensionTargetView)
        Direct3DRessourceHandler.GebeFrei(targetPigmentSuspensionView)
        Direct3DRessourceHandler.GebeFrei(targetPigmentSuspensionTexture)


        Direct3DRessourceHandler.GebeFrei(sourcePigmentDepositTargetView)
        Direct3DRessourceHandler.GebeFrei(sourcePigmentDepositView)
        Direct3DRessourceHandler.GebeFrei(sourcePigmentDepositTexture)

        Direct3DRessourceHandler.GebeFrei(targetPigmentDepositTargetView)
        Direct3DRessourceHandler.GebeFrei(targetPigmentDepositView)
        Direct3DRessourceHandler.GebeFrei(targetPigmentDepositTexture)

    End Sub

    Private Sub GebeAusgaberessourcenFrei()

        Direct3DRessourceHandler.GebeFrei(renderTargetView)

        Direct3DRessourceHandler.GebeFrei(stagingTexture)
        Direct3DRessourceHandler.GebeFrei(renderTargetTexture)

    End Sub

    Private Sub GebeLegacyWaterRessourcenFrei()

        Direct3DRessourceHandler.GebeFrei(sourceWaterTargetView)
        Direct3DRessourceHandler.GebeFrei(sourceWaterView)
        Direct3DRessourceHandler.GebeFrei(sourceWaterTexture)

        Direct3DRessourceHandler.GebeFrei(targetWaterTargetView)
        Direct3DRessourceHandler.GebeFrei(targetWaterView)
        Direct3DRessourceHandler.GebeFrei(targetWaterTexture)

    End Sub

    Private Sub GebeShaderRessourcenFrei()

        Direct3DRessourceHandler.GebeFrei(copyPixelShader)
        Direct3DRessourceHandler.GebeFrei(copyVertexShader)

        Direct3DRessourceHandler.GebeFrei(kuwaharaPixelShader)
        Direct3DRessourceHandler.GebeFrei(kuwaharaVertexShader)

        Direct3DRessourceHandler.GebeFrei(pigmentInitializerPixelShader)
        Direct3DRessourceHandler.GebeFrei(pigmentInitializerVertexShader)

        Direct3DRessourceHandler.GebeFrei(waterInitializerPixelShader)
        Direct3DRessourceHandler.GebeFrei(waterInitializerVertexShader)

        Direct3DRessourceHandler.GebeFrei(waterFlowConstantBuffer)
        Direct3DRessourceHandler.GebeFrei(waterFlowPixelShader)
        Direct3DRessourceHandler.GebeFrei(waterFlowVertexShader)

        Direct3DRessourceHandler.GebeFrei(pigmentFlowConstantBuffer)
        Direct3DRessourceHandler.GebeFrei(pigmentFlowPixelShader)
        Direct3DRessourceHandler.GebeFrei(pigmentFlowVertexShader)

        Direct3DRessourceHandler.GebeFrei(pigmentDisplayConstantBuffer)
        Direct3DRessourceHandler.GebeFrei(pigmentDisplayPixelShader)
        Direct3DRessourceHandler.GebeFrei(pigmentDisplayVertexShader)

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