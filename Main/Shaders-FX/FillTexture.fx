#define PS_SHADERMODEL ps_4_0

float Value;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 Coords : TEXCOORD0;
};

float4 SetPixel(VertexShaderOutput input) : COLOR
{
	//get current 
	return float4(Value,0,0,1.0f);
}

technique Technique1
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL SetPixel();
	}
}
