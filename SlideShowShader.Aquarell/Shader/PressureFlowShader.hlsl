// ============================================================================
// PressureFlowShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Transportiert den Wasserdruck / die Wasserhöhe p entlang
//      des Velocity-Feldes.
//
//      Verwendet einen einfachen konservativen Upwind-Flux.
//
//      p_neu = p_alt - dt * div(p * u)
//
// Dadurch wird nicht bloß die PressureMap verschoben, sondern Wasser
// verlässt Zellen und kommt in Nachbarzellen wieder an.
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
// exakt 16 Byte
// ============================================================================

cbuffer PressureFlowConstants : register(b0)
{
    uint mode;

    float timeStep;
    float maxPressure;
    float displayPressureScale;
};


// ============================================================================
// Eingaben
// ============================================================================

Texture2D<float> pressureTexture : register(t0);
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
// Hilfsfunktionen
// ============================================================================

int2 ClampPixelPosition(int2 pixelPosition, uint2 textureSize)
{
    return clamp(pixelPosition, int2(0, 0), int2(int(textureSize.x) - 1, int(textureSize.y) - 1));
}


float ReadPressure(int2 pixelPosition, uint2 textureSize)
{
    return pressureTexture.Load(int3(ClampPixelPosition(pixelPosition, textureSize), 0));
}


float2 ReadVelocity(int2 pixelPosition, uint2 textureSize)
{
    return velocityTexture.Load(int3(ClampPixelPosition(pixelPosition, textureSize), 0));
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

    float pressureCenter;
    float pressureLeft;
    float pressureRight;
    float pressureUp;
    float pressureDown;

    float2 velocityCenter;
    float2 velocityLeft;
    float2 velocityRight;
    float2 velocityUp;
    float2 velocityDown;

    float velocityFaceLeft;
    float velocityFaceRight;
    float velocityFaceUp;
    float velocityFaceDown;

    float fluxLeft;
    float fluxRight;
    float fluxUp;
    float fluxDown;

    float divergence;
    float newPressure;


    // ========================================================================
    // MODE_UPDATE
    // ========================================================================

    if (mode == MODE_UPDATE)
    {
        pressureTexture.GetDimensions(textureWidth, textureHeight);

        textureSize = uint2(textureWidth, textureHeight);
        
        pixelPosition = int2(input.position.xy);


        // --------------------------------------------------------------------
        // Zustand des aktuellen Pixels und seiner vier Nachbarn
        // --------------------------------------------------------------------

        pressureCenter = ReadPressure(pixelPosition, textureSize);
        pressureLeft =   ReadPressure(pixelPosition + int2(-1, 0), textureSize);
        pressureRight =  ReadPressure(pixelPosition + int2(1, 0), textureSize);
        pressureUp =     ReadPressure(pixelPosition + int2(0, -1), textureSize);
        pressureDown =   ReadPressure(pixelPosition + int2(0, 1), textureSize);


        velocityCenter = ReadVelocity(pixelPosition, textureSize);
        velocityLeft =   ReadVelocity(pixelPosition + int2(-1, 0), textureSize);
        velocityRight =  ReadVelocity(pixelPosition + int2(1, 0), textureSize);
        velocityUp =     ReadVelocity(pixelPosition + int2(0, -1), textureSize);
        velocityDown =   ReadVelocity(pixelPosition + int2(0, 1), textureSize);

        // --------------------------------------------------------------------
        // Geschwindigkeit an den Zellgrenzen
        // --------------------------------------------------------------------

        velocityFaceLeft =  0.5F * (velocityLeft.x + velocityCenter.x);
        velocityFaceRight = 0.5F * (velocityCenter.x + velocityRight.x);
        velocityFaceUp =    0.5F * (velocityUp.y + velocityCenter.y);
        velocityFaceDown =  0.5F * (velocityCenter.y + velocityDown.y);


        // --------------------------------------------------------------------
        // Upwind-Flux:
        //
        // Je nach Fließrichtung liefert diejenige Zelle den Druck,
        // aus der das Wasser tatsächlich kommt.
        // --------------------------------------------------------------------

        if (velocityFaceLeft >= 0.0F)
        {
            fluxLeft = velocityFaceLeft * pressureLeft;
        }
        else
        {
            fluxLeft = velocityFaceLeft * pressureCenter;
        }


        if (velocityFaceRight >= 0.0F)
        {
            fluxRight = velocityFaceRight * pressureCenter;
        }
        else
        {
            fluxRight = velocityFaceRight * pressureRight;
        }


        if (velocityFaceUp >= 0.0F)
        {
            fluxUp = velocityFaceUp * pressureUp;
        }
        else
        {
            fluxUp = velocityFaceUp * pressureCenter;
        }


        if (velocityFaceDown >= 0.0F)
        {
            fluxDown = velocityFaceDown * pressureCenter;
        }
        else
        {
            fluxDown = velocityFaceDown * pressureDown;
        }


        // --------------------------------------------------------------------
        // Divergenz des Wasserflusses
        // --------------------------------------------------------------------

        divergence = (fluxRight - fluxLeft) + (fluxDown - fluxUp);

        newPressure = pressureCenter - timeStep * divergence;


        // --------------------------------------------------------------------
        // Sicherheitsnetze für den ersten PoC.
        //
        // Negative Wasserhöhe ist unmöglich.
        //
        // maxPressure verhindert bei einem numerischen Ausreißer, dass uns
        // ein einzelner Pixel die gesamte Simulation zerlegt.
        // --------------------------------------------------------------------

        newPressure = clamp(newPressure, 0.0F, maxPressure);

        return float4(newPressure, 0.0F, 0.0F, 1.0F);
    }


    // ========================================================================
    // MODE_DISPLAY
    // ========================================================================

    if (mode == MODE_DISPLAY)
    {
        pressureTexture.GetDimensions(textureWidth, textureHeight);
        
        pixelPosition = int2(saturate(input.texCoord) * float2(textureWidth - 1, textureHeight - 1));
        
        pressureCenter = pressureTexture.Load(int3(pixelPosition, 0));
        
        pressureCenter = saturate(pressureCenter * displayPressureScale);

        return float4(pressureCenter, pressureCenter, pressureCenter, 1.0F);
    }


    // ========================================================================
    // unbekannter Modus
    // ========================================================================

    return float4(1.0F, 1.0F, 0.0F, 1.0F);
}