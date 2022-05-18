using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs
{
    public class FrostBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AequusPlayer>().buffResistHeat = true;
        }
    }
}