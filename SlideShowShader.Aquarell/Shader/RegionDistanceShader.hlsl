// ============================================================================
// RegionDistanceShader.hlsl
// ============================================================================
//
// Ermittelt aus dem Kuwahara-Bild die Entfernung jedes Pixels zur nächsten
// erkannten Kuwahara-Grenze.
//
// EIN Shader, vier Betriebsmodi:
//
//      MODE_INITIALIZE = 0
//          Grenzpixel erkennen und als Seeds initialisieren.
//
//      MODE_PROPAGATE = 1
//          Jump-Flood / Relaxationsschritt.
//
//      MODE_FINALIZE = 2
//          Seed-Koordinate in echte Pixeldistanz umwandeln.
//
//      MODE_DISPLAY = 3
//          RegionDistanceMap als Graustufenbild visualisieren.
//
// ---------------------------------------------------------------------------
// Seed-Texturen:
//
//      RG = Pixelkoordinate des aktuell nächsten bekannten Grenzpixels.
//
// Ungültiger / noch unbekannter Seed:
//
//      (-1, -1)
//
// ---------------------------------------------------------------------------
// ChangedCounter:
//
//      RWStructuredBuffer<uint> auf register(u1)
//
// Bei jeder ECHTEN Verbesserung der Distanz wird atomar um 1 erhöht.
//
// Wichtig:
// Wir zählen keine bloßen Änderungen der Seed-ID bei gleicher Entfernung.
// Sonst könnten äquidistante Seeds den Algorithmus unnötig am Leben halten.
// ============================================================================


// ============================================================================
// Konstanten
// ============================================================================

static const uint MODE_INITIALIZE = 0;
static const uint MODE_PROPAGATE = 1;
static const uint MODE_FINALIZE = 2;
static const uint MODE_DISPLAY = 3;

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
//      Sprungweite des Jump-Flood-Passes.
//
// regionColorThreshold
//      Mindestfarbunterschied für eine Kuwahara-Grenze.
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

// Aktueller Seed-Zustand.
Texture2D<float2> sourceSeedTexture : register(t1);

// Fertige rohe Distanzkarte.
Texture2D<float> regionDistanceTexture : register(t2);


// ============================================================================
// Atomic Changed Counter
// ============================================================================
//
// Da gleichzeitig EIN RenderTarget auf OM-Slot 0 gebunden ist,
// muss der Pixelshader-UAV ab Slot 1 beginnen.
//
// Deshalb ausdrücklich register(u1).
// ============================================================================

RWStructuredBuffer<uint> changedCounter : register(u1);


// ============================================================================
// Vertex Shader
// ============================================================================

struct VSOutput
{
    float4 position : SV_POSITION;
};


VSOutput VSMain(uint vertexId : SV_VertexID)
{
    VSOutput output;

    float2 position;


    if (vertexId == 0)
    {
        position = float2(-1.0F, -1.0F);
    }
    else if (vertexId == 1)
    {
        position = float2(-1.0F, 3.0F);
    }
    else
    {
        position = float2(3.0F, -1.0F);
    }


    output.position =
        float4(
            position,
            0.0F,
            1.0F);

    return output;
}


// ============================================================================
// Hilfsfunktionen
// ============================================================================

bool IsInsideTexture(
    int2 pixelPosition,
    uint2 textureSize)
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


float3 ReadKuwaharaColor(
    int2 pixelPosition)
{
    return
        kuwaharaTexture.Load(
            int3(
                pixelPosition,
                0)).rgb;
}


float CalculateColorDistanceSquared(
    float3 colorA,
    float3 colorB)
{
    float3 difference;

    difference =
        colorA -
        colorB;

    return
        dot(
            difference,
            difference);
}


// ============================================================================
// Regionsgrenze erkennen
// ============================================================================
//
// Ein Pixel ist Grenzpixel, wenn:
//
//      - er am äußeren Bildrand liegt
//
//      ODER
//
//      - mindestens einer seiner acht direkten Nachbarn einen ausreichend
//        großen Kuwahara-Farbunterschied besitzt.
//
// Dadurch erledigt dieser Shader gleichzeitig die Aufgabe eines separaten
// RegionBoundaryShaders.
//
// EIN Shader. Versprochen. ;-)
// ============================================================================

bool IsRegionBoundary(
    int2 pixelPosition,
    uint2 textureSize)
{
    float3 centerColor;
    float3 neighborColor;

    float thresholdSquared;
    float colorDistanceSquared;

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


    centerColor =
        ReadKuwaharaColor(
            pixelPosition);

    thresholdSquared =
        regionColorThreshold *
        regionColorThreshold;


    [unroll]
    for (int neighborIndex = 0;
         neighborIndex < 8;
         neighborIndex++)
    {
        neighborPosition =
            pixelPosition +
            neighborOffsets[neighborIndex];


        // ------------------------------------------------------------
        // Äußerer Bildrand gilt ebenfalls als Regionsgrenze.
        // ------------------------------------------------------------

        if (!IsInsideTexture(
                neighborPosition,
                textureSize))
        {
            return true;
        }


        neighborColor =
            ReadKuwaharaColor(
                neighborPosition);


        colorDistanceSquared =
            CalculateColorDistanceSquared(
                centerColor,
                neighborColor);


        if (colorDistanceSquared >
            thresholdSquared)
        {
            return true;
        }
    }


    return false;
}


// ============================================================================
// Seed prüfen
// ============================================================================

bool IsValidSeed(
    float2 seed)
{
    return
        seed.x >= 0.0F &&
        seed.y >= 0.0F;
}


// ============================================================================
// Quadratische Distanz Pixel -> Seed
// ============================================================================
//
// Für den Vergleich brauchen wir keine Quadratwurzel.
// ============================================================================

float CalculateSeedDistanceSquared(
    float2 pixelPosition,
    float2 seed)
{
    float2 difference;

    difference =
        pixelPosition -
        seed;

    return
        dot(
            difference,
            difference);
}


// ============================================================================
// Kandidaten-Seed bewerten
// ============================================================================

void EvaluateSeedCandidate(
    int2 candidatePosition,
    uint2 textureSize,
    float2 pixelPosition,
    inout float2 bestSeed,
    inout float bestDistanceSquared)
{
    float2 candidateSeed;
    float candidateDistanceSquared;


    if (!IsInsideTexture(
            candidatePosition,
            textureSize))
    {
        return;
    }


    candidateSeed =
        sourceSeedTexture.Load(
            int3(
                candidatePosition,
                0));


    if (!IsValidSeed(candidateSeed))
    {
        return;
    }


    candidateDistanceSquared =
        CalculateSeedDistanceSquared(
            pixelPosition,
            candidateSeed);


    if (candidateDistanceSquared <
        bestDistanceSquared)
    {
        bestDistanceSquared =
            candidateDistanceSquared;

        bestSeed =
            candidateSeed;
    }
}


// ============================================================================
// Pixel Shader
// ============================================================================

float4 PSMain(
    VSOutput input) : SV_TARGET
{
    uint textureWidth;
    uint textureHeight;

    uint2 textureSize;

    int2 pixelPosition;

    float2 pixelPositionFloat;

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

    kuwaharaTexture.GetDimensions(
        textureWidth,
        textureHeight);

    textureSize =
        uint2(
            textureWidth,
            textureHeight);


    pixelPosition =
        int2(
            input.position.xy);

    pixelPositionFloat =
        float2(
            pixelPosition);


    // ========================================================================
    // MODE 0
    // Grenzerkennung / Seed-Initialisierung
    // ========================================================================

    if (mode == MODE_INITIALIZE)
    {
        if (IsRegionBoundary(
                pixelPosition,
                textureSize))
        {
            // Dieses Pixel IST eine Kuwahara-Grenze.
            //
            // Der nächste bekannte Grenzpunkt ist damit es selbst.

            return
                float4(
                    pixelPositionFloat,
                    0.0F,
                    1.0F);
        }


        // Noch keine Grenze bekannt.

        return
            float4(
                INVALID_SEED,
                INVALID_SEED,
                0.0F,
                1.0F);
    }


    // ========================================================================
    // MODE 1
    // Jump Flood / Relaxation
    // ========================================================================

    if (mode == MODE_PROPAGATE)
    {
        currentSeed =
            sourceSeedTexture.Load(
                int3(
                    pixelPosition,
                    0));


        bestSeed =
            currentSeed;


        if (IsValidSeed(currentSeed))
        {
            currentDistanceSquared =
                CalculateSeedDistanceSquared(
                    pixelPositionFloat,
                    currentSeed);

            bestDistanceSquared =
                currentDistanceSquared;
        }
        else
        {
            currentDistanceSquared =
                3.402823466E+38F;

            bestDistanceSquared =
                3.402823466E+38F;
        }


        step =
            max(
                int(jumpStep),
                1);


        // ------------------------------------------------------------
        // Acht Nachbarbereiche in aktueller Sprungweite prüfen.
        // ------------------------------------------------------------

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


        // ------------------------------------------------------------
        // ChangedCount nur bei einer ECHTEN Verbesserung.
        //
        // Ein anderer Seed mit exakt derselben Distanz zählt NICHT.
        //
        // Dadurch verhindern wir Oszillation zwischen äquidistanten
        // Grenz-Seeds.
        // ------------------------------------------------------------

        if (bestDistanceSquared + 0.0001F <
            currentDistanceSquared)
        {
            InterlockedAdd(
                changedCounter[0],
                1,
                previousCounterValue);
        }


        return
            float4(
                bestSeed,
                0.0F,
                1.0F);
    }


    // ========================================================================
    // MODE 2
    // Finale Seed-Koordinate -> echte Pixeldistanz
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
            // Sollte nach erfolgreicher Konvergenz niemals passieren.
            //
            // Sehr großer Diagnosewert statt stillschweigendem 0.

            finalDistance =
                65535.0F;
        }
        else
        {
            finalDistance =
                length(
                    pixelPositionFloat -
                    currentSeed);
        }


        return
            float4(
                finalDistance,
                0.0F,
                0.0F,
                1.0F);
    }


    // ========================================================================
    // MODE 3
    // Debug-Darstellung
    // ========================================================================

    finalDistance =
        regionDistanceTexture.Load(
            int3(
                pixelPosition,
                0));


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