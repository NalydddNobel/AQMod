using System;

namespace Aequus.Core.Utilities;

public static class ExtendColor {
    /// <returns><paramref name="color"/>, with its R,G,B,A Maxed to the specified <paramref name="amt"/>.</returns>
    public static Color MaxRGBA(this Color color, Byte amt) {
        return color.MaxRGBA(amt, amt);
    }
    /// <returns><paramref name="color"/>, with R,G,B Maxed to the specified <paramref name="amt"/>, and <see cref="Color.A"/> maxed to <paramref name="a"/>.</returns>
    public static Color MaxRGBA(this Color color, Byte amt, Byte a) {
        return color.MaxRGBA(amt, amt, amt, a);
    }
    /// <returns><paramref name="color"/>, with R,G,B,A Maxed to <paramref name="r"/>,<paramref name="g"/>,<paramref name="b"/>,<paramref name="a"/> accordingly.</returns>
    public static Color MaxRGBA(this Color color, Byte r, Byte g, Byte b, Byte a) {
        color.R = Math.Max(color.R, r);
        color.G = Math.Max(color.G, g);
        color.B = Math.Max(color.B, b);
        color.A = Math.Max(color.A, a);
        return color;
    }

    public static Color HueShift(this Color color, Single multiplier) {
        var hsl = Main.rgbToHsl(color);
        Single lerpValue = Math.Clamp(1f - multiplier, 0f, 1f);
        Single shiftEnd = 0.7f;
        if (hsl.X < 0.2f) {
            hsl.X = MathHelper.Lerp(hsl.X, -shiftEnd, lerpValue);
        }
        else {
            hsl.X = MathHelper.Lerp(hsl.X, shiftEnd, lerpValue);
        }
        return Main.hslToRgb(hsl);
    }

    public static Color HueAdd(this Color color, Single hueAdd) {
        var hsl = Main.rgbToHsl(color);
        hsl.X += hueAdd;
        hsl.X %= 1f;
        return Main.hslToRgb(hsl);
    }

    public static Color HueSet(this Color color, Single hue) {
        var hsl = Main.rgbToHsl(color);
        hsl.X = hue;
        return Main.hslToRgb(hsl);
    }

    public static Color SaturationMultiply(this Color color, Single saturation) {
        var hsl = Main.rgbToHsl(color);
        hsl.Y = Math.Clamp(hsl.Y * saturation, 0f, 1f);
        return Main.hslToRgb(hsl);
    }

    public static Color SaturationSet(this Color color, Single saturation) {
        var hsl = Main.rgbToHsl(color);
        hsl.Y = saturation;
        return Main.hslToRgb(hsl);
    }

    public static Color GetLastPrismColor(Projectile projectile, Single index, Single luminance = 0.5f, Single alphaMultiplier = 0f) {
        Single lastPrismHue = projectile.GetLastPrismHue(index % 6f, ref luminance, ref alphaMultiplier);
        Single lastPrismHue2 = projectile.GetLastPrismHue((index + 1f) % 6f, ref luminance, ref alphaMultiplier);
        return Main.hslToRgb(MathHelper.Lerp(lastPrismHue, lastPrismHue2, Math.Abs(index) % 1f), 1f, luminance);
    }
    public static Color GetLastPrismColor(Int32 player, Single position) {
        ExtendProjectile._dummyProjectile.owner = player;
        return GetLastPrismColor(ExtendProjectile._dummyProjectile, position);
    }
    public static Color GetLastPrismColor(Player player, Single position) {
        return GetLastPrismColor(player.whoAmI, position);
    }
}
