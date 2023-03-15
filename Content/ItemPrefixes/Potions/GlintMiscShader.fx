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

float2 FrameFix(float2 coords)
{
    float frameSizeX = uSourceRect.z / uImageSize0.x;
    float x = coords.x % frameSizeX;
    float frameSizeY = uLegacyArmorSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return float2(x * 1 / frameSizeX, y * 1 / frameSizeY);
}

float4 Enchantment(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a == 0)
    {
        return color * sampleColor;
    }
    float2 textureCoords = FrameFix(coords);
    float4 mergeColor = tex2D(uImage1, float2((uTime * 0.21f + textureCoords.x * 0.2) % 1, (uTime * 0.0042f + textureCoords.y * 0.2) % 1));
    mergeColor *= min(min(mergeColor.r, mergeColor.g), mergeColor.b) + max(max(mergeColor.r, mergeColor.g), mergeColor.b) * uOpacity;
    return float4(color.r * sampleColor.r + mergeColor.r * sampleColor.a,
    color.g * sampleColor.g + mergeColor.g * sampleColor.a,
    color.b * sampleColor.b + mergeColor.b * sampleColor.a, color.a * sampleColor.a);
}

technique Technique1
{
    pass EnchantmentPass
    {
        PixelShader = compile ps_2_0 Enchantment();
    }   
}