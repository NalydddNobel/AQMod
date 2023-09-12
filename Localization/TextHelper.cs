using System;
using Terraria.Localization;

namespace Aequus.Localization;

public static class TextHelper {
    public static string Seconds(double value) {
        return Math.Round(value / 60.0).ToString("0.0", Language.ActiveCulture.CultureInfo.NumberFormat).Replace(".0", "");
    }
}