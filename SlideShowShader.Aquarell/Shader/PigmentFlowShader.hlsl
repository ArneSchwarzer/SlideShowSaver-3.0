// ============================================================================
// PigmentFlowShader.hlsl
//
// Transportiert Pigmente entlang des aktuellen Wassergefälles.
//
// Ziel dieses Passes:
// - Pigmente sollen nicht einfach diffus geglättet werden.
// - Die WaterMap / Wassertextur bestimmt die bevorzugte Transportrichtung.
// - Pigment wird aus höher gelegenen Wasserbereichen in niedrigere Bereiche
//   gezogen.
// - Noch KEINE Ablagerung.
// - Noch KEINE Verdunstung.
// - Noch KEINE Papierstruktur.
// - Noch KEINE physikalisch exakte Massenerhaltung.
//
// Dieser Shader ist bewusst eine optisch orientierte Approximation.
// ============================================================================


// ============================================================================
// Constant Buffer
// ============================================================================

cbuffer PigmentFlowConstants : register(b0)
{
    float pigmentTransportStrength;

    // Padding für 16-Byte-Ausrichtung des Constant Buffers.
    float3 paddingPigmentFlow;
};


// ============================================================================
// Shader Resources
// ============================================================================

// Aktueller Pigmentzustand.
Texture2D<float4> sourcePigment : register(t0);

// Aktueller Wasserzustand.
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
    uint textureWidth;
    uint textureHeight;

    uint pigmentWidth;
    uint pigmentHeight;

    uint2 textureSize;
    int2 pixelPosition;

    float waterCenter;
    float waterNorth;
    float waterEast;
    float waterSouth;
    float waterWest;

    float flowNorth;
    float flowEast;
    float flowSouth;
    float flowWest;

    float totalIncomingFlow;
    float transportAmount;

    float4 pigmentCenter;
    float4 pigmentNorth;
    float4 pigmentEast;
    float4 pigmentSouth;
    float4 pigmentWest;

    float4 incomingPigment;
    float4 resultPigment;


    // ------------------------------------------------------------------------
    // Texturgröße bestimmen.
    // ------------------------------------------------------------------------

    sourceWater.GetDimensions(textureWidth, textureHeight);
    sourcePigment.GetDimensions(pigmentWidth, pigmentHeight);

    textureSize = uint2(
        min(textureWidth, pigmentWidth),
        min(textureHeight, pigmentHeight)
    );


    // ------------------------------------------------------------------------
    // Aktuelle Pixelposition.
    // ------------------------------------------------------------------------

    pixelPosition = int2(position.xy);


    // ------------------------------------------------------------------------
    // Aktuellen Wasserstand lesen.
    // ------------------------------------------------------------------------

    waterCenter = ReadWater(pixelPosition, textureSize);


    // ------------------------------------------------------------------------
    // Nachbarn lesen.
    //
    // Die gleiche räumliche Distanz wie beim WaterFlowShader verwenden.
    // Damit arbeiten Wassertransport und Pigmenttransport auf derselben
    // räumlichen Skala.
    // ------------------------------------------------------------------------

    const int flowDistance = 8;

    waterNorth = ReadWater(
        pixelPosition + int2(0, -flowDistance),
        textureSize
    );

    waterEast = ReadWater(
        pixelPosition + int2(flowDistance, 0),
        textureSize
    );

    waterSouth = ReadWater(
        pixelPosition + int2(0, flowDistance),
        textureSize
    );

    waterWest = ReadWater(
        pixelPosition + int2(-flowDistance, 0),
        textureSize
    );


    // ------------------------------------------------------------------------
    // Pigmente lesen.
    // ------------------------------------------------------------------------

    pigmentCenter = ReadPigment(
        pixelPosition,
        textureSize
    );

    pigmentNorth = ReadPigment(
        pixelPosition + int2(0, -flowDistance),
        textureSize
    );

    pigmentEast = ReadPigment(
        pixelPosition + int2(flowDistance, 0),
        textureSize
    );

    pigmentSouth = ReadPigment(
        pixelPosition + int2(0, flowDistance),
        textureSize
    );

    pigmentWest = ReadPigment(
        pixelPosition + int2(-flowDistance, 0),
        textureSize
    );


    // ------------------------------------------------------------------------
    // Wassergefälle bestimmen.
    //
    // Pull-Modell:
    //
    // Das aktuelle Pixel erhält Pigment aus einem Nachbarpixel dann,
    // wenn dort mehr Wasser vorhanden ist als am aktuellen Pixel.
    //
    // Ein höherer Wasserstand beim Nachbarn bedeutet:
    //
    //      Nachbar -> aktuelles Pixel
    //
    // ------------------------------------------------------------------------

    flowNorth = max(
        0.0F,
        waterNorth - waterCenter
    );

    flowEast = max(
        0.0F,
        waterEast - waterCenter
    );

    flowSouth = max(
        0.0F,
        waterSouth - waterCenter
    );

    flowWest = max(
        0.0F,
        waterWest - waterCenter
    );


    // ------------------------------------------------------------------------
    // Gesamtes hereinkommendes Wassergefälle.
    // ------------------------------------------------------------------------

    totalIncomingFlow =
        flowNorth +
        flowEast +
        flowSouth +
        flowWest;


    // ------------------------------------------------------------------------
    // Standard:
    // Pigment bleibt zunächst unverändert.
    // ------------------------------------------------------------------------

    resultPigment = pigmentCenter;


    // ------------------------------------------------------------------------
    // Pigment aus den höher gelegenen Wasserbereichen übernehmen.
    //
    // Die Nachbarpigmente werden entsprechend der Wassergradienten gewichtet.
    // ------------------------------------------------------------------------

    if (totalIncomingFlow > 0.00001F)
    {
        incomingPigment =
            pigmentNorth * flowNorth +
            pigmentEast * flowEast +
            pigmentSouth * flowSouth +
            pigmentWest * flowWest;

        incomingPigment /= totalIncomingFlow;


        // --------------------------------------------------------------------
        // Optische Transportstärke.
        //
        // pigmentTransportStrength ist bewusst KEINE physikalische Größe.
        //
        // Sie bestimmt lediglich, wie deutlich das Wassergefälle innerhalb
        // eines Iterationsschrittes Pigment verschiebt.
        //
        // Beispielwerte für Tests:
        //
        //      2.0  = schwach
        //      4.0  = moderat
        //      8.0  = kräftig
        //     16.0  = sehr kräftig
        //
        // --------------------------------------------------------------------

        transportAmount = saturate(
            totalIncomingFlow *
            pigmentTransportStrength
        );


        // --------------------------------------------------------------------
        // Pigment in Richtung des wassergetriebenen Nachbarmixes bewegen.
        // --------------------------------------------------------------------

        resultPigment = lerp(
            pigmentCenter,
            incomingPigment,
            transportAmount
        );
    }


    // ------------------------------------------------------------------------
    // Ergebnis zurückgeben.
    // ------------------------------------------------------------------------

    return resultPigment;
}

// ============================================================================
// PigmentFlowVertexShader.hlsl
//
// Minimaler Fullscreen-Vertex-Shader für den PigmentFlow-Pass.
//
// Er erzeugt aus einem 3-Vertex-Drawcall ein Fullscreen-Dreieck.
// Dadurch benötigen wir weder VertexBuffer noch InputLayout.
//
// Erwarteter Drawcall:
//
//     renderContext.Draw(3UI, 0UI)
//
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


    // ------------------------------------------------------------------------
    // Fullscreen-Dreieck erzeugen.
    //
    // Vertex 0 = (-1, -1)
    // Vertex 1 = (-1,  3)
    // Vertex 2 = ( 3, -1)
    //
    // Das Dreieck überdeckt damit vollständig den Viewport.
    // ------------------------------------------------------------------------

    position = float2(
        (vertexId == 2) ? 3.0F : -1.0F,
        (vertexId == 1) ? 3.0F : -1.0F
    );


    // ------------------------------------------------------------------------
    // Texturkoordinaten passend zum Fullscreen-Dreieck.
    //
    // Für PigmentFlowShader.hlsl momentan nicht zwingend erforderlich,
    // da der Pixelshader über SV_POSITION arbeitet.
    //
    // Wir geben sie trotzdem sauber aus, falls wir sie später benötigen.
    // ------------------------------------------------------------------------

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