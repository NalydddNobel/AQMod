using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion {
    public class PiranhaPlantBuff : BaseMinionBuff
    {
        protected override int MinionProj => ModContent.ProjectileType<PiranhaPlantMinion>();
    }
}