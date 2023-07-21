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

float4 CrabsonScreenShader(float2 coords : TEXCOORD0) : COLOR0
{
    float2 wobble = pow(float2(length(tex2D(uImage2, coords * 1.5f + float2(uTime * 0.02f, uTime * 0.05f))) - 1, length(tex2D(uImage2, coords)) - 1) * uOpacity, 2);
    float2 fromCenter = abs(coords - float2(0.5, 0.5));
    float4 color = tex2D(uImage0, coords + wobble * 0.06f * (fromCenter.x + fromCenter.y));
    float3 clr = lerp(float3(0, 0, 0), float3(uColor), uIntensity * uOpacity * pow(fromCenter.x + fromCenter.y, 4));
    color.r += clr.r;
    color.g += clr.g;
    color.b += clr.b;
    return color;
}

technique Technique1
{
    pass CrabsonScreenShaderPass
    {
        PixelShader = compile ps_2_0 CrabsonScreenShader();
    }
}