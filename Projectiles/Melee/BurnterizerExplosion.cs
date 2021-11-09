using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class BurnterizerExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.timeLeft = 2;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.hide = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (projectile.ai[0] > 0)
            {
                projectile.active = false;
            }
            projectile.ai[0]++;
        }
    }
}