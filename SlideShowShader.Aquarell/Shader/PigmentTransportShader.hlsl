// ============================================================================
// PigmentTransportShader.hlsl
// ============================================================================
//
// Aufgabe:
//
//      Transportiert die im Wasser suspendierte Pigmentmasse entlang
//      des aktuellen Velocity-Feldes.
//
// Interner Pigmentzustand:
//
//      RGB = premultiplizierte Pigmentfarbmasse
//      A   = Pigmentmasse
//
// Deshalb wird der komplette float4-Zustand konservativ transportiert.
//
// Für V1.0:
//
//      - keine Ablagerung
//      - keine Desorption
//      - keine Pigmentdiffusion
//      - keine papierabhängige Granulation
//
// Zunächst ausschließlich:
//
//      Suspension + Velocity -> neue Suspension
//
// ============================================================================


// ============================================================================
// Constant Buffer
//
// exakt 16 Byte
// ============================================================================

cbuffer PigmentTransportConstants : register(b0)
{
	float timeStep;
	float transportStrength;

	float reserve1;
	float reserve2;
};


// ============================================================================
// Eingaben
// ============================================================================

Texture2D<float4> pigmentTexture : register(t0);
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


float4 ReadPigment(int2 pixelPosition, uint2 textureSize)
{
	return pigmentTexture.Load(int3(ClampPixelPosition(pixelPosition, textureSize), 0));
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

	float4 pigmentCenter;
	float4 pigmentLeft;
	float4 pigmentRight;
	float4 pigmentUp;
	float4 pigmentDown;

	float2 velocityCenter;
	float2 velocityLeft;
	float2 velocityRight;
	float2 velocityUp;
	float2 velocityDown;

	float velocityFaceLeft;
	float velocityFaceRight;
	float velocityFaceUp;
	float velocityFaceDown;

	float4 fluxLeft;
	float4 fluxRight;
	float4 fluxUp;
	float4 fluxDown;

	float4 divergence;
	float4 newPigment;


	pigmentTexture.GetDimensions(textureWidth, textureHeight);
	
	textureSize = uint2(textureWidth, textureHeight);

	pixelPosition = int2(input.position.xy);


    // ========================================================================
    // Pigmentzustand lesen
    // ========================================================================

	pigmentCenter = ReadPigment(pixelPosition, textureSize);
	pigmentLeft =   ReadPigment(pixelPosition + int2(-1, 0), textureSize);
	pigmentRight =  ReadPigment(pixelPosition + int2(1, 0), textureSize);
	pigmentUp =     ReadPigment(pixelPosition + int2(0, -1), textureSize);
	pigmentDown =   ReadPigment(pixelPosition + int2(0, 1), textureSize);


    // ========================================================================
    // Velocity lesen
    // ========================================================================

	velocityCenter = ReadVelocity(pixelPosition, textureSize);
	velocityLeft =   ReadVelocity(pixelPosition + int2(-1, 0), textureSize);
	velocityRight =  ReadVelocity(pixelPosition + int2(1, 0), textureSize);
	velocityUp =     ReadVelocity(pixelPosition + int2(0, -1), textureSize);
	velocityDown =   ReadVelocity(pixelPosition + int2(0, 1), textureSize);


    // ========================================================================
    // Geschwindigkeit an den Zellflächen
    // ========================================================================

	velocityFaceLeft =  0.5F * (velocityLeft.x + velocityCenter.x);
	velocityFaceRight = 0.5F * (velocityCenter.x + velocityRight.x);
	velocityFaceUp =    0.5F * (velocityUp.y + velocityCenter.y);
	velocityFaceDown =  0.5F * (velocityCenter.y + velocityDown.y);


    // ========================================================================
    // Geschlossene Bildgrenzen
    //
    // Pigment darf nicht aus der Simulation hinauslaufen.
    // ========================================================================

	if (pixelPosition.x <= 0)
	{
		velocityFaceLeft = 0.0F;
	}

	if (pixelPosition.x >= int(textureWidth) - 1)
	{
		velocityFaceRight = 0.0F;
	}

	if (pixelPosition.y <= 0)
	{
		velocityFaceUp = 0.0F;
	} 

	if (pixelPosition.y >= int(textureHeight) - 1)
	{
		velocityFaceDown = 0.0F;
	}


    // ========================================================================
    // Transportstärke
    // ========================================================================

	velocityFaceLeft *= transportStrength;
	velocityFaceRight *= transportStrength;
	velocityFaceUp *= transportStrength;
	velocityFaceDown *= transportStrength;


    // ========================================================================
    // Upwind Flux X-
    // ========================================================================

	if (velocityFaceLeft >= 0.0F)
	{
		fluxLeft = velocityFaceLeft * pigmentLeft;
	}
	else
	{
		fluxLeft = velocityFaceLeft * pigmentCenter;
	}


    // ========================================================================
    // Upwind Flux X+
    // ========================================================================

	if (velocityFaceRight >= 0.0F)
	{
		fluxRight = velocityFaceRight * pigmentCenter;
	}
	else
	{
		fluxRight = velocityFaceRight * pigmentRight;
	}


    // ========================================================================
    // Upwind Flux Y-
    // ========================================================================

	if (velocityFaceUp >= 0.0F)
	{
		fluxUp = velocityFaceUp * pigmentUp;
	}
	else
	{
		fluxUp = velocityFaceUp * pigmentCenter;
	}


    // ========================================================================
    // Upwind Flux Y+
    // ========================================================================

	if (velocityFaceDown >= 0.0F)
	{
		fluxDown = velocityFaceDown * pigmentCenter;
	}
	else
	{
		fluxDown = velocityFaceDown * pigmentDown;
	}


    // ========================================================================
    // Divergenz des Pigmentflusses
    // ========================================================================

	divergence = (fluxRight - fluxLeft) + (fluxDown - fluxUp);

	newPigment = pigmentCenter - timeStep * divergence;


    // ========================================================================
    // Numerisches Sicherheitsnetz
    //
    // Pigmentmasse und premultiplizierte Farbmasse dürfen nicht negativ
    // werden.
    //
    // Nach oben wird NICHT begrenzt:
    //
    // Pigment darf sich lokal akkumulieren.
    // ========================================================================

	newPigment = max(newPigment, float4(0.0F, 0.0F, 0.0F, 0.0F));


	return newPigment;
}