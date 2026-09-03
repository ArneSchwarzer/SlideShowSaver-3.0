// ============================================================================
// PigmentDisplayShader.hlsl
//
// Wandelt den internen Pigmentzustand in das sichtbar ausgegebene
// Aquarellbild um.
//
// PigmentTexture:
//     RGB = premultiplizierte Pigmentfarbe
//     A   = Pigmentmenge
//
// Dünn pigmentierte Bereiche werden mit der vom SlideShowSaver
// vorgegebenen Hintergrundfarbe aufgefüllt.
//
// Das Ausgabe-Alpha ist immer 1.0.
// ============================================================================


// ============================================================================
// Constant Buffer
// ============================================================================

cbuffer PigmentDisplayConstants : register(b0)
{
    float4 backgroundColor;
};


// ============================================================================
// Shader Resources
// ============================================================================

Texture2D<float4> suspensionTexture : register(t0);
Texture2D<float4> depositTexture : register(t1);

// ============================================================================
// Sampler
// ============================================================================

SamplerState pigmentSampler : register(s0);


// ============================================================================
// Vertex-Ausgabe
// ============================================================================

struct VS_OUTPUT
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


// ============================================================================
// Vertex Shader
//
// Gleiche Fullscreen-Triangle-Geometrie wie beim Copy-Shader.
//
// Entscheidend:
// Die Texturkoordinaten laufen unabhängig von der Position des Viewports
// sauber über die vollständige Quelltexture.
// ============================================================================

VS_OUTPUT VSMain(uint vertexId : SV_VertexID)
{
    VS_OUTPUT output;

    float2 positions[3] =
    {
        float2(-1.0F, -1.0F),
        float2(-1.0F, 3.0F),
        float2(3.0F, -1.0F)
    };

    float2 texCoords[3] =
    {
        float2(0.0F, 1.0F),
        float2(0.0F, -1.0F),
        float2(2.0F, 1.0F)
    };

    output.position = float4(positions[vertexId], 0.0F, 1.0F);

    output.texCoord = texCoords[vertexId];

    return output;
}


// ============================================================================
// Pixel Shader
// ============================================================================

float4 PSMain(VS_OUTPUT input) : SV_TARGET
{
    float4 pigment;

    float pigmentMass;
    float coverage;

    float3 pigmentColor;
    float3 resultColor;


    // ------------------------------------------------------------------------
    // Simulationszustand lesen.
    //
    // RGB = premultiplizierte Pigmentfarbmasse
    // A   = Pigmentmasse
    // ------------------------------------------------------------------------

    pigment =
        suspensionTexture.Sample(pigmentSampler, input.texCoord) +
        depositTexture.Sample(pigmentSampler, input.texCoord);

    // ------------------------------------------------------------------------
    // Pigmentmasse.
    //
    // Anders als früher wird diese NICHT auf 0 ... 1 begrenzt.
    // ------------------------------------------------------------------------

    pigmentMass = max(pigment.a, 0.0F);


    // ------------------------------------------------------------------------
    // Eigentliche Pigmentfarbe aus der premultiplizierten Farbmasse
    // zurückgewinnen.
    //
    // Beispiel:
    //
    //      RGB = 0.4 * Rot
    //      A   = 0.4
    //
    // ergibt wieder:
    //
    //      Pigmentfarbe = Rot
    //
    // Bei praktisch pigmentfreien Pixeln verwenden wir Schwarz als
    // bedeutungslosen Fallback; wegen coverage = 0 wird dieser Wert ohnehin
    // nicht sichtbar.
    // ------------------------------------------------------------------------

    if (pigmentMass > 0.00001F)
    {
        pigmentColor = pigment.rgb / pigmentMass;
    }
    else
    {
        pigmentColor = float3(0.0F, 0.0F, 0.0F);
    }


    // ------------------------------------------------------------------------
    // Pigmentmasse -> sichtbare Deckung.
    //
    // V0.x:
    //
    // Für den ersten Diagnosetest verwenden wir bewusst die einfachste
    // mögliche Abbildung.
    //
    //      Masse 0.0 -> 0 % Deckung
    //      Masse 0.5 -> 50 % Deckung
    //      Masse 1.0 -> 100 % Deckung
    //      Masse >1  -> weiterhin 100 % Deckung
    //
    // WICHTIG:
    //
    // Pigmentmasse > 1.0 bleibt INTERN vollständig erhalten.
    // Lediglich die sichtbare Deckung sättigt bei 1.0.
    //
    // Dadurch kann akkumuliertes Pigment in späteren Iterationen wieder
    // weitertransportiert werden.
    // ------------------------------------------------------------------------

    coverage = saturate(pigmentMass);


    // ------------------------------------------------------------------------
    // Sichtbare Pigmentfarbe über HintergrundFarbeSaver komponieren.
    //
    // Kein Alpha-Blending mit einem eventuell noch hinter dem Shader
    // sichtbaren alten SSS-Bild.
    //
    // Das Resultat wird deshalb explizit vollständig aus:
    //
    //      Pigmentfarbe
    //      +
    //      HintergrundFarbeSaver
    //
    // aufgebaut.
    // ------------------------------------------------------------------------

    resultColor = pigmentColor * coverage + backgroundColor.rgb * (1.0F - coverage);


    // ------------------------------------------------------------------------
    // Ausgabe vollständig opak.
    // ------------------------------------------------------------------------

    return float4(saturate(resultColor), 1.0F);
}