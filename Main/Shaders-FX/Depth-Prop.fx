
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

matrix WorldViewProjection;
float ClipDirection;
float ClipOffset;

float3 ClipMapPosition;
float4 ClipMapRange;

float HeightScale = 0.25f;
float TerrainScale = 0.25f;
float TexSize = 0.0625f;
int TerrainWidth = 32;
int TerrainHeight = 32;

int IsOrthogonal = 0;

float FarPlane = 1000.0f;

Texture2D ClipPlaneMap;
sampler2D ClipPlaneSampler = sampler_state
{
	Texture = <ClipPlaneMap>;
	magfilter = Linear;
	minfilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexInstanceShaderInput
{
	float3 InstancePosition : NORMAL0;
	float3 InstanceForward : NORMAL1;
	float3 InstanceUp : NORMAL2;
	float3 InstanceLeft : NORMAL3;
	float3 Scale : NORMAL4;
};

struct VertexShaderInput
{
	float3 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 ScreenSpacePosition : POSITION0;
	float3 WorldSpacePosition : TEXCOORD0;
	float DepthZ : TEXCOORD1;
};

VertexShaderOutput MainVS(in VertexShaderInput input, VertexInstanceShaderInput instanceInput)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 InputPosition = float4(input.Position.xyz * instanceInput.Scale.xyz, 1.0f);
	float4 InstancePosition = float4(instanceInput.InstancePosition.xyz, 1.0f);

	float4x4 World;
	World[0] = float4(instanceInput.InstanceForward, 0.0f);
	World[1] = float4(instanceInput.InstanceUp, 0.0f);
	World[2] = float4(instanceInput.InstanceLeft, 0.0f);
	World[3] = float4(InstancePosition);

	float4 WorldSpacePosition = mul(InputPosition, World) + float4(InstancePosition.xyz, 1.0f);
	float4 ScreenSpacePosition = mul(WorldSpacePosition, WorldViewProjection);

	output.ScreenSpacePosition = ScreenSpacePosition;
	output.WorldSpacePosition = WorldSpacePosition;
	output.DepthZ = lerp(ScreenSpacePosition.z / FarPlane, ScreenSpacePosition.z, IsOrthogonal);
	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
	float3 WorldPosition = input.WorldSpacePosition;

	float WPX = (WorldPosition.x - ClipMapPosition.y) / ClipMapRange.x;
	float WPY = (WorldPosition.y - ClipMapPosition.x) / ClipMapRange.y;

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

	return float4(input.DepthZ, input.DepthZ, input.DepthZ,1.0f);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};