float uvEnd;
float uvStart;
float4 ringColor;

float4 main(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float radius = distance(float2(0.5, 0.5), coords) / 2;
    return radius < uvEnd && radius > uvStart ? ringColor : float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 main();
    }
}