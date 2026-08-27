// ============================================================================
// BilateralShader.hlsl
//
// Bilaterale Glättung für die Aquarell-Vorverarbeitung.
//
// Ziel:
//     - lokale Bilddetails reduzieren
//     - starke Farb-/Helligkeitskanten möglichst erhalten
//     - zusammenhängendere Farbflächen erzeugen
//
// Algorithmische Grundlage:
//
//     Bilateral Filtering
//
//     Tomasi, C.; Manduchi, R.
//     "Bilateral Filtering for Gray and Color Images"
//     Proceedings of ICCV, 1998.
//
// Grundidee:
//
// Für jedes Nachbarpixel werden zwei Gewichte kombiniert:
//
//     1. Spatial Weight
//        Wie weit liegt das Sample geometrisch vom Mittelpunkt entfernt?
//
//     2. Range Weight
//        Wie stark unterscheidet sich die Farbe vom Mittelpunkt?
//
// Nur Pixel, die sowohl räumlich nahe als auch farblich ähnlich sind,
// tragen stark zum Ergebnis bei.
//
// Dadurch wird innerhalb homogener Bereiche geglättet, während deutliche
// Kanten wesentlich weniger verwischt werden.
//
// Diese Implementierung dient zunächst ausschließlich dem Vergleichstest
// innerhalb von Aquarell V0.1.
// ============================================================================


Texture2D sourceTexture : register(t0);

SamplerState sourceSampler : register(s0);


// ============================================================================
// Parameter
// ============================================================================
//
// Für den ersten Vergleich bewusst fest codiert.
//
// radius
//     Filterradius in Pixeln.
//
// sigmaSpatial
//     Stärke der geometrischen Gewichtung.
//
// sigmaRange
//     Stärke der Farbähnlichkeitsgewichtung.
//
// Kleinere sigmaRange-Werte:
//     stärkere Kantenerhaltung.
//
// Größere sigmaRange-Werte:
//     stärkere Vermischung über Farbgrenzen hinweg.
// ============================================================================

static const int radius = 4;

static const float sigmaSpatial = 3.0;
static const float sigmaRange = 0.10;


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
// ============================================================================

VertexOutput VSMain(uint vertexId : SV_VertexID)
{
    VertexOutput output;

    float2 positions[3] =
    {
        float2(-1.0, -1.0),
        float2(-1.0,  3.0),
        float2( 3.0, -1.0)
    };

    float2 texCoords[3] =
    {
        float2(0.0,  1.0),
        float2(0.0, -1.0),
        float2(2.0,  1.0)
    };


    output.position = float4(positions[vertexId], 0.0, 1.0);

    output.texCoord = texCoords[vertexId];


    return output;
}


// ============================================================================
// Hilfsfunktion: Gauß-Gewicht
// ============================================================================
//
// exp(-(x² / (2 * sigma²)))
//
// Wird sowohl für räumliche Distanz als auch Farbdistanz verwendet.
// ============================================================================

float GaussianWeight(float distanceSquared, float sigma)
{
    float sigmaSquared;

    sigmaSquared =  sigma * sigma;

    return exp(-distanceSquared / (2.0 * sigmaSquared));
}


// ============================================================================
// PixelShader
// ============================================================================

float4 PSMain(VertexOutput input) : SV_TARGET
{
    float2 textureSize;
    float2 texelSize;

    float4 centerColor;
    float4 sampleColor;

    float3 colorDifference;

    float spatialDistanceSquared;
    float rangeDistanceSquared;

    float spatialWeight;
    float rangeWeight;
    float combinedWeight;

    float4 accumulatedColor;
    float accumulatedWeight;

    float2 sampleOffset;
    float2 sampleUV;

    int x;
    int y;


    // ------------------------------------------------------------------------
    // Texture-Dimensionen bestimmen.
    //
    // GetDimensions liefert hier die tatsächliche Auflösung der Eingabetextur.
    // Damit bleibt der Filter unabhängig von der Bildschirmauflösung.
    // ------------------------------------------------------------------------

    sourceTexture.GetDimensions(textureSize.x, textureSize.y);

    texelSize = 1.0 / textureSize;


    // ------------------------------------------------------------------------
    // Mittelpunkt lesen.
    // ------------------------------------------------------------------------

    centerColor = sourceTexture.SampleLevel(sourceSampler, input.texCoord, 0.0);


    // ------------------------------------------------------------------------
    // Akkumulatoren initialisieren.
    // ------------------------------------------------------------------------

    accumulatedColor = float4(0.0, 0.0, 0.0, 0.0);

    accumulatedWeight = 0.0;


    // ------------------------------------------------------------------------
    // Bilateraler Kernel.
    // ------------------------------------------------------------------------

    for (y = -radius; y <= radius; y++)
    {
        for (x = -radius; x <= radius; x++)
        {
            // ---------------------------------------------------------------
            // Sampleposition.
            // ---------------------------------------------------------------

            sampleOffset = float2((float)x, (float)y);

            sampleUV = input.texCoord + sampleOffset * texelSize;


            // ---------------------------------------------------------------
            // Nachbarfarbe lesen.
            // ---------------------------------------------------------------

            sampleColor = sourceTexture.SampleLevel(sourceSampler, sampleUV, 0.0);


            // ---------------------------------------------------------------
            // Räumliche Distanz.
            // ---------------------------------------------------------------

            spatialDistanceSquared = dot(sampleOffset, sampleOffset);


            // ---------------------------------------------------------------
            // Farbdistanz.
            //
            // Alpha wird bewusst ignoriert.
            // Für unsere Quellbilder ist nur RGB relevant.
            // ---------------------------------------------------------------

            colorDifference = sampleColor.rgb - centerColor.rgb;

            rangeDistanceSquared = dot(colorDifference, colorDifference);


            // ---------------------------------------------------------------
            // Gewichte berechnen.
            // ---------------------------------------------------------------

            spatialWeight = GaussianWeight(spatialDistanceSquared, sigmaSpatial);

            rangeWeight = GaussianWeight(rangeDistanceSquared, sigmaRange);

            combinedWeight = spatialWeight * rangeWeight;


            // ---------------------------------------------------------------
            // Akkumulieren.
            // ---------------------------------------------------------------

            accumulatedColor += sampleColor * combinedWeight;

            accumulatedWeight += combinedWeight;
        }
    }


    // ------------------------------------------------------------------------
    // Normalisieren.
    // ------------------------------------------------------------------------

    if (accumulatedWeight > 0.000001)
    {
        accumulatedColor /= accumulatedWeight;
    }
    else
    {
        accumulatedColor = centerColor;
    }


    // ------------------------------------------------------------------------
    // Alpha des Originalpixels beibehalten.
    // ------------------------------------------------------------------------

    accumulatedColor.a = centerColor.a;


    return accumulatedColor;
}