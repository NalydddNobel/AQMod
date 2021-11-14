using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class Bubbler : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.magic = true;
            projectile.friendly = true;
            projectile.aiStyle = 95;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.99f;
        }
    }
}