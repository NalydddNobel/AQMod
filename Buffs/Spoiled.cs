using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class Spoiled : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            if (aQPlayer.lootIterations < 1)
            {
                aQPlayer.lootIterations = 1;
            }
        }
    }
}