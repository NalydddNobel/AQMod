using Aequus.Common;
using Aequus.Projectiles.Magic;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Items.Unused
{
    public class LiquidGun : ModItem
    {
        public struct LiquidInfo
        {
            public byte LiquidID { get; internal set; }
            public string TexturePath;
            public Func<int> DrainageDustID;
            public Func<int> ProjDustID;
            public Color BarColor;

            public LiquidInfo(int liquidID, string gunTexture, Color barColor, Func<int> drainDust, Func<int> projDust)
            {
                LiquidID = (byte)liquidID;
                TexturePath = gunTexture;
                DrainageDustID = drainDust;
                ProjDustID = projDust;
                BarColor = barColor;
            }

            public LiquidInfo(int liquidID, string gunTexture, Color barColor, int drainDust, int projDust) : this(liquidID, gunTexture, barColor, () => drainDust, () => projDust)
            {
            }
        }

        private static Dictionary<byte, LiquidInfo> liquidCatalogue;
        private static Asset<Texture2D> originalTextureCache;

        public virtual byte LiquidAmountMax => 75;

        public byte LiquidType;
        public byte LiquidAmount;

        private Asset<Texture2D> altTextureCache;

        protected override bool CloneNewInstances => true;

        public static void AssignLiquidInfo(LiquidInfo info)
        {
            if (liquidCatalogue.ContainsKey(info.LiquidID))
            {
                liquidCatalogue[info.LiquidID] = info;
                return;
            }
            liquidCatalogue.Add(info.LiquidID, info);
        }

        public static bool TryGetLiquidInfo(int liquidID, out LiquidInfo info)
        {
            return liquidCatalogue.TryGetValue((byte)liquidID, out info);
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            liquidCatalogue = new Dictionary<byte, LiquidInfo>();
            AssignLiquidInfo(new LiquidInfo(LiquidID.Water, $"{Texture}Water", new Color(200, 222, 255, 255), () => Dust.dustWater(), () => Dust.dustWater()));
            AssignLiquidInfo(new LiquidInfo(LiquidID.Lava, $"{Texture}Lava", new Color(255, 211, 200, 255), DustID.Lava, DustID.Torch));
            AssignLiquidInfo(new LiquidInfo(LiquidID.Honey, $"{Texture}Honey", new Color(255, 255, 200, 255), DustID.Honey, DustID.Honey2));
            AssignLiquidInfo(new LiquidInfo(3, $"{Texture}Shimmer", new Color(233, 233, 255, 255), DustID.AncientLight, DustID.AncientLight));
        }

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowMasksHandler.AddGlowmask($"{Texture}Lava_Glow");
                GlowMasksHandler.AddGlowmask($"{Texture}Shimmer_Glow");
                originalTextureCache = TextureAssets.Item[Type];
            }
            Item.ResearchUnlockCount = 1;
        }

        public override void Unload()
        {
            liquidCatalogue?.Clear();
            liquidCatalogue = null;
        }

        //public override ModItem Clone(Item newEntity)
        //{
        //    var val = base.Clone(newEntity);
        //    ((LiquidGun)val).CheckTexture();
        //    return val;
        //}

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
            Item.damage = 17;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Gray;
            //Item.shoot = ModContent.ProjectileType<LiquidGunProj>();
            Item.shoot = ProjectileID.WaterGun;
            Item.shootSpeed = 1f;
            Item.value = ItemDefaults.ValueCrabCrevice;
            Item.autoReuse = true;
            Item.knockBack = 1.5f;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.UseSound = SoundID.SplashWeak.WithPitchOffset(-0.1f);
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        //public override bool CanUseItem(Player player)
        //{
        //    return LiquidAmount > 0;
        //}

        //public override void HoldItem(Player player)
        //{
        //    if (player.HeldItemFixed().ModItem is LiquidGun liquidGun)
        //    {
        //        liquidGun.HandleHoldEffectsFixed(player);
        //    }
        //}

        //private void HandleHoldEffectsFixed(Player player)
        //{
        //    if (LiquidAmount >= LiquidAmountMax)
        //    {
        //        return;
        //    }

        //    var r = AequusHelpers.TileRectangle(player.Center, 16, 16);
        //    int closestX = 0;
        //    int closestY = 0;
        //    float closestDistance = 160f;
        //    for (int i = r.X; i <= r.X + r.Width; i++)
        //    {
        //        for (int j = r.Y; j <= r.Y + r.Height; j++)
        //        {
        //            if (WorldGen.InWorld(i, j, 10) && Main.tile[i, j].LiquidAmount > 0)
        //            {
        //                if (LiquidAmount > 0 && Main.tile[i, j].LiquidType != LiquidType || !Collision.CanHitLine(player.position, player.width, player.height, new Vector2(i * 16f, j * 16f), 16, 16))
        //                {
        //                    continue;
        //                }
        //                float distance = Vector2.Distance(player.Center, new Vector2(i * 16f + 8f, j * 16f + 8f)) + Main.rand.NextFloat(-60f, 16f);
        //                if (distance < closestDistance)
        //                {
        //                    closestDistance = distance;
        //                    closestX = i;
        //                    closestY = j;
        //                }
        //                if (distance < 160f)
        //                {
        //                    Lighting.AddLight(new Vector2(i * 16f + 8f, j * 16f + 8f), new Vector3(0.2f, 0.2f, 0.2f));
        //                    if (Main.rand.NextBool(240))
        //                    {
        //                        var d = Dust.NewDustDirect(new Vector2(i * 16f, j * 16f + (int)((255 - Main.tile[closestX, closestY].LiquidAmount) / 255f * 16f)), 16, 16, DustID.TreasureSparkle, Scale: Main.rand.NextFloat(0.6f, 1f) / 2f);
        //                        d.velocity = Vector2.Zero;
        //                        d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        //                        d.fadeIn = d.scale + 0.66f;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (closestX != 0 && closestY != 0)
        //    {
        //        var m = this;
        //        byte liquidAmt = 4;
        //        if (!player.CheckMana(20, pay: false))
        //        {
        //            return;
        //        }
        //        if (Main.GameUpdateCount % 12 == 0)
        //        {
        //            player.CheckMana(16, pay: true);
        //            SoundEngine.PlaySound(SoundID.SplashWeak.WithPitchOffset(0.5f), new Vector2(closestX * 16 + 8f, closestY * 16 + 8f));
        //            if (LiquidAmount == 0 || LiquidType != Main.tile[closestX, closestY].LiquidType)
        //            {
        //                LiquidType = (byte)Main.tile[closestX, closestY].LiquidType;
        //                CheckTexture();
        //            }

        //            m.LiquidAmount += liquidAmt;
        //            if (m.LiquidAmount > m.LiquidAmountMax)
        //            {
        //                m.LiquidAmount = m.LiquidAmountMax;
        //            }
        //        }

        //        int dustID = -1;
        //        if (TryGetLiquidInfo(LiquidType, out var info))
        //        {
        //            dustID = info.DrainageDustID();
        //        }
        //        if (dustID != -1)
        //        {
        //            if (Main.GameUpdateCount % 4 == 0)
        //            {
        //                var d = Dust.NewDustPerfect(new Rectangle(closestX * 16, closestY * 16 + (int)((255 - Main.tile[closestX, closestY].LiquidAmount) / 255f * 16f), 16, 16).ClosestDistance(player.Center), dustID, newColor: new Color(255, 255, 255, 0), Scale: Main.rand.NextFloat(0.6f, 1f));
        //                d.velocity = (player.Center - d.position).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) / 16f + player.velocity;
        //                d.noGravity = true;
        //                d.fadeIn = d.scale + 0.2f;
        //            }
        //            if (Main.GameUpdateCount % 5 == 0)
        //            {
        //                var waterPoint = new Rectangle(closestX * 16, closestY * 16 + (int)((255 - Main.tile[closestX, closestY].LiquidAmount) / 255f * 16f), 16, 16).ClosestDistance(player.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(24f));

        //                var nextPos = player.Center;
        //                var diff = waterPoint - nextPos;
        //                int length = Math.Max((int)(diff.Length() / 4f), 1);
        //                for (int i = 0; i < length; i++)
        //                {
        //                    nextPos += Vector2.Normalize(diff) * 4f;
        //                    var d = Dust.NewDustPerfect(nextPos, dustID, Scale: Main.rand.NextFloat(0.6f, 1.5f));
        //                    d.velocity = player.velocity;
        //                    d.noGravity = true;
        //                    d.fadeIn = d.scale + 0.2f;
        //                }
        //            }
        //        }
        //    }
        //}

        //public void ClearTexture()
        //{
        //    altTextureCache = null;
        //    Item.glowMask = -1;
        //    if (Main.dedServ)
        //        return;
        //    TextureAssets.Item[Type] = originalTextureCache;
        //}
        //public void CheckTexture()
        //{
        //    if (Main.dedServ || LiquidAmount == 0)
        //    {
        //        ClearTexture();
        //        return;
        //    }

        //    if (altTextureCache != null)
        //    {
        //        TextureAssets.Item[Type] = altTextureCache;
        //        return;
        //    }

        //    if (!liquidCatalogue.TryGetValue(LiquidType, out var info))
        //    {
        //        ClearTexture();
        //        return;
        //    }

        //    TextureAssets.Item[Type] = altTextureCache = ModContent.Request<Texture2D>(info.TexturePath, AssetRequestMode.ImmediateLoad);
        //    GlowMasksHandler.TryGetID($"{info.TexturePath}_Glow", out Item.glowMask);
        //}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (LiquidAmount > 0)
            {
                string text = Language.GetTextValueWith($"Mods.{Mod.Name}.ItemTooltip.{Name}.AmountLeft",
                    new { Amount = Math.Max((int)(LiquidAmount / (float)LiquidAmountMax * 100f), 1), Liquid = TextHelper.LiquidName(LiquidType) });
                tooltips.Insert(tooltips.GetIndex("Tooltip#"), new TooltipLine(Mod, "LiquidTooltip", text));
            }
        }

        //public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        //{
        //    if (AequusUI.CurrentItemSlot.Context == ItemSlot.Context.HotbarItem && LiquidAmount > 0)
        //    {
        //        var center = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
        //        var bottom = center + new Vector2(0f, TextureAssets.InventoryBack.Value.Height / 2f) * Main.inventoryScale;
        //        if (Main.playerInventory)
        //        {
        //            bottom.Y -= 12f * Main.inventoryScale;
        //        }
        //        var color = Color.White;
        //        if (TryGetLiquidInfo(LiquidType, out var info))
        //        {
        //            color = info.BarColor;
        //        }
        //        DrawHealthbar(spriteBatch, TextureAssets.Hb2.Value, bottom, color, 1f);
        //        DrawHealthbar(spriteBatch, TextureAssets.Hb1.Value, bottom, color, LiquidAmount / (float)LiquidAmountMax);
        //    }
        //    CheckTexture();
        //    if (altTextureCache != null)
        //    {
        //        spriteBatch.Draw(altTextureCache.Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        //        return false;
        //    }
        //    return true;
        //}

        //public void DrawHealthbar(SpriteBatch spriteBatch, Texture2D hbTexture, Vector2 loc, Color color, float progress)
        //{
        //    float scale = Main.inventoryScale;
        //    int width = (int)Math.Max(hbTexture.Width * progress, 1) / 2 * 2;
        //    spriteBatch.Draw(hbTexture, loc, new Rectangle(0, 0, width, 6), color, 0f, new Vector2(hbTexture.Width / 2f, 3f), scale, SpriteEffects.None, 0f);
        //    spriteBatch.Draw(hbTexture, loc + new Vector2(0f, 6f) * scale, new Rectangle(0, hbTexture.Height - 6, width, 6), color, 0f, new Vector2(hbTexture.Width / 2f, 3f), scale, SpriteEffects.None, 0f);
        //}

        //public void DrawHealthbar(SpriteBatch spriteBatch, Texture2D hbTexture, Vector2 loc, Color color)
        //{
        //    float scale = Main.inventoryScale;
        //    spriteBatch.Draw(hbTexture, loc, new Rectangle(0, 0, hbTexture.Width, 6), color, 0f, new Vector2(hbTexture.Width / 2f, 3f), scale, SpriteEffects.None, 0f);
        //    spriteBatch.Draw(hbTexture, loc + new Vector2(0f, 6f) * scale, new Rectangle(0, hbTexture.Height - 6, hbTexture.Width, 6), color, 0f, new Vector2(hbTexture.Width / 2f, 3f), scale, SpriteEffects.None, 0f);
        //}

        //public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        //{
        //    CheckTexture();
        //    return true;
        //}

        //public void PreDrawPlayer(Player player, AequusPlayer aequus, ref PlayerDrawSet drawInfo)
        //{
        //    CheckTexture();
        //    if (altTextureCache != null)
        //    {
        //        TextureAssets.Item[Type] = altTextureCache;
        //    }
        //}
    }
}