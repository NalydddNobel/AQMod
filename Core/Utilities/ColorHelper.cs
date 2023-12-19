using Microsoft.Xna.Framework;
using System;

namespace Aequus.Core.Utilities;

public static class ColorHelper {
    public static Color ColorFurniture => new Color(191, 142, 111, 255);
    public static Color ColorLightedFurniture => new Color(253, 221, 3, 255);

    public static Color MaxRGBA(this Color color, byte amt) {
        return color.MaxRGBA(amt, amt);
    }
    public static Color MaxRGBA(this Color color, byte amt, byte a) {
        return color.MaxRGBA(amt, amt, amt, a);
    }
    public static Color MaxRGBA(this Color color, byte r, byte g, byte b, byte a) {
        color.R = Math.Max(color.R, r);
        color.G = Math.Max(color.G, g);
        color.B = Math.Max(color.B, b);
        color.A = Math.Max(color.A, a);
        return color;
    }

    public static Color HueMultiply(this Color color, float multiplier) {
        var hsl = Main.rgbToHsl(color);
        float lerpValue = Math.Clamp(1f - multiplier, 0f, 1f);
        float shiftEnd = 0.7f;
        if (hsl.X < 0.2f) {
            hsl.X = MathHelper.Lerp(hsl.X, -shiftEnd, lerpValue);
        }
        else {
            hsl.X = MathHelper.Lerp(hsl.X, shiftEnd, lerpValue);
        }
        return Main.hslToRgb(hsl);
    }

    public static Color HueAdd(this Color color, float hueAdd) {
        var hsl = Main.rgbToHsl(color);
        hsl.X += hueAdd;
        hsl.X %= 1f;
        return Main.hslToRgb(hsl);
    }

    public static Color HueSet(this Color color, float hue) {
        var hsl = Main.rgbToHsl(color);
        hsl.X = hue;
        return Main.hslToRgb(hsl);
    }

    public static Color SaturationMultiply(this Color color, float saturation) {
        var hsl = Main.rgbToHsl(color);
        hsl.Y = Math.Clamp(hsl.Y * saturation, 0f, 1f);
        return Main.hslToRgb(hsl);
    }

    public static Color SaturationSet(this Color color, float saturation) {
        var hsl = Main.rgbToHsl(color);
        hsl.Y = saturation;
        return Main.hslToRgb(hsl);
    }
}