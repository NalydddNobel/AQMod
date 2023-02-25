using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public class MeteorRubble : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.timeLeft = 10;
            Projectile.hide = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }
    }
}