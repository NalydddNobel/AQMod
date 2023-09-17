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

    #region Time
    public static string Seconds(double value) {
        return (value / 60.0).ToString("0.0", Language.ActiveCulture.CultureInfo.NumberFormat).Replace(".0", "");
    }
    #endregion
}