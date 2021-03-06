using Aequus.Buffs;
using Aequus.Graphics;
using Aequus.NPCs;
using Aequus.NPCs.Friendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Items
{
    public static class AequusTooltips
    {
        public struct ItemDedication
        {
            public readonly Color color;

            public ItemDedication(Color color)
            {
                this.color = color;
            }
        }

        public class TooltipsGlobal : GlobalItem
        {
            public override void Load()
            {
                Dedicated = new Dictionary<int, ItemDedication>();
                //[ModContent.ItemType<MothmanMask>()] = new ItemDedication(new Color(50, 75, 250, 255)),
                //[ModContent.ItemType<RustyKnife>()] = new ItemDedication(new Color(30, 255, 60, 255)),
                //[ModContent.ItemType<Thunderbird>()] = new ItemDedication(new Color(200, 125, 255, 255)),
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

                    if (Dedicated.TryGetValue(item.type, out var dedication))
                    {
                        tooltips.Insert(GetIndex(tooltips, "Master"), new TooltipLine(Mod, "DedicatedItem", AequusText.GetText("Tooltips.DedicatedItem")) { OverrideColor = dedication.color });
                    }

                    if (Main.npcShop > 0)
                    {
                        if (player.talkNPC != -1 && item.isAShopItem && item.buy && item.tooltipContext == ItemSlot.Context.ShopItem && Main.npc[player.talkNPC].type == ModContent.NPCType<Exporter>())
                            ModifyPriceTooltip(item, tooltips, "Chat.Exporter");
                    }
                    else if (aequus.showPrices)
                    {
                        if ((item.value >= 0 && (item.type < ItemID.CopperCoin || item.type > ItemID.PlatinumCoin)) || tooltips.Find((t) => t.Name == "Price") != null || tooltips.Find((t) => t.Name == "SpecialPrice") != null)
                        {
                            AddPriceTooltip(player, item, tooltips);
                        }
                    }

                    if (item.ModItem is ItemHooks.IUpdateBank || (AequusItem.BankEquipFuncs.Contains(item.type) && item.type != ItemID.CellPhone))
                    {
                        tooltips.Insert(GetIndex(tooltips, "Tooltip#") + 1, new TooltipLine(Mod, "BankFunctions", AequusText.GetText("Tooltips.InventoryPiggyBankFunction")));
                    }

                    if (aequus.moroSummonerFruit && AequusItem.SummonStaff.Contains(item.type))
                    {
                        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "UseMana");
                    }

                    if (AequusItem.LegendaryFish.Contains(item.type))
                    {
                        if (NPC.AnyNPCs(NPCID.Angler))
                            tooltips.Insert(tooltips.GetIndex("Quest"), new TooltipLine(Mod, "AnglerHint", AequusText.GetText("AnglerHint")) { OverrideColor = HintColor, });
                        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "Quest");
                    }

                    if (item.buffType > 0 && BuffID.Sets.IsWellFed[item.buffType] && AequusBuff.IsWellFedButDoesntIncreaseLifeRegen.Contains(item.buffType))
                    {
                        tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "WellFedExpert");
                    }
                }
                catch
                {
                }
            }
            public void AddPriceTooltip(Player player, Item item, List<TooltipLine> tooltips)
            {
                player.GetItemExpectedPrice(item, out var calcForSelling, out var calcForBuying);
                int value = (item.isAShopItem || item.buyOnce) ? calcForBuying : calcForSelling;
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

            public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
            {
                if (line.Mod == "Aequus")
                {
                    if (line.Name == "Fake")
                    {
                        return false;
                    }
                    if (line.Name == "DedicatedItem")
                    {
                        DrawDedicatedTooltip(line);
                        return false;
                    }
                    if (line.Name == "ShopQuote")
                    {
                        ShopQuotes.DrawShopQuote(line, Main.npc[Main.LocalPlayer.talkNPC]);
                        return false;
                    }
                }
                return true;
            }
        }

        public static Dictionary<int, ItemDedication> Dedicated { get; private set; }

        internal static readonly string[] TooltipNames = new string[]
        {
                "ItemName",
                "Favorite",
                "FavoriteDesc",
                "Social",
                "SocialDesc",
                "Damage",
                "CritChance",
                "Speed",
                "Knockback",
                "FishingPower",
                "NeedsBait",
                "BaitPower",
                "Equipable",
                "WandConsumes",
                "Quest",
                "Vanity",
                "Defense",
                "PickPower",
                "AxePower",
                "HammerPower",
                "TileBoost",
                "HealLife",
                "HealMana",
                "UseMana",
                "Placeable",
                "Ammo",
                "Consumable",
                "Material",
                "Tooltip#",
                "EtherianManaWarning",
                "WellFedExpert",
                "BuffTime",
                "OneDropLogo",
                "PrefixDamage",
                "PrefixSpeed",
                "PrefixCritChance",
                "PrefixUseMana",
                "PrefixSize",
                "PrefixShootSpeed",
                "PrefixKnockback",
                "PrefixAccDefense",
                "PrefixAccMaxMana",
                "PrefixAccCritChance",
                "PrefixAccDamage",
                "PrefixAccMoveSpeed",
                "PrefixAccMeleeSpeed",
                "SetBonus",
                "Expert",
                "Master",
                "JourneyResearch",
                "BestiaryNotes",
                "SpecialPrice",
                "Price",
        };

        public static Color HintColor => new Color(225, 100, 255, 255);
        public static Color DemonSiegeTooltip => new Color(255, 170, 150, 255);
        public static Color ItemDrawbackTooltip => Color.Lerp(Color.Red, Color.White, 0.5f);

        internal static void PercentageModifier(int num, int originalNum, string key, List<TooltipLine> tooltips, bool lowerIsGood = false)
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
            { IsModifier = true, IsModifierBad = num < originalNum ? lowerIsGood : !lowerIsGood, });
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

        public static int GetIndex(this List<TooltipLine> tooltips, string lineName)
        {
            int myIndex = FindLineIndex(lineName);
            int i = 0;
            for (; i < tooltips.Count; i++)
            {
                if (tooltips[i].Mod == "Terraria" && FindLineIndex(tooltips[i].Name) >= myIndex)
                {
                    if (lineName == "Tooltip#")
                    {
                        int finalIndex = i;
                        while (i < tooltips.Count)
                        {
                            if (tooltips[i].Name.StartsWith("Tooltip"))
                            {
                                finalIndex = i;
                            }
                            i++;
                        }
                        return finalIndex;
                    }
                    return i;
                }
            }
            return i;
        }

        private static int FindLineIndex(string name)
        {
            if (name.StartsWith("Tooltip"))
            {
                name = "Tooltip#";
            }
            for (int i = 0; i < TooltipNames.Length; i++)
            {
                if (name == TooltipNames[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public static void ChangeVanillaLine(List<TooltipLine> tooltips, string name, Action<TooltipLine> modify)
        {
            foreach (var t in tooltips)
            {
                if (t.Name == name)
                {
                    modify(t);
                    return;
                }
            }
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
            if (string.IsNullOrWhiteSpace(text)) // since you can rename items.
            {
                return;
            }
            var font = FontAssets.MouseText.Value;
            var size = font.MeasureString(text);
            var center = size / 2f;
            var transparentColor = color * 0.4f;
            transparentColor.A = 0;
            var texture = ModContent.Request<Texture2D>(Aequus.AssetsPath + "UI/NarrizuulBloom").Value;
            var spotlightOrigin = texture.Size() / 2f;
            float spotlightRotation = rotation + MathHelper.PiOver2;
            var spotlightScale = new Vector2(1.2f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4f) * 0.145f, center.Y * 0.15f);

            // black BG
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, Color.Black * 0.3f, rotation,
            spotlightOrigin, new Vector2(size.X / texture.Width * 2f, center.Y / texture.Height * 2.5f), SpriteEffects.None, 0f);
            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, new Vector2(x, y), Color.Black,
                rotation, origin, baseScale);

            if (Aequus.HQ)
            {
                var rand = AequusEffects.EffectRand;
                int reset = rand.SetRand(Main.LocalPlayer.name.GetHashCode());

                // particles
                var particleTexture = TextureCache.Bloom[0].Value;
                var particleOrigin = particleTexture.Size() / 2f;
                int amt = (int)rand.Rand(size.X / 3, size.X);
                for (int i = 0; i < amt; i++)
                {
                    float lifeTime = (rand.Rand(20f) + Main.GlobalTimeWrappedHourly * 2f) % 20f;
                    int baseParticleX = (int)rand.Rand(4f, size.X - 4f);
                    int particleX = baseParticleX + (int)AequusHelpers.Wave(lifeTime + Main.GlobalTimeWrappedHourly * rand.Rand(2f, 5f), -rand.Rand(3f, 10f), rand.Rand(3f, 10f));
                    int particleY = (int)rand.Rand(10f);
                    float scale = rand.Rand(0.2f, 0.4f);
                    if (baseParticleX > 14 && baseParticleX < size.X - 14 && rand.RandChance(6))
                    {
                        scale *= 2f;
                    }
                    scale /= 4.5f;
                    var clr = color;
                    if (lifeTime < 0.3f)
                    {
                        clr *= lifeTime / 0.3f;
                    }
                    if (lifeTime < 5f)
                    {
                        if (lifeTime > MathHelper.PiOver2)
                        {
                            float timeMult = (lifeTime - MathHelper.PiOver2) / MathHelper.PiOver2;
                            scale -= timeMult * 0.4f;
                            if (scale < 0f)
                            {
                                continue;
                            }
                            int colorMinusAmount = (int)(timeMult * 255f);
                            clr.R = (byte)Math.Max(clr.R - colorMinusAmount, 0);
                            clr.G = (byte)Math.Max(clr.G - colorMinusAmount, 0);
                            clr.B = (byte)Math.Max(clr.B - colorMinusAmount, 0);
                            clr.A = (byte)Math.Max(clr.A - colorMinusAmount, 0);
                            if (clr.R == 0 && clr.G == 0 && clr.B == 0 && clr.A == 0)
                            {
                                continue;
                            }
                        }
                        if (scale > 0.4f)
                        {
                            Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f + 10), null, clr * 2f, 0f, particleOrigin, scale * 0.5f, SpriteEffects.None, 0f);
                        }
                        Main.spriteBatch.Draw(particleTexture, new Vector2(x + particleX, y + particleY - lifeTime * 15f + 10), null, clr, 0f, particleOrigin, scale, SpriteEffects.None, 0f);
                    }
                }

                rand.SetRand(reset);
            }

            // light effect
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.3f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.3f, spotlightRotation,
               spotlightOrigin, spotlightScale * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, transparentColor * 0.35f, spotlightRotation,
               spotlightOrigin, spotlightScale * 2f, SpriteEffects.None, 0f);

            // colored text
            ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x, y), color,
                rotation, origin, baseScale);

            // glowy effect on text
            float wave = AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, 0f, 1f);
            for (int i = 1; i <= 2; i++)
            {
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x + wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x - wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
            }
        }

        public static void UsesLife(this List<TooltipLine> tooltips, ModItem item, int amt)
        {
            tooltips.Insert(GetIndex(tooltips, "UseMana"),
                new TooltipLine(item.Mod, "UsesLife", AequusText.GetText("Tooltips.UsesLife", amt)));
        }

        public static TooltipLine Find(this List<TooltipLine> tooltips, string name)
        {
            return tooltips.Find((t) => t.Mod == "Terraria" && t.Name.Equals(name));
        }

        public static TooltipLine ItemName(this List<TooltipLine> tooltips)
        {
            return tooltips.Find("ItemName");
        }

        public static void PreTooltip(this List<TooltipLine> tooltips, ModItem item, string name, string key)
        {
            tooltips.Insert(GetIndex(tooltips, "Material"),
                    new TooltipLine(item.Mod, name, AequusText.GetText(key)));
        }

        public static void PreTooltip(this List<TooltipLine> tooltips, ModItem item, string name, string key, params object[] args)
        {
            tooltips.Insert(GetIndex(tooltips, "Material"),
                    new TooltipLine(item.Mod, name, AequusText.GetText(key, args)));
        }

        public static void RemoveCritChance(this List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "CritChance");
        }

        public static void RemoveCritChanceModifier(this List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll((t) => t.Mod == "Terraria" && t.Name == "PrefixCritChance");
        }
    }
}