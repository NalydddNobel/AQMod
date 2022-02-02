using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class Shade : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().shade = true;
            player.aggro -= 400;
        }
    }
}