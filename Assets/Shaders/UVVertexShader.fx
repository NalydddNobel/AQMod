sampler Texture : register(s0);
float4x4 XViewProjection;
float2 UVMultiplier;
float2 UVAdd;

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(float4 inPos : POSITION, float2 inUV : TEXCOORD0, float4 inColor : COLOR0)
{
    VertexShaderOutput Output;

    Output.Position = mul(inPos, XViewProjection);
    Output.UV = inUV * UVMultiplier + UVAdd;
    Output.Color = inColor;

    return Output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return tex2D(Texture, float2(input.UV.x % 1, input.UV.y % 1)) * input.Color;
}

technique UVWrap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}