float CenterX : register(c0);
float CenterY : register(c1);
float Scale : register(c2);
float MaxIterations : register(c3);
float ViewportWidth : register(c4);
float ViewportHeight : register(c5);
float GradientOffset : register(c6);
float GradientIndex : register(c7);

float3 PaletteRainbow(float t)
{
    float r = 0.5 + 0.5 * cos(6.28318 * (t + 0.00));
    float g = 0.5 + 0.5 * cos(6.28318 * (t + 0.33));
    float b = 0.5 + 0.5 * cos(6.28318 * (t + 0.67));
    return float3(r, g, b);
}

float3 PaletteZebra(float t)
{
    float v = 0.5 + 0.5 * cos(6.28318 * t * 16.0);
    v = smoothstep(0.35, 0.65, v);
    return float3(v, v, v);
}

float3 PaletteJoker(float t)
{
    float3 darkPurple = float3(0.10, 0.00, 0.18);
    float3 violet = float3(0.48, 0.00, 0.75);
    float3 green = float3(0.00, 0.85, 0.20);
    float3 lime = float3(0.55, 1.00, 0.10);

    float p = frac(t * 3.0);

    if (p < 0.33)
        return lerp(darkPurple, violet, p / 0.33);

    if (p < 0.66)
        return lerp(violet, green, (p - 0.33) / 0.33);

    return lerp(green, lime, (p - 0.66) / 0.34);
}

float3 PaletteWakanda(float t)
{
    float3 deepPurple = float3(0.06, 0.00, 0.12);
    float3 royalPurple = float3(0.35, 0.05, 0.55);
    float3 brown = float3(0.38, 0.20, 0.08);
    float3 white = float3(0.95, 0.90, 0.82);

    float p = frac(t * 4.0);

    if (p < 0.35)
        return lerp(deepPurple, royalPurple, p / 0.35);

    if (p < 0.65)
        return lerp(royalPurple, brown, (p - 0.35) / 0.30);

    if (p < 0.82)
        return lerp(brown, white, (p - 0.65) / 0.17);

    return lerp(white, deepPurple, (p - 0.82) / 0.18);
}

float3 PaletteWeihnachten(float t)
{
    float3 darkGreen = float3(0.00, 0.18, 0.05);
    float3 green = float3(0.00, 0.55, 0.12);
    float3 red = float3(0.70, 0.02, 0.02);
    float3 brown = float3(0.35, 0.18, 0.07);
    float3 white = float3(0.95, 0.95, 0.90);

    float p = frac(t * 5.0);

    if (p < 0.25)
        return lerp(darkGreen, green, p / 0.25);

    if (p < 0.50)
        return lerp(green, red, (p - 0.25) / 0.25);

    if (p < 0.70)
        return lerp(red, brown, (p - 0.50) / 0.20);

    if (p < 0.85)
        return lerp(brown, white, (p - 0.70) / 0.15);

    return lerp(white, darkGreen, (p - 0.85) / 0.15);
}

float3 PalettePastell(float t)
{
    float r = 0.75 + 0.20 * cos(6.28318 * (t + 0.00));
    float g = 0.78 + 0.17 * cos(6.28318 * (t + 0.33));
    float b = 0.82 + 0.15 * cos(6.28318 * (t + 0.67));

    return saturate(float3(r, g, b));
}

float3 GetPaletteColor(float t)
{
    t = frac(t + GradientOffset);

    if (GradientIndex < 0.5)
        return PaletteRainbow(t);

    if (GradientIndex < 1.5)
        return PaletteZebra(t);

    if (GradientIndex < 2.5)
        return PaletteJoker(t);

    if (GradientIndex < 3.5)
        return PaletteWakanda(t);

    if (GradientIndex < 4.5)
        return PaletteWeihnachten(t);

    return PalettePastell(t);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float aspect = ViewportWidth / max(ViewportHeight, 1.0);

    // Numerisch etwas sauberer als direkt alles in einer Zeile zu addieren.
    float2 pixelOffset;
    pixelOffset.x = (uv.x - 0.5) * Scale * aspect;
    pixelOffset.y = (uv.y - 0.5) * Scale;

    float2 c = float2(CenterX, CenterY) + pixelOffset;

    float zx = 0.0;
    float zy = 0.0;

    float iteration = MaxIterations;

    for (int i = 0; i < 1000; i++)
    {
        if (i >= MaxIterations)
            break;

        float zx2 = zx * zx;
        float zy2 = zy * zy;

        if ((zx2 + zy2) > 4.0)
        {
            iteration = i;
            break;
        }

        zy = 2.0 * zx * zy + c.y;
        zx = zx2 - zy2 + c.x;
    }

    if (iteration >= MaxIterations)
        return float4(0.0, 0.0, 0.0, 1.0);

    float t = iteration / MaxIterations;
    float3 color = GetPaletteColor(t);

    return float4(color, 1.0);
}