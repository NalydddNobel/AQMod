sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float2 uTargetPosition;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float FrameYFix(float2 coords)
{
    float frameSizeY = uSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return y * 1 / frameSizeY;
}

float2 FrameFix(float2 coords)
{
    float frameSizeX = uSourceRect.z / uImageSize0.x;
    float x = coords.x % frameSizeX;
    float frameSizeY = uLegacyArmorSourceRect.w / uImageSize0.y;
    float y = coords.y % frameSizeY;
    return float2(x * 1 / frameSizeX, y * 1 / frameSizeY);
}

float2 RotationToVector2(float f)
{
    return float2(cos(f), sin(f));
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

float4 TextureScrolling(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float time = uTime * uSaturation + (color.r + color.g + color.b) / 3;
    float2 gradientCoords = RotationToVector2(uOpacity) * time;
    gradientCoords.x %= 1;
    gradientCoords.y %= 1;
    return tex2D(uImage1, gradientCoords) * sampleColor * color.a;
}

float4 Unchained(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float intensity = (cos(uTime * 10) + 1) / 2;
    float2 wave = float2(sin(uTime * 2.9f) / uImageSize0.x * intensity, cos(uTime * 2.9f) / uImageSize0.y * intensity);
    float4 color = tex2D(uImage0, coords) * sampleColor;
    float4 colorAdditive = 0;
    float A = color.a;
    color *= 0.2f;
    color.a = A;
    colorAdditive += tex2D(uImage0, coords + wave);
    colorAdditive += tex2D(uImage0, coords - wave);
    colorAdditive += tex2D(uImage0, coords + wave * 2);
    colorAdditive += tex2D(uImage0, coords - wave * 2);
    colorAdditive += tex2D(uImage0, coords + wave * 3);
    colorAdditive += tex2D(uImage0, coords - wave * 3);
    colorAdditive.a = 0;
    colorAdditive *= sampleColor.a;
    //float3 coloring = lerp(float3(0.9333f, 0.5098f, 0.9333f), float3(0.5764f, 0.4392f, 0.8588f), intensity);
    colorAdditive.r *= uColor.r;
    colorAdditive.g *= uColor.g;
    colorAdditive.b *= uColor.b;
    return color + colorAdditive;
}

float4 HoriztonalWave(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
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

float4 RedSprite(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 textureCoords = FrameFix(coords);
    color = lerp(color, tex2D(uImage0, float2((textureCoords.x + sin(uTime * 20 + textureCoords.y * 30) * 0.05f) % 1, textureCoords.y)), 0.5f);
    color.r *= uColor.r;
    color.g *= uColor.g;
    color.b *= uColor.b; 
    return color * sampleColor;
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

const float TWO_PI_OVER_3 = 6.28 / 3;

float normalsin(float time)
{
    return (sin(time) + 1) / 2;
}

float4 ColorDistort(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float time = uTime * 20;
    float2 textureCoords = FrameFix(coords);
    float rLERP = normalsin(origColor.r * 10 + time + textureCoords.y);
    origColor.r = lerp(origColor.r, rLERP, 0.5 * origColor.a);
    float gLERP = normalsin(origColor.g * 10 + time + textureCoords.x);
    origColor.g = lerp(origColor.g, gLERP, 0.5 * origColor.a);
    float bLERP = normalsin(origColor.b * 10 + time + origColor.r);
    origColor.b = lerp(origColor.b, bLERP, 0.5 * origColor.a);
    return origColor * sampleColor;
}

float3 rainbow(float time)
{
    return float3(sin(time), sin(time + TWO_PI_OVER_3), sin(time + TWO_PI_OVER_3 * 2));
}

float4 Rainbow(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float3 rainBOW = rainbow(uTime * 6 + sin(coords.x * 2) + (origColor.r + origColor.g + origColor.b)) * origColor.a;
    rainBOW.r = lerp(origColor.r, rainBOW.r, uOpacity * rainBOW.r);
    rainBOW.g = lerp(origColor.g, rainBOW.g, uOpacity * rainBOW.g);
    rainBOW.b = lerp(origColor.b, rainBOW.b, uOpacity * rainBOW.b);
    return float4(rainBOW.r, rainBOW.g, rainBOW.b, origColor.a) * sampleColor;
}

float4 HyperRainbow(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float3 rainBOW = rainbow(uTime * 6 + sin(coords.x * 2)) * origColor.a;
    rainBOW.r = lerp(origColor.r, rainBOW.r, uOpacity * rainBOW.r);
    rainBOW.g = lerp(origColor.g, rainBOW.g, uOpacity * rainBOW.g);
    rainBOW.b = lerp(origColor.b, rainBOW.b, uOpacity * rainBOW.b);
    return float4(rainBOW.r, rainBOW.g, rainBOW.b, origColor.a) * sampleColor;
}

float4 Hypno(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 textureCoords = FrameFix(coords);
    float offset = sin(uTime + textureCoords.y * 25) * 0.05 * uDirection;
    color.r = tex2D(uImage0, float2(coords.x + offset, coords.y)).r;
    color.b = tex2D(uImage0, float2(coords.x - offset, coords.y)).b;
    return color * sampleColor;
}

float4 BreakSprite(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 textureCoords = FrameFix(coords);
    float pixelSize = 1 / uImageSize0;
    float yPixel = textureCoords.y / pixelSize;
    float offset = sin(uTime + textureCoords.y * 25) * 0.05 * uDirection;
    if (yPixel % 8 < 4)
    {
        color = tex2D(uImage0, float2(coords.x + pixelSize * 4, coords.y));
    }
    else
    {
        color = tex2D(uImage0, float2(coords.x - pixelSize * 4, coords.y));
    }
    return color * sampleColor;
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

float4 SuperHypno(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0)
{
    float pixelSize = 1 / uImageSize0;
    float offset = uOpacity * 2;
    float rotation = uTime * 8;
    float2 normal = float2(sin(rotation), cos(rotation));
    float4 origColor = tex2D(uImage0, coords);
    float r = tex2D(uImage0, coords + (normal * pixelSize * offset)).r;
    normal = float2(sin(rotation + TWO_PI_OVER_3), cos(rotation + TWO_PI_OVER_3));
    float g = tex2D(uImage0, coords + (normal * pixelSize * offset)).g;
    normal = float2(sin(rotation + TWO_PI_OVER_3 * 2), cos(rotation + TWO_PI_OVER_3 * 2));
    float b = tex2D(uImage0, coords + (normal * pixelSize * offset)).b;
    return float4(r, g, b, origColor.a) * sampleColor;
}

float4 WeirdHypno(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0)
{
    float2 pixelSize = 1 / uImageSize0;
    if (isnan(uImageSize0.y) || pixelSize.y == 0)
    {
        pixelSize.x = 1;
        pixelSize.y = 1;
    }
    float offset = uOpacity * 4;
    float rotation = uTime * 8 + sin(coords.x + uTime + coords.y * 10) * 8;
    float2 normal = float2(sin(rotation), cos(rotation));
    float4 origColor = tex2D(uImage0, coords);
    float r = tex2D(uImage0, coords + (normal * pixelSize * offset)).r;
    normal = float2(sin(rotation + TWO_PI_OVER_3), cos(rotation + TWO_PI_OVER_3));
    float g = tex2D(uImage0, coords + (normal * pixelSize * offset)).g;
    normal = float2(sin(rotation + TWO_PI_OVER_3 * 2), cos(rotation + TWO_PI_OVER_3 * 2));
    float b = tex2D(uImage0, coords + (normal * pixelSize * offset)).b;
    return float4(r, g, b, origColor.a) * sampleColor;
}

bool isBlank(float4 color)
{
    return color.a == 0 && color.r == 0 && color.g == 0 && color.b == 0;
}

float4 Outline(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(1, 1, 1, 1) * sampleColor;
        }
    }
    return color * sampleColor;
}

float4 OutlineAlpha(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(1, 1, 1, 1) * sampleColor.a;
        }
    }
    return color * sampleColor;
}

float4 OutlineColor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(1 * uColor.r, 1 * uColor.g, 1 * uColor.b, 1) * sampleColor;
        }
    }
    return color * sampleColor;
}

float4 OutlineColorAlpha(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (isBlank(color))
    {
        float2 pixelSize = 1 / uImageSize0;
        if (!isBlank(tex2D(uImage0, coords + float2(pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(-pixelSize.x * 2, 0))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, pixelSize.y * 2))) ||
            !isBlank(tex2D(uImage0, coords + float2(0, -pixelSize.y * 2))))
        {
            return float4(1 * uColor.r, 1 * uColor.g, 1 * uColor.b, 1) * sampleColor.a;
        }
    }
    return color * sampleColor;
}

float2 minMaxRGB(float3 rgb)
{
    return float2(min(min(rgb.r, rgb.g), rgb.b), max(max(rgb.r, rgb.g), rgb.b));
}

float L(float2 minMax)
{
    return (minMax.y + minMax.x) / 2;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
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

float4 ImageScroll(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a == 0)
    {
        return color * sampleColor;
    }
    float2 textureCoords = FrameFix(coords);
    float time = uTime * 0.2;
    float4 mergeColor = tex2D(uImage1, float2((time + textureCoords.x * 0.5) % 1, (time * 0.75 + textureCoords.y * 0.5) % 1));
    mergeColor *= L(minMaxRGB(float3(mergeColor.r, mergeColor.g, mergeColor.b)));
    mergeColor.a = 1;
    return color * sampleColor + (mergeColor * 0.5);
}

float4 Monitor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 origColor = tex2D(uImage0, coords);
    float time = uTime;
    float r = (sin(origColor.r + time + coords.y * 10) + 1) / 2;
    origColor.r = origColor.r - (r - origColor.r) * origColor.a;
    float g = (sin(origColor.g + time * 2 + sin(coords.y * 15 + coords.x * 10) * 10) + 1) / 2;
    origColor.g = origColor.g - (r - origColor.g) * origColor.a;
    float b = (sin(origColor.b + time * 2 + sin(coords.y * 11 + coords.x * 24) * 10) + 1) / 2;
    origColor.b = origColor.b - (r - origColor.b) * origColor.a;
    return origColor * sampleColor;
}

float4 ShieldBeams(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
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

float4 MonoSpotlight(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - 0.5) / 0.5;
    double dY = (coords.y - 0.5) / 0.5;
    double c = sqrt(dX * dX + dY * dY);
    if (c == 0)
    {
        return float4(1, 1, 1, 1);
    }
    else
    {
        float value = 1 - (float) c;
        return float4(value, value, value, value);
    }
}

float4 Spotlight(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float dX = (coords.x - 0.5) / 0.5;
    double dY = (coords.y - 0.5) / 0.5;
    double c = sqrt(dX * dX + dY * dY);
    if (c == 0)
    {
        return float4(1, 1, 1, 1) * sampleColor;
    }
    else
    {
        float value = 1 - (float) c;
        return float4(value, value, value, value) * sampleColor;
    }
}

float4 Fade(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a != 0)
    {
        return float4(coords.x * sampleColor.r, coords.x * sampleColor.g, coords.x * sampleColor.b, color.a * sampleColor.a);
    }
    return color * sampleColor;
}

float4 FadeYProgress(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a != 0)
    {
        float y = coords.y * uOpacity;
        if (y <= 0)
        {
            return float4(0, 0, 0, color.a * sampleColor.a);
        }
        return float4(y * sampleColor.r, y * sampleColor.g, y * sampleColor.b, color.a * sampleColor.a);
    }
    return color * sampleColor;
}

float4 FadeYProgressAlpha(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a != 0)
    {
        float y = coords.y * uOpacity;
        y = uOpacity - y;
        y -= 0.01;
        if (y <= 0)
        {
            return float4(0, 0, 0, 0);
        }
        return float4(y * color.r * sampleColor.r, y * color.g * sampleColor.g, y * color.b * sampleColor.b, y * color.a * sampleColor.a);
    }
    return color * sampleColor;
}

float4 SpikeFade(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (color.a != 0)
    {
        float y = coords.y * uOpacity;
        y = uOpacity - y;
        y -= 0.01;
        if (y <= 0)
        {
            return float4(0, 0, 0, 0);
        }
        return float4(y * color.r * sampleColor.r, y * color.g * sampleColor.g, y * color.b * sampleColor.b, y * color.a * sampleColor.a);
    }
    return color * sampleColor;
}

technique Technique1
{
    pass AquaticShaderPass
    {
        PixelShader = compile ps_3_0 AquaticShader();
    }   
    pass TextureScrollingPass
    {
        PixelShader = compile ps_3_0 TextureScrolling();
    }   
    pass HoriztonalWavePass
    {
        PixelShader = compile ps_3_0 HoriztonalWave();
    }
    pass EnchantmentPass
    {
        PixelShader = compile ps_3_0 Enchantment();
    }
    pass RedSpritePass
    {
        PixelShader = compile ps_3_0 RedSprite();
    }
    pass CensorPass
    {
        PixelShader = compile ps_3_0 Censor();
    }
    pass SpotlightPass
    {
        PixelShader = compile ps_3_0 Spotlight();
    }
    pass MonoSpotlightPass
    {
        PixelShader = compile ps_3_0 MonoSpotlight();
    }
    pass FadePass
    {
        PixelShader = compile ps_3_0 Fade();
    }
    pass FadeYProgressPass
    {
        PixelShader = compile ps_3_0 FadeYProgress();
    }
    pass FadeYProgressAlphaPass
    {
        PixelShader = compile ps_3_0 FadeYProgressAlpha();
    }
    pass SpikeFadePass
    {
        PixelShader = compile ps_3_0 SpikeFade();
    }
    pass ScrollPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
    pass ImageScrollPass
    {
        PixelShader = compile ps_3_0 ImageScroll();
    }
    pass MonitorPass
    {
        PixelShader = compile ps_3_0 Monitor();
    }
    pass ShieldBeamsPass
    {
        PixelShader = compile ps_3_0 ShieldBeams();
    }
    pass OutlinePass
    {
        PixelShader = compile ps_3_0 Outline();
    }
    pass OutlineAlphaPass
    {
        PixelShader = compile ps_3_0 OutlineAlpha();
    }
    pass OutlineColorPass
    {
        PixelShader = compile ps_3_0 OutlineColor();
    }
    pass OutlineColorAlphaPass
    {
        PixelShader = compile ps_3_0 OutlineColorAlpha();
    }
    pass HypnoPass
    {
        PixelShader = compile ps_3_0 Hypno();
    }
    pass BreakSpritePass
    {
        PixelShader = compile ps_3_0 BreakSprite();
    }
    pass SimplifyPass
    {
        PixelShader = compile ps_3_0 Simplify();
    }
    pass ColorDistortPass
    {
        PixelShader = compile ps_3_0 ColorDistort();
    }
    pass RainbowPass
    {
        PixelShader = compile ps_3_0 Rainbow();
    }
    pass HyperRainbowPass
    {
        PixelShader = compile ps_3_0 HyperRainbow();
    }
    pass CensorTechnique
    {
        PixelShader = compile ps_3_0 Censor();
    }
}