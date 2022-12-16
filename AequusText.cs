using Aequus.Common;
using Aequus.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus
{
    public class AequusText : IOnModLoad
    {
        public static FieldInfo translationsField;
        public static Dictionary<string, ModTranslation> Text;
        public static Color BossSummonMessage => new Color(175, 75, 255, 255);
        internal static Color EventMessage => new Color(50, 255, 130, 255);

        public static string Unknown => GetText("Unknown");
        public static string ArmorSetBonusKey => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");

        public static void NewFromDict(string key, string key2, object obj)
        {
            NewFromDict(key, key2, (text) => AequusHelpers.FormatWith(text, obj));
        }
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

        void ILoadable.Load(Mod mod)
        {
        }

        void IOnModLoad.OnModLoad(Aequus aequus)
        {
            translationsField = typeof(ModTranslation).GetField("translations", BindingFlags.NonPublic | BindingFlags.Instance);
            Text = (Dictionary<string, ModTranslation>)typeof(LocalizationLoader).GetField("translations", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        }

        void ILoadable.Unload()
        {
            translationsField = null;
            Text = null;
        }
        public static ModTranslation GetTranslation(string key)
        {
            return Text["Mods.Aequus." + key];
        }

        public static string TryGetText(string key)
        {
            key = "Mods.Aequus." + key;
            if (Text.TryGetValue(key, out var translation))
            {
                return translation.GetTranslation(Language.ActiveCulture);
            }
            return key;
        }

        public static string GetText(string key)
        {
            return GetTranslation(key).GetTranslation(Language.ActiveCulture);
        }

        public static string GetText(string key, params object[] args)
        {
            return string.Format(GetText(key), args);
        }

        public static string GetTextWith(string key, object obj)
        {
            return Language.GetTextValueWith("Mods.Aequus." + key, obj);
        }

        public static string PriceText(int value, bool buy = false)
        {
            string text = "";
            int platinum = 0;
            int gold = 0;
            int silver = 0;
            int copper = 0;
            int itemValue = value;
            if (itemValue < 1)
            {
                itemValue = 1;
            }
            if (itemValue >= 1000000)
            {
                platinum = itemValue / 1000000;
                itemValue -= platinum * 1000000;
            }
            if (itemValue >= 10000)
            {
                gold = itemValue / 10000;
                itemValue -= gold * 10000;
            }
            if (itemValue >= 100)
            {
                silver = itemValue / 100;
                itemValue -= silver * 100;
            }
            if (itemValue >= 1)
            {
                copper = itemValue;
            }

            if (platinum > 0)
            {
                text = text + platinum + " " + Lang.inter[15].Value + " ";
            }
            if (gold > 0)
            {
                if (!string.IsNullOrEmpty(text))
                    text += " ";
                text = text + gold + " " + Lang.inter[16].Value;
            }
            if (silver > 0)
            {
                if (!string.IsNullOrEmpty(text))
                    text += " ";
                text = text + silver + " " + Lang.inter[17].Value;
            }
            if (copper > 0)
            {
                if (!string.IsNullOrEmpty(text))
                    text += " ";
                text = text + copper + " " + Lang.inter[18].Value;
            }
            return text;
        }

        public static string WatchTime(double time, bool dayTime)
        {
            string text = "AM";
            if (!dayTime)
            {
                time += 54000.0;
            }

            time = time / 86400.0 * 24.0;
            time = time - 7.5 - 12.0;
            if (time < 0.0)
            {
                time += 24.0;
            }

            if (time >= 12.0)
            {
                text = "PM";
            }

            int intTime = (int)time;
            double deltaTime = time - intTime;
            deltaTime = (int)(deltaTime * 60.0);
            string text2 = string.Concat(deltaTime);
            if (deltaTime < 10.0)
            {
                text2 = "0" + text2;
            }

            if (intTime > 12)
            {
                intTime -= 12;
            }

            if (intTime == 0)
            {
                intTime = 12;
            }

            return $"{intTime}:{text2} {text}";
        }

        public static string UseAnimationLine(float useAnimation)
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

        public static string KnockbackLine(float knockback)
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
        
        public static string ColorCommand(string text, Color color, bool alphaPulse = false)
        {
            if (alphaPulse)
            {
                color = Colors.AlphaDarken(color);
            }
            return "[c/" + color.Hex3() + ":" + text + "]";
        }

        public static string ItemCommand(int itemID)
        {
            return "[i:" + itemID + "]";
        }
        public static string ItemCommand<T>() where T : ModItem
        {
            return ItemCommand(ModContent.ItemType<T>());
        }

        public static void Broadcast(string text, Color color)
        {
            text = "Mods.Aequus." + text;
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(Language.GetTextValue(text), color);
            }
            else
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), color);
            }
        }
        public static void Broadcast(string text, Color color, params object[] args)
        {
            text = "Mods.Aequus." + text;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(text, args), color);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text, args), color);
            }
        }
        public static void BroadcastAwakened(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                BroadcastAwakened(npc.TypeName);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                BroadcastAwakened(Lang.GetNPCName(npc.netID).Key);
            }
        }
        public static void BroadcastAwakened(string npcName)
        {
            Broadcast("Announcement.HasAwoken", BossSummonMessage, npcName);
        }

        public static string GetRarityNameValue(int rare)
        {
            if (AequusItem.RarityNames.TryGetValue(rare, out string rarityName))
            {
                return Language.GetTextValue(rarityName);
            }

            if (rare >= ItemRarityID.Count)
            {
                return Language.GetTextValue(AequusHelpers.CapSpaces(RarityLoader.GetRarity(rare).Name)).Replace(" Rarity", "");
            }
            return GetText("Unknown");
        }

        public static bool ContainsKey(string key)
        {
            return Language.GetTextValue(key) != key;
        }

        public static bool TryGetText(string key, out string text)
        {
            text = Language.GetTextValue(key);
            return text != key;
        }

        public static string LiquidName(int liquidType)
        {
            switch (liquidType)
            {
                default:
                    return Unknown;

                case LiquidID.Water:
                    return Language.GetTextValue("Mods.Aequus.Water");
                case LiquidID.Lava:
                    return Language.GetTextValue("Mods.Aequus.Lava");
                case LiquidID.Honey:
                    return Language.GetTextValue("Mods.Aequus.Honey");
                case 3:
                    return Language.GetTextValue("Mods.Aequus.Shimmer");
            }
        }

        public static string NPCKeyName(int npcID, Mod myMod = null)
        {
            if (npcID < Main.maxNPCTypes)
                return NPCID.Search.GetName(npcID);

            var modNPC = NPCLoader.GetNPC(npcID);
            if (myMod != null && modNPC.Mod.Name == myMod.Name)
                return modNPC.Name;

            return $"{modNPC.Mod.Name}_{modNPC.Name}";
        }
        public static string ItemKeyName(int itemID, Mod myMod = null)
        {
            if (itemID < Main.maxItemTypes)
                return ItemID.Search.GetName(itemID);

            var modItem = ItemLoader.GetItem(itemID);
            if (myMod != null && modItem.Mod.Name == myMod.Name)
                return modItem.Name;

            return $"{modItem.Mod.Name}_{modItem.Name}";
        }

        public static string IDSearchName(string name)
        {
            if (name.StartsWith("Aequus/"))
            {
                var split = name.Split('/');
                name = "";
                for (int i = 1; i < split.Length; i++)
                {
                    if (i != 1)
                        name += "/";
                    name += split[i];
                }
            }
            //else if (!name.Contains('/'))
            //{
            //    name = "Terraria_" + name;
            //}

            return name.Replace('/', '_');
        }

        public static string GetKeybindKeys(ModKeybind keybind)
        {
            string value = "";
            foreach (var s in keybind.GetAssignedKeys())
            {
                if (!string.IsNullOrEmpty(value))
                    value += ", ";
                value += s;
            }
            if (string.IsNullOrEmpty(value))
            {
                value = GetText("KeyUnbound");
            }
            return value;
        }
    }
}