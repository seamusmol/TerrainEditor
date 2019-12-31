#define PS_SHADERMODEL ps_4_0

float HeightScale = 0.01f;
float FarPlane = 1000.0f;
float TerrainCoverHeight = 2.0f;
float DepthOffset;

//HeightMap
Texture2D HeightMap;
sampler2D HeightMapSampler = sampler_state
{
	Texture = <HeightMap>;
	mipFilter = Linear;
	magfilter = Linear;
	minfilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};
//DepthMap
Texture2D DepthMap;
sampler2D DepthMapSampler = sampler_state
{
	Texture = <InputTexture>;
	mipFilter = Linear;
	magfilter = Linear;
	minfilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

float4 Color = float4(1, 0, 0, 1);
float4 EnabledColor = float4(1, 0, 0, 1);

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 Coords : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 HeightRGBA = tex2D(HeightMapSampler, input.Coords).rgba;
	int Height = (int(HeightRGBA.r * 255) << 24) + (int(HeightRGBA.g * 255) << 16) + (int(HeightRGBA.b * 255) << 8) + (int(HeightRGBA.a * 255));
	//height relative to depth
	float H = float(Height) * HeightScale + DepthOffset;
	float Depth = FarPlane * tex2D(DepthMapSampler, input.Coords).r;

	float4 OutputColor = float4(0,0,0,0);
	OutputColor.r = H / FarPlane;
	OutputColor.a = 1.0f;

	return OutputColor;
}

technique Technique1
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
