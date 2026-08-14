Texture2D oldImageTexture : register(t0);
Texture2D newImageTexture : register(t1);
Texture2D maskTexture : register(t2);

SamplerState sourceSampler : register(s0);

cbuffer RevealParameter : register(b0)
{
    float progress;
    float padding1;
    float padding2;
    float padding3;
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
    float4 oldColor;
    float4 newColor;
    float maskValue;
    float reveal;

    oldColor =
        oldImageTexture.Sample(
            sourceSampler,
            input.texCoord);

    newColor =
        newImageTexture.Sample(
            sourceSampler,
            input.texCoord);

    maskValue =
        maskTexture.Sample(
            sourceSampler,
            input.texCoord).r;

    reveal =
        step(
            maskValue,
            progress);

    return lerp(
        oldColor,
        newColor,
        reveal);
}