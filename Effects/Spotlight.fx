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

float4 MonoSpotlight(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - 0.5) / 0.5;
    double dY = (coords.y - 0.5) / 0.5;
    double c = sqrt(dX * dX + dY * dY);
    if (c == 0)
    {
        return float4(1, 1, 1, 1);
    }
    else
    {
        float value = 1 - (float) c;
        return float4(value, value, value, value);
    }
}

float4 Spotlight(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - 0.5) / 0.5;
    double dY = (coords.y - 0.5) / 0.5;
    double c = sqrt(dX * dX + dY * dY);
    if (c == 0)
    {
        return float4(1, 1, 1, 1) * sampleColor;
    }
    else
    {
        float value = 1 - (float) c;
        return float4(value, value, value, value) * sampleColor;
    }
}

float4 Fade(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a != 0)
    {
        return float4(coords.x * sampleColor.r, coords.x * sampleColor.g, coords.x * sampleColor.b, color.a * sampleColor.a);
    }
    return color * sampleColor;
}

float4 FadeYProgress(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a != 0)
    {
        float y = coords.y * uOpacity;
        if (y <= 0)
        {
            return float4(0, 0, 0, color.a * sampleColor.a);
        }
        return float4(y * sampleColor.r, y * sampleColor.g, y * sampleColor.b, color.a * sampleColor.a);
    }
    return color * sampleColor;
}

float4 FadeYProgressAlpha(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a != 0)
    {
        float y = coords.y * uOpacity;
        y = uOpacity - y;
        y -= 0.01;
        if (y <= 0)
        {
            return float4(0, 0, 0, 0);
        }
        return float4(y * color.r * sampleColor.r, y * color.g * sampleColor.g, y * color.b * sampleColor.b, y * color.a * sampleColor.a);
    }
    return color * sampleColor;
}

float4 SpikeFade(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a != 0)
    {
        float y = coords.y * uOpacity;
        y = uOpacity - y;
        y -= 0.01;
        if (y <= 0)
        {
            return float4(0, 0, 0, 0);
        }
        return float4(y * color.r * sampleColor.r, y * color.g * sampleColor.g, y * color.b * sampleColor.b, y * color.a * sampleColor.a);
    }
    return color * sampleColor;
}

technique Technique1
{
    pass SpotlightPass
    {
        PixelShader = compile ps_2_0 Spotlight();
    }
    pass MonoSpotlightPass
    {
        PixelShader = compile ps_2_0 MonoSpotlight();
    }
    pass FadePass
    {
        PixelShader = compile ps_2_0 Fade();
    }
    pass FadeYProgressPass
    {
        PixelShader = compile ps_2_0 FadeYProgress();
    }
    pass FadeYProgressAlphaPass
    {
        PixelShader = compile ps_2_0 FadeYProgressAlpha();
    }
    pass SpikeFadePass
    {
        PixelShader = compile ps_2_0 SpikeFade();
    }
}