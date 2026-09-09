// ============================================================================
// PaperInitializerShader.hlsl
//
// Initialisiert die grundlegende Papier-Höhenkarte h(x,y).
//
// Ausgabe:
//      R = Papierhöhe h im Bereich ungefähr 0.35 ... 0.65
//
// Die Papierstruktur besteht bewusst nur aus mehreren einfachen,
// deterministischen Value-Noise-Ebenen.
//
// V 1.0:
//      Generische leichte Papierunebenheit.
//
// Spätere Versionen:
//      Papierarten
//      Fasern
//      Büttenstruktur
//      Absorption
//      Makroprägung
//      usw.
// ============================================================================


// ============================================================================
// Vertex Shader
// ============================================================================

struct VSOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


VSOutput VSMain(uint vertexID : SV_VertexID)
{
    VSOutput output;

    float2 position;

    if (vertexID == 0)
    {
        position = float2(-1.0f, -1.0f);
        output.texCoord = float2(0.0f, 1.0f);
    }
    else if (vertexID == 1)
    {
        position = float2(-1.0f, 3.0f);
        output.texCoord = float2(0.0f, -1.0f);
    }
    else
    {
        position = float2(3.0f, -1.0f);
        output.texCoord = float2(2.0f, 1.0f);
    }

    output.position = float4(position, 0.0f, 1.0f);

    return output;
}


// ============================================================================
// Hilfsfunktionen
// ============================================================================


// ---------------------------------------------------------------------------
// Deterministischer Pseudozufallswert 0 ... 1.
//
// Kein echter Zufall:
// Für dieselbe Zellposition entsteht bei jedem Renderdurchlauf derselbe Wert.
// ---------------------------------------------------------------------------

float Hash21(float2 position)
{
    position = frac(position * float2(123.34f, 456.21f));

    position += dot(position, position + 45.32f);

    return frac(position.x * position.y);
}


// ---------------------------------------------------------------------------
// Sehr einfaches zweidimensionales Value Noise.
//
// Die Zufallswerte liegen auf einem groben Raster.
// Innerhalb der Rasterzelle werden sie weich interpoliert.
// ---------------------------------------------------------------------------

float ValueNoise(float2 position)
{
    float2 cell;
    float2 localPosition;
    float2 interpolation;

    float value00;
    float value10;
    float value01;
    float value11;

    float valueTop;
    float valueBottom;

    cell = floor(position);
    localPosition = frac(position);

    // Smoothstep-Kurve.
    interpolation = localPosition * localPosition * (3.0f - 2.0f * localPosition);

    value00 = Hash21(cell + float2(0.0f, 0.0f));
    value10 = Hash21(cell + float2(1.0f, 0.0f));
    value01 = Hash21(cell + float2(0.0f, 1.0f));
    value11 = Hash21(cell + float2(1.0f, 1.0f));

    valueTop = lerp(value00, value10, interpolation.x);

    valueBottom = lerp(value01, value11, interpolation.x);

    return lerp(valueTop, valueBottom, interpolation.y);
}


// ============================================================================
// Pixel Shader
// ============================================================================

float PSMain(VSOutput input) : SV_TARGET
{
    float2 uv;

    float largeStructure;
    float mediumStructure;
    float fineStructure;

    float paperVariation;
    float paperHeight;

    uv = saturate(input.texCoord);


    // ------------------------------------------------------------------------
    // Mehrere Frequenzbereiche.
    //
    // Keine dieser Ebenen soll bereits eine sichtbare Papierstruktur
    // darstellen. Gemeinsam erzeugen sie lediglich eine leicht unebene,
    // organische Oberfläche.
    // ------------------------------------------------------------------------

    largeStructure =  ValueNoise(uv * float2(18.0f, 18.0f));
    mediumStructure = ValueNoise(uv * float2(55.0f, 55.0f));
    fineStructure =   ValueNoise(uv * float2(160.0f, 160.0f));


    // ------------------------------------------------------------------------
    // Ebenen mischen.
    //
    // Summe der Gewichte = 1.0.
    // ------------------------------------------------------------------------

    paperVariation = largeStructure * 0.50f + mediumStructure * 0.35f + fineStructure * 0.15f;


    // ------------------------------------------------------------------------
    // ValueNoise liefert 0 ... 1.
    //
    // Um Zentrum 0.5 verschieben und Amplitude bewusst klein halten.
    //
    // ungefähr:
    //
    //      0.40 ... 0.60
    //
    // Das reicht für unser erstes Papier völlig aus.
    // ------------------------------------------------------------------------

    paperHeight = 0.5f + (paperVariation - 0.5f) * 0.20f;

    return saturate(paperHeight);
}