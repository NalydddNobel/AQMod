sampler2D Tiles : register(s0);
sampler2D Glows : register(s1);

float glowMagnitude;
float outlineGlowMagnitude;
float2 imageSize;
float2 glowOffsetUV;
const float pixelSize = 2;

bool anyA(float2 uv)
{
    return tex2D(Tiles, uv).a < 1;
}

bool anySquare(float2 uv, sampler2D input)
{
    float2 pixelsToUV = float2(pixelSize / imageSize.x, pixelSize / imageSize.y);
    float2 up = float2(0, -pixelsToUV.y);
    float2 down = float2(0, pixelsToUV.y);
    float2 left = float2(-pixelsToUV.y, 0);
    float2 right = float2(pixelsToUV.y, 0);

    return anyA(uv + up) || anyA(uv + down) || anyA(uv + left) || anyA(uv + right);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(Tiles, uv);
    float4 maskColor = tex2D(Glows, uv + glowOffsetUV);
    float glow = anySquare(uv, Tiles) ? outlineGlowMagnitude : glowMagnitude;
	
    return maskColor * (int) color.a * glow;
}

float4 water(float2 uv : TEXCOORD) : COLOR
{
    float2 pixelsToUV = float2(pixelSize / imageSize.x, pixelSize / imageSize.y);
    float2 up = float2(0, -pixelsToUV.y);
    float4 color = tex2D(Tiles, uv);
    float4 maskColor = tex2D(Glows, uv + glowOffsetUV);
    float glow = anyA(uv + up) ? outlineGlowMagnitude : glowMagnitude;
	
    return maskColor * (int) color.a * glow;
}

float4 refraction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(Tiles, uv);
    float2 maskUV = abs((uv + float2(imageSize.x, imageSize.y)) % 1);
    float4 maskColor = tex2D(Glows, maskUV);
	
    return color * sampleColor + color * maskColor * sampleColor * glowMagnitude;
}

technique Technique1
{
    pass Tile
    {
        PixelShader = compile ps_2_0 main();
    }
    pass Water
    {
        PixelShader = compile ps_2_0 water();
    }
    pass Refraction
    {
        PixelShader = compile ps_2_0 refraction();
    }
}