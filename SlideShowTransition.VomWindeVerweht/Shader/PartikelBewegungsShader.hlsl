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

cbuffer BewegungsParameter : register(b0)
{
    float deltaTime;
    float geschwindigkeit;
    float renderBreite;
    float richtung;

    uint partikelAnzahl;

    float padding1;
    float padding2;
    float padding3;
};

[numthreads(64, 1, 1)]
void CSMain(uint3 dispatchThreadID : SV_DispatchThreadID)
{
    uint index;
    PartikelDaten partikel;

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

    /*
     * Erster Proof of Concept:
     *
     * Alle Partikel bewegen sich mit derselben konstanten
     * Geschwindigkeit ausschließlich horizontal.
     *
     * richtung:
     *  1.0 = nach rechts
     * -1.0 = nach links
     */

    partikel.position.x += geschwindigkeit * richtung * deltaTime;

    /*
     * Ein Partikel stirbt erst dann, wenn sein vollständiges
     * Quad den Bildschirm verlassen hat.
     */

    if (richtung > 0.0)
    {
        if (partikel.position.x - partikel.groesse.x * 0.5 > renderBreite)
        {
            partikel.lebt = 0;
        }
    }
    else
    {
        if (partikel.position.x + partikel.groesse.x * 0.5 < 0.0)
        {
            partikel.lebt = 0;
        }
    }

    PartikelBuffer[index] = partikel;
}