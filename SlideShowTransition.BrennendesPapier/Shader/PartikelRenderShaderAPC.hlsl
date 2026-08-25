// PartikelRenderShaderAPC.hlsl
//
// V0.1
//
// Partikel-RenderShader mit Active Particle Compaction (APC)
//
// - Position kommt aus StructuredBuffer<Particle>
// - Größe kommt aus StructuredBuffer<Particle>
// - Seitenverhältnis wird korrigiert
//
// Compile Targets:
// VSMain -> vs_5_0
// PSMain -> ps_5_0

#define PARTICLE_COUNT 8192
#define VERTICES_PER_PARTICLE 6


struct Particle
{
    float2 Position;
    float2 Velocity;

    float Age;
    float Lifetime;

    float Size;

    uint Seed;
};


struct VSOutput
{
    float4 position : SV_Position;
    float lifeProgress : TEXCOORD0;
};


cbuffer ParticleRenderParameter : register(b0)
{
    // renderHoehe / renderBreite
    //
    // Beispiel 3840 x 2160:
    // 2160 / 3840 = 0.5625
    float AspectCorrection;

    float Padding1;
    float Padding2;
    float Padding3;
};


StructuredBuffer<Particle> Particles : register(t0);
StructuredBuffer<uint> RenderPartikelIndices : register(t1);
Texture2D<float4> ParticleGradient : register(t0);
SamplerState ParticleGradientSampler : register(s0);

VSOutput VSMain(uint vertexId : SV_VertexID, uint instanceId : SV_InstanceID)
{
    VSOutput output;

    uint particleIndex;
    uint cornerIndex;

    Particle particle;

    float2 centerClip;
    float2 corner;
    float2 halfSizeClip;
    float2 positionClip;

    particleIndex = RenderPartikelIndices[instanceId];

    cornerIndex = vertexId;
       

    particle = Particles[particleIndex];

    if (particle.Lifetime <= 0.0)
    {
        output.position = float4(-2.0, -2.0, 0.0, 1.0);

        output.lifeProgress = 1.0;

        return output;
    }

    output.lifeProgress = saturate(particle.Age / max(particle.Lifetime, 0.0001));


    // -----------------------------------------
    // Partikelposition
    // -----------------------------------------
    //
    // Unser Buffer:
    //
    // (0,0) = links oben
    // (1,1) = rechts unten
    //
    // D3D Clip Space:
    //
    // (-1,+1) = links oben
    // (+1,-1) = rechts unten

    centerClip.x =
        particle.Position.x * 2.0 - 1.0;

    centerClip.y =
        1.0 - particle.Position.y * 2.0;


    // -----------------------------------------
    // Partikelgröße
    // -----------------------------------------
    //
    // Size ist auf die Bildschirmhöhe bezogen.
    //
    // Wegen des nichtquadratischen RenderTargets
    // muss die X-Ausdehnung korrigiert werden,
    // damit das Ergebnis in echten Pixeln
    // quadratisch wird.

    halfSizeClip.y =
        particle.Size;

    halfSizeClip.x =
        particle.Size * AspectCorrection;


    // -----------------------------------------
    // Zwei Dreiecke = ein Quad
    // -----------------------------------------
    //
    // A -------- B
    // |        / |
    // |      /   |
    // |    /     |
    // |  /       |
    // |/         |
    // C -------- D
    //
    // Triangle 1:
    // A, B, C
    //
    // Triangle 2:
    // C, B, D

    if (cornerIndex == 0)
    {
        // A
        corner =
            float2(
                -1.0,
                 1.0);
    }
    else if (cornerIndex == 1)
    {
        // B
        corner =
            float2(
                 1.0,
                 1.0);
    }
    else if (cornerIndex == 2)
    {
        // C
        corner =
            float2(
                -1.0,
                -1.0);
    }
    else if (cornerIndex == 3)
    {
        // C
        corner =
            float2(
                -1.0,
                -1.0);
    }
    else if (cornerIndex == 4)
    {
        // B
        corner =
            float2(
                 1.0,
                 1.0);
    }
    else
    {
        // D
        corner =
            float2(
                 1.0,
                -1.0);
    }


    positionClip =
        centerClip +
        float2(
            corner.x * halfSizeClip.x,
            corner.y * halfSizeClip.y);


    output.position =
        float4(
            positionClip,
            0.0,
            1.0);

    return output;
}


float4 PSMain(VSOutput input) : SV_Target
{
    float2 gradientUV;

    gradientUV = float2(saturate(input.lifeProgress), 0.5);

    return ParticleGradient.Sample(ParticleGradientSampler, gradientUV);
}