using Aequus.Buffs.Debuffs;
using Terraria;

namespace Aequus.Projectiles.Summon.Necro
{
    public class LocustLarge : LocustSmall
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            LocustDebuff.AddStack(target, 120, 2);
        }
    }
}