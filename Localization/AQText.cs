using Terraria.Localization;
using Terraria.Utilities;

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

        private static TextCallback callback;

        internal static void UpdateCallback()
        {
            callback?.UpdateCallback();
        }

        public static LocalizedText AequusItemName(string internalName)
        {
            return ModText("ItemName." + internalName);
        }

        public static string DisableThing(object arg)
        {
            return string.Format(ModText("Common.DisableThing").Value, arg);
        }

        public static string EnableThing(object arg)
        {
            return string.Format(ModText("Common.EnableThing").Value, arg);
        }

        public static LocalizedText RobsterChat(int type)
        {
            return ModText("Common.RobsterChat" + type);
        }

        public static LocalizedText EightballAnswer(int type)
        {
            return ModText("Common.EightballAnswer" + type);
        }

        public static LocalizedText EightballMisc(int type)
        {
            return ModText("Common.EightballMisc" + type);
        }

        public static LocalizedText CrabSeason()
        {
            return ModText("Common.CrabSeason");
        }

        public static LocalizedText GlimmerEvent()
        {
            return ModText("Common.GlimmerEvent");
        }

        public static LocalizedText GlimmerEventWarning()
        {
            return ModText("Common.GlimmerEventWarning");
        }

        public static LocalizedText GlimmerEventEnding()
        {
            return ModText("Common.GlimmerEventEnding");
        }

        public static LocalizedText ModText(string key)
        {
            var text = Language.GetText(Key + key);
            if (text.Value == key)
            {
                return callback == null ? text : callback.OnMissingText(key, text);
            }
            return text;
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
            callback = new TextCallback();
        }

        internal static void Unload()
        {
            callback = null;
        }
    }
}