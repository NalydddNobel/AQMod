using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria.Localization;
using Terraria.Utilities;

namespace Aequus.Core.Utilities;
public static class ExtendLanguage {
    private record struct ColoredText(string Text, Color Color);

    public static string PrettyPrint(string text) {
        return Regex.Replace(text, "([A-Z])", " $1").Trim();
    }

    /// <summary>Gets a <see cref="ILocalizedModType"/>'s "DisplayName" value.</summary>
    public static LocalizedText GetRandomLocalizationFromCategory(this ILocalizedModType localizedModType, string suffix, UnifiedRandom random = null) {
        string filterText = localizedModType.GetLocalizationKey(suffix);
        return Language.SelectRandom((key, value) => key.StartsWith(filterText), random);
    }

    /// <summary>Gets a <see cref="ILocalizedModType"/>'s "DisplayName" value.</summary>
    public static LocalizedText GetDisplayName(ILocalizedModType localizedModType) {
        return localizedModType.GetLocalization("DisplayName");
    }

    /// <summary><inheritdoc cref="GetDisplayName(ILocalizedModType)"/></summary>
    public static LocalizedText GetDisplayName<T>() where T : class, ILocalizedModType {
        return GetDisplayName(ModContent.GetInstance<T>());
    }

    public static LocalizedText GetDialogue(this ILocalizedModType localizedModType, string suffix) {
        return localizedModType.GetLocalization($"Dialogue.{suffix}");
    }

    public static LocalizedText GetMapEntry<T>() where T : ModTile, ILocalizedModType {
        return ModContent.GetInstance<T>().GetLocalization("MapEntry");
    }

    /// <returns>Whether this key has a value. (<see cref="Language.GetTextValue(string)"/> doesnt return the key.)</returns>
    public static bool ContainsKey(string key) {
        return Language.GetTextValue(key) != key;
    }

    /// <returns>Whether the text exists. (<see cref="Language.GetTextValue(string)"/> doesnt return the key.)</returns>
    public static bool TryGet(string key, out LocalizedText text) {
        text = Language.GetText(key);
        return text.Key != text.Value;
    }
    /// <returns><inheritdoc cref="TryGet(string, out LocalizedText)"/></returns>
    public static bool TryGetValue(string key, out string text) {
        bool value = TryGet(key, out var localizedText);
        text = localizedText.Value;
        return value;
    }

    /// <returns>The Category Key. (Mods.ModName.Category.Suffix)</returns>
    public static string GetCategoryKey(this ILocalizedModType self, string suffix, Func<string> defaultValueFactory = null) {
        return $"Mods.{self.Mod.Name}.{self.LocalizationCategory}.{suffix}";
    }

    /// <returns>A localized text using a category key. (Mods.ModName.Category.Suffix)</returns>
    public static LocalizedText GetCategoryText(this ILocalizedModType self, string suffix, Func<string> defaultValueFactory = null) {
        return Language.GetOrRegister(GetCategoryKey(self, suffix), defaultValueFactory);
    }

    /// <returns>A text value using a category key. (Mods.ModName.Category.Suffix)</returns>
    public static string GetCategoryTextValue(this ILocalizedModType self, string suffix) {
        return self.GetCategoryText(suffix).Value;
    }

    /// <returns>Localized name of a <see cref="System.DayOfWeek"/> value.</returns>
    public static LocalizedText DayOfWeek(DayOfWeek dayOfWeek) {
        return Language.GetText("Mods.Aequus.Misc.DayOfTheWeek." + dayOfWeek.ToString());
    }

    /// <returns>Price Text for the specified value, or <paramref name="NoValueText"/> if value is less than or equal to 0.</returns>
    public static string PriceText(long value, string NoValueText = "") {
        return string.Join(' ', GetPriceTextSegments(value, NoValueText).Select((t) => t.Text));
    }

    /// <returns><inheritdoc cref="PriceText(long, string)"/> Colored using chat commands.</returns>
    public static string PriceTextColored(long value, string NoValueText = "", bool pulse = false) {
        return string.Join(' ', GetPriceTextSegments(value, NoValueText).Select((t) => t.Color == Color.White ? t.Text : ChatTagWriter.Color(pulse ? Colors.AlphaDarken(t.Color) : t.Color, t.Text)));
    }

    private static IEnumerable<ColoredText> GetPriceTextSegments(long value, string NoValueText = "") {
        int platinum = 0;
        int gold = 0;
        int silver = 0;
        int copper = 0;
        int itemValue = (int)value;

        if (itemValue < 1) {
            yield return new ColoredText(NoValueText, Color.White);
        }

        if (itemValue >= Item.platinum) {
            platinum = itemValue / Item.platinum;
            itemValue -= platinum * Item.platinum;
        }
        if (itemValue >= Item.gold) {
            gold = itemValue / Item.gold;
            itemValue -= gold * Item.gold;
        }
        if (itemValue >= Item.silver) {
            silver = itemValue / Item.silver;
            itemValue -= silver * Item.silver;
        }
        if (itemValue >= Item.copper) {
            copper = itemValue;
        }

        if (platinum > 0) {
            yield return new ColoredText(platinum + " " + LanguageDatabase.inter[15].Value, TCommonColor.CoinPlatinum);
        }
        if (gold > 0) {
            yield return new ColoredText(gold + " " + LanguageDatabase.inter[16].Value, TCommonColor.CoinGold);
        }
        if (silver > 0) {
            yield return new ColoredText(silver + " " + LanguageDatabase.inter[17].Value, TCommonColor.CoinSilver);
        }
        if (copper > 0) {
            yield return new ColoredText(copper + " " + LanguageDatabase.inter[18].Value, TCommonColor.CoinCopper);
        }
    }

    /// <param name="useAnimation">Item use animation.</param>
    /// <returns>Localized use animation (speed) text based off vanilla thresholds.</returns>
    public static string GetUseAnimationText(float useAnimation) {
        if (useAnimation <= 8) {
            return Language.GetTextValue("LegacyTooltip.6");
        }
        else if (useAnimation <= 20) {
            return Language.GetTextValue("LegacyTooltip.7");
        }
        else if (useAnimation <= 25) {
            return Language.GetTextValue("LegacyTooltip.8");
        }
        else if (useAnimation <= 30) {
            return Language.GetTextValue("LegacyTooltip.9");
        }
        else if (useAnimation <= 35) {
            return Language.GetTextValue("LegacyTooltip.10");
        }
        else if (useAnimation <= 45) {
            return Language.GetTextValue("LegacyTooltip.11");
        }
        else if (useAnimation <= 55) {
            return Language.GetTextValue("LegacyTooltip.12");
        }
        return Language.GetTextValue("LegacyTooltip.13");
    }

    /// <param name="knockback">Weapon Knockback.</param>
    /// <returns>Localized knockback text based off vanilla thresholds.</returns>
    public static string GetKnockbackText(float knockback) {
        if (knockback == 0f) {
            return Language.GetTextValue("LegacyTooltip.14");
        }
        else if (knockback <= 1.5) {
            return Language.GetTextValue("LegacyTooltip.15");
        }
        else if (knockback <= 3f) {
            return Language.GetTextValue("LegacyTooltip.16");
        }
        else if (knockback <= 4f) {
            return Language.GetTextValue("LegacyTooltip.17");
        }
        else if (knockback <= 6f) {
            return Language.GetTextValue("LegacyTooltip.18");
        }
        else if (knockback <= 7f) {
            return Language.GetTextValue("LegacyTooltip.19");
        }
        else if (knockback <= 9f) {
            return Language.GetTextValue("LegacyTooltip.20");
        }
        else if (knockback <= 11f) {
            return Language.GetTextValue("LegacyTooltip.21");
        }
        return Language.GetTextValue("LegacyTooltip.22");
    }

    /// <param name="keybind"></param>
    /// <returns>An enumerable of each key for this keybind. Returns "Unbound Key" if no keys are assigned.</returns>
    public static IEnumerable<string> GetKeybindKeys(ModKeybind keybind) {
        List<string> keys = keybind.GetAssignedKeys();

        if (keys.Count == 0) {
            yield return Language.GetTextValue("Mods.Aequus.KeyUnbound");
        }
        else {
            foreach (var s in keys) {
                yield return s;
            }
        }
    }

    /// <summary>Copied from example mod and doesnt put any effort to localize. Uses Terraria day/night cycle styled time data.</summary>
    /// <param name="time">The Day/Night cycle time.</param>
    /// <param name="dayTime">Whether it's day or night.</param>
    public static string WatchTime(double time, bool dayTime) {
        string text = "AM";
        if (!dayTime) {
            time += 54000.0;
        }

        time = time / 86400.0 * 24.0;
        time = time - 7.5 - 12.0;
        if (time < 0.0) {
            time += 24.0;
        }

        if (time >= 12.0) {
            text = "PM";
        }

        int intTime = (int)time;
        double deltaTime = time - intTime;
        deltaTime = (int)(deltaTime * 60.0);
        string text2 = string.Concat(deltaTime);
        if (deltaTime < 10.0) {
            text2 = "0" + text2;
        }

        if (intTime > 12) {
            intTime -= 12;
        }

        if (intTime == 0) {
            intTime = 12;
        }

        return $"{intTime}:{text2} {text}";
    }

    /// <summary>Converts ticks to seconds, up to 1 decimal place.</summary>
    public static string Minutes(double value) {
        return Decimals(value / 3600.0);
    }

    /// <summary>Converts ticks to seconds, up to 1 decimal place.</summary>
    public static string Seconds(double value) {
        return Decimals(value / 60.0);
    }

    /// <summary>Converts a decimal value into percentage text, up to 1 decimal place.</summary>
    public static string Percent(double value) {
        return Decimals(value * 100f);
    }

    /// <summary>Converts value into decimal text, up to 1 decimal place.</summary>
    public static string Decimals(double value) {
        return value.ToString("0.0", Language.ActiveCulture.CultureInfo.NumberFormat).Replace(".0", "");
    }

    /// <summary>Registers a localization key if it doesn't exist. Only ran if compiled with a DEBUG symbol.</summary>
    [Conditional("DEBUG")]
    internal static void RegisterKey(string key) {
        Language.GetOrRegister(key);
    }
}
