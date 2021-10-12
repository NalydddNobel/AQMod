using AQMod.Assets;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class JerryBubble : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 0f)
            {
                projectile.ai[0]++;
                return;
            }
            float speed = -4f;
            float addY = -0.05f;
            if (Main.expertMode)
            {
                speed *= 2f;
                addY *= 1.5f;
            }
            if (Collision.WetCollision(projectile.position, projectile.width, projectile.height))
            {
                speed *= 2f;
                addY *= 1.5f;
            }
            if (projectile.velocity.Y > speed)
                projectile.velocity.Y += addY;

            Player player = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)];
            if (player.dead || !player.active)
                projectile.velocity.X *= 0.96f;
            else
            {
                projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, player.position.X + player.width / 2f < projectile.position.X + projectile.width / 2f ? -1 : 1, 0.08f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.ai[0] < 0f)
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.X != oldVelocity.X)
                    projectile.velocity.X = -oldVelocity.X;
                if (projectile.velocity.Y != oldVelocity.Y)
                    projectile.velocity.Y = -oldVelocity.Y;
                return false;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (lightColor.R < 40)
                lightColor.R = 40;
            if (lightColor.G < 40)
                lightColor.G = 40;
            if (lightColor.B < 40)
                lightColor.B = 40;
            lightColor.A = (byte)(90 + (Math.Sin(projectile.timeLeft * 0.35f) + 1f) / 2f * 100f);
            return lightColor;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var drawPosition = projectile.Center - Main.screenPosition;
            lightColor = projectile.GetAlpha(lightColor);
            var size = lightColor.A / 255f + 0.1f;
            var orig = texture.Size() / 2f;
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, projectile.rotation, orig, projectile.scale * size, SpriteEffects.None, 0f);
            return false;
        }
    }
}