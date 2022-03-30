using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public sealed class RayExplosion : ModProjectile
    {
        public override string Texture => Aequus.TextureNone;

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft + 2;
            Projectile.penetrate = -1;
        }
    }
}