sampler Texture : register(s0);
float time;

float4 main(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - 0.5);
    float dY = (coords.y - 0.5);
    if (dX > 0)
    {
        dX = 0 - dX;
    }
    if (dX > 0)
    {
        dX = 0 - dX;
    }
    float cVALUE = sqrt(dX * dX + dY * dY);
    cVALUE += sin(time * 6.66 + coords.y * 20 + sin(dX * 6.28) * 10) * 0.1 * (1 - coords.y);
    if (cVALUE - 0.1 < 0.2)
    {
        cVALUE = cVALUE - 0.2;
        cVALUE = cVALUE * 10;
        if (cVALUE > 0)
        {
            float a = 1 - cVALUE;
            float3 clr = lerp(float3(1, 0, 0), float3(0.8f, 0, 0.4f), cVALUE) * 5;
            clr = lerp(clr, float3(0.4f, 0.8f, 0) * 5, coords.y * (0.45f + sin(time * 10) * 0.1f));
            return float4(cVALUE * clr.r * a, cVALUE * clr.g * a, cVALUE * clr.b * a, a);
        }
        return float4(0, 0, 0, 1);
    }
    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}