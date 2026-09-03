// ============================================================================
// VelocityShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      MODE_UPDATE
//
//          Berechnet aus dem lokalen hydraulischen Gradienten eine neue
//          Fließgeschwindigkeit.
//
//          Bisher:
//
//              u_neu =
//                  u_alt * damping
//                  - pressureGradientStrength * grad(p)
//
//          Jetzt:
//
//              hydraulicHead =
//                  pressure
//                  + paperHeight * paperHeightInfluence
//
//              u_neu =
//                  u_alt * damping
//                  - pressureGradientStrength * grad(hydraulicHead)
//
// ---------------------------------------------------------------------------
// Bedeutung:
//
//      pressure
//          = lokale Wasserhöhe
//
//      paperHeight
//          = lokale Höhe / Topografie des Papiers
//
//      hydraulicHead
//          = effektive Höhenenergie der Wasseroberfläche
//
// Wasser bewegt sich damit nicht mehr nur aufgrund von Unterschieden in der
// Wasserhöhe, sondern reagiert zusätzlich auf Höhen und Senken im Papier.
//
// Hohe Papierstellen:
//
//      erhöhen lokal den hydraulischen Kopf.
//
// Tiefe Papierstellen:
//
//      senken lokal den hydraulischen Kopf.
//
// Dadurch wird Wasser bevorzugt in Vertiefungen und entlang der
// Mikrostruktur des Papiers geführt.
//
// ---------------------------------------------------------------------------
// Noch NICHT enthalten:
//
//      - Absorption
//      - Desorption
//      - PigmentDeposit
//      - papierabhängige Pigmentbindung
//      - Capillary Flow
//
// Diese Effekte werden später auf derselben PaperMap aufbauen.
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
// Exakt 32 Byte.
// ============================================================================

cbuffer VelocityConstants : register(b0)
{
    uint mode;

    float pressureGradientStrength;
    float maxVelocity;
    float displayVelocityScale;

    float damping;
    float viscosity;

    float paperHeightInfluence;
    float reserve1;
};


// ============================================================================
// Eingaben
// ============================================================================

// Wasserhöhe / Pressure
Texture2D<float> pressureTexture : register(t0);

// Velocity (u,v)
Texture2D<float2> velocityTexture : register(t1);

// Papier-Höhenkarte
Texture2D<float> paperTexture : register(t2);


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
// Hilfsfunktionen
// ============================================================================

int2 ClampPixelPosition(int2 pixelPosition, uint2 textureSize)
{
    int2 maximumPosition;


    maximumPosition = int2(int(textureSize.x) - 1, int(textureSize.y) - 1);

    return clamp(pixelPosition, int2(0, 0), maximumPosition);
 }


float ReadHydraulicHead(int2 pixelPosition, uint2 textureSize)
{
    int2 clampedPosition;

    float pressure;
    float paperHeight;

    float hydraulicHead;


    clampedPosition = ClampPixelPosition(pixelPosition, textureSize);


    pressure = pressureTexture.Load(int3(clampedPosition, 0));


    paperHeight = paperTexture.Load(int3(clampedPosition, 0));


    // ------------------------------------------------------------------------
    // Die absolute mittlere Papierhöhe ist unwichtig.
    //
    // Für den Gradienten zählt ausschließlich die lokale Differenz.
    //
    // Deshalb ist keine explizite Zentrierung um 0.5 erforderlich:
    //
    //      grad(p + (h - 0.5) * k)
    //
    // ist identisch zu:
    //
    //      grad(p + h * k)
    //
    // da der Gradient einer Konstanten 0 ist.
    // ------------------------------------------------------------------------

    hydraulicHead = pressure + paperHeight * paperHeightInfluence;


    return hydraulicHead;
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


    float hydraulicHeadLeft;
    float hydraulicHeadRight;
    float hydraulicHeadUp;
    float hydraulicHeadDown;

    float2 hydraulicGradient;


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
        // Hydraulischen Kopf der vier direkten Nachbarn lesen.
        //
        // Dieser enthält:
        //
        //      Wasserhöhe
        //          +
        //      Papierhöhe
        //
        // --------------------------------------------------------------------

        hydraulicHeadLeft =  ReadHydraulicHead(pixelPosition + int2(-1, 0), textureSize);
        hydraulicHeadRight = ReadHydraulicHead(pixelPosition + int2(1, 0), textureSize);
        hydraulicHeadUp =    ReadHydraulicHead(pixelPosition + int2(0, -1), textureSize);
        hydraulicHeadDown =  ReadHydraulicHead(pixelPosition + int2(0, 1), textureSize);


        // --------------------------------------------------------------------
        // Zentraler Differenzenquotient.
        //
        // Nicht mehr nur:
        //
        //      grad(pressure)
        //
        // sondern:
        //
        //      grad(pressure + paperHeight)
        //
        // --------------------------------------------------------------------

        hydraulicGradient = float2((hydraulicHeadRight - hydraulicHeadLeft) * 0.5F,
                                   (hydraulicHeadDown - hydraulicHeadUp) * 0.5F);
        
        oldVelocity = velocityTexture.Load(int3(pixelPosition, 0));


        // --------------------------------------------------------------------
        // Beschleunigung entlang des hydraulischen Gradienten.
        //
        // NEGATIVES Vorzeichen:
        //
        // Wasser bewegt sich vom höheren zum niedrigeren hydraulischen Kopf.
        // --------------------------------------------------------------------

        newVelocity = oldVelocity * damping - hydraulicGradient * pressureGradientStrength;


        // --------------------------------------------------------------------
        // Numerische Notbremse.
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
        
        newVelocity =  velocityTexture.Load(int3(pixelPosition, 0));

        displayVelocity = newVelocity * displayVelocityScale;


        // --------------------------------------------------------------------
        // Farbkodierung:
        //
        // Rechts  -> Rot
        // Unten   -> Grün
        // Links   -> Blau
        // Oben    -> ebenfalls Blauanteil
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