sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 GrayscaleMask(float2 coords : TEXCOORD0) : COLOR0
{
    float pixelSizeX = 1 / uScreenResolution.x;
    float4 color = tex2D(uImage0, coords);
    
    float4 fogColor = tex2D(uImage1, coords);
    float fogIntensity = (fogColor.r + fogColor.g + fogColor.b) / 3;
    float gray = (color.r + color.g + color.b) / 4;
    
    float4 finalColor = lerp(color, float4(gray, gray, gray, color.a), fogIntensity);
    
    finalColor.r += ((tex2D(uImage0, coords + float2(pixelSizeX * 2, 0)) - color) * fogColor).
    r;

    return finalColor;
}

technique Technique1
{
    pass GrayscaleMaskPass
    {
        PixelShader = compile ps_2_0 GrayscaleMask();
    }
}