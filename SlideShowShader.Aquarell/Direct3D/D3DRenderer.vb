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

    '---------------------------------
    ' D3D11-Grundsystem
    '---------------------------------

    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext
    Private renderFeatureLevel As FeatureLevel

    '---------------------------------
    ' Quellbild
    '---------------------------------

    Private sourceTexture As ID3D11Texture2D
    Private sourceView As ID3D11ShaderResourceView

    '---------------------------------
    ' Privates RenderTarget
    '---------------------------------

    Private renderTargetTexture As ID3D11Texture2D
    Private renderTargetView As ID3D11RenderTargetView

    '---------------------------------
    ' CPU-Readback
    '---------------------------------

    Private stagingTexture As ID3D11Texture2D

    '---------------------------------
    ' Copy-Shader
    '---------------------------------

    Private copyVertexShader As ID3D11VertexShader
    Private copyPixelShader As ID3D11PixelShader

    '---------------------------------
    ' Kuwahara-Shader
    '---------------------------------

    Private kuwaharaVertexShader As ID3D11VertexShader
    Private kuwaharaPixelShader As ID3D11PixelShader

    '---------------------------------
    ' Testuren für Simulation
    '---------------------------------
    Private sourceWaterTexture As ID3D11Texture2D
    Private sourceWaterView As ID3D11ShaderResourceView
    Private sourceWaterTargetView As ID3D11RenderTargetView

    Private targetWaterTexture As ID3D11Texture2D
    Private targetWaterView As ID3D11ShaderResourceView
    Private targetWaterTargetView As ID3D11RenderTargetView

    Private sourcePigmentTexture As ID3D11Texture2D
    Private sourcePigmentView As ID3D11ShaderResourceView
    Private sourcePigmentTargetView As ID3D11RenderTargetView

    Private targetPigmentTexture As ID3D11Texture2D
    Private targetPigmentView As ID3D11ShaderResourceView
    Private targetPigmentTargetView As ID3D11RenderTargetView

    Private kuwaharaTexture As ID3D11Texture2D
    Private kuwaharaView As ID3D11ShaderResourceView
    Private kuwaharaTargetView As ID3D11RenderTargetView

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

        sourceTexture = Direct3DRessourceHandler.ErstelleTextureAusImage(renderDevice, baseImage)

        sourceView = renderDevice.CreateShaderResourceView(sourceTexture)

        renderTargetTexture = Direct3DRessourceHandler.ErstelleRenderTargetTexture(renderDevice, renderBreite, renderHoehe)

        renderTargetView = renderDevice.CreateRenderTargetView(renderTargetTexture)

        stagingTexture = Direct3DRessourceHandler.ErstelleStagingTexture(renderDevice, renderBreite, renderHoehe)

        If sourceView Is Nothing Then
            Throw New InvalidOperationException("Die Source-SRV konnte nicht erzeugt werden.")
        End If

        If renderTargetView Is Nothing Then
            Throw New InvalidOperationException("Die RenderTargetView konnte nicht erzeugt werden.")
        End If

    End Sub

    Private Sub InitialisiereShader()

        Dim copyVertexShaderCode() As Byte
        Dim copyPixelShaderCode() As Byte

        Dim kuwaharaVertexShaderCode() As Byte
        Dim kuwaharaPixelShaderCode() As Byte

        copyVertexShaderCode = LadeShaderBytecode("AquarellCopyShaderVS.cso")
        copyPixelShaderCode = LadeShaderBytecode("AquarellCopyShaderPS.cso")

        kuwaharaVertexShaderCode = LadeShaderBytecode("KuwaharaShaderVS.cso")
        kuwaharaPixelShaderCode = LadeShaderBytecode("KuwaharaShaderPS.cso")

        copyVertexShader = renderDevice.CreateVertexShader(copyVertexShaderCode)
        copyPixelShader = renderDevice.CreatePixelShader(copyPixelShaderCode)

        kuwaharaVertexShader = renderDevice.CreateVertexShader(kuwaharaVertexShaderCode)
        kuwaharaPixelShader = renderDevice.CreatePixelShader(kuwaharaPixelShaderCode)

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

#End Region

#Region "Rendering"

    Friend Function RenderTestbild() As Bitmap

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


        ' ============================================================
        ' TEST V0.1a
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
        '
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
        ' PASS 2: Kuwahara unten links
        ' ============================================================

        renderContext.RSSetViewport(
            New Viewport(
                0.0F,
                CSng(quadrantHoehe),
                CSng(quadrantBreite),
                CSng(quadrantHoehe),
                0.0F,
                1.0F))

        renderContext.VSSetShader(kuwaharaVertexShader)
        renderContext.PSSetShader(kuwaharaPixelShader)

        renderContext.Draw(3UI, 0UI)

        ' SRV wieder lösen.
        '
        ' Das wird später bei Multipass besonders wichtig, weil dieselbe
        ' Texture niemals gleichzeitig als Input und Output gebunden
        ' bleiben darf.

        renderContext.PSSetShaderResource(0UI, Nothing)
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

        '
        ' Alle Bindings entfernen, bevor Ressourcen freigegeben werden.
        '

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

        Direct3DRessourceHandler.GebeFrei(sourceView)

        '---------------------------------
        ' RenderTargetView
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(renderTargetView)

        '---------------------------------
        ' Texturen
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(stagingTexture)
        Direct3DRessourceHandler.GebeFrei(renderTargetTexture)
        Direct3DRessourceHandler.GebeFrei(sourceTexture)

        '---------------------------------
        ' Shader
        '---------------------------------

        Direct3DRessourceHandler.GebeFrei(copyPixelShader)
        Direct3DRessourceHandler.GebeFrei(copyVertexShader)

        Direct3DRessourceHandler.GebeFrei(kuwaharaPixelShader)
        Direct3DRessourceHandler.GebeFrei(kuwaharaVertexShader)

        '---------------------------------
        ' Context
        '---------------------------------

        If renderContext IsNot Nothing Then

            renderContext.ClearState()
            renderContext.Flush()

            renderContext.Dispose()
            renderContext = Nothing

        End If

        '---------------------------------
        ' Device
        '---------------------------------

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