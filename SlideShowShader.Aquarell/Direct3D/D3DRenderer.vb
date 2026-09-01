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

    Private Const REGION_DISTANCE_MODE_BOUNDARY As UInteger = 0UI
    Private Const REGION_DISTANCE_MODE_PROPAGATE As UInteger = 1UI
    Private Const REGION_DISTANCE_MODE_FINALIZE As UInteger = 2UI
    Private Const REGION_DISTANCE_MODE_DISPLAY As UInteger = 3UI
    Private Const REGION_DISTANCE_COLOR_THRESHOLD As Single = 0.15F
    ' Reine Notbremse. KEIN reguläres Abbruchkriterium.
    Private Const REGION_DISTANCE_MAX_SAFETY_PASSES As Integer = 8192

    Private Const PRESSURE_DISTANCE_SCALE As Single = 64.0F

    Private Const VELOCITY_MODE_UPDATE As UInteger = 0UI
    Private Const VELOCITY_MODE_DISPLAY As UInteger = 1UI

    Private Const VELOCITY_PRESSURE_GRADIENT_STRENGTH As Single = 8.0F
    Private Const VELOCITY_MAX As Single = 1.0F
    Private Const VELOCITY_DISPLAY_SCALE As Single = 8.0F

    Private Const PRESSURE_FLOW_MODE_UPDATE As UInteger = 0UI
    Private Const PRESSURE_FLOW_MODE_DISPLAY As UInteger = 1UI

    Private Const PRESSURE_FLOW_TIME_STEP As Single = 0.2F
    Private Const PRESSURE_FLOW_MAX_PRESSURE As Single = 2.0F
    Private Const PRESSURE_FLOW_DISPLAY_SCALE As Single = 1.0F

    Private Const TEST_CURTIS_ITERATIONEN As Integer = 16

    Private Const PIGMENT_TRANSPORT_TIME_STEP As Single = 0.2F
    Private Const PIGMENT_TRANSPORT_STRENGTH As Single = 1.0F

#End Region

#Region "Variablen"

    '---------------------------------
    ' D3D11-Grundsystem
    '---------------------------------
    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext
    Private renderFeatureLevel As FeatureLevel

#Region "Texturen, Views und TargetViews"

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

#End Region

#Region "Shader & Buffer"

    '---------------------------------
    ' Shader allgemein
    '---------------------------------
    Private renderSampler As ID3D11SamplerState

    '---------------------------------
    ' Copy Shader
    '---------------------------------
    Private copyVertexShader As ID3D11VertexShader
    Private copyPixelShader As ID3D11PixelShader

    '---------------------------------
    ' Kuwahara Shader
    '---------------------------------
    Private kuwaharaVertexShader As ID3D11VertexShader
    Private kuwaharaPixelShader As ID3D11PixelShader

    '---------------------------------
    ' Paper Initializer Shader
    '---------------------------------
    Private paperInitializerVertexShader As ID3D11VertexShader
    Private paperInitializerPixelShader As ID3D11PixelShader

    '---------------------------------
    ' Region Distance Shader
    '---------------------------------
    Private regionDistanceVertexShader As ID3D11VertexShader
    Private regionDistancePixelShader As ID3D11PixelShader

    Private regionDistanceConstantBuffer As ID3D11Buffer

    '---------------------------------
    ' Region Distance Changed Counter
    '---------------------------------
    Private regionDistanceChangedCounterBuffer As ID3D11Buffer
    Private regionDistanceChangedCounterView As ID3D11UnorderedAccessView

    Private regionDistanceChangedCounterStagingBuffer As ID3D11Buffer

    '---------------------------------
    ' Pressure Initializer Shader
    '---------------------------------
    Private pressureInitializerVertexShader As ID3D11VertexShader
    Private pressureInitializerPixelShader As ID3D11PixelShader

    Private pressureInitializerConstantBuffer As ID3D11Buffer

    '---------------------------------
    ' Velocity Shader
    '---------------------------------
    Private velocityVertexShader As ID3D11VertexShader
    Private velocityPixelShader As ID3D11PixelShader

    Private velocityConstantBuffer As ID3D11Buffer

    '---------------------------------
    ' Pressure Flow Shader
    '---------------------------------
    Private pressureFlowVertexShader As ID3D11VertexShader
    Private pressureFlowPixelShader As ID3D11PixelShader

    Private pressureFlowConstantBuffer As ID3D11Buffer

    '---------------------------------
    ' Pigment Transport Shader
    '---------------------------------
    Private pigmentTransportVertexShader As ID3D11VertexShader
    Private pigmentTransportPixelShader As ID3D11PixelShader

    Private pigmentTransportConstantBuffer As ID3D11Buffer

    '---------------------------------
    ' Pigment Initializer Shader
    '---------------------------------
    Private pigmentInitializerVertexShader As ID3D11VertexShader
    Private pigmentInitializerPixelShader As ID3D11PixelShader

    '---------------------------------
    ' Water Initializer Shader
    '
    ' LEGACY - nach Umbau von RenderBild entfernen
    '---------------------------------
    Private waterInitializerVertexShader As ID3D11VertexShader
    Private waterInitializerPixelShader As ID3D11PixelShader

    '---------------------------------
    ' Water Flow Shader
    '
    ' LEGACY - nach Umbau von RenderBild entfernen
    '---------------------------------
    Private waterFlowVertexShader As ID3D11VertexShader
    Private waterFlowPixelShader As ID3D11PixelShader
    Private waterFlowConstantBuffer As ID3D11Buffer

    '---------------------------------
    ' Pigment Flow Shader
    '
    ' LEGACY - wird beim Umbau der Pigmentsimulation ersetzt
    '---------------------------------
    Private pigmentFlowVertexShader As ID3D11VertexShader
    Private pigmentFlowPixelShader As ID3D11PixelShader
    Private pigmentFlowConstantBuffer As ID3D11Buffer

    '---------------------------------
    ' Pigment Display Shader
    '---------------------------------
    Private pigmentDisplayVertexShader As ID3D11VertexShader
    Private pigmentDisplayPixelShader As ID3D11PixelShader
    Private pigmentDisplayConstantBuffer As ID3D11Buffer

#End Region

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
    Private Structure RegionDistanceConstants

        Public mode As UInteger
        Public jumpStep As UInteger

        Public regionColorThreshold As Single
        Public displayDistanceScale As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PressureInitializerConstants

        Public pressureDistanceScale As Single

        Public reserve1 As Single
        Public reserve2 As Single
        Public reserve3 As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure VelocityConstants

        Public mode As UInteger

        Public pressureGradientStrength As Single
        Public maxVelocity As Single
        Public displayVelocityScale As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PressureFlowConstants

        Public mode As UInteger

        Public timeStep As Single
        Public maxPressure As Single
        Public displayPressureScale As Single

    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure PigmentTransportConstants

        Public timeStep As Single
        Public transportStrength As Single

        Public reserve1 As Single
        Public reserve2 As Single

    End Structure

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

        InitialisiereRegionDistanceConstantBuffer()
        InitialisierePressureInitializerConstantBuffer()
        InitialisiereVelocityConstantBuffer()
        InitialisierePressureFlowConstantBuffer()
        InitialisierePigmentTransportConstantBuffer()

        'Legacy
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

        InitialisiereVertexPixelShader("AquarellCopyShader", copyVertexShader, copyPixelShader)
        InitialisiereVertexPixelShader("KuwaharaShader", kuwaharaVertexShader, kuwaharaPixelShader)
        InitialisiereVertexPixelShader("PaperInitializerShader", paperInitializerVertexShader, paperInitializerPixelShader)
        InitialisiereVertexPixelShader("RegionDistanceShader", regionDistanceVertexShader, regionDistancePixelShader)
        InitialisiereVertexPixelShader("PressureInitializerShader", pressureInitializerVertexShader, pressureInitializerPixelShader)
        InitialisiereVertexPixelShader("VelocityShader", velocityVertexShader, velocityPixelShader)
        InitialisiereVertexPixelShader("PressureFlowShader", pressureFlowVertexShader, pressureFlowPixelShader)
        InitialisiereVertexPixelShader("PigmentTransportShader", pigmentTransportVertexShader, pigmentTransportPixelShader)

        'Legacy
        InitialisiereVertexPixelShader("PigmentInitializerShader", pigmentInitializerVertexShader, pigmentInitializerPixelShader)
        InitialisiereVertexPixelShader("WaterInitializerShader", waterInitializerVertexShader, waterInitializerPixelShader)
        InitialisiereVertexPixelShader("WaterFlowShader", waterFlowVertexShader, waterFlowPixelShader)
        InitialisiereVertexPixelShader("PigmentFlowShader", pigmentFlowVertexShader, pigmentFlowPixelShader)
        InitialisiereVertexPixelShader("PigmentDisplayShader", pigmentDisplayVertexShader, pigmentDisplayPixelShader)

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

#Region "RegionDistance"

    Private Sub InitialisiereRegionDistanceRessourcen()

        Dim counterViewDescription As UnorderedAccessViewDescription

        ' ================================================================
        ' Ergebnis der Distance Transformation
        ' ================================================================

        InitialisiereSimulationsRessource(regionDistanceTexture, regionDistanceView, regionDistanceTargetView,
                                          Format.R32_Float)

        ' ================================================================
        ' Ping-Pong-Arbeitsfelder des Jump-Flood-Algorithmus
        '
        ' R32G32_Float ist bewusst gewählt:
        '
        ' X = Seed-X
        ' Y = Seed-Y
        '
        ' Auch bei 4K sollen die Pixelkoordinaten ohne Half-Float-
        ' Quantisierung gespeichert werden.
        ' ================================================================

        InitialisiereSimulationsRessource(sourceRegionSeedTexture, sourceRegionSeedView, sourceRegionSeedTargetView,
                                          Format.R32G32_Float)

        InitialisiereSimulationsRessource(targetRegionSeedTexture, targetRegionSeedView, targetRegionSeedTargetView,
                                          Format.R32G32_Float)

        ' ================================================================
        ' ChangedCounter für die exakte lokale Relaxation
        ' ================================================================

        regionDistanceChangedCounterBuffer = Direct3DRessourceHandler.ErstelleStructuredCounterBuffer(renderDevice)

        regionDistanceChangedCounterStagingBuffer =
        Direct3DRessourceHandler.ErstelleStagingCounterBuffer(renderDevice)

        counterViewDescription =
        New UnorderedAccessViewDescription(
            regionDistanceChangedCounterBuffer,
            Format.Unknown,
            0UI,
            1UI,
            BufferUnorderedAccessViewFlags.None)

        regionDistanceChangedCounterView =
        renderDevice.CreateUnorderedAccessView(
            regionDistanceChangedCounterBuffer,
            counterViewDescription)

        If regionDistanceChangedCounterView Is Nothing Then

            Throw New InvalidOperationException(
            "Die RegionDistance-ChangedCounter-UAV konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub SetzeRegionDistanceChangedCounterZurueck()

        Dim zeroValue As UInteger

        zeroValue = 0UI

        renderContext.UpdateSubresource(zeroValue, regionDistanceChangedCounterBuffer)

    End Sub

    Private Function LeseRegionDistanceChangedCounter() As UInteger

        Dim mappedResource As MappedSubresource
        Dim changedCount As UInteger

        changedCount = 0UI

        renderContext.CopyResource(regionDistanceChangedCounterStagingBuffer, regionDistanceChangedCounterBuffer)

        mappedResource = renderContext.Map(regionDistanceChangedCounterStagingBuffer, 0UI, MapMode.Read,
                                           Vortice.Direct3D11.MapFlags.None)


        Try

            changedCount = CUInt(Marshal.ReadInt32(mappedResource.DataPointer))

        Finally

            renderContext.Unmap(regionDistanceChangedCounterStagingBuffer, 0UI)

        End Try


        Return changedCount

    End Function

    Private Sub TauscheRegionSeedRessourcen()

        Dim tempTexture As ID3D11Texture2D
        Dim tempView As ID3D11ShaderResourceView
        Dim tempTargetView As ID3D11RenderTargetView


        tempTexture = sourceRegionSeedTexture
        sourceRegionSeedTexture = targetRegionSeedTexture
        targetRegionSeedTexture = tempTexture


        tempView = sourceRegionSeedView
        sourceRegionSeedView = targetRegionSeedView
        targetRegionSeedView = tempView

        tempTargetView = sourceRegionSeedTargetView
        sourceRegionSeedTargetView = targetRegionSeedTargetView
        targetRegionSeedTargetView = tempTargetView

    End Sub

    Private Sub ErzeugeRegionBoundarySeeds()

        AktualisiereRegionDistanceConstantBuffer(REGION_DISTANCE_MODE_BOUNDARY, 0UI)

        renderContext.OMSetRenderTargets(sourceRegionSeedTargetView)

        SetzeVollbildViewport()

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(regionDistanceVertexShader)
        renderContext.PSSetShader(regionDistancePixelShader)

        renderContext.PSSetConstantBuffer(0UI, regionDistanceConstantBuffer)

        ' Die Boundary-Erkennung arbeitet ausschließlich auf Kuwahara.
        renderContext.PSSetShaderResource(0UI, kuwaharaView)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

    Private Function FuehreRegionDistancePropagationsPassAus(jumpStep As UInteger, changedCountLesen As Boolean) _
        As UInteger

        Dim changedCount As UInteger


        changedCount = 0UI


        SetzeRegionDistanceChangedCounterZurueck()
        AktualisiereRegionDistanceConstantBuffer(REGION_DISTANCE_MODE_PROPAGATE, jumpStep)

        renderContext.OMSetRenderTargets(targetRegionSeedTargetView)

        ' Ein RTV liegt auf Output-Slot 0.
        '
        ' Deshalb liegt unsere UAV auf Slot 1
        ' und der HLSL-Counter entsprechend auf register(u1).

        renderContext.OMSetUnorderedAccessView(1UI, regionDistanceChangedCounterView)

        SetzeVollbildViewport()

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(regionDistanceVertexShader)
        renderContext.PSSetShader(regionDistancePixelShader)

        renderContext.PSSetConstantBuffer(0UI, regionDistanceConstantBuffer)
        renderContext.PSSetShaderResource(1UI, sourceRegionSeedView)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(1UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)
        renderContext.OMSetUnorderedAccessView(1UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)


        If changedCountLesen Then

            changedCount = LeseRegionDistanceChangedCounter()

        End If


        TauscheRegionSeedRessourcen()


        Return changedCount

    End Function

    Private Sub BerechneRegionDistance()

        Dim maxDimension As Integer
        Dim jumpStep As Integer

        Dim changedCount As UInteger
        Dim refinementPasses As Integer

        maxDimension = Math.Max(renderBreite, renderHoehe)

        ' ========================================================================
        ' 1. Relevante Kuwahara-Grenzen direkt als Boundary-Seeds erzeugen
        '
        ' Boundary-Erkennung:
        '
        '   - mehrskaliger Farbgradient
        '   - REGION_DISTANCE_COLOR_THRESHOLD
        '   - Non-Maximum Suppression
        ' ========================================================================

        ErzeugeRegionBoundarySeeds()

        ' ========================================================================
        ' 2. Größte sinnvolle Jump-Flood-Sprungweite bestimmen
        ' ========================================================================

        jumpStep = 1

        Do While jumpStep < maxDimension

            jumpStep *= 2

        Loop

        jumpStep \= 2

        If jumpStep < 1 Then
            jumpStep = 1
        End If

        ' ========================================================================
        ' 3. Jump Flood
        '
        ' Die groben Sprungweiten benötigen keinen CPU-Readback.
        ' ========================================================================

        Do While jumpStep > 1

            FuehreRegionDistancePropagationsPassAus(CUInt(jumpStep), False)

            jumpStep \= 2

        Loop

        ' ========================================================================
        ' 4. Erster exakter Pass mit jumpStep = 1
        '
        ' Ab hier verwenden wir ChangedCount.
        ' ========================================================================

        changedCount = FuehreRegionDistancePropagationsPassAus(1UI, True)

        refinementPasses = 1

        ' ========================================================================
        ' 5. Lokale Relaxation bis keine Verbesserung mehr stattfindet
        ' ========================================================================

        Do While changedCount > 0UI

            If refinementPasses >= REGION_DISTANCE_MAX_SAFETY_PASSES Then

                Throw New InvalidOperationException(
                "RegionDistance erreichte die Sicherheitsgrenze von " &
                REGION_DISTANCE_MAX_SAFETY_PASSES.ToString() &
                " Relaxationspässen. Der Algorithmus konvergiert nicht.")

            End If

            changedCount = FuehreRegionDistancePropagationsPassAus(1UI, True)

            refinementPasses += 1

        Loop

        ' ========================================================================
        ' 6. Seed-Koordinaten -> echte Pixeldistanz
        ' ========================================================================

        AktualisiereRegionDistanceConstantBuffer(REGION_DISTANCE_MODE_FINALIZE, 0UI)

        renderContext.OMSetRenderTargets(regionDistanceTargetView)

        SetzeVollbildViewport()

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(regionDistanceVertexShader)
        renderContext.PSSetShader(regionDistancePixelShader)

        renderContext.PSSetConstantBuffer(0UI, regionDistanceConstantBuffer)

        renderContext.PSSetShaderResource(1UI, sourceRegionSeedView)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(1UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

        LogHandling.LogDebug("Aquarell D3D: RegionDistance abgeschlossen. Refinement-Pässe: " &
                             refinementPasses.ToString())

    End Sub

#End Region

#Region "Buffer"

    Private Sub InitialisiereRegionDistanceConstantBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer

        bufferGroesse = Marshal.SizeOf(GetType(RegionDistanceConstants))

        If bufferGroesse <> 16 Then

            Throw New InvalidOperationException("RegionDistanceConstants besitzt eine unerwartete Größe. " &
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

        regionDistanceConstantBuffer = renderDevice.CreateBuffer(bufferDescription)


        If regionDistanceConstantBuffer Is Nothing Then
            Throw New InvalidOperationException("Der RegionDistance-ConstantBuffer konnte nicht erzeugt werden.")
        End If

    End Sub

    Private Sub AktualisiereRegionDistanceConstantBuffer(mode As UInteger, jumpStep As UInteger)

        Dim regionDistanceParameter As RegionDistanceConstants


        regionDistanceParameter.mode = mode
        regionDistanceParameter.jumpStep = jumpStep
        regionDistanceParameter.regionColorThreshold = REGION_DISTANCE_COLOR_THRESHOLD
        regionDistanceParameter.displayDistanceScale = Math.Max(1.0F, CSng(Math.Min(renderBreite, renderHoehe)) *
                                                                0.5F)

        renderContext.UpdateSubresource(regionDistanceParameter, regionDistanceConstantBuffer)

    End Sub

    Private Sub InitialisierePressureInitializerConstantBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer


        bufferGroesse = Marshal.SizeOf(GetType(PressureInitializerConstants))


        If bufferGroesse <> 16 Then

            Throw New InvalidOperationException("PressureInitializerConstants besitzt eine unerwartete Größe. " &
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

        pressureInitializerConstantBuffer = renderDevice.CreateBuffer(bufferDescription)

        If pressureInitializerConstantBuffer Is Nothing Then

            Throw New InvalidOperationException(
            "Der PressureInitializer-ConstantBuffer konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub AktualisierePressureInitializerConstantBuffer()

        Dim pressureInitializerParameter As PressureInitializerConstants


        pressureInitializerParameter.pressureDistanceScale = PRESSURE_DISTANCE_SCALE

        pressureInitializerParameter.reserve1 = 0.0F
        pressureInitializerParameter.reserve2 = 0.0F
        pressureInitializerParameter.reserve3 = 0.0F


        renderContext.UpdateSubresource(pressureInitializerParameter, pressureInitializerConstantBuffer)

    End Sub

    Private Sub InitialisiereVelocityConstantBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer


        bufferGroesse = Marshal.SizeOf(GetType(VelocityConstants))


        If bufferGroesse <> 16 Then

            Throw New InvalidOperationException("VelocityConstants besitzt eine unerwartete Größe. " &
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

        velocityConstantBuffer = renderDevice.CreateBuffer(bufferDescription)

        If velocityConstantBuffer Is Nothing Then

            Throw New InvalidOperationException("Der Velocity-ConstantBuffer konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub AktualisiereVelocityConstantBuffer(mode As UInteger)

        Dim velocityParameter As VelocityConstants


        velocityParameter.mode = mode
        velocityParameter.pressureGradientStrength = VELOCITY_PRESSURE_GRADIENT_STRENGTH
        velocityParameter.maxVelocity = VELOCITY_MAX
        velocityParameter.displayVelocityScale = VELOCITY_DISPLAY_SCALE

        renderContext.UpdateSubresource(velocityParameter, velocityConstantBuffer)

    End Sub

    Private Sub InitialisierePressureFlowConstantBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer


        bufferGroesse = Marshal.SizeOf(GetType(PressureFlowConstants))


        If bufferGroesse <> 16 Then

            Throw New InvalidOperationException("PressureFlowConstants besitzt eine unerwartete Größe. " &
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

        pressureFlowConstantBuffer = renderDevice.CreateBuffer(bufferDescription)


        If pressureFlowConstantBuffer Is Nothing Then

            Throw New InvalidOperationException("Der PressureFlow-ConstantBuffer konnte nicht erzeugt werden.")

        End If

    End Sub

    Private Sub AktualisierePressureFlowConstantBuffer(mode As UInteger)

        Dim pressureFlowParameter As PressureFlowConstants

        pressureFlowParameter.mode = mode
        pressureFlowParameter.timeStep = PRESSURE_FLOW_TIME_STEP
        pressureFlowParameter.maxPressure = PRESSURE_FLOW_MAX_PRESSURE
        pressureFlowParameter.displayPressureScale = PRESSURE_FLOW_DISPLAY_SCALE

        renderContext.UpdateSubresource(pressureFlowParameter, pressureFlowConstantBuffer)

    End Sub

    Private Sub InitialisierePigmentTransportConstantBuffer()

        Dim bufferDescription As BufferDescription
        Dim bufferGroesse As Integer


        bufferGroesse = Marshal.SizeOf(GetType(PigmentTransportConstants))


        If bufferGroesse <> 16 Then

            Throw New InvalidOperationException("PigmentTransportConstants besitzt eine unerwartete Größe. " &
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

        pigmentTransportConstantBuffer = renderDevice.CreateBuffer(bufferDescription)


        If pigmentTransportConstantBuffer Is Nothing Then
            Throw New InvalidOperationException("Der PigmentTransport-ConstantBuffer konnte nicht erzeugt werden.")
        End If

    End Sub

    Private Sub AktualisierePigmentTransportConstantBuffer()

        Dim pigmentTransportParameter As PigmentTransportConstants


        pigmentTransportParameter.timeStep = PIGMENT_TRANSPORT_TIME_STEP
        pigmentTransportParameter.transportStrength = PIGMENT_TRANSPORT_STRENGTH
        pigmentTransportParameter.reserve1 = 0.0F
        pigmentTransportParameter.reserve2 = 0.0F


        renderContext.UpdateSubresource(pigmentTransportParameter, pigmentTransportConstantBuffer)

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

#End Region

#End Region

#Region "Simulation"

    Private Sub SimuliereCurtisWasser(iterationen As Integer)

        Dim i As Integer


        If iterationen <= 0 Then
            Exit Sub
        End If


        For i = 0 To iterationen - 1

            ' ============================================================
            ' 1. Druck erzeugt Geschwindigkeit
            ' ============================================================

            BerechneVelocityAusPressure()

            ' ============================================================
            ' 2. Geschwindigkeit transportiert Wasser / Pressure
            ' ============================================================

            BerechnePressureAusVelocity()

            ' ============================================================
            ' 3. Dieselbe Geschwindigkeit transportiert Pigment
            ' ============================================================

            BerechnePigmentTransport()

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

    Private Sub BerechneVelocityAusPressure()

        AktualisiereVelocityConstantBuffer(VELOCITY_MODE_UPDATE)


        ' ================================================================
        ' sourcePressure + sourceVelocity
        '                  ↓
        '             targetVelocity
        ' ================================================================

        renderContext.OMSetRenderTargets(targetVelocityTargetView)

        SetzeVollbildViewport()

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(velocityVertexShader)
        renderContext.PSSetShader(velocityPixelShader)

        renderContext.PSSetConstantBuffer(0UI, velocityConstantBuffer)
        renderContext.PSSetShaderResource(0UI, sourcePressureView)
        renderContext.PSSetShaderResource(1UI, sourceVelocityView)

        renderContext.Draw(3UI, 0UI)


        ' ================================================================
        ' Pipeline lösen
        ' ================================================================

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetShaderResource(1UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

        ' ================================================================
        ' Velocity Ping-Pong
        '
        ' targetVelocity enthält jetzt den neuen Zustand.
        ' Er wird zum neuen sourceVelocity.
        ' ================================================================

        TauscheVelocityRessourcen()

    End Sub

    Private Sub TauscheVelocityRessourcen()

        Dim tempTexture As ID3D11Texture2D
        Dim tempView As ID3D11ShaderResourceView
        Dim tempTargetView As ID3D11RenderTargetView


        tempTexture = sourceVelocityTexture
        sourceVelocityTexture = targetVelocityTexture
        targetVelocityTexture = tempTexture

        tempView = sourceVelocityView
        sourceVelocityView = targetVelocityView
        targetVelocityView = tempView

        tempTargetView = sourceVelocityTargetView
        sourceVelocityTargetView = targetVelocityTargetView
        targetVelocityTargetView = tempTargetView

    End Sub

    Private Sub BerechnePressureAusVelocity()

        AktualisierePressureFlowConstantBuffer(PRESSURE_FLOW_MODE_UPDATE)


        ' ================================================================
        ' sourcePressure + sourceVelocity
        '                  ↓
        '             targetPressure
        ' ================================================================

        renderContext.OMSetRenderTargets(targetPressureTargetView)

        SetzeVollbildViewport()

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(pressureFlowVertexShader)
        renderContext.PSSetShader(pressureFlowPixelShader)

        renderContext.PSSetConstantBuffer(0UI, pressureFlowConstantBuffer)
        renderContext.PSSetShaderResource(0UI, sourcePressureView)
        renderContext.PSSetShaderResource(1UI, sourceVelocityView)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetShaderResource(1UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

        TauschePressureRessourcen()

    End Sub

    Private Sub TauschePressureRessourcen()

        Dim tempTexture As ID3D11Texture2D
        Dim tempView As ID3D11ShaderResourceView
        Dim tempTargetView As ID3D11RenderTargetView


        tempTexture = sourcePressureTexture
        sourcePressureTexture = targetPressureTexture
        targetPressureTexture = tempTexture

        tempView = sourcePressureView
        sourcePressureView = targetPressureView
        targetPressureView = tempView

        tempTargetView = sourcePressureTargetView
        sourcePressureTargetView = targetPressureTargetView
        targetPressureTargetView = tempTargetView

    End Sub

    Private Sub BerechnePigmentTransport()

        AktualisierePigmentTransportConstantBuffer()


        ' ================================================================
        ' sourcePigmentSuspension + sourceVelocity
        '                  ↓
        '       targetPigmentSuspension
        ' ================================================================

        renderContext.OMSetRenderTargets(targetPigmentSuspensionTargetView)

        SetzeVollbildViewport()

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(pigmentTransportVertexShader)
        renderContext.PSSetShader(pigmentTransportPixelShader)

        renderContext.PSSetConstantBuffer(0UI, pigmentTransportConstantBuffer)
        renderContext.PSSetShaderResource(0UI, sourcePigmentSuspensionView)
        renderContext.PSSetShaderResource(1UI, sourceVelocityView)

        renderContext.Draw(3UI, 0UI)

        ' ================================================================
        ' Pipeline lösen
        ' ================================================================

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetShaderResource(1UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

        TauschePigmentSuspensionRessourcen()

    End Sub

    Private Sub TauschePigmentSuspensionRessourcen()

        Dim tempTexture As ID3D11Texture2D
        Dim tempView As ID3D11ShaderResourceView
        Dim tempTargetView As ID3D11RenderTargetView

        tempTexture = sourcePigmentSuspensionTexture
        sourcePigmentSuspensionTexture = targetPigmentSuspensionTexture
        targetPigmentSuspensionTexture = tempTexture

        tempView = sourcePigmentSuspensionView
        sourcePigmentSuspensionView = targetPigmentSuspensionView
        targetPigmentSuspensionView = tempView

        tempTargetView = sourcePigmentSuspensionTargetView
        sourcePigmentSuspensionTargetView = targetPigmentSuspensionTargetView
        targetPigmentSuspensionTargetView = tempTargetView

    End Sub

#End Region

#Region "Rendering Helpers"

    Private Sub SetzeVollbildViewport()

        renderContext.RSSetViewport(
    New Viewport(
        0.0F,
        0.0F,
        CSng(renderBreite),
        CSng(renderHoehe),
        0.0F,
        1.0F))

    End Sub

#End Region

#Region "Rendering"

    Friend Function RenderBild() As Bitmap

        Dim ergebnis As Bitmap

        ergebnis = Nothing

        PruefeRenderBereitschaft()

        InitialisiereBildzustand()
        InitialisierePigmentzustand()
        InitialisiereWasserzustand()

        SimuliereCurtisWasser(TEST_CURTIS_ITERATIONEN)

        InitialisiereKontrollansicht()

        ZeigeSimulationsergebnis()

        ergebnis = ErzeugeAusgabebild()

        Return ergebnis

    End Function

    Private Sub PruefeRenderBereitschaft()

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(D3DRenderer))
        End If

        If Not istInitialisiert Then
            Throw New InvalidOperationException("Der D3DRenderer wurde noch nicht initialisiert.")
        End If

    End Sub

    Private Sub InitialisiereBildzustand()

        BerechneKuwahara()
        BerechneRegionDistance()

    End Sub

    Private Sub BerechneKuwahara()

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

    End Sub

    Private Sub InitialisierePigmentzustand()

        InitialisierePigmentSuspension()

    End Sub

    Private Sub InitialisierePigmentSuspension()

        renderContext.OMSetRenderTargets(sourcePigmentSuspensionTargetView)
        renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(renderBreite), CSng(renderHoehe), 0.0F, 1.0F))

        renderContext.VSSetShader(pigmentInitializerVertexShader)
        renderContext.PSSetShader(pigmentInitializerPixelShader)

        renderContext.PSSetShaderResource(0UI, kuwaharaView)
        renderContext.PSSetSampler(0UI, renderSampler)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

    Private Sub InitialisiereWasserzustand()

        InitialisierePapier()

        InitialisierePressure()
        InitialisiereVelocity()

    End Sub

    Private Sub InitialisierePapier()

        renderContext.OMSetRenderTargets(paperTargetView)
        renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(renderBreite), CSng(renderHoehe), 0.0F, 1.0F))

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(paperInitializerVertexShader)
        renderContext.PSSetShader(paperInitializerPixelShader)

        renderContext.Draw(3UI, 0UI)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

    Private Sub InitialisierePressure()

        AktualisierePressureInitializerConstantBuffer()

        renderContext.OMSetRenderTargets(sourcePressureTargetView)

        SetzeVollbildViewport()

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(pressureInitializerVertexShader)
        renderContext.PSSetShader(pressureInitializerPixelShader)

        renderContext.PSSetConstantBuffer(0UI, pressureInitializerConstantBuffer)
        renderContext.PSSetShaderResource(0UI, regionDistanceView)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

    Private Sub InitialisiereVelocity()

        D3D11InteropHelper.ClearRenderTargetView(renderContext, sourceVelocityTargetView, 0.0F, 0.0F, 0.0F, 0.0F)

        D3D11InteropHelper.ClearRenderTargetView(renderContext, targetVelocityTargetView, 0.0F, 0.0F, 0.0F, 0.0F)

    End Sub

    Private Sub InitialisiereLegacyWasserzustand()

        renderContext.OMSetRenderTargets(sourceWaterTargetView)

        renderContext.RSSetViewport(
            New Viewport(
                0.0F,
                0.0F,
                CSng(renderBreite),
                CSng(renderHoehe),
                0.0F,
                1.0F))

        renderContext.VSSetShader(waterInitializerVertexShader)
        renderContext.PSSetShader(waterInitializerPixelShader)

        renderContext.PSSetShaderResource(0UI, kuwaharaView)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

    Private Sub InitialisiereKontrollansicht()

        D3D11InteropHelper.ClearRenderTargetView(renderContext, renderTargetView, 0.0F, 0.0F, 0.0F, 1.0F)

        ZeigeOriginal()
        ZeigeKuwahara()
        ZeigeKontrollmonitor()

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

    Private Sub ZeigeOriginal()

        Dim quadrantBreite As Integer
        Dim quadrantHoehe As Integer

        quadrantBreite = Math.Max(1, renderBreite \ 2)
        quadrantHoehe = Math.Max(1, renderHoehe \ 2)

        ' ============================================================
        ' Originalbild oben links
        ' ============================================================

        renderContext.OMSetRenderTargets(renderTargetView)

        renderContext.RSSetViewport(
        New Viewport(
            0.0F,
            0.0F,
            CSng(quadrantBreite),
            CSng(quadrantHoehe),
            0.0F,
            1.0F))

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(copyVertexShader)
        renderContext.PSSetShader(copyPixelShader)

        renderContext.PSSetShaderResource(0UI, sourceView)
        renderContext.PSSetSampler(0UI, renderSampler)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)

    End Sub

    Private Sub ZeigeKuwahara()

        Dim quadrantBreite As Integer
        Dim quadrantHoehe As Integer

        quadrantBreite = Math.Max(1, renderBreite \ 2)
        quadrantHoehe = Math.Max(1, renderHoehe \ 2)

        ' ============================================================
        ' Kuwahara unten links
        ' ============================================================

        renderContext.OMSetRenderTargets(renderTargetView)

        renderContext.RSSetViewport(
        New Viewport(
            0.0F,
            CSng(quadrantHoehe),
            CSng(quadrantBreite),
            CSng(quadrantHoehe),
            0.0F,
            1.0F))

        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(copyVertexShader)
        renderContext.PSSetShader(copyPixelShader)

        renderContext.PSSetShaderResource(0UI, kuwaharaView)
        renderContext.PSSetSampler(0UI, renderSampler)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)

    End Sub

    Private Sub ZeigeKontrollmonitor()

        Dim quadrantBreite As Integer
        Dim quadrantHoehe As Integer

        quadrantBreite = Math.Max(1, renderBreite \ 2)
        quadrantHoehe = Math.Max(1, renderHoehe \ 2)


        AktualisierePressureFlowConstantBuffer(PRESSURE_FLOW_MODE_DISPLAY)

        renderContext.OMSetRenderTargets(renderTargetView)
        renderContext.RSSetViewport(New Viewport(CSng(quadrantBreite), 0.0F, CSng(quadrantBreite),
                                                 CSng(quadrantHoehe), 0.0F, 1.0F))
        renderContext.IASetPrimitiveTopology(PrimitiveTopology.TriangleList)
        renderContext.OMSetBlendState(Nothing)

        renderContext.VSSetShader(pressureFlowVertexShader)
        renderContext.PSSetShader(pressureFlowPixelShader)

        renderContext.PSSetConstantBuffer(0UI, pressureFlowConstantBuffer)
        renderContext.PSSetShaderResource(0UI, sourcePressureView)

        renderContext.Draw(3UI, 0UI)


        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

    Private Sub ZeigeSimulationsergebnis()

        Dim quadrantBreite As Integer
        Dim quadrantHoehe As Integer

        quadrantBreite = Math.Max(1, renderBreite \ 2)
        quadrantHoehe = Math.Max(1, renderHoehe \ 2)

        ' ============================================================
        ' Simulationsergebnis unten rechts
        '
        ' sourcePigmentSuspensionView enthält:
        '
        ' RGB = premultiplizierte Pigmentfarbe
        ' A   = Pigmentmenge
        '
        ' Der PigmentDisplayShader erzeugt daraus die sichtbare,
        ' vollständig opake Darstellung.
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

        ' Pipeline sauber verlassen.

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetConstantBuffer(0UI, Nothing)

        D3D11InteropHelper.UnbindRenderTarget(renderContext)

    End Sub

    Private Function ErzeugeAusgabebild() As Bitmap

        Dim ergebnis As Bitmap

        ergebnis = Nothing

        renderContext.CopyResource(stagingTexture, renderTargetTexture)

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

#Region "Shader Helperfunktionen"

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

    Private Function InitialisiereShaderHelper(Of T As Class)(shaderFilename As String, shaderSuffix As String,
                                                              shaderTypName As String,
                                                              shaderErzeugen As Func(Of Byte(), T)) As T

        Dim shaderCode() As Byte
        Dim shader As T

        shaderCode = LadeShaderBytecode(shaderFilename & shaderSuffix & ".cso")

        shader = shaderErzeugen(shaderCode)

        If shader Is Nothing Then

            Throw New InvalidOperationException("Der " & shaderFilename & "-" & shaderTypName &
                                                " konnte nicht erzeugt werden.")

        End If

        Return shader

    End Function

    Private Sub InitialisiereVertexPixelShader(shaderFilename As String, ByRef vertexShader As ID3D11VertexShader,
                                               ByRef pixelShader As ID3D11PixelShader)

        vertexShader = InitialisiereShaderHelper(shaderFilename, "VS", "VertexShader",
            Function(shaderCode)
                Return renderDevice.CreateVertexShader(shaderCode)
            End Function)


        pixelShader = InitialisiereShaderHelper(shaderFilename, "PS", "PixelShader",
            Function(shaderCode)
                Return renderDevice.CreatePixelShader(shaderCode)
            End Function)

    End Sub

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

        Direct3DRessourceHandler.GebeFrei(regionDistanceChangedCounterView)
        Direct3DRessourceHandler.GebeFrei(regionDistanceChangedCounterStagingBuffer)
        Direct3DRessourceHandler.GebeFrei(regionDistanceChangedCounterBuffer)

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

        Direct3DRessourceHandler.GebeFrei(paperInitializerPixelShader)
        Direct3DRessourceHandler.GebeFrei(paperInitializerVertexShader)

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

        Direct3DRessourceHandler.GebeFrei(regionDistanceConstantBuffer)
        Direct3DRessourceHandler.GebeFrei(regionDistancePixelShader)
        Direct3DRessourceHandler.GebeFrei(regionDistanceVertexShader)

        Direct3DRessourceHandler.GebeFrei(pressureInitializerConstantBuffer)
        Direct3DRessourceHandler.GebeFrei(pressureInitializerPixelShader)
        Direct3DRessourceHandler.GebeFrei(pressureInitializerVertexShader)

        Direct3DRessourceHandler.GebeFrei(velocityConstantBuffer)
        Direct3DRessourceHandler.GebeFrei(velocityPixelShader)
        Direct3DRessourceHandler.GebeFrei(velocityVertexShader)

        Direct3DRessourceHandler.GebeFrei(pressureFlowConstantBuffer)
        Direct3DRessourceHandler.GebeFrei(pressureFlowPixelShader)
        Direct3DRessourceHandler.GebeFrei(pressureFlowVertexShader)

        Direct3DRessourceHandler.GebeFrei(pigmentTransportConstantBuffer)
        Direct3DRessourceHandler.GebeFrei(pigmentTransportPixelShader)
        Direct3DRessourceHandler.GebeFrei(pigmentTransportVertexShader)

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