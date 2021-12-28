float pulse = 0;
float intensity = 0;
float time = 0;
float2 screenOrigin; // float2(0.5f, 0.8f)
float3 color;

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
        float value = 1 - (float) c;
        return float4(color.r * value, color.g * value, color.b * value, value);
    }
}

technique Technique1
{
    pass BackgroundEffectPass
    {
        PixelShader = compile ps_2_0 BackgroundEffect();
    }
}