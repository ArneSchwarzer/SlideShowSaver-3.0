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

RWStructuredBuffer<PartikelDaten> PartikelBuffer : register(u0);
RWStructuredBuffer<int> LebendZaehler : register(u1);
AppendStructuredBuffer<uint> RenderPartikelIndicesFein : register(u2);
AppendStructuredBuffer<uint> RenderPartikelIndicesMittel : register(u3);
AppendStructuredBuffer<uint> RenderPartikelIndicesGrob : register(u4);
Texture2D<float2> FlowField : register(t0);
SamplerState FlowFieldSampler : register(s0);

Texture2D<float> RandAbloeseFeld : register(t1);
SamplerState RandAbloeseFeldSampler : register(s1);

cbuffer BewegungsParameter : register(b0)
{
    float deltaTime;
    float renderBreite;
    float renderHoehe;

    uint partikelAnzahl;

    float abloeseProgress;

    float gravitation;
    
    float padding2;
    float padding3;
};

static const float WIND_KOPPLUNG = 2.5;
static const float LUFTWIDERSTAND = 0.15;

static const int STATUS_TOT = 0;
static const int STATUS_RUHEND = 1;
static const int STATUS_AKTIV = 2;

static const float START_HAUPTWIND_ANTEIL = 0.70;

/*
 * Breite der aktiven Ablösezone in Progress-Einheiten.
 *
 * Nur Partikel unmittelbar an der wandernden Abbruchkante
 * dürfen neu in den Flug übergehen.
 */
static const float ABLOESE_KANTENBREITE = 0.018;

/*
 * Kleine individuelle Auflockerung der Kante.
 *
 * Bewusst wesentlich geringer als bisher, damit aus der
 * Abbruchkante keine breite Erosionszone wird.
 */
static const float ABLOESE_ZUFALL = 0.004;

/* 
 * Hilfsfunktionen
 *
 * Hash (Deterministischer Pseudo-Zufall
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

void FuegePartikelZurRenderListeHinzu(uint index, int lod)
{
    if (lod == 0)
    {
        RenderPartikelIndicesFein.Append(index);
    }
    else if (lod == 1)
    {
        RenderPartikelIndicesMittel.Append(index);
    }
    else
    {
        RenderPartikelIndicesGrob.Append(index);
    }
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

    float windEmpfindlichkeit;
    float gewichtFaktor;
    float startGewichtFaktor;

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
        
    float abloeseWert;
    float individuellerAbloeseOffset;

    float2 lokaleFlowRichtung;
    float2 individuelleRichtung;
    float2 startRichtung;

    float randomWinkel;
    
    
    index = dispatchThreadID.x;

    if (index >= partikelAnzahl)
    {
        return;
    }

    partikel = PartikelBuffer[index];

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
     * RANDBASIERTE ABLÖSUNG
     * ---------------------------------------------------------
     *
     * Das RandAbloeseFeld enthält pro Bildschirmposition
     * den Zeitpunkt 0..1, zu dem der Wind diese Stelle
     * erreicht.
     *
     * Die globale Ablöserichtung und die globale
     * FlowField-Richtung stimmen dabei überein.
     */

        abloeseWert = RandAbloeseFeld.SampleLevel(RandAbloeseFeldSampler, flowUV, 0.0);

    /*
     * Sehr kleine individuelle Abweichung.
     *
     * Sie verhindert eine mathematisch perfekte Schnittkante,
     * ohne wieder eine breite Ablösezone zu erzeugen.
     */
        individuellerAbloeseOffset = lerp(-ABLOESE_ZUFALL, ABLOESE_ZUFALL, Hash01(index * 11 + 37));

        abloeseWert = saturate(abloeseWert + individuellerAbloeseOffset);

    /*
     * ---------------------------------------------------------
     * ABRUCHKANTE
     * ---------------------------------------------------------
     *
     * Ein ruhendes Partikel kann nur dann abheben, wenn die
     * wandernde Ablösefront gerade seine Position erreicht.
     *
     * Vor der Kante:
     *     StartBild bleibt vollständig erhalten.
     *
     * An der Kante:
     *     Partikel wird herausgerissen und beginnt zu fliegen.
     *
     * Hinter der Kante:
     *     Ein noch immer ruhendes Partikel gehört nicht mehr
     *     zum StartBild und wird verworfen.
     */

        if (abloeseProgress < abloeseWert)
        {
            PartikelBuffer[index] = partikel;

        /*
         * Ruhend, aber sichtbar:
         * Das Partikel gehört weiterhin zum StartBild.
         */
            FuegePartikelZurRenderListeHinzu(index, partikel.lod);

            return;
        }

        if (abloeseProgress <= abloeseWert + ABLOESE_KANTENBREITE)
        {
            partikel.status = STATUS_AKTIV;

    /*
     * Ab hier bleibt unser bestehender Code zur
     * Initialisierung des fliegenden Partikels stehen.
     */

        /*
         * Aktuelle lokale FlowField-Richtung.
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
         * Individuelle Kornparameter.
         */

            randomRichtung = Hash01(index * 3 + 1);

            randomGeschwindigkeit = Hash01(index * 3 + 2);

            randomVertikal = Hash01(index * 3 + 3);

        /*
         * Individuelle aerodynamische Abweichung
         * von +/- 30 Grad.
         */

            randomWinkel = (randomRichtung * 2.0 - 1.0) * 0.523599;

            individuelleRichtung.x = lokaleFlowRichtung.x * cos(randomWinkel) - lokaleFlowRichtung.y * 
                                     sin(randomWinkel);

            individuelleRichtung.y = lokaleFlowRichtung.x * sin(randomWinkel) + lokaleFlowRichtung.y *
                                     cos(randomWinkel);

        /*
         * -----------------------------------------------------
         * Startflugrichtung
         * -----------------------------------------------------
         *
         * Das FlowField ist jetzt bewusst klar dominant.
         *
         * Der Nutzer soll optisch sofort lesen können:
         *
         * "Der Wind kommt von dort und trägt das Bild fort."
         *
         * Die Kornindividualität bricht die Bewegung lediglich
         * leicht auf.
         */

            startRichtung = lokaleFlowRichtung * 0.80 + individuelleRichtung * 0.20;

            if (length(startRichtung) > 0.0001)
            {
                startRichtung = normalize(startRichtung);
            }
            else
            {
                startRichtung = lokaleFlowRichtung;
            }

        /*
         * Individuelle Startgeschwindigkeit.
         */

            startGeschwindigkeit = length(zielGeschwindigkeit) * lerp(0.70, 1.45, randomGeschwindigkeit);

        /*
         * Gewicht:
         *
         * leichte Körner werden kräftiger mitgerissen,
         * schwere starten träger.
         */

            startGewichtFaktor = 1.0 / max(partikel.gewicht, 0.1);
            startGewichtFaktor = clamp(startGewichtFaktor, 0.55, 2.0);

            startGeschwindigkeit *= startGewichtFaktor;

            partikel.geschwindigkeit.xy = startRichtung * startGeschwindigkeit;

        /*
         * Kleine individuelle vertikale Abweichung.
         */

            partikel.geschwindigkeit.y += (randomVertikal * 2.0 - 1.0) * 45.0;
        }
        else
        {
        /*
         * Die Ablösefront ist bereits über dieses Partikel
         * hinweggezogen.
         *
         * Es darf jetzt nicht länger als Teil des StartBildes
         * dargestellt werden.
         */
            partikel.status = STATUS_TOT;

            int vorherigerWert;

            InterlockedAdd(LebendZaehler[0], -1, vorherigerWert);

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

    /*
     * Individuelles Gewicht bestimmt die Trägheit gegenüber
     * dem Wind.
     *
     * gewicht < 1.0:
     * leichte Körner folgen dem Wind schnell.
     *
     * gewicht > 1.0:
     * schwere Körner reagieren deutlich träger.
     */
    gewichtFaktor = 1.0 / max(partikel.gewicht, 0.1);

    windEmpfindlichkeit = clamp(gewichtFaktor, 0.30, 2.5);

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
    partikel.geschwindigkeit.y += gravitation * deltaTime;
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
     * Eigenrotation integrieren.
     * ---------------------------------------------------------
     *
     * LOD Fein:
     *     Keine Rotation.
     *
     * LOD Mittel:
     *     Nur Rotation um die Z-Achse.
     *
     * LOD Grob:
     *     Vollständige XYZ-Rotation.
     *
     * Ruhende Partikel gelangen nicht bis hierher.
     * Sie werden weiter oben bereits in die APC-Renderliste
     * geschrieben und verlassen den Compute Shader.
     */

    if (partikel.lod == 1)
    {
        partikel.rotation.z += partikel.rotationsGeschwindigkeit.z * deltaTime;
    }
    else if (partikel.lod == 2)
    {
        partikel.rotation += partikel.rotationsGeschwindigkeit * deltaTime;
    }
    
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
    else
    {
    /*
     * Nur tatsächlich noch darzustellende Partikel
     * landen im APC-Renderindexbuffer.
     */
        FuegePartikelZurRenderListeHinzu(index, partikel.lod);
    }

    PartikelBuffer[index] = partikel;
}