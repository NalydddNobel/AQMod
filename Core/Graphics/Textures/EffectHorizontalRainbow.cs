using System;

namespace Aequu2.Core.Graphics.Textures;

public class EffectHorizontalRainbow : IColorEffect {
    private int _left;
    private int _right;
    private int _width;

    void IColorEffect.Prepare(in ColorEffectContext context) {
        _left = int.MaxValue;
        _right = int.MinValue;
        for (int i = 0; i < context.Colors.Length; i++) {
            int x = i % context.TextureWidth;
            Color color = context.Colors[i];
            if (color.A > 0) {
                _left = int.Min(x, _left);
                _right = int.Max(x, _right);
            }
        }
        _width = _right - _left;
    }

    Color IColorEffect.GetColor(in ColorEffectContext context) {
        int x = context.index % context.TextureWidth / 2 * 2;
        if (x < _left || x > _right) {
            return context.Color;
        }

        float horizontalProgress = (x - _left) / (float)_width;

        Color color = context.Color;
        Vector3 myHSL = Main.rgbToHsl(color);
        myHSL.X = horizontalProgress;
        myHSL.Z = MathF.Pow(myHSL.Z, 2f);
        return Main.hslToRgb(myHSL) with { A = color.A };
    }
}
