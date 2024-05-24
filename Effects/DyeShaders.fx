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
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

const float3 kRGBToYPrime = float3(0.299, 0.587, 0.114);
const float3 kRGBToI = float3(0.596, -0.275, -0.321);
const float3 kRGBToQ = float3(0.212, -0.523, 0.311);

const float3 kYIQToR = float3(1.0, 0.956, 0.621);
const float3 kYIQToG = float3(1.0, -0.272, -0.647);
const float3 kYIQToB = float3(1.0, -1.107, 1.704);

const float TWO_THIRDS_PI = 6.28 / 3;

float Mean(float2 minMax)
{
    return (minMax.y + minMax.x) / 2;
}

float2 minMaxRGB(float3 rgb)
{
    return float2(min(min(rgb.r, rgb.g), rgb.b), max(max(rgb.r, rgb.g), rgb.b));
}

float normalSin(float time)
{
    return (sin(time) + 1) / 2;
}

float2 RotationToVector(float f)
{
    return float2(cos(f), sin(f));
}

float FrameYFix(float2 coords)
{
    float frameSizeY = uSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return y / frameSizeY;
}

float2 FrameFix(float2 coords)
{
    float frameSizeX = uSourceRect.z / uImageSize0.x;
    float x = coords.x % frameSizeX;
    float frameSizeY = uLegacyArmorSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return float2(x / frameSizeX, y / frameSizeY);
}

float2 FrameFix2(float2 coords)
{
    float pixelSize = 2;
    float frameSizeX = uSourceRect.z / uImageSize0.x;
    float x = floor(coords.x * uImageSize0.x / pixelSize) / uImageSize0.x * pixelSize % frameSizeX;
    float frameSizeY = uLegacyArmorSourceRect.w / uImageSize0.y;
    float y = floor(coords.y * uImageSize0.y / pixelSize) / uImageSize0.y * pixelSize % frameSizeY;
    return float2(x / frameSizeX, y / frameSizeY);
}

float3 ApplyHue(float3 col, float hueAdjust)
{
    const float3 k = float3(0.57735, 0.57735, 0.57735);
    half cosAngle = cos(hueAdjust);
    return col * cosAngle + cross(k, col) * sin(hueAdjust) + k * dot(k, col) * (1.0 - cosAngle);
}

float3 fastHueShiftTest(float3 color, float hueSet)
{
    float YPrime = dot(color, kRGBToYPrime);
    float I = dot(color, kRGBToI);
    float Q = dot(color, kRGBToQ);
    float hue = atan(float2(Q, I));
    float chroma = sqrt(I * I + Q * Q);

    hue = hueSet;

    Q = chroma * sin(hue);
    I = chroma * cos(hue);

    float3 yIQ = float3(YPrime, I, Q);

    return float3(dot(yIQ, kYIQToR), dot(yIQ, kYIQToG), dot(yIQ, kYIQToB));
}

float4 HueShift(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    color = float4(ApplyHue(color.rgb, color.r * 0.1 + uTime), color.a);
    return float4(color.r, color.g, color.b, color.a) * sampleColor;
}

float4 Enchantment(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a == 0)
    {
        return color * sampleColor;
    }
    float2 textureCoords = FrameFix(coords);
    float4 mergeColor = tex2D(uImage1, float2((uTime * 0.21f + textureCoords.x * 0.2) % 1, (uTime * 0.0042f + textureCoords.y * 0.2) % 1));
    mergeColor *= min(min(mergeColor.r, mergeColor.g), mergeColor.b) + max(max(mergeColor.r, mergeColor.g), mergeColor.b) * uOpacity;
    return float4(color.r * sampleColor.r + mergeColor.r * sampleColor.a,
    color.g * sampleColor.g + mergeColor.g * sampleColor.a,
    color.b * sampleColor.b + mergeColor.b * sampleColor.a, color.a * sampleColor.a);
}

float4 Censor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixel = coords * uImageSize0;
    int pixelSize = uOpacity;
    int x = pixel.x % pixelSize;
    if (x > 0)
    {
        pixel.x -= x;
    }
    int y = pixel.y % pixelSize;
    if (y > 0)
    {
        pixel.y -= y;
    }
    return tex2D(uImage0, pixel / uImageSize0) * sampleColor;
}

float4 Frostbite(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    float2 pixelSize = 1 / uImageSize0;
    float2 fixedCoords = FrameFix(coords);
    float4 val = tex2D(uImage1, float2(fixedCoords.x, abs(fixedCoords.y / 3 - uTime * 0.2f) % 1));
    float multiplier = 1 + (val.r + val.b + val.g);
    return float4(fastHueShiftTest(color.rgb * multiplier, 3), 1) * color.a * sampleColor;

}

float4 AncientFrostbite(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float ySine = sin(uTime * 20 + coords.x * 30);
    float3 multiplyColor = lerp(uColor, uSecondaryColor, (sin(uTime * 3 + FrameYFix(coords.y) + ySine * 2) + 1) / 2);
    color = lerp(color, tex2D(uImage0, float2(coords.x, (coords.y + ySine * (1 / uImageSize0.y * 2)) % 1)), 0.5f);
    color.r *= multiplyColor.r;
    color.g *= multiplyColor.g;
    color.b *= multiplyColor.b;
    return color * sampleColor;
}

float4 Gust(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float noise = length(tex2D(uImage1, (FrameFix(coords) + float2(0.3f * uTime, 0.2f * uTime)) / 10).rgb);
    return lerp(color * sampleColor, float4(uColor, color.a * sampleColor.a), noise * color.a * length(color.rgb));
}

float4 HellBeam(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float3 MERGEColor = uColor;
    float intensity = ((sin((color.r * color.g * color.b) * 100 + uTime * uOpacity + coords.x * 10 * uDirection) + 1) / 2) * uSaturation;
    float4 mergeColor = float4(MERGEColor.r * color.a, MERGEColor.g * color.a, MERGEColor.b * color.a, color.a);
    if (color.r < uLightSource.r * color.a)
    {
        color.r = uLightSource.r * color.a;
    }
    if (color.g < uLightSource.g * color.a)
    {
        color.g = uLightSource.g * color.a;
    }
    if (color.b < uLightSource.b * color.a)
    {
        color.b = uLightSource.b * color.a;
    }
    color *= float4(uSecondaryColor.r, uSecondaryColor.g, uSecondaryColor.b, 1);
    color = lerp(color, mergeColor, float4(intensity, intensity, intensity, intensity));
    intensity *= intensity;
    intensity *= intensity;
    color = lerp(color, float4(1 * color.a, 1 * color.a, 1 * color.a, color.a), float4(intensity, intensity, intensity, intensity));
    return color * sampleColor;
}

bool isBlank_Legacy(float4 color)
{
    return color.a == 0 && color.r == 0 && color.g == 0 && color.b == 0;
}

float4 Outline(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank_Legacy(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank_Legacy(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank_Legacy(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank_Legacy(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank_Legacy(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(uColor.rgb, 1) * sampleColor;
        }
    }
    return color * sampleColor;
}

float4 Breakdown(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float time = uTime * 20;
    float2 textureCoords = FrameFix(coords);
    float rLERP = normalSin(origColor.r * 10 + time + textureCoords.y);
    origColor.r = lerp(origColor.r, rLERP, 0.5 * origColor.a);
    float gLERP = normalSin(origColor.g * 10 + time + textureCoords.x);
    origColor.g = lerp(origColor.g, gLERP, 0.5 * origColor.a);
    float bLERP = normalSin(origColor.b * 10 + time + origColor.r);
    origColor.b = lerp(origColor.b, bLERP, 0.5 * origColor.a);
    return origColor * sampleColor;
}

float3 rainbow(float time)
{
    return float3(sin(time), sin(time + TWO_THIRDS_PI), sin(time + TWO_THIRDS_PI * 2));
}

float4 Disco(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float3 rainBOW = rainbow(uTime * 6 + sin(coords.x * 2) + (origColor.r + origColor.g + origColor.b)) * origColor.a;
    rainBOW.r = lerp(origColor.r, rainBOW.r, uOpacity * rainBOW.r);
    rainBOW.g = lerp(origColor.g, rainBOW.g, uOpacity * rainBOW.g);
    rainBOW.b = lerp(origColor.b, rainBOW.b, uOpacity * rainBOW.b);
    return float4(rainBOW.r, rainBOW.g, rainBOW.b, origColor.a) * sampleColor;
}

float4 RedSprite(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 textureCoords = FrameFix(coords);
    color = lerp(color, tex2D(uImage0, float2((coords.x + sin(uTime * 20 + textureCoords.y * 30) * (2 / uImageSize0.x)) % 1, coords.y)), 0.5f);
    color.r *= uColor.r;
    color.g *= uColor.g;
    color.b *= uColor.b;
    return color * sampleColor;
}

float4 Scorching(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 pixelSize = 1 / uImageSize0;
    float2 fixedCoords = FrameFix2(coords);
    float4 val = tex2D(uImage1, float2(fixedCoords.x / 8 * pixelSize.x, (fixedCoords.y / 8 + uTime * (0.03f + sin(fixedCoords.x * 20) * 0.02f)) % 1));
    float brightness = (val.r + val.g) * fixedCoords.y * color.r;
    return lerp(float4(fixedCoords.y * min(color.r + color.g + color.b, 1), 0, 0, 1), float4(1, 0.1f, 0, 1), brightness * 12) * color.a * sampleColor;
}

float4 Scroll(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 mergeColor = tex2D(uImage0, float2((uTime * 0.5 + coords.x) % 1, (uTime * 0.5 + coords.y) % 1));
    mergeColor.a = color.a;
    if (mergeColor.a == 0)
    {
        return color * sampleColor;
    }
    return mergeColor * sampleColor;
}

float4 Simplify(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelSize = 1 / uImageSize0;
    float4 color = tex2D(uImage0, coords);
    float4 color2 = tex2D(uImage0, coords + float2(pixelSize.x * 2, 0));
    color.r = sqrt(color.r) * sqrt(color2.r);
    color.g = sqrt(color.g) * sqrt(color2.g);
    color.b = sqrt(color.b) * sqrt(color2.b);
    color2 = tex2D(uImage0, coords - float2(pixelSize.x * 2, 0));
    color.r = sqrt(color.r) * sqrt(color2.r);
    color.g = sqrt(color.g) * sqrt(color2.g);
    color.b = sqrt(color.b) * sqrt(color2.b);
    return color * sampleColor;
}

float4 Tidal(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelLength = 1 / uImageSize0;
    float4 color = tex2D(uImage0, coords + float2(0, sin(uTime * 5 + coords.x * 4) * pixelLength.y * 1));
    if (!any(color))
        color = tex2D(uImage0, coords);
    float brightness = (color.r + color.g + color.b) / 3;
    return float4(uColor * brightness, color.a) * sampleColor;
}

float4 AquaticShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 frameCoords = FrameFix(coords);
    float2 upVal = float2(0, 4 / uImageSize0.y);
    float4 color = frameCoords.y - upVal.y < 0.1 ? float4(0, 0, 0, 0) : tex2D(uImage0, coords - upVal);
    float2 ratio = uImageSize0 / 4;

    //return float4(frameCoords.y, 0, 0, 1);
    float4 color2 = tex2D(uImage0, coords);
    float time = frameCoords.x * 20 + uTime * 2 + frameCoords.y * 2;
    float wave = (sin(time) + 1) * 0.25f;
    float val = abs((frameCoords.y * 3 + wave - uTime * 1.5) % 1);
    val = val > 0.5 ? pow(val, 2) + 0.5f : val;
    float4 endColor = tex2D(uImage1, float2(frameCoords.x, val));
    float flerp = endColor.r + endColor.g + endColor.b;
    return lerp(color2, pow(endColor, 2), flerp / 2 * (color.a + color2.a) / 2 - frameCoords.y * 0.4) * sampleColor;
}

technique Technique1
{
    pass HueShiftPass
    {
        PixelShader = compile ps_2_0 HueShift();
    }
    pass EnchantmentPass
    {
        PixelShader = compile ps_2_0 Enchantment();
    }
    pass CensorPass
    {
        PixelShader = compile ps_2_0 Censor();
    }
    pass FrostbitePass
    {
        PixelShader = compile ps_2_0 Frostbite();
    }
    pass AncientFrostbitePass
    {
        PixelShader = compile ps_2_0 AncientFrostbite();
    }
    pass GustPass
    {
        PixelShader = compile ps_2_0 Gust();
    }
    pass HellBeamPass
    {
        PixelShader = compile ps_2_0 HellBeam();
    }
    pass OutlinePass
    {
        PixelShader = compile ps_2_0 Outline();
    }
    pass BreakdownPass
    {
        PixelShader = compile ps_2_0 Breakdown();
    }
    pass DiscoPass
    {
        PixelShader = compile ps_2_0 Disco();
    }
    pass RedSpritePass
    {
        PixelShader = compile ps_2_0 RedSprite();
    }
    pass ScorchingPass
    {
        PixelShader = compile ps_2_0 Scorching();
    }
    pass ScrollPass
    {
        PixelShader = compile ps_2_0 Scroll();
    }
    pass SimplifyPass
    {
        PixelShader = compile ps_2_0 Simplify();
    }
    pass TidalPass
    {
        PixelShader = compile ps_2_0 Tidal();
    }
    pass AquaticShaderPass
    {
        PixelShader = compile ps_2_0 AquaticShader();
    }
}