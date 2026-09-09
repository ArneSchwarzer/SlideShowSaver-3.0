sampler2D GradientSampler : register(s0);

float CenterXHigh : register(c0);
float CenterXLow : register(c1);
float CenterYHigh : register(c2);
float CenterYLow : register(c3);
float ScaleHigh : register(c4);
float ScaleLow : register(c5);
float MaxIterations : register(c6);
float ViewportWidth : register(c7);
float ViewportHeight : register(c8);
float GradientOffset : register(c9);
float Rotation : register(c10);

float2 ds_normalize(float2 a)
{
    float s = a.x + a.y;
    float e = a.y - (s - a.x);
    return float2(s, e);
}

float2 ds_set(float a)
{
    return float2(a, 0.0);
}

float2 ds_add(float2 a, float2 b)
{
    float s = a.x + b.x;
    float v = s - a.x;
    float e = (a.x - (s - v)) + (b.x - v) + a.y + b.y;
    return ds_normalize(float2(s, e));
}

float2 ds_sub(float2 a, float2 b)
{
    return ds_add(a, float2(-b.x, -b.y));
}

float2 ds_mul(float2 a, float2 b)
{
    float split = 4097.0;

    float cona = a.x * split;
    float conb = b.x * split;

    float a1 = cona - (cona - a.x);
    float b1 = conb - (conb - b.x);

    float a2 = a.x - a1;
    float b2 = b.x - b1;

    float c11 = a.x * b.x;
    float c21 = (((a1 * b1 - c11) + a1 * b2) + a2 * b1) + a2 * b2;

    c21 += a.x * b.y + a.y * b.x;

    return ds_normalize(float2(c11, c21));
}

float2 ds_mul_float(float2 a, float b)
{
    return ds_mul(a, ds_set(b));
}

float ds_to_float(float2 a)
{
    return a.x + a.y;
}

float3 GetPaletteColor(float t)
{
    t = frac(t + GradientOffset);
    return tex2D(GradientSampler, float2(t, 0.5)).rgb;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float aspect = ViewportWidth / max(ViewportHeight, 1.0);

    float px = (uv.x - 0.5) * aspect;
    float py = uv.y - 0.5;

    float sinAngle = sin(Rotation);
    float cosAngle = cos(Rotation);

    float rotatedX = px * cosAngle - py * sinAngle;
    float rotatedY = px * sinAngle + py * cosAngle;

    px = rotatedX;
    py = rotatedY;

    float2 scale = float2(ScaleHigh, ScaleLow);

    float2 cx = ds_add(
        float2(CenterXHigh, CenterXLow),
        ds_mul_float(scale, px));

    float2 cy = ds_add(
        float2(CenterYHigh, CenterYLow),
        ds_mul_float(scale, py));

    float2 zx = ds_set(0.0);
    float2 zy = ds_set(0.0);

    float iteration = MaxIterations;

    for (int i = 0; i < 1000; i++)
    {
        if (i >= MaxIterations)
            break;

        float2 zx2 = ds_mul(zx, zx);
        float2 zy2 = ds_mul(zy, zy);

        float mag = ds_to_float(zx2) + ds_to_float(zy2);

        if (mag > 4.0)
        {
            iteration = i;
            break;
        }

        float2 zxy = ds_mul(zx, zy);

        float2 newZx = ds_add(ds_sub(zx2, zy2), cx);
        float2 newZy = ds_add(ds_mul_float(zxy, 2.0), cy);

        zx = newZx;
        zy = newZy;
    }

    if (iteration >= MaxIterations)
        return float4(0.0, 0.0, 0.0, 1.0);

    float t = iteration / MaxIterations;
    float3 color = GetPaletteColor(t);

    return float4(color, 1.0);
}