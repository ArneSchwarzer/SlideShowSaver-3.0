// ============================================================================
// WaterFlowShader.hlsl
// ============================================================================
//
// SlideShowSaver 3.0
// Shader: Aquarell
//
// Aufgabe:
//
// Dieser Shader bildet den ersten bewusst einfachen Simulationsschritt
// unserer Aquarell-Wassersimulation.
//
// Für jedes Pixel wird die aktuelle Wassermenge mit den vier direkten
// Nachbarn verglichen:
//
//                    Nord
//                      |
//             West -- P -- Ost
//                      |
//                     Süd
//
// Das neue Wasserniveau bewegt sich ein Stück in Richtung des mittleren
// Niveaus dieser vier Nachbarn.
//
// Dies ist ausdrücklich KEINE vollständige Fluid-Simulation und insbesondere
// keine Lösung der Navier-Stokes-Gleichungen.
//
// Wir verwenden hier eine einfache Diffusionsapproximation:
//
//      Wneu = Waktuell + k * (WnachbarnMittel - Waktuell)
//
// Die Viskosität beeinflusst k:
//
//      hohe Viskosität     -> langsamer Ausgleich
//      niedrige Viskosität -> schneller Ausgleich
//
// Ziel dieses ersten Proof-of-Concepts ist ausschließlich:
//
//      "Kann sich unser Wasserfeld über mehrere Ping-Pong-Iterationen
//       stabil und kontrollierbar räumlich ausgleichen?"
//
// Papierstruktur, Absorption, Pigmenttransport, Verdunstung und
// Oberflächenspannung kommen bewusst später.
//
// ============================================================================


Texture2D<float> sourceWater : register(t0);


// ============================================================================
// SIMULATIONSPARAMETER
// ============================================================================
//
// viscosity enthält die Viskosität direkt in mPa·s.
//
// Beispiele:
//
//      Aceton              ~ 0.3
//      Wasser              ~ 1.0
//      Sonnenblumenöl      ~ 60
//      Honig               ~ 5000
//
// Der TrackBar-Logarithmus wird NICHT hier berechnet.
// Der Shader erhält bereits den physikalisch lesbaren Endwert.
//
// HLSL-ConstantBuffer werden in 16-Byte-Blöcken organisiert.
// Die drei reserve-Werte halten deshalb das Layout explizit sauber.
//
// ============================================================================

cbuffer WaterFlowSettings : register(b0)
{
    float viscosity;

    float reserve1;
    float reserve2;
    float reserve3;
};


// ============================================================================
// VERTEX OUTPUT
// ============================================================================

struct VertexOutput
{
    float4 position : SV_POSITION;
};


// ============================================================================
// FULLSCREEN TRIANGLE
// ============================================================================
//
// Wie bei unseren übrigen D3D-Pässen wird kein VertexBuffer benötigt.
//
// Das übergroße Dreieck deckt das vollständige RenderTarget ab.
//
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

    output.position = float4(positions[vertexId], 0.0, 1.0);

    return output;
}


// ============================================================================
// WASSER LESEN
// ============================================================================
//
// Für die eigentliche Simulation verwenden wir absichtlich Texture.Load()
// statt eines Samplers.
//
// Wir benötigen hier keine interpolierten Werte, sondern exakt den
// Wasserstand eines konkreten Texels.
//
// Gleichzeitig werden Koordinaten am Bildrand geklemmt. Damit besitzt ein
// Randpixel gewissermaßen außerhalb des Bildes einen Nachbarn mit seinem
// eigenen Wasserstand.
//
// Dadurch fließt kein Wasser aus unserer Simulation "aus dem Bild heraus".
//
// ============================================================================

float ReadWater(int2 pixelPosition, int2 textureSize)
{
    int2 clampedPosition;

    clampedPosition = clamp(pixelPosition,
                            int2(0, 0),
                            textureSize - int2(1, 1));

    return sourceWater.Load(int3(clampedPosition, 0));
}


// ============================================================================
// VISKOSITÄT -> DIFFUSIONSSTÄRKE
// ============================================================================
//
// Eine direkte Division:
//
//      1 / viscosity
//
// wäre für unseren großen Parameterbereich ungeeignet.
//
// Bei Honig würde praktisch keinerlei Bewegung mehr stattfinden, während
// sehr niedrige Viskositäten unverhältnismäßig dominant würden.
//
// Deshalb verwenden wir eine komprimierte Fluiditätsfunktion:
//
//      fluidity = 2 / (1 + sqrt(viscosity))
//
// Damit gilt insbesondere:
//
//      viscosity = 1.0 mPa·s
//
//      fluidity = 1.0
//
// Wasser bildet also unseren gut verständlichen Referenzpunkt.
//
// Dünnflüssigere Medien dürfen ausdrücklich fluidity > 1 erreichen.
// Aceton wird damit NICHT künstlich auf Wasser-Niveau gekappt.
//
// Die eigentliche Diffusionsstärke ergibt sich anschließend aus:
//
//      diffusionStrength = baseDiffusion * fluidity
//
// Erst ganz am Ende begrenzen wir den absoluten Simulationsschritt auf
// maxDiffusion.
//
// Dadurch bleibt unser Viskositätsbereich differenziert, ohne numerisch
// unvernünftige Diffusionsschritte zuzulassen.
//
// Dieser Zusammenhang ist eine künstlerisch/technische Approximation und
// keine physikalisch vollständige Beschreibung realer Fluidmechanik.
//
// ============================================================================

float CalculateDiffusionStrength(float currentViscosity)
{
    const float maxDiffusion = 0.45;
    const float lowViscosityMaxDiffusion = 0.85;
    const float waterViscosity = 1.0;
    const float minimumViscosity = 0.1;
    const float lowViscosityCurve = 1.7;

    float safeViscosity;
    float fluidity;
    float diffusionStrength;

    float waterDiffusionStrength;
    float lowViscosityPosition;
    float lowViscosityFactor;


    safeViscosity = max(currentViscosity, minimumViscosity);


    // ------------------------------------------------------------------------
    // Wasser und dickere Flüssigkeiten:
    //
    // Hier behalten wir unsere bisherige, bereits getestete Kurve vollständig
    // bei.
    // ------------------------------------------------------------------------

    if (safeViscosity >= waterViscosity)
    {
        fluidity = 1.0 / (1.0 + sqrt(safeViscosity));

        diffusionStrength = maxDiffusion * fluidity;

        return diffusionStrength;
    }


    // ------------------------------------------------------------------------
    // Dünnflüssiger als Wasser:
    //
    // Der physikalische Zusammenhang wird hier bewusst verlassen.
    //
    // Ziel ist nicht die korrekte Simulation realer Flüssigkeiten, sondern
    // eine deutlich sichtbare gestalterische Abstufung zwischen beispielsweise
    // 0.1, 0.3 und 1.0 mPa·s.
    //
    // Wasser bei 1.0 mPa·s bleibt dabei exakt an derselben Stelle der alten
    // Kurve.
    // ------------------------------------------------------------------------

    waterDiffusionStrength =
        maxDiffusion * (1.0 / (1.0 + sqrt(waterViscosity)));


    // ------------------------------------------------------------------------
    // Logarithmische Position innerhalb des Bereichs:
    //
    //      0.1 mPa·s -> 0.0
    //      1.0 mPa·s -> 1.0
    //
    // Dadurch passt die Berechnung zur ebenfalls logarithmischen
    // Wahrnehmung unseres großen Viskositätsbereichs.
    // ------------------------------------------------------------------------

    lowViscosityPosition =
        log10(safeViscosity / minimumViscosity) /
        log10(waterViscosity / minimumViscosity);

    lowViscosityPosition = saturate(lowViscosityPosition);


    // ------------------------------------------------------------------------
    // Bereich zusätzlich auseinanderziehen.
    //
    // Durch den Exponenten > 1 bleibt die Diffusionsstärke bei sehr dünnen
    // Flüssigkeiten länger hoch und fällt erst in Richtung Wasser deutlich ab.
    // ------------------------------------------------------------------------

    lowViscosityFactor =
        pow(lowViscosityPosition, lowViscosityCurve);


    // ------------------------------------------------------------------------
    // Zwischen unserem bewusst starken "sehr dünnflüssig"-Wert und dem
    // bisherigen Wasserwert interpolieren.
    // ------------------------------------------------------------------------

    diffusionStrength =
        lerp(lowViscosityMaxDiffusion,
             waterDiffusionStrength,
             lowViscosityFactor);


    return diffusionStrength;
}


// ============================================================================
// PIXEL SHADER
// ============================================================================

float PSMain(VertexOutput input) : SV_TARGET
{
    uint textureWidth;
    uint textureHeight;

    int2 textureSize;
    int2 pixelPosition;

    float waterCenter;

    float waterNorth;
    float waterEast;
    float waterSouth;
    float waterWest;

    float neighborAverage;
    float diffusionStrength;

    float waterNew;


    // ------------------------------------------------------------------------
    // Texturgröße bestimmen.
    // ------------------------------------------------------------------------

    sourceWater.GetDimensions(textureWidth, textureHeight);

    textureSize = int2(textureWidth, textureHeight);


    // ------------------------------------------------------------------------
    // SV_POSITION enthält bereits die Position des aktuell berechneten
    // Zielpixels.
    //
    // Die Umwandlung nach int2 liefert uns dessen Texelkoordinate.
    // ------------------------------------------------------------------------

    pixelPosition = int2(input.position.xy);


    // ------------------------------------------------------------------------
    // Aktuellen Wasserstand und direkte Nachbarn lesen.
    // ------------------------------------------------------------------------

    waterCenter = ReadWater(pixelPosition, textureSize);

    const int flowDistance = 8;

    waterNorth = ReadWater(pixelPosition + int2(0, -flowDistance), textureSize);
    waterEast = ReadWater(pixelPosition + int2(flowDistance, 0), textureSize);
    waterSouth = ReadWater(pixelPosition + int2(0, flowDistance), textureSize);
    waterWest = ReadWater(pixelPosition + int2(-flowDistance, 0), textureSize);

    // ------------------------------------------------------------------------
    // Einfacher Mittelwert der vier Nachbarn.
    // ------------------------------------------------------------------------

    neighborAverage =
        (waterNorth +
         waterEast +
         waterSouth +
         waterWest) * 0.25;


    // ------------------------------------------------------------------------
    // Viskosität in die Geschwindigkeit unseres lokalen Wasserausgleichs
    // übersetzen.
    // ------------------------------------------------------------------------

    diffusionStrength = CalculateDiffusionStrength(viscosity);


    // ------------------------------------------------------------------------
    // "Diffusion für Anfänger".
    //
    // Wir bewegen uns nur einen Teil des Weges vom aktuellen Wasserstand
    // zum mittleren Niveau der Umgebung.
    //
    // Bei:
    //
    //      center > average
    //
    // sinkt der lokale Wasserstand.
    //
    // Bei:
    //
    //      center < average
    //
    // steigt er.
    //
    // Da dies aus einer bestehenden Texture gelesen und vollständig in eine
    // zweite Texture geschrieben wird, eignet sich dieser Schritt direkt für
    // unsere sourceWater/targetWater-Ping-Pong-Architektur.
    // ------------------------------------------------------------------------

    waterNew =
        waterCenter +
        diffusionStrength * (neighborAverage - waterCenter);


    // ------------------------------------------------------------------------
    // Numerischer Sicherheitsgurt.
    //
    // Mit der aktuellen Formel sollte aus ausschließlich positiven
    // Eingangswerten ohnehin kein negativer Wasserstand entstehen.
    //
    // Trotzdem wollen wir keine negativen Flüssigkeitsmengen in die nächste
    // Iteration einschleppen.
    //
    // Nach oben clampen wir bewusst NICHT auf 1.0.
    //
    // 1.0 ist für unsere Simulation keine physikalische Maximalmenge,
    // sondern momentan nur Teil unserer gewählten Normierung.
    // ------------------------------------------------------------------------

    waterNew = max(waterNew, 0.0);

    return waterNew;
}