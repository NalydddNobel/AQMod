using Terraria;

namespace Aequus.Projectiles.Misc
{
    public class BellowsProj : PumpinatorProj
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 20;
            Projectile.height = 20;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void DoDust()
        {
        }
    }
}