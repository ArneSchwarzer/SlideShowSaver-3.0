// ============================================================================
// PigmentFlowShader.hlsl
//
// Transportiert Pigmentmasse entlang des Wassergefälles.
//
// Datenmodell:
//
//      RGB = premultiplizierte Pigmentfarbmasse
//      A   = Pigmentmasse
//
// WICHTIG:
//
// A ist KEINE sichtbare Deckung.
//
// Die Pigmentmasse darf deshalb:
//
//      < 1.0   sein   -> Pigment wurde abtransportiert
//      = 1.0   sein   -> ursprüngliche Pigmentmenge
//      > 1.0   sein   -> Pigment hat sich angesammelt
//
// Die Umrechnung von Pigmentmasse in sichtbare Deckung erfolgt erst im
// PigmentDisplayShader.
//
// WICHTIG:
//
// Pigment wird nicht nur von Nachbarn hereingezogen.
// Das aktuelle Pixel verliert zugleich Pigment entsprechend seines
// eigenen Wasserabflusses.
//
// Dadurch entstehen erstmals tatsächlich dünner pigmentierte Bereiche,
// in denen später die Hintergrund-/Papierfarbe durchscheinen kann.
//
// Noch NICHT enthalten:
//
// - Pigmentablagerung
// - Verdunstung
// - Papierabsorption
// - Granulation
// - echte physikalische Massenerhaltung
//
// Der Shader bleibt bewusst eine optische Approximation.
// ============================================================================


// ============================================================================
// Constant Buffer
// ============================================================================

cbuffer PigmentFlowConstants : register(b0)
{
    float pigmentTransportStrength;

    float3 paddingPigmentFlow;
};


// ============================================================================
// Shader Resources
// ============================================================================

Texture2D<float4> sourcePigment : register(t0);

Texture2D<float> sourceWater : register(t1);


// ============================================================================
// Sampler
// ============================================================================

SamplerState pointSampler : register(s0);


// ============================================================================
// Hilfsfunktionen
// ============================================================================

float ReadWater(int2 pixelPosition, uint2 textureSize)
{
    pixelPosition.x = clamp(pixelPosition.x, 0, int(textureSize.x) - 1);
    pixelPosition.y = clamp(pixelPosition.y, 0, int(textureSize.y) - 1);

    return sourceWater.Load(int3(pixelPosition, 0));
}


float4 ReadPigment(int2 pixelPosition, uint2 textureSize)
{
    pixelPosition.x = clamp(pixelPosition.x, 0, int(textureSize.x) - 1);
    pixelPosition.y = clamp(pixelPosition.y, 0, int(textureSize.y) - 1);

    return sourcePigment.Load(int3(pixelPosition, 0));
}


// ============================================================================
// Pixel Shader
// ============================================================================

float4 PSMain(float4 position : SV_POSITION) : SV_TARGET
{
    uint waterWidth;
    uint waterHeight;

    uint pigmentWidth;
    uint pigmentHeight;

    uint2 textureSize;

    int2 pixelPosition;

    float waterCenter;

    float waterNorth;
    float waterEast;
    float waterSouth;
    float waterWest;

    float incomingNorth;
    float incomingEast;
    float incomingSouth;
    float incomingWest;

    float outgoingNorth;
    float outgoingEast;
    float outgoingSouth;
    float outgoingWest;

    float totalIncomingFlow;
    float totalOutgoingFlow;

    float incomingTransport;
    float outgoingTransport;

    float4 pigmentCenter;

    float4 pigmentNorth;
    float4 pigmentEast;
    float4 pigmentSouth;
    float4 pigmentWest;

    float4 incomingPigment;

    float4 remainingPigment;
    float4 resultPigment;
    

    // ------------------------------------------------------------------------
    // Texturgröße bestimmen.
    // ------------------------------------------------------------------------

    sourceWater.GetDimensions(waterWidth, waterHeight);

    sourcePigment.GetDimensions(pigmentWidth, pigmentHeight);

    textureSize = uint2(min(waterWidth, pigmentWidth), min(waterHeight, pigmentHeight));


    // ------------------------------------------------------------------------
    // Aktuelle Pixelposition.
    // ------------------------------------------------------------------------

    pixelPosition = int2(position.xy);


    // ------------------------------------------------------------------------
    // Gleiche Transportdistanz wie bisher.
    //
    // Naturkonstante Nr. 392 des SSS-Frameworks. ;-)
    // ------------------------------------------------------------------------

    const int flowDistance = 8;


    // ------------------------------------------------------------------------
    // Wasser lesen.
    // ------------------------------------------------------------------------

    waterCenter = ReadWater(pixelPosition, textureSize);

    waterNorth = ReadWater(pixelPosition + int2(0, -flowDistance), textureSize);
    waterEast = ReadWater(pixelPosition + int2(flowDistance, 0), textureSize);
    waterSouth = ReadWater(pixelPosition + int2(0, flowDistance), textureSize);
    waterWest = ReadWater(pixelPosition + int2(-flowDistance, 0), textureSize);
    
    // ------------------------------------------------------------------------
    // Pigmente lesen.
    // ------------------------------------------------------------------------

    pigmentCenter = ReadPigment(pixelPosition, textureSize);

    pigmentNorth = ReadPigment(pixelPosition + int2(0, -flowDistance), textureSize);
    pigmentEast = ReadPigment(pixelPosition + int2(flowDistance, 0), textureSize);
    pigmentSouth = ReadPigment(pixelPosition + int2(0, flowDistance), textureSize);
    pigmentWest = ReadPigment(pixelPosition + int2(-flowDistance, 0), textureSize);

    // ------------------------------------------------------------------------
    // HEREINKOMMENDER Transport.
    //
    // Nachbar besitzt höheren Wasserstand:
    //
    //      Nachbar -> Center
    // ------------------------------------------------------------------------

    incomingNorth = max(0.0F, waterNorth - waterCenter);
    incomingEast = max(0.0F, waterEast - waterCenter);
    incomingSouth = max(0.0F, waterSouth - waterCenter);
    incomingWest = max(0.0F, waterWest - waterCenter);

    totalIncomingFlow = incomingNorth + incomingEast + incomingSouth + incomingWest;
    
    // ------------------------------------------------------------------------
    // ABFLIESSENDER Transport.
    //
    // Center besitzt höheren Wasserstand:
    //
    //      Center -> Nachbar
    //
    // GENAU DAS fehlte bislang.
    // ------------------------------------------------------------------------

    outgoingNorth = max(0.0F, waterCenter - waterNorth);
    outgoingEast = max(0.0F, waterCenter - waterEast);
    outgoingSouth = max(0.0F, waterCenter - waterSouth);
    outgoingWest = max(0.0F, waterCenter - waterWest);

    totalOutgoingFlow = outgoingNorth + outgoingEast + outgoingSouth + outgoingWest;
    
    // ------------------------------------------------------------------------
    // Transportstärken bestimmen.
    //
    // Beide Werte bewusst saturieren:
    //
    // Wir wollen niemals mehr als die gesamte vorhandene Pigmentmasse
    // innerhalb EINER Iteration wegtransportieren.
    // ------------------------------------------------------------------------

    incomingTransport = saturate(totalIncomingFlow * pigmentTransportStrength);
    outgoingTransport = saturate(totalOutgoingFlow * pigmentTransportStrength);
    
    // ------------------------------------------------------------------------
    // Eigenes Pigment reduzieren.
    //
    // Weil RGB premultipliziert vorliegt, dürfen RGB und Alpha gemeinsam
    // skaliert werden.
    // ------------------------------------------------------------------------

    remainingPigment = pigmentCenter * (1.0F - outgoingTransport);

    // ------------------------------------------------------------------------
    // Hereinkommendes Pigment bestimmen.
    // ------------------------------------------------------------------------

    incomingPigment = float4(0.0F, 0.0F, 0.0F, 0.0F);
    
    if (totalIncomingFlow > 0.00001F)
    {
        incomingPigment = pigmentNorth * incomingNorth + pigmentEast * incomingEast + pigmentSouth * incomingSouth +
                          pigmentWest * incomingWest;

        incomingPigment /= totalIncomingFlow;
        incomingPigment *= incomingTransport;
    }
    
    // ------------------------------------------------------------------------
    // Verbleibendes + hereinkommendes Pigment.
    //
    // WICHTIG:
    //
    // Ab jetzt gilt:
    //
    //      RGB = premultiplizierte PigmentFARBmasse
    //      A   = PigmentMASSE
    //
    // A ist ausdrücklich KEINE sichtbare Deckung mehr.
    //
    // Deshalb darf die Pigmentmasse auch größer als 1.0 werden.
    //
    // Beispiel:
    //
    //      A = 0.4   dünn pigmentiert
    //      A = 1.0   ursprüngliche Pigmentmenge
    //      A = 1.8   Pigmentakkumulation
    //
    // Wie diese Masse später sichtbar wird, entscheidet ausschließlich
    // PigmentDisplayShader.hlsl.
    // ------------------------------------------------------------------------

    resultPigment = remainingPigment + incomingPigment;


    // ------------------------------------------------------------------------
    // Negative Masse darf natürlich nicht entstehen.
    //
    // Nach oben wird dagegen bewusst NICHT begrenzt.
    // ------------------------------------------------------------------------

    resultPigment = max(resultPigment, float4(0.0F, 0.0F, 0.0F, 0.0F));
    
    return resultPigment;
}

// ============================================================================
// Fullscreen Vertex Shader
// ============================================================================

struct VS_OUTPUT
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


VS_OUTPUT VSMain(
    uint vertexId : SV_VertexID)
{
    VS_OUTPUT output;

    float2 shaderPosition;
    float2 texCoord;


    shaderPosition = float2((vertexId == 2) ? 3.0F : -1.0F, (vertexId == 1) ? 3.0F : -1.0F);

    texCoord = float2((vertexId == 2) ? 2.0F : 0.0F, (vertexId == 1) ? -1.0F : 1.0F);
    
    output.position = float4(shaderPosition, 0.0F, 1.0F);

    output.texCoord = texCoord;


    return output;
}