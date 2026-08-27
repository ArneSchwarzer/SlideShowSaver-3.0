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

    float2 lokalePosition;
    float2 pixelPosition;
    float2 ndcPosition;

    float sinRotation;
    float cosRotation;
    
    float tiefe;

    partikelIndex = RenderPartikelIndices[instanceID];

    partikel = PartikelBuffer[partikelIndex];

    ErmittleEckeUndUV(vertexID, ecke, uvFaktor);

    lokalePosition = ecke * partikel.groesse;

    sincos(partikel.rotation.z, sinRotation, cosRotation);

    lokalePosition =
        float2(
            lokalePosition.x * cosRotation -
            lokalePosition.y * sinRotation,

            lokalePosition.x * sinRotation +
            lokalePosition.y * cosRotation);

    pixelPosition = partikel.position.xy + lokalePosition;

    ndcPosition = PixelZuNDC(pixelPosition, renderBreite, renderHoehe);
    
    tiefe = ErmittlePartikelBasisTiefe(partikelIndex);

    output.position = float4(ndcPosition, tiefe, 1.0);

    output.uv = lerp(partikel.uvRect.xy, partikel.uvRect.zw, uvFaktor);

    return output;
}