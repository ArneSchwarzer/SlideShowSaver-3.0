// ============================================================================
// PigmentFlowShader.hlsl
// ============================================================================
//
// SlideShowSaver 3.0
// Shader: Aquarell
//
// Aufgabe:
//
// Transportiert die bereits initialisierten Pigmente entsprechend dem
// lokalen Wassergefälle.
//
// Dies ist bewusst KEINE vollständige physikalische Pigmentsimulation.
//
// Für diesen ersten Proof-of-Concept interessiert ausschließlich:
//
//      "Folgen die Pigmente sichtbar dem sich bewegenden Wasser?"
//
// Noch NICHT enthalten:
//
//      - Pigmentablagerung
//      - Papierabsorption
//      - Pigmentgrößen
//      - Granulation
//      - Verdunstung
//      - Kantenablagerung
//      - echte Farbmischung
//
// Pigmentdarstellung:
//
//      RGB = Pigmentfarbe * Pigmentmenge
//      A   = Pigmentmenge
//
// Die RGB-Werte sind also bereits mit der Pigmentmenge vormultipliziert.
//
// ============================================================================


Texture2D<float4> sourcePigment : register(t0);
Texture2D<float> sourceWater : register(t1);


// ============================================================================
// VERTEX OUTPUT
// ============================================================================

struct VertexOutput
{
    float4 position : SV_POSITION;
};


// ============================================================================
// FULLSCREEN TRIANGLE
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
// WASSER LESEN
// ============================================================================

float ReadWater(int2 pixelPosition,
                int2 textureSize)
{
    int2 clampedPosition;

    clampedPosition =
        clamp(pixelPosition,
              int2(0, 0),
              textureSize - int2(1, 1));

    return sourceWater.Load(int3(clampedPosition, 0));
}


// ============================================================================
// PIGMENT LESEN
// ============================================================================

float4 ReadPigment(int2 pixelPosition,
                   int2 textureSize)
{
    int2 clampedPosition;

    clampedPosition =
        clamp(pixelPosition,
              int2(0, 0),
              textureSize - int2(1, 1));

    return sourcePigment.Load(int3(clampedPosition, 0));
}


// ============================================================================
// PIXEL SHADER
// ============================================================================

float4 PSMain(VertexOutput input) : SV_TARGET
{
    uint textureWidth;
    uint textureHeight;

    int2 textureSize;
    int2 pixelPosition;

    float waterNorth;
    float waterEast;
    float waterSouth;
    float waterWest;

    float2 waterGradient;
    float gradientLength;
    float2 flowDirection;

    float transportStrength;
    float transportDistance;

    float2 sourceOffset;
    int2 sourcePosition;

    float4 pigmentCenter;
    float4 pigmentTransported;
    float4 pigmentNew;


    // ------------------------------------------------------------------------
    // Texturgröße bestimmen.
    // ------------------------------------------------------------------------

    sourceWater.GetDimensions(textureWidth, textureHeight);

    textureSize = int2(textureWidth, textureHeight);

    pixelPosition = int2(input.position.xy);


    // ------------------------------------------------------------------------
    // Aktuelles Pigment lesen.
    // ------------------------------------------------------------------------

    pigmentCenter =
        ReadPigment(pixelPosition,
                    textureSize);


    // ------------------------------------------------------------------------
    // Lokales Wassergefälle bestimmen.
    //
    // Wir verwenden absichtlich dieselbe räumliche Größenordnung wie unser
    // inzwischen kalibrierter WaterFlowShader.
    //
    // Dadurch "sieht" der Pigmenttransport ungefähr dieselben Wasserstrukturen
    // wie der eigentliche Wasserpass.
    // ------------------------------------------------------------------------

    const int flowDistance = 8;

    waterNorth =
        ReadWater(pixelPosition +
                  int2(0, -flowDistance),
                  textureSize);

    waterEast =
        ReadWater(pixelPosition +
                  int2(flowDistance, 0),
                  textureSize);

    waterSouth =
        ReadWater(pixelPosition +
                  int2(0, flowDistance),
                  textureSize);

    waterWest =
        ReadWater(pixelPosition +
                  int2(-flowDistance, 0),
                  textureSize);


    // ------------------------------------------------------------------------
    // Gradient:
    //
    // positive X-Richtung:
    //      Osten besitzt mehr Wasser als Westen
    //
    // positive Y-Richtung:
    //      Süden besitzt mehr Wasser als Norden
    //
    // Wasser soll von hohen zu niedrigen Bereichen fließen.
    // Deshalb verwenden wir anschließend -gradient.
    // ------------------------------------------------------------------------

    waterGradient =
        float2(waterEast - waterWest,
               waterSouth - waterNorth);


    gradientLength =
        length(waterGradient);


    // ------------------------------------------------------------------------
    // In vollständig ebenem Wasser gibt es keine sinnvolle Flussrichtung.
    //
    // Ohne diesen Schutz würde normalize(float2(0,0)) numerisch unnötig
    // problematisch.
    // ------------------------------------------------------------------------

    if (gradientLength < 0.000001)
    {
        return pigmentCenter;
    }


    flowDirection =
        -waterGradient / gradientLength;


    // ------------------------------------------------------------------------
    // TRANSPORTSTÄRKE
    // ------------------------------------------------------------------------
    //
    // Noch KEIN Benutzerparameter.
    //
    // Wir wollen heute Nacht lediglich sehen, ob Pigmente tatsächlich
    // schwimmen.
    //
    // Die Stärke hängt vom Wassergefälle ab:
    //
    //      kleines Gefälle -> wenig Bewegung
    //      großes Gefälle  -> mehr Bewegung
    //
    // Für unseren PoC wird der Effekt absichtlich deutlich sichtbar gehalten.
    // ------------------------------------------------------------------------

    const float maximumTransportStrength = 0.65;
    const float gradientAmplification = 4.0;

    transportStrength =
        saturate(gradientLength * gradientAmplification);

    transportStrength *=
        maximumTransportStrength;


    // ------------------------------------------------------------------------
    // TRANSPORTDISTANZ
    // ------------------------------------------------------------------------
    //
    // Wir bewegen Pigment nicht gleich um ganze flowDistance Pixel.
    //
    // Der Wassergradient wird zwar über 8 Pixel gemessen, das Pigment soll
    // innerhalb EINER Iteration aber nur einen kleinen Schritt zurücklegen.
    //
    // Über unsere 16 Iterationen akkumuliert sich daraus eine deutlich
    // sichtbare Bewegung.
    // ------------------------------------------------------------------------

    const float maximumTransportDistance = 3.0;

    transportDistance =
        maximumTransportDistance *
        saturate(gradientLength * gradientAmplification);


    // ------------------------------------------------------------------------
    // PULL-ADVEKTION
    // ------------------------------------------------------------------------
    //
    // Wir berechnen das Pigment, das am aktuellen Zielpixel ankommen soll.
    //
    // Wenn das Wasser beispielsweise nach rechts fließt, fragen wir:
    //
    //      "Welches Pigment befand sich etwas links von mir?"
    //
    // Deshalb wird entgegen der Flussrichtung gelesen.
    // ------------------------------------------------------------------------

    sourceOffset =
        -flowDirection *
        transportDistance;


    sourcePosition =
        pixelPosition +
        int2(round(sourceOffset));


    pigmentTransported =
        ReadPigment(sourcePosition,
                    textureSize);


    // ------------------------------------------------------------------------
    // Aktuelles und transportiertes Pigment mischen.
    //
    // Da sowohl RGB als auch A gemeinsam interpoliert werden, bleibt unsere
    // vormultiplizierte Pigmentdarstellung konsistent.
    // ------------------------------------------------------------------------

    pigmentNew =
        lerp(pigmentCenter,
             pigmentTransported,
             transportStrength);


    // ------------------------------------------------------------------------
    // Numerischer Sicherheitsgurt.
    // ------------------------------------------------------------------------

    pigmentNew =
        max(pigmentNew,
            float4(0.0, 0.0, 0.0, 0.0));


    return pigmentNew;
}