Texture2D maskTexture : register(t0);
Texture2D gradientTexture : register(t1);

SamplerState sourceSampler : register(s0);

cbuffer GradientenParameter : register(b0)
{
    float progress;
    float brandkantenBreite;

    float padding1;
    float padding2;
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
    float maskValue;
    float frontDistance;
    float gradientPosition;

    float4 gradientColor;

    maskValue =
        maskTexture.Sample(
            sourceSampler,
            input.texCoord).r;

    /*
        frontDistance beschreibt, wie weit die Brandfront diesen Pixel
        zeitlich bereits ueberschritten hat.

        < 0:
            Brandfront hat den Pixel noch nicht erreicht.

        = 0:
            Pixel liegt exakt auf der aktuellen Reveal-Grenze.

        > 0:
            Brandfront ist bereits ueber diesen Pixel hinweggewandert.
    */
    frontDistance = maskValue - progress;

    /*
        Ausserhalb der aktiven Brandkante zeichnet dieser Pass ueberhaupt
        nichts. RevealShader darunter bleibt dadurch unveraendert sichtbar.
    */
    if (frontDistance < 0.0 ||
        frontDistance > brandkantenBreite)
    {
        return float4(
            0.0,
            0.0,
            0.0,
            0.0);
    }

    /*
        Der Gradient laeuft raeumlich:

            intaktes Papier
                  |
            dunkel / verkohlt       Gradient 0.0
                  |
            rot / orange / Effektfarbe
                  |
            weiss / heiss             Gradient 1.0
                  |
            Material verschwindet

        Direkt an der Reveal-Grenze muss deshalb GradientPosition 1.0
        gelten. Am hinteren Rand der Brandkante gilt 0.0.
    */
    gradientPosition =
        1.0 -
        saturate(
            frontDistance /
            brandkantenBreite);

    gradientColor =
        gradientTexture.Sample(
            sourceSampler,
            float2(
                gradientPosition,
                0.5));

    return gradientColor;
}