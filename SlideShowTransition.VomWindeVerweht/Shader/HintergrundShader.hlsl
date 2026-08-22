Texture2D HintergrundBild : register(t0);
SamplerState HintergrundSampler : register(s0);

struct VSOutput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD0;
};

VSOutput VSMain(uint vertexID : SV_VertexID)
{
    VSOutput output;

    float2 position;
    float2 uv;

    if (vertexID == 0)
    {
        position = float2(-1.0, 1.0);
        uv = float2(0.0, 0.0);
    }
    else if (vertexID == 1)
    {
        position = float2(3.0, 1.0);
        uv = float2(2.0, 0.0);
    }
    else
    {
        position = float2(-1.0, -3.0);
        uv = float2(0.0, 2.0);
    }

    output.position = float4(position, 0.0, 1.0);

    output.uv = uv;

    return output;
}

float4 PSMain(VSOutput input) : SV_TARGET
{
    return HintergrundBild.Sample(HintergrundSampler, input.uv);
}