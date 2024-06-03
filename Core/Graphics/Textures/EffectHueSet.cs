using System;

namespace Aequus.Core.Graphics.Textures;

public struct EffectHueSet(float Hue) : IColorEffect {
    Color IColorEffect.GetColor(in ColorEffectContext context) {
        Color color = context.Color;
        byte velocity = Math.Max(Math.Max(color.R, color.G), color.B);
        return color.HueSet(Hue) with { A = color.A };
    }
}
