// ============================================================
// MandelbrotPerturbation.fx
//
// Perturbation Rendering:
//     Z(n+1) = Z(n)^2 + C
//
// Pixelbahn:
//     Z(n) + dZ(n)
//
// Perturbationsgleichung:
//     dZ(n+1) = 2 * Z(n) * dZ(n)
//               + dZ(n)^2
//               + dC
//
// Referenzorbit:
//     s1 = Realteil, jeweils High/Low in R/G
//     s2 = ImaginÃƒÂ¤rteil, jeweils High/Low in R/G
// ============================================================


// ------------------------------------------------------------
// Texturen
// ------------------------------------------------------------

sampler2D GradientSampler : register(s0);

sampler2D ReferenceOrbitRealSampler : register(s1);
sampler2D ReferenceOrbitImagSampler : register(s2);


// ------------------------------------------------------------
// Shaderkonstanten
// ------------------------------------------------------------

float ScaleHigh : register(c0);
float ScaleLow : register(c1);

float MaxIterations : register(c2);
float OrbitLength : register(c3);
float OrbitTextureWidth : register(c4);

float ViewportWidth : register(c5);
float ViewportHeight : register(c6);

float GradientOffset : register(c7);
float Rotation : register(c8);


// ============================================================
// Double-Single-Arithmetik
//
// float2:
//     x = High-Anteil
//     y = Low-Anteil
// ============================================================

float2 ds_set(float value)
{
    return float2(value, 0.0);
}


float2 ds_normalize(float2 value)
{
    float high;
    float low;

    high = value.x + value.y;
    low = value.y - (high - value.x);

    return float2(high, low);
}


float2 ds_add(float2 a, float2 b)
{
    float sum;
    float virtualB;
    float error;
    float resultHigh;
    float resultLow;

    sum = a.x + b.x;
    virtualB = sum - a.x;

    error =
        (a.x - (sum - virtualB)) +
        (b.x - virtualB) +
        a.y +
        b.y;

    resultHigh = sum + error;
    resultLow = error - (resultHigh - sum);

    return float2(resultHigh, resultLow);
}


float2 ds_sub(float2 a, float2 b)
{
    return ds_add(
        a,
        float2(-b.x, -b.y));
}


float2 ds_mul(float2 a, float2 b)
{
    const float split = 4097.0;

    float conA;
    float conB;

    float aHigh;
    float aLow;
    float bHigh;
    float bLow;

    float productHigh;
    float productError;

    conA = a.x * split;
    conB = b.x * split;

    aHigh = conA - (conA - a.x);
    bHigh = conB - (conB - b.x);

    aLow = a.x - aHigh;
    bLow = b.x - bHigh;

    productHigh = a.x * b.x;

    productError =
        (((aHigh * bHigh - productHigh) +
          aHigh * bLow) +
          aLow * bHigh) +
          aLow * bLow;

    productError +=
        a.x * b.y +
        a.y * b.x;

    return ds_normalize(
        float2(productHigh, productError));
}


float2 ds_mul_float(float2 a, float b)
{
    return ds_mul(
        a,
        ds_set(b));
}


float ds_to_float(float2 value)
{
    return value.x + value.y;
}


// ============================================================
// Komplexe Double-Single-Zahl
//
// Realteil und ImaginÃƒÂ¤rteil werden getrennt als float2 gefÃƒÂ¼hrt.
// ============================================================

struct ComplexDS
{
    float2 Real;
    float2 Imaginary;
};


ComplexDS complex_zero()
{
    ComplexDS result;

    result.Real = ds_set(0.0);
    result.Imaginary = ds_set(0.0);

    return result;
}


ComplexDS complex_add(ComplexDS a, ComplexDS b)
{
    ComplexDS result;

    result.Real =
        ds_add(a.Real, b.Real);

    result.Imaginary =
        ds_add(a.Imaginary, b.Imaginary);

    return result;
}


ComplexDS complex_square(ComplexDS value)
{
    ComplexDS result;

    float2 realSquared;
    float2 imaginarySquared;
    float2 realImaginary;

    realSquared =
        ds_mul(value.Real, value.Real);

    imaginarySquared =
        ds_mul(value.Imaginary, value.Imaginary);

    realImaginary =
        ds_mul(value.Real, value.Imaginary);

    result.Real =
        ds_sub(
            realSquared,
            imaginarySquared);

    result.Imaginary =
        ds_mul_float(
            realImaginary,
            2.0);

    return result;
}


ComplexDS complex_multiply(ComplexDS a, ComplexDS b)
{
    ComplexDS result;

    float2 realReal;
    float2 imagImag;
    float2 realImag;
    float2 imagReal;

    realReal =
        ds_mul(a.Real, b.Real);

    imagImag =
        ds_mul(a.Imaginary, b.Imaginary);

    realImag =
        ds_mul(a.Real, b.Imaginary);

    imagReal =
        ds_mul(a.Imaginary, b.Real);

    result.Real =
        ds_sub(
            realReal,
            imagImag);

    result.Imaginary =
        ds_add(
            realImag,
            imagReal);

    return result;
}


ComplexDS complex_multiply_float(ComplexDS value, float factor)
{
    ComplexDS result;

    result.Real =
        ds_mul_float(value.Real, factor);

    result.Imaginary =
        ds_mul_float(value.Imaginary, factor);

    return result;
}


// ============================================================
// Referenzorbit aus den Texturen lesen
// ============================================================

float DecodeOrbitValue(float3 encodedColor)
{
    float redByte;
    float greenByte;
    float blueByte;
    float encodedValue;
    float normalizedValue;

    // Die normalisierten Texturkanäle wieder näherungsweise
    // in ihre Bytewerte 0 bis 255 zurückführen.
    redByte =
        floor(encodedColor.r * 255.0 + 0.5);

    greenByte =
        floor(encodedColor.g * 255.0 + 0.5);

    blueByte =
        floor(encodedColor.b * 255.0 + 0.5);

    encodedValue =
        redByte * 65536.0 +
        greenByte * 256.0 +
        blueByte;

    normalizedValue =
        encodedValue / 16777215.0;

    // [0, 1] zurück nach [-2, +2].
    return normalizedValue * 4.0 - 2.0;
}


float2 ReadReferenceOrbitFloat(int index)
{
    float textureU;
    float4 textureCoordinate;
    float4 realSample;
    float4 imaginarySample;

    textureU =
        ((float)index + 0.5) /
        max(OrbitTextureWidth, 1.0);

    textureCoordinate =
        float4(
            textureU,
            0.5,
            0.0,
            0.0);

    realSample =
        tex2Dlod(
            ReferenceOrbitRealSampler,
            textureCoordinate);

    imaginarySample =
        tex2Dlod(
            ReferenceOrbitImagSampler,
            textureCoordinate);

    return float2(
        DecodeOrbitValue(realSample.rgb),
        DecodeOrbitValue(imaginarySample.rgb));
}

// ============================================================
// Gradient
// ============================================================

float3 GetPaletteColor(float t)
{
    t = frac(t + GradientOffset);

    return tex2D(
        GradientSampler,
        float2(t, 0.5)).rgb;
}


// ============================================================
// Pixel Shader
// ============================================================

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float aspect;
    float pixelX;
    float pixelY;

    float scale;
    float2 deltaC;
    float2 deltaZ;
    float2 referenceZ;
    float2 actualZ;

    float magnitudeSquared;
    float iteration;
    float colorPosition;

    int iterationLimit;
    int i;

    aspect =
        ViewportWidth /
        max(ViewportHeight, 1.0);

    pixelX =
        (uv.x - 0.5) *
        aspect;

    pixelY =
        uv.y - 0.5;

    scale =
        ScaleHigh + ScaleLow;

    deltaC =
        float2(
            pixelX * scale,
            pixelY * scale);

    deltaZ =
        float2(0.0, 0.0);

    iteration = MaxIterations;

    iterationLimit =
        (int)min(
            MaxIterations,
            OrbitLength);

    [loop]
    for (i = 0; i < 2000; i++)
    {
        float deltaReal;
        float deltaImaginary;

        if (i >= iterationLimit)
        {
            break;
        }

        referenceZ =
            ReadReferenceOrbitFloat(i);

        actualZ =
            referenceZ +
            deltaZ;

        magnitudeSquared =
            dot(actualZ, actualZ);

        if (magnitudeSquared > 4.0)
        {
            iteration = (float)i;
            break;
        }

        // ?z(n+1) = 2·Z(n)·?z(n) + ?z(n)² + ?c

        deltaReal =
            2.0 *
            (referenceZ.x * deltaZ.x -
             referenceZ.y * deltaZ.y) +
            (deltaZ.x * deltaZ.x -
             deltaZ.y * deltaZ.y) +
            deltaC.x;

        deltaImaginary =
            2.0 *
            (referenceZ.x * deltaZ.y +
             referenceZ.y * deltaZ.x) +
            2.0 *
            deltaZ.x *
            deltaZ.y +
            deltaC.y;

        deltaZ =
            float2(
                deltaReal,
                deltaImaginary);
    }

    if (iteration >= MaxIterations)
    {
        return float4(
            0.0,
            0.0,
            0.0,
            1.0);
    }

    colorPosition =
        iteration /
        max(MaxIterations, 1.0);

    return float4(
        GetPaletteColor(colorPosition),
        1.0);
}