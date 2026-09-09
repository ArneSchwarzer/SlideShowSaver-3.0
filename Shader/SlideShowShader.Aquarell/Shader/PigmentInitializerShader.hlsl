Texture2D kuwaharaTexture : register(t0);

SamplerState sourceSampler : register(s0);

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

    output.position = float4(positions[vertexId], 0.0, 1.0);

    output.texCoord = texCoords[vertexId];

    return output;
}

float4 PSMain(VertexOutput input) : SV_TARGET
{
    float3 pigmentColor;
    float pigmentAmount;

    pigmentColor = kuwaharaTexture.SampleLevel(sourceSampler, input.texCoord, 0.0).rgb;

    pigmentAmount = 1.0;

    return float4(pigmentColor * pigmentAmount, pigmentAmount);
}