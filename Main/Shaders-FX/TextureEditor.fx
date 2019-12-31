#define PS_SHADERMODEL ps_4_0

float4 Color;
float ColorFloat;
float LerpValue;

float TextureWidth0;
float TextureWidth1;
float TextureHeight0;
float TextureHeight1;

Texture2D InputTexture;
sampler2D InputTextureSampler = sampler_state
{
	Texture = <InputTexture>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Clamp;
	AddressV = Clamp;
};

Texture2D InputTexture2;
sampler2D InputTextureSampler2 = sampler_state
{
	Texture = <InputTexture2>;
	magfilter = None;
	minfilter = None;
	mipfilter = None;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 Coords : TEXCOORD0;
};

float4 GenerateNormalsPass(VertexShaderOutput input) : COLOR
{
	float2 TexCoord = input.Coords;
	float2 Neighbor = float2(1.0f / TextureWidth0, 1.0f / TextureHeight0);

	float4 HeightRGBA00 = tex2Dlod(InputTextureSampler, float4(TexCoord.x, TexCoord.y, 0, 0)).rgba;
	float4 HeightRGBA10 = tex2Dlod(InputTextureSampler, float4(TexCoord.x + Neighbor.x, TexCoord.y, 0, 0)).rgba;
	float4 HeightRGBA01 = tex2Dlod(InputTextureSampler, float4(TexCoord.x, TexCoord.y + Neighbor.y, 0, 0)).rgba;

	int H00 = (int(HeightRGBA00.r * 255) << 24) + (int(HeightRGBA00.g * 255) << 16) + (int(HeightRGBA00.b * 255) << 8) + (int(HeightRGBA00.a * 255));
	int H10 = (int(HeightRGBA10.r * 255) << 24) + (int(HeightRGBA10.g * 255) << 16) + (int(HeightRGBA10.b * 255) << 8) + (int(HeightRGBA10.a * 255));
	int H01 = (int(HeightRGBA01.r * 255) << 24) + (int(HeightRGBA01.g * 255) << 16) + (int(HeightRGBA01.b * 255) << 8) + (int(HeightRGBA01.a * 255));

	float3 Normal = normalize(cross(float3(1, 0, H10 - H00) , float3(0.0, 1, H01 - H00)));
	Normal *= Normal.rgb * 0.001f;
	Normal.rgb = HeightRGBA00;

	return float4(Normal, 1.0f);
}

float4 MergeMinFloatPass(VertexShaderOutput input) : COLOR
{
	return min(tex2D(InputTextureSampler, input.Coords).r,tex2D(InputTextureSampler2, input.Coords).r);
}

float4 MergeMaxFloatPass(VertexShaderOutput input) : COLOR
{
	return max(tex2D(InputTextureSampler, input.Coords).r,tex2D(InputTextureSampler2, input.Coords).r);
}

float4 CopyTexturePass(VertexShaderOutput input) : COLOR
{
	return tex2D(InputTextureSampler, input.Coords);
}

float4 FillColorPass(VertexShaderOutput input) : COLOR
{
	return Color;
}

float4 FillFloatPass(VertexShaderOutput input) : COLOR
{
	return float4(ColorFloat,0,0,1.0f);
}

technique GenerateNormals
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL GenerateNormalsPass();
	}
}

technique MergeMinFloat
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL MergeMinFloatPass();
	}
}

technique MergeMaxFloat
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL MergeMaxFloatPass();
	}
}

technique CopyTexture
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL CopyTexturePass();
	}
}

technique FillColor
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL FillColorPass();
	}
}

technique FillFloat
{
	pass Pass0
	{
		PixelShader = compile PS_SHADERMODEL FillFloatPass();
	}
}
