using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Localization
{
    internal static class AQText
    {
        public const string Key = "Mods.AQMod.";
        public const string ConfigNameKey = "$" + Key + "Common.ConfigName_";
        public const string ConfigHeaderKey = "$" + Key + "Common.ConfigHeader_";
        public const string ConfigValueKey = "$" + Key + "Common.Config_";

        public const int LangIDInterGrapplingHook = 90;
        public const int LangIDInterMount = 91;
        public const int LangIDInterPet = 92;
        public const int LangIDInterMinecart = 93;
        public const int LangIDInterLightPet = 94;

        internal static FieldInfo translationsField;
        internal static Dictionary<string, ModTranslation> modTranslations;

        public static string KeybindText(ModHotKey key)
        {
            foreach (var k in key.GetAssignedKeys())
            {
                return Language.GetTextValue("Mods.AQMod.Common.KeyBound", k);
            }
            return Language.GetTextValue("Mods.AQMod.Common.KeyUnbound");
        }

        public static string chooselocalizationtext(string en_US, string zh_Hans = null, string ru_RU = null)
        {
            if (ru_RU != null && Language.ActiveCulture == GameCulture.Russian)
            {
                return ru_RU;
            }
            else if (zh_Hans != null && Language.ActiveCulture == GameCulture.Chinese)
            {
                return zh_Hans;
            }
            else
            {
                return en_US;
            }
        }

        public static LocalizedText RobsterChat(int type)
        {
            return ModText("Common.RobsterChat" + type);
        }

        public static LocalizedText ModText(string key)
        {
            return Language.GetText(Key + key);
        }

        public static LocalizedText ArmorSetBonus(string setBonus)
        {
            return Language.GetText(Key + "Common.ArmorSetBonus_" + setBonus);
        }

        internal static string moonPhaseName(int phase)
        {
            switch (phase)
            {
                default:
                    {
                        return "Unknown";
                    }

                case 0:
                    {
                        return "Full Moon";
                    }

                case 1:
                    {
                        return "Waning Gibbious";
                    }

                case 2:
                    {
                        return "Third Quarter";
                    }

                case 3:
                    {
                        return "Waning Crescent";
                    }

                case 4:
                    {
                        return "New Moon";
                    }

                case 5:
                    {
                        return "Waxing Crescent";
                    }

                case 6:
                    {
                        return "First Quarter";
                    }

                case 7:
                    {
                        return "Waxing Gibbious";
                    }
            }
        }

        internal static void Load()
        {
            translationsField = typeof(ModTranslation).GetField("translations", BindingFlags.NonPublic | BindingFlags.Instance);
            modTranslations = (Dictionary<string, ModTranslation>)typeof(AQMod).GetField("translations", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(AQMod.Instance);
        }

        internal static void Unload()
        {
            translationsField = null;
            modTranslations = null;
        }

        internal static void AdjustTranslation(string key, string newKey, Func<string, string> modifyText)
        {
            AdjustTranslation(key, newKey, modifyText, AQMod.Instance);
        }
        internal static void AdjustTranslation(string key, string newKey, Func<string, string> modifyText, AQMod aQMod)
        {
            try
            {
                List<(int, string)> replacements = new List<(int, string)>();
                var dict = modTranslations["Mods.AQMod." + key].GetTranslationsDict();
                foreach (var value in dict)
                {
                    replacements.Add((value.Key, modifyText(value.Value)));
                }
                var text = aQMod.CreateTranslation(newKey);
                foreach (var value in replacements)
                {
                    text.AddTranslation(value.Item1, value.Item2);
                }
                aQMod.AddTranslation(text);
            }
            catch (Exception ex)
            {
                throw new Exception("failed on adjusting the {" + key + "} key.", ex);
            }
        }
        internal static Dictionary<int, string> GetTranslationsDict(this ModTranslation text)
        {
            return (Dictionary<int, string>)translationsField.GetValue(text);
        }

        public static string Item<T>() where T : ModItem
        {
            return Item(ModContent.ItemType<T>());
        }

        public static string Item(int item)
        {
            return "[i:" + item + "]";
        }
    }
}