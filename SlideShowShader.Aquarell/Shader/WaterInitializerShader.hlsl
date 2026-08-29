// ============================================================================
// WaterInitializerShader.hlsl
// ============================================================================
//
// SlideShowSaver 3.0
// Shader: Aquarell
//
// Aufgabe:
//
// Aus dem bereits durch Classic Kuwahara vereinfachten Bild wird das
// initiale Wasserfeld für die Aquarell-Simulation erzeugt.
//
// WICHTIG:
//
// Diese WaterMap ist ausdrücklich KEINE physikalisch vollständige
// Beschreibung einer realen Wasseroberfläche.
//
// Für die aktuelle frühe Simulationsstufe soll sie vor allem:
//
//      - zum Bild passende räumliche Strukturen besitzen
//      - große homogene Kuwahara-Flächen weitgehend ruhig lassen
//      - an Kuwahara-Farbgrenzen leichte Senken erzeugen
//      - eine kleine natürliche Grundvariation besitzen
//
// Warum SENKEN an Kanten?
//
// Unsere aktuelle Simulation kennt noch:
//
//      KEINE Verdunstung
//      KEINE Kapillarwirkung
//      KEINE Pigmentablagerung
//      KEINE Papierabsorption
//
// Würden wir die Kanten jetzt erhöhen, würde der WaterFlowShader Pigmente
// tendenziell von diesen Kanten wegtransportieren.
//
// Deshalb verwenden wir für V0.x bewusst eine künstlerisch motivierte
// Ersatzregel:
//
//      Kuwahara-Kante -> leichte Wassersenke
//
// Dadurch werden Pigmente in der späteren Transportphase eher zu den
// Farbgrenzen hingezogen.
//
// Sobald echte Pigmentablagerung und Verdunstung existieren, darf dieses
// Modell erneut überprüft und physikalisch sinnvoller aufgebaut werden.
//
// ============================================================================


Texture2D<float4> kuwaharaTexture : register(t0);


// ============================================================================
// Vertex-Ausgabe
// ============================================================================

struct VertexOutput
{
    float4 position : SV_POSITION;
};


// ============================================================================
// Fullscreen Triangle
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


    output.position = float4(positions[vertexId], 0.0, 1.0);
    
    return output;
}


// ============================================================================
// Kuwahara-Farbe lesen
// ============================================================================
//
// Wir verwenden bewusst Load().
//
// Die WaterMap soll nicht auf interpolierten Farben beruhen, sondern auf den
// tatsächlich vorhandenen Kuwahara-Texeln.
//
// Randkoordinaten werden geklemmt.
// ============================================================================

float3 ReadKuwaharaColor(int2 pixelPosition, int2 textureSize)
{
    int2 clampedPosition;
    
    clampedPosition = clamp(pixelPosition, int2(0, 0), textureSize - int2(1, 1));
    
    return kuwaharaTexture.Load(int3(clampedPosition, 0)).rgb;
}


// ============================================================================
// Farbdifferenz
// ============================================================================
//
// Länge des RGB-Differenzvektors.
//
// Da RGB-Komponenten zwischen 0 und 1 liegen, beträgt die theoretische
// Maximallänge:
//
//      sqrt(3)
//
// Durch Division mit sqrt(3) erhalten wir ungefähr einen Bereich:
//
//      0.0 ... 1.0
//
// ============================================================================

float CalculateColorDifference(float3 colorA, float3 colorB)
{
    const float inverseMaxRgbDistance = 0.57735026919;
    
    return length(colorA - colorB) * inverseMaxRgbDistance;
}


// ============================================================================
// Niedrigfrequente Grundvariation
// ============================================================================
//
// Ein echter Maler verteilt Wasser nicht mathematisch perfekt gleichmäßig.
//
// Wir wollen deshalb eine SEHR leichte räumliche Variation erzeugen.
//
// Noch verwenden wir dafür bewusst kein FBM.
//
// Das hier ist lediglich eine billige deterministische Approximation aus zwei
// Sinusfeldern.
//
// Wichtig:
//
// Die Variation bleibt klein gegenüber den eigentlichen Kuwahara-Strukturen.
// Sie soll das Wasser lediglich etwas "lebendig" machen.
//
// ============================================================================

float CalculateWaterVariation(int2 pixelPosition)
{
    float x;
    float y;

    float variationA;
    float variationB;

    float variation;


    x = (float) pixelPosition.x;
    y = (float) pixelPosition.y;


    // ------------------------------------------------------------------------
    // Zwei unterschiedlich große Wellenlängen verhindern ein zu deutlich
    // sichtbares regelmäßiges Muster.
    // ------------------------------------------------------------------------

    variationA = sin(x / 71.0) * sin(y / 89.0);
    variationB = sin((x + y) / 137.0) * sin((x - y) / 113.0);
    
    variation = variationA * 0.65 + variationB * 0.35;
    
    // Ergebnis ungefähr -1 ... +1.

    return variation;
}


// ============================================================================
// PixelShader
// ============================================================================

float PSMain(VertexOutput input) : SV_TARGET
{
    // ------------------------------------------------------------------------
    // Die Werte hier sind ausdrücklich erste PoC-Werte.
    //
    // baseWater
    //      Mittlerer Wasserstand.
    //
    // variationStrength
    //      Kleine natürliche Grundvariation.
    //
    // edgeDepth
    //      Maximale zusätzliche Absenkung an einer Kuwahara-Kante.
    //
    // edgeSampleDistance
    //      Abstand, in dem nach Farbunterschieden gesucht wird.
    //
    // 8 Pixel entsprechen bewusst ungefähr der derzeitigen räumlichen
    // Reichweite unseres WaterFlowShader.
    // ------------------------------------------------------------------------

    const float baseWater = 0.22;

    const float variationStrength = 0.025;

    const float edgeDepth = 0.06;

    const int edgeSampleDistance = 8;


    // ------------------------------------------------------------------------
    // Schwellwerte für die EdgeMask.
    //
    // Kleine Farbunterschiede innerhalb einer Kuwahara-Fläche sollen
    // ignoriert werden.
    //
    // Erst deutlichere Farbwechsel sollen eine relevante Wassersenke
    // erzeugen.
    // ------------------------------------------------------------------------

    const float edgeThresholdLow = 0.025;

    const float edgeThresholdHigh = 0.15;


    uint textureWidth;
    uint textureHeight;

    int2 textureSize;
    int2 pixelPosition;


    float3 colorCenter;

    float3 colorNorth;
    float3 colorEast;
    float3 colorSouth;
    float3 colorWest;


    float differenceNorth;
    float differenceEast;
    float differenceSouth;
    float differenceWest;

    float maximumDifference;

    float edgeMask;

    float waterVariation;

    float water;


    // ========================================================================
    // Texturdimensionen
    // ========================================================================

    kuwaharaTexture.GetDimensions(textureWidth, textureHeight);
    
    textureSize = int2(textureWidth, textureHeight);
    
    pixelPosition = int2(input.position.xy);


    // ========================================================================
    // Kuwahara-Farben lesen
    // ========================================================================

    colorCenter = ReadKuwaharaColor(pixelPosition, textureSize);
    
    colorNorth =  ReadKuwaharaColor(pixelPosition + int2(0, -edgeSampleDistance), textureSize);
    colorEast =   ReadKuwaharaColor(pixelPosition + int2(edgeSampleDistance, 0), textureSize);
    colorSouth =  ReadKuwaharaColor(pixelPosition + int2(0, edgeSampleDistance), textureSize);
    colorWest =   ReadKuwaharaColor(pixelPosition + int2(-edgeSampleDistance, 0), textureSize);

    // ========================================================================
    // Lokale Farbunterschiede bestimmen
    // ========================================================================

    differenceNorth = CalculateColorDifference(colorCenter, colorNorth);
    differenceEast =  CalculateColorDifference(colorCenter, colorEast);
    differenceSouth = CalculateColorDifference(colorCenter, colorSouth);
    differenceWest =  CalculateColorDifference(colorCenter, colorWest);
    
    // ------------------------------------------------------------------------
    // Uns interessiert zunächst die stärkste lokale Grenze.
    //
    // Dadurch reagiert die WaterMap auch dann deutlich, wenn beispielsweise
    // nur östlich des Pixels eine Kuwahara-Farbgrenze liegt.
    // ------------------------------------------------------------------------

    maximumDifference = max(max(differenceNorth, differenceEast), max(differenceSouth, differenceWest));


    // ========================================================================
    // EdgeMask
    // ========================================================================

    edgeMask = smoothstep(edgeThresholdLow, edgeThresholdHigh, maximumDifference);


    // ========================================================================
    // Natürliche Grundvariation
    // ========================================================================

    waterVariation = CalculateWaterVariation(pixelPosition);


    // ========================================================================
    // Initialer Wasserstand
    // ========================================================================
    //
    //             Grundwasser
    //
    //                 +
    //
    //         kleine räumliche Variation
    //
    //                 -
    //
    //          Senke an Kuwahara-Kanten
    //
    // ========================================================================

    water = baseWater + waterVariation * variationStrength - edgeMask * edgeDepth;


    // ------------------------------------------------------------------------
    // Sicherheitsgurt.
    //
    // Negative Wassermengen wollen wir selbstverständlich nicht erzeugen.
    // Nach oben ist derzeit keine künstliche Begrenzung notwendig.
    // ------------------------------------------------------------------------

    water = max(water, 0.0);


    return water;
}