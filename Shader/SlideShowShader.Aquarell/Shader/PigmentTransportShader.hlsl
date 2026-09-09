// ============================================================================
// PigmentTransportShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Transportiert suspendiertes Pigment zusammen mit dem Wasser.
//
// Curtis-nahe Interpretation:
//
//      p   = lokale Wasserhöhe / Pressure
//      g^k = Pigmentkonzentration im Wasser
//
// Die Pigmenttextur speichert derzeit:
//
//      RGB = premultiplizierte Pigmentfarbe / Konzentration
//      A   = Pigmentkonzentration
//
// Entscheidend:
//
//      Pigment wird NICHT unabhängig vom Wasser mit
//
//          velocity * pigment
//
//      transportiert.
//
// Stattdessen wird zuerst derselbe Wasserfluss bestimmt, der auch im
// PressureFlowShader verwendet wird:
//
//          waterFlux = velocity * pressure
//
// und daran die Pigmentkonzentration gekoppelt:
//
//          pigmentFlux = waterFlux * pigmentConcentration
//
// Dadurch transportieren Wasser und Pigment dieselbe Flüssigkeitsmenge.
//
// Die konservierte Größe während des Transportes ist damit:
//
//          pigmentSurfaceMass = pressure * concentration
//
// Nach dem Transport wird aus:
//
//          neue Pigmentflächenmasse
//          ------------------------
//             neuer Pressure
//
// wieder die lokale Pigmentkonzentration bestimmt.
//
// Für diesen Entwicklungsstand noch NICHT enthalten:
//
//      - PigmentDeposit
//      - Adsorption
//      - Desorption
//      - Papierabhängige Granulation
//      - Capillary Layer
//
// ============================================================================


// ============================================================================
// Constant Buffer
//
// exakt 16 Byte
// ============================================================================

cbuffer PigmentTransportConstants : register(b0)
{
    float timeStep;
    float transportStrength;
    float minimumPressure;
    float reserve1;
};


// ============================================================================
// Eingaben
//
// t0 = Pigmentkonzentration zum Zeitpunkt n
// t1 = Velocity für diese Simulationszeitscheibe
// t2 = Pressure zum Zeitpunkt n
// t3 = bereits berechneter Pressure zum Zeitpunkt n + 1
// ============================================================================

Texture2D<float4> pigmentTexture : register(t0);
Texture2D<float2> velocityTexture : register(t1);
Texture2D<float> oldPressureTexture : register(t2);
Texture2D<float> newPressureTexture : register(t3);


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
// Hilfsfunktionen
// ============================================================================

int2 ClampPixelPosition( int2 pixelPosition, uint2 textureSize)
{
    return clamp(pixelPosition, int2(0, 0), int2(int(textureSize.x) - 1, int(textureSize.y) - 1));
}


float4 ReadPigment(int2 pixelPosition, uint2 textureSize)
{
    return pigmentTexture.Load(int3(ClampPixelPosition(pixelPosition, textureSize), 0));
}


float2 ReadVelocity(int2 pixelPosition, int2 textureSize)
{
    return velocityTexture.Load(int3(ClampPixelPosition(pixelPosition, textureSize), 0));
}


float ReadOldPressure(int2 pixelPosition, uint2 textureSize)
{
    return oldPressureTexture.Load(int3(ClampPixelPosition(pixelPosition, textureSize), 0));
}


float ReadNewPressure(int2 pixelPosition, uint2 textureSize)
{
    return newPressureTexture.Load(int3(ClampPixelPosition(pixelPosition, textureSize), 0));
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


    // ========================================================================
    // Pigmentkonzentrationen
    // ========================================================================

    float4 pigmentCenter;
    float4 pigmentLeft;
    float4 pigmentRight;
    float4 pigmentUp;
    float4 pigmentDown;


    // ========================================================================
    // Pressure n
    // ========================================================================

    float pressureCenter;
    float pressureLeft;
    float pressureRight;
    float pressureUp;
    float pressureDown;


    // ========================================================================
    // Pressure n + 1
    // ========================================================================

    float newPressureCenter;


    // ========================================================================
    // Velocity
    // ========================================================================

    float2 velocityCenter;
    float2 velocityLeft;
    float2 velocityRight;
    float2 velocityUp;
    float2 velocityDown;


    // ========================================================================
    // Geschwindigkeit an Zellflächen
    // ========================================================================

    float velocityFaceLeft;
    float velocityFaceRight;
    float velocityFaceUp;
    float velocityFaceDown;


    // ========================================================================
    // Wasserfluss an Zellflächen
    // ========================================================================

    float waterFluxLeft;
    float waterFluxRight;
    float waterFluxUp;
    float waterFluxDown;


    // ========================================================================
    // Pigmentfluss an Zellflächen
    // ========================================================================

    float4 pigmentFluxLeft;
    float4 pigmentFluxRight;
    float4 pigmentFluxUp;
    float4 pigmentFluxDown;


    // ========================================================================
    // Transportzustand
    // ========================================================================

    float4 oldPigmentSurfaceMass;
    float4 pigmentFluxDivergence;
    float4 newPigmentSurfaceMass;

    float4 newPigmentConcentration;


    // ========================================================================
    // Texturgröße / aktuelle Pixelposition
    // ========================================================================

    pigmentTexture.GetDimensions(textureWidth, textureHeight);
    
    textureSize = uint2(textureWidth, textureHeight);

    pixelPosition = int2(input.position.xy);


    // ========================================================================
    // Pigmentkonzentration zum Zeitpunkt n
    // ========================================================================

    pigmentCenter = ReadPigment(pixelPosition, textureSize);
    pigmentLeft =   ReadPigment(pixelPosition + int2(-1, 0), textureSize);
    pigmentRight =  ReadPigment(pixelPosition + int2(1, 0), textureSize);
    pigmentUp =     ReadPigment(pixelPosition + int2(0, -1), textureSize);
    pigmentDown =   ReadPigment(pixelPosition + int2(0, 1), textureSize);


    // ========================================================================
    // Wasserhöhe / Pressure zum Zeitpunkt n
    // ========================================================================

    pressureCenter = ReadOldPressure(pixelPosition, textureSize);
    pressureLeft =   ReadOldPressure(pixelPosition + int2(-1, 0), textureSize);
    pressureRight =  ReadOldPressure(pixelPosition + int2(1, 0), textureSize);
    pressureUp =     ReadOldPressure(pixelPosition + int2(0, -1), textureSize);
    pressureDown =   ReadOldPressure(pixelPosition + int2(0, 1), textureSize);


    // ========================================================================
    // Bereits berechnete neue Wasserhöhe
    // ========================================================================

    newPressureCenter = ReadNewPressure(pixelPosition, textureSize);


    // ========================================================================
    // Velocity
    // ========================================================================

    velocityCenter = ReadVelocity(pixelPosition, textureSize);
    velocityLeft =   ReadVelocity(pixelPosition + int2(-1, 0), textureSize);
    velocityRight =  ReadVelocity(pixelPosition + int2(1, 0), textureSize);
    velocityUp =     ReadVelocity(pixelPosition + int2(0, -1), textureSize);
    velocityDown =   ReadVelocity(pixelPosition + int2(0, 1), textureSize);


    // ========================================================================
    // Geschwindigkeit an den Zellflächen
    //
    // Exakt dieselbe Grundidee wie im PressureFlowShader.
    // ========================================================================

    velocityFaceLeft =  0.5F * (velocityLeft.x + velocityCenter.x);
    velocityFaceRight = 0.5F * (velocityCenter.x + velocityRight.x);
    velocityFaceUp =    0.5F * (velocityUp.y + velocityCenter.y);
    velocityFaceDown =  0.5F * (velocityCenter.y + velocityDown.y);


    // ========================================================================
    // Geschlossene Bildgrenzen
    // ========================================================================

    if (pixelPosition.x <= 0)
    {
        velocityFaceLeft = 0.0F;
    }

    if (pixelPosition.x >= int(textureWidth) - 1)
    {
        velocityFaceRight = 0.0F;
    }

    if (pixelPosition.y <= 0)
    {
        velocityFaceUp = 0.0F;
    }

    if (pixelPosition.y >= int(textureHeight) - 1)
    {
        velocityFaceDown = 0.0F;
    }


    // ========================================================================
    // Wasserfluss
    //
    // WICHTIG:
    //
    // Wie beim PressureFlowShader entscheidet die Flussrichtung,
    // aus welcher Zelle die transportierte Wassermenge stammt.
    //
    // Dieser Wasserfluss ist die Grundlage des Pigmenttransportes.
    // ========================================================================

    if (velocityFaceLeft >= 0.0F)
    {
        waterFluxLeft = velocityFaceLeft * pressureLeft;
    }
    else
    {
        waterFluxLeft = velocityFaceLeft * pressureCenter;
    }


    if (velocityFaceRight >= 0.0F)
    {
        waterFluxRight = velocityFaceRight * pressureCenter;
    }
    else
    {
        waterFluxRight = velocityFaceRight * pressureRight;
    }


    if (velocityFaceUp >= 0.0F)
    {
        waterFluxUp = velocityFaceUp * pressureUp;
    }
    else
    {
        waterFluxUp = velocityFaceUp * pressureCenter;
    }


    if (velocityFaceDown >= 0.0F)
    {
        waterFluxDown = velocityFaceDown * pressureCenter;
    }
    else
    {
        waterFluxDown = velocityFaceDown * pressureDown;
    }


    // ========================================================================
    // Experimentelle Transportstärke
    //
    // Für physikalisch synchronen Wasser-/Pigmenttransport sollte dieser
    // Wert 1.0 sein.
    // ========================================================================

    waterFluxLeft *= transportStrength;
    waterFluxRight *= transportStrength;
    waterFluxUp *= transportStrength;
    waterFluxDown *= transportStrength;


    // ========================================================================
    // Pigmentfluss
    //
    // PigmentFlux =
    //
    //      Wasserfluss
    //          *
    //      Pigmentkonzentration der Upwind-Zelle
    //
    // Damit transportieren wir nicht mehr eine unabhängige Pigmentdichte,
    // sondern die im tatsächlich bewegten Wasser enthaltene Pigmentmenge.
    // ========================================================================

    if (waterFluxLeft >= 0.0F)
    {
        pigmentFluxLeft = waterFluxLeft * pigmentLeft;
    }
    else
    {
        pigmentFluxLeft = waterFluxLeft * pigmentCenter;
    }


    if (waterFluxRight >= 0.0F)
    {
        pigmentFluxRight = waterFluxRight * pigmentCenter;
    }
    else
    {
        pigmentFluxRight = waterFluxRight * pigmentRight;
    }


    if (waterFluxUp >= 0.0F)
    {
        pigmentFluxUp = waterFluxUp * pigmentUp;
    }
    else
    {
        pigmentFluxUp = waterFluxUp * pigmentCenter;
    }


    if (waterFluxDown >= 0.0F)
    {
        pigmentFluxDown = waterFluxDown * pigmentCenter;
    }
    else
    {
        pigmentFluxDown = waterFluxDown * pigmentDown;
    }


    // ========================================================================
    // Alte Pigmentflächenmasse
    //
    // Konzentration allein ist NICHT die konservierte Größe.
    //
    //      Masse pro Fläche = Wasserhöhe * Konzentration
    // ========================================================================

    oldPigmentSurfaceMass = pigmentCenter * pressureCenter;


    // ========================================================================
    // Divergenz des Pigment-Massenflusses
    // ========================================================================

    pigmentFluxDivergence = (pigmentFluxRight - pigmentFluxLeft) + (pigmentFluxDown - pigmentFluxUp);


    // ========================================================================
    // Neue Pigmentflächenmasse
    // ========================================================================

    newPigmentSurfaceMass = oldPigmentSurfaceMass - timeStep * pigmentFluxDivergence;


    // ========================================================================
    // Numerisches Sicherheitsnetz
    //
    // Negative Pigmentmasse ist unmöglich.
    // ========================================================================

    newPigmentSurfaceMass = max(newPigmentSurfaceMass, float4(0.0F, 0.0F, 0.0F, 0.0F));


    // ========================================================================
    // Neue Konzentration rekonstruieren
    //
    //      g_neu = m_neu / p_neu
    //
    // Solange Wasser vorhanden ist, ergibt sich daraus wieder die
    // Pigmentkonzentration.
    //
    // ------------------------------------------------------------------------
    // Übergangsregel bis PigmentDeposit implementiert ist:
    //
    // Bei praktisch trockenem Pixel behalten wir den bisherigen
    // Pigmentzustand.
    //
    // Warum?
    //
    // Im endgültigen Modell müsste dort suspendiertes Pigment in den
    // Deposit-Zustand übergehen. Diesen Schritt besitzen wir noch nicht.
    //
    // Würden wir das Pigment jetzt stattdessen einfach löschen, würden
    // künstlich pigmentfreie / hintergrundfarbene Linien entstehen.
    //
    // Diese Regel wird mit Einführung von Adsorption/Deposit wieder
    // entfernt.
    // ========================================================================

    if (newPressureCenter > minimumPressure)
    {
        newPigmentConcentration = newPigmentSurfaceMass / newPressureCenter;
    }
    else
    {
        newPigmentConcentration = pigmentCenter;
    }


    // ========================================================================
    // Auch die rekonstruierte Konzentration darf nicht negativ sein.
    //
    // Nach oben wird bewusst NICHT geklemmt.
    //
    // Lokal erhöhte Konzentrationen sind erlaubt und später gerade für
    // Pigmentablagerung / dunklere Aquarellsäume interessant.
    // ========================================================================

    newPigmentConcentration = max(newPigmentConcentration, float4(0.0F, 0.0F, 0.0F, 0.0F));


    return newPigmentConcentration;
}