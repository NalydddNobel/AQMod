using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon.PassiveHelmets
{
    public sealed class FlowerCrownProj : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.FlowerPetal;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.scale = 0.8f;
            projectile.timeLeft = 600;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 1)
            {
                if (projectile.timeLeft > 240)
                {
                    projectile.timeLeft = 240;
                }
                if (projectile.timeLeft < 85)
                {
                    projectile.scale -= 0.0055555f;
                    projectile.gfxOffY += 0.01f;
                }
            }
            else
            {
                if (projectile.position.Y < Main.worldSurface * 16f && Main.tile[((int)projectile.position.X + projectile.width / 2) /16, ((int)projectile.position.Y + projectile.height / 2) / 16].wall == WallID.None)
                {
                    projectile.ai[1]++;

                    float decrement = Math.Max(projectile.ai[1], 0f);
                    float gotoSpeed = Main.windSpeed * 5f + AQUtils.Wave(projectile.timeLeft * 0.25f, -2f, 0.2f) * Math.Sign(Main.windSpeed);
                    projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, gotoSpeed, 0.01f) * (1f - Math.Min(decrement * 0.001f, 0.95f));
                    if (Main.windSpeed.Abs() > 0.1f)
                    {
                        projectile.velocity.Y = 0.02f + decrement * 0.002f + AQUtils.Wave(projectile.timeLeft * 0.02f, -0f, 0.1f);
                    }
                    else
                    {
                        projectile.velocity.Y = 0.02f + decrement * 0.00008f;
                    }
                    projectile.rotation += Math.Max(projectile.velocity.X * 0.015f, 0.015f);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = Vector2.Zero;
            projectile.damage = 0;
            projectile.tileCollide = false;
            projectile.ai[0] = 1f;
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var frame = texture.Frame(verticalFrames: Main.projFrames[projectile.type], frameY: projectile.frame);
            var origin = frame.Size() / 2f;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f + projectile.gfxOffY) - Main.screenPosition;

            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f / trailLength * i;
                spriteBatch.Draw(texture, projectile.oldPos[i] + offset, frame, lightColor * (1f - progress) * 0.5f, projectile.rotation, origin, projectile.scale * (1f - progress), SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, projectile.position + offset, frame, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}