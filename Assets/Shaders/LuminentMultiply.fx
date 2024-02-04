sampler Texture : register(s0);

float4 main(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(Texture, coords);
    float brightness = (color.r + color.g + color.b) / 3;

    return brightness < 0.5 ? float4((color * brightness * 2).rgb, color.a) * sampleColor : lerp(color * sampleColor, float4(1, 1, 1, color.a), (brightness - 0.5) * 2);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}