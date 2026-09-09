// ============================================================================
// PigmentDisplayShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Erzeugt aus
//
//          suspendiertem Pigment
//          +
//          abgelagertem Pigment
//
//      die sichtbare Aquarellfarbe.
//
// Pigmentdarstellung:
//
//      RGB = premultiplizierte Pigmentfarbe
//      A   = Pigmentmenge
//
// Suspension und Deposit werden für die Anzeige wieder zu einer
// Gesamtpigmentmenge vereinigt.
//
// ============================================================================


// ============================================================================
// Constant Buffer
//
// exakt 16 Byte
// ============================================================================

cbuffer PigmentDisplayConstants : register(b0)
{
    float backgroundRed;
    float backgroundGreen;
    float backgroundBlue;
    float backgroundAlpha;
};


// ============================================================================
// Eingaben
//
// t0 = suspendiertes Pigment
// t1 = abgelagertes Pigment
// ============================================================================

Texture2D<float4> suspensionTexture : register(t0);
Texture2D<float4> depositTexture : register(t1);

SamplerState sourceSampler : register(s0);


// ============================================================================
// Vertex Shader
// ============================================================================

struct VSOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


VSOutput VSMain(uint vertexId : SV_VertexID)
{
    VSOutput output;

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

    output.texCoord = texCoords[vertexId];


    return output;
}


// ============================================================================
// Pixel Shader
// ============================================================================

float4 PSMain(VSOutput input) : SV_TARGET
{
    float4 suspension;
    float4 deposit;

    float3 totalPremultipliedColor;
    float totalPigmentAmount;

    float3 pigmentColor;
    float3 backgroundColor;

    float visibleAmount;
    float3 resultColor;


    // ========================================================================
    // Pigmentzustände lesen
    // ========================================================================

    suspension =
        max(
            suspensionTexture.SampleLevel(sourceSampler, input.texCoord, 0.0F),
            float4(0.0F, 0.0F, 0.0F, 0.0F)
        );

    deposit =
        max(
            depositTexture.SampleLevel(sourceSampler, input.texCoord, 0.0F),
            float4(0.0F, 0.0F, 0.0F, 0.0F)
        );


    // ========================================================================
    // Gesamtpigment
    //
    // Beide Texturen speichern:
    //
    //      RGB = Farbe * Pigmentmenge
    //      A   = Pigmentmenge
    //
    // Deshalb können beide Zustände direkt addiert werden.
    // ========================================================================

    totalPremultipliedColor =
        suspension.rgb +
        deposit.rgb;

    totalPigmentAmount =
        suspension.a +
        deposit.a;


    // ========================================================================
    // Pigmentfarbe zurückgewinnen
    // ========================================================================

    if (totalPigmentAmount > 0.000001F)
    {
        pigmentColor =
            totalPremultipliedColor /
            totalPigmentAmount;
    }
    else
    {
        pigmentColor =
            float3(
                backgroundRed,
                backgroundGreen,
                backgroundBlue
            );
    }


    // ========================================================================
    // Sichtbare Deckung
    //
    // Mehr als 1.0 Pigmentmenge bedeutet nicht mehr als vollständig deckend.
    //
    // Später können wir hier bei Bedarf eine optisch realistischere
    // Pigment-/Papier-Übertragungsfunktion einsetzen.
    // ========================================================================

    visibleAmount =
        saturate(totalPigmentAmount);


    backgroundColor =
        float3(
            backgroundRed,
            backgroundGreen,
            backgroundBlue
        );


    resultColor =
        lerp(
            backgroundColor,
            pigmentColor,
            visibleAmount
        );


    return float4(resultColor, 1.0F);
}