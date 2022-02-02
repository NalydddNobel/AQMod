using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class SeltzerRainProjectile : ModProjectile
    {
        private const int MouseDistance = 100;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
                projectile.ai[0] = projectile.velocity.Length();
            if (Main.myPlayer == projectile.owner)
            {
                if (projectile.ai[0] > 1f)
                {
                    projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, Main.MouseWorld - projectile.Center, 0.005f)) * projectile.ai[0];
                    projectile.ai[0] *= 0.96f;
                    if (projectile.ai[0] <= 1f)
                        projectile.ai[0] = -0.01f;
                }
                else
                {
                    float distance = Vector2.Distance(projectile.Center, Main.MouseWorld);
                    if (distance < MouseDistance)
                    {
                        float speed = MathHelper.Lerp(2f, 1f, distance / MouseDistance) * projectile.ai[0];
                        projectile.velocity = Vector2.Normalize(Vector2.Lerp(projectile.velocity, (Main.MouseWorld - projectile.Center) * speed, 0.025f));
                    }
                    if (projectile.ai[0] > -1f)
                        projectile.ai[0] -= 0.03f;
                }
                projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item118, projectile.position);
            int count = 20;
            float rot = MathHelper.TwoPi / count;
            var center = projectile.Center + new Vector2(-1f, -1f);
            var type = ModContent.DustType<MonoDust>();
            var color = new Color(50, 15, 190, 0);
            float off = projectile.width / 2f;
            float dustSpeed = projectile.velocity.Length() / 2f;
            for (int i = 0; i < count; i++)
            {
                var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                int d = Dust.NewDust(center + normal * off, 2, 2, type, 0f, 0f, 0, color);
                Main.dust[d].velocity = normal * dustSpeed;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f - Main.screenPosition.X, projectile.height / 2f - Main.screenPosition.Y);
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = frame.Size() / 2f;
            var color = new Color(250, 250, 250, 128);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            color *= 0.5f;
            float colorMult = 1f / trailLength;
            for (int i = 0; i < trailLength; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero)
                    break;
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset, frame, color * (colorMult * (trailLength - i + 1)), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            }
            float dist = Vector2.Distance(projectile.Center, Main.MouseWorld);
            if (dist < 200f)
            {
                colorMult = 1f - (dist / MouseDistance);
                texture = AQTextures.Lights[LightTex.Spotlight10x50];
                frame = new Rectangle(0, 0, texture.Width, texture.Height);
                color = new Color(50, 15, 190, 0) * colorMult;
                origin = texture.Size() / 2f;

                var texture2 = AQTextures.Lights[LightTex.Spotlight20x20];

                Main.spriteBatch.Draw(texture2, projectile.position + offset, null, color, projectile.rotation, texture2.Size() / 2f, projectile.scale * (colorMult * colorMult), SpriteEffects.None, 0f);

                var scale = new Vector2(projectile.scale * (0.55f - (1f - colorMult) * 0.2f), projectile.scale * (0.95f - (1f - colorMult) * 0.55f));

                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);

                scale *= 0.9f;

                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 1.4f, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 1.4f, projectile.rotation + MathHelper.PiOver2, origin, scale, SpriteEffects.None, 0f);

                scale *= 0.9f;
                scale *= 1f - (float)Math.Sin(projectile.timeLeft) * 0.1f;

                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.8f, projectile.rotation + MathHelper.PiOver4, origin, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.8f, projectile.rotation + MathHelper.PiOver4 * 3f, origin, scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}