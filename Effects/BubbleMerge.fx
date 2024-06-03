sampler Texture : register(s0);
sampler OutlineTexture : register(s1);

float4 main(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(Texture, coords);
    float4 outlineColor = tex2D(OutlineTexture, coords);

    return outlineColor * (1 - color.a) * sampleColor.a;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}