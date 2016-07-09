float4x4 World;
float4x4 Projection;
float4x4 View;

float4x4 ViewProjection;
float3 CameraPos;
float3 blockPos[20];

float4x4 WorldInverse;

Texture2D DiffuseTexture;

bool IsLightingEnabled;

float4 AmbientColor;
float AmbientIntensity;

float3 LightDirection;

float4 DiffuseColor;
float DiffuseIntensity;

float Shine;
float4 SpecularColor;    
float SpecularIntensity;

float3 ViewVector;

float4 ColorFromSource;

sampler2D DiffuseSampler = sampler_state
{
	Texture = <DiffuseTexture>;
	magfilter = ANISOTROPIC;
    minfilter = ANISOTROPIC;
};

struct VsIN
{
    float4 Position : POSITION0;
	float4 Normal	: NORMAL0;
	float2 UV       : TEXCOORD0;
};

struct VsOUT
{
    float4 Position : POSITION0;
	float4 Color	: COLOR0;
	float2 UV       : TEXCOORD0;   
	float3 Normal : TEXCOORD1;
};

VsOUT TexturedLitVertexShader(VsIN input)
{
    VsOUT output  = (VsOUT)1;

    output.Position = mul(input.Position, World);    
	output.Position = mul(output.Position, View);
	output.Position = mul(output.Position, Projection);

	float4 normal = mul(input.Normal, transpose(WorldInverse));
    float lightIntensity = dot(normal, LightDirection);
    output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	//output.Color = (0.0f, 0.0f, 100.0f, 255.0f);// * output.Color;

	output.Normal = normalize(normal);
	normalize(LightDirection);

	output.UV = input. UV;

	return output;
}

float4 TexturedLitPixelShader(VsOUT input) : COLOR0
{
	float4 outColor;
	outColor = ColorFromSource;
	float4 position = input.Position;

	outColor = tex2D(DiffuseSampler, float2(input.UV.x, input.UV.y)) * (outColor);

	//light
	float3 normalizedLightVec = LightDirection;
	float3 normal = input.Normal;

	float3 reflectionVector = (normalize(2 * dot(normalizedLightVec, normal) * normal - normalizedLightVec));
    float3 viewPostion		= normalize(mul(normalize(ViewVector), World));

	float dotProduct = dot(reflectionVector, viewPostion);

	float4 specularColor = (SpecularIntensity * SpecularColor * max(pow(dotProduct, Shine), 0) * length(input.Color / 2));

	outColor *= saturate(input.Color + AmbientColor * AmbientIntensity + specularColor);

    return outColor;
}

technique DefaultEffect
{
    Pass pass1
    {
		VertexShader = compile vs_2_0 TexturedLitVertexShader();
        PixelShader  = compile ps_2_0 TexturedLitPixelShader();
    }
}
