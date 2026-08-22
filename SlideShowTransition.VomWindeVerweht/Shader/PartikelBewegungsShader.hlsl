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

RWStructuredBuffer<PartikelDaten> PartikelBuffer : register(u0);
RWStructuredBuffer<int> LebendZaehler : register(u1);

Texture2D<float2> FlowField : register(t0);
SamplerState FlowFieldSampler : register(s0);

cbuffer BewegungsParameter : register(b0)
{
    float deltaTime;
    float renderBreite;
    float renderHoehe;

    uint partikelAnzahl;

    float padding1;
    float padding2;
    float padding3;
    float padding4;
};

[numthreads(64, 1, 1)]
void CSMain(uint3 dispatchThreadID : SV_DispatchThreadID)
{
    uint index;

    PartikelDaten partikel;

    float2 flowUV;
    float2 flowGeschwindigkeit;

    index = dispatchThreadID.x;

    if (index >= partikelAnzahl)
    {
        return;
    }

    partikel = PartikelBuffer[index];

    if (partikel.lebt == 0)
    {
        return;
    }

    flowUV.x = saturate(partikel.position.x / renderBreite);
    flowUV.y = saturate(partikel.position.y / renderHoehe);

    flowGeschwindigkeit = FlowField.SampleLevel(FlowFieldSampler, flowUV, 0.0);

    partikel.geschwindigkeit.x = flowGeschwindigkeit.x;
    partikel.geschwindigkeit.y = flowGeschwindigkeit.y;
    
    partikel.position.x += partikel.geschwindigkeit.x * deltaTime;
    partikel.position.y += partikel.geschwindigkeit.y * deltaTime;

    /*
     * Horizontaler Bildschirmtod.
     */
    if (flowGeschwindigkeit.x > 0.0)
    {
        if (partikel.position.x - partikel.groesse.x * 0.5 > renderBreite)
        {
            partikel.lebt = 0;
        }
    }
    else if (flowGeschwindigkeit.x < 0.0)
    {
        if (partikel.position.x + partikel.groesse.x * 0.5 < 0.0)
        {
            partikel.lebt = 0;
        }
    }

    /*
     * Auch komplett oben/unten verschwundene Partikel sind tot.
     */
    if (partikel.position.y + partikel.groesse.y * 0.5 < 0.0)
    {
        partikel.lebt = 0;
    }

    if (partikel.position.y - partikel.groesse.y * 0.5 > renderHoehe)
    {
        partikel.lebt = 0;
    }

    /*
 * Ein Partikel erreicht diese Stelle nur dann lebend,
 * wenn er zu Beginn dieses Dispatches noch gelebt hat.
 *
 * Stirbt er in diesem Frame, wird der globale Lebendzähler
 * deshalb exakt einmal vermindert.
 */
    if (partikel.lebt == 0)
    {
        int vorherigerWert;

        InterlockedAdd(LebendZaehler[0], -1, vorherigerWert);
    }

    PartikelBuffer[index] = partikel;
}