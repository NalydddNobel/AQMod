using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Dusts;
using AQMod.Effects.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class MagicWandProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 38;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 3;
            projectile.alpha = 240;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 4;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
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

            if (projectile.alpha > 255)
            {
                return;
            }
            float alpha = 1f - projectile.alpha / 255f;
            if (projectile.localAI[0] > 2f * (projectile.extraUpdates + 1f))
            {
                int d = Dust.NewDust(center, 2, 2, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(150, 100, 255, 0), 1.15f);
                Main.dust[d].velocity = Vector2.Normalize(projectile.velocity.RotatedBy(MathHelper.PiOver2)) * (float)Math.Sin(projectile.timeLeft * 0.75f) * 2f;
                if (Main.rand.NextBool(3))
                {
                    d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(222, 100, 255, 0) * alpha, Main.rand.NextFloat(0.6f, 1.25f));
                    Main.dust[d].velocity = -projectile.velocity * 0.1f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.rand.NextBool(5))
                    {
                        var dustPos2 = projectile.Center + Vector2.Normalize(projectile.velocity) * (projectile.width / 2f - 6f);
                        AQMod.Particles.PostDrawPlayers.AddParticle(
                            new SparkleParticle(dustPos2, -projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)) * Main.rand.NextFloat(0.07f, 0.4f),
                            new Color(220, 220, 220, 0) * alpha, Main.rand.NextFloat(0.25f, 0.75f)));
                    }
                    if (Main.rand.NextBool(4))
                    {
                        float scale = Main.rand.NextFloat(1f, 1.45f);
                        var dustPos1 = projectile.Center + Vector2.Normalize(projectile.velocity) * (projectile.width / 2f - 6f);
                        AQMod.Particles.PostDrawPlayers.AddParticle(
                            new EmberParticle(dustPos1, (projectile.velocity * 0.015f).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)),
                            new Color(175, 60, 220, 0) * alpha, scale));
                    }
                }
            }
            else
            {
                projectile.localAI[0]++;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.timeLeft = reader.ReadInt32();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.alpha > 255)
            {
                return false;
            }
            float alpha = 1f - projectile.alpha / 255f;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            var drawCoordinates = projectile.position + offset - Main.screenPosition;
            float scaling = AQUtils.Wave(Main.GlobalTime * 25f, 0.9f, 1.1f);
            var bloom = LegacyTextureCache.Lights[LightTex.Spotlight66x66];
            var bloomOrigin = bloom.Size() / 2f;
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f - 1f / trailLength * i;
                Main.spriteBatch.Draw(bloom, projectile.oldPos[i] + offset - Main.screenPosition, null, Color.Lerp(new Color(100, 10, 200, 0), new Color(10, 0, 100, 0), progress) * (1f - progress) * scaling * alpha, 0f, bloomOrigin, projectile.scale * 0.8f * progress, SpriteEffects.None, 0f);
            }
            trailLength /= 2;
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f - 1f / trailLength * i;
                Main.spriteBatch.Draw(bloom, projectile.oldPos[i] + offset - Main.screenPosition, null, Color.Lerp(new Color(150, 20, 255, 0), new Color(100, 10, 200, 0), progress) * (1f - progress) * scaling * alpha, 0f, bloomOrigin, projectile.scale * 0.8f * progress, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(bloom, drawCoordinates, null, new Color(150, 20, 255, 0) * scaling * alpha, 0f, bloomOrigin, projectile.scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloom, drawCoordinates, null, new Color(100, 10, 200, 0) * scaling * alpha, 0f, bloomOrigin, projectile.scale * 0.8f, SpriteEffects.None, 0f);
            var texture = Main.projectileTexture[projectile.type];
            var origin = texture.Size() / 2f;
            Main.spriteBatch.Draw(texture, drawCoordinates, null, projectile.GetAlpha(lightColor) * scaling * alpha, 0f, origin, projectile.scale * 0.8f * scaling, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            int type = ModContent.DustType<MonoDust>();
            var clr = new Color(200, 100, 255, 0);
            int d = Dust.NewDust(center, 2, 2, type, 0f, 0f, 0, clr, 1.5f);
            float sum = projectile.position.X + projectile.position.Y;
            Main.dust[d].velocity = Vector2.Zero;

            int projectileX = (int)projectile.position.X + projectile.width / 2;
            int projectileY = (int)projectile.position.Y + projectile.height / 2;
            Main.PlaySound(SoundID.DD2_BetsyFireballShot.SoundId, projectileX, projectileY, SoundID.DD2_BetsyFireballShot.Style, 0.85f, -0.25f);

            int height = 12 * 16;
            var rect = new Rectangle(projectileX, projectileY - height / 2, 16, height);
            for (int k = 0; k < height / 2; k++)
            {
                int d1 = Dust.NewDust(rect.TopLeft(), rect.Width, rect.Height, type, 0, 0, 0, clr, 2f);
                Main.dust[d1].rotation = 0f;
                Main.dust[d1].velocity *= 0.5f;
            }
            if (Main.netMode != NetmodeID.Server && AQConfigClient.Instance.EffectQuality >= 1f)
            {
                for (int k = 0; k < height; k++)
                {
                    var velo = projectile.velocity * 0.015f;
                    var particlePos = new Vector2(Main.rand.Next(rect.X, rect.X + rect.Width), Main.rand.Next(rect.Y, rect.Y + rect.Height));
                    AQMod.Particles.PostDrawPlayers.AddParticle(
                        new EmberParticle(particlePos, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-0.1f, 0.1f)),
                        new Color(180, 100, 255, 0), 1.35f));
                }
            }
            type = ModContent.ProjectileType<MagicPillar>();

            int p = Projectile.NewProjectile(projectile.position + new Vector2(projectile.width / 2f - 8f, projectile.height / 2f - height / 2f), new Vector2(20f, 0f), type, projectile.damage, projectile.knockBack, projectile.owner);
            Main.projectile[p].height = height;
            p = Projectile.NewProjectile(projectile.position + new Vector2(projectile.width / 2f + -8f, projectile.height / 2f + -height / 2f), new Vector2(-20f, 0f), type, projectile.damage, projectile.knockBack, projectile.owner);
            Main.projectile[p].height = height;
            p = Projectile.NewProjectile(projectile.Center, new Vector2(-20f, 0f), ModContent.ProjectileType<MagicExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
            Main.projectile[p].width = Main.projectile[p].height = (int)(height * 2.5f);
            Main.projectile[p].position.X -= Main.projectile[p].width / 2f;
            Main.projectile[p].position.Y -= Main.projectile[p].width / 2f;
            if (AQConfigClient.Instance.EffectQuality >= 1f)
                dustFlower(6, height, height * 1.25f, (int)(125 * AQConfigClient.Instance.EffectQuality));
        }

        private void dustFlower(int petals, float minSize, float maxSize, int amount = 40)
        {
            var center = projectile.Center;
            float r = MathHelper.TwoPi / amount;
            float mult = r * petals;
            int type = ModContent.DustType<MonoDust>();
            var clr = new Color(200, 100, 255, 0);
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
