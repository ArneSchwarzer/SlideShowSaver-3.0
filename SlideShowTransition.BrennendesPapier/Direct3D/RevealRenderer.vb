Imports System.Runtime.InteropServices
Imports System.Windows.Media.Imaging
Imports SlideShowDirect3DInterop
Imports SlideShowLogging
Imports Vortice.Direct3D11

Friend Class RevealRenderer
    Implements IDisposable

#Region "Variablendeklaration"

    Private renderDevice As ID3D11Device
    Private renderContext As ID3D11DeviceContext

    Private renderBreite As Integer
    Private renderHoehe As Integer

    'Reveal-Bilder
    Private oldImageTexture As ID3D11Texture2D
    Private newImageTexture As ID3D11Texture2D

    Private oldImageView As ID3D11ShaderResourceView
    Private newImageView As ID3D11ShaderResourceView

    'Reveal-Shader
    Private revealVertexShader As ID3D11VertexShader
    Private revealPixelShader As ID3D11PixelShader
    Private revealParameterBuffer As ID3D11Buffer

    'Verzerrung
    Private distortionZeit As Single
    Private effektNachlaufZeit As Single

    Private verzerrungsBreite As Single
    Private verzerrungsEffektStaerke As Single
    Private magieRasterHoehe As Single

    Private gradientIstAktiv As Boolean
    Private verzerrungIstAktiv As Boolean

    Private aktuellerModus As String

    Private Const MAGIE_NACHLAUF_DAUER As Single = 4.0F
    Private Const REVEAL_NACHLAUF_DAUER As Single = 0.5F
    Private Const SAEURE_NACHLAUF_DAUER As Single = 3.0F
    Private Const FEUER_NACHLAUF_DAUER As Single = 2.0F
    Private Const BLITZ_NACHLAUF_DAUER As Single = 1.0F

    Private wurdeBereinigt As Boolean

#End Region

#Region "Structures & Enums"

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RevealShaderParameter

        Public progress As Single
        Public zeit As Single
        Public renderBreite As Single
        Public renderHoehe As Single

        Public distortionModus As UInteger
        Public magieRasterHoehe As Single
        Public distortionStaerke As Single
        Public magieNachlaufProgress As Single

        Public verzerrungAktiv As UInteger
        Public gradientAktiv As UInteger
        Public verzerrungsEffektStaerke As Single
        Public verzerrungsBreite As Single

        Public revealNachlaufProgress As Single
        Public saeureNachlaufProgress As Single
        Public feuerNachlaufProgress As Single
        Public blitzNachlaufProgress As Single

    End Structure

#End Region

#Region "Initialisierung"

    Friend Sub Initialisiere(device As ID3D11Device, context As ID3D11DeviceContext, parameter As RenderParameter)

        If wurdeBereinigt Then
            Throw New ObjectDisposedException(NameOf(RevealRenderer))
        End If

        If device Is Nothing Then
            Throw New ArgumentNullException(NameOf(device))
        End If

        If context Is Nothing Then
            Throw New ArgumentNullException(NameOf(context))
        End If

        If parameter Is Nothing Then
            Throw New ArgumentNullException(NameOf(parameter))
        End If

        If parameter.oldImage Is Nothing Then
            Throw New ArgumentNullException(NameOf(parameter.oldImage))
        End If

        If parameter.newImage Is Nothing Then
            Throw New ArgumentNullException(NameOf(parameter.newImage))
        End If

        renderDevice = device
        renderContext = context

        renderBreite = parameter.breite
        renderHoehe = parameter.hoehe

        gradientIstAktiv = parameter.gradientAktiv
        verzerrungIstAktiv = parameter.verzerrungAktiv

        aktuellerModus = parameter.modus

        verzerrungsBreite = Math.Max(0.01F, parameter.verzerrungsBreite)
        verzerrungsEffektStaerke = Math.Max(0.0F, parameter.verzerrungsStaerke)

        'UI-Wert "Scherbengröße": hoch = große Scherben
        'Shader-Rasterhöhe: hoch = kleine Scherben
        magieRasterHoehe = Math.Max(5.0F, 35.0F - parameter.magieScherbenGroesse)

        distortionZeit = 0.0F
        effektNachlaufZeit = 0.0F

        InitialisiereRevealTexturen(parameter.oldImage, parameter.newImage)
        InitialisiereRevealShader()
        InitialisiereRevealParameterBuffer()

        AktualisiereParameter(0.0F, 0.0F)

    End Sub

    Private Sub InitialisiereRevealTexturen(oldImage As BitmapSource, newImage As BitmapSource)

        oldImageTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, oldImage)
        newImageTexture = Direct3DRessourceHandler.ErstelleTextureAusBitmapSource(renderDevice, newImage)

        oldImageView = renderDevice.CreateShaderResourceView(oldImageTexture)
        newImageView = renderDevice.CreateShaderResourceView(newImageTexture)

        LogHandling.LogDebug("D3D: Reveal-Bildtexturen und ShaderResourceViews erzeugt.")

    End Sub

    Private Sub InitialisiereRevealShader()

        Dim vertexShaderCode() As Byte
        Dim pixelShaderCode() As Byte

        vertexShaderCode = D3DRenderer.LadeShaderBytecode("RevealShaderVS.cso")
        pixelShaderCode = D3DRenderer.LadeShaderBytecode("RevealShaderPS.cso")

        revealVertexShader = renderDevice.CreateVertexShader(vertexShaderCode)
        revealPixelShader = renderDevice.CreatePixelShader(pixelShaderCode)

        LogHandling.LogDebug("D3D: RevealShader wurde geladen.")

    End Sub

    Private Sub InitialisiereRevealParameterBuffer()

        Dim bufferDescription As BufferDescription

        bufferDescription =
            New BufferDescription(
               64UI,
               BindFlags.ConstantBuffer,
               ResourceUsage.Dynamic,
               CpuAccessFlags.Write)

        revealParameterBuffer = renderDevice.CreateBuffer(bufferDescription)

    End Sub

#End Region

#Region "Rendering"

    Friend Sub AktualisiereParameter(progress As Single, deltaTime As Single)

        Dim mappedResource As MappedSubresource
        Dim parameter As RevealShaderParameter

        If deltaTime > 0.0F Then

            distortionZeit += deltaTime

            If progress >= 1.0F Then

                effektNachlaufZeit += deltaTime

            Else

                effektNachlaufZeit = 0.0F

            End If

        End If

        parameter.progress = Math.Max(0.0F, Math.Min(1.0F, progress))
        parameter.zeit = distortionZeit
        parameter.renderBreite = CSng(renderBreite)
        parameter.renderHoehe = CSng(renderHoehe)
        parameter.distortionModus = ErmittleDistortionModus()
        parameter.magieRasterHoehe = magieRasterHoehe

        'Bewusst der reine Einblendfaktor
        'der fertigen Distortion.
        parameter.distortionStaerke = 1.0F
        parameter.magieNachlaufProgress = Math.Max(0.0F, Math.Min(1.0F, effektNachlaufZeit / MAGIE_NACHLAUF_DAUER))
        parameter.verzerrungAktiv = If(verzerrungIstAktiv, 1UI, 0UI)
        parameter.gradientAktiv = If(gradientIstAktiv, 1UI, 0UI)
        parameter.verzerrungsEffektStaerke = verzerrungsEffektStaerke
        parameter.verzerrungsBreite = verzerrungsBreite
        parameter.revealNachlaufProgress = Math.Max(0.0F, Math.Min(1.0F, effektNachlaufZeit / REVEAL_NACHLAUF_DAUER))
        parameter.saeureNachlaufProgress = Math.Max(0.0F, Math.Min(1.0F, effektNachlaufZeit / SAEURE_NACHLAUF_DAUER))
        parameter.feuerNachlaufProgress = Math.Max(0.0F, Math.Min(1.0F, effektNachlaufZeit / FEUER_NACHLAUF_DAUER))
        parameter.blitzNachlaufProgress = Math.Max(0.0F, Math.Min(1.0F, effektNachlaufZeit / BLITZ_NACHLAUF_DAUER))

        mappedResource = renderContext.Map(revealParameterBuffer, 0UI, MapMode.WriteDiscard,
                                           Vortice.Direct3D11.MapFlags.None)

        Marshal.StructureToPtr(parameter, mappedResource.DataPointer, False)

        renderContext.Unmap(revealParameterBuffer, 0UI)

    End Sub

    Private Function ErmittleDistortionModus() As UInteger

        If Not verzerrungIstAktiv Then
            Return 0UI
        End If


        Select Case aktuellerModus

            Case "Feuer"

                Return 1UI

            Case "Blitze"

                Return 2UI

            Case "Säure"

                Return 3UI

            Case "Magie"

                Return 4UI

            Case Else

                Return 0UI

        End Select

    End Function

    Friend Sub Render(maskView As ID3D11ShaderResourceView, sampler As ID3D11SamplerState)

        If maskView Is Nothing Then
            Throw New ArgumentNullException(NameOf(maskView))
        End If

        If sampler Is Nothing Then
            Throw New ArgumentNullException(NameOf(sampler))
        End If

        renderContext.OMSetBlendState(Nothing)
        renderContext.VSSetShader(revealVertexShader)
        renderContext.PSSetShader(revealPixelShader)
        renderContext.PSSetShaderResource(0UI, oldImageView)
        renderContext.PSSetShaderResource(1UI, newImageView)
        renderContext.PSSetShaderResource(2UI, maskView)
        renderContext.PSSetSampler(0UI, sampler)
        renderContext.PSSetConstantBuffer(0UI, revealParameterBuffer)

        renderContext.Draw(3UI, 0UI)

        renderContext.PSSetShaderResource(0UI, Nothing)
        renderContext.PSSetShaderResource(1UI, Nothing)
        renderContext.PSSetShaderResource(2UI, Nothing)

    End Sub

#End Region

#Region "Bereinigen & Dispose"

    Private Sub BeendeUndBereinigeRevealRenderer()

        If wurdeBereinigt Then
            Exit Sub
        End If

        wurdeBereinigt = True

        Direct3DRessourceHandler.GebeFrei(revealParameterBuffer)
        Direct3DRessourceHandler.GebeFrei(newImageView)
        Direct3DRessourceHandler.GebeFrei(oldImageView)
        Direct3DRessourceHandler.GebeFrei(newImageTexture)
        Direct3DRessourceHandler.GebeFrei(oldImageTexture)
        Direct3DRessourceHandler.GebeFrei(revealPixelShader)
        Direct3DRessourceHandler.GebeFrei(revealVertexShader)

    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose

        If wurdeBereinigt Then
            Exit Sub
        End If

        BeendeUndBereinigeRevealRenderer()

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class
