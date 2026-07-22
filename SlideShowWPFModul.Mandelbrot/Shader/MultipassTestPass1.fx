// ============================================================
// MultipassTestPass1.fx
//
// Isolationstest:
// Wird ein ps_3_0-Shader beim RenderTargetBitmap.Render()
// überhaupt ausgeführt?
// ============================================================

sampler2D InputSampler
    : register(s0);

float4 main(
    float2 uv : TEXCOORD0) : COLOR
{
    float4 inputSample;

    inputSample =
        tex2D(
            InputSampler,
            uv);

    // Der minimale Bezug auf den Eingang verhindert,
    // dass der Sampler vollständig wegoptimiert wird.
    return float4(
        1.0,
        inputSample.g * 0.0,
        inputSample.b * 0.0,
        1.0);
}