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

Friend Class D3DRenderer
    Implements IDisposable

#Region "Variablendeklaration"

#Region "Variablen"

    ' Persistentes D3D11 Device
    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext
    Private renderFeatureLevel As FeatureLevel

    Private d3dImage As D3D11Image

    'Renderer
    Private gradientRenderer As GradientRenderer
    Private partikelRenderer As PartikelRendererAPC
    Private revealRenderer As RevealRenderer

    'Maske
    Private maskTexture As ID3D11Texture2D
    Private maskView As ID3D11ShaderResourceView

    'Sampler
    Private renderSampler As ID3D11SamplerState

    'Dimensionen
    Private renderBreite As Integer
    Private renderHoehe As Integer

    ' Lifecycle
    Private wurdeBereinigt As Boolean
    Private istInitialisiert As Boolean

#End Region

#Region "Delegaten"

    Private Delegate Sub ClearRenderTargetViewDelegate(renderTargetView As ID3D11RenderTargetView, ByRef color As Color4)

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

    Public Sub Initialisiere(parameter As RenderParameter)

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(D3DRenderer))
        End If

        If parameter Is Nothing Then
            Throw New ArgumentNullException(NameOf(parameter))
        End If

        If parameter.breite <= 0 OrElse parameter.hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(parameter.breite))
        End If

        If parameter.gradientAktiv AndAlso parameter.gradientBitmap Is Nothing Then
            Throw New ArgumentNullException(NameOf(parameter.gradientBitmap))
        End If

        If parameter.partikelAktiv AndAlso parameter.particleGradientBitmap Is Nothing Then
            Throw New ArgumentNullException(NameOf(parameter.particleGradientBitmap))
        End If

        LogHandling.LogDebug("D3D: Initialisiere() beginnt.")

        renderBreite = parameter.breite
        renderHoehe = parameter.hoehe

        InitialisiereDirect3D()
        InitialisiereBrandMaskenTextur(parameter.brandMaske)
        InitialisiereSampler()

        revealRenderer = New RevealRenderer()

        revealRenderer.Initialisiere(renderDevice, renderContext, parameter)

        If parameter.gradientAktiv Then

            gradientRenderer = New GradientRenderer()

            gradientRenderer.Initialisiere(renderDevice, renderContext, parameter.gradientBitmap,
                                           parameter.brandkantenBreite)

        End If

        If parameter.partikelAktiv Then

            partikelRenderer = New PartikelRendererAPC()

            partikelRenderer.Initialisiere(renderDevice, renderContext, renderFeatureLevel, parameter.breite,
                                           parameter.hoehe, parameter.particleGradientBitmap,
                                           parameter.schwerkraftAktiv, parameter.partikelLebensdauer)

        End If

        If gradientRenderer IsNot Nothing Then
            gradientRenderer.AktualisiereParameter(0.0F)
        End If

        d3dImage = New D3D11Image()
        d3dImage.WindowOwner = Direct3DRessourceHandler.ErmittleInteropFensterHandle()
        d3dImage.OnRender = AddressOf RenderSurface
        d3dImage.SetPixelSize(parameter.breite, parameter.hoehe)

        istInitialisiert = True

        '###############################################
        '# Debugging                                   #
        '#                                             #
        '# Kleiner Testaufruf von BlitzTexturGenerator #
        '#                                             #
        '# Für die geplante V 2.0                      #
        '#                                             #
        '# Inline-Dims sind bewusst für diese Ausnahme #
        '# gesetzt.                                    #
        '###############################################

        'Dim generator As BlitzTexturGenerator
        'Dim blitz As BlitzTexturDaten

        'generator = New BlitzTexturGenerator()

        'blitz = generator.ErzeugeBlitzTextur(512, 512, 12345)

        'blitz.Bitmap.Save("BlitzTest.png", System.Drawing.Imaging.ImageFormat.Png)

        LogHandling.LogDebug("D3D: Initialisiere() beendet.")

    End Sub

    Private Sub InitialisiereBrandMaskenTextur(brandMaske As BitmapSource)

        maskTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, brandMaske)

        maskView = renderDevice.CreateShaderResourceView(maskTexture)

        LogHandling.LogDebug("D3D: Brandmaskentextur und ShaderResourceView erzeugt.")

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

        renderSampler = renderDevice.CreateSamplerState(samplerDescription)

    End Sub

#End Region

#Region "Rendering"

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

        revealRenderer.AktualisiereParameter(progress, deltaTime)

        If gradientRenderer IsNot Nothing Then

            gradientRenderer.AktualisiereParameter(progress)

        End If

        If partikelRenderer IsNot Nothing AndAlso deltaTime > 0.0F Then

            partikelRenderer.Simuliere(deltaTime, progress, maskView)

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

        If maskView Is Nothing Then
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

            revealRenderer.Render(maskView, renderSampler)

            '---------------------------------
            ' Pass 2: Gradient
            '---------------------------------

            If gradientRenderer IsNot Nothing Then

                gradientRenderer.Render(maskView, renderSampler)

            End If

            '---------------------------------
            ' Pass 3: Partikel
            '---------------------------------

            If partikelRenderer IsNot Nothing Then

                partikelRenderer.Render(renderSampler)

            End If

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

    Friend Shared Function LadeShaderBytecode(dateiname As String) As Byte()

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

    Friend Function GibAPCAnzahlZurueck() As Integer

        If partikelRenderer Is Nothing Then
            Return -1
        End If

        Return partikelRenderer.GibAPCAnzahlZurueck()

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
        ' Sampler
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(renderSampler)

        '---------------------------------
        ' Shader Resource Views
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(maskView)

        '---------------------------------
        ' Texturen
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(maskTexture)

        '---------------------------------
        ' GradientRenderer
        '---------------------------------
        If gradientRenderer IsNot Nothing Then

            gradientRenderer.Dispose()
            gradientRenderer = Nothing

        End If

        '---------------------------------
        ' PartikelRenderer
        '---------------------------------
        If partikelRenderer IsNot Nothing Then

            partikelRenderer.Dispose()
            partikelRenderer = Nothing

        End If

        '---------------------------------
        ' RevealRenderer
        '---------------------------------
        If revealRenderer IsNot Nothing Then

            revealRenderer.Dispose()
            revealRenderer = Nothing

        End If

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