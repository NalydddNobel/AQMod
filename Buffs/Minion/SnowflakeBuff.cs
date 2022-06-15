using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class SnowflakeBuff : MinionBuffBase
    {
        protected override int MinionProj => ModContent.ProjectileType<SnowflakeMinion>();
    }
}