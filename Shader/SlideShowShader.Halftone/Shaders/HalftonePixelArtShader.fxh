// HalftonePixel.fxh
sampler2D inputTex : register(s0);
sampler2D paperTex : register(s1); // optional (enable via flag)

float2 texSize : register(c0); // 1 durch texSize.xy nuetzlich
float4 params0 : register(c1); // cellSize, dotScale, gamma, mode (0=Pixel,1=Halftone)
float4 anglesCMYK : register(c2); // in degrees: C,M,Y,K
float4 flags : register(c3); // x=useCMYK, y=usePaper, z=posterizeLevels, w=unused

// Hilfsfunktionen
float3 ToLinear(float3 c)
{
    return pow(c, 2.2);
}
float3 ToSRGB(float3 c)
{
    return pow(c, 1.0 / 2.2);
}

float3 Posterize(float3 c, float levels)
{
    if (levels <= 1.0)
        return c;
    float3 q = floor(c * (levels - 1.0) + 0.5) / (levels - 1.0);
    return saturate(q);
}

float2 rotate(float2 p, float rad)
{
    float s = sin(rad), c = cos(rad);
    return float2(c * p.x - s * p.y, s * p.x + c * p.y);
}

// HalftoneDichte fuer einen Kanal
float channelDot(float val, float2 uv, float angleDeg, float cell)
{
    // Kanalraster: UV drehen, dann in Zellkoordinaten ueberfuehren
    float rad = radians(angleDeg);
    float2 ruv = rotate(uv - 0.5, rad) + 0.5;
    float2 grid = ruv * cell;
    float2 cellCenter = (floor(grid) + 0.5) / cell;

    // Distanz zum Zellzentrum ergibt Punkt
    float2 d = (ruv - cellCenter) * cell; // in ZellNormkoordinaten
    float dist = length(d);

    // PunktRadius aus Kanalwert (dunkel = grosser Punkt)
    float dotScale = params0.y; // 0..1 Skala
    float radius = dotScale * (1.0 - val) * 0.5; // 0..0.5 (Zellhalbweite)

    // Weiche Kante: smoothstep
    float edge = smoothstep(radius, radius - 0.5, dist);
    // edge ≈ 0 in Punkt, 1 ausserhalb → invertieren, damit „Tinte“=1 im Punkt
    return 1.0 - edge;
}

float4 mainPS(float2 uv : TEXCOORD0) : COLOR
{
    float cell = max(1.0, params0.x);
    float mode = params0.w;
    float useCMYK = flags.x;
    float usePaper = flags.y;
    float levels = max(1.0, flags.z);

    float3 src = tex2D(inputTex, uv).rgb;
    src = ToLinear(src); // in Linear

    if (mode < 0.5)
    {
        //PIXELATE
        float2 grid = uv * cell;
        float2 center = (floor(grid) + 0.5) / cell;
        float3 avg = tex2D(inputTex, center).rgb; // 1 Tap als Naeherung
        avg = ToLinear(avg);
        avg = Posterize(avg, levels);
        float3 outRGB = ToSRGB(avg);

        if (usePaper > 0.5)
        {
            float paper = tex2D(paperTex, uv).r;
            outRGB = saturate(outRGB * lerp(1.0, paper, 0.25)); // dezentes Multiply
        }

        // GammaRegler
        outRGB = pow(outRGB, 1.0 / max(0.001, params0.z));
        return float4(outRGB, 1);
    }
    else
    {
        //HALFTONE
        float3 rgb = Posterize(src, levels);

        float c, m, y, k;
        // einfache RGB in CMYK
        float3 cmy = 1.0 - rgb;
        k = min(cmy.r, min(cmy.g, cmy.b));
        float denom = max(1e-6, 1.0 - k);
        c = (cmy.r - k) / denom;
        m = (cmy.g - k) / denom;
        y = (cmy.b - k) / denom;

        float dC = channelDot(c, uv, anglesCMYK.x, cell);
        float dM = channelDot(m, uv, anglesCMYK.y, cell);
        float dY = channelDot(y, uv, anglesCMYK.z, cell);
        float dK = channelDot(k, uv, anglesCMYK.w, cell);

        // Rekonstruktion: subtraktiver Druck (vereinfachtes Modell)
        float3 ink =
            (1.0 - dK) * float3(0, 0, 0) +
            (1.0 - dC) * float3(0, 1, 1) + // Cyan zieht Rot ab
            (1.0 - dM) * float3(1, 0, 1) + // Magenta zieht Gruen ab
            (1.0 - dY) * float3(1, 1, 0); // Yellow zieht Blau ab

        // Von 4 Ueberlagerungen zu RGB einfache Mischung, dann clamp
        float3 outRGB = saturate(1.0 - ink * 0.7); // 0.7 = Deckkraft Tuning

        if (usePaper > 0.5)
        {
            float paper = tex2D(paperTex, uv).r;
            outRGB = saturate(outRGB * lerp(1.0, paper, 0.25));
        }

        outRGB = ToSRGB(outRGB);
        outRGB = pow(outRGB, 1.0 / max(0.001, params0.z));
        return float4(outRGB, 1);
    }
}

technique t0
{
    pass P0
    {
        PixelShader = compile ps_2_0 mainPS();
    }
}
