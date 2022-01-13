using AQMod.Assets;
using AQMod.Common.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public class LotusShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SentryShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 600;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 87f)
            {
                if (projectile.velocity.Y < 8f)
                    projectile.velocity.Y += 0.5f;
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.01f;
            }
            else if (projectile.ai[0] > 86f)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.01f;
            }
            else if (projectile.ai[0] > 70f)
            {
                projectile.velocity.Y += 0.5f;
                projectile.ai[0]++;
                if (Main.rand.NextBool(4))
                {
                    int d = Dust.NewDust(projectile.position + new Vector2((float)Math.Sin(projectile.position.Y / 32f) * 8f, 0f), projectile.width, projectile.height, DustID.Fire);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.25f;
                }
            }
            else if (projectile.ai[0] > 60f)
            {
                projectile.velocity *= 0.9f;
                if (projectile.velocity.Length() <= 0.5f)
                {
                    projectile.velocity = new Vector2(0f, 0f);
                    projectile.ai[0]++;
                }
                if (Main.rand.NextBool(8))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.01f;
                }
            }
            else
            {
                projectile.ai[0] += 0.5f;
                if (Main.rand.NextBool(8))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 0.01f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[0] = 87f;
            projectile.velocity *= 0.1f;
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = projectile.position.Y < projectile.ai[1];
            return true;
        }

        public static void Draw(Vector2 center, float rotation, float scale, int i, float j)
        {
            var texture = AQTextures.Lights[LightTex.Spotlight30x30];
            float intensity = ModContent.GetInstance<AQConfigClient>().EffectIntensity * (((float)Math.Sin(Main.GlobalTime * 4f) + 1f) / 4f + 0.2f + (1f - j / 9f) * 0.75f);
            var orig = texture.Size() / 2f;
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, new Color(128, 95, 10, 0) * intensity, 0f, orig, scale * intensity, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, new Color(56, 25, 1, 0) * intensity, 0f, orig, scale * intensity * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, new Color(56, 25, 1, 0) * intensity, 0f, orig, new Vector2(scale * intensity * 0.2f, scale * intensity * 1.75f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, new Color(56, 25, 1, 0) * intensity, MathHelper.PiOver2, orig, new Vector2(scale * intensity * 0.2f, scale * intensity * 1.75f), SpriteEffects.None, 0f);
            texture = TextureGrabber.GetProjectile(ModContent.ProjectileType<LotusShot>());
            orig = texture.Size() / 2f;
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, new Color(255, 255, 255, 128), rotation, orig, scale + 0.25f * intensity, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition + Utils.RandomVector2(Main.rand, -2f, 2f) * intensity, null, new Color(255, 255, 255, 128), rotation, orig, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition + Utils.RandomVector2(Main.rand, -2f, 2f) * intensity, null, new Color(255, 255, 255, 128), rotation, orig, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition + Utils.RandomVector2(Main.rand, -4f, 4f) * intensity, null, new Color(128, 128, 128, 0), rotation, orig, scale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Draw(projectile.Center, projectile.rotation, projectile.scale, projectile.whoAmI, projectile.velocity.Length());
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.01f;
            }
        }
    }
}