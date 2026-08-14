sampler2D oldImageSampler : register(s0);
sampler2D newImageSampler : register(s1);
sampler2D maskSampler : register(s2);

float progress : register(c0);

float4 main(float2 uv : TEXCOORD0) : COLOR
{
    float4 oldColor;
    float4 newColor;
    float maskValue;
    float reveal;

    oldColor = tex2D(oldImageSampler, uv);
    newColor = tex2D(newImageSampler, uv);

    maskValue = tex2D(maskSampler, uv).r;

    reveal = step(maskValue, progress);

    return lerp(oldColor, newColor, reveal);
}