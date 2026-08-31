// ============================================================================
// RegionDistanceShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Aus dem Kuwahara-Ergebnis eine geometrische Distanzkarte erzeugen,
//      die später als Grundlage für die initiale WaterPressureMap dient.
//
// Neue Pipeline:
//
//      Kuwahara
//          |
//      relevante Farbgrenzen erkennen
//          |
//      Boundary Seeds
//          |
//      Jump Flood / lokale Relaxation
//          |
//      echte Pixeldistanz zur nächsten Grenze
//
// ---------------------------------------------------------------------------
// WICHTIG:
//
// Die frühere iterative Regionssegmentierung über Region-Labels wird für
// die eigentliche Grenzerkennung NICHT mehr verwendet.
//
// Stattdessen analysiert MODE_BOUNDARY das Kuwahara-Bild unmittelbar.
//
// Die Grenzerkennung arbeitet mehrskalig:
//
//      kleine Distanz  -> lokale Struktur
//      mittlere Distanz -> eigentliche Kuwahara-Fläche
//      große Distanz   -> stabile größere Farbgrenze
//
// Dadurch soll nicht jede kleine interne Kuwahara-Abstufung automatisch
// eine neue Region erzeugen.
//
// ---------------------------------------------------------------------------
// Diagnosefarben in MODE_DISPLAY:
//
//      GRAUSTUFEN
//          gültige DistanceMap
//
//      GELB
//          ungültiger/unbekannter Shader-Modus
//
// ============================================================================


// ============================================================================
// Modi
// ============================================================================

static const uint MODE_BOUNDARY = 0;
static const uint MODE_PROPAGATE = 1;
static const uint MODE_FINALIZE = 2;
static const uint MODE_DISPLAY = 3;

// ============================================================================
// Allgemeine Konstanten
// ============================================================================

static const float INVALID_SEED = -1.0F;


// ============================================================================
// Boundary-Erkennung
// ============================================================================
//
// Wir prüfen drei räumliche Skalen.
//
// Für natives 4K sind 4 / 8 / 16 Pixel zunächst ein sinnvoller
// Ausgangspunkt.
//
// Die Gewichte bevorzugen die mittlere und große Skala.
//
// Das ist ABSICHTLICH keine physikalische Größe.
// Wir wollen relevante gemalte Farbflächen aus Kuwahara erkennen.
// ============================================================================

static const int BOUNDARY_RADIUS_SMALL = 4;
static const int BOUNDARY_RADIUS_MEDIUM = 8;
static const int BOUNDARY_RADIUS_LARGE = 16;

static const float BOUNDARY_WEIGHT_SMALL = 0.20F;
static const float BOUNDARY_WEIGHT_MEDIUM = 0.35F;
static const float BOUNDARY_WEIGHT_LARGE = 0.45F;


// ============================================================================
// Constant Buffer
// ============================================================================
//
// Exakt 16 Byte.
//
// mode
//      Betriebsmodus.
//
// jumpStep
//      Sprungweite für Jump Flood / Relaxation.
//
// regionColorThreshold
//      Jetzt:
//
//      Schwellenwert für die relevante mehrskalige Farbgradientenstärke.
//
//      Der Name bleibt vorerst erhalten, damit die VB-Seite nicht
//      gleichzeitig geändert werden muss.
//
// displayDistanceScale
//      Pixeldistanz, die im Kontrollmonitor Weiß ergibt.
// ============================================================================

cbuffer RegionDistanceConstants : register(b0)
{
    uint mode;
    uint jumpStep;

    float regionColorThreshold;
    float displayDistanceScale;
};


// ============================================================================
// Eingaben
// ============================================================================

// Kuwahara-Ergebnis.
Texture2D<float4> kuwaharaTexture : register(t0);

// Aktueller Distance-Seed-Zustand.
Texture2D<float2> sourceSeedTexture : register(t1);

// Fertige rohe Distanzkarte.
Texture2D<float> regionDistanceTexture : register(t2);

// ============================================================================
// Atomic Changed Counter
// ============================================================================
//
// Wird von MODE_PROPAGATE verwendet, um während der lokalen
// Relaxation festzustellen, ob noch Seed-Verbesserungen stattfinden.
// ============================================================================

RWStructuredBuffer<uint> changedCounter : register(u1);


// ============================================================================
// Vertex Shader
// ============================================================================

struct VSOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


VSOutput VSMain(uint vertexId : SV_VertexID)
{
    VSOutput output;

    float2 position;
    float2 texCoord;


    if (vertexId == 0)
    {
        position = float2(-1.0F, -1.0F);
        texCoord = float2(0.0F, 1.0F);
    }
    else if (vertexId == 1)
    {
        position = float2(-1.0F, 3.0F);
        texCoord = float2(0.0F, -1.0F);
    }
    else
    {
        position = float2(3.0F, -1.0F);
        texCoord = float2(2.0F, 1.0F);
    }


    output.position = float4(position, 0.0F, 1.0F);

    output.texCoord = texCoord;


    return output;
}


// ============================================================================
// Allgemeine Hilfsfunktionen
// ============================================================================

bool IsInsideTexture(int2 pixelPosition, uint2 textureSize)
{
    if (pixelPosition.x < 0)
    {
        return false;
    }

    if (pixelPosition.y < 0)
    {
        return false;
    }

    if (pixelPosition.x >= int(textureSize.x))
    {
        return false;
    }

    if (pixelPosition.y >= int(textureSize.y))
    {
        return false;
    }


    return true;
}


// ============================================================================
// Kuwahara-Hilfsfunktionen
// ============================================================================

int2 ClampPixelPosition(int2 pixelPosition, uint2 textureSize)
{
    int2 maximumPosition;
    
    maximumPosition = int2(int(textureSize.x) - 1, int(textureSize.y) - 1);

    return clamp(pixelPosition, int2(0, 0), maximumPosition);
}


float3 ReadKuwaharaColor(int2 pixelPosition, int2 textureSize)
{
    int2 clampedPosition;
    
    clampedPosition = ClampPixelPosition(pixelPosition, textureSize);


    return kuwaharaTexture.Load(int3(clampedPosition, 0)).rgb;
}


float CalculateColorDistance(float3 colorA, float3 colorB)
{
    float3 difference;
    
    difference = colorA - colorB;


    return sqrt(dot(difference, difference));
}


// ============================================================================
// Boundary-Analyse einer einzelnen räumlichen Skala
// ============================================================================
//
// Neben der Gradientenstärke bestimmen wir eine grobe Gradientenrichtung.
//
// Richtungen:
//
//      0 = horizontaler Gradient
//          -> Grenze verläuft ungefähr vertikal
//
//      1 = vertikaler Gradient
//          -> Grenze verläuft ungefähr horizontal
//
//      2 = Diagonale links-oben -> rechts-unten
//
//      3 = Diagonale rechts-oben -> links-unten
//
// Für die Non-Maximum Suppression genügt diese Quantisierung vollkommen.
// Wir brauchen keine exakte Winkelbestimmung.
// ============================================================================

struct BoundaryGradient
{
    float strength;
    uint direction;
};


BoundaryGradient CalculateBoundaryGradientAtRadius(int2 pixelPosition, uint2 textureSize, int radius)
{
    BoundaryGradient result;

    float3 colorLeft;
    float3 colorRight;

    float3 colorUp;
    float3 colorDown;

    float3 colorUpperLeft;
    float3 colorUpperRight;

    float3 colorLowerLeft;
    float3 colorLowerRight;

    float horizontalGradient;
    float verticalGradient;

    float diagonalGradientA;
    float diagonalGradientB;


    colorLeft =       ReadKuwaharaColor(pixelPosition + int2(-radius, 0), textureSize);
    colorRight =      ReadKuwaharaColor(pixelPosition + int2(radius, 0), textureSize);
    colorUp =         ReadKuwaharaColor(pixelPosition + int2(0, -radius), textureSize);
    colorDown =       ReadKuwaharaColor(pixelPosition + int2(0, radius), textureSize);
    colorUpperLeft =  ReadKuwaharaColor(pixelPosition + int2(-radius, -radius), textureSize);
    colorUpperRight = ReadKuwaharaColor(pixelPosition + int2(radius, -radius), textureSize);
    colorLowerLeft =  ReadKuwaharaColor(pixelPosition + int2(-radius, radius), textureSize);
    colorLowerRight = ReadKuwaharaColor(pixelPosition + int2(radius, radius), textureSize);
    
    horizontalGradient = CalculateColorDistance(colorLeft, colorRight);    
    verticalGradient =   CalculateColorDistance(colorUp, colorDown);
    diagonalGradientA =  CalculateColorDistance(colorUpperLeft, colorLowerRight);
    diagonalGradientB =  CalculateColorDistance(colorUpperRight, colorLowerLeft);


    // ------------------------------------------------------------------------
    // Stärkste Richtung bestimmen.
    // ------------------------------------------------------------------------

    result.strength = horizontalGradient;
    result.direction = 0;


    if (verticalGradient > result.strength)
    {
        result.strength = verticalGradient;
        result.direction = 1;
    }


    if (diagonalGradientA > result.strength)
    {
        result.strength = diagonalGradientA;
        result.direction = 2;
    }


    if (diagonalGradientB > result.strength)
    {
        result.strength = diagonalGradientB;
        result.direction = 3;
    }


    return result;
}


// ============================================================================
// Mehrskalige Boundary-Analyse
// ============================================================================
//
// Die Stärke wird wie bisher aus drei räumlichen Skalen kombiniert.
//
// Für die NMS-Richtung verwenden wir diejenige einzelne Skala, die den
// stärksten gewichteten Beitrag liefert.
//
// Dadurch richtet sich die Unterdrückung vorzugsweise nach der optisch
// dominantesten Struktur.
// ============================================================================

BoundaryGradient CalculateMultiscaleBoundaryGradient(int2 pixelPosition, uint2 textureSize)
{
    BoundaryGradient gradientSmall;
    BoundaryGradient gradientMedium;
    BoundaryGradient gradientLarge;

    BoundaryGradient result;

    float weightedSmall;
    float weightedMedium;
    float weightedLarge;


    gradientSmall =  CalculateBoundaryGradientAtRadius(pixelPosition, textureSize, BOUNDARY_RADIUS_SMALL);
    gradientMedium = CalculateBoundaryGradientAtRadius(pixelPosition, textureSize, BOUNDARY_RADIUS_MEDIUM);
    gradientLarge =  CalculateBoundaryGradientAtRadius(pixelPosition, textureSize, BOUNDARY_RADIUS_LARGE);
    
    weightedSmall =  gradientSmall.strength * BOUNDARY_WEIGHT_SMALL;
    weightedMedium = gradientMedium.strength * BOUNDARY_WEIGHT_MEDIUM;
    weightedLarge =  gradientLarge.strength * BOUNDARY_WEIGHT_LARGE;


    // ------------------------------------------------------------------------
    // Gesamtstärke bleibt die gewichtete Summe.
    // ------------------------------------------------------------------------

    result.strength = weightedSmall + weightedMedium + weightedLarge;


    // ------------------------------------------------------------------------
    // Richtung vom stärksten gewichteten Einzelbeitrag übernehmen.
    // ------------------------------------------------------------------------

    result.direction = gradientSmall.direction;


    if (weightedMedium > weightedSmall)
    {
        result.direction = gradientMedium.direction;
    }
    
    if (weightedLarge > max(weightedSmall, weightedMedium))
    {
        result.direction = gradientLarge.direction;
    }


    return result;
}


// ============================================================================
// Nur Boundary-Stärke liefern
//
// Hilfsfunktion für die Non-Maximum Suppression.
// ============================================================================

float CalculateMultiscaleBoundaryStrength(int2 pixelPosition, uint2 textureSize)
{
    BoundaryGradient gradient;
    
    gradient = CalculateMultiscaleBoundaryGradient(pixelPosition, textureSize);
    
    return gradient.strength;
}


// ============================================================================
// Non-Maximum Suppression
// ============================================================================
//
// Problem der bisherigen Fassung:
//
// Eine starke Farbgrenze wird von vielen Pixeln gleichzeitig "gesehen".
//
// Beispiel:
//
//      0.08  0.16  0.31  0.48  0.55  0.44  0.26  0.11
//
// Bei Threshold 0.1 wurden daraus mehrere Boundary-Seeds:
//
//            X     X     X     X     X     X
//
// NMS behält nur das lokale Maximum entlang der Gradientenrichtung:
//
//                        X
//
// Dadurch wird aus einem breiten Cyan-Band eine dünne Boundary-Linie.
// ============================================================================

bool IsLocalBoundaryMaximum(int2 pixelPosition, uint2 textureSize,  BoundaryGradient centerGradient)
{
    int2 directionOffset;

    int2 neighborPositionA;
    int2 neighborPositionB;

    float neighborStrengthA;
    float neighborStrengthB;


    // ------------------------------------------------------------------------
    // Quantisierte Gradientenrichtung in Pixeloffset übersetzen.
    // ------------------------------------------------------------------------

    if (centerGradient.direction == 0)
    {
        directionOffset = int2(1, 0);
    }
    else if (centerGradient.direction == 1)
    {
        directionOffset = int2(0, 1);
    }
    else if (centerGradient.direction == 2)
    {
        directionOffset = int2(1, 1);
    }
    else
    {
        directionOffset = int2(1, -1);
    }


    neighborPositionA = pixelPosition - directionOffset;
    neighborPositionB = pixelPosition + directionOffset;


    // ------------------------------------------------------------------------
    // Am Bildrand fehlt ggf. ein Nachbar.
    //
    // Dort behandeln wir die fehlende Seite als Stärke 0.
    // ------------------------------------------------------------------------

    neighborStrengthA = 0.0F;
    neighborStrengthB = 0.0F;


    if (IsInsideTexture(neighborPositionA, textureSize))
    {
        neighborStrengthA = CalculateMultiscaleBoundaryStrength(neighborPositionA, textureSize);
    }


    if (IsInsideTexture(neighborPositionB, textureSize))
    {
        neighborStrengthB = CalculateMultiscaleBoundaryStrength(neighborPositionB, textureSize);
    }


    // ------------------------------------------------------------------------
    // Ein echtes lokales Maximum muss mindestens so stark sein wie beide
    // Nachbarn entlang der Gradientenrichtung.
    //
    // Ein kleiner Epsilon-Versatz verhindert unnötige Instabilität bei
    // nahezu identischen Float-Werten.
    // ------------------------------------------------------------------------

    return
        centerGradient.strength + 0.000001F >= neighborStrengthA &&
        centerGradient.strength + 0.000001F >= neighborStrengthB;
}


// ============================================================================
// Finale Kuwahara-Boundary-Entscheidung
// ============================================================================
//
// Reihenfolge:
//
//      1. Multiscale Gradient berechnen
//      2. Threshold prüfen
//      3. Non-Maximum Suppression
//
// WICHTIG:
//
// Der Threshold entscheidet weiterhin:
//
//      "Ist dieser Farbwechsel überhaupt relevant?"
//
// Die NMS entscheidet anschließend:
//
//      "Welcher Pixel innerhalb dieses Übergangs ist die eigentliche Grenze?"
//
// Damit müssen wir den Threshold NICHT künstlich erhöhen, nur um dicke
// Boundary-Bänder loszuwerden.
// ============================================================================

bool IsKuwaharaBoundary(int2 pixelPosition, uint2 textureSize)
{
    BoundaryGradient centerGradient;
    
    centerGradient = CalculateMultiscaleBoundaryGradient(pixelPosition, textureSize);


    // ------------------------------------------------------------------------
    // Erst Relevanz prüfen.
    // ------------------------------------------------------------------------

    if (centerGradient.strength < regionColorThreshold)
    {
        return false;
    }


    // ------------------------------------------------------------------------
    // Danach dickes Band auf lokales Maximum ausdünnen.
    // ------------------------------------------------------------------------

    if (!IsLocalBoundaryMaximum(pixelPosition, textureSize, centerGradient))
    {
        return false;
    }


    return true;
}


// ============================================================================
// Distance-Seed-Hilfsfunktionen
// ============================================================================

bool IsValidSeed(float2 seed)
{
    return seed.x >= 0.0F && seed.y >= 0.0F;
}


float CalculateSeedDistanceSquared(float2 pixelPosition, float2 seed)
{
    float2 difference;


    difference = pixelPosition - seed;


    return dot(difference, difference);
}


void EvaluateSeedCandidate(int2 candidatePosition, uint2 textureSize, float2 pixelPosition, inout float2 bestSeed,
                           inout float bestDistanceSquared)
{
    float2 candidateSeed;
    float candidateDistanceSquared;


    if (!IsInsideTexture(candidatePosition, textureSize))
    {
        return;
    }


    candidateSeed = sourceSeedTexture.Load(int3(candidatePosition, 0));

    if (!IsValidSeed(candidateSeed))
    {
        return;
    }


    candidateDistanceSquared = CalculateSeedDistanceSquared(pixelPosition, candidateSeed);


    if (candidateDistanceSquared < bestDistanceSquared)
    {
        bestDistanceSquared = candidateDistanceSquared;

        bestSeed = candidateSeed;
    }
}


// ============================================================================
// Pixel Shader
// ============================================================================

float4 PSMain(VSOutput input) : SV_TARGET
{
    uint textureWidth;
    uint textureHeight;

    uint2 textureSize;

    int2 pixelPosition;
    float2 pixelPositionFloat;

    float2 currentSeed;
    float2 bestSeed;

    float currentDistanceSquared;
    float bestDistanceSquared;

    float finalDistance;
    float displayValue;

    uint previousCounterValue;

    int step;


    // ------------------------------------------------------------------------
    // Pixelposition bestimmen.
    // ------------------------------------------------------------------------

    pixelPosition = int2(input.position.xy);
    
    pixelPositionFloat = float2(pixelPosition);


    // ------------------------------------------------------------------------
    // Texturdimensionen bestimmen.
    //
    // Jeder Modus fragt genau die Ressource ab, die in diesem Modus
    // tatsächlich gebunden ist.
    // ------------------------------------------------------------------------

    if (mode == MODE_PROPAGATE || mode == MODE_FINALIZE)
    {
        sourceSeedTexture.GetDimensions(textureWidth, textureHeight);
    }
    else if (mode == MODE_DISPLAY)
    {
        regionDistanceTexture.GetDimensions(textureWidth, textureHeight);
    }
    else
    {
        kuwaharaTexture.GetDimensions(textureWidth, textureHeight);
    }


    textureSize = uint2(textureWidth, textureHeight);

    
    // ========================================================================
    // MODE 0
    // Relevante Kuwahara-Grenzen direkt in Boundary-Seeds umwandeln
    // ========================================================================

    if (mode == MODE_BOUNDARY)
    {
        if (IsKuwaharaBoundary(pixelPosition, textureSize))
        {
            return float4(pixelPositionFloat, 0.0F, 1.0F);
        }


        return float4(INVALID_SEED, INVALID_SEED, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE 1
    // Jump Flood / lokale Relaxation
    // ========================================================================

    if (mode == MODE_PROPAGATE)
    {
        currentSeed = sourceSeedTexture.Load(int3(pixelPosition, 0));
        
        bestSeed = currentSeed;


        if (IsValidSeed(currentSeed))
        {
            currentDistanceSquared = CalculateSeedDistanceSquared(pixelPositionFloat, currentSeed);
            bestDistanceSquared = currentDistanceSquared;
        }
        else
        {
            currentDistanceSquared = 3.402823466E+38F;
            bestDistanceSquared =    3.402823466E+38F;
        }


        step = max(int(jumpStep), 1);


        EvaluateSeedCandidate(
            pixelPosition + int2(-step, -step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(0, -step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(step, -step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);
        
        EvaluateSeedCandidate(
            pixelPosition + int2(-step, 0),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(step, 0),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);
        
        EvaluateSeedCandidate(
            pixelPosition + int2(-step, step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(0, step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);

        EvaluateSeedCandidate(
            pixelPosition + int2(step, step),
            textureSize,
            pixelPositionFloat,
            bestSeed,
            bestDistanceSquared);


        if (bestDistanceSquared + 0.0001F < currentDistanceSquared)
        {
            InterlockedAdd(changedCounter[0], 1, previousCounterValue);
        }


        return float4(bestSeed, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE 2
    // Seed-Koordinate -> echte Pixeldistanz
    //
    // -1 bedeutet:
    // Kein gültiger Boundary-Seed hat dieses Pixel erreicht.
    // ========================================================================

    if (mode == MODE_FINALIZE)
    {
        currentSeed = sourceSeedTexture.Load(int3(pixelPosition, 0));
        
        if (!IsValidSeed(currentSeed))
        {
            return float4(-1.0F, 0.0F, 0.0F, 1.0F);
        }


        finalDistance = length(pixelPositionFloat - currentSeed);


        return float4(finalDistance, 0.0F, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE 3
    // Diagnose-/Kontrollansicht
    // ========================================================================

    if (mode == MODE_DISPLAY)
    {
        uint displayWidth;
        uint displayHeight;

        int2 displayPixelPosition;


        regionDistanceTexture.GetDimensions(displayWidth, displayHeight);
        
        displayPixelPosition = int2(saturate(input.texCoord) * float2(displayWidth - 1, displayHeight - 1));
        
        finalDistance = regionDistanceTexture.Load(int3(displayPixelPosition, 0));


        
        // --------------------------------------------------------------------
        // Gültige Distanz:
        // Schwarz nahe der Grenze -> heller zum Flächeninneren.
        // --------------------------------------------------------------------

        displayValue = saturate(finalDistance / max(displayDistanceScale, 1.0F));
        
        return float4(displayValue, displayValue, displayValue, 1.0F);
    }


    // ========================================================================
    // Sicherheitsnetz
    //
    // GELB darf im regulären Betrieb niemals erscheinen.
    // ========================================================================

    return float4(1.0F, 1.0F, 0.0F, 1.0F);
}