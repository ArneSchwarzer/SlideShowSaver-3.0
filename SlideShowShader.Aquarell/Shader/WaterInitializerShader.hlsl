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


// ============================================================================
// Einfacher deterministischer Testwert.
//
// KEIN endgültiger Noise-Generator.
//
// Er dient ausschließlich dazu, für den ersten Wasserfluss räumlich
// unterschiedliche Wasserstände bereitzustellen.
// ============================================================================

float CalculateWaterVariation(float2 uv)
{
    float value;

    value = sin(uv.x * 31.0) * sin(uv.y * 23.0);

    value = value * 0.5 + 0.5;

    return value;
}


float PSMain(VertexOutput input) : SV_TARGET
{
    const int cellSize = 128;

    int2 pixelPosition;
    int2 cellPosition;

    float water;

    pixelPosition = int2(input.position.xy);

    cellPosition =
        pixelPosition / cellSize;

    if (((cellPosition.x + cellPosition.y) & 1) == 0)
    {
        water = 0.10;
    }
    else
    {
        water = 0.30;
    }

    return water;
}