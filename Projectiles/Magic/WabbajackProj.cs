﻿using Aequus.Content;
using Aequus.Items.Weapons.Magic.Misc.Wabbajack.Debuffs;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Projectiles.Magic {
    public class WabbajackProj : ModProjectile {
        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            LegacyPushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            Projectile.scale = 0.9f;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
        }

        public override void AI() {
            Projectile.ShimmerReflection();
            if (Projectile.alpha > 0) {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }
            if (Main.rand.NextBool(9) && Projectile.alpha < 100) {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, Color.Red.UseA(0), 1f);
            }
            Projectile.rotation += Projectile.velocity.Length() * Main.rand.NextFloat(0.01f, 0.0157f);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            var origin = frame.Size() / 2f;
            var center = Projectile.Center;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);

            Main.spriteBatch.Draw(AequusTextures.Bloom0, center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, Projectile.scale * 0.3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(AequusTextures.Bloom0, center - Main.screenPosition, null, Color.Red * Projectile.Opacity, Projectile.rotation, AequusTextures.Bloom0.Size() / 2f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, new Color(250, 180, 170, 160) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            bool weakEnoughForInstantKill = target.lifeMax < 1200 && target.defense < 50 && target.realLife == -1;
            var rand = new WeightedRandom<int>(Main.rand);
            rand.Add(0, 2f);
            rand.Add(1, 1.5f);
            rand.Add(2, 1.5f);
            rand.Add(3, 1.5f);
            rand.Add(4, 2f);
            switch (rand.Get()) {
                case 1: {
                        if (!target.immortal && (target.realLife == -1 || target.realLife == target.whoAmI))
                            target.AddBuff(ModContent.BuffType<WabbajackTeleport>(), 30);
                    }
                    break;
                case 2: {
                        if (!target.immortal && !target.boss && weakEnoughForInstantKill)
                            target.AddBuff(ModContent.BuffType<WabbajackTransformBunny>(), 30);
                    }
                    break;
                case 3: {
                        if (!target.immortal && !target.boss && weakEnoughForInstantKill)
                            target.AddBuff(ModContent.BuffType<WabbajackTransformFood>(), 30);
                    }
                    break;
                case 4: {
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, new Vector2(Math.Sign(Projectile.Center.X - Main.player[Projectile.owner].Center.X), 0f),
                            ProjectileID.Grenade, 40 + Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    break;
            }
        }

        public override void OnKill(int timeLeft) {
            var center = Projectile.Center;
            float size = Projectile.width / 2f;
            for (int i = 0; i < 30; i++) {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>());
                var n = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi).ToRotationVector2();
                Main.dust[d].position = center + n * Main.rand.NextFloat(0f, size);
                Main.dust[d].velocity = n * Main.rand.NextFloat(2f, 7f);
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.75f);
                Main.dust[d].color = Color.Red.UseA(0) * Main.rand.NextFloat(0.8f, 2f);
            }
        }
    }

    public class WabbajackEffect : ModProjectile {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60;
        }

        public override void OnSpawn(IEntitySource source) {
            if (Helper.HereditarySource(source, out var entity)) {
                Projectile.ai[0] = entity.width;
                Projectile.position.X = entity.position.X;
            }
        }

        public override void AI() {
            if (Projectile.localAI[0] > 0f) {
                return;
            }
            Projectile.localAI[0]++;
            for (int i = 0; i < Projectile.ai[0] * 1.5f; i++) {
                int d = Dust.NewDust(Projectile.position, (int)Projectile.ai[0], Projectile.height, ModContent.DustType<MonoDust>());
                Main.dust[d].velocity = new Vector2(0f, -1f).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.5f, 9f);
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 2.75f);
                Main.dust[d].color = Color.Red.UseA(100) * Main.rand.NextFloat(0.8f, 2f);
            }
            SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
        }
    }
}