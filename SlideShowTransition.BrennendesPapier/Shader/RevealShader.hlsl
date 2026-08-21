Texture2D oldImageTexture : register(t0);
Texture2D newImageTexture : register(t1);
Texture2D maskTexture : register(t2);

SamplerState sourceSampler : register(s0);

cbuffer RevealParameter : register(b0)
{
    float progress;
    float zeit;
    float renderBreite;
    float renderHoehe;

    uint distortionModus;
    float magieRasterHoehe;
    float distortionStaerke;
    float magieNachlaufProgress;

    uint verzerrungAktiv;
    uint gradientAktiv;
    float verzerrungsEffektStaerke;
    float verzerrungsBreite;

    float revealNachlaufProgress;
    float saeureNachlaufProgress;
    float feuerNachlaufProgress;
    float blitzNachlaufProgress;
};

// ------------------------------------------------------------
// VertexShader
//
// ------------------------------------------------------------

struct VertexOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};

VertexOutput VSMain(uint vertexId : SV_VertexID)
{
    VertexOutput output;

    float2 positions[3] =
    {
        float2(-1.0, -1.0),
        float2(-1.0, 3.0),
        float2(3.0, -1.0)
    };

    float2 texCoords[3] =
    {
        float2(0.0, 1.0),
        float2(0.0, -1.0),
        float2(2.0, 1.0)
    };

    output.position = float4(positions[vertexId], 0.0, 1.0);

    output.texCoord = texCoords[vertexId];

    return output;
}

// ------------------------------------------------------------
// Deterministischer Pseudo-Zufall
//
// aka Hash-Funktionen
// ------------------------------------------------------------

uint HashUint(uint value)
{
    value ^= value >> 16;
    value *= 0x7FEB352Du;
    value ^= value >> 15;
    value *= 0x846CA68Bu;
    value ^= value >> 16;

    return value;
}

uint ErmittleZellHash(int2 zelle)
{
    uint hash;

    hash = (uint) zelle.x * 0x8DA6B343u;
    hash ^= (uint) zelle.y * 0xD8163841u;

    return HashUint(hash);
}

float Hash01(uint value)
{
    uint hash;

    hash = HashUint(value);

    return (float) (hash & 0x00FFFFFFu) / 16777215.0;
}

float HashSigned(uint value)
{
    return Hash01(value) * 2.0 - 1.0;
}

// ------------------------------------------------------------
// Allgemeine FBM-Funktionen
//
// Generische Noise-Basis für verschiedene Effekte.
// ------------------------------------------------------------

float2 NoiseFade(float2 wert)
{
    return wert * wert * (3.0 - 2.0 * wert);
}

float ZufallAusZelle(int2 zelle, uint salt)
{
    uint hash;

    hash =
        HashUint(
            (uint) zelle.x * 0x68E31DA4u ^
            (uint) zelle.y * 0xB5297A4Du ^
            salt);

    return (float) (hash & 0x00FFFFFFu) / 16777215.0;
}

float ValueNoise2D(float2 position, uint salt)
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
    
    wert00 = ZufallAusZelle(zelle + int2(0, 0), salt);
    wert10 = ZufallAusZelle(zelle + int2(1, 0), salt);
    wert01 = ZufallAusZelle(zelle + int2(0, 1), salt);
    wert11 = ZufallAusZelle(zelle + int2(1, 1), salt);
    
    wertOben = lerp(wert00, wert10, fade.x);
    wertUnten = lerp(wert01, wert11, fade.x);

    return lerp(wertOben, wertUnten, fade.y);
}

// ------------------------------------------------------------
// Säure
//
// Bereich für den Verzerrungseffekt "Saeure"
// ------------------------------------------------------------

float SaeureFBM(float2 position, uint salt)
{
    float wert;
    float amplitude;
    float frequenz;

    uint oktave;
    
    wert = 0.0;

    amplitude = 0.5;

    frequenz = 1.0;


    for (oktave = 0u; oktave < 5u; oktave++)
    {
        wert += ValueNoise2D(position * frequenz, salt + oktave * 0x9E3779B9u) * amplitude;

        frequenz *= 2.07;

        amplitude *= 0.52;
    }


    return wert;
}

float ErmittleSaeureMediumDichte(float2 uv)
{
    float aspectRatio;

    float2 position;
    float2 warpPosition;
    float2 warp;

    float basisDichte;
    float detailDichte;

    float dichte;


    aspectRatio = renderBreite / max(renderHoehe, 1.0);
    
    position = float2(uv.x * aspectRatio, uv.y);
    
    // Langsame Bewegung des Mediums.

    position += float2(zeit * 0.035, zeit * -0.018);


    // --------------------------------------------------------
    // Domain Warp.
    // --------------------------------------------------------

    warpPosition = position * 2.2;

    warp.x =SaeureFBM(warpPosition + float2(17.3, 41.7), 0xC2B2AE35u);
    warp.y =SaeureFBM(warpPosition + float2(63.1, 11.9), 0x27D4EB2Fu);
    
    warp = warp * 2.0 - 1.0;
    
    position += warp * 0.65;
    
    // --------------------------------------------------------
    // Großräumige Dichte.
    // --------------------------------------------------------

    basisDichte = SaeureFBM(position * 2.4, 0xA511E9B3u);
    
    // Etwas feinere Struktur.

    detailDichte = SaeureFBM(position * 5.3 + float2(31.7, 7.9), 0x63D83595u);
    
    dichte = basisDichte * 0.72 + detailDichte * 0.28;
    
    return saturate(dichte);
}

float ErmittleSaeureSchlieren(float2 uv, float2 normale, float2 tangente)
{
    float aspectRatio;

    float entlang;
    float quer;

    float2 schlierenPosition;
    float2 warp;

    float schlieren1;
    float schlieren2;

    float schlieren;


    aspectRatio = renderBreite / max(renderHoehe, 1.0);
    
    uv.x *= aspectRatio;

    entlang = dot(uv, tangente);

    quer = dot(uv, normale);
    
    // --------------------------------------------------------
    // Stark anisotrop:
    //
    // entlang langsam,
    // quer deutlich schneller.
    //
    // Dadurch entstehen lange Fasern statt Wolkenflecken.
    // --------------------------------------------------------

    schlierenPosition = float2(entlang * 3.2, quer * 18.0);
    
    schlierenPosition.x += zeit * 0.18;
    
    warp.x = SaeureFBM(schlierenPosition * 0.55 + float2(9.7, 53.1), 0xB5297A4Du);
    warp.y = SaeureFBM(schlierenPosition * 0.55 + float2(47.2, 3.4), 0x1B56C4E9u);
    
    warp = warp * 2.0 - 1.0;
    
    schlierenPosition += warp * float2(1.1, 2.8);
    
    schlieren1 = SaeureFBM(schlierenPosition, 0x68E31DA4u);
    schlieren2 = SaeureFBM(schlierenPosition * float2(1.7, 0.85) + float2(21.3, 37.8), 0xD8163841u);
    
    schlieren =  schlieren1 * 0.68 + schlieren2 * 0.32;
    
    // Nur die dichteren Bereiche zu klaren Schlieren machen.

    schlieren = smoothstep(0.46, 0.72, schlieren);
    
    return saturate(schlieren);
}

float ErmittleSaeureDunstEinfluss(float maskValue, float aktuellerProgress)
{
    float frontDistance;

    float breiteVorFront;
    float breiteHinterFront;

    float effektProgress;

    float einfluss;


    // Dunst darf der Reaktionsfront etwas vorauslaufen
    // und sehr lange hinter ihr bestehen bleiben.

    breiteVorFront = verzerrungsBreite * 0.75;
    breiteHinterFront = verzerrungsBreite * 1.65;
    
    effektProgress = aktuellerProgress + saeureNachlaufProgress * breiteHinterFront;
    
    frontDistance = maskValue - effektProgress;
    
    if (frontDistance >= 0.0)
    {
        einfluss = 1.0 - smoothstep(0.0, breiteVorFront, frontDistance);
    }
    else
    {
        einfluss = 1.0 - smoothstep(0.0, breiteHinterFront, -frontDistance);
    }
    
    return saturate(einfluss);
}

float ErmittleSaeureKorrosionsEinfluss(float maskValue, float aktuellerProgress)
{
    float frontDistance;

    float breiteVorFront;
    float breiteHinterFront;

    float effektProgress;

    float einfluss;
    
    // Die eigentliche optische Korrosion bleibt viel enger
    // an der chemisch aktiven Front als der Dunst.

    breiteVorFront = verzerrungsBreite * 0.45;
    breiteHinterFront = verzerrungsBreite * 0.60;
    
    effektProgress = aktuellerProgress + saeureNachlaufProgress * breiteHinterFront;
    
    frontDistance = maskValue - effektProgress;
    
    if (frontDistance >= 0.0)
    {
        einfluss = 1.0 - smoothstep(0.0, breiteVorFront, frontDistance);
    }
    else
    {
        einfluss = 1.0 - smoothstep(0.0, breiteHinterFront, -frontDistance);
    }


    // Die ohnehin schon kleinere Zone zusätzlich etwas
    // konzentrieren. Dadurch bekommt die Brechung eine
    // klarere Reaktionsfront statt eines breiten Wasserfilms.

    einfluss = smoothstep(0.12, 0.88, einfluss);
    
    return saturate(einfluss);
}

float2 ErmittleBrandFrontNormale(float2 uv)
{
    float2 texelSize;

    float links;
    float rechts;
    float oben;
    float unten;

    float2 gradient;
    float gradientLaenge;


    texelSize = float2(1.0 / max(renderBreite, 1.0), 1.0 / max(renderHoehe, 1.0));
    
    links = maskTexture.SampleLevel(sourceSampler, uv - float2(texelSize.x, 0.0), 0).r;
    rechts = maskTexture.SampleLevel(sourceSampler, uv + float2(texelSize.x, 0.0), 0).r;
    oben = maskTexture.SampleLevel(sourceSampler, uv - float2(0.0, texelSize.y), 0).r;
    unten = maskTexture.SampleLevel(sourceSampler, uv + float2(0.0, texelSize.y), 0).r;
    
    gradient = float2(rechts - links, unten - oben);

    gradientLaenge = length(gradient);
    
    if (gradientLaenge < 0.00001)
    {
        return float2(0.0, -1.0);
    }
    
    return gradient / gradientLaenge;
}

float2 ErmittleSaeureDistortionUV(float2 uv, float frontEinfluss)
{
    float2 normale;
    float2 tangente;

    float2 texelSize;

    float dichteLinks;
    float dichteRechts;
    float dichteOben;
    float dichteUnten;

    float2 dichteGradient;

    float schlieren;
    float tangentialeBrechung;

    float2 saeureUV;


    normale = ErmittleBrandFrontNormale(uv);

    tangente = float2(-normale.y, normale.x);
    
    texelSize = float2(2.0 / max( renderBreite, 1.0), 2.0 / max(renderHoehe, 1.0));

    // --------------------------------------------------------
    // Gradient des Dichtefeldes.
    //
    // Änderung der Dichte = Änderung des Brechungsindex.
    // --------------------------------------------------------

    dichteLinks = ErmittleSaeureMediumDichte(uv - float2(texelSize.x, 0.0));
    dichteRechts = ErmittleSaeureMediumDichte(uv + float2( texelSize.x, 0.0));
    dichteOben = ErmittleSaeureMediumDichte(uv -  float2( 0.0, texelSize.y));
    dichteUnten = ErmittleSaeureMediumDichte(uv + float2(0.0, texelSize.y));
    
    dichteGradient = float2(dichteRechts - dichteLinks, dichteUnten - dichteOben);
    
    schlieren = ErmittleSaeureSchlieren(uv, normale, tangente);

    tangentialeBrechung = (schlieren - 0.5) * 2.0;
    
    saeureUV = uv;
    
    // Hauptbrechung durch das Medium.

    saeureUV += dichteGradient * 0.055 * verzerrungsEffektStaerke * frontEinfluss;
    
    // Etwas gerichtete Strömung entlang der Front.

    saeureUV += tangente * tangentialeBrechung * 0.006 * verzerrungsEffektStaerke * frontEinfluss;
    
    return saturate(saeureUV);
}

float3 ErmittleSaeureDunstFarbe(float dichte)
{
    float3 dunklerDunst;
    float3 hellerDunst;
    
    dunklerDunst = float3(0.26, 0.27, 0.10);
    hellerDunst = float3( 0.62, 0.63, 0.24);
    
    return lerp(dunklerDunst, hellerDunst, saturate(dichte));
}

float ErmittleSaeureDunstDichte(float2 uv, float frontEinfluss)  
{
    float2 normale;
    float2 tangente;

    float mediumDichte;
    float schlieren;

    float dichte;
    
    normale = ErmittleBrandFrontNormale(uv);

    tangente = float2(-normale.y, normale.x);
    
    mediumDichte = ErmittleSaeureMediumDichte(uv);
    
    schlieren = ErmittleSaeureSchlieren(uv, normale, tangente);
    
    // --------------------------------------------------------
    // Schlieren leben IM Medium.
    // --------------------------------------------------------

    schlieren *= smoothstep(0.28, 0.72, mediumDichte);

    dichte = mediumDichte * 0.78 + schlieren * 0.48;
    dichte = smoothstep(0.18, 0.88, dichte);
    
    return saturate(dichte * frontEinfluss);
}

// ------------------------------------------------------------
// Hitze
//
// Bereich für den Verzerrungseffekt "Hitze", der, mit 
// unterschiedlichen Parametern, sowohl für "Feuer" als auch
// für "Blitze" die flimmernde Luft simuliert. 
// ------------------------------------------------------------

float HitzeFBM(float2 position)
{
    float wert;
    float amplitude;
    float frequenz;

    uint oktave;


    wert = 0.0;
    amplitude = 0.5;
    frequenz = 1.0;
    
    for (
        oktave = 0u;
        oktave < 5u;
        oktave++)
    {
        wert += ValueNoise2D(position * frequenz, 0x517CC1B7u + oktave * 0x9E3779B9u) * amplitude;
        frequenz *= 2.03;
        amplitude *= 0.50;
    }
    
    return wert;
}

float ErmittleHitzeFeld(float2 uv, bool istBlitz)
{
    float aspectRatio;

    float2 position;
    float2 warp;

    float basis;
    float detail;

    float aufstieg;
    float turbulenz;
    float basisFrequenz;
    float detailFrequenz;
    float warpStaerke;

    float feld;


    aspectRatio = renderBreite / max(renderHoehe, 1.0);
    
    position = float2(uv.x * aspectRatio, uv.y);
    
    if (istBlitz)
    {
        // Blitz:
        // schneller, feiner und nervöser.

        aufstieg = 0.11;
        turbulenz = 0.055;
        basisFrequenz = 4.2;
        detailFrequenz = 9.5;
        warpStaerke = 0.32;
    }
    else
    {
        // Feuer:
        // langsamer, größer und thermischer.

        aufstieg = 0.055;
        turbulenz = 0.025;
        basisFrequenz = 2.4;
        detailFrequenz = 5.8;
        warpStaerke = 0.46;
    }


    // Heiße Luft steigt nach oben.
    // UV-Y wächst nach unten.

    position.y -= zeit * aufstieg;

    // Kleine seitliche Bewegung.

    position.x += sin(zeit * 0.37) * turbulenz;


    // --------------------------------------------------------
    // Domain Warp
    // --------------------------------------------------------

    warp.x = HitzeFBM(position * 1.7 + float2(13.7, 41.3));
    warp.y = HitzeFBM(position * 1.7 + float2(57.1, 8.9));
    
    warp = warp * 2.0 - 1.0;
    
    position += warp * warpStaerke;
    
    basis = HitzeFBM(position * basisFrequenz);
    
    detail = HitzeFBM(position * detailFrequenz + float2(31.7, 19.4));
    
    feld = basis * 0.72 + detail * 0.28;
    
    return saturate(feld);
}

float ErmittleHitzeFrontEinfluss(float maskValue, float aktuellerProgress, bool istBlitz)
{
    float frontDistance;

    float breiteVorFront;
    float breiteHinterFront;

    float effektProgress;
    float nachlaufProgress;

    float einfluss;


    if (istBlitz)
    {
        breiteVorFront = verzerrungsBreite * 0.50;
        breiteHinterFront = verzerrungsBreite * 0.68;
        nachlaufProgress = blitzNachlaufProgress;
    }
    else
    {
        breiteVorFront = verzerrungsBreite * 0.65;
        breiteHinterFront = verzerrungsBreite * 0.95;
        nachlaufProgress = feuerNachlaufProgress;
    }


    effektProgress = aktuellerProgress + nachlaufProgress * breiteHinterFront;

    frontDistance = maskValue - effektProgress;


    if (frontDistance >= 0.0)
    {
        einfluss = 1.0 - smoothstep(0.0, breiteVorFront, frontDistance);
    }
    else
    {
        einfluss = 1.0 - smoothstep( 0.0, breiteHinterFront, -frontDistance);
    }


    return saturate(smoothstep(0.08, 0.92, einfluss));
}

float2 ErmittleHitzeDistortionUV(float2 uv, float hitzeEinfluss, bool istBlitz)
{
    float2 sampleAbstand;

    float links;
    float rechts;
    float oben;
    float unten;

    float2 temperaturGradient;

    float brechungsStaerke;

    float2 hitzeUV;


    sampleAbstand = float2(8.0 / max(renderBreite, 1.0), 8.0 / max(renderHoehe, 1.0));

    links = ErmittleHitzeFeld(uv - float2(sampleAbstand.x, 0.0), istBlitz);
    rechts = ErmittleHitzeFeld(uv + float2(sampleAbstand.x, 0.0), istBlitz);
    oben = ErmittleHitzeFeld(uv - float2(0.0, sampleAbstand.y), istBlitz);
    unten = ErmittleHitzeFeld(uv + float2(0.0, sampleAbstand.y), istBlitz);
    
    temperaturGradient = float2(rechts - links, unten - oben);
    
    if (istBlitz)
    {
        brechungsStaerke = 0.155;
    }
    else
    {
        brechungsStaerke = 0.095;
    }
    
    hitzeUV = uv;
    
    hitzeUV += temperaturGradient * brechungsStaerke * verzerrungsEffektStaerke * hitzeEinfluss;
    
    return saturate(hitzeUV);
}

// ------------------------------------------------------------
// Magie
//
// Bereich für den Verzerrungseffekt "Magie"
// ------------------------------------------------------------

struct MagieDreieck
{
    int2 zelle;

    uint dreieckIndex;
    uint dreieckId;

    bool diagonaleSteigt;

    float2 lokal;
    float2 pivotUV;
};

MagieDreieck ErmittleMagieDreieck(float2 uv)
{
// ------------------------------------------------------------
// Bestimmt Zelle und Dreieck für eine UV-Position.
//
// WICHTIG:
// Das Raster ist im Bildschirmraum quadratisch,
// nicht im UV-Raum.
// ------------------------------------------------------------
    
    MagieDreieck ergebnis;

    float aspectRatio;

    float zellHoeheUV;
    float zellBreiteUV;
    
    float2 pivotLokal;

    float2 rasterPosition;

    uint zellHash;


    // --------------------------------------------------------
    // Physisch quadratische Zellen.
    //
    // Bei 3840 × 2160 ist eine UV-Einheit horizontal
    // wesentlich größer als vertikal.
    // --------------------------------------------------------

    aspectRatio = renderBreite / max(renderHoehe, 1.0);

    zellHoeheUV = 1.0 / max(magieRasterHoehe, 1.0);
    zellBreiteUV = zellHoeheUV / aspectRatio;


    // --------------------------------------------------------
    // Kontinuierliche Rasterkoordinate.
    // --------------------------------------------------------

    rasterPosition = float2(uv.x / zellBreiteUV, uv.y / zellHoeheUV);
    
    ergebnis.zelle = (int2) floor(rasterPosition);
    ergebnis.lokal = frac(rasterPosition);


    // --------------------------------------------------------
    // Pro Zelle Diagonale festlegen.
    //
    // Bit 0:
    //     0 = \
    //     1 = /
    // --------------------------------------------------------

    zellHash = ErmittleZellHash(ergebnis.zelle);

    ergebnis.diagonaleSteigt = (zellHash & 1u) != 0u;


    // --------------------------------------------------------
    // Dreieck bestimmen.
    // --------------------------------------------------------

    if (ergebnis.diagonaleSteigt)
    {
        // /
        //
        //      /|
        //     / |
        //    /  |
        //   /___|
        //
        // Grenze:
        // lokal.x + lokal.y = 1

        if (ergebnis.lokal.x + ergebnis.lokal.y < 1.0)
        {
            ergebnis.dreieckIndex = 0u;
        }
        else
        {
            ergebnis.dreieckIndex = 1u;
        }
    }
    else
    {
        // \
        //
        //   |\ 
        //   | \
        //   |  \
        //   |___\
        //
        // Grenze:
        // lokal.y = lokal.x

        if (ergebnis.lokal.y < ergebnis.lokal.x)
        {
            ergebnis.dreieckIndex = 0u;
        }
        else
        {
            ergebnis.dreieckIndex = 1u;
        }
    }


    // --------------------------------------------------------
    // Eindeutige ID:
    //
    // Zellhash + Dreieck 0/1.
    //
    // Diese ID bleibt für die gesamte Transition stabil.
    // --------------------------------------------------------

    ergebnis.dreieckId = HashUint(zellHash ^ (ergebnis.dreieckIndex * 0x9E3779B9u));


    // --------------------------------------------------------
    // Echter Schwerpunkt des jeweiligen Dreiecks.
    //
    // Schwerpunkt eines Dreiecks:
    //
    //     S = (A + B + C) / 3
    //
    // Da unsere vier möglichen Dreiecksformen feststehen,
    // können wir die lokalen Schwerpunktkoordinaten direkt
    // angeben.
    // --------------------------------------------------------
    
    if (ergebnis.diagonaleSteigt)
    {
      // Diagonale /
      //
      // Dreieck 0: (0,0), (1,0), (0,1)
      // Schwerpunkt: (1/3, 1/3)

        if (ergebnis.dreieckIndex == 0u)
        {
            pivotLokal = float2(1.0 / 3.0, 1.0 / 3.0);
        }
        else
        {
        // Dreieck 1: (1,1), (1,0), (0,1)
        // Schwerpunkt: (2/3, 2/3)

            pivotLokal = float2(2.0 / 3.0, 2.0 / 3.0);
        }
    }
    else
    {
        // Diagonale \
        //
        // Dreieck 0: (0,0), (1,0), (1,1)
        // Schwerpunkt: (2/3, 1/3)

        if (ergebnis.dreieckIndex == 0u)
        {
            pivotLokal = float2(2.0 / 3.0, 1.0 / 3.0);
        }
        else
        {
        // Dreieck 1: (0,0), (0,1), (1,1)
        // Schwerpunkt: (1/3, 2/3)

            pivotLokal = float2(1.0 / 3.0, 2.0 / 3.0);
        }
    }

    ergebnis.pivotUV = float2(((float) ergebnis.zelle.x + pivotLokal.x) * zellBreiteUV, 
                             ((float) ergebnis.zelle.y + pivotLokal.y) * zellHoeheUV);
    
    return ergebnis;
}

float ErmittleMagieFrontEinfluss(float maskValue, float aktuellerProgress)
{
    float frontDistance;

    float breiteVorFront;
    float breiteHinterFront;

    float einfluss;

    float effektProgress;


    // Magische Verzerrung darf deutlich vorauseilen
    // und bleibt hinter der Front länger bestehen.

    breiteVorFront = verzerrungsBreite;

    breiteHinterFront = verzerrungsBreite * 1.22;


    // --------------------------------------------------------
    // Während des Nachlaufs wandert die virtuelle
    // Magiefront über progress = 1 hinaus.
    // --------------------------------------------------------

    effektProgress = aktuellerProgress + magieNachlaufProgress * breiteHinterFront;


    frontDistance = maskValue - effektProgress;


    if (frontDistance >= 0.0)
    {
        einfluss = 1.0 - smoothstep(0.0, breiteVorFront, frontDistance);
    }
    else
    {
        einfluss = 1.0 - smoothstep(0.0, breiteHinterFront, -frontDistance);
    }


    return saturate(einfluss);
}

float2 ErmittleMagieDistortionUV(float2 uv, MagieDreieck dreieck, float frontEinfluss)
{
    float aspectRatio;

    float zellHoeheUV;
    float zellBreiteUV;

    float rotation;
    float scaleX;
    float scaleY;
    float shearX;
    float shearY;

    float offsetX;
    float offsetY;

    float cosRotation;
    float sinRotation;

    float2 lokalePosition;
    float2 transformiertePosition;
    float2 magieUV;

    uint dreieckId;


    dreieckId = dreieck.dreieckId;


    // --------------------------------------------------------
    // Zellgröße erneut bestimmen.
    //
    // Transformation erfolgt anschließend im lokalen,
    // quadratischen Zellraum.
    //
    // Dadurch ist eine Rotation auch auf 16:9 tatsächlich
    // eine Rotation und keine UV-verzerrte Eierbewegung.
    // --------------------------------------------------------

    aspectRatio = renderBreite / max(renderHoehe, 1.0);

    zellHoeheUV = 1.0 / max(magieRasterHoehe, 1.0);

    zellBreiteUV = zellHoeheUV / aspectRatio;


    // --------------------------------------------------------
    // Deterministische Transformationsparameter.
    //
    // Jede Eigenschaft bekommt einen eigenen Hash-Salt.
    // --------------------------------------------------------

    // Rotation 
    rotation = HashSigned(dreieckId ^ 0xA511E9B3u) * 0.13962634 * verzerrungsEffektStaerke;
    
    // Skalierung

    scaleX = 1.0 + HashSigned(dreieckId ^ 0x63D83595u) * 0.10 * verzerrungsEffektStaerke;
    scaleY = 1.0 + HashSigned(dreieckId ^ 0xB5297A4Du) * 0.10 * verzerrungsEffektStaerke;
    
    // Scherung 

    shearX = HashSigned(dreieckId ^ 0x68E31DA4u) * 0.07 * verzerrungsEffektStaerke;
    shearY = HashSigned(dreieckId ^ 0x1B56C4E9u) * 0.07 * verzerrungsEffektStaerke;


    // Translation relativ zur Zellgröße.
    //
    // Dadurch bleibt der Effekt unabhängig von
    // Bildschirmauflösung und Seitenverhältnis.
    
    // Translation 

    offsetX = HashSigned(dreieckId ^ 0xC2B2AE35u) * 0.18 * verzerrungsEffektStaerke;
    offsetY = HashSigned(dreieckId ^ 0x27D4EB2Fu) * 0.18 * verzerrungsEffektStaerke;
    
    cosRotation = cos(rotation);
    sinRotation = sin(rotation);


    // --------------------------------------------------------
    // UV relativ zum echten Dreiecksschwerpunkt in einen
    // quadratischen lokalen Raum transformieren.
    // --------------------------------------------------------

    lokalePosition = float2((uv.x - dreieck.pivotUV.x) / zellBreiteUV, (uv.y - dreieck.pivotUV.y) / zellHoeheUV);


    // --------------------------------------------------------
    // Skalierung.
    // --------------------------------------------------------

    transformiertePosition = float2(lokalePosition.x * scaleX, lokalePosition.y * scaleY);


    // --------------------------------------------------------
    // Scherung.
    // --------------------------------------------------------

    transformiertePosition =  float2(transformiertePosition.x + transformiertePosition.y * shearX,
                                     transformiertePosition.y + transformiertePosition.x * shearY);


    // --------------------------------------------------------
    // Rotation.
    // --------------------------------------------------------

    transformiertePosition = float2(transformiertePosition.x * cosRotation - transformiertePosition.y * sinRotation,
                                    transformiertePosition.x * sinRotation + transformiertePosition.y * cosRotation);


    // --------------------------------------------------------
    // Translation.
    // --------------------------------------------------------

    transformiertePosition += float2(offsetX, offsetY);


    // Zurück in UV-Koordinaten.

    magieUV = dreieck.pivotUV + float2(transformiertePosition.x * zellBreiteUV, 
                                       transformiertePosition.y * zellHoeheUV);


    // --------------------------------------------------------
    // Magie über FrontEinfluss sanft ein- und ausblenden.
    // --------------------------------------------------------

    return lerp(uv, magieUV, saturate(frontEinfluss * distortionStaerke));
}

float ErmittleMagieDreieckKantenDistanz(MagieDreieck dreieck)
{
    float distanzA;
    float distanzB;
    float distanzDiagonal;

    float2 lokal;

    lokal = dreieck.lokal;


    if (dreieck.diagonaleSteigt)
    {
        // ----------------------------------------------------
        // Diagonale /
        //
        // Dreieck 0:
        // x >= 0
        // y >= 0
        // x + y <= 1
        //
        // Dreieck 1:
        // x <= 1
        // y <= 1
        // x + y >= 1
        // ----------------------------------------------------

        if (dreieck.dreieckIndex == 0u)
        {
            distanzA = lokal.x;
            distanzB = lokal.y;
        }
        else
        {
            distanzA = 1.0 - lokal.x;
            distanzB = 1.0 - lokal.y;
        }

        distanzDiagonal = abs(lokal.x + lokal.y - 1.0) * 0.70710678;
    }
    else
    {
        // ----------------------------------------------------
        // Diagonale \
        //
        // Dreieck 0:
        // y <= x
        //
        // Dreieck 1:
        // y >= x
        // ----------------------------------------------------

        if (dreieck.dreieckIndex == 0u)
        {
            distanzA = 1.0 - lokal.x;
            distanzB = lokal.y;
        }
        else
        {
            distanzA = lokal.x;
            distanzB = 1.0 - lokal.y;
        }

        distanzDiagonal = abs(lokal.y - lokal.x) * 0.70710678;
    }


    return min(min(distanzA, distanzB), distanzDiagonal);
}

float ErmittleMagieKantenEinfluss(MagieDreieck dreieck, float frontEinfluss)
{
    float kantenDistanz;
    float lokalePixelGroesse;
    float kantenBreite;
    float kantenEinfluss;
    
    kantenDistanz = ErmittleMagieDreieckKantenDistanz(dreieck);


    // --------------------------------------------------------
    // Ein Pixel als Anteil einer Zellhöhe.
    //
    // Eine Zelle besitzt ungefähr:
    //
    // renderHoehe / magieRasterHoehe
    //
    // Pixel.
    // --------------------------------------------------------

    lokalePixelGroesse = magieRasterHoehe / max(renderHoehe, 1.0);


    // Etwa 2,5 Pixel breite optische Glaskante.

    kantenBreite = lokalePixelGroesse * 2.5;    
    kantenEinfluss = 1.0 - smoothstep(0.0, kantenBreite, kantenDistanz);

    return saturate(kantenEinfluss * frontEinfluss);
}

float3 ErmittleMagieGlasKantenFarbe(float3 bildFarbe)
{
    float luminanz;
    float3 dunklesGlas;
    float3 hellesGlas;

    luminanz = dot(bildFarbe, float3(0.2126, 0.7152, 0.0722));
    
    // Variante klassisches Glas
    // dunklesGlas = float3(0.145, 0.195, 0.215);
    // hellesGlas =  float3(0.565, 0.720, 0.705);

    // Variante wilde Magie
    dunklesGlas = float3(0.20, 0.06, 0.34);
    hellesGlas = float3(0.48, 0.62, 0.92);
    
    // Variante arkanes Glas
    // dunklesGlas = float3(0.16, 0.055, 0.24);
    // hellesGlas = float3(0.38, 0.68, 0.78);

    return lerp(dunklesGlas, hellesGlas, saturate(luminanz));
}

// ------------------------------------------------------------
// Pixel Shader
//
// ------------------------------------------------------------

float4 PSMain(VertexOutput input) : SV_TARGET
{
    MagieDreieck magieDreieck;

    float2 distortedUV;

    float4 oldColor;
    float4 newColor;

    float maskValue;
    float reveal;

    float magieFrontEinfluss;
    
    float4 basisColor;
    float4 originalOldColor;
    float4 originalNewColor;
    float4 originalColor;

    float kantenEinfluss;
    float fresnelSpitze;
    float3 glasKantenFarbe;
    float revealProgress;
    
    float saeureDunstEinfluss;
    float saeureKorrosionsEinfluss;

    float saeureDunstDichte;
    float saeureDunstAlpha;
    float3 saeureDunstFarbe;
    
    float hitzeFrontEinfluss;

    bool istBlitz;

    revealProgress =    progress +    revealNachlaufProgress *    0.012;

    // --------------------------------------------------------
    // Effektgeometrie wird grundsätzlich mit den ORIGINALEN
    // UV-Koordinaten abgefragt.
    // --------------------------------------------------------

    maskValue = maskTexture.Sample(sourceSampler, input.texCoord).r;


    // --------------------------------------------------------
    // Standard:
    // Realität bleibt zunächst unangetastet.
    // --------------------------------------------------------

    distortedUV = input.texCoord;


    // --------------------------------------------------------
    // Magische Realitätsfragmentierung.
    // --------------------------------------------------------

    if (verzerrungAktiv != 0u && distortionModus == 4u)
    {
        magieDreieck = ErmittleMagieDreieck(input.texCoord);

        magieFrontEinfluss = ErmittleMagieFrontEinfluss(maskValue, progress);

        distortedUV = ErmittleMagieDistortionUV(input.texCoord, magieDreieck, magieFrontEinfluss);
    }

    // --------------------------------------------------------
    // Säureschwaden erschweren die Sicht und brennen in den
    // Augen.
    // --------------------------------------------------------
    
    if (verzerrungAktiv != 0u && distortionModus == 3u)
    {
        saeureDunstEinfluss = ErmittleSaeureDunstEinfluss(maskValue, progress);
        saeureKorrosionsEinfluss = ErmittleSaeureKorrosionsEinfluss(maskValue, progress);
        
        distortedUV = ErmittleSaeureDistortionUV(input.texCoord, saeureKorrosionsEinfluss);
    }
    
    // --------------------------------------------------------
    // Thermische Luftverzerrung.
    //
    // Feuer und Blitze benutzen denselben mathematischen
    // Effekt, unterscheiden sich jedoch in ihrer
    // Parametrisierung.
    // --------------------------------------------------------

    if (verzerrungAktiv != 0u && (distortionModus == 1u || distortionModus == 2u))
    {
        istBlitz = distortionModus == 2u;
        
        hitzeFrontEinfluss = ErmittleHitzeFrontEinfluss(maskValue, progress, istBlitz);

        distortedUV = ErmittleHitzeDistortionUV(input.texCoord, hitzeFrontEinfluss,  istBlitz);
    }
    

    // --------------------------------------------------------
    // Beide Realitäten werden durch dasselbe lokale
    // Verzerrungsfeld betrachtet.
    //
    // Dadurch bleibt der Bildwechsel geometrisch konsistent.
    // --------------------------------------------------------

    oldColor = oldImageTexture.Sample(sourceSampler, distortedUV);
    newColor = newImageTexture.Sample(sourceSampler, distortedUV);


    // --------------------------------------------------------
    // Reveal selbst bleibt vollständig auf den ORIGINALEN
    // Koordinaten.
    //
    // Die Magie zerbricht die Realität,
    // nicht die Brand-/Effektmaske.
    // --------------------------------------------------------

    if (gradientAktiv != 0u)
    {
        reveal = step(maskValue, revealProgress);
    }
    else
    // Falls Gradient abgeschaltet ist, bekommt Reveal eine kleine kleine weiche
    // Kante, eher ein Anti-Aliasing als ein Gradient
    {
        reveal = smoothstep(maskValue - 0.006, maskValue + 0.006, revealProgress);
    }

    basisColor = lerp(oldColor, newColor, reveal);

   // ------------------------------------------------------------
// Nur im Säure-Modus wabert eine gräßlich grüne Dunstwolke
// über dem Reveal-Bereich.
// ------------------------------------------------------------

    if (verzerrungAktiv != 0u && distortionModus == 3u)
    {
        saeureDunstDichte = ErmittleSaeureDunstDichte(input.texCoord, saeureDunstEinfluss);
        saeureDunstFarbe = ErmittleSaeureDunstFarbe(saeureDunstDichte);
        saeureDunstAlpha = saeureDunstDichte * 0.55;

        basisColor.rgb = lerp(basisColor.rgb, saeureDunstFarbe, saeureDunstAlpha);
    }
    
    // ------------------------------------------------------------
    // Nur im Magie-Modus besitzen die Realitätsfragmente
    // sichtbare optische Grenzflächen.
    // ------------------------------------------------------------

    if (verzerrungAktiv != 0u && distortionModus == 4u)
    {
        kantenEinfluss = ErmittleMagieKantenEinfluss(magieDreieck, magieFrontEinfluss);


    // --------------------------------------------------------
    // Unverzerrte Realität zusätzlich lesen.
    //
    // Diese wird an der Fragmentkante leicht eingemischt.
    // Dadurch wirkt die Kante transparent/brechend und
    // nicht wie eine darübergemalte Linie.
    // --------------------------------------------------------

        originalOldColor = oldImageTexture.Sample(sourceSampler, input.texCoord);
        originalNewColor = newImageTexture.Sample(sourceSampler, input.texCoord);

        originalColor = lerp(originalOldColor, originalNewColor, reveal);


    // --------------------------------------------------------
    // Bildabhängige kühle Glasfarbe.
    // --------------------------------------------------------

        glasKantenFarbe = ErmittleMagieGlasKantenFarbe(basisColor.rgb);


    // --------------------------------------------------------
    // Breiterer dunkler/transparenter Glassaum.
    // --------------------------------------------------------

        basisColor.rgb = lerp(basisColor.rgb, originalColor.rgb, kantenEinfluss * 0.22);
        basisColor.rgb = lerp(basisColor.rgb, glasKantenFarbe, kantenEinfluss * 0.34);


    // --------------------------------------------------------
    // Sehr schmale helle Fresnelkante.
    //
    // Durch Potenzieren bleibt nur der innerste Bereich
    // der bereits schmalen Kantenmaske übrig.
    // --------------------------------------------------------

        fresnelSpitze = pow(kantenEinfluss, 4.0);

        basisColor.rgb += float3(0.52, 0.68, 0.67) * fresnelSpitze * 0.20;
        basisColor.rgb = saturate(basisColor.rgb);
    }
    
    return basisColor;
}