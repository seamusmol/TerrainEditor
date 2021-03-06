﻿#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

matrix WorldViewProjection;
float3 Position;

float TerrainScale;
int TerrainWidth;
int TerrainLength;
float TerrainDepth;
float TexSize = 16.0f;
float Time;
float WaterSpeed = 1.0f;
float WaterRepeatTime = 1.0f;

float NearPlane = 0.1f;
float FarPlane = 1000.0f;

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
float EdgeBlendDistance = 1.0f;

float WaveHeight = 0.25f;
float HeightScale = 0.25f;

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
sampler2D FoamSampler = sampler_state
{
	Texture = (FoamMap);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
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
	float3 Normal : TEXCOORD2;
	float3 Tangent : TEXCOORD3;
	float3 Binormal : TEXCOORD4;
	float3 ToCameraVector : TEXCOORD5;
	float3 FromLightVector: TEXCOORD6;
	float WaterDepth : TEXCOORD7;
	float TerrainHeight : TEXCOORD8;
	float DepthZ : TEXCOORD9;
	float3x3 TangentBasis : TBASIS;
};

float CalculateOffSet(float4 FlowData)
{
	float TimeOffset = FlowData.a / 2.0f;
	float WaveBackTime = FlowData.b * Time;
	int EnableWaveBack = min(WaveBackTime * 255, 1.0f);

	float2 NormalOffset = (Time / TexSize) % TexSize;
	float PulsingOffset = (WaveBackTime / 2.0f + sin(sin(WaveBackTime) + WaveBackTime * 1.5f) + TimeOffset) / TexSize;

	return NormalOffset * (1 - EnableWaveBack) + abs(PulsingOffset * EnableWaveBack);
}

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
	WorldPos.z += (input.TerrainHeight + input.WaterHeight);

	WorldPos.xy *= TerrainScale;
	WorldPos.xyz += Position.xyz;

	//add HeightMap
	int WaterHeight = (int(ClipHeightRGBA.x * 255.0) << 24) + (int(ClipHeightRGBA.y * 255.0) << 16) + (int(ClipHeightRGBA.z * 255.0) << 8) + int(ClipHeightRGBA.w * 255.0);
	ClipHeight *= HeightScale;
	ClipHeight -= TerrainDepth;

	WorldPos.z += WaterHeight;


	float PX = (WorldPos.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPos.y - Position.y) / (TerrainLength * TerrainScale);

	//calculate Displacement
	float2 WaveData = tex2Dlod(WaveDataSampler0, float4(PX, PY, 0, 0)).rg;
	WaveData.x = max(WaveData.x * 512, 2);
	WaveData.y = max(WaveData.y * 8, 0.0f);

	float2 UV = float2((WorldPos.xy - Position.xy) / WaveData.x);
	float4 VertexFlowData = tex2Dlod(WaterDataSampler0, float4(PX, PY, 0, 0));
	float2 VertexFlowDirection = float2(VertexFlowData.rg * 2.0f - 1.0f) * WaterSpeed;
	float2 Offset = VertexFlowDirection.rg * CalculateOffSet(VertexFlowData);

	float Displacement = (tex2Dlod(NoiseSampler, float4(UV.xy + Offset.xy, 0, 0)).g) * WaveData.y;
	WorldPos.z -= Displacement;

	float4 World = mul(WorldPos, WorldViewProjection);
	float3 ViewDirection = CameraPosition.xyz - (WorldPos.xyz);

	float3 Normal = normalize( cross( float3(ChunkWidth, 0, H21 - H11),float3(0.0, ChunkLength, H12 - H11)));

	float3 Tangent = float3(0, 0, 0);
	float3 BiTangent = float3(0, 0, 0);
	GenerateTanBiTan(Normal, Tangent, BiTangent);

	float3x3 TBN = float3x3(Tangent, BiTangent, float3(0,0,1));
	output.TangentBasis = transpose(TBN);

	output.WorldSpacePosition = WorldPos.xyz;
	output.ScreenSpacePosition = World;
	output.Position = World;
	output.Normal = Normal;
	output.WaterDepth = max(input.WaterHeight, 0);

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

	float DX = PX % InvMatX;
	float DY = PY % InvMatY;

	float WaterData = tex2D(WaterDataSampler1, float2(PX, PY)).a;
	if (WaterData == 0)
	{
		return float4(0, 0, 0, 0);
	}

	float2 PD = float2(DX / InvMatX, DY / InvMatY);

	float PDA = clamp(0.5f - PD.x, 0.0f, 0.5f);
	float PDB = clamp(PD.x - 0.5f, 0.0f, 0.5f);
	float PDC = clamp(0.5f - PD.y, 0.0f, 0.5f);
	float PDD = clamp(PD.y - 0.5f, 0.0f, 0.5f);

	float PDOX = (0.5f - PD.x) *  InvMatX;
	float PDOY = (0.5f - PD.y) *  InvMatY;

	float OX = InvMatX;
	float OY = InvMatY;

	float4 FlowData0 = tex2D(WaterDataSampler0, float2(PX, PY));
	float4 FlowData1 = tex2D(WaterDataSampler0, float2(PX - OX, PY));
	float4 FlowData2 = tex2D(WaterDataSampler0, float2(PX + OX, PY));
	float4 FlowData3 = tex2D(WaterDataSampler0, float2(PX, PY - OY));
	float4 FlowData4 = tex2D(WaterDataSampler0, float2(PX, PY + OY));

	float2 FlowDir0 = (FlowData0.rg * 2.0f - 1.0f) * float2(-1.0f, 1.0f) * WaterSpeed;
	float2 FlowDir1 = (FlowData1.rg * 2.0f - 1.0f) * float2(-1.0f, 1.0f) * WaterSpeed;
	float2 FlowDir2 = (FlowData2.rg * 2.0f - 1.0f) * float2(-1.0f, 1.0f) * WaterSpeed;
	float2 FlowDir3 = (FlowData3.rg * 2.0f - 1.0f) * float2(-1.0f, 1.0f) * WaterSpeed;
	float2 FlowDir4 = (FlowData4.rg * 2.0f - 1.0f) * float2(-1.0f, 1.0f) * WaterSpeed;

	float3 Noise = tex2D(NoiseSampler, WorldUV.xy);

	float2 Offset0 = FlowDir0 * (CalculateOffSet(FlowData0));
	float2 Offset1 = FlowDir1 * (CalculateOffSet(FlowData1));
	float2 Offset2 = FlowDir2 * (CalculateOffSet(FlowData2));
	float2 Offset3 = FlowDir3 * (CalculateOffSet(FlowData3));
	float2 Offset4 = FlowDir4 * (CalculateOffSet(FlowData4));

	float2 Offset0B = FlowDir0 * (CalculateOffSet(FlowData0) + 0.5f);
	float2 Offset1B = FlowDir1 * (CalculateOffSet(FlowData1) + 0.5f);
	float2 Offset2B = FlowDir2 * (CalculateOffSet(FlowData2) + 0.5f);
	float2 Offset3B = FlowDir3 * (CalculateOffSet(FlowData3) + 0.5f);
	float2 Offset4B = FlowDir4 * (CalculateOffSet(FlowData4) + 0.5f);

	float3 Normal0 = tex2D(WaveSampler, WorldUV.xy + Offset0) / 8.0f;
	float3 NormalA = tex2D(WaveSampler, WorldUV.xy + Offset1);
	float3 NormalB = tex2D(WaveSampler, WorldUV.xy + Offset2);
	float3 NormalC = tex2D(WaveSampler, WorldUV.xy + Offset3);
	float3 NormalD = tex2D(WaveSampler, WorldUV.xy + Offset4);

	float3 Normal0P = tex2D(WaveSampler2, WorldUV.xy + Offset0B) / 8.0f;
	float3 NormalAP = tex2D(WaveSampler2, WorldUV.xy + Offset1B);
	float3 NormalBP = tex2D(WaveSampler2, WorldUV.xy + Offset2B);
	float3 NormalCP = tex2D(WaveSampler2, WorldUV.xy + Offset3B);
	float3 NormalDP = tex2D(WaveSampler2, WorldUV.xy + Offset4B);

	float FlowVal0 = max(abs(FlowDir0.x), abs(FlowDir0.y));
	float FlowVal1 = max(abs(FlowDir1.x), abs(FlowDir1.y));
	float FlowVal2 = max(abs(FlowDir2.x), abs(FlowDir2.y));
	float FlowVal3 = max(abs(FlowDir3.x), abs(FlowDir3.y));
	float FlowVal4 = max(abs(FlowDir4.x), abs(FlowDir4.y));

	float PulseLerp0 = saturate(abs(1 - (FlowVal0 * (Time + Noise.r) + Noise.r) % 2.0));
	float PulseLerp1 = saturate(abs(1 - (FlowVal1 * (Time + Noise.r) + Noise.r) % 2.0));
	float PulseLerp2 = saturate(abs(1 - (FlowVal2 * (Time + Noise.r) + Noise.r) % 2.0));
	float PulseLerp3 = saturate(abs(1 - (FlowVal3 * (Time + Noise.r) + Noise.r) % 2.0));
	float PulseLerp4 = saturate(abs(1 - (FlowVal4 * (Time + Noise.r) + Noise.r) % 2.0));

	//lerp pulse
	float3 PulseNormal0 = lerp(Normal0, Normal0P, PulseLerp0);
	float3 PulseNormalA = lerp(NormalA, NormalAP, PulseLerp1);
	float3 PulseNormalB = lerp(NormalB, NormalBP, PulseLerp2);
	float3 PulseNormalC = lerp(NormalC, NormalCP, PulseLerp3);
	float3 PulseNormalD = lerp(NormalD, NormalDP, PulseLerp4);

	float3 LerpA = lerp(PulseNormal0, PulseNormalA, PDA);
	float3 LerpB = lerp(PulseNormal0, PulseNormalB, PDB);
	float3 LerpC = lerp(PulseNormal0, PulseNormalC, PDC);
	float3 LerpD = lerp(PulseNormal0, PulseNormalD, PDD);

	float FlowLerpA = lerp(FlowVal0, FlowVal1, PDA);
	float FlowLerpB = lerp(FlowVal0, FlowVal2, PDB);
	float FlowLerpC = lerp(FlowVal0, FlowVal3, PDC);
	float FlowLerpD = lerp(FlowVal0, FlowVal4, PDD);

	float3 MixBump = normalize((LerpA + LerpB + LerpC + LerpD));
	MixBump = MixBump * 2.0f - 1.0f;

	float FlowLerp = clamp((FlowLerpA + FlowLerpB + FlowLerpC + FlowLerpD) / 4.0f, -0.0f, 1.0f);

	float2 ndc = float2(input.ScreenSpacePosition.x / input.ScreenSpacePosition.w / 2.0f + 0.5f, input.ScreenSpacePosition.y / input.ScreenSpacePosition.w / 2.0f + 0.5f);

	float2 ReflectTex = float2(ndc.x, ndc.y);
	float2 RefractTex = float2(ndc.x, -ndc.y);

	float Depth = tex2D(DepthSampler, RefractTex).r;
	float FloorDistance = 2.0f * NearPlane * FarPlane / (FarPlane + NearPlane - (2.0f * Depth - 1.0f) * (FarPlane - NearPlane));

	float Depth2 = input.DepthZ;
	float WaterDistance = 2.0f * NearPlane * FarPlane / (FarPlane + NearPlane - (2.0f * Depth2 - 1.0f) * (FarPlane - NearPlane));

	float WaterDepthZ = FloorDistance - WaterDistance;
	WaterDepthZ = clamp(2.0f * pow(WaterDepthZ * 100.0f, 2.0f), 0.0f, 1.0f);

	ReflectTex = (ReflectTex + (float2(MixBump.x, MixBump.y) / 2.0f) / TexSize * clamp(WaterDepthZ, 0.0f, 0.15f) * saturate(input.WaterDepth * 10.0f - 0.1f));
	RefractTex = (RefractTex + (float2(MixBump.x, MixBump.y) / 2.0f) / TexSize * clamp(WaterDepthZ, 0.0f, 0.15f) * saturate(input.WaterDepth * 10.0f - 0.1f));

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

	//float3 BumpColor = GetPixelBumpValue(MixBump, input.ToCameraVector, input.FromLightVector);
	//BumpColor *= WaterColorData1.r * 2.0f;
	float4 FoamTexture = tex2D(FoamSampler, WorldUV);

	float FoamValue = FoamTexture.r * clamp(WaterColorData1.b, 0.0f, 0.33f) * 3.0f + FoamTexture.g * clamp(WaterColorData1.b - 0.33f, 0.0f, 0.33f) * 3.0f + FoamTexture.b * clamp(WaterColorData1.b - 0.66f, 0.0f, 0.33f) * 3.0f;
	FoamValue = max(FoamValue, min(ceil(WaterColorData1.b * 255), 1.0f) * saturate(1.0f - input.WaterDepth * 10.0f) * (FoamTexture.r + FoamTexture.g + FoamTexture.b));


	//Plug in PBR
	float4 Albedo = ReflectRefractColor;

	float3 FinalNormal = MixBump;
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

int ColorResolutionX;
int ColorResolutionY;

Texture2D SceneDepthTexture;
sampler2D SceneDepthSampler = sampler_state
{
	Texture = (SceneDepthTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D SceneTexture;
sampler2D SceneColorSampler = sampler_state
{
	Texture = (SceneTexture);
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct SecondVertexOutput
{
	float4 Position : POSITION0;
	float3 WorldSpacePosition : TEXCOORD1;
	float4 ScreenSpacePosition : TEXCOORD2;
	float Depth : TEXCOORD3;
	float4 ClipSpaceA: TEXCOORD4;
	float4 ClipSpaceB: TEXCOORD5;
	float4 ClipSpaceC: TEXCOORD6;
	float4 ClipSpaceD: TEXCOORD7;
};

SecondVertexOutput SecondVS(in VertexShaderInput input)
{
	SecondVertexOutput output = (SecondVertexOutput)0;

	uint VID = input.VertexID / 12;

	float4 WorldPos = float4(VID / TerrainLength, (VID % TerrainLength), 0, 1);

	WorldPos.xy += TriangleVertices[Indices[input.VertexID % 12]];
	WorldPos.z += (input.TerrainHeight + input.WaterHeight);

	WorldPos.xy *= TerrainScale;
	WorldPos.xyz += Position.xyz;

	float PX = (WorldPos.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPos.y - Position.y) / (TerrainLength * TerrainScale);

	float2 WaveData = tex2Dlod(WaveDataSampler0, float4(PX, PY, 0, 0)).rg;
	WaveData.x = max(WaveData.x * 512, 8);
	WaveData.y = max(WaveData.y * 8, 0.0f);

	float2 UV = (float2(WorldPos.xy) % (WaveData.x)) / (WaveData.x);
	float4 VertexFlowData = tex2Dlod(WaterDataSampler0, float4(PX, PY, 0, 0));
	float2 VertexFlowDirection = (VertexFlowData.rg * 2.0f - 1.0f) * WaterSpeed;
	float2 Offset = VertexFlowDirection.rg * CalculateOffSet(VertexFlowData);

	float4 Displacement = (tex2Dlod(NoiseSampler, float4(UV.xy + Offset.xy, 0, 0)).g) * WaveData.y;

	WorldPos.z -= Displacement;

	float4 World = mul(WorldPos, WorldViewProjection);

	output.WorldSpacePosition = WorldPos.xyz;
	output.ScreenSpacePosition = World;

	output.Position = World;
	output.Depth = World.z / FarPlane;

	output.ClipSpaceA = mul(WorldPos + float4(-0.02f, 0, 0, 0), WorldViewProjection);
	output.ClipSpaceB = mul(WorldPos + float4(0.02f, 0, 0, 0), WorldViewProjection);
	output.ClipSpaceC = mul(WorldPos + float4(0, -0.02f, 0, 0), WorldViewProjection);
	output.ClipSpaceD = mul(WorldPos + float4(0, 0.02f, 0, 0), WorldViewProjection);
	return output;
}

float4 SecondPS(SecondVertexOutput input) : COLOR
{
	float3 WorldPosition = input.WorldSpacePosition;

	float InvMatX = 1.0 / DataWidth;
	float InvMatY = 1.0 / DataHeight;

	float2 WorldUV = frac(WorldPosition / TexSize);

	float PX = (WorldPosition.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPosition.y - Position.y) / (TerrainLength * TerrainScale);

	float DX = fmod(PX, InvMatX);
	float DY = fmod(PY, InvMatY);

	float2 PD = float2(DX / InvMatX, DY / InvMatY);

	float PDA = clamp(0.5f - PD.x, 0.0f, 0.5f);
	float PDB = clamp(PD.x - 0.5f, 0.0f, 0.5f);
	float PDC = clamp(0.5f - PD.y, 0.0f, 0.5f);
	float PDD = clamp(PD.y - 0.5f, 0.0f, 0.5f);

	float2 ndc = float2(input.ScreenSpacePosition.x / input.ScreenSpacePosition.w / 2.0f + 0.5f, input.ScreenSpacePosition.y / input.ScreenSpacePosition.w / 2.0f + 0.5f);

	float2 ProjectiveCoord = float2(ndc.x, -ndc.y);

	float Depth = tex2D(SceneDepthSampler, ProjectiveCoord).r;
	float Depth2 = input.Depth;

	float WaterDepthZ = Depth - Depth2;
	float IsVisible = min(ceil(WaterDepthZ), 1.0);

	if (IsVisible <= 0.0f)
	{
		return float4(0,0,0,0);
	}

	//Set First pixel Normal to invisible 
	float EdgeDist = max(max(abs(0.5f - PD.x) - 0.465f, abs(0.5f - PD.y) - 0.465f), 0.0f);
	float IsEdge = ceil(EdgeDist);

	float4 PassColor = tex2D(SceneColorSampler, ProjectiveCoord);
	//apply blend to edge
	float2 NDCA = float2(input.ClipSpaceA.x / input.ClipSpaceA.w / 2.0f + 0.5f, input.ClipSpaceA.y / input.ClipSpaceA.w / 2.0f + 0.5f);
	float2 NDCB = float2(input.ClipSpaceB.x / input.ClipSpaceB.w / 2.0f + 0.5f, input.ClipSpaceB.y / input.ClipSpaceB.w / 2.0f + 0.5f);
	float2 NDCC = float2(input.ClipSpaceC.x / input.ClipSpaceC.w / 2.0f + 0.5f, input.ClipSpaceC.y / input.ClipSpaceC.w / 2.0f + 0.5f);
	float2 NDCD = float2(input.ClipSpaceD.x / input.ClipSpaceD.w / 2.0f + 0.5f, input.ClipSpaceD.y / input.ClipSpaceD.w / 2.0f + 0.5f);
	//sample surrounding Texels
	float4 M1 = tex2D(SceneColorSampler, float2(NDCA.x, -NDCA.y));
	float4 M2 = tex2D(SceneColorSampler, float2(NDCB.x, -NDCB.y));
	float4 M3 = tex2D(SceneColorSampler, float2(NDCC.x, -NDCC.y));
	float4 M4 = tex2D(SceneColorSampler, float2(NDCD.x, -NDCD.y));

	float4 MergedPixel = M1 + M2 + M3 + M4;
	MergedPixel /= 4;

	MergedPixel = lerp(MergedPixel, PassColor, 0.75f);

	float4 Color = float4(0,0,0,0);

	Color = (Color * (1.0f - IsEdge) + MergedPixel * IsEdge) * IsVisible;

	float HasBrush = 0;
	HasBrush += min(1.0f, floor(BrushDistance / distance(BrushPosition.xy, WorldPosition.xy))) * (1.0f - min(1.0f, BrushShape)) * min(1.0f, BrushDistance);
	HasBrush += min(1.0f, floor(BrushDistance / max(distance(BrushPosition.x, WorldPosition.x), distance(BrushPosition.y, WorldPosition.y)))) * (1.0f - min(1.0f, 1.0f - BrushShape)) * min(1.0f, BrushDistance);

	return Color * (1.0f - HasBrush) + float4(lerp(Color.xyz, BrushHighLightColor.xyz, BrushHighLightColor.w), 1.0f) * HasBrush;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}

	pass P1
	{
		VertexShader = compile VS_SHADERMODEL SecondVS();
		PixelShader = compile PS_SHADERMODEL SecondPS();
	}


};