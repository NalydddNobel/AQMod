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

float FrameCoordinateFix(float source, float endSource, float position)
{
    return (position - source) / (endSource - source);
}

float2 minMaxRGB(float3 rgb)
{
    return float2(min(min(rgb.r, rgb.g), rgb.b), max(max(rgb.r, rgb.g), rgb.b));
}

float L(float2 minMax)
{
    return (minMax.y + minMax.x) / 2;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 mergeColor = tex2D(uImage0, float2((uTime * 0.5 + coords.x) % 1, (uTime * 0.5 + coords.y) % 1));
    mergeColor.a = color.a;
    if (mergeColor.a == 0)
    {
        return color * sampleColor;
    }
    return mergeColor * sampleColor;
}

float4 ImageScroll(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a == 0)
    {
        return color * sampleColor;
    }
    float textureY = FrameCoordinateFix(uSourceRect.y, uSourceRect.y + uSourceRect.w, coords.y);
    float time = uTime * 0.2;
    float4 mergeColor = tex2D(uImage1, float2((time + coords.x * 0.5) % 1, (time * 0.75 + textureY * 0.5) % 1));
    mergeColor *= L(minMaxRGB(float3(mergeColor.r, mergeColor.g, mergeColor.b)));
    mergeColor.a = 1;
    return color * sampleColor + (mergeColor * 0.5);
}

technique Technique1
{
    pass ScrollPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
    pass ImageScrollPass
    {
        PixelShader = compile ps_2_0 ImageScroll();
    }
}