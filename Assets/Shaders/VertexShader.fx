float4x4 XViewProjection;

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(float4 inPos : POSITION, float4 inColor : COLOR0)
{
    VertexShaderOutput Output;
     
    Output.Position = mul(inPos, XViewProjection);
    Output.Color = inColor;
 
    return Output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

technique Untextured
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}