Imports System.Reflection
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports SharpGen.Runtime
Imports SlideShowDirect3DInterop
Imports TqkLibrary.Wpf.Interop.DirectX
Imports Vortice.Direct3D
Imports Vortice.Direct3D11
Imports Vortice.DXGI
Imports Vortice.Mathematics

Friend Class D3DRenderer
    Implements IDisposable

#Region "Variablendeklaration"

    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext
    Private renderFeatureLevel As FeatureLevel

    Private d3dImage As D3D11Image

    Private mosaikRenderer As MosaikRendererAPC

    Private renderSampler As ID3D11SamplerState

    '---------------------------------
    ' WPF / D3D11Image Shared Surface
    '---------------------------------

    Private sharedTexture As ID3D11Texture2D

    '---------------------------------
    ' Privater GPU-Backbuffer
    '---------------------------------

    Private backBufferTexture As ID3D11Texture2D
    Private backBufferRenderTargetView As ID3D11RenderTargetView

    Private depthTexture As ID3D11Texture2D
    Private depthStencilView As ID3D11DepthStencilView

    Private letzterSurfacePointer As IntPtr
    Private renderAnforderungOffen As Integer

    Private renderBreite As Integer
    Private renderHoehe As Integer

    Private istInitialisiert As Boolean
    Private wurdeBereinigt As Boolean

#End Region

#Region "Eigenschaften"

    Public ReadOnly Property FrameImage As ImageSource

        Get
            Return d3dImage
        End Get

    End Property

    Friend ReadOnly Property KannNaechstenFrameRendern As Boolean

        Get

            Return Threading.Volatile.Read(renderAnforderungOffen) = 0

        End Get

    End Property

#End Region

#Region "Initialisierung"

    Friend Sub Initialisiere(breite As Integer, hoehe As Integer, partikel() As PartikelDaten,
                             altesBild As BitmapSource, neuesBild As BitmapSource, flowField As FlowFieldDaten,
                             randAbloeseFeld As RandAbloeseFeldDaten)

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(D3DRenderer))
        End If

        If breite <= 0 OrElse hoehe <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(breite))
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

        If flowField Is Nothing Then
            Throw New ArgumentNullException(NameOf(flowField))
        End If

        If randAbloeseFeld Is Nothing Then
            Throw New ArgumentNullException(NameOf(randAbloeseFeld))
        End If

        renderBreite = breite
        renderHoehe = hoehe

        Try

            InitialisiereDirect3D()
            InitialisiereSampler()

            mosaikRenderer = New MosaikRendererAPC()

            mosaikRenderer.Initialisiere(renderDevice, renderContext, renderBreite, renderHoehe, partikel,
                                         altesBild, neuesBild, flowField, randAbloeseFeld)

            d3dImage = New D3D11Image()
            d3dImage.WindowOwner = Direct3DRessourceHandler.ErmittleInteropFensterHandle()
            d3dImage.OnRender = AddressOf RenderSurface
            d3dImage.SetPixelSize(renderBreite, renderHoehe)

            istInitialisiert = True

        Catch

            BeendeUndBereinigeRenderer()

            Throw

        End Try

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

            Throw New InvalidOperationException("Das Direct3D11-Device konnte nicht erzeugt werden. HRESULT: " &
                                                result.Code.ToString())

        End If

        renderFeatureLevel = featureLevel

    End Sub

    Private Sub InitialisiereSampler()

        Dim description As SamplerDescription

        description = New SamplerDescription()
        description.Filter = Filter.MinMagMipLinear
        description.AddressU = TextureAddressMode.Clamp
        description.AddressV = TextureAddressMode.Clamp
        description.AddressW = TextureAddressMode.Clamp
        description.MinLOD = 0.0F
        description.MaxLOD = Single.MaxValue

        renderSampler = renderDevice.CreateSamplerState(description)

    End Sub

#End Region

#Region "Rendering"

    Friend Sub AktualisiereFlowField(neuesFlowField As FlowFieldDaten)

        If wurdeBereinigt Then
            Exit Sub
        End If

        If Not istInitialisiert Then
            Exit Sub
        End If

        If mosaikRenderer Is Nothing Then
            Exit Sub
        End If

        If neuesFlowField Is Nothing Then
            Throw New ArgumentNullException(NameOf(neuesFlowField))
        End If

        mosaikRenderer.AktualisiereFlowField(neuesFlowField)

    End Sub

    Friend Function RenderFrame(deltaTime As Single, abloeseProgress As Single, gravitation As Single,
                                pruefeTransitionsende As Boolean) As Integer

        Dim anzahlLebendePartikel As Integer

        If wurdeBereinigt Then
            Return -1
        End If

        If Not istInitialisiert Then
            Return -1
        End If

        If d3dImage Is Nothing Then
            Return -1
        End If

        If Threading.Interlocked.CompareExchange(renderAnforderungOffen, 1, 0) <> 0 Then
            Return -1
        End If

        mosaikRenderer.Simuliere(deltaTime, abloeseProgress, gravitation)


        ' Erst den neuen Zustand rendern.
        ' Wenn das letzte Partikel gerade gestorben ist,
        ' besteht dieser Frame bereits ausschließlich
        ' aus dem neuen Hintergrundbild.
        '
        Try

            d3dImage.RequestRender()

        Catch

            Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

            Throw

        End Try

        If Not pruefeTransitionsende Then
            Return -1
        End If

        anzahlLebendePartikel = mosaikRenderer.GibAnzahlLebendePartikelZurueck()

        Return anzahlLebendePartikel

    End Function

    Private Sub AktualisiereRenderSurface(surfacePointer As IntPtr)

        Dim surface As IDXGISurface
        Dim dxgiResource As IDXGIResource

        Dim neueSharedTexture As ID3D11Texture2D

        Dim neueBackBufferTexture As ID3D11Texture2D
        Dim neueBackBufferRenderTargetView As ID3D11RenderTargetView

        Dim sharedDescription As Texture2DDescription
        Dim backBufferDescription As Texture2DDescription

        Dim neueDepthTexture As ID3D11Texture2D
        Dim neueDepthStencilView As ID3D11DepthStencilView

        Dim depthDescription As Texture2DDescription

        Dim sharedHandle As IntPtr

        surface = Nothing
        dxgiResource = Nothing

        neueSharedTexture = Nothing

        neueBackBufferTexture = Nothing
        neueBackBufferRenderTargetView = Nothing

        neueDepthTexture = Nothing
        neueDepthStencilView = Nothing

        '---------------------------------
        ' Privaten Depthbuffer erzeugen
        '---------------------------------

        depthDescription = New Texture2DDescription(
            Format.D32_Float,
            sharedDescription.Width,
            sharedDescription.Height,
            1UI,
            1UI,
            BindFlags.DepthStencil,
            ResourceUsage.Default,
            CpuAccessFlags.None,
            sharedDescription.SampleDescription.Count,
            sharedDescription.SampleDescription.Quality,
            ResourceOptionFlags.None)

        neueDepthTexture = renderDevice.CreateTexture2D(depthDescription)

        If neueDepthTexture Is Nothing Then

            Throw New InvalidOperationException("Der private Depthbuffer konnte nicht erzeugt werden.")

        End If

        neueDepthStencilView = renderDevice.CreateDepthStencilView(neueDepthTexture)

        If neueDepthStencilView Is Nothing Then

            Throw New InvalidOperationException("Die DepthStencilView konnte nicht erzeugt werden.")

        End If

        sharedHandle = IntPtr.Zero

        If surfacePointer = IntPtr.Zero Then

            Throw New ArgumentException("Der SurfacePointer ist ungültig.", NameOf(surfacePointer))

        End If

        Try

            '---------------------------------
            ' WPF-SharedTexture öffnen
            '---------------------------------

            surface = New IDXGISurface(surfacePointer)

            dxgiResource = surface.QueryInterface(Of IDXGIResource)()

            sharedHandle = dxgiResource.SharedHandle

            If sharedHandle = IntPtr.Zero Then

                Throw New InvalidOperationException(
                "Die DXGI-Resource besitzt keinen gültigen SharedHandle.")

            End If

            neueSharedTexture = renderDevice.OpenSharedResource(Of ID3D11Texture2D)(sharedHandle)

            If neueSharedTexture Is Nothing Then

                Throw New InvalidOperationException(
                "Die SharedTexture konnte nicht geöffnet werden.")

            End If

            '---------------------------------
            ' Privaten Backbuffer erzeugen
            '---------------------------------
            '
            ' Format, Größe und Multisampling werden von der
            ' tatsächlichen WPF-SharedTexture übernommen.
            '
            ' Dadurch ist CopyResource später garantiert zwischen
            ' kompatiblen Texturen unterwegs.
            '

            sharedDescription = neueSharedTexture.Description

            backBufferDescription =
                New Texture2DDescription(
                    sharedDescription.Format,
                    sharedDescription.Width,
                    sharedDescription.Height,
                    sharedDescription.MipLevels,
                    sharedDescription.ArraySize,
                    BindFlags.RenderTarget,
                    ResourceUsage.Default,
                    CpuAccessFlags.None,
                    sharedDescription.SampleDescription.Count,
                    sharedDescription.SampleDescription.Quality,
                    ResourceOptionFlags.None)

            neueBackBufferTexture = renderDevice.CreateTexture2D(backBufferDescription)

            If neueBackBufferTexture Is Nothing Then

                Throw New InvalidOperationException("Der private Backbuffer konnte nicht erzeugt werden.")

            End If

            neueBackBufferRenderTargetView = renderDevice.CreateRenderTargetView(neueBackBufferTexture)

            If neueBackBufferRenderTargetView Is Nothing Then

                Throw New InvalidOperationException(
                "Die RenderTargetView des privaten Backbuffers konnte nicht erzeugt werden.")

            End If

            '---------------------------------
            ' Alte Ressourcen ersetzen
            '---------------------------------
            '
            ' Erst nachdem sämtliche neuen Ressourcen erfolgreich
            ' erzeugt wurden, geben wir den bisherigen Satz frei.
            '
            Direct3DRessourceHandler.GebeFrei(depthStencilView)
            Direct3DRessourceHandler.GebeFrei(depthTexture)
            Direct3DRessourceHandler.GebeFrei(backBufferRenderTargetView)
            Direct3DRessourceHandler.GebeFrei(backBufferTexture)
            Direct3DRessourceHandler.GebeFrei(sharedTexture)

            sharedTexture = neueSharedTexture
            backBufferTexture = neueBackBufferTexture
            backBufferRenderTargetView = neueBackBufferRenderTargetView
            letzterSurfacePointer = surfacePointer
            depthTexture = neueDepthTexture
            depthStencilView = neueDepthStencilView

            ' Besitz ist jetzt an die Member übergegangen.
            '

            neueDepthTexture = Nothing
            neueDepthStencilView = Nothing

            neueSharedTexture = Nothing

            neueBackBufferTexture = Nothing
            neueBackBufferRenderTargetView = Nothing

        Finally

            Direct3DRessourceHandler.GebeFrei(neueDepthStencilView)
            Direct3DRessourceHandler.GebeFrei(neueDepthTexture)
            Direct3DRessourceHandler.GebeFrei(neueBackBufferRenderTargetView)
            Direct3DRessourceHandler.GebeFrei(neueBackBufferTexture)
            Direct3DRessourceHandler.GebeFrei(neueSharedTexture)
            Direct3DRessourceHandler.GebeFrei(dxgiResource)

            ' Wie bereits erfolgreich getestet:
            ' surface selbst NICHT über GebeFrei()/Dispose freigeben.

            surface = Nothing

        End Try

    End Sub

    Private Sub RenderSurface(surfacePointer As IntPtr, isNewSurface As Boolean)

        If surfacePointer = IntPtr.Zero Then

            Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

            Exit Sub

        End If

        Try

            If isNewSurface OrElse
                   sharedTexture Is Nothing OrElse
                   backBufferTexture Is Nothing OrElse
                   backBufferRenderTargetView Is Nothing OrElse
                   letzterSurfacePointer <> surfacePointer Then

                AktualisiereRenderSurface(surfacePointer)

            End If

            '---------------------------------
            ' STILLES KÄMMERLEIN
            '---------------------------------
            '
            ' Der komplette Frame wird ausschließlich in den
            ' privaten Backbuffer gerendert.
            '
            ' WPF kann diese Texture niemals sehen.

            renderContext.OMSetRenderTargets(backBufferRenderTargetView, depthStencilView)
            renderContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0F, 0)
            renderContext.RSSetViewport(New Viewport(0.0F, 0.0F, CSng(renderBreite), CSng(renderHoehe), 0.0F, 1.0F))

            ' Innerhalb dieses Aufrufs:
            '
            ' 1. NeuesBild
            ' 2. sämtliche sichtbaren APC-Partikel
            '
            ' Erst nach der Rückkehr ist unser Kunstwerk fertig.

            mosaikRenderer.Render(renderSampler)

            D3D11InteropHelper.UnbindRenderTarget(renderContext)

            '---------------------------------
            ' FERTIGEN FRAME VERÖFFENTLICHEN
            '---------------------------------
            '
            ' CopyResource ist in derselben Immediate-Context-
            ' Command Queue hinter allen vorherigen Drawcalls.
            '
            ' Die SharedTexture wird während des eigentlichen
            ' Frameaufbaus überhaupt nicht beschrieben.
            '
            ' Erst nachdem Hintergrund- und Partikelpass vollständig
            ' in die private BackbufferTexture eingereiht wurden,
            ' wird der fertige Frame als ein zusammenhängender
            ' CopyResource-Schritt in die SharedTexture übertragen.

            renderContext.CopyResource(sharedTexture, backBufferTexture)

            ' Für unsere D3D11Image-Interop weiterhin erforderlich.
            '
            ' Der Test ohne Flush hat eindeutig gezeigt, dass WPF
            ' andernfalls nicht zuverlässig den fertigen Frame sieht.

            renderContext.Flush()

        Finally

            ' Erst nachdem der vollständige Frame in die
            ' SharedTexture übertragen wurde, darf der nächste
            ' Simulations-/Renderframe beginnen.

            Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

        End Try

    End Sub

#End Region

#Region "Shader"

    Friend Shared Function LadeShaderBytecode(dateiname As String) As Byte()

        Dim assembly As Assembly
        Dim ressourcenNamen() As String
        Dim ressourcenName As String
        Dim stream As IO.Stream
        Dim daten() As Byte

        assembly = Assembly.GetExecutingAssembly()

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

        Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

        If d3dImage IsNot Nothing Then

            d3dImage.OnRender = Nothing

            d3dImage.Dispose()
            d3dImage = Nothing

        End If

        If renderContext IsNot Nothing Then

            renderContext.ClearState()
            renderContext.Flush()

        End If

        If mosaikRenderer IsNot Nothing Then

            mosaikRenderer.Dispose()
            mosaikRenderer = Nothing

        End If

        '---------------------------------
        ' Privater Backbuffer
        '---------------------------------
        Direct3DRessourceHandler.GebeFrei(depthStencilView)
        Direct3DRessourceHandler.GebeFrei(depthTexture)

        depthStencilView = Nothing
        depthTexture = Nothing

        Direct3DRessourceHandler.GebeFrei(backBufferRenderTargetView)
        Direct3DRessourceHandler.GebeFrei(backBufferTexture)

        backBufferRenderTargetView = Nothing
        backBufferTexture = Nothing

        '---------------------------------
        ' WPF SharedTexture
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(sharedTexture)

        sharedTexture = Nothing

        letzterSurfacePointer = IntPtr.Zero

        Direct3DRessourceHandler.GebeFrei(renderSampler)

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

        renderBreite = 0
        renderHoehe = 0

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