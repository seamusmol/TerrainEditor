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
float ShineDamper = 20.0;
float Reflectivity = 0.6;
float MinSpecular = 0.0f;

float WaveHeight = 0.25f;

float3 BrushPosition;
float4 BrushHighLightColor;
float BrushDistance;
int BrushShape;


static const float PI = 3.141592f;
static const float HalfPI = 1.570796f;
static const float Epsilon = 0.00001f;

static const float2 TriangleVertices[] =
{
	float2(0,0), float2(1,0), float2(0,1), float2(1,1)
};

static const int Indices[] =
{
	3,1,0, 0,2,3
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
	AddressU = Clamp;
	AddressV = Clamp;
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
	AddressU = Clamp;
	AddressV = Clamp;
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

Texture2D HeightMap;
sampler2D HeightMapSampler = sampler_state
{
	Texture = <HeightMap>;
	magfilter = Point;
	minfilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

Texture2D WaterHeightMap;
sampler2D WaterHeightMapSampler = sampler_state
{
	Texture = <WaterHeightMap>;
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
	float3 WorldSpacePosition : TEXCOORD0;
	float4 ScreenSpacePosition : TEXCOORD1;
	float3 ToCameraVector : TEXCOORD2;
	float3 FromLightVector: TEXCOORD3;
	float WaterDepth : TEXCOORD4;
	float TerrainHeight : TEXCOORD5;
	float DepthZ : TEXCOORD6;
	float3x3 TangentBasis : TBASIS;
};

float CalculateOffSet(float4 FlowData)
{
	float TimeOffset = FlowData.a / 2.0f;
	float WaveBackTime = FlowData.b * Time;
	int EnableWaveBack = min(WaveBackTime * 1000, 1);

	float2 NormalOffset = (Time / TexSize) % (TexSize + Epsilon);
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

float CalculateDisplacement(float2 WorldPos)
{
	float PX = (WorldPos.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPos.y - Position.y) / (TerrainLength * TerrainScale);

	float2 WaveData = tex2Dlod(WaveDataSampler0, float4(PX, PY, 0, 0)).rg;
	WaveData.x = max(WaveData.x * 512, 8);
	WaveData.y = max(WaveData.y * 8, 0.0f);
	float2 UV = (float2(WorldPos.xy) % (WaveData.x)) / (WaveData.x);
	float4 VertexFlowData = tex2Dlod(WaterDataSampler0, float4(PX, PY, 0, 0));
	float2 VertexFlowDirection = (VertexFlowData.rg * 2.0f - 1.0f) * WaterSpeed;
	float2 Offset = VertexFlowDirection.rg * CalculateOffSet(VertexFlowData);

	float Displacement = (tex2Dlod(NoiseSampler, float4(UV.xy + Offset.xy, 0, 0)).g) * WaveData.y;
	return tex2Dlod(NoiseSampler, float4(UV.xy + Offset.xy, 0, 0)).g * WaveData.y;
}

float3 CalculateNormal(float Height, float D01, float D10, float D11, float D21, float D12)
{
	float3 P01 = float3(-1.0f, 0.0f, Height - D01);
	float3 P10 = float3(0.0f, -1.0f, Height - D10);
	float3 P11 = float3(0.0f, 0.0f, Height - D11);
	float3 P21 = float3(1.0f, 0.0f, Height - D21);
	float3 P12 = float3(0.0f, 1.0f, Height - D12);

	float3 N1 = normalize(-cross(P10 - P11, P01 - P11));
	float3 N2 = normalize(-cross(P21 - P11, P10 - P11));
	float3 N3 = normalize(-cross(P12 - P11, P21 - P11));
	float3 N4 = normalize(-cross(P01 - P11, P12 - P11));

	return (N1 + N2 + N3 + N4) / 4;
}

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	uint VID = input.VertexID / 6;

	float4 WorldPos = float4(VID / TerrainLength, (VID % TerrainLength), 0, 1);
	WorldPos.xy += TriangleVertices[Indices[input.VertexID % 6]];
	WorldPos.xy *= TerrainScale;

	WorldPos.xyz += Position.xyz;

	float D01 = CalculateDisplacement(WorldPos.xy + float2(0, -TerrainScale));
	float D10 = CalculateDisplacement(WorldPos.xy + float2(-TerrainScale, 0));
	float D11 = CalculateDisplacement(WorldPos.xy + float2(0, 0));
	float D21 = CalculateDisplacement(WorldPos.xy + float2(TerrainScale, 0));
	float D12 = CalculateDisplacement(WorldPos.xy + float2(0, TerrainScale));

	float PX = (WorldPos.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPos.y - Position.y) / (TerrainLength * TerrainScale);

	float4 HeightRGBA00 = tex2Dlod(HeightMapSampler, float4(PX, PY, 0, 0)).rgba;
	float4 WaterHeightRGBA00 = tex2Dlod(WaterHeightMapSampler, float4(PX, PY, 0, 0)).rgba;

	int Height = (int(HeightRGBA00.r * 255) << 24) + (int(HeightRGBA00.g * 255) << 16) + (int(HeightRGBA00.b * 255) << 8) + (int(HeightRGBA00.a * 255));
	int WaterHeight = (int(WaterHeightRGBA00.r * 255) << 24) + (int(WaterHeightRGBA00.g * 255) << 16) + (int(WaterHeightRGBA00.b * 255) << 8) + (int(WaterHeightRGBA00.a * 255));

	WorldPos.z += (Height + WaterHeight) * HeightScale;
	float3 Normal = CalculateNormal(WorldPos.z, D01, D10, D11, D21, D12);

	WorldPos.z -= D11;

	float4 World = mul(WorldPos, WorldViewProjection);
	float3 ViewDirection = CameraPosition.xyz - (WorldPos.xyz);

	float3 Tangent = float3(0, 0, 0);
	float3 BiTangent = float3(0, 0, 0);
	GenerateTanBiTan(Normal, Tangent, BiTangent);

	float3x3 TBN = float3x3(Tangent, BiTangent, Normal);
	output.TangentBasis = transpose(TBN);
	output.WorldSpacePosition = WorldPos.xyz;
	output.ScreenSpacePosition = World;
	output.Position = World;
	output.WaterDepth = max(WaterHeight* HeightScale, 0);
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

	float3 Noise = tex2D(NoiseSampler, WorldUV.xy).rgb;

	float FlowVal0 = max(abs(FlowDir0.x), abs(FlowDir0.y));
	float FlowVal1 = max(abs(FlowDir1.x), abs(FlowDir1.y));
	float FlowVal2 = max(abs(FlowDir2.x), abs(FlowDir2.y));
	float FlowVal3 = max(abs(FlowDir3.x), abs(FlowDir3.y));
	float FlowVal4 = max(abs(FlowDir4.x), abs(FlowDir4.y));

	float PulseLerp0 = saturate(abs(1 - (FlowVal0 * (Time + Noise.b) + Noise.b) % 2.0));
	float PulseLerp1 = saturate(abs(1 - (FlowVal1 * (Time + Noise.b) + Noise.b) % 2.0));
	float PulseLerp2 = saturate(abs(1 - (FlowVal2 * (Time + Noise.b) + Noise.b) % 2.0));
	float PulseLerp3 = saturate(abs(1 - (FlowVal3 * (Time + Noise.b) + Noise.b) % 2.0));
	float PulseLerp4 = saturate(abs(1 - (FlowVal4 * (Time + Noise.b) + Noise.b) % 2.0));

	float3 PulseNormal0 = lerp(
		tex2D(WaveSampler, WorldUV.xy + FlowDir0 * (CalculateOffSet(FlowData0))) / 8.0f,
		tex2D(WaveSampler2, WorldUV.xy + FlowDir0 * (CalculateOffSet(FlowData0) + 0.5f)) / 8.0f,
		PulseLerp0);

	float3 LerpA = lerp(
		PulseNormal0,
		lerp(tex2D(WaveSampler, WorldUV.xy + FlowDir1 * (CalculateOffSet(FlowData1))), tex2D(WaveSampler2, WorldUV.xy + FlowDir1 * (CalculateOffSet(FlowData1) + 0.5f)), PulseLerp1),
		PDA);

	float3 LerpB = lerp(
		PulseNormal0,
		lerp(tex2D(WaveSampler, WorldUV.xy + FlowDir2 * (CalculateOffSet(FlowData2))), tex2D(WaveSampler2, WorldUV.xy + FlowDir2 * (CalculateOffSet(FlowData2) + 0.5f)), PulseLerp2),
		PDB);

	float3 LerpC = lerp(
		PulseNormal0,
		lerp(tex2D(WaveSampler, WorldUV.xy + FlowDir3 * (CalculateOffSet(FlowData3))), tex2D(WaveSampler2, WorldUV.xy + FlowDir3 * (CalculateOffSet(FlowData3) + 0.5f)), PulseLerp3),
		PDC);

	float3 LerpD = lerp(
		PulseNormal0,
		lerp(tex2D(WaveSampler, WorldUV.xy + FlowDir4 * (CalculateOffSet(FlowData4))), tex2D(WaveSampler2, WorldUV.xy + FlowDir4 * (CalculateOffSet(FlowData4) + 0.5f)), PulseLerp4),
		PDD);

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

	ReflectTex = (ReflectTex + (float2(MixBump.x, MixBump.y) / 2.0f) / TexSize * clamp(WaterDepthZ, 0.0f, 0.15f) * saturate(input.WaterDepth * 20.0f));
	RefractTex = (RefractTex + (float2(MixBump.x, MixBump.y) / 2.0f) / TexSize * clamp(WaterDepthZ, 0.0f, 0.15f) * saturate(input.WaterDepth * 20.0f));

	ReflectTex.x = clamp(ReflectTex.x, 0.001f, 0.999f);
	ReflectTex.y = clamp(ReflectTex.y, 0.001f, 0.999f);
	RefractTex.x = clamp(RefractTex.x, 0.001f, 0.999f);
	RefractTex.y = clamp(RefractTex.y, -0.999f, 0.001f);

	float4 ReflectColor = tex2D(ReflectionSampler, ReflectTex);
	float4 RefractColor = tex2D(RefractionSampler, RefractTex);

	float4 WaterColorData1 = tex2D(ColorDataSampler1, float2(PX, PY));

	float3 ViewVector = normalize(input.ToCameraVector);
	float RefractiveFactor = dot(ViewVector, float3(0, 0, 1.0f));
	RefractiveFactor = saturate(pow(RefractiveFactor, WaterColorData1.g * 2.5f));

	float4 DepthColorMult = float4(0,0,0,0);
	float ColorDistance = (FloorDistance - WaterDistance) * 100.0f;

	DepthColorMult.r = clamp(1.0f - (ColorDistance * WaterColorData1.a), 0.5f, 1.0f);
	DepthColorMult.g = clamp(1.0f - (ColorDistance * WaterColorData1.a / 2.0f), 0.6f, 1.0f);
	DepthColorMult.b = clamp(1.0f - (ColorDistance * WaterColorData1.a / 5.0f), 0.9f, 1.0f);
	DepthColorMult.a = clamp((ColorDistance * WaterColorData1.a * 5.0f), 0.0f, 1.0f);

	float4 WaterColor = tex2D(ColorDataSampler0, float2(PX, PY));

	float HasWaterColorDepth = min(DepthColorMult.a * 255, 1.0f);
	float4 RefractWaterColor = lerp(RefractColor, float4(lerp(RefractColor.rgb, WaterColor.rgb, WaterColor.a), 1.0f), DepthColorMult.a) * HasWaterColorDepth + float4(lerp(RefractColor.rgb, WaterColor.rgb, WaterColor.a), 1.0f) * (1.0f - HasWaterColorDepth);

	RefractWaterColor /= HalfPI;
	ReflectColor /= HalfPI;
	RefractWaterColor = lerp(ReflectColor, RefractWaterColor, RefractiveFactor);
	float4 ReflectRefractColor = RefractWaterColor;

	float4 FoamTexture = tex2D(FoamSampler, WorldUV);
	float FoamValue = FoamTexture.r * clamp(WaterColorData1.b, 0.0f, 0.33f) * 3.0f + FoamTexture.g * clamp(WaterColorData1.b - 0.33f, 0.0f, 0.33f) * 3.0f + FoamTexture.b * clamp(WaterColorData1.b - 0.66f, 0.0f, 0.33f) * 3.0f;

	float3 Albedo = ReflectRefractColor.rgb;
	float3 FinalNormal = MixBump;
	FinalNormal = lerp(float3(0, 0, 1), FinalNormal* 2.0f, WaterColorData1.r);
	FinalNormal = normalize(mul(input.TangentBasis, FinalNormal));
	float FinalRoughness = 0.14f;

	float3 LO = normalize(CameraPosition - input.WorldSpacePosition);
	float3 LI = normalize(LightPosition - input.WorldSpacePosition);
	float3 LH = normalize(LO + LI);
	float CosTheta = max(dot(LH, LO), 0.0);
	float CosLi = max(dot(FinalNormal, LI), 0.0f);
	float CosLh = max(dot(FinalNormal, LH), 0.0f);
	float CosLo = max(dot(FinalNormal, LO), 0.0f);

	float3 F0 = float3(0.05f, 0.05f, 0.05f);

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
	Color.rgb += Albedo.rgb;

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

	uint VID = input.VertexID / 6;

	float4 WorldPos = float4(VID / TerrainLength, (VID % TerrainLength), 0, 1);
	WorldPos.xy += TriangleVertices[Indices[input.VertexID % 6]];
	WorldPos.xy *= TerrainScale;
	WorldPos.xyz += Position.xyz;

	float D11 = CalculateDisplacement(WorldPos.xy + float2(0, 0));

	float PX = (WorldPos.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPos.y - Position.y) / (TerrainLength * TerrainScale);

	float4 HeightRGBA00 = tex2Dlod(HeightMapSampler, float4(PX, PY, 0, 0)).rgba;
	float4 WaterHeightRGBA00 = tex2Dlod(WaterHeightMapSampler, float4(PX, PY, 0, 0)).rgba;

	int Height = (int(HeightRGBA00.r * 255) << 24) + (int(HeightRGBA00.g * 255) << 16) + (int(HeightRGBA00.b * 255) << 8) + (int(HeightRGBA00.a * 255));
	int WaterHeight = (int(WaterHeightRGBA00.r * 255) << 24) + (int(WaterHeightRGBA00.g * 255) << 16) + (int(WaterHeightRGBA00.b * 255) << 8) + (int(WaterHeightRGBA00.a * 255));

	WorldPos.z += (Height + WaterHeight) * HeightScale;
	WorldPos.z -= D11;

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

	float EdgeDist = max(max(abs(0.5f - PD.x) - 0.465f, abs(0.5f - PD.y) - 0.465f), 0.0f);
	float IsEdge = ceil(EdgeDist);

	float4 PassColor = tex2D(SceneColorSampler, ProjectiveCoord);

	float2 NDCA = float2(input.ClipSpaceA.x / input.ClipSpaceA.w / 2.0f + 0.5f, input.ClipSpaceA.y / input.ClipSpaceA.w / 2.0f + 0.5f);
	float2 NDCB = float2(input.ClipSpaceB.x / input.ClipSpaceB.w / 2.0f + 0.5f, input.ClipSpaceB.y / input.ClipSpaceB.w / 2.0f + 0.5f);
	float2 NDCC = float2(input.ClipSpaceC.x / input.ClipSpaceC.w / 2.0f + 0.5f, input.ClipSpaceC.y / input.ClipSpaceC.w / 2.0f + 0.5f);
	float2 NDCD = float2(input.ClipSpaceD.x / input.ClipSpaceD.w / 2.0f + 0.5f, input.ClipSpaceD.y / input.ClipSpaceD.w / 2.0f + 0.5f);

	float4 M1 = tex2D(SceneColorSampler, float2(NDCA.x, -NDCA.y));
	float4 M2 = tex2D(SceneColorSampler, float2(NDCB.x, -NDCB.y));
	float4 M3 = tex2D(SceneColorSampler, float2(NDCC.x, -NDCC.y));
	float4 M4 = tex2D(SceneColorSampler, float2(NDCD.x, -NDCD.y));

	float4 MergedPixel = M1 + M2 + M3 + M4;
	MergedPixel /= 4;

	MergedPixel = lerp(MergedPixel, PassColor, 0.1f);

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
