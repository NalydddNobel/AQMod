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

float4 hellportal(float dX, float dY, float coordsY, float3 clr) : COLOR0
{
    if (dX > 0)
    {
        dX = 0 - dX;
    }
    float cVALUE = sqrt(dX * dX + dY * dY);
    if (cVALUE < 0.2)
    {
        return float4(0, 0, 0, 1);
    }
    cVALUE += sin(uTime * 6.66 + coordsY * 20 + sin(dX * 6.28) * 10) * 0.1 * (1 - coordsY);
    if (cVALUE - 0.1 < 0.2)
    {
        cVALUE = cVALUE - 0.2;
        cVALUE = 1 - cVALUE * 16;
        return float4((1 - cVALUE) * clr.r, (1 - cVALUE) * clr.g, (1 - cVALUE) * clr.b, cVALUE);
    }
    return float4(0, 0, 0, 0);
}

float4 DemonicPortal(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - 0.5);
    float dY = (coords.y - 0.5);
    if (dX > 0)
    {
        dX = 0 - dX;
    }
    return hellportal(dX, dY, coords.y, uColor);
}

float4 SpiderPortal(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - 0.5);
    float dY = (coords.y - 0.5);
    if (dX < 0)
    {
        dX = -dX;
    }
    return hellportal(dX, dY, coords.y, uColor);
}

technique Technique1
{
    pass DemonicPortalPass
    {
        PixelShader = compile ps_2_0 DemonicPortal();
    }
    pass SpiderPortalPass
    {
        PixelShader = compile ps_2_0 SpiderPortal();
    }
}