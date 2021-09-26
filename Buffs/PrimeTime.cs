using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class PrimeTime : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.endurance += 0.5f;
            player.allDamage += 0.5f;
        }
    }
}