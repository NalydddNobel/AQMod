using AQMod.Dusts.GaleStreams;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class RedSpriteCloudLightning : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.timeLeft = 90;

            projectile.GetGlobalProjectile<AQProjectile>().SetupTemperatureStats(20);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var orig = texture.Size() / 2f;
            var drawPosition = projectile.Center;
            var scale = new Vector2(projectile.scale, projectile.scale);
            float speedX = projectile.velocity.X.Abs();
            lightColor = projectile.GetAlpha(lightColor);
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            var origin = frame.Size() / 2f;
            float electric = 3f + (float)Math.Sin(Main.GlobalTime * 5f);
            if (electric > 0f)
            {
                for (int i = 0; i < 8; i++)
                {
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition + new Vector2(electric, 0f).RotatedBy(MathHelper.PiOver4 * i + Main.GlobalTime * 5f), frame, new Color(100, 20, 2, 0), projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                }
            }
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            for (int i = 0; i < 50; i++)
            {
                int d = Dust.NewDust(projectile.position, 16, 16, ModContent.DustType<RedSpriteDust>());
                Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
            }
        }
    }
}