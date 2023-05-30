using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion {
    public class MindfungusBuff : BaseMinionBuff
    {
        protected override int MinionProj => ModContent.ProjectileType<MindfungusMinion>();
    }
}