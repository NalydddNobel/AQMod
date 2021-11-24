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

float4 Censor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixel = coords * uImageSize0;
    int pixelSize = uOpacity;
    int x = pixel.x % pixelSize;
    if (x > 0)
    {
        pixel.x -= x;
    }
    int y = pixel.y % pixelSize;
    if (y > 0)
    {
        pixel.y -= y;
    }
    return tex2D(uImage0, pixel / uImageSize0) * sampleColor;
}

technique Technique1
{
    pass CensorTechnique
    {
        PixelShader = compile ps_2_0 Censor();
    }
}