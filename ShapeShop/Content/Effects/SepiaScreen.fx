uniform extern texture ScreenTexture;    

sampler ScreenS = sampler_state
{
    Texture = <ScreenTexture>;    
};

float4 PixelShaderFunction(float4 position : POSITION0, float4 c : COLOR0, float2 texCoord : TEXCOORD0) : COLOR
{
	float4 color = tex2D(ScreenS, texCoord);

	float value = (color.r + color.g + color.b) / 3;
	color.r = value;
	color.g = value;
	color.b = value;

	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();        
    }
}