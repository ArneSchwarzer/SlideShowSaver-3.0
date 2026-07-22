// ============================================================
// MultipassTestPass2.fx
//
// Zweiter Pass des Multipass-Proof-of-Concepts.
//
// Der erste Pass wird über s0 eingelesen.
// Die 24-Bit-Zahl wird decodiert.
// Anschließend wird 0,125 addiert.
//
// Das Ergebnis wird als Graustufe ausgegeben.
// ============================================================


// ------------------------------------------------------------
// Zustandstextur aus Pass 1
// ------------------------------------------------------------

sampler2D StateSampler
    : register(s0);


// ============================================================
// 24-Bit-Decodierung
// ============================================================

float Decode24BitNormalized(
    float4 sampleValue)
{
    float redByte;
    float greenByte;
    float blueByte;

    float encodedValue;

    redByte =
        floor(
            sampleValue.r *
            255.0 +
            0.5);

    greenByte =
        floor(
            sampleValue.g *
            255.0 +
            0.5);

    blueByte =
        floor(
            sampleValue.b *
            255.0 +
            0.5);

    encodedValue =
        redByte *
        65536.0 +
        greenByte *
        256.0 +
        blueByte;

    return
        encodedValue /
        16777215.0;
}


// ============================================================
// Hauptshader
// ============================================================

float4 main(
    float2 uv : TEXCOORD) : COLOR
{
    float4 stateSample;

    float oldValue;
    float newValue;

    stateSample =
        tex2D(
            StateSampler,
            uv);

    oldValue =
        Decode24BitNormalized(
            stateSample);

    newValue =
        saturate(
            oldValue +
            0.125);

    return float4(
        newValue,
        newValue,
        newValue,
        1.0);
}