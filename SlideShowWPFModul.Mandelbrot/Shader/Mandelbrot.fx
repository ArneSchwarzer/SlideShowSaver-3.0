sampler2D GradientSampler : register(s0);

float CenterX : register(c0);
float CenterY : register(c1);
float Scale : register(c2);
float MaxIterations : register(c3);
float ViewportWidth : register(c4);
float ViewportHeight : register(c5);
float GradientOffset : register(c6);

float3 GetPaletteColor(float t)
{
    t = frac(t + GradientOffset);
    return tex2D(GradientSampler, float2(t, 0.5)).rgb;
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