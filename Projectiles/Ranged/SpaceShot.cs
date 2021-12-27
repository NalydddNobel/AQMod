using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public class SpaceShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = -1;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(250, 250, 250, 0);

        public override void AI()
        {
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = ProjectileID.Bullet;
            }
            if (Projectile.ai[1] == 0f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 14f)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 55);
            }
            if (Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.Item9);
                var center = Projectile.Center;
                var speed = Projectile.velocity.Length() * 0.9f;
                int type = (int)Projectile.ai[0];
                const float rotation = MathHelper.TwoPi / 5;
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), center, new Vector2(speed, 0f).RotatedBy(Projectile.rotation + (rotation * i)), type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }
    }
}
