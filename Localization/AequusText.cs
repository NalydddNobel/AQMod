using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Chat;

namespace Aequus.Localization
{
    public sealed partial class AequusText
    {
        internal static FieldInfo translationsField;
        internal static Dictionary<string, ModTranslation> modTranslations;

        internal static void OnModLoad(Aequus aequus)
        {
            translationsField = typeof(ModTranslation).GetField("translations", BindingFlags.NonPublic | BindingFlags.Instance);
            modTranslations = (Dictionary<string, ModTranslation>)typeof(LocalizationLoader).GetField("translations", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        }

        internal static void AdjustTranslation(string key, string newKey, Func<string, string> modifyText)
        {
            AdjustTranslation(key, newKey, modifyText, Aequus.Instance);
        }
        internal static void AdjustTranslation(string key, string newKey, Func<string, string> modifyText, Aequus aQMod)
        {
            try
            {
                List<(int, string)> replacements = new List<(int, string)>();
                var dict = GetTranslationsDict(modTranslations["Mods.Aequus." + key]);
                foreach (var value in dict)
                {
                    replacements.Add((value.Key, modifyText(value.Value)));
                }
                var text = LocalizationLoader.CreateTranslation("Mods.Aequus." + newKey);
                foreach (var value in replacements)
                {
                    text.AddTranslation(value.Item1, value.Item2);
                }
                LocalizationLoader.AddTranslation(text);
            }
            catch (Exception ex)
            {
                throw new Exception("failed on adjusting the {" + key + "} key.", ex);
            }
        }
        internal static Dictionary<int, string> GetTranslationsDict(ModTranslation text)
        {
            return (Dictionary<int, string>)translationsField.GetValue(text);
        }
    }
}