using Aequus;
using Aequus.Common.Particles;
using Aequus.Common.Recipes;
using Aequus.Content.Necromancy;
using Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Corruption;
using Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Crimson;
using Aequus.NPCs;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Misc.Friendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.Misc.Healer {
    [AutoloadGlowMask]
    public class FriendshipMagick : ModItem {

        public override void SetStaticDefaults() {
            AequusRecipes.AddShimmerCraft(ModContent.ItemType<ZombieSceptre>(), Type);
            AequusRecipes.AddShimmerCraft(ModContent.ItemType<CrimsonSceptre>(), Type);
            AequusItem.HasCooldown.Add(Type);
            Item.staff[Type] = true;
        }

        public override void SetDefaults() {

            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<FriendshipMagickProj>();
            Item.shootSpeed = 2f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.mana = 20;
            Item.UseSound = SoundID.Item4;
        }

        public override bool CanUseItem(Player player) {
            return !player.Aequus().HasCooldown;
        }

        public override bool? UseItem(Player player) {
            player.Aequus().SetCooldown(600, ignoreStats: true, Item);
            return null;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, Vector2.Distance(position, Main.MouseWorld) / velocity.Length());
            return false;
        }

        public override void AddRecipes() {
        }
    }
}

namespace Aequus.Projectiles.Misc.Friendly {
    public class FriendshipMagickProj : ModProjectile {
        public virtual float Tier => 2.33f;

        public override void SetDefaults() {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.alpha = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.75f;
            Projectile.timeLeft = 180;
            Projectile.alpha = 250;
            Projectile.hide = true;
            Projectile.extraUpdates = 50;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 100, 255, 200);
        }

        public override bool? CanDamage() {
            return false;
        }

        public override void AI() {

            if ((int)Projectile.ai[0] == 0) {
                Projectile.ai[1]--;
                Projectile.timeLeft = 180;
                if (Projectile.ai[1] < 0f) {
                    Projectile.ai[1] = 0f;
                    Projectile.ai[0] = 1f;
                    Projectile.numUpdates = 0;
                    Projectile.extraUpdates = 0;
                    Projectile.velocity = Vector2.Zero;
                }
                return;
            }

            int healingRange = 100;

            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.velocity = Vector2.Zero;

            if (Main.netMode != NetmodeID.Server && !Main.rand.NextBool(4)) {
                var center = Projectile.Center;
                var v = Main.rand.NextVector2Unit();
                float opacity = Projectile.ai[1] / healingRange * Projectile.Opacity;
                ParticleSystem.New<FriendshipParticle>(
                    ParticleLayer.BehindAllNPCs).Setup(
                    Projectile.Center + v * Main.rand.NextFloat(Projectile.ai[1] - 30f, Projectile.ai[1] + 10f),
                    v.RotatedBy(Projectile.direction * MathHelper.PiOver2) * 1.5f,
                    Color.White with { A = 50 } * opacity,
                    Color.HotPink with { A = 100 } * 0.225f * opacity,
                    Main.rand.NextFloat(0.6f, 1.2f) * opacity,
                    0.2f, 0f);
            }

            float velocityMultiplier = 0.88f;

            Projectile.ai[0]++;
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && Projectile.Distance(Main.npc[i].getRect().ClosestPointInRect(Projectile.Center)) < healingRange) {
                    velocityMultiplier *= 0.5f;
                    if (Projectile.ai[0] > 30f) {
                        if (Main.npc[i].boss || !Main.npc[i].TryGetGlobalNPC<AequusNPC>(out var aequusNPC) || aequusNPC.IsZombie) {
                            continue;
                        }

                        int healAmt = Helper.CalcHealing(Main.npc[i].life, Main.npc[i].lifeMax, Main.npc[i].lifeMax / 10);

                        if (healAmt > 0) {

                            foreach (var v in Helper.CircularVector(8, Main.rand.NextFloat(MathHelper.TwoPi))) {
                                float off = Main.rand.NextFloat(0.7f, 1f);
                                var d = Dust.NewDustPerfect(Main.npc[i].Center + v * Main.npc[i].Size * off, ModContent.DustType<MonoDust>(), v * -Main.npc[i].Size * 0.08f * off, newColor: new Color(255, 100, 222, 0),
                                    Scale: Main.rand.NextFloat(1f, 1.6f));
                                d.alpha = 100;
                                d.fadeIn = d.scale + 0.6f;
                                d.velocity += Main.npc[i].velocity;
                            }
                            Main.npc[i].life += healAmt;
                            if (Main.netMode != NetmodeID.MultiplayerClient) {
                                Projectile.netUpdate = true;
                                Main.npc[i].HealEffect(healAmt, broadcast: true);
                                Main.npc[i].netUpdate = true;
                            }
                            continue;
                        }

                        if (Main.npc[i].friendly || Main.npc[i].lifeMax < 5) {
                            continue;
                        }

                        // Make friendly when at max HP
                        Main.npc[i].Aequus().friendship = true;
                    }

                    // Spawn sparkle
                    if ((int)Projectile.ai[0] % 6 == 0) {
                        ParticleSystem.New<MonoFlashParticle>(
                            ParticleLayer.AboveNPCs).Setup(
                            Main.rand.NextFromRect(Main.npc[i].Hitbox),
                            Vector2.Zero,
                            Color.HotPink.UseA(0) * Projectile.Opacity,
                            Color.HotPink with { A = 0 } * 0.33f * Projectile.Opacity,
                            Main.rand.NextFloat(0.5f, 1.1f) * Projectile.Opacity,
                            0.2f, 0f);
                    }
                }
            }

            if (Projectile.ai[0] > 30f) {
                Projectile.ai[0] = 1f;
            }

            if (Projectile.timeLeft < 60) {
                Projectile.alpha += 5;
            }
            else if (Projectile.alpha > 0) {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.ai[1] < healingRange) {
                Projectile.ai[1] += Projectile.Opacity * 4f;
                if (Projectile.ai[1] >= healingRange) {
                    Projectile.ai[1] = healingRange;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.velocity.Length() > 4f) {
                Projectile.velocity *= velocityMultiplier;
            }
            else {
                Projectile.velocity = Vector2.Normalize(Projectile.velocity).UnNaN() * 4f;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.5f, 1f));
            foreach (var v in Helper.CircularVector(40)) {
                Lighting.AddLight(Projectile.Center + v * (Projectile.ai[1] - 12f), new Vector3(0.44f, 0.1f, 0.44f));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {

            Projectile.ai[0] = 1f;
            Projectile.ai[1] = 0f;
            Projectile.numUpdates = 0;
            Projectile.extraUpdates = 0;
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;
            var aura = AequusTextures.FriendshipMagickProj_Aura;

            float auraScale = Projectile.scale * Projectile.ai[1] / texture.Width / 4f + 0.1f;
            var auraColor = Color.White with { A = 100 };
            if (Projectile.ai[0] > 5f) {

                float scale = (Projectile.ai[0] - 5f) / 25f;
                Main.spriteBatch.Draw(aura, Projectile.Center - Main.screenPosition, null, auraColor * Projectile.Opacity * scale * 0.5f,
                    0f, aura.Size() / 2f, auraScale * scale * 1.1f, SpriteEffects.None, 0f);
            }
            if (Projectile.ai[0] < 15f) {

                float scale = Projectile.ai[0] / 15f;
                Main.spriteBatch.Draw(aura, Projectile.Center - Main.screenPosition, null, auraColor * Projectile.Opacity * (1f - scale) * 0.5f,
                    0f, aura.Size() / 2f, auraScale * 1.1f + scale * 0.1f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(aura, Projectile.Center - Main.screenPosition, null, auraColor * Projectile.Opacity,
                0f, aura.Size() / 2f, auraScale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity,
                Projectile.rotation + Helper.Wave(Main.GlobalTimeWrappedHourly * 10f, -0.15f, 0.15f), origin, Projectile.scale + Helper.Wave(Main.GlobalTimeWrappedHourly * 7f, -0.15f, 0.15f), SpriteEffects.None, 0f);
            return false;
        }
    }

    public sealed class FriendshipParticle : BaseBloomParticle<FriendshipParticle> {
        public int flash;

        protected override void SetDefaults() {
            SetFramedTexture(AequusTextures.FriendshipParticle, 3);
            bloomOrigin = AequusTextures.Bloom0.Size() / 2f;
            flash = 0;
        }

        public override void Update(ref ParticleRendererSettings settings) {

            Velocity *= 0.98f;
            flash++;
            if (flash < 40) {
                Scale *= 1.005f;
            }
            else {
                Scale *= 0.9f;
            }

            if (Scale <= 0.1f || float.IsNaN(Scale)) {
                RestInPool();
                return;
            }
            if (!dontEmitLight)
                Lighting.AddLight(Position, BloomColor.ToVector3() * 0.5f);
            Position += Velocity;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {

            spritebatch.Draw(AequusTextures.Bloom0, Position - Main.screenPosition, null, BloomColor * Scale, Rotation, bloomOrigin, Scale * BloomScale, SpriteEffects.None, 0f);
            spritebatch.Draw(texture, Position - Main.screenPosition, frame, Color, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool friendship;

        private void PostAI_UpdateFriendship(NPC npc) {
            if (!friendship) {
                return;
            }

            if (npc.life < npc.lifeMax) {
                friendship = false;
                npc.friendly = false;
                npc.netUpdate = true;
                return;
            }

            npc.friendly = true;
        }
    }
}

namespace Aequus.Projectiles {
    public partial class AequusProjectile {
        public bool friendship;

        private void PostAI_UpdateFriendship(Projectile projectile) {
            if (!friendship) {
                return;
            }

            if (!HasNPCOwner || !Main.npc[sourceNPC].Aequus().friendship) {
                friendship = false;
                projectile.hostile = true;
                projectile.netUpdate = true;
                return;
            }

            projectile.hostile = false;
        }
    }
}