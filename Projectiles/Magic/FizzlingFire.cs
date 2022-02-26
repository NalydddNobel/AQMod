using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Dusts;
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
        private int _lifeTime = 1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 18;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 35;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 5;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            _lifeTime += 1;
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>());
            var diff = Main.dust[d].position - projectile.Center;
            float dustIntensity = diff.Length() / projectile.width;
            Main.dust[d].color = new Color(0.75f * dustIntensity, 0.75f * dustIntensity, 1f * dustIntensity, 0f);
            Main.dust[d].scale = dustIntensity * 2f;
            Main.dust[d].velocity = Vector2.Normalize(Main.dust[d].velocity) * 8f * dustIntensity;
            if (projectile.timeLeft < 13 && projectile.alpha < byte.MaxValue)
            {
                projectile.alpha += 30;
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

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Buffs.Debuffs.CorruptionHellfire.Inflict(target, 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            float scale = projectile.scale;
            if (_lifeTime < 10)
            {
                scale *= _lifeTime / 10f;
            }
            var texture = ModContent.GetTexture(this.GetPath("_Big"));
            var offset = new Vector2(projectile.width / 2f - Main.screenPosition.X, projectile.height / 2f - Main.screenPosition.Y);
            int frameWidth = texture.Width / 3;
            int frameHeight = texture.Height;
            var frame = new Rectangle(0, 0, frameWidth - 2, frameHeight - 2);
            var origin = frame.Size() / 2f; int a = projectile.alpha;
            var color = new Color(90 - a, 90 - a, 110 - a, 0);
            var rand = new UnifiedRandom(Main.player[projectile.owner].name.GetHashCode() + projectile.whoAmI);
            float intensity = AQConfigClient.Instance.EffectIntensity;
            intensity *= 2f;
            color *= intensity;
            for (int i = 0; i < 4; i++)
            {
                frame.X = rand.Next(3) * frameWidth;
                var off2 = new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4));
                Main.spriteBatch.Draw(texture, projectile.position + offset + off2, frame, color, projectile.rotation, origin, scale * 1.25f, SpriteEffects.None, 0f);
            }

            texture = Main.projectileTexture[projectile.type];
            frameWidth = texture.Width / FramesX;
            frameHeight = texture.Height / Main.projFrames[projectile.type];
            frame = new Rectangle(0, 0, frameWidth - 2, frameHeight - 2);
            origin = frame.Size() / 2f;
            frame.Y = frameHeight;
            for (int i = 0; i < 3; i++)
            {
                frame.X = rand.Next(FramesX) * frameWidth;
                var off2 = new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-8, 8));
                Main.spriteBatch.Draw(texture, projectile.position + offset + off2, frame, color, projectile.rotation, origin, scale * 1.4f, SpriteEffects.None, 0f);
            }
            frame.Y = 0;
            color = new Color(180 - a, 180 - a, 220 - a, 0);
            color *= intensity;
            for (int i = 0; i < 6; i++)
            {
                frame.X = rand.Next(FramesX) * frameWidth;
                var off2 = new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4));
                Main.spriteBatch.Draw(texture, projectile.position + offset + off2, frame, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            }

            if (AQConfigClient.Instance.EffectQuality < 1f)
            {
                return false;
            }

            texture = LegacyTextureCache.Lights[LightTex.Spotlight66x66];
            frame = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = frame.Size() / 2f;
            color = new Color(90 - a, 90 - a, 165 - a, 0);
            color *= intensity;
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.15f, projectile.rotation, origin, scale * 2f * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.06f, projectile.rotation, origin, scale * 4f * 1.25f, SpriteEffects.None, 0f);
            return false;
        }
    }
}