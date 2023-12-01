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

float4 StariteCore(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //float4 color = tex2D(uImage0, coords);
    
    //float2 p = -1.0 + 2.0 * GetUVForFrame(coords);
    //float r = sqrt(dot(p, p));
    //float f = (1.0 - sqrt(1.0 - r)) / (r);
    //float4 colorCoordinate1 = tex2D(uImage2, float2(p.x * f + uTime, p.y * f + uTime + 1) * 0.1 % 1);
    //float4 colorCoordinate2 = tex2D(uImage2, abs(float2(p.x * f - uTime * 1.11, p.y * f - uTime * 0.66 + 1)) * 0.3 % 1);
    
    //float4 resultColor = tex2D(uImage1, float2((colorCoordinate1.r * colorCoordinate2.r + uTime * uOpacity) % 1, 1 - color.r)) * color.a * sampleColor;
    //resultColor.r = min(resultColor.r, 1);
    //resultColor.g = min(resultColor.g, 1);
    //resultColor.b = min(resultColor.b, 1);
    //resultColor.a = min(resultColor.a, 1);
    //return resultColor;
    
    float4 color = tex2D(uImage0, coords);
    
    float2 p = -1.0 + 2.0 * coords.xy;
    float r = sqrt(dot(p, p));
    float f = (1.0 - sqrt(1.0 - r)) / (r);
    float2 circleUV = float2(p.x * f + uTime, p.y * f + uTime + 1) % 1;
    float4 colorCoordinate1 = tex2D(uImage2, float2(p.x * f + uTime, p.y * f + uTime + 1) * 0.1 % 1);
    float4 colorCoordinate2 = tex2D(uImage2, abs(float2(p.x * f - uTime * 1.11, p.y * f - uTime * 0.66 + 1)) * 0.3 % 1);
    return tex2D(uImage1, float2((lerp(colorCoordinate1.r * colorCoordinate2.r, 0, color.r) + uTime * uOpacity + color.r) % 1, 1 - color.r)) * color.a * sampleColor;
}


technique Technique1
{
    pass StariteCorePass
    {
        PixelShader = compile ps_2_0 StariteCore();
    }
}