using Aequus.Common.Items;
using System;
using System.Collections.Generic;
using Terraria.GameContent.UI;

namespace Aequus.Content.Items.Accessories.Informational.Monocle;

public sealed class MonocleGlobalItem : GlobalItem {
    private TooltipLine GetMonoclePriceTip(Item item, Player player) {
        player.GetItemExpectedPrice(item, out var calcForSelling, out var calcForBuying);
        long value = item.isAShopItem || item.buyOnce ? calcForBuying : calcForSelling;
        if (item.shopSpecialCurrency != -1) {
            string[] text = new string[1];
            int line = 0;
            CustomCurrencyManager.GetPriceText(item.shopSpecialCurrency, text, ref line, value);
            return new TooltipLine(Mod, "SpecialPrice", text[0]) { OverrideColor = Color.White, };
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

            TooltipLine t = new(Mod, "Price", Lang.tip[item.buy ? 50 : 49].Value + " " + text);

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
            return new(Mod, "Price", Lang.tip[51].Value) { OverrideColor = new(120, 120, 120) };
        }
        return null;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
        if (!Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoMoneyMonocle || ModContent.GetInstance<MonocleBuilderToggle>().CurrentState != 0
            || item.value < 0 || item.IsACoin || tooltips.Find((t) => t.Name == "Price" || t.Name == "SpecialPrice") != null) {
            return;
        }

        var tt = GetMonoclePriceTip(item, Main.LocalPlayer);
        if (tt != null) {
            tooltips.Insert(tooltips.GetIndex("Price"), tt);
        }
    }
}