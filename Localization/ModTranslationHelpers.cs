using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Localization
{
    internal sealed class ModTranslationHelpers : ILoadable
    {
        internal static FieldInfo translationsField;
        internal static Dictionary<string, ModTranslation> Text;

        internal static void NewFromDict(string key, string key2, Func<string, string> modifyText)
        {
            try
            {
                List<(int, string)> replacements = new List<(int, string)>();
                var dict = GetTranslationsDict(Text["Mods.Aequus." + key]);
                foreach (var value in dict)
                {
                    replacements.Add((value.Key, modifyText(value.Value)));
                }
                var text = LocalizationLoader.CreateTranslation("Mods.Aequus." + key + key2);
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

        internal static void OnModLoad(Aequus mod)
        {
            translationsField = typeof(ModTranslation).GetField("translations", BindingFlags.NonPublic | BindingFlags.Instance);
            Text = (Dictionary<string, ModTranslation>)typeof(LocalizationLoader).GetField("translations", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        }

        void ILoadable.Load(Mod mod)
        {
        }

        void ILoadable.Unload()
        {
            translationsField = null;
            Text = null;
        }
    }
}
