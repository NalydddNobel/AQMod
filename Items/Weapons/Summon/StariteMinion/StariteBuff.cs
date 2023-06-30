using Aequus.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.StariteMinion {
    public class StariteBuff : BaseMinionBuff {
        protected override int MinionProj => ModContent.ProjectileType<StariteMinion>();
    }
}
