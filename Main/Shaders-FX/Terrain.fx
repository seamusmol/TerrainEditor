
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

int MaterialWidth = 0;
int MaterialHeight = 0;

float3 BrushPosition;
float4 BrushHighLightColor;
float BrushDistance;
int BrushShape;
//materials per quad

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

Texture2D RoughnessMap;
sampler2D RoughnessSampler = sampler_state
{
	Texture = <RoughnessMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D MaterialNormalMap;
sampler2D MaterialNormalSampler = sampler_state
{
	Texture = <MaterialNormalMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D MetalnessMap;
sampler2D MetalnessSampler = sampler_state
{
	Texture = <MetalnessMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D DecalDiffuseMap;
sampler2D DecalSampler = sampler_state
{
	Texture = <DecalDiffuseMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};


Texture2D DecalRoughnessMap;
sampler2D DecalRoughnessSampler = sampler_state
{
	Texture = <DecalRoughnessMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D DecalNormalMap;
sampler2D DecalNormalSampler = sampler_state
{
	Texture = <DecalNormalMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D DecalMetalnessMap;
sampler2D DecalMetalnessSampler = sampler_state
{
	Texture = <DecalMetalnessMap>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D DecalToleranceMap;
sampler2D DecalToleranceSampler = sampler_state
{
	Texture = <DecalToleranceMap>;
	AddressU = Wrap;
	AddressV = Wrap;
};

Texture2D MaterialData0;
sampler2D DataSampler0 = sampler_state
{
	Texture = <MaterialData0>;
	magfilter = Point;
	minfilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

Texture2D MaterialData1;
sampler2D DataSampler1 = sampler_state
{
	Texture = <MaterialData1>;
	magfilter = Linear;
	minfilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
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

float GetBilinearHeight(int VertexID, inout float3 Normal)
{
	float4 FloorPos = float4(VertexID / 6 / TerrainHeight, (VertexID / 6 % TerrainHeight), 0, 1);
	float2 Pos = TriangleVertices[Indices[VertexID % 6]];

	float2 UV = FloorPos + Pos;

	float2 TexSize = float2(TerrainWidth, TerrainHeight);

	float4 HeightRGBA01 = tex2Dlod(HeightMapSampler, float4((UV.xy + float2(-1.0f, 0)) / TexSize.xy, 0, 0)).rgba;
	float4 HeightRGBA10 = tex2Dlod(HeightMapSampler, float4((UV.xy + float2(0, -1.0f)) / TexSize.xy, 0, 0)).rgba;
	float4 HeightRGBA11 = tex2Dlod(HeightMapSampler, float4(UV.xy / TexSize.xy, 0, 0)).rgba;
	float4 HeightRGBA21 = tex2Dlod(HeightMapSampler, float4((UV.xy + float2(1.0f, 0)) / TexSize.xy, 0, 0)).rgba;
	float4 HeightRGBA12 = tex2Dlod(HeightMapSampler, float4((UV.xy + float2(0, 1.0f)) / TexSize.xy, 0, 0)).rgba;

	int H01 = (int(HeightRGBA01.r * 255) << 24) + (int(HeightRGBA01.g * 255) << 16) + (int(HeightRGBA01.b * 255) << 8) + (int(HeightRGBA01.a * 255));
	int H10 = (int(HeightRGBA10.r * 255) << 24) + (int(HeightRGBA10.g * 255) << 16) + (int(HeightRGBA10.b * 255) << 8) + (int(HeightRGBA10.a * 255));
	int H11 = (int(HeightRGBA11.r * 255) << 24) + (int(HeightRGBA11.g * 255) << 16) + (int(HeightRGBA11.b * 255) << 8) + (int(HeightRGBA11.a * 255));
	int H21 = (int(HeightRGBA21.r * 255) << 24) + (int(HeightRGBA21.g * 255) << 16) + (int(HeightRGBA21.b * 255) << 8) + (int(HeightRGBA21.a * 255));
	int H12 = (int(HeightRGBA12.r * 255) << 24) + (int(HeightRGBA12.g * 255) << 16) + (int(HeightRGBA12.b * 255) << 8) + (int(HeightRGBA12.a * 255));

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

	return H11;
}

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	uint VID = input.VertexID / 6;

	float4 WorldPos = float4(VID / TerrainHeight, (VID % TerrainHeight), 0, 1);
	//WorldPos.xy += TriangleVertices[Indices[input.VertexID % 12]];

	float3 Normal = float3(0, 0, 0);
	float H = GetBilinearHeight(input.VertexID, Normal);

	WorldPos.xy += TriangleVertices[Indices[input.VertexID % 6]];
	WorldPos.xy *= TerrainScale;
	WorldPos.z += H * HeightScale;
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

float3 CalculateTriplanarFloat3(float3 Blending, float3 WorldUV, float2 TexCoord, sampler2D Texture)
{
	float3 AlbedoX0 = tex2D(Texture, WorldUV.zy * float2(TextureSize, TextureSize).xy + TexCoord.xy).rgb * Blending.x;
	float3 AlbedoY0 = tex2D(Texture, WorldUV.xz * float2(TextureSize, TextureSize).xy + TexCoord.xy).rgb * Blending.y;
	float3 AlbedoZ0 = tex2D(Texture, WorldUV.xy * float2(TextureSize, TextureSize).xy + TexCoord.xy).rgb * Blending.z;

	return AlbedoX0 + AlbedoY0 + AlbedoZ0;
}

float CalculateTriplanarFloat(float3 Blending, float3 WorldUV, float2 TexCoord, sampler2D Texture)
{
	float AlbedoX0 = tex2D(Texture, WorldUV.zy * float2(TextureSize, TextureSize).xy + TexCoord.xy).r * Blending.x;
	float AlbedoY0 = tex2D(Texture, WorldUV.xz * float2(TextureSize, TextureSize).xy + TexCoord.xy).r * Blending.y;
	float AlbedoZ0 = tex2D(Texture, WorldUV.xy * float2(TextureSize, TextureSize).xy + TexCoord.xy).r * Blending.z;

	return AlbedoX0 + AlbedoY0 + AlbedoZ0;
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

	float WPX = (WorldPosition.x - ClipMapPosition.x) / ClipMapRange.x;
	float WPY = (WorldPosition.y - ClipMapPosition.y) / ClipMapRange.y;

	float4 ClipHeightRGBA = tex2D(ClipPlaneSampler, float2(WPX, WPY));

	int ClipH = (int(ClipHeightRGBA.r * 255) << 24) + (int(ClipHeightRGBA.g * 255) << 16) + (int(ClipHeightRGBA.b * 255) << 8) + (int(ClipHeightRGBA.a * 255));

	float ClipHeight = ClipH;

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

	float3 WorldUV = abs(WorldPosition % 1.0f);

	float PX = (WorldPosition.x - Position.x) / (TerrainWidth * TerrainScale);
	float PY = (WorldPosition.y - Position.y) / (TerrainHeight * TerrainScale);

	float4 MaterialData0 = tex2D(DataSampler0, float2(PX, PY));
	float4 MaterialData1 = tex2D(DataSampler1, float2(WPX, WPY));

	uint Material = MaterialData0.r * 255;
	uint BlendMaterial = MaterialData0.g * 255;
	uint DecalMaterial = MaterialData0.b * 255;

	float BlendAlpha = MaterialData1.r;
	float DecalAlpha = MaterialData1.g;

	float2 MatTex = float2(Material / 16, Material % 16) * TextureSize;
	float2 SecondMatTex = float2(BlendMaterial / 16, BlendMaterial % 16) * TextureSize;
	float2 DecalMatTex = float2(DecalMaterial / 16, DecalMaterial % 16) * TextureSize;

	float3 Blending = abs(input.Normal);
	Blending = Blending / (Blending.x + Blending.y + Blending.z);

	//main Material
	float3 Albedo = CalculateTriplanarFloat3(Blending, WorldUV, MatTex, DiffuseSampler);
	float3 Normal = CalculateTriplanarFloat3(Blending, WorldUV, MatTex, MaterialNormalSampler);
	float Roughness = CalculateTriplanarFloat(Blending, WorldUV, MatTex, RoughnessSampler);
	float Metalness = CalculateTriplanarFloat(Blending, WorldUV, MatTex, MetalnessSampler);
	//Blend Material
	float3 BlendAlbedo = CalculateTriplanarFloat3(Blending, WorldUV, SecondMatTex, DiffuseSampler);
	float3 BlendNormal = CalculateTriplanarFloat3(Blending, WorldUV, SecondMatTex, MaterialNormalSampler);
	float BlendRoughness = CalculateTriplanarFloat(Blending, WorldUV, SecondMatTex, RoughnessSampler);
	float BlendMetalness = CalculateTriplanarFloat(Blending, WorldUV, SecondMatTex, MetalnessSampler);
	//Decal Material
	float3 DecalAlbedo = CalculateTriplanarFloat3(Blending, WorldUV, DecalMatTex, DecalSampler);
	float3 DecalNormal = CalculateTriplanarFloat3(Blending, WorldUV, DecalMatTex, DecalNormalSampler);
	float DecalRoughness = CalculateTriplanarFloat(Blending, WorldUV, DecalMatTex, DecalRoughnessSampler);
	float DecalMetalness = CalculateTriplanarFloat(Blending, WorldUV, DecalMatTex, DecalMetalnessSampler);
	//Decal
	float DecTol = CalculateTriplanarFloat(Blending, WorldUV, DecalMatTex, DecalToleranceSampler);

	float3 BaseAlbedo = lerp(Albedo, BlendAlbedo, BlendAlpha);
	float3 BaseNormal = lerp(Normal, BlendNormal, BlendAlpha);
	float3 BaseRoughness = lerp(Roughness, BlendRoughness, BlendAlpha);
	float3 BaseMetalness = lerp(Metalness, BlendMetalness, BlendAlpha);

	int AboveTolerance = min(floor(DecTol / DecalAlpha), 1.0);

	float3 FinalAlbedo = BaseAlbedo * AboveTolerance + DecalAlbedo * (1.0f - AboveTolerance);
	float3 FinalNormal = BaseNormal * AboveTolerance + DecalNormal * (1.0f - AboveTolerance);
	FinalNormal = FinalNormal * 2.0f - 1.0f;
	FinalNormal = normalize(mul(input.TangentBasis, FinalNormal));

	float FinalRoughness = BaseRoughness * AboveTolerance + DecalRoughness * (1.0f - AboveTolerance);
	float FinalMetalness = BaseMetalness * AboveTolerance + DecalMetalness * (1.0f - AboveTolerance);

	//Plugin Final Values into BDFR
	float3 LO = normalize(CameraPosition - input.WorldSpacePosition);
	float3 LI = normalize(LightPosition - input.WorldSpacePosition);
	float3 LH = normalize(LO + LI);
	float CosTheta = max(dot(LH, LO), 0.0);
	float CosLi = max(dot(FinalNormal, LI), 0.0f);
	float CosLh = max(dot(FinalNormal, LH), 0.0f);
	float CosLo = max(dot(FinalNormal, LO), 0.0f);

	float3 F0 = lerp(float3(0.04f, 0.04f, 0.04f), FinalAlbedo, FinalMetalness);

	float3 F = GetFresnel(CosTheta, F0);
	float D = GetNormalDistribution(CosLh, FinalRoughness);
	float G = GetGeometrySmith(CosLi, CosLo, FinalRoughness);

	float3 KD = max(lerp(float3(1, 1, 1) - F, float3(0, 0, 0), FinalMetalness), 0.0f);
	float3 Diffuse = KD * FinalAlbedo;
	float3 Specular = (F * D * G) / max(4.0 * CosLi * CosLo, Epsilon);
	float3 DirectLighting = (Diffuse + Specular) * LightColor * CosLi;

	//Ambient
	float3 F2 = saturate(GetFresnel(CosLo, F0));
	float3 KD2 = lerp(1.0 - F2, 0.0, FinalMetalness);
	float3 diffuseIBL = KD2 * FinalAlbedo * AmbientLightColor;

	float4 Color = float4(DirectLighting + diffuseIBL, 1.0f);
	//Color = Color * 0.001f + float4(FinalNormal,1.0f);

	//Color = Color * 0.001f + float4(Albedo, 1.0f);

	//circle,square
	float HasBrush = 0;
	HasBrush += min(1.0f, floor(BrushDistance / distance(BrushPosition.xy, WorldPosition.xy))) * (1.0f - min(1.0f, BrushShape)) * min(1.0f, BrushDistance);
	HasBrush += min(1.0f, floor(BrushDistance / max(distance(BrushPosition.x, WorldPosition.x), distance(BrushPosition.y, WorldPosition.y)))) * (1.0f - min(1.0f, 1.0f - BrushShape)) * min(1.0f, BrushDistance);

	Color.rgb = Color.xyz * (1.0f - HasBrush) + float3(lerp(Color.xyz, BrushHighLightColor.xyz, BrushHighLightColor.w)) * HasBrush;

	return float4(Color.rgb, 1.0f);

	//return float4(Color.xyz, 1.0) * (1.0f - HasBrush) + float4(lerp(Color.xyz, BrushHighLightColor.xyz, BrushHighLightColor.w), 1.0f) * HasBrush;


}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};