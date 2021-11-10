sampler2D uHotLayer : register(s0);
sampler2D uColdLayer : register(S1);

float uThicknessFromEdge;
float uOutlineThickness;
float2 uScreenResolution;

float4 DoOutline(float2 uv : TEXCOORD) : COLOR
{
    float4 color;
    color = tex2D(uHotLayer, uv.xy);
    if (color.a != 0)
    {
        float2 pixelSize = float2(1 / uScreenResolution.x, 1 / uScreenResolution.y);
        pixelSize = float2(pixelSize.x * uThicknessFromEdge, pixelSize.y * uThicknessFromEdge);
        if (tex2D(uHotLayer, float2(uv.x + pixelSize.x, uv.y)).a == 0
  	        || tex2D(uHotLayer, float2(uv.x - pixelSize.x, uv.y)).a == 0
  	        || tex2D(uHotLayer, float2(uv.x, uv.y + pixelSize.y)).a == 0
  	        || tex2D(uHotLayer, float2(uv.x, uv.y - pixelSize.y)).a == 0)
        {
            return color;
        }
        pixelSize = float2(pixelSize.x * uOutlineThickness, pixelSize.y * uOutlineThickness);
        if (tex2D(uHotLayer, float2(uv.x, uv.y + pixelSize.y)).a == 0
  	        || tex2D(uHotLayer, float2(uv.x, uv.y - pixelSize.y)).a == 0
  	        || tex2D(uHotLayer, float2(uv.x + pixelSize.x, uv.y)).a == 0
  	        || tex2D(uHotLayer, float2(uv.x - pixelSize.x, uv.y)).a == 0)
        {
            return float4(color.r * 0.5, color.g * 0.5, color.b * 0.5, color.a);
        }
    }
    return color;
}

float4 MergeColors(float2 uv : TEXCOORD) : COLOR
{
    float4 color;
    color = tex2D(uHotLayer, uv.xy);
    float4 color2 = tex2D(uColdLayer, uv.xy);
    if (color.a != 0)
    {
        if (color2.a != 0)
        {
            return float4(1, 1, 1, color.a);
        }
        return color;
    }
    return color2;
}

technique Technique1
{
    pass DoOutlinePass
    {
        PixelShader = compile ps_2_0 DoOutline();
    }
    pass MergeColorsPass
    {
        PixelShader = compile ps_2_0 MergeColors();
    }
}