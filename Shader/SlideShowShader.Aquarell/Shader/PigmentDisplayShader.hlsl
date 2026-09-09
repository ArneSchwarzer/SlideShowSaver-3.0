// ============================================================================
// PigmentDisplayShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Rendert den sichtbaren Pigmentzustand aus:
//
//          - suspendierter Pigmentmasse
//          - abgelagerter Pigmentmasse
//          - Hintergrundfarbe
//
// Ab diesem Entwicklungsstand besitzen Suspension und Deposit dieselbe
// Semantik:
//
//      RGB = premultiplizierte Pigmentfarbe / Pigmentmasse pro Fläche
//      A   = Pigmentmasse pro Fläche
//
// Deshalb können beide Zustände direkt addiert werden:
//
//      totalPigmentMass
//
//          =
//
//      suspensionMass + depositMass
//
// Die eigentliche Pigmentfarbe ergibt sich anschließend aus:
//
//      pigmentColor
//
//          =
//
//      totalPremultipliedColor / totalPigmentAmount
//
// Die gesamte Pigmentmenge bestimmt die sichtbare Deckung gegenüber der
// Hintergrundfarbe.
//
// WICHTIG:
//
// Pressure wird für die Darstellung NICHT mehr benötigt.
//
// Der Wasserzustand beeinflusst das Bild ausschließlich indirekt über:
//
//      - Pigmenttransport
//      - Adsorption
//      - Desorption
//
// ============================================================================


// ============================================================================
// Constant Buffer
//
// exakt 16 Byte
//
// Muss PigmentDisplayConstants auf VB-Seite entsprechen.
// ============================================================================

cbuffer PigmentDisplayConstants : register(b0)
{
    float backgroundRed;
    float backgroundGreen;
    float backgroundBlue;
    float backgroundAlpha;
};


// ============================================================================
// Eingaben
//
// t0 = suspendierte Pigmentmasse
//
//      RGB = premultiplizierte Pigmentfarbe / Masse
//      A   = suspendierte Pigmentmasse
//
// t1 = abgelagerte Pigmentmasse
//
//      RGB = premultiplizierte Pigmentfarbe / Masse
//      A   = abgelagerte Pigmentmasse
//
// ============================================================================

Texture2D<float4> suspensionTexture : register(t0);
Texture2D<float4> depositTexture : register(t1);

SamplerState linearSampler : register(s0);


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
// Pixel Shader
// ============================================================================

float4 PSMain(VSOutput input) : SV_TARGET
{
    float4 suspensionMass;
    float4 depositMass;

    float4 totalPigmentMass;

    float3 totalPremultipliedColor;
    float totalPigmentAmount;

    float3 pigmentColor;

    float visibleAmount;

    float4 backgroundColor;
    float3 resultColor;


    // ========================================================================
    // Pigmentzustände lesen
    //
    // SampleLevel mit normalisierten Texturkoordinaten ist hier absichtlich
    // gewählt.
    //
    // Der Shader kann dadurch auch in einem Teil-Viewport gerendert werden,
    // ohne dass SV_POSITION fälschlich als absolute Position innerhalb der
    // vollständigen Pigmenttexturen interpretiert wird.
    // ========================================================================

    suspensionMass =
        max(
            suspensionTexture.SampleLevel(
                linearSampler,
                input.texCoord,
                0.0F
            ),
            float4(
                0.0F,
                0.0F,
                0.0F,
                0.0F
            )
        );

    depositMass =
        max(
            depositTexture.SampleLevel(
                linearSampler,
                input.texCoord,
                0.0F
            ),
            float4(
                0.0F,
                0.0F,
                0.0F,
                0.0F
            )
        );


    // ========================================================================
    // Gesamtpigment
    //
    // Beide Zustände besitzen dieselbe Einheit:
    //
    //      Pigmentmasse pro Fläche
    //
    // Deshalb ist die Addition jetzt unmittelbar korrekt.
    // ========================================================================

    totalPigmentMass =
        suspensionMass +
        depositMass;

    totalPremultipliedColor =
        totalPigmentMass.rgb;

    totalPigmentAmount =
        max(
            totalPigmentMass.a,
            0.0F
        );


    // ========================================================================
    // Hintergrundfarbe
    // ========================================================================

    backgroundColor =
        float4(
            backgroundRed,
            backgroundGreen,
            backgroundBlue,
            backgroundAlpha
        );


    // ========================================================================
    // Pigmentfarbe rekonstruieren
    //
    // RGB wurde mit der Pigmentmasse premultipliziert gespeichert.
    //
    // Deshalb:
    //
    //      color = premultipliedColor / amount
    //
    // Bei pigmentfreiem Pixel vermeiden wir die Division vollständig.
    // ========================================================================

    if (totalPigmentAmount > 0.000001F)
    {
        pigmentColor =
            totalPremultipliedColor /
            totalPigmentAmount;
    }
    else
    {
        pigmentColor =
            backgroundColor.rgb;
    }


    // ========================================================================
    // Sichtbare Pigmentdeckung
    //
    // Für V1 verwenden wir weiterhin die einfache lineare Abbildung:
    //
    //      Pigmentmasse 0 -> Hintergrund vollständig sichtbar
    //      Pigmentmasse 1 -> Pigment vollständig sichtbar
    //
    // Pigmentmassen > 1 bleiben in der Simulation erhalten.
    //
    // Lediglich für die Darstellung wird die Deckung auf 1 begrenzt.
    //
    // Später kann diese Abbildung beispielsweise durch eine nichtlineare
    // optische Dichte ersetzt werden.
    // ========================================================================

    visibleAmount =
        saturate(
            totalPigmentAmount
        );


    // ========================================================================
    // Darstellung
    // ========================================================================

    resultColor =
        lerp(
            backgroundColor.rgb,
            pigmentColor,
            visibleAmount
        );


    return
        float4(
            saturate(resultColor),
            backgroundColor.a
        );
}