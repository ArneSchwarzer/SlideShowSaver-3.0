// ============================================================================
// PressureInitializerShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Erzeugt aus der RegionDistanceMap den initialen Wasserdruck p.
//
//      RegionDistance:
//          0       = Regionsgrenze
//          größer  = weiter im Inneren einer Region
//
//      Pressure:
//          0       = kein Druck / keine Wasserhöhe an der Grenze
//          1       = maximale initiale Wasserhöhe
//
// Für V1.0 zunächst bewusst simpel:
//
//      p = saturate(distance / pressureDistanceScale)
//
// Späterer Anflanschpunkt:
//
//      - PaperMap
//      - lokale Wassermenge
//      - zufällige Wash-Variation
//      - ggf. nichtlineare Druckkurve
//
// ============================================================================


// ============================================================================
// Constant Buffer
// ============================================================================
//
// Exakt 16 Byte.
// ============================================================================

cbuffer PressureInitializerConstants : register(b0)
{
    float pressureDistanceScale;

    float reserve1;
    float reserve2;
    float reserve3;
};


// ============================================================================
// Eingaben
// ============================================================================

Texture2D<float> regionDistanceTexture : register(t0);


// ============================================================================
// Vertex Shader
// ============================================================================

struct VSOutput
{
    float4 position : SV_POSITION;
    float2 texCoord : TEXCOORD0;
};


VSOutput VSMain(uint vertexId : SV_VertexID)
{
    VSOutput output;

    float2 position;
    float2 texCoord;


    if (vertexId == 0)
    {
        position = float2(-1.0F, -1.0F);
        texCoord = float2(0.0F, 1.0F);
    }
    else if (vertexId == 1)
    {
        position = float2(-1.0F, 3.0F);
        texCoord = float2(0.0F, -1.0F);
    }
    else
    {
        position = float2(3.0F, -1.0F);
        texCoord = float2(2.0F, 1.0F);
    }


    output.position = float4(position, 0.0F, 1.0F);

    output.texCoord = texCoord;


    return output;
}


// ============================================================================
// Pixel Shader
// ============================================================================

float PSMain(VSOutput input) : SV_TARGET
{
    int2 pixelPosition;

    float regionDistance;
    float pressure;


    pixelPosition = int2(input.position.xy);
    
    regionDistance = regionDistanceTexture.Load(int3(pixelPosition, 0));


    // ------------------------------------------------------------------------
    // Sicherheitsnetz.
    //
    // RegionDistance sollte regulär niemals negativ sein.
    // ------------------------------------------------------------------------

    regionDistance = max(regionDistance, 0.0F);


    // ------------------------------------------------------------------------
    // Distanz -> initialer Wasserdruck.
    //
    // Noch bewusst linear.
    // ------------------------------------------------------------------------

    pressure = saturate(regionDistance / max(pressureDistanceScale, 1.0F));


    return pressure;
}