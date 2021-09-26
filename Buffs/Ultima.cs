using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class Ultima : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense += 20;
            player.lifeRegen += 6;
        }
    }
}