sampler2D Tiles : register(s0);
sampler2D Glows : register(s1);

float glowMagnitude;
float outlineGlowMagnitude;
float2 imageSize;

bool anySquare(float2 uv, sampler2D input)
{
    float2 pixelsToUV = float2(2.0f / imageSize.x, 2.0f / imageSize.y);
    return tex2D(input, uv + float2(0, -pixelsToUV.y)).a <= 0 || tex2D(input, uv + float2(0, pixelsToUV.y)).a <= 0 || tex2D(input, uv + float2(pixelsToUV.x, 0)).a <= 0 || tex2D(input, uv + float2(-pixelsToUV.x, 0)).a <= 0;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(Tiles, uv);
    float4 maskColor = tex2D(Glows, uv);
    float glow = anySquare(uv, Tiles) ? outlineGlowMagnitude : glowMagnitude;
	
    return maskColor * color.a * glow;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}