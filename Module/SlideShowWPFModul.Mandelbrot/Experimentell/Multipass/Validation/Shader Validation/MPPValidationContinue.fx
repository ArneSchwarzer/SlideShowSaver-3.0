// ============================================================
// MPPValidationContinue.fx
//
// Encode-Decode-Encode-Identitätstest:
//
// Liest einen 24-Bit-kodierten normalisierten Wert,
// dekodiert ihn und kodiert ihn anschließend unverändert erneut.
//
// Erwartung:
// Das sichtbare Bild bleibt über alle Ping-Pong-Pässe stabil.
// ============================================================

sampler2D StateSampler : register(s0);


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


float4 Encode24BitNormalized(
    float value)
{
    float encodedValue;

    float redByte;
    float greenByte;
    float blueByte;

    value =
        saturate(
            value);

    encodedValue =
        floor(
            value *
            16777215.0 +
            0.5);

    redByte =
        floor(
            encodedValue /
            65536.0);

    encodedValue -=
        redByte *
        65536.0;

    greenByte =
        floor(
            encodedValue /
            256.0);

    blueByte =
        encodedValue -
        greenByte *
        256.0;

    return float4(
        redByte / 255.0,
        greenByte / 255.0,
        blueByte / 255.0,
        1.0);
}


float4 main(
    float2 uv : TEXCOORD0) : COLOR0
{
    float4 stateSample;
    float decodedValue;

    stateSample =
        tex2D(
            StateSampler,
            uv);

    decodedValue =
        Decode24BitNormalized(
            stateSample);

    return
        Encode24BitNormalized(
            decodedValue);
}