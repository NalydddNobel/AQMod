using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Weapons.Magic.LiquidGun
{
    public class LiquidGun : ModItem
    {
        public static List<int> LiquidGunIDs { get; private set; }
        public static Dictionary<byte, int> LiquidToItemID { get; private set; }

        public virtual byte LiquidAmountMax => 100;

        public byte LiquidType;
        public byte LiquidAmount;

        protected override bool CloneNewInstances => true;

        public override void Load()
        {
            if (LiquidGunIDs == null)
                LiquidGunIDs = new List<int>();
            if (LiquidToItemID == null)
                LiquidToItemID = new Dictionary<byte, int>();
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            LiquidGunIDs.Add(Type);
        }

        public override void Unload()
        {
            LiquidGunIDs?.Clear();
            LiquidGunIDs = null;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["LiquidType"] = LiquidType;
            tag["LiquidAmount"] = LiquidAmount;
        }

        public override void LoadData(TagCompound tag)
        {
            LiquidType = tag.Get<byte>("LiquidType");
            LiquidAmount = tag.Get<byte>("LiquidAmount");
        }

        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileID.WaterGun;
            Item.shootSpeed = 16f;
            Item.value = Item.sellPrice(gold: 7, silver: 50);
            Item.autoReuse = true;
            Item.knockBack = 1f;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.UseSound = SoundID.SplashWeak.WithPitchOffset(-0.1f);
        }

        public override void OnResearched(bool fullyResearched)
        {
            foreach (var i in LiquidGunIDs)
            {
                CreativeUI.ResearchItem(i);
            }
        }

        public override bool CanUseItem(Player player)
        {
            return LiquidAmount > 0;
        }

        public override void HoldItem(Player player)
        {
            if (LiquidAmount >= LiquidAmountMax)
            {
                return;
            }

            var r = AequusHelpers.TileRectangle(player.Center, 16, 16);
            int closestX = 0;
            int closestY = 0;
            float closestDistance = 160f;
            for (int i = r.X; i <= r.X + r.Width; i++)
            {
                for (int j = r.Y; j <= r.Y + r.Height; j++)
                {
                    if (WorldGen.InWorld(i, j, 10) && Main.tile[i, j].LiquidAmount > 0)
                    {
                        if ((LiquidAmount > 0 && Main.tile[i, j].LiquidType != LiquidType) || !Collision.CanHitLine(player.position, player.width, player.height, new Vector2(i * 16f, j * 16f), 16, 16))
                        {
                            continue;
                        }
                        float distance = Vector2.Distance(player.Center, new Vector2(i * 16f + 8f, j * 16f + 8f)) + Main.rand.NextFloat(-60f, 16f);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestX = i;
                            closestY = j;
                        }
                        if (distance < 160f)
                        {
                            Lighting.AddLight(new Vector2(i * 16f + 8f, j * 16f + 8f), new Vector3(0.2f, 0.2f, 0.2f));
                            if (Main.rand.NextBool(240))
                            {
                                var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f + (int)((255 - Main.tile[closestX, closestY].LiquidAmount) / 255f * 16f)), 16, 16, DustID.TreasureSparkle, Scale: Main.rand.NextFloat(0.6f, 1f) / 2f);
                                d.velocity = Vector2.Zero;
                                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                                d.fadeIn = d.scale + 0.66f;
                            }
                        }
                    }
                }
            }
            if (closestX != 0 && closestY != 0)
            {
                var m = this;
                if (Main.GameUpdateCount % 12 == 0)
                {
                    SoundEngine.PlaySound(SoundID.SplashWeak.WithPitchOffset(0.5f), new Vector2(closestX * 16 + 8f, closestY * 16 + 8f));
                    if (LiquidAmount == 0 || LiquidType != Main.tile[closestX, closestY].LiquidType)
                    {
                        LiquidType = (byte)Main.tile[closestX, closestY].LiquidType;
                        foreach (var pair in LiquidToItemID)
                        {
                            if (pair.Key == LiquidType)
                            {
                                Item.Transform(pair.Value);
                                m = Item.ModItem<LiquidGun>();
                                m.LiquidType = LiquidType;
                                break;
                            }
                        }
                    }

                    m.LiquidAmount += (byte)Math.Max(m.LiquidAmount / 10, 1);
                    if (m.LiquidAmount > m.LiquidAmountMax)
                    {
                        m.LiquidAmount = m.LiquidAmountMax;
                    }
                }

                int dustID = DustID.Lava;
                if (LiquidType == LiquidID.Water)
                {
                    dustID = Dust.dustWater();
                }
                else if (LiquidType == LiquidID.Honey)
                {
                    dustID = DustID.Honey;
                }
                if (Main.GameUpdateCount % 4 == 0)
                {
                    var d = Dust.NewDustPerfect(AequusHelpers.ClosestDistance(new Rectangle(closestX * 16, closestY * 16 + (int)((255 - Main.tile[closestX, closestY].LiquidAmount) / 255f * 16f), 16, 16), player.Center), dustID, newColor: new Color(255, 255, 255, 0), Scale: Main.rand.NextFloat(0.6f, 1f));
                    d.velocity = (player.Center - d.position).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) / 16f + player.velocity;
                    d.noGravity = true;
                    d.fadeIn = d.scale + 0.2f;
                }
                if (Main.GameUpdateCount % 5 == 0)
                {
                    var waterPoint = AequusHelpers.ClosestDistance(new Rectangle(closestX * 16, closestY * 16 + (int)((255 - Main.tile[closestX, closestY].LiquidAmount) / 255f * 16f), 16, 16), player.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(24f));

                    var nextPos = player.Center;
                    var diff = waterPoint - nextPos;
                    int length = Math.Max((int)(diff.Length() / 4f), 1);
                    for (int i = 0; i < length; i++)
                    {
                        nextPos += Vector2.Normalize(diff) * 4f;
                        var d = Dust.NewDustPerfect(nextPos, dustID, Scale: Main.rand.NextFloat(0.6f, 1.5f));
                        d.velocity = player.velocity;
                        d.noGravity = true;
                        d.fadeIn = d.scale + 0.2f;
                    }
                }
            }
        }

        public void LiquidAmountTooltip(List<TooltipLine> tooltips, string liquidType)
        {
            string text = Language.GetTextValueWith($"Mods.{Mod.Name}.ItemTooltip.LiquidGun.AmountLeft",
                new { Amount = Math.Max((int)(LiquidAmount / (float)LiquidAmountMax * 100f), 1), Liquid = liquidType });
            tooltips.Insert(AequusTooltips.GetIndex(tooltips, "Tooltip#"), new TooltipLine(Mod, "LiquidTooltip", text));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (LiquidAmount > 0)
                LiquidAmount--;
            if (LiquidAmount == 0)
            {
                Item.Transform(ModContent.ItemType<LiquidGun>());
            }
            return true;
        }
    }
}