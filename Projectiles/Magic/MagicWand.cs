using AQMod.Assets.Graphics.ParticlesLayers;
using AQMod.Common.WorldGeneration;
using AQMod.Content.Dusts;
using AQMod.Content.Particles;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using AQMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class MagicWand : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            var center = projectile.Center;
            if (Main.myPlayer == projectile.owner)
            {
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[0] = 1f;
                    projectile.timeLeft = (int)((center - Main.MouseWorld) / projectile.velocity.Length()).Length();
                    projectile.velocity /= 1 + projectile.extraUpdates;
                    projectile.timeLeft *= 1 + projectile.extraUpdates;
                }
            }

            if (projectile.localAI[0] > 2f * (projectile.extraUpdates + 1f))
            {
                int d = Dust.NewDust(center, 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(255, 150, 150, 0), 1.5f);
                Main.dust[d].velocity = Vector2.Normalize(projectile.velocity.RotatedBy(MathHelper.PiOver2)) * (float)Math.Sin(projectile.timeLeft * 0.75f) * 2f;
                if (Main.rand.NextBool(3))
                {
                    d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(255, 222, 222, 0), Main.rand.NextFloat(0.6f, 1.25f));
                    Main.dust[d].velocity = -projectile.velocity * 0.1f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    var velo = projectile.velocity * 0.015f;
                    for (int i = 0; i < 3; i++)
                    {
                        var pos = projectile.position - new Vector2(2f, 2f);
                        var rect = new Rectangle((int)pos.X, (int)pos.Y, projectile.width + 4, projectile.height + 4);
                        var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new MonoParticleEmber(dustPos, -projectile.velocity * Main.rand.NextFloat(0.015f, 0.1f),
                            new Color(255, 150, 150, 0)));
                    }
                    float scale = Main.rand.NextFloat(1.25f, 2.25f);
                    var dustPos1 = projectile.Center + Vector2.Normalize(projectile.velocity) * (projectile.width / 2f - 6f);
                    ParticleLayers.AddParticle_PostDrawPlayers(
                        new MonoParticleEmber(dustPos1, velo,
                        new Color(220, 220, 220, 0), scale));
                    ParticleLayers.AddParticle_PostDrawPlayers(
                        new MonoParticleEmber(dustPos1, velo,
                        new Color(60, 45, 45, 0), scale * 2f));
                }
            }
            else
            {
                projectile.localAI[0]++;
            }
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            int type = ModContent.DustType<MonoDust>();
            var clr = new Color(255, 150, 150, 0);
            int d = Dust.NewDust(center, 2, 2, type, 0f, 0f, 0, clr, 1.5f);
            float sum = projectile.position.X + projectile.position.Y;
            Main.dust[d].velocity = Vector2.Zero;
            int x = (int)(center.X / 16f);
            int y = (int)(center.Y / 16f);
            const int maxHeight = 6;
            int topY = -maxHeight;
            int bottomY = maxHeight;
            for (int i = 0; i < maxHeight; i++)
            {
                if (AQWorldGen.ActiveAndSolid(x, y - maxHeight + i))
                {
                    topY = -maxHeight + i;
                }
                if (AQWorldGen.ActiveAndSolid(x, y + maxHeight - i))
                {
                    bottomY = maxHeight - i;
                }
            }
            float x2 = x * 16f;
            float x3 = x * 16f + 8f;
            int projectileX = (int)projectile.position.X + projectile.width / 2;
            int projectileY = (int)projectile.position.Y + projectile.height / 2;
            Main.PlaySound(SoundID.DD2_BetsyFireballShot.SoundId, projectileX, projectileY, SoundID.DD2_BetsyFireballShot.Style, 0.8f, 0.5f);
            if (Main.myPlayer == projectile.owner && AQMod.TonsofScreenShakes)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[projectile.owner].Center);
                if (distance < 800)
                    ScreenShakeManager.AddEffect(new BasicScreenShake(10, AQMod.MultIntensity((int)(1600f - distance) / 400)));
            }

            for (int yy = y + topY; yy < y + bottomY; yy++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int d1 = Dust.NewDust(new Vector2(x2, yy * 16f), 16, 16, type, 0, 0, 0, clr, 2f);
                    Main.dust[d1].velocity *= 0.5f;
                }
            }
            if (Main.netMode != NetmodeID.Server && AQMod.EffectQuality >= 1f)
            {
                for (int yy = y + topY; yy < y + bottomY; yy++)
                {
                    var velo = projectile.velocity * 0.015f;
                    for (int i = 0; i < 3; i++)
                    {
                        var pos = new Vector2(x2, yy * 16f);
                        var rect = new Rectangle((int)pos.X, (int)pos.Y, 16, 16);
                        var dustPos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                        ParticleLayers.AddParticle_PostDrawPlayers(
                            new MonoParticleEmber(dustPos, new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.1f, 0.1f)),
                            new Color(255, 180, 180, 0)));
                    }
                }
            }
            type = ModContent.ProjectileType<MagicPillar>();
            int height = (y + bottomY - (y + topY)) * 16;
            int p = Projectile.NewProjectile(x3, (y + topY) * 16f, 20f, 0f, type, projectile.damage, projectile.knockBack, projectile.owner);
            Main.projectile[p].height = height;
            p = Projectile.NewProjectile(x3, (y + topY) * 16f, -20f, 0f, type, projectile.damage, projectile.knockBack, projectile.owner);
            Main.projectile[p].height = height;
            dustFlower(6, projectile.width * 2f, projectile.height * 4f, (int)(50 * AQMod.EffectQuality));
        }

        private void dustFlower(int petals, float minSize, float maxSize, int amount = 40)
        {
            var center = projectile.Center;
            float r = MathHelper.TwoPi / amount;
            float mult = r * petals;
            int type = ModContent.DustType<MonoDust>();
            var clr = new Color(255, 150, 150, 0);
            float rotOffset = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            for (int i = 0; i < amount; i++)
            {
                for (float j = 0f; j <= 0.2f; j += 0.1f)
                {
                    var off = new Vector2(0f, MathHelper.Lerp(minSize, maxSize, ((float)Math.Sin(i * mult) + 1f) / 2f)).RotatedBy(i * r + rotOffset + j);
                    int d = Dust.NewDust(center + off,
                        2, 2, type, 0f, 0f, 0, clr, 1f);
                    Main.dust[d].velocity *= 0.1f;
                    Main.dust[d].velocity += Vector2.Normalize(off) * 3f;
                }
            }
        }
    }
}
