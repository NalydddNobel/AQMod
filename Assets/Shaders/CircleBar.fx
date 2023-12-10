sampler uImage0 : register(s0);
float uOpacity;

float4 main(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 topLeftCoords = coords - float2(0.5, 0.5);
    float2 p = -1.0 + 2.0 * coords;
    float r = sqrt(dot(p, p));
    float textureUVX = abs(sin(atan2(topLeftCoords.y, topLeftCoords.x) / 2));
    
    if (textureUVX > uOpacity)
    {
        return float4(0, 0, 0, 0);
    }
    
    float4 color = tex2D(uImage0, float2(textureUVX * 0.99, r * 0.99));

    return color * sampleColor;
}

technique Technique1
{
    pass Main
    {
        PixelShader = compile ps_2_0 main();
    }
}