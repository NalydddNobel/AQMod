using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class Bloodthirst : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().bloodthirst = true;
        }
    }
}