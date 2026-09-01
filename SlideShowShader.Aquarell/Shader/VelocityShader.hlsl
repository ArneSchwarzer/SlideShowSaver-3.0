// ============================================================================
// VelocityShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      MODE_UPDATE
//
//          Berechnet aus dem lokalen Druckgradienten eine neue
//          Flieﬂgeschwindigkeit:
//
//              u_neu = u_alt - pressureGradientStrength * grad(p)
//
//          Pressure:
//              hoch im Regionsinneren
//              niedrig an Regionsgrenzen
//
//          Damit zeigt die resultierende Beschleunigung vom hohen Druck
//          in Richtung des niedrigeren Drucks.
//
//
//      MODE_DISPLAY
//
//          Visualisiert das Velocity-Feld im Kontrollmonitor.
//
//          Farbkodierung:
//
//              R = Bewegung nach rechts
//              G = Bewegung nach unten
//              B = Bewegung nach links / oben
//
//          Die Helligkeit entspricht n‰herungsweise dem Betrag der
//          Geschwindigkeit.
//
// ============================================================================


// ============================================================================
// Modi
// ============================================================================

static const uint MODE_UPDATE = 0;
static const uint MODE_DISPLAY = 1;


// ============================================================================
// Constant Buffer
//
// Exakt 16 Byte.
// ============================================================================

cbuffer VelocityConstants : register(b0)
{
    uint mode;

    float pressureGradientStrength;
    float maxVelocity;
    float displayVelocityScale;

    float damping;
    float viscosity;

    float reserve1;
    float reserve2;
};


// ============================================================================
// Eingaben
// ============================================================================

// p
Texture2D<float> pressureTexture : register(t0);

// (u,v)
Texture2D<float2> velocityTexture : register(t1);


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
// Hilfsfunktion
// ============================================================================

int2 ClampPixelPosition(int2 pixelPosition, uint2 textureSize)
{
    int2 maximumPosition;
    
    maximumPosition = int2(int(textureSize.x) - 1, int(textureSize.y) - 1);

    return clamp(pixelPosition, int2(0, 0), maximumPosition);
}


// ============================================================================
// Pixel Shader
// ============================================================================

float4 PSMain(VSOutput input) : SV_TARGET
{
    uint textureWidth;
    uint textureHeight;

    uint2 textureSize;

    int2 pixelPosition;

    float pressureLeft;
    float pressureRight;
    float pressureUp;
    float pressureDown;

    float2 pressureGradient;

    float2 oldVelocity;
    float2 newVelocity;

    float velocityLength;

    float2 displayVelocity;
    float3 displayColor;


    // ========================================================================
    // MODE_UPDATE
    // ========================================================================

    if (mode == MODE_UPDATE)
    {
        pressureTexture.GetDimensions(textureWidth, textureHeight);

        textureSize = uint2(textureWidth, textureHeight);

        pixelPosition = int2(input.position.xy);


        // --------------------------------------------------------------------
        // Druckwerte der vier direkten Nachbarn.
        //
        // Am Texturrand verwenden wir Clamp.
        // --------------------------------------------------------------------

        pressureLeft =  pressureTexture.Load(int3(ClampPixelPosition(pixelPosition + int2(-1, 0), textureSize), 0));
        pressureRight = pressureTexture.Load(int3(ClampPixelPosition(pixelPosition + int2(1, 0), textureSize), 0));
        pressureUp =    pressureTexture.Load(int3(ClampPixelPosition(pixelPosition + int2(0, -1), textureSize), 0));
        pressureDown =  pressureTexture.Load(int3(ClampPixelPosition(pixelPosition + int2(0, 1), textureSize), 0));
        
        // --------------------------------------------------------------------
        // Zentraler Differenzenquotient.
        // --------------------------------------------------------------------

        pressureGradient = float2((pressureRight - pressureLeft) * 0.5F, (pressureDown - pressureUp) * 0.5F);

        oldVelocity = velocityTexture.Load(int3(pixelPosition, 0));


        // --------------------------------------------------------------------
        // Druckbeschleunigung.
        //
        // NEGATIVES Vorzeichen:
        //
        //     Wasser bewegt sich vom hohen zum niedrigeren Druck.
        // --------------------------------------------------------------------

        newVelocity = oldVelocity * damping - pressureGradient * pressureGradientStrength;

        // --------------------------------------------------------------------
        // Notbremse f¸r den ersten Curtis-PoC.
        // --------------------------------------------------------------------

        velocityLength = length(newVelocity);
        
        if (velocityLength > maxVelocity)
        {
            newVelocity = newVelocity / velocityLength * maxVelocity;
        }
        
        return float4(newVelocity, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE_DISPLAY
    // ========================================================================

    if (mode == MODE_DISPLAY)
    {
        velocityTexture.GetDimensions(textureWidth, textureHeight);
        
        pixelPosition = int2(saturate(input.texCoord) * float2(textureWidth - 1, textureHeight - 1));

        newVelocity = velocityTexture.Load(int3(pixelPosition, 0));

        displayVelocity = newVelocity * displayVelocityScale;


        // --------------------------------------------------------------------
        // Farbkodierung:
        //
        // Rechts  -> Rot
        // Unten   -> Gr¸n
        // Links   -> Blau
        // Oben    -> ebenfalls Blauanteil
        //
        // Noch bewusst als technische Diagnoseanzeige.
        // --------------------------------------------------------------------

        displayColor.r = saturate(max(displayVelocity.x, 0.0F));
        displayColor.g = saturate(max(displayVelocity.y, 0.0F));
        displayColor.b = saturate(max(-displayVelocity.x, 0.0F) + max(-displayVelocity.y, 0.0F));
        
        return float4(displayColor, 1.0F);
    }


    // ========================================================================
    // Unbekannter Modus = Gelb
    // ========================================================================

    return float4(1.0F, 1.0F, 0.0F, 1.0F);
}