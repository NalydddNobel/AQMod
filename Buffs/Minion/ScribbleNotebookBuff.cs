using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class ScribbleNotebookBuff : MinionBuffBase
    {
        protected override int MinionProj => ModContent.ProjectileType<ScribbleNotebookMinion>();
    }
}