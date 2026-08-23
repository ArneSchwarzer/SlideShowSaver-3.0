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

static const float ABLOESE_MINIMUM = 0.12;
static const float ABLOESE_MAXIMUM = 0.85;

static const float REFERENZ_PARTIKELGROESSE = 8.0;

static const float START_HAUPTWIND_ANTEIL = 0.70;

/* 
Hash-Generator für Pseudo-Zufallswerte per Partikel
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

    float turbulenz;
    float abloeseSchwelle;

    float widerstandsFaktor;
    
    float randomRichtung;
    float randomGeschwindigkeit;
    float randomVertikal;
    float randomWindKopplung;

    float startGeschwindigkeit;
    float2 startRichtung;

    float winkel;
    float sinWinkel;
    float cosWinkel;
    
    float2 hauptWindVektor;
    float2 lokalerStartVektor;

    float lokaleWindStaerke;
    float hauptWindVorzeichen;
    
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
        turbulenz = abs(zielGeschwindigkeit.y) / max(abs(zielGeschwindigkeit.x), 1.0);
        turbulenz = saturate(turbulenz * 3.0);

        /*
         * Zu Beginn liegt die Schwelle hoch.
         * Mit wachsendem Progress sinkt sie kontinuierlich.
         */
        abloeseSchwelle = lerp(ABLOESE_MAXIMUM, ABLOESE_MINIMUM, abloeseProgress);

        /*
         * Bei progress == 1 wird garantiert jedes noch
         * ruhende Partikel freigegeben.
         */
        if (abloeseProgress >= 1.0 ||    turbulenz >= abloeseSchwelle)
        {
            partikel.status =        STATUS_AKTIV;

            /*
             * Jedes Korn besitzt aufgrund seiner individuellen
             * Geometrie eine etwas andere aerodynamische Reaktion.
             *
             * Die Zufallswerte werden deterministisch aus dem
             * Partikelindex erzeugt.
             */
            randomRichtung = Hash01(index * 3 + 1);

            randomGeschwindigkeit = Hash01(index * 3 + 2);

            randomVertikal = Hash01(index * 3 + 3);

            /*
             * Kleine zufällige Winkelabweichung vom lokalen Wind.
             *
             * +/- 25 Grad.
             */
             winkel = (randomRichtung * 2.0 - 1.0) * 0.436332;

            sincos(winkel, sinWinkel, cosWinkel);

            /*
             * Lokalen FlowField-Vektor um den individuellen
             * Kornwinkel drehen.
             */
            
            lokalerStartVektor.x = zielGeschwindigkeit.x * cosWinkel - zielGeschwindigkeit.y * sinWinkel;
            lokalerStartVektor.y = zielGeschwindigkeit.x * sinWinkel + zielGeschwindigkeit.y * cosWinkel;

            /*
             * Betrag des lokalen Windes erhalten.
             */
            
            lokaleWindStaerke = length(zielGeschwindigkeit);

            /*
             * Globale Hauptwindrichtung.
             */
            
            hauptWindVorzeichen = zielGeschwindigkeit.x >= 0.0 ? 1.0 : -1.0;

            hauptWindVektor = float2(hauptWindVorzeichen * lokaleWindStaerke, 0.0);

            /*
             * Beim Ablösen dominiert zunächst der Hauptwind.
             *
             * Das lokale FlowField und der individuelle Kornwinkel
             * bleiben aber deutlich erhalten.
             */
            
            startRichtung = lerp(lokalerStartVektor, hauptWindVektor, START_HAUPTWIND_ANTEIL);

            /*
             * Individuelle Startgeschwindigkeit:
             * 65 bis 135 Prozent.
             */
            
            startGeschwindigkeit = lerp(0.65, 1.35, randomGeschwindigkeit);

            partikel.geschwindigkeit.xy = startRichtung * startGeschwindigkeit;

            /*
             * Zusätzlich ein kleiner vertikaler Kick.
             *
             * Der darf sowohl nach oben als auch nach unten gehen.
             * Damit zerbrechen direkt beim Ablösen auch lokale
             * horizontale Bänder.
             */
            
            partikel.geschwindigkeit.y += (randomVertikal * 2.0 - 1.0) * 45.0;
        }
        else
        {
            /*
             * Ruhendes Korn:
             * keinerlei Bewegung, keinerlei Gravitation.
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
    
    randomWindKopplung = lerp(0.75, 1.25, Hash01(index * 7 + 17));
    
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