// ============================================================================
// PressureInitializerShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Erzeugt aus der RegionDistanceMap den initialen Wasserdruck p.
//
// RegionDistance:
//
//      0       = Regionsgrenze
//      größer  = weiter im Inneren einer Region
//
// Pressure:
//
//      0       = kein initialer Druck an der Regionsgrenze
//      nahe 1  = hohe initiale Wasserhöhe im Regionsinneren
//
// ----------------------------------------------------------------------------
// Problem der früheren linearen Abbildung
// ----------------------------------------------------------------------------
//
// Frühere Formel:
//
//      pressure = distance / pressureDistanceScale
//
// Dadurch hing die erreichbare Wasserhöhe unmittelbar von der absoluten
// Größe einer Region ab.
//
// Beispiel bei scale = 64:
//
//      kleine Region, maxDistance = 8
//          -> maxPressure = 0.125
//
//      große Region, maxDistance = 64
//          -> maxPressure = 1.0
//
// Große Regionen erhielten damit systematisch sehr viel stärkere
// Druckreservoirs als kleine Regionen.
//
// ----------------------------------------------------------------------------
// Neue sättigende Abbildung
// ----------------------------------------------------------------------------
//
//      pressure = 1 - exp(-distance / scale)
//
// Die Funktion steigt anfangs relativ schnell an und nähert sich anschließend
// asymptotisch dem Wert 1.
//
// Dadurch:
//
//      - erhalten kleine Regionen deutlich mehr Anfangsdruck
//      - bleiben Regionsgrenzen weiterhin exakt bei 0
//      - werden große Regionen nicht zusätzlich bestraft
//      - verlieren sehr große Regionen ihren proportionalen Größenvorteil
//
// Diese Näherung ersetzt noch KEINE echte regionsweise Normalisierung.
//
// Eine spätere vollständig geometrisch normierte Variante könnte lauten:
//
//      normalizedDistance =
//          distanceToBoundary / maximumDistanceOfRegion
//
// Dafür müsste RegionDistance jedoch zusätzlich pro Region den maximalen
// Distanzwert kennen.
//
// Für den aktuellen Aquarell-Shader ist die sättigende Näherung bewusst
// einfacher und ausreichend.
//
// Spätere Anflanschpunkte:
//
//      - PaperMap
//      - lokale Wassermenge
//      - zufällige Wash-Variation
//      - weitere nichtlineare Druckkurven
//
// ============================================================================


// ============================================================================
// Constant Buffer
// ============================================================================
//
// Exakt 16 Byte.
// ============================================================================

cbuffer PressureInitializerConstants : register(b0)
{
    float pressureDistanceScale;

    float reserve1;
    float reserve2;
    float reserve3;
};


// ============================================================================
// Eingaben
// ============================================================================

Texture2D<float> regionDistanceTexture : register(t0);


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


    output.position =
        float4(
            position,
            0.0F,
            1.0F);

    output.texCoord =
        texCoord;


    return output;
}


// ============================================================================
// Pixel Shader
// ============================================================================

float PSMain(VSOutput input) : SV_TARGET
{
    int2 pixelPosition;

    float regionDistance;
    float safeScale;

    float pressure;


    pixelPosition =
        int2(
            input.position.xy);


    regionDistance =
        regionDistanceTexture.Load(
            int3(
                pixelPosition,
                0));


    // ========================================================================
    // Sicherheitsnetz
    //
    // RegionDistance sollte regulär niemals negativ sein.
    // ========================================================================

    regionDistance =
        max(
            regionDistance,
            0.0F);


    safeScale =
        max(
            pressureDistanceScale,
            0.0001F);


    // ========================================================================
    // Distanz -> initialer Wasserdruck
    //
    // Neue sättigende Kurve:
    //
    //      p = 1 - exp(-d / scale)
    //
    // Eigenschaften:
    //
    //      d = 0
    //          -> p = 0
    //
    //      d steigt
    //          -> p steigt schnell
    //
    //      d wird sehr groß
    //          -> p nähert sich 1
    //
    // Dadurch bekommen kleine Regionen relativ deutlich mehr Anfangsdruck,
    // während große Regionen nicht mehr proportional immer mächtiger werden.
    // ========================================================================

    pressure =
        1.0F -
        exp(
            -regionDistance /
            safeScale);


    // ========================================================================
    // Numerisches Sicherheitsnetz
    // ========================================================================

    pressure =
        saturate(
            pressure);


    return pressure;
}