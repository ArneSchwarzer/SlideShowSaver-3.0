// PixelArtShader.fxh (ps_2_0, ASCII only)

sampler2D inputTex : register(s0);

// c1: (cellSize, levelsPerChannel, _, _)
float4 params0     : register(c1);

float3 posterize(float3 c, float levels)
{
    float L = max(1.0, levels);
    return floor(c * (L - 1.0) + 0.5) / (L - 1.0);
}

float4 mainPS(float2 uv : TEXCOORD0) : COLOR
{
    float cell   = max(1.0, params0.x);
    float levels = max(1.0, params0.y);

    // auf Zellzentrum samplen
    float2 grid   = uv * cell;
    float2 center = (floor(grid) + 0.5) / cell;

    float3 col = tex2D(inputTex, center).rgb;
    col = posterize(col, levels);

    return float4(col, 1.0);
}

technique t0 { pass P0 { PixelShader = compile ps_2_0 mainPS(); } }
