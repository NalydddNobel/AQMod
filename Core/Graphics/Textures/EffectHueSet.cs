namespace Aequu2.Core.Graphics.Textures;

public struct EffectHueSet(float Hue) : IColorEffect {
    Color IColorEffect.GetColor(in ColorEffectContext context) {
        Color color = context.Color;
        return color.HueSet(Hue) with { A = color.A };
    }
}
