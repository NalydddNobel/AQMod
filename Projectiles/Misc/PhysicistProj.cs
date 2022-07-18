using Aequus.Projectiles.Magic;

namespace Aequus.Projectiles.Misc
{
    public sealed class PhysicistProj : UmystickBullet
    {
        public override string Texture => AequusHelpers.GetPath<UmystickBullet>();

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
        }
    }
}