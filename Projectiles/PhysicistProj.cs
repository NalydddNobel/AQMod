using AQMod.Projectiles.Magic;

namespace AQMod.Projectiles
{
    public sealed class PhysicistProj : UmystickMoon
    {
        public override string Texture => AQUtils.GetPath<UmystickMoon>();

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
        }
    }
}