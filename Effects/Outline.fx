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

bool isBlank(float4 color)
{
    return color.a == 0 && color.r == 0 && color.g == 0 && color.b == 0;
}

float4 Outline(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(1, 1, 1, 1) * sampleColor;
        }
    }
    return color * sampleColor;
}

float4 OutlineAlpha(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(1, 1, 1, 1) * sampleColor.a;
        }
    }
    return color * sampleColor;
}

float4 OutlineColor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(1 * uColor.r, 1 * uColor.g, 1 * uColor.b, 1) * sampleColor;
        }
    }
    return color * sampleColor;
}

float4 OutlineColorAlpha(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(1 * uColor.r, 1 * uColor.g, 1 * uColor.b, 1) * sampleColor.a;
        }
    }
    return color * sampleColor;
}

technique Technique1
{
    pass OutlinePass
    {
        PixelShader = compile ps_2_0 Outline();
    }

    pass OutlineAlphaPass
    {
        PixelShader = compile ps_2_0 OutlineAlpha();
    }

    pass OutlineColorPass
    {
        PixelShader = compile ps_2_0 OutlineColor();
    }

    pass OutlineColorAlphaPass
    {
        PixelShader = compile ps_2_0 OutlineColorAlpha();
    }
}