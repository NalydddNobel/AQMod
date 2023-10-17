using Aequus.Common.DataSets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusItem {
    private TooltipLine Tooltip_Monocle_GetTooltipLine(Item item, Player player) {
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

            TooltipLine t = new(Aequus.Instance, "Price", Lang.tip[item.buy ? 50 : 49].Value + " " + text);

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
            return new(Aequus.Instance, "Price", Lang.tip[51].Value) { OverrideColor = new Color(120, 120, 120, 255) };
        }
        return null;
    }

    private void TooltipMonocle(Item item, List<TooltipLine> tooltips, Player localPlayer, AequusPlayer localAequusPlayer) {
        if (!localAequusPlayer.accMonocle) {
            return;
        }
        if (item.value >= 0 && !item.IsACoin && tooltips.Find((t) => t.Name == "Price" || t.Name == "SpecialPrice") == null) {
            var tt = Tooltip_Monocle_GetTooltipLine(item, localPlayer);
            if (tt != null) {
                tooltips.Insert(tooltips.GetIndex("Price"), tt);
            }
        }
    }

    private void TooltipShimmerTransform(Item item, List<TooltipLine> tooltips, Player localPlayer, AequusPlayer localAequusPlayer) {
        if (!localAequusPlayer.accShimmerMonocle) {
            return;
        }

        int itemId = ItemID.Sets.ShimmerCountsAsItem[item.type] != -1 ? ItemID.Sets.ShimmerCountsAsItem[item.type] : item.type;
        var tooltipColor = Color.Lerp(Color.White, Color.BlueViolet, 0.33f);
        if (itemId == ItemID.GelBalloon && !NPC.unlockedSlimeRainbowSpawn) {
            tooltips.Insert(tooltips.GetIndex("Material", 1), new(Mod, "Shimmerable", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.ShimmerableToNPC", Lang.GetNPCNameValue(NPCID.TownSlimeRainbow))) { OverrideColor = tooltipColor });
        }

        if (ItemID.Sets.ShimmerTransformToItem[itemId] <= 0 || ItemSets.ShimmerTooltipResultIgnore.Contains(ItemID.Sets.ShimmerTransformToItem[itemId]) || !item.CanShimmer()) {
            return;
        }

        tooltips.Insert(tooltips.GetIndex("Material", 1), new(Mod, "Shimmerable", Language.GetTextValue("Mods.Aequus.Items.CommonTooltips.Shimmerable", ItemID.Sets.ShimmerTransformToItem[itemId], Lang.GetItemNameValue(ItemID.Sets.ShimmerTransformToItem[itemId]))) { OverrideColor = tooltipColor });
    }
}