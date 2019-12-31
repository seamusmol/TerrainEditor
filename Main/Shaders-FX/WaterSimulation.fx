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

Texture2D DepthInput;
sampler2D DepthInputSampler = sampler_state
{
	Texture = <DepthInput>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = clamp;
	AddressV = clamp;
};

Texture2D LastFrame;
sampler2D LastFrameSampler = sampler_state
{
	Texture = <LastFrame>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = clamp;
	AddressV = clamp;
};

Texture2D CurrentFrame;
sampler2D CurrentFrameSampler = sampler_state
{
	Texture = <CurrentFrame>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = clamp;
	AddressV = clamp;
};

Texture2D TerrainHeightMap;
sampler2D TerrainHeightSampler = sampler_state
{
	Texture = <TerrainHeightMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = clamp;
	AddressV = clamp;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//get worldTexCoord
	float2 TexCoord = input.Coords;

	float2 Neighbor = float2(1.0f / MaterialWidth, 1.0f / MaterialHeight);

	// Look up all the neighbor heights
	float H11 = tex2D(CurrentFrameSampler, TexCoord);
	float H01 = tex2D(CurrentFrameSampler, TexCoord + Neighbor * float2(-1, 0)).r;
	float H21 = tex2D(CurrentFrameSampler, TexCoord + Neighbor * float2(1, 0)).r;
	float H10 = tex2D(CurrentFrameSampler, TexCoord + Neighbor * float2(0, -1)).r;
	float H12 = tex2D(CurrentFrameSampler, TexCoord + Neighbor * float2(0, 1)).r;
	float PreviousHeight = tex2D(LastFrameSampler, TexCoord).r;
	//half dampening = tex2D(dampeningSampler, i.texCoord);
	float Dampening = 0.25f;

	// Compute the acceleration of the point based upon its neighbors
	float Acceleration = Dampening * 1.0f * (H01 + H21 + H10 + H12 - 4.0 * H11);

	// Do Verlet integration
	float NewHeight = H11 - PreviousHeight * Acceleration;

	return float4(NewHeight, 0, 0, 1.0f);
}

technique Technique1
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
