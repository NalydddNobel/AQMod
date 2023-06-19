using Aequus.Common.Graphics;
using Aequus.Items.Materials;
using Aequus.Items.Weapons.Ranged.Misc;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Ranged;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Items.Weapons.Ranged.Misc {
    [AutoloadGlowMask]
    public class BoundBow : ModItem {
        public const int MaxAmmo = 15;
        public const int RegenerationDelay = 50;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.damage = 72;
            Item.crit = 21;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<BoundBowProj>();
            Item.shootSpeed = 24.5f;
            Item.UseSound = SoundID.Item5;
            Item.value = Item.sellPrice(gold: 2, silver: 75);
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 4f;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(lightColor.R, lightColor.G, lightColor.B, lightColor.A - 80);
        }

        public override bool CanUseItem(Player player) {
            return player.Aequus().boundBowAmmo > 0;
        }

        public override bool? UseItem(Player player) {
            var aequus = player.Aequus();
            aequus.boundBowAmmo--;
            aequus.boundBowAmmoTimer = 120;
            return true;
        }

        public override Vector2? HoldoutOffset() {
            return new Vector2(8f);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            if (Main.playerInventory || Main.gameMenu || !Main.PlayerLoaded || AequusUI.CurrentItemSlot.Context == ItemSlot.Context.HotbarItem)
                return;

            if (!Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var aequus))
                return;

            var center = ItemSlotRenderer.InventoryItemGetCorner(position, frame, scale);
            center.X -= TextureAssets.InventoryBack.Value.Width / 2f * Main.inventoryScale;
            center.Y += TextureAssets.InventoryBack.Value.Height / 2f * Main.inventoryScale;
            string ammo = aequus.boundBowAmmo.ToString();

            var color = Color.Lerp(Color.BlueViolet, Color.White, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.45f, 1f));
            var font = FontAssets.MouseText.Value;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, ammo, center + new Vector2(8f, -24f) * Main.inventoryScale, color, 0f, Vector2.Zero, new Vector2(1f) * Main.inventoryScale * 0.8f, spread: Main.inventoryScale);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.EbonwoodBow)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient<PossessedShard>(5)
                .AddTile(TileID.Anvils)
                .TryRegisterBefore(ItemID.OnyxBlaster)
                .Clone()
                .ReplaceItem(ItemID.EbonwoodBow, ItemID.ShadewoodBow)
                .TryRegisterAfter(ItemID.OnyxBlaster);
        }
    }
}

namespace Aequus {
    public partial class AequusPlayer : ModPlayer {
        public int boundBowAmmo;
        public int boundBowAmmoTimer;

        public void Initialize_BoundBow() {
            boundBowAmmo = BoundBow.MaxAmmo;
            boundBowAmmoTimer = BoundBow.RegenerationDelay;
        }

        public void clientClone_BoundBow(AequusPlayer clone) {
            clone.boundBowAmmo = boundBowAmmo;
            clone.boundBowAmmoTimer = boundBowAmmoTimer;
        }

        public bool ShouldSyncBoundBow(AequusPlayer client) {
            return boundBowAmmo != client.boundBowAmmo || client.boundBowAmmoTimer <= 0 && boundBowAmmoTimer > 0;
        }

        public void PostUpdate_BoundBow() {
            if (boundBowAmmoTimer > 0)
                boundBowAmmoTimer--;
            if (boundBowAmmoTimer <= 0) {
                bool selected = Main.myPlayer == Player.whoAmI && Player.HeldItem.ModItem is BoundBow;
                if (Main.netMode != NetmodeID.Server) {
                    float volume = 0.2f;
                    if (selected) {
                        volume = 0.55f;
                        ScreenShake.SetShake(4);
                    }
                    SoundEngine.PlaySound(AequusSounds.boundBowRecharge with { Volume = volume, }, Player.Center);

                    Vector2 widthMethod(float p) => new Vector2(16f) * (float)MathF.Sin(p * MathHelper.Pi);
                    Color colorMethod(float p) => Color.BlueViolet.UseA(150) * 1.1f;

                    for (int i = 0; i < 8; i++) {
                        var d = Dust.NewDustPerfect(Player.position + new Vector2(Player.width * 2f * Main.rand.NextFloat(1f) - Player.width / 2f, Player.height * Main.rand.NextFloat(0.2f, 1.2f)), ModContent.DustType<MonoSparkleDust>(),
                            new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-4.5f, -1f)), newColor: Color.BlueViolet.UseA(25), Scale: Main.rand.NextFloat(0.5f, 1.25f));
                        d.fadeIn = d.scale + 0.5f;
                        d.color *= d.scale;
                    }
                    for (int i = 0; i < 3; i++) {
                        var prim = new TrailRenderer(TrailTextures.Trail[3].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
                        var v = new Vector2(Player.width * 2f / 3f * i - Player.width / 2f + Main.rand.NextFloat(-6f, 6f), Player.height * Main.rand.NextFloat(0.9f, 1.2f));
                        var particle = ParticleSystem.Fetch<BoundBowTrailParticle>().Setup(prim, Player.position + v, new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), Main.rand.NextFloat(-10f, -8f)),
                            scale: Main.rand.NextFloat(0.85f, 1.5f), trailLength: 10, drawDust: false);
                        particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
                        particle.prim.GetColor = (p) => colorMethod(p) * Math.Min(particle.Scale, 1.5f);
                        ParticleSystem.GetLayer(ParticleLayer.AbovePlayers).Add(particle);
                        if (i < 2) {
                            prim = new TrailRenderer(TrailTextures.Trail[3].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
                            particle = ParticleSystem.Fetch<BoundBowTrailParticle>().Setup(prim, Player.position + new Vector2(Player.width * i, Player.height * Main.rand.NextFloat(0.9f, 1.2f) + 10f), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-12.4f, -8.2f)),
                            scale: Main.rand.NextFloat(0.85f, 1.5f), trailLength: 10, drawDust: false);
                            particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
                            particle.prim.GetColor = (p) => new Color(35, 10, 125, 150) * Math.Min(particle.Scale, 1.5f);
                            ParticleSystem.GetLayer(ParticleLayer.BehindPlayers).Add(particle);
                        }
                    }
                }
                boundBowAmmo++;
                boundBowAmmoTimer = BoundBow.RegenerationDelay;
            }
            if (boundBowAmmo >= BoundBow.MaxAmmo) {
                boundBowAmmoTimer = BoundBow.RegenerationDelay;
            }
        }
    }
}