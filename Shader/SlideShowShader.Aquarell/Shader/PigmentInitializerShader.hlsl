// ============================================================================
// PigmentInitializerShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Erzeugt den initialen suspendierten Pigmentzustand aus:
//
//          - dem abstrahierten Kuwahara-Bild
//          - der initialen Wasserhöhe / Pressure
//
// Ab diesem Entwicklungsstand speichert die PigmentSuspensionTexture
// ausschließlich:
//
//      RGB = premultiplizierte suspendierte Pigmentmasse pro Fläche
//      A   = suspendierte Pigmentmasse pro Fläche
//
// Die persistente Pigmentgröße ist damit:
//
//      m_susp = p * g
//
// Für die Initialisierung setzen wir:
//
//      g_initial = 1.0
//
// Damit folgt:
//
//      m_susp_initial
//
//          =
//
//      pressure_initial * 1.0
//
//          =
//
//      pressure_initial
//
// Die Kuwahara-Farbe bestimmt die Pigmentfarbe.
//
// Dadurch enthält ein Pixel mit viel Wasser anfänglich entsprechend mehr
// suspendierte Pigmentmasse als ein Pixel mit wenig Wasser.
//
// ============================================================================


// ============================================================================
// Eingaben
//
// t0 = Kuwahara-Bild
// t1 = initiale PressureTexture
// ============================================================================

Texture2D<float4> kuwaharaTexture : register(t0);
Texture2D<float> pressureTexture : register(t1);


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
    uint textureWidth;
    uint textureHeight;

    int2 pixelPosition;

    float3 pigmentColor;

    float pressure;

    float initialPigmentConcentration;
    float initialPigmentMass;

    float3 premultipliedPigmentColor;


    // ========================================================================
    // Aktuelle Pixelposition
    // ========================================================================

    kuwaharaTexture.GetDimensions(
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
    // Pigmentfarbe
    //
    // Das Kuwahara-Bild liefert die abstrahierte Ausgangsfarbe.
    //
    // Die Farbe selbst beschreibt noch KEINE Pigmentmenge.
    // ========================================================================

    pigmentColor =
        saturate(
            kuwaharaTexture.Load(
                int3(
                    pixelPosition,
                    0
                )
            ).rgb
        );


    // ========================================================================
    // Initiale Wasserhöhe
    // ========================================================================

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
    // Initiale Pigmentkonzentration
    //
    // Für V1 starten wir weiterhin mit:
    //
    //      g_initial = 1
    //
    // Das bedeutet:
    //
    //      eine Einheit Pigmentkonzentration im vorhandenen Wasser.
    //
    // Die Konzentration wird NICHT persistent gespeichert.
    // ========================================================================

    initialPigmentConcentration = 1.0F;


    // ========================================================================
    // Initiale suspendierte Pigmentmasse
    //
    //      m = p * g
    //
    // Bei g_initial = 1 folgt:
    //
    //      m = p
    // ========================================================================

    initialPigmentMass =
        pressure *
        initialPigmentConcentration;


    // ========================================================================
    // Premultiplizierte Pigmentfarbe
    //
    // RGB und A tragen dieselbe Pigmentmasse:
    //
    //      RGB = Farbe * Masse
    //      A   = Masse
    // ========================================================================

    premultipliedPigmentColor =
        pigmentColor *
        initialPigmentMass;


    // ========================================================================
    // Ausgabe
    //
    // PigmentSuspensionTexture:
    //
    //      RGB = premultiplizierte suspendierte Pigmentmasse
    //      A   = suspendierte Pigmentmasse
    // ========================================================================

    return
        float4(
            premultipliedPigmentColor,
            initialPigmentMass
        );
}