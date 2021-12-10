using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public class Firecracker : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.aiStyle = -1;
            projectile.noEnchantments = true;
            projectile.timeLeft = 240;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (projectile.timeLeft <= 3)
            {
                if (projectile.timeLeft == 2)
                {
                    projectile.tileCollide = false;
                    projectile.velocity = -Vector2.Normalize(projectile.velocity);
                    projectile.alpha = 255;
                    projectile.hide = true;
                    projectile.position += new Vector2(projectile.width / 2f, projectile.height / 2f);
                    projectile.width = 120;
                    projectile.height = 120;
                    projectile.position -= new Vector2(projectile.width / 2f, projectile.height / 2f);
                }
                return;
            }
            projectile.rotation += 0.25f * projectile.direction;
            projectile.ai[0] += 1f;
            var center = projectile.Center;
            if (projectile.ai[0] >= 3f)
            {
                projectile.alpha -= 40;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
            if (projectile.ai[0] >= 15f)
            {
                projectile.velocity.Y += 0.15f;
                if (projectile.velocity.Y > 24f)
                {
                    projectile.velocity.Y = 24f;
                }
                projectile.velocity.X *= 0.992f;
            }
            if (projectile.alpha == 0)
            {
                int d = Dust.NewDust(center, 4, 4, DustID.Fire);
                Main.dust[d].scale = 1.5f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Main.dust[d].velocity * 0.25f;
                Main.dust[d].velocity = Main.dust[d].velocity.RotatedBy(-MathHelper.Pi / 2f * projectile.direction);
            }
            projectile.spriteDirection = projectile.direction;
            if (projectile.wet && projectile.timeLeft > 3)
            {
                projectile.timeLeft = 3;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.timeLeft > 3)
                projectile.timeLeft = 3;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.timeLeft > 3)
                projectile.timeLeft = 3;
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            SoundID.Item14.PlaySound(projectile.Center, 0.6f, 0.5f);
            var center = projectile.Center;
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire);
                Main.dust[d].velocity = (Main.dust[d].position - center) * 0.1f;
            }
            if (Main.myPlayer == projectile.owner)
            {
                var velo = projectile.velocity * 9f;
                center += projectile.velocity * 4f;
                for (int i = 0; i < 13; i++)
                {
                    int p = Projectile.NewProjectile(center, velo.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)), ProjectileID.MolotovFire + Main.rand.Next(3), projectile.damage, projectile.knockBack, projectile.owner);
                    Main.projectile[p].extraUpdates = 1;
                    Main.projectile[p].timeLeft /= 2;
                }
            }
        }
    }
}
