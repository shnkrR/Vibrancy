sampler TextureSampler : register(s0);

float DepthofField;

float4 blurPixelShader(float2 input : TEXCOORD0) : COLOR0
{
	float4 output;

	output = tex2D(TextureSampler, input.xy);

	for (float value = 0; value < 7; value++)
	{
		output += tex2D(TextureSampler, float2(input.x + (value/2500), input.y + (value/2500)));
		output += tex2D(TextureSampler, float2(input.x + (value/2500), input.y - (value/2500)));
		output += tex2D(TextureSampler, float2(input.x - (value/2500), input.y - (value/2500)));
		output += tex2D(TextureSampler, float2(input.x - (value/2500), input.y + (value/2500)));
	}

	output = output/(value);
	//output = output/(2);

    return output;
}

// Pixel shader combines the bloom image with the original
// scene, using tweakable intensity levels and saturation.
// This is the final step in applying a bloom postprocess.

sampler BloomSampler : register(s1);
sampler BaseSampler : register(s2);

float BloomIntensity = 0.5;
float BaseIntensity = 1.0;

float BloomSaturation = 0.0;
float BaseSaturation = 1.0;


// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the bloom and original base image colors.
    float4 bloom = tex2D(BloomSampler, texCoord);
    float4 base = tex2D(BaseSampler, texCoord);
    
    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // Combine the two images.
    return base + bloom;
}

technique blurGaussian
{
    Pass pass1
    {
        PixelShader  = compile ps_2_0 blurPixelShader();
    }
}

technique BloomCombine
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}


