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

float4 MossEnemyShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float noise = tex2D(uImage1, float2(
    (coords.x + uTime) * 0.05 % 1, 
    (coords.y + uTime) * 0.02 % 1)).
    rgb;
    float value = (0.1f) * noise;

    float4 color2 = tex2D(uImage0, coords + float2(0, value))
    + tex2D(uImage0, coords + float2(0, -value))
    + tex2D(uImage0, coords + float2(value, 0))
    + tex2D(uImage0, coords + float2(-value, 0));
    float4 clr = color * sampleColor;
    return lerp(clr, float4(uColor.r, uColor.g, uColor.b, 1) * clr.a, 1 - color2.a / 4);
}

technique Technique1
{
    pass MossEnemyShaderPass
    {
        PixelShader = compile ps_2_0 MossEnemyShader();
    }
}