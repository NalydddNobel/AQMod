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

float4 SpotlightCoordinate(float2 coords : TEXCOORD0) : COLOR0
{
    //float2 drawPosition = uScreenPosition + uScreenResolution / 2;
    float2 drawPosition = uTargetPosition - uScreenPosition;
    float size = 120 * uIntensity;
    float2 drawCoordinate = coords * uScreenResolution;
    float distance = length(drawCoordinate - drawPosition);
    if (distance < size)
    {
        return lerp(tex2D(uImage0, coords), float4(1, 1, 1, 1), 1 - distance / size);
    }
    return tex2D(uImage0, coords);
}

float4 Vignette(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float mult = length(float2(coords.x - 0.5f, coords.y - 0.5f)) / 1.4142135f;
    mult = min(mult * uIntensity * uOpacity, 1);
    return color * (1 - mult);
}

float4 FlashCoordinate(float2 coords : TEXCOORD0) : COLOR0
{
    float2 target = uTargetPosition;
    float2 dir = normalize((uScreenPosition + coords * uScreenResolution) - target);
    float2 pixelScale = float2(1 / uScreenResolution.x, 1 / uScreenResolution.y) * 2;
    float4 color = tex2D(uImage0, coords + dir * pixelScale * uIntensity * 12);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity * 12);
    color += tex2D(uImage0, coords + dir * pixelScale * uIntensity * 10);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity * 10);
    color += tex2D(uImage0, coords + dir * pixelScale * uIntensity * 8);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity * 8);
    color += tex2D(uImage0, coords + dir * pixelScale * uIntensity * 6);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity * 6);
    color += tex2D(uImage0, coords + dir * pixelScale * uIntensity * 5);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity * 5);
    color += tex2D(uImage0, coords + dir * pixelScale * uIntensity * 4);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity * 4);
    color += tex2D(uImage0, coords + dir * pixelScale * uIntensity * 3);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity * 3);
    color += tex2D(uImage0, coords + dir * pixelScale * uIntensity * 2);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity * 2);
    color += tex2D(uImage0, coords + dir * pixelScale * uIntensity);
    color += tex2D(uImage0, coords - dir * pixelScale * uIntensity);
    float intensity = max(uIntensity / 4.0f, 1 / 18.0f);
    return (color + tex2D(uImage0, coords)) * intensity;
}

technique Technique1
{
    pass FlashCoordinatePass
    {
        PixelShader = compile ps_2_0 FlashCoordinate();
    }
    pass SpotlightCoordinatePass
    {
        PixelShader = compile ps_2_0 SpotlightCoordinate();
    }
    pass VignettePass
    {
        PixelShader = compile ps_2_0 Vignette();
    }
}