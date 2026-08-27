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

    float3 lokalePosition;
    float3 tempPosition;

    float2 pixelPosition;
    float2 ndcPosition;

    float sinX;
    float cosX;

    float sinY;
    float cosY;

    float sinZ;
    float cosZ;
    
    float basisTiefe;
    float lokaleTiefe;
    float tiefe;

    float kameraAbstand;
    float perspektivFaktor;

    partikelIndex = RenderPartikelIndices[instanceID];

    partikel = PartikelBuffer[partikelIndex];

    ErmittleEckeUndUV(vertexID, ecke, uvFaktor);

    lokalePosition = float3(ecke.x * partikel.groesse.x, ecke.y * partikel.groesse.y, 0.0);

    /*
     * X-Rotation
     */

    sincos(partikel.rotation.x, sinX, cosX);

    tempPosition.x = lokalePosition.x;
    tempPosition.y = lokalePosition.y * cosX - lokalePosition.z * sinX;
    tempPosition.z = lokalePosition.y * sinX + lokalePosition.z * cosX;

    lokalePosition = tempPosition;

    /*
     * Y-Rotation
     */

    sincos(partikel.rotation.y, sinY, cosY);

    tempPosition.x = lokalePosition.x * cosY + lokalePosition.z * sinY;
    tempPosition.y = lokalePosition.y;
    tempPosition.z = -lokalePosition.x * sinY + lokalePosition.z * cosY;

    lokalePosition = tempPosition;

    /*
     * Z-Rotation
     */

    sincos(partikel.rotation.z, sinZ, cosZ);

    tempPosition.x = lokalePosition.x * cosZ - lokalePosition.y * sinZ;
    tempPosition.y = lokalePosition.x * sinZ + lokalePosition.y * cosZ;
    tempPosition.z = lokalePosition.z;

    lokalePosition = tempPosition;

    /*
     * Einfache perspektivische Projektion.
     *
     * Der Kamerabstand skaliert mit der Rendergröße,
     * damit der Effekt bei verschiedenen Auflösungen
     * vergleichbar bleibt.
     */

    kameraAbstand = max(renderBreite, renderHoehe) * 1.2;

    perspektivFaktor = kameraAbstand / max(kameraAbstand - lokalePosition.z, kameraAbstand * 0.1);
    
    basisTiefe = ErmittlePartikelBasisTiefe(partikelIndex);

    /*
     * Positive lokale Z-Werte liegen durch unsere
     * Perspektivdefinition näher an der Kamera.
     *
     * Deshalb muss der Depth-Wert kleiner werden.
     */

    lokaleTiefe = lokalePosition.z / kameraAbstand;

    tiefe = saturate(basisTiefe - lokaleTiefe * 0.35);

    pixelPosition = partikel.position.xy + lokalePosition.xy * perspektivFaktor;

    ndcPosition = PixelZuNDC(pixelPosition, renderBreite, renderHoehe);

    output.position = float4(ndcPosition, tiefe, 1.0);

    output.uv = lerp(partikel.uvRect.xy, partikel.uvRect.zw, uvFaktor);

    return output;
}