struct PartikelDaten
{
    float3 position;
    float3 geschwindigkeit;

    float2 groesse;

    float4 uvRect;

    float3 rotation;
    float3 rotationsGeschwindigkeit;

    int lod;
    int status;

    float gewicht;
};

struct VSOutput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
};

void ErmittleEckeUndUV(uint vertexID, out float2 ecke, out float2 uvFaktor)
{
    if (vertexID == 0)
    {
        ecke = float2(-0.5, -0.5);
        uvFaktor = float2(0.0, 0.0);
    }
    else if (vertexID == 1)
    {
        ecke = float2(0.5, -0.5);
        uvFaktor = float2(1.0, 0.0);
    }
    else if (vertexID == 2)
    {
        ecke = float2(-0.5, 0.5);
        uvFaktor = float2(0.0, 1.0);
    }
    else if (vertexID == 3)
    {
        ecke = float2(-0.5, 0.5);
        uvFaktor = float2(0.0, 1.0);
    }
    else if (vertexID == 4)
    {
        ecke = float2(0.5, -0.5);
        uvFaktor = float2(1.0, 0.0);
    }
    else
    {
        ecke = float2(0.5, 0.5);
        uvFaktor = float2(1.0, 1.0);
    }
}

float2 PixelZuNDC(float2 pixelPosition, float renderBreite, float renderHoehe)
{
    float2 ndcPosition;

    ndcPosition.x = pixelPosition.x / renderBreite * 2.0 - 1.0;
    ndcPosition.y = 1.0 - pixelPosition.y / renderHoehe * 2.0;

    return ndcPosition;
}

float ErmittlePartikelBasisTiefe(uint partikelIndex)
{
    /*
     * Stabile kleine Tiefenstaffelung.
     *
     * 0.45 ... 0.55
     *
     * Gleicher Partikelindex ergibt in jedem Frame
     * exakt dieselbe Z-Ebene.
     */

    uint hashWert;

    float faktor;

    hashWert = partikelIndex * 1664525u + 1013904223u;

    faktor = (hashWert & 0xFFFFu) / 65535.0;

    return lerp(0.45, 0.55, faktor);
}