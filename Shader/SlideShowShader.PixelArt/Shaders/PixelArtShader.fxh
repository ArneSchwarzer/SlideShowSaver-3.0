sampler2D inputTex : register(s0);

// c0: (1/width, 1/height, 0, 0)
float4 texelSize : register(c0);

// c1: (cellPx, levels, _, _)
float4 params0 : register(c1);

float3 posterize(float3 c, float levels)
{
    float L = max(1.0, levels);
    return floor(c * (L - 1.0) + 0.5) / (L - 1.0);
}

float4 mainPS(float2 uv : TEXCOORD0) : COLOR
{
    float cellPx = max(1.0, params0.x);
    float levels = max(1.0, params0.y);

    // UV -> Pixelkoordinaten
    float2 px = uv / texelSize.xy;

    // Blockzentrum in Pixeln bestimmen und zurück nach UV
    float2 centerPx = (floor(px / cellPx) + 0.5) * cellPx;
    float2 centerUV = centerPx * texelSize.xy;

    float3 col = tex2D(inputTex, centerUV).rgb;
    col = posterize(col, levels);
    return float4(col, 1.0);
}

technique t0
{
    pass P0
    {
        PixelShader = compile ps_2_0 mainPS();
    }
}
