Texture2D sourceTexture : register(t0);

SamplerState sourceSampler : register(s0);


// ============================================================================
// Vertex-Ausgabe
// ============================================================================

struct VertexOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


// ============================================================================
// VertexShader
//
// Fullscreen-Triangle ohne VertexBuffer.
//
// Das Dreieck überdeckt den aktuell von D3D11 gesetzten Viewport vollständig.
// Für V0.1a beträgt dieser Viewport nur 50 % der Bildbreite und -höhe und liegt
// oben links.
//
// Damit wird exakt dasselbe Quellbild unverändert in den linken oberen
// Quadranten gerendert.
// ============================================================================

VertexOutput VSMain(uint vertexId : SV_VertexID)
{
    VertexOutput output;

    float2 positions[3] =
    {
        float2(-1.0, -1.0),
        float2(-1.0, 3.0),
        float2(3.0, -1.0)
    };

    float2 texCoords[3] =
    {
        float2(0.0, 1.0),
        float2(0.0, -1.0),
        float2(2.0, 1.0)
    };


    output.position =
        float4(
            positions[vertexId],
            0.0,
            1.0);

    output.texCoord =
        texCoords[vertexId];


    return output;
}


// ============================================================================
// PixelShader
//
// Absichtlich exakt keine Bildmanipulation.
//
// Dieser Pass dient ausschließlich dazu, unseren vollständigen
//
//     System.Drawing.Image
//         -> D3D11
//         -> Shader
//         -> RenderTarget
//         -> Staging
//         -> System.Drawing.Bitmap
//
// Roundtrip zu validieren.
//
// Wenn das Ergebnis nicht farblich korrekt ist, liegt der Fehler folglich
// nicht in einem Bildalgorithmus, sondern in unserem D3D-Fundament.
// ============================================================================

float4 PSMain(VertexOutput input) : SV_TARGET
{
    return sourceTexture.Sample(
        sourceSampler,
        input.texCoord);
}