using Aequus.Items;
using Microsoft.Xna.Framework;
using ReLogic.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus {
    public class TextHelper : IPostAddRecipes
    {
        public static Color BossSummonMessage = new Color(175, 75, 255, 255);
        public static Color EventMessage = new Color(50, 255, 130, 255);
        public static Color PrefixGood = new Color(120, 190, 120, 255);
        public static Color PrefixBad = new Color(190, 120, 120, 255);

        public static FormatOptions DefaultPercent = new(0);

        public static CultureInfo InvariantCulture => CultureInfo.InvariantCulture;

        private record struct TextModification(string Key, Action<LocalizedText> ModificationMethod);
        public record struct FormatOptions(byte ShowDecimals) {
            public string GetF() {
                return "F" + ShowDecimals;
            }
        }

        internal class Modifications {
            public static void UpdateItemCommands(LocalizedText text) {
                text.SetValue(Helper.SubstitutionRegex.Replace(text.Value, (match) =>
                {
                    if (match.Groups[1].Length != 0) {
                        return "";
                    }

                    string name = match.Groups[2].ToString();
                    if (ItemID.Search.TryGetId(name, out int directItem)) {
                        return ItemCommand(directItem);
                    }
                    if (ItemID.Search.TryGetId($"Aequus/{name}", out int item)) {
                        return ItemCommand(item);
                    }
                    return "";
                }));
            }
        }

        public class Create {
            public static string Percent(float percent, FormatOptions formatOptions = default) {
                string value = (percent * 100f).ToString(formatOptions.GetF(), InvariantCulture);
                return value;
            }
            public static string MultiplierPercentDifference(float percent, FormatOptions formatOptions = default) {
                return Percent(1f - Math.Abs(percent));
            }

            public static string ChancePercent(float chancePercent, FormatOptions formatOptions = default) {
                string value = (chancePercent * 100f).ToString(formatOptions.GetF(), InvariantCulture);
                return value;
            }
            public static string ChanceFracPercent(float denominator, float numerator = 1f, FormatOptions formatOptions = default) {
                return ChancePercent(numerator / denominator, formatOptions);
            }
        }

        internal static MethodInfo LocalizedText_SetValue;
        private static List<TextModification> textModifications = new();

        public static string Unknown => GetTextValue("Unknown");
        public static string ArmorSetBonusKey => Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");

        internal static void ModifyText(string key, Action<LocalizedText> action)
        {
            textModifications.Add(new("Mods.Aequus." + key, action));
        }

        public void Load(Mod mod)
        {
            LocalizedText_SetValue = typeof(LocalizedText).GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance);
            On_LanguageManager.SetLanguage_GameCulture += LanguageManager_SetLanguage;
        }

        private static void LanguageManager_SetLanguage(On_LanguageManager.orig_SetLanguage_GameCulture orig, LanguageManager self, GameCulture culture) {

            bool updateText = self.ActiveCulture != culture;

            orig(self, culture);

            if (updateText) {
                foreach (var t in textModifications) {
                    t.ModificationMethod(self.GetText(t.Key));
                }
            }
        }

        public void PostAddRecipes(Aequus aequus) {
            foreach (var t in textModifications) {
                t.ModificationMethod(Language.GetText(t.Key));
            }
        }

        public void Unload()
        {
            textModifications.Clear();
        }

        public static LocalizedText GetItemName<T>() where T : ModItem {
            var modItem = ModContent.GetInstance<T>();
            return Language.GetText("Mods." + modItem.Mod.Name + ".Items." + modItem.Name + ".DisplayName");
        }

        public static LocalizedText GetOrRegister(string key, Func<string> makeDefaultValue = null)
        {
            return Language.GetOrRegister("Mods.Aequus." + key, makeDefaultValue);
        }
        public static LocalizedText GetText(string key)
        {
            return Language.GetText("Mods.Aequus." + key);
        }

        public static string GetTextValue(string key)
        {
            return GetText(key).Value;
        }

        public static string GetTextValue(string key, params object[] args)
        {
            return Language.GetTextValue("Mods.Aequus." + key, args);
        }

        public static string GetTextValueWith(string key, object obj)
        {
            return Language.GetTextValueWith("Mods.Aequus." + key, obj);
        }

        public static string PriceTextColored(long value, string noValue = "", bool alphaPulse = false) {
            if (value < 1) {
                return noValue;
            }

            List<string> list = new();
            var coins = Utils.CoinsSplit(value);

            for (int i = coins.Length - 1; i >= 0; i--) {

                if (coins[i] < 1) {
                    continue;
                }

                var clr = i switch {
                    3 => Colors.CoinPlatinum,
                    2 => Colors.CoinGold,
                    1 => Colors.CoinSilver,
                    _ => Colors.CoinCopper,
                };
                list.Add(ColorCommand(coins[i].ToString() + " " + Lang.inter[18 - i], clr, alphaPulse));
            }

            return string.Join(' ', list);
        }
        public static string PriceText(long value, string noValue = "") {
            return string.Join(' ', PriceTextList(value));
        }
        public static List<string> PriceTextList(long value, string noValue = "") {
            // bad bad!
            List<string> text = new();
            int platinum = 0;
            int gold = 0;
            int silver = 0;
            int copper = 0;
            int itemValue = (int)value;
            if (itemValue < 1)
            {
                text.Add(noValue);
                return text;
            }
            if (itemValue >= Item.platinum)
            {
                platinum = itemValue / Item.platinum;
                itemValue -= platinum * Item.platinum;
            }
            if (itemValue >= Item.gold)
            {
                gold = itemValue / Item.gold;
                itemValue -= gold * Item.gold;
            }
            if (itemValue >= Item.silver)
            {
                silver = itemValue / Item.silver;
                itemValue -= silver * Item.silver;
            }
            if (itemValue >= Item.copper)
            {
                copper = itemValue;
            }

            if (platinum > 0)
            {
                text.Add(platinum + " " + Lang.inter[15].Value);
            }
            if (gold > 0)
            {
                text.Add(gold + " " + Lang.inter[16].Value);
            }
            if (silver > 0)
            {
                text.Add(silver + " " + Lang.inter[17].Value);
            }
            if (copper > 0)
            {
                text.Add(copper + " " + Lang.inter[18].Value);
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

        public static string ColorCommandStart(Color color, bool alphaPulse = false)
        {
            if (alphaPulse)
            {
                color = Colors.AlphaDarken(color);
            }
            return $"[c/{color.Hex3()}:";
        }
        public static string ColorCommand(string text, Color color, bool alphaPulse = false)
        {
            return $"{ColorCommandStart(color, alphaPulse)}{text}]";
        }

        public static string ItemCommand(int itemID)
        {
            return "[i:" + itemID + "]";
        }
        public static string ItemCommand<T>() where T : ModItem
        {
            return ItemCommand(ModContent.ItemType<T>());
        }

        public static void Broadcast(string text, Color color, params object[] args) {
            text = "Mods.Aequus." + text;
            if (Main.netMode == NetmodeID.SinglePlayer) {
                Main.NewText(Language.GetTextValue(text, args), color);
            }
            else if (Main.netMode == NetmodeID.Server) {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text, args), color);
            }
        }
        /// <summary>
        /// Broadcasts a language key with "Mods.Aequus." appended behind it.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void Broadcast(string text, Color color) {
            BroadcastLiteral("Mods.Aequus." + text, color);
        }
        /// <summary>
        /// Broadcasts a literal key.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        public static void BroadcastLiteral(string text, Color color) {
            if (Main.netMode != NetmodeID.Server) {
                Main.NewText(Language.GetTextValue(text), color);
                return;
            }

            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), color);
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
                return Language.GetTextValue(Helper.CapSpaces(RarityLoader.GetRarity(rare).Name)).Replace(" Rarity", "");
            }
            return GetTextValue("Unknown");
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
            return liquidType switch
            {
                LiquidID.Water => Language.GetTextValue("Mods.Aequus.Water"),
                LiquidID.Lava => Language.GetTextValue("Mods.Aequus.Lava"),
                LiquidID.Honey => Language.GetTextValue("Mods.Aequus.Honey"),
                3 => Language.GetTextValue("Mods.Aequus.Shimmer"),
                _ => Unknown,
            };
        }

        public static string NPCKeyName(int npcID, Mod myMod = null)
        {
            if (npcID < NPCID.Count)
                return NPCID.Search.GetName(npcID);

            var modNPC = NPCLoader.GetNPC(npcID);
            if (myMod != null && modNPC.Mod.Name == myMod.Name)
                return modNPC.Name;

            return $"{modNPC.Mod.Name}_{modNPC.Name}";
        }
        public static string ItemKeyName(int itemID, Mod myMod = null)
        {
            if (itemID < ItemID.Count)
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
                value = GetTextValue("KeyUnbound");
            }
            return value;
        }

        public static string GetInternalNameOrUnknown(int id, IdDictionary search)
        {
            if (search.TryGetName(id, out string name))
                return name;
            return "Unknown";
        }
    }

    public static class TextExtensions {
        public static string AddLine(this string text, string append) {
            if (!string.IsNullOrEmpty(text)) {
                text += "\n";
            }
            return text + append;
        }
    }
}