using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria.Localization;

namespace Aequus.Core.Utilities;
public static class ExtendLanguage {
    private record struct ColoredText(String Text, Color Color);

    /// <summary>Gets a <see cref="ILocalizedModType"/>'s "DisplayName" value.</summary>
    public static LocalizedText GetDisplayName(ILocalizedModType localizedModType) {
        return localizedModType.GetLocalization("DisplayName");
    }

    /// <summary><inheritdoc cref="GetDisplayName(ILocalizedModType)"/></summary>
    public static LocalizedText GetDisplayName<T>() where T : class, ILocalizedModType {
        return GetDisplayName(ModContent.GetInstance<T>());
    }

    public static LocalizedText GetDialogue(this ILocalizedModType localizedModType, String suffix) {
        return localizedModType.GetLocalization($"Dialogue.{suffix}");
    }

    public static LocalizedText GetMapEntry<T>() where T : ModTile, ILocalizedModType {
        return ModContent.GetInstance<T>().GetLocalization("MapEntry");
    }

    /// <returns>Whether this key has a value. (<see cref="Language.GetTextValue(String)"/> doesnt return the key.)</returns>
    public static Boolean ContainsKey(String key) {
        return Language.GetTextValue(key) != key;
    }

    /// <returns>Whether the text exists. (<see cref="Language.GetTextValue(String)"/> doesnt return the key.)</returns>
    public static Boolean TryGet(String key, out LocalizedText text) {
        text = Language.GetText(key);
        return text.Key != text.Value;
    }
    /// <returns><inheritdoc cref="TryGet(String, out LocalizedText)"/></returns>
    public static Boolean TryGetValue(String key, out String text) {
        Boolean value = TryGet(key, out var localizedText);
        text = localizedText.Value;
        return value;
    }

    /// <returns>The Category Key. (Mods.ModName.Category.Suffix)</returns>
    public static String GetCategoryKey(this ILocalizedModType self, String suffix, Func<String> defaultValueFactory = null) {
        return $"Mods.{self.Mod.Name}.{self.LocalizationCategory}.{suffix}";
    }

    /// <returns>A localized text using a category key. (Mods.ModName.Category.Suffix)</returns>
    public static LocalizedText GetCategoryText(this ILocalizedModType self, String suffix, Func<String> defaultValueFactory = null) {
        return Language.GetOrRegister(GetCategoryKey(self, suffix), defaultValueFactory);
    }

    /// <returns>A text value using a category key. (Mods.ModName.Category.Suffix)</returns>
    public static String GetCategoryTextValue(this ILocalizedModType self, String suffix) {
        return self.GetCategoryText(suffix).Value;
    }

    /// <returns>Localized name of a <see cref="System.DayOfWeek"/> value.</returns>
    public static LocalizedText DayOfWeek(DayOfWeek dayOfWeek) {
        return Language.GetText("Mods.Aequus.Misc.DayOfTheWeek." + dayOfWeek.ToString());
    }

    /// <returns>Price Text for the specified value, or <paramref name="NoValueText"/> if value is less than or equal to 0.</returns>
    public static String PriceText(Int64 value, String NoValueText = "") {
        return String.Join(' ', GetPriceTextSegments(value, NoValueText).Select((t) => t.Text));
    }

    /// <returns><inheritdoc cref="PriceText(Int64, String)"/> Colored using chat commands.</returns>
    public static String PriceTextColored(Int64 value, String NoValueText = "", Boolean AlphaPulse = false) {
        return String.Join(' ', GetPriceTextSegments(value, NoValueText).Select((t) => t.Color == Color.White ? t.Text : ChatCommandInserts.ColorCommand(t.Text, t.Color, AlphaPulse)));
    }

    private static IEnumerable<ColoredText> GetPriceTextSegments(Int64 value, String NoValueText = "") {
        Int32 platinum = 0;
        Int32 gold = 0;
        Int32 silver = 0;
        Int32 copper = 0;
        Int32 itemValue = (Int32)value;

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
            yield return new ColoredText(platinum + " " + Lang.inter[15].Value, Colors.CoinPlatinum);
        }
        if (gold > 0) {
            yield return new ColoredText(gold + " " + Lang.inter[16].Value, Colors.CoinGold);
        }
        if (silver > 0) {
            yield return new ColoredText(silver + " " + Lang.inter[17].Value, Colors.CoinSilver);
        }
        if (copper > 0) {
            yield return new ColoredText(copper + " " + Lang.inter[18].Value, Colors.CoinCopper);
        }
    }

    /// <param name="useAnimation">Item use animation.</param>
    /// <returns>Localized use animation (speed) text based off vanilla thresholds.</returns>
    public static String GetUseAnimationText(Single useAnimation) {
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
    public static String GetKnockbackText(Single knockback) {
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
    public static IEnumerable<String> GetKeybindKeys(ModKeybind keybind) {
        List<String> keys = keybind.GetAssignedKeys();

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
    public static String WatchTime(Double time, Boolean dayTime) {
        String text = "AM";
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

        Int32 intTime = (Int32)time;
        Double deltaTime = time - intTime;
        deltaTime = (Int32)(deltaTime * 60.0);
        String text2 = String.Concat(deltaTime);
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
    public static String Minutes(Double value) {
        return Decimals(value / 3600.0);
    }

    /// <summary>Converts ticks to seconds, up to 1 decimal place.</summary>
    public static String Seconds(Double value) {
        return Decimals(value / 60.0);
    }

    /// <summary>Converts value into percentage text, up to 1 decimal place.</summary>
    public static String Percent(Double value) {
        return Decimals(value * 100f);
    }

    /// <summary>Converts value into decimal text, up to 1 decimal place.</summary>
    public static String Decimals(Double value) {
        return value.ToString("0.0", Language.ActiveCulture.CultureInfo.NumberFormat).Replace(".0", "");
    }

    /// <summary>Registers a localization key if it doesn't exist. Only ran if compiled with a DEBUG symbol.</summary>
    [Conditional("DEBUG")]
    internal static void RegisterKey(String key) {
        Language.GetOrRegister(key);
    }
}
