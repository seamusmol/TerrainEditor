
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

matrix WorldViewProjection;
float ClipDirection = 0.0f;
float ClipOffset;

float3 AmbientLightColor = float3(0.75f, 0.75f, 0.75f);
float3 LightColor = float3(0.788f, 0.886f, 1.0f);

float3 CameraPosition;
float3 LightPosition;

float3 ClipMapPosition;
float4 ClipMapRange;

float3 Position;
float TerrainScale = 0.25f;
float TextureSize = 0.0625f;
int TerrainWidth = 32;
int TerrainHeight = 32;
float HeightScale = 0.05f;
float TerrainDepth = 100;
float DepthFarPlane = 200;

int MaterialWidth = 0;
int MaterialHeight = 0;

float3 BrushPosition;
float4 BrushHighLightColor;
float BrushDistance;
int BrushShape;

static const float PI = 3.141592f;
static const float Epsilon = 0.00001f;
static const float OneEpsilon = 1.00001f;

float4x4 RotationMatrix;

static const float2 TriangleVertices[] =
{
	float2(0,0), float2(1,0), float2(0,1), float2(1,1)
};

static const int Indices[] =
{
	3,1,0, 0,2,3
};

static const int NormalBiasIndex[] =
{
	0,3,2, 2,1,0
};

Texture2D HeightMap;
sampler2D HeightMapSampler = sampler_state
{
	Texture = <HeightMap>;
	mipFilter = Point;
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
	float4 Position : POSITION0;
	float4 ScreenSpacePosition : TEXCOORD0;
	float3 WorldSpacePosition : TEXCOORD1;
	float3 Normal : TEXCOORD2;
	float3x3 TangentBasis : TBASIS;
};

void GenerateTanBiTan(float3 Normal, inout float3 Tangent, inout float3 Bitangent)
{
	float3 C1 = cross(Normal, float3(0.0, 0.0, 1.0));
	float3 C2 = cross(Normal, float3(0.0, 1.0, 0.0));
	if (length(C1) > length(C2))
	{
		Tangent = C1;
	}
	else
	{
		Tangent = C2;
	}
	Tangent = normalize(Tangent);
	Bitangent = normalize(cross(Normal, Tangent));
}

//Normalized Float based height map
float GetBilinearHeight(int VertexID, inout float3 Normal)
{
	float4 FloorPos = float4(VertexID / 6 / TerrainHeight, (VertexID / 6 % TerrainHeight), 0, 1);
	float2 Pos = TriangleVertices[Indices[VertexID % 6]];

	float2 UV = FloorPos + Pos;

	float2 TexSize = float2(TerrainWidth, TerrainHeight);

	float H01 = tex2Dlod(HeightMapSampler, float4((UV.xy + float2(-1.0f, 0)) / TexSize.xy, 0, 0)).r * DepthFarPlane;
	float H10 = tex2Dlod(HeightMapSampler, float4((UV.xy + float2(0, -1.0f)) / TexSize.xy, 0, 0)).r * DepthFarPlane;
	float H11 = tex2Dlod(HeightMapSampler, float4(UV.xy / TexSize.xy, 0, 0)).r * DepthFarPlane;
	float H21 = tex2Dlod(HeightMapSampler, float4((UV.xy + float2(1.0f, 0)) / TexSize.xy, 0, 0)).r * DepthFarPlane;
	float H12 = tex2Dlod(HeightMapSampler, float4((UV.xy + float2(0, 1.0f)) / TexSize.xy, 0, 0)).r * DepthFarPlane;

	float3 P01 = float3(-1.0f, 0.0f, H01);
	float3 P10 = float3(0.0f, -1.0f, H10);
	float3 P11 = float3(0.0f, 0.0f, H11);
	float3 P21 = float3(1.0f, 0.0f, H21);
	float3 P12 = float3(0.0f, 1.0f, H12);

	float3 Normals[] =
	{
		normalize(-cross(P10 - P11, P01 - P11)),
		normalize(-cross(P21 - P11, P10 - P11)),
		normalize(-cross(P12 - P11, P21 - P11)),
		normalize(-cross(P01 - P11, P12 - P11))
	};

	Normal = (Normals[0] + Normals[1] + Normals[2] + Normals[3]) / 4.0f;
	Normal = (Normals[NormalBiasIndex[VertexID % 6]] + Normal) / 2.0f;

	return H11 - TerrainDepth;
}

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	uint VID = input.VertexID / 6;

	float4 WorldPos = float4(VID / TerrainHeight, (VID % TerrainHeight), 0, 1);

	float3 Normal = float3(0, 0, 0);
	float H = GetBilinearHeight(input.VertexID, Normal);

	WorldPos.xy += TriangleVertices[Indices[input.VertexID % 6]];
	WorldPos.xy *= TerrainScale;
	WorldPos.z += H;
	WorldPos.xyz += Position.xyz;

	float3 Tangent = float3(0, 0, 0);
	float3 BiTangent = float3(0, 0, 0);
	GenerateTanBiTan(Normal, Tangent, BiTangent);

	float3x3 TBN = float3x3(Tangent, BiTangent, Normal);
	output.TangentBasis = transpose(TBN);
	float4 ScreenSpacePosition = mul(WorldPos, WorldViewProjection);

	output.Position = ScreenSpacePosition;
	output.WorldSpacePosition = WorldPos.xyz;
	output.ScreenSpacePosition = ScreenSpacePosition;
	output.Normal = Normal;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 WorldPosition = input.WorldSpacePosition;

	return float4(1.0f,0,0, 1.0f);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
