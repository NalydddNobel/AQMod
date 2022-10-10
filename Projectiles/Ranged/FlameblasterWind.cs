using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class FlameblasterWind : PumpinatorProj
    {
        public override bool OnlyPushHostilePlayers => true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 90;
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.extraUpdates = 8;
        }
    }
}