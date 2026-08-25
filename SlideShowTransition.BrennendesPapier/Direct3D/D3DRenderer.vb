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

    '---------------------------------
    ' WPF / D3D11Image Shared Surface
    '---------------------------------

    Private sharedTexture As ID3D11Texture2D

    '---------------------------------
    ' Privater GPU-Backbuffer
    '---------------------------------

    Private backBufferTexture As ID3D11Texture2D
    Private backBufferRenderTargetView As ID3D11RenderTargetView

    '---------------------------------
    ' Render-Synchronisation
    '---------------------------------

    Private letzterSurfacePointer As IntPtr
    Private renderAnforderungOffen As Integer

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

    Friend ReadOnly Property KannNaechstenFrameRendern As Boolean

        Get

            Return Threading.Volatile.Read(renderAnforderungOffen) = 0

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

        ' Der Frame wird jetzt für die Renderpipeline reserviert.
        '
        ' Sollte zwischen dem Check in TransitionMain und diesem
        ' Aufruf doch noch eine Renderanforderung offen sein,
        ' wird sicherheitshalber nichts verändert.

        If Threading.Interlocked.CompareExchange(renderAnforderungOffen, 1, 0) <> 0 Then

            Exit Sub

        End If

        Try

            '---------------------------------
            ' Framezustand aktualisieren
            '---------------------------------

            revealRenderer.AktualisiereParameter(progress, deltaTime)

            If gradientRenderer IsNot Nothing Then

                gradientRenderer.AktualisiereParameter(progress)

            End If

            If partikelRenderer IsNot Nothing AndAlso deltaTime > 0.0F Then

                partikelRenderer.Simuliere(deltaTime, progress, maskView)

            End If

            '---------------------------------
            ' Rendering anfordern
            '---------------------------------

            d3dImage.RequestRender()

        Catch

            ' Falls RequestRender() selbst scheitert,
            ' darf die Pipeline nicht dauerhaft gesperrt bleiben.


            Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

            Throw

        End Try

    End Sub

    Private Sub AktualisiereRenderSurface(surfacePointer As IntPtr)

        Dim surface As IDXGISurface
        Dim dxgiResource As IDXGIResource

        Dim neueSharedTexture As ID3D11Texture2D

        Dim neueBackBufferTexture As ID3D11Texture2D
        Dim neueBackBufferRenderTargetView As ID3D11RenderTargetView

        Dim sharedDescription As Texture2DDescription
        Dim backBufferDescription As Texture2DDescription

        Dim sharedHandle As IntPtr

        surface = Nothing
        dxgiResource = Nothing

        neueSharedTexture = Nothing

        neueBackBufferTexture = Nothing
        neueBackBufferRenderTargetView = Nothing

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

                Throw New InvalidOperationException("Die DXGI-Resource besitzt keinen gültigen SharedHandle.")

            End If

            neueSharedTexture = renderDevice.OpenSharedResource(Of ID3D11Texture2D)(sharedHandle)

            If neueSharedTexture Is Nothing Then

                Throw New InvalidOperationException("Die SharedTexture konnte nicht geöffnet werden.")

            End If

            '---------------------------------
            ' Privaten Backbuffer erzeugen
            '---------------------------------
            '
            ' Der Backbuffer übernimmt das Layout der
            ' tatsächlichen WPF-SharedTexture.
            '
            ' Damit sind beide Ressourcen für den späteren
            ' CopyResource-Aufruf strukturell kompatibel.
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
            ' Alten Ressourcensatz ersetzen
            '---------------------------------
            '
            ' Erst wenn der neue Satz vollständig erzeugt wurde,
            ' wird der bisherige freigegeben.

            Direct3DRessourceHandler.GebeFrei(backBufferRenderTargetView)
            Direct3DRessourceHandler.GebeFrei(backBufferTexture)
            Direct3DRessourceHandler.GebeFrei(sharedTexture)

            sharedTexture = neueSharedTexture
            backBufferTexture = neueBackBufferTexture
            backBufferRenderTargetView = neueBackBufferRenderTargetView
            letzterSurfacePointer = surfacePointer

            ' Besitz an die Member übertragen.

            neueSharedTexture = Nothing
            neueBackBufferTexture = Nothing
            neueBackBufferRenderTargetView = Nothing

        Finally

            Direct3DRessourceHandler.GebeFrei(neueBackBufferRenderTargetView)
            Direct3DRessourceHandler.GebeFrei(neueBackBufferTexture)
            Direct3DRessourceHandler.GebeFrei(neueSharedTexture)
            Direct3DRessourceHandler.GebeFrei(dxgiResource)

            ' WICHTIG:
            '
            ' IDXGISurface stammt aus dem von Tqk gelieferten
            ' SurfacePointer.
            '
            ' Wie bei VWV bereits empirisch bestätigt:
            ' NICHT über GebeFrei()/Dispose freigeben.

            surface = Nothing

        End Try

    End Sub

    Private Sub RenderSurface(surfacePointer As IntPtr, isNewSurface As Boolean)

        If surfacePointer = IntPtr.Zero Then

            Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

            Exit Sub

        End If

        If maskView Is Nothing Then

            Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

            Exit Sub

        End If

        Try

            '---------------------------------
            ' SharedTexture / Backbuffer
            '---------------------------------

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
            ' Der komplette Frame entsteht ausschließlich
            ' im privaten Backbuffer.
            '
            ' WPF besitzt keinerlei Zugriff auf diese Texture
            ' und kann deshalb keinen teilweise aufgebauten
            ' Frame mehr präsentieren.

            renderContext.OMSetRenderTargets(backBufferRenderTargetView)
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
            ' Pass 3: APC-Partikel
            '---------------------------------

            If partikelRenderer IsNot Nothing Then

                partikelRenderer.Render(renderSampler)

            End If

            '---------------------------------
            ' Privaten Frame abschließen
            '---------------------------------

            D3D11InteropHelper.UnbindRenderTarget(renderContext)

            '---------------------------------
            ' Fertigen Frame veröffentlichen
            '---------------------------------
            '
            ' Die SharedTexture wurde während Reveal,
            ' Gradient und Partikelpass überhaupt nicht
            ' verändert.
            '
            ' Erst jetzt wird das vollständig aufgebaute
            ' Kunstwerk in einem einzigen CopyResource-
            ' Schritt veröffentlicht.
            '

            renderContext.CopyResource(sharedTexture, backBufferTexture)


            ' Für die D3D11Image-Interop weiterhin erforderlich.
            '
            ' Finger weg. Wir wissen inzwischen, was passiert,
            ' wenn man Flush() flushen möchte. ;-)

            renderContext.Flush()

        Catch ex As Exception

            LogHandling.LogError("D3D: Fehler beim Rendern: " & ex.ToString())

        Finally

            '---------------------------------
            ' Backpressure freigeben
            '---------------------------------
            '
            ' Erst nachdem der vollständige Frame in die
            ' SharedTexture übertragen wurde, darf der nächste
            ' Simulations-/Renderzustand erzeugt werden.

            Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

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

#End Region

#Region "Bereinigung & Dispose"

    Private Sub BeendeUndBereinigeRenderer()

        istInitialisiert = False

        Threading.Interlocked.Exchange(renderAnforderungOffen, 0)

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

        '---------------------------------
        ' Privater Backbuffer
        '---------------------------------

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