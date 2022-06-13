using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class SentryBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.maxTurrets++;
        }
    }
}