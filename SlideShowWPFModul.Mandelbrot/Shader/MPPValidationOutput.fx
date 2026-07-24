// ============================================================
// MPPValidationOutput.fx
//
// Diagnosetest:
// Reicht die Ausgabe des aktuellen Ping-Pong-Nodes
// unverändert an den Bildschirm weiter.
// ============================================================

sampler2D StateSampler : register(s0);

float4 main(
    float2 uv : TEXCOORD0) : COLOR0
{
    return tex2D(
        StateSampler,
        uv);
}