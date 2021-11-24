using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public class MassiveIceSpike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.timeLeft = 600;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
        }
    }
}