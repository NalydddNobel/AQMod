namespace Aequus.Core.Graphics.Textures;

public struct EffectHorizontalRainbow : IColorEffect {
    readonly Color IColorEffect.GetColor(in ColorEffectContext context) {
        int pixelX = context.index % context.TextureWidth / 2 * 2;
        float horizontalProgress = pixelX / (float)context.TextureWidth;

        Color color = context.Color;
        Vector3 myHSL = Main.rgbToHsl(color);
        myHSL.X = horizontalProgress;
        myHSL.Y *= 2f;
        return Main.hslToRgb(myHSL) with { A = color.A };
    }
}
