sampler TextureSampler : register(s0);

float BlurStrength;

float4 blurPixelShader(float2 input : TEXCOORD0) : COLOR0
{
	float4 output;

	output = tex2D(TextureSampler, input.xy);

	for (float value = 0; value < 7.0; value++)
	{
		output += tex2D(TextureSampler, float2(input.x + (value/2500.0f), input.y + (value/2500.0f)));
		output += tex2D(TextureSampler, float2(input.x + (value/2500.0f), input.y - (value/2500.0f)));
		output += tex2D(TextureSampler, float2(input.x - (value/2500.0f), input.y - (value/2500.0f)));
		output += tex2D(TextureSampler, float2(input.x - (value/2500.0f), input.y + (value/2500.0f)));
	}

	output = output/(value);

    return output;
}

technique blurGaussian
{
    Pass pass1
    {
        PixelShader  = compile ps_2_0 blurPixelShader();
    }
}