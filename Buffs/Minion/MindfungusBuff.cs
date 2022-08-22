using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class MindfungusBuff : MinionBuffBase
    {
        protected override int MinionProj => ModContent.ProjectileType<MindfungusProj>();
    }
}