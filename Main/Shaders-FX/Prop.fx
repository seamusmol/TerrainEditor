
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

matrix WorldViewProjection;
float3 Position;

float3 AmbientLightColor = float3(0.25f, 0.25f, 0.25f);
float3 LightColor = float3(0.788f, 0.886f, 1.0f);

float3 CameraPosition;
float3 LightPosition;

float ClipDirection;
float ClipOffset;

float3 ClipMapPosition;
float4 ClipMapRange;

static const float PI = 3.141592f;
static const float Epsilon = 0.00001f;

static const float3 HighLightColors[] =
{
	float3(0,0,0),
	float3(0,0,1),
	float3(0,1,0),
	float3(1,0,0),
	float3(1.0f,0.63f,0)
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

Texture2D NormalMap;
sampler2D NormalSampler = sampler_state
{
	Texture = <NormalMap>;
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

struct VertexInstanceShaderInput
{
	float3 InstancePosition : NORMAL0;
	float3 InstanceForward : NORMAL1;
	float3 InstanceUp : NORMAL2;
	float3 InstanceLeft : NORMAL3;
	float3 Scale : NORMAL4;
	uint4 PropData: TANGENT0;
};

struct VertexShaderInput
{
	float3 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL5;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 WorldSpacePosition: POSITION1;
	float2 TexCoord : TEXCOORD4;
	float3 Normal : TEXCOORD6;
	float3x3 TangentBasis : TBASIS;
	uint PropData0 : TEXCOORD5;
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

VertexShaderOutput MainVS(in VertexShaderInput input, VertexInstanceShaderInput instanceInput)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 InputPosition = float4(input.Position.xyz * instanceInput.Scale.xyz, 0.0f);
	float4 InstancePosition = float4(instanceInput.InstancePosition.xyz, 1.0f);

	float4x4 World;
	World[0] = float4(instanceInput.InstanceForward, 0.0f);
	World[1] = float4(instanceInput.InstanceUp, 0.0f);
	World[2] = float4(instanceInput.InstanceLeft, 0.0f);
	World[3] = float4(InstancePosition);

	float4 WorldSpacePosition = mul(InputPosition, World) + InstancePosition;
	float4 ScreenSpacePosition = mul(WorldSpacePosition, WorldViewProjection);

	float3 Tangent = float3(0, 0, 0);
	float3 BiTangent = float3(0, 0, 0);
	GenerateTanBiTan(input.Normal, Tangent, BiTangent);

	//float3x3 TBN = float3x3(input.Tangent, cross(input.Normal, input.Tangent), input.Normal);

	float3x3 TBN = float3x3(Tangent, BiTangent, input.Normal);
	TBN = mul((float3x3)World, transpose(TBN));

	output.TangentBasis = TBN;

	output.TexCoord = input.TexCoord;
	output.Position = ScreenSpacePosition;
	output.WorldSpacePosition = WorldSpacePosition;
	output.Normal = input.Normal;
	//output.Tangent = input.Tangent;

	output.PropData0 = instanceInput.PropData.x;

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

	float WPX = (WorldPosition.x - ClipMapPosition.x) / ClipMapRange.x;
	float WPY = (WorldPosition.y - ClipMapPosition.y) / ClipMapRange.y;

	float4 ClipHeightRGBA = tex2D(ClipPlaneSampler, float2(WPX, WPY));
	int ClipH = (int(ClipHeightRGBA.r * 255) << 24) + (int(ClipHeightRGBA.g * 255) << 16) + (int(ClipHeightRGBA.b * 255) << 8) + (int(ClipHeightRGBA.a * 255));

	float ClipHeight = ClipH;

	ClipHeight *= ClipMapRange.z;
	ClipHeight -= ClipMapRange.w;

	if (ClipDirection == -1)
	{
		if (input.WorldSpacePosition.z > ClipHeight + ClipOffset)
		{
			discard;
		}
	}

	if (ClipDirection == 1)
	{
		if (input.WorldSpacePosition.z < ClipHeight - ClipOffset)
		{
			discard;
		}
	}

	float3 Albedo = tex2D(DiffuseSampler, input.TexCoord).rgb;
	float Roughness = tex2D(RoughnessSampler, input.TexCoord);
	float Metalness = tex2D(MetalnessSampler, input.TexCoord).r;
	float3 Normal = normalize(tex2D(NormalSampler, input.TexCoord).rgb * 2.0f - 1.0f);
	Normal = normalize(mul(input.TangentBasis, Normal));

	float3 LO = normalize(CameraPosition - input.WorldSpacePosition);
	float3 LI = normalize(LightPosition - input.WorldSpacePosition);
	float3 LH = normalize(LO + LI);

	float CosLi = max(dot(Normal, LI), 0.0f);
	float CosLh = max(dot(Normal, LH), 0.0f);
	float CosLo = max(dot(Normal, LO), 0.0f);

	float CosTheta = max(dot(LH, LO), 0.0);
	float3 F0 = lerp(float3(0.04f, 0.04f, 0.04f), Albedo, Metalness);

	float3 F = GetFresnel(CosTheta, F0);
	float D = GetNormalDistribution(CosLh, Roughness);
	float G = GetGeometrySmith(CosLi, CosLo, Roughness);


	float3 KD = lerp(float3(1, 1, 1) - F, float3(0, 0, 0), Metalness);

	float3 Diffuse = KD * Albedo;

	float3 Specular = (F * D * G) / max(4.0 * CosLi * CosLo, Epsilon);

	float3 DirectLighting = (Diffuse + Specular) * LightColor * CosLi;

	//Ambient
	float3 F2 = saturate(GetFresnel(CosLo, F0));
	float3 KD2 = lerp(1.0 - F2, 0.0, Metalness);

	float3 diffuseIBL = KD2 * Albedo * AmbientLightColor;

	float3 SpecularIBL = Albedo * LightColor * Metalness * CosLi;

	float4 Color = float4(DirectLighting + diffuseIBL + SpecularIBL, 1.0f);

	int HasHighLight = min(input.PropData0, 1);

	Color = Color * (1.0f - HasHighLight) + float4(lerp(Color.rgb, HighLightColors[input.PropData0], 0.5f), 1.0f) * HasHighLight;

	//TODO
	//Add HighLight

	//Color = Color * 0.001f + float4(Albedo.rgb, 1.0f);
	//Color = Color * 0.001f + float4(input.Tangent.rgb, 1.0f);
	//Color = Color * 0.001f + float4(input.Normal.rgb, 1.0f);
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