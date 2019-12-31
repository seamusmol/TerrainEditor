
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

matrix WorldViewProjection;
float3 Position;
float ClipDirection;
float ClipOffset;

float3 ClipMapPosition;
float4 ClipMapRange;
float TerrainScale = 0.25f;
float TexSize = 0.0625f;
int TerrainWidth = 32;
int TerrainHeight = 32;

float HeightScale = 0.5f;

int IsOrthogonal = 0;

float FarPlane = 1000.0f;
//materials per quad

static const float2 TriangleVertices[] =
{
	float2(0,0), float2(1,0), float2(0,1), float2(1,1)
};

static const int Indices[] =
{
	3,1,0, 0,2,3
};

Texture2D ClipPlaneMap;
sampler2D ClipPlaneSampler = sampler_state
{
	Texture = <ClipPlaneMap>;
	magfilter = Linear;
	minfilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

Texture2D HeightMap;
sampler2D HeightMapSampler = sampler_state
{
	Texture = <HeightMap>;
	magfilter = Point;
	minfilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	uint VertexID : SV_VertexID;
};

struct VertexShaderOutput
{
	float4 ScreenSpacePosition : POSITION0;
	float3 WorldSpacePosition : TEXCOORD0;
	float DepthZ : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	uint VID = input.VertexID / 6;
	float4 WorldPos = float4(VID / TerrainHeight, (VID % TerrainHeight), 0, 1);
	WorldPos.xy += TriangleVertices[Indices[input.VertexID % 6]];


	float4 HeightRGBA = tex2Dlod(HeightMapSampler, float4(WorldPos.x / TerrainWidth, WorldPos.y / TerrainHeight, 0, 0)).rgba;
	int Height = (int(HeightRGBA.r * 255) << 24) + (int(HeightRGBA.g * 255) << 16) + (int(HeightRGBA.b * 255) << 8) + (int(HeightRGBA.a * 255));

	WorldPos.xy *= TerrainScale;
	WorldPos.xyz += Position.xyz;
	WorldPos.z += Height * HeightScale;

	float4 ScreenSpacePosition = mul(WorldPos, WorldViewProjection);

	output.ScreenSpacePosition = ScreenSpacePosition;
	output.WorldSpacePosition = WorldPos;
	output.DepthZ = lerp(ScreenSpacePosition.z / FarPlane, ScreenSpacePosition.z, IsOrthogonal);
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 WorldPosition = input.WorldSpacePosition;

	float WPX = (WorldPosition.x - Position.x) / (TerrainWidth * TerrainScale);
	float WPY = (WorldPosition.y - Position.y) / (TerrainHeight * TerrainScale);
	float4 ClipHeightRGBA = tex2D(ClipPlaneSampler, float2(WPX, WPY));

	int ClipHeight = (int(ClipHeightRGBA.x * 255.0) << 24) + (int(ClipHeightRGBA.y * 255.0) << 16) + (int(ClipHeightRGBA.z * 255.0) << 8) + int(ClipHeightRGBA.w * 255.0);
	ClipHeight *= ClipMapRange.z;
	ClipHeight -= ClipMapRange.w;

	if (ClipDirection == -1)
	{
		if (WorldPosition.z > ClipHeight + ClipOffset)
		{
			discard;
		}
	}

	if (ClipDirection == 1)
	{
		if (WorldPosition.z < ClipHeight - ClipOffset)
		{
			discard;
		}
	}
	return float4(input.DepthZ, input.DepthZ, input.DepthZ, 1.0f);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}



};