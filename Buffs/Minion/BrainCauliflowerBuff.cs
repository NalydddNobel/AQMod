using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class BrainCauliflowerBuff : MinionBuffBase
    {
        protected override int MinionProj => ModContent.ProjectileType<BrainCauliflowerMinion>();
    }
}