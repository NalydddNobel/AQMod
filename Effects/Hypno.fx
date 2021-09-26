sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

float FrameYFix(float2 coords)
{
    float frameSizeY = uSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return y * 1 / frameSizeY;
}

float4 Hypno(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float offset = sin(uTime + FrameYFix(coords.y) * 25) * 0.05 * uDirection;
    color.r = tex2D(uImage0, float2(coords.x + offset, coords.y)).r;
    color.b = tex2D(uImage0, float2(coords.x - offset, coords.y)).b;
    return color * sampleColor;
}

float4 BreakSprite(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float y = FrameYFix(coords.y);
    float pixelSize = 1 / uImageSize0;
    float yPixel = y / pixelSize;
    float offset = sin(uTime + y * 25) * 0.05 * uDirection;
    if (yPixel % 8 < 4)
    {
        color = tex2D(uImage0, float2(coords.x + pixelSize * 4, coords.y));
    }
    else
    {
        color = tex2D(uImage0, float2(coords.x - pixelSize * 4, coords.y));
    }
    return color * sampleColor;
}

float4 Simplify(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelSize = 1 / uImageSize0;
    float4 color = tex2D(uImage0, coords);
    float4 color2 = tex2D(uImage0, coords + float2(pixelSize.x * 2, 0));
    color.r = sqrt(color.r) * sqrt(color2.r);
    color.g = sqrt(color.g) * sqrt(color2.g);
    color.b = sqrt(color.b) * sqrt(color2.b);
    color2 = tex2D(uImage0, coords - float2(pixelSize.x * 2, 0));
    color.r = sqrt(color.r) * sqrt(color2.r);
    color.g = sqrt(color.g) * sqrt(color2.g);
    color.b = sqrt(color.b) * sqrt(color2.b);
    return color * sampleColor;
}

technique Technique1
{
    pass HypnoPass
    {
        PixelShader = compile ps_2_0 Hypno();
    }
    pass BreakSpritePass
    {
        PixelShader = compile ps_2_0 BreakSprite();
    }
    pass SimplifyPass
    {
        PixelShader = compile ps_2_0 Simplify();
    }
}