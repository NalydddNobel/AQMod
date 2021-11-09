using AQMod.Assets;
using AQMod.Common;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class AirBladeGhost : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.alpha = 0;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.manualDirectionChange = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 40;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255) * (projectile.alpha / 255f);
        }

        public override void AI()
        {
            var player = Main.player[projectile.owner];
            var center = projectile.Center;
            if (projectile.ai[0] == 0f && projectile.ai[1] == 0f)
            {
                projectile.ai[0] = projectile.velocity.X;
                projectile.ai[1] = projectile.velocity.Y;
                projectile.timeLeft = Main.rand.Next(30, 130);
            }
            int target = AQNPC.FindTarget(projectile.Center, 400f);
            if (target != -1)
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.npc[target].Center - center) * (float)Math.Sqrt(projectile.ai[0] * projectile.ai[0] + projectile.ai[1] * projectile.ai[1]) * 2f, 0.1f);
            }
            else
            {
                projectile.velocity = new Vector2(projectile.ai[0], projectile.ai[1]).RotatedBy((float)Math.Sin((projectile.timeLeft - 40f) / 15f));
            }
            if (projectile.timeLeft < 15)
            {
                projectile.alpha -= 15;
            }
            if (projectile.timeLeft > 25 && projectile.alpha < 200)
            {
                projectile.alpha += 15;
            }
            projectile.direction = projectile.velocity.X <= 0f ? -1 : 1;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.ToRotation() + (MathHelper.PiOver4 * 3f);
            if (projectile.spriteDirection == -1)
                projectile.rotation += -MathHelper.PiOver2;
            float progress = projectile.alpha / 255f;
            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(235, 150, 25, 20) * Main.rand.NextFloat(0.8f, 1.25f) * progress, 0.8f);
                Main.dust[d].velocity = -projectile.velocity * 0.1f;
            }
        }

        public override void Kill(int timeLeft)
        {
            float progress = projectile.alpha / 255f;
            for (int i = 0; i < 15; i++)
            {
                int d = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(235, 150, 25, 20) * Main.rand.NextFloat(0.8f, 1.25f) * progress, 1.35f);
                Main.dust[d].velocity = -projectile.velocity * 0.15f;
                d = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(235, 150, 25, 20) * Main.rand.NextFloat(0.15f, 0.45f) * progress, 2f);
                Main.dust[d].velocity = -projectile.velocity * Main.rand.NextFloat(0.2f, 0.35f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            var drawPosition = projectile.Center + Vector2.Normalize(projectile.velocity) * (projectile.width / 2f);
            var effect = SpriteEffects.None;
            var origin = new Vector2(10f, 10f);
            if (projectile.direction == -1)
            {
                origin.X = texture.Width - origin.X;
                effect = SpriteEffects.FlipHorizontally;
            }
            lightColor = projectile.GetAlpha(lightColor);
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effect, 0f);
            float progress = projectile.alpha / 255f;
            lightColor *= 0.2f + progress * 0.2f;
            lightColor.A = 0;
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, drawPosition + new Vector2(((float)Math.Sin(Main.GlobalTime * 10f) + 1f) + 2f, 0f).RotatedBy(MathHelper.PiOver2 * i) - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effect, 0f);
            }
            if (projectile.localAI[0] != 0f)
            {
                texture = TextureCache.Lights[Assets.Textures.SpotlightID.Spotlight30x30];
                frame = texture.Frame();
                lightColor = new Color(130, 100, 12, 1) * AQMod.EffectIntensity;
                origin = texture.Size() / 2f;
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress * 0.3f, 0f, origin, projectile.scale, effect, 0f);
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress, 0f, origin, new Vector2(projectile.scale * 2f, projectile.scale * 0.2f * progress), effect, 0f);
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress, 0f, origin, new Vector2(projectile.scale * 0.2f * progress, projectile.scale * 2f), effect, 0f);
                if (AQMod.EffectQuality >= 1f)
                {
                    lightColor *= 0.25f;
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress * 0.5f, 0f, origin, projectile.scale, effect, 0f);
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress, 0f, origin, new Vector2(projectile.scale * 2f, projectile.scale * 0.2f * progress), effect, 0f);
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress, 0f, origin, new Vector2(projectile.scale * 0.2f * progress, projectile.scale * 2f), effect, 0f);
                }
            }
            return false;
        }
    }
}