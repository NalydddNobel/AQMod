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
const float3 kRGBToYPrime = float3(0.299, 0.587, 0.114);
const float3 kRGBToI = float3(0.596, -0.275, -0.321);
const float3 kRGBToQ = float3(0.212, -0.523, 0.311);

const float3 kYIQToR = float3(1.0, 0.956, 0.621);
const float3 kYIQToG = float3(1.0, -0.272, -0.647);
const float3 kYIQToB = float3(1.0, -1.107, 1.704);

float3 hueShiftTest(float3 color, float hueAdjust)
{
    float YPrime = dot(color, kRGBToYPrime);
    float I = dot(color, kRGBToI);
    float Q = dot(color, kRGBToQ);
    float hue = atan(float2(Q, I));
    float chroma = sqrt(I * I + Q * Q);

    hue += hueAdjust;

    Q = chroma * sin(hue);
    I = chroma * cos(hue);

    float3 yIQ = float3(YPrime, I, Q);

    return float3(dot(yIQ, kYIQToR), dot(yIQ, kYIQToG), dot(yIQ, kYIQToB));
}

float4 HueShift(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    color = float4(hueShiftTest(float3(color.r, color.g, color.b), uTime * 0.33f), color.a);
    return float4(color.r, color.g, color.b, color.a) * sampleColor;
}

technique Technique1
{
    pass HueShiftPass
    {
        PixelShader = compile ps_2_0 HueShift();
    }
}