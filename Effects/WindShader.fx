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

float4 WavyStuff(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 pixelSize = 1 / uImageSize0 * 2;
    float intensity = length(float2(coords.x - 0.5f, coords.y - 0.5f));
    coords.x += sin(intensity * 2022 + uTime) * pixelSize.x * 8 * intensity;
    coords.y += sin(intensity * 3322 + uTime) * pixelSize.y * 8 * intensity;
    return tex2D(uImage0, coords);
}

float4 DoOutlines(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelSize = 1 / uImageSize0 * 2;
    if (tex2D(uImage0, coords).a == 0)
    {
        if (tex2D(uImage0, float2(coords.x + pixelSize.x, coords.y)).a != 0 ||
            tex2D(uImage0, float2(coords.x - pixelSize.x, coords.y)).a != 0 ||
            tex2D(uImage0, float2(coords.x, coords.y + pixelSize.y)).a != 0 ||
            tex2D(uImage0, float2(coords.x, coords.y - pixelSize.y)).a != 0)
        {
            return float4(0, 0, 0, 1);
        }
    }
    return tex2D(uImage0, coords);
}

float4 MakeTransparent(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float multiplier = 1;
    multiplier = (1 - length(float2(coords.x - 0.5f, coords.y - 0.5f)) * 4);
    return tex2D(uImage0, coords) * multiplier;
}

technique Technique1
{
    pass WavyStuffPass
    {
        PixelShader = compile ps_2_0 WavyStuff();
    }
    pass DoOutlinesPass
    {
        PixelShader = compile ps_2_0 DoOutlines();
    }
    pass MakeTransparentPass
    {
        PixelShader = compile ps_2_0 MakeTransparent();
    }
}