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
int Repetitions;

float4 GetOffsetClr(float2 coords, float2 offset)
{
    return tex2D(uImage0, coords + offset) + tex2D(uImage0, coords - offset);
}

float4 FlashCoordinate(float2 coords : TEXCOORD0) : COLOR0
{
    float2 target = uTargetPosition;
    float2 screenLocation = (uScreenPosition + coords * uScreenResolution);
    float2 dir = normalize(screenLocation - target);
    
    float4 color;
    float intensity = max((uIntensity / ((Repetitions) / 2.5f)), 1.0f / (Repetitions * 2 + 1));

    float2 flashDirection = ((dir * (float2(1 / uScreenResolution.x, 1 / uScreenResolution.y) * 2)) 
    * (4.0f / Repetitions) * min(length(screenLocation - target) / 20, 10))
    * uIntensity;
    for (int i = 0; i < Repetitions; i++)
    {
        color += GetOffsetClr(coords, flashDirection * i);
    }
    
    return (tex2D(uImage0, coords)
     + color) * intensity;
}

float4 FlashCoordinate_Old(float2 coords : TEXCOORD0) : COLOR0
{
    float2 target = uTargetPosition;
    float2 dir = normalize((uScreenPosition + coords * uScreenResolution) - target);
    float2 pixelScale = float2(1 / uScreenResolution.x, 1 / uScreenResolution.y) * 2;
    float2 offsetNormal = dir * pixelScale * uIntensity;
    float4 color = GetOffsetClr(coords, offsetNormal * 11);
    color += GetOffsetClr(coords, offsetNormal * 10);
    color += GetOffsetClr(coords, offsetNormal * 9);
    color += GetOffsetClr(coords, offsetNormal * 8);
    color += GetOffsetClr(coords, offsetNormal * 7);
    color += GetOffsetClr(coords, offsetNormal * 6);
    color += GetOffsetClr(coords, offsetNormal * 5);
    color += GetOffsetClr(coords, offsetNormal * 4);
    color += GetOffsetClr(coords, offsetNormal * 3);
    color += GetOffsetClr(coords, offsetNormal * 2);
    color += GetOffsetClr(coords, offsetNormal);
    float intensity = max(uIntensity / 4.0f, 1 / 22.0f);
    return (color + tex2D(uImage0, coords)) * intensity;
}

technique Technique1
{
    pass FlashCoordinatePass
    {
        PixelShader = compile ps_3_0 FlashCoordinate();
    }
}