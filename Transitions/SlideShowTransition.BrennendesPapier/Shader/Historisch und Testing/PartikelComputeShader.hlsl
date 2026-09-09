// PartikelComputeShader.hlsl
//
// V0.4
//
// GPU-Partikelsimulation mit Brandkanten-Emitter.
//
// Flugbahn:
// 1. Geburt mit / gegen lokale Brandrichtung
// 2. individuelle zeitlich weiche Verwirbelung
// 3. altersabhängiger perspektivischer Drift
//    vom Bildschirmzentrum nach außen
//
// Zusätzlich:
// - langsame Mehrheit
// - wenige schnelle Ausreißer
// - persistente Bewegung
// - Alter / Tod
// - stochastische Wiedergeburt
//
// Compile Target:
// CSMain -> cs_5_0


#define PARTICLE_COUNT 8192
#define SPAWN_SEARCH_ATTEMPTS 32

#define PI 3.14159265358979323846


struct Particle
{
    float2 position;
    float2 velocity;

    float age;
    float lifetime;

    float size;

    uint seed;
};


cbuffer ParticleSimulationParameter : register(b0)
{
    float deltaTime;
    float progress;
    float emitterBreite;
    float spawnRate;

    float schwerkraftAktiv;
    float zeit;
    float flowFieldStaerke;
    float partikelMaxLebensdauer;

    float padding1;
    float padding2;
    float padding3;
    float padding4;
};


RWStructuredBuffer<Particle> particles : register(u0);


// Zeitcodierte Brandmaske.
//
// Jeder Pixel enthält ungefähr den Progress,
// bei dem dieser Bildpunkt verbrennt.
Texture2D<float4> brandMaske : register(t0);


// ------------------------------------------------------------
// Zufall
// ------------------------------------------------------------

uint HashUint(uint value)
{
    value ^= value >> 16;
    value *= 0x7FEB352D;
    value ^= value >> 15;
    value *= 0x846CA68B;
    value ^= value >> 16;

    return value;
}


float Random01(inout uint seed)
{
    seed = HashUint(seed);

    return (float) (seed & 0x00FFFFFF) / 16777215.0;
}


float RandomRange(inout uint seed, float minimum, float maximum)
{
    return minimum + Random01(seed) * (maximum - minimum);
}


// ------------------------------------------------------------
// Deterministischer Zufallswert aus einem vorhandenen Seed.
//
// Anders als Random01() verändert diese Funktion den Seed nicht.
// Das brauchen wir für die laufende Verwirbelung.
// ------------------------------------------------------------

float ZufallAusSeed(uint value)
{
    uint hash;

    hash = HashUint(value);

    return (float) (hash & 0x00FFFFFF) / 16777215.0;
}

// ------------------------------------------------------------
// Dreht einen 2D-Vektor um einen Winkel.
// ------------------------------------------------------------

float2 DreheVektor(float2 eingangsVektor, float winkel)
{
    float sinWinkel;
    float cosWinkel;

    sinWinkel = sin(winkel);
    cosWinkel = cos(winkel);

    return
        float2(
            eingangsVektor.x * cosWinkel -
            eingangsVektor.y * sinWinkel,

            eingangsVektor.x * sinWinkel +
            eingangsVektor.y * cosWinkel);
}

// ------------------------------------------------------------
// Sucht einen zufälligen Punkt auf der aktuellen Brandkante.
// ------------------------------------------------------------

bool FindeBrandkantenPosition(inout uint seed, out float2 position)
{
    uint maskenBreite;
    uint maskenHoehe;

    uint versuch;

    uint x;
    uint y;

    float2 kandidat;

    float brandZeit;
    float abstandZurBrandkante;

    brandMaske.GetDimensions(maskenBreite, maskenHoehe);

    position = float2(0.0, 0.0);

    for (
        versuch = 0;
        versuch < SPAWN_SEARCH_ATTEMPTS;
        versuch++)
    {
        kandidat.x = Random01(seed);
        kandidat.y = Random01(seed);

        x = min((uint) (kandidat.x * maskenBreite), maskenBreite - 1);
        y = min((uint) (kandidat.y * maskenHoehe), maskenHoehe - 1);

        brandZeit = brandMaske.Load(int3(x, y, 0)).r;

        abstandZurBrandkante = abs(brandZeit - progress);

        if (abstandZurBrandkante <= emitterBreite)
        {
            position = kandidat;

            return true;
        }
    }

    return false;
}


// ------------------------------------------------------------
// Ermittelt die lokale Ausbreitungsrichtung der Brandkante.
//
// Die Brandmaske enthält die Brandzeit T(x,y).
//
// Der Gradient von T zeigt in die Richtung steigender
// Brandzeit und damit ungefähr in die lokale
// Ausbreitungsrichtung des Brandes.
//
//     T oben
//
// T links   X   T rechts
//
//     T unten
//
// Wir kompensieren zusätzlich das Seitenverhältnis,
// da unsere Partikelpositionen in normalisierten
// Bildschirmkoordinaten gespeichert werden.
// ------------------------------------------------------------

float2 ErmittleBrandrichtung(float2 position)
{
    uint maskenBreite;
    uint maskenHoehe;

    int x;
    int y;

    int xLinks;
    int xRechts;
    int yOben;
    int yUnten;

    float brandLinks;
    float brandRechts;
    float brandOben;
    float brandUnten;

    float gradientX;
    float gradientY;

    float aspectCorrection;

    float2 richtung;

    brandMaske.GetDimensions(maskenBreite, maskenHoehe);

    x = clamp((int) (position.x * maskenBreite), 0, (int) maskenBreite - 1);
    y = clamp((int) (position.y * maskenHoehe),  0, (int) maskenHoehe - 1);

    xLinks = max(x - 1, 0);
    xRechts = min(x + 1, (int) maskenBreite - 1);
    yOben = max(y - 1, 0);
    yUnten = min(y + 1, (int) maskenHoehe - 1);

    brandLinks = brandMaske.Load(int3(xLinks, y, 0)).r;
    brandRechts = brandMaske.Load(int3(xRechts, y, 0)).r;
    brandOben = brandMaske.Load(int3(x, yOben, 0)).r;
    brandUnten = brandMaske.Load(int3(x, yUnten, 0)).r;

    gradientX = brandRechts - brandLinks;
    gradientY = brandUnten - brandOben;

    // Ein horizontaler UV-Weg von 1.0 entspricht
    // mehr Pixeln als ein vertikaler UV-Weg von 1.0.
    //
    // Die Geschwindigkeit definieren wir relativ zur
    // Bildschirmhöhe.

    aspectCorrection = (float) maskenHoehe / (float) maskenBreite;

    richtung = float2(gradientX * aspectCorrection, gradientY);

    if (dot(richtung, richtung) < 0.00000001)
    {
        return float2(0.0, -1.0);
    }

    return normalize(richtung);
}


// ------------------------------------------------------------
// Ermittelt die perspektivische Richtung.
//
// Wir blicken senkrecht auf das Bild.
//
// Ein Partikel, das scheinbar aus der Bildebene aufsteigt,
// wandert perspektivisch vom Projektionszentrum weg.
//
// Bildschirmzentrum:
// (0.5, 0.5)
// ------------------------------------------------------------

float2 ErmittlePerspektivRichtung(float2 position)
{
    float2 radial;

    float laenge;

    radial = position - float2(0.5, 0.5);

    laenge = length(radial);

    if (laenge < 0.0001)
    {
        return float2(0.0, -1.0);
    }

    return radial / laenge;
}


// ------------------------------------------------------------
// Übergangs-Verwirbelung V0.6.
//
// Ab jetzt - kohärentes Vektorfeld.
//
// ------------------------------------------------------------

// ------------------------------------------------------------
// 2D-Hashwert für räumliche Felder.
//
// Liefert für jede ganzzahlige Zelle einen stabilen,
// reproduzierbaren Zufallswert.
// ------------------------------------------------------------

float ZufallAusZelle(int2 zelle, uint salt)
{
    uint hash;

    hash = HashUint((uint) zelle.x * 0x8DA6B343u ^ (uint) zelle.y * 0xD8163841u ^ salt);

    return (float) (hash & 0x00FFFFFF) / 16777215.0;
}


// ------------------------------------------------------------
// Weiche Interpolationskurve für Value Noise.
// ------------------------------------------------------------

float2 NoiseFade(float2 wert)
{
    return wert * wert * (3.0 - 2.0 * wert);
}


// ------------------------------------------------------------
// Einfaches räumlich kohärentes 2D-Value-Noise.
// ------------------------------------------------------------

float ValueNoise2D(float2 position)
{
    int2 zelle;

    float2 lokal;
    float2 fade;

    float wert00;
    float wert10;
    float wert01;
    float wert11;

    float wertOben;
    float wertUnten;

    zelle = (int2) floor(position);

    lokal = frac(position);

    fade = NoiseFade(lokal);

    wert00 = ZufallAusZelle(zelle + int2(0, 0), 0xA511E9B3u);
    wert10 = ZufallAusZelle(zelle + int2(1, 0), 0xA511E9B3u);
    wert01 = ZufallAusZelle(zelle + int2(0, 1), 0xA511E9B3u);
    wert11 = ZufallAusZelle(zelle + int2(1, 1), 0xA511E9B3u);

    wertOben = lerp(wert00, wert10, fade.x);
    wertUnten = lerp(wert01, wert11, fade.x);

    return lerp(wertOben, wertUnten, fade.y);
}


// ------------------------------------------------------------
// FBM.
//
// Vier Oktaven reichen für unser Flowfield völlig aus.
// Das FBM erzeugt NICHT selbst die Strömungsrichtung,
// sondern verzerrt lediglich die Voronoi-Domäne.
// ------------------------------------------------------------

float FBM2D(float2 position)
{
    float wert;
    float amplitude;
    float frequenz;

    uint oktave;

    wert = 0.0;

    amplitude =  0.5;

    frequenz = 1.0;

    for (oktave = 0; oktave < 4; oktave++)
    {
        wert += ValueNoise2D(position * frequenz) * amplitude;

        frequenz *= 2.03;

        amplitude *= 0.5;
    }

    return wert;
}


// ------------------------------------------------------------
// Feature-Point einer Voronoi-Zelle.
//
// Jede Zelle erhält einen stabilen zufälligen Punkt.
// ------------------------------------------------------------

float2 ErmittleVoronoiPunkt(int2 zelle)
{
    float2 offset;

    offset.x = ZufallAusZelle(zelle, 0x68E31DA4u);
    offset.y = ZufallAusZelle(zelle, 0xB5297A4Du);

    return (float2) zelle + offset;
}


// ------------------------------------------------------------
// Strömungsrichtung einer Voronoi-Zelle.
//
// Jede Zelle besitzt eine eigene Grundrichtung.
//
// Die Richtung verändert sich LANGSAM über die Zeit.
// Dadurch "atmet" das Feld, ohne hektisch zu zittern.
// ------------------------------------------------------------

float2 ErmittleZellStroemung(int2 zelle, float aktuelleZeit)
{
    float grundWinkel;
    float phase;
    float zeitVariation;
    float winkel;

    grundWinkel = ZufallAusZelle(zelle, 0x1B56C4E9u) * 2.0 * PI;

    phase = ZufallAusZelle(zelle, 0xC2B2AE35u) * 2.0 * PI;

    zeitVariation = sin(aktuelleZeit * 0.22 + phase) * 0.45;

    winkel = grundWinkel + zeitVariation;

    return float2(cos(winkel), sin(winkel));
}


// ------------------------------------------------------------
// FBM-verzerrtes Voronoi-Flowfield.
//
// 1. Bildschirmkoordinaten werden auf echtes
//    Seitenverhältnis korrigiert.
// 2. FBM verzerrt die Abfrageposition.
// 3. Wir suchen die zwei nächsten Voronoi-Zellen.
// 4. Ihre Strömungsvektoren werden an den Zellgrenzen
//    weich ineinander überführt.
//
// Ergebnis:
// Benachbarte Partikel erleben ähnliche Kräfte.
// ------------------------------------------------------------

float2 ErmittleFlowField(float2 position, float aktuelleZeit)
{
    uint maskenBreite;
    uint maskenHoehe;

    float aspectRatio;

    float2 feldPosition;
    float2 warpPosition;

    float warpX;
    float warpY;

    int2 basisZelle;
    int2 nachbarOffset;
    int2 aktuelleZelle;

    int2 naechsteZelle;
    int2 zweiteZelle;

    float2 featurePoint;
    float2 differenz;

    float distanzQuadrat;
    float naechsteDistanz;
    float zweiteDistanz;

    float2 ersteRichtung;
    float2 zweiteRichtung;

    float mischung;

    int x;
    int y;

    brandMaske.GetDimensions(maskenBreite, maskenHoehe);

    aspectRatio = (float) maskenBreite / (float) maskenHoehe;


    // --------------------------------------------------------
    // Großräumige Zellen.
    //
    // Ca. 5 Zellen über die Bildschirmhöhe.
    // --------------------------------------------------------

    feldPosition = float2(position.x * aspectRatio, position.y) * 5.0;


    // --------------------------------------------------------
    // Langsam wandernder Domain-Warp.
    // --------------------------------------------------------

    warpPosition = feldPosition * 0.55 + float2(aktuelleZeit * 0.035, aktuelleZeit * -0.027);

    warpX = FBM2D(warpPosition + float2(13.17, 7.31));
    warpY = FBM2D(warpPosition + float2(41.73, 29.11));
    
    // Von 0..1 nach ungefähr -1..+1.
    warpX = warpX * 2.0 - 1.0;
    warpY = warpY * 2.0 - 1.0;


    // Deutliche, aber nicht groteske Verformung.
    feldPosition += float2(warpX, warpY) * 1.10;
    
    basisZelle = (int2) floor(feldPosition);

    naechsteDistanz = 1000000.0;
    zweiteDistanz = 1000000.0;
    
    naechsteZelle = basisZelle;
    zweiteZelle = basisZelle;


    // --------------------------------------------------------
    // Nächste und zweitnächste Voronoi-Zelle suchen.
    // --------------------------------------------------------

    for (y = -1; y <= 1; y++)
    {
        for (x = -1; x <= 1; x++)
        {
            nachbarOffset = int2(x, y);

            aktuelleZelle = basisZelle + nachbarOffset;

            featurePoint = ErmittleVoronoiPunkt(aktuelleZelle);

            differenz = featurePoint - feldPosition;

            distanzQuadrat = dot(differenz, differenz);

            if (distanzQuadrat < naechsteDistanz)
            {
                zweiteDistanz = naechsteDistanz;

                zweiteZelle = naechsteZelle;

                naechsteDistanz = distanzQuadrat;

                naechsteZelle = aktuelleZelle;
            }
            else if (distanzQuadrat < zweiteDistanz)
            {
                zweiteDistanz = distanzQuadrat;

                zweiteZelle = aktuelleZelle;
            }
        }
    }


    ersteRichtung = ErmittleZellStroemung(naechsteZelle, aktuelleZeit);

    zweiteRichtung = ErmittleZellStroemung(zweiteZelle, aktuelleZeit);


    // --------------------------------------------------------
    // Je ähnlicher die beiden Distanzen sind, desto näher
    // befinden wir uns an einer Zellgrenze.
    //
    // Dort mischen wir beide Richtungen weich.
    // Tief innerhalb einer Zelle dominiert deren Richtung.
    // --------------------------------------------------------

    mischung = saturate(0.5 - (zweiteDistanz - naechsteDistanz) * 1.5);

    mischung = smoothstep(0.0, 0.5, mischung);

    return normalize(lerp(ersteRichtung, zweiteRichtung, mischung));
}


// ------------------------------------------------------------
// Ermittelt den lokalen thermischen Einfluss der Brandmaske.
//
// Die Brandmaske enthält für jeden Bildpunkt den Zeitpunkt,
// zu dem die Brandfront diesen Punkt erreicht.
//
// Vor der Brandfront:
//     keine thermische Wirkung.
//
// Direkt an bzw. kurz hinter der Brandfront:
//     maximale thermische Wirkung.
//
// Weiter hinter der Brandfront:
//     langsames Abklingen der heißen Luft.
// ------------------------------------------------------------


float ErmittleHitzeEinfluss(float2 position, float aktuellerProgress)
{
    uint maskenBreite;
    uint maskenHoehe;

    int2 texelPosition;

    float brandZeit;
    float zeitSeitBrand;

    float aufheizBreite;
    float abkuehlBreite;

    float aufheizen;
    float abkuehlen;


    // --------------------------------------------------------
    // Abmessungen der zeitcodierten Brandmaske ermitteln.
    // --------------------------------------------------------

    brandMaske.GetDimensions(maskenBreite, maskenHoehe);


    // --------------------------------------------------------
    // Normierte Partikelposition 0..1 in Texelkoordinaten
    // der Brandmaske umrechnen.
    // --------------------------------------------------------

    texelPosition = int2(position.x * (float) (maskenBreite - 1), position.y * (float) (maskenHoehe - 1));
    texelPosition = clamp(texelPosition, int2(0, 0), int2((int) maskenBreite - 1, (int) maskenHoehe - 1));


    // --------------------------------------------------------
    // Zeitpunkt lesen, zu dem die Brandfront diese Position
    // erreicht.
    // --------------------------------------------------------

    brandZeit = brandMaske.Load(int3(texelPosition, 0));
    
    zeitSeitBrand = aktuellerProgress - brandZeit;


    // --------------------------------------------------------
    // Etwas Wirkung bereits unmittelbar VOR dem Eintreffen
    // der sichtbaren Brandfront.
    //
    // Heiße Luft wartet schließlich nicht höflich darauf,
    // dass der Pixel offiziell verbrannt ist. :-)
    // --------------------------------------------------------
    
    aufheizBreite = 0.025;

    aufheizen = smoothstep(-aufheizBreite, 0.0, zeitSeitBrand);


    // --------------------------------------------------------
    // Deutlich längere Abkühlzone HINTER der Brandfront.
    // --------------------------------------------------------

    abkuehlBreite = 0.18;
    
    abkuehlen = 1.0 - smoothstep(0.0, abkuehlBreite, zeitSeitBrand);


    return saturate(aufheizen * abkuehlen);
}

// ------------------------------------------------------------
// Wiedergeburt an der aktuellen Brandkante.
// ------------------------------------------------------------

bool VersuchePartikelZuErzeugen(uint particleIndex, inout Particle particle)
{
    uint seed;

    float spawnChance;

    float2 spawnPosition;

    float2 brandRichtung;
    float2 startRichtung;

    float richtungsVorzeichen;

    float winkelStreuung;
    float geschwindigkeit;

    float schnell;

    seed = particle.seed;

    // Alle ursprünglich leeren Slots besitzen Seed = 0.
    // Der Slotindex erzeugt einen eindeutigen Startzustand.

    if (seed == 0)
    {
        seed =  HashUint(particleIndex + 0x9E3779B9);
    }

    // spawnRate beschreibt die gewünschte Zahl von
    // Spawn-VERSUCHEN über den gesamten Pool pro Sekunde.
    //
    // Die tatsächlich geborene Zahl hängt zusätzlich davon ab,
    // wie viele Slots tot sind und ob ein Punkt auf der
    // aktuellen Brandkante gefunden wird.

    spawnChance = spawnRate * deltaTime / PARTICLE_COUNT;

    if (Random01(seed) > spawnChance)
    {
        particle.seed = seed;

        return false;
    }

    if (!FindeBrandkantenPosition(seed, spawnPosition))
    {
        particle.seed = seed;

        return false;
    }

    particle.position = spawnPosition;


    // --------------------------------------------------------
    // Lokale Brandrichtung.
    // --------------------------------------------------------

    brandRichtung = ErmittleBrandrichtung(spawnPosition);


    // --------------------------------------------------------
    // 70 % mit der Brandrichtung,
    // 30 % gegen die Brandrichtung.
    // --------------------------------------------------------

    if (Random01(seed) < 0.7)
    {
        richtungsVorzeichen = 1.0;
    }
    else
    {
        richtungsVorzeichen = -1.0;
    }

    startRichtung = brandRichtung * richtungsVorzeichen;


    // --------------------------------------------------------
    // Leichte individuelle Winkelstreuung.
    // 
    // +/- 25 Grad.
    // --------------------------------------------------------

    winkelStreuung = RandomRange(seed, -0.436332313, 0.436332313);

    startRichtung = DreheVektor(startRichtung, winkelStreuung);


    // --------------------------------------------------------
    // Geschwindigkeitsklassen.
    //
    // 92 %:
    // langsame / normale Funken
    //
    // 8 %:
    // schnelle Ausreißer mit stärkerem Aufwärtsanteil
    // --------------------------------------------------------

    schnell = Random01(seed);

    if (schnell < 0.08)
    {
        // Schnelle Funken dürfen deutlicher nach oben ziehen.

        startRichtung = normalize(startRichtung * 0.55 + float2(0.0, -1.0) * 0.45);

        geschwindigkeit = RandomRange(seed, 0.10, 0.19);
    }
    else
    {
        geschwindigkeit = RandomRange(seed, 0.020, 0.055);
    }

    particle.velocity = startRichtung * geschwindigkeit;


    // --------------------------------------------------------
    // Lebenszyklus.
    // --------------------------------------------------------

    particle.age = 0.0;
    particle.lifetime = RandomRange(seed, partikelMaxLebensdauer * 0.5, partikelMaxLebensdauer);
    particle.size = RandomRange(seed, 0.0012, 0.0030);
    particle.seed = seed;

    return true;
}


// ------------------------------------------------------------
// Simulation
// ------------------------------------------------------------

[numthreads(64, 1, 1)]
void CSMain(uint3 dispatchThreadId : SV_DispatchThreadID)
{
    uint particleIndex;

    Particle particle;

    float lifeProgress;

    float2 flowRichtung;
    float2 perspektivRichtung;
    float2 schwerkraft;
    
    float hitzeEinfluss;
    float effektiveFlowStaerke;
    
    float perspektivStaerke;

    particleIndex = dispatchThreadId.x;

    if (particleIndex >= PARTICLE_COUNT)
    {
        return;
    }

    particle = particles[particleIndex];


    // --------------------------------------------------------
    // Toter / leerer Slot:
    // Chance auf Wiedergeburt an der Brandkante.
    // --------------------------------------------------------

    if (particle.lifetime <= 0.0)
    {
    // --------------------------------------------------------
    // Nachlaufphase:
    //
    // Sobald die eigentliche Transition progress = 1 erreicht,
    // wird die Emission vollständig abgeschaltet.
    //
    // Bereits lebende Partikel dürfen weiter simuliert werden,
    // tote Slots bleiben jedoch tot.
    // --------------------------------------------------------

        if (progress < 1.0)
        {
            VersuchePartikelZuErzeugen(particleIndex, particle);
        }

        particles[particleIndex] = particle;

        return;
    }


    // --------------------------------------------------------
    // Altern.
    // --------------------------------------------------------

    particle.age += deltaTime;


    // --------------------------------------------------------
    // Tod durch Alter.
    //
    // Bob Ross' Happy Little Accident im Lebenszyklus-
    // Gradient wird hier selbstverständlich NICHT angefasst.
    // --------------------------------------------------------

    if (particle.age >= particle.lifetime)
    {
        particle.lifetime = 0.0;

        particles[particleIndex] = particle;

        return;
    }


    lifeProgress = saturate(particle.age / particle.lifetime);


// --------------------------------------------------------
// 1. Räumlich kohärentes Strömungsfeld.
//
// Die Stärke des Feldes wird durch die lokale thermische
// Aktivität der zeitcodierten Brandmaske moduliert.
// --------------------------------------------------------

    flowRichtung = ErmittleFlowField(particle.position, zeit);

    hitzeEinfluss = ErmittleHitzeEinfluss(particle.position, progress);

    effektiveFlowStaerke = flowFieldStaerke * hitzeEinfluss;

    particle.velocity += flowRichtung * effektiveFlowStaerke * deltaTime;


  // --------------------------------------------------------
// 2. Projektionsmodell / Schwerkraft.
// --------------------------------------------------------

    if (schwerkraftAktiv > 0.5)
    {
    // Bild hängt:
    // konstante Gravitation nach Bildschirm-unten.

        schwerkraft = float2(0.0, 0.055);

        particle.velocity += schwerkraft * deltaTime;
    }
    else
    {
    // Top View:
    // altersabhängiger perspektivischer Höhen-Drift.

        perspektivRichtung = ErmittlePerspektivRichtung(particle.position);

        perspektivStaerke = lifeProgress * lifeProgress * 0.060;

        particle.velocity += perspektivRichtung * perspektivStaerke * deltaTime;
    }

    // --------------------------------------------------------
    // Bewegung.
    // --------------------------------------------------------

    particle.position += particle.velocity * deltaTime;


    // --------------------------------------------------------
    // Außerhalb des sichtbaren Bereiches?
    // --------------------------------------------------------

    if (particle.position.x < -0.05 ||
        particle.position.x > 1.05 ||
        particle.position.y < -0.05 ||
        particle.position.y > 1.05)
    {
        particle.lifetime = 0.0;

        particles[particleIndex] = particle;
        
        return;
    }


    particles[particleIndex] = particle;
}