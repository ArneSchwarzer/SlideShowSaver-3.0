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
};

RWStructuredBuffer<PartikelDaten> PartikelBuffer : register(u0);
RWStructuredBuffer<int> LebendZaehler : register(u1);

Texture2D<float2> FlowField : register(t0);
SamplerState FlowFieldSampler : register(s0);

Texture2D<float> DuenenFeld : register(t1);
SamplerState DuenenFeldSampler : register(s1);

cbuffer BewegungsParameter : register(b0)
{
    float deltaTime;
    float renderBreite;
    float renderHoehe;

    uint partikelAnzahl;

    float abloeseProgress;

    float padding1;
    float padding2;
    float padding3;
};

static const float WIND_KOPPLUNG = 2.5;
static const float LUFTWIDERSTAND = 0.15;
static const float GRAVITATION = 18.0;

static const int STATUS_TOT = 0;
static const int STATUS_RUHEND = 1;
static const int STATUS_AKTIV = 2;

static const float REFERENZ_PARTIKELGROESSE = 8.0;

static const float START_HAUPTWIND_ANTEIL = 0.70;

/* 
 * Hilfsfunktionen
 *
 * Hash (Deterministischer Pseudo-Zufall,
 * Periodische Differenz (Gradient / HeatMap-Helper
 *
 */

uint HashUint(uint wert)
{
    wert ^= wert >> 16;
    wert *= 0x7FEB352D;
    wert ^= wert >> 15;
    wert *= 0x846CA68B;
    wert ^= wert >> 16;

    return wert;
}

float Hash01(uint wert)
{
    return
        (HashUint(wert) & 0x00FFFFFF) /
        16777215.0;
}

float PeriodischeDifferenz(float wertA,  float wertB)
{
    float differenz;

    differenz = wertA - wertB;

    if (differenz > 0.5)
    {
        differenz -= 1.0;
    }
    else if (differenz < -0.5)
    {
        differenz += 1.0;
    }

    return differenz;
}

/* 
CSMain Hauptfunktion
*/

[numthreads(64, 1, 1)]
void CSMain(uint3 dispatchThreadID : SV_DispatchThreadID)
{
    uint index;

    PartikelDaten partikel;

    float2 flowUV;
    float2 zielGeschwindigkeit;
    float2 windBeschleunigung;

    float partikelGroesse;
    float windEmpfindlichkeit;

    float widerstandsFaktor;
    
    float randomRichtung;
    float randomGeschwindigkeit;
    float randomVertikal;
    float randomWindKopplung;

    float startGeschwindigkeit;

    float winkel;
    float sinWinkel;
    float cosWinkel;
    
    float2 hauptWindVektor;
    float2 lokalerStartVektor;

    float lokaleWindStaerke;
    float hauptWindVorzeichen;
    
    float duenenWert;
    float individuellerAbloeseOffset;

    uint duenenBreite;
    uint duenenHoehe;

    float2 duenenTexelGroesse;

    float dueneLinks;
    float dueneRechts;
    float dueneOben;
    float dueneUnten;

    float2 duenenGradient;
    float2 duenenRichtung;

    float2 lokaleFlowRichtung;
    float2 individuelleRichtung;
    float2 startRichtung;

    float randomWinkel;
    
    index = dispatchThreadID.x;

    if (index >= partikelAnzahl)
    {
        return;
    }

    partikel =
        PartikelBuffer[index];

    if (partikel.status == STATUS_TOT)
    {
        return;
    }

    /*
     * Bildschirmposition -> FlowField-UV.
     */
    flowUV.x = saturate(partikel.position.x / renderBreite);
    flowUV.y = saturate(partikel.position.y / renderHoehe);

    zielGeschwindigkeit = FlowField.SampleLevel(FlowFieldSampler, flowUV, 0.0);

    /*
     * ---------------------------------------------------------
     * PHASE 1:
     * Lokales Ablösen des Partikels.
     * ---------------------------------------------------------
     *
     * Die vertikale Komponente des FlowFields dient hier
     * gleichzeitig als einfache Turbulenz-Heatmap.
     *
     * Je stärker die Strömung vom horizontalen Hauptwind
     * abweicht, desto früher wird ein Partikel aufgewirbelt.
     */

    if (partikel.status == STATUS_RUHEND)
    {
    /*
     * ---------------------------------------------------------
     * DÜNENFELD:
     *
     * Das Feld beschreibt ausschließlich die bereits vorhandene
     * historische Dünengeometrie.
     *
     * Es ist vollständig unabhängig vom aktuellen FlowField.
     * ---------------------------------------------------------
     */

        duenenWert = DuenenFeld.SampleLevel(DuenenFeldSampler, flowUV, 0.0);

    /*
     * Winziger individueller Unterschied:
     *
     * Die makroskopische Dünenkante bleibt erhalten,
     * einzelne Körner lösen sich aber geringfügig früher
     * oder später.
     */
        individuellerAbloeseOffset = lerp(-0.025, 0.025, Hash01(index * 11 + 37));

        duenenWert = saturate(duenenWert + individuellerAbloeseOffset);

        if (abloeseProgress >= duenenWert)
        {
            partikel.status = STATUS_AKTIV;

        /*
         * -----------------------------------------------------
         * Lokalen Gradienten des Dünenfeldes bestimmen.
         * -----------------------------------------------------
         */

            DuenenFeld.GetDimensions(duenenBreite, duenenHoehe);

            duenenTexelGroesse = 1.0 / float2(max(duenenBreite, 1), max(duenenHoehe, 1));
            
            dueneLinks =DuenenFeld.SampleLevel(DuenenFeldSampler, flowUV - float2(duenenTexelGroesse.x, 0.0), 0.0);
            dueneRechts = DuenenFeld.SampleLevel(DuenenFeldSampler, flowUV + float2(duenenTexelGroesse.x, 0.0), 0.0);
            dueneOben = DuenenFeld.SampleLevel(DuenenFeldSampler, flowUV - float2(0.0, duenenTexelGroesse.y), 0.0);
            dueneUnten = DuenenFeld.SampleLevel(DuenenFeldSampler, flowUV + float2(0.0, duenenTexelGroesse.y), 0.0);

            duenenGradient.x = PeriodischeDifferenz(dueneRechts, dueneLinks);
            duenenGradient.y = PeriodischeDifferenz(dueneUnten,  dueneOben);

            if (length(duenenGradient) > 0.0001)
            {
                duenenRichtung = normalize(duenenGradient);
            }
            else
            {
                duenenRichtung = float2(0.0, 0.0);
            }

        /*
         * Aktuelle FlowField-Richtung.
         */
            if (length(zielGeschwindigkeit) > 0.0001)
            {
                lokaleFlowRichtung = normalize(zielGeschwindigkeit);
            }
            else
            {
                lokaleFlowRichtung = float2(0.0, 0.0);
            }

        /*
         * Individuelle Kornrichtung.
         *
         * +/- 30 Grad um die aktuelle Windrichtung.
         */
            randomRichtung = Hash01(index * 3 + 1);
            randomGeschwindigkeit = Hash01(index * 3 + 2);
            randomVertikal = Hash01(index * 3 + 3);
            randomWinkel = (randomRichtung * 2.0 - 1.0) * 0.523599;

            individuelleRichtung.x = lokaleFlowRichtung.x * cos(randomWinkel) - lokaleFlowRichtung.y * 
                                     sin(randomWinkel);
            individuelleRichtung.y = lokaleFlowRichtung.x * sin(randomWinkel) + lokaleFlowRichtung.y *
                                     cos(randomWinkel);

        /*
         * -----------------------------------------------------
         * Startflugrichtung:
         *
         * 55 % Dünengefälle
         * 35 % aktueller Wind
         * 10 % individuelle Korngeometrie
         *
         * Der Dünenhang entscheidet also zunächst stark,
         * WIE das Korn aus seiner Oberfläche herausgerissen
         * wird.
         *
         * Unmittelbar danach übernimmt wieder das normale
         * FlowField.
         * -----------------------------------------------------
         */

            startRichtung = duenenRichtung * 0.55 + lokaleFlowRichtung * 0.35 + individuelleRichtung * 0.10;

            if (length(startRichtung) > 0.0001)
            {
                startRichtung = normalize(startRichtung);
            }
            else
            {
                startRichtung = lokaleFlowRichtung;
            }

            startGeschwindigkeit = length(zielGeschwindigkeit) * lerp(0.70, 1.45, randomGeschwindigkeit);

            partikel.geschwindigkeit.xy = startRichtung * startGeschwindigkeit;

        /*
         * Kleiner individueller vertikaler Kick.
         */
            partikel.geschwindigkeit.y += (randomVertikal * 2.0 - 1.0) * 45.0;
        }
        else
        {
        /*
         * Noch nicht weit genug den Dünenhang
         * hinab erodiert.
         */
            PartikelBuffer[index] = partikel;

            return;
        }
    }

    /*
     * ---------------------------------------------------------
     * PHASE 2:
     * Freie Partikelbewegung.
     * ---------------------------------------------------------
     */

    partikelGroesse = max(1.0, (partikel.groesse.x + partikel.groesse.y) * 0.5);

    /*
     * Größe dient als einfacher Masse-/Trägheitsproxy.
     *
     * 1 px  -> stärker windempfindlich
     * 8 px  -> ungefähr Referenz
     * groß  -> zunehmend träger
     *
     * Clamp verhindert extreme Werte.
     */
    windEmpfindlichkeit = sqrt(REFERENZ_PARTIKELGROESSE /partikelGroesse);
    windEmpfindlichkeit = clamp(windEmpfindlichkeit, 0.35, 2.0);

    /*
     * Permanenter kleiner aerodynamischer Unterschied.
     *
     * Derselbe Partikel erhält in jedem Frame denselben Faktor,
     * weil der Hash nur vom Index abhängt.
     */
    
    randomWindKopplung = lerp(0.65, 1.35, Hash01(index * 7 + 17));
    
    /*
     * Das FlowField ist eine Zielgeschwindigkeit.
     * Das Partikel nähert sich ihr mit eigener Trägheit.
     */
    windBeschleunigung = (zielGeschwindigkeit - partikel.geschwindigkeit.xy) * WIND_KOPPLUNG * windEmpfindlichkeit *
                          randomWindKopplung;
    
    partikel.geschwindigkeit.xy += windBeschleunigung * deltaTime;

    /*
     * Kleine konstante Gravitation.
     */
    partikel.geschwindigkeit.y += GRAVITATION * deltaTime;

    /*
     * Luftwiderstand.
     */
    widerstandsFaktor = max(0.0, 1.0 - LUFTWIDERSTAND * deltaTime);

    partikel.geschwindigkeit.xy *= widerstandsFaktor;

    /*
     * Position integrieren.
     */
    partikel.position.xy += partikel.geschwindigkeit.xy * deltaTime;

    /*
     * ---------------------------------------------------------
     * Bildschirmtod.
     * ---------------------------------------------------------
     */

    if (partikel.geschwindigkeit.x > 0.0)
    {
        if (partikel.position.x - partikel.groesse.x * 0.5 > renderBreite)
        {
            partikel.status = STATUS_TOT;
        }
    }
    else if (partikel.geschwindigkeit.x < 0.0)
    {
        if (partikel.position.x + partikel.groesse.x * 0.5 < 0.0)
        {
            partikel.status = STATUS_TOT;
        }
    }

    if (partikel.position.y + partikel.groesse.y * 0.5 < 0.0)
    {
        partikel.status = STATUS_TOT;
    }

    if (partikel.position.y - partikel.groesse.y * 0.5 > renderHoehe)
    {
        partikel.status = STATUS_TOT;
    }

    /*
     * Der Lebendzähler umfasst sowohl ruhende als auch
     * aktive Partikel. Er wird nur beim endgültigen Tod
     * genau einmal vermindert.
     */
    if (partikel.status == STATUS_TOT)
    {
        int vorherigerWert;

        InterlockedAdd(LebendZaehler[0], -1, vorherigerWert);
    }

    PartikelBuffer[index] = partikel;
}