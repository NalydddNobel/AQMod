sampler uImage0 : register(s0);
sampler uImage1 : register(s1); // Automatically Images/Misc/Perlin via Force Shader testing option
sampler uImage2 : register(s2); // Automatically Images/Misc/noise via Force Shader testing option
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

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 color2 = tex2D(uImage1, coords);
    float4 color3 = color;
    float lerpAmt = (color2.r + color2.g + color2.b) / 3 * color2.a;
    if (lerpAmt > 0)
    {
        float2 crushedCoords = coords * uScreenResolution;
        crushedCoords.x = round(crushedCoords.x / 3) * 3 / uScreenResolution.x;
        crushedCoords.y = round(crushedCoords.y / 3) * 3 / uScreenResolution.y;
        float4 color4 = tex2D(uImage0, crushedCoords);
        float intensity = round(pow((color4.r + color4.g + color4.b) / 3, 3) * 10 * uIntensity) / (10 * uIntensity);
        if (intensity < 0.1f)
        {
            color3 = lerp(float4(15 / 255.0f, 56 / 255.0f, 15 / 255.0f, color4.a), float4(48 / 255.0f, 98 / 255.0f, 48 / 255.0f, color4.a), intensity / 0.11f);
        }
        else if (intensity < 0.25f)
        {
            color3 = lerp(float4(48 / 255.0f, 98 / 255.0f, 48 / 255.0f, color4.a), float4(139 / 255.0f, 172 / 255.0f, 15 / 255.0f, color4.a), (intensity - 0.1f) / 0.15f);
        }
        else if (intensity < 0.5f)
        {
            color3 = lerp(float4(139 / 255.0f, 172 / 255.0f, 15 / 255.0f, color4.a), float4(155 / 255.0f, 188 / 255.0f, 15 / 255.0f, color4.a), (intensity - 0.25f) / 0.25f);
        }
        else
        {
            color3 = float4(155 / 255.0f, 188 / 255.0f, 15 / 255.0f, color4.a);
        }
    }
    return lerp(color, color3, lerpAmt);
}

technique Technique1
{
    pass ModdersToolkitShaderPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}