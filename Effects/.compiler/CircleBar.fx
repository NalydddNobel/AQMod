sampler uImage0 : register(s0);
float uOpacity;
float uImageSizeX;
float uImageSizeY;

const float2 Half = float2(0.5, 0.5);
const float4 None = float4(0, 0, 0, 0);

float4 main(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelizedCoords = (floor(coords * float2(uImageSizeX, uImageSizeY)) + Half) / float2(uImageSizeX, uImageSizeY);
    float2 topLeftCoords = pixelizedCoords - Half;
    float2 p = -1.0 + 2.0 * pixelizedCoords;
    float r = sqrt(dot(p, p));
    
    float textureUVX = abs(sin(atan2(topLeftCoords.y, topLeftCoords.x) / 2));

    float4 color = textureUVX > uOpacity ? None : tex2D(uImage0, float2(textureUVX * 0.99, r * 0.99));
    return color * sampleColor;
}

technique Technique1
{
    pass Main
    {
        PixelShader = compile ps_2_0 main();
    }
}