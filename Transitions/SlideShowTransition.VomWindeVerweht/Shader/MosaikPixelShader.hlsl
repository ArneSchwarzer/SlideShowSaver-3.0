#include "MosaikShaderCommon.hlsli"

Texture2D QuellBild : register(t0);
SamplerState QuellSampler : register(s0);

float4 PSMain(VSOutput input) : SV_TARGET
{
    return QuellBild.Sample(QuellSampler, input.uv);
}