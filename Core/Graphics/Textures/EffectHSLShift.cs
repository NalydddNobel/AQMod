namespace Aequus.Core.Graphics.Textures;

public struct EffectHSLShift(float Hue, float Saturation, float Light) : IColorEffect {
    public EffectHSLShift(Vector3 hsl) : this(hsl.X, hsl.Y, hsl.Z) { }

    Color IColorEffect.GetColor(in ColorEffectContext context) {
        Color color = context.Color;
        Vector3 myHSL = Main.rgbToHsl(color);
        myHSL.X = Hue;
        myHSL.Y *= Saturation;
        myHSL.Z *= MathHelper.Lerp(Light, 1f, 0.2f);
        return Main.hslToRgb(myHSL) with { A = color.A };
    }
}
