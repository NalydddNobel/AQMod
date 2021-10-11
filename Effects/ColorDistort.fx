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

const float TWO_PI_OVER_3 = 6.28 / 3;

float FrameYFix(float2 coords)
{
    float frameSizeY = uSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return y * 1 / frameSizeY;
}

float lerp(float value, float value2, float amount)
{
    return value - (value - value2) * amount;
}

float normalsin(float time)
{
    return (sin(time) + 1) / 2;
}

float4 ColorDistort(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float time = uTime * 20;
    float yFIX = FrameYFix(coords);
    float rLERP = normalsin(origColor.r * 10 + time + yFIX);
    origColor.r = lerp(origColor.r, rLERP, 0.5 * origColor.a);
    float gLERP = normalsin(origColor.g * 10 + time + coords.x);
    origColor.g = lerp(origColor.g, gLERP, 0.5 * origColor.a);
    float bLERP = normalsin(origColor.b * 10 + time + origColor.r);
    origColor.b = lerp(origColor.b, bLERP, 0.5 * origColor.a);
    return origColor * sampleColor;
}

float3 rainbow(float time)
{
    return float3(sin(time), sin(time + TWO_PI_OVER_3), sin(time + TWO_PI_OVER_3 * 2));
}

float4 Rainbow(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float3 rainBOW = rainbow(uTime * 6 + sin(coords.x * 2) + (origColor.r + origColor.g + origColor.b)) * origColor.a;
    rainBOW.r = lerp(origColor.r, rainBOW.r, uOpacity * rainBOW.r);
    rainBOW.g = lerp(origColor.g, rainBOW.g, uOpacity * rainBOW.g);
    rainBOW.b = lerp(origColor.b, rainBOW.b, uOpacity * rainBOW.b);
    return float4(rainBOW.r, rainBOW.g, rainBOW.b, origColor.a) * sampleColor;
}

float4 HyperRainbow(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float3 rainBOW = rainbow(uTime * 6 + sin(coords.x * 2)) * origColor.a;
    rainBOW.r = lerp(origColor.r, rainBOW.r, uOpacity * rainBOW.r);
    rainBOW.g = lerp(origColor.g, rainBOW.g, uOpacity * rainBOW.g);
    rainBOW.b = lerp(origColor.b, rainBOW.b, uOpacity * rainBOW.b);
    return float4(rainBOW.r, rainBOW.g, rainBOW.b, origColor.a) * sampleColor;
}

technique Technique1
{
    pass ColorDistortPass
    {
        PixelShader = compile ps_2_0 ColorDistort();
    }
    pass RainbowPass
    {
        PixelShader = compile ps_2_0 Rainbow();
    }
    pass HyperRainbowPass
    {
        PixelShader = compile ps_2_0 HyperRainbow();
    }
}