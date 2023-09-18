using System.Net;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Core.Utilities;

public static class TextHelper {
    #region Localization
    public static string GetCategoryTextValue(this ILocalizedModType self, string suffix) {
        return GetCategoryText(self, suffix).Value;
    }

    public static LocalizedText GetCategoryText(this ILocalizedModType self, string suffix) {
        return Language.GetText($"Mods.{self.Mod.Name}.{self.LocalizationCategory}.{suffix}");
    }
    #endregion

    public static string Decimals(double value) {
        return (value).ToString("0.0", Language.ActiveCulture.CultureInfo.NumberFormat).Replace(".0", "");
    }

    #region Time
    /// <summary>
    /// Converts ticks to seconds, up to 1 decimal place.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Seconds(double value) {
        return Decimals(value / 60.0);
    }
    #endregion
}