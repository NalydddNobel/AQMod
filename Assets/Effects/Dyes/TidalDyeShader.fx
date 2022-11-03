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
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float4 TidalDye(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelLength = 1 / uImageSize0;
    float4 color = tex2D(uImage0, coords + float2(0, sin(uTime * 5 + coords.x * 4) * pixelLength.y * 1));
    if (!any(color))
        color = tex2D(uImage0, coords);
    float brightness = (color.r + color.g + color.b) / 3;
    return float4(uColor * brightness, color.a) * sampleColor;
}

technique Technique1
{
    pass TidalDyePass
    {
        PixelShader = compile ps_2_0 TidalDye();
    }
}