using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus
{
    public class AequusText : ILoadable
    {
        public static FieldInfo translationsField;
        public static Dictionary<string, ModTranslation> Text;
        public static Color BossSummonMessage => new Color(175, 75, 255, 255);
        internal static Color EventMessage => new Color(50, 255, 130, 255);

        public static void NewFromDict(string key, string key2, Func<string, string> modifyText)
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
        public static Dictionary<int, string> GetTranslationsDict(ModTranslation text)
        {
            return (Dictionary<int, string>)translationsField.GetValue(text);
        }

        public static void OnModLoad(Aequus mod)
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

        public static string UseAnimText(float useAnimation)
        {
            if (useAnimation <= 8)
            {
                return Language.GetTextValue("LegacyTooltip.6");
            }
            else if (useAnimation <= 20)
            {
                return Language.GetTextValue("LegacyTooltip.7");
            }
            else if (useAnimation <= 25)
            {
                return Language.GetTextValue("LegacyTooltip.8");
            }
            else if (useAnimation <= 30)
            {
                return Language.GetTextValue("LegacyTooltip.9");
            }
            else if (useAnimation <= 35)
            {
                return Language.GetTextValue("LegacyTooltip.10");
            }
            else if (useAnimation <= 45)
            {
                return Language.GetTextValue("LegacyTooltip.11");
            }
            else if (useAnimation <= 55)
            {
                return Language.GetTextValue("LegacyTooltip.12");
            }
            return Language.GetTextValue("LegacyTooltip.13");
        }
        public static string KBText(float knockback)
        {
            if (knockback == 0f)
            {
                return Language.GetTextValue("LegacyTooltip.14");
            }
            else if (knockback <= 1.5)
            {
                return Language.GetTextValue("LegacyTooltip.15");
            }
            else if (knockback <= 3f)
            {
                return Language.GetTextValue("LegacyTooltip.16");
            }
            else if (knockback <= 4f)
            {
                return Language.GetTextValue("LegacyTooltip.17");
            }
            else if (knockback <= 6f)
            {
                return Language.GetTextValue("LegacyTooltip.18");
            }
            else if (knockback <= 7f)
            {
                return Language.GetTextValue("LegacyTooltip.19");
            }
            else if (knockback <= 9f)
            {
                return Language.GetTextValue("LegacyTooltip.20");
            }
            else if (knockback <= 11f)
            {
                return Language.GetTextValue("LegacyTooltip.21");
            }
            return Language.GetTextValue("LegacyTooltip.22");
        }
        public static string ItemText(int item)
        {
            return "[i:" + item + "]";
        }
        public static string ItemText<T>() where T : ModItem
        {
            return ItemText(ModContent.ItemType<T>());
        }

        public static void HasAwakened(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                HasAwakened(npc.TypeName);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                HasAwakened(Lang.GetNPCName(npc.netID).Key);
            }
        }
        public static void HasAwakened(string npcName)
        {
            Broadcast("Announcement.HasAwoken", BossSummonMessage, npcName);
        }
        public static void Broadcast(string text, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(text), color);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), color);
            }
        }
        public static void Broadcast(string text, Color color, params object[] args)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(text, args), color);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text, args), color);
            }
        }
    }
}
