Texture2D sourceTexture : register(t0);
SamplerState sourceSampler : register(s0);

cbuffer ShaderParameter : register(b0)
{
    float orangeStaerke;
    float3 padding;
};

struct VertexOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};

VertexOutput VSMain(uint vertexId : SV_VertexID)
{
    VertexOutput output;

    float2 positions[3] =
    {
        float2(-1.0, -1.0),
        float2(-1.0, 3.0),
        float2(3.0, -1.0)
    };

    float2 texCoords[3] =
    {
        float2(0.0, 1.0),
        float2(0.0, -1.0),
        float2(2.0, 1.0)
    };

    output.position =
        float4(
            positions[vertexId],
            0.0,
            1.0);

    output.texCoord =
        texCoords[vertexId];

    return output;
}

float4 PSMain(VertexOutput input) : SV_TARGET
{
    float4 original;
    float3 orange;
    float3 result;

    original =
        sourceTexture.Sample(
            sourceSampler,
            input.texCoord);

    orange =
        float3(
            original.r * 1.15,
            original.g * 0.60,
            original.b * 0.18);

    result =
        lerp(
            original.rgb,
            orange,
            orangeStaerke);

    return float4(
        saturate(result),
        original.a);
}