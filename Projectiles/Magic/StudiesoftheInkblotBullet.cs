using AQMod.Assets;
using AQMod.Common.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class StudiesoftheInkblotBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 275;
            projectile.penetrate = -1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return GetProjectileColor(projectile.frame);
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] >= 100f)
            {
                projectile.ai[0]++;
                if (projectile.ai[0] > 120f && projectile.localAI[1] <= 0f)
                {
                    projectile.position.X -= 40f;
                    projectile.position.Y -= 40f;
                    projectile.width += 80;
                    projectile.height += 80;
                    projectile.localAI[1] = 1f;
                }
                if (projectile.ai[0] > 140f)
                {
                    projectile.ai[0] = 100f;
                    projectile.localAI[1] = 0f;
                    projectile.position.X += 40f;
                    projectile.position.Y += 40f;
                    projectile.width -= 80;
                    projectile.height -= 80;
                }
            }
            if ((projectile.Center - Main.player[projectile.owner].Center).Length() < 1250f)
            {
                projectile.timeLeft = 20;
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 5;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }

            projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(projectile.velocity) * projectile.ai[1], projectile.velocity.Length() / projectile.ai[1] * 0.1f);

            if (projectile.alpha < 200)
            {
                if (projectile.alpha > 0)
                    projectile.alpha -= 25;
                projectile.localAI[0] *= 0.8f;
                float colorMultiplier = 1 - projectile.alpha / 255f;
                Lighting.AddLight(projectile.Center, projectile.GetAlpha(default(Color)).ToVector3() * projectile.scale * 0.5f * colorMultiplier);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.alpha > 255)
            {
                return false;
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 20f;
            }
            float colorMultiplier = 1 - projectile.alpha / 255f;
            float colorMultiplierSquared = colorMultiplier * colorMultiplier;
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f);
            var spotlight = LegacyTextureCache.Lights[LightTex.Spotlight66x66];
            spriteBatch.Draw(spotlight, projectile.position + offset - Main.screenPosition, null, projectile.GetAlpha(lightColor) * colorMultiplierSquared, projectile.rotation, spotlight.Size() / 2f, projectile.scale * 0.6f, SpriteEffects.None, 0f);
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, frame, new Color(255, 255, 255, 255) * colorMultiplierSquared, projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            if (projectile.localAI[0] > projectile.scale * 0.6f)
            {
                spriteBatch.Draw(spotlight, projectile.position + offset - Main.screenPosition, null, projectile.GetAlpha(lightColor) * colorMultiplier, projectile.rotation, spotlight.Size() / 2f, projectile.scale * projectile.localAI[0], SpriteEffects.None, 0f);
                spriteBatch.Draw(spotlight, projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255) * colorMultiplier, projectile.rotation, spotlight.Size() / 2f, projectile.scale * projectile.localAI[0] * 0.7f, SpriteEffects.None, 0f);
            }

            if (projectile.ai[0] > 120f)
            {
                var explosionTexture = mod.GetTexture("Projectiles/Explosion");
                int explosionFrameNumber = (int)((projectile.ai[0] - 120f) / 4f);
                var explosionFrame = explosionTexture.Frame(verticalFrames: 5, frameY: explosionFrameNumber);
                var explosionOrigin = explosionFrame.Size() / 2f;
                spriteBatch.Draw(explosionTexture, projectile.position + offset - Main.screenPosition, explosionFrame, projectile.frame == 1 ? Color.Red : Color.Blue, 0f, explosionOrigin, projectile.scale * 2f, SpriteEffects.None, 0f);
            }
            return false;
        }

        public static Color GetProjectileColor(int frame)
        {
            switch (frame)
            {
                default:
                    return new Color(255, 255, 255, 255);
                case 1:
                    return new Color(255, 10, 10, 255);
                case 2:
                    return new Color(255, 255, 10, 255);
                case 3:
                    return new Color(50, 255, 10, 255);
                case 4:
                    return new Color(10, 255, 255, 255);
                case 5:
                    return new Color(10, 10, 255, 255);
                case 6:
                    return new Color(255, 10, 255, 255);
            }
        }
    }
}