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

float4 Stars(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float time = uTime + uSaturation * 100;
    float4 color = tex2D(uImage1, (coords * 4 + float2(time * (0.005f + uSaturation * 0.03f), 0)) % 1) + tex2D(uImage1, (coords * 4 + float2(0, time * (0.005f + uSaturation * 0.04f))) % 1);
    float value = (color.r + color.g + color.b) / uRotation;

    return value > 0.5f ? (float4(value / 0.5f * uColor.r, value / 0.5f * uColor.g, value / 0.5f * uColor.b, 0) * tex2D(uImage0, coords).a * uOpacity * sampleColor) : float4(0, 0, 0, 0);
}

technique Technique1
{
    pass StarsPass
    {
        PixelShader = compile ps_2_0 Stars();
    }
}