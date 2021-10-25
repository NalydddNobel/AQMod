using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class PowPunchExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.timeLeft = 12;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] > 0)
                projectile.friendly = false;
            projectile.ai[0]++;
        }
    }
}