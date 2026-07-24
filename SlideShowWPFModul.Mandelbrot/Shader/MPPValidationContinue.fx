// ============================================================
// MPPValidationContinue.fx
//
// Diagnosetest:
// Der Eingangssampler bleibt zwar registriert, wird aber bewusst
// nicht ausgewertet.
//
// Die Ausgabe h‰ngt ausschlieﬂlich von PassIndex und MaxPasses ab.
// ============================================================

sampler2D StateTexture : register(s0);

float PassIndex : register(c0);
float MaxPasses : register(c1);

float4 main(float2 texCoord : TEXCOORD0) : COLOR0
{
    float safeMaxPasses;
    float redValue;

    safeMaxPasses =
        max(MaxPasses, 1.0);

    redValue =
        saturate(
            PassIndex /
            safeMaxPasses);

    return float4(
        redValue,
        0.0,
        0.0,
        1.0);
}