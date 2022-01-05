using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee.Yoyo
{
    public abstract class YoyoType : ModProjectile
    {
        protected abstract float TopSpeed { get; }
        protected abstract float MaxRange { get; }
        protected abstract float LifeTimeMultiplier { get; }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = LifeTimeMultiplier;
            ProjectileID.Sets.YoyosMaximumRange[projectile.type] = MaxRange;
            ProjectileID.Sets.YoyosTopSpeed[projectile.type] = TopSpeed;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 99;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
        }
    }
}