float CenterX : register(c0);
float CenterY : register(c1);
float Scale : register(c2);
float MaxIterations : register(c3);
float ViewportWidth : register(c4);
float ViewportHeight : register(c5);
float GradientOffset : register(c6);
float GradientIndex : register(c7);

float3 PaletteRainbow(float t)
{
	float r = 0.5 + 0.5 * cos(6.28318 * (t + 0.00));
	float g = 0.5 + 0.5 * cos(6.28318 * (t + 0.33));
	float b = 0.5 + 0.5 * cos(6.28318 * (t + 0.67));
	return float3(r, g, b);
}

float3 PaletteZebra(float t)
{
	float v = step(0.5, frac(t * 20.0));
	return float3(v, v, v);
}

float3 PaletteJoker(float t)
{
	float r = 0.5 + 0.5 * sin(6.28318 * t);
	float g = 0.5 + 0.5 * sin(6.28318 * (t + 0.25));
	float b = 0.8 + 0.2 * sin(6.28318 * (t + 0.5));
	return float3(r, g, b);
}

float3 GetPaletteColor(float t)
{
	t = frac(t + GradientOffset);

	if (GradientIndex < 0.5)
		return PaletteRainbow(t);

	if (GradientIndex < 1.5)
		return PaletteZebra(t);

	return PaletteJoker(t);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float aspect = ViewportWidth / max(ViewportHeight, 1.0);

	float x = (uv.x - 0.5) * Scale * aspect + CenterX;
	float y = (uv.y - 0.5) * Scale + CenterY;

	float zx = 0.0;
	float zy = 0.0;

	float iteration = 0.0;

	for (int i = 0; i < 1000; i++)
	{
		if (i >= MaxIterations)
			break;

		float zx2 = zx * zx - zy * zy + x;
		float zy2 = 2.0 * zx * zy + y;

		zx = zx2;
		zy = zy2;

		if ((zx * zx + zy * zy) > 4.0)
		{
			iteration = i;
			break;
		}
	}

	if (iteration <= 0.0)
		return float4(0.0, 0.0, 0.0, 1.0);

	float t = iteration / MaxIterations;
	float3 color = GetPaletteColor(t);

	return float4(color, 1.0);
}