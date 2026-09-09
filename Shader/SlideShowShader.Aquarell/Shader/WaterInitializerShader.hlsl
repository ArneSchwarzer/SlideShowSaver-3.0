// ============================================================================
// WaterInitializerShader.hlsl
//
// SlideShowSaver 3.0
// Shader: Aquarell
//
// V0.x - Multi-Scale-Distance-WaterMap
//
// Aufgabe:
//
// Aus dem bereits durch Classic Kuwahara vereinfachten Bild wird ein
// Wasserpotential erzeugt.
//
// Anders als die bisherige WaterMap erzeugt diese Version NICHT nur ein
// breiteres Band um Kuwahara-Kanten.
//
// Stattdessen wird für jedes Pixel näherungsweise bestimmt:
//
//      Wie weit kann ich mich vom Pixel entfernen,
//      bevor ich auf eine deutlich andere Kuwahara-Farbfläche treffe?
//
// Daraus entsteht:
//
//      nahe an einer Grenze
//          -> niedriger Wasserstand
//
//      weit im Inneren einer Fläche
//          -> hoher Wasserstand
//
// Der PigmentFlowShader kann dadurch Pigmente aus den Zentren großer
// Kuwahara-Flächen in Richtung ihrer Grenzen transportieren.
//
// WICHTIG:
//
// Diese WaterMap ist weiterhin KEINE physikalisch korrekte Simulation.
// Sie ist ein bewusst künstlerisch konstruiertes Potentialfeld.
//
// Für diesen Test gibt es ausdrücklich:
//
//      KEIN Sinusfeld
//      KEIN Rauschen
//      KEINE Papierstruktur
//
// Damit lässt sich die Wirkung der Distanz-WaterMap isoliert beurteilen.
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
        float2(-1.0F, -1.0F),
        float2(-1.0F, 3.0F),
        float2(3.0F, -1.0F)
    };


    output.position = float4(positions[vertexId], 0.0F, 1.0F);

    return output;
}


// ============================================================================
// Kuwahara-Farbe lesen
// ============================================================================

float3 ReadKuwaharaColor(
    int2 pixelPosition,
    int2 textureSize)
{
    int2 clampedPosition;


    clampedPosition =
        clamp(
            pixelPosition,
            int2(0, 0),
            textureSize - int2(1, 1)
        );


    return kuwaharaTexture.Load(
        int3(
            clampedPosition,
            0
        )
    ).rgb;
}


// ============================================================================
// Farbdifferenz
// ============================================================================
//
// RGB-Abstand normiert ungefähr auf:
//
//      0.0 ... 1.0
//
// ============================================================================

float CalculateColorDifference(
    float3 colorA,
    float3 colorB)
{
    const float inverseMaximumRgbDistance = 0.57735026919F;


    return
        length(colorA - colorB)
        * inverseMaximumRgbDistance;
}


// ============================================================================
// Maximale Farbdifferenz in acht Richtungen
// ============================================================================
//
// Es werden nicht nur Nord/Ost/Süd/West geprüft, sondern zusätzlich die
// vier Diagonalen.
//
// Dadurch ist die Distanzsuche weniger abhängig von der Orientierung einer
// Kuwahara-Grenze.
//
// ============================================================================

float CalculateMaximumDifferenceAtDistance(
    int2 pixelPosition,
    int2 textureSize,
    float3 centerColor,
    int sampleDistance)
{
    float differenceNorth;
    float differenceNorthEast;
    float differenceEast;
    float differenceSouthEast;
    float differenceSouth;
    float differenceSouthWest;
    float differenceWest;
    float differenceNorthWest;

    float maximumDifference;


    differenceNorth =
        CalculateColorDifference(
            centerColor,
            ReadKuwaharaColor(
                pixelPosition + int2(0, -sampleDistance),
                textureSize
            )
        );


    differenceNorthEast =
        CalculateColorDifference(
            centerColor,
            ReadKuwaharaColor(
                pixelPosition + int2(sampleDistance, -sampleDistance),
                textureSize
            )
        );


    differenceEast =
        CalculateColorDifference(
            centerColor,
            ReadKuwaharaColor(
                pixelPosition + int2(sampleDistance, 0),
                textureSize
            )
        );


    differenceSouthEast =
        CalculateColorDifference(
            centerColor,
            ReadKuwaharaColor(
                pixelPosition + int2(sampleDistance, sampleDistance),
                textureSize
            )
        );


    differenceSouth =
        CalculateColorDifference(
            centerColor,
            ReadKuwaharaColor(
                pixelPosition + int2(0, sampleDistance),
                textureSize
            )
        );


    differenceSouthWest =
        CalculateColorDifference(
            centerColor,
            ReadKuwaharaColor(
                pixelPosition + int2(-sampleDistance, sampleDistance),
                textureSize
            )
        );


    differenceWest =
        CalculateColorDifference(
            centerColor,
            ReadKuwaharaColor(
                pixelPosition + int2(-sampleDistance, 0),
                textureSize
            )
        );


    differenceNorthWest =
        CalculateColorDifference(
            centerColor,
            ReadKuwaharaColor(
                pixelPosition + int2(-sampleDistance, -sampleDistance),
                textureSize
            )
        );


    maximumDifference =
        max(
            max(
                max(differenceNorth, differenceNorthEast),
                max(differenceEast, differenceSouthEast)
            ),
            max(
                max(differenceSouth, differenceSouthWest),
                max(differenceWest, differenceNorthWest)
            )
        );


    return maximumDifference;
}


// ============================================================================
// Kantenprüfung für eine Entfernung
// ============================================================================
//
// Sobald der Farbunterschied den Schwellwert überschreitet, behandeln wir
// diese Entfernung als:
//
//      "Hier wurde erstmals eine andere Kuwahara-Fläche erreicht."
//
// ============================================================================

bool EdgeFoundAtDistance(
    int2 pixelPosition,
    int2 textureSize,
    float3 centerColor,
    int sampleDistance,
    float edgeThreshold)
{
    float maximumDifference;


    maximumDifference =
        CalculateMaximumDifferenceAtDistance(
            pixelPosition,
            textureSize,
            centerColor,
            sampleDistance
        );


    return maximumDifference >= edgeThreshold;
}


// ============================================================================
// Pixel Shader
// ============================================================================

float PSMain(VertexOutput input) : SV_TARGET
{
    // ========================================================================
    // TESTPARAMETER
    // ========================================================================
    //
    // baseWaterEdge
    //
    //      Wasserstand unmittelbar an einer Kuwahara-Grenze.
    //
    //
    // baseWaterInterior
    //
    //      Wasserstand in sehr großen Flächen, bei denen selbst in 512 Pixel
    //      Entfernung keine deutlich andere Kuwahara-Farbe gefunden wurde.
    //
    //
    // edgeThreshold
    //
    //      Wie groß muss ein Farbunterschied sein, damit er als Übergang zu
    //      einer anderen Kuwahara-Fläche gilt?
    //
    // ========================================================================

    const float baseWaterEdge = 0.12F;

    const float baseWaterInterior = 0.30F;

    const float edgeThreshold = 0.055F;


    uint textureWidth;
    uint textureHeight;

    int2 textureSize;
    int2 pixelPosition;

    float3 centerColor;

    float distanceFactor;

    float water;


    // ========================================================================
    // Texturdimensionen
    // ========================================================================

    kuwaharaTexture.GetDimensions(
        textureWidth,
        textureHeight
    );


    textureSize =
        int2(
            textureWidth,
            textureHeight
        );


    pixelPosition =
        int2(
            input.position.xy
        );


    // ========================================================================
    // Ausgangsfarbe
    // ========================================================================

    centerColor =
        ReadKuwaharaColor(
            pixelPosition,
            textureSize
        );


    // ========================================================================
    // Multi-Scale-Distanzsuche
    // ========================================================================
    //
    // WICHTIG:
    //
    // Die Reihenfolge ist Teil des Algorithmus.
    //
    // Sobald eine Grenze gefunden wurde, werden größere Entfernungen NICHT
    // mehr berücksichtigt.
    //
    // Dadurch gilt:
    //
    //      once edge, always edge
    //
    // und nicht mehr:
    //
    //      "Bei 32 Pixeln war eine Grenze,
    //       aber bei 64 Pixeln sieht es zufällig wieder ähnlich aus."
    //
    //
    // distanceFactor:
    //
    //      0.00 -> direkt an Grenze
    //      ...
    //      1.00 -> sehr tief innerhalb einer großen Fläche
    //
    // ========================================================================

    if (EdgeFoundAtDistance(
            pixelPosition,
            textureSize,
            centerColor,
            8,
            edgeThreshold))
    {
        distanceFactor = 0.00F;
    }
    else if (EdgeFoundAtDistance(
                 pixelPosition,
                 textureSize,
                 centerColor,
                 16,
                 edgeThreshold))
    {
        distanceFactor = 0.12F;
    }
    else if (EdgeFoundAtDistance(
                 pixelPosition,
                 textureSize,
                 centerColor,
                 32,
                 edgeThreshold))
    {
        distanceFactor = 0.25F;
    }
    else if (EdgeFoundAtDistance(
                 pixelPosition,
                 textureSize,
                 centerColor,
                 64,
                 edgeThreshold))
    {
        distanceFactor = 0.40F;
    }
    else if (EdgeFoundAtDistance(
                 pixelPosition,
                 textureSize,
                 centerColor,
                 128,
                 edgeThreshold))
    {
        distanceFactor = 0.58F;
    }
    else if (EdgeFoundAtDistance(
                 pixelPosition,
                 textureSize,
                 centerColor,
                 256,
                 edgeThreshold))
    {
        distanceFactor = 0.78F;
    }
    else if (EdgeFoundAtDistance(
                 pixelPosition,
                 textureSize,
                 centerColor,
                 512,
                 edgeThreshold))
    {
        distanceFactor = 0.92F;
    }
    else
    {
        distanceFactor = 1.00F;
    }


    // ========================================================================
    // Distanz -> Wasserpotential
    // ========================================================================
    //
    // Große Kuwahara-Fläche:
    //
    //              hoher Wasserstand
    //
    //                      |
    //                      v
    //
    //      Zentrum ---------------------- Zentrum
    //                 \              /
    //                  \            /
    //                   \          /
    //                    \        /
    //                     \      /
    //                      Kante
    //
    //
    // Der PigmentFlowShader soll dadurch von den Flächenzentren in Richtung
    // der Grenzen transportieren.
    //
    // smoothstep sorgt dafür, dass die ohnehin nur grob geschätzten
    // Distanzstufen nicht vollständig linear in Wasserhöhe übersetzt werden.
    // ========================================================================

    distanceFactor =
        smoothstep(
            0.0F,
            1.0F,
            distanceFactor
        );


    water =
        lerp(
            baseWaterEdge,
            baseWaterInterior,
            distanceFactor
        );


    // ========================================================================
    // Sicherheitsgurt
    // ========================================================================

    water = max(water, 0.0F);


    return water;
}