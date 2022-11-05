using Aequus.Biomes.DemonSiege;
using Aequus.Buffs.Empowered;
using Aequus.Content;
using Aequus.Items;
using Aequus.Items.Prefixes.Potions;
using Aequus.NPCs.Friendly.Town;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Common.GlobalItems
{
    public class TooltipsGlobalItem : GlobalItem
    {
        public interface IDrawRarity
        {
            void DrawDevTooltip(DrawableTooltipLine line)
            {
                DrawDevTooltip(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color));
            }
            void DrawDevTooltip(string text, int x, int y, Color color)
            {
                DrawDevTooltip(text, x, y, 0f, Vector2.Zero, Vector2.One, color);
            }
            void DrawDevTooltip(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color);
        }

        public struct ItemDedication
        {
            public readonly Color color;

            public ItemDedication(Color color)
            {
                this.color = color;
            }
        }

        public static Color HintColor => new Color(225, 100, 255, 255);

        public static Dictionary<int, ItemDedication> Dedicated { get; private set; }

        public override void Load()
        {
            Dedicated = new Dictionary<int, ItemDedication>();
            //[ModContent.ItemType<RustyKnife>()] = new ItemDedication(new Color(30, 255, 60, 255)),
        }

        public override void Unload()
        {
            Dedicated?.Clear();
            Dedicated = null;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            try
            {
                var player = Main.LocalPlayer;
                var aequus = player.Aequus();

                if (item.type == ItemID.SoulofMight)
                {
                    foreach (var tt in tooltips)
                    {
                        if (tt.Name == "Tooltip0")
                        {
                            tt.Text = AequusText.GetText("ItemTooltip.Terraria_SoulofMight");
                        }
                    }
                }

                if (Dedicated.TryGetValue(item.type, out var dedication))
                {
                    tooltips.Insert(tooltips.GetIndex("Master"), new TooltipLine(Mod, "DedicatedItem", AequusText.GetText("ItemTooltip.Common.DedicatedItem")) { OverrideColor = dedication.color });
                }

                if (Main.npcShop > 0)
                {
                    if (player.talkNPC != -1 && item.isAShopItem && item.buy && item.tooltipContext == ItemSlot.Context.ShopItem && Main.npc[player.talkNPC].type == ModContent.NPCType<Exporter>())
                        ModifyPriceTooltip(item, tooltips, "Chat.Exporter");
                }
                else if (aequus.accPriceMonocle)
                {
                    if (item.value >= 0 && (item.type < ItemID.CopperCoin || item.type > ItemID.PlatinumCoin) || tooltips.Find((t) => t.Name == "Price") != null || tooltips.Find((t) => t.Name == "SpecialPrice") != null)
                    {
                        AddPriceTooltip(player, item, tooltips);
                    }
                }

                if (aequus.moroSummonerFruit && AequusItem.SummonStaff.Contains(item.type))
                {
                    tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "UseMana");
                }

                if (ExporterQuests.QuestItems.Contains(item.type))
                {
                    if (NPC.AnyNPCs(ModContent.NPCType<Exporter>()))
                        tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#") + 1, tooltips.Count), new TooltipLine(Mod, "ExporterHint", AequusText.GetText("ItemTooltip.Misc.ExporterHint")) { OverrideColor = HintColor, });
                    tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "Quest");
                }
                if (AequusItem.LegendaryFishIDs.Contains(item.type))
                {
                    if (NPC.AnyNPCs(NPCID.Angler))
                        tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#") + 1, tooltips.Count), new TooltipLine(Mod, "AnglerHint", AequusText.GetText("ItemTooltip.Misc.AnglerHint")) { OverrideColor = HintColor, });
                    tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "Quest");
                }

                if (item.pick > 0)
                {
                    float pickDamage = Math.Max(Main.LocalPlayer.Aequus().pickTileDamage, 0f);
                    if (item.pick != (int)(item.pick * pickDamage))
                    {
                        foreach (var t in tooltips)
                        {
                            if (t.Mod == "Terraria" && t.Name == "PickPower")
                            {
                                string sign = "-";
                                var color = new Color(190, 120, 120, 255);
                                if (pickDamage > 1f)
                                {
                                    sign = "+";
                                    color = new Color(120, 190, 120, 255);
                                }
                                t.Text = $"{item.pick}{AequusText.ColorCommand($"({sign}{(int)Math.Abs(item.pick * (1f - pickDamage))})", color, alphaPulse: true)}{Language.GetTextValue("LegacyTooltip.26")}";
                            }
                        }
                    }
                }

                if (item.prefix == ModContent.PrefixType<DoubledTimePrefix>() && item.buffTime != ContentSamples.ItemsByType[item.type].buffTime)
                {
                    PercentageModifier(item.buffTime, ContentSamples.ItemsByType[item.type].buffTime, "BuffDuration", tooltips, higherIsGood: true);
                }
                else if (item.prefix == ModContent.PrefixType<EmpoweredPrefix>())
                {
                    if (AequusText.TryGetText($"Mods.Aequus.ItemTooltip.Empowered.{AequusText.ItemKeyName(item.type, Mod)}", out string text))
                    {
                        foreach (var tt in tooltips)
                        {
                            if (tt.Name == "Tooltip0")
                            {
                                tt.Text = text;
                            }
                        }
                    }

                    float statIncrease = 1f;
                    if (BuffLoader.GetBuff(item.buffType) is EmpoweredBuffBase empoweredBuff)
                    {
                        statIncrease = empoweredBuff.StatIncrease;
                    }
                    PercentageModifier(statIncrease, "BuffEmpowerment", tooltips, statIncrease > 0f);
                }
                else if (item.prefix == ModContent.PrefixType<SplashPrefix>())
                {
                    for (int i = 0; i < tooltips.Count; i++)
                    {
                        if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "PrefixShootSpeed")
                        {
                            tooltips[i] = new TooltipLine(Mod, "PrefixSplash", AequusText.GetText("Prefixes.SplashPotion")) { IsModifier = true, IsModifierBad = false, };
                        }
                    }
                }
                else if (item.prefix == ModContent.PrefixType<BoundedPrefix>())
                {
                    tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), new TooltipLine(Aequus.Instance, "PrefixBounded", AequusText.GetText("Prefixes.BoundedPotion"))
                    { IsModifier = true, IsModifierBad = false, });
                }

                //if (DemonSiegeSystem.RegisteredSacrifices.TryGetValue(item.type, out var val) && !val.Hide && val.Progression == UpgradeProgressionType.PreHardmode)
                //{
                //    tooltips.Insert(Math.Min(tooltips.GetIndex("Tooltip#") + 1, tooltips.Count), new TooltipLine(Mod, "DemonSiegeHint", AequusText.GetText("ItemTooltip.Misc.DemonSiegeHint")));
                //}
                //TestLootBagTooltip(item, tooltips);
            }
            catch
            {
            }
        }
        public void TestLootBagTooltip(Item item, List<TooltipLine> tooltips)
        {
            var dropTable = Main.ItemDropsDB.GetRulesForItemID(item.type, includeGlobalDrops: false);

            if (dropTable.Count == 0)
            {
                return;
            }

            var drops = new List<DropRateInfo>();
            foreach (var rule in dropTable)
            {
                rule.ReportDroprates(drops, new DropRateInfoChainFeed(1f));
            }
            foreach (var drop in drops)
            {
                string text = AequusText.ItemCommand(drop.itemId);
                if (drop.stackMin == drop.stackMax)
                {
                    if (drop.stackMin > 1)
                    {
                        text += $" ({drop.stackMin})";
                    }
                }
                else
                {
                    text += $" ({drop.stackMin} - {drop.stackMax})";
                }
                text += " " + (int)(drop.dropRate * 10000f) / 100f + "%";
                tooltips.Add(new TooltipLine(Mod, Lang.GetItemNameValue(drop.itemId), text));
                if (drop.conditions != null && drop.conditions.Count > 0)
                {
                    foreach (var cond in drop.conditions)
                    {
                        if (cond == null)
                        {
                            continue;
                        }

                        string extraDesc = cond.GetConditionDescription();
                        string condText = cond.GetType().FullName;
                        if (!string.IsNullOrEmpty(extraDesc))
                            condText = $"{condText} '{extraDesc}': {cond.CanDrop(info: new DropAttemptInfo() { IsInSimulation = false, item = -1, npc = Main.npc[0], player = Main.LocalPlayer, rng = Main.rand, IsExpertMode = Main.expertMode, IsMasterMode = Main.masterMode })}";

                        tooltips.Add(new TooltipLine(Mod, Lang.GetItemNameValue(drop.itemId) + " Condition " + cond.GetType().FullName, condText));
                    }
                }
            }
        }
        public TooltipLine GetPriceTooltipLine(Player player, Item item)
        {
            player.GetItemExpectedPrice(item, out var calcForSelling, out var calcForBuying);
            int value = item.isAShopItem || item.buyOnce ? calcForBuying : calcForSelling;
            if (item.shopSpecialCurrency != -1)
            {
                string[] text = new string[1];
                int line = 0;
                CustomCurrencyManager.GetPriceText(item.shopSpecialCurrency, text, ref line, value);
                return new TooltipLine(Mod, "SpecialPrice", text[0]) { OverrideColor = Color.White, };
            }
            else if (value > 0)
            {
                string text = "";
                int platinum = 0;
                int gold = 0;
                int silver = 0;
                int copper = 0;
                int itemValue = value * item.stack;
                if (!item.buy)
                {
                    itemValue = value / 5;
                    if (itemValue < 1)
                    {
                        itemValue = 1;
                    }
                    int num3 = itemValue;
                    itemValue *= item.stack;
                    int amount = Main.shopSellbackHelper.GetAmount(item);
                    if (amount > 0)
                    {
                        itemValue += (-num3 + calcForBuying) * Math.Min(amount, item.stack);
                    }
                }
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
                    text = text + gold + " " + Lang.inter[16].Value + " ";
                }
                if (silver > 0)
                {
                    text = text + silver + " " + Lang.inter[17].Value + " ";
                }
                if (copper > 0)
                {
                    text = text + copper + " " + Lang.inter[18].Value + " ";
                }

                var t = new TooltipLine(Mod, "Price", Lang.tip[item.buy ? 50 : 49].Value + " " + text);

                if (platinum > 0)
                {
                    t.OverrideColor = Colors.CoinPlatinum;
                }
                else if (gold > 0)
                {
                    t.OverrideColor = Colors.CoinGold;
                }
                else if (silver > 0)
                {
                    t.OverrideColor = Colors.CoinSilver;
                }
                else if (copper > 0)
                {
                    t.OverrideColor = Colors.CoinCopper;
                }
                return t;
            }
            else if (item.type != ItemID.DefenderMedal)
            {
                return new TooltipLine(Mod, "Price", Lang.tip[51].Value) { OverrideColor = new Color(120, 120, 120, 255) };
            }
            return null;
        }
        public void AddPriceTooltip(Player player, Item item, List<TooltipLine> tooltips)
        {
            player.GetItemExpectedPrice(item, out var calcForSelling, out var calcForBuying);
            int value = item.isAShopItem || item.buyOnce ? calcForBuying : calcForSelling;
            if (item.shopSpecialCurrency != -1)
            {
                string[] text = new string[1];
                int line = 0;
                CustomCurrencyManager.GetPriceText(item.shopSpecialCurrency, text, ref line, value);
                tooltips.Add(new TooltipLine(Mod, "SpecialPrice", text[0]) { OverrideColor = Color.White, });
            }
            else if (value > 0)
            {
                string text = "";
                int platinum = 0;
                int gold = 0;
                int silver = 0;
                int copper = 0;
                int itemValue = value * item.stack;
                if (!item.buy)
                {
                    itemValue = value / 5;
                    if (itemValue < 1)
                    {
                        itemValue = 1;
                    }
                    int num3 = itemValue;
                    itemValue *= item.stack;
                    int amount = Main.shopSellbackHelper.GetAmount(item);
                    if (amount > 0)
                    {
                        itemValue += (-num3 + calcForBuying) * Math.Min(amount, item.stack);
                    }
                }
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
                    text = text + gold + " " + Lang.inter[16].Value + " ";
                }
                if (silver > 0)
                {
                    text = text + silver + " " + Lang.inter[17].Value + " ";
                }
                if (copper > 0)
                {
                    text = text + copper + " " + Lang.inter[18].Value + " ";
                }

                var t = new TooltipLine(Mod, "Price", Lang.tip[item.buy ? 50 : 49].Value + " " + text);

                if (platinum > 0)
                {
                    t.OverrideColor = Colors.CoinPlatinum;
                }
                else if (gold > 0)
                {
                    t.OverrideColor = Colors.CoinGold;
                }
                else if (silver > 0)
                {
                    t.OverrideColor = Colors.CoinSilver;
                }
                else if (copper > 0)
                {
                    t.OverrideColor = Colors.CoinCopper;
                }
                tooltips.Add(t);
            }
            else if (item.type != ItemID.DefenderMedal)
            {
                tooltips.Add(new TooltipLine(Mod, "Price", Lang.tip[51].Value) { OverrideColor = new Color(120, 120, 120, 255) });
            }
        }
        public bool ModifyPriceTooltip(Item item, List<TooltipLine> lines, string key)
        {
            var t = lines.Find("Price");
            if (t != null)
            {
                t.Text = t.Text.Replace(Lang.inter[15].Value, AequusText.GetText(key + ".ShopPrice.Platinum"));
                t.Text = t.Text.Replace(Lang.inter[16].Value, AequusText.GetText(key + ".ShopPrice.Gold"));
                t.Text = t.Text.Replace(Lang.inter[17].Value, AequusText.GetText(key + ".ShopPrice.Silver"));
                t.Text = t.Text.Replace(Lang.inter[18].Value, AequusText.GetText(key + ".ShopPrice.Copper"));
            }
            return false;
        }
        public void FitTooltipBackground(List<TooltipLine> lines, int width, int height, int index = -1, string firstBoxName = "Fake")
        {
            var font = FontAssets.MouseText.Value;
            var measurement = font.MeasureString(AequusHelpers.AirCharacter.ToString());
            string t = "";
            var stringSize = Vector2.Zero;
            for (int i = 0; i < width; i++)
            {
                t += AequusHelpers.AirCharacter;
                stringSize = ChatManager.GetStringSize(font, t, Vector2.One);
                if (stringSize.X > width)
                {
                    break;
                }
            }

            if (index == -1)
            {
                index = lines.Count - 1;
            }

            int linesY = Math.Max((int)(height / stringSize.Y), 1);
            for (int i = 0; i < linesY; i++)
            {
                lines.Insert(index, new TooltipLine(Mod, "Fake_" + i, t));
            }
            lines.Insert(index, new TooltipLine(Mod, firstBoxName, t));
        }

        internal static void PercentageModifier(float value, string key, List<TooltipLine> tooltips, bool good)
        {
            tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), new TooltipLine(Aequus.Instance, key, AequusText.GetText("Prefixes." + key, (value > 0f ? "+" : "") + (int)(value * 100f) + "%"))
            { IsModifier = true, IsModifierBad = !good, });
        }
        internal static void PercentageModifier(int num, int originalNum, string key, List<TooltipLine> tooltips, bool higherIsGood = false)
        {
            if (num == originalNum)
            {
                return;
            }

            float value = num / (float)originalNum;
            if (value < 1f)
            {
                value = 1f - value;
            }
            else
            {
                value--;
            }
            tooltips.Insert(tooltips.GetIndex("PrefixAccMeleeSpeed"), new TooltipLine(Aequus.Instance, key, AequusText.GetText("Prefixes." + key, (num > originalNum ? "+" : "-") + (int)(value * 100f) + "%"))
            { IsModifier = true, IsModifierBad = num < originalNum ? higherIsGood : !higherIsGood, });
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Aequus")
            {
                if (line.Name.StartsWith("Fake"))
                {
                    return false;
                }
                if (line.Name == "DedicatedItem")
                {
                    DrawDedicatedTooltip(line);
                    return false;
                }
            }
            else if (line.Mod == "Terraria")
            {
                if (line.Name == "ItemName" && item.rare >= ItemRarityID.Count && RarityLoader.GetRarity(item.rare) is IDrawRarity drawRare)
                {
                    drawRare.DrawDevTooltip(line);
                    return false;
                }
            }
            return true;
        }

        public static void DrawDedicatedTooltip(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            float brightness = Main.mouseTextColor / 255f;
            float brightnessProgress = (Main.mouseTextColor - 190f) / (byte.MaxValue - 190f);
            color = Colors.AlphaDarken(color);
            color.A = 0;
            var font = FontAssets.MouseText.Value;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(x, y), new Color(0, 0, 0, 255), rotation, origin, baseScale);
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f)
            {
                var coords = new Vector2(x, y);
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, coords, new Color(0, 0, 0, 255), rotation, origin, baseScale);
            }
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver2 + 0.01f)
            {
                var coords = new Vector2(x, y) + f.ToRotationVector2() * (brightness / 2f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords, color * 0.8f, rotation, origin, baseScale);
            }
            for (float f = 0f; f < MathHelper.TwoPi; f += MathHelper.PiOver4 + 0.01f)
            {
                var coords = new Vector2(x, y) + (f + Main.GlobalTimeWrappedHourly).ToRotationVector2() * (brightnessProgress * 3f);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, coords, color * 0.2f, rotation, origin, baseScale);
            }
        }
        public static void DrawDedicatedTooltip(string text, int x, int y, Color color)
        {
            DrawDedicatedTooltip(text, x, y, 0f, Vector2.Zero, Vector2.One, color);
        }
        public static void DrawDedicatedTooltip(DrawableTooltipLine line)
        {
            DrawDedicatedTooltip(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color));
        }

        public static void DrawDevTooltip(DrawableTooltipLine line)
        {
            DrawDevTooltip(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color));
        }
        public static void DrawDevTooltip(string text, int x, int y, Color color)
        {
            DrawDevTooltip(text, x, y, 0f, Vector2.Zero, Vector2.One, color);
        }
        public static void DrawDevTooltip(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
        }
    }
}