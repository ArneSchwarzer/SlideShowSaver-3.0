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
// Übergangs-Verwirbelung V0.4.
//
// Noch KEIN kohärentes Vektorfeld.
//
// Jeder Funke bekommt aus Seed + Alter zwei weiche
// Schwingungen. Dadurch verändert sich die Flugrichtung
// kontinuierlich statt frameweise zufällig zu zittern.
//
// Diese Methode wird später durch unser echtes
// Strömungs-/Vektorfeld ersetzt.
// ------------------------------------------------------------

float2 ErmittleVerwirbelung(Particle particle)
{
    float phaseX;
    float phaseY;

    float frequenzX;
    float frequenzY;

    float staerke;

    float verwirbelungX;
    float verwirbelungY;

    phaseX = ZufallAusSeed(particle.seed ^ 0xA511E9B3) * 2.0 * PI;
    phaseY = ZufallAusSeed(particle.seed ^ 0x63D83595) * 2.0 * PI;

    frequenzX = 2.0 + ZufallAusSeed(particle.seed ^ 0xB5297A4D) * 3.0;
    frequenzY = 1.5 + ZufallAusSeed(particle.seed ^ 0x68E31DA4) * 2.5;

    staerke = 0.008 + ZufallAusSeed(particle.seed ^ 0x1B56C4E9) * 0.018;

    verwirbelungX = sin(particle.age * frequenzX + phaseX);
    verwirbelungY = cos(particle.age * frequenzY + phaseY);

    return float2(verwirbelungX, verwirbelungY) * staerke;
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

        geschwindigkeit = RandomRange(seed, 0.12, 0.22);
    }
    else
    {
        geschwindigkeit = RandomRange(seed, 0.025, 0.070);
    }

    particle.velocity = startRichtung * geschwindigkeit;


    // --------------------------------------------------------
    // Lebenszyklus.
    // --------------------------------------------------------

    particle.age = 0.0;
    particle.lifetime = RandomRange(seed, 1.6, 3.2);
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

    float2 verwirbelung;
    float2 perspektivRichtung;

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
        VersuchePartikelZuErzeugen(particleIndex, particle);

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
    // 1. Lokale Verwirbelung.
    //
    // Kleine Beschleunigung, die sich weich über die Zeit
    // verändert.
    // --------------------------------------------------------

    verwirbelung = ErmittleVerwirbelung(particle);

    particle.velocity += verwirbelung * deltaTime;


    // --------------------------------------------------------
    // 2. Perspektivischer Höhen-Drift.
    //
    // Je älter das Partikel, desto stärker wird der Einfluss.
    //
    // quadratisch:
    //
    // jung    -> fast 0
    // mittel  -> merkbar
    // alt     -> deutlich
    // --------------------------------------------------------

    perspektivRichtung = ErmittlePerspektivRichtung(particle.position);

    perspektivStaerke = lifeProgress * lifeProgress * 0.060;

    particle.velocity += perspektivRichtung * perspektivStaerke * deltaTime;


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