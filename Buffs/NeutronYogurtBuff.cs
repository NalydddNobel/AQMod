using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class NeutronYogurtBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            AQBuff.WellFedMinor(player);
        }
    }
}