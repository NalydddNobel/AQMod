using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class GebulbaStaff : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.timeLeft = 300;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.penetrate = 5;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.25f;
        }

        private void CollisionEffects(Vector2 velocity)
        {
            Vector2 spawnPos = projectile.position + velocity;
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Boing").WithPitchVariance(projectile.velocity.Length() / 16f), projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(spawnPos, projectile.width, projectile.height, DustID.t_Slime);
                Main.dust[d].color = AQNPC.BlueSlimeColor;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool doEffects = false;
            if (oldVelocity.X != projectile.velocity.X && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                projectile.position.X += projectile.velocity.X * 0.9f;
                projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (oldVelocity.Y != projectile.velocity.Y && oldVelocity.Y.Abs() > 2f)
            {
                doEffects = true;
                projectile.position.Y += projectile.velocity.Y * 0.9f;
                projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }
            if (doEffects)
            {
                CollisionEffects(projectile.velocity);
            }
            else
            {
                projectile.velocity *= 0.95f;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item85.WithPitchVariance(2f), projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.t_Slime);
                Main.dust[d].color = AQNPC.BlueSlimeColor;
            }
        }
    }
}