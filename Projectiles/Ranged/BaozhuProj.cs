using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class BaozhuProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.noEnchantments = true;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 3;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 60);
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 3)
            {
                if (Projectile.timeLeft == 2)
                {
                    Projectile.tileCollide = false;
                    Projectile.velocity = -Vector2.Normalize(Projectile.velocity);
                    Projectile.alpha = 255;
                    Projectile.hide = true;
                    Projectile.position += new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                    Projectile.width = 120;
                    Projectile.height = 120;
                    Projectile.position -= new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                }
                return;
            }
            Projectile.rotation += 0.1f * Projectile.direction;
            Projectile.ai[0] += 1f;
            var center = Projectile.Center;
            if (Projectile.ai[0] >= 3f)
            {
                Projectile.alpha -= 40;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.ai[0] >= 15f)
            {
                Projectile.velocity.Y += 0.1f;
                if (Projectile.velocity.Y > 24f)
                {
                    Projectile.velocity.Y = 24f;
                }
                Projectile.velocity.X *= 0.997f;
            }
            if (Projectile.alpha == 0)
            {
                int d = Dust.NewDust(center, 4, 4, DustID.Torch);
                Main.dust[d].scale = 1.5f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Main.dust[d].velocity * 0.25f;
                Main.dust[d].velocity = Main.dust[d].velocity.RotatedBy(-MathHelper.Pi / 2f * Projectile.direction);
                d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(175, 50, 10, 10));
                Main.dust[d].scale = 1.75f * Main.rand.NextFloat(0.5f, 1.1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Projectile.velocity * 0.2f * Main.rand.NextFloat(0.9f, 1.1f);

                if (Main.rand.NextBool(8))
                {
                    d = Dust.NewDust(center, 4, 4, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(150, 10, 2, 20));
                    Main.dust[d].scale = 1.25f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(0.9f, 1.75f);
                }
            }
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.wet && Projectile.timeLeft > 3)
            {
                Projectile.timeLeft = 3;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.timeLeft > 3)
                Projectile.timeLeft = 3;
            target.AddBuff(BuffID.OnFire3, 480);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 3)
                Projectile.timeLeft = 3;
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item14.WithPitch(0.1f), Projectile.Center);
            }
            var center = Projectile.Center;
            float radius = Projectile.Size.Length() / 2f * 0.55f;
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 0, default, Main.rand.NextFloat(1.5f, 2.75f));
                Main.dust[d].position = center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(radius);
                Main.dust[d].velocity = (Main.dust[d].position - center) * 0.125f;
                Main.dust[d].velocity.Y -= 2f;
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < 60; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default, Main.rand.NextFloat(1.5f, 2.75f));
                Main.dust[d].position = center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(radius);
                Main.dust[d].velocity = (Main.dust[d].position - center) * 0.275f;
                Main.dust[d].noGravity = true;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                var velo = Projectile.velocity * 9f;
                center += Projectile.velocity * 4f;
                for (int i = 0; i < 13; i++)
                {
                    var shootTo = velo.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                    var shootLocation = center + Vector2.Normalize(shootTo) * 15f;
                    if (Collision.SolidCollision(shootLocation, 16, 16))
                    {
                        shootLocation = center;
                        shootTo = -shootTo;
                    }
                    int p = Projectile.NewProjectile(Projectile.GetSource_Death(), shootLocation, shootTo, ProjectileID.MolotovFire + Main.rand.Next(3), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[p].extraUpdates += 2;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame();
            var origin = new Vector2(frame.Width / 2f, 5f);
            var color = Projectile.GetAlpha(lightColor);
            var offset = Projectile.Size / 2f;
            int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f - 1f / trailLength * i;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, new Color(188, 128, 10, 10) * progress, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            foreach (var v in AequusHelpers.CircularVector(4, Projectile.rotation))
            {
                Main.spriteBatch.Draw(texture, Projectile.position + v * 2f + offset - Main.screenPosition, frame, new Color(128, 128, 10, 10), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
