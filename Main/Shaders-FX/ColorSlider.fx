#define PS_SHADERMODEL ps_4_0

float4 Color;
float4 EnabledColor;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 Coords : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 OutputColor = float4(0,0,0,0);
	OutputColor.r += Color.r * (1 - EnabledColor.r) + (input.Coords.x * EnabledColor.r);
	OutputColor.g += Color.g * (1 - EnabledColor.g) + (input.Coords.x * EnabledColor.g);
	OutputColor.b += Color.b * (1 - EnabledColor.b) + (input.Coords.x * EnabledColor.b);
	OutputColor.a += Color.a * (1 - EnabledColor.a) + (input.Coords.x * EnabledColor.a);
	return OutputColor;
}

technique Technique1
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
