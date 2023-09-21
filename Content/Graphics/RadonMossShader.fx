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

float4 RadonShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float colorAmount = 4;
    color = float4(floor(color.r * colorAmount), floor(color.g * colorAmount), floor(color.b * colorAmount), floor(color.a * colorAmount)) / colorAmount;
    return color;
}

float4 RadonShaderDebug(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float colorAmount = 4;
    color = float4(floor(color.r * colorAmount), floor(color.g * colorAmount), floor(color.b * colorAmount), floor(color.a * colorAmount)) / colorAmount;
    return float4(1, 1, 1, 1) * color.a;
}

technique Technique1
{
    pass RadonShaderPass
    {
        PixelShader = compile ps_2_0 RadonShader();
    }
    pass RadonShaderDebugPass
    {
        PixelShader = compile ps_2_0 RadonShaderDebug();
    }
}