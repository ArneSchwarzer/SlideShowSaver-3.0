// ============================================================
// MPPValidationSeed.fx
//
// Erzeugt vier bekannte, 24-Bit-codierte Zustandswerte.
// ============================================================

sampler2D InputSampler
    : register(s0);


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
    float2 uv : TEXCOORD0) : COLOR
{
    float4 inputSample;
    float testValue;

    inputSample =
        tex2D(
            InputSampler,
            uv);

    if (uv.x < 0.5)
    {
        testValue =
            uv.y < 0.5
            ? 0.10
            : 0.55;
    }
    else
    {
        testValue =
            uv.y < 0.5
            ? 0.30
            : 0.75;
    }

    // Der minimale Bezug bindet den WPF-Eingang tatsächlich ein.
    testValue *=
        inputSample.a;

    return
        Encode24BitNormalized(
            testValue);
}