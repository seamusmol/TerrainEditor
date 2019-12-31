#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

matrix WorldViewProjection;
float3 Position;

float TerrainScale;
int TerrainWidth;
int TerrainLength;
float HeightScale = 0.25f;
float TerrainDepth;
float TexSize = 16.0f;
float Time;
float WaterSpeed = 1.0f;
float WaterRepeatTime = 1.0f;

float NearPlane = 0.1f;
float FarPlane = 1000.0f;

float HeightNearPlane = 0.0f;
float HeightFarPlane = 1000.0f;

int DataWidth = 0;
int DataHeight = 0;

float3 CameraPosition;
float3 LightPosition;

float3 AmbientLightColor = float3(0.75f, 0.75f, 0.75f);
float3 LightColor = float3(0.788f, 0.886f, 1.0f);

float4 ReflectionColor = float4(1.0f, 1.0f, 1.0f, 1.0f);
float4 DiffuseColor = float4(1.0f, 1.0f, 1.0f, 1.0f);
float ShineDamper = 20.0;
float Reflectivity = 0.6;
float MinSpecular = 0.0f;

float WaveHeight = 0.25f;

float3 BrushPosition;
float4 BrushHighLightColor;
float BrushDistance;
int BrushShape;

static const float PI = 3.141592f;
static const float Epsilon = 0.00001f;

static const float2 TriangleVertices[] =
{
	float2(0,0), float2(1,0), float2(0,1), float2(1,1),float2(0.5,0.5)
};

static const int Indices[] =
{
	4,1,0, 4,3,1, 4,2,3, 4,0,2
};

static const float2 TriangleTexCoords[] =
{
	float2(0,0), float2(0.0625f,0), float2(0, 0.0625f), float2(0.0625f,0.0625f)
};

Texture2D WaterDataMap0;
sampler2D WaterDataSampler0 = sampler_state
{
	Texture = <WaterDataMap0>;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

Texture2D WaterDataMap1;
sampler2D WaterDataSampler1 = sampler_state
{
	Texture = <WaterDataMap1>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D WaveDataMap0;
sampler2D WaveDataSampler0 = sampler_state
{
	Texture = <WaveDataMap0>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

Texture2D ColorDataMap0;
sampler2D ColorDataSampler0 = sampler_state
{
	Texture = <ColorDataMap0>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

Texture2D ColorDataMap1;
sampler2D ColorDataSampler1 = sampler_state
{
	Texture = <ColorDataMap1>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D ReflectionMap;
sampler ReflectionSampler = sampler_state
{
	Texture = <ReflectionMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D RefractionMap;
sampler RefractionSampler = sampler_state
{
	Texture = <RefractionMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D DepthMap;
sampler DepthSampler = sampler_state
{
	Texture = <DepthMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D WaveMap;
sampler2D WaveSampler = sampler_state
{
	Texture = (WaveMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D NoiseMap;
sampler2D NoiseSampler = sampler_state
{
	Texture = (NoiseMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D WaveMap2;
sampler2D WaveSampler2 = sampler_state
{
	Texture = (WaveMap2);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D FoamMap;
sampler2D FoamSampler = sampler_state
{
	Texture = (FoamMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D WaterHeightMap;
sampler2D WaterHeightSampler = sampler_state
{
	Texture = (WaterHeightMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = clamp;
	AddressV = clamp;
};

Texture2D NormalMap;
sampler2D NormalMapSampler = sampler_state
{
	Texture = (NormalMap);
	MinFilter = Linear;
	MagFilter = Linear;
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
	float3 WorldSpacePosition : TEXCOORD0;
	float4 ScreenSpacePosition : TEXCOORD1;
	float3 ToCameraVector : TEXCOORD2;
	float3 FromLightVector: TEXCOORD3;
	float TerrainHeight : TEXCOORD4;
	float DepthZ : TEXCOORD5;
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

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	uint VID = input.VertexID / 12;

	float4 WorldPos = float4(VID / TerrainLength, (VID % TerrainLength), 0, 1);

	WorldPos.xy += TriangleVertices[Indices[input.VertexID % 12]];
	WorldPos.xy *= TerrainScale;
	WorldPos.xyz += Position.xyz;

	float PX = (WorldPos.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPos.y - Position.y) / (TerrainLength * TerrainScale);

	float H11RGBA = tex2Dlod(WaterHeightSampler, float4(PX, PY, 0, 0)).r;
	float H21RGBA = tex2Dlod(WaterHeightSampler, float4(PX + 1.0f / TerrainWidth, PY, 0, 0)).r;
	float H12RGBA = tex2Dlod(WaterHeightSampler, float4(PX, PY + 1.0f / TerrainLength, 0, 0)).r;

	float H11 = HeightFarPlane * H11RGBA - TerrainDepth;
	float H21 = HeightFarPlane * H21RGBA - TerrainDepth;
	float H12 = HeightFarPlane * H12RGBA - TerrainDepth;

	WorldPos.z += H11;

	float4 World = mul(WorldPos, WorldViewProjection);
	float3 ViewDirection = CameraPosition.xyz - (WorldPos.xyz);

	output.WorldSpacePosition = WorldPos.xyz;
	output.ScreenSpacePosition = World;
	output.Position = World;

	output.DepthZ = World.z / FarPlane;

	output.ToCameraVector = CameraPosition - WorldPos.xyz;
	output.FromLightVector = WorldPos.xyz - LightPosition;

	return output;
}

float3 GetFresnel(float CosTheta, float3 Fresnel)
{
	return Fresnel + (1.0 - Fresnel) * pow(1.0 - CosTheta, 5.0);
}

float GetNormalDistribution(float CosLH, float Roughness)
{
	float Alpha = Roughness * Roughness;
	float AlphaSQ = Alpha * Alpha;

	float denom = (CosLH * CosLH) * (AlphaSQ - 1.0) + 1.0;
	return AlphaSQ / (PI * denom * denom);
}

float GeometrySchlickGGX(float NdotV, float Roughness)
{
	float nom = NdotV;
	float denom = NdotV * (1.0 - Roughness) + Roughness;

	return nom / denom;
}

float gaSchlickG1(float CosTheta, float k)
{
	return CosTheta / (CosTheta * (1.0 - k) + k);
}

float GetGeometrySmith(float CosLi, float CosLo, float Roughness)
{
	float r = Roughness + 1.0;
	float k = (r * r) / 8.0;
	return gaSchlickG1(CosLi, k) * gaSchlickG1(CosLo, k);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 WorldPosition = input.WorldSpacePosition;

	float InvMatX = 1.0 / DataWidth;
	float InvMatY = 1.0 / DataHeight;

	float2 WorldUV = frac(WorldPosition / TexSize);

	float PX = (WorldPosition.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPosition.y - Position.y) / (TerrainLength * TerrainScale);

	float WaterData = tex2D(WaterDataSampler1, float2(PX, PY)).a;
	if (WaterData == 0)
	{
		return float4(0, 0, 0, 0);
	}

	float3 Normal = tex2D(NormalMapSampler, float2(PX,PY));

	float2 ndc = float2(input.ScreenSpacePosition.x / input.ScreenSpacePosition.w / 2.0f + 0.5f, input.ScreenSpacePosition.y / input.ScreenSpacePosition.w / 2.0f + 0.5f);

	float2 ReflectTex = float2(ndc.x, ndc.y);
	float2 RefractTex = float2(ndc.x, -ndc.y);

	float Depth = tex2D(DepthSampler, RefractTex).r;
	float FloorDistance = 2.0f * NearPlane * FarPlane / (FarPlane + NearPlane - (2.0f * Depth - 1.0f) * (FarPlane - NearPlane));

	float Depth2 = input.DepthZ;
	float WaterDistance = 2.0f * NearPlane * FarPlane / (FarPlane + NearPlane - (2.0f * Depth2 - 1.0f) * (FarPlane - NearPlane));

	float WaterDepthZ = FloorDistance - WaterDistance;
	WaterDepthZ = clamp(2.0f * pow(WaterDepthZ * 100.0f, 2.0f), 0.0f, 1.0f);

	ReflectTex = (ReflectTex + (float2(Normal.x, Normal.y) / 2.0f) / TexSize * clamp(WaterDepthZ, 0.0f, 0.15f));
	RefractTex = (RefractTex + (float2(Normal.x, Normal.y) / 2.0f) / TexSize * clamp(WaterDepthZ, 0.0f, 0.15f));

	ReflectTex.x = clamp(ReflectTex.x, 0.001f, 0.999f);
	ReflectTex.y = clamp(ReflectTex.y, 0.001f, 0.999f);
	RefractTex.x = clamp(RefractTex.x, 0.001f, 0.999f);
	RefractTex.y = clamp(RefractTex.y, -0.999f, 0.001f);

	//ReflectTex.y = clamp(ReflectTex.y, 0.0001f, 0.9999f);
	//RefractTex.y = clamp(RefractTex.y, -0.9999f, 0.001f);

	float4 ReflectColor = tex2D(ReflectionSampler, ReflectTex);
	float4 RefractColor = tex2D(RefractionSampler, RefractTex);

	float4 WaterColorData1 = tex2D(ColorDataSampler1, float2(PX, PY));

	float3 ViewVector = normalize(input.ToCameraVector);
	float RefractiveFactor = dot(ViewVector, float3(0, 0, 1.0f));
	RefractiveFactor = saturate(pow(RefractiveFactor, WaterColorData1.g * 2.5f));

	float4 DepthColorMult = float4(0,0,0,0);
	float ColorDistance = (FloorDistance - WaterDistance) * 100.0f;

	//Deeper-> Refraction Color Gets darker
	//WaterColor -> Gets darker, les transparent

	DepthColorMult.r = clamp(1.0f - (ColorDistance * WaterColorData1.a), 0.5f, 1.0f);
	DepthColorMult.g = clamp(1.0f - (ColorDistance * WaterColorData1.a / 2.0f), 0.6f, 1.0f);
	DepthColorMult.b = clamp(1.0f - (ColorDistance * WaterColorData1.a / 5.0f), 0.9f, 1.0f);
	DepthColorMult.a = clamp((ColorDistance * WaterColorData1.a * 5.0f), 0.0f, 1.0f);

	float4 WaterColor = tex2D(ColorDataSampler0, float2(PX, PY));

	float HasWaterColorDepth = min(DepthColorMult.a * 255, 1.0f);
	float4 RefractWaterColor = lerp(RefractColor, float4(lerp(RefractColor.rgb, WaterColor.rgb, WaterColor.a), 1.0f), DepthColorMult.a) * HasWaterColorDepth + float4(lerp(RefractColor.rgb, WaterColor.rgb, WaterColor.a), 1.0f) * (1.0f - HasWaterColorDepth);

	RefractWaterColor = lerp(ReflectColor, RefractWaterColor, RefractiveFactor);

	float4 ReflectRefractColor = RefractWaterColor;

	float4 FoamTexture = tex2D(FoamSampler, WorldUV);

	float FoamValue = FoamTexture.r * clamp(WaterColorData1.b, 0.0f, 0.33f) * 3.0f + FoamTexture.g * clamp(WaterColorData1.b - 0.33f, 0.0f, 0.33f) * 3.0f + FoamTexture.b * clamp(WaterColorData1.b - 0.66f, 0.0f, 0.33f) * 3.0f;
	FoamValue = max(FoamValue, min(ceil(WaterColorData1.b * 255), 1.0f) * (FoamTexture.r + FoamTexture.g + FoamTexture.b));


	//Plug in PBR
	float4 Albedo = ReflectRefractColor;

	float3 FinalNormal = Normal;
	FinalNormal *= WaterColorData1.r * 2.0f;
	FinalNormal = normalize(mul(input.TangentBasis, FinalNormal));
	float FinalRoughness = 0.3f;

	//Plugin Final Values into BDFR
	float3 LO = normalize(CameraPosition - input.WorldSpacePosition);
	float3 LI = normalize(LightPosition - input.WorldSpacePosition);
	float3 LH = normalize(LO + LI);
	float CosTheta = max(dot(LH, LO), 0.0);
	float CosLi = max(dot(FinalNormal, LI), 0.0f);
	float CosLh = max(dot(FinalNormal, LH), 0.0f);
	float CosLo = max(dot(FinalNormal, LO), 0.0f);

	float3 F0 = float3(0.02f, 0.02f,0.02f);

	float3 F = GetFresnel(CosTheta, F0);
	float D = GetNormalDistribution(CosLh, FinalRoughness);
	float G = GetGeometrySmith(CosLi, CosLo, FinalRoughness);

	float3 KD = float3(1, 1, 1) - F;
	float3 Diffuse = KD * Albedo;
	float3 Specular = (F * D * G) / max(4.0 * CosLi * CosLo, Epsilon);
	float3 DirectLighting = (Diffuse + Specular) * LightColor * CosLi;

	float3 diffuseIBL = 0.0f;

	float4 Color = float4(DirectLighting + diffuseIBL, 1.0f);
	Color += float4(FoamValue, FoamValue, FoamValue, FoamValue);
	Color += Albedo;
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