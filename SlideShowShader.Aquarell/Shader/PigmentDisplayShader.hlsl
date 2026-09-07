// ============================================================================
// PigmentDepositShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Tauscht Pigment zwischen zwei Zuständen aus:
//
//          Suspension  = Pigment, das im Wasser transportiert wird
//          Deposit     = Pigment, das auf / im Papier abgelagert ist
//
// Curtis-nahe Interpretation:
//
//      g^k = suspendierte Pigmentkonzentration
//      d^k = auf dem Papier abgelagertes Pigment
//
// Modelliert:
//
//      Adsorption:
//          Suspension -> Deposit
//
//      Desorption:
//          Deposit -> Suspension
//
// WICHTIG:
//
//      Der Shader erzeugt oder vernichtet KEIN Pigment.
//
//      Was aus der Suspension entfernt wird,
//      wird exakt dem Deposit hinzugefügt.
//
//      Was aus dem Deposit desorbiert wird,
//      wird exakt der Suspension hinzugefügt.
//
// Damit gilt lokal:
//
//      PigmentGesamt_neu = PigmentGesamt_alt
//
// adsorptionStrength und desorptionStrength werden NICHT als direkter
// Pigmentanteil pro Curtis-Iteration interpretiert.
//
// Stattdessen sind sie zeitabhängige Austauschraten.
//
// Aus einer Rate k und dem Zeitschritt dt wird der tatsächlich innerhalb
// einer Curtis-Iteration ausgetauschte Anteil:
//
//      factor = 1 - exp(-k * dt)
//
// Diese Form besitzt zwei wichtige Eigenschaften:
//
//      1. factor liegt immer im Bereich 0 ... 1.
//
//      2. Die Wirkung hängt wesentlich weniger von der willkürlich
//         gewählten Anzahl der Curtis-Iterationen ab.
//
// Für V1.0 besitzt das Papier noch KEINE eigene AbsorbencyMap.
//
// Die vorhandene PaperMap beschreibt ausschließlich die Papierhöhe
// und beeinflusst bereits die Wasserströmung.
//
// Eine echte papierabhängige Adsorption / Desorption folgt später.
//
// ============================================================================


// ============================================================================
// Constant Buffer
//
// exakt 32 Byte
// ============================================================================

cbuffer PigmentDepositConstants : register(b0)
{
    float adsorptionStrength;
    float desorptionStrength;

    float minimumPressure;
    float referencePressure;

    float timeStep;

    float reserve1;
    float reserve2;
    float reserve3;
};


// ============================================================================
// Eingaben
//
// t0 = bereits transportierte Pigment-Suspension
// t1 = bisheriger Pigment-Deposit
// t2 = bereits berechneter Pressure zum Zeitpunkt n + 1
//
// WICHTIG:
//
// Der Deposit-Pass läuft NACH dem PigmentTransport.
//
// Deshalb enthält suspensionTexture bereits das Transportergebnis
// derselben Curtis-Iteration.
// ============================================================================

Texture2D<float4> suspensionTexture : register(t0);
Texture2D<float4> depositTexture : register(t1);
Texture2D<float> pressureTexture : register(t2);


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
// MRT-Ausgabe
//
// Target 0 = neue Suspension
// Target 1 = neuer Deposit
// ============================================================================

struct PigmentDepositOutput
{
    float4 suspension : SV_TARGET0;
    float4 deposit : SV_TARGET1;
};


// ============================================================================
// Pixel Shader
// ============================================================================

PigmentDepositOutput PSMain(VSOutput input)
{
    PigmentDepositOutput output;

    uint textureWidth;
    uint textureHeight;

    int2 pixelPosition;


    float4 oldSuspension;
    float4 oldDeposit;

    float pressure;
    float normalizedPressure;

    float dryness;

    float adsorptionFactor;
    float desorptionFactor;

    float4 adsorbedPigment;
    float4 desorbedPigment;

    float4 newSuspension;
    float4 newDeposit;


    // ========================================================================
    // Aktuelle Pixelposition
    // ========================================================================

    suspensionTexture.GetDimensions(textureWidth, textureHeight);

    pixelPosition = int2(input.position.xy);

    pixelPosition =
        clamp(
            pixelPosition,
            int2(0, 0),
            int2(int(textureWidth) - 1, int(textureHeight) - 1)
        );


    // ========================================================================
    // Zustände lesen
    // ========================================================================

    oldSuspension =
        max(
            suspensionTexture.Load(int3(pixelPosition, 0)),
            float4(0.0F, 0.0F, 0.0F, 0.0F)
        );

    oldDeposit =
        max(
            depositTexture.Load(int3(pixelPosition, 0)),
            float4(0.0F, 0.0F, 0.0F, 0.0F)
        );

    pressure =
        max(
            pressureTexture.Load(int3(pixelPosition, 0)),
            0.0F
        );


    // ========================================================================
    // Pressure normieren
    //
    // referencePressure bedeutet:
    //
    //      pressure >= referencePressure
    //          -> vollständig "nass"
    //
    //      pressure == 0
    //          -> vollständig "trocken"
    //
    // Die Normierung dient ausschließlich der Adsorptions-/
    // Desorptionslogik.
    //
    // Der eigentliche Pressure-Zustand wird nicht verändert.
    // ========================================================================

    normalizedPressure = saturate(pressure / max(referencePressure, minimumPressure));


    // ========================================================================
    // Trockenheit
    //
    //      normalizedPressure = 1
    //          -> dryness = 0
    //
    //      normalizedPressure = 0
    //          -> dryness = 1
    //
    // Die quadratische Kennlinie sorgt dafür, dass Pigment in gut
    // benetzten Bereichen länger mobil bleibt.
    //
    // Erst mit zunehmender Austrocknung steigt die Adsorption deutlich an.
    //
    // Dies verhindert insbesondere, dass Pigment bereits während einer
    // noch kräftigen Strömung sofort auf dem Papier festgesetzt wird.
    // ========================================================================

    dryness = 1.0F - normalizedPressure;

    dryness = dryness * dryness;


    // ========================================================================
    // Adsorption
    //
    // adsorptionStrength ist eine Rate und KEIN direkter Anteil.
    //
    // Die diskrete Austauschmenge ergibt sich aus:
    //
    //      factor = 1 - exp(-rate * dt)
    //
    // Zusätzlich wird die Rate durch die lokale Trockenheit gewichtet.
    // ========================================================================

    adsorptionFactor =
        1.0F -
        exp(
            -max(adsorptionStrength, 0.0F)
            * max(timeStep, 0.0F)
            * dryness
        );


    // ========================================================================
    // Desorption
    //
    // Deposit kann nur wieder mobilisiert werden, wenn Wasser vorhanden ist.
    //
    // Je stärker die Benetzung, desto größer die mögliche Desorption.
    //
    // Für den aktuellen Test bleibt desorptionStrength auf 0.
    //
    // Die vollständige Logik bleibt trotzdem bereits korrekt implementiert.
    // ========================================================================

    if (pressure > minimumPressure)
    {
        desorptionFactor =
            1.0F -
            exp(
                -max(desorptionStrength, 0.0F)
                * max(timeStep, 0.0F)
                * normalizedPressure
            );
    }
    else
    {
        desorptionFactor = 0.0F;
    }


    // ========================================================================
    // Tatsächlich ausgetauschte Pigmentmengen
    //
    // Beide Faktoren liegen mathematisch im Bereich 0 ... 1.
    //
    // Deshalb kann niemals mehr Pigment aus einem Zustand entnommen werden,
    // als dort tatsächlich vorhanden ist.
    // ========================================================================

    adsorbedPigment = oldSuspension * adsorptionFactor;
    desorbedPigment = oldDeposit * desorptionFactor;


    // ========================================================================
    // Neue Zustände
    //
    // Massenerhaltend:
    //
    // Suspension:
    //
    //      alt
    //      - Adsorption
    //      + Desorption
    //
    // Deposit:
    //
    //      alt
    //      + Adsorption
    //      - Desorption
    // ========================================================================

    newSuspension = oldSuspension - adsorbedPigment  + desorbedPigment;
    newDeposit = oldDeposit + adsorbedPigment - desorbedPigment;


    // ========================================================================
    // Numerisches Sicherheitsnetz
    // ========================================================================

    newSuspension =
        max(
            newSuspension,
            float4(0.0F, 0.0F, 0.0F, 0.0F)
        );

    newDeposit =
        max(
            newDeposit,
            float4(0.0F, 0.0F, 0.0F, 0.0F)
        );


    // ========================================================================
    // MRT-Ausgabe
    // ========================================================================

    output.suspension = newSuspension;
    output.deposit = newDeposit;


    return output;
}