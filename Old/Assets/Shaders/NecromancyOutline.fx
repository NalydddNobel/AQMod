sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float2 uTargetPosition;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 uImageSize2;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;
float4 uShaderSpecificData;

float getOutlinePower(float4 value)
{
    return 1 - lerp((value.r + value.g + value.b) / 3, 1, 1 - value.a);

}

float4 NecromancyOutline(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float outlinePower = 1 - lerp((color.r + color.g + color.b) / 3, 1, color.a);
    if (outlinePower > 0.1f)
    {
        float2 pixelSize = 1 / uImageSize0;
        float outlinePower2 = getOutlinePower(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0)))
        + getOutlinePower(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) 
        + getOutlinePower(tex2D(uImage0, coords + float2(0, pixelSize.x * 2)))
        + getOutlinePower(tex2D(uImage0, coords + float2(0, -pixelSize.x * 2)));
        
        return float4(uColor, 1) * sampleColor.a * outlinePower * min(outlinePower2, 1);
    }
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass NecromancyOutlinePass
    {
        PixelShader = compile ps_2_0 NecromancyOutline();
    }
}