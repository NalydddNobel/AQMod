using System;

namespace Aequu2.Core.Graphics.Textures;

public struct EffectVelocityGrayscale : IColorEffect {
    Color IColorEffect.GetColor(in ColorEffectContext context) {
        Color color = context.Color;
        byte velocity = Math.Max(Math.Max(color.R, color.G), color.B);
        return color with { R = velocity, G = velocity, B = velocity, A = color.A };
    }
}
