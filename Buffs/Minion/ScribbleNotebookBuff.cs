using Aequus.Common.Buffs;
using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion {
    public class ScribbleNotebookBuff : BaseMinionBuff
    {
        protected override int MinionProj => ModContent.ProjectileType<ScribbleNotebookMinion>();
    }
}