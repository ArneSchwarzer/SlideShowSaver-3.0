struct PartikelDaten
{
    float3 position;
    float3 geschwindigkeit;

    float2 groesse;

    float4 uvRect;

    float3 rotation;
    float3 rotationsGeschwindigkeit;

    int lod;
    int lebt;
};

StructuredBuffer<PartikelDaten> PartikelBuffer : register(t0);

cbuffer RenderParameter : register(b0)
{
    float renderBreite;
    float renderHoehe;

    float padding1;
    float padding2;
};

Texture2D QuellBild : register(t0);
SamplerState QuellSampler : register(s0);

struct VSOutput
{
    float4 position : SV_POSITION;

    float2 uv : TEXCOORD0;
    float2 lokaleUV : TEXCOORD1;
    float2 partikelGroesse : TEXCOORD2;
};

VSOutput VSMain(uint vertexID : SV_VertexID)
{
    VSOutput output;

    uint partikelIndex;
    uint lokalerVertex;

    PartikelDaten partikel;

    float2 ecke;
    float2 uvFaktor;

    float2 pixelPosition;
    float2 ndcPosition;

    partikelIndex = vertexID / 6;
    lokalerVertex = vertexID % 6;

    partikel = PartikelBuffer[partikelIndex];

    /*
     * Tote Partikel werden vollständig außerhalb
     * des sichtbaren Clip-Bereichs abgelegt.
     */

    if (partikel.lebt == 0)
    {
        output.position =
            float4(
                2.0,
                2.0,
                0.0,
                1.0);

        output.uv =
            float2(
                0.0,
                0.0);

        output.lokaleUV =
            float2(
                0.0,
                0.0);

        output.partikelGroesse =
            float2(
                1.0,
                1.0);

        return output;
    }

    /*
     * Zwei Dreiecke pro Quad.
     */

    if (lokalerVertex == 0)
    {
        ecke = float2(-0.5, -0.5);
        uvFaktor = float2(0.0, 0.0);
    }
    else if (lokalerVertex == 1)
    {
        ecke = float2(0.5, -0.5);
        uvFaktor = float2(1.0, 0.0);
    }
    else if (lokalerVertex == 2)
    {
        ecke = float2(-0.5, 0.5);
        uvFaktor = float2(0.0, 1.0);
    }
    else if (lokalerVertex == 3)
    {
        ecke = float2(-0.5, 0.5);
        uvFaktor = float2(0.0, 1.0);
    }
    else if (lokalerVertex == 4)
    {
        ecke = float2(0.5, -0.5);
        uvFaktor = float2(1.0, 0.0);
    }
    else
    {
        ecke = float2(0.5, 0.5);
        uvFaktor = float2(1.0, 1.0);
    }

    pixelPosition =
        partikel.position.xy +
        ecke *
        partikel.groesse;

    /*
     * Pixelkoordinaten -> NDC.
     */

    ndcPosition.x =
        pixelPosition.x /
        renderBreite *
        2.0 -
        1.0;

    ndcPosition.y =
        1.0 -
        pixelPosition.y /
        renderHoehe *
        2.0;

    output.position =
        float4(
            ndcPosition,
            0.0,
            1.0);

    output.uv =
        lerp(
            partikel.uvRect.xy,
            partikel.uvRect.zw,
            uvFaktor);

    output.lokaleUV =
        uvFaktor;

    output.partikelGroesse =
        partikel.groesse;

    return output;
}

float4 PSMain(VSOutput input) : SV_TARGET
{
    float4 farbe;

    float2 abstandRandUV;
    float2 abstandRandPixel;

    float kleinsterRandAbstand;
    float randBreitePixel;

    farbe =
        QuellBild.Sample(
            QuellSampler,
            input.uv);

    /*
     * Debug-Raster:
     * Abstand zur Partikelkante zunächst in lokalen UVs.
     */

    abstandRandUV =
        float2(
            min(
                input.lokaleUV.x,
                1.0 - input.lokaleUV.x),

            min(
                input.lokaleUV.y,
                1.0 - input.lokaleUV.y));

    /*
     * In echte Partikelpixel umrechnen.
     */

    abstandRandPixel =
        abstandRandUV *
        input.partikelGroesse;

    kleinsterRandAbstand =
        min(
            abstandRandPixel.x,
            abstandRandPixel.y);

    randBreitePixel = 1.0;

    if (kleinsterRandAbstand < randBreitePixel)
    {
        /*
         * Offizielle Testfarbe.
         */
        return float4(
            1.0,
            0.5,
            0.0,
            1.0);
    }

    return farbe;
}