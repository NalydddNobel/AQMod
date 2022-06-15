using Aequus.Projectiles.Summon;
using Terraria.ModLoader;

namespace Aequus.Buffs.Minion
{
    public class RedSpriteMinionBuff : MinionBuffBase
    {
        protected override int MinionProj => ModContent.ProjectileType<RedSpriteMinion>();
    }
}