// ============================================================
// MPPValidationOutput.fx
//
// Identity-Test.
//
// Gibt den Eingang unverändert aus.
// ============================================================

sampler2D StateSampler : register(s0);

float4 main(
    float2 uv : TEXCOORD0) : COLOR
{
    return tex2D(
        StateSampler,
        uv);
}