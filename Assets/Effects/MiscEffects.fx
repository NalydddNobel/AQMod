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

float2 GetUVForFrame(float2 coords)
{
    float frameSizeX = (uSourceRect.z / uImageSize0.x);
    float x = coords.x % frameSizeX;
    float frameSizeY = uSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return float2(x * 1 / frameSizeX, y * 1 / frameSizeY);
}

float4 VerticalGradient(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    return float4(lerp(uColor, uSecondaryColor, GetUVForFrame(coords).y), sampleColor.a) * tex2D(uImage0, coords).a;
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
    pass SpikeFadePass
    {
        PixelShader = compile ps_2_0 SpikeFade();
    }
    pass VerticalGradientPass
    {
        PixelShader = compile ps_2_0 VerticalGradient();
    }
}