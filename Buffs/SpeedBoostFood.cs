using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs
{
    public class SpeedBoostFood : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.runSpeedBoost += 0.1f;
            aQPlayer.wingSpeedBoost += 0.1f;
            AQBuff.WellFedMinor(player);
        }
    }
}