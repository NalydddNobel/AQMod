using AQMod.Assets;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.Projectiles.Magic
{
    public class FizzlingFire : ModProjectile
    {
        public const int FramesX = 6;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 30;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 5;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>());
            var diff = Main.dust[d].position - projectile.Center;
            float dustIntensity = diff.Length() / projectile.width;
            Main.dust[d].color = new Color(0.75f * dustIntensity, 0.75f * dustIntensity, 1f * dustIntensity, 0f);
            Main.dust[d].scale = dustIntensity * 2f;
            Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].velocity) * 8f * dustIntensity;
            if (projectile.timeLeft < 10 && projectile.alpha < byte.MaxValue)
            {
                projectile.alpha += 20;
                if (projectile.alpha > byte.MaxValue)
                    projectile.alpha = byte.MaxValue;
            }
            if (projectile.timeLeft > 2 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.velocity *= 0.9f;
                projectile.timeLeft -= 4;
                if (projectile.timeLeft < 2)
                {
                    projectile.timeLeft = 2;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Buffs.Debuffs.CorruptionHellfire.Inflict(target, 120);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>());
                var diff = Main.dust[d].position - projectile.Center;
                float dustIntensity = diff.Length() / projectile.width;
                Main.dust[d].color = new Color(0.75f * dustIntensity, 0.75f * dustIntensity, 1f * dustIntensity, 0f);
                Main.dust[d].scale = dustIntensity * 2f;
                Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].velocity) * 16f * dustIntensity;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = ModContent.GetTexture(this.GetPath("_Big"));
            var offset = new Vector2(projectile.width / 2f - Main.screenPosition.X, projectile.height / 2f - Main.screenPosition.Y);
            int frameWidth = texture.Width / 3;
            int frameHeight = texture.Height;
            var frame = new Rectangle(0, 0, frameWidth - 2, frameHeight - 2);
            var origin = frame.Size() / 2f;
            var color = new Color(100, 100, 100, 0);
            var rand = new UnifiedRandom(Main.player[projectile.owner].name.GetHashCode() + projectile.whoAmI);
            float intensity = (1f - projectile.alpha / 255f) *  AQConfigClient.c_EffectIntensity;
            intensity *= 2f;
            color *= intensity;
            for (int i = 0; i < 4; i++)
            {
                frame.X = rand.Next(3) * frameWidth;
                var off2 = new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4));
                Main.spriteBatch.Draw(texture, projectile.position + offset + off2, frame, color, projectile.rotation, origin, projectile.scale * 1.25f, SpriteEffects.None, 0f);
            }

            texture = Main.projectileTexture[projectile.type];
            frameWidth = texture.Width / FramesX;
            frameHeight = texture.Height / Main.projFrames[projectile.type];
            frame = new Rectangle(0, 0, frameWidth - 2, frameHeight - 2);
            origin = frame.Size() / 2f;
            float scale = projectile.scale * 1.4f;
            frame.Y = frameHeight;
            for (int i = 0; i < 3; i++)
            {
                frame.X = rand.Next(FramesX) * frameWidth;
                var off2 = new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-8, 8));
                Main.spriteBatch.Draw(texture, projectile.position + offset + off2, frame, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            }
            frame.Y = 0;
            scale = projectile.scale;
            color = new Color(200, 200, 200, 0);
            color *= intensity;
            for (int i = 0; i < 6; i++)
            {
                frame.X = rand.Next(FramesX) * frameWidth;
                var off2 = new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4));
                Main.spriteBatch.Draw(texture, projectile.position + offset + off2, frame, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            }

            if (AQConfigClient.c_EffectQuality < 1f)
            {
                return false;
            }

            texture = AQTextures.Lights[LightTex.Spotlight66x66];
            frame = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = frame.Size() / 2f;
            color = new Color(90, 90, 165, 0);
            color *= intensity;
            scale = projectile.scale * 1.25f;
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.15f, projectile.rotation, origin, scale * 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.06f, projectile.rotation, origin, scale * 4f, SpriteEffects.None, 0f);
            return false;
        }
    }
}