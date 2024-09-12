using Aequus.Buffs.Misc.Empowered;
using Aequus.Common.DataSets;
using Aequus.Common.Items;
using Aequus.Content.ItemRarities;
using Aequus.CrossMod;
using Aequus.NPCs.Town.ExporterNPC;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus;
public partial class AequusItem {
    public static Color HintColor => new Color(225, 100, 255, 255);

    private void Tooltip_ExporterDoubloons(Item item, List<TooltipLine> tooltips, NPC chatNPC) {
        if (chatNPC.type == ModContent.NPCType<Exporter>())
            ModifyPriceTooltip(item, tooltips, "Chat.Exporter");
    }

    private void Tooltip_Price(Item item, List<TooltipLine> tooltips, Player player, AequusPlayer aequus) {
        if (Main.npcShop > 0) {
            if (player.talkNPC != -1 && item.isAShopItem && item.buy && item.tooltipContext == ItemSlot.Context.ShopItem) {
                Tooltip_ExporterDoubloons(item, tooltips, Main.npc[player.talkNPC]);
            }
        }
        else if (aequus.accPriceMonocle) {
            if (item.value >= 0 && !item.IsACoin && tooltips.Find((t) => t.Name == "Price" || t.Name == "SpecialPrice") == null) {
                AddPriceTooltip(player, item, tooltips);
            }
        }
    }

    private void Tooltip_WeirdHints(Item item, List<TooltipLine> tooltips) {
        if (ItemSets.LegendaryFish.Contains(item.type)) {
            if (NPC.AnyNPCs(NPCID.Angler)) {
                tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#"), tooltips.Count), new TooltipLine(Mod, "AnglerHint", TextHelper.GetTextValue("Items.LegendaryFishHint")) { OverrideColor = HintColor, });
            }

            tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "Quest");
        }
    }

    private void Tooltip_BuffConflicts(Item item, List<TooltipLine> tooltips) {
        int originalBuffType = EmpoweredBuffBase.GetDepoweredBuff(item.buffType);
        if (originalBuffType > 0 && BuffSets.BuffConflicts.TryGetValue(originalBuffType, out var l) && l != null && l.Count > 0) {
            string text = "";
            if (l.Count == 1) {
                text = TextHelper.GetTextValueWith("Items.CommonItemTooltip.NewPotionsBalancing", new { PotionName = Lang.GetBuffName(l[0]), });
            }
            else {
                for (int i = 0; i < l.Count - 1; i++) {
                    if (!string.IsNullOrEmpty(text)) {
                        text += ", ";
                    }
                    text += Lang.GetBuffName(l[i]);
                }
                text = TextHelper.GetTextValueWith("Items.CommonItemTooltip.NewPotionsBalancing2", new { PotionName = text, PotionName2 = Lang.GetBuffName(l[^1]), });
            }
            tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#"), tooltips.Count), new TooltipLine(Mod, "PotionConflict", text));
        }
    }

    private void Tooltip_PickBreak(Item item, List<TooltipLine> tooltips) {
        if (item.pick > 0) {
            float pickDamage = Math.Max(Main.LocalPlayer.Aequus().pickTileDamage, 0f);
            if (item.pick != (int)(item.pick * pickDamage)) {
                foreach (var t in tooltips) {
                    if (t.Mod == "Terraria" && t.Name == "PickPower") {
                        string sign = "-";
                        var color = new Color(190, 120, 120, 255);
                        if (pickDamage > 1f) {
                            sign = "+";
                            color = new Color(120, 190, 120, 255);
                        }
                        t.Text = $"{item.pick}{TextHelper.ColorCommand($"({sign}{(int)Math.Abs(item.pick * (1f - pickDamage))})", color, alphaPulse: true)}{Language.GetTextValue("LegacyTooltip.26")}";
                    }
                }
            }
        }
    }

    private void Tooltip_DefenseChange(Item item, List<TooltipLine> tooltips) {
        if (defenseChange != 0) {
            for (int i = 0; i < tooltips.Count; i++) {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "Defense") {
                    if (defenseChange == -item.defense) {
                        tooltips.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (defenseChange <= -item.defense) {
                        tooltips[i].Text = $"-{tooltips[i].Text}";
                        break;
                    }
                    var text = tooltips[i].Text.Split(' ');
                    text[0] += defenseChange > 0 ?
                        TextHelper.ColorCommand($"(+{defenseChange})", TextHelper.PrefixGood, alphaPulse: true) :
                        TextHelper.ColorCommand($"({defenseChange})", TextHelper.PrefixBad, alphaPulse: true);
                    tooltips[i].Text = string.Join(' ', text);
                    break;
                }
            }
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        try {
            ModifyTooltipsInner(item, tooltips);
            var player = Main.LocalPlayer;
            var aequus = player.Aequus();

            Tooltip_WeirdHints(item, tooltips);
            Tooltip_BuffConflicts(item, tooltips);
            Tooltip_PickBreak(item, tooltips);
            Tooltip_DefenseChange(item, tooltips);
            Tooltip_Price(item, tooltips, player, aequus);
            CalamityMod.ModifyTooltips(item, tooltips);
        }
        catch {
        }
    }

    #region Tooltip Drawing
    public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset) {
        if (line.Mod == "Aequus") {
            if (line.Name.StartsWith("Fake")) {
                return false;
            }
            return true;
        }
        else if (line.Mod == "Terraria") {
            if (line.Name == "ItemName") {
                if (item.rare >= ItemRarityID.Count && RarityLoader.GetRarity(item.rare) is IDrawRarity drawRare) {
                    drawRare.DrawTooltipLine(line);
                    return false;
                }
            }
        }
        return true;
    }
    #endregion

    #region Static Methods
    private static void TestLootBagTooltip(Item item, List<TooltipLine> tooltips) {
        var dropTable = Helper.GetListOfDrops(Main.ItemDropsDB.GetRulesForItemID(item.type));

        for (int i = 0; i < dropTable.Count; i++) {
            tooltips.Add(new TooltipLine(Aequus.Instance, $"Drop{i}", dropTable[i]));
        }
    }
    private static void DebugEnemyDrops(int npcID, List<TooltipLine> tooltips) {
        var dropTable = Helper.GetListOfDrops(Main.ItemDropsDB.GetRulesForNPCID(npcID, includeGlobalDrops: false));

        for (int i = 0; i < dropTable.Count; i++) {
            tooltips.Add(new TooltipLine(Aequus.Instance, $"Drop{i}", dropTable[i]));
        }
    }
    internal static TooltipLine GetPriceTooltipLine(Player player, Item item) {
        player.GetItemExpectedPrice(item, out var calcForSelling, out var calcForBuying);
        long value = item.isAShopItem || item.buyOnce ? calcForBuying : calcForSelling;
        if (item.shopSpecialCurrency != -1) {
            string[] text = new string[1];
            int line = 0;
            CustomCurrencyManager.GetPriceText(item.shopSpecialCurrency, text, ref line, value);
            return new TooltipLine(Aequus.Instance, "SpecialPrice", text[0]) { OverrideColor = Color.White, };
        }
        else if (value > 0) {
            string text = "";
            long platinum = 0;
            long gold = 0;
            long silver = 0;
            long copper = 0;
            long itemValue = value * item.stack;
            if (!item.buy) {
                itemValue = value / 5;
                if (itemValue < 1) {
                    itemValue = 1;
                }
                long num3 = itemValue;
                itemValue *= item.stack;
                int amount = Main.shopSellbackHelper.GetAmount(item);
                if (amount > 0) {
                    itemValue += (-num3 + calcForBuying) * Math.Min(amount, item.stack);
                }
            }
            if (itemValue < 1) {
                itemValue = 1;
            }
            if (itemValue >= 1000000) {
                platinum = itemValue / 1000000;
                itemValue -= platinum * 1000000;
            }
            if (itemValue >= 10000) {
                gold = itemValue / 10000;
                itemValue -= gold * 10000;
            }
            if (itemValue >= 100) {
                silver = itemValue / 100;
                itemValue -= silver * 100;
            }
            if (itemValue >= 1) {
                copper = itemValue;
            }

            if (platinum > 0) {
                text = text + platinum + " " + Lang.inter[15].Value + " ";
            }
            if (gold > 0) {
                text = text + gold + " " + Lang.inter[16].Value + " ";
            }
            if (silver > 0) {
                text = text + silver + " " + Lang.inter[17].Value + " ";
            }
            if (copper > 0) {
                text = text + copper + " " + Lang.inter[18].Value + " ";
            }

            var t = new TooltipLine(Aequus.Instance, "Price", Lang.tip[item.buy ? 50 : 49].Value + " " + text);

            if (platinum > 0) {
                t.OverrideColor = Colors.CoinPlatinum;
            }
            else if (gold > 0) {
                t.OverrideColor = Colors.CoinGold;
            }
            else if (silver > 0) {
                t.OverrideColor = Colors.CoinSilver;
            }
            else if (copper > 0) {
                t.OverrideColor = Colors.CoinCopper;
            }
            return t;
        }
        else if (item.type != ItemID.DefenderMedal) {
            return new TooltipLine(Aequus.Instance, "Price", Lang.tip[51].Value) { OverrideColor = new Color(120, 120, 120, 255) };
        }
        return null;
    }
    private static void AddPriceTooltip(Player player, Item item, List<TooltipLine> tooltips) {
        var tt = GetPriceTooltipLine(player, item);
        if (tt != null) {
            tooltips.Add(tt);
        }
    }
    private static bool ModifyPriceTooltip(Item item, List<TooltipLine> lines, string key) {
        var t = lines.Find("Price");
        if (t != null) {
            t.Text = t.Text.Replace(Lang.inter[15].Value, TextHelper.GetTextValue(key + ".ShopPrice.Platinum"));
            t.Text = t.Text.Replace(Lang.inter[16].Value, TextHelper.GetTextValue(key + ".ShopPrice.Gold"));
            t.Text = t.Text.Replace(Lang.inter[17].Value, TextHelper.GetTextValue(key + ".ShopPrice.Silver"));
            t.Text = t.Text.Replace(Lang.inter[18].Value, TextHelper.GetTextValue(key + ".ShopPrice.Copper"));
        }
        return false;
    }
    internal static void FitTooltipBackground(List<TooltipLine> lines, int width, int height, int index = -1, string firstBoxName = "Fake") {
        var font = FontAssets.MouseText.Value;
        var measurement = font.MeasureString(Helper.AirCharacter.ToString());
        string t = "";
        var stringSize = Vector2.Zero;
        for (int i = 0; i < width; i++) {
            t += Helper.AirCharacter;
            stringSize = ChatManager.GetStringSize(font, t, Vector2.One);
            if (stringSize.X > width) {
                break;
            }
        }

        if (index == -1) {
            index = lines.Count - 1;
        }

        var mod = Aequus.Instance;
        int linesY = Math.Max((int)(height / stringSize.Y), 1);
        for (int i = 0; i < linesY; i++) {
            lines.Insert(index, new TooltipLine(mod, "Fake_" + i, t));
        }
        lines.Insert(index, new TooltipLine(mod, firstBoxName, t));
    }

    internal static TooltipLine PercentageModifierLine(float value, string key, bool negativeGood = false) {
        return new TooltipLine(Aequus.Instance, key, TextHelper.GetTextValue("Prefixes." + key, (value > 0f ? "+" : "") + (int)(value * 100f) + "%")) { IsModifier = true, IsModifierBad = value < 0f ? !negativeGood : negativeGood, };
    }
    internal static TooltipLine PercentageModifierLine(int num, int originalNum, string key, bool negativeGood = false) {
        if (num == originalNum) {
            return null;
        }

        float value = num / (float)originalNum;
        value = value < 1f ? -Math.Abs(1f - value) : value - 1f;

        return PercentageModifierLine(value, key, negativeGood);
    }
    internal static void PercentageModifier(float value, string key, List<TooltipLine> tooltips, bool good) {
        tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), PercentageModifierLine(value, key, good));
    }
    #endregion
}