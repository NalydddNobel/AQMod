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

float colorLerpMult;
float3 thirdColor;

float4 DemonicPortalHardcoded(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - 0.5);
    float dY = (coords.y - 0.5);
    if (dX > 0)
    {
        dX = 0 - dX;
    }
    if (dX > 0)
    {
        dX = 0 - dX;
    }
    float cVALUE = sqrt(dX * dX + dY * dY);
    cVALUE += sin(uTime * 6.66 + coords.y * 20 + sin(dX * 6.28) * 10) * 0.1 * (1 - coords.y);
    if (cVALUE - 0.1 < 0.2)
    {
        cVALUE = cVALUE - 0.2;
        cVALUE = cVALUE * 10;
        if (cVALUE > 0)
        {
            float a = 1 - cVALUE;
            float3 clr = lerp(float3(1, 0, 0), float3(0.8f, 0, 0.4f), cVALUE * uSaturation) * uOpacity * 5;
            clr = lerp(clr, float3(0.4f, 0.8f, 0) * uOpacity * 5, coords.y * (0.45f + sin(uTime * 10) * 0.1f));
            return float4(cVALUE * clr.r * a, cVALUE * clr.g * a, cVALUE * clr.b * a, a);
        }
        return float4(0, 0, 0, 1);
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
    if (dX > 0)
    {
        dX = 0 - dX;
    }
    float cVALUE = sqrt(dX * dX + dY * dY);
    cVALUE += sin(uTime * 6.66 + coords.y * 20 + sin(dX * 6.28) * 10) * 0.1 * (1 - coords.y);
    if (cVALUE - 0.1 < 0.2)
    {
        cVALUE = cVALUE - 0.2;
        cVALUE = cVALUE * 10;
        if (cVALUE > 0)
        {
            float a = 1 - cVALUE;
            float3 clr = lerp(uColor, uSecondaryColor, cVALUE * uSaturation) * uOpacity;
            clr = lerp(clr, thirdColor * uOpacity, coords.y * colorLerpMult);
            return float4(cVALUE * clr.r * a, cVALUE * clr.g * a, cVALUE * clr.b * a, a);
        }
        return float4(0, 0, 0, 1);
    }
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass DemonicPortalPass
    {
        PixelShader = compile ps_2_0 DemonicPortalHardcoded();
    }
    pass DemonicPortalPass2
    {
        PixelShader = compile ps_2_0 DemonicPortal();
    }
}