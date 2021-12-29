sampler image : register(s1);

float pulse = 0;
float intensity = 0;
float time = 0;
float2 screenOrigin; // float2(0.5f, 0.8f)
float3 color;
float minRange;
float maxRange;

float4 BackgroundEffect(float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - screenOrigin.x) * intensity;
    float dY = (coords.y - screenOrigin.y) * intensity;
    float c = sqrt(dX * dX + dY * dY) - coords.y -
    abs((coords.x - 0.5f) * pulse) + abs((coords.y - 0.5f) * pulse) -
    (sin(time * 5 + coords.x * 10) * 0.025f) +
    (sin(time + dX) * 0.05f);
    if (c == 0)
    {
        return float4(color.r, color.g, color.b, 1);
    }
    else
    {
        float value = 1 - c;
        return float4(color.r * value, color.g * value, color.b * value, value);
    }
}

float4 MagicalCurrentAurora(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    if (coords.y > minRange && coords.y < maxRange)
    {
        float waveFunction = ((sin(coords.x * 24 + time) + 1) + (sin(coords.x * 11 - time * 0.6f) + 1)) / 8;
        float value = (coords.y - minRange) * (1 / (maxRange - minRange));
        if (value > waveFunction)
        {
            float lerpAmount = (value - waveFunction) * 2;
            float progress = 1 - lerpAmount;
            if (progress > 0)
            {
                if (progress > 0.9f)
                {
                    float m = (1 - (progress - 0.90f) * 10) * intensity;
                    return sampleColor * m;
                }
                float sparkle = (tex2D(image, float2(coords.x * 5 + time * 0.5f, coords.y * 4 + time * 0.09f + waveFunction * 0.4f)).r - 0.75f);
                if (sparkle > 0)
                {
                    progress *= 3;
                    return float4(progress, progress, progress, 0) * intensity;
                }
                float m2 = (progress + 0.1f) * intensity;
                return sampleColor * m2;
            }
        }
    }
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass MagicalCurrentAuroraPass
    {
        PixelShader = compile ps_2_0 MagicalCurrentAurora();
    }
    pass BackgroundEffectPass
    {
        PixelShader = compile ps_2_0 BackgroundEffect();
    }
}