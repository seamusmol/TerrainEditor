#define PS_SHADERMODEL ps_4_0

float4 Color;
float4 EnabledColor;

float3 Position;
float TexSize = 0.0625f;

int MaterialWidth = 0;
int MaterialHeight = 0;

float ChunkWidth = 32;
float ChunkLength = 32;

float TerrainHeightScale = 0.1f;


struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 Coords : TEXCOORD0;
};

Texture2D InputMap;
sampler2D InputSampler = sampler_state
{
	Texture = <DepthInput>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = wrap;
	AddressV = wrap;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//get worldTexCoord
	float2 TexCoord = input.Coords;

	float2 Neighbor = float2(1.0f / MaterialWidth, 1.0f / MaterialHeight);

	// Look up all the neighbor heights
	float H11 = tex2D(InputSampler, TexCoord).r;
	float H21 = tex2D(InputSampler, TexCoord + Neighbor * float2(1, 0)).r;
	float H12 = tex2D(InputSampler, TexCoord + Neighbor * float2(0, 1)).r;

	float3 Normal = normalize(cross(float3(ChunkWidth, 0, H21 - H11) , float3(0.0, ChunkLength, H12 - H11)));

	return float4(Normal,1.0f);
}

technique Technique1
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
