using AQMod.Assets;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    internal class WhackAZombie : ModProjectile
    {
        public override string Texture => "AQMod/" + LegacyTextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.timeLeft = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ownerHitCheck = true;
        }
    }
}