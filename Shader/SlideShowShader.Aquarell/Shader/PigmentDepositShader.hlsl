// ============================================================================
// PigmentDepositShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Tauscht Pigment zwischen zwei massenbasierten Zuständen aus:
//
//          Suspension
//              = im Wasser bewegliche Pigmentmasse pro Fläche
//
//          Deposit
//              = auf / im Papier abgelagerte Pigmentmasse pro Fläche
//
// Beide Zustände besitzen damit ab diesem Entwicklungsstand dieselbe
// physikalische Bedeutung und dieselbe Einheit.
//
// Curtis-nahe Interpretation:
//
//      m_susp
//          = suspendierte Pigmentmasse pro Fläche
//
//      d
//          = abgelagerte Pigmentmasse pro Fläche
//
//      p
//          = lokale Wasserhöhe / Pressure
//
// Die Pigmentkonzentration
//
//      g = m_susp / p
//
// wird für diesen Deposit-Pass NICHT benötigt.
//
// Der Pressure bestimmt hier ausschließlich, wie stark Pigment adsorbiert
// beziehungsweise wieder desorbiert wird.
//
// ---------------------------------------------------------------------------
//
// Adsorption:
//
//      Suspension -> Deposit
//
// Desorption:
//
//      Deposit -> Suspension
//
// Der Austausch ist lokal massenerhaltend:
//
//      m_susp_neu + d_neu
//
//          =
//
//      m_susp_alt + d_alt
//
// Es wird also weder Pigment erzeugt noch vernichtet.
//
// Für V1.0 besitzt das Papier noch KEINE eigene AbsorbencyMap.
//
// Die vorhandene PaperMap beschreibt ausschließlich die Papierhöhe und
// beeinflusst bereits die Wasserströmung.
//
// Eine papierabhängige Adsorption / Desorption kann später ergänzt werden.
//
// ============================================================================


// ============================================================================
// Constant Buffer
//
// exakt 32 Byte
//
// Muss exakt PigmentDepositConstants auf VB-Seite entsprechen.
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
// t0 = bereits transportierte suspendierte Pigmentmasse
//
//      RGB = premultiplizierte Pigmentfarbe / Pigmentmasse
//      A   = suspendierte Pigmentmasse
//
// t1 = bisheriger Pigment-Deposit
//
//      RGB = premultiplizierte Pigmentfarbe / Pigmentmasse
//      A   = abgelagerte Pigmentmasse
//
// t2 = bereits berechneter Pressure zum Zeitpunkt n + 1
//
// WICHTIG:
//
// Der Deposit-Pass läuft NACH dem PigmentTransport.
//
// suspensionTexture enthält deshalb bereits die durch den Transport
// berechnete neue suspendierte Pigmentmasse.
//
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
// Target 0 = neue suspendierte Pigmentmasse
// Target 1 = neuer Pigment-Deposit
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


    // ------------------------------------------------------------------------
    // Eingangszustände
    // ------------------------------------------------------------------------

    float4 oldSuspensionMass;
    float4 oldDepositMass;

    float pressure;
    float normalizedPressure;


    // ------------------------------------------------------------------------
    // Austauschparameter
    // ------------------------------------------------------------------------

    float adsorptionRate;
    float desorptionRate;

    float adsorptionFactor;
    float desorptionFactor;


    // ------------------------------------------------------------------------
    // Ausgetauschte Pigmentmassen
    // ------------------------------------------------------------------------

    float4 adsorbedPigmentMass;
    float4 desorbedPigmentMass;


    // ------------------------------------------------------------------------
    // Neue Zustände
    // ------------------------------------------------------------------------

    float4 newSuspensionMass;
    float4 newDepositMass;


    // ========================================================================
    // Aktuelle Pixelposition
    // ========================================================================

    suspensionTexture.GetDimensions(
        textureWidth,
        textureHeight
    );

    pixelPosition =
        int2(
            input.position.xy
        );

    pixelPosition =
        clamp(
            pixelPosition,
            int2(0, 0),
            int2(
                int(textureWidth) - 1,
                int(textureHeight) - 1
            )
        );


    // ========================================================================
    // Zustände lesen
    //
    // Beide Pigmenttexturen enthalten bereits PigmentMASSE pro Fläche.
    //
    // Es ist deshalb KEINE Umrechnung über Pressure notwendig.
    // ========================================================================

    oldSuspensionMass =
        max(
            suspensionTexture.Load(
                int3(
                    pixelPosition,
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

    oldDepositMass =
        max(
            depositTexture.Load(
                int3(
                    pixelPosition,
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

    pressure =
        max(
            pressureTexture.Load(
                int3(
                    pixelPosition,
                    0
                )
            ),
            0.0F
        );


    // ========================================================================
    // Pressure normieren
    //
    // referencePressure definiert den für die Austauschlogik als vollständig
    // nass betrachteten Zustand:
    //
    //      pressure <= 0
    //          -> normalizedPressure = 0
    //
    //      pressure >= referencePressure
    //          -> normalizedPressure = 1
    //
    // Pressure verändert NICHT die vorhandene Pigmentmasse.
    //
    // Er steuert ausschließlich die Stärke von Adsorption und Desorption.
    // ========================================================================

    normalizedPressure =
        saturate(
            pressure /
            max(
                referencePressure,
                minimumPressure
            )
        );


    // ========================================================================
    // Adsorptionsrate
    //
    // Wenig Wasser:
    //
    //      stärkere Ablagerung auf dem Papier
    //
    // Viel Wasser:
    //
    //      Pigment bleibt stärker mobil
    //
    // Bei vollständig trockenem Zustand:
    //
    //      adsorptionRate = adsorptionStrength
    //
    // Bei referencePressure oder darüber:
    //
    //      adsorptionRate = 0
    // ========================================================================

    adsorptionRate =
        max(
            adsorptionStrength,
            0.0F
        )
        *
        (1.0F - normalizedPressure);


    // ========================================================================
    // Desorptionsrate
    //
    // Nur vorhandenes Wasser kann bereits abgelagertes Pigment wieder
    // mobilisieren.
    //
    // Deshalb steigt die Desorption mit normalizedPressure.
    // ========================================================================

    if (pressure > minimumPressure)
    {
        desorptionRate =
            max(
                desorptionStrength,
                0.0F
            )
            *
            normalizedPressure;
    }
    else
    {
        desorptionRate = 0.0F;
    }


    // ========================================================================
    // Austauschrate -> Anteil dieser Curtis-Iteration
    //
    // Für V1 verwenden wir bewusst:
    //
    //      factor = rate * dt
    //
    // saturate garantiert:
    //
    //      0 <= factor <= 1
    //
    // Damit kann in einer einzelnen Iteration niemals mehr Pigment
    // übertragen werden, als im jeweiligen Zustand vorhanden ist.
    // ========================================================================

    adsorptionFactor =
        saturate(
            adsorptionRate *
            max(
                timeStep,
                0.0F
            )
        );

    desorptionFactor =
        saturate(
            desorptionRate *
            max(
                timeStep,
                0.0F
            )
        );


    // ========================================================================
    // Tatsächlich ausgetauschte Pigmentmassen
    //
    // Jetzt ist die Rechnung dimensionsmäßig sauber:
    //
    //      Masse * dimensionsloser Anteil = Masse
    // ========================================================================

    adsorbedPigmentMass =
        oldSuspensionMass *
        adsorptionFactor;

    desorbedPigmentMass =
        oldDepositMass *
        desorptionFactor;


    // ========================================================================
    // Neue Zustände
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
    //
    // Damit gilt lokal exakt:
    //
    //      newSuspensionMass + newDepositMass
    //
    //          =
    //
    //      oldSuspensionMass + oldDepositMass
    // ========================================================================

    newSuspensionMass =
        oldSuspensionMass
        - adsorbedPigmentMass
        + desorbedPigmentMass;

    newDepositMass =
        oldDepositMass
        + adsorbedPigmentMass
        - desorbedPigmentMass;


    // ========================================================================
    // Numerisches Sicherheitsnetz
    //
    // Negative Pigmentmassen sind unmöglich.
    //
    // Nach oben wird bewusst NICHT geklemmt.
    // ========================================================================

    newSuspensionMass =
        max(
            newSuspensionMass,
            float4(
                0.0F,
                0.0F,
                0.0F,
                0.0F
            )
        );

    newDepositMass =
        max(
            newDepositMass,
            float4(
                0.0F,
                0.0F,
                0.0F,
                0.0F
            )
        );


    // ========================================================================
    // MRT-Ausgabe
    //
    // Beide Targets speichern dieselbe Art von Größe:
    //
    //      Pigmentmasse pro Fläche
    //
    // Es findet KEINE Konzentrationsrekonstruktion mehr statt.
    // ========================================================================

    output.suspension = newSuspensionMass;
    output.deposit = newDepositMass;


    return output;
}