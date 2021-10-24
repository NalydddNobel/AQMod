using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class Opposing : ModBuff
    {
        public override void SetDefaults()
        {

        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AQPlayer>().opposingForce = true;
        }
    }
}