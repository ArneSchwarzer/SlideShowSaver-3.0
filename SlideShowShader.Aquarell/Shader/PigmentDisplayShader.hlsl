// ============================================================================
// PigmentDisplayShader.hlsl
//
// Wandelt den internen Pigmentzustand in das sichtbar ausgegebene
// Aquarellbild um.
//
// PigmentTexture:
//     RGB = premultiplizierte Pigmentfarbe
//     A   = Pigmentmenge
//
// Dünn pigmentierte Bereiche werden mit der vom SlideShowSaver
// vorgegebenen Hintergrundfarbe aufgefüllt.
//
// Das Ausgabe-Alpha ist immer 1.0.
// ============================================================================


// ============================================================================
// Constant Buffer
// ============================================================================

cbuffer PigmentDisplayConstants : register(b0)
{
    float4 backgroundColor;
};


// ============================================================================
// Shader Resources
// ============================================================================

Texture2D<float4> pigmentTexture : register(t0);


// ============================================================================
// Sampler
// ============================================================================

SamplerState pigmentSampler : register(s0);


// ============================================================================
// Vertex-Ausgabe
// ============================================================================

struct VS_OUTPUT
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


// ============================================================================
// Vertex Shader
//
// Gleiche Fullscreen-Triangle-Geometrie wie beim Copy-Shader.
//
// Entscheidend:
// Die Texturkoordinaten laufen unabhängig von der Position des Viewports
// sauber über die vollständige Quelltexture.
// ============================================================================

VS_OUTPUT VSMain(uint vertexId : SV_VertexID)
{
    VS_OUTPUT output;

    float2 positions[3] =
    {
        float2(-1.0F, -1.0F),
        float2(-1.0F, 3.0F),
        float2(3.0F, -1.0F)
    };

    float2 texCoords[3] =
    {
        float2(0.0F, 1.0F),
        float2(0.0F, -1.0F),
        float2(2.0F, 1.0F)
    };

    output.position =
        float4(
            positions[vertexId],
            0.0F,
            1.0F
        );

    output.texCoord =
        texCoords[vertexId];

    return output;
}


// ============================================================================
// Pixel Shader
// ============================================================================

float4 PSMain(VS_OUTPUT input) : SV_TARGET
{
    float4 pigment;

    float pigmentAmount;

    float3 pigmentColor;
    float3 resultColor;


    // ------------------------------------------------------------------------
    // WICHTIG:
    //
    // NICHT input.position.xy verwenden!
    //
    // SV_POSITION enthält absolute RenderTarget-Koordinaten und würde beim
    // Rendering in einen Teil-Viewport nur einen Ausschnitt der Pigmenttexture
    // adressieren.
    //
    // texCoord läuft dagegen immer über das vollständige Quellbild.
    // ------------------------------------------------------------------------

    pigment =
        pigmentTexture.Sample(
            pigmentSampler,
            input.texCoord
        );


    // ------------------------------------------------------------------------
    // Pigmentmenge begrenzen.
    // ------------------------------------------------------------------------

    pigmentAmount =
        saturate(
            pigment.a
        );


    // ------------------------------------------------------------------------
    // RGB enthält bereits die mit der Pigmentmenge gewichtete Farbe.
    // ------------------------------------------------------------------------

    pigmentColor =
        pigment.rgb;


    // ------------------------------------------------------------------------
    // Fehlende Pigmentmenge mit HintergrundFarbeSaver auffüllen.
    // ------------------------------------------------------------------------

    resultColor =
        pigmentColor +
        backgroundColor.rgb *
        (1.0F - pigmentAmount);


    // ------------------------------------------------------------------------
    // Ausgabe vollständig opak.
    // ------------------------------------------------------------------------

    return float4(
        saturate(resultColor),
        1.0F
    );
}