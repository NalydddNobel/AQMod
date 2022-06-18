using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class CorruptPlantBuff : UniMinionBuffBase
    {
        protected override int minionProj => ModContent.ProjectileType<CorruptPlantMinion>();
        protected override int CounterProj => ModContent.ProjectileType<CorruptPlantCounter>();
    }
}