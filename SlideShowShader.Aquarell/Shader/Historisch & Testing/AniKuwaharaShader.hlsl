// ============================================================================
// AniKuwaharaShader.hlsl
//
// Anisotroper Kuwahara-Filter für die Aquarell-Vorverarbeitung.
//
// Ziel:
//
//     - malerische Flächenbildung wie beim klassischen Kuwahara
//     - lokale Kantenrichtungen stärker berücksichtigen
//     - längliche Bildstrukturen entlang ihrer Orientierung glätten
//     - Clustering-/Blockartefakte des klassischen Kuwahara reduzieren
//
// Algorithmische Grundlage:
//
//     Kyprianidis, J. E.; Kang, H.; Döllner, J.
//     "Image and Video Abstraction by Anisotropic Kuwahara Filtering"
//     Computer Graphics Forum, 2009.
//
//     Kyprianidis, J. E.; Semmo, A.; Kang, H.; Döllner, J.
//     "Anisotropic Kuwahara Filtering with Polynomial Weighting Functions"
//     NPAR / TPCG, 2010.
//
// Grundidee:
//
//     1. Lokalen Bildgradienten bestimmen.
//     2. Daraus einen Structure Tensor bilden.
//     3. Eigenwerte und Hauptrichtung des Tensors bestimmen.
//     4. Filterellipse entlang der lokalen Struktur ausrichten.
//     5. Ellipse in acht gewichtete Sektoren zerlegen.
//     6. Pro Sektor Mittelwert und Varianz berechnen.
//     7. Homogene Sektoren stärker gewichten.
//
// Diese Implementierung ist bewusst eine kompakte Testvariante für
// Aquarell V0.x und keine vollständige Reproduktion der Referenzpipeline.
// ============================================================================


Texture2D sourceTexture : register(t0);

SamplerState sourceSampler : register(s0);


// ============================================================================
// Testparameter
// ============================================================================
//
// Nach unseren klassischen Kuwahara-Experimenten starten wir keineswegs
// zaghaft.
//
// radius
//     Grundradius des anisotropen Filters.
//
// alpha
//     Stärke der Anisotropie.
//
//     Höher:
//         Ellipse wird bei gerichteten Strukturen schlanker.
//
//     Niedriger:
//         Verhalten nähert sich stärker einem isotropen Kuwahara.
//
// q
//     Stärke der Bevorzugung homogener Sektoren.
//
//     Höher:
//         malerischer, selektiver.
//
//     Niedriger:
//         weichere Mischung der Sektoren.
//
// ============================================================================

static const int radius = 96;

static const float alpha = 1.0;
static const float q = 8.0;


// ============================================================================
// Vertex-Ausgabe
// ============================================================================

struct VertexOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


// ============================================================================
// Fullscreen-Triangle
// ============================================================================

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


    output.position =
        float4(
            positions[vertexId],
            0.0,
            1.0);

    output.texCoord =
        texCoords[vertexId];


    return output;
}


// ============================================================================
// Hilfsfunktion: RGB-Sample
// ============================================================================

float3 SampleRGB(
    float2 uv)
{
    return sourceTexture.SampleLevel(
        sourceSampler,
        uv,
        0.0).rgb;
}


// ============================================================================
// Structure Tensor
//
// Für den ersten Test wird der lokale Gradient direkt per Sobel aus RGB
// geschätzt.
//
// Anschließend:
//
//     Jxx = dot(gx, gx)
//     Jyy = dot(gy, gy)
//     Jxy = dot(gx, gy)
//
// Daraus entsteht:
//
//     | Jxx  Jxy |
//     | Jxy  Jyy |
//
// ============================================================================

float3 CalculateStructureTensor(
    float2 uv,
    float2 texelSize)
{
    float3 c00;
    float3 c10;
    float3 c20;

    float3 c01;
    float3 c21;

    float3 c02;
    float3 c12;
    float3 c22;

    float3 gx;
    float3 gy;

    float jxx;
    float jyy;
    float jxy;


    c00 = SampleRGB(uv + float2(-texelSize.x, -texelSize.y));
    c10 = SampleRGB(uv + float2(0.0, -texelSize.y));
    c20 = SampleRGB(uv + float2(texelSize.x, -texelSize.y));

    c01 = SampleRGB(uv + float2(-texelSize.x, 0.0));
    c21 = SampleRGB(uv + float2(texelSize.x, 0.0));

    c02 = SampleRGB(uv + float2(-texelSize.x, texelSize.y));
    c12 = SampleRGB(uv + float2(0.0, texelSize.y));
    c22 = SampleRGB(uv + float2(texelSize.x, texelSize.y));


    gx =
        (
            -c00 -
            2.0 * c01 -
            c02 +
            c20 +
            2.0 * c21 +
            c22
        ) * 0.25;


    gy =
        (
            -c00 -
            2.0 * c10 -
            c20 +
            c02 +
            2.0 * c12 +
            c22
        ) * 0.25;


    jxx =
        dot(
            gx,
            gx);

    jyy =
        dot(
            gy,
            gy);

    jxy =
        dot(
            gx,
            gy);


    return float3(
        jxx,
        jyy,
        jxy);
}


// ============================================================================
// Berechnet lokale Hauptrichtung und Anisotropie.
//
// Für einen symmetrischen 2x2-Tensor:
//
//     | a  b |
//     | b  c |
//
// sind:
//
//     trace = a + c
//
//     delta = sqrt((a-c)^2 + 4b^2)
//
//     lambda1 = (trace + delta) / 2
//     lambda2 = (trace - delta) / 2
//
// Die Anisotropie wird normiert:
//
//     A = (lambda1 - lambda2) / (lambda1 + lambda2)
//
// ============================================================================

void CalculateOrientation(
    float3 tensor,
    out float angle,
    out float anisotropy)
{
    float a;
    float c;
    float b;

    float trace;
    float delta;

    float lambda1;
    float lambda2;

    float denominator;


    a =
        tensor.x;

    c =
        tensor.y;

    b =
        tensor.z;


    trace =
        a + c;

    delta =
        sqrt(
            max(
                0.0,
                (a - c) * (a - c) +
                4.0 * b * b));


    lambda1 =
        0.5 *
        (trace + delta);

    lambda2 =
        0.5 *
        (trace - delta);


    denominator =
        lambda1 +
        lambda2 +
        0.000001;


    anisotropy =
        saturate(
            (lambda1 - lambda2) /
            denominator);


    // ------------------------------------------------------------------------
    // Hauptrichtung des Tensors.
    //
    // Für unsere Filterellipse interessiert uns letztlich die Richtung
    // entlang der lokalen Struktur.
    //
    // Die klassische Tensorformel liefert die Richtung des stärksten
    // Gradienten.
    //
    // Deshalb drehen wir anschließend um 90 Grad, um entlang statt quer
    // zur dominanten Kante zu filtern.
    // ------------------------------------------------------------------------

    angle =
        0.5 *
        atan2(
            2.0 * b,
            a - c);

    angle +=
        1.57079632679;
}


// ============================================================================
// Rotiert einen 2D-Vektor.
//
// ============================================================================

float2 RotateVector(
    float2 value,
    float angle)
{
    float sineValue;
    float cosineValue;


    sineValue =
        sin(angle);

    cosineValue =
        cos(angle);


    return float2(
        cosineValue * value.x -
        sineValue * value.y,

        sineValue * value.x +
        cosineValue * value.y);
}


// ============================================================================
// PixelShader
// ============================================================================

float4 PSMain(VertexOutput input) : SV_TARGET
{
    const int sectorCount = 8;

    uint textureWidth;
    uint textureHeight;

    float2 texelSize;

    float4 centerColor;

    float3 tensor;

    float angle;
    float anisotropy;

    float majorRadius;
    float minorRadius;

    float3 mean[sectorCount];
    float3 secondMoment[sectorCount];

    float weightSum[sectorCount];

    float3 sampleColor;

    float2 sampleOffset;
    float2 localOffset;
    float2 normalizedOffset;

    float ellipseDistanceSquared;

    float sampleAngle;
    float sectorPosition;

    float angularDistance;

    float sectorWeight;
    float radialWeight;
    float combinedWeight;

    float3 variance;

    float varianceMagnitude;
    float confidence;

    float3 finalColor;
    float finalWeight;

    int sectorIndex;

    int x;
    int y;
    int k;


    sourceTexture.GetDimensions(
        textureWidth,
        textureHeight);

    texelSize =
        1.0 /
        float2(
            (float) textureWidth,
            (float) textureHeight);


    centerColor =
        sourceTexture.SampleLevel(
            sourceSampler,
            input.texCoord,
            0.0);


    // ========================================================================
    // Lokale Bildstruktur bestimmen
    // ========================================================================

    tensor =
        CalculateStructureTensor(
            input.texCoord,
            texelSize);

    CalculateOrientation(
        tensor,
        angle,
        anisotropy);


    // ========================================================================
    // Ellipsenform bestimmen
    //
    // Bei anisotropy = 0:
    //
    //     majorRadius = radius
    //     minorRadius = radius
    //
    // Bei stärkerer lokaler Richtung:
    //
    //     majorRadius wächst leicht
    //     minorRadius schrumpft
    //
    // Dadurch wird entlang von Features stärker geglättet als quer dazu.
    // ========================================================================

    majorRadius =
        (float) radius *
        (1.0 + alpha * anisotropy);

    minorRadius =
        (float) radius /
        (1.0 + alpha * anisotropy);


    // ========================================================================
    // Akkumulatoren initialisieren
    // ========================================================================

    for (k = 0; k < sectorCount; k++)
    {
        mean[k] =
            float3(
                0.0,
                0.0,
                0.0);

        secondMoment[k] =
            float3(
                0.0,
                0.0,
                0.0);

        weightSum[k] =
            0.0;
    }


    // ========================================================================
    // Anisotroper Sampling-Bereich
    //
    // Wir durchsuchen zunächst weiterhin ein quadratisches Bounding-Feld.
    // Samples außerhalb der lokal orientierten Ellipse werden verworfen.
    //
    // Für V0.x ist das bewusst einfacher und nachvollziehbarer als eine
    // maximal optimierte Bounding-Box-Berechnung.
    // ========================================================================

    for (y = -radius * 2; y <= radius * 2; y++)
    {
        for (x = -radius * 2; x <= radius * 2; x++)
        {
            sampleOffset =
                float2(
                    (float) x,
                    (float) y);


            // ----------------------------------------------------------------
            // In den lokal ausgerichteten Koordinatenraum drehen.
            // ----------------------------------------------------------------

            localOffset =
                RotateVector(
                    sampleOffset,
                    -angle);


            normalizedOffset =
                float2(
                    localOffset.x /
                    majorRadius,

                    localOffset.y /
                    minorRadius);


            ellipseDistanceSquared =
                dot(
                    normalizedOffset,
                    normalizedOffset);


            if (ellipseDistanceSquared > 1.0)
            {
                continue;
            }


            sampleColor =
                SampleRGB(
                    input.texCoord +
                    sampleOffset *
                    texelSize);


            // ================================================================
            // Radiale Gewichtung
            //
            // Zentrum stärker, Ellipsenrand schwächer.
            // ================================================================

            radialWeight =
                exp(
                    -3.125 *
                    ellipseDistanceSquared);


            // ================================================================
            // Winkel des Samples innerhalb der Ellipse.
            // ================================================================

            sampleAngle =
                atan2(
                    normalizedOffset.y,
                    normalizedOffset.x);

            if (sampleAngle < 0.0)
            {
                sampleAngle +=
                    6.28318530718;
            }


            // ================================================================
            // Acht weiche Sektoren.
            //
            // Jeder Samplepunkt darf mehr als einen Sektor beeinflussen.
            //
            // Das reduziert die harten Blockgrenzen des klassischen
            // Kuwahara.
            // ================================================================

            for (k = 0; k < sectorCount; k++)
            {
                float sectorCenter;

                sectorCenter =
                    6.28318530718 *
                    (
                        ((float) k + 0.5) /
                        (float) sectorCount
                    );


                angularDistance =
                    abs(
                        sampleAngle -
                        sectorCenter);


                angularDistance =
                    min(
                        angularDistance,
                        6.28318530718 -
                        angularDistance);


                // ------------------------------------------------------------
                // Weiche sektorförmige Gewichtung.
                //
                // Ein Sample erhält in der Nähe der Sektormitte großes Gewicht
                // und fällt Richtung Sektorgrenze quadratisch ab.
                // ------------------------------------------------------------

                sectorWeight =
                    saturate(
                        1.0 -
                        angularDistance /
                        (3.14159265359 /
                         4.0));

                sectorWeight *=
                    sectorWeight;


                combinedWeight =
                    radialWeight *
                    sectorWeight;


                if (combinedWeight <= 0.000001)
                {
                    continue;
                }


                mean[k] +=
                    sampleColor *
                    combinedWeight;

                secondMoment[k] +=
                    sampleColor *
                    sampleColor *
                    combinedWeight;

                weightSum[k] +=
                    combinedWeight;
            }
        }
    }


    // ========================================================================
    // Sektoren bewerten
    //
    // Anders als beim klassischen Kuwahara wählen wir NICHT einfach den einen
    // Sektor mit der kleinsten Varianz.
    //
    // Stattdessen werden alle Sektormittelwerte gemischt.
    //
    // Homogene Sektoren erhalten deutlich mehr Gewicht.
    //
    // Dieses Vorgehen entspricht der Grundidee des generalisierten /
    // anisotropen Kuwahara und reduziert abrupte Regionswechsel.
    // ========================================================================

    finalColor =
        float3(
            0.0,
            0.0,
            0.0);

    finalWeight =
        0.0;


    for (k = 0; k < sectorCount; k++)
    {
        if (weightSum[k] <= 0.000001)
        {
            continue;
        }


        mean[k] /=
            weightSum[k];


        secondMoment[k] /=
            weightSum[k];


        variance =
            secondMoment[k] -
            mean[k] *
            mean[k];


        variance =
            max(
                variance,
                float3(
                    0.0,
                    0.0,
                    0.0));


        varianceMagnitude =
            sqrt(
                variance.r +
                variance.g +
                variance.b);


        // --------------------------------------------------------------------
        // Homogene Sektoren stark bevorzugen.
        //
        // Entspricht konzeptionell:
        //
        //     weight = variance ^ (-q)
        //
        // mit Sicherheitsuntergrenze.
        // --------------------------------------------------------------------

        confidence =
            pow(
                max(
                    0.0001,
                    varianceMagnitude),
                -q);


        finalColor +=
            mean[k] *
            confidence;

        finalWeight +=
            confidence;
    }


    if (finalWeight > 0.000001)
    {
        finalColor /=
            finalWeight;
    }
    else
    {
        finalColor =
            centerColor.rgb;
    }


    return float4(
        finalColor,
        centerColor.a);
}