// ============================================================================
// KuwaharaShader.hlsl
//
// Klassischer Kuwahara-Filter für die Aquarell-Vorverarbeitung.
//
// Ziel:
//     - fotografische Detailstruktur deutlich vereinfachen
//     - größere zusammenhängende Farbflächen erzeugen
//     - starke Objekt- und Farbgrenzen möglichst erhalten
//
// Algorithmische Grundlage:
//
//     Klassischer Kuwahara-Filter.
//
// Der Filter betrachtet um jeden Pixel vier überlappende Teilregionen:
//
//     Nordwest
//     Nordost
//     Südwest
//     Südost
//
// Für jede Region werden berechnet:
//
//     - mittlere RGB-Farbe
//     - RGB-Varianz
//
// Anschließend wird die Region mit der geringsten Gesamtvarianz gewählt.
// Deren mittlere Farbe wird als Ausgabefarbe verwendet.
//
// Dadurch werden homogene Bereiche stark geglättet, während Bereiche auf
// unterschiedlichen Seiten einer Kante nicht einfach miteinander vermischt
// werden.
//
// Diese Implementierung dient zunächst ausschließlich dem Vergleichstest
// innerhalb von Aquarell V0.1.
//
// Literaturhinweis:
//
// Der Kuwahara-Filter wurde ursprünglich in den 1970er Jahren für
// kantenbewahrende Bildglättung entwickelt und später vielfach für
// nichtfotorealistische Bildabstraktion und malerische Effekte verwendet.
//
// Für die spätere Aquarell-Pipeline ist besonders interessant, dass Kuwahara
// nicht einfach "weichzeichnet", sondern Bildregionen aktiv zu homogeneren
// Farbflächen zusammenfasst.
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
//     Radius jeder Teilregion in Pixeln.
//
// Bei radius = 6 besitzt jede Region:
//
//     7 x 7 = 49 Samples
//
// inklusive Mittelpunkt.
//
// Der Mittelpunkt gehört bewusst zu allen vier Regionen.
//
// Größerer Radius:
//
//     - stärkere Flächenbildung
//     - stärkerer Detailverlust
//     - eventuell blockigeres Erscheinungsbild
//
// Kleinerer Radius:
//
//     - subtilerer Effekt
//     - mehr fotografische Struktur bleibt erhalten
// ============================================================================

static const int radius = 32;


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
        float2(-1.0, 3.0),
        float2(3.0, -1.0)
    };

    float2 texCoords[3] =
    {
        float2(0.0, 1.0),
        float2(0.0, -1.0),
        float2(2.0, 1.0)
    };


    output.position = float4(positions[vertexId], 0.0, 1.0);

    output.texCoord = texCoords[vertexId];
    
    return output;
}


// ============================================================================
// Hilfsstruktur für eine Kuwahara-Region
// ============================================================================

struct RegionResult
{
    float3 meanColor;
    float varianceScore;
};


// ============================================================================
// Berechnet Mittelwert und Varianz einer rechteckigen Teilregion.
//
// startX / endX
// startY / endY
//
// sind Pixeloffsets relativ zum aktuellen Mittelpunkt.
//
// Beispiel Nordwest:
//
//     startX = -radius
//     endX   = 0
//     startY = -radius
//     endY   = 0
//
// ============================================================================

RegionResult EvaluateRegion(float2 centerUV, float2 texelSize, int startX, int endX, int startY, int endY)
{
    RegionResult result;

    float3 sumColor;
    float3 sumSquaredColor;

    float3 meanColor;
    float3 variance;

    float3 sampleColor;

    float2 sampleUV;

    int sampleCount;

    int x;
    int y;


    sumColor = float3(0.0, 0.0, 0.0);

    sumSquaredColor = float3(0.0, 0.0, 0.0);

    sampleCount = 0;


    for (y = startY; y <= endY; y++)
    {
        for (x = startX; x <= endX; x++)
        {
            sampleUV = centerUV + float2((float) x, (float) y) * texelSize;

            sampleColor = sourceTexture.SampleLevel(sourceSampler, sampleUV, 0.0).rgb;

            sumColor += sampleColor;

            sumSquaredColor += sampleColor * sampleColor;

            sampleCount++;
        }
    }


    meanColor = sumColor / (float) sampleCount;


    // ------------------------------------------------------------------------
    // Varianz:
    //
    //     Var(X) = E(X²) - E(X)²
    //
    // getrennt für R, G und B.
    // ------------------------------------------------------------------------

    variance = sumSquaredColor / (float) sampleCount - meanColor * meanColor;


    // ------------------------------------------------------------------------
    // Numerische Rundungsfehler können theoretisch minimale negative Werte
    // erzeugen.
    //
    // Deshalb sicherheitshalber auf >= 0 begrenzen.
    // ------------------------------------------------------------------------

    variance = max(variance, float3(0.0, 0.0, 0.0));

    result.meanColor = meanColor;


    // ------------------------------------------------------------------------
    // Gesamtbewertung der Region.
    //
    // Für den ersten Test verwenden wir bewusst die Summe der RGB-Varianzen.
    //
    // Damit bewertet der Filter echte Farbunterschiede und nicht nur
    // Helligkeitsunterschiede.
    // ------------------------------------------------------------------------

    result.varianceScore = variance.r + variance.g + variance.b;
    
    return result;
}


// ============================================================================
// PixelShader
// ============================================================================

float4 PSMain(VertexOutput input) : SV_TARGET
{
    uint textureWidth;
    uint textureHeight;

    float2 texelSize;

    float4 centerColor;

    RegionResult regionNW;
    RegionResult regionNE;
    RegionResult regionSW;
    RegionResult regionSE;

    float3 selectedColor;
    float selectedVariance;


    // ------------------------------------------------------------------------
    // Tatsächliche Eingabetexturgröße bestimmen.
    // ------------------------------------------------------------------------

    sourceTexture.GetDimensions(textureWidth, textureHeight);

    texelSize = 1.0 / float2((float) textureWidth, (float) textureHeight);


    // ------------------------------------------------------------------------
    // Originalpixel lesen.
    //
    // Alpha übernehmen wir später unverändert.
    // ------------------------------------------------------------------------

    centerColor = sourceTexture.SampleLevel(sourceSampler, input.texCoord, 0.0);


    // ========================================================================
    // Vier überlappende Kuwahara-Regionen
    // ========================================================================


    // ------------------------------------------------------------------------
    // Nordwest
    // ------------------------------------------------------------------------

    regionNW = EvaluateRegion(input.texCoord, texelSize, -radius, 0, -radius, 0);


    // ------------------------------------------------------------------------
    // Nordost
    // ------------------------------------------------------------------------

    regionNE = EvaluateRegion(input.texCoord, texelSize, 0, radius, -radius, 0);


    // ------------------------------------------------------------------------
    // Südwest
    // ------------------------------------------------------------------------

    regionSW = EvaluateRegion(input.texCoord, texelSize, -radius, 0, 0, radius);


    // ------------------------------------------------------------------------
    // Südost
    // ------------------------------------------------------------------------

    regionSE = EvaluateRegion(input.texCoord, texelSize, 0, radius, 0, radius);


    // ========================================================================
    // Homogenste Region auswählen
    // ========================================================================

    selectedColor = regionNW.meanColor;

    selectedVariance = regionNW.varianceScore;


    if (regionNE.varianceScore < selectedVariance)
    {
        selectedColor = regionNE.meanColor;

        selectedVariance =regionNE.varianceScore;
    }


    if (regionSW.varianceScore < selectedVariance)
    {
        selectedColor = regionSW.meanColor;

        selectedVariance = regionSW.varianceScore;
    }


    if (regionSE.varianceScore < selectedVariance)
    {
        selectedColor = regionSE.meanColor;

        selectedVariance = regionSE.varianceScore;
    }


    // ------------------------------------------------------------------------
    // Alpha des Originalpixels beibehalten.
    // ------------------------------------------------------------------------

    return float4(selectedColor, centerColor.a);
}