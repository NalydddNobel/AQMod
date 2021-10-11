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

float2 minMaxRGB(float3 rgb)
{
    return float2(min(min(rgb.r, rgb.g), rgb.b), max(max(rgb.r, rgb.g), rgb.b));
}

float L(float2 minMax)
{
    return (minMax.y + minMax.x) / 2;
}

float FrameYFix(float2 coords)
{
    float frameSizeY = uSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return y * 1 / frameSizeY;
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
    float textureY = FrameYFix(coords);
    float time = uTime * 0.2;
    float4 mergeColor = tex2D(uImage1, float2((time + coords.x * 0.5) % 1, (time * 0.75 + textureY * 0.5) % 1));
    mergeColor *= L(minMaxRGB(float3(mergeColor.r, mergeColor.g, mergeColor.b)));
    mergeColor.a = 1;
    return color * sampleColor + (mergeColor * 0.5);
}

float4 Monitor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float time = uTime;
    float r = (sin(origColor.r + time + coords.y * 10) + 1) / 2;
    origColor.r = origColor.r - (r - origColor.r) * origColor.a;
    float g = (sin(origColor.g + time * 2 + sin(coords.y * 15 + coords.x * 10) * 10) + 1) / 2;
    origColor.g = origColor.g - (r - origColor.g) * origColor.a;
    float b = (sin(origColor.b + time * 2 + sin(coords.y * 11 + coords.x * 24) * 10) + 1) / 2;
    origColor.b = origColor.b - (r - origColor.b) * origColor.a;
    return origColor * sampleColor;
}

float4 ShieldBeams(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float3 MERGEColor = uColor;
    float intensity = ((sin((color.r * color.g * color.b) * 100 + uTime * 20 + coords.x * 10 + uWorldPosition.x / 16 * uDirection) + 1) / 2) * uOpacity;
    float4 mergeColor = float4(MERGEColor.r * color.a, MERGEColor.g * color.a, MERGEColor.b * color.a, color.a);
    if (color.r < uLightSource.r * color.a)
    {
        color.r = uLightSource.r * color.a;
    }
    if (color.g < uLightSource.g * color.a)
    {
        color.g = uLightSource.g * color.a;
    }
    if (color.b < uLightSource.b * color.a)
    {
        color.b = uLightSource.b * color.a;
    }
    color *= float4(uSecondaryColor.r, uSecondaryColor.g, uSecondaryColor.b, 1);
    color = lerp(color, mergeColor, float4(intensity, intensity, intensity, intensity));
    intensity *= intensity;
    intensity *= intensity;
    color = lerp(color, float4(1 * color.a, 1 * color.a, 1 * color.a, color.a), float4(intensity, intensity, intensity, intensity));
    return color * sampleColor;
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
    pass MonitorPass
    {
        PixelShader = compile ps_2_0 Monitor();
    }
    pass ShieldBeamsPass
    {
        PixelShader = compile ps_2_0 ShieldBeams();
    }
}