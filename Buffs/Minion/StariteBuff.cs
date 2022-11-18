using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class StariteBuff : BaseMinionBuff
    {
        protected override int MinionProj => ModContent.ProjectileType<StariteMinion>();
    }
}
