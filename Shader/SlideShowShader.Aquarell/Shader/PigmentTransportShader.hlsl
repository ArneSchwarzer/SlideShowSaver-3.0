// ============================================================================
// PigmentTransportShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Transportiert suspendierte Pigmentmasse zusammen mit dem Wasser.
//
// Curtis-nahe Interpretation:
//
//      p   = lokale Wasserhöhe / Pressure
//      g^k = Pigmentkonzentration im Wasser
//
// Persistent gespeichert wird ab diesem Entwicklungsstand jedoch NICHT
// mehr die Konzentration selbst.
//
// Die PigmentSuspensionTexture enthält:
//
//      RGB = premultiplizierte suspendierte Pigmentmasse pro Fläche
//      A   = suspendierte Pigmentmasse pro Fläche
//
// Kurz:
//
//      m_susp = p * g
//
// Die für den Transport benötigte Konzentration wird ausschließlich
// temporär berechnet:
//
//      g = m_susp / p
//
// Der Pigmentfluss ergibt sich dann aus:
//
//      pigmentFlux
//
//          =
//
//      waterFlux * pigmentConcentration
//
// Dadurch wird genau die Pigmentmenge transportiert, die im tatsächlich
// bewegten Wasser enthalten ist.
//
// Der persistent gespeicherte Zustand bleibt dagegen durchgehend:
//
//      Pigmentmasse pro Fläche
//
// Vorteile:
//
//      - keine Vermischung von Konzentration und Masse zwischen Shadern
//      - kein künstlicher Trockenheits-Sonderfall
//      - direkte Übergabe der Restmasse an PigmentDepositShader
//      - Suspension und Deposit besitzen dieselbe physikalische Einheit
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
// t0 = suspendierte Pigmentmasse zum Zeitpunkt n
// t1 = Velocity für diese Simulationszeitscheibe
// t2 = Pressure zum Zeitpunkt n
// t3 = bereits berechneter Pressure zum Zeitpunkt n + 1
//
// WICHTIG:
//
// Die SuspensionTexture speichert ab jetzt MASSE und keine Konzentration.
//
// Die Konzentration wird aus:
//
//      suspensionMass / pressure
//
// nur temporär für die Flussberechnung rekonstruiert.
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

int2 ClampPixelPosition(
    int2 pixelPosition,
    uint2 textureSize)
{
    return clamp(
        pixelPosition,
        int2(0, 0),
        int2(
            int(textureSize.x) - 1,
            int(textureSize.y) - 1
        )
    );
}


float4 ReadPigmentMass(
    int2 pixelPosition,
    uint2 textureSize)
{
    return
        max(
            pigmentTexture.Load(
                int3(
                    ClampPixelPosition(
                        pixelPosition,
                        textureSize
                    ),
                    0
                )
            ),
            float4(
                0.0F,
                0.0F,
                0.0F,
                0.0F
            )
        );
}


float2 ReadVelocity(
    int2 pixelPosition,
    uint2 textureSize)
{
    return
        velocityTexture.Load(
            int3(
                ClampPixelPosition(
                    pixelPosition,
                    textureSize
                ),
                0
            )
        );
}


float ReadOldPressure(
    int2 pixelPosition,
    uint2 textureSize)
{
    return
        max(
            oldPressureTexture.Load(
                int3(
                    ClampPixelPosition(
                        pixelPosition,
                        textureSize
                    ),
                    0
                )
            ),
            0.0F
        );
}


float ReadNewPressure(
    int2 pixelPosition,
    uint2 textureSize)
{
    return
        max(
            newPressureTexture.Load(
                int3(
                    ClampPixelPosition(
                        pixelPosition,
                        textureSize
                    ),
                    0
                )
            ),
            0.0F
        );
}


// ============================================================================
// Konzentration aus Pigmentmasse und Wasserhöhe
//
//      g = m / p
//
// Bei praktisch trockenem Pixel existiert keine sinnvoll definierte
// mobile Pigmentkonzentration.
//
// Deshalb gilt dort:
//
//      g = 0
//
// WICHTIG:
//
// Die eventuell noch vorhandene Pigmentmasse wird dadurch NICHT gelöscht.
//
// Sie bleibt persistent in der PigmentSuspensionTexture erhalten und kann
// anschließend vom PigmentDepositShader auf das Papier übertragen werden.
// ============================================================================

float4 BerechnePigmentKonzentration(
    float4 pigmentMass,
    float pressure)
{
    if (pressure > minimumPressure)
    {
        return
            pigmentMass /
            pressure;
    }


    return
        float4(
            0.0F,
            0.0F,
            0.0F,
            0.0F
        );
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
    // Pigmentmassen
    // ========================================================================

    float4 pigmentMassCenter;
    float4 pigmentMassLeft;
    float4 pigmentMassRight;
    float4 pigmentMassUp;
    float4 pigmentMassDown;


    // ========================================================================
    // Pigmentkonzentrationen
    //
    // Werden ausschließlich temporär für die Flussberechnung benötigt.
    // ========================================================================

    float4 pigmentConcentrationCenter;
    float4 pigmentConcentrationLeft;
    float4 pigmentConcentrationRight;
    float4 pigmentConcentrationUp;
    float4 pigmentConcentrationDown;


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
    // Neuer Pigmentzustand
    // ========================================================================

    float4 pigmentFluxDivergence;
    float4 newPigmentMass;


    // ========================================================================
    // Texturgröße / aktuelle Pixelposition
    // ========================================================================

    pigmentTexture.GetDimensions(
        textureWidth,
        textureHeight
    );

    textureSize =
        uint2(
            textureWidth,
            textureHeight
        );

    pixelPosition =
        int2(
            input.position.xy
        );


    // ========================================================================
    // Suspendierte Pigmentmassen zum Zeitpunkt n
    // ========================================================================

    pigmentMassCenter =
        ReadPigmentMass(
            pixelPosition,
            textureSize
        );

    pigmentMassLeft =
        ReadPigmentMass(
            pixelPosition + int2(-1, 0),
            textureSize
        );

    pigmentMassRight =
        ReadPigmentMass(
            pixelPosition + int2(1, 0),
            textureSize
        );

    pigmentMassUp =
        ReadPigmentMass(
            pixelPosition + int2(0, -1),
            textureSize
        );

    pigmentMassDown =
        ReadPigmentMass(
            pixelPosition + int2(0, 1),
            textureSize
        );


    // ========================================================================
    // Wasserhöhe / Pressure zum Zeitpunkt n
    // ========================================================================

    pressureCenter =
        ReadOldPressure(
            pixelPosition,
            textureSize
        );

    pressureLeft =
        ReadOldPressure(
            pixelPosition + int2(-1, 0),
            textureSize
        );

    pressureRight =
        ReadOldPressure(
            pixelPosition + int2(1, 0),
            textureSize
        );

    pressureUp =
        ReadOldPressure(
            pixelPosition + int2(0, -1),
            textureSize
        );

    pressureDown =
        ReadOldPressure(
            pixelPosition + int2(0, 1),
            textureSize
        );


    // ========================================================================
    // Bereits berechnete neue Wasserhöhe
    //
    // Für die persistente Pigmentmasse benötigen wir sie nicht mehr zur
    // Rekonstruktion einer Konzentration.
    //
    // Sie wird trotzdem gelesen, weil sie den Wasserzustand n + 1 beschreibt
    // und für Konsistenzkontrollen / spätere Erweiterungen bereitsteht.
    // ========================================================================

    newPressureCenter =
        ReadNewPressure(
            pixelPosition,
            textureSize
        );


    // ========================================================================
    // Pigmentkonzentrationen rekonstruieren
    //
    //      g = m / p
    //
    // Nur diese Konzentrationen werden für den eigentlichen Pigmentfluss
    // verwendet.
    // ========================================================================

    pigmentConcentrationCenter =
        BerechnePigmentKonzentration(
            pigmentMassCenter,
            pressureCenter
        );

    pigmentConcentrationLeft =
        BerechnePigmentKonzentration(
            pigmentMassLeft,
            pressureLeft
        );

    pigmentConcentrationRight =
        BerechnePigmentKonzentration(
            pigmentMassRight,
            pressureRight
        );

    pigmentConcentrationUp =
        BerechnePigmentKonzentration(
            pigmentMassUp,
            pressureUp
        );

    pigmentConcentrationDown =
        BerechnePigmentKonzentration(
            pigmentMassDown,
            pressureDown
        );


    // ========================================================================
    // Velocity
    // ========================================================================

    velocityCenter =
        ReadVelocity(
            pixelPosition,
            textureSize
        );

    velocityLeft =
        ReadVelocity(
            pixelPosition + int2(-1, 0),
            textureSize
        );

    velocityRight =
        ReadVelocity(
            pixelPosition + int2(1, 0),
            textureSize
        );

    velocityUp =
        ReadVelocity(
            pixelPosition + int2(0, -1),
            textureSize
        );

    velocityDown =
        ReadVelocity(
            pixelPosition + int2(0, 1),
            textureSize
        );


    // ========================================================================
    // Geschwindigkeit an den Zellflächen
    //
    // Dieselbe Grundidee wie im PressureFlowShader.
    // ========================================================================

    velocityFaceLeft =
        0.5F *
        (
            velocityLeft.x +
            velocityCenter.x
        );

    velocityFaceRight =
        0.5F *
        (
            velocityCenter.x +
            velocityRight.x
        );

    velocityFaceUp =
        0.5F *
        (
            velocityUp.y +
            velocityCenter.y
        );

    velocityFaceDown =
        0.5F *
        (
            velocityCenter.y +
            velocityDown.y
        );


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
    // Upwind-Schema:
    //
    // Die Flussrichtung entscheidet, aus welcher Zelle die tatsächlich
    // transportierte Wassermenge stammt.
    // ========================================================================

    if (velocityFaceLeft >= 0.0F)
    {
        waterFluxLeft =
            velocityFaceLeft *
            pressureLeft;
    }
    else
    {
        waterFluxLeft =
            velocityFaceLeft *
            pressureCenter;
    }


    if (velocityFaceRight >= 0.0F)
    {
        waterFluxRight =
            velocityFaceRight *
            pressureCenter;
    }
    else
    {
        waterFluxRight =
            velocityFaceRight *
            pressureRight;
    }


    if (velocityFaceUp >= 0.0F)
    {
        waterFluxUp =
            velocityFaceUp *
            pressureUp;
    }
    else
    {
        waterFluxUp =
            velocityFaceUp *
            pressureCenter;
    }


    if (velocityFaceDown >= 0.0F)
    {
        waterFluxDown =
            velocityFaceDown *
            pressureCenter;
    }
    else
    {
        waterFluxDown =
            velocityFaceDown *
            pressureDown;
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
    //      pigmentFlux
    //
    //          =
    //
    //      waterFlux
    //          *
    //      pigmentConcentration der Upwind-Zelle
    //
    // Die Konzentration wird aus der gespeicherten Pigmentmasse und dem
    // Pressure der jeweiligen Upwind-Zelle rekonstruiert.
    // ========================================================================

    if (waterFluxLeft >= 0.0F)
    {
        pigmentFluxLeft =
            waterFluxLeft *
            pigmentConcentrationLeft;
    }
    else
    {
        pigmentFluxLeft =
            waterFluxLeft *
            pigmentConcentrationCenter;
    }


    if (waterFluxRight >= 0.0F)
    {
        pigmentFluxRight =
            waterFluxRight *
            pigmentConcentrationCenter;
    }
    else
    {
        pigmentFluxRight =
            waterFluxRight *
            pigmentConcentrationRight;
    }


    if (waterFluxUp >= 0.0F)
    {
        pigmentFluxUp =
            waterFluxUp *
            pigmentConcentrationUp;
    }
    else
    {
        pigmentFluxUp =
            waterFluxUp *
            pigmentConcentrationCenter;
    }


    if (waterFluxDown >= 0.0F)
    {
        pigmentFluxDown =
            waterFluxDown *
            pigmentConcentrationCenter;
    }
    else
    {
        pigmentFluxDown =
            waterFluxDown *
            pigmentConcentrationDown;
    }


    // ========================================================================
    // Divergenz des Pigment-Massenflusses
    // ========================================================================

    pigmentFluxDivergence =
        (pigmentFluxRight - pigmentFluxLeft)
        +
        (pigmentFluxDown - pigmentFluxUp);


    // ========================================================================
    // Neue suspendierte Pigmentmasse
    //
    // Die PigmentSuspensionTexture speichert bereits die konservierte Größe.
    //
    // Deshalb ist KEINE anschließende Division durch newPressure mehr nötig.
    //
    // Das ist der entscheidende Unterschied zum bisherigen Shader.
    // ========================================================================

    newPigmentMass =
        pigmentMassCenter
        -
        timeStep *
        pigmentFluxDivergence;


    // ========================================================================
    // Numerisches Sicherheitsnetz
    //
    // Negative Pigmentmasse ist unmöglich.
    //
    // Nach oben wird bewusst NICHT geklemmt.
    //
    // Lokale Pigmentakkumulation ist erlaubt und später insbesondere für
    // dunklere Aquarellsäume und Granulation interessant.
    // ========================================================================

    newPigmentMass =
        max(
            newPigmentMass,
            float4(
                0.0F,
                0.0F,
                0.0F,
                0.0F
            )
        );


    // ========================================================================
    // Ausgabe
    //
    // KEINE Konzentrationsrekonstruktion.
    // KEINE Trockenheits-Notlösung.
    //
    // Ausgegeben wird direkt:
    //
    //      suspendierte Pigmentmasse pro Fläche
    //
    // Selbst wenn newPressure praktisch 0 ist, bleibt eventuell vorhandene
    // Pigmentmasse damit erhalten.
    //
    // Der anschließend laufende PigmentDepositShader entscheidet darüber,
    // welcher Anteil davon auf dem Papier abgelagert wird.
    // ========================================================================

    return newPigmentMass;
}