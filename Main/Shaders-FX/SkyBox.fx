
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

matrix WorldViewProjection;

float3 WorldPosition;

Texture2D DiffuseMap;
sampler2D DiffuseSampler = sampler_state
{
	Texture = <DiffuseMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
	float3 Position : POSITION0;
	float3 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 Position = float4(input.Position, 1);
	Position.xyz += WorldPosition;

	output.Position = mul((Position), WorldViewProjection);
	output.TexCoord = input.Position.xyz;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 Color = tex2D(DiffuseSampler, input.TexCoord);

	return Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};