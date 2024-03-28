sampler Texture : register(s0);
sampler Texture2 : register(s1);

float4 main(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(Texture, coords);
    float4 color2 = tex2D(Texture2, coords);

    return color * color2 * sampleColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}