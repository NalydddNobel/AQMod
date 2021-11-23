using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class TemperatureBombCold : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            int target = (int)projectile.ai[0];
            if ((int)projectile.ai[1] > 60)
            {
                projectile.velocity.X *= 0.975f;
            }
            projectile.velocity.Y += 0.4f;
            if (projectile.velocity.Y > 1f && (Main.player[target].dead || !Main.player[target].active || Main.player[target].position.Y < projectile.position.Y))
            {
                projectile.tileCollide = true;
            }
        }

        public override void Kill(int timeLeft)
        {
        }
    }
}