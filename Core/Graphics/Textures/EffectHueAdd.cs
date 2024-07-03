namespace Aequu2.Core.Graphics.Textures;

public struct EffectHueAdd(float Hue) : IColorEffect {
    Color IColorEffect.GetColor(in ColorEffectContext context) {
        Color color = context.Color;
        return color.HueAdd(Hue) with { A = color.A };
    }
}
