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

    Private mosaikRenderer As MosaikRenderer

    Private renderSampler As ID3D11SamplerState

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

#End Region

#Region "Initialisierung"

    Friend Sub Initialisiere(breite As Integer, hoehe As Integer, partikel() As PartikelDaten,
                             altesBild As BitmapSource, neuesBild As BitmapSource, flowField As FlowFieldDaten)

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

        renderBreite = breite
        renderHoehe = hoehe

        Try

            InitialisiereDirect3D()
            InitialisiereSampler()

            mosaikRenderer = New MosaikRenderer()

            mosaikRenderer.Initialisiere(renderDevice, renderContext, renderBreite, renderHoehe, partikel,
                                         altesBild, neuesBild, flowField)

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

    Friend Function RenderFrame(deltaTime As Single, pruefeTransitionsende As Boolean) As Integer

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

        mosaikRenderer.Simuliere(deltaTime)


        ' Erst den neuen Zustand rendern.
        ' Wenn das letzte Partikel gerade gestorben ist,
        ' besteht dieser Frame bereits ausschließlich
        ' aus dem neuen Hintergrundbild.
        '
        d3dImage.RequestRender()

        If Not pruefeTransitionsende Then
            Return -1
        End If

        anzahlLebendePartikel = mosaikRenderer.GibAnzahlLebendePartikelZurueck()

        Return anzahlLebendePartikel

    End Function

    Private Sub RenderSurface(surfacePointer As IntPtr, isNewSurface As Boolean)

        Dim surface As IDXGISurface
        Dim dxgiResource As IDXGIResource
        Dim sharedTexture As ID3D11Texture2D
        Dim renderTargetView As ID3D11RenderTargetView
        Dim sharedHandle As IntPtr

        surface = Nothing
        dxgiResource = Nothing
        sharedTexture = Nothing
        renderTargetView = Nothing
        sharedHandle = IntPtr.Zero

        If surfacePointer = IntPtr.Zero Then
            Exit Sub
        End If

        Try

            surface = New IDXGISurface(surfacePointer)

            dxgiResource = surface.QueryInterface(Of IDXGIResource)()

            sharedHandle = dxgiResource.SharedHandle

            If sharedHandle = IntPtr.Zero Then

                Throw New InvalidOperationException("Die DXGI-Resource besitzt keinen gültigen SharedHandle.")

            End If

            sharedTexture = renderDevice.OpenSharedResource(Of ID3D11Texture2D)(sharedHandle)

            renderTargetView = renderDevice.CreateRenderTargetView(sharedTexture)

            renderContext.OMSetRenderTargets(renderTargetView)

            renderContext.RSSetViewport(
                New Viewport(
                    0.0F,
                    0.0F,
                    CSng(renderBreite),
                    CSng(renderHoehe),
                    0.0F,
                    1.0F))

            mosaikRenderer.Render(renderSampler)

            D3D11InteropHelper.UnbindRenderTarget(renderContext)

            renderContext.Flush()

        Finally

            Direct3DRessourceHandler.GebeFrei(renderTargetView)
            Direct3DRessourceHandler.GebeFrei(sharedTexture)
            Direct3DRessourceHandler.GebeFrei(dxgiResource)
            'Direct3DRessourceHandler.GebeFrei(surface)

            renderTargetView = Nothing
            sharedTexture = Nothing
            dxgiResource = Nothing
            surface = Nothing

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