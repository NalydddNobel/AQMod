using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class DebuffSpread : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().spreadDebuffs = true;
        }
    }
}