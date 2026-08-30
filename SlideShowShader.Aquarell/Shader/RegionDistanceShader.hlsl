// ============================================================================
// RegionDistanceShader.hlsl
// ============================================================================
//
// Erzeugt aus dem Kuwahara-Bild echte zusammenhängende Farbregionen
// und berechnet anschließend für jedes Pixel die Entfernung zur nächsten
// Regionsgrenze.
//
// Pipeline:
//
//      MODE_REGION_INITIALIZE = 0
//          Jedes Pixel startet als eigene Region.
//
//      MODE_REGION_GROW = 1
//          Regionen wachsen deterministisch zusammen.
//          Verglichen wird gegen den Repräsentanten der Kandidatenregion,
//          NICHT bloß gegen den direkten Nachbarpixel.
//
//      MODE_BOUNDARY = 2
//          Aus den stabilen Regions-Labels werden echte Grenz-Seeds erzeugt.
//
//      MODE_PROPAGATE = 3
//          Jump-Flood / Relaxationsschritt der Distance Transformation.
//
//      MODE_FINALIZE = 4
//          Seed-Koordinate in echte Pixeldistanz umwandeln.
//
//      MODE_DISPLAY = 5
//          RegionDistanceMap als Graustufenbild visualisieren.
//
// ---------------------------------------------------------------------------
// Region-Label-Texturen:
//
//      RG = Pixelkoordinate des Regionsrepräsentanten.
//
// ---------------------------------------------------------------------------
// Seed-Texturen:
//
//      RG = Pixelkoordinate des aktuell nächsten bekannten Grenzpixels.
//
// Ungültiger Distance-Seed:
//
//      (-1, -1)
//
// ---------------------------------------------------------------------------
// ChangedCounter:
//
//      RWStructuredBuffer<uint> auf register(u1)
//
// Wird sowohl für REGION_GROW als auch für PROPAGATE benutzt.
// ============================================================================


// ============================================================================
// Konstanten
// ============================================================================

static const uint MODE_REGION_INITIALIZE = 0;
static const uint MODE_REGION_GROW = 1;
static const uint MODE_BOUNDARY = 2;
static const uint MODE_PROPAGATE = 3;
static const uint MODE_FINALIZE = 4;
static const uint MODE_DISPLAY = 5;

static const float INVALID_SEED = -1.0F;


// ============================================================================
// Constant Buffer
// ============================================================================
//
// Exakt 16 Byte.
//
// mode
//      Betriebsmodus.
//
// jumpStep
//      Sprungweite der Distance-Propagation.
//      Für Region-Growing derzeit unbenutzt.
//
// regionColorThreshold
//      Maximale Farbdistanz zwischen einem Pixel und dem
//      Repräsentanten einer Kandidatenregion.
//
// displayDistanceScale
//      Distanz, die auf dem Kontrollmonitor als Weiß dargestellt wird.
// ============================================================================

cbuffer RegionDistanceConstants : register(b0)
{
    uint mode;
    uint jumpStep;

    float regionColorThreshold;
    float displayDistanceScale;
};


// ============================================================================
// Eingaben
// ============================================================================

// Kuwahara-Ergebnis.
Texture2D<float4> kuwaharaTexture : register(t0);

// Aktueller Distance-Seed-Zustand.
Texture2D<float2> sourceSeedTexture : register(t1);

// Fertige rohe Distanzkarte.
Texture2D<float> regionDistanceTexture : register(t2);

// Aktueller Region-Label-Zustand.
//
// RG = Pixelkoordinate des Regionsrepräsentanten.
Texture2D<float2> sourceRegionLabelTexture : register(t3);


// ============================================================================
// Atomic Changed Counter
// ============================================================================
//
// Da gleichzeitig EIN RenderTarget auf OM-Slot 0 gebunden ist,
// beginnt der Pixelshader-UAV bei Slot 1.
// ============================================================================

RWStructuredBuffer<uint> changedCounter : register(u1);


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

    float2 position;
    float2 texCoord;


    if (vertexId == 0)
    {
        position = float2(-1.0F, -1.0F);
        texCoord = float2(0.0F, 1.0F);
    }
    else if (vertexId == 1)
    {
        position = float2(-1.0F, 3.0F);
        texCoord = float2(0.0F, -1.0F);
    }
    else
    {
        position = float2(3.0F, -1.0F);
        texCoord = float2(2.0F, 1.0F);
    }


    output.position = float4(position, 0.0F, 1.0F);

    output.texCoord = texCoord;

    return output;
}

// ============================================================================
// Allgemeine Hilfsfunktionen
// ============================================================================

bool IsInsideTexture(int2 pixelPosition, int2 textureSize)
{
    if (pixelPosition.x < 0)
    {
        return false;
    }

    if (pixelPosition.y < 0)
    {
        return false;
    }

    if (pixelPosition.x >= int(textureSize.x))
    {
        return false;
    }

    if (pixelPosition.y >= int(textureSize.y))
    {
        return false;
    }

    return true;
}


float3 ReadKuwaharaColor(int2 pixelPosition)
{
    return kuwaharaTexture.Load(int3(pixelPosition, 0)).rgb;
}


float CalculateColorDistanceSquared(float3 colorA, float3 colorB)
{
    float3 difference;

    difference = colorA - colorB;

    return dot(difference, difference);
}


// ============================================================================
// Region-Label-Hilfsfunktionen
// ============================================================================

float2 ReadRegionLabel(int2 pixelPosition)
{
    return sourceRegionLabelTexture.Load(int3(pixelPosition, 0));
}


bool IsSameRegion(float2 regionA, float2 regionB)
{
    return
        regionA.x == regionB.x &&
        regionA.y == regionB.y;
}


// Deterministische Priorität.
//
// Der lexikographisch kleinere Regionsrepräsentant gewinnt:
//
//      zuerst kleineres Y,
//      bei gleichem Y kleineres X.
//
// Dadurch kann der Region-Grow-Pass nicht zwischen gleichwertigen
// Kandidaten hin- und herspringen.

bool IsRegionSeedSmaller(float2 candidateSeed, float2 currentSeed)
{
    if (candidateSeed.y < currentSeed.y)
    {
        return true;
    }

    if (candidateSeed.y > currentSeed.y)
    {
        return false;
    }

    return candidateSeed.x < currentSeed.x;
}


// Prüft, ob ein benachbarter Regionsrepräsentant für das aktuelle
// Pixel übernommen werden darf.

void EvaluateRegionCandidate(int2 neighborPosition, uint2 textureSize, float3 currentPixelColor, 
                             float thresholdSquared, inout float2 bestRegionSeed)
{
    float2 candidateRegionSeed;
    int2 candidateRegionPosition;
    float3 candidateRegionColor;
    float colorDistanceSquared;
    
    if (!IsInsideTexture(neighborPosition, textureSize))
    {
        return;
    }


    candidateRegionSeed = ReadRegionLabel(neighborPosition);
    
    // ------------------------------------------------------------
    // Nur Kandidaten betrachten, die nach unserer deterministischen
    // Ordnung überhaupt "kleiner" sind als die bisher beste Region.
    // ------------------------------------------------------------

    if (!IsRegionSeedSmaller(candidateRegionSeed, bestRegionSeed))
    {
        return;
    }


    candidateRegionPosition = int2(candidateRegionSeed);


    if (!IsInsideTexture(candidateRegionPosition, textureSize))
    {
        return;
    }


    // ------------------------------------------------------------
    // WICHTIG:
    //
    // Wir vergleichen NICHT:
    //
    //      aktueller Pixel <-> Nachbarpixel
    //
    // sondern:
    //
    //      aktueller Pixel <-> Repräsentant der Kandidatenregion
    //
    // Dadurch verhindern wir das schleichende Color-Chaining über
    // viele kleine lokale Farbunterschiede.
    // ------------------------------------------------------------

    candidateRegionColor = ReadKuwaharaColor(candidateRegionPosition);

    colorDistanceSquared = CalculateColorDistanceSquared(currentPixelColor, candidateRegionColor);
    
    if (colorDistanceSquared <= thresholdSquared)
    {
        bestRegionSeed = candidateRegionSeed;
    }
}


// ============================================================================
// Echte Regionsgrenze erkennen
// ============================================================================
//
// Anders als die alte Version wird hier KEIN Farbunterschied mehr geprüft.
//
// Ein Pixel ist genau dann Regionsgrenze, wenn mindestens ein direkter
// Nachbar zu einer anderen stabilen Region gehört.
//
// Auch der äußere Bildrand gilt als Grenze.
// ============================================================================

bool IsRegionBoundary(int2 pixelPosition, uint2 textureSize)
{
    float2 myRegion;
    float2 neighborRegion;

    int2 neighborPosition;

    static const int2 neighborOffsets[8] =
    {
        int2(-1, -1),
        int2(0, -1),
        int2(1, -1),

        int2(-1, 0),
        int2(1, 0),

        int2(-1, 1),
        int2(0, 1),
        int2(1, 1)
    };


    myRegion = ReadRegionLabel(pixelPosition);
    
    [unroll]
    for (int neighborIndex = 0; neighborIndex < 8; neighborIndex++)
    {
        neighborPosition = pixelPosition + neighborOffsets[neighborIndex];
        
        if (!IsInsideTexture(neighborPosition, textureSize))
        {
            return true;
        }
        
        neighborRegion = ReadRegionLabel(neighborPosition);
        
        if (!IsSameRegion(myRegion, neighborRegion))
        {
            return true;
        }
    }
    
    return false;
}


// ============================================================================
// Distance-Seed-Hilfsfunktionen
// ============================================================================

bool IsValidSeed(float2 seed)
{
    return
        seed.x >= 0.0F &&
        seed.y >= 0.0F;
}


float CalculateSeedDistanceSquared(float2 pixelPosition, float2 seed)
{
    float2 difference;

    difference = pixelPosition - seed;

    return dot(difference, difference);
}


void EvaluateSeedCandidate(int2 candidatePosition, uint2 textureSize, float2 pixelPosition, inout float2 bestSeed,
                           inout float bestDistanceSquared)
{
    float2 candidateSeed;
    float candidateDistanceSquared;
    
    if (!IsInsideTexture(candidatePosition, textureSize))
    {
        return;
    }
    
    candidateSeed = sourceSeedTexture.Load(int3(candidatePosition, 0));

    if (!IsValidSeed(candidateSeed))
    {
        return;
    }
    
    candidateDistanceSquared = CalculateSeedDistanceSquared(pixelPosition, candidateSeed);

    if (candidateDistanceSquared < bestDistanceSquared)
    {
        bestDistanceSquared = candidateDistanceSquared;

        bestSeed = candidateSeed;
    }
}


// ============================================================================
// Pixel Shader
// ============================================================================

float4 PSMain(VSOutput input) : SV_TARGET
{
    uint textureWidth;
    uint textureHeight;

    uint2 textureSize;

    int2 pixelPosition;
    float2 pixelPositionFloat;

    float3 currentPixelColor;

    float2 currentRegionSeed;
    float2 bestRegionSeed;

    float thresholdSquared;

    float2 currentSeed;
    float2 bestSeed;

    float currentDistanceSquared;
    float bestDistanceSquared;

    float finalDistance;
    float displayValue;

    uint previousCounterValue;

    int step;


    // ------------------------------------------------------------------------
    // Dimensionen bestimmen.
    // ------------------------------------------------------------------------

    kuwaharaTexture.GetDimensions(textureWidth, textureHeight);

    textureSize = uint2(textureWidth, textureHeight);
    
    pixelPosition = int2(input.position.xy);

    pixelPositionFloat = float2(pixelPosition);


    // ========================================================================
    // MODE 0
    // Region Labels initialisieren
    //
    // Jedes Pixel startet als eigene Region.
    // Der Repräsentant ist zunächst die eigene Pixelkoordinate.
    // ========================================================================

    if (mode == MODE_REGION_INITIALIZE)
    {
        return float4(pixelPositionFloat, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE 1
    // Regionen wachsen lassen
    // ========================================================================

    if (mode == MODE_REGION_GROW)
    {
        currentPixelColor = ReadKuwaharaColor(pixelPosition);

        currentRegionSeed = ReadRegionLabel(pixelPosition);
        
        bestRegionSeed = currentRegionSeed;
        
        thresholdSquared = regionColorThreshold * regionColorThreshold;


        // ------------------------------------------------------------
        // Acht direkte Nachbarn prüfen.
        // ------------------------------------------------------------

        EvaluateRegionCandidate(
            pixelPosition + int2(-1, -1),
            textureSize,
            currentPixelColor,
            thresholdSquared,
            bestRegionSeed);

        EvaluateRegionCandidate(
            pixelPosition + int2(0, -1),
            textureSize,
            currentPixelColor,
            thresholdSquared,
            bestRegionSeed);

        EvaluateRegionCandidate(
            pixelPosition + int2(1, -1),
            textureSize,
            currentPixelColor,
            thresholdSquared,
            bestRegionSeed);
        
        EvaluateRegionCandidate(
            pixelPosition + int2(-1, 0),
            textureSize,
            currentPixelColor,
            thresholdSquared,
            bestRegionSeed);

        EvaluateRegionCandidate(
            pixelPosition + int2(1, 0),
            textureSize,
            currentPixelColor,
            thresholdSquared,
            bestRegionSeed);
        
        EvaluateRegionCandidate(
            pixelPosition + int2(-1, 1),
            textureSize,
            currentPixelColor,
            thresholdSquared,
            bestRegionSeed);

        EvaluateRegionCandidate(
            pixelPosition + int2(0, 1),
            textureSize,
            currentPixelColor,
            thresholdSquared,
            bestRegionSeed);

        EvaluateRegionCandidate(
            pixelPosition + int2(1, 1),
            textureSize,
            currentPixelColor,
            thresholdSquared,
            bestRegionSeed);


        // ------------------------------------------------------------
        // Nur bei echter Regionsänderung ChangedCount erhöhen.
        // ------------------------------------------------------------

        if (!IsSameRegion(bestRegionSeed, currentRegionSeed))
        {
            InterlockedAdd(changedCounter[0], 1, previousCounterValue);
        }
        
        return float4(bestRegionSeed, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE 2
    // Aus den stabilen Regionen echte Boundary-Seeds erzeugen
    // ========================================================================

    if (mode == MODE_BOUNDARY)
    {
        if (IsRegionBoundary(pixelPosition, textureSize))
        {
            return float4(pixelPositionFloat, 0.0F, 1.0F);
        }


        return float4(INVALID_SEED, INVALID_SEED, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE 3
    // Jump Flood / Relaxation
    // ========================================================================

    if (mode == MODE_PROPAGATE)
    {
        currentSeed = sourceSeedTexture.Load(int3(pixelPosition, 0));
        
        bestSeed = currentSeed;
        
        if (IsValidSeed(currentSeed))
        {
            currentDistanceSquared = CalculateSeedDistanceSquared(pixelPositionFloat, currentSeed);

            bestDistanceSquared = currentDistanceSquared;
        }
        else
        {
            currentDistanceSquared = 3.402823466E+38F;

            bestDistanceSquared = 3.402823466E+38F;
        }
        
        step = max(int(jumpStep), 1);
        
        EvaluateSeedCandidate(
            pixelPosition + int2(-step, -step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(0, -step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(step, -step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);
        
        EvaluateSeedCandidate(
            pixelPosition + int2(-step, 0),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(step, 0),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);
        
        EvaluateSeedCandidate(
            pixelPosition + int2(-step, step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(0, step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(step, step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);
        
        if (bestDistanceSquared + 0.0001F < currentDistanceSquared)
        {
            InterlockedAdd(changedCounter[0], 1, previousCounterValue);
        }
        
        return float4(bestSeed, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE 4
    // Seed-Koordinate -> echte Pixeldistanz
    //
    // Diagnose:
    //
    //      -1.0 = Zu diesem Pixel ist KEIN gültiger Grenz-Seed gelangt.
    //
    // regionDistanceTexture ist R32_FLOAT. Deshalb speichern wir hier
    // bewusst keinen RGB-Testfarbwert. Die eigentliche Warnfarbe wird
    // erst im MODE_DISPLAY aus diesem Sentinel erzeugt.
    // ========================================================================

    if (mode == MODE_FINALIZE)
    {
        currentSeed =
            sourceSeedTexture.Load(
                int3(
                    pixelPosition,
                    0));


        if (!IsValidSeed(currentSeed))
        {
            return
                float4(
                    -1.0F,
                    0.0F,
                    0.0F,
                    1.0F);
        }


        finalDistance =
            length(
                pixelPositionFloat -
                currentSeed);


        return
            float4(
                finalDistance,
                0.0F,
                0.0F,
                1.0F);
    }


    // ========================================================================
    // MODE 5
    // Diagnose-/Kontrollansicht
    //
    // Farbcode:
    //
    //      MAGENTA
    //          Kein gültiger Distance-Seed angekommen.
    //          regionDistanceTexture enthält -1.0.
    //
    //      CYAN
    //          Exakter Grenzpixel bzw. praktisch Distanz 0.
    //
    //      GRAUSTUFEN
    //          Gültige Distanz.
    //          Schwarz = nahe an der Grenze
    //          Weiß    = weit von der Grenze entfernt
    //
    // Falls ein unbekannter Mode ankommt:
    //
    //      GELB
    // ========================================================================

    if (mode == MODE_DISPLAY)
    {
        uint displayWidth;
        uint displayHeight;

        int2 displayPixelPosition;


        regionDistanceTexture.GetDimensions(
            displayWidth,
            displayHeight);


        displayPixelPosition =
            int2(
                saturate(input.texCoord) *
                float2(
                    displayWidth - 1,
                    displayHeight - 1));


        finalDistance =
            regionDistanceTexture.Load(
                int3(
                    displayPixelPosition,
                    0));


        // ------------------------------------------------------------
        // MAGENTA:
        // Kein gültiger Seed hat dieses Pixel erreicht.
        // ------------------------------------------------------------

        if (finalDistance < 0.0F)
        {
            return
                float4(
                    1.0F,
                    0.0F,
                    1.0F,
                    1.0F);
        }


        // ------------------------------------------------------------
        // CYAN:
        // Grenzpixel / Distanz praktisch Null.
        //
        // Damit sehen wir sofort, wo die tatsächlich erzeugten Seeds
        // liegen und können sie von normalen dunklen Gradienten
        // unterscheiden.
        // ------------------------------------------------------------

        if (finalDistance <= 0.5F)
        {
            return
                float4(
                    0.0F,
                    1.0F,
                    1.0F,
                    1.0F);
        }


        // ------------------------------------------------------------
        // Gültige Distanz:
        // normale Graustufenanzeige.
        // ------------------------------------------------------------

        displayValue =
            saturate(
                finalDistance /
                max(
                    displayDistanceScale,
                    1.0F));


        return
            float4(
                displayValue,
                displayValue,
                displayValue,
                1.0F);
    }


    // ========================================================================
    // Sicherheitsnetz
    //
    // GELB bedeutet:
    // Ein unbekannter oder falsch gesetzter mode hat PSMain erreicht.
    // Dieser Fall darf regulär niemals auftreten.
    // ========================================================================

    return
        float4(
            1.0F,
            1.0F,
            0.0F,
            1.0F);
}