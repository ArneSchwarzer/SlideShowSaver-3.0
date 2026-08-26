#include "MosaikShaderCommon.hlsli"

StructuredBuffer<PartikelDaten> PartikelBuffer : register(t0);
StructuredBuffer<uint> RenderPartikelIndices : register(t1);

cbuffer RenderParameter : register(b0)
{
    float renderBreite;
    float renderHoehe;

    float padding1;
    float padding2;
};

VSOutput VSMain(uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID)
{
    VSOutput output;

    uint partikelIndex;

    PartikelDaten partikel;

    float2 ecke;
    float2 uvFaktor;

    float2 pixelPosition;
    float2 ndcPosition;

    partikelIndex = RenderPartikelIndices[instanceID];

    partikel = PartikelBuffer[partikelIndex];
    
    ErmittleEckeUndUV(vertexID, ecke, uvFaktor);

    pixelPosition = partikel.position.xy + ecke * partikel.groesse;

    ndcPosition = PixelZuNDC(pixelPosition, renderBreite, renderHoehe);

    output.position = float4(ndcPosition, 0.0, 1.0);

    output.uv = lerp(partikel.uvRect.xy, partikel.uvRect.zw, uvFaktor);

    return output;
}