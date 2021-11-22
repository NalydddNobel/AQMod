using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public class SpaceShot : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.aiStyle = -1;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 250, 250, 0);

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[0] = ProjectileID.Bullet;
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.rotation = projectile.velocity.ToRotation();
            }
            projectile.ai[1]++;
            if (projectile.ai[1] >= 14f)
            {
                projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item9);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 55);
            }
            if (Main.myPlayer == projectile.owner)
            {
                var center = projectile.Center;
                var speed = projectile.velocity.Length() * 0.9f;
                int type = (int)projectile.ai[0];
                const float rotation = MathHelper.TwoPi / 5;
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(center, new Vector2(speed, 0f).RotatedBy(projectile.rotation + (rotation * i)), type, projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
        }
    }
}
