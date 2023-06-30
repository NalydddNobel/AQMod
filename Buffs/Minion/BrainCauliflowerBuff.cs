using Aequus.Common.Buffs;
using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion {
    public class BrainCauliflowerBuff : BaseMinionBuff
    {
        protected override int MinionProj => ModContent.ProjectileType<BrainCauliflowerMinion>();
    }
}