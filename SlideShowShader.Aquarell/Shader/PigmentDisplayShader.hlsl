// ============================================================================
// PigmentDisplayShader.hlsl
//
// Wandelt den internen Pigmentzustand in das tatsächlich sichtbare
// Aquarellbild um.
//
// PigmentTexture:
//     RGB = premultiplizierte Pigmentfarbe
//     A   = Pigmentmenge
//
// Die Pigmentmenge ist ausdrücklich KEIN Ausgabe-Alpha.
//
// Dünn pigmentierte Bereiche geben stattdessen die vom SlideShowSaver
// vorgegebene Hintergrundfarbe frei.
//
// Das Ergebnis dieses Shaders ist anschließend immer vollständig opak.
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
// Vertex Shader
// ============================================================================

struct VS_OUTPUT
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


VS_OUTPUT VSMain(uint vertexId : SV_VertexID)
{
    VS_OUTPUT output;

    float2 position;
    float2 texCoord;

    position = float2(
        (vertexId == 2) ? 3.0F : -1.0F,
        (vertexId == 1) ? 3.0F : -1.0F
    );

    texCoord = float2(
        (vertexId == 2) ? 2.0F : 0.0F,
        (vertexId == 1) ? -1.0F : 1.0F
    );

    output.position = float4(
        position,
        0.0F,
        1.0F
    );

    output.texCoord = texCoord;

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

    pigment = pigmentTexture.Load(
        int3(int2(input.position.xy), 0)
    );

    pigmentAmount = saturate(pigment.a);

    // RGB der PigmentTexture ist bereits mit der Pigmentmenge gewichtet.
    pigmentColor = pigment.rgb;

    // Fehlende Pigmentmenge wird mit der Saver-Hintergrundfarbe aufgefüllt.
    resultColor =
        pigmentColor +
        backgroundColor.rgb * (1.0F - pigmentAmount);

    // Ganz wichtig:
    //
    // Das Alpha der PigmentTexture ist Simulationsinformation.
    // Es darf NICHT nach außen an wpfModulMain gelangen.
    //
    // Das fertige Aquarellbild ist vollständig opak.

    return float4(
        saturate(resultColor),
        1.0F
    );
}