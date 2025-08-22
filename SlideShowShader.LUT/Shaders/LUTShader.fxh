// ============================================================================
// LUTShader.fxh  (ps_2_0)
//  - sampler0: Eingabebild
//  - sampler1: 3D-LUT als 2D-Atlas (Breite=N*N, Höhe=N), linear gefiltert
//  - c1:       (x=N, y=1/(N-1) [Reserve], z=strength 0..1, w=1/(N*N))
// ----------------------------------------------------------------------------
// Atlas-Layout (r,g,b ? [0..N-1]):
//   u = (r + b*N + 0.5)/(N*N)
//   v = (g + 0.5)/N
// Wir nutzen Hardware-Bilinear in (r,g) und lerpen nur noch entlang b (z).
// ============================================================================

sampler2D srcTex : register(s0);
sampler2D lutTex : register(s1);

// Shader-Konstanten: (N, 1/(N-1), strength, 1/(N*N))
float4 lutParams : register(c1);

// --- Channel layout contract -------------------------------------------------
// Unsere Atlas-Bitmaps sind BGRA8 (WPF Bgra32/Pbgra32).
// Manche Pipelines lesen BGRA-Quellen als BGR zurück. Um robust zu sein,
// swizzlen wir hier zentral. Bei echter RGBA-Sampling-Pipeline: Define auf 0.
// (Per Build umschaltbar: /D LUT_ATLAS_BGRA=0)
#ifndef LUT_ATLAS_BGRA
#define LUT_ATLAS_BGRA 0
#endif

inline float3 NormalizeLutRead(float3 v)
{
#if LUT_ATLAS_BGRA
    return v.bgr; // R<->B tauschen (BGRA-Quellen)
#else
    return v.rgb;    // unverändert (echtes RGBA)
#endif
}

// --- 3D-LUT-Sampling: 2× bilinear (xy) + 1× lerp (z) ------------------------
// Voraussetzung: lutTex ist linear gefiltert (WPF: ja).
inline float3 sampleLUT2D(float3 rgb)
{
    float N = lutParams.x;
    float invW = lutParams.w; // 1/(N*N)
    float invN = N * invW; // 1/N

    // In Gitterkoordinaten [0..N-1]
    float3 p = saturate(rgb) * (N - 1.0);
    float3 i0 = floor(p);
    float3 f = p - i0; // (fx, fy, fz) in [0..1)

    // Fraktionale r/g innerhalb der aktuellen Z-Scheibe
    float rF = i0.x + f.x;
    float gF = i0.y + f.y;

    // u0 = (rF + i0.z*N + 0.5)/(N*N);  u1 = u0 + N/(N*N) = u0 + 1/N
    float u0 = (rF + i0.z * N + 0.5) * invW;
    float u1 = u0 + invN;

    // v  = (gF + 0.5)/N
    float v = (gF + 0.5) * invN;

    // Zwei bilineare Samples (xy) aus den Z-Scheiben i0.z und i0.z+1
    float3 c0 = tex2D(lutTex, float2(u0, v)).rgb;
    float3 c1 = tex2D(lutTex, float2(u1, v)).rgb;

    // Linear entlang z
    float3 lut = lerp(c0, c1, f.z);

    return NormalizeLutRead(lut);
}

// --- Hauptshader -------------------------------------------------------------
float4 mainPS(float2 uv : TEXCOORD0) : COLOR
{
    float3 src = tex2D(srcTex, uv).rgb;
    float3 lut = sampleLUT2D(src);

    float s = saturate(lutParams.z); // Stärke 0..1
    float3 outRGB = lerp(src, lut, s);
    return float4(outRGB, 1.0);
}

// --- Technique ---------------------------------------------------------------
technique LUTTech
{
    pass P0
    {
        PixelShader = compile ps_2_0 mainPS();
    }
}
