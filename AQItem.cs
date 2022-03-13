using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Common.Utilities.Colors;
using AQMod.Common.WorldGeneration;
using AQMod.Content.Players;
using AQMod.Content.World.Events;
using AQMod.Dusts;
using AQMod.Effects;
using AQMod.Items;
using AQMod.Items.Accessories;
using AQMod.Items.Misc;
using AQMod.Items.Potions;
using AQMod.Items.Reforges;
using AQMod.Items.Tools.Fishing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace AQMod
{
    public class AQItem : GlobalItem
    {
        public static class Hooks
        {
            internal static void ApplyHooks()
            {
                On.Terraria.Item.SetDefaults += PostSetDefaults;
            }

            private static void PostSetDefaults(On.Terraria.Item.orig_SetDefaults orig, Item self, int Type, bool noMatCheck)
            {
                orig(self, Type, noMatCheck);
                try
                {
                    self.GetGlobalItem<AQItem>().PostSetDefaults(self, Type, noMatCheck);
                }
                catch
                {
                }
            }
        }

        public sealed class Sets
        {
            public static HashSet<int> Crate { get; private set; }
            public static HashSet<int> NoRename { get; private set; }
            public static HashSet<int> NoAutoswing { get; private set; }
            public static HashSet<int> DashEquip { get; private set; }
            public static HashSet<int> ItemIDRenewalBlacklist { get; private set; }
            public static HashSet<int> AmmoIDRenewalBlacklist { get; private set; }

            internal static void Load()
            {
                ItemIDRenewalBlacklist = new HashSet<int>()
                {
                    ItemID.RocketII,
                    ItemID.RocketIV,
                    ItemID.Ale,
                    ItemID.Bone,
                    ItemID.Seed,
                    ItemID.Wire,
                };
                AmmoIDRenewalBlacklist = new HashSet<int>()
                {
                    AmmoID.FallenStar,
                    AmmoID.Gel,
                    AmmoID.Sand,
                    AmmoID.Snowball,
                    AmmoID.Flare,
                    AmmoID.Solution,
                };

                Crate = new HashSet<int>()
                {
                    ItemID.WoodenCrate,
                    ItemID.IronCrate,
                    ItemID.GoldenCrate,
                    ItemID.FloatingIslandFishingCrate,
                    ItemID.CorruptFishingCrate,
                    ItemID.CrimsonFishingCrate,
                    ItemID.HallowedFishingCrate,
                    ItemID.JungleFishingCrate,
                    ItemID.DungeonFishingCrate,
                };


                NoAutoswing = new HashSet<int>()
                {
                };

                NoRename = new HashSet<int>()
                {
                    ModContent.ItemType<GiftItem>(),
                };

                DashEquip = new HashSet<int>()
                {
                    ItemID.EoCShield,
                    ItemID.Tabi,
                    ItemID.MasterNinjaGear,
                };
            }

            internal static void Unload()
            {
                Crate?.Clear();
                Crate = null;
                NoAutoswing?.Clear();
                NoAutoswing = null;
                NoRename?.Clear();
                NoRename = null;
                DashEquip?.Clear();
                DashEquip = null;
            }
        }

        public static class Prices
        {
            public static int PotionValue => Item.sellPrice(silver: 2);
            public static int EnergySellValue => Item.sellPrice(silver: 10);
            public static int CrabCreviceValue => Item.sellPrice(silver: 25);
            public static int CorruptionWeaponValue => Item.sellPrice(silver: 50);
            public static int CrimsonWeaponValue => Item.sellPrice(silver: 55);
            public static int GlimmerWeaponValue => Item.sellPrice(silver: 75);
            public static int DemonSiegeWeaponValue => Item.sellPrice(silver: 80);
            public static int MemorialistItemBuyValue => Item.buyPrice(gold: 20);
            public static int OmegaStariteWeaponValue => Item.sellPrice(gold: 4, silver: 50);
            public static int GaleStreamsWeaponValue => Item.sellPrice(gold: 4);
            public static int PostMechsEnergyWeaponValue => Item.sellPrice(gold: 6, silver: 50);
            public static int PillarWeaponValue => Item.sellPrice(gold: 10);
        }

        public const int RarityCrabCrevice = ItemRarityID.Blue;
        public const int RarityGlimmer = ItemRarityID.Green;
        public const int RarityDemonSiege = ItemRarityID.Orange;
        public const int RarityOmegaStarite = ItemRarityID.LightRed;
        public const int RarityGaleStreams = ItemRarityID.LightRed;
        public const int RarityDedicatedItem = ItemRarityID.Cyan;

        internal const int RarityBanner = ItemRarityID.Blue;
        internal const int RarityDemoniteCrimtane = ItemRarityID.Blue;
        internal const int RarityDungeon = ItemRarityID.Green;
        internal const int RarityQueenBee = ItemRarityID.Green;
        internal const int RarityJungle = ItemRarityID.Orange;
        internal const int RarityMolten = ItemRarityID.Orange;
        internal const int RarityPet = ItemRarityID.Orange;
        internal const int RarityWallofFlesh = ItemRarityID.LightRed;
        internal const int RarityPreMechs = ItemRarityID.LightRed;
        internal const int RarityCobaltMythrilAdamantite = ItemRarityID.LightRed;
        internal const int RarityMechs = ItemRarityID.Pink;
        internal const int RarityPlantera = ItemRarityID.Lime;
        internal const int RarityHardmodeDungeon = ItemRarityID.Yellow;
        internal const int RarityMartians = ItemRarityID.Yellow;
        internal const int RarityDukeFishron = ItemRarityID.Yellow;
        internal const int RarityLunaticCultist = ItemRarityID.Cyan;
        internal const int RarityPillars = ItemRarityID.Red;
        internal const int RarityMoonLord = ItemRarityID.Red;

        private static readonly string[] TooltipNames = new string[]
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
            "SpecialPrice",
            "Price",
        };

        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;

        internal GlowmaskData Glowmask { get; private set; }
        public float cooldownMultiplier;
        public float comboMultiplier;
        public int basePrice;

        public AQItem()
        {
            Glowmask = default(GlowmaskData);
            cooldownMultiplier = 1f;
            comboMultiplier = 1f;
        }

        private void GlowmaskDataCheck(Item item)
        {
            if (!AQMod.Loading && Main.netMode != NetmodeID.Server &&
                GlowmaskData.ItemToGlowmask != null && GlowmaskData.ItemToGlowmask.TryGetValue(item.type, out GlowmaskData glowmask))
            {
                this.Glowmask = glowmask;
            }
        }

        public override void SetDefaults(Item item)
        {
        }

        public void PostSetDefaults(Item item, int type, bool noMaterialCheck)
        {
            basePrice = item.value;
            if (!noMaterialCheck)
            {
                GlowmaskDataCheck(item);
            }
        }

        private void Tooltips_DedicatedItemCheck(Item item, List<TooltipLine> tooltips)
        {
            if (item.modItem is IDedicatedItem dedicatedItem)
            {
                tooltips.Add(new TooltipLine(mod, "DedicatedItem", Language.GetTextValue("Mods.AQMod.Tooltips.DedicatedItem")) { overrideColor = dedicatedItem.DedicatedColoring });
            }
        }
        private void Tooltips_ManaPerSecondCheck(Item item, List<TooltipLine> tooltips)
        {
            if (item.modItem is IManaPerSecond m)
            {
                int? value = m.ManaPerSecond;
                int manaCost;
                if (value != null)
                {
                    manaCost = value.Value;
                }
                else
                {
                    manaCost = Main.player[Main.myPlayer].GetManaCost(item);
                    int usesPerSecond = 60 / PlayerHooks.TotalUseTime(item.useTime, Main.player[Main.myPlayer], item);
                    if (usesPerSecond != 0)
                    {
                        manaCost *= usesPerSecond;
                    }
                }
                foreach (var t in tooltips)
                {
                    if (t.mod == "Terraria" && t.Name == "UseMana")
                    {
                        t.text = string.Format(Language.GetTextValue("Mods.AQMod.Tooltips.ManaPerSecond"), manaCost);
                    }
                }
            }
        }
        private void Tooltips_PickBreakCheck(Item item, AQPlayer aQPlayer, List<TooltipLine> tooltips)
        {
            if (item.pick > 0 && aQPlayer.pickBreak)
            {
                foreach (var t in tooltips)
                {
                    if (t.mod == "Terraria" && t.Name == "PickPower")
                    {
                        t.text = item.pick / 2 + Language.GetTextValue("LegacyTooltip.26");
                        t.overrideColor = new Color(128, 128, 128, 255);
                    }
                }
            }
        }
        private void Tooltips_FidgetSpinnerCheck(Item item, AQPlayer aQPlayer, List<TooltipLine> tooltips)
        {
            if (aQPlayer.fidgetSpinner && !item.channel && CheckAutoswingable(Main.LocalPlayer, item, ignoreChanneled: true))
            {
                foreach (var t in tooltips)
                {
                    if (t.mod == "Terraria" && t.Name == "Speed")
                    {
                        AQPlayer.forceAutoswing = true;
                        string text = UseAnimTooltip(PlayerHooks.TotalUseTime(item.useTime, Main.LocalPlayer, item));
                        AQPlayer.forceAutoswing = false;
                        if (t.text != text)
                        {
                            t.text = text +
                            " (" + Lang.GetItemName(ModContent.ItemType<FidgetSpinner>()).Value + ")";
                            t.overrideColor = new Color(200, 200, 200, 255);
                        }
                    }
                }
            }
        }
        private void Tooltips_DemonSiegeUpgradeCheck(Item item, List<TooltipLine> tooltips)
        {
            if (DemonSiege.GetUpgrade(item) != null)
            {
                int index = FindTTLineSpot(tooltips, "Material");
                tooltips.Insert(index, new TooltipLine(mod, "DemonSiegeUpgrade", Language.GetTextValue("Mods.AQMod.Tooltips.DemonSiegeUpgrade")) { overrideColor = AQMod.DemonSiegeTooltip, });
            }
        }
        private void Tooltips_HookBarbBlacklistCheck(Item item, List<TooltipLine> tooltips)
        {
            if (item.shoot > ProjectileID.None && AQProjectile.Sets.HookBarbBlacklist.Contains(item.shoot))
            {
                int index = FindTTLineSpot(tooltips, "Material");
                tooltips.Insert(index, new TooltipLine(mod, "HookBarbBlacklist", Language.GetTextValue("Mods.AQMod.Tooltips.HookBarbBlacklist")) { overrideColor = new Color(255, 255, 255), });
            }
        }
        private void Tooltips_HermitStorageCheck(List<TooltipLine> tooltips)
        {
            if (!PlayerStorage.hoverStorage.IsAir)
            {
                var span = PlayerStorage.hoverStorage.TimeSinceStored;
                int seconds = (int)span.TotalSeconds;
                int minutes = seconds / 60;
                int hours = minutes / 60;
                int days = hours / 24;
                int months = days / 30;
                string text;
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                {
                    text = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift)
                        ? Language.GetTextValue("Mods.AQMod.HermitCrab.Second" + (seconds == 1 ? "" : "s") + "Ago", seconds)
                        : Language.GetTextValue("Mods.AQMod.HermitCrab.StoredAt", PlayerStorage.hoverStorage.WhenStored);
                }
                else if (months > 0)
                {
                    text = Language.GetTextValue("Mods.AQMod.HermitCrab.Month" + (months == 1 ? "" : "s") + "Ago", months);
                }
                else if (days > 0)
                {
                    text = Language.GetTextValue("Mods.AQMod.HermitCrab.Day" + (days == 1 ? "" : "s") + "Ago", days);
                }
                else if (hours > 0)
                {
                    text = Language.GetTextValue("Mods.AQMod.HermitCrab.Hour" + (hours == 1 ? "" : "s") + "Ago", hours);
                }
                else if (minutes > 0)
                {
                    text = Language.GetTextValue("Mods.AQMod.HermitCrab.Minute" + (minutes == 1 ? "" : "s") + "Ago", minutes);
                }
                else
                {
                    text = Language.GetTextValue("Mods.AQMod.HermitCrab.Second" + (seconds == 1 ? "" : "s") + "Ago", seconds);
                }
                tooltips.Add(new TooltipLine(mod, "StorageDate", text));
                PlayerStorage.hoverStorage = new PlayerStorage.HermitCrabStorage();
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.social)
            {
                return;
            }
            try
            {
                var aQPlayer = Main.LocalPlayer.GetModPlayer<AQPlayer>();
                if (AQConfigClient.Instance.DemonSiegeUpgradeTooltip)
                {
                    Tooltips_DemonSiegeUpgradeCheck(item, tooltips);
                }
                if (AQConfigClient.Instance.HookBarbBlacklistTooltip)
                {
                    Tooltips_HookBarbBlacklistCheck(item, tooltips);
                }
                if (item.type > Main.maxItemTypes)
                {
                    Tooltips_ManaPerSecondCheck(item, tooltips);
                    Tooltips_DedicatedItemCheck(item, tooltips);
                }
                Tooltips_PickBreakCheck(item, aQPlayer, tooltips);
                Tooltips_FidgetSpinnerCheck(item, aQPlayer, tooltips);
                Tooltips_HermitStorageCheck(tooltips);
                if (item.prefix >= PrefixID.Count && ModPrefix.GetPrefix(item.prefix) is AQPrefix aQPrefix)
                {
                    aQPrefix.ModifyTooltips(item, this, aQPlayer, tooltips);
                }
            }
            catch
            {
            }
        }

        public static void DrawString_Dedicated(DrawableTooltipLine line)
        {
            DrawString_Dedicated(line.text, line.X, line.Y, line.rotation, line.origin, line.baseScale, line.overrideColor.GetValueOrDefault(line.color));
        }
        public static void DrawString_Dedicated(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            color = Colors.AlphaDarken(color);
            color.A = 0;
            float xOff = (float)Math.Sin(Main.GlobalTime * 15f) + 1f;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), new Color(0, 0, 0, 255), rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color * 0.8f, rotation, origin, baseScale);
            ChatManager.DrawColorCodedString(Main.spriteBatch, Main.fontMouseText, text, new Vector2(x, y), color * 0.8f, rotation, origin, baseScale);
        }
        public static void DrawString_Developer(DrawableTooltipLine line)
        {
            DrawString_Developer(line.text, line.X, line.Y, line.rotation, line.origin, line.baseScale, line.overrideColor.GetValueOrDefault(line.color));
        }
        public static void DrawString_Developer(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color)
        {
            if (string.IsNullOrWhiteSpace(text)) // since you can rename items.
            {
                return;
            }
            var font = Main.fontMouseText;
            var size = font.MeasureString(text);
            var center = size / 2f;
            var transparentColor = color * 0.4f;
            transparentColor.A = 0;
            var texture = LegacyTextureCache.Lights[LightTex.Spotlight12x66];
            var spotlightOrigin = texture.Size() / 2f;
            float spotlightRotation = rotation + MathHelper.PiOver2;
            var spotlightScale = new Vector2(1.2f + (float)Math.Sin(Main.GlobalTime * 4f) * 0.145f, center.Y * 0.15f);

            // black BG
            Main.spriteBatch.Draw(texture, new Vector2(x, y - 6f) + center, null, Color.Black * 0.3f, rotation,
            spotlightOrigin, new Vector2(size.X / texture.Width * 2f, center.Y / texture.Height * 2.5f), SpriteEffects.None, 0f);
            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, text, new Vector2(x, y), Color.Black,
                rotation, origin, baseScale);

            if (ModContent.GetInstance<AQConfigClient>().EffectQuality > 0.5f)
            {
                FX.TempSetRand(Main.LocalPlayer.name.GetHashCode(), out int reset);
                // particles
                var particleTexture = LegacyTextureCache.Lights[LightTex.Spotlight15x15];
                var particleOrigin = particleTexture.Size() / 2f;
                int amt = (int)FX.Rand((int)size.X / 3, (int)size.X);
                for (int i = 0; i < amt; i++)
                {
                    float lifeTime = (FX.Rand(20f) + Main.GlobalTime * 2f) % 20f;
                    int baseParticleX = (int)FX.Rand(4, (int)size.X - 4);
                    int particleX = baseParticleX + (int)AQUtils.Wave(lifeTime + Main.GlobalTime * FX.Rand(2f, 5f), -FX.Rand(3f, 10f), FX.Rand(3f, 10f));
                    int particleY = (int)FX.Rand(10);
                    float scale = FX.Rand(0.2f, 0.4f);
                    if (baseParticleX > 14 && baseParticleX < size.X - 14 && FX.RandChance(6))
                    {
                        scale *= 2f;
                    }
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

                FX.SetRand(reset);
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
            float wave = AQUtils.Wave(Main.GlobalTime * 10f, 0f, 1f);
            for (int i = 1; i <= 2; i++)
            {
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x + wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
                ChatManager.DrawColorCodedString(Main.spriteBatch, font, text, new Vector2(x - wave * 1f * i, y), transparentColor,
                    rotation, origin, baseScale);
            }
        }
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "AQMod" && line.Name == "DedicatedItem")
            {
                DrawString_Dedicated(line);
                return false;
            }
            return true;
        }

        private bool CooldownCheck(Item item, Player player)
        {
            if (item.modItem is ICooldown cool)
            {
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                ushort cooldown = cool.Cooldown(player, aQPlayer);
                if (cooldown > 0)
                {
                    return aQPlayer.ItemCooldownCheck(0, item: item);
                }
            }
            return true;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            return CooldownCheck(item, player);
        }
        public override bool UseItem(Item item, Player player)
        {
            if (item.modItem is ICooldown cool)
            {
                var aQPlayer = player.GetModPlayer<AQPlayer>();
                ushort cooldown = cool.Cooldown(player, aQPlayer);
                if (cooldown > 0)
                {
                    aQPlayer.ItemCooldownCheck(0, item: item);
                }
            }
            return false;
        }

        public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            speed *= aQPlayer.wingSpeedBoost;
            acceleration *= aQPlayer.wingSpeedBoost;
        }

        public override void VerticalWingSpeeds(Item item, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            ascentWhenFalling *= aQPlayer.wingSpeedBoost;
            ascentWhenRising *= aQPlayer.wingSpeedBoost;
            maxCanAscendMultiplier *= aQPlayer.wingSpeedBoost;
            maxAscentMultiplier *= aQPlayer.wingSpeedBoost;
            constantAscend *= aQPlayer.wingSpeedBoost;
        }

        public override void ExtractinatorUse(int extractType, ref int resultType, ref int resultStack)
        {
            if (Main.gameMenu || AQMod.Loading || Main.myPlayer == -1 || Main.LocalPlayer == null)
            {
                return;
            }

            var player = Main.LocalPlayer;
            var aQPlayer = player.GetModPlayer<AQPlayer>();

            aQPlayer.ExtractinatorCount++;
            aQPlayer.extractorAirMask++;
            aQPlayer.extractorHelmet++;

            if (aQPlayer.extractinatorCounter && AQPlayer.extractinatorBlipDelay == 0)
            {
                CombatText.NewText(new Rectangle(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16), Color.Gray, aQPlayer.ExtractinatorCount);
                AQPlayer.extractinatorBlipDelay = 8;
            }

            if (aQPlayer.extractorAirMask >= 500 + Main.rand.Next(-100, 100))
            {
                aQPlayer.extractorAirMask = 0;
                Item.NewItem(new Rectangle(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16), ModContent.ItemType<ExtractorAirMask>());
            }
            else if (aQPlayer.extractorHelmet >= 500 + Main.rand.Next(-100, 100))
            {
                aQPlayer.extractorHelmet = 0;
                Item.NewItem(new Rectangle(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16), ModContent.ItemType<ExtractorHelmet>());
            }
        }

        public override void OpenVanillaBag(string context, Player player, int arg)
        {
            switch (context)
            {
                case "bossBag":
                    {
                        switch (arg)
                        {
                            case ItemID.QueenBeeBossBag:
                                {
                                    if (Main.rand.NextBool())
                                        player.QuickSpawnItem(ModContent.ItemType<BeeRod>());
                                }
                                break;

                            case ItemID.WallOfFleshBossBag:
                                {
                                    if (Main.hardMode)
                                        player.QuickSpawnItem(ModContent.ItemType<OpposingPotion>(), Main.rand.Next(4) + 1);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (item.type < Main.maxItemTypes)
                return;
            Glowmask.DrawWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.playerInventory)
            {
                try
                {
                    if (item.modItem is ICooldown)
                    {
                        var aQPlayer = Main.LocalPlayer.GetModPlayer<AQPlayer>();
                        if (aQPlayer.itemCooldown > 0 && aQPlayer.itemCooldownMax > 0)
                        {
                            float progress = aQPlayer.itemCooldown / (float)aQPlayer.itemCooldownMax;
                            var texture = mod.GetTexture("Items/InventoryBack");
                            int frameY = (int)(texture.Height * progress);
                            var uiFrame = new Rectangle(0, texture.Height - frameY, texture.Width, frameY);
                            position.Y += uiFrame.Y * Main.inventoryScale;
                            var center = position + frame.Size() / 2f * scale;

                            spriteBatch.Draw(texture, center, uiFrame, new Color(255, 255, 225, 250) * (0.75f + progress * 0.25f), 0f, texture.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
                        }
                    }
                }
                catch
                {

                }
            }
            return true;
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (item.type < Main.maxItemTypes)
                return;
            Glowmask.DrawInv(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            grabRange = (int)(grabRange * player.GetModPlayer<AQPlayer>().grabReach);
        }

        public static bool ItemOnGroundAlready(int type)
        {
            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i].active && Main.item[i].type == type)
                    return true;
            }
            return false;
        }

        public static void DropMoney(int value, Rectangle rect)
        {
            if (value >= Item.platinum)
            {
                int platinum = value / Item.platinum;
                Item.NewItem(rect, ItemID.GoldCoin, platinum);
                value -= Item.platinum * platinum;
                if (value <= 0)
                    return;
            }
            if (value >= Item.gold)
            {
                int gold = value / Item.gold;
                Item.NewItem(rect, ItemID.GoldCoin, gold);
                value -= Item.gold * gold;
                if (value <= 0)
                    return;
            }
            if (value >= Item.silver)
            {
                int silver = value / Item.silver;
                Item.NewItem(rect, ItemID.SilverCoin, silver);
                value -= Item.silver * silver;
            }
            if (value > 0)
                Item.NewItem(rect, ItemID.CopperCoin, value);
        }

        public static void DropInstancedItem(int player, Rectangle rect, int item, int stack = 1)
        {
            DropInstancedItem(player, rect.X, rect.Y, rect.Width, rect.Height, item, stack);
        }

        public static void DropInstancedItem(int player, Vector2 Position, int width, int height, int item, int stack = 1)
        {
            DropInstancedItem(player, (int)Position.X, (int)Position.Y, width, height, item, stack);
        }

        public static void DropInstancedItem(int player, int x, int y, int width, int height, int item, int stack = 1)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                int i = Item.NewItem(x, y, width, height, item, stack, noBroadcast: true);
                Main.itemLockoutTime[i] = 54000;
                NetMessage.SendData(MessageID.InstancedItem, player, -1, null, i);
                Main.item[i].active = false;
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Item.NewItem(x, y, width, height, item, stack);
            }
        }

        public static int GetGrabRange(Item item, Player player)
        {
            int grabRange = Player.defaultItemGrabRange;
            if (player.manaMagnet && (item.type == ItemID.Star || item.type == ItemID.SoulCake || item.type == ItemID.SugarPlum))
            {
                grabRange += Item.manaGrabRange;
            }
            if (player.lifeMagnet && (item.type == ItemID.Heart || item.type == ItemID.CandyApple || item.type == ItemID.CandyCane))
            {
                grabRange += Item.lifeGrabRange;
            }
            ItemLoader.GrabRange(item, player, ref grabRange);
            return grabRange;
        }

        public static Item GetDefault(int type)
        {
            var item = new Item();
            item.SetDefaults(type);
            return item;
        }
        public static Item GetDefault(Item item)
        {
            var item2 = new Item();
            item2.SetDefaults(item.type);
            return item2;
        }

        internal static string UseAnimTooltip(float useAnimation)
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

        internal static string KBTooltip(float knockback)
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

        internal static int FindTTLineSpot(List<TooltipLine> tooltips, string tooltipName)
        {
            switch (tooltipName)
            {
                case "Damage":
                    {
                        for (int i = tooltips.Count - 1; i >= 0; i--)
                        {
                            TooltipLine t = tooltips[i];
                            if (t.mod != "Terraria")
                                continue;
                            switch (t.Name)
                            {
                                case "Favorite":
                                case "FavoriteDesc":
                                case "Social":
                                case "SocialDesc":
                                case "Damage":
                                    return i + 1;
                            }
                        }
                    }
                    break;

                case "Material":
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            case "CritChance":
                            case "Speed":
                            case "Knockback":
                            case "FishingPower":
                            case "NeedsBait":
                            case "BaitPower":
                            case "Equipable":
                            case "WandConsumes":
                            case "Quest":
                            case "Vanity":
                            case "Defense":
                            case "PickPower":
                            case "AxePower":
                            case "HammerPower":
                            case "TileBoost":
                            case "HealLife":
                            case "HealMana":
                            case "UseMana":
                            case "Placeable":
                            case "Ammo":
                            case "Consumable":
                            case "Material":
                                return i + 1;
                        }
                    }
                    break;

                case "Tooltip#":
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            case "CritChance":
                            case "Speed":
                            case "Knockback":
                            case "FishingPower":
                            case "NeedsBait":
                            case "BaitPower":
                            case "Equipable":
                            case "WandConsumes":
                            case "Quest":
                            case "Vanity":
                            case "Defense":
                            case "PickPower":
                            case "AxePower":
                            case "HammerPower":
                            case "TileBoost":
                            case "HealLife":
                            case "HealMana":
                            case "UseMana":
                            case "Placeable":
                            case "Ammo":
                            case "Consumable":
                            case "Material":
                            case "Tooltip0":
                                return i + 1;
                        }
                    }
                    break;

                case "Equipable":
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            case "CritChance":
                            case "Speed":
                            case "Knockback":
                            case "FishingPower":
                            case "NeedsBait":
                            case "BaitPower":
                            case "Equipable":
                            case "Tooltip0":
                                return i + 1;
                        }
                    }
                    break;

                case "Expert":
                    for (int i = tooltips.Count - 1; i >= 0; i--)
                    {
                        TooltipLine t = tooltips[i];
                        if (t.mod != "Terraria")
                            continue;
                        switch (t.Name)
                        {
                            case "Favorite":
                            case "FavoriteDesc":
                            case "Social":
                            case "SocialDesc":
                            case "Damage":
                            case "CritChance":
                            case "Speed":
                            case "Knockback":
                            case "FishingPower":
                            case "NeedsBait":
                            case "BaitPower":
                            case "Equipable":
                            case "WandConsumes":
                            case "Quest":
                            case "Vanity":
                            case "Defense":
                            case "PickPower":
                            case "AxePower":
                            case "HammerPower":
                            case "TileBoost":
                            case "HealLife":
                            case "HealMana":
                            case "UseMana":
                            case "Placeable":
                            case "Ammo":
                            case "Consumable":
                            case "Material":
                            case "Tooltip0":
                            case "EtherianManaWarning":
                            case "WellFedExpert":
                            case "BuffTime":
                            case "OneDropLogo":
                            case "PrefixDamage":
                            case "PrefixSpeed":
                            case "PrefixCritChance":
                            case "PrefixUseMana":
                            case "PrefixSize":
                            case "PrefixShootSpeed":
                            case "PrefixKnockback":
                            case "PrefixAccDefense":
                            case "PrefixAccMaxMana":
                            case "PrefixAccCritChance":
                            case "PrefixAccDamage":
                            case "PrefixAccMoveSpeed":
                            case "PrefixAccMeleeSpeed":
                            case "SetBonus":
                            case "Expert":
                                return i + 1;
                        }
                    }
                    break;
            }
            return 1;
        }

        public static bool CheckAutoswingable(Player player, Item item, bool ignoreChanneled = false)
        {
            if (!item.autoReuse && item.useTime != 0 && item.useTime == item.useAnimation)
            {
                if (!ignoreChanneled && (item.channel || item.noUseGraphic))
                {
                    return player.ownedProjectileCounts[item.shoot] < item.stack && !Sets.NoAutoswing.Contains(item.type);
                }
                return player.altFunctionUse != 2;
            }
            return false;
        }

        internal static LocalizedText DamageTip(bool melee = false, bool ranged = false, bool magic = false, bool summon = false, bool thrown = false)
        {
            return melee ? Lang.tip[2] : (ranged ? Lang.tip[3] : (magic ? Lang.tip[4] : (thrown ? Lang.tip[58] : ((!summon) ? Lang.tip[55] : Lang.tip[53]))));
        }

        internal static void DrawEnergyItemInv(SpriteBatch spriteBatch, IColorGradient grad, Item item, Vector2 position, Vector2 origin, float scale)
        {
            spriteBatch.Draw(ModContent.GetTexture(item.modItem.GetPath("_Glow")), position, null, grad.GetColor(Main.GlobalTime), 0f, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.itemTexture[item.type], position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        internal static void DrawEnergyItemWorld(SpriteBatch spriteBatch, IColorGradient grad, Item item, float rotation, float scale, Vector2 spotlightOffset = default(Vector2))
        {
            var frame = new Rectangle(0, 0, Main.itemTexture[item.type].Width, Main.itemTexture[item.type].Height);
            Vector2 origin = frame.Size() / 2f;
            var drawPosition = new Vector2(item.position.X - Main.screenPosition.X + origin.X + item.width / 2 - origin.X, item.position.Y - Main.screenPosition.Y + origin.Y + item.height - frame.Height);
            drawPosition = new Vector2((int)drawPosition.X, drawPosition.Y);

            spriteBatch.Draw(ModContent.GetTexture(item.modItem.GetPath("_Glow")), drawPosition, frame, grad.GetColor(Main.GlobalTime).UseA(0), rotation, origin, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.BeginShader(Main.spriteBatch);
            SamplerRenderer.Light(drawPosition + spotlightOffset, (scale + (float)Math.Sin(Main.GlobalTime * 4f) * 0.1f + 0.65f) * 36f, grad.GetColor(Main.GlobalTime).UseA(0));
            Main.spriteBatch.End();
            BatcherMethods.GeneralEntities.Begin(Main.spriteBatch);

            spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

        }

        public static void UpdateEnergyItem(Item Item, Color clr, Vector3 lightClr)
        {
            if (Main.GameUpdateCount % 12 == 0)
            {
                clr.A = 0;
                int d = Dust.NewDust(Item.position, Item.width, Item.height - 4, ModContent.DustType<EnergyPulse>(), 0f, 0f, 0, clr);
                Main.dust[d].alpha = Main.rand.Next(0, 35);
                Main.dust[d].scale = Main.rand.NextFloat(0.95f, 1.15f);
                if (Main.dust[d].scale > 1f)
                    Main.dust[d].noGravity = true;
                Main.dust[d].velocity = new Vector2(Main.rand.NextFloat(-0.15f, 0.15f), Main.rand.NextFloat(-3.5f, -1.75f));
            }
            Lighting.AddLight(Item.position, lightClr);
        }

        public static Vector2 WorldDrawPos(Item item)
        {
            return new Vector2(item.position.X - Main.screenPosition.X + Main.itemTexture[item.type].Width / 2 + item.width / 2 - Main.itemTexture[item.type].Width / 2, item.position.Y - Main.screenPosition.Y + Main.itemTexture[item.type].Height / 2 + item.height - Main.itemTexture[item.type].Height + 2f);
        }

        public static bool MirrorCheck(Player player)
        {
            if (player.position.Y - 500f > Main.worldSurface * 16f)
                return false;
            int tileX = (int)(player.position.X + player.width / 2f) / 16;
            int tileY = (int)(player.position.Y + player.height / 2f) / 16;
            if (Main.tile[tileX, tileY] == null)
                Main.tile[tileX, tileY] = new Tile();
            if (AQWorldGen.TileObstructedFromLight(tileX, tileY))
                return false;
            for (int i = 0; i < 15; i++)
            {
                if (tileY - 1 - i <= 0)
                    break;
                if (Main.tile[tileX, tileY - 1 - i] == null)
                    Main.tile[tileX, tileY - 1 - i] = new Tile();
                if (AQWorldGen.TileObstructedFromLight(tileX, tileY - 1 - i))
                    return false;
            }
            return true;
        }

        public static Color DemonSiegeItemAlpha(Color lightColor)
        {
            return new Color(lightColor.R * lightColor.R, lightColor.G * lightColor.G, lightColor.B * lightColor.B, lightColor.A);
        }

        public static int GetLineIndex(List<TooltipLine> tooltips, string lineName)
        {
            int myIndex = InternalLineIndexFinder(lineName);
            //Main.NewText(myIndex);
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].mod == "Terraria" && InternalLineIndexFinder(tooltips[i].Name) >= myIndex)
                {
                    return i;
                }
            }
            return 1;
        }

        private static int InternalLineIndexFinder(string name)
        {
            for (int i = 0; i < TooltipNames.Length; i++)
            {
                if (name == TooltipNames[i])
                {
                    return i;
                }
            }
            return -1;
        }
    }
}